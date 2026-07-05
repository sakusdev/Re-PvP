#!/usr/bin/env bash
set -euo pipefail

CONFIGURATION="${1:-Release}"
SOLUTION="${2:-RePvP.sln}"

echo "Re-PvP build helper"
echo "Configuration: $CONFIGURATION"
echo "Solution: $SOLUTION"
echo ""

"$(dirname "$0")/check-refs.sh"

echo ""
echo "Building..."
dotnet build "$SOLUTION" -c "$CONFIGURATION"

echo ""
echo "Build complete. Expected output:"
echo "src/RePvP/bin/$CONFIGURATION/netstandard2.1/RePvP.dll"
