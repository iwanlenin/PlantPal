using CommunityToolkit.Maui;
using Plugin.LocalNotification;
using CoreInterfaces = PlantPal.Core.Interfaces;
using Microsoft.Extensions.Logging;
using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;
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
        builder.Services.AddSingleton<IWateringLogRepository, StubWateringLogRepository>();
        builder.Services.AddSingleton<CoreInterfaces.IPermissionService, PermissionService>();
        builder.Services.AddSingleton<CoreInterfaces.INotificationScheduler, MauiNotificationScheduler>();
        builder.Services.AddSingleton<CoreInterfaces.INotificationService, NotificationService>();
        builder.Services.AddSingleton<IImageCacheService, StubImageCacheService>();
        builder.Services.AddSingleton<IConnectivityService, StubConnectivityService>();
        builder.Services.AddSingleton<IPlantSpeciesService, PlantSpeciesService>();
        builder.Services.AddSingleton<INavigationService, ShellNavigationService>();

        // ── ViewModels ────────────────────────────────────────────────────────
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<AddPlantViewModel>();

        // ── Pages ─────────────────────────────────────────────────────────────
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<AddPlantPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    // ── Stub implementations ───────────────────────────────────────────────────
    // These are temporary placeholders. Each will be replaced with a real
    // implementation in its dedicated phase.

    private sealed class StubWateringLogRepository : IWateringLogRepository
    {
        public Task<List<WateringLog>> GetByPlantIdAsync(int plantId) => Task.FromResult(new List<WateringLog>());
        public Task SaveAsync(WateringLog log) => Task.CompletedTask;
        public Task DeleteByPlantIdAsync(int plantId) => Task.CompletedTask;
    }

    private sealed class StubImageCacheService : IImageCacheService
    {
        public Task<string> GetThumbnailPathAsync(string speciesKey) => Task.FromResult(string.Empty);
        public Task<string> GetDetailImageAsync(string speciesKey) => Task.FromResult(string.Empty);
    }

    private sealed class StubConnectivityService : IConnectivityService
    {
        public bool IsConnected => false;
    }

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
