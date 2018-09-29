**Build Health** is a [Stardew Valley](http://stardewvalley.net/) mod which lets you level up
your endurance to increase your max health as you play.

Compatible with Stardew Valley 1.2+ on Linux, Mac, and Windows.

## Installation
1. [Install the latest version of SMAPI](https://github.com/Pathoschild/SMAPI/releases).
2. Install [this mod from Nexus mods](http://www.nexusmods.com/stardewvalley/mods/445).
3. Run the game using SMAPI.

**NOTE:** to undo the mod's changes to your player, edit the `data\*.json` file for your save and
change the "ClearModEffects" field to `true`.

## Usage
You'll automatically get XP for...

* using tools;
* sleeping;
* eating or drinking;
* taking damage;
* running out of stamina (becoming exhausted);
* passing out (either by working while exhausted or or staying up late).

Get enough XP, and your health will level up.

Edit `config.json` to configure the mod settings.

## Versions
1.0
* Initial release.

1.1:
* Updated to Stardew Valley 1.1 and SMAPI 0.40 1.1-3.

1.3:
* Updated to Stardew Valley 1.2 and SMAPI 1.12.

1.4:
* Updated for SMAPI 2.0.
* Switched to standard JSON config & data files.
* Fixed issue where saves with the same name would overwrite each other's endurance level data.
* Fixed minor bugs when you load a save after exiting to title.
* Internal refactoring.

1.4.1:
* Enabled update checks in SMAPI 2.0+.
