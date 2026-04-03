namespace PlantPal.Core.Models;

/// <summary>
/// Represents a single recorded watering event for a plant.
/// </summary>
public class WateringLog
{
    /// <summary>Gets or sets the unique identifier for this watering log entry (SQLite primary key).</summary>
    [SQLite.PrimaryKey]
    [SQLite.AutoIncrement]
    public int Id { get; set; }

    /// <summary>Gets or sets the identifier of the plant that was watered (foreign key to <see cref="Plant.Id"/>).</summary>
    public int PlantId { get; set; }

    /// <summary>Gets or sets the date and time when the plant was watered.</summary>
    public DateTime WateredAt { get; set; }

    /// <summary>
    /// Gets or sets the stable cross-device identifier (UUID string).
    /// Generated on first save; used to reconcile records during cloud sync.
    /// </summary>
    public string SyncId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the UTC timestamp of the last modification.
    /// Used for last-write-wins conflict resolution during sync.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
