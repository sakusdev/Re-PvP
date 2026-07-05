# Implementation Tasks

This file breaks the MVP into small development tasks that can be handed to Codex or another coding agent.

## Milestone 0: Repository Setup

- [ ] Decide mod framework target.
  - Likely: BepInEx plugin for R.E.P.O.
- [ ] Create solution/project structure.
- [ ] Add build instructions.
- [ ] Add basic plugin entry point.
- [ ] Confirm plugin loads in-game.

## Milestone 1: Core Round State

- [ ] Add `GamePhase` enum.

Suggested phases:

```csharp
public enum GamePhase
{
    WaitingForPlayers,
    Preparation,
    Heist,
    Alarm,
    Extraction,
    RoundEnd
}
```

- [ ] Add `RoundManager`.
- [ ] Add config-backed values.
- [ ] Add simple debug logging for phase transitions.
- [ ] Add manual force-start debug command or keybind.

## Milestone 2: Team Assignment

- [ ] Detect active players.
- [ ] Randomly select one Hunter.
- [ ] Assign every other player as Heister.
- [ ] Store role state per player.
- [ ] Display/log assigned roles.
- [ ] Prevent Hunter from counting as a Heister.

## Milestone 3: Timer and Win Conditions

- [ ] Add round timer.
- [ ] End round with Hunter victory when timer expires before quota.
- [ ] Detect all Heisters dead/downed if possible.
- [ ] Add result display/log.
- [ ] Reset state after round end.

## Milestone 4: Cash Tracking

- [ ] Find hook for valuable cash-in event.
- [ ] Track `CurrentCash`.
- [ ] Determine or configure `RequiredCash`.
- [ ] Trigger Alarm phase when quota is reached.
- [ ] Ensure Hunter cannot increase cash total.

## Milestone 5: Extraction

- [ ] Find existing extraction/exit system.
- [ ] Enable extraction only after quota.
- [ ] Track extracted Heisters.
- [ ] Add extraction timer.
- [ ] End round with Heister victory if extraction condition is met.
- [ ] End round with Hunter victory if extraction expires with no extraction.

## Milestone 6: Hunter Basics

- [ ] Apply Hunter movement speed multiplier.
- [ ] Add Hunter visual or name indicator for testing.
- [ ] Prevent Hunter from cashing in valuables.
- [ ] Optionally increase Hunter knockback/physical impact.
- [ ] Reset Hunter stats after round end.

## Milestone 7: Pulse Scan

- [ ] Add input binding for Hunter ability.
- [ ] Add cooldown.
- [ ] Add duration timer.
- [ ] Reveal nearby Heisters.
- [ ] Add fallback text-based pings if visual outlines are difficult.
- [ ] Prevent use during Preparation phase.

## Milestone 8: Playtest Config

- [ ] Expose values in config file.
- [ ] Add easy test presets.
- [ ] Log final round stats.
- [ ] Record common balance problems.

## Suggested Config Keys

```text
Enabled=true
PreparationTime=30
RoundTimeLimit=720
ExtractionTime=90
RequiredCashPercent=0.60
HunterSpeedMultiplier=1.15
PulseScanDuration=4
PulseScanCooldown=45
PulseScanRange=40
MinimumHeistersToExtract=1
```

## First Playtest Checklist

- [ ] Can a round start reliably?
- [ ] Is exactly one Hunter assigned?
- [ ] Can Heisters still recover valuables normally?
- [ ] Does quota trigger extraction?
- [ ] Can the Hunter actually pressure players?
- [ ] Is Hunter too oppressive?
- [ ] Is the extraction phase exciting?
- [ ] Does the round end correctly?
