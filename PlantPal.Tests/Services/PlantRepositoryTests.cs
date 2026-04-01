using PlantPal.Core.Models;
using PlantPal.Core.Services;

namespace PlantPal.Tests.Services;

/// <summary>
/// Integration tests for <see cref="DatabaseService"/> using per-test temp SQLite files.
/// Each test gets an isolated database to avoid connection-pool sharing.
/// </summary>
public class PlantRepositoryTests : IDisposable
{
    private readonly List<string> tempFiles = new();

    private DatabaseService CreateService()
    {
        var path = Path.Combine(Path.GetTempPath(), $"plantpal_test_{Guid.NewGuid()}.db");
        this.tempFiles.Add(path);
        return new DatabaseService(path);
    }

    /// <summary>Cleans up temp database files after each test.</summary>
    public void Dispose()
    {
        foreach (var file in this.tempFiles)
        {
            try { File.Delete(file); } catch { /* best effort */ }
        }
    }

    private static Plant CreateTestPlant(string name = "Test Plant", int interval = 7)
    {
        return new Plant
        {
            Name = name,
            Species = "Monstera",
            Location = "Living Room",
            WateringIntervalDays = interval,
            LastWateredDate = new DateTime(2026, 3, 1),
        };
    }

    [Fact]
    public async Task SaveAsync_Then_GetAllAsync_Returns_Plant()
    {
        var service = this.CreateService();
        var plant = CreateTestPlant();

        await service.SaveAsync(plant);
        var all = await service.GetAllAsync();

        Assert.Single(all);
        Assert.Equal("Test Plant", all[0].Name);
    }

    [Fact]
    public async Task SaveAsync_ExistingPlant_Updates_Name()
    {
        var service = this.CreateService();
        var plant = CreateTestPlant();

        await service.SaveAsync(plant);
        var saved = (await service.GetAllAsync())[0];

        saved.Name = "Renamed Plant";
        await service.SaveAsync(saved);

        var updated = await service.GetByIdAsync(saved.Id);
        Assert.NotNull(updated);
        Assert.Equal("Renamed Plant", updated.Name);
    }

    [Fact]
    public async Task DeleteAsync_Then_GetAllAsync_Does_Not_Contain_Plant()
    {
        var service = this.CreateService();
        var plant = CreateTestPlant();

        await service.SaveAsync(plant);
        var saved = (await service.GetAllAsync())[0];

        await service.DeleteAsync(saved.Id);
        var all = await service.GetAllAsync();

        Assert.Empty(all);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_Returns_Plant()
    {
        var service = this.CreateService();
        var plant = CreateTestPlant();

        await service.SaveAsync(plant);
        var saved = (await service.GetAllAsync())[0];

        var result = await service.GetByIdAsync(saved.Id);

        Assert.NotNull(result);
        Assert.Equal("Test Plant", result.Name);
    }

    [Fact]
    public async Task SaveAsync_Sets_NextWaterDate_Via_RecalculateNextWaterDate()
    {
        var service = this.CreateService();
        var plant = CreateTestPlant(interval: 7);
        plant.LastWateredDate = new DateTime(2026, 3, 1);

        await service.SaveAsync(plant);
        var saved = (await service.GetAllAsync())[0];

        Assert.Equal(new DateTime(2026, 3, 8), saved.NextWaterDate);
    }

    [Fact]
    public async Task GetAllAsync_EmptyDb_Returns_EmptyList()
    {
        var service = this.CreateService();

        var all = await service.GetAllAsync();

        Assert.NotNull(all);
        Assert.Empty(all);
    }

    [Fact]
    public async Task GetByIdAsync_NegativeId_Returns_Null()
    {
        var service = this.CreateService();

        var result = await service.GetByIdAsync(-1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_NonexistentId_Returns_Null()
    {
        var service = this.CreateService();

        var result = await service.GetByIdAsync(99999);

        Assert.Null(result);
    }

    [Fact]
    public async Task SaveAsync_NullName_Throws_ArgumentException()
    {
        var service = this.CreateService();
        var plant = CreateTestPlant();
        plant.Name = null!;

        await Assert.ThrowsAsync<ArgumentException>(() => service.SaveAsync(plant));
    }

    [Fact]
    public async Task DeleteAsync_NonexistentId_Completes_Without_Throwing()
    {
        var service = this.CreateService();

        var exception = await Record.ExceptionAsync(() => service.DeleteAsync(99999));

        Assert.Null(exception);
    }
}
