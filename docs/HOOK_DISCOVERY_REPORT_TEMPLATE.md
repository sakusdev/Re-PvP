# Hook Discovery Report

Use this template after launching the game with:

```ini
LogStartupDiagnostics = true
EnableHarmonyPatches = false
```

Copy relevant BepInEx log lines into each section.

## Environment

```text
Game version:
BepInEx version:
Unity version:
TargetFramework tested:
Build result:
```

## Loaded Assemblies

Paste lines like:

```text
Assembly loaded: BepInEx ...
Assembly loaded: 0Harmony ...
Assembly loaded: UnityEngine ...
Assembly loaded: UnityEngine.CoreModule ...
Assembly loaded: Assembly-CSharp ...
```

## Player Candidate Types

Paste diagnostics:

```text
Player discovery candidate type: ...
```

Candidate selected:

```text
Type:
Assembly:
Useful methods:
Useful fields/properties:
```

## Cash-In Candidate Types

Paste diagnostics:

```text
Valuable cash-in candidate type: ...
```

Candidate selected:

```text
Type:
Method:
Cash value source:
Does it fire once per valuable:
Who owns/cashed the value:
```

## Extraction Candidate Types

Paste diagnostics:

```text
Extraction candidate type: ...
```

Candidate selected:

```text
Type:
Method:
Argument that identifies player:
Can extraction happen before quota:
Does base game end round automatically:
```

## Movement / Hunter Stats

```text
Player movement type:
Speed field/property:
Sprint/stamina field/property:
Reset behavior:
Network replication notes:
```

## Networking

```text
Network framework observed:
RPC pattern:
Host-authoritative state:
State sync target:
```

## Next Patch Changes

List exact code changes to make:

```text
ValuableCashInPatch:
ExtractionPatch:
PlayerDiscoveryPatch:
UnityPlayerProvider:
UnityLocalPlayerResolver:
HunterStatController:
```
