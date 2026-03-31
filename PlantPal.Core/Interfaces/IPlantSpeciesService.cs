using PlantPal.Core.Models;

namespace PlantPal.Core.Interfaces;

/// <summary>
/// Defines the contract for accessing the built-in list of 40 common European houseplant species.
/// </summary>
public interface IPlantSpeciesService
{
    /// <summary>Returns the full read-only list of all supported plant species.</summary>
    IReadOnlyList<PlantSpecies> GetAll();

    /// <summary>
    /// Finds a species by common name (case-insensitive). Returns null if not found.
    /// Returns null for null or empty input without throwing.
    /// </summary>
    /// <param name="name">The common name to search for (e.g. "Monstera").</param>
    PlantSpecies? FindByName(string? name);

    /// <summary>
    /// Returns the suggested watering interval in days for the given species key.
    /// Returns 7 (default) if the key is not found.
    /// </summary>
    /// <param name="speciesKey">The species identifier (e.g. "monstera_deliciosa").</param>
    int GetSuggestedInterval(string speciesKey);
}
