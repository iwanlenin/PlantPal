# Phase 02: PlantSpeciesService — TDD

**Status:** ⬜ Pending
**Branch:** `feature/phase-02-species-service`
**Est. time:** ~45 min

---

## Goal
Full 40-species European houseplant list with Wikipedia slugs, watering intervals, bundled thumbnails, and full TDD coverage.

## What to teach
This is our first real TDD cycle. We write the tests first — they'll fail because there's no implementation yet (RED). Then we write PlantSpeciesService to make them pass (GREEN). Then we look at the code and ask: can it be cleaner? (REFACTOR). Writing the test first forces us to think about edge cases before we're too invested in the implementation.

## Decisions required
1. Should 'Other / Unknown' species return a default interval of 7 days, or should the interval field be mandatory?

## Files to create/modify
- `PlantPal/Models/PlantSpecies.cs`
- `PlantPal/Services/PlantSpeciesService.cs`
- `PlantPal.Tests/Services/PlantSpeciesServiceTests.cs`
- `PlantPal/Resources/Images/Plants/` (40 thumbnails — via download script)
- `scripts/download-plant-images.sh`

## Prior state
`IPlantSpeciesService` already exists in `PlantPal/Interfaces/IPlantSpeciesService.cs` from Phase 01.
Do not recreate it — only implement it.

## Claude Code prompt

```
Explain the TDD red-green-refactor cycle to me using this service as the example before
writing any code.

Then follow TDD strictly:

STEP 1 — Write tests first (PlantPal.Tests/Services/PlantSpeciesServiceTests.cs):
Write xUnit tests covering:

Positive cases:
- GetAll() returns exactly 40 species
- FindByName("Monstera") returns species with WateringIntervalDays = 7
- FindByName is case-insensitive ("monstera" == "Monstera")
- GetSuggestedInterval("monstera_deliciosa") returns 7
- Every species has a non-null WikipediaSlug
- Every species has a non-null ThumbnailAssetPath

Negative cases:
- FindByName("NonExistentPlant") returns null
- FindByName(null) returns null without throwing
- FindByName("") returns null without throwing
- GetSuggestedInterval("unknown_key") returns 7 (default fallback)

Run tests: dotnet test PlantPal.Tests/ — confirm they ALL FAIL (red). Show me the failure output.

STEP 2 — Write implementation:

PlantSpecies model (PlantPal/Models/PlantSpecies.cs):
Id (string key e.g. "monstera_deliciosa"), CommonName, LatinName, WateringIntervalDays,
WikipediaSlug, ThumbnailAssetPath, DetailAssetPath. XML doc comments on class and all properties.
Use this. prefix on all instance member access.

PlantSpeciesService (PlantPal/Services/PlantSpeciesService.cs) implements IPlantSpeciesService
with the full 40-plant European list:
Monstera deliciosa=7, Pothos=7, Snake Plant=14, Peace Lily=7, Spider Plant=7,
Rubber Plant=10, ZZ Plant=14, Aloe Vera=14, Orchid=7, Calathea=4,
Philodendron=7, Dracaena=10, English Ivy=5, Boston Fern=3, Chinese Money Plant=7,
Anthurium=7, Bird of Paradise=7, Begonia=5, Yucca=14, Croton=5,
Dumb Cane=7, Hoya=10, Prayer Plant=5, Umbrella Plant=7, African Violet=4,
Cyclamen=5, Christmas Cactus=10, Alocasia=7, Peperomia=10, Jade Plant=14,
Tradescantia=5, Weeping Fig=7, Areca Palm=5, Parlour Palm=7,
Cast Iron Plant=14, Monstera Adansonii=7, Fiddle Leaf Fig=7,
Echeveria=14, Cactus Mix=14, Flamingo Flower=7

For each species: set WikipediaSlug (e.g. "Monstera_deliciosa") and
ThumbnailAssetPath ("plants/{key}_thumb.png"). XML doc comments on class and all public members.
Use this. prefix on all instance member access.

Also create scripts/download-plant-images.sh that fetches Wikimedia Commons thumbnails
for all 40 species using their Wikipedia slugs via the Wikipedia REST API.
Save to PlantPal/Resources/Images/Plants/ as {key}_thumb.png (80x80) and {key}_detail.png (300x300).
Explain what the script does before running it.

STEP 3 — Run tests again: confirm all GREEN. Show me the output.
STEP 4 — Review the implementation. Ask me: is there anything about the service design you want
to change before we move on?

Do not create any files not explicitly listed above.

Success condition: ./scripts/test-all.sh is green. Update BUILD_STATUS.md checklist.

Commit: ./scripts/commit-phase.sh "feat: PlantSpeciesService with 40 species and TDD coverage"
```

## Success condition
- All tests in `PlantSpeciesServiceTests.cs` pass
- `./scripts/test-all.sh` is green
- `BUILD_STATUS.md` Phase 02 checked

## Deviations from plan
<!-- Fill in after completion -->
