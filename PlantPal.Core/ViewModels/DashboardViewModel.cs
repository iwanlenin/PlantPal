using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;

namespace PlantPal.Core.ViewModels;

/// <summary>
/// ViewModel for the dashboard screen. Exposes due and upcoming plants,
/// and commands for watering, adding, and navigating to plant detail.
/// </summary>
public partial class DashboardViewModel : ObservableObject
{
    private readonly IPlantRepository repository;
    private readonly INotificationService notificationService;
    private readonly INavigationService navigationService;
    private readonly IPermissionService permissionService;

    /// <summary>
    /// Plants whose <see cref="Plant.NextWaterDate"/> is today or in the past (overdue).
    /// Displayed at the top of the dashboard as the primary call-to-action list.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<Plant> dueTodayPlants = [];

    /// <summary>
    /// Plants due for watering within the next 7 days but not yet overdue.
    /// Gives the user a planning view so they can prepare for upcoming care.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<Plant> upcomingPlants = [];

    /// <summary>True when both due and upcoming collections are empty.</summary>
    [ObservableProperty]
    private bool isEmpty;

    /// <summary>True when the last load operation encountered an error.</summary>
    [ObservableProperty]
    private bool hasError;

    /// <summary>True while an async operation is in progress.</summary>
    [ObservableProperty]
    private bool isLoading;

    /// <summary>True when the app should display a notification permission banner.</summary>
    [ObservableProperty]
    private bool showNotificationBanner;

    /// <summary>
    /// True when weather-aware watering is enabled. Set by the page code-behind from Preferences
    /// before <see cref="LoadPlantsCommand"/> executes. Outdoor plants are marked
    /// <see cref="Plant.IsWeatherAdjusted"/> accordingly in <see cref="LoadPlantsAsync"/>.
    /// </summary>
    [ObservableProperty]
    private bool isWeatherAwareEnabled;

    /// <summary>
    /// Initialises a new instance of <see cref="DashboardViewModel"/>.
    /// </summary>
    /// <param name="repository">The plant data repository.</param>
    /// <param name="notificationService">The notification scheduling service.</param>
    /// <param name="navigationService">The navigation service.</param>
    /// <param name="permissionService">The permission service.</param>
    public DashboardViewModel(
        IPlantRepository repository,
        INotificationService notificationService,
        INavigationService navigationService,
        IPermissionService permissionService)
    {
        this.repository = repository;
        this.notificationService = notificationService;
        this.navigationService = navigationService;
        this.permissionService = permissionService;
    }

    /// <summary>
    /// Loads all plants from the repository and categorises them into
    /// <see cref="DueTodayPlants"/> and <see cref="UpcomingPlants"/>.
    /// </summary>
    /// <remarks>
    /// Plants with no <see cref="Plant.NextWaterDate"/> (never watered) are excluded from
    /// both lists — they haven't been assigned a schedule yet so there's nothing to remind about.
    /// If <see cref="IsWeatherAwareEnabled"/> is true, outdoor plants (Balcony/Garden) are
    /// flagged with <see cref="Plant.IsWeatherAdjusted"/> = true. This is a UI hint only;
    /// the actual postponement is applied by <see cref="INotificationService.RescheduleAllAsync"/>.
    /// Sets <see cref="HasError"/> if the repository throws; the exception is not re-thrown.
    /// </remarks>
    [RelayCommand]
    public async Task LoadPlantsAsync()
    {
        this.IsLoading = true;
        this.HasError = false;

        try
        {
            var permission = await this.permissionService.CheckNotificationPermissionAsync();
            this.ShowNotificationBanner = permission != PermissionResult.Granted;

            var plants = await this.repository.GetAllAsync();
            var today = DateTime.Today;
            var windowEnd = today.AddDays(7);

            if (this.IsWeatherAwareEnabled)
            {
                foreach (var p in plants.Where(p =>
                    string.Equals(p.Location, "Balcony", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(p.Location, "Garden", StringComparison.OrdinalIgnoreCase)))
                {
                    p.IsWeatherAdjusted = true;
                }
            }

            this.DueTodayPlants = new ObservableCollection<Plant>(
                plants.Where(p => p.NextWaterDate.HasValue && p.NextWaterDate.Value.Date <= today));

            this.UpcomingPlants = new ObservableCollection<Plant>(
                plants.Where(p => p.NextWaterDate.HasValue
                    && p.NextWaterDate.Value.Date > today
                    && p.NextWaterDate.Value.Date <= windowEnd));

            this.IsEmpty = this.DueTodayPlants.Count == 0 && this.UpcomingPlants.Count == 0;
        }
        catch
        {
            this.HasError = true;
        }
        finally
        {
            this.IsLoading = false;
        }
    }

    /// <summary>
    /// Records that <paramref name="plant"/> was watered today, persists it, and reschedules
    /// its next reminder. Safe to call when notification permission is denied — the save
    /// always completes regardless of whether the notification can be scheduled.
    /// </summary>
    /// <param name="plant">The plant that was watered. Null is ignored.</param>
    /// <remarks>
    /// The notification failure is intentionally swallowed: a denied notification permission
    /// is a valid app state and must never block the user from logging a watering.
    /// The dashboard is not automatically reloaded after this call — the plant moves off the
    /// due list only when <see cref="LoadPlantsCommand"/> is executed again (e.g. on next appear).
    /// </remarks>
    [RelayCommand]
    public async Task WaterNowAsync(Plant? plant)
    {
        if (plant is null)
        {
            return;
        }

        plant.LastWateredDate = DateTime.Today;
        plant.RecalculateNextWaterDate();
        await this.repository.SaveAsync(plant);

        try
        {
            await this.notificationService.ScheduleReminderAsync(plant);
        }
        catch
        {
            // Notification failure must never prevent the save from succeeding.
        }
    }

    /// <summary>Navigates to the Add Plant page.</summary>
    [RelayCommand]
    public async Task AddPlantAsync() =>
        await this.navigationService.NavigateToAsync("AddPlant");

    /// <summary>Navigates to the detail page for the specified plant.</summary>
    /// <param name="plant">The plant to open.</param>
    [RelayCommand]
    public async Task OpenPlantAsync(Plant? plant)
    {
        if (plant is null)
        {
            return;
        }

        await this.navigationService.NavigateToAsync(
            "PlantDetail", new Dictionary<string, object> { ["plantId"] = plant.Id.ToString() });
    }
}
