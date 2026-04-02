using Android.App;
using Android.Runtime;
using Plugin.LocalNotification;

namespace PlantPal;

/// <summary>
/// Android application entry point. Creates the watering reminder notification channel
/// on startup so notifications can be delivered on Android 8+.
/// </summary>
[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override void OnCreate()
    {
        base.OnCreate();
        this.CreateNotificationChannel();
    }

    /// <summary>
    /// Creates the PlantPal watering reminder notification channel (Android 8+ requirement).
    /// High importance ensures the notification appears as a heads-up banner.
    /// </summary>
    private void CreateNotificationChannel()
    {
        if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.O)
        {
            return;
        }

        var channel = new NotificationChannel(
            "plantpal_reminders",
            "Watering Reminders",
            NotificationImportance.High)
        {
            Description = "Reminds you when your plants need watering.",
        };

        var manager = (NotificationManager?)this.GetSystemService(NotificationService)!;
        manager.CreateNotificationChannel(channel);
    }
}
