namespace PlantPal.Core.Models;

/// <summary>
/// Represents a single message in a plant advisor conversation.
/// Stored in SQLite; role is either "user" or "assistant".
/// </summary>
public class AdvisorMessage
{
    /// <summary>Gets or sets the unique identifier (SQLite primary key).</summary>
    [SQLite.PrimaryKey]
    [SQLite.AutoIncrement]
    public int Id { get; set; }

    /// <summary>Gets or sets the ID of the plant this message belongs to.</summary>
    [SQLite.Indexed]
    public int PlantId { get; set; }

    /// <summary>Gets or sets the message role: "user" or "assistant".</summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>Gets or sets the message text content.</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>Gets or sets the UTC timestamp when the message was created.</summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
