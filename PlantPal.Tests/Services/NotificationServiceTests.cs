using NSubstitute;
using NSubstitute.ExceptionExtensions;
using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;
using PlantPal.Core.Services;

namespace PlantPal.Tests.Services;

/// <summary>
/// Unit tests for <see cref="NotificationService"/> using NSubstitute mocks.
/// All MAUI scheduling is abstracted behind <see cref="INotificationScheduler"/>.
/// </summary>
public class NotificationServiceTests
{
    private readonly IPermissionService permissionService = Substitute.For<IPermissionService>();
    private readonly INotificationScheduler scheduler = Substitute.For<INotificationScheduler>();

    private NotificationService CreateService() =>
        new(this.permissionService, this.scheduler);

    private static Plant CreatePlant(DateTime? nextWaterDate = null) => new()
    {
        Id = 1,
        Name = "Monty",
        Species = "Monstera",
        Location = "Living Room",
        WateringIntervalDays = 7,
        NextWaterDate = nextWaterDate,
    };

    // ── Positive cases ─────────────────────────────────────────────────────────

    [Fact]
    public async Task ScheduleReminderAsync_NotificationsDisabled_Does_Not_Schedule()
    {
        this.permissionService.CheckNotificationPermissionAsync()
            .Returns(Task.FromResult(PermissionResult.Denied));
        var svc = this.CreateService();
        var plant = CreatePlant(DateTime.Today.AddDays(3));

        await svc.ScheduleReminderAsync(plant);

        await this.scheduler.DidNotReceive().ShowAsync(
            Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
    }

    [Fact]
    public async Task ScheduleReminderAsync_NullNextWaterDate_Does_Not_Schedule()
    {
        this.permissionService.CheckNotificationPermissionAsync()
            .Returns(Task.FromResult(PermissionResult.Granted));
        var svc = this.CreateService();
        var plant = CreatePlant(nextWaterDate: null);

        await svc.ScheduleReminderAsync(plant);

        await this.scheduler.DidNotReceive().ShowAsync(
            Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
    }

    [Fact]
    public async Task ScheduleReminderAsync_PermissionGranted_Schedules_At_9am()
    {
        this.permissionService.CheckNotificationPermissionAsync()
            .Returns(Task.FromResult(PermissionResult.Granted));
        var svc = this.CreateService();
        var waterDate = DateTime.Today.AddDays(3);
        var plant = CreatePlant(waterDate);

        await svc.ScheduleReminderAsync(plant);

        await this.scheduler.Received(1).ShowAsync(
            plant.Id,
            Arg.Any<string>(),
            Arg.Any<string>(),
            waterDate.Date.AddHours(9));
    }

    [Fact]
    public async Task CancelReminderAsync_Calls_Scheduler_Cancel()
    {
        var svc = this.CreateService();

        await svc.CancelReminderAsync(42);

        await this.scheduler.Received(1).CancelAsync(42);
    }

    [Fact]
    public async Task CancelReminderAsync_NonexistentId_Does_Not_Throw()
    {
        this.scheduler.CancelAsync(Arg.Any<int>()).Returns(Task.CompletedTask);
        var svc = this.CreateService();

        var ex = await Record.ExceptionAsync(() => svc.CancelReminderAsync(99999));

        Assert.Null(ex);
    }

    [Fact]
    public async Task RescheduleAllAsync_EmptyList_Calls_CancelAll_Only()
    {
        var svc = this.CreateService();

        await svc.RescheduleAllAsync([]);

        await this.scheduler.Received(1).CancelAllAsync();
        await this.scheduler.DidNotReceive().ShowAsync(
            Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
    }

    // ── Negative cases ─────────────────────────────────────────────────────────

    [Fact]
    public async Task ScheduleReminderAsync_NullPlant_Throws_ArgumentNullException()
    {
        var svc = this.CreateService();

        await Assert.ThrowsAsync<ArgumentNullException>(() => svc.ScheduleReminderAsync(null!));
    }

    [Fact]
    public async Task RescheduleAllAsync_NullList_Throws_ArgumentNullException()
    {
        var svc = this.CreateService();

        await Assert.ThrowsAsync<ArgumentNullException>(() => svc.RescheduleAllAsync(null!));
    }
}
