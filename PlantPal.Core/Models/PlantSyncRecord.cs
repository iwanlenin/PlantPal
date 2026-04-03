namespace PlantPal.Core.Models;

/// <summary>
/// Data-transfer object used to exchange plant data with Supabase.
/// Maps to the <c>plants</c> table schema.
/// </summary>
public class PlantSyncRecord
{
    /// <summary>Gets or sets the stable cross-device UUID that identifies this plant.</summary>
    public string SyncId { get; set; } = string.Empty;

    /// <summary>Gets or sets the user-given nickname.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the species name.</summary>
    public string Species { get; set; } = string.Empty;

    /// <summary>Gets or sets the location within the home.</summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>Gets or sets the watering interval in days.</summary>
    public int WateringIntervalDays { get; set; }

    /// <summary>Gets or sets the last watered date, or null if never watered.</summary>
    public DateTime? LastWateredDate { get; set; }

    /// <summary>Gets or sets the UTC timestamp of the last local modification.</summary>
    public DateTime UpdatedAt { get; set; }
}
