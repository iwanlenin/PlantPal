# Phase 05: Dashboard UI + Add Plant Form

**Status:** ⬜ Pending
**Branch:** `feature/phase-05-dashboard-ui`
**Est. time:** ~50 min

---

## Goal
Working dashboard and add/edit plant form — app usable end-to-end for the first time.

## What to teach
This is the first phase where you'll actually SEE the app running. The ViewModel logic is already proven by tests — the UI just wires up to it. XAML binds directly to ViewModel properties via `{Binding}`. If a binding is wrong, the UI just shows nothing rather than crashing — use the Output window in Visual Studio to catch binding errors.

## Design references
Before building any UI, read these files:
- `docs/design/design-system.md` — color tokens, typography, radii
- `docs/design/design-spec.md` — full component and surface rules
- `docs/design/mockups/mvp-dashboard-tasks/screen.png` — dashboard variant 1
- `docs/design/mockups/mvp-dashboard-planner/screen.png` — dashboard variant 2
- `docs/design/mockups/mvp-add-plant/screen.png` — add plant form mockup

## Decisions required
1. Color scheme: the design system uses "The Botanical Archivist" (warm off-white `#faf9f5`, forest green gradient `#396637` → `#517f4e`). Do you want to apply this now, or use a basic theme first and polish in v1.0?

## Files to create/modify
- `PlantPal/Pages/DashboardPage.xaml`
- `PlantPal/Pages/DashboardPage.xaml.cs`
- `PlantPal/Pages/AddPlantPage.xaml`
- `PlantPal/Pages/AddPlantPage.xaml.cs`
- `PlantPal/ViewModels/AddPlantViewModel.cs`
- `PlantPal.Tests/ViewModels/AddPlantViewModelTests.cs`
- `PlantPal/AppShell.xaml`
- `PlantPal/Resources/Styles/Colors.xaml` (design tokens)

## Prior state
The following already exist — do not recreate:
- `DashboardViewModel.cs` from Phase 04
- All interfaces from Phase 01
- `PlantSpeciesService.cs` from Phase 02
- `DatabaseService.cs` from Phase 03

## Claude Code prompt

```
Before building the UI, explain to me:
1. How XAML data binding works — {Binding PropertyName} and {Binding Command}
2. What BindingContext is and how it connects a Page to a ViewModel
3. How CommunityToolkit.Mvvm's [RelayCommand] and [ObservableProperty] attributes work

Ask me to confirm the color scheme preference before writing any XAML.

Then write AddPlantViewModel tests first (same TDD pattern — positive + negative cases for:
SaveAsync validates name, SaveAsync calls repository, species selection updates interval,
edit mode pre-fills fields). Confirm RED, then implement, confirm GREEN.

THEN build:

AddPlantPage.xaml / AddPlantViewModel.cs:
- Photo picker (tap → ActionSheet: Camera / Gallery via MediaPicker, degrade gracefully if
  permission denied — show snackbar via CommunityToolkit.Maui, save to
  FileSystem.AppDataDirectory/photos/)
- Species SearchBar binding to ObservableProperty filtering PlantSpeciesService.GetAll()
- Suggestion chip: "💧 Suggested every N days" updates live on selection
- Watering interval Stepper (min 1, max 90)
- Location Picker: Living Room, Bedroom, Kitchen, Bathroom, Balcony, Garden, Other
- Last Watered DatePicker (default today)
- Save button with IsLoading state
- Edit mode: shell navigation parameter ?plantId=X pre-fills form, shows Delete button

DashboardPage.xaml:
- "Needs Water" CollectionView bound to DueTodayPlants
- "Upcoming" CollectionView bound to UpcomingPlants
- Empty state (IsEmpty binding)
- Plant card with thumbnail (species thumbnail from IPlantSpeciesService)
- "Water Now" button with confirmation checkmark animation (200ms, CommunityToolkit.Maui)
- FAB "+" button navigating to AddPlantPage

AppShell.xaml: register routes for DashboardPage and AddPlantPage.

After building, run: ./scripts/test-all.sh
Show me any binding errors from the Output window.

In Visual Studio: F5 on Android Emulator. Click 'Reload All' when VS prompts about external
file changes.

Do not create any files not explicitly listed above.

Success condition: ./scripts/test-all.sh is green. Update BUILD_STATUS.md checklist.

Commit: ./scripts/commit-phase.sh "feat: Dashboard and AddPlant UI with XAML bindings"
```

## Success condition
- All tests (including new `AddPlantViewModelTests`) pass
- `./scripts/test-all.sh` is green
- App runs on Android Emulator showing the dashboard
- `BUILD_STATUS.md` Phase 05 checked

## Visual Studio steps
In Visual Studio 2022: F5 on Android Emulator target. Click "Reload All" if VS detects external file changes.

## Deviations from plan
<!-- Fill in after completion -->
