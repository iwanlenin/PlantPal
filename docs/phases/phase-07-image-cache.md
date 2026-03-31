# Phase 07: ImageCacheService — TDD

**Status:** ⬜ Pending
**Branch:** `feature/phase-07-image-cache`
**Est. time:** ~50 min

## Goal
Species thumbnails bundled, detail images downloaded on demand and cached locally, full offline fallback.

## Decisions required
1. How long should cached detail images be considered fresh before re-downloading? **30 days recommended** — or never expire (simpler, slightly stale images possible). Confirm before proceeding.

## Prior state
- `IImageCacheService` in `PlantPal.Core/Interfaces/IImageCacheService.cs` — do not recreate
- `IConnectivityService` in `PlantPal.Core/Interfaces/IConnectivityService.cs` — do not recreate
- Species thumbnails in `PlantPal/Resources/Images/Plants/` from Phase 02

## Files to create/modify
- `PlantPal/Services/ImageCacheService.cs`
- `PlantPal/Services/ConnectivityService.cs`
- `PlantPal/Interfaces/IHttpClientWrapper.cs`
- `PlantPal.Tests/Services/ImageCacheServiceTests.cs`

## Tests (ImageCacheServiceTests.cs)
Mock `IConnectivityService` and `IHttpClientWrapper`.

Positive: GetDetailImageAsync when cached → returns local path, no HTTP call · GetDetailImageAsync not cached + online → downloads, saves to `CacheDirectory/plant_images/{key}_detail.jpg`, returns path · GetDetailImageAsync not cached + offline → returns bundled thumbnail, sets IsUsingFallback=true · GetThumbnailPathAsync always returns bundled asset immediately · cached but expired (if expiry chosen) → re-downloads

Negative: HTTP 404 → thumbnail fallback, no crash · network timeout → thumbnail fallback, no crash · GetDetailImageAsync(null) throws ArgumentNullException · GetDetailImageAsync("unknown_key") → returns generic placeholder

## Implementation notes

**IHttpClientWrapper** (`PlantPal/Interfaces/IHttpClientWrapper.cs`): `Task<byte[]> GetBytesAsync(string url)` — injectable HTTP abstraction for testability.

**ConnectivityService.cs:** wraps `Connectivity.NetworkAccess`.

**ImageCacheService.cs:**
- Cache path: `Path.Combine(FileSystem.CacheDirectory, "plant_images")`
- `GetThumbnailPathAsync(key)` → returns `"plants/{key}_thumb.png"` immediately
- `GetDetailImageAsync(key)`:
  1. Check local cache → return if exists (and fresh per expiry choice)
  2. Check connectivity → if offline: return thumbnail path
  3. Wikipedia URL: `https://en.wikipedia.org/api/rest_v1/page/summary/{slug}`
  4. Download via `IHttpClientWrapper` → save to cache → return path
  5. Any exception → return thumbnail fallback, never crash

## Success condition
- All tests in `ImageCacheServiceTests.cs` pass
- `./scripts/test-all.sh` is green
- `BUILD_STATUS.md` Phase 07 checked
- Commit: `./scripts/commit-phase.sh "feat: ImageCacheService with offline fallback and TDD coverage"`

## Deviations from plan
<!-- Fill in after completion -->
