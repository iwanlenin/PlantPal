# Phase 09: Android Home Screen Widget

**Status:** ⬜ Pending
**Branch:** `feature/phase-09-android-widget`
**Est. time:** ~90 min

## Goal
2×2 Android widget showing count of plants due today, updates hourly.

## Decisions required
1. Widget style: show just the count ("3 plants need water"), or also the name of the most urgent plant?

## Prior state
- `DatabaseService.cs` from Phase 03 — widget accesses DB at same path directly (no DI available in widget process)
- `Plant.cs` from Phase 01

## Files to create/modify
- `PlantPal/Platforms/Android/PlantWidget.cs`
- `PlantPal/Platforms/Android/Resources/layout/plant_widget.xml`
- `PlantPal/Platforms/Android/Resources/xml/plant_widget_info.xml`
- `PlantPal/Platforms/Android/AndroidManifest.xml` (update)
- `PlantPal.Tests/Integration/WidgetDbQueryTests.cs`
- `PlantPal/Services/NotificationService.cs` (update RescheduleAllAsync)

## Tests (WidgetDbQueryTests.cs)
In-memory SQLite. Tests cover the DB query helper in isolation (not the widget itself — widget is platform-only, not unit-testable).

Positive: 3 plants with NextWaterDate ≤ today → query returns 3 · 0 plants due → returns 0

Negative: empty DB → returns 0

## Implementation notes

**plant_widget.xml:** green rounded card · large count TextView (id: `widget_count`) · label "plants need water today" / "All plants happy 🌿" when count=0 · use `@color` resources matching app palette

**PlantWidget.cs** extends `AppWidgetProvider`:
- `OnUpdate`: open SQLite at `FileSystem.AppDataDirectory + "plantpal.db"` directly
- Count rows where `NextWaterDate <= DateTime.Now.Date`
- Update `RemoteViews` with count + label
- Tap → `PendingIntent` opening DashboardPage via deep link

**plant_widget_info.xml:** `minResizeWidth` 2 cells · `updatePeriodMillis=3600000` (1 hour) · `previewLayout` → plant_widget.xml

**NotificationService.RescheduleAllAsync():** trigger `AppWidgetManager.UpdateAppWidget` after rescheduling so widget refreshes on water.

## Success condition
- All tests pass including `WidgetDbQueryTests`
- `./scripts/test-all.sh` is green
- Widget added to emulator home screen shows correct count
- `BUILD_STATUS.md` Phase 09 checked
- Commit: `./scripts/commit-phase.sh "feat: Android home screen widget with hourly updates"`

## Visual Studio steps
F5 on Android Emulator. Long-press home screen → Widgets → find PlantPal widget → add. Verify count updates when a plant is watered.

## Deviations from plan
<!-- Fill in after completion -->
