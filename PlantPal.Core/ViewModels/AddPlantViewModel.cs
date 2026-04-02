using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;

namespace PlantPal.Core.ViewModels;

/// <summary>
/// ViewModel for the Add/Edit Plant page. Handles species search, form
/// validation, save, and delete (edit mode only).
/// </summary>
public partial class AddPlantViewModel : ObservableObject
{
    private readonly IPlantRepository repository;
    private readonly IPlantSpeciesService speciesService;
    private readonly INavigationService navigationService;

    private int editPlantId;

    /// <summary>User-entered plant name.</summary>
    [ObservableProperty]
    private string name = string.Empty;

    /// <summary>Selected or entered species name.</summary>
    [ObservableProperty]
    private string species = string.Empty;

    /// <summary>Selected location within the home.</summary>
    [ObservableProperty]
    private string location = string.Empty;

    /// <summary>Watering frequency in days.</summary>
    [ObservableProperty]
    private int wateringIntervalDays = 7;

    /// <summary>Date the plant was last watered.</summary>
    [ObservableProperty]
    private DateTime lastWateredDate = DateTime.Today;

    /// <summary>Local file path to the user's photo of this plant.</summary>
    [ObservableProperty]
    private string? photoPath;

    /// <summary>Species search text for filtering.</summary>
    [ObservableProperty]
    private string searchText = string.Empty;

    /// <summary>Currently selected species from the filtered list.</summary>
    [ObservableProperty]
    private PlantSpecies? selectedSpecies;

    /// <summary>Filtered list of species matching the search text.</summary>
    [ObservableProperty]
    private ObservableCollection<PlantSpecies> filteredSpecies = [];

    /// <summary>Validation error for the Name field.</summary>
    [ObservableProperty]
    private string nameError = string.Empty;

    /// <summary>True when editing an existing plant.</summary>
    [ObservableProperty]
    private bool isEditMode;

    /// <summary>True while a save or delete operation is in progress.</summary>
    [ObservableProperty]
    private bool isLoading;

    /// <summary>
    /// Initialises a new instance of <see cref="AddPlantViewModel"/>.
    /// </summary>
    public AddPlantViewModel(
        IPlantRepository repository,
        IPlantSpeciesService speciesService,
        INavigationService navigationService)
    {
        this.repository = repository;
        this.speciesService = speciesService;
        this.navigationService = navigationService;
        this.FilteredSpecies = new ObservableCollection<PlantSpecies>(this.speciesService.GetAll());
    }

    /// <summary>
    /// Called when <see cref="SearchText"/> changes. Filters the species list.
    /// </summary>
    partial void OnSearchTextChanged(string value)
    {
        var all = this.speciesService.GetAll();
        if (string.IsNullOrWhiteSpace(value))
        {
            this.FilteredSpecies = new ObservableCollection<PlantSpecies>(all);
        }
        else
        {
            this.FilteredSpecies = new ObservableCollection<PlantSpecies>(
                all.Where(s => s.CommonName.Contains(value, StringComparison.OrdinalIgnoreCase)));
        }
    }

    /// <summary>
    /// Called when <see cref="SelectedSpecies"/> changes. Updates species name and interval.
    /// </summary>
    partial void OnSelectedSpeciesChanged(PlantSpecies? value)
    {
        if (value is null)
        {
            return;
        }

        this.Species = value.CommonName;
        this.WateringIntervalDays = value.WateringIntervalDays;
    }

    /// <summary>
    /// Loads an existing plant for editing by its ID.
    /// </summary>
    /// <param name="plantId">The plant's primary key.</param>
    public async Task LoadPlantAsync(int plantId)
    {
        var plant = await this.repository.GetByIdAsync(plantId);
        if (plant is null)
        {
            return;
        }

        this.editPlantId = plant.Id;
        this.Name = plant.Name;
        this.Species = plant.Species;
        this.Location = plant.Location;
        this.WateringIntervalDays = plant.WateringIntervalDays;
        this.LastWateredDate = plant.LastWateredDate ?? DateTime.Today;
        this.PhotoPath = plant.PhotoPath;
        this.IsEditMode = true;
    }

    /// <summary>
    /// Validates and saves the plant. Navigates back on success.
    /// </summary>
    [RelayCommand]
    public async Task SaveAsync()
    {
        this.NameError = string.Empty;

        if (string.IsNullOrWhiteSpace(this.Name))
        {
            this.NameError = "Plant name is required.";
            return;
        }

        this.IsLoading = true;
        try
        {
            var plant = new Plant
            {
                Id = this.IsEditMode ? this.editPlantId : 0,
                Name = this.Name.Trim(),
                Species = this.Species,
                Location = this.Location,
                WateringIntervalDays = this.WateringIntervalDays,
                LastWateredDate = this.LastWateredDate,
                PhotoPath = this.PhotoPath,
            };

            await this.repository.SaveAsync(plant);
            await this.navigationService.GoBackAsync();
        }
        finally
        {
            this.IsLoading = false;
        }
    }

    /// <summary>
    /// Deletes the plant being edited. Only available in edit mode.
    /// </summary>
    [RelayCommand]
    public async Task DeleteAsync()
    {
        if (!this.IsEditMode)
        {
            return;
        }

        this.IsLoading = true;
        try
        {
            await this.repository.DeleteAsync(this.editPlantId);
            await this.navigationService.GoBackAsync();
        }
        finally
        {
            this.IsLoading = false;
        }
    }
}
