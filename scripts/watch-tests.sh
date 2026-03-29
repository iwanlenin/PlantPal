#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

echo "Watching PlantPal tests — Ctrl+R to force rerun, Ctrl+C to exit."

exec dotnet watch test \
  --project "$SCRIPT_DIR/../PlantPal.Tests/PlantPal.Tests.csproj" \
  --no-hot-reload \
  -- --verbosity minimal
