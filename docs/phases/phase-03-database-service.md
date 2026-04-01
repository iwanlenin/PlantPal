# Phase 03: Database Layer — TDD

**Status:** ✅ Complete
**Branch:** `feature/phase-03-database-service`
**Est. time:** ~40 min

---

## Goal
SQLite PlantRepository with full CRUD, TDD coverage including edge cases.

## What to teach
Repository pattern separates "how data is stored" from "what the app does with data". ViewModels call `IPlantRepository.SaveAsync()` without knowing or caring that SQLite is underneath. In tests, we swap in a fake (mock) repository — tests run in milliseconds with no real database file. This is why interfaces matter.

## Decisions required
1. SQLite runs on a background thread — should `SaveAsync` throw an exception if validation fails (e.g. empty Name), or return a bool success result? **Exception is cleaner for TDD** — confirm before proceeding.

## Files to create/modify
- `PlantPal.Core/Services/DatabaseService.cs`
- `PlantPal.Tests/Services/PlantRepositoryTests.cs`
- `PlantPal/MauiProgram.cs` (replaced stub with real DatabaseService registration)

## Prior state
`IPlantRepository` already exists in `PlantPal/Interfaces/IPlantRepository.cs` from Phase 01.
`Plant.cs` already exists with `RecalculateNextWaterDate()` from Phase 01.
Do not recreate these — only implement and reference them.

## Claude Code prompt

```
Explain to me: what is the Repository pattern and why does it matter? How does it differ from
accessing SQLite directly in a ViewModel? Show me a concrete before/after code example
(pseudocode) illustrating the difference.

Then TDD:

STEP 1 — Write tests first (PlantPal.Tests/Services/PlantRepositoryTests.cs):
Note: DatabaseService tests use a real in-memory SQLite DB (pass ":memory:" as path).
This is an integration test for the DB layer — explain why this is acceptable here versus mocks.

Positive cases:
- SaveAsync(plant) then GetAllAsync() returns list containing that plant
- SaveAsync(existingPlant with modified Name) updates — GetByIdAsync returns updated name
- DeleteAsync(id) then GetAllAsync() does not contain deleted plant
- GetByIdAsync(validId) returns correct plant
- SaveAsync sets NextWaterDate correctly via RecalculateNextWaterDate()
- GetAllAsync on empty DB returns empty list (not null)

Negative cases:
- GetByIdAsync(-1) returns null without throwing
- GetByIdAsync(99999) returns null without throwing
- SaveAsync(plant with null Name) throws ArgumentException
- DeleteAsync(nonexistent id) completes without throwing

Run tests — confirm RED.

STEP 2 — Implement PlantPal/Services/DatabaseService.cs:
- Implements IPlantRepository
- XML doc comments on class and every method
- Constructor takes dbPath string, creates SQLite connection
- Use this.connection for the SQLite connection field
- this. prefix on all private and instance member access
- CreateTableAsync<Plant>() on initialization
- SaveAsync: validate plant.Name is not null/empty (throw ArgumentException if so),
  call RecalculateNextWaterDate(), then InsertOrReplaceAsync
- All methods async

Register in MauiProgram.cs:
var dbPath = Path.Combine(FileSystem.AppDataDirectory, "plantpal.db");
builder.Services.AddSingleton<IPlantRepository>(new DatabaseService(dbPath));

STEP 3 — Run tests — confirm GREEN.
STEP 4 — Show me the test coverage percentage. Explain what "code coverage" means and what a
good target is.

Do not create any files not explicitly listed above.

Success condition: ./scripts/test-all.sh is green. Update BUILD_STATUS.md checklist.

Commit: ./scripts/commit-phase.sh "feat: SQLite DatabaseService with TDD coverage"
```

## Success condition
- All tests in `PlantRepositoryTests.cs` pass
- `./scripts/test-all.sh` is green
- `BUILD_STATUS.md` Phase 03 checked

## Deviations from plan
- `DatabaseService` placed in `PlantPal.Core/Services/` (not `PlantPal/Services/`) — no MAUI dependencies, test project references Core only
- Tests use per-test temp files instead of `:memory:` — SQLiteAsyncConnection pools `:memory:` connections causing shared state across tests
- `SaveAsync` uses separate `Insert`/`Update` instead of `InsertOrReplaceAsync` — avoids SQLite auto-replacing on key collision
- `SaveAsync` throws `ArgumentException` on null/empty Name — confirmed as cleaner for TDD
