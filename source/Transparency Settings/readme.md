**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Esca-MMC/TransparencySettings**

----

# Transparency Settings
 A mod for the game Stardew Valley that allows players to customize the distance at which trees, buildings, and various other objects become transparent. The mod generates a `config.json` file with detailed options and key bindings.

## Contents
* [Installation](#installation)
* [Settings](#settings)
    * [Object Type Settings](#object-type-settings)
    * [Key Bindings](#key-bindings)
* [Translation](#translation)
    * [Available Languages](#available-languages)


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

Note: Some objects can't be made transparent by this mod. Non-interactive objects "drawn" directly on the in-game maps, like town buildings or some trees/bushes, won't be affected.

Name | Valid settings | Description
-----|----------------|------------
Enable | **true** or false | If true, these objects will use your custom transparency settings. If false, they will use Stardew's default transparency system.
BelowPlayerOnly | **true** or false | If true, these objects will only be transparent when they are "below" the player's vertical position.
TileDistance | Any integer, e.g. **5** | These objects will be transparent while the player is within this number of tiles. Setting this to a negative number will disable transparency completely (e.g. `"TileDistance": -1`).
MinimumOpacity | A decimal from 0 to 1, e.g. **0.4** | This is how transparent objects can be. 0 is completely transparent, and 1 is completely visible.

### Key Bindings
These settings allows players to customize the key bindings for this mod's transparency toggle buttons.

These settings use SMAPI's [Multi-key bindings](https://stardewvalleywiki.com/Modding:Player_Guide/Key_Bindings#Multi-key_bindings) system. For more information about valid button names, refer to the wiki's [Key Bindings](https://stardewvalleywiki.com/Modding:Player_Guide/Key_Bindings) page.

Name | Default binding | Description
-----|-----------------|------------
DisableTransparency | None | Disables all custom transparency, reverting to Stardew's default system. Press it again to re-enable your custom settings.
FullTransparency | None | Enables maximum transparency, making all valid object types transparent regardless of distance. Press it again to re-enable your custom settings.

## Translation
This mod supports translation of its Generic Mod Config Menu (GMCM) setting names and descriptions.

The mod will load a file from the `TransparencySettings/i18n` folder that matches the current language code. If no matching translation exists, it will use [`default.json`](https://github.com/Esca-MMC/TransparencySettings/blob/master/TransparencySettings/i18n/default.json).

See the Stardew Valley Wiki's [Modding:Translations](https://stardewvalleywiki.com/Modding:Translations) page for more information. Please feel free to submit translation files through GitHub, Nexus Mods, ModDrop, or Discord.

### Available Languages
Language | File | Contributor(s)
---------|------|------------
English | [default.json](https://github.com/Esca-MMC/TransparencySettings/blob/master/TransparencySettings/i18n/default.json) | [Esca-MMC](https://github.com/Esca-MMC)
French | [fr.json](https://github.com/Esca-MMC/TransparencySettings/blob/master/TransparencySettings/i18n/fr.json) | [Caranud](https://www.nexusmods.com/users/745980)