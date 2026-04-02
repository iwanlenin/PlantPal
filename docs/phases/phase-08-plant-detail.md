# Phase 08: Plant Detail Page + Watering History

**Status:** ✅ Complete
**Branch:** `feature/phase-08-plant-detail`
**Est. time:** ~50 min

---

## Goal
Plant detail screen with large cached image, watering history log, and history stored in the DB.

## What to teach
WateringLog is a separate DB table linked to plants by foreign key. This is a one-to-many relationship: one plant has many watering logs. We add a second repository (`IWateringLogRepository`) rather than extending `IPlantRepository` — this keeps each class focused on one responsibility. Notice the pattern: model → interface → tests → implementation → UI. Same every time.

## Decisions required
1. Should notes be optional on a watering log, or should we skip notes entirely?

## Files to create/modify
- `PlantPal/Models/WateringLog.cs`
- `PlantPal/Services/WateringLogRepository.cs`
- `PlantPal/ViewModels/PlantDetailViewModel.cs`
- `PlantPal/Pages/PlantDetailPage.xaml`
- `PlantPal/Pages/PlantDetailPage.xaml.cs`
- `PlantPal.Tests/ViewModels/PlantDetailViewModelTests.cs`
- `PlantPal.Tests/Services/WateringLogRepositoryTests.cs`

## Prior state
The following already exist — do not recreate:
- `IWateringLogRepository` (`PlantPal/Interfaces/IWateringLogRepository.cs`) from Phase 01
- `IImageCacheService` (`PlantPal/Interfaces/IImageCacheService.cs`) from Phase 01
- `ImageCacheService.cs` from Phase 07
- `DatabaseService.cs` from Phase 03

## Claude Code prompt

```
Explain what a foreign key relationship is in SQLite and how sqlite-net-pcl handles it
(it doesn't enforce FK constraints — explain why that means our delete logic must manually
clean up logs when a plant is deleted).

Ask me the notes decision.

TDD for WateringLogRepository (PlantPal.Tests/Services/WateringLogRepositoryTests.cs):
Same TDD pattern — write tests, confirm RED, implement, confirm GREEN.
Use in-memory SQLite (":memory:").
Positive: SaveAsync then GetByPlantIdAsync returns log. DeleteByPlantIdAsync removes all logs
for that plant. GetByPlantIdAsync on empty returns empty list.
Negative: GetByPlantIdAsync for nonexistent plantId returns empty list (not null, not throw).

TDD for PlantDetailViewModel (PlantPal.Tests/ViewModels/PlantDetailViewModelTests.cs):
Mock IPlantRepository, IWateringLogRepository, IImageCacheService, INavigationService.

Key tests:
- LoadAsync sets PlantDetailImage via ImageCacheService (verify GetDetailImageAsync called)
- LoadAsync when ImageCacheService returns fallback: IsShowingFallbackImage = true,
  banner text contains "No internet"
- WaterNowAsync inserts WateringLog AND updates Plant.LastWateredDate
- DeletePlantAsync calls both IPlantRepository.DeleteAsync AND
  IWateringLogRepository.DeleteByPlantIdAsync

Run tests — confirm RED. Implement. Confirm GREEN.

Build PlantDetailPage.xaml:
- Hero image bound to PlantDetailImage (FileImageSource)
- "No internet — showing placeholder" banner: visible when IsShowingFallbackImage = true
- "Water Now" button → WaterNowCommand
- History CollectionView (newest first, bound to WateringHistory)
- Edit and Delete buttons
- this. prefix, XML doc comments on ViewModel

Update DashboardPage: tapping plant card navigates to PlantDetailPage (not AddPlantPage).
Update DeleteAsync in DatabaseService to also call WateringLogRepository.DeleteByPlantIdAsync.

In Visual Studio: F5 on Android Emulator. Click 'Reload All' if VS prompts.

Do not create any files not explicitly listed above.

Success condition: ./scripts/test-all.sh is green. Update BUILD_STATUS.md checklist.

Commit: ./scripts/commit-phase.sh "feat: plant detail page, watering history, image cache integration"
```

## Success condition
- All tests pass including `PlantDetailViewModelTests` and `WateringLogRepositoryTests`
- `./scripts/test-all.sh` is green
- `BUILD_STATUS.md` Phase 08 checked

## Visual Studio steps
F5 on Android Emulator. Tap a plant to open detail. Tap "Water Now" and verify history appears below.

## Deviations from plan
- `WateringLogRepository` placed in `PlantPal.Core/Services/` (not `PlantPal/Services/`) — test project references Core only
- `PlantDetailViewModel` placed in `PlantPal.Core/ViewModels/` (not `PlantPal/ViewModels/`) — same testability rationale
- `WateringLog` model already existed in `PlantPal.Core/Models/` — not recreated
- Cascade delete handled in `PlantDetailViewModel.DeleteAsync` (calls both repos) — `DatabaseService.DeleteAsync` unchanged
- Log notes: skipped (confirmed by user) — WateringLog has only PlantId + WateredAt
- `DashboardViewModel.OpenPlantAsync` updated to navigate to `"PlantDetail"` (was temporarily `"AddPlant"`)
