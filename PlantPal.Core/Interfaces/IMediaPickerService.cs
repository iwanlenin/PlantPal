using PlantPal.Core.Models;

namespace PlantPal.Core.Interfaces;

/// <summary>
/// Abstracts the platform media picker so ViewModels can request photos without MAUI dependencies.
/// Returns null when the user cancels or denies permission.
/// </summary>
public interface IMediaPickerService
{
    /// <summary>Opens the device photo gallery. Returns null if cancelled or permission denied.</summary>
    Task<PhotoResult?> PickPhotoAsync();

    /// <summary>Opens the device camera. Returns null if cancelled or permission denied.</summary>
    Task<PhotoResult?> CapturePhotoAsync();
}
