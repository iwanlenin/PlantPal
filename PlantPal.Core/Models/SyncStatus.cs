namespace PlantPal.Core.Models;

/// <summary>Represents the current state of the cloud sync service.</summary>
public enum SyncStatus
{
    /// <summary>No sync is in progress.</summary>
    Idle,

    /// <summary>A sync operation is currently running.</summary>
    Syncing,

    /// <summary>The last sync attempt failed.</summary>
    Error,

    /// <summary>The device has no internet connection; sync was skipped.</summary>
    Offline,
}
