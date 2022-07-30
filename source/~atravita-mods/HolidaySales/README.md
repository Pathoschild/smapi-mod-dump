**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/StardewMods**

----

Holiday Sales
===========================
![Header image](docs/shops.gif)

It never really made sense to me that Pelican Town would close down when, say, Ridgeside, had a festival. By default, this mod keeps stores "in town" open when mod maps have festivals. (It also works the other way around - doors stay open on mod maps when the town has a festival.)

Or keep them open during all festivals, be mean like that, I guess.

Note: you still can't access the actual map of the festival before the festival time - this will prevent you from walking to Pierre's if there's a festival in Town, for example.

## Install

1. Install the latest version of [SMAPI](https://smapi.io).
2. Download and install [AtraCore](https://www.nexusmods.com/stardewvalley/mods/12932).
2. Download this mod and unzip it into `Stardew Valley/Mods`.
3. Run the game using SMAPI.

## Uninstall
Simply delete from your Mods directory.

## Configuration
Run SMAPI at least once with this mod installed to generate the `config.json`, or use [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) to configure.

* `Closed` - default vanilla behavior - stores closed on the day of a festival.
* `Map Dependent` - stores closed if they're in the same region as a place with a festival, but stay open otherwise. Pelican Town is considered a region, otherwise any maps that start with `Custom_<ModName>_` are assumed to share a region.
* `Open` - stores are open during festival days.

## Technical note:

* This mod assumes that custom locations are added using the naming convention `Custom_<ModName>_<MapName>`. I'm aware that not every mod uses that convention correctly. 

## Compatibility

* Works with Stardew Valley 1.5.6 on Linux/macOS/Windows.
* Works in single player, multiplayer, and split-screen mode. Should be fine if installed for only one player in multiplayer.
* Should be compatible with most other mods.

## See also

[Changelog](docs/changelog.md)
