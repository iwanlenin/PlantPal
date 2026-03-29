#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
readonly RESULTS_DIR="$SCRIPT_DIR/../TestResults"

echo "Running PlantPal test suite..."

dotnet test "$SCRIPT_DIR/../PlantPal.Tests/PlantPal.Tests.csproj" \
  --no-restore \
  --verbosity minimal \
  --logger "console;verbosity=normal" \
  --logger "trx;LogFileName=results.trx" \
  --results-directory "$RESULTS_DIR"

echo "All tests passed."
