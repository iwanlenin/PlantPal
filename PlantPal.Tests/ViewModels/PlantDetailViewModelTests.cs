using NSubstitute;
using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;
using PlantPal.Core.ViewModels;

namespace PlantPal.Tests.ViewModels;

/// <summary>
/// Unit tests for <see cref="PlantDetailViewModel"/> using NSubstitute mocks.
/// </summary>
public class PlantDetailViewModelTests
{
    private readonly IPlantRepository plantRepository = Substitute.For<IPlantRepository>();
    private readonly IWateringLogRepository wateringLogRepository = Substitute.For<IWateringLogRepository>();
    private readonly IImageCacheService imageCacheService = Substitute.For<IImageCacheService>();
    private readonly INavigationService navigationService = Substitute.For<INavigationService>();

    private PlantDetailViewModel CreateViewModel() =>
        new(this.plantRepository, this.wateringLogRepository, this.imageCacheService, this.navigationService);

    private static Plant CreatePlant(int id = 1) => new()
    {
        Id = id,
        Name = "Monty",
        Species = "monstera_deliciosa",
        Location = "Living Room",
        WateringIntervalDays = 7,
        LastWateredDate = DateTime.Today.AddDays(-7),
        NextWaterDate = DateTime.Today,
    };

    // ── Positive cases ────────────────────────────────────────────────────────

    [Fact]
    public async Task LoadAsync_SetsPlant_From_Repository()
    {
        var plant = CreatePlant();
        this.plantRepository.GetByIdAsync(1).Returns(plant);
        this.wateringLogRepository.GetByPlantIdAsync(1).Returns([]);
        this.imageCacheService.GetDetailImageAsync(Arg.Any<string>()).Returns("plants/fallback.png");
        var vm = this.CreateViewModel();

        await vm.LoadAsync(1);

        Assert.Equal(plant, vm.Plant);
    }

    [Fact]
    public async Task LoadAsync_Calls_GetDetailImageAsync_With_SpeciesKey()
    {
        var plant = CreatePlant();
        this.plantRepository.GetByIdAsync(1).Returns(plant);
        this.wateringLogRepository.GetByPlantIdAsync(1).Returns([]);
        this.imageCacheService.GetDetailImageAsync(Arg.Any<string>()).Returns("/cache/image.jpg");
        var vm = this.CreateViewModel();

        await vm.LoadAsync(1);

        await this.imageCacheService.Received(1).GetDetailImageAsync("monstera_deliciosa");
        Assert.Equal("/cache/image.jpg", vm.PlantDetailImage);
    }

    [Fact]
    public async Task LoadAsync_WhenImagePathIsAssetPath_SetsIsShowingFallbackImage_True()
    {
        var plant = CreatePlant();
        this.plantRepository.GetByIdAsync(1).Returns(plant);
        this.wateringLogRepository.GetByPlantIdAsync(1).Returns([]);
        this.imageCacheService.GetDetailImageAsync(Arg.Any<string>())
            .Returns("plants/monstera_deliciosa_thumb.png");
        var vm = this.CreateViewModel();

        await vm.LoadAsync(1);

        Assert.True(vm.IsShowingFallbackImage);
    }

    [Fact]
    public async Task WaterNowAsync_Inserts_WateringLog_And_Updates_LastWateredDate()
    {
        var plant = CreatePlant();
        this.plantRepository.GetByIdAsync(1).Returns(plant);
        this.wateringLogRepository.GetByPlantIdAsync(1).Returns([]);
        this.imageCacheService.GetDetailImageAsync(Arg.Any<string>()).Returns("plants/thumb.png");
        var vm = this.CreateViewModel();
        await vm.LoadAsync(1);

        await vm.WaterNowCommand.ExecuteAsync(null);

        await this.wateringLogRepository.Received(1).SaveAsync(
            Arg.Is<WateringLog>(l => l.PlantId == plant.Id));
        await this.plantRepository.Received(1).SaveAsync(
            Arg.Is<Plant>(p => p.Id == plant.Id && p.LastWateredDate!.Value.Date == DateTime.Today));
    }

    [Fact]
    public async Task DeleteAsync_Calls_PlantRepository_And_WateringLogRepository()
    {
        var plant = CreatePlant();
        this.plantRepository.GetByIdAsync(1).Returns(plant);
        this.wateringLogRepository.GetByPlantIdAsync(1).Returns([]);
        this.imageCacheService.GetDetailImageAsync(Arg.Any<string>()).Returns("plants/thumb.png");
        var vm = this.CreateViewModel();
        await vm.LoadAsync(1);

        await vm.DeleteCommand.ExecuteAsync(null);

        await this.plantRepository.Received(1).DeleteAsync(plant.Id);
        await this.wateringLogRepository.Received(1).DeleteByPlantIdAsync(plant.Id);
    }
}
