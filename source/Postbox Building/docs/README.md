**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/i-saac-b/PostBoxMod**

----

# PostBoxMod
Adds a new building that allows sending gifts to NPCs from your farm! No more chasing your favorite villager halfway across the map, or realizing too late that you forgot their schedule.

Thanks to Katie Pearson for the lovely art assets!

## Features
New building in Robin's construction menu: the Postbox! Accepts gifts and sends them to NPCs overnight. 
Provides a slight debuff to relationship points gained from gifts given this way (configurable; default 25% reduction)
Compatible with custom NPCs and items!

## Install
This mod requires [SMAPI](https://smapi.io/) as well as [SpaceCore](https://www.nexusmods.com/stardewvalley/mods/1348).
Install [this mod from Nexus](https://www.nexusmods.com/stardewvalley/mods/17614)
Run using SMAPI and enjoy!

For any advice on how to start modding/using mods please refer to [the wiki](https://stardewvalleywiki.com/Modding:Player_Guide/Getting_Started).

## Configurability
Configurable relationship point penalty for mailed gifts (Default: .75)

Configurable G cost (Default: 2500)

Configurable Material cost:

- Free: No materials!
- Normal (Default): 2 Iron Bars | 5 Clay | 50 Stone
- Expensive: 10 Iron Bars | 5 Gold Bars | 1 Iridium Bar
- Endgame: 10 Iridium Bars | 5 Radioactive Bars | 10 Battery Packs | 1 Prismatic Shard
- Custom: Loads from the CustomPostboxMaterialCost string. Formatted as one string, "itemId itemCount itemId itemCount [etc.]" (Check out https://stardewlist.com/ for easy item ID lookups!)

## Compatibility
Compatible with Stardew Valley version 1.5.6+ (and likely versions 1.5+, but not tested), in both singleplayer and multiplayer!

There are no known or expected incompatibilities with other mods, unless they greatly modify how Robin's building interface/building logic in general works.

## Translation
This mod was written with SMAPI's [i18n translation API](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Translation). If you have the desire and ability to contribute a translation to this mod, feel free to reach out or make a pull request. 
