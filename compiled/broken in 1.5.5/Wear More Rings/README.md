# Wear More Rings

## Description
Configure the amount of ring slots in your inventory. By default adds 6 additional ring slots.

## Config
*Note: run Stardew Valley once with this mod enabled to generate the `config.json` file.*

* `Rings`: Number of ring slots available. Normally this is 2, this mod increases it to 8. Max = 20.

## Console Commands
* `player_resetmodifiers`: Clears buffs, then resets and reapplies the modifiers applied by boots & rings.

## Dependencies
This mod requires the following mods to be installed:

* [SMAPI](https://www.nexusmods.com/stardewvalley/mods/2400)
* [StardewHack](https://www.nexusmods.com/stardewvalley/mods/3213)

## Known bugs
Please report bugs on [GitHub](https://github.com/bcmpinc/StardewHack/issues).

* This mod does not support android / mobile.
* While rings from the [Giant Crop Ring](https://www.nexusmods.com/stardewvalley/mods/1182) mod can be equipped in the additional slots, their effects won't be applied. Rings from the [MoreRings](https://www.nexusmods.com/stardewvalley/mods/2054) mod, v1.0.3+ should work though.
* This mod is incompatible with [Multiplayer Time](https://www.nexusmods.com/stardewvalley/mods/2543). Using both will cause save game corruption. This can (hopefully) be fixed by uninstalling either of these mods. If you keep Multiplayer Time, the mod should resolve the issue automatically. If you use Wear More Rings you need to execute the `player_resetmodifiers` console command manually.

## Changes
#### 1.0:
* Created this mod. It hasn't been extensively tested and hence might still have some bugs.

#### 1.1:
* Fixed horse animation and dismounting bug.

#### 1.2:
* Added Mod Integration API, [IWearMoreRingsAPI](https://github.com/bcmpinc/StardewHack/blob/master/WearMoreRings/IWearMoreRingsAPI.cs), for mods that add new types of rings.
* Fixed error due to save methods being called on multiplayer clients.
* Fixed issue with unequipping one ring disabling the glow effect of all rings.
* Fixed rings disappearing when shift+clicking them in your inventory.

#### 1.3:
* Fix exception during startup on windows.

#### 1.4:
* Fixed old rings not working & disappearing on save&reload (hopefully).

#### 1.5:
* Don't delete all rings if one has an unknown object ID.
* Fix support for Stardew Valley 1.3.36 on MacOS.

#### 1.6:
* Change the network protocol version to prevent people with the mod from connecting to servers without the mod and vice-versa.

#### 2.1:
* Updated for Stardew Valley 1.4.

#### 2.2:
* Fix issue with shift-click equipping items not properly updating the GUI.
* Fix startup issues on MacOS & Windows.

#### 3.0:
* Updated for Stardew Valley 1.4.4.

#### 3.1:
* Updated for Stardew Valley 1.5.
* Added two extra ring slots.
* Mod no longer modifies the network protocol.
* Rings are now stored in a way that works for combined and modded rings.

#### 3.2:
* Fixed issue with some effects of combined rings not being applied.

#### 3.3:
* Hide the chest in the [Chests Anywhere](https://www.nexusmods.com/stardewvalley/mods/518) mod.
* Added the `player_resetmodifiers` command for people who added/removed rings from the hidden chest using Chests Anywhere to fix any issues this might have caused.

#### 3.4:
* Rearanged ring slots. The right ring is now positioned in the first column, second from the top.
* Added config parameter `Rings` for the amount of ring slots. Anything above 8 will result in text overlaying the ring slots.

#### 4.0:
* Added integration for [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098).
* Add 64-bit support
