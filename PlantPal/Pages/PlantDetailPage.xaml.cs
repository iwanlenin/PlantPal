using PlantPal.Core.ViewModels;

namespace PlantPal.Pages;

/// <summary>
/// Plant detail page — shows the hero image, care info, watering history, and action buttons.
/// </summary>
[QueryProperty(nameof(PlantId), "plantId")]
public partial class PlantDetailPage : ContentPage
{
    private readonly PlantDetailViewModel viewModel;

    /// <summary>Initialises the plant detail page with its ViewModel.</summary>
    public PlantDetailPage(PlantDetailViewModel viewModel)
    {
        this.InitializeComponent();
        this.viewModel = viewModel;
        this.BindingContext = viewModel;
    }

    /// <summary>
    /// Shell navigation parameter. When set, loads the plant for display.
    /// </summary>
    public string? PlantId
    {
        set
        {
            if (int.TryParse(value, out var id))
            {
                _ = this.viewModel.LoadAsync(id);
            }
        }
    }

    /// <summary>Navigates to the Plant Advisor page for the current plant.</summary>
    private async void OnAskClaudeTapped(object? sender, TappedEventArgs e)
    {
        if (this.viewModel.Plant is null)
        {
            return;
        }

        await Shell.Current.GoToAsync("PlantAdvisor", new Dictionary<string, object>
        {
            ["plantId"] = this.viewModel.Plant.Id.ToString(),
            ["species"] = this.viewModel.Plant.Species,
            ["plantName"] = this.viewModel.Plant.Name,
        });
    }
}
