# Bigger Backpack

## Description
Buy a bigger backpack at Pierre's for only 50,000g! This backpack has 48 slots.

## Console Commands
* `player_setbackpacksize slots`: Changes the size of your inventory. Valid values for slots are 12, 24, 36 and 48. Items in the removed slots will be spilled onto the ground.

## Dependencies
This mod requires the following mods to be installed:

* [SMAPI](https://www.nexusmods.com/stardewvalley/mods/2400)
* [StardewHack](https://www.nexusmods.com/stardewvalley/mods/3213)

## Known bugs
Please report bugs on [GitHub](https://github.com/spacechase0/BiggerBackpack/issues) or [Nexus](https://www.nexusmods.com/stardewvalley/mods/1845?tab=bugs).

## Uninstalling
To remove the mod, first run `player_setbackpacksize 36` in the SMAPI console and safe your game. 

## Changes
#### 1.1:
* Added dependency on StardewHack.
* Fix inventory for shops and shipping bins.
* Ring can now (hopefully) also be bought using gamepad.
* The backpack no longer overlays the player or deluxe backpack.
* Compatible with the [Wear More Rings](https://www.nexusmods.com/stardewvalley/mods/3214) mod.
* The `player_setbackpacksize` will spill items on the ground rather than deleting them.

#### 1.2:
* Fixed `player_setbackpacksize` not working when empty inventory slots are being removed.
* Add support for the community center.

#### 2.0:
* Updated for Stardew Valley 1.4

#### 2.1:
* Fix that equipment icons sometimes overlapped inventory.

#### 2.3:
* Fix position where money is drawn in shops.

#### 3.0:
* Allow content patcher mods, such as [Garden Variety UI](https://www.nexusmods.com/stardewvalley/mods/3879) to override the custom assets used in this mod.
