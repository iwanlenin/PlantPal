using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;

namespace PlantPal.Core.ViewModels;

/// <summary>
/// ViewModel for the plant detail page. Loads the plant, its watering history,
/// and its cached detail image. Supports watering and deletion.
/// </summary>
public partial class PlantDetailViewModel : ObservableObject
{
    private readonly IPlantRepository plantRepository;
    private readonly IWateringLogRepository wateringLogRepository;
    private readonly IImageCacheService imageCacheService;
    private readonly INavigationService navigationService;

    /// <summary>Initialises the ViewModel with its required services.</summary>
    public PlantDetailViewModel(
        IPlantRepository plantRepository,
        IWateringLogRepository wateringLogRepository,
        IImageCacheService imageCacheService,
        INavigationService navigationService)
    {
        this.plantRepository = plantRepository;
        this.wateringLogRepository = wateringLogRepository;
        this.imageCacheService = imageCacheService;
        this.navigationService = navigationService;
    }

    /// <summary>Gets or sets the plant being displayed.</summary>
    [ObservableProperty]
    private Plant? plant;

    /// <summary>Gets or sets the local file path or asset path for the detail image.</summary>
    [ObservableProperty]
    private string plantDetailImage = string.Empty;

    /// <summary>Gets or sets a value indicating whether the hero image is a bundled fallback (offline or not cached).</summary>
    [ObservableProperty]
    private bool isShowingFallbackImage;

    /// <summary>Gets or sets a value indicating whether an async operation is in progress.</summary>
    [ObservableProperty]
    private bool isLoading;

    /// <summary>Gets the watering history for the current plant, newest first.</summary>
    public ObservableCollection<WateringLog> WateringHistory { get; } = [];

    /// <summary>Loads the plant, its watering history, and its detail image.</summary>
    /// <param name="plantId">The primary key of the plant to load.</param>
    public async Task LoadAsync(int plantId)
    {
        this.IsLoading = true;
        try
        {
            this.Plant = await this.plantRepository.GetByIdAsync(plantId);
            if (this.Plant is null)
            {
                return;
            }

            var logs = await this.wateringLogRepository.GetByPlantIdAsync(plantId);
            this.WateringHistory.Clear();
            foreach (var log in logs)
            {
                this.WateringHistory.Add(log);
            }

            var imagePath = await this.imageCacheService.GetDetailImageAsync(this.Plant.Species);
            this.PlantDetailImage = imagePath;
            this.IsShowingFallbackImage = imagePath.StartsWith("plants/", StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            this.IsLoading = false;
        }
    }

    /// <summary>Records a watering event and refreshes the history list.</summary>
    [RelayCommand]
    public async Task WaterNowAsync()
    {
        if (this.Plant is null)
        {
            return;
        }

        var log = new WateringLog
        {
            PlantId = this.Plant.Id,
            WateredAt = DateTime.UtcNow,
        };
        await this.wateringLogRepository.SaveAsync(log);

        this.Plant.LastWateredDate = DateTime.Today;
        this.Plant.RecalculateNextWaterDate();
        await this.plantRepository.SaveAsync(this.Plant);

        var logs = await this.wateringLogRepository.GetByPlantIdAsync(this.Plant.Id);
        this.WateringHistory.Clear();
        foreach (var l in logs)
        {
            this.WateringHistory.Add(l);
        }
    }

    /// <summary>Navigates to the edit form for the current plant.</summary>
    [RelayCommand]
    public async Task EditAsync()
    {
        if (this.Plant is null)
        {
            return;
        }

        await this.navigationService.NavigateToAsync(
            "AddPlant", new Dictionary<string, object> { ["plantId"] = this.Plant.Id.ToString() });
    }

    /// <summary>Deletes the plant and all its watering logs, then navigates back.</summary>
    [RelayCommand]
    public async Task DeleteAsync()
    {
        if (this.Plant is null)
        {
            return;
        }

        await this.plantRepository.DeleteAsync(this.Plant.Id);
        await this.wateringLogRepository.DeleteByPlantIdAsync(this.Plant.Id);
        await this.navigationService.GoBackAsync();
    }
}
