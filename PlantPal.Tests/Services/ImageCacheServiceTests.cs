using NSubstitute;
using NSubstitute.ExceptionExtensions;
using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;
using PlantPal.Core.Services;

namespace PlantPal.Tests.Services;

/// <summary>
/// Unit tests for <see cref="ImageCacheService"/> with mocked HTTP, connectivity, and species data.
/// Each test uses an isolated temp directory for cache file operations.
/// </summary>
public class ImageCacheServiceTests : IDisposable
{
    private readonly IConnectivityService connectivity = Substitute.For<IConnectivityService>();
    private readonly IHttpClientWrapper httpClient = Substitute.For<IHttpClientWrapper>();
    private readonly IPlantSpeciesService speciesService = Substitute.For<IPlantSpeciesService>();
    private readonly string cacheBasePath;

    private const string WikiApiJson = """
        {
            "originalimage": {
                "source": "https://upload.wikimedia.org/wikipedia/commons/test.jpg"
            }
        }
        """;

    private static readonly byte[] FakeImageBytes = [0x89, 0x50, 0x4E, 0x47];

    public ImageCacheServiceTests()
    {
        this.cacheBasePath = Path.Combine(Path.GetTempPath(), "plantpal_test_cache_" + Guid.NewGuid());
        this.speciesService.GetAll().Returns(new List<PlantSpecies>
        {
            new()
            {
                Id = "monstera_deliciosa",
                CommonName = "Monstera",
                LatinName = "Monstera deliciosa",
                WikipediaSlug = "Monstera_deliciosa",
                ThumbnailAssetPath = "plants/monstera_deliciosa_thumb.png",
                DetailAssetPath = "plants/monstera_deliciosa_detail.png",
                WateringIntervalDays = 7,
            },
        });
    }

    private ImageCacheService CreateService() =>
        new(this.connectivity, this.httpClient, this.speciesService, this.cacheBasePath);

    public void Dispose()
    {
        if (Directory.Exists(this.cacheBasePath))
        {
            Directory.Delete(this.cacheBasePath, recursive: true);
        }
    }

    // ── Positive cases ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetThumbnailPathAsync_ReturnsAssetPath()
    {
        var service = this.CreateService();

        var result = await service.GetThumbnailPathAsync("monstera_deliciosa");

        Assert.Equal("plants/monstera_deliciosa_thumb.png", result);
        await this.httpClient.DidNotReceiveWithAnyArgs().GetStringAsync(default!);
    }

    [Fact]
    public async Task GetDetailImageAsync_CacheHit_ReturnsLocalPath_WithoutHttp()
    {
        Directory.CreateDirectory(this.cacheBasePath);
        var cachedPath = Path.Combine(this.cacheBasePath, "monstera_deliciosa_detail.jpg");
        await File.WriteAllBytesAsync(cachedPath, FakeImageBytes);

        var service = this.CreateService();
        var result = await service.GetDetailImageAsync("monstera_deliciosa");

        Assert.Equal(cachedPath, result);
        await this.httpClient.DidNotReceiveWithAnyArgs().GetStringAsync(default!);
    }

    [Fact]
    public async Task GetDetailImageAsync_CacheMiss_Online_DownloadsAndCaches()
    {
        this.connectivity.IsConnected.Returns(true);
        this.httpClient.GetStringAsync(Arg.Any<string>()).Returns(WikiApiJson);
        this.httpClient.GetBytesAsync(Arg.Any<string>()).Returns(FakeImageBytes);

        var service = this.CreateService();
        var result = await service.GetDetailImageAsync("monstera_deliciosa");

        var expectedPath = Path.Combine(this.cacheBasePath, "monstera_deliciosa_detail.jpg");
        Assert.Equal(expectedPath, result);
        Assert.True(File.Exists(expectedPath));
        await this.httpClient.Received(1).GetStringAsync(
            Arg.Is<string>(u => u.Contains("Monstera_deliciosa")));
    }

    [Fact]
    public async Task GetDetailImageAsync_CacheMiss_Offline_ReturnsThumbnail()
    {
        this.connectivity.IsConnected.Returns(false);

        var service = this.CreateService();
        var result = await service.GetDetailImageAsync("monstera_deliciosa");

        Assert.Equal("plants/monstera_deliciosa_thumb.png", result);
        await this.httpClient.DidNotReceiveWithAnyArgs().GetStringAsync(default!);
    }

    // ── Negative cases ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetDetailImageAsync_HttpError_ReturnsThumbnail()
    {
        this.connectivity.IsConnected.Returns(true);
        this.httpClient.GetStringAsync(Arg.Any<string>())
            .ThrowsAsync(new HttpRequestException("404 Not Found"));

        var service = this.CreateService();
        var result = await service.GetDetailImageAsync("monstera_deliciosa");

        Assert.Equal("plants/monstera_deliciosa_thumb.png", result);
    }

    [Fact]
    public async Task GetDetailImageAsync_Timeout_ReturnsThumbnail()
    {
        this.connectivity.IsConnected.Returns(true);
        this.httpClient.GetStringAsync(Arg.Any<string>())
            .ThrowsAsync(new TaskCanceledException("Request timed out"));

        var service = this.CreateService();
        var result = await service.GetDetailImageAsync("monstera_deliciosa");

        Assert.Equal("plants/monstera_deliciosa_thumb.png", result);
    }

    [Fact]
    public async Task GetDetailImageAsync_NullKey_ThrowsArgumentNullException()
    {
        var service = this.CreateService();

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => service.GetDetailImageAsync(null!));
    }

    [Fact]
    public async Task GetDetailImageAsync_UnknownKey_ReturnsPlaceholder()
    {
        var service = this.CreateService();
        var result = await service.GetDetailImageAsync("nonexistent_species");

        Assert.Equal("plants/placeholder_thumb.png", result);
    }
}
