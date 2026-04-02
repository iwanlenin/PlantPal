using PlantPal.Core.Models;
using SQLite;

namespace PlantPal.Core.Services;

/// <summary>
/// Provides the database query used by the Android home screen widget to count
/// plants due for watering today. Kept in Core so the logic can be integration-tested
/// without Android platform dependencies.
/// </summary>
public static class WidgetDbQuery
{
    /// <summary>
    /// Returns the number of plants whose <see cref="Plant.NextWaterDate"/> is today or in the past.
    /// </summary>
    /// <param name="connection">An open SQLite connection (table will be created if absent).</param>
    public static async Task<int> CountDuePlantsAsync(SQLiteAsyncConnection connection)
    {
        await connection.CreateTableAsync<Plant>();
        var today = DateTime.Today;
        return await connection.Table<Plant>()
            .CountAsync(p => p.NextWaterDate != null && p.NextWaterDate <= today);
    }

    /// <summary>
    /// Opens a connection to the database at <paramref name="dbPath"/> and returns the due plant count.
    /// Used by the Android widget provider which cannot access MAUI's DI container.
    /// </summary>
    /// <param name="dbPath">Absolute path to the SQLite database file.</param>
    public static async Task<int> CountDuePlantsAsync(string dbPath)
    {
        var connection = new SQLiteAsyncConnection(dbPath);
        try
        {
            return await CountDuePlantsAsync(connection);
        }
        finally
        {
            await connection.CloseAsync();
        }
    }
}
