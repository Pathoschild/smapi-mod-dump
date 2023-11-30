**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Esca-MMC/DestroyableBushes**

----

# Destroyable Bushes
A mod for the game Stardew Valley, allowing players to destroy every type of bush with an upgraded axe. Destroyed bushes drop small amounts of wood and respawn after 3 days. These features can be customized in the config.json file.

## Contents
* [Installation](#installation)
* [Options](#options)
* [Commands](#commands)
	* [add_bush](#add_bush)
	* [remove_bush](#remove_bush)
* [Translation](#translation)
	* [Available Languages](#available-languages)

## Installation
1. **Install the latest version of [SMAPI](https://smapi.io/).**
2. **Download Destroyable Bushes** from [the Releases page on GitHub](https://github.com/Esca-MMC/DestroyableBushes/releases), Nexus Mods, or ModDrop.
3. **Unzip Destroyable Bushes** into the `Stardew Valley\Mods` folder.

Multiplayer notes:
* This mod should affect each player separately. Players who want to destroy bushes should install Destroyable Bushes and customize their own options as needed.
* The host player's "WhenBushesRegrow" setting will be used.

## Options
Destroyable Bushes includes options to limit where bushes can be destroyed, change the speed at which bushes regrow, and change the amount of wood dropped by each type of bush.

To edit these options:

1. **Run the game** using SMAPI. This will generate the mod's **config.json** file in the `Stardew Valley\Mods\DestroyableBushes` folder.
2. **Exit the game** and open the **config.json** file with any text editing program.

This mod also supports [spacechase0](https://github.com/spacechase0)'s [Generic Mod Config Menu](https://spacechase0.com/mods/stardew-valley/generic-mod-config-menu/) (GMCM). Players with that mod will be able to change config.json settings from Stardew's main menu.

The available settings are:

Name | Valid settings | Description
-----|----------------|------------
WhenBushesRegrow | A number and a unit of time, e.g. **"3 days"** (or *null* to never regrow bushes) | If the unit is "days", bushes will respawn after that number of days. "Seasons" (or "months") will respawn bushes after that many seasons (on the first day of the season). "Years" will respawn bushes after that many years (on the first day of Spring).
DestroyableBushLocations | A list of location names, e.g. `["farm", "forest", "woods"]` | A list of locations where bushes will be destroyable. If the list is blank, bushes will be destroyable at all locations. Names should be in quotation marks and separated by commas. To find a location's name, you may need to use another mod such as [Debug Mode](https://www.nexusmods.com/stardewvalley/mods/679/).
DestroyableBushTypes | N/A | The settings below control which types of bush will be destroyable.
SmallBushes | **true** or false | If true, small bushes will be destoyable.
MediumBushes | **true** or false | If true, medium bushes will be destoyable. These are the type that can produce berries.
LargeBushes | **true** or false | If true, large bushes will be destoyable.
AmountOfWoodDropped | N/A | The settings below control how many pieces of wood are dropped by each bush type when destroyed. Players with the Forester profession will receive 25% more wood.
SmallBushes | A positive integer (default **2**) | The number of wood pieces dropped by small bushes when destroyed.
MediumBushes | A positive integer (default **4**) | The number of wood pieces dropped by medium bushes when destroyed.
LargeBushes | A positive integer (default **8**) | The number of wood pieces dropped by large bushes when destroyed.
GreenTeaBushes | A positive integer (default **0**) | The number of wood pieces dropped by green tea bushes when destroyed.

## Commands
This mod adds the following commands to SMAPI's console. They require the Console Commands mod, which is installed automatically by SMAPI.

### add_bush
The `add_bush` command creates a bush of the specified size. Bushes added by this command will regrow if that setting is enabled.

**Usage:** `add_bush <size> [x y] [location]`
* **size**: The bush's size, as a name or number. 0 = "small", 1 = "medium", 2 = "large", 3 = "tea".
* **x y** (optional): The bush's tile coordinates. If not provided, the bush will appear in front of the player.
* **location** (optional): The name of the bush's map, e.g. \"Farm\". If not provided, the player's current map will be used.

**Examples:**
* `add_bush 2`
* `add_bush large`
* `add_bush 2 64 19`
* `add_bush 2 64 19 farm`

### remove_bush
The `remove_bush` command removes a bush from the specified location. Bushes removed by this command will NOT regrow.

**Usage:** `remove_bush [x y] [location]`
* **x y** (optional): The bush's tile coordinates. If not provided, a bush will be removed on, or in front of, the player.
* **location** (optional): The name of the bush's map, e.g. \"Farm\". If not provided, the player's current map will be used.

**Examples:**
* `remove_bush`
* `remove_bush 64 19`
* `remove_bush 64 19 farm`

## Translation
Destroyable Bushes supports translation of its Generic Mod Config Menu (GMCM) setting names and descriptions.

The mod will load a file from the `DestroyableBushes/i18n` folder that matches the current language code. If no matching translation exists, it will use [`default.json`](https://github.com/Esca-MMC/DestroyableBushes/blob/master/DestroyableBushes/i18n/default.json).

See the Stardew Valley Wiki's [Modding:Translations](https://stardewvalleywiki.com/Modding:Translations) page for more information. Please feel free to submit translation files through GitHub, Nexus Mods, ModDrop, or Discord.

### Available Languages
Language | File | Contributor(s)
---------|------|------------
English | [default.json](https://github.com/Esca-MMC/DestroyableBushes/blob/master/DestroyableBushes/i18n/default.json) | [Esca-MMC](https://github.com/Esca-MMC)
Korean | [ko.json](https://github.com/Esca-MMC/DestroyableBushes/blob/master/DestroyableBushes/i18n/ko.json) | [wally232](https://github.com/wally232)
Turkish | [tr.json](https://github.com/Esca-MMC/DestroyableBushes/blob/master/DestroyableBushes/i18n/tr.json) | [Rupurudu](https://github.com/Rupurudu)
