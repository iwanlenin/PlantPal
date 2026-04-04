namespace PlantPal.Core.Models;

/// <summary>
/// Holds the raw bytes and MIME type of a photo selected from the gallery or captured by the camera.
/// </summary>
public class PhotoResult
{
    /// <summary>Gets the raw image bytes.</summary>
    public byte[] Bytes { get; init; } = [];

    /// <summary>Gets the MIME type, e.g. <c>image/jpeg</c> or <c>image/png</c>.</summary>
    public string MimeType { get; init; } = "image/jpeg";
}
