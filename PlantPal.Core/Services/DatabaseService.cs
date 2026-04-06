using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;
using SQLite;

namespace PlantPal.Core.Services;

/// <summary>
/// SQLite-backed implementation of <see cref="IPlantRepository"/>.
/// Creates the Plant table on first use and provides full CRUD operations.
/// </summary>
public class DatabaseService : IPlantRepository
{
    private readonly SQLiteAsyncConnection connection;
    private bool isInitialised;

    /// <summary>
    /// Initialises the service with the specified database file path.
    /// Pass <c>":memory:"</c> for an in-memory database (used in tests).
    /// </summary>
    /// <param name="dbPath">The full file path to the SQLite database.</param>
    public DatabaseService(string dbPath)
    {
        this.connection = new SQLiteAsyncConnection(dbPath);
    }

    /// <inheritdoc />
    public async Task<List<Plant>> GetAllAsync()
    {
        await this.InitAsync();
        return await this.connection.Table<Plant>().ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Plant?> GetByIdAsync(int id)
    {
        await this.InitAsync();
        return await this.connection.Table<Plant>().FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Uses an insert-or-update pattern: if <see cref="Plant.Id"/> is 0 the record is new
    /// and the database auto-increments the Id; otherwise the existing row is updated.
    ///
    /// <see cref="Plant.SyncId"/> is assigned here on first save (when empty) so every plant
    /// has a stable cross-device identity before it ever reaches <see cref="Core.Services.SyncService"/>.
    /// This prevents duplicate rows in Supabase, which uses SyncId as the upsert conflict key.
    ///
    /// <see cref="Plant.UpdatedAt"/> is always overwritten with <see cref="DateTime.UtcNow"/>
    /// so the last-write-wins logic in <see cref="Core.Services.SyncService.SyncDownAsync"/>
    /// can determine which version of a plant is newer when merging remote and local changes.
    ///
    /// <see cref="Plant.RecalculateNextWaterDate"/> is called unconditionally because any edit
    /// (including changing the watering interval without logging a new watering) must push the
    /// next reminder date forward.
    /// </remarks>
    public async Task SaveAsync(Plant plant)
    {
        if (string.IsNullOrWhiteSpace(plant.Name))
        {
            throw new ArgumentException("Plant name must not be null or empty.", nameof(plant));
        }

        await this.InitAsync();

        if (string.IsNullOrEmpty(plant.SyncId))
        {
            plant.SyncId = Guid.NewGuid().ToString();
        }

        plant.UpdatedAt = DateTime.UtcNow;
        plant.RecalculateNextWaterDate();

        if (plant.Id != 0)
        {
            await this.connection.UpdateAsync(plant);
        }
        else
        {
            await this.connection.InsertAsync(plant);
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id)
    {
        await this.InitAsync();
        await this.connection.DeleteAsync<Plant>(id);
    }

    /// <summary>
    /// Ensures the Plant table exists and is up to date, called lazily before every operation.
    /// </summary>
    /// <remarks>
    /// sqlite-net-pcl's <c>CreateTableAsync</c> is idempotent: it creates the table if it does
    /// not exist, and adds any columns that are present in the model but absent in the schema.
    /// This means adding new columns to <see cref="Plant"/> (e.g. SyncId, UpdatedAt) automatically
    /// migrates existing databases on first run after an app update — no manual ALTER TABLE needed.
    /// The <see cref="isInitialised"/> flag avoids redundant schema checks on every operation.
    /// </remarks>
    private async Task InitAsync()
    {
        if (this.isInitialised)
        {
            return;
        }

        await this.connection.CreateTableAsync<Plant>();
        this.isInitialised = true;
    }
}
