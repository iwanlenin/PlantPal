using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;

namespace PlantPal.Services;

/// <summary>
/// MAUI implementation of <see cref="IMediaPickerService"/>.
/// Uses <see cref="MediaPicker"/> to access the gallery and camera.
/// Returns null if the user cancels or permission is denied.
/// </summary>
public class MediaPickerService : IMediaPickerService
{
    /// <inheritdoc />
    public async Task<PhotoResult?> PickPhotoAsync()
    {
        try
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Choose a photo of your plant",
            });

            return result is null ? null : await this.ReadPhotoAsync(result);
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<PhotoResult?> CapturePhotoAsync()
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                return null;
            }

            var result = await MediaPicker.CapturePhotoAsync();
            return result is null ? null : await this.ReadPhotoAsync(result);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Reads a <see cref="FileResult"/> stream into a byte array and detects its MIME type.
    /// </summary>
    /// <remarks>
    /// <see cref="FileResult.ContentType"/> is not always populated reliably on Android and iOS,
    /// so MIME type is derived from the file extension instead. The bytes are read into a
    /// <see cref="MemoryStream"/> because the platform stream may not support seeking, and the
    /// Anthropic vision API requires the complete byte array for base64 encoding.
    /// </remarks>
    private async Task<PhotoResult> ReadPhotoAsync(FileResult file)
    {
        await using var stream = await file.OpenReadAsync();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        return new PhotoResult
        {
            Bytes = ms.ToArray(),
            MimeType = GetMimeType(file.FileName),
        };
    }

    /// <summary>
    /// Maps a file extension to its MIME type. Returns <c>image/jpeg</c> for any unrecognised extension.
    /// </summary>
    /// <remarks>
    /// JPEG is the default because Android and iOS cameras produce JPEG files in the vast majority
    /// of cases. The extension is lowercased before comparison so mixed-case names (.JPG, .Jpg)
    /// are handled correctly. GIF is included for completeness even though it is rarely used
    /// for plant photos; the Anthropic vision API accepts all four supported types.
    /// </remarks>
    private static string GetMimeType(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "image/jpeg",
        };
    }
}
