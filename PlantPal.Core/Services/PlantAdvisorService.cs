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

    /// <summary>
    /// Builds the Anthropic Messages API JSON request body for a text-only conversation turn.
    /// </summary>
    /// <remarks>
    /// Includes the conversation context (previous user and assistant messages) to enable
    /// multi-turn conversations. The context is a sliding window controlled by the ViewModel —
    /// only the last N messages are passed in, so this method does not need to truncate further.
    /// The system prompt positions Claude as a species-specific plant care expert.
    /// max_tokens of 1024 is a hard ceiling; responses exceeding it are truncated mid-sentence.
    /// </remarks>
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
    /// Builds the Anthropic Messages API JSON body for a vision (image analysis) request.
    /// </summary>
    /// <remarks>
    /// The Anthropic vision API requires the user message content to be an array of typed blocks
    /// rather than a plain string. This method produces: image block → text block, which is the
    /// required ordering. The image is base64-encoded and sent inline in the JSON body — Supabase
    /// has no file storage role here; the bytes travel directly to the Anthropic API.
    /// If <paramref name="question"/> is empty, a default diagnosis prompt is substituted so that
    /// sending a photo with no typed text still produces a useful response.
    /// The system prompt is specialist-framed to keep Claude focused on health diagnosis
    /// rather than general plant trivia.
    /// </remarks>
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

    /// <summary>
    /// Extracts the first text content block from an Anthropic API response.
    /// </summary>
    /// <remarks>
    /// The Anthropic API always returns a <c>content</c> array; Claude may produce multiple
    /// blocks (e.g. text + tool use). This method reads only index [0], which is always the
    /// primary text response for models without tool use. If the API changes its response
    /// structure, this will throw a <see cref="System.Text.Json.JsonException"/> which the
    /// callers (<see cref="AskAboutPlantAsync"/> / <see cref="DiagnosePlantAsync"/>) catch
    /// and convert to a user-friendly error string.
    /// </remarks>
    private string ParseResponse(string json)
    {
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement
            .GetProperty("content")[0]
            .GetProperty("text")
            .GetString() ?? string.Empty;
    }
}
