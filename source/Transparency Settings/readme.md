**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Esca-MMC/TransparencySettings**

----

# Transparency Settings
 A mod for the game Stardew Valley that allows players to customize the distance at which trees, buildings, and certain other large objects become transparent. The mod generates a `config.json` file with detailed options and key bindings.

## Contents
* [Installation](#installation)
* [Settings](#settings)
    * [Object Type Settings](#object-type-settings)
    * [Key Bindings](#key-bindings)


## Installation
1. **Install the latest version of [SMAPI](https://smapi.io/).**
2. **Download Transparency Settings** from [the Releases page on GitHub](https://github.com/Esca-MMC/TransparencySettings/releases), Nexus Mods, or ModDrop.
3. **Unzip Transparency Settings** into the `Stardew Valley\Mods` folder.

## Settings
Transparency Settings includes settings that control the distance and conditions for each object type to become transparent. It also includes customizable key bindings for some transparency toggle buttons. GMCM is recommended for easier customization (see below).

To edit these settings:

1. **Run the game** using SMAPI. This will generate the mod's **config.json** file in the `Stardew Valley\Mods\TransparencySettings` folder.
2. **Exit the game** and open the **config.json** file with any text editing program.

This mod also supports [spacechase0](https://github.com/spacechase0)'s [Generic Mod Config Menu](https://spacechase0.com/mods/stardew-valley/generic-mod-config-menu/) (GMCM). Players with that mod will be able to change the settings with in-game menus.

### Object Type Settings
These settings customize the behavior of each object type that can become transparent.

Note: Some objects cannot be transparent because they are static "hard-coded" features on the in-game texture maps. If a building cannot be built/moved by players (e.g. the main farmhouse) or if a tree/bush cannot be shaken, this mod will **not** make it transparent.

Name | Valid settings | Description
-----|----------------|------------
Enable | **true** or false | If true, these objects will use your custom transparency settings. If false, they will use Stardew's default transparency system.
BelowPlayerOnly | **true** or false | If true, these objects will only be transparent when they are "below" the player's vertical position.
TileDistance | Any integer, e.g. **5** | These objects will be transparent while the player is within this number of tiles. Setting this to a negative number will disable transparency completely (e.g. `"TileDistance": -1`).

### Key Bindings
These settings allows players to customize the key bindings for this mod's transparency toggle buttons.

These settings use SMAPI's [Multi-key bindings](https://stardewvalleywiki.com/Modding:Player_Guide/Key_Bindings#Multi-key_bindings) system. For more information about valid button names, refer to the wiki's [Key Bindings](https://stardewvalleywiki.com/Modding:Player_Guide/Key_Bindings) page.

Name | Default binding | Description
-----|-----------------|------------
DisableTransparency | `Left Shift` + `~` | Disables all custom transparency, reverting to Stardew's default system. Press it again to re-enable your custom settings.
FullTransparency | `~` | Enables maximum transparency, making all valid object types transparent regardless of distance. Press it again to re-enable your custom settings.
