# PlantPal — Package & Environment Reference

## Development Environment

| Component | Version | Installed | Description |
|---|---|---|---|
| OS | Windows 11 Pro | Yes | Host operating system |
| IDE | Visual Studio 2022 | Yes | Primary IDE |
| .NET SDK | .NET 10.0.201 | Yes | SDK for building MAUI apps |
| Target Framework | .NET MAUI 10 | Yes | Cross-platform mobile framework |
| Shell / CI Runner | bash (GitHub Actions) | Yes | CI pipeline (ci.yml) |

---

## NuGet Packages

### PlantPal.Core (interfaces + models)

| Package | Version | Installed | Description |
|---|---|---|---|
| sqlite-net-pcl | 1.9.172 | Yes | SQLite ORM — model attributes and `SQLiteAsyncConnection` for DatabaseService |

### PlantPal (main app)

| Package | Version | Installed | Description |
|---|---|---|---|
| CommunityToolkit.Mvvm | 8.4.2 | Yes | MVVM source generators (`[ObservableProperty]`, `[RelayCommand]`) |
| CommunityToolkit.Maui | 14.0.1 | Yes | MAUI UI toolkit (snackbar, animations, etc.) |
| sqlite-net-pcl | 1.9.172 | Yes | Local SQLite database access |
| Plugin.LocalNotification | 14.1.0 | Yes | Local notification scheduling |
| MauiIcons.Material | — | No | Material icon font for MAUI |

### PlantPal.Tests

| Package | Version | Installed | Description |
|---|---|---|---|
| xunit | 2.9.3 | Yes | Unit test framework |
| xunit.runner.visualstudio | 3.1.4 | Yes | VS Test Explorer integration |
| NSubstitute | 5.3.0 | Yes | Mocking library |
| Microsoft.NET.Test.Sdk | 17.14.1 | Yes | .NET test host |
| coverlet.collector | 6.0.4 | Yes | Code coverage collection |

---

## Notes

- Update Version and Installed columns as packages are added via NuGet / `dotnet add package`.
- Keep this file updated whenever a package is added, upgraded, or removed.
- The SQLite Alpine RID warning (NETSDK1206) is harmless — not relevant for Android/iOS targets.
