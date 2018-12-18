# Harvest With Scythe
Allows you to harvest all crops and forage using the scythe. They can also still be plucked, without having to press a toggle button. Unlike other 'harvest with scythe' mods, this patch does not affect your savegame. So it is possible to revert to the old behavior by removing this patch.

## Config
Note: run Stardew Valley once with this mod enabled to generate the `config.json` file.
* HarvestForage: Should the game be patched to allow harvesting forage with the scythe? Default = true.
* AllHaveQuality: Should quality be applied to additional harvest? Default = false.
* ScytheHarvestFlowers: Can flowers be harvested with the scythe? Default = true.
* AllowManualHarvest: Whether crops should also remain pluckable by hand. Default = true;

## Known bugs
Please report bugs on: https://github.com/bcmpinc/StardewHack/issues

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
