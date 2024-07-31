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
#### 7.0:
* Localization support.
* Russian & Ukrainian translation

#### 6.0:
* Update [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) bindings.

#### 4.0:
* Added integration for [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098).
* Add 64-bit support
