# Phase 10: Settings Page + Final Polish

**Status:** ✅ Complete
**Branch:** `feature/phase-10-settings-polish`
**Est. time:** ~30 min

---

## Goal
Settings page, tab bar, app icon, splash screen, Water Now animation. v1.0 complete.

## What to teach
This phase is about the gap between "it works" and "it feels good." The Water Now animation is a micro-interaction — it gives the user immediate feedback that their tap did something. Small moments like this are what make apps feel native vs built.

## Decisions required
None — proceed after all previous phases are green.

## Files to create/modify
- `PlantPal/Pages/SettingsPage.xaml`
- `PlantPal/Pages/SettingsPage.xaml.cs`
- `PlantPal/Pages/PlantListPage.xaml`
- `PlantPal/Pages/PlantListPage.xaml.cs`
- `PlantPal/AppShell.xaml` (update — add tabs)
- `PlantPal/Resources/AppIcon/appicon.svg`
- `PlantPal/MauiProgram.cs` (splash screen config)
- `PlantPal/Pages/DashboardPage.xaml.cs` (Water Now animation)

## Prior state
The following already exist — do not recreate:
- All interfaces and services from Phases 01–09
- `DashboardPage.xaml` from Phase 05
- `PlantDetailViewModel.cs` from Phase 08

## Claude Code prompt

```
Build final v1.0 polish:

SettingsPage.xaml:
- Permissions section:
  - Notifications row: show current status via IPermissionService (green ✓ or amber ⚠️),
    "Open Settings" button using AppInfo.ShowSettingsUI() if denied
  - Photo Access row: same pattern
- Preferences section:
  - Reminder time TimePicker (default 09:00, save to Preferences.Set("reminder_time"))

AppShell.xaml:
- Two tabs: Dashboard (plant/leaf icon via MauiIcons.Material), All Plants (list icon)
- PlantListPage: full list of all plants, swipe-to-delete (calls DeletePlantAsync),
  tap to navigate to PlantDetailPage

App Icon:
- Create a simple leaf SVG at Resources/AppIcon/appicon.svg
- Configure ForegroundFile and Color in PlantPal.csproj

Splash screen:
- Dark green (#0f1a0f) background, white "PlantPal" text
- Configure in MauiProgram.cs

"Water Now" button animation in DashboardPage.xaml.cs:
- On tap: animate button (scale to 1.1, switch to confirmed-green color, text "✓ Done")
- After 800ms: animate back to normal state
- Use MAUI animation API: await button.ScaleTo(1.1, 100)

Run full test suite: ./scripts/test-all.sh
If any tests are failing, fix them before committing.

In Visual Studio: F5 on Android Emulator.

Do not create any files not explicitly listed above.

Success condition: ./scripts/test-all.sh is green. Update BUILD_STATUS.md checklist.

Commit: ./scripts/commit-phase.sh "feat: settings page, tab bar, app icon, polish"
Create PR: gh pr create --title "v1.0: Full PlantPal Experience" --body "Adds photos, history, widget, settings, polish"
```

## Success condition
- `./scripts/test-all.sh` is fully green
- App runs with tab bar, settings page, app icon, splash screen
- Water Now animation plays correctly
- `BUILD_STATUS.md` Phase 10 checked

## Visual Studio steps
F5 on Android Emulator. Verify: tab bar visible, Settings page opens, app icon appears, splash screen shows on cold launch, Water Now animation plays.

## Deviations from plan
- `MauiIcons.Material` not installed — used Unicode characters for tab icons (🌱 📋 ⚙) instead
- `SettingsViewModel.OpenSettingsCommand` raises an event (`OpenSettingsRequested`) instead of calling `AppInfo.ShowSettingsUI()` directly, since Core has no MAUI reference; page code-behind subscribes to the event
- Reminder time persisted in `SettingsPage.xaml.cs` code-behind via `Preferences.Set/Get` (not in ViewModel) for the same reason
- Splash screen SVG already had `<MauiSplashScreen>` entry in csproj — only updated Color from `#512BD4` to `#0f1a0f` and replaced the SVG content with a leaf + "PlantPal" text
- No new tests added — phase is pure UI/polish with no new business logic
