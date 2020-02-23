# Wear More Rings

## Description
Adds 4 additional ring slots to your inventory.

## Dependencies
This mod requires the following mods to be installed:

* [SMAPI](https://www.nexusmods.com/stardewvalley/mods/2400)
* [StardewHack](https://www.nexusmods.com/stardewvalley/mods/3213)

## Known bugs
Please report bugs on [GitHub](https://github.com/bcmpinc/StardewHack/issues).

* The mod changes the network protocol. So you cannot connect to players who don't have this mod installed. In that case it will refuse to connect giving a "version mismatch" error.
* While rings from the [Giant Crop Ring](https://www.nexusmods.com/stardewvalley/mods/1182) mod can be equipped in the additional slots, their effects won't be applied. Rings from the [MoreRings](https://www.nexusmods.com/stardewvalley/mods/2054) mod, v1.0.3+ should work though.
* Rings from mods might disappear on load due to ID's of modded items changing between restarts. There's currently no easy way to fix this.

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
