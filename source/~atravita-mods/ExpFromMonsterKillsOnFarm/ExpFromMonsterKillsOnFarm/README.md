**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/StardewMods**

----

Farm Combat Grants XP Too
==========================

In vanilla, when you kill a monster on the farm, you get no XP from it; nor does it count towards quests. This mod makes it so you do.

![Shows XP gain from a kill of a dino on the farm.](docs/dinokill.gif)

**NOTE: THE XP NUMBERS ARE FROM [UI INFO SUITE 2](https://github.com/Annosz/UIInfoSuite2/releases). THIS MOD DOES NOT ADD XP NUMBERS, IT JUST RESTORES THE ABILITY TO GET XP ON THE FARM.**

### Install

1. Install the latest version of [SMAPI](https://smapi.io).
2. Download and install [AtraCore](https://www.nexusmods.com/stardewvalley/mods/12932).
2. Download this mod and unzip it into `Stardew Valley/Mods`.
3. Run the game using SMAPI.

### Config options
Run SMAPI at least once with this mod installed to generate the `config.json`, or use [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) to configure.

* `GainExp` restores XP gain from monster kills on farm.
* `QuestCompletion` counts monster kills on farm towards billboard quest completion.
* `SpecialOrderCompletion` counts monster kills on farm towards special order objectives.

### Compatibility

* Works with Stardew Valley 1.5.6 on Linux/macOS/Windows.
* Works in single player, multiplayer, and split-screen mode. Can probably be installed by just one player in multiplayer.

### See Also:

* [Release Notes](docs/CHANGELOG.MD)

Requires SMAPI, uses harmony.
