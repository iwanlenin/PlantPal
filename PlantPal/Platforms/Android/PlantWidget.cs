using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;
using PlantPal.Core.Services;

namespace PlantPal.Platforms.Android;

/// <summary>
/// Android home screen widget that displays the count of plants due for watering today.
/// Updates hourly via the system alarm. Accesses SQLite directly — cannot use MAUI DI.
/// </summary>
[BroadcastReceiver(
    Name = "com.companyname.plantpal.PlantWidget",
    Label = "PlantPal Widget",
    Exported = true)]
[IntentFilter(new[] { AppWidgetManager.ActionAppwidgetUpdate })]
[MetaData(AppWidgetManager.MetaDataAppwidgetProvider,
          Resource = "@xml/plant_widget_info")]
public class PlantWidget : AppWidgetProvider
{
    /// <inheritdoc />
    public override void OnUpdate(
        Context? context,
        AppWidgetManager? appWidgetManager,
        int[]? appWidgetIds)
    {
        if (context is null || appWidgetManager is null || appWidgetIds is null)
        {
            return;
        }

        foreach (var widgetId in appWidgetIds)
        {
            UpdateWidget(context, appWidgetManager, widgetId);
        }
    }

    /// <summary>Queries the database and refreshes a single widget instance.</summary>
    /// <param name="context">Android context.</param>
    /// <param name="manager">AppWidget manager for applying RemoteViews.</param>
    /// <param name="widgetId">The ID of the widget instance to update.</param>
    internal static void UpdateWidget(Context context, AppWidgetManager manager, int widgetId)
    {
        var dbPath = System.IO.Path.Combine(
            context.FilesDir!.AbsolutePath,
            "plantpal.db");

        int count;
        try
        {
            count = WidgetDbQuery.CountDuePlantsAsync(dbPath).GetAwaiter().GetResult();
        }
        catch
        {
            count = 0;
        }

        var views = new RemoteViews(context.PackageName!, Resource.Layout.plant_widget);

        if (count > 0)
        {
            views.SetTextViewText(Resource.Id.widget_count, count.ToString());
            views.SetTextViewText(Resource.Id.widget_label, "plants need water today");
        }
        else
        {
            views.SetTextViewText(Resource.Id.widget_count, "🌿");
            views.SetTextViewText(Resource.Id.widget_label, "All plants happy");
        }

        // Tap opens the app.
        var intent = new Intent(context, typeof(MainActivity));
        intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop);
        var pendingIntent = PendingIntent.GetActivity(
            context,
            0,
            intent,
            PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
        views.SetOnClickPendingIntent(Resource.Layout.plant_widget, pendingIntent);

        manager.UpdateAppWidget(widgetId, views);
    }
}
