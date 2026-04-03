---
name: Phase 12 done — next is Phase 13
description: Phases 01–12 complete and on main. 88 tests passing. Next is Phase 13 (Cloud Sync).
type: project
---

PlantPal v1.0 (phases 01–10) and v2.0 Phases 11–12 are complete. All PRs merged, main is up to date. 88 tests passing.

**Why:** Completed across multiple sessions.

**How to apply:** Do not re-implement anything from phases 01–12. Next work is Phase 13 (Cloud Sync). Read `BUILD_STATUS.md` and `docs/phases/phase-13-cloud-sync.md` to confirm state before starting.

## Completed phases summary
- **Phase 01–06 (MVP):** Scaffold, PlantSpeciesService, DatabaseService (SQLite), DashboardViewModel, Dashboard UI + AddPlant form, PermissionService + NotificationService
- **Phase 07:** ImageCacheService (Wikipedia REST API), ConnectivityService, HttpClientWrapper — TDD
- **Phase 08:** PlantDetailPage, WateringLogRepository, PlantDetailViewModel — hero image, watering history, Water Now, Edit, Delete
- **Phase 09:** Android home screen widget (AppWidgetProvider), WidgetDbQuery helper in Core (testable), 3 integration tests
- **Phase 10:** SettingsPage, PlantListPage, 3-tab TabBar, green leaf app icon, dark green splash screen, Water Now animation
- **Phase 11:** WeatherService (Open-Meteo API), weather-adjusted notifications for outdoor plants (Balcony/Garden), 🌧️ badge on dashboard, Settings toggle
- **Phase 12:** Claude Plant Advisor — in-app chat per plant, SecureStorage API key, SQLite chat history, adjustable context window (10/20/50/All) with cost hint

## Post-v1.0 hotfixes on main
- Missing `System.ComponentModel` using in `SettingsPage.xaml.cs`
- `Shell.UnselectedColor` was `Gray200` (invisible on Windows) — fixed in `Styles.xaml` and `AppShell.xaml`
- `NavigationPage.BarTextColor` / `IconColor` were `Gray200` — back button invisible on Windows; fixed to `Black`

## Phase 12 notes
- `ISecureStorageService` added to Core to abstract MAUI SecureStorage for testability
- `test-all.sh` updated: build → unblock DLLs (Windows App Control policy) → test --no-build
