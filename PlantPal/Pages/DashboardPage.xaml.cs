using PlantPal.Core.Interfaces;
using PlantPal.Core.ViewModels;

namespace PlantPal.Pages;

/// <summary>
/// Dashboard page displaying plants that need water and upcoming waterings.
/// </summary>
public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel viewModel;
    private readonly ISyncService syncService;

    /// <summary>
    /// Initialises the dashboard page with its ViewModel and sync service.
    /// </summary>
    public DashboardPage(DashboardViewModel viewModel, ISyncService syncService)
    {
        this.InitializeComponent();
        this.viewModel = viewModel;
        this.syncService = syncService;
        this.BindingContext = viewModel;
    }

    /// <inheritdoc />
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        this.viewModel.IsWeatherAwareEnabled = Preferences.Get("weather_watering", false);
        await this.viewModel.LoadPlantsCommand.ExecuteAsync(null);

        // Subscribe to connectivity changes for auto-sync.
        Connectivity.ConnectivityChanged += this.OnConnectivityChanged;

        // Attempt background sync each time the dashboard becomes visible.
        _ = this.TryBackgroundSyncAsync();
    }

    /// <inheritdoc />
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Connectivity.ConnectivityChanged -= this.OnConnectivityChanged;
    }

    /// <summary>
    /// Opens device Settings so the user can grant notification permission manually.
    /// Called when the user taps the notification permission banner.
    /// </summary>
    private void OnNotificationBannerTapped(object? sender, TappedEventArgs e) =>
        AppInfo.ShowSettingsUI();

    /// <summary>
    /// Animates the "Mark as Watered" button: scales up, turns confirmed-green, then restores.
    /// </summary>
    private async void OnWaterNowClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button) return;

        var originalBrush = button.Background;
        var originalText = button.Text;

        await button.ScaleTo(1.1, 100);
        button.Background = new SolidColorBrush(Color.FromArgb("#4CAF50"));
        button.Text = "✓ Done";

        await Task.Delay(800);

        button.Text = originalText;
        button.Background = originalBrush;
        await button.ScaleTo(1.0, 100);
    }

    /// <summary>Triggers a sync when the device regains internet connectivity.</summary>
    private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        if (e.NetworkAccess == NetworkAccess.Internet)
        {
            _ = this.TryBackgroundSyncAsync();
        }
    }

    /// <summary>Runs a full sync in the background, silently ignoring errors.</summary>
    private async Task TryBackgroundSyncAsync()
    {
        try
        {
            await this.syncService.SyncUpAsync();
            await this.syncService.SyncDownAsync();
        }
        catch
        {
            // Background sync must never crash the UI.
        }
    }
}
