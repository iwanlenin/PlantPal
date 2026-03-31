using PlantPal.Core.Models;

namespace PlantPal.Core.Interfaces;

/// <summary>
/// Defines the contract for persisting and retrieving <see cref="Plant"/> records.
/// </summary>
public interface IPlantRepository
{
    /// <summary>Returns all plants stored in the local database.</summary>
    Task<List<Plant>> GetAllAsync();

    /// <summary>Returns the plant with the specified identifier, or null if not found.</summary>
    /// <param name="id">The plant's primary key.</param>
    Task<Plant?> GetByIdAsync(int id);

    /// <summary>Inserts a new plant or updates an existing one (matched by <see cref="Plant.Id"/>).</summary>
    /// <param name="plant">The plant to save.</param>
    Task SaveAsync(Plant plant);

    /// <summary>Deletes the plant with the specified identifier. Has no effect if the id does not exist.</summary>
    /// <param name="id">The primary key of the plant to delete.</param>
    Task DeleteAsync(int id);
}
