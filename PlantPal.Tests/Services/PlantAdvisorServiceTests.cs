using NSubstitute;
using NSubstitute.ExceptionExtensions;
using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;
using PlantPal.Core.Services;

namespace PlantPal.Tests.Services;

/// <summary>
/// Unit tests for <see cref="PlantAdvisorService"/>.
/// </summary>
public class PlantAdvisorServiceTests
{
    private readonly IHttpClientWrapper http;
    private readonly ISecureStorageService secureStorage;
    private readonly PlantAdvisorService sut;

    /// <summary>Initialises mocks and the system under test.</summary>
    public PlantAdvisorServiceTests()
    {
        this.http = Substitute.For<IHttpClientWrapper>();
        this.secureStorage = Substitute.For<ISecureStorageService>();
        this.sut = new PlantAdvisorService(this.http, this.secureStorage);
    }

    // ── IsConfigured ───────────────────────────────────────────────────────────

    [Fact]
    public async Task IsConfigured_WhenApiKeyStored_ReturnsTrue()
    {
        this.secureStorage.GetAsync("anthropic_api_key").Returns("sk-test-key");

        await this.sut.InitialiseAsync();

        Assert.True(this.sut.IsConfigured);
    }

    [Fact]
    public async Task IsConfigured_WhenNoApiKey_ReturnsFalse()
    {
        this.secureStorage.GetAsync("anthropic_api_key").Returns((string?)null);

        await this.sut.InitialiseAsync();

        Assert.False(this.sut.IsConfigured);
    }

    // ── No API key guard ───────────────────────────────────────────────────────

    [Fact]
    public async Task AskAboutPlantAsync_WhenNotConfigured_ReturnsSetupMessage()
    {
        this.secureStorage.GetAsync("anthropic_api_key").Returns((string?)null);
        await this.sut.InitialiseAsync();

        var result = await this.sut.AskAboutPlantAsync("Monstera", "Why yellow leaves?", []);

        Assert.Equal("Please add your Anthropic API key in Settings", result);
        await this.http.DidNotReceive().GetStringAsync(Arg.Any<string>());
    }

    // ── Positive case ──────────────────────────────────────────────────────────

    [Fact]
    public async Task AskAboutPlantAsync_ValidResponse_ReturnsContent()
    {
        this.secureStorage.GetAsync("anthropic_api_key").Returns("sk-test-key");
        await this.sut.InitialiseAsync();
        this.http.PostStringAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(BuildResponse("Water less frequently."));

        var result = await this.sut.AskAboutPlantAsync("Monstera", "Yellow leaves?", []);

        Assert.Equal("Water less frequently.", result);
    }

    // ── Error cases ────────────────────────────────────────────────────────────

    [Fact]
    public async Task AskAboutPlantAsync_Api401_ReturnsInvalidKeyMessage()
    {
        this.secureStorage.GetAsync("anthropic_api_key").Returns("sk-bad-key");
        await this.sut.InitialiseAsync();
        this.http.PostStringAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .ThrowsAsync(new HttpRequestException("Unauthorized", null, System.Net.HttpStatusCode.Unauthorized));

        var result = await this.sut.AskAboutPlantAsync("Monstera", "Yellow leaves?", []);

        Assert.Equal("API key invalid — check Settings", result);
    }

    [Fact]
    public async Task AskAboutPlantAsync_Timeout_ReturnsConnectionMessage()
    {
        this.secureStorage.GetAsync("anthropic_api_key").Returns("sk-test-key");
        await this.sut.InitialiseAsync();
        this.http.PostStringAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .ThrowsAsync(new TaskCanceledException("timeout"));

        var result = await this.sut.AskAboutPlantAsync("Monstera", "Yellow leaves?", []);

        Assert.Equal("Couldn't reach Claude — check your connection", result);
    }

    [Fact]
    public async Task AskAboutPlantAsync_Api500_ReturnsGracefulError()
    {
        this.secureStorage.GetAsync("anthropic_api_key").Returns("sk-test-key");
        await this.sut.InitialiseAsync();
        this.http.PostStringAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .ThrowsAsync(new HttpRequestException("Server error", null, System.Net.HttpStatusCode.InternalServerError));

        var result = await this.sut.AskAboutPlantAsync("Monstera", "Yellow leaves?", []);

        Assert.False(string.IsNullOrWhiteSpace(result));
        Assert.DoesNotContain("exception", result, StringComparison.OrdinalIgnoreCase);
    }

    // ── DiagnosePlantAsync (vision) ────────────────────────────────────────────

    [Fact]
    public async Task DiagnosePlantAsync_WhenNotConfigured_ReturnsSetupMessage()
    {
        this.secureStorage.GetAsync("anthropic_api_key").Returns((string?)null);
        await this.sut.InitialiseAsync();

        var result = await this.sut.DiagnosePlantAsync("Monstera", "What's wrong?", [0xFF, 0xD8], "image/jpeg", []);

        Assert.Equal("Please add your Anthropic API key in Settings", result);
        await this.http.DidNotReceive().PostStringAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task DiagnosePlantAsync_ValidResponse_ReturnsContent()
    {
        this.secureStorage.GetAsync("anthropic_api_key").Returns("sk-test-key");
        await this.sut.InitialiseAsync();
        this.http.PostStringAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(BuildResponse("Your plant looks healthy but slightly overwatered."));

        var result = await this.sut.DiagnosePlantAsync("Monstera", "Diagnose this", [0xFF, 0xD8], "image/jpeg", []);

        Assert.Equal("Your plant looks healthy but slightly overwatered.", result);
    }

    [Fact]
    public async Task DiagnosePlantAsync_Api401_ReturnsInvalidKeyMessage()
    {
        this.secureStorage.GetAsync("anthropic_api_key").Returns("sk-bad-key");
        await this.sut.InitialiseAsync();
        this.http.PostStringAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .ThrowsAsync(new HttpRequestException("Unauthorized", null, System.Net.HttpStatusCode.Unauthorized));

        var result = await this.sut.DiagnosePlantAsync("Monstera", "Diagnose", [0xFF], "image/jpeg", []);

        Assert.Equal("API key invalid — check Settings", result);
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private static string BuildResponse(string text) =>
        $$"""
        {
          "content": [
            { "type": "text", "text": "{{text}}" }
          ]
        }
        """;
}
