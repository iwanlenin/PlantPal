using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models;
using Plugin.LocalNotification.Core.Models.AndroidOption;
using PlantPal.Core.Interfaces;

namespace PlantPal.Services;

/// <summary>
/// Concrete <see cref="INotificationScheduler"/> implementation that delegates
/// to <c>Plugin.LocalNotification</c> for platform-level scheduling.
/// </summary>
public class MauiNotificationScheduler : INotificationScheduler
{
    /// <inheritdoc />
    public async Task ShowAsync(int id, string title, string body, DateTime notifyAt)
    {
        var request = new NotificationRequest
        {
            NotificationId = id,
            Title = title,
            Description = body,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = notifyAt,
                RepeatType = NotificationRepeat.No,
            },
            Android = new AndroidOptions
            {
                ChannelId = "plantpal_reminders",
                Priority = AndroidPriority.High,
            },
        };

        await LocalNotificationCenter.Current.Show(request);
    }

    /// <inheritdoc />
    public Task CancelAsync(int id)
    {
        LocalNotificationCenter.Current.Cancel(id);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task CancelAllAsync()
    {
        LocalNotificationCenter.Current.CancelAll();
        return Task.CompletedTask;
    }
}
