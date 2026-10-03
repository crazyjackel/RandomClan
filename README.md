# Random Clan

A **Monster Train 2** clan that rebuilds itself every run.

Each card starts as a placeholder with **Randomized**. At run setup, those cards copy mechanics from matching donors in other clans, keep their own art, then apply a dose of **Chaos** modifications (cost tweaks, traits, statuses, stats, and more).

## Requirements

- [BepInEx](https://thunderstore.io/c/monster-train-2/p/BepInEx/BepInExPack/)
- [Trainworks Reloaded](https://thunderstore.io/c/monster-train-2/p/MT2/Trainworks_Reloaded/) `0.7.28+`
- [Conductor](https://thunderstore.io/c/monster-train-2/p/Conductor/Conductor/) `0.5.14+`

## How it plays

1. Pick **Random** as a clan (or ally).
2. On run start, every `Randomized` card pulls a donor of the same type/rarity (and banner/starter/champion rules).
3. **Unit / Spell / Room / Champion Chaos N** applies up to **N** wording-safe modifications from the mod registry.

Higher Chaos means more mutations. The floor for most cards is Chaos **2**; the champion is Chaos **3**.

## Notes

- Card and character art stay on the Random Clan placeholders — only mechanics are borrowed.
- Randomization is seeded with the run, so the same seed rebuilds the same clan.

## For modders

| Path | Role |
| --- | --- |
| `Plugin.cs`, `ClanRandomizer.cs` | Entry + orchestrator |
| `patches/` | Harmony patches |
| `json/`, `textures/` | Trainworks content |
| `Modifications/` | Chaos registry (`Cards/`, `Units/`, `Rooms/`, `Champions/`) |
| `Copying/`, `Snapshots/` | Donor copy + pristine restore |

Building from source needs authenticated GitHub Packages access for `TrainworksReloaded.Base` / `Conductor` (see `nuget.config`).
