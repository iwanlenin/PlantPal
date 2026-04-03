using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;

namespace PlantPal.Core.ViewModels;

/// <summary>
/// ViewModel for the Plant Advisor chat page.
/// Manages conversation history, context window size, Claude API calls, and photo attachments.
/// </summary>
public partial class PlantAdvisorViewModel : ObservableObject
{
    /// <summary>Available context window sizes: number of messages sent to the API. 0 = All.</summary>
    public static readonly IReadOnlyList<int> ContextWindowOptions = [10, 20, 50, 0];

    private readonly IPlantAdvisorService advisorService;
    private readonly IAdvisorMessageRepository messageRepository;
    private readonly INavigationService navigationService;
    private readonly IMediaPickerService mediaPicker;

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

    /// <summary>Raw bytes of the photo attached to the next message. Null when no photo is attached.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasAttachedPhoto))]
    private byte[]? attachedImageBytes;

    /// <summary>MIME type of the attached photo.</summary>
    [ObservableProperty]
    private string attachedImageMimeType = "image/jpeg";

    /// <summary>Gets a value indicating whether a photo is currently attached to the pending message.</summary>
    public bool HasAttachedPhoto => this.AttachedImageBytes is { Length: > 0 };

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
        INavigationService navigationService,
        IMediaPickerService mediaPicker)
    {
        this.advisorService = advisorService;
        this.messageRepository = messageRepository;
        this.navigationService = navigationService;
        this.mediaPicker = mediaPicker;
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
    /// Sends <see cref="CurrentQuestion"/> (optionally with an attached photo) to Claude
    /// and appends both messages to the conversation.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanSend))]
    public async Task SendAsync()
    {
        var question = this.CurrentQuestion.Trim();

        // Require either a question or an attached photo before sending.
        if (string.IsNullOrEmpty(question) && !this.HasAttachedPhoto)
        {
            return;
        }

        var hasPhoto = this.HasAttachedPhoto;
        var imageBytes = this.AttachedImageBytes;
        var mimeType = this.AttachedImageMimeType;

        this.CurrentQuestion = string.Empty;
        this.AttachedImageBytes = null;
        this.IsLoading = true;

        // Store a human-readable version of the user message.
        var displayContent = hasPhoto
            ? (string.IsNullOrEmpty(question) ? "📷 Photo sent for diagnosis" : $"📷 {question}")
            : question;

        var userMessage = new AdvisorMessage
        {
            PlantId = this.PlantId,
            Role = "user",
            Content = displayContent,
            Timestamp = DateTime.UtcNow,
        };

        await this.messageRepository.SaveAsync(userMessage);
        this.Messages.Add(userMessage);

        try
        {
            var context = this.BuildContext();
            string response;

            if (hasPhoto && imageBytes is not null)
            {
                response = await this.advisorService.DiagnosePlantAsync(
                    this.Species, question, imageBytes, mimeType, context);
            }
            else
            {
                response = await this.advisorService.AskAboutPlantAsync(
                    this.Species, question, context);
            }

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

    /// <summary>Opens the photo gallery so the user can attach a photo to the next message.</summary>
    [RelayCommand]
    public async Task AttachPhotoFromGalleryAsync()
    {
        var photo = await this.mediaPicker.PickPhotoAsync();
        if (photo is not null)
        {
            this.AttachedImageBytes = photo.Bytes;
            this.AttachedImageMimeType = photo.MimeType;
        }
    }

    /// <summary>Opens the camera so the user can take a photo to attach to the next message.</summary>
    [RelayCommand]
    public async Task AttachPhotoFromCameraAsync()
    {
        var photo = await this.mediaPicker.CapturePhotoAsync();
        if (photo is not null)
        {
            this.AttachedImageBytes = photo.Bytes;
            this.AttachedImageMimeType = photo.MimeType;
        }
    }

    /// <summary>Removes the currently attached photo.</summary>
    [RelayCommand]
    public void RemovePhoto()
    {
        this.AttachedImageBytes = null;
        this.AttachedImageMimeType = "image/jpeg";
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
