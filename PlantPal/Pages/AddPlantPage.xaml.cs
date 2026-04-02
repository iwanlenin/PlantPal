using PlantPal.Core.ViewModels;

namespace PlantPal.Pages;

/// <summary>
/// Page for adding a new plant or editing an existing one.
/// </summary>
[QueryProperty(nameof(PlantId), "plantId")]
public partial class AddPlantPage : ContentPage
{
    private readonly AddPlantViewModel viewModel;

    /// <summary>
    /// Initialises the add/edit plant page with its ViewModel.
    /// </summary>
    public AddPlantPage(AddPlantViewModel viewModel)
    {
        this.InitializeComponent();
        this.viewModel = viewModel;
        this.BindingContext = viewModel;
    }

    /// <summary>
    /// Shell navigation parameter for edit mode. When set, loads the plant for editing.
    /// </summary>
    public string? PlantId
    {
        set
        {
            if (int.TryParse(value, out var id))
            {
                _ = this.viewModel.LoadPlantAsync(id);
            }
        }
    }
}
