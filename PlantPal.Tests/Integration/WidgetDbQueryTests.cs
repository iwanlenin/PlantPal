using PlantPal.Core.Models;
using PlantPal.Core.Services;
using SQLite;

namespace PlantPal.Tests.Integration;

/// <summary>
/// Integration tests for the widget DB query logic using real in-memory SQLite.
/// Verifies the due-plant count query independently of Android platform code.
/// </summary>
public class WidgetDbQueryTests
{
    private static async Task<SQLiteAsyncConnection> CreatePopulatedDb(IEnumerable<Plant> plants)
    {
        // Use a unique named in-memory database per test to avoid sqlite-net-pcl connection pooling
        // reusing the same :memory: connection across test instances.
        var uniqueName = Guid.NewGuid().ToString("N");
        var db = new SQLiteAsyncConnection($"file:{uniqueName}?mode=memory&cache=shared");
        await db.CreateTableAsync<Plant>();
        foreach (var plant in plants)
        {
            await db.InsertAsync(plant);
        }

        return db;
    }

    private static Plant CreatePlant(DateTime? nextWaterDate) => new()
    {
        Name = "Test Plant",
        Species = "monstera_deliciosa",
        WateringIntervalDays = 7,
        NextWaterDate = nextWaterDate,
    };

    [Fact]
    public async Task CountDuePlants_ThreeDuePlants_ReturnsThree()
    {
        var plants = new[]
        {
            CreatePlant(DateTime.Today),
            CreatePlant(DateTime.Today.AddDays(-1)),
            CreatePlant(DateTime.Today.AddDays(-3)),
            CreatePlant(DateTime.Today.AddDays(2)), // upcoming — not due
        };
        var db = await CreatePopulatedDb(plants);

        var count = await WidgetDbQuery.CountDuePlantsAsync(db);

        Assert.Equal(3, count);
    }

    [Fact]
    public async Task CountDuePlants_NoDuePlants_ReturnsZero()
    {
        var plants = new[]
        {
            CreatePlant(DateTime.Today.AddDays(1)),
            CreatePlant(DateTime.Today.AddDays(5)),
        };
        var db = await CreatePopulatedDb(plants);

        var count = await WidgetDbQuery.CountDuePlantsAsync(db);

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task CountDuePlants_EmptyDatabase_ReturnsZero()
    {
        var db = await CreatePopulatedDb([]);

        var count = await WidgetDbQuery.CountDuePlantsAsync(db);

        Assert.Equal(0, count);
    }
}
