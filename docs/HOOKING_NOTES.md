# Hooking Notes

This document tracks the remaining places where the current Re-PvP skeleton must be connected to real R.E.P.O. game code.

The project currently contains the game-mode state machine, role assignment, debug overlay, cash bridge, and extraction trigger component. The missing part is identifying the actual R.E.P.O. classes/events to patch.

## Current Integration Entry Points

### Cash-In Event

Use:

```csharp
ValuableCashInBridge.OnValuableCashed(cashValue, source);
```

or:

```csharp
RePvPApi.NotifyValuableCashed(cashValue);
```

Needed hook:

- Find the method called when a valuable is successfully cashed in.
- Extract the final cash value from that event/method.
- Ensure the cash is counted only once.
- Ensure Hunter cash-ins do not count once role/player ownership can be determined.

## Extraction Event

Use:

```csharp
RePvPApi.NotifyPlayerEnteredExtraction(playerGameObject);
```

or attach:

```csharp
ExtractionZone
```

to a trigger collider.

Needed hook:

- Find the real extraction/exit zone object.
- Add `ExtractionZone` to it when `GamePhase.Extraction` starts.
- Or patch the real extraction method and call the API directly.

## Player Discovery

Current class:

```csharp
UnityPlayerProvider
```

Current behavior:

- Uses heuristic GameObject search.
- Looks for object names like `Player`, `SemiFunc`, `Avatar`, or `Character`.
- Requires `CharacterController`, `Rigidbody`, or child `Camera`.

Needed hook:

- Replace with real multiplayer/session player list.
- Store stable player IDs.
- Distinguish local player from remote players.
- Attach Steam/user display names if available.

## Hunter Ability Input

Current behavior:

- `Q` triggers Pulse Scan globally.

Needed hook:

- Only allow local Hunter to activate Pulse Scan.
- Sync ability activation over network if required.
- Add real visual indication instead of log-only detection.

## Hunter Stat Modifier

Current class:

```csharp
HunterStatController
```

Current behavior:

- Logs intended speed multiplier.

Needed hook:

- Find actual movement/stamina/stat component.
- Apply speed multiplier when round starts.
- Reset speed multiplier on round end.
- Make sure changes replicate correctly in multiplayer.

## UI

Current behavior:

- IMGUI debug overlay shows phase, cash, time, Hunter, extraction count.

Needed hook:

- Replace or supplement with in-game HUD messages.
- Add alarm/extraction notifications.
- Add Hunter-only scan feedback.

## Suggested Next Codex Task

Ask Codex to inspect decompiled R.E.P.O. / BepInEx symbols and find:

1. Player/session manager class
2. Valuable cash-in method
3. Extraction/exit method
4. Movement/stat component
5. Network RPC pattern used by the game

Then wire those into:

- `UnityPlayerProvider`
- `ValuableCashInBridge`
- `ExtractionZone` or `RePvPApi.NotifyPlayerEnteredExtraction`
- `HunterStatController`
- `HunterAbilityController`
