# Codex Next Steps

The current codebase now has a playable-mode skeleton plus tentative Harmony patch scaffolding.

## Current State

Implemented:

- BepInEx plugin entry point
- Config bindings
- Round phase state machine
- Hunter/Heister role assignment
- Cash tracking bridge
- Extraction bridge
- Debug overlay
- Pulse Scan skeleton
- Local Hunter input guard
- Harmony bootstrap
- Tentative reflection-based patches

Still missing:

- Exact R.E.P.O. class and method names
- Real multiplayer player list
- Real cash-in event hook
- Real extraction event hook
- Real movement stat mutation
- Network-safe sync for role assignment and ability use

## High-Priority Codex Task

Inspect the decompiled R.E.P.O. assemblies and replace the tentative patch candidates with exact targets.

Focus files:

```text
src/RePvP/Patching/ValuableCashInPatch.cs
src/RePvP/Patching/ExtractionPatch.cs
src/RePvP/Patching/PlayerDiscoveryPatch.cs
src/RePvP/UnityPlayerProvider.cs
src/RePvP/UnityLocalPlayerResolver.cs
src/RePvP/HunterStatController.cs
```

## What to Find

### 1. Player List

Find the authoritative multiplayer player/session manager.

Need:

- All active players
- Local player
- Stable player ID
- Display name
- Player GameObject / transform

Then replace:

```csharp
UnityPlayerProvider
UnityLocalPlayerResolver
```

### 2. Valuable Cash-In

Find the method/event where a valuable is successfully converted into money.

Need:

- Final cash amount
- Who cashed it in, if available
- Whether the event fires once or multiple times

Then update:

```csharp
ValuableCashInPatch
ValuableCashInBridge
```

### 3. Extraction

Find the method/event where a player successfully exits/extracts.

Need:

- Player object or player ID
- Whether extraction can happen before quota
- Whether the base game ends immediately on extraction

Then update:

```csharp
ExtractionPatch
ExtractionZone
RoundManager.TryMarkHeisterExtracted
```

### 4. Hunter Movement

Find the player movement/stat component.

Need:

- Base movement speed field/property
- Sprint/stamina fields if relevant
- Network replication behavior

Then update:

```csharp
HunterStatController
```

### 5. Network Sync

Determine how R.E.P.O. syncs state.

Need to sync:

- Hunter selection
- Phase transitions
- Cash total
- Extraction state
- Round end result
- Pulse Scan activation if visualized for multiple players

Possible next file to create:

```text
src/RePvP/Networking/RePvPNetworkState.cs
```

## Acceptance Criteria for Next Implementation Pass

- Mod builds locally with copied DLL refs.
- Plugin loads without patch exceptions.
- F6 starts a round with real players.
- F7 cash debug still works.
- Actual cash-in increases `CurrentCash`.
- Actual extraction calls `MarkHeisterExtracted`.
- Only the Hunter can trigger Pulse Scan.
- Debug overlay shows accurate phase/cash/time.
