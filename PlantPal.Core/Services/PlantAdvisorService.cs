using System.Net;
using System.Text;
using System.Text.Json;
using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;

namespace PlantPal.Core.Services;

/// <summary>
/// Sends plant care questions to the Anthropic Claude API and returns text responses.
/// Always returns a user-facing string — never throws.
/// Call <see cref="InitialiseAsync"/> once after construction to populate <see cref="IsConfigured"/>.
/// </summary>
public class PlantAdvisorService : IPlantAdvisorService
{
    private const string ApiKeyStorageKey = "anthropic_api_key";
    private const string ApiUrl = "https://api.anthropic.com/v1/messages";
    private const string Model = "claude-sonnet-4-20250514";
    private const string AnthropicVersion = "2023-06-01";

    private readonly IHttpClientWrapper http;
    private readonly ISecureStorageService secureStorage;

    private string? cachedApiKey;

    /// <inheritdoc />
    public bool IsConfigured => !string.IsNullOrWhiteSpace(this.cachedApiKey);

    /// <summary>
    /// Initialises a new instance of <see cref="PlantAdvisorService"/>.
    /// </summary>
    /// <param name="http">HTTP client wrapper for API calls.</param>
    /// <param name="secureStorage">Secure storage for the API key.</param>
    public PlantAdvisorService(IHttpClientWrapper http, ISecureStorageService secureStorage)
    {
        this.http = http;
        this.secureStorage = secureStorage;
    }

    /// <summary>
    /// Loads the API key from secure storage into memory.
    /// Call this once during initialisation so <see cref="IsConfigured"/> is accurate.
    /// </summary>
    public async Task InitialiseAsync()
    {
        this.cachedApiKey = await this.secureStorage.GetAsync(ApiKeyStorageKey);
    }

    /// <inheritdoc />
    public async Task<string?> GetApiKeyAsync() =>
        await this.secureStorage.GetAsync(ApiKeyStorageKey);

    /// <inheritdoc />
    public async Task SetApiKeyAsync(string key)
    {
        await this.secureStorage.SetAsync(ApiKeyStorageKey, key);
        this.cachedApiKey = key;
    }

    /// <inheritdoc />
    public async Task<string> AskAboutPlantAsync(
        string species,
        string question,
        IList<AdvisorMessage> context)
    {
        if (!this.IsConfigured)
        {
            return "Please add your Anthropic API key in Settings";
        }

        try
        {
            var body = this.BuildRequestBody(species, question, context);
            var json = await this.http.PostStringAsync(ApiUrl, body, this.cachedApiKey!);
            return this.ParseResponse(json);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            return "API key invalid — check Settings";
        }
        catch (TaskCanceledException)
        {
            return "Couldn't reach Claude — check your connection";
        }
        catch
        {
            return "Something went wrong — please try again";
        }
    }

    /// <inheritdoc />
    public async Task<string> DiagnosePlantAsync(
        string species,
        string question,
        byte[] imageBytes,
        string mimeType,
        IList<AdvisorMessage> context)
    {
        if (!this.IsConfigured)
        {
            return "Please add your Anthropic API key in Settings";
        }

        try
        {
            var body = this.BuildVisionRequestBody(species, question, imageBytes, mimeType, context);
            var json = await this.http.PostStringAsync(ApiUrl, body, this.cachedApiKey!);
            return this.ParseResponse(json);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            return "API key invalid — check Settings";
        }
        catch (TaskCanceledException)
        {
            return "Couldn't reach Claude — check your connection";
        }
        catch
        {
            return "Something went wrong — please try again";
        }
    }

    /// <summary>Builds the Anthropic Messages API JSON request body.</summary>
    private string BuildRequestBody(string species, string question, IList<AdvisorMessage> context)
    {
        var messages = new List<object>();

        foreach (var msg in context)
        {
            messages.Add(new { role = msg.Role, content = msg.Content });
        }

        messages.Add(new { role = "user", content = question });

        var payload = new
        {
            model = Model,
            max_tokens = 1024,
            system = $"You are a plant care expert. The user is asking about their {species}. Give practical, concise advice.",
            messages
        };

        return JsonSerializer.Serialize(payload);
    }

    /// <summary>
    /// Builds the Anthropic Messages API JSON body for a vision request.
    /// The final user message contains an image block followed by a text block.
    /// </summary>
    private string BuildVisionRequestBody(
        string species,
        string question,
        byte[] imageBytes,
        string mimeType,
        IList<AdvisorMessage> context)
    {
        var messages = new List<object>();

        foreach (var msg in context)
        {
            messages.Add(new { role = msg.Role, content = msg.Content });
        }

        // The vision message has array content: image block + text block.
        messages.Add(new
        {
            role = "user",
            content = new object[]
            {
                new
                {
                    type = "image",
                    source = new
                    {
                        type = "base64",
                        media_type = mimeType,
                        data = Convert.ToBase64String(imageBytes),
                    },
                },
                new
                {
                    type = "text",
                    text = string.IsNullOrWhiteSpace(question)
                        ? "Please give a full health diagnosis of my plant."
                        : question,
                },
            },
        });

        var payload = new
        {
            model = Model,
            max_tokens = 1024,
            system = $"You are a plant health expert. The user is sharing a photo of their {species}. " +
                     "Provide a comprehensive diagnosis: identify any visible issues such as pests, disease, " +
                     "overwatering, underwatering, or nutrient deficiencies. Give specific actionable care recommendations.",
            messages,
        };

        return JsonSerializer.Serialize(payload);
    }

    /// <summary>Extracts the first text content block from an Anthropic API response.</summary>
    private string ParseResponse(string json)
    {
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement
            .GetProperty("content")[0]
            .GetProperty("text")
            .GetString() ?? string.Empty;
    }
}
