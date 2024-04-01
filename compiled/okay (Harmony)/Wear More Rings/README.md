# Wear More Rings

## Description
Configure the amount of ring slots in your inventory. By default adds 6 additional ring slots.

## Config
*Note: run Stardew Valley once with this mod enabled to generate the `config.json` file.*

* `Rings`: Number of ring slots available. Normally this is 2, this mod increases it to 8. Max = 20.
* `BonusTrinket`: Whether you get two trinket slots for the price of one (untested).

## Console Commands
* `player_openforge`:        Opens the forge menu.
* `world_destroyringchests`: Removes the chests used for storing player's rings. Any items contained therein will be dropped at your feet.

## Dependencies
This mod requires the following mods to be installed:

* [SMAPI](https://www.nexusmods.com/stardewvalley/mods/2400)
* [StardewHack](https://www.nexusmods.com/stardewvalley/mods/3213)

## Known bugs
Please report bugs on [GitHub](https://github.com/bcmpinc/StardewHack/issues).

* The forge menu does not show your ring slots.
* This mod does not support android / mobile.
* This mod might be incompatible with [Multiplayer Time](https://www.nexusmods.com/stardewvalley/mods/2543). Using both will cause save game corruption. This can (hopefully) be fixed by uninstalling either of these mods. If you keep Multiplayer Time, the mod should resolve the issue automatically. If you use Wear More Rings you need to execute the `player_resetmodifiers` console command manually.
* This mod is incompatible with [BalancedCombineManyRings](https://www.nexusmods.com/stardewvalley/mods/8981) mod.
* Ring Slots in the Inventory screen can overlap with other text in the UI, especially with a high `Rings` setting. This won't be fixed.

## API
Mods adding additional rings most should work out of the box. For backwards compatibility this mod provides an API to access the equipped rings: [IWearMoreRingsAPI](https://github.com/bcmpinc/StardewHack/blob/master/WearMoreRings/IWearMoreRingsAPI.cs).

## Changes
#### 7.4:
* Fix issue with equipping rings removing the effects and unequipping adding them.

#### 7.3:
* Fix issue with ring effects (e.g. magnetism) not being applied.

#### 7.2:
* Fixed compatibility issue with SpaceCore mod.

#### 7.1:
* Updated for Stardew Valley 1.6
* Added config option for bonus trinket (untested).
* Localization support.
* Russian & Ukrainian translation

#### 6.4:
* Fixed null pointer error when saving in generic mod config menu.

#### 6.3:
* Name the combined ring used for ring storage to clearly communicate its purpose. 
* Prevent crash caused by equipping the combined ring inside of itself.
* Add support for local co-op (aka split-screen).
* Removed ring slots from the forge menu so the player cannot accidentally unequip the container ring there.

#### 6.2:
* Added `player_dismantle_nested` command to get rid of nested combined rings, prior to accessing the forge.
* Fixed issue with re-joining in multiplayer creating nested combined rings.

#### 6.1:
* Re-added the [IWearMoreRingsAPI](https://github.com/bcmpinc/StardewHack/blob/master/WearMoreRings/IWearMoreRingsAPI.cs).
* Rings no longer drop during migration.
* If the player has rings in slots that are not accessible due to a lower ring capacity, these rings are dropped.

#### 6.0:
* Rewrite most of the mod for better multiplayer support. It now uses Combined Rings rather than a hidden chest for ring storage. Note that after the update you have to manually re-equip your rings.
* Update [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) bindings.
* Added `world_destroyringchests` command to destroy any remaining old rings chests.
* Removed the API as it's no longer needed.

#### 4.0:
* Added integration for [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098).
* Add 64-bit support

#### 3.4:
* Rearranged ring slots. The right ring is now positioned in the first column, second from the top.
* Added config parameter `Rings` for the amount of ring slots. Anything above 8 will result in text overlaying the ring slots.

#### 3.3:
* Hide the chest in the [Chests Anywhere](https://www.nexusmods.com/stardewvalley/mods/518) mod.
* Added the `player_resetmodifiers` command for people who added/removed rings from the hidden chest using Chests Anywhere to fix any issues this might have caused.

#### 3.2:
* Fixed issue with some effects of combined rings not being applied.

#### 3.1:
* Updated for Stardew Valley 1.5.
* Added two extra ring slots.
* Mod no longer modifies the network protocol.
* Rings are now stored in a way that works for combined and modded rings.

#### 3.0:
* Updated for Stardew Valley 1.4.4.
