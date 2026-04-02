using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;
using PlantPal.Core.Services;
using PlantPal.Core.ViewModels;

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
        builder.Services.AddSingleton<INotificationService, StubNotificationService>();
        builder.Services.AddSingleton<IPermissionService, StubPermissionService>();
        builder.Services.AddSingleton<IImageCacheService, StubImageCacheService>();
        builder.Services.AddSingleton<IConnectivityService, StubConnectivityService>();
        builder.Services.AddSingleton<IPlantSpeciesService, PlantSpeciesService>();
        builder.Services.AddSingleton<INavigationService, StubNavigationService>();
        builder.Services.AddTransient<DashboardViewModel>();

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

    private sealed class StubNotificationService : INotificationService
    {
        public bool AreNotificationsEnabled => false;
        public Task ScheduleReminderAsync(Plant plant) => Task.CompletedTask;
        public Task CancelReminderAsync(int plantId) => Task.CompletedTask;
        public Task RescheduleAllAsync(List<Plant> plants) => Task.CompletedTask;
    }

    private sealed class StubPermissionService : IPermissionService
    {
        public Task<PermissionResult> CheckNotificationPermissionAsync() => Task.FromResult(PermissionResult.Denied);
        public Task<PermissionResult> RequestNotificationPermissionAsync() => Task.FromResult(PermissionResult.Denied);
        public Task<PermissionResult> CheckPhotoPermissionAsync() => Task.FromResult(PermissionResult.Denied);
        public Task<PermissionResult> RequestPhotoPermissionAsync() => Task.FromResult(PermissionResult.Denied);
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

private sealed class StubNavigationService : INavigationService
    {
        public Task NavigateToAsync(string route, Dictionary<string, object>? parameters = null) => Task.CompletedTask;
        public Task GoBackAsync() => Task.CompletedTask;
    }
}
