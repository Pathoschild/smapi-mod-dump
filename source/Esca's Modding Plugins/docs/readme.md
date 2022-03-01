**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Esca-MMC/EscasModdingPlugins**

----

# Esca's Modding Plugins (EMP)
A mod for the game Stardew Valley that adds new data assets, map/tile properties, and features for other mods to use.

## Contents
* [Installation](#installation)
  * [Multiplayer Note](#multiplayer-note)
* [Features for Players](#features-for-players)
* [Features for Modders](#features-for-modders)

## Installation
1. **Install the latest version of [SMAPI](https://smapi.io/).**
2. **Download EMP** from [the Releases page on GitHub](https://github.com/Esca-MMC/EscasModdingPlugins/releases), Nexus Mods, or ModDrop.
3. **Unzip EMP** into the `Stardew Valley\Mods` folder.

Mods that use EMP should now work correctly.

### Multiplayer Note
* It is recommended that **all players** install this mod for multiplayer sessions.

## Features for Players
EMP lets players enable some of its features as "quality of life" changes for personal use.

Feature | Summary
--------|--------
Place beds anywhere | If enabled, you can place beds at any location.
Place mini-fridges anywhere | If enabled, you can place mini-fridges at any location.
Pass out safely anywhere | If enabled, you won't lose money if you pass out by staying awake too late (2:00 AM). NOTE: This doesn't change what happens if you run out of health.

To edit these options:

1. **Run the game** using SMAPI. This will generate the mod's **config.json** file in the `Stardew Valley\Mods\EscasModdingPlugins` folder.
2. **Exit the game** and open the **config.json** file with any text editing program.

EMP also supports [spacechase0](https://github.com/spacechase0)'s [Generic Mod Config Menu](https://spacechase0.com/mods/stardew-valley/generic-mod-config-menu/) (GMCM).

## Features for Modders
The table below summarizes each feature EMP provides for other mods.

**To use these features in your own mods, see the [EMP Modding Guide](emp-modding-guide.md).**

Feature | Summary
--------|--------
Bed Placement | Allow players to place moveable (furniture) beds at a location. Also includes an option to prevent money loss and NPC "rescue" letters if players run out of stamina at a location.
Custom Order Boards | Create new Special Orders boards that only display orders from a custom category. (See the [Data/SpecialOrders](https://stardewvalleywiki.com/Modding:Special_orders) asset.)
Destroyable Bushes | Mark bushes as destroyable at specific locations and/or tiles, allowing players to remove them with any upgraded axe.
Fish Locations | Give locations multiple "zones" with different fish, including fish from other locations. (See the [Data/Locations](https://stardewvalleywiki.com/Modding:Location_data) asset.)
Kitchen Features | Allow players to open the cooking menu at non-farmhouse locations, similar to [Action kitchen](https://stardewvalleywiki.com/Modding:Maps#Tile_properties_2). Also includes an option to allow players to place [Mini-Fridges](https://stardewvalleywiki.com/Mini-Fridge).
Water Color | Change the color of all water at a location.