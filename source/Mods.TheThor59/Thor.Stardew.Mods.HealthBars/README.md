**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/TheThor59/StardewMods**

----

# SD-EnemyHealthBars
Stardew Valley mod that displays enemies health bars

## Description
This mod has been created to display enemies health bars when playing to stardew valley. The bar displays using colors and life count indicator. The color change from green to red according to the ratio current life against max life.

The indicator will only display the health of enemies depending on the number of them have killed and your combat level.

![Screenshot](https://github.com/TheThor59/SD-EnemyHealthBars/blob/master/Thor.Stardew.Mods.HealthBars/images/screenshot1.png)

## Installing
Please ensure first that you have installed [SMAPI](https://www.smapi.io/).
To install this mod, please download the zip file and uncompress it in the mods folder of you Stardew Valley installation.

## Configuration
This mod has 2 configuration possibilities
- Invert coloring of the enemies life
- Deactivate experience requirements on life display

To edit the configuration, open the mods folder of your Stardew Valley installation and go inside EnemyHealthBars folder. Edit file config.json and :
- Set variable ColorScheme to 1 if you want to use alternative color scheme
- Set variable EnableXPNeeded to false if you want to deactivate experience requirements.

## Uninstalling
To uninstall this mod, just remove the folder EnemyHealthBars from you Stardew Valley mods folder.

## Improvements
I'll improve the portage as there are some display issues that I encountered and the design is not fully RP relatively to the game interface.

## Credits
This mod is a portage of an existing outdated mod that was no longer compatible with latest version of Stardew Valley and SMAPI. The initial author is Maurício Gomes. Exisiting mod informations are [here](https://www.nexusmods.com/stardewvalley/mods/193) and initial sources are located [here](https://gitlab.com/speeder1/SMAPIHealthbarMod).

## License
This code and this project is using [GPL3](https://gnu.org/licenses/gpl.html), same license as the initial project created by Maurício Gomes.
