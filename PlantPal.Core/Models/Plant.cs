namespace PlantPal.Core.Models;

/// <summary>
/// Represents an indoor plant tracked by the user.
/// </summary>
public class Plant
{
    /// <summary>Gets or sets the unique identifier for the plant (SQLite primary key).</summary>
    [SQLite.PrimaryKey]
    [SQLite.AutoIncrement]
    public int Id { get; set; }

    /// <summary>Gets or sets the user-given nickname for the plant.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the species name of the plant.</summary>
    public string Species { get; set; } = string.Empty;

    /// <summary>Gets or sets the location of the plant within the home.</summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>Gets or sets how many days between each watering.</summary>
    public int WateringIntervalDays { get; set; }

    /// <summary>Gets or sets the date the plant was last watered. Null if never watered.</summary>
    public DateTime? LastWateredDate { get; set; }

    /// <summary>Gets or sets the calculated next watering date. Null until first watering is logged.</summary>
    public DateTime? NextWaterDate { get; set; }

    /// <summary>Gets or sets the local file path to the user's photo of this plant. Null if no photo.</summary>
    public string? PhotoPath { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this plant's watering reminder is currently
    /// weather-adjusted. Not persisted — set at runtime by the ViewModel when the feature is active.
    /// </summary>
    [SQLite.Ignore]
    public bool IsWeatherAdjusted { get; set; }

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

    /// <summary>
    /// Recalculates <see cref="NextWaterDate"/> based on <see cref="LastWateredDate"/>
    /// and <see cref="WateringIntervalDays"/>. Has no effect if <see cref="LastWateredDate"/> is null.
    /// </summary>
    public void RecalculateNextWaterDate()
    {
        this.NextWaterDate = this.LastWateredDate?.AddDays(this.WateringIntervalDays);
    }
}
