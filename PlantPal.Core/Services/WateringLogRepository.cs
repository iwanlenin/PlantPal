using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;
using SQLite;

namespace PlantPal.Core.Services;

/// <summary>
/// SQLite-backed implementation of <see cref="IWateringLogRepository"/>.
/// Creates the WateringLog table on first use.
/// </summary>
public class WateringLogRepository : IWateringLogRepository
{
    private readonly SQLiteAsyncConnection connection;
    private bool isInitialised;

    /// <summary>
    /// Initialises the repository with the specified database file path.
    /// Pass <c>":memory:"</c> for an in-memory database (used in tests).
    /// </summary>
    /// <param name="dbPath">The full file path to the SQLite database.</param>
    public WateringLogRepository(string dbPath)
    {
        this.connection = new SQLiteAsyncConnection(dbPath);
    }

    /// <inheritdoc />
    public async Task<List<WateringLog>> GetAllAsync()
    {
        await this.InitAsync();
        return await this.connection.Table<WateringLog>().ToListAsync();
    }

    /// <inheritdoc />
    public async Task<List<WateringLog>> GetByPlantIdAsync(int plantId)
    {
        await this.InitAsync();
        return await this.connection.Table<WateringLog>()
            .Where(l => l.PlantId == plantId)
            .OrderByDescending(l => l.WateredAt)
            .ToListAsync();
    }

    /// <inheritdoc />
    /// <remarks>
    /// Always inserts — watering logs are immutable once created. There is no update path
    /// because past watering events should not be edited (they form an audit trail).
    /// Deletion is only possible at the plant level via <see cref="DeleteByPlantIdAsync"/>.
    ///
    /// <see cref="WateringLog.SyncId"/> and <see cref="WateringLog.UpdatedAt"/> are managed
    /// here for the same reasons as in <see cref="DatabaseService.SaveAsync"/>: SyncId gives
    /// each log entry a stable cross-device identity, and UpdatedAt allows
    /// <see cref="Core.Services.SyncService"/> to detect logs that haven't been pushed yet.
    /// </remarks>
    public async Task SaveAsync(WateringLog log)
    {
        await this.InitAsync();

        if (string.IsNullOrEmpty(log.SyncId))
        {
            log.SyncId = Guid.NewGuid().ToString();
        }

        log.UpdatedAt = DateTime.UtcNow;
        await this.connection.InsertAsync(log);
    }

    /// <inheritdoc />
    public async Task DeleteByPlantIdAsync(int plantId)
    {
        await this.InitAsync();
        await this.connection.Table<WateringLog>().DeleteAsync(l => l.PlantId == plantId);
    }

    /// <summary>Ensures the WateringLog table exists. Called lazily before every operation.</summary>
    private async Task InitAsync()
    {
        if (this.isInitialised)
        {
            return;
        }

        await this.connection.CreateTableAsync<WateringLog>();
        this.isInitialised = true;
    }
}
