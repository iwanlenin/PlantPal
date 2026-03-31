# PlantPal — Claude Code Project Context

## Memory
See [.claude/MEMORY.md](./.claude/MEMORY.md) for the index of all remembered rules, feedback, and project context.
Read it at the start of every session. Update it whenever a new rule or preference is established.

## Package & environment reference
See [PACKAGES.md](./PACKAGES.md) for all NuGet packages, versions, and dev environment details.
Keep that file updated whenever packages are added or changed.

---

## What this project is
PlantPal is a .NET MAUI mobile app for Android and iOS that helps users track
watering schedules for indoor plants. Users add plants, log waterings, and
receive reminders when plants need water.

---

## Tech stack
| Concern | Choice |
|---|---|
| Framework | .NET MAUI 10, targeting Android and iOS |
| Language | C# |
| Architecture | MVVM — CommunityToolkit.Mvvm |
| Local DB | SQLite via sqlite-net-pcl |
| Notifications | Plugin.LocalNotification |
| Icons | MauiIcons.Material |
| Tests | xUnit + NSubstitute (confirmed) |
| CI | GitHub Actions |
| Models location | Inside main MAUI project — no shared Models project (confirmed) |

---

## Architecture rules — always follow these

### Interface-first
Every service has an interface in `PlantPal/Interfaces/`. ViewModels only ever
reference interfaces, never concrete implementations. MAUI registers concretes
in `MauiProgram.cs`.

### MVVM
- Pages contain zero business logic — only XAML bindings and lifecycle calls
- ViewModels use `[ObservableProperty]` and `[RelayCommand]` from CommunityToolkit.Mvvm
- Navigation goes through `INavigationService`

### TDD — red → green → refactor
1. Write failing tests first
2. Confirm they fail (show output)
3. Write implementation to pass them
4. Confirm all green (show output)
5. Refactor if needed

Test project: `PlantPal.Tests/` — plain .NET class library, no MAUI references.
Use NSubstitute for mocks. Tests must cover positive cases, negative cases,
and permission-denied / offline edge cases.

### Permission strategy
The app never crashes or breaks when permissions are denied. Treat denied
permissions as a valid app state:
- Notifications denied → silent no-op on scheduling, dashboard shows banner
- Photo access denied → graceful snackbar, plant saved without photo
- All permission checks go through `IPermissionService`

### Image caching strategy
- Species thumbnails: bundled in APK (`Resources/Images/Plants/`)
- Detail images: downloaded from Wikipedia REST API on first view, cached to
  `FileSystem.CacheDirectory/plant_images/`
- Offline + no cache: show bundled thumbnail, display "No internet" banner
- Never crash on missing image — always fall back

---

## Coding standards — always apply

- `this.` prefix on all private and instance member access in C#
- XML doc comments on every class and every public method
- One concern per prompt — never combine unrelated features in one session
- Every phase ends with a passing test run before committing

## Development workflow — always follow this order

1. **Plan mode first** — before writing any code, enter Plan mode (`/plan`). Explore the codebase, design the approach, confirm with the user.
2. **Development second** — only begin implementation after the plan has been reviewed and approved.

Never skip planning and jump straight to coding, even for small changes.

## Assumptions policy — strictly enforced

- Never assume anything that has not been explicitly confirmed by the user
- Do not assume a package is installed unless PACKAGES.md marks it as Yes
- Do not assume a phase is complete unless the build status checklist is checked
- Do not assume a file, interface, or feature exists unless you have read it yourself in this session
- When in doubt, ask — do not guess and proceed

## Command explanation policy — strictly enforced

- Before running any shell command, explain in plain English: what it does, what it changes, and why it is needed at this point
- This applies to every Bash invocation: git, dotnet, winget, gh, curl, and any other CLI tool
- Never run a command silently or without prior explanation

---

## Project structure
```
PlantPal.Core/        # plain net10.0 classlib — referenced by both app and tests
  Interfaces/         # All service interfaces (IPlantRepository, INotificationService, etc.)
  Models/             # Plain C# models (Plant, WateringLog, PlantSpecies, PermissionResult)
PlantPal/             # .NET MAUI 10 app (net10.0-android;net10.0-ios)
  Services/           # Concrete implementations of PlantPal.Core interfaces
  ViewModels/         # One ViewModel per page
  Pages/              # XAML pages (UI only, no logic)
  Resources/
    Images/Plants/    # Bundled species thumbnails
    AppIcon/          # App icon assets
  Platforms/
    Android/          # Android-specific code (widget, notifications)
    iOS/              # iOS-specific code
PlantPal.Tests/       # plain net10.0 xUnit project — references PlantPal.Core only
  Services/           # Service unit tests
  ViewModels/         # ViewModel unit tests
  Integration/        # Integration tests (e.g. widget DB query)
scripts/
  test-all.sh         # Run full test suite
  watch-tests.sh      # Watch mode — re-runs on file save
  commit-phase.sh     # Run tests then commit and push
  new-branch.sh       # Create feature branch
```

---

## Key interfaces (defined in PlantPal.Core/Interfaces/ — do not change signatures without asking)
- `IPlantRepository` — CRUD for Plant
- `IWateringLogRepository` — CRUD for WateringLog
- `INotificationService` — schedule / cancel reminders
- `IPermissionService` — check and request permissions
- `IImageCacheService` — get thumbnail and detail images
- `IConnectivityService` — wrap network access
- `IPlantSpeciesService` — 40-species European houseplant list
- `INavigationService` — Shell navigation wrapper

---

## Plant species list
40 common European indoor houseplants with suggested watering intervals.
Defined in `PlantSpeciesService`. Examples:
- Monstera deliciosa = 7 days
- Cactus mix = 14 days
- Boston Fern = 3 days
- Calathea = 4 days
See full list in `PlantPal/Services/PlantSpeciesService.cs`.

---

## Git workflow
- `main` = always green (CI must pass)
- Feature branches: `feature/phase-name`
- Commit format: `feat:`, `fix:`, `test:`, `docs:`, `chore:`
- Never commit with failing tests — `commit-phase.sh` enforces this

---

## How to run tests
```bash
./scripts/test-all.sh        # run once
./scripts/watch-tests.sh     # watch mode
```

## How to run the app
Use Visual Studio 2022. Select target (Android Emulator / Windows) from the
toolbar dropdown and press F5. When VS prompts "files modified outside the
editor" after Claude Code edits — click Reload All.

---

## Current build status
See [BUILD_STATUS.md](./BUILD_STATUS.md) for the full phase checklist.

---

## What to do at the start of every session
1. Read this file (you are doing that now)
2. Check [BUILD_STATUS.md](./BUILD_STATUS.md) to know where we are
3. Ask me to confirm which phase we are working on before writing any code
4. Create a feature branch if one does not exist: `./scripts/new-branch.sh "feature/phase-name"`
