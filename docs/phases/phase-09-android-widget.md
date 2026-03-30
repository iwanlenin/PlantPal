# Phase 09: Android Home Screen Widget

**Status:** ⬜ Pending
**Branch:** `feature/phase-09-android-widget`
**Est. time:** ~90 min

---

## Goal
2×2 Android widget showing count of plants due today, updates hourly.

## What to teach
Android widgets run in a separate process from the main app — they cannot use MAUI's DI container or services. They access the SQLite database directly by path. This is one of the few places in the app where we break the interface pattern — widget code is inherently platform-specific and not unit-testable in the traditional sense. We write an integration test instead that verifies the DB query logic separately.

## Decisions required
1. Widget style: show just the count ("3 plants need water"), or also the name of the most urgent plant?

## Files to create/modify
- `PlantPal/Platforms/Android/PlantWidget.cs`
- `PlantPal/Platforms/Android/Resources/layout/plant_widget.xml`
- `PlantPal/Platforms/Android/Resources/xml/plant_widget_info.xml`
- `PlantPal/Platforms/Android/AndroidManifest.xml` (update)
- `PlantPal.Tests/Integration/WidgetDbQueryTests.cs`

## Prior state
The following already exist — do not recreate:
- `DatabaseService.cs` from Phase 03 (widget accesses DB at same path directly, not via DI)
- `Plant.cs` model from Phase 01

## Claude Code prompt

```
Explain to me: why can Android widgets not use MAUI's dependency injection? What is a
RemoteViews object and why does the widget update model work differently from normal app UI?

Ask me the widget style decision.

Create integration test first (PlantPal.Tests/Integration/WidgetDbQueryTests.cs):
- Test the SQL query logic that counts due plants in isolation (using in-memory SQLite)
- Positive: 3 plants with NextWaterDate <= today → query returns count 3
- Positive: 0 plants due → query returns 0 (not null, not exception)
- Negative: empty database → returns 0

Run tests — confirm RED. Implement the query helper. Confirm GREEN.

Then implement:

Platforms/Android/Resources/layout/plant_widget.xml:
- Green rounded card
- Large number TextView (id: widget_count)
- Label "plants need water today" / "All plants happy 🌿" (when count = 0)
- Use @color resources matching app palette

PlantWidget.cs extends AppWidgetProvider:
- OnUpdate: open SQLite at FileSystem.AppDataDirectory + "plantpal.db" directly (no DI)
- Count rows where NextWaterDate <= DateTime.Now.Date
- Update RemoteViews with count and appropriate label
- Widget tap: PendingIntent opening DashboardPage via deep link
- this. prefix where applicable, XML doc comments

Resources/xml/plant_widget_info.xml:
- minResizeWidth 2 cells
- updatePeriodMillis = 3600000 (1 hour)
- previewLayout pointing to plant_widget.xml

Register in AndroidManifest.xml.

Update NotificationService.RescheduleAllAsync(): trigger AppWidgetManager.UpdateAppWidget
after rescheduling so widget refreshes when plants are watered.

In Visual Studio: F5 on Android Emulator target.
Manually add widget to home screen and verify it shows the correct count.

Do not create any files not explicitly listed above.

Success condition: ./scripts/test-all.sh is green. Update BUILD_STATUS.md checklist.

Commit: ./scripts/commit-phase.sh "feat: Android home screen widget with hourly updates"
```

## Success condition
- All tests pass including `WidgetDbQueryTests`
- `./scripts/test-all.sh` is green
- Widget added to emulator home screen shows correct count
- `BUILD_STATUS.md` Phase 09 checked

## Visual Studio steps
F5 on Android Emulator. Long-press home screen → Widgets → find PlantPal widget → add it. Verify count updates when a plant is watered.

## Deviations from plan
<!-- Fill in after completion -->
