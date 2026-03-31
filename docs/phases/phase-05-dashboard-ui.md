# Phase 05: Dashboard UI + Add Plant Form

**Status:** ⬜ Pending
**Branch:** `feature/phase-05-dashboard-ui`
**Est. time:** ~50 min

## Goal
Working dashboard and add/edit plant form — app usable end-to-end for the first time.

## Decisions required
1. Color scheme: apply "The Botanical Archivist" design now (warm off-white `#faf9f5`, forest green `#396637`→`#517f4e`) or basic theme first and polish in v1.0?

## Prior state
- `DashboardViewModel.cs` from Phase 04 — do not recreate
- All interfaces and services from Phases 01–03

## Files to create/modify
- `PlantPal/Pages/DashboardPage.xaml` + `.xaml.cs`
- `PlantPal/Pages/AddPlantPage.xaml` + `.xaml.cs`
- `PlantPal/ViewModels/AddPlantViewModel.cs`
- `PlantPal.Tests/ViewModels/AddPlantViewModelTests.cs`
- `PlantPal/AppShell.xaml`
- `PlantPal/Resources/Styles/Colors.xaml`

## Tests (AddPlantViewModelTests.cs)
TDD first. Positive: SaveAsync validates Name · SaveAsync calls IPlantRepository · species selection updates WateringIntervalDays · edit mode pre-fills all fields
Negative: SaveAsync with empty Name → error state, no crash · SaveAsync when photo permission denied → saves plant without photo, shows snackbar

## UI spec

**AddPlantPage.xaml:**
- Photo picker → ActionSheet (Camera / Gallery via MediaPicker) · permission denied → snackbar, save without photo · save to `FileSystem.AppDataDirectory/photos/`
- Species SearchBar filtering `PlantSpeciesService.GetAll()`
- Suggestion chip: "💧 Suggested every N days" updates live on selection
- Watering interval Stepper (min 1, max 90)
- Location Picker: Living Room, Bedroom, Kitchen, Bathroom, Balcony, Garden, Other
- Last Watered DatePicker (default today)
- Save button with IsLoading state
- Edit mode: `?plantId=X` shell parameter pre-fills form, shows Delete button

**DashboardPage.xaml:**
- "Needs Water" CollectionView → `DueTodayPlants`
- "Upcoming" CollectionView → `UpcomingPlants`
- Empty state bound to `IsEmpty`
- Plant card with species thumbnail
- "Water Now" button with 200ms checkmark animation (CommunityToolkit.Maui)
- FAB "+" → AddPlantPage

**AppShell.xaml:** register routes for DashboardPage and AddPlantPage.

## Success condition
- All tests including `AddPlantViewModelTests` pass
- `./scripts/test-all.sh` is green
- App runs on Android Emulator showing the dashboard
- `BUILD_STATUS.md` Phase 05 checked
- Commit: `./scripts/commit-phase.sh "feat: Dashboard and AddPlant UI with XAML bindings"`

## Visual Studio steps
F5 on Android Emulator. Click "Reload All" if VS detects external file changes.

## Deviations from plan
<!-- Fill in after completion -->
