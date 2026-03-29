#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

if [[ $# -ne 1 ]]; then
  echo "Usage: $0 \"<commit message>\"" >&2
  exit 1
fi

readonly COMMIT_MSG="$1"

echo "Step 1/3: Running tests before commit..."
"$SCRIPT_DIR/test-all.sh"

echo "Step 2/3: Staging all changes..."
git -C "$SCRIPT_DIR/.." add -A

echo "Step 3/3: Committing and pushing..."
git -C "$SCRIPT_DIR/.." commit -m "$COMMIT_MSG"
git -C "$SCRIPT_DIR/.." push

echo "Done. Phase committed and pushed."
