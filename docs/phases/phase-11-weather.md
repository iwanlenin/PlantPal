# Phase 11: Weather-Adjusted Watering

**Status:** ⬜ Pending
**Branch:** `feature/phase-11-weather`
**Est. time:** ~90 min

## Goal
Automatically postpone outdoor plant reminders after rainfall via Open-Meteo API (free, no key required).

## Decisions required
1. Apply weather adjustment to all plants, or only outdoor locations (Balcony / Garden)? **Outdoor-only recommended** — confirm before proceeding.

## Prior state
- `IConnectivityService` + `ConnectivityService.cs` from Phase 07
- `IHttpClientWrapper` from Phase 07
- `NotificationService.cs` from Phase 06
- `SettingsPage.xaml` from Phase 10

## Files to create/modify
- `PlantPal/Interfaces/IWeatherService.cs`
- `PlantPal/Services/WeatherService.cs`
- `PlantPal.Tests/Services/WeatherServiceTests.cs`
- `PlantPal/Pages/DashboardPage.xaml` (weather badge)
- `PlantPal/Pages/SettingsPage.xaml` (weather toggle)

## Tests (WeatherServiceTests.cs)
Mock `IHttpClientWrapper` and `IConnectivityService`.

Positive: online + rain > 5mm → `GetPostponementDaysAsync` returns 1 · rain > 10mm → returns 2 · rain ≤ 5mm → returns 0

Negative: offline → returns 0 · API 500 → returns 0 · timeout → returns 0

## Implementation notes

**WeatherService.cs:**
- API: `https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&daily=precipitation_sum&forecast_days=1&past_days=1`
- Request location permission on first use · cache coordinates in `Preferences("location_lat", "location_lon")`
- Any failure returns 0 — never crash

**NotificationService.RescheduleAllAsync():** for applicable plants (outdoor if outdoor-only chosen), get `GetPostponementDaysAsync()` → schedule at `NextWaterDate + postponementDays`

**DashboardPage:** 🌧️ badge on plant card visible when `plant.IsWeatherAdjusted=true`

**SettingsPage:** toggle "Weather-aware watering" → `Preferences.Set("weather_watering", bool)`

## Success condition
- All tests pass including `WeatherServiceTests`
- `./scripts/test-all.sh` is green
- `BUILD_STATUS.md` Phase 11 checked
- Commit: `./scripts/commit-phase.sh "feat: weather-adjusted watering via Open-Meteo"`

## Deviations from plan
<!-- Fill in after completion -->
