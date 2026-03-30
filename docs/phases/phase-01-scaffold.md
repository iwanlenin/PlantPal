# Phase 01: Solution Scaffold, Interfaces & Test Project

**Status:** ⬜ Pending
**Branch:** `feature/phase-01-scaffold`
**Est. time:** ~45 min

---

## Goal
Solution structure, all interfaces defined, test project wired, NuGet packages installed.

## What to teach
We define ALL interfaces before writing a single implementation. This is the foundation of both TDD and good architecture. Every service the app uses will be hidden behind an interface — this means we can swap implementations, mock them in tests, and never couple ViewModels to platform code. Think of interfaces as contracts: "I don't care HOW you store a plant, just that you can Save, Get, Delete it."

## Decisions required
1. Confirm: xUnit + NSubstitute for tests (recommended) or NUnit + Moq?
2. Should the solution also include a shared Models project, or keep models inside the main MAUI project?

## Files to create/modify
- `PlantPal.sln`
- `PlantPal/Interfaces/IPlantRepository.cs`
- `PlantPal/Interfaces/IWateringLogRepository.cs`
- `PlantPal/Interfaces/INotificationService.cs`
- `PlantPal/Interfaces/IPermissionService.cs`
- `PlantPal/Interfaces/IImageCacheService.cs`
- `PlantPal/Interfaces/IConnectivityService.cs`
- `PlantPal/Interfaces/IPlantSpeciesService.cs`
- `PlantPal/Interfaces/INavigationService.cs`
- `PlantPal/Models/Plant.cs`
- `PlantPal/Models/PermissionResult.cs`
- `PlantPal.Tests/PlantPal.Tests.csproj`
- `PlantPal/MauiProgram.cs`

## Claude Code prompt

```
Before writing anything, explain to me:
1. What a .NET MAUI solution structure looks like and why we separate concerns into folders
2. What an interface is and why every service we write will have one
3. What TDD means and how the red-green-refactor cycle works

Then ask me to confirm the test framework choice (xUnit + NSubstitute is the recommendation —
explain briefly why before asking).

Once confirmed, create:

SOLUTION:
- PlantPal.sln containing two projects: PlantPal (MAUI app) and PlantPal.Tests (.NET class library)
- Install in PlantPal: sqlite-net-pcl, Plugin.LocalNotification, CommunityToolkit.Mvvm, CommunityToolkit.Maui
- Install in PlantPal.Tests: xunit, xunit.runner.visualstudio, NSubstitute, Microsoft.NET.Test.Sdk, coverlet.collector

MODELS (PlantPal/Models/):
- Plant.cs: Id, Name, Species, Location, WateringIntervalDays, LastWateredDate (DateTime?),
  NextWaterDate (DateTime?), PhotoPath (string?)
  - XML doc comment on class and every property
  - this. prefix on all instance member access
  - Method: RecalculateNextWaterDate() — sets this.NextWaterDate = this.LastWateredDate?.AddDays(this.WateringIntervalDays)
- PermissionResult.cs: enum with Granted, Denied, DeniedPermanently

INTERFACES (PlantPal/Interfaces/) — XML doc comment on every interface and method:
- IPlantRepository: Task<List<Plant>> GetAllAsync(), Task<Plant?> GetByIdAsync(int id),
  Task SaveAsync(Plant plant), Task DeleteAsync(int id)
- IWateringLogRepository: Task<List<WateringLog>> GetByPlantIdAsync(int plantId),
  Task SaveAsync(WateringLog log), Task DeleteByPlantIdAsync(int plantId)
- INotificationService: Task ScheduleReminderAsync(Plant plant), Task CancelReminderAsync(int plantId),
  Task RescheduleAllAsync(List<Plant> plants), bool AreNotificationsEnabled
- IPermissionService: Task<PermissionResult> CheckNotificationPermissionAsync(),
  Task<PermissionResult> RequestNotificationPermissionAsync(),
  Task<PermissionResult> CheckPhotoPermissionAsync(), Task<PermissionResult> RequestPhotoPermissionAsync()
- IImageCacheService: Task<string> GetThumbnailPathAsync(string speciesKey),
  Task<string> GetDetailImageAsync(string speciesKey)
- IConnectivityService: bool IsConnected
- IPlantSpeciesService: IReadOnlyList<PlantSpecies> GetAll(), PlantSpecies? FindByName(string name),
  int GetSuggestedInterval(string speciesKey)
- INavigationService: Task NavigateToAsync(string route, Dictionary<string,object>? parameters = null),
  Task GoBackAsync()

Register all interfaces in MauiProgram.cs as placeholders (empty stub implementations for now —
we'll replace phase by phase).

Do not create any files not explicitly listed above.

Success condition: dotnet build PlantPal.Tests/PlantPal.Tests.csproj succeeds and
./scripts/test-all.sh runs (even with 0 tests). Update BUILD_STATUS.md checklist.

Commit: git checkout -b feature/phase-01-scaffold && ./scripts/commit-phase.sh "feat: solution scaffold, interfaces, and test project"
```

## Success condition
- `dotnet build PlantPal.Tests/` succeeds
- `./scripts/test-all.sh` runs without error (0 tests is fine at this stage)
- All interfaces exist with correct signatures
- `BUILD_STATUS.md` Phase 01 checked

## Deviations from plan
<!-- Fill in after completion -->
