using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;

namespace PlantPal.Core.Services;

/// <summary>
/// Schedules and cancels local plant watering reminders.
/// Permission checks are delegated to <see cref="IPermissionService"/>.
/// Actual platform scheduling is delegated to <see cref="INotificationScheduler"/>.
/// All methods are safe to call when notifications are denied — they silently no-op.
/// When <see cref="IWeatherService"/> is provided, outdoor plant reminders are postponed after rainfall.
/// </summary>
public class NotificationService : INotificationService
{
    private static readonly HashSet<string> OutdoorLocations =
        new(StringComparer.OrdinalIgnoreCase) { "Balcony", "Garden" };

    private readonly IPermissionService permissionService;
    private readonly INotificationScheduler scheduler;
    private readonly IWeatherService? weatherService;

    /// <inheritdoc />
    public bool AreNotificationsEnabled { get; private set; }

    /// <summary>
    /// Initialises a new instance of <see cref="NotificationService"/>.
    /// </summary>
    /// <param name="permissionService">The permission service used to check notification access.</param>
    /// <param name="scheduler">The platform scheduler used to show and cancel notifications.</param>
    /// <param name="weatherService">Optional weather service. When provided, outdoor plant reminders are weather-adjusted.</param>
    public NotificationService(
        IPermissionService permissionService,
        INotificationScheduler scheduler,
        IWeatherService? weatherService = null)
    {
        this.permissionService = permissionService;
        this.scheduler = scheduler;
        this.weatherService = weatherService;
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="plant"/> is null.</exception>
    public async Task ScheduleReminderAsync(Plant plant)
    {
        ArgumentNullException.ThrowIfNull(plant);

        var permission = await this.permissionService.CheckNotificationPermissionAsync();
        this.AreNotificationsEnabled = permission == PermissionResult.Granted;

        if (!this.AreNotificationsEnabled || plant.NextWaterDate is null)
        {
            return;
        }

        var notifyAt = plant.NextWaterDate.Value.Date.AddHours(9);
        var title = $"🌿 Time to water {plant.Name}!";
        var body = $"{plant.Species} in {plant.Location}";

        await this.scheduler.ShowAsync(plant.Id, title, body, notifyAt);
    }

    /// <inheritdoc />
    public async Task CancelReminderAsync(int plantId) =>
        await this.scheduler.CancelAsync(plantId);

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="plants"/> is null.</exception>
    public async Task RescheduleAllAsync(List<Plant> plants)
    {
        ArgumentNullException.ThrowIfNull(plants);

        await this.scheduler.CancelAllAsync();

        var permission = await this.permissionService.CheckNotificationPermissionAsync();
        this.AreNotificationsEnabled = permission == PermissionResult.Granted;

        if (!this.AreNotificationsEnabled)
        {
            return;
        }

        foreach (var plant in plants.Where(p => p.NextWaterDate.HasValue))
        {
            var notifyAt = plant.NextWaterDate!.Value.Date.AddHours(9);

            if (this.weatherService is not null && OutdoorLocations.Contains(plant.Location))
            {
                var postponeDays = await this.weatherService.GetPostponementDaysAsync();
                notifyAt = notifyAt.AddDays(postponeDays);
            }

            var title = $"🌿 Time to water {plant.Name}!";
            var body = $"{plant.Species} in {plant.Location}";
            await this.scheduler.ShowAsync(plant.Id, title, body, notifyAt);
        }
    }
}
