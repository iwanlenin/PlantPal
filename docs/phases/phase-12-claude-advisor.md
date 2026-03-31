# Phase 12: Claude Plant Advisor

**Status:** ⬜ Pending
**Branch:** `feature/phase-12-claude-advisor`
**Est. time:** ~60 min

## Goal
In-app chat with Claude about plant care. API key stored in device keychain (SecureStorage).

## Decisions required
1. Should chat history persist between sessions (SQLite), or start fresh each time?

## Prior state
- `IHttpClientWrapper` from Phase 07
- `PlantDetailPage.xaml` from Phase 08 (add "Ask Claude" button)

## Files to create/modify
- `PlantPal/Interfaces/IPlantAdvisorService.cs`
- `PlantPal/Services/PlantAdvisorService.cs`
- `PlantPal/ViewModels/PlantAdvisorViewModel.cs`
- `PlantPal/Pages/PlantAdvisorPage.xaml` + `.xaml.cs`
- `PlantPal.Tests/Services/PlantAdvisorServiceTests.cs`
- `PlantPal/Pages/PlantDetailPage.xaml` (add "Ask Claude 🤖" button)

## Tests (PlantAdvisorServiceTests.cs)
Mock `IHttpClientWrapper`.

Positive: `AskAboutPlantAsync("Monstera", "Why yellow leaves?")` returns non-empty string · API key set → `IsConfigured=true`

Negative: no API key → `IsConfigured=false`, `AskAboutPlantAsync` returns "Please add your Anthropic API key in Settings" · API 401 → "API key invalid — check Settings" · timeout → "Couldn't reach Claude — check your connection" · API 500 → graceful message, no crash

## Implementation notes

**IPlantAdvisorService:**
- `Task<string> AskAboutPlantAsync(string species, string question)`
- `Task SetApiKeyAsync(string key)` / `Task<string?> GetApiKeyAsync()`
- `bool IsConfigured`

**PlantAdvisorService.cs:**
- API key via `SecureStorage.GetAsync/SetAsync("anthropic_api_key")` — never Preferences (unencrypted)
- POST to `https://api.anthropic.com/v1/messages` with `claude-sonnet-4-20250514`, plant species in system prompt

**PlantAdvisorPage.xaml:**
- Chat bubble CollectionView (user messages right / Claude left)
- Quick question chips: "Is my schedule right?" · "Leaves yellowing?" · "Which fertilizer?"
- API key setup row (if not configured): "Add API key →" opens SecureEntry dialog
- Text input + send button, disabled while `IsLoading`
- Plant name at top

## Success condition
- All tests pass including `PlantAdvisorServiceTests`
- `./scripts/test-all.sh` is green
- `BUILD_STATUS.md` Phase 12 checked
- Commit: `./scripts/commit-phase.sh "feat: Claude Plant Advisor with secure API key storage"`

## Deviations from plan
<!-- Fill in after completion -->
