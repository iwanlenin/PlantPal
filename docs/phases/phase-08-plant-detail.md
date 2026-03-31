# Phase 08: Plant Detail Page + Watering History

**Status:** ⬜ Pending
**Branch:** `feature/phase-08-plant-detail`
**Est. time:** ~50 min

## Goal
Plant detail screen with large cached image, watering history log, history stored in DB.

## Decisions required
1. Should notes be optional on a watering log, or skip notes entirely?

## Prior state
- `IWateringLogRepository` in `PlantPal.Core/Interfaces/IWateringLogRepository.cs` — do not recreate
- `WateringLog.cs` in `PlantPal.Core/Models/WateringLog.cs` — do not recreate
- `IImageCacheService` + `ImageCacheService.cs` from Phase 07
- `DatabaseService.cs` from Phase 03

## Files to create/modify
- `PlantPal/Services/WateringLogRepository.cs`
- `PlantPal/ViewModels/PlantDetailViewModel.cs`
- `PlantPal/Pages/PlantDetailPage.xaml` + `.xaml.cs`
- `PlantPal.Tests/ViewModels/PlantDetailViewModelTests.cs`
- `PlantPal.Tests/Services/WateringLogRepositoryTests.cs`
- `PlantPal/Services/DatabaseService.cs` (update DeleteAsync to cascade)
- `PlantPal/Pages/DashboardPage.xaml` (tap plant → PlantDetailPage, not AddPlantPage)

## Tests (WateringLogRepositoryTests.cs)
In-memory SQLite. Positive: SaveAsync then GetByPlantIdAsync returns log · DeleteByPlantIdAsync removes all logs for that plant · GetByPlantIdAsync on empty returns empty list

Negative: GetByPlantIdAsync(nonexistent plantId) returns empty list (not null, not throw)

## Tests (PlantDetailViewModelTests.cs)
Mock `IPlantRepository`, `IWateringLogRepository`, `IImageCacheService`, `INavigationService`.

Key tests: LoadAsync calls GetDetailImageAsync and sets PlantDetailImage · LoadAsync when ImageCacheService returns fallback → IsShowingFallbackImage=true · WaterNowAsync inserts WateringLog AND updates Plant.LastWateredDate · DeletePlantAsync calls IPlantRepository.DeleteAsync AND IWateringLogRepository.DeleteByPlantIdAsync

## UI spec (PlantDetailPage.xaml)
- Hero image bound to `PlantDetailImage` (FileImageSource)
- "No internet — showing placeholder" banner visible when `IsShowingFallbackImage=true`
- "Water Now" button → `WaterNowCommand`
- History CollectionView, newest first, bound to `WateringHistory`
- Edit and Delete buttons

**DatabaseService.cs:** update `DeleteAsync` to also call `WateringLogRepository.DeleteByPlantIdAsync`.

## Success condition
- All tests pass including `PlantDetailViewModelTests` and `WateringLogRepositoryTests`
- `./scripts/test-all.sh` is green
- `BUILD_STATUS.md` Phase 08 checked
- Commit: `./scripts/commit-phase.sh "feat: plant detail page, watering history, image cache integration"`

## Visual Studio steps
F5 on Android Emulator. Tap a plant → detail opens. Tap "Water Now" → history entry appears below.

## Deviations from plan
<!-- Fill in after completion -->
