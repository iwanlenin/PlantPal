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

    /// <summary>Reads the stream from a <see cref="FileResult"/> into a byte array.</summary>
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

    /// <summary>Returns the MIME type based on file extension. Defaults to image/jpeg.</summary>
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
