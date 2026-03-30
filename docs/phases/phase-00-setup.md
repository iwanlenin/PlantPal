# Phase 00: Setup — Git, CI & Scripts

**Status:** ✅ Complete
**Branch:** `main`
**Completed:** 2026-03-30

---

## Goal
GitHub repo with MAUI .gitignore, CI pipeline, and dev workflow scripts.

## What was built
- Local git repo initialised, branch named `main`, remote wired to `https://github.com/iwanlenin/PlantPal`
- `.gitignore` and `.gitattributes` (LF line endings enforced)
- `scripts/test-all.sh`, `watch-tests.sh`, `commit-phase.sh`, `new-branch.sh`
- `.github/workflows/ci.yml` — runs on push/PR to main, scoped to `PlantPal.Tests`

## Deviations from original plan
- **MAUI workload not on CI** — CI scoped to test project only (ubuntu-latest, no MAUI workload). Intentional: no MAUI dependencies in test project. APK CI can be added later.
- **One CI workflow file** — `ci.yml` handles both push and PR triggers. The planned `pr-check.yml` (PR comment with test summary) was omitted — unnecessary complexity for a solo developer.
- **WORKFLOW.md not created** — the daily development loop cheat sheet was not written. Pending decision: create standalone, fold into Phase 01, or skip.
- **Repo is public** — visibility not confirmed before creation (was already created).

## See also
- `docs/claude-steps/phase-00-setup.md` — session log with Mermaid diagram
