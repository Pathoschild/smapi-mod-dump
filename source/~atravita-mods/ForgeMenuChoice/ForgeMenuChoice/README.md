**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/StardewMods**

----

Pick Your Enchantment/ForgeMenuChoice
===========================

![Header image](docs/tools.gif)

This mod lets you pick your enchantment at the forge. No longer do you have to save scum to get the enchantment you wanted! Just place your tool or weapon in the left slot, and a prismatic shard in the right, and use the little arrows to select your new enchantment! 

Much thanks to Khloe Leclair for the UI textures! I do strongly recommend you check out [her mods](https://www.nexusmods.com/stardewvalley/users/138772588?tab=user+files). Especially [Almanac](https://www.nexusmods.com/stardewvalley/mods/11022), which is downright gorgeous.

## Install

1. Install the latest version of [SMAPI](https://smapi.io).
2. Download and install [AtraCore](https://www.nexusmods.com/stardewvalley/mods/12932).
2. Download this mod and unzip it into `Stardew Valley/Mods`.
3. Run the game using SMAPI.

## Configuration
Run SMAPI with the mod installed at least once to generate the config.json file, or use Generic Mod Config Menu to configure.

1. `TooltipBehavior` adjusts whether or not the tooltips exist. Options are: On, Off, or Immersive, where Immersive only makes the tooltips appear if any farmer has seen Journal Scrap 8.
2. `EnableTooltipAutogeneration`: the data for the tooltips is parsed from Journal Scrap 9; if you have a mod that edits that, you may want to disable tooltips or remove automatic tooltip generation.

For mod-added enchantments: please let me know about them! The easiest way to make this mod show tooltips for your enchantment is by patching [`Mods/atravita_ForgeMenuChoice_Tooltip_Data`](ForgeMenuChoice/README.md). 

## Uninstall
Simply delete the mod from your Mods directory.

## Compatibility

* Works with Stardew Valley 1.5.6 on Linux/macOS/Windows.
* Works in single player, multiplayer, and split-screen mode. Should be fine if installed for one player only.
* Should be compatible with most other mods. Tested with [Spacecore](https://www.nexusmods.com/stardewvalley/mods/1348), [Many Enchantments](https://www.nexusmods.com/stardewvalley/mods/8824), and [Enchantable Scythes](https://www.nexusmods.com/stardewvalley/mods/10668). Note: this mod patches Spacecore's forge menu.

## Additionally:

* Thanks to [martin66789](https://forums.nexusmods.com/index.php?/user/27323031-martin66789/) for the Hungarian translation, love1997 for the Chinese translation, and c4ttzinhaa(https://github.com/atravita-mods/StardewMods/commits?author=c4ttzinhaa) for the Portuguese translation!

## See also

[Changelog](docs/Changelog.md)
