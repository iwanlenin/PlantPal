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

    /// <summary>Plants that are due for watering today or overdue.</summary>
    [ObservableProperty]
    private ObservableCollection<Plant> dueTodayPlants = [];

    /// <summary>Plants due for watering within the next 7 days (but not today).</summary>
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
    /// Sets <see cref="HasError"/> if the repository throws.
    /// </summary>
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
    /// Records that the specified plant was watered today, saves it, and reschedules
    /// its notification. Notification failures are swallowed — saving always completes.
    /// Does nothing if <paramref name="plant"/> is null.
    /// </summary>
    /// <param name="plant">The plant that was watered.</param>
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
            "PlantDetail", new Dictionary<string, object> { ["plantId"] = plant.Id });
    }
}
