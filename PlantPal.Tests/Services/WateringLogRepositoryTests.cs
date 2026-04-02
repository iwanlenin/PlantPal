using PlantPal.Core.Models;
using PlantPal.Core.Services;

namespace PlantPal.Tests.Services;

/// <summary>
/// Unit tests for <see cref="WateringLogRepository"/> using real in-memory SQLite.
/// Each test creates a fresh database instance for full isolation.
/// </summary>
public class WateringLogRepositoryTests
{
    private static WateringLogRepository CreateRepository() =>
        new(":memory:");

    private static WateringLog CreateLog(int plantId) => new()
    {
        PlantId = plantId,
        WateredAt = DateTime.UtcNow,
    };

    // ── Positive cases ────────────────────────────────────────────────────────

    [Fact]
    public async Task SaveAsync_Then_GetByPlantIdAsync_Returns_Log()
    {
        var repo = CreateRepository();
        var log = CreateLog(plantId: 1);

        await repo.SaveAsync(log);
        var result = await repo.GetByPlantIdAsync(1);

        Assert.Single(result);
        Assert.Equal(1, result[0].PlantId);
    }

    [Fact]
    public async Task DeleteByPlantIdAsync_Removes_All_Logs_For_Plant()
    {
        var repo = CreateRepository();
        await repo.SaveAsync(CreateLog(plantId: 1));
        await repo.SaveAsync(CreateLog(plantId: 1));
        await repo.SaveAsync(CreateLog(plantId: 2));

        await repo.DeleteByPlantIdAsync(1);

        var logsForPlant1 = await repo.GetByPlantIdAsync(1);
        var logsForPlant2 = await repo.GetByPlantIdAsync(2);
        Assert.Empty(logsForPlant1);
        Assert.Single(logsForPlant2);
    }

    [Fact]
    public async Task GetByPlantIdAsync_EmptyDb_Returns_Empty_List()
    {
        var repo = CreateRepository();

        var result = await repo.GetByPlantIdAsync(1);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    // ── Negative cases ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByPlantIdAsync_NonexistentPlantId_Returns_Empty_List()
    {
        var repo = CreateRepository();
        await repo.SaveAsync(CreateLog(plantId: 1));

        var result = await repo.GetByPlantIdAsync(999);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
