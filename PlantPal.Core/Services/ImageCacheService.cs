using System.Text.Json;
using PlantPal.Core.Interfaces;

namespace PlantPal.Core.Services;

/// <summary>
/// Resolves plant species images with download caching and offline fallback.
/// Thumbnails are always served from bundled assets. Detail images are downloaded
/// from Wikipedia on first request and cached to disk. Never throws on missing image.
/// </summary>
public class ImageCacheService : IImageCacheService
{
    private const string PlaceholderPath = "plants/placeholder_thumb.png";
    private const string WikiApiBase = "https://en.wikipedia.org/api/rest_v1/page/summary/";

    private readonly IConnectivityService connectivityService;
    private readonly IHttpClientWrapper httpClient;
    private readonly IPlantSpeciesService speciesService;
    private readonly string cacheBasePath;

    /// <summary>
    /// Initialises a new instance of the <see cref="ImageCacheService"/> class.
    /// </summary>
    /// <param name="connectivityService">Connectivity checker for offline detection.</param>
    /// <param name="httpClient">HTTP abstraction for downloading images.</param>
    /// <param name="speciesService">Species data provider for Wikipedia slugs and asset paths.</param>
    /// <param name="cacheBasePath">Absolute path to the local cache directory for detail images.</param>
    public ImageCacheService(
        IConnectivityService connectivityService,
        IHttpClientWrapper httpClient,
        IPlantSpeciesService speciesService,
        string cacheBasePath)
    {
        this.connectivityService = connectivityService;
        this.httpClient = httpClient;
        this.speciesService = speciesService;
        this.cacheBasePath = cacheBasePath;
    }

    /// <inheritdoc />
    public Task<string> GetThumbnailPathAsync(string speciesKey)
    {
        ArgumentNullException.ThrowIfNull(speciesKey);

        var species = this.speciesService.GetAll().FirstOrDefault(s => s.Id == speciesKey);
        return Task.FromResult(species?.ThumbnailAssetPath ?? PlaceholderPath);
    }

    /// <inheritdoc />
    public async Task<string> GetDetailImageAsync(string speciesKey)
    {
        ArgumentNullException.ThrowIfNull(speciesKey);

        var species = this.speciesService.GetAll().FirstOrDefault(s => s.Id == speciesKey);
        if (species is null)
        {
            return PlaceholderPath;
        }

        var cachedPath = Path.Combine(this.cacheBasePath, $"{speciesKey}_detail.jpg");

        // Cache hit — return immediately.
        if (File.Exists(cachedPath))
        {
            return cachedPath;
        }

        // Offline — fall back to bundled thumbnail.
        if (!this.connectivityService.IsConnected)
        {
            return species.ThumbnailAssetPath;
        }

        try
        {
            // Fetch Wikipedia summary to get the image URL.
            var json = await this.httpClient.GetStringAsync(WikiApiBase + species.WikipediaSlug);
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("originalimage", out var imageObj) ||
                !imageObj.TryGetProperty("source", out var sourceEl))
            {
                return species.ThumbnailAssetPath;
            }

            var imageUrl = sourceEl.GetString();
            if (string.IsNullOrEmpty(imageUrl))
            {
                return species.ThumbnailAssetPath;
            }

            // Download and cache.
            var imageBytes = await this.httpClient.GetBytesAsync(imageUrl);
            Directory.CreateDirectory(this.cacheBasePath);
            await File.WriteAllBytesAsync(cachedPath, imageBytes);

            return cachedPath;
        }
        catch (Exception)
        {
            // Never crash — fall back to thumbnail on any failure.
            return species.ThumbnailAssetPath;
        }
    }
}
