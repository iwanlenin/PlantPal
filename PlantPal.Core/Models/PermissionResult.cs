namespace PlantPal.Core.Models;

/// <summary>
/// Represents the outcome of a platform permission check or request.
/// </summary>
public enum PermissionResult
{
    /// <summary>The permission has been granted by the user.</summary>
    Granted,

    /// <summary>The permission has been denied by the user. The app may request it again.</summary>
    Denied,

    /// <summary>
    /// The permission has been permanently denied. The OS will no longer show a request dialog —
    /// the user must navigate to device Settings to grant it manually.
    /// </summary>
    DeniedPermanently
}
