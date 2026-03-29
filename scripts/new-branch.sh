#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

if [[ $# -ne 1 ]]; then
  echo "Usage: $0 \"<branch-name>\"" >&2
  exit 1
fi

readonly BRANCH="$1"

if [[ "$BRANCH" =~ [[:space:]] || "$BRANCH" == /* ]]; then
  echo "ERROR: Invalid branch name: '$BRANCH'" >&2
  exit 1
fi

echo "Switching to main and pulling latest..."
git -C "$SCRIPT_DIR/.." checkout main
git -C "$SCRIPT_DIR/.." pull --ff-only origin main

echo "Creating branch: $BRANCH"
git -C "$SCRIPT_DIR/.." checkout -b "$BRANCH"

echo "Switched to new branch: $BRANCH"
