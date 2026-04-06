using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;

namespace PlantPal.Services;

/// <summary>
/// Communicates with the Supabase backend via its PostgREST and Auth REST APIs.
/// Stores the session JWT in <see cref="ISecureStorageService"/> so it survives app restarts.
/// </summary>
public class SupabaseClientService : ISupabaseClient
{
    private const string ProjectUrl = "https://ldpuwqtergmajiedkyqo.supabase.co";
    private const string AnonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImxkcHV3cXRlcmdtYWppZWRreXFvIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NzUyMjU4NzIsImV4cCI6MjA5MDgwMTg3Mn0.UCpWlVa5Ra2FkKH9ePrWuBb2pPwQ9xIsJA2V7rMPMJw";
    private const string JwtStorageKey = "supabase_jwt";
    private const string UserIdStorageKey = "supabase_user_id";
    private const string UserEmailStorageKey = "supabase_user_email";

    private static readonly HttpClient HttpClient = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly ISecureStorageService secureStorage;

    private string? jwtToken;

    /// <summary>Initialises the service and begins loading a persisted session.</summary>
    public SupabaseClientService(ISecureStorageService secureStorage)
    {
        this.secureStorage = secureStorage;
    }

    /// <inheritdoc />
    public bool IsSignedIn => !string.IsNullOrEmpty(this.jwtToken);

    /// <inheritdoc />
    public string? CurrentUserId { get; private set; }

    /// <inheritdoc />
    public string? CurrentUserEmail { get; private set; }

    /// <summary>
    /// Loads a previously persisted session from secure storage.
    /// Call this once at app startup before using any other methods.
    /// </summary>
    public async Task LoadSessionAsync()
    {
        this.jwtToken = await this.secureStorage.GetAsync(JwtStorageKey);
        this.CurrentUserId = await this.secureStorage.GetAsync(UserIdStorageKey);
        this.CurrentUserEmail = await this.secureStorage.GetAsync(UserEmailStorageKey);
    }

    /// <inheritdoc />
    public async Task<bool> SignInWithEmailAsync(string email, string password)
    {
        return await this.AuthenticateAsync(email, password, "password");
    }

    /// <inheritdoc />
    public async Task<bool> SignUpWithEmailAsync(string email, string password)
    {
        return await this.AuthenticateAsync(email, password, "signup");
    }

    /// <inheritdoc />
    public async Task SignOutAsync()
    {
        this.jwtToken = null;
        this.CurrentUserId = null;
        this.CurrentUserEmail = null;
        await this.secureStorage.RemoveAsync(JwtStorageKey);
        await this.secureStorage.RemoveAsync(UserIdStorageKey);
        await this.secureStorage.RemoveAsync(UserEmailStorageKey);
    }

    /// <inheritdoc />
    public async Task UpsertPlantsAsync(IList<PlantSyncRecord> plants)
    {
        var url = $"{ProjectUrl}/rest/v1/plants?on_conflict=sync_id";
        await this.PostJsonAsync(url, plants);
    }

    /// <inheritdoc />
    public async Task UpsertWateringLogsAsync(IList<WateringLogSyncRecord> logs)
    {
        var url = $"{ProjectUrl}/rest/v1/watering_logs?on_conflict=sync_id";
        await this.PostJsonAsync(url, logs);
    }

    /// <inheritdoc />
    public async Task<IList<PlantSyncRecord>> FetchPlantsAsync()
    {
        var url = $"{ProjectUrl}/rest/v1/plants?select=*";
        var json = await this.GetJsonAsync(url);
        return JsonSerializer.Deserialize<List<PlantSyncRecord>>(json, JsonOptions)
               ?? new List<PlantSyncRecord>();
    }

    /// <inheritdoc />
    public async Task<IList<WateringLogSyncRecord>> FetchWateringLogsAsync()
    {
        var url = $"{ProjectUrl}/rest/v1/watering_logs?select=*";
        var json = await this.GetJsonAsync(url);
        return JsonSerializer.Deserialize<List<WateringLogSyncRecord>>(json, JsonOptions)
               ?? new List<WateringLogSyncRecord>();
    }

    // ------------------------------------------------------------------
    // Private helpers
    // ------------------------------------------------------------------

    /// <summary>
    /// Handles both sign-in and account creation via the Supabase Auth REST API.
    /// </summary>
    /// <param name="grantType">"password" for sign-in via token endpoint; "signup" for new account.</param>
    /// <remarks>
    /// The two flows differ in endpoint and response shape:
    ///
    /// Sign-in (grantType = "password"):
    ///   POST /auth/v1/token?grant_type=password → { access_token, user: { id } }
    ///   The JWT is stored in <see cref="ISecureStorageService"/> and used for all
    ///   subsequent REST calls. This is the normal session flow.
    ///
    /// Sign-up (grantType = "signup"):
    ///   POST /auth/v1/signup → { id, email }
    ///   No token is issued; Supabase sends a confirmation email.
    ///   The user must confirm their email and then sign in separately to obtain a token.
    ///   If email confirmation is disabled in the Supabase project settings, the user
    ///   will be signed in automatically on the next sign-in attempt.
    ///
    /// Returns false on any non-2xx response without logging details to avoid leaking
    /// credentials into logs.
    /// </remarks>
    private async Task<bool> AuthenticateAsync(string email, string password, string grantType)
    {
        string url;
        object body;

        if (grantType == "signup")
        {
            url = $"{ProjectUrl}/auth/v1/signup";
            body = new { email, password };
        }
        else
        {
            url = $"{ProjectUrl}/auth/v1/token?grant_type=password";
            body = new { email, password };
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("apikey", AnonKey);
        request.Content = new StringContent(
            JsonSerializer.Serialize(body, JsonOptions),
            Encoding.UTF8,
            "application/json");

        var response = await HttpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // sign-up returns user object; sign-in returns access_token + user
        if (grantType == "signup")
        {
            // After sign-up, user must confirm email before signing in (unless confirmations are disabled)
            if (root.TryGetProperty("id", out var idEl))
            {
                this.CurrentUserId = idEl.GetString();
                this.CurrentUserEmail = email;
                return true;
            }

            return false;
        }

        if (!root.TryGetProperty("access_token", out var tokenEl))
        {
            return false;
        }

        this.jwtToken = tokenEl.GetString();
        this.CurrentUserEmail = email;

        if (root.TryGetProperty("user", out var userEl)
            && userEl.TryGetProperty("id", out var userIdEl))
        {
            this.CurrentUserId = userIdEl.GetString();
        }

        await this.secureStorage.SetAsync(JwtStorageKey, this.jwtToken ?? string.Empty);
        await this.secureStorage.SetAsync(UserEmailStorageKey, email);
        await this.secureStorage.SetAsync(UserIdStorageKey, this.CurrentUserId ?? string.Empty);

        return true;
    }

    /// <summary>
    /// Sends an authenticated POST request to a Supabase PostgREST endpoint with a JSON body.
    /// </summary>
    /// <remarks>
    /// Includes both required Supabase auth headers:
    ///   <c>apikey</c>: the anonymous key that identifies the project (safe to embed in the app).
    ///   <c>Authorization: Bearer {jwt}</c>: the signed-in user's session token.
    /// The <c>Prefer: resolution=merge-duplicates</c> header tells PostgREST to perform an
    /// upsert (insert or update) on conflict with the column specified in the URL's
    /// <c>on_conflict</c> query parameter (SyncId). Without this header the request would
    /// fail with a unique constraint violation if the record already exists.
    /// Throws <see cref="HttpRequestException"/> on non-2xx responses; callers must handle it.
    /// </remarks>
    private async Task PostJsonAsync<T>(string url, T body)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("apikey", AnonKey);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.jwtToken);
        request.Headers.Add("Prefer", "resolution=merge-duplicates");
        request.Content = new StringContent(
            JsonSerializer.Serialize(body, JsonOptions),
            Encoding.UTF8,
            "application/json");

        var response = await HttpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Sends an authenticated GET request to a Supabase PostgREST endpoint and returns the JSON body.
    /// </summary>
    /// <remarks>
    /// Row-level security (RLS) on the Supabase tables filters the result set automatically:
    /// the server inspects the JWT's <c>sub</c> claim and returns only rows where
    /// <c>user_id = auth.uid()</c>. No client-side filtering is needed.
    /// Throws <see cref="HttpRequestException"/> on non-2xx responses; callers must handle it.
    /// </remarks>
    private async Task<string> GetJsonAsync(string url)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("apikey", AnonKey);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.jwtToken);

        var response = await HttpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
