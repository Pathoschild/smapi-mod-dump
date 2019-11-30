# Yet Another Harvest With Scythe Mod

## Description
Allows you to harvest all crops and forage using the scythe. They can also still be plucked, without having to press a toggle button (though this can be disabled in `config.json`). Unlike other 'harvest with scythe' mods, this patch does not affect your savegame. So it is possible to revert to the old behavior by simply removing this mod.

## Config
*Note: run Stardew Valley once with this mod enabled to generate the `config.json` file.*

* `HarvestMode`: Valid values are: `HAND` (only pluckable), `SCYTHE` (only scythable), `BOTH` (both pluckable and scythable).
  * `Flowers`: How flowers can be harvested. Default = BOTH.
  * `Forage`:  How forage can be harvested (Setting this to HAND disables all patches related to handling forage). Default = BOTH.
  * `SpringOnion`: How spring onions can be harvested. Default = BOTH.
  * `PluckableCrops`: How crops that normally can only be harvested by hand can be harvested. Default = BOTH.
  * `ScythableCrops`: How crops that normally can only be harvested with a scythe can be harvested. Default = SCYTHE.

## Dependencies
This mod requires the following mods to be installed:

* [SMAPI](https://www.nexusmods.com/stardewvalley/mods/2400)
* [StardewHack](https://www.nexusmods.com/stardewvalley/mods/3213)

## Known bugs
Please report bugs on [GitHub](https://github.com/bcmpinc/StardewHack/issues).

## Changes
#### 0.3:
* Removed dependecy on FixScytheExp. The bug patched by that mod has been fixed in StardewValley 1.3.25.

#### 0.5:
* There is no longer a difference in quality for additional harvest depending on whether it was harvested by hand or by scythe.
* Added option to configure whether additional harvest should receive a quality modifier.
* The HarvestSeeds option has been removed. The patch that ensures that sunflowers drop seeds when harvested with scythe will always be applied.

#### 0.6:
* Added option to disallow harvesting without the scythe.
* Added option to disallow harvesting colored flowers using the scythe.
* Fixed the size of dropped spring onions.
* Fixed flowers harvested with the scythe dropping only a single color.
* Fixed harvesting forage in the bat cave and mines. 

#### 1.0:
* Added dependency on StardewHack.

#### 1.1:
* Fix support for Stardew Valley 1.3.36 on MacOS.

#### 2.0:
* Updated for Stardew Valley 1.4
* Rewrote the mod, such that plucking is disallowed while the scythe is equipped. This should fix various issues with harvesting.
* Generalized config, such that harvest mode is configurable separately for every kind of crop/forage.
* The `AllHaveQuality` configuration parameter has been removed, the associated effect has been removed.
* The format of the config file has been rewritten.

#### 2.1:
* Now also works with the golden scythe.
