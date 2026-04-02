namespace PlantPal.Core.Interfaces;

/// <summary>
/// Abstracts HTTP operations so they can be mocked in unit tests.
/// </summary>
public interface IHttpClientWrapper
{
    /// <summary>Fetches the response body as a string from the specified URL.</summary>
    /// <param name="url">The absolute URL to request.</param>
    Task<string> GetStringAsync(string url);

    /// <summary>Downloads the response body as a byte array from the specified URL.</summary>
    /// <param name="url">The absolute URL to request.</param>
    Task<byte[]> GetBytesAsync(string url);
}
