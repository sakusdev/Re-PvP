param(
    [string]$ProjectDir = "src/RePvP"
)

$ErrorActionPreference = "Stop"

$libDir = Join-Path $ProjectDir "lib"
$required = @(
    "BepInEx.dll",
    "0Harmony.dll",
    "UnityEngine.dll",
    "UnityEngine.CoreModule.dll"
)

Write-Host "Checking local reference DLLs in: $libDir"

$missing = @()
foreach ($dll in $required) {
    $path = Join-Path $libDir $dll
    if (Test-Path $path) {
        $file = Get-Item $path
        Write-Host "OK      $dll ($([Math]::Round($file.Length / 1KB, 1)) KB)"
    }
    else {
        Write-Host "MISSING $dll" -ForegroundColor Yellow
        $missing += $dll
    }
}

if ($missing.Count -gt 0) {
    Write-Host ""
    Write-Host "Missing required DLL references:" -ForegroundColor Red
    $missing | ForEach-Object { Write-Host "- $_" -ForegroundColor Red }
    Write-Host ""
    Write-Host "Copy them from your local R.E.P.O. / BepInEx install into $libDir."
    exit 1
}

Write-Host "All local references are present."
