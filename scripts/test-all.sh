#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
readonly RESULTS_DIR="$SCRIPT_DIR/../TestResults"

echo "Running PlantPal test suite..."

# Build first, then unblock DLLs to satisfy Windows Application Control policy,
# then run without rebuilding.
dotnet build "$SCRIPT_DIR/../PlantPal.Tests/PlantPal.Tests.csproj" --no-restore -v quiet
powershell -Command "Get-ChildItem '$SCRIPT_DIR/..' -Recurse -Filter '*.dll' | Unblock-File" 2>/dev/null || true

dotnet test "$SCRIPT_DIR/../PlantPal.Tests/PlantPal.Tests.csproj" \
  --no-build \
  --no-restore \
  --verbosity minimal \
  --logger "console;verbosity=normal" \
  --logger "trx;LogFileName=results.trx" \
  --results-directory "$RESULTS_DIR"

echo "All tests passed."
