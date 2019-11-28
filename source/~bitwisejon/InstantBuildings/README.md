**Instant Buildings from Farm** is a [Stardew Valley](http://stardewvalley.net/) v1.4x mod which lets you build any building normally sold by the carpenter directly from your farm without waiting for days for Robin to build them. Buildings are created/upgraded instantly, and the magician buildings are included too! The configuration file allows you to turn off the resource requirements needed to build so you can play around in a sandbox mode if you want.

## Contents
* [Install](#install)
* [Use](#use)
* [Buildings](#buildings)
* [Configuration](#configuration)
* [Mod Compatibility](#mod-compatibility)
* [Versions](#versions)
* [See also](#see-also)

## Install
1. [Install the latest version of SMAPI](https://smapi.io/).
2. Install [this mod from Nexus mods](http://www.nexusmods.com/stardewvalley/mods/2070).
3. Run the game using SMAPI.

## Use
Press the 'B' key (configurable) to open the building catalogue anywhere on your farm:
> ![](screenshots/catalogue.png)

Select a building to build and watch it happen instantly:
> ![](screenshots/instant-build.png)

Upgrades happen instantly too:
> ![](screenshots/instant-upgrade.png)

You can also move and destroy buildings directly from your farm:
> ![](screenshots/destroy-building.png)

## Buildings
All of the following buildings can be built instantly from the catalogue menu on your farm:
* Coop
* Barn
* Well
* Silo
* Mill
* Shed
* Cabins (for multiplayer)
* Stable (the horse will appear the next day)
* Tractor Garage (requires TractorMod and tractor will appear the next day)
* Slime Hutch
* Junimo Hut
* Earth Obelisk
* Water Obelisk
* Gold Clock

The following buildings can be upgraded instantly provided you already have the required building:
* Big Coop (requires Coop)
* Deluxe Coop (requires Big Coop)
* Big Barn (requires Barn)
* Deluxe Barn (requires Big Barn)

In multiplayer, any player can use the build menu and the new building will appear instantly for all players.

## Configuration
This mod creates a config.json file the first time you run it. Once created, open the file in a text editor to configure the mod. You can set the following options:

setting | default | effect
:------ | :------ | :-----
`BuildUsesResources` | `true` | Set to false to enable sandbox mode. Buildings will cost no gold or resources.
`AllowMagicalBuildingsWithoutMagicInk` | `false` | If false, will only allow the Magician buildings to be built once the player has the magic ink. Set to true for "sandbox" mode.
`ToggleInstantBuildMenuButton` | `B` | Change this value to bind the Instant Build menu to another key.

## Mod Compatibility
**Note:*** This mod is only compatible with version 1.4x of Stardew Valley (released 11/26/2019) and works in multiplayer as well. Starting with version 1.0.7 of this mod, Stardew Valley v1.3x is no longer supported. 

This mod is fully compatible with:
* [TractorMod](http://www.nexusmods.com/stardewvalley/mods/1401) by Pathoschild. The Tractor Garage will appear in the build menu if the mod is installed.
* [One Click Shed Reloader](http://www.nexusmods.com/stardewvalley/mods/2052) by me.

## Versions
See [release notes](release-notes.md).

## See also
* [Nexus mod](http://www.nexusmods.com/stardewvalley/mods/2070)

