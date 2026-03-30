# Phase 12: Claude Plant Advisor

**Status:** ⬜ Pending
**Branch:** `feature/phase-12-claude-advisor`
**Est. time:** ~60 min

---

## Goal
In-app chat with Claude about plant care. API key stored securely in device keychain.

## What to teach
This calls the Anthropic API directly from the mobile app. The API key is stored in `SecureStorage` — the platform's encrypted keychain. Never store API keys in code, config files, or `Preferences` (which is unencrypted). The chat ViewModel follows the same pattern as everything else — interface, tests, implementation. The interesting TDD challenge here is mocking the HTTP client to simulate API responses.

## Decisions required
1. Should the chat history persist between sessions (stored in SQLite), or start fresh each time the advisor is opened?

## Files to create/modify
- `PlantPal/Interfaces/IPlantAdvisorService.cs`
- `PlantPal/Services/PlantAdvisorService.cs`
- `PlantPal/ViewModels/PlantAdvisorViewModel.cs`
- `PlantPal/Pages/PlantAdvisorPage.xaml`
- `PlantPal/Pages/PlantAdvisorPage.xaml.cs`
- `PlantPal.Tests/Services/PlantAdvisorServiceTests.cs`

## Prior state
The following already exist — do not recreate:
- `IHttpClientWrapper` from Phase 07
- `PlantDetailPage.xaml` from Phase 08 (add "Ask Claude" button)

## Claude Code prompt

```
Explain: what is SecureStorage and how does it differ from Preferences? What are the risks
of storing an API key insecurely on a mobile device?

Ask me the chat history persistence decision.

TDD (PlantPal.Tests/Services/PlantAdvisorServiceTests.cs):
Mock IHttpClientWrapper.

Positive cases:
- AskAboutPlant("Monstera", "Why yellow leaves?") → returns non-empty string response
- API key set → IsConfigured = true

Negative cases:
- No API key set → IsConfigured = false, AskAboutPlant returns
  "Please add your Anthropic API key in Settings"
- API returns 401 → returns "API key invalid — check Settings"
- API timeout → returns "Couldn't reach Claude — check your connection"
- API returns 500 → returns graceful error message, no crash

Run — confirm RED. Implement. Confirm GREEN.

IPlantAdvisorService (PlantPal/Interfaces/IPlantAdvisorService.cs):
- Task<string> AskAboutPlantAsync(string species, string question)
- Task SetApiKeyAsync(string key)
- Task<string?> GetApiKeyAsync()
- bool IsConfigured

PlantAdvisorService.cs implements IPlantAdvisorService:
- GetApiKeyAsync() / SetApiKeyAsync() via SecureStorage.GetAsync / SetAsync("anthropic_api_key")
- AskAboutPlantAsync: POST to https://api.anthropic.com/v1/messages
  with model claude-sonnet-4-20250514, include plant species in system prompt
- IsConfigured: checks SecureStorage for key presence
- this. prefix, XML doc comments

PlantAdvisorPage.xaml:
- Chat bubble CollectionView (user messages right / Claude messages left)
- Quick question chips: "Is my schedule right?", "Leaves yellowing?", "Which fertilizer?"
- API key setup row (if not configured): "Add API key →" opens SecureEntry dialog
- Text input + send button, disabled while IsLoading
- Plant name shown at top

Add "Ask Claude 🤖" button to PlantDetailPage.xaml navigating to PlantAdvisorPage with
plant pre-selected.

Do not create any files not explicitly listed above.

Success condition: ./scripts/test-all.sh is green. Update BUILD_STATUS.md checklist.

Commit: ./scripts/commit-phase.sh "feat: Claude Plant Advisor with secure API key storage"
```

## Success condition
- All tests pass including `PlantAdvisorServiceTests`
- `./scripts/test-all.sh` is green
- `BUILD_STATUS.md` Phase 12 checked

## Deviations from plan
<!-- Fill in after completion -->
