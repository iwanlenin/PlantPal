# Phase 07: ImageCacheService — TDD

**Status:** ✅ Complete
**Branch:** `feature/phase-07-image-cache`
**Est. time:** ~50 min

---

## Goal
Species thumbnails bundled, detail images downloaded on demand and cached locally, full offline fallback.

## What to teach
Caching is a classic computer science problem: "store something expensive so you don't have to fetch it again." Our strategy: thumbnails are always offline (bundled in APK). Detail images are downloaded once then stored in CacheDirectory. If offline when first requested, show the bundled thumbnail instead. CacheDirectory is correct here because images can be re-downloaded — we never cache user data there.

## Decisions required
1. How long should cached detail images be considered "fresh" before re-downloading? **30 days recommended** — or never expire (simpler, slightly stale images possible). Confirm before proceeding.

## Files to create/modify
- `PlantPal/Services/ImageCacheService.cs`
- `PlantPal/Services/ConnectivityService.cs`
- `PlantPal/Interfaces/IHttpClientWrapper.cs`
- `PlantPal.Tests/Services/ImageCacheServiceTests.cs`

## Prior state
The following already exist — do not recreate:
- `IImageCacheService` (`PlantPal/Interfaces/IImageCacheService.cs`) from Phase 01
- `IConnectivityService` (`PlantPal/Interfaces/IConnectivityService.cs`) from Phase 01
- Species thumbnails in `PlantPal/Resources/Images/Plants/` from Phase 02

## Claude Code prompt

```
Explain to me:
1. The difference between FileSystem.AppDataDirectory and FileSystem.CacheDirectory — when to
   use each and why images belong in CacheDirectory
2. Why we mock IConnectivityService in tests rather than actually checking network status
3. What a "cache miss" vs "cache hit" is in this context

Ask me the cache expiry decision before proceeding.

TDD — Write tests first (PlantPal.Tests/Services/ImageCacheServiceTests.cs):
Mock IConnectivityService and IHttpClientWrapper.

Positive cases:
- GetDetailImageAsync when cached file exists → returns local path without HTTP request
- GetDetailImageAsync when not cached and online → downloads, saves to
  CacheDirectory/plant_images/{key}_detail.jpg, returns local path
- GetDetailImageAsync when not cached and OFFLINE → returns bundled thumbnail path,
  sets IsUsingFallback = true
- GetThumbnailPathAsync always returns local bundled asset path immediately
- GetDetailImageAsync for cached but expired file → re-downloads (if expiry was chosen)

Negative cases:
- GetDetailImageAsync when download fails (HTTP 404) → returns thumbnail fallback, no crash
- GetDetailImageAsync when download fails (network timeout) → returns thumbnail fallback, no crash
- GetDetailImageAsync(null) → throws ArgumentNullException
- GetDetailImageAsync("unknown_key") → returns generic placeholder image

Run — confirm RED.

STEP 2 — Implement:

IHttpClientWrapper (PlantPal/Interfaces/IHttpClientWrapper.cs): injectable HTTP abstraction
for testability. Task<byte[]> GetBytesAsync(string url).

ConnectivityService.cs: wraps Connectivity.NetworkAccess, implements IConnectivityService.
this. prefix, XML doc comments.

ImageCacheService.cs implements IImageCacheService:
- Constructor: IConnectivityService, IHttpClientWrapper, IPlantSpeciesService
- this. prefix on all fields, XML doc comments
- Cache path: Path.Combine(FileSystem.CacheDirectory, "plant_images")
- GetThumbnailPathAsync: returns "plants/{key}_thumb.png" (bundled resource) immediately
- GetDetailImageAsync:
  1. Check local cache → return if exists (and not expired per chosen strategy)
  2. Check connectivity → if offline: return thumbnail path
  3. Build Wikipedia image URL via https://en.wikipedia.org/api/rest_v1/page/summary/{slug}
  4. Download via IHttpClientWrapper → save to cache → return local path
  5. Any exception → return thumbnail fallback (never crash)

STEP 3 — Run tests — confirm GREEN.

Do not create any files not explicitly listed above.

Success condition: ./scripts/test-all.sh is green. Update BUILD_STATUS.md checklist.

Commit: ./scripts/commit-phase.sh "feat: ImageCacheService with offline fallback and TDD coverage"
```

## Success condition
- All tests in `ImageCacheServiceTests.cs` pass
- `./scripts/test-all.sh` is green
- `BUILD_STATUS.md` Phase 07 checked

## Deviations from plan
- `IHttpClientWrapper` placed in `PlantPal.Core/Interfaces/` (not `PlantPal/Interfaces/`) — test project only references Core
- `ImageCacheService` placed in `PlantPal.Core/Services/` (not `PlantPal/Services/`) — same testability rationale; cache base path passed as constructor `string` parameter (same pattern as `DatabaseService(string dbPath)`)
- Cache expiry: never expire (confirmed by user) — file presence on disk is the sole cache-hit check
- `HttpClientWrapper` placed in `PlantPal/Services/` as concrete MAUI wrapper
- `DashboardViewModel.OpenPlantAsync` navigation target corrected from non-existent `"PlantDetail"` to `"AddPlant"` (PlantDetail page is Phase 08)
