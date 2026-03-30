# Phase 13: Cloud Sync via Supabase

**Status:** ⬜ Pending
**Branch:** `feature/phase-13-cloud-sync`
**Est. time:** ~3 hrs

---

## Goal
Plants and watering logs synced across devices. Google sign-in. Offline-first architecture.

## What to teach
This is the most architecturally complex phase. We keep SQLite as the source of truth — the app always reads and writes locally first. Sync is a background process that reconciles local changes with the server. Last-write-wins is the simplest conflict resolution strategy — it works well for personal use where only one person is editing. The offline banner and sync indicator are just UI reflections of `IConnectivityService` and `ISyncService` state.

## Decisions required
1. Supabase free tier has row-level security — do you want user-isolated data (each user sees only their own plants, **strongly recommended**) or a single shared table?
2. Should sync happen automatically in the background (on connectivity change), or only when the user pulls to refresh?

## Files to create/modify
- `PlantPal/Interfaces/ISyncService.cs`
- `PlantPal/Services/SyncService.cs`
- `PlantPal.Tests/Services/SyncServiceTests.cs`
- `PlantPal/Models/Plant.cs` (add `UpdatedAt` field — DB migration required)
- `PlantPal/Models/WateringLog.cs` (add `UpdatedAt` field)
- `PlantPal/Pages/AccountPage.xaml`
- `PlantPal/Pages/AccountPage.xaml.cs`
- `PlantPal/AppShell.xaml` (add Account tab)

## Prior state
The following already exist — do not recreate:
- `IConnectivityService` and `ConnectivityService.cs` from Phase 07
- `IPlantRepository`, `IWateringLogRepository` and implementations from Phases 01/03/08
- `Plant.cs` and `WateringLog.cs` models from Phases 01/08

> [!WARNING]
> Adding `UpdatedAt` to `Plant` and `WateringLog` requires a SQLite schema migration.
> Explain the migration strategy before implementing.

## Claude Code prompt

```
Explain to me: what is an offline-first architecture? Why do we write to SQLite first rather
than the server first? What is a "sync conflict" and what does "last-write-wins" mean as a
conflict resolution strategy?

Ask me both decision questions before proceeding.

TDD (PlantPal.Tests/Services/SyncServiceTests.cs):
Mock IPlantRepository, IWateringLogRepository, and ISupabaseClient.

Positive cases:
- SyncUpAsync() calls GetAllAsync and upserts changed records to Supabase
- SyncDownAsync() merges remote changes into local SQLite (last-write-wins by UpdatedAt)
- SyncUpAsync when offline → queues changes locally, completes without throwing

Negative cases:
- Supabase returns 500 → SyncStatus = SyncStatus.Error, does not corrupt local data
- SyncDownAsync with no remote changes → completes, local data unchanged

Run — confirm RED. Implement. Confirm GREEN.

ISyncService (PlantPal/Interfaces/ISyncService.cs):
- Task SyncUpAsync(), Task SyncDownAsync()
- SyncStatus Status (enum: Idle, Syncing, Error, Offline)
- DateTime? LastSyncedAt

SyncService.cs implements ISyncService:
- Constructor: IPlantRepository, IWateringLogRepository, IConnectivityService, ISupabaseClient
- this. prefix, XML doc comments
- Uses supabase-csharp NuGet

Model updates:
- Add UpdatedAt (DateTime) to Plant and WateringLog
- Implement SQLite migration: check if column exists, ALTER TABLE if not
- Explain the migration approach before writing it

Authentication:
- Google Sign-In via Supabase OAuth (WebAuthenticator.AuthenticateAsync)
- Store session token in SecureStorage

AccountPage.xaml:
- Signed-in state: user email, sync status badge, last synced time, connected devices,
  manual sync button
- Offline state: reassurance message "Your plants are safe — will sync when back online"
- Sign-out button

Dashboard header: sync indicator badge bound to SyncService.Status.
Offline banner bound to IConnectivityService.IsConnected = false.

Run full test suite: ./scripts/test-all.sh
All must be green before committing.

Do not create any files not explicitly listed above.

Success condition: ./scripts/test-all.sh is green. Update BUILD_STATUS.md checklist.

Commit: ./scripts/commit-phase.sh "feat: Supabase cloud sync with offline-first architecture"
Create PR: gh pr create --title "v2.0: Smart Features" --body "Weather adjustment, Claude advisor, cloud sync"
```

## Success condition
- All tests pass including `SyncServiceTests`
- `./scripts/test-all.sh` fully green
- App tested: add plant on device 1, sync, verify it appears on device 2
- `BUILD_STATUS.md` Phase 13 checked

## Deviations from plan
<!-- Fill in after completion -->
