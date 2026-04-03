using System.ComponentModel;
using PlantPal.Core.ViewModels;

namespace PlantPal.Pages;

/// <summary>
/// Settings page for managing permissions and app preferences.
/// </summary>
public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel viewModel;

    /// <summary>
    /// Initialises the settings page with its ViewModel.
    /// </summary>
    public SettingsPage(SettingsViewModel viewModel)
    {
        this.InitializeComponent();
        this.viewModel = viewModel;
        this.BindingContext = viewModel;
        viewModel.OpenSettingsRequested += (_, _) => AppInfo.ShowSettingsUI();
    }

    /// <inheritdoc />
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Restore saved reminder time from Preferences before loading status labels.
        var savedMinutes = Preferences.Get("reminder_time", 9.0 * 60);
        this.viewModel.ReminderTime = TimeSpan.FromMinutes(savedMinutes);

        // Restore weather-aware watering toggle.
        this.viewModel.IsWeatherAwareEnabled = Preferences.Get("weather_watering", false);

        await this.viewModel.LoadAsync();
    }

    /// <summary>Handles taps on either "Open Settings" button.</summary>
    private void OnOpenSettingsTapped(object? sender, EventArgs e) =>
        AppInfo.ShowSettingsUI();

    /// <summary>Persists the reminder time to Preferences whenever the TimePicker value changes.</summary>
    private void OnReminderTimeChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(TimePicker.Time)) return;
        Preferences.Set("reminder_time", this.viewModel.ReminderTime.TotalMinutes);
    }

    /// <summary>Persists the weather-aware watering toggle to Preferences when it changes.</summary>
    private void OnWeatherToggled(object? sender, ToggledEventArgs e) =>
        Preferences.Set("weather_watering", e.Value);
}
