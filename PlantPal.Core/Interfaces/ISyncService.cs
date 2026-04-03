using PlantPal.Core.Models;

namespace PlantPal.Core.Interfaces;

/// <summary>
/// Defines the contract for the offline-first cloud sync service.
/// All reads and writes go to SQLite first; sync reconciles local changes with Supabase.
/// </summary>
public interface ISyncService
{
    /// <summary>Gets the current sync state.</summary>
    SyncStatus Status { get; }

    /// <summary>Gets the UTC time of the last successful sync, or null if never synced.</summary>
    DateTime? LastSyncedAt { get; }

    /// <summary>Raised whenever <see cref="Status"/> changes, so ViewModels can update bindings.</summary>
    event EventHandler? StatusChanged;

    /// <summary>
    /// Pushes all local changes to Supabase.
    /// If offline, sets <see cref="Status"/> to <see cref="SyncStatus.Offline"/> and returns without throwing.
    /// </summary>
    Task SyncUpAsync();

    /// <summary>
    /// Pulls remote changes and merges them into local SQLite using last-write-wins on <c>UpdatedAt</c>.
    /// If offline, sets <see cref="Status"/> to <see cref="SyncStatus.Offline"/> and returns without throwing.
    /// </summary>
    Task SyncDownAsync();
}
