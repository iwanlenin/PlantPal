# Phase 06: PermissionService + NotificationService — TDD

**Status:** ⬜ Pending
**Branch:** `feature/phase-06-permissions-notifications`
**Est. time:** ~40 min

## Goal
Graceful permission handling + local notifications. App works correctly whether permissions are granted or denied.

## Decisions required
1. When notifications are denied: show "enable notifications" banner immediately on dashboard load, or only after the user first tries to save a plant?

## Prior state
- `IPermissionService` in `PlantPal.Core/Interfaces/IPermissionService.cs` — do not recreate
- `INotificationService` in `PlantPal.Core/Interfaces/INotificationService.cs` — do not recreate
- `PermissionResult.cs` in `PlantPal.Core/Models/PermissionResult.cs` — do not recreate
- `DashboardViewModel.cs` with `ShowNotificationBanner` property from Phase 04

## Files to create/modify
- `PlantPal/Services/PermissionService.cs`
- `PlantPal/Services/NotificationService.cs`
- `PlantPal.Tests/Services/NotificationServiceTests.cs`
- `PlantPal.Tests/Services/PermissionServiceTests.cs`
- `PlantPal/Platforms/Android/MainApplication.cs`
- `PlantPal/Pages/DashboardPage.xaml` (amber banner)

## Tests (NotificationServiceTests.cs)
Mock `IPermissionService`.

Positive: ScheduleReminderAsync when AreNotificationsEnabled=false → no crash, no-op · ScheduleReminderAsync(plant with null NextWaterDate) completes without scheduling · CancelReminderAsync(id) completes even if notification doesn't exist · RescheduleAllAsync(emptyList) completes without throwing

Negative: ScheduleReminderAsync(null) throws ArgumentNullException · RescheduleAllAsync(null) throws ArgumentNullException

## Implementation notes

**PermissionService.cs:**
- `CheckNotificationPermissionAsync()` → `Permissions.CheckStatusAsync<Permissions.PostNotifications>()`
- `RequestNotificationPermissionAsync()` → request + map to `PermissionResult` enum
- `CheckPhotoPermissionAsync()` → `Permissions.CheckStatusAsync<Permissions.Photos>()`

**NotificationService.cs:**
- Constructor takes `IPermissionService`
- `ScheduleReminderAsync`: no-op if `!AreNotificationsEnabled` or `plant.NextWaterDate == null`
- Schedule at `NextWaterDate.Date + 09:00` via Plugin.LocalNotification
- Title: `"🌿 Time to water {plant.Name}!"` · Body: `"{plant.Species} in {plant.Location}"`
- `NotificationId = plant.Id`
- `AreNotificationsEnabled`: async permission check, cache result 30 seconds

**Android:** configure notification channel in `Platforms/Android/MainApplication.cs` with importance High.

**DashboardViewModel:** on `LoadPlantsAsync`, check notification permission → set `ShowNotificationBanner` if denied.

**DashboardPage.xaml:** amber banner bound to `ShowNotificationBanner`, tap → open Settings.

## Success condition
- All tests pass including `NotificationServiceTests` and `PermissionServiceTests`
- `./scripts/test-all.sh` is green
- `BUILD_STATUS.md` Phase 06 checked
- Commit: `./scripts/commit-phase.sh "feat: PermissionService and NotificationService with graceful degradation"`

## Visual Studio steps
F5 on Android Emulator. Deny notification permission when prompted. Confirm amber banner appears. App must remain fully functional.

## Deviations from plan
<!-- Fill in after completion -->
