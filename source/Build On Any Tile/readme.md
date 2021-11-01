**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Esca-MMC/BuildOnAnyTile**

----

# Build On Any Tile
 A mod for the game Stardew Valley, allowing buildings to be placed on otherwise nonvalid tiles, such as water and other obstructions.

## Contents
* [Installation](#installation)
* [Options](#options)
* [Translation](#translation)
     * [Available Translations](#available-translations)

## Installation
1. **Install the latest version of [SMAPI](https://smapi.io/).**
2. **Download Build On Any Tile** from [the Releases page on GitHub](https://github.com/Esca-MMC/BuildOnAnyTile/releases), Nexus Mods, or ModDrop.
3. **Unzip Build On Any Tile** into the `Stardew Valley\Mods` folder.

Multiplayer note:
* Each player who wants to place buildings on nonvalid tiles will need to install this mod, but it is not required otherwise.

## Options
Build On Any Tile includes options to enable or disable building placement on certain terrain types and obstructions.

To edit these options:

1. **Run the game** using SMAPI. This will generate the mod's **config.json** file in the `Stardew Valley\Mods\BuildOnAnyTile` folder.
2. **Exit the game** and open the **config.json** file with any text editing program.

This mod also supports [spacechase0](https://github.com/spacechase0)'s [Generic Mod Config Menu](https://spacechase0.com/mods/stardew-valley/generic-mod-config-menu/) (GMCM). Players with that mod will be able to change config.json settings from Stardew's main menu.

The available settings are:

Name | Valid settings | Description
-----|----------------|------------
BuildOnAllTerrainFeatures | **false**, true | Enable this to place buildings on crops, trees, and similar objects. **WARNING:** Buildings will immediately delete those objects when placed over them.
BuildOnOtherBuildings | **true**, false | Enable this to place buildings on tiles that already contain other buildings.
BuildOnWater | **true**, false | Enable this to place buildings on water tiles.
BuildOnImpassableTiles | **true**, false | Enable this to place buildings on impassable tiles, e.g. cliffs and other obstructions.
BuildOnNoFurnitureTiles | **true**, false | Enable this to place buildings on tiles with the "NoFurniture" property.
BuildOnCaveAndShippingZones | **true**, false | Enable this to place buildings on tiles in the preset cave entrance and shipping bin areas.

## Translation

Build On Any Tile supports translation of its Generic Mod Config Menu setting names and descriptions.

When Stardew Valley is launched, a file with the appropriate language code is loaded from the `BuildOnAnyTile/i18n` folder, or [`default.json`](https://github.com/Esca-MMC/BuildOnAnyTile/blob/master/BuildOnAnyTile/i18n/default.json) if no specific translation exists. Note that the game must be restarted to update GMCM's displayed language.

See the Stardew Valley Wiki's [Modding:Translations](https://stardewvalleywiki.com/Modding:Translations) page for more information. Please feel free to submit translation files via pull request, linking the file in a GitHub issue, or any other method.

### Available Translations

Language | File | Contributor(s)
---------|------|------------
English | [default.json](https://github.com/Esca-MMC/BuildOnAnyTile/blob/master/BuildOnAnyTile/i18n/default.json) | [Esca-MMC](https://github.com/Esca-MMC)
Korean | [ko.json](https://github.com/Esca-MMC/BuildOnAnyTile/blob/master/BuildOnAnyTile/i18n/zh.json) | [wally232](https://github.com/wally232)
Simplified Chinese | [zh.json](https://github.com/Esca-MMC/BuildOnAnyTile/blob/master/BuildOnAnyTile/i18n/zh.json) | [chasqiu](https://forums.nexusmods.com/index.php?/user/72860268-chasqiu/)
