using PlantPal.Core.Models;

namespace PlantPal.Core.Interfaces;

/// <summary>
/// Defines the contract for communicating with the Supabase backend.
/// Abstracts authentication and data access so sync logic is testable without network calls.
/// </summary>
public interface ISupabaseClient
{
    /// <summary>Gets a value indicating whether a user is currently signed in.</summary>
    bool IsSignedIn { get; }

    /// <summary>Gets the Supabase user ID of the signed-in user, or null if not signed in.</summary>
    string? CurrentUserId { get; }

    /// <summary>Gets the email address of the signed-in user, or null if not signed in.</summary>
    string? CurrentUserEmail { get; }

    /// <summary>Signs in with the given email and password. Returns true on success.</summary>
    Task<bool> SignInWithEmailAsync(string email, string password);

    /// <summary>Creates a new account with the given email and password. Returns true on success.</summary>
    Task<bool> SignUpWithEmailAsync(string email, string password);

    /// <summary>Signs out the current user and clears stored credentials.</summary>
    Task SignOutAsync();

    /// <summary>Upserts the given plant records to the remote <c>plants</c> table.</summary>
    Task UpsertPlantsAsync(IList<PlantSyncRecord> plants);

    /// <summary>Upserts the given watering log records to the remote <c>watering_logs</c> table.</summary>
    Task UpsertWateringLogsAsync(IList<WateringLogSyncRecord> logs);

    /// <summary>Returns all plant records owned by the signed-in user from the remote table.</summary>
    Task<IList<PlantSyncRecord>> FetchPlantsAsync();

    /// <summary>Returns all watering log records owned by the signed-in user from the remote table.</summary>
    Task<IList<WateringLogSyncRecord>> FetchWateringLogsAsync();
}
