# PlantPal — Package & Environment Reference

## Development Environment

| Component | Version | Installed | Description |
|---|---|---|---|
| OS | Windows 11 Pro | Yes | Host operating system |
| IDE | Visual Studio 2022 | Yes | Primary IDE |
| .NET SDK | .NET 9 | Yes | SDK for building MAUI apps |
| Target Framework | .NET MAUI 9 | Yes | Cross-platform mobile framework |
| Shell / CI Runner | bash (GitHub Actions) | No | CI pipeline (not yet configured) |

---

## NuGet Packages

### PlantPal (main app)

| Package | Version | Installed | Description |
|---|---|---|---|
| CommunityToolkit.Mvvm | — | No | MVVM source generators (`[ObservableProperty]`, `[RelayCommand]`) |
| sqlite-net-pcl | — | No | Local SQLite database access |
| Plugin.LocalNotification | — | No | Local notification scheduling |
| MauiIcons.Material | — | No | Material icon font for MAUI |

### PlantPal.Tests

| Package | Version | Installed | Description |
|---|---|---|---|
| xUnit | — | No | Unit test framework |
| NSubstitute | — | No | Mocking library |
| Microsoft.NET.Test.Sdk | — | No | .NET test host |
| xunit.runner.visualstudio | — | No | VS Test Explorer integration |

---

## Notes

- Update Version and Installed columns as packages are added via NuGet / `dotnet add package`.
- Keep this file updated whenever a package is added, upgraded, or removed.
