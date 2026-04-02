using PlantPal.Core.ViewModels;

namespace PlantPal.Pages;

/// <summary>
/// Dashboard page displaying plants that need water and upcoming waterings.
/// </summary>
public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel viewModel;

    /// <summary>
    /// Initialises the dashboard page with its ViewModel.
    /// </summary>
    public DashboardPage(DashboardViewModel viewModel)
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
}
