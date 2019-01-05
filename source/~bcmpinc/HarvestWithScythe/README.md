# Yet Another Harvest With Scythe Mod

## Description
Allows you to harvest all crops and forage using the scythe. They can also still be plucked, without having to press a toggle button (though this can be disabled in `config.json`). Unlike other 'harvest with scythe' mods, this patch does not affect your savegame. So it is possible to revert to the old behavior by simply removing this mod.

## Config
*Note: run Stardew Valley once with this mod enabled to generate the `config.json` file.*

* `HarvestForage`: Should the game be patched to allow harvesting forage with the scythe? Default = true.
* `AllHaveQuality`: If the crop drops more than 1 harvest, should the additional harvest have the same quality as the first? Otherwise additional harvest will always have normal quality. Default = false.
* `ScytheHarvestFlowers`: Can flowers be harvested with the scythe? Note that if this is disabled, flowers can still be plucked by hand, regardless of whether plucking by hand is disabled. Default = true.
* `AllowManualHarvest`: Whether crops should also remain pluckable by hand. Default = true;

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
