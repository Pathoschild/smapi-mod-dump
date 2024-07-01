# Tilled Soil Decay

## Description
Prevents watered tilled soil from disappearing during the night. If you only want to change the decay rate, set the delays to 0. A rate of 0 means tilled soil will never disappear during the night. A rate of 1 means that it will always disappear (after the given delay).

## Config
*Note: run Stardew Valley once with this mod enabled to generate the `config.json` file.*

* `DryingRateMultiplier`: Chance multiplier that tilled soil will disappear. On the farm this chance is normally 10%, but this mod increases that to 50%.
* `Delay`: Number of consecutive days that the patch must have been without water, before it can disappear during the night. Normally this is 0, but for the farm this mod sets it to 2. Note that rain waters the patch and thus resets the days without water counter to 0.

## Dependencies
This mod requires the following mods to be installed:

* [SMAPI](https://www.nexusmods.com/stardewvalley/mods/2400)
* [StardewHack](https://www.nexusmods.com/stardewvalley/mods/3213)

## Known bugs
Please report bugs on [GitHub](https://github.com/bcmpinc/StardewHack/issues).

## Changes
#### 7.4:
* Korean translation.

#### 7.3:
* Fix soil outside of farm areas not decaying.

#### 7.2:
* Don't apply soil decay delay outside of farm areas.

#### 7.0:
* Updated for Stardew Valley 1.6
* Localization support.
* Removed most settings in favor of a single `DryingRateMultiplier`.

#### 6.0:
* Update [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) bindings.

#### 5.7:
* Soil decay at the start of a new season was not removed. This version allows you to configure that decay.

#### 5.6:
* Added back changing soil decay at the start of a new season, due to popular request.
* Updated for Stardew Valley 1.5.6

#### 5.2:
* Removed changing soil decay at the start of a new season, as it seems to have been removed from the game.

#### 5.1:
* Add support for Ginger Island

#### 4.0:
* Added integration for [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098).
* Add 64-bit support

#### 3.0:
* Added support for greenhouse soil.
