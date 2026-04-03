using CommunityToolkit.Maui;
using Plugin.LocalNotification;
using CoreInterfaces = PlantPal.Core.Interfaces;
using Microsoft.Extensions.Logging;
using PlantPal.Core.Interfaces;
using PlantPal.Core.Services;
using PlantPal.Core.ViewModels;
using PlantPal.Pages;
using PlantPal.Services;

namespace PlantPal;

/// <summary>
/// Entry point for the MAUI application. Configures the DI container and app services.
/// </summary>
public static class MauiProgram
{
    /// <summary>Creates and configures the <see cref="MauiApp"/> instance.</summary>
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseLocalNotification()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ── Service registrations ──────────────────────────────────────────────
        // Stub implementations registered here will be replaced phase by phase.
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "plantpal.db");
        builder.Services.AddSingleton<IPlantRepository>(new DatabaseService(dbPath));
        builder.Services.AddSingleton<IWateringLogRepository>(
            new WateringLogRepository(dbPath));
        builder.Services.AddSingleton<CoreInterfaces.IPermissionService, PermissionService>();
        builder.Services.AddSingleton<CoreInterfaces.INotificationScheduler, MauiNotificationScheduler>();
        builder.Services.AddSingleton<CoreInterfaces.IWeatherService, WeatherService>();
        builder.Services.AddSingleton<CoreInterfaces.INotificationService>(sp =>
            new NotificationService(
                sp.GetRequiredService<CoreInterfaces.IPermissionService>(),
                sp.GetRequiredService<CoreInterfaces.INotificationScheduler>(),
                sp.GetRequiredService<CoreInterfaces.IWeatherService>()));
        builder.Services.AddSingleton<IConnectivityService, ConnectivityService>();
        builder.Services.AddSingleton<CoreInterfaces.IHttpClientWrapper, HttpClientWrapper>();
        builder.Services.AddSingleton<IPlantSpeciesService, PlantSpeciesService>();
        builder.Services.AddSingleton<IImageCacheService>(sp =>
            new ImageCacheService(
                sp.GetRequiredService<IConnectivityService>(),
                sp.GetRequiredService<CoreInterfaces.IHttpClientWrapper>(),
                sp.GetRequiredService<IPlantSpeciesService>(),
                Path.Combine(FileSystem.CacheDirectory, "plant_images")));
        builder.Services.AddSingleton<INavigationService, ShellNavigationService>();
        builder.Services.AddSingleton<CoreInterfaces.ISecureStorageService, SecureStorageService>();
        builder.Services.AddSingleton<IAdvisorMessageRepository>(new AdvisorMessageRepository(dbPath));
        builder.Services.AddSingleton<CoreInterfaces.IPlantAdvisorService, PlantAdvisorService>();

        // ── ViewModels ────────────────────────────────────────────────────────
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<AddPlantViewModel>();
        builder.Services.AddTransient<PlantDetailViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<PlantListViewModel>();
        builder.Services.AddTransient<PlantAdvisorViewModel>();

        // ── Pages ─────────────────────────────────────────────────────────────
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<AddPlantPage>();
        builder.Services.AddTransient<PlantDetailPage>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<PlantListPage>();
        builder.Services.AddTransient<PlantAdvisorPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    // ── Stub implementations ───────────────────────────────────────────────────
    // These are temporary placeholders. Each will be replaced with a real
    // implementation in its dedicated phase.

    /// <summary>
    /// Real navigation service using Shell. Replaces StubNavigationService.
    /// </summary>
    private sealed class ShellNavigationService : INavigationService
    {
        public async Task NavigateToAsync(string route, Dictionary<string, object>? parameters = null)
        {
            if (parameters is not null)
            {
                await Shell.Current.GoToAsync(route, parameters);
            }
            else
            {
                await Shell.Current.GoToAsync(route);
            }
        }

        public async Task GoBackAsync() =>
            await Shell.Current.GoToAsync("..");
    }
}
