# Waterproof Items
 A mod for the game Stardew Valley, causing items dropped in water to float instead of being destroyed. It also includes an option to automatically teleport floating items to the nearest player, making them easier to retrieve.

## Contents
* [Installation](#installation)
* [Options](#options)

## Installation
1. **Install the latest version of [SMAPI](https://smapi.io/).**
2. **Download Waterproof Items** from [the Releases page on GitHub](https://github.com/Esca-MMC/WaterproofItems/releases), Nexus Mods, or ModDrop.
3. **Unzip Waterproof Items** into the `Stardew Valley\Mods` folder.

Multiplayer note:
* It is recommended that **all players** install this mod for multiplayer sessions. Items may only float if the host and all nearby farmhands have this mod.

## Options
Waterproof Items includes options to teleport floating items to the nearest player and/or disable the visual "floating" effect on items in water.

To edit these options:

1. **Run the game** using SMAPI. This will generate the mod's **config.json** file in the `Stardew Valley\Mods\WaterproofItems` folder.
2. **Exit the game** and open the **config.json** file with any text editing program.

This mod also supports [spacechase0](https://github.com/spacechase0)'s [Generic Mod Config Menu](https://spacechase0.com/mods/stardew-valley/generic-mod-config-menu/) (GMCM). Players with that mod will be able to change config.json settings from Stardew's main menu.

The available settings are:

Name | Valid settings | Description
-----|----------------|------------
EnableCosmeticFloatingEffect | **true** or false | If true, items in water will appear to move up and down on "waves". Disabling this may improve performance on some systems.
TeleportItemsOutOfWater | true or **false** | If true, items will teleport to the nearest player whenever they touch water. Enabling this makes it easier to retrieve items from water.
