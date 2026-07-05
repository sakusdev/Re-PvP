# Re-PvP

**Re-PvP** is an experimental asymmetric PvP mod concept for **R.E.P.O.**, centered around a game mode called **Heist & Hunter**.

In this mode, most players become **Heisters**, who must recover enough valuables and escape. One player becomes the **Hunter**, whose job is to track, disrupt, delay, and eliminate the Heisters before they can extract.

## Core Concept

> Getting the money is only half the job.  
> Getting out alive is the real heist.

Heist & Hunter keeps the chaotic physics-based recovery gameplay of R.E.P.O. while adding an asymmetric PvP layer.

- **Heisters** win by collecting enough cash value and extracting alive.
- **Hunter** wins by wiping the Heisters, preventing the quota from being reached, or stopping extraction.
- Monsters may remain as a third-party environmental threat, but should be tuned to avoid pure chaos.

## Current Implementation Status

The repository now contains an early BepInEx/Harmony mod skeleton:

- BepInEx plugin entry point
- Config bindings
- Round phase state machine
- Hunter/Heister role assignment
- Debug overlay
- Debug controls
- Cash-in and extraction bridge APIs
- Tentative Harmony patch scaffolding
- Startup dependency diagnostics

The tentative Harmony patches are **disabled by default** until exact R.E.P.O. class and method names are verified.

For local setup, see:

- [`docs/BUILD_AND_TEST.md`](docs/BUILD_AND_TEST.md)
- [`docs/HOOKING_NOTES.md`](docs/HOOKING_NOTES.md)
- [`docs/CODEX_NEXT_STEPS.md`](docs/CODEX_NEXT_STEPS.md)

## Basic Rules

Example default settings:

```text
Heisters: 4-5
Hunter: 1
Required Cash: 60% of map value
Time Limit: 12 minutes
Preparation Time: 30 seconds
Extraction Time: 90 seconds
Extraction Required: true
```

## Round Flow

1. **Preparation Phase**
   - Heisters get a short head start.
   - Hunter is frozen, blinded, or otherwise limited.

2. **Heist Phase**
   - Heisters search for valuables, carry them, and cash them in.
   - Hunter searches for Heisters and disrupts their routes.

3. **Alarm Phase**
   - Triggered when the required cash value is reached.
   - Alarm lights/sounds activate.
   - Hunter is notified of extraction.

4. **Extraction Phase**
   - Heisters must reach the extraction zone.
   - Hunter attempts to stop them before the extraction timer ends.

## MVP Scope

The first playable version should stay very small:

- Randomly select one Hunter
- Assign all other players as Heisters
- Track total cashed-in value
- Start an extraction phase when quota is reached
- End the round with Heister/Hunter victory
- Give Hunter a simple movement/stat boost
- Add a basic Pulse Scan ability if possible

See [`docs/MVP_SPEC.md`](docs/MVP_SPEC.md) for the implementation-focused MVP plan.

## Debug Controls

```text
F6  Start a Re-PvP round
F7  Add debug cash
F8  Force alarm/extraction, only after round has started
F9  End round as Heister win
F10 End round as Hunter win
Q   Hunter Pulse Scan, local Hunter only
```

## Long-Term Ideas

Possible future systems:

- Heister classes: Runner, Carrier, Scout, Medic
- Hunter types: Brute, Stalker, Poltergeist
- Counterplay items: Flash Beacon, Decoy Radio, Glue Bomb, Value Scanner
- Hunter abilities: Pulse Scan, Charge/Blink, Disrupt, Curse Valuable
- High-risk valuable types: fragile antiques, noisy relics, cursed items, heavy safes

## Status

Early prototype / planning repository.

The mode skeleton exists, but real R.E.P.O. gameplay integration still requires verified hooks for players, cash-in, extraction, movement, and networking.
