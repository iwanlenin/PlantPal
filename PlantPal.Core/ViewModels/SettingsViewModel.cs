using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;

namespace PlantPal.Core.ViewModels;

/// <summary>
/// ViewModel for the Settings page. Exposes permission status and reminder time preference.
/// </summary>
public partial class SettingsViewModel : ObservableObject
{
    private readonly IPermissionService permissionService;

    /// <summary>Human-readable notification permission status, e.g. "✓ Granted" or "⚠ Denied".</summary>
    [ObservableProperty]
    private string notificationStatus = string.Empty;

    /// <summary>Human-readable photo permission status, e.g. "✓ Granted" or "⚠ Denied".</summary>
    [ObservableProperty]
    private string photoStatus = string.Empty;

    /// <summary>
    /// Daily reminder time. The page code-behind persists this value to Preferences
    /// because Preferences is a MAUI API not available in PlantPal.Core.
    /// </summary>
    [ObservableProperty]
    private TimeSpan reminderTime = new TimeSpan(9, 0, 0);

    /// <summary>
    /// Whether weather-aware watering is enabled. The page code-behind persists this to Preferences.
    /// </summary>
    [ObservableProperty]
    private bool isWeatherAwareEnabled;

    /// <summary>Initialises a new instance of <see cref="SettingsViewModel"/>.</summary>
    public SettingsViewModel(IPermissionService permissionService)
    {
        this.permissionService = permissionService;
    }

    /// <summary>Loads current permission statuses from the platform.</summary>
    public async Task LoadAsync()
    {
        var notificationResult = await this.permissionService.CheckNotificationPermissionAsync();
        this.NotificationStatus = this.FormatPermissionResult(notificationResult);

        var photoResult = await this.permissionService.CheckPhotoPermissionAsync();
        this.PhotoStatus = this.FormatPermissionResult(photoResult);
    }

    /// <summary>Opens the device Settings app so the user can manually grant permissions.</summary>
    [RelayCommand]
    private void OpenSettings()
    {
        // AppInfo.ShowSettingsUI() is called from the page code-behind to avoid MAUI deps in Core.
        // This command exists so the ViewModel owns the intent; the page invokes the platform API.
        this.OpenSettingsRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Raised when the user taps "Open Settings". The page subscribes and calls AppInfo.ShowSettingsUI().</summary>
    public event EventHandler? OpenSettingsRequested;

    // ── Private helpers ────────────────────────────────────────────────────────

    private string FormatPermissionResult(PermissionResult result) =>
        result == PermissionResult.Granted ? "✓ Granted" : "⚠ Denied";
}
