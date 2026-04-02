---
name: v1.0 complete — next is v2.0
description: All 10 v1.0 phases are done and merged to main. v2.0 starts at Phase 11.
type: project
---

v1.0 of PlantPal is feature-complete. All phases 01–10 are committed, PRs merged, and main is up to date (commit fdf2f5d). 74 tests, all passing.

**Why:** Completed across multiple sessions covering scaffold through final polish.

**How to apply:** Do not re-implement anything from phases 01–10. Next work is v2.0, starting with Phase 11 (Weather-Adjusted Watering). Read `BUILD_STATUS.md` and `docs/phases/README.md` to confirm state before starting.

## Completed phases summary
- **Phase 01–06 (MVP):** Scaffold, PlantSpeciesService, DatabaseService (SQLite), DashboardViewModel, Dashboard UI + AddPlant form, PermissionService + NotificationService
- **Phase 07:** ImageCacheService (Wikipedia REST API), ConnectivityService, HttpClientWrapper — TDD
- **Phase 08:** PlantDetailPage, WateringLogRepository, PlantDetailViewModel — hero image, watering history, Water Now, Edit, Delete
- **Phase 09:** Android home screen widget (AppWidgetProvider), WidgetDbQuery helper in Core (testable), 3 integration tests
- **Phase 10:** SettingsPage, PlantListPage, 3-tab TabBar, green leaf app icon, dark green splash screen, Water Now animation

## Post-v1.0 hotfixes on main
- Missing `System.ComponentModel` using in `SettingsPage.xaml.cs`
- `Shell.UnselectedColor` was `Gray200` (invisible on Windows) — fixed in `Styles.xaml` and `AppShell.xaml`
- `NavigationPage.BarTextColor` / `IconColor` were `Gray200` — back button invisible on Windows; fixed to `Black`
