using PlantPal.Core.Models;

namespace PlantPal.Core.Interfaces;

/// <summary>
/// Defines the contract for persisting and retrieving <see cref="WateringLog"/> records.
/// </summary>
public interface IWateringLogRepository
{
    /// <summary>Returns all watering logs for the specified plant, newest first.</summary>
    /// <param name="plantId">The primary key of the plant whose logs to retrieve.</param>
    Task<List<WateringLog>> GetByPlantIdAsync(int plantId);

    /// <summary>Inserts a new watering log entry.</summary>
    /// <param name="log">The watering log to save.</param>
    Task SaveAsync(WateringLog log);

    /// <summary>Deletes all watering logs associated with the specified plant.</summary>
    /// <param name="plantId">The primary key of the plant whose logs to delete.</param>
    Task DeleteByPlantIdAsync(int plantId);
}
