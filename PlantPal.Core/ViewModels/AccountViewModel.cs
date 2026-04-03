using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;

namespace PlantPal.Core.ViewModels;

/// <summary>
/// ViewModel for the Account page.
/// Manages sign-in/sign-out state and exposes manual sync controls.
/// </summary>
public partial class AccountViewModel : ObservableObject
{
    private readonly ISupabaseClient supabaseClient;
    private readonly ISyncService syncService;
    private readonly IConnectivityService connectivityService;

    /// <summary>Gets or sets a value indicating whether a user is signed in.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSignedOut))]
    private bool isSignedIn;

    /// <summary>Gets a value indicating whether no user is currently signed in.</summary>
    public bool IsSignedOut => !this.IsSignedIn;

    /// <summary>Gets or sets the signed-in user's email address.</summary>
    [ObservableProperty]
    private string userEmail = string.Empty;

    /// <summary>Gets or sets a value indicating whether a sync is currently in progress.</summary>
    [ObservableProperty]
    private bool isSyncing;

    /// <summary>Gets or sets a value indicating whether the last sync attempt failed.</summary>
    [ObservableProperty]
    private bool hasSyncError;

    /// <summary>Gets or sets a value indicating whether the device is offline.</summary>
    [ObservableProperty]
    private bool isOffline;

    /// <summary>Gets or sets the formatted last-synced time string, e.g. "Last synced: 14:32".</summary>
    [ObservableProperty]
    private string lastSyncedText = "Never synced";

    /// <summary>Gets or sets the email entered in the sign-in form.</summary>
    [ObservableProperty]
    private string emailInput = string.Empty;

    /// <summary>Gets or sets the password entered in the sign-in form.</summary>
    [ObservableProperty]
    private string passwordInput = string.Empty;

    /// <summary>Gets or sets the error message shown when sign-in fails.</summary>
    [ObservableProperty]
    private string signInError = string.Empty;

    /// <summary>Initialises the ViewModel with its required services.</summary>
    public AccountViewModel(
        ISupabaseClient supabaseClient,
        ISyncService syncService,
        IConnectivityService connectivityService)
    {
        this.supabaseClient = supabaseClient;
        this.syncService = syncService;
        this.connectivityService = connectivityService;

        this.syncService.StatusChanged += this.OnSyncStatusChanged;
    }

    /// <summary>Loads the current session state. Call from the page's OnAppearing.</summary>
    public void Refresh()
    {
        this.IsSignedIn = this.supabaseClient.IsSignedIn;
        this.UserEmail = this.supabaseClient.CurrentUserEmail ?? string.Empty;
        this.IsOffline = !this.connectivityService.IsConnected;
        this.UpdateLastSyncedText();
    }

    /// <summary>Signs in with the email and password entered in the form.</summary>
    [RelayCommand]
    private async Task SignInAsync()
    {
        this.SignInError = string.Empty;

        if (string.IsNullOrWhiteSpace(this.EmailInput) || string.IsNullOrWhiteSpace(this.PasswordInput))
        {
            this.SignInError = "Please enter your email and password.";
            return;
        }

        var success = await this.supabaseClient.SignInWithEmailAsync(this.EmailInput.Trim(), this.PasswordInput);
        if (success)
        {
            this.PasswordInput = string.Empty;
            this.Refresh();
            await this.ManualSyncAsync();
        }
        else
        {
            this.SignInError = "Sign in failed. Check your email and password.";
        }
    }

    /// <summary>Creates a new account with the email and password entered in the form.</summary>
    [RelayCommand]
    private async Task SignUpAsync()
    {
        this.SignInError = string.Empty;

        if (string.IsNullOrWhiteSpace(this.EmailInput) || string.IsNullOrWhiteSpace(this.PasswordInput))
        {
            this.SignInError = "Please enter your email and password.";
            return;
        }

        var success = await this.supabaseClient.SignUpWithEmailAsync(this.EmailInput.Trim(), this.PasswordInput);
        if (success)
        {
            this.SignInError = "Account created! Check your email to confirm, then sign in.";
            this.PasswordInput = string.Empty;
        }
        else
        {
            this.SignInError = "Sign up failed. This email may already be registered.";
        }
    }

    /// <summary>Signs out the current user.</summary>
    [RelayCommand]
    private async Task SignOutAsync()
    {
        await this.supabaseClient.SignOutAsync();
        this.Refresh();
    }

    /// <summary>Runs a full up+down sync immediately.</summary>
    [RelayCommand]
    private async Task ManualSyncAsync()
    {
        await this.syncService.SyncUpAsync();
        await this.syncService.SyncDownAsync();
    }

    private void OnSyncStatusChanged(object? sender, EventArgs e)
    {
        this.IsSyncing = this.syncService.Status == SyncStatus.Syncing;
        this.HasSyncError = this.syncService.Status == SyncStatus.Error;
        this.IsOffline = this.syncService.Status == SyncStatus.Offline
                         || !this.connectivityService.IsConnected;
        this.UpdateLastSyncedText();
    }

    private void UpdateLastSyncedText()
    {
        this.LastSyncedText = this.syncService.LastSyncedAt.HasValue
            ? $"Last synced: {this.syncService.LastSyncedAt.Value.ToLocalTime():HH:mm}"
            : "Never synced";
    }
}
