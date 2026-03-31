using PlantPal.Core.Models;

namespace PlantPal.Core.Interfaces;

/// <summary>
/// Defines the contract for checking and requesting platform permissions.
/// All permission interactions in the app must go through this interface — never call platform APIs directly.
/// </summary>
public interface IPermissionService
{
    /// <summary>Checks the current notification permission status without prompting the user.</summary>
    Task<PermissionResult> CheckNotificationPermissionAsync();

    /// <summary>Requests notification permission from the user. Returns the result of the request.</summary>
    Task<PermissionResult> RequestNotificationPermissionAsync();

    /// <summary>Checks the current photo library access permission status without prompting the user.</summary>
    Task<PermissionResult> CheckPhotoPermissionAsync();

    /// <summary>Requests photo library access permission from the user. Returns the result of the request.</summary>
    Task<PermissionResult> RequestPhotoPermissionAsync();
}
