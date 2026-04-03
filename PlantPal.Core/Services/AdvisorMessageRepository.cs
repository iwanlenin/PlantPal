using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;
using SQLite;

namespace PlantPal.Core.Services;

/// <summary>
/// SQLite-backed implementation of <see cref="IAdvisorMessageRepository"/>.
/// Shares the same database file as other repositories.
/// </summary>
public class AdvisorMessageRepository : IAdvisorMessageRepository
{
    private readonly SQLiteAsyncConnection connection;
    private bool isInitialised;

    /// <summary>
    /// Initialises a new instance of <see cref="AdvisorMessageRepository"/>.
    /// </summary>
    /// <param name="dbPath">Full path to the SQLite database file.</param>
    public AdvisorMessageRepository(string dbPath)
    {
        this.connection = new SQLiteAsyncConnection(dbPath);
    }

    /// <inheritdoc />
    public async Task<List<AdvisorMessage>> GetByPlantIdAsync(int plantId)
    {
        await this.InitAsync();
        return await this.connection
            .Table<AdvisorMessage>()
            .Where(m => m.PlantId == plantId)
            .OrderBy(m => m.Timestamp)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task SaveAsync(AdvisorMessage message)
    {
        await this.InitAsync();
        if (message.Id == 0)
        {
            await this.connection.InsertAsync(message);
        }
        else
        {
            await this.connection.UpdateAsync(message);
        }
    }

    /// <inheritdoc />
    public async Task DeleteByPlantIdAsync(int plantId)
    {
        await this.InitAsync();
        await this.connection.ExecuteAsync(
            "DELETE FROM AdvisorMessage WHERE PlantId = ?", plantId);
    }

    private async Task InitAsync()
    {
        if (this.isInitialised)
        {
            return;
        }

        await this.connection.CreateTableAsync<AdvisorMessage>();
        this.isInitialised = true;
    }
}
