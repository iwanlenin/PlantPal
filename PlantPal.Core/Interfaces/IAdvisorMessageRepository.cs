using PlantPal.Core.Models;

namespace PlantPal.Core.Interfaces;

/// <summary>
/// Defines storage operations for plant advisor chat messages.
/// </summary>
public interface IAdvisorMessageRepository
{
    /// <summary>Returns all messages for the given plant, ordered oldest-first.</summary>
    Task<List<AdvisorMessage>> GetByPlantIdAsync(int plantId);

    /// <summary>Inserts or updates a message.</summary>
    Task SaveAsync(AdvisorMessage message);

    /// <summary>Deletes all messages for the given plant.</summary>
    Task DeleteByPlantIdAsync(int plantId);
}
