# Networking Notes

Re-PvP does not currently implement real multiplayer synchronization.

This document records the temporary architecture added to make future network work easier.

## Current State Packet

File:

```text
src/RePvP/Networking/RoundStatePacket.cs
```

`RoundStatePacket` is a transport-friendly representation of `RoundSnapshot`.

Fields:

```text
phase
currentCash
requiredCash
phaseTimeRemaining
roundTimeRemaining
heisterCount
extractedCount
hunterName
cashProgress
```

## Serializer

File:

```text
src/RePvP/Networking/RoundStateSerializer.cs
```

This provides a minimal JSON-style serializer without adding external dependencies.

It is intended for:

- Debug logs
- Future RPC payload inspection
- Simple host/client state diffing

## Broadcaster Placeholder

File:

```text
src/RePvP/Networking/RoundStateBroadcaster.cs
```

This subscribes to `RoundEventBus` and logs event updates.

It is disabled by default with:

```ini
[General]
EnableNetworkBroadcastPlaceholder = false
```

Enable only when debugging future sync behavior:

```ini
EnableNetworkBroadcastPlaceholder = true
```

## Future Work

Replace the log-only broadcaster with the real R.E.P.O. networking/RPC layer.

Likely responsibilities:

- Host owns authoritative round state.
- Host sends phase/cash/extraction changes to clients.
- Clients render UI from received state.
- Clients should not independently decide winners.
- Hunter ability usage should be validated by host.

## Suggested Sync Events

```text
RoundStarted
PhaseChanged
CashChanged
HeisterExtracted
RoundEnded
HunterAbilityUsed
```

## Safety Rule

Until exact networking APIs are verified, keep all network code as placeholders and avoid touching live multiplayer state.
