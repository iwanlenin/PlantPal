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
}
