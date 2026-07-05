# Build and Local Test Guide

This project is still an early BepInEx mod skeleton. It requires local DLL references from your installed R.E.P.O. / BepInEx environment.

## 1. Copy local DLL references

Copy these files into:

```text
src/RePvP/lib/
```

Required:

```text
BepInEx.dll
0Harmony.dll
UnityEngine.dll
UnityEngine.CoreModule.dll
```

Likely locations:

```text
<R.E.P.O. install>/BepInEx/core/BepInEx.dll
<R.E.P.O. install>/BepInEx/core/0Harmony.dll
<R.E.P.O. install>/REPO_Data/Managed/UnityEngine.dll
<R.E.P.O. install>/REPO_Data/Managed/UnityEngine.CoreModule.dll
```

The exact folder name may differ depending on the game build and platform.

Do not commit these DLLs.

## 2. Check references

Windows PowerShell:

```powershell
./scripts/check-refs.ps1
```

Linux/macOS shell:

```bash
bash ./scripts/check-refs.sh
```

## 3. Build

From the repository root:

```bash
dotnet build RePvP.sln -c Release
```

Or use the helper scripts:

```powershell
./scripts/build.ps1
```

```bash
bash ./scripts/build.sh
```

Output should appear under:

```text
src/RePvP/bin/Release/netstandard2.1/RePvP.dll
```

If `netstandard2.1` is not compatible with the local game/BepInEx environment, update `TargetFramework` in:

```text
src/RePvP/RePvP.csproj
```

Possible alternatives to test:

```text
netstandard2.0
net472
net46
```

## 4. Install the plugin

Copy the built DLL to:

```text
<R.E.P.O. install>/BepInEx/plugins/RePvP.dll
```

Launch the game once. BepInEx should generate:

```text
BepInEx/config/dev.sakus.repvp.cfg
```

## 5. Early safe config

For first launch, keep tentative Harmony patches disabled:

```ini
[General]
Enabled = true
EnableHarmonyPatches = false
LogStartupDiagnostics = true
```

An example config is available at:

```text
config/dev.repvp.example.cfg
```

## 6. Debug controls

```text
F4  Mark the next active Heister as extracted, extraction phase only
F5  Spawn 4 debug player capsules
F6  Start a Re-PvP round
F7  Add configured debug cash
F8  Force alarm/extraction, only after round has started
F9  End round as Heister win
F10 End round as Hunter win
F11 Log full Re-PvP debug state and team details
F12 Reset Re-PvP state to WaitingForPlayers
Ctrl+F12 Clear debug players and reset state
Q   Hunter Pulse Scan, local Hunter only
```

Custom cash input:

```text
Type digits, then press Enter = add that amount as debug cash
Backspace = edit the buffer
Escape = clear the buffer
```

Debug player flow:

```text
F5 -> spawn debug players
F6 -> start round using those debug players
F7 or custom cash -> reach quota
F8 or quota -> start extraction flow
F4 -> extract one active Heister
Ctrl+F12 -> clear debug players
```

## 7. What to check in BepInEx logs

Look for:

```text
Re-PvP 0.1.0 loaded.
Dependency diagnostics start.
Assembly loaded: BepInEx ...
Assembly loaded: 0Harmony ...
Assembly loaded: UnityEngine ...
Harmony patches are disabled by config.
```

If `LogStartupDiagnostics=true`, the plugin will also print whether tentative hook candidate types were found.

Use this template to record findings:

```text
docs/HOOK_DISCOVERY_REPORT_TEMPLATE.md
```

## 8. First test checklist

- [ ] Game launches without BepInEx errors.
- [ ] Re-PvP debug overlay appears.
- [ ] F5 spawns 4 debug player capsules.
- [ ] F6 starts a round using debug players.
- [ ] A Hunter is selected.
- [ ] F7 increases cash.
- [ ] Typing digits + Enter adds a custom cash amount.
- [ ] Quota triggers Alarm/Extraction.
- [ ] F8 does not work before round start.
- [ ] F4 extracts one active Heister during Extraction.
- [ ] Required extraction count ends the round as Heister win.
- [ ] F11 logs complete state and team details.
- [ ] F12 resets back to WaitingForPlayers.
- [ ] Ctrl+F12 clears debug players.
- [ ] Q only works if the local player is detected as Hunter.

## 9. When enabling Harmony patches

Only set this after the real R.E.P.O. class/method names are verified:

```ini
EnableHarmonyPatches = true
```

Current patches are still tentative and may not target the correct game methods.
