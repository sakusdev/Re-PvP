param(
    [string]$Configuration = "Release",
    [string]$Solution = "RePvP.sln"
)

$ErrorActionPreference = "Stop"

Write-Host "Re-PvP build helper"
Write-Host "Configuration: $Configuration"
Write-Host "Solution: $Solution"
Write-Host ""

& "$PSScriptRoot/check-refs.ps1"

Write-Host ""
Write-Host "Building..."
dotnet build $Solution -c $Configuration

Write-Host ""
Write-Host "Build complete. Expected output:"
Write-Host "src/RePvP/bin/$Configuration/netstandard2.1/RePvP.dll"
