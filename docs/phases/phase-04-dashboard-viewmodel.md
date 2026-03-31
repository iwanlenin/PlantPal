# Phase 04: Dashboard ViewModel — TDD

**Status:** ⬜ Pending
**Branch:** `feature/phase-04-dashboard-viewmodel`
**Est. time:** ~45 min

## Goal
DashboardViewModel fully tested with mocked dependencies — no MAUI, no emulator needed.

## Decisions required
None — proceed once prior phases are green.

## Prior state
Interfaces from Phase 01 — do not recreate: `IPlantRepository` · `INotificationService` · `INavigationService` · `IPermissionService`

## Files to create/modify
- `PlantPal/ViewModels/DashboardViewModel.cs`
- `PlantPal.Tests/ViewModels/DashboardViewModelTests.cs`

## Tests (DashboardViewModelTests.cs)
Setup: NSubstitute mocks for `IPlantRepository`, `INotificationService`, `INavigationService`, `IPermissionService`.

Positive: LoadPlantsAsync() calls GetAllAsync() exactly once · plants with NextWaterDate ≤ today → DueTodayPlants · plants with NextWaterDate within 7 days → UpcomingPlants · plants with NextWaterDate > 7 days → neither collection · WaterNowAsync(plant) calls SaveAsync with updated LastWateredDate=today · WaterNowAsync(plant) calls ScheduleReminderAsync with that plant · IsEmpty=true when repo returns empty list · IsEmpty=false when repo returns plants

Negative: WaterNowAsync(null) does not throw · LoadPlantsAsync when repo throws → HasError=true, no crash · notification permission denied → WaterNowAsync still saves (notifications non-blocking)

## Implementation notes
- Extends `ObservableObject` (CommunityToolkit.Mvvm)
- `[ObservableProperty]`: `DueTodayPlants`, `UpcomingPlants`, `IsEmpty`, `HasError`, `IsLoading`, `ShowNotificationBanner`
- `[RelayCommand]`: `LoadPlantsCommand`, `WaterNowCommand(Plant plant)`, `AddPlantCommand`, `OpenPlantCommand(Plant plant)`

## Success condition
- All tests in `DashboardViewModelTests.cs` pass
- `./scripts/test-all.sh` is green
- `BUILD_STATUS.md` Phase 04 checked
- Commit: `./scripts/commit-phase.sh "feat: DashboardViewModel with full NSubstitute TDD"`

## Deviations from plan
<!-- Fill in after completion -->
