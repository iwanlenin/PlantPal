using PlantPal.Core.Models;

namespace PlantPal.Core.Interfaces;

/// <summary>
/// Provides AI plant care advice via the Anthropic Claude API.
/// Always returns a user-facing string — never throws.
/// </summary>
public interface IPlantAdvisorService
{
    /// <summary>Gets a value indicating whether an API key has been stored.</summary>
    bool IsConfigured { get; }

    /// <summary>
    /// Loads the API key from secure storage into memory. Call once during page initialisation.
    /// </summary>
    Task InitialiseAsync();

    /// <summary>Returns the stored API key, or null if not set.</summary>
    Task<string?> GetApiKeyAsync();

    /// <summary>Stores the API key in secure storage.</summary>
    Task SetApiKeyAsync(string key);

    /// <summary>
    /// Sends a question about a plant to Claude and returns the response.
    /// <paramref name="context"/> is the recent message history to include (typically last N messages).
    /// Returns a user-friendly error string on any failure — never throws.
    /// </summary>
    Task<string> AskAboutPlantAsync(string species, string question, IList<AdvisorMessage> context);
}
