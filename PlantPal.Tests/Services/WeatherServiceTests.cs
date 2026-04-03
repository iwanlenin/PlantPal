using NSubstitute;
using NSubstitute.ExceptionExtensions;
using PlantPal.Core.Interfaces;
using PlantPal.Core.Services;

namespace PlantPal.Tests.Services;

/// <summary>
/// Unit tests for <see cref="WeatherService"/>.
/// </summary>
public class WeatherServiceTests
{
    private readonly IHttpClientWrapper http;
    private readonly IConnectivityService connectivity;
    private readonly WeatherService sut;

    /// <summary>Initialises mocks and the system under test.</summary>
    public WeatherServiceTests()
    {
        this.http = Substitute.For<IHttpClientWrapper>();
        this.connectivity = Substitute.For<IConnectivityService>();
        this.sut = new WeatherService(this.http, this.connectivity);
    }

    // ── Positive cases ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetPostponementDaysAsync_Online_RainOver10mm_Returns2()
    {
        this.connectivity.IsConnected.Returns(true);
        this.sut.SetCoordinates(52.0, 13.0);
        this.http.GetStringAsync(Arg.Any<string>()).Returns(BuildResponse(11.5));

        var result = await this.sut.GetPostponementDaysAsync();

        Assert.Equal(2, result);
    }

    [Fact]
    public async Task GetPostponementDaysAsync_Online_RainOver5mm_Returns1()
    {
        this.connectivity.IsConnected.Returns(true);
        this.sut.SetCoordinates(52.0, 13.0);
        this.http.GetStringAsync(Arg.Any<string>()).Returns(BuildResponse(7.0));

        var result = await this.sut.GetPostponementDaysAsync();

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task GetPostponementDaysAsync_Online_RainAtOrBelow5mm_Returns0()
    {
        this.connectivity.IsConnected.Returns(true);
        this.sut.SetCoordinates(52.0, 13.0);
        this.http.GetStringAsync(Arg.Any<string>()).Returns(BuildResponse(3.0));

        var result = await this.sut.GetPostponementDaysAsync();

        Assert.Equal(0, result);
    }

    // ── Negative / edge cases ──────────────────────────────────────────────────

    [Fact]
    public async Task GetPostponementDaysAsync_Offline_Returns0()
    {
        this.connectivity.IsConnected.Returns(false);
        this.sut.SetCoordinates(52.0, 13.0);

        var result = await this.sut.GetPostponementDaysAsync();

        Assert.Equal(0, result);
        await this.http.DidNotReceive().GetStringAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task GetPostponementDaysAsync_ApiReturnsInvalidJson_Returns0()
    {
        this.connectivity.IsConnected.Returns(true);
        this.sut.SetCoordinates(52.0, 13.0);
        this.http.GetStringAsync(Arg.Any<string>()).Returns("Internal Server Error");

        var result = await this.sut.GetPostponementDaysAsync();

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetPostponementDaysAsync_ApiThrows_Returns0()
    {
        this.connectivity.IsConnected.Returns(true);
        this.sut.SetCoordinates(52.0, 13.0);
        this.http.GetStringAsync(Arg.Any<string>()).ThrowsAsync(new TaskCanceledException("timeout"));

        var result = await this.sut.GetPostponementDaysAsync();

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetPostponementDaysAsync_CoordinatesNotSet_Returns0()
    {
        this.connectivity.IsConnected.Returns(true);
        // SetCoordinates never called — coordinates remain 0,0

        var result = await this.sut.GetPostponementDaysAsync();

        Assert.Equal(0, result);
        await this.http.DidNotReceive().GetStringAsync(Arg.Any<string>());
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    /// <summary>Builds a minimal Open-Meteo daily forecast JSON response.</summary>
    private static string BuildResponse(double precipitationMm) =>
        $$"""
        {
          "daily": {
            "precipitation_sum": [{{precipitationMm}}]
          }
        }
        """;
}
