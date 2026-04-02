using PlantPal.Core.ViewModels;

namespace PlantPal.Pages;

/// <summary>
/// Dashboard page displaying plants that need water and upcoming waterings.
/// </summary>
public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel viewModel;

    /// <summary>
    /// Initialises the dashboard page with its ViewModel.
    /// </summary>
    public DashboardPage(DashboardViewModel viewModel)
    {
        this.InitializeComponent();
        this.viewModel = viewModel;
        this.BindingContext = viewModel;
    }

    /// <inheritdoc />
    /// <inheritdoc />
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await this.viewModel.LoadPlantsCommand.ExecuteAsync(null);
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
}
