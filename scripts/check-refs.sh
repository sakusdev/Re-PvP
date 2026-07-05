#!/usr/bin/env bash
set -euo pipefail

PROJECT_DIR="${1:-src/RePvP}"
LIB_DIR="$PROJECT_DIR/lib"

required=(
  "BepInEx.dll"
  "0Harmony.dll"
  "UnityEngine.dll"
  "UnityEngine.CoreModule.dll"
)

echo "Checking local reference DLLs in: $LIB_DIR"

missing=0
for dll in "${required[@]}"; do
  path="$LIB_DIR/$dll"
  if [[ -f "$path" ]]; then
    size=$(wc -c < "$path" | tr -d ' ')
    echo "OK      $dll (${size} bytes)"
  else
    echo "MISSING $dll"
    missing=1
  fi
done

if [[ "$missing" -ne 0 ]]; then
  echo ""
  echo "Missing required DLL references."
  echo "Copy them from your local R.E.P.O. / BepInEx install into $LIB_DIR."
  exit 1
fi

echo "All local references are present."
