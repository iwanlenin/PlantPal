using PlantPal.Core.Interfaces;
using PlantPal.Core.Models;

namespace PlantPal.Core.Services;

/// <summary>
/// Offline-first cloud sync service that reconciles local SQLite data with Supabase.
/// All reads and writes happen locally first; sync is a background reconciliation step.
/// Conflict resolution uses last-write-wins on <see cref="Plant.UpdatedAt"/> /
/// <see cref="WateringLog.UpdatedAt"/>.
/// </summary>
public class SyncService : ISyncService
{
    private readonly IPlantRepository plantRepository;
    private readonly IWateringLogRepository wateringLogRepository;
    private readonly IConnectivityService connectivityService;
    private readonly ISupabaseClient supabaseClient;

    /// <summary>
    /// Initialises the sync service with its required dependencies.
    /// </summary>
    public SyncService(
        IPlantRepository plantRepository,
        IWateringLogRepository wateringLogRepository,
        IConnectivityService connectivityService,
        ISupabaseClient supabaseClient)
    {
        this.plantRepository = plantRepository;
        this.wateringLogRepository = wateringLogRepository;
        this.connectivityService = connectivityService;
        this.supabaseClient = supabaseClient;
    }

    /// <inheritdoc />
    public SyncStatus Status { get; private set; } = SyncStatus.Idle;

    /// <inheritdoc />
    public DateTime? LastSyncedAt { get; private set; }

    /// <inheritdoc />
    public event EventHandler? StatusChanged;

    /// <inheritdoc />
    public async Task SyncUpAsync()
    {
        if (!this.connectivityService.IsConnected)
        {
            this.SetStatus(SyncStatus.Offline);
            return;
        }

        if (!this.supabaseClient.IsSignedIn)
        {
            return;
        }

        this.SetStatus(SyncStatus.Syncing);

        try
        {
            var plants = await this.plantRepository.GetAllAsync();

            // Assign SyncId to any plant that doesn't have one yet.
            // This can happen for plants created before Phase 13 (pre-sync migration)
            // or for plants created offline that have not been saved since the upgrade.
            // SyncId must exist before upload so Supabase can use it as the upsert key
            // (on_conflict=sync_id). Without it, the upsert would create duplicate rows.
            foreach (var plant in plants.Where(p => string.IsNullOrEmpty(p.SyncId)))
            {
                plant.SyncId = Guid.NewGuid().ToString();
                await this.plantRepository.SaveAsync(plant);
            }

            var plantRecords = plants.Select(p => new PlantSyncRecord
            {
                SyncId = p.SyncId,
                Name = p.Name,
                Species = p.Species,
                Location = p.Location,
                WateringIntervalDays = p.WateringIntervalDays,
                LastWateredDate = p.LastWateredDate,
                UpdatedAt = p.UpdatedAt == default ? DateTime.UtcNow : p.UpdatedAt,
            }).ToList();

            await this.supabaseClient.UpsertPlantsAsync(plantRecords);

            var logs = await this.wateringLogRepository.GetAllAsync();
            var plantSyncIdByLocalId = plants.ToDictionary(p => p.Id, p => p.SyncId);

            var logRecords = logs
                .Where(l => plantSyncIdByLocalId.ContainsKey(l.PlantId))
                .Select(l => new WateringLogSyncRecord
                {
                    SyncId = string.IsNullOrEmpty(l.SyncId) ? Guid.NewGuid().ToString() : l.SyncId,
                    PlantSyncId = plantSyncIdByLocalId[l.PlantId],
                    WateredAt = l.WateredAt,
                    UpdatedAt = l.UpdatedAt == default ? l.WateredAt : l.UpdatedAt,
                }).ToList();

            await this.supabaseClient.UpsertWateringLogsAsync(logRecords);

            this.LastSyncedAt = DateTime.UtcNow;
            this.SetStatus(SyncStatus.Idle);
        }
        catch
        {
            this.SetStatus(SyncStatus.Error);
        }
    }

    /// <inheritdoc />
    public async Task SyncDownAsync()
    {
        if (!this.connectivityService.IsConnected)
        {
            this.SetStatus(SyncStatus.Offline);
            return;
        }

        if (!this.supabaseClient.IsSignedIn)
        {
            return;
        }

        this.SetStatus(SyncStatus.Syncing);

        try
        {
            var remotePlants = await this.supabaseClient.FetchPlantsAsync();
            var localPlants = await this.plantRepository.GetAllAsync();
            var localBySyncId = localPlants
                .Where(p => !string.IsNullOrEmpty(p.SyncId))
                .ToDictionary(p => p.SyncId);

            foreach (var remote in remotePlants)
            {
                if (localBySyncId.TryGetValue(remote.SyncId, out var local))
                {
                    // Last-write-wins conflict resolution: only overwrite the local record if
                    // the remote version is strictly newer. This protects local edits made
                    // while offline — if the user edited a plant on this device after the last
                    // sync, local.UpdatedAt will be newer than remote.UpdatedAt, and the local
                    // version is kept. The next SyncUpAsync will push the local version up.
                    // This assumes all device clocks are in sync with UTC; clock skew could
                    // cause the wrong version to win, but this is acceptable for a personal app.
                    if (remote.UpdatedAt > local.UpdatedAt)
                    {
                        local.Name = remote.Name;
                        local.Species = remote.Species;
                        local.Location = remote.Location;
                        local.WateringIntervalDays = remote.WateringIntervalDays;
                        local.LastWateredDate = remote.LastWateredDate;
                        local.UpdatedAt = remote.UpdatedAt;
                        await this.plantRepository.SaveAsync(local);
                    }
                }
                else
                {
                    // No local plant has this SyncId — it was created on another device.
                    // Since there is no conflicting record to merge, we adopt the remote
                    // data as-is. The remote's UpdatedAt is trusted because it came from
                    // Supabase, which enforces per-user RLS (only the owner's records are returned).
                    var newPlant = new Plant
                    {
                        SyncId = remote.SyncId,
                        Name = remote.Name,
                        Species = remote.Species,
                        Location = remote.Location,
                        WateringIntervalDays = remote.WateringIntervalDays,
                        LastWateredDate = remote.LastWateredDate,
                        UpdatedAt = remote.UpdatedAt,
                    };
                    await this.plantRepository.SaveAsync(newPlant);
                }
            }

            var remoteLogs = await this.supabaseClient.FetchWateringLogsAsync();
            var refreshedPlants = await this.plantRepository.GetAllAsync();
            var localIdBySyncId = refreshedPlants
                .Where(p => !string.IsNullOrEmpty(p.SyncId))
                .ToDictionary(p => p.SyncId, p => p.Id);

            var allLocalLogs = await this.wateringLogRepository.GetAllAsync();
            var localLogsBySyncId = allLocalLogs
                .Where(l => !string.IsNullOrEmpty(l.SyncId))
                .ToDictionary(l => l.SyncId);

            foreach (var remote in remoteLogs)
            {
                if (!localLogsBySyncId.ContainsKey(remote.SyncId)
                    && localIdBySyncId.TryGetValue(remote.PlantSyncId, out var localPlantId))
                {
                    var newLog = new WateringLog
                    {
                        SyncId = remote.SyncId,
                        PlantId = localPlantId,
                        WateredAt = remote.WateredAt,
                        UpdatedAt = remote.UpdatedAt,
                    };
                    await this.wateringLogRepository.SaveAsync(newLog);
                }
            }

            this.LastSyncedAt = DateTime.UtcNow;
            this.SetStatus(SyncStatus.Idle);
        }
        catch
        {
            this.SetStatus(SyncStatus.Error);
        }
    }

    /// <summary>
    /// Sets <see cref="Status"/> to the new value and fires <see cref="StatusChanged"/> so that
    /// bound ViewModels (AccountViewModel, DashboardPage) can update their UI immediately.
    /// </summary>
    private void SetStatus(SyncStatus status)
    {
        this.Status = status;
        this.StatusChanged?.Invoke(this, EventArgs.Empty);
    }
}
