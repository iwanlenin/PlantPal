using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;

namespace PlantPal.Core.ViewModels;

/// <summary>
/// ViewModel for the All Plants list page. Exposes all plants with commands to navigate and delete.
/// </summary>
public partial class PlantListViewModel : ObservableObject
{
    private readonly IPlantRepository plantRepository;
    private readonly INavigationService navigationService;

    /// <summary>All plants in the database.</summary>
    [ObservableProperty]
    private ObservableCollection<Plant> plants = new();

    /// <summary>Initialises a new instance of <see cref="PlantListViewModel"/>.</summary>
    public PlantListViewModel(IPlantRepository plantRepository, INavigationService navigationService)
    {
        this.plantRepository = plantRepository;
        this.navigationService = navigationService;
    }

    /// <summary>Loads all plants from the repository into <see cref="Plants"/>.</summary>
    [RelayCommand]
    public async Task LoadPlantsAsync()
    {
        var all = await this.plantRepository.GetAllAsync();
        this.Plants = new ObservableCollection<Plant>(all);
    }

    /// <summary>Navigates to the detail page for the given plant.</summary>
    [RelayCommand]
    public async Task OpenPlantAsync(Plant plant)
    {
        await this.navigationService.NavigateToAsync(
            "PlantDetail",
            new Dictionary<string, object> { ["plantId"] = plant.Id.ToString() });
    }

    /// <summary>Deletes the given plant from the repository and removes it from the list.</summary>
    [RelayCommand]
    public async Task DeletePlantAsync(Plant plant)
    {
        await this.plantRepository.DeleteAsync(plant.Id);
        this.Plants.Remove(plant);
    }
}
