# Phase 11: Weather-Adjusted Watering

**Status:** ⬜ Pending
**Branch:** `feature/phase-11-weather`
**Est. time:** ~90 min

---

## Goal
Automatically postpone outdoor plant reminders after rainfall via Open-Meteo API (free, no key required).

## What to teach
This phase introduces an external API dependency. The key architecture decision: weather adjustment happens in the notification scheduling layer, not in the data layer. We don't change `NextWaterDate` in the DB — we only shift the notification time. This way the "real" schedule is preserved if the user turns weather-adjustment off.

## Decisions required
1. Should weather adjustment apply to all plants, or only outdoor locations (Balcony / Garden)? **Outdoor-only recommended** — confirm before proceeding.

## Files to create/modify
- `PlantPal/Interfaces/IWeatherService.cs`
- `PlantPal/Services/WeatherService.cs`
- `PlantPal.Tests/Services/WeatherServiceTests.cs`
- `PlantPal/Pages/DashboardPage.xaml` (weather badge on plant cards)
- `PlantPal/Pages/SettingsPage.xaml` (weather toggle)

## Prior state
The following already exist — do not recreate:
- `IConnectivityService` and `ConnectivityService.cs` from Phase 07
- `IHttpClientWrapper` from Phase 07
- `NotificationService.cs` from Phase 06
- `SettingsPage.xaml` from Phase 10

## Claude Code prompt

```
Explain: why do we adjust the notification time rather than the stored NextWaterDate in the DB?
What would break if we modified the DB record instead?

Ask me the outdoor-only decision.

TDD first (PlantPal.Tests/Services/WeatherServiceTests.cs):
Mock IHttpClientWrapper and IConnectivityService.

Positive cases:
- Online + rain > 5mm today → GetPostponementDaysAsync returns 1
- Online + rain > 10mm today → GetPostponementDaysAsync returns 2
- Online + rain <= 5mm → GetPostponementDaysAsync returns 0

Negative cases:
- Offline → returns 0 days (graceful, no crash)
- API returns 500 → returns 0 days (graceful fallback)
- API timeout → returns 0 days (graceful fallback)

Run — confirm RED. Implement. Confirm GREEN.

WeatherService.cs implements IWeatherService:
- Constructor: IHttpClientWrapper, IConnectivityService
- this. prefix, XML doc comments
- Uses Open-Meteo: https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&daily=precipitation_sum&forecast_days=1&past_days=1
- Request location permission, cache coordinates in Preferences ("location_lat", "location_lon")
- Any failure returns 0 (never crash)

Update NotificationService.RescheduleAllAsync():
- For applicable plants (outdoor locations if outdoor-only chosen), get postponement days
  from IWeatherService
- Apply offset before scheduling: schedule at NextWaterDate + postponementDays days

Add to DashboardPage plant cards: 🌧️ weather badge visible when plant.IsWeatherAdjusted = true.
Add PlantDetailViewModel.IsWeatherAdjusted ObservableProperty.

Add Settings toggle: "Weather-aware watering" (Preferences.Set("weather_watering", true/false)).

Do not create any files not explicitly listed above.

Success condition: ./scripts/test-all.sh is green. Update BUILD_STATUS.md checklist.

Commit: ./scripts/commit-phase.sh "feat: weather-adjusted watering via Open-Meteo"
```

## Success condition
- All tests pass including `WeatherServiceTests`
- `./scripts/test-all.sh` is green
- `BUILD_STATUS.md` Phase 11 checked

## Deviations from plan
<!-- Fill in after completion -->
