**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/bcmpinc/StardewHack**

----

# Grass Growth

## Description
Allows you to fully configure where and how fast grass growths & spreads. Normally long grass only spreads on digable soil. With this mod it can spread everywhere on your farm. Useful for the forest farm, where there's plenty of grass tiles, but the digable soil is scarce.

## Config
*Note: run Stardew Valley once with this mod enabled to generate the `config.json` file.*

* `DisableGrowth`: Whether grass growth & spreading should be suppressed entirely. Default = false.
* `GrowEverywhere`: Whether grass spreads almost everywhere. If false, grass spreading is limited to tillable tiles. Default = true.
* `GrowthChance`: The chance that grass growth or spreads. Default = 0.65 (=65%).
* `SpreadChance`: The chance for each neighbouring tile that the grass will spread there. Default = 0.25 (=25%).
* `DailyGrowth`: The number of iterations that grass growth is applied per day (max=10). Default = 1.
* `MonthlyGrowth`: Additional iterations that grass growth is applied at the start of each month (max=100). Default = 40.

## Dependencies
This mod requires the following mods to be installed:

* [SMAPI](https://www.nexusmods.com/stardewvalley/mods/2400)
* [StardewHack](https://www.nexusmods.com/stardewvalley/mods/3213)
* [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) - optional

## Known bugs
Please report bugs on [GitHub](https://github.com/bcmpinc/StardewHack/issues).

## Changes
#### 6.0:
* Update [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) bindings.

#### 4.0:
* Added integration for [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098).
* Add 64-bit support
