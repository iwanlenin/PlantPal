# Phase 06: PermissionService + NotificationService — TDD

**Status:** ✅ Complete
**Branch:** `feature/phase-06-permissions-notifications`
**Est. time:** ~40 min

---

## Goal
Graceful permission handling + local notifications. App works correctly with or without permissions granted.

## What to teach
Platform permissions are one of the trickiest parts of mobile development. The key insight: the app must NEVER crash or show a broken state because a user said "no" to a permission. We treat denied permissions as a valid app state, not an error. The notification service schedules silently — if notifications are off, it simply no-ops. The dashboard shows a banner if off, but everything else keeps working.

## Decisions required
1. When notifications are denied: show the "enable notifications" banner immediately on dashboard load, or only after the user first tries to save a plant?

## Files to create/modify
- `PlantPal.Core/Interfaces/INotificationScheduler.cs` (new — abstracts Plugin.LocalNotification for testability)
- `PlantPal.Core/Services/NotificationService.cs` (moved to Core, not MAUI — uses INotificationScheduler)
- `PlantPal/Services/PermissionService.cs`
- `PlantPal/Services/MauiNotificationScheduler.cs` (new — concrete Plugin.LocalNotification wrapper)
- `PlantPal.Tests/Services/NotificationServiceTests.cs`
- `PlantPal/Platforms/Android/MainApplication.cs`
- `PlantPal/Pages/DashboardPage.xaml` (amber notification banner)
- `PlantPal/Pages/DashboardPage.xaml.cs` (Settings tap handler)
- `PlantPal.Core/ViewModels/DashboardViewModel.cs` (permission check on load)

## Prior state
The following already exist — do not recreate:
- `IPermissionService` (`PlantPal/Interfaces/IPermissionService.cs`) from Phase 01
- `INotificationService` (`PlantPal/Interfaces/INotificationService.cs`) from Phase 01
- `PermissionResult.cs` (`PlantPal/Models/PermissionResult.cs`) from Phase 01
- `DashboardViewModel.cs` with `ShowNotificationBanner` property from Phase 04

## Claude Code prompt

```
Explain to me:
1. How Android and iOS handle notification permissions differently (Android 13+ requires explicit
   request, iOS always prompts on first use)
2. What DeniedPermanently means — why it matters to link users to Settings rather than
   re-requesting (the OS will ignore repeated requests)
3. Why we wrap platform permission APIs in IPermissionService rather than calling them
   directly in ViewModels

Ask me the decision question before proceeding.

TDD — STEP 1, write tests for NotificationService (PlantPal.Tests/Services/NotificationServiceTests.cs):
Use NSubstitute to mock IPermissionService.

Positive cases:
- ScheduleReminderAsync(plant) does nothing if AreNotificationsEnabled = false — no crash
- ScheduleReminderAsync(plant with null NextWaterDate) completes without scheduling
- CancelReminderAsync(id) completes even if notification doesn't exist
- RescheduleAllAsync(emptyList) completes without throwing

Negative cases:
- ScheduleReminderAsync(null) throws ArgumentNullException
- RescheduleAllAsync(null) throws ArgumentNullException

Run tests — confirm RED.

STEP 2 — Implement:

PermissionService.cs (implements IPermissionService):
- CheckNotificationPermissionAsync() → wraps Permissions.CheckStatusAsync<Permissions.PostNotifications>()
- RequestNotificationPermissionAsync() → requests, maps to PermissionResult enum
- CheckPhotoPermissionAsync() → wraps Permissions.CheckStatusAsync<Permissions.Photos>() (Android)
- All use this. for fields, XML doc comments

NotificationService.cs (implements INotificationService):
- Constructor takes IPermissionService — uses this.permissionService
- ScheduleReminderAsync: early-return (no-op) if !this.AreNotificationsEnabled or
  plant.NextWaterDate is null
- Schedule at NextWaterDate.Date + 09:00 using Plugin.LocalNotification
- Notification title: "🌿 Time to water {plant.Name}!", body: "{plant.Species} in {plant.Location}"
- NotificationId = plant.Id (for cancellation)
- AreNotificationsEnabled: async-check permission status, cache result for 30 seconds
- this. prefix on all fields, XML doc comments

Android: configure notification channel in Platforms/Android/MainApplication.cs with
importance High.

STEP 3 — Run tests — confirm GREEN.

Update DashboardViewModel: on LoadPlantsAsync, check notification permission → set
ShowNotificationBanner if denied.
Update DashboardPage.xaml: add amber banner bound to ShowNotificationBanner with
tap-to-open-settings action.

Run: ./scripts/test-all.sh

In Visual Studio: F5 on Android Emulator. Deny notification permission when prompted and
confirm the banner appears on the dashboard.

Do not create any files not explicitly listed above.

Success condition: ./scripts/test-all.sh is green. Update BUILD_STATUS.md checklist.

Commit: ./scripts/commit-phase.sh "feat: PermissionService and NotificationService with graceful degradation"
```

## Success condition
- All tests pass including `NotificationServiceTests` and `PermissionServiceTests`
- `./scripts/test-all.sh` is green
- App tested on emulator: deny permission → banner appears, app does not crash
- `BUILD_STATUS.md` Phase 06 checked

## Visual Studio steps
F5 on Android Emulator. Deny notification permission when prompted. Confirm amber banner appears on dashboard. App should remain fully functional.

## Deviations from plan
- `NotificationService` placed in `PlantPal.Core/Services/` (not `PlantPal/Services/`) — requires `INotificationScheduler` abstraction so unit tests don't depend on MAUI
- Added `INotificationScheduler` interface to Core — thin wrapper around `Plugin.LocalNotification` so scheduling can be mocked in tests
- Added `MauiNotificationScheduler` in `PlantPal/Services/` — concrete implementation using `Plugin.LocalNotification` v14 API (namespace: `Plugin.LocalNotification.Core.Models`)
- `PermissionServiceTests.cs` not created — `PermissionService` wraps MAUI `Permissions` API directly; no testable logic independent of platform
- Banner decision: show on every dashboard load (confirmed by user)
- Plugin namespace: `Plugin.LocalNotification.Core.Models` and `Plugin.LocalNotification.Core.Models.AndroidOption` (not `Plugin.LocalNotification.AndroidOption` as assumed)
