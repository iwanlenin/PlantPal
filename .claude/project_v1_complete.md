---
name: Phase 11 done — next is Phase 12
description: Phases 01–11 complete and on main. 81 tests passing. Next is Phase 12 (Claude Plant Advisor).
type: project
---

PlantPal v1.0 (phases 01–10) and v2.0 Phase 11 are complete. All PRs merged, main is up to date. 81 tests passing.

**Why:** Completed across multiple sessions.

**How to apply:** Do not re-implement anything from phases 01–11. Next work is Phase 12 (Claude Plant Advisor). Read `BUILD_STATUS.md` and `docs/phases/phase-12-claude-advisor.md` to confirm state before starting.

## Completed phases summary
- **Phase 01–06 (MVP):** Scaffold, PlantSpeciesService, DatabaseService (SQLite), DashboardViewModel, Dashboard UI + AddPlant form, PermissionService + NotificationService
- **Phase 07:** ImageCacheService (Wikipedia REST API), ConnectivityService, HttpClientWrapper — TDD
- **Phase 08:** PlantDetailPage, WateringLogRepository, PlantDetailViewModel — hero image, watering history, Water Now, Edit, Delete
- **Phase 09:** Android home screen widget (AppWidgetProvider), WidgetDbQuery helper in Core (testable), 3 integration tests
- **Phase 10:** SettingsPage, PlantListPage, 3-tab TabBar, green leaf app icon, dark green splash screen, Water Now animation
- **Phase 11:** WeatherService (Open-Meteo API), weather-adjusted notifications for outdoor plants (Balcony/Garden), 🌧️ badge on dashboard, Settings toggle

## Post-v1.0 hotfixes on main
- Missing `System.ComponentModel` using in `SettingsPage.xaml.cs`
- `Shell.UnselectedColor` was `Gray200` (invisible on Windows) — fixed in `Styles.xaml` and `AppShell.xaml`
- `NavigationPage.BarTextColor` / `IconColor` were `Gray200` — back button invisible on Windows; fixed to `Black`
