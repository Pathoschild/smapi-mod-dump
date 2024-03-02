# Yet Another Harvest With Scythe Mod

## Description
Allows you to harvest all crops and forage using the scythe. They can also still be plucked, without having to press a toggle button (though this can be disabled in `config.json`). Unlike other 'harvest with scythe' mods, this patch does not affect your savegame. So it is possible to revert to the old behavior by simply removing this mod.

## Config
*Note: run Stardew Valley once with this mod enabled to generate the `config.json` file.*

* `HarvestWithSword`: Whether a sword can be used instead of a scythe. Default = false.
* `HarvestMode`: Valid values are: `HAND` (only pluckable), `SCYTHE` (only scythable), `BOTH` (both pluckable and scythable), `GOLD` (both pluckable and scythable, but only with the golden scythe).
  * `PluckableCrops`: How crops that normally can only be harvested by hand can be harvested. Default = BOTH.
  * `ScythableCrops`: How crops that normally can only be harvested with a scythe can be harvested. Default = SCYTHE.
  * `Flowers`: How flowers can be harvested. Default = BOTH.
  * `Forage`:  How forage can be harvested (Setting this to HAND disables all patches related to handling forage). Default = BOTH.
  * `SpringOnion`: How spring onions can be harvested. Default = BOTH.

## Dependencies
This mod requires the following mods to be installed:

* [SMAPI](https://www.nexusmods.com/stardewvalley/mods/2400)
* [StardewHack](https://www.nexusmods.com/stardewvalley/mods/3213)

## Known bugs
Please report bugs on [GitHub](https://github.com/bcmpinc/StardewHack/issues).

* You cannot get golden walnuts on Ginger Island by harvesting crops using the scythe. 

## Changes
#### 6.0:
* Update [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) bindings.

#### 5.6:
* Fix issue with [Tractor Mod](https://www.nexusmods.com/stardewvalley/mods/1401) harvesting weeds as weeds.

#### 5.3:
* Sunflowers are now considered flowers.

#### 5.2:
* Less invasive method to allow using swords as scythe, to fix issues with swords' stats being invisible and being unable to trash or sell them.

#### 5.1
* Fix issue with swinging scythe when trying to pickup eggs.
* Fix issue with fertilizer not being usable when using the GOLD setting.
* Fix compatiblity issue with [MoreRings](https://www.nexusmods.com/stardewvalley/mods/2054) mod.

#### 4.0:
* Added integration for [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098).
* Added option to limit harvesting with scythe to the golden scythe.
* Added option to allow harvesting with sword.
* Add 64-bit support.
* Fix issue with harvesting forage using the [Tractor Mod](https://www.nexusmods.com/stardewvalley/mods/1401).

