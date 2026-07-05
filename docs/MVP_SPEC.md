# Heist & Hunter MVP Specification

This document defines the first playable MVP for the **Re-PvP: Heist & Hunter** mode.

The goal is not perfect balance or a complete class system. The goal is to prove that a simple asymmetric PvP loop works inside R.E.P.O.:

> One Hunter hunts. Everyone else steals, cashes in, and escapes.

## MVP Design Goals

1. Keep the normal R.E.P.O. recovery loop recognizable.
2. Add one player-controlled Hunter as a constant threat.
3. Make money collection matter, but require extraction before final victory.
4. Avoid complex classes, custom maps, or advanced UI in the first version.
5. Make the mode testable with a small group as quickly as possible.

## Player Teams

### Heisters

All non-Hunter players.

Initial behavior:

- Use normal player controls.
- Can collect and move valuables.
- Can cash in valuables.
- Can revive allies if the base game allows it.
- Win by reaching the cash requirement and extracting.

### Hunter

One randomly selected player.

Initial behavior:

- Cannot cash in valuables.
- Has increased movement speed.
- Has increased knockback or physical pressure if available.
- Has access to a simple tracking ability if feasible.
- Wins by preventing Heisters from extracting successfully.

## Default MVP Settings

```text
Preparation Time: 30 seconds
Round Time Limit: 12 minutes
Required Cash: 60% of estimated map value
Extraction Time: 90 seconds
Hunter Count: 1
Hunter Speed Multiplier: 1.15x
Pulse Scan Duration: 4 seconds
Pulse Scan Cooldown: 45 seconds
```

These values are placeholders and should be exposed as config values as soon as possible.

## Round State Machine

```text
WaitingForPlayers
  -> Preparation
  -> Heist
  -> Alarm
  -> Extraction
  -> RoundEnd
```

## Phase 1: WaitingForPlayers

Responsible for preparing the round.

Required behavior:

- Detect eligible players.
- Select one Hunter randomly.
- Assign all others as Heisters.
- Reset round variables.
- Initialize cash tracking.

Round variables:

```text
HunterPlayer
HeisterPlayers
CurrentCash
RequiredCash
RoundTimer
ExtractionActive
ExtractedPlayers
DownedOrDeadPlayers
```

## Phase 2: Preparation

Short opening period to prevent instant kills.

Required behavior:

- Heisters can move.
- Hunter is frozen, blinded, or heavily limited.
- Timer counts down from 30 seconds.
- After timer ends, transition to Heist phase.

Minimum implementation:

- Disable Hunter movement or teleport Hunter to a delayed spawn point.

## Phase 3: Heist

Main recovery phase.

Required behavior:

- Heisters collect and cash in valuables normally.
- Mod tracks total cashed-in value.
- Hunter can move and interfere.
- If CurrentCash >= RequiredCash, transition to Alarm phase.
- If timer reaches 0 before quota, Hunter wins.
- If all Heisters are downed/dead, Hunter wins.

## Phase 4: Alarm

Short transition phase before extraction.

Required behavior:

- Activate extraction objective.
- Notify all players that extraction has started.
- Notify Hunter of extraction location.
- Optionally boost Hunter slightly.
- Transition immediately or after a very short delay to Extraction phase.

Minimum implementation:

- Show a message/log notification.
- Enable extraction zone.

## Phase 5: Extraction

Final escape phase.

Required behavior:

- Start extraction timer.
- Heisters must reach extraction zone.
- Track how many Heisters escape.
- If enough Heisters escape, Heisters win.
- If extraction timer expires and no required escape condition is met, Hunter wins.
- If all remaining Heisters are downed/dead, Hunter wins.

Initial win rule:

```text
At least 1 Heister extracts = Heister minor victory
Half or more Heisters extract = Heister victory
All Heisters extract = Perfect Heister victory
No Heisters extract = Hunter victory
```

For MVP, this can be simplified to:

```text
At least 1 Heister extracts = Heisters win
0 Heisters extract = Hunter wins
```

## Hunter MVP Ability: Pulse Scan

Pulse Scan is the first Hunter ability.

Behavior:

- On activation, reveal nearby Heisters for a short duration.
- Should not be permanent wallhack.
- Should have a long cooldown.

Suggested values:

```text
Detection Duration: 4 seconds
Cooldown: 45 seconds
Detection Range: Medium/large, configurable
```

Fallback implementation:

- Instead of full outlines, display approximate direction/distance text.
- Or briefly ping Heister positions on Hunter's screen.

## Heister Counterplay MVP

Minimum counterplay should be physics-based first:

- Throw objects at Hunter.
- Close doors.
- Block paths with objects.
- Use map layout to break line of sight.

Dedicated items such as Flash Beacon, Decoy Radio, and Glue Bomb are not required for MVP.

## Win Conditions

### Heisters Win

Heisters win if:

1. CurrentCash >= RequiredCash
2. Extraction phase starts
3. At least one Heister extracts before the extraction timer expires

### Hunter Wins

Hunter wins if:

- All Heisters are eliminated/downed
- Round timer expires before quota is reached
- Extraction timer expires with no successful extraction

## Out of Scope for MVP

Do not implement these in the first version unless the base systems are already stable:

- Multiple Hunter types
- Heister class system
- Custom maps
- Complex custom UI
- Full ability trees
- Monster control system
- Advanced animations
- Perfect balancing

## Suggested Implementation Order

1. Create plugin/mod entry point.
2. Add config values.
3. Add round state enum.
4. Select Hunter at round start.
5. Track Heister/Hunter teams.
6. Track cash total.
7. Add round timer.
8. Add quota reached detection.
9. Add extraction phase.
10. Add win/loss result display.
11. Add Hunter stat boost.
12. Add Pulse Scan.
13. Playtest and adjust values.

## Debug Commands

Useful commands for development:

```text
/re-pvp force-start
/re-pvp force-hunter <player>
/re-pvp set-cash <amount>
/re-pvp trigger-extraction
/re-pvp end-round heisters
/re-pvp end-round hunter
```

Actual command names can change depending on the modding framework.

## Open Questions

- How should the mod hook into R.E.P.O.'s cash/value tracking?
- Can extraction be implemented using existing level exits?
- Can Hunter be prevented from cashing in valuables cleanly?
- Should monsters target Hunter at all?
- What is the cleanest way to display team/phase information without custom UI?
