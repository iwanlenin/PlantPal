# Phase 03: Database Layer — TDD

**Status:** ⬜ Pending
**Branch:** `feature/phase-03-database-service`
**Est. time:** ~40 min

## Goal
SQLite PlantRepository with full CRUD, TDD coverage including edge cases.

## Decisions required
1. Should `SaveAsync` throw an exception if validation fails (e.g. empty Name), or return a bool? **Exception is cleaner for TDD** — confirm before proceeding.

## Prior state
- `IPlantRepository` in `PlantPal.Core/Interfaces/IPlantRepository.cs` — do not recreate
- `Plant.cs` with `RecalculateNextWaterDate()` in `PlantPal.Core/Models/Plant.cs` — do not recreate

## Files to create/modify
- `PlantPal/Services/DatabaseService.cs`
- `PlantPal.Tests/Services/PlantRepositoryTests.cs`
- `PlantPal/MauiProgram.cs` (replace stub registration)

## Tests (PlantRepositoryTests.cs)
Use real in-memory SQLite (`:memory:`) — acceptable here because this IS the DB layer.

Positive: SaveAsync then GetAllAsync returns that plant · SaveAsync(existing) updates — GetByIdAsync returns new name · DeleteAsync then GetAllAsync does not contain plant · GetByIdAsync(validId) returns correct plant · SaveAsync sets NextWaterDate via RecalculateNextWaterDate() · GetAllAsync on empty DB returns empty list (not null)

Negative: GetByIdAsync(-1) returns null without throwing · GetByIdAsync(99999) returns null without throwing · SaveAsync(null Name) throws ArgumentException · DeleteAsync(nonexistent id) completes without throwing

## Implementation notes
- Constructor takes `dbPath` string, creates SQLite connection
- `CreateTableAsync<Plant>()` on initialization
- `SaveAsync`: validate `plant.Name` is not null/empty (throw `ArgumentException`), call `RecalculateNextWaterDate()`, then `InsertOrReplaceAsync`
- `MauiProgram.cs` registration: `var dbPath = Path.Combine(FileSystem.AppDataDirectory, "plantpal.db"); builder.Services.AddSingleton<IPlantRepository>(new DatabaseService(dbPath));`

## Success condition
- All tests in `PlantRepositoryTests.cs` pass
- `./scripts/test-all.sh` is green
- `BUILD_STATUS.md` Phase 03 checked
- Commit: `./scripts/commit-phase.sh "feat: SQLite DatabaseService with TDD coverage"`

## Deviations from plan
<!-- Fill in after completion -->
