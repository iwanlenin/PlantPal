using PlantPal.Core.Models;

namespace PlantPal.Core.Interfaces;

/// <summary>
/// Defines the contract for scheduling and cancelling local plant watering reminders.
/// Implementations must never throw when notification permissions are denied — treat denial as a silent no-op.
/// </summary>
public interface INotificationService
{
    /// <summary>Gets a value indicating whether the user has granted notification permissions.</summary>
    bool AreNotificationsEnabled { get; }

    /// <summary>
    /// Schedules a local notification reminder for the specified plant's next watering date.
    /// Does nothing if notifications are disabled or <see cref="Plant.NextWaterDate"/> is null.
    /// </summary>
    /// <param name="plant">The plant for which to schedule a reminder.</param>
    Task ScheduleReminderAsync(Plant plant);

    /// <summary>Cancels the scheduled reminder for the specified plant. Has no effect if no reminder exists.</summary>
    /// <param name="plantId">The primary key of the plant whose reminder to cancel.</param>
    Task CancelReminderAsync(int plantId);

    /// <summary>Cancels all existing reminders and reschedules them for the provided list of plants.</summary>
    /// <param name="plants">The current list of plants to reschedule reminders for.</param>
    Task RescheduleAllAsync(List<Plant> plants);
}
