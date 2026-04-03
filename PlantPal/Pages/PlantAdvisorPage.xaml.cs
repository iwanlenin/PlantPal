using PlantPal.Core.ViewModels;

namespace PlantPal.Pages;

/// <summary>
/// Code-behind for the Plant Advisor chat page.
/// Handles API key entry dialog, quick question chips, context window taps, and auto-scroll.
/// </summary>
[QueryProperty(nameof(PlantId), "plantId")]
[QueryProperty(nameof(Species), "species")]
[QueryProperty(nameof(PlantName), "plantName")]
public partial class PlantAdvisorPage : ContentPage
{
    private readonly PlantAdvisorViewModel viewModel;

    /// <summary>Initialises the page with its ViewModel.</summary>
    public PlantAdvisorPage(PlantAdvisorViewModel viewModel)
    {
        this.InitializeComponent();
        this.viewModel = viewModel;
        this.BindingContext = viewModel;

        this.viewModel.Messages.CollectionChanged += this.OnMessagesChanged;
    }

    /// <summary>Shell query parameter — sets the plant ID on the ViewModel.</summary>
    public string? PlantId
    {
        set
        {
            if (int.TryParse(value, out var id))
            {
                this.viewModel.PlantId = id;
            }
        }
    }

    /// <summary>Shell query parameter — sets the species on the ViewModel.</summary>
    public string? Species
    {
        set => this.viewModel.Species = value ?? string.Empty;
    }

    /// <summary>Shell query parameter — sets the plant name on the ViewModel.</summary>
    public string? PlantName
    {
        set => this.viewModel.PlantName = value ?? string.Empty;
    }

    /// <inheritdoc />
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await this.viewModel.LoadCommand.ExecuteAsync(null);
    }

    /// <summary>Prompts the user to enter their Anthropic API key and saves it.</summary>
    private async void OnAddApiKeyTapped(object? sender, TappedEventArgs e)
    {
        var key = await this.DisplayPromptAsync(
            "Anthropic API Key",
            "Enter your API key. It is stored securely in the device keychain and never leaves your device.",
            placeholder: "sk-ant-...",
            maxLength: 200);

        if (!string.IsNullOrWhiteSpace(key))
        {
            await this.viewModel.SetApiKeyCommand.ExecuteAsync(key);
        }
    }

    /// <summary>Sets the context window size from the tapped chip's CommandParameter.</summary>
    private void OnContextWindowTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is string param && int.TryParse(param, out var size))
        {
            this.viewModel.ContextWindowSize = size;
        }
    }

    /// <summary>Fills the input field with a quick question and sends it.</summary>
    private async void OnQuickQuestionTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is string question)
        {
            this.viewModel.CurrentQuestion = question;
            await this.viewModel.SendCommand.ExecuteAsync(null);
        }
    }

    /// <summary>Scrolls to the last message whenever a new one is added.</summary>
    private void OnMessagesChanged(
        object? sender,
        System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (this.viewModel.Messages.Count > 0)
        {
            this.ChatCollectionView.ScrollTo(
                this.viewModel.Messages[^1],
                animate: true);
        }
    }
}
