# Phase 04: Dashboard ViewModel — TDD

**Status:** ✅ Complete
**Branch:** `feature/phase-04-dashboard-viewmodel`
**Est. time:** ~45 min

---

## Goal
DashboardViewModel fully tested with mocked repository — no MAUI dependencies.

## What to teach
This phase shows TDD's biggest payoff. We test the ViewModel — all the logic of "which plants are due today", "what happens when I tap Water Now" — without ever running the app or touching an emulator. If the logic is wrong, the test tells you in under a second. NSubstitute lets us fake the repository: "pretend GetAllAsync returns this list" and verify "was SaveAsync called exactly once with the updated plant?"

## Decisions required
None — proceed once prior phases are green.

## Files to create/modify
- `PlantPal.Core/ViewModels/DashboardViewModel.cs`
- `PlantPal.Tests/ViewModels/DashboardViewModelTests.cs`
- `PlantPal/MauiProgram.cs` (DashboardViewModel registered as Transient)

## Prior state
The following interfaces already exist from Phase 01 — do not recreate them:
- `IPlantRepository` (`PlantPal/Interfaces/IPlantRepository.cs`)
- `INotificationService` (`PlantPal/Interfaces/INotificationService.cs`)
- `INavigationService` (`PlantPal/Interfaces/INavigationService.cs`)
- `IPermissionService` (`PlantPal/Interfaces/IPermissionService.cs`)

## Claude Code prompt

```
Explain NSubstitute to me with a short example before we start. Show: how to create a mock,
how to set up a return value, and how to verify a method was called.

Then TDD:

STEP 1 — Write tests (PlantPal.Tests/ViewModels/DashboardViewModelTests.cs):
Setup: mock IPlantRepository, mock INotificationService, mock INavigationService,
mock IPermissionService.

Positive cases:
- LoadPlantsAsync() calls repository.GetAllAsync() exactly once
- Plants with NextWaterDate <= today appear in DueTodayPlants collection
- Plants with NextWaterDate in next 7 days appear in UpcomingPlants collection
- Plants with NextWaterDate > 7 days from now appear in neither collection
- WaterNowAsync(plant) calls repository.SaveAsync() with updated LastWateredDate = today
- WaterNowAsync(plant) calls notificationService.ScheduleReminderAsync() with the plant
- IsEmpty is true when repository returns empty list
- IsEmpty is false when repository returns plants

Negative cases:
- WaterNowAsync(null) does not throw
- LoadPlantsAsync() when repository throws: sets HasError = true, does not crash
- Notification permission denied: WaterNowAsync still saves the plant (notifications are non-blocking)

Run tests — confirm RED.

STEP 2 — Implement PlantPal/ViewModels/DashboardViewModel.cs:
- Extends ObservableObject (CommunityToolkit.Mvvm)
- Constructor takes IPlantRepository, INotificationService, INavigationService, IPermissionService
- Use this. prefix on all private fields
- [ObservableProperty]: DueTodayPlants, UpcomingPlants, IsEmpty, HasError, IsLoading,
  ShowNotificationBanner
- [RelayCommand]: LoadPlantsCommand, WaterNowCommand(Plant plant), AddPlantCommand,
  OpenPlantCommand(Plant plant)
- XML doc comments on class and all public members

STEP 3 — Run tests — confirm GREEN.
STEP 4 — Ask me: are there any loading states or error states you want to handle differently?

Do not create any files not explicitly listed above.

Success condition: ./scripts/test-all.sh is green. Update BUILD_STATUS.md checklist.

Commit: ./scripts/commit-phase.sh "feat: DashboardViewModel with full NSubstitute TDD"
```

## Success condition
- All tests in `DashboardViewModelTests.cs` pass
- `./scripts/test-all.sh` is green
- `BUILD_STATUS.md` Phase 04 checked

## Deviations from plan
- `DashboardViewModel` placed in `PlantPal.Core/ViewModels/` (not `PlantPal/ViewModels/`) — test project references Core only, same pattern as Phases 02/03
- `CommunityToolkit.Mvvm` added to `PlantPal.Core.csproj` — required for `ObservableObject`, `[ObservableProperty]`, `[RelayCommand]` in Core
- `WaterNowAsync` swallows notification exceptions explicitly — notification failure must never prevent save from completing
