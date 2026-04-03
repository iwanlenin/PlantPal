using PlantPal.Core.ViewModels;

namespace PlantPal.Pages;

/// <summary>
/// Account page — shows sign-in / sign-out state and manual sync controls.
/// </summary>
public partial class AccountPage : ContentPage
{
    private readonly AccountViewModel viewModel;

    /// <summary>Initialises the account page with its ViewModel.</summary>
    public AccountPage(AccountViewModel viewModel)
    {
        this.InitializeComponent();
        this.viewModel = viewModel;
        this.BindingContext = viewModel;
    }

    /// <inheritdoc />
    protected override void OnAppearing()
    {
        base.OnAppearing();
        this.viewModel.Refresh();
    }
}
