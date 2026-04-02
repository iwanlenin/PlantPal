using PlantPal.Pages;

namespace PlantPal;

/// <summary>
/// Application Shell — registers navigation routes.
/// </summary>
public partial class AppShell : Shell
{
    public AppShell()
    {
        this.InitializeComponent();
        Routing.RegisterRoute("AddPlant", typeof(AddPlantPage));
        Routing.RegisterRoute("PlantDetail", typeof(PlantDetailPage));
    }
}
