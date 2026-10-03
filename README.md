# Random Clan

A Trainworks Reloaded mod that rebuilds a placeholder clan each run from other clans’ cards, then applies wording-safe modifications.

**Flow:** `SaveManager.SetupRun` Prefix → cards with `Randomized` copy from donors without that trait → apply N mods from `*ModifierN` traits via `ICardModification` registry.

## Layout

| Path | Role |
| --- | --- |
| `Plugin.cs`, `ClanRandomizer.cs` | Entry + orchestrator |
| `patches/` | Harmony patches |
| `json/`, `textures/` | Trainworks content |
| `Constants/` | Shared tunables (marker names, status families, mod steps) |
| `Extensions/` | `this` helpers on game types |
| `Modifications/` | Registry + `Cards/` `Units/` `Rooms/` `Champions/` mods |
| `CardTraits/` | No-op marker `CardTraitState` classes |
| `Copying/`, `Snapshots/` | Donor copy + pristine restore |

Requires authenticated GitHub Packages access for `TrainworksReloaded.Base` / `Conductor` (see `nuget.config`).
