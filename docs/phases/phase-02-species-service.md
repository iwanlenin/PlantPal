# Phase 02: PlantSpeciesService — TDD

**Status:** ⬜ Pending
**Branch:** `feature/phase-02-species-service`
**Est. time:** ~45 min

## Goal
Full 40-species European houseplant list with Wikipedia slugs, watering intervals, bundled thumbnails, TDD coverage.

## Decisions required
1. Should 'Other / Unknown' species return a default interval of 7 days, or should the interval field be mandatory?

## Prior state
- `IPlantSpeciesService` in `PlantPal.Core/Interfaces/IPlantSpeciesService.cs` — do not recreate
- `PlantSpecies.cs` in `PlantPal.Core/Models/PlantSpecies.cs` — do not recreate

## Files to create/modify
- `PlantPal/Services/PlantSpeciesService.cs`
- `PlantPal.Tests/Services/PlantSpeciesServiceTests.cs`
- `PlantPal/Resources/Images/Plants/` (40 thumbnails via download script)
- `scripts/download-plant-images.sh`

## Tests (PlantSpeciesServiceTests.cs)
Positive: GetAll() returns exactly 40 species · FindByName("Monstera") returns WateringIntervalDays=7 · FindByName case-insensitive · GetSuggestedInterval("monstera_deliciosa") returns 7 · every species has non-null WikipediaSlug and ThumbnailAssetPath

Negative: FindByName("NonExistentPlant") returns null · FindByName(null) returns null without throwing · FindByName("") returns null without throwing · GetSuggestedInterval("unknown_key") returns 7 (default fallback)

## Species list (40 plants, key=interval days)
Monstera deliciosa=7, Pothos=7, Snake Plant=14, Peace Lily=7, Spider Plant=7,
Rubber Plant=10, ZZ Plant=14, Aloe Vera=14, Orchid=7, Calathea=4,
Philodendron=7, Dracaena=10, English Ivy=5, Boston Fern=3, Chinese Money Plant=7,
Anthurium=7, Bird of Paradise=7, Begonia=5, Yucca=14, Croton=5,
Dumb Cane=7, Hoya=10, Prayer Plant=5, Umbrella Plant=7, African Violet=4,
Cyclamen=5, Christmas Cactus=10, Alocasia=7, Peperomia=10, Jade Plant=14,
Tradescantia=5, Weeping Fig=7, Areca Palm=5, Parlour Palm=7,
Cast Iron Plant=14, Monstera Adansonii=7, Fiddle Leaf Fig=7,
Echeveria=14, Cactus Mix=14, Flamingo Flower=7

WikipediaSlug = `"Monstera_deliciosa"` (title case underscore). ThumbnailAssetPath = `"plants/{key}_thumb.png"`.

`scripts/download-plant-images.sh` fetches Wikimedia Commons thumbnails via Wikipedia REST API → `PlantPal/Resources/Images/Plants/{key}_thumb.png` (80×80) and `{key}_detail.png` (300×300).

## Success condition
- All tests in `PlantSpeciesServiceTests.cs` pass
- `./scripts/test-all.sh` is green
- `BUILD_STATUS.md` Phase 02 checked
- Commit: `./scripts/commit-phase.sh "feat: PlantSpeciesService with 40 species and TDD coverage"`

## Deviations from plan
<!-- Fill in after completion -->
