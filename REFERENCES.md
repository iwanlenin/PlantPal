# PlantPal — Environment & Tool References

Read this file instead of searching for tool paths with Bash. Update it whenever a new tool is confirmed installed or a path changes.

---

## Shell environment note

The Claude Code bash tool runs **non-interactive, non-login** bash shells.
`.bash_profile` and `.bashrc` are NOT sourced automatically.

**Fix applied:** `BASH_ENV=C:/Users/anysi/.bashrc` is set as a Windows user
environment variable (via `SetEnvironmentVariable` in the registry). Bash
sources `BASH_ENV` for every non-interactive shell. Takes effect in new
Claude Code sessions — not the current one.

If `gh` or another tool is missing from PATH mid-session, use the full path
from the table below as a one-off workaround.

---

## Installed CLI tools

| Tool | Command | Full path | Version |
|---|---|---|---|
| .NET SDK | `dotnet` | `/c/Program Files/dotnet/dotnet.exe` | 10.0.201 |
| GitHub CLI | `gh` | `/c/Program Files/GitHub CLI/gh.exe` | 2.89.0 |
| Git | `git` | `/mingw64/bin/git.exe` | (Git for Windows) |
| Node.js | `node` | `/c/Program Files/nodejs/node.exe` | (on PATH) |
| Flutter | `flutter` | `/c/flutter/bin/flutter` | (on PATH) |

---

## PATH additions in `.bashrc`

```bash
export PATH="$PATH:/c/Program Files/GitHub CLI"
```

`.bashrc` is at `C:/Users/anysi/.bashrc`.
`BASH_ENV` points to it, so new non-interactive shells pick up PATH automatically.

---

## GitHub

| Item | Value |
|---|---|
| Repo URL | `https://github.com/iwanlenin/PlantPal` |
| Default branch | `main` |
| CI workflow | `.github/workflows/ci.yml` |
| Branch pattern | `feature/phase-NN-name` |

---

## Dev machine

| Item | Value |
|---|---|
| OS | Windows (Git Bash via Claude Code) |
| SDK | .NET 10.0.201 |
| IDE | Visual Studio 2022 |
| MAUI workload | Installed (confirmed Phase 01) |
| Android emulator | Configured in VS |

---

## NuGet packages

See [PACKAGES.md](./PACKAGES.md) for all package versions and install status.
