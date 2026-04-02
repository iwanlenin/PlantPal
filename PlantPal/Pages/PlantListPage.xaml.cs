using PlantPal.Core.ViewModels;

namespace PlantPal.Pages;

/// <summary>
/// Page showing the full list of all plants with swipe-to-delete and tap-to-detail.
/// </summary>
public partial class PlantListPage : ContentPage
{
    private readonly PlantListViewModel viewModel;

    /// <summary>
    /// Initialises the plant list page with its ViewModel.
    /// </summary>
    public PlantListPage(PlantListViewModel viewModel)
    {
        this.InitializeComponent();
        this.viewModel = viewModel;
        this.BindingContext = viewModel;
    }

    /// <inheritdoc />
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await this.viewModel.LoadPlantsCommand.ExecuteAsync(null);
    }

    /// <summary>Navigates to the Add Plant page when the FAB is tapped.</summary>
    private async void OnAddPlantTapped(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync("AddPlant");
}
