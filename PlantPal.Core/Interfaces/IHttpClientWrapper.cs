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

    /// <summary>
    /// POSTs a JSON body to the specified URL with the given bearer token and returns the response body as a string.
    /// Throws <see cref="HttpRequestException"/> on non-success status codes.
    /// </summary>
    /// <param name="url">The absolute URL to POST to.</param>
    /// <param name="jsonBody">The JSON request body.</param>
    /// <param name="bearerToken">The Authorization bearer token.</param>
    Task<string> PostStringAsync(string url, string jsonBody, string bearerToken);
}
