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
}
