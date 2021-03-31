**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/bcmpinc/StardewHack**

----

# Always Scroll Map

## Description
Makes the map scroll past the edge of the map. Useful for:

* planting seeds while walking;
* doing out-of-bounds glitches;
* checking for out of bounds objects.

It can be toggled on/off using the '`;`' button (or any other button you specify in `config.json`).

## Config
*Note: run Stardew Valley once with this mod enabled to generate the `config.json` file.*

* `EnabledIndoors`: Should the mod be enabled in indoor areas upon loading the game? Default = true.
* `EnabledOutdoors`: Should the mod be enabled in outdoor areas upon loading the game? Default = false.
* `ToggleScroll`: The key used to toggle always scroll map. Default = `OemSemicolon`.

## Dependencies
This mod requires the following mods to be installed:

* [SMAPI](https://www.nexusmods.com/stardewvalley/mods/2400)
* [StardewHack](https://www.nexusmods.com/stardewvalley/mods/3213)
* [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) - optional

## Known bugs
Please report bugs on [GitHub](https://github.com/bcmpinc/StardewHack/issues).

* On the bus-stop map, the HUD also moves when scrolling past the edge of the map.
* On a day with falling leaves, the leaves disappear when scrolling past the west edge of the map.

## Changes
#### 0.3:
* Added a configurable toggle button to enable/disable this mod during the game.

#### 0.5:
* Toggles for indoor and outdoor areas separately.

#### 0.6:
* Prepare mod to be SMAPI 3.0 compatible.

#### 1.0:
* Added dependency on StardewHack.

#### 2.0:
* Updated for Stardew Valley 1.4

#### 4.0:
* Added integration for [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098).
