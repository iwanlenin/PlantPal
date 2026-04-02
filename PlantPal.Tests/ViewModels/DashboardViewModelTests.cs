using NSubstitute;
using NSubstitute.ExceptionExtensions;
using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;
using PlantPal.Core.ViewModels;

namespace PlantPal.Tests.ViewModels;

/// <summary>
/// Unit tests for <see cref="DashboardViewModel"/> using NSubstitute mocks.
/// No database, no MAUI — pure logic tests.
/// </summary>
public class DashboardViewModelTests
{
    private readonly IPlantRepository repository = Substitute.For<IPlantRepository>();
    private readonly INotificationService notificationService = Substitute.For<INotificationService>();
    private readonly INavigationService navigationService = Substitute.For<INavigationService>();
    private readonly IPermissionService permissionService = Substitute.For<IPermissionService>();

    private DashboardViewModel CreateViewModel() =>
        new(this.repository, this.notificationService, this.navigationService, this.permissionService);

    private static Plant CreatePlant(int id, DateTime? nextWaterDate) => new()
    {
        Id = id,
        Name = $"Plant {id}",
        Species = "Monstera",
        WateringIntervalDays = 7,
        LastWateredDate = nextWaterDate?.AddDays(-7),
        NextWaterDate = nextWaterDate,
    };

    // ── Positive cases ─────────────────────────────────────────────────────────

    [Fact]
    public async Task LoadPlantsAsync_Calls_Repository_Once()
    {
        this.repository.GetAllAsync().Returns([]);
        var vm = this.CreateViewModel();

        await vm.LoadPlantsCommand.ExecuteAsync(null);

        await this.repository.Received(1).GetAllAsync();
    }

    [Fact]
    public async Task LoadPlantsAsync_DueToday_Plant_Appears_In_DueTodayPlants()
    {
        var today = DateTime.Today;
        var plant = CreatePlant(1, today);
        this.repository.GetAllAsync().Returns([plant]);
        var vm = this.CreateViewModel();

        await vm.LoadPlantsCommand.ExecuteAsync(null);

        Assert.Single(vm.DueTodayPlants);
        Assert.Equal(plant.Id, vm.DueTodayPlants[0].Id);
    }

    [Fact]
    public async Task LoadPlantsAsync_Overdue_Plant_Appears_In_DueTodayPlants()
    {
        var yesterday = DateTime.Today.AddDays(-1);
        var plant = CreatePlant(1, yesterday);
        this.repository.GetAllAsync().Returns([plant]);
        var vm = this.CreateViewModel();

        await vm.LoadPlantsCommand.ExecuteAsync(null);

        Assert.Single(vm.DueTodayPlants);
    }

    [Fact]
    public async Task LoadPlantsAsync_Upcoming_Plant_Appears_In_UpcomingPlants()
    {
        var inThreeDays = DateTime.Today.AddDays(3);
        var plant = CreatePlant(1, inThreeDays);
        this.repository.GetAllAsync().Returns([plant]);
        var vm = this.CreateViewModel();

        await vm.LoadPlantsCommand.ExecuteAsync(null);

        Assert.Single(vm.UpcomingPlants);
        Assert.Equal(plant.Id, vm.UpcomingPlants[0].Id);
    }

    [Fact]
    public async Task LoadPlantsAsync_FarFuture_Plant_Appears_In_Neither_Collection()
    {
        var inTenDays = DateTime.Today.AddDays(10);
        var plant = CreatePlant(1, inTenDays);
        this.repository.GetAllAsync().Returns([plant]);
        var vm = this.CreateViewModel();

        await vm.LoadPlantsCommand.ExecuteAsync(null);

        Assert.Empty(vm.DueTodayPlants);
        Assert.Empty(vm.UpcomingPlants);
    }

    [Fact]
    public async Task WaterNowAsync_Saves_Plant_With_Updated_LastWateredDate()
    {
        var plant = CreatePlant(1, DateTime.Today);
        this.repository.GetAllAsync().Returns([plant]);
        var vm = this.CreateViewModel();

        await vm.WaterNowCommand.ExecuteAsync(plant);

        await this.repository.Received(1).SaveAsync(
            Arg.Is<Plant>(p => p.Id == plant.Id && p.LastWateredDate!.Value.Date == DateTime.Today));
    }

    [Fact]
    public async Task WaterNowAsync_Calls_ScheduleReminderAsync()
    {
        var plant = CreatePlant(1, DateTime.Today);
        var vm = this.CreateViewModel();

        await vm.WaterNowCommand.ExecuteAsync(plant);

        await this.notificationService.Received(1).ScheduleReminderAsync(
            Arg.Is<Plant>(p => p.Id == plant.Id));
    }

    [Fact]
    public async Task LoadPlantsAsync_EmptyRepository_Sets_IsEmpty_True()
    {
        this.repository.GetAllAsync().Returns([]);
        var vm = this.CreateViewModel();

        await vm.LoadPlantsCommand.ExecuteAsync(null);

        Assert.True(vm.IsEmpty);
    }

    [Fact]
    public async Task LoadPlantsAsync_WithPlants_Sets_IsEmpty_False()
    {
        var plant = CreatePlant(1, DateTime.Today);
        this.repository.GetAllAsync().Returns([plant]);
        var vm = this.CreateViewModel();

        await vm.LoadPlantsCommand.ExecuteAsync(null);

        Assert.False(vm.IsEmpty);
    }

    [Fact]
    public async Task OpenPlantAsync_Navigates_To_AddPlant_With_String_PlantId()
    {
        // Regression: Shell.GoToAsync throws InvalidCastException if plantId is int, not string.
        var plant = CreatePlant(42, DateTime.Today);
        var vm = this.CreateViewModel();

        await vm.OpenPlantCommand.ExecuteAsync(plant);

        await this.navigationService.Received(1).NavigateToAsync(
            "PlantDetail",
            Arg.Is<Dictionary<string, object>>(d =>
                d.ContainsKey("plantId") && d["plantId"] is string && (string)d["plantId"] == "42"));
    }

    [Fact]
    public async Task OpenPlantAsync_NullPlant_Does_Not_Navigate()
    {
        var vm = this.CreateViewModel();

        await vm.OpenPlantCommand.ExecuteAsync(null);

        await this.navigationService.DidNotReceiveWithAnyArgs().NavigateToAsync(default!, default);
    }

    // ── Negative cases ─────────────────────────────────────────────────────────

    [Fact]
    public async Task WaterNowAsync_NullPlant_Does_Not_Throw()
    {
        var vm = this.CreateViewModel();

        var exception = await Record.ExceptionAsync(() => vm.WaterNowCommand.ExecuteAsync(null));

        Assert.Null(exception);
    }

    [Fact]
    public async Task LoadPlantsAsync_RepositoryThrows_Sets_HasError_True()
    {
        this.repository.GetAllAsync().ThrowsAsync(new Exception("DB error"));
        var vm = this.CreateViewModel();

        await vm.LoadPlantsCommand.ExecuteAsync(null);

        Assert.True(vm.HasError);
    }

    [Fact]
    public async Task WaterNowAsync_NotificationPermissionDenied_Still_Saves_Plant()
    {
        var plant = CreatePlant(1, DateTime.Today);
        this.notificationService.ScheduleReminderAsync(Arg.Any<Plant>())
            .ThrowsAsync(new Exception("Permission denied"));
        var vm = this.CreateViewModel();

        await vm.WaterNowCommand.ExecuteAsync(plant);

        await this.repository.Received(1).SaveAsync(Arg.Is<Plant>(p => p.Id == plant.Id));
    }
}
