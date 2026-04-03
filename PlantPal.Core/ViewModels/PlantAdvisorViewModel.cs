using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;

namespace PlantPal.Core.ViewModels;

/// <summary>
/// ViewModel for the Plant Advisor chat page.
/// Manages conversation history, context window size, and Claude API calls.
/// </summary>
public partial class PlantAdvisorViewModel : ObservableObject
{
    /// <summary>Available context window sizes: number of messages sent to the API. 0 = All.</summary>
    public static readonly IReadOnlyList<int> ContextWindowOptions = [10, 20, 50, 0];

    private readonly IPlantAdvisorService advisorService;
    private readonly IAdvisorMessageRepository messageRepository;
    private readonly INavigationService navigationService;

    /// <summary>The plant ID this conversation belongs to. Set via Shell query parameter.</summary>
    [ObservableProperty]
    private int plantId;

    /// <summary>The plant's species name, included in the Claude system prompt. Set via Shell query parameter.</summary>
    [ObservableProperty]
    private string species = string.Empty;

    /// <summary>The plant's display name, shown in the page header. Set via Shell query parameter.</summary>
    [ObservableProperty]
    private string plantName = string.Empty;

    /// <summary>The full conversation history loaded from the database.</summary>
    [ObservableProperty]
    private ObservableCollection<AdvisorMessage> messages = [];

    /// <summary>The text the user has typed in the input field.</summary>
    [ObservableProperty]
    private string currentQuestion = string.Empty;

    /// <summary>True while an API call is in progress.</summary>
    [ObservableProperty]
    private bool isLoading;

    /// <summary>True when the service has a valid API key stored.</summary>
    [ObservableProperty]
    private bool isConfigured;

    /// <summary>
    /// Number of most-recent messages sent as context to the API.
    /// 0 means all messages are sent.
    /// </summary>
    [ObservableProperty]
    private int contextWindowSize = 10;

    /// <summary>True when <see cref="ContextWindowSize"/> is 0 (send all history).</summary>
    public bool IsFullContextWarningVisible => this.ContextWindowSize == 0;

    /// <summary>True when <see cref="ContextWindowSize"/> is greater than 10 (cost hint visible).</summary>
    public bool IsCostHintVisible => this.ContextWindowSize != 10;

    /// <summary>
    /// Initialises a new instance of <see cref="PlantAdvisorViewModel"/>.
    /// </summary>
    public PlantAdvisorViewModel(
        IPlantAdvisorService advisorService,
        IAdvisorMessageRepository messageRepository,
        INavigationService navigationService)
    {
        this.advisorService = advisorService;
        this.messageRepository = messageRepository;
        this.navigationService = navigationService;
    }

    partial void OnContextWindowSizeChanged(int value)
    {
        this.OnPropertyChanged(nameof(this.IsFullContextWarningVisible));
        this.OnPropertyChanged(nameof(this.IsCostHintVisible));
    }

    /// <summary>
    /// Loads the conversation history for the current plant and refreshes <see cref="IsConfigured"/>.
    /// </summary>
    [RelayCommand]
    public async Task LoadAsync()
    {
        await this.advisorService.InitialiseAsync();
        this.IsConfigured = this.advisorService.IsConfigured;

        var history = await this.messageRepository.GetByPlantIdAsync(this.PlantId);
        this.Messages = new ObservableCollection<AdvisorMessage>(history);
    }

    /// <summary>
    /// Sends <see cref="CurrentQuestion"/> to Claude and appends both messages to the conversation.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanSend))]
    public async Task SendAsync()
    {
        var question = this.CurrentQuestion.Trim();
        if (string.IsNullOrEmpty(question))
        {
            return;
        }

        this.CurrentQuestion = string.Empty;
        this.IsLoading = true;

        var userMessage = new AdvisorMessage
        {
            PlantId = this.PlantId,
            Role = "user",
            Content = question,
            Timestamp = DateTime.UtcNow,
        };

        await this.messageRepository.SaveAsync(userMessage);
        this.Messages.Add(userMessage);

        try
        {
            var context = this.BuildContext();
            var response = await this.advisorService.AskAboutPlantAsync(
                this.Species, question, context);

            var assistantMessage = new AdvisorMessage
            {
                PlantId = this.PlantId,
                Role = "assistant",
                Content = response,
                Timestamp = DateTime.UtcNow,
            };

            await this.messageRepository.SaveAsync(assistantMessage);
            this.Messages.Add(assistantMessage);
        }
        finally
        {
            this.IsLoading = false;
        }
    }

    /// <summary>Stores the API key and refreshes <see cref="IsConfigured"/>.</summary>
    /// <param name="key">The Anthropic API key entered by the user.</param>
    [RelayCommand]
    public async Task SetApiKeyAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        await this.advisorService.SetApiKeyAsync(key.Trim());
        this.IsConfigured = true;
    }

    /// <summary>Deletes all messages for the current plant from the database and clears the UI.</summary>
    [RelayCommand]
    public async Task ClearHistoryAsync()
    {
        await this.messageRepository.DeleteByPlantIdAsync(this.PlantId);
        this.Messages.Clear();
    }

    private bool CanSend() => !this.IsLoading;

    /// <summary>Returns the context slice to send to the API based on <see cref="ContextWindowSize"/>.</summary>
    private IList<AdvisorMessage> BuildContext()
    {
        if (this.ContextWindowSize == 0 || this.Messages.Count <= this.ContextWindowSize)
        {
            return this.Messages.ToList();
        }

        return this.Messages
            .Skip(this.Messages.Count - this.ContextWindowSize)
            .ToList();
    }
}
