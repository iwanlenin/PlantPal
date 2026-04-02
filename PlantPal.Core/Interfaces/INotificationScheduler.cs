namespace PlantPal.Core.Interfaces;

/// <summary>
/// Abstracts the platform notification scheduling API so that
/// <see cref="Services.NotificationService"/> can be unit-tested without MAUI dependencies.
/// </summary>
public interface INotificationScheduler
{
    /// <summary>
    /// Schedules a local notification to fire at the specified time.
    /// </summary>
    /// <param name="id">Unique notification identifier (used for cancellation).</param>
    /// <param name="title">Notification title.</param>
    /// <param name="body">Notification body text.</param>
    /// <param name="notifyAt">The date and time to deliver the notification.</param>
    Task ShowAsync(int id, string title, string body, DateTime notifyAt);

    /// <summary>
    /// Cancels the notification with the specified identifier. Has no effect if no such notification exists.
    /// </summary>
    /// <param name="id">The notification identifier to cancel.</param>
    Task CancelAsync(int id);

    /// <summary>Cancels all scheduled notifications.</summary>
    Task CancelAllAsync();
}
