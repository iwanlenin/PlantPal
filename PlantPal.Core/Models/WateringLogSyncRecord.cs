namespace PlantPal.Core.Models;

/// <summary>
/// Data-transfer object used to exchange watering log data with Supabase.
/// Maps to the <c>watering_logs</c> table schema.
/// </summary>
public class WateringLogSyncRecord
{
    /// <summary>Gets or sets the stable cross-device UUID that identifies this log entry.</summary>
    public string SyncId { get; set; } = string.Empty;

    /// <summary>Gets or sets the <see cref="PlantSyncRecord.SyncId"/> of the plant that was watered.</summary>
    public string PlantSyncId { get; set; } = string.Empty;

    /// <summary>Gets or sets the UTC date and time when the plant was watered.</summary>
    public DateTime WateredAt { get; set; }

    /// <summary>Gets or sets the UTC timestamp of the last modification.</summary>
    public DateTime UpdatedAt { get; set; }
}
