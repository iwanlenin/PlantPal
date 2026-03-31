# Phase 13: Cloud Sync via Supabase

**Status:** ⬜ Pending
**Branch:** `feature/phase-13-cloud-sync`
**Est. time:** ~3 hrs

## Goal
Plants and watering logs synced across devices. Google sign-in. Offline-first (SQLite is source of truth, sync is background reconciliation).

## Decisions required
1. User-isolated data with row-level security (each user sees only their plants — **strongly recommended**), or single shared table?
2. Auto-sync on connectivity change, or only on manual pull-to-refresh?

## Prior state
- `IConnectivityService` + `ConnectivityService.cs` from Phase 07
- `IPlantRepository`, `IWateringLogRepository` and implementations from Phases 01/03/08
- `Plant.cs` and `WateringLog.cs` from Phases 01/08

> [!WARNING]
> Adding `UpdatedAt` to `Plant` and `WateringLog` requires a SQLite schema migration. Explain migration strategy before implementing.

## Files to create/modify
- `PlantPal/Interfaces/ISyncService.cs`
- `PlantPal/Services/SyncService.cs`
- `PlantPal.Tests/Services/SyncServiceTests.cs`
- `PlantPal/Models/Plant.cs` (add `UpdatedAt` + migration)
- `PlantPal/Models/WateringLog.cs` (add `UpdatedAt` + migration)
- `PlantPal/Pages/AccountPage.xaml` + `.xaml.cs`
- `PlantPal/AppShell.xaml` (add Account tab)

## Tests (SyncServiceTests.cs)
Mock `IPlantRepository`, `IWateringLogRepository`, `ISupabaseClient`.

Positive: `SyncUpAsync()` calls `GetAllAsync` and upserts changed records · `SyncDownAsync()` merges remote changes (last-write-wins by `UpdatedAt`) · `SyncUpAsync` when offline → queues locally, no throw

Negative: Supabase 500 → `SyncStatus=Error`, local data intact · `SyncDownAsync` with no remote changes → local data unchanged

## Implementation notes

**ISyncService:**
- `Task SyncUpAsync()` / `Task SyncDownAsync()`
- `SyncStatus Status` (enum: Idle, Syncing, Error, Offline)
- `DateTime? LastSyncedAt`

**SyncService.cs:** constructor takes `IPlantRepository`, `IWateringLogRepository`, `IConnectivityService`, `ISupabaseClient` · uses `supabase-csharp` NuGet

**SQLite migration:** check if `UpdatedAt` column exists, `ALTER TABLE` if not — explain strategy before writing

**Authentication:** Google Sign-In via Supabase OAuth (`WebAuthenticator.AuthenticateAsync`) · session token in `SecureStorage`

**AccountPage.xaml:** signed-in state (email, sync badge, last synced, manual sync button) · offline state ("Your plants are safe — will sync when back online") · sign-out button

**Dashboard header:** sync status badge bound to `SyncService.Status` · offline banner bound to `!IConnectivityService.IsConnected`

## Success condition
- All tests pass including `SyncServiceTests`
- `./scripts/test-all.sh` fully green
- App tested: add plant on device 1 → sync → verify on device 2
- `BUILD_STATUS.md` Phase 13 checked
- Commit: `./scripts/commit-phase.sh "feat: Supabase cloud sync with offline-first architecture"`
- PR: `gh pr create --title "v2.0: Smart Features" --body "Weather adjustment, Claude advisor, cloud sync"`

## Deviations from plan
<!-- Fill in after completion -->
