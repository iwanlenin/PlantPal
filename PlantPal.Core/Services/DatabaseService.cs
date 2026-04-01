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
    public async Task SaveAsync(Plant plant)
    {
        if (string.IsNullOrWhiteSpace(plant.Name))
        {
            throw new ArgumentException("Plant name must not be null or empty.", nameof(plant));
        }

        await this.InitAsync();
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
    /// Ensures the Plant table exists. Called lazily before every operation.
    /// </summary>
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
