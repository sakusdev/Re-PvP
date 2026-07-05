# Events and UI Notes

Re-PvP now uses an instance-based `RoundEventBus` to decouple round state changes from presentation.

## Event Bus

File:

```text
src/RePvP/Events/RoundEventBus.cs
```

Events:

```csharp
PhaseChanged(GamePhase previous, GamePhase current)
CashChanged(int currentCash, int requiredCash)
HeisterExtracted(PlayerRef player, int extractedCount, int requiredCount)
RoundEnded(Team winner, string reason)
```

`RoundManager` raises these events when:

- Phase changes
- Cash changes
- A Heister extracts
- The round ends

## Message Feed

File:

```text
src/RePvP/UI/MessageFeed.cs
```

`MessageFeed` stores short-lived HUD-style text messages. The debug overlay renders visible messages under the round state.

## Presenter

File:

```text
src/RePvP/UI/RoundMessagePresenter.cs
```

`RoundMessagePresenter` subscribes to `RoundEventBus` and pushes user-facing messages into `MessageFeed`.

It implements `IDisposable` so subscriptions can be removed during plugin teardown.

## Current Presentation

File:

```text
src/RePvP/DebugOverlayRenderer.cs
```

The current UI is still an IMGUI debug overlay. Future work can replace this with real R.E.P.O. UI while keeping the same event bus.

## Why This Matters

This separation makes it easier to add later:

- Real HUD messages
- Sound effects
- Network state sync
- Replay/debug logs
- Round analytics
- Better observer/spectator UI
