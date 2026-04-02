using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;

namespace PlantPal.Services;

/// <summary>
/// Wraps MAUI platform permission APIs. All permission interactions in the app
/// must go through <see cref="IPermissionService"/> — never call platform APIs directly.
/// </summary>
public class PermissionService : IPermissionService
{
    /// <inheritdoc />
    public async Task<PermissionResult> CheckNotificationPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
        return this.MapStatus(status);
    }

    /// <inheritdoc />
    public async Task<PermissionResult> RequestNotificationPermissionAsync()
    {
        var status = await Permissions.RequestAsync<Permissions.PostNotifications>();
        return this.MapStatus(status);
    }

    /// <inheritdoc />
    public async Task<PermissionResult> CheckPhotoPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Photos>();
        return this.MapStatus(status);
    }

    /// <inheritdoc />
    public async Task<PermissionResult> RequestPhotoPermissionAsync()
    {
        var status = await Permissions.RequestAsync<Permissions.Photos>();
        return this.MapStatus(status);
    }

    /// <summary>Maps a MAUI <see cref="PermissionStatus"/> to a <see cref="PermissionResult"/>.</summary>
    private PermissionResult MapStatus(PermissionStatus status) => status switch
    {
        PermissionStatus.Granted => PermissionResult.Granted,
        PermissionStatus.Restricted => PermissionResult.Granted,
        PermissionStatus.Limited => PermissionResult.Granted,
        PermissionStatus.Denied => PermissionResult.Denied,
        _ => PermissionResult.DeniedPermanently,
    };
}
