# Phase 10: Settings Page + Final Polish

**Status:** ⬜ Pending
**Branch:** `feature/phase-10-settings-polish`
**Est. time:** ~30 min

## Goal
Settings page, tab bar, app icon, splash screen, Water Now animation. v1.0 complete.

## Decisions required
None — proceed after all previous phases are green.

## Prior state
- All interfaces and services from Phases 01–09
- `DashboardPage.xaml` from Phase 05
- `PlantDetailViewModel.cs` from Phase 08

## Files to create/modify
- `PlantPal/Pages/SettingsPage.xaml` + `.xaml.cs`
- `PlantPal/Pages/PlantListPage.xaml` + `.xaml.cs`
- `PlantPal/AppShell.xaml` (add tabs)
- `PlantPal/Resources/AppIcon/appicon.svg`
- `PlantPal/MauiProgram.cs` (splash screen config)
- `PlantPal/Pages/DashboardPage.xaml.cs` (Water Now animation)

## Implementation notes

**SettingsPage.xaml:**
- Permissions section: Notifications row (green ✓ or amber ⚠️ via `IPermissionService`) + "Open Settings" button using `AppInfo.ShowSettingsUI()` if denied · Photo Access row (same pattern)
- Preferences section: Reminder time `TimePicker` (default 09:00, `Preferences.Set("reminder_time")`)

**AppShell.xaml:** two tabs — Dashboard (leaf icon via MauiIcons.Material) + All Plants (list icon)

**PlantListPage:** full plant list · swipe-to-delete (calls `DeletePlantAsync`) · tap → PlantDetailPage

**App icon:** simple leaf SVG at `Resources/AppIcon/appicon.svg` · configure `ForegroundFile` and `Color` in `PlantPal.csproj`

**Splash screen:** dark green `#0f1a0f` background, white "PlantPal" text · configure in `MauiProgram.cs`

**Water Now animation (DashboardPage.xaml.cs):** tap → `await button.ScaleTo(1.1, 100)` + confirmed-green color + text "✓ Done" → after 800ms animate back

## Success condition
- `./scripts/test-all.sh` fully green
- App runs with tab bar, settings page, app icon, splash screen, Water Now animation
- `BUILD_STATUS.md` Phase 10 checked
- Commit: `./scripts/commit-phase.sh "feat: settings page, tab bar, app icon, polish"`
- PR: `gh pr create --title "v1.0: Full PlantPal Experience" --body "Adds photos, history, widget, settings, polish"`

## Visual Studio steps
F5 on Android Emulator. Verify: tab bar visible · Settings page opens · app icon appears · splash screen on cold launch · Water Now animation plays.

## Deviations from plan
<!-- Fill in after completion -->
