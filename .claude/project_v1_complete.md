---
name: Phase 14 done — all phases complete
description: Phases 01–14 complete and on main. 96 tests passing. All v2.0 features shipped.
type: project
---

PlantPal v1.0 (phases 01–10) and v2.0 Phases 11–14 are complete. All PRs merged, main is up to date. 96 tests passing.

**Why:** Completed across multiple sessions.

**How to apply:** Do not re-implement anything from phases 01–14. Read `BUILD_STATUS.md` to confirm state before planning any new work.

## Completed phases summary
- **Phase 01–06 (MVP):** Scaffold, PlantSpeciesService, DatabaseService (SQLite), DashboardViewModel, Dashboard UI + AddPlant form, PermissionService + NotificationService
- **Phase 07:** ImageCacheService (Wikipedia REST API), ConnectivityService, HttpClientWrapper — TDD
- **Phase 08:** PlantDetailPage, WateringLogRepository, PlantDetailViewModel — hero image, watering history, Water Now, Edit, Delete
- **Phase 09:** Android home screen widget (AppWidgetProvider), WidgetDbQuery helper in Core (testable), 3 integration tests
- **Phase 10:** SettingsPage, PlantListPage, 3-tab TabBar, green leaf app icon, dark green splash screen, Water Now animation
- **Phase 11:** WeatherService (Open-Meteo API), weather-adjusted notifications for outdoor plants (Balcony/Garden), 🌧️ badge on dashboard, Settings toggle
- **Phase 12:** Claude Plant Advisor — in-app chat per plant, SecureStorage API key, SQLite chat history, adjustable context window (10/20/50/All) with cost hint
- **Phase 13:** Supabase cloud sync — offline-first, last-write-wins on UpdatedAt, SyncId UUID per record, email/password auth, AccountPage, auto-sync on connectivity change
- **Phase 14:** Plant health photo diagnosis — camera/gallery in Ask Claude chat, Anthropic vision API (base64 image block), full diagnosis (pests/disease/overwatering/nutrients)

## Post-v1.0 hotfixes on main
- Missing `System.ComponentModel` using in `SettingsPage.xaml.cs`
- `Shell.UnselectedColor` was `Gray200` (invisible on Windows) — fixed in `Styles.xaml` and `AppShell.xaml`
- `NavigationPage.BarTextColor` / `IconColor` were `Gray200` — back button invisible on Windows; fixed to `Black`

## Phase 14 notes
- `IMediaPickerService` abstraction in Core; `MediaPickerService` MAUI concrete — returns null on cancel/deny (never crashes)
- Vision request uses same `IHttpClientWrapper.PostStringAsync`; `BuildVisionRequestBody` constructs mixed-content array
- Photos not stored in SQLite — too large; chat history stores "📷 ..." display text instead
- `PlantAdvisorViewModel.SendAsync` routes to `DiagnosePlantAsync` when `HasAttachedPhoto` is true

## Phase 13 notes
- Supabase project ref: `ldpuwqtergmajiedkyqo`; tables: `plants`, `watering_logs` with RLS
- Supabase tables created via Management API (PAT provided by user)
- MCP server config in `.mcp.json` with PAT header for direct DB access in future sessions
- `SupabaseClientService` uses plain HTTP (System.Text.Json) — no supabase-csharp dependency
- sqlite-net-pcl auto-migrates new columns (SyncId, UpdatedAt) on existing user devices
- `IWateringLogRepository` gained `GetAllAsync()` for sync

## Phase 12 notes
- `ISecureStorageService` added to Core to abstract MAUI SecureStorage for testability
- `test-all.sh` updated: build → unblock DLLs (Windows App Control policy) → test --no-build
