using System.Net.Http.Headers;
using System.Text;
using PlantPal.Core.Interfaces;

namespace PlantPal.Services;

/// <summary>
/// Concrete <see cref="IHttpClientWrapper"/> implementation using a shared <see cref="HttpClient"/> instance.
/// </summary>
public class HttpClientWrapper : IHttpClientWrapper
{
    private static readonly HttpClient Client = new();

    /// <inheritdoc />
    public async Task<string> GetStringAsync(string url) =>
        await Client.GetStringAsync(url);

    /// <inheritdoc />
    public async Task<byte[]> GetBytesAsync(string url) =>
        await Client.GetByteArrayAsync(url);

    /// <inheritdoc />
    public async Task<string> PostStringAsync(string url, string jsonBody, string bearerToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
