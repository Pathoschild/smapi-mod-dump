# Pollen Sprites
A mod for the game Stardew Valley, adding a new type of monster called a Pollen Sprite. They appear on windy days in spring, float around players harmlessly, and sometimes drop random seeds when defeated.

## Contents
* [Installation](#installation)
* [Options](#options)
* [Customization](#customization)

## Installation
1. **Install the latest version of [SMAPI](https://smapi.io/).**
2. **Install the latest version of [Content Patcher](https://www.nexusmods.com/stardewvalley/mods/1915).**
3. **Install the latest version of [Farm Type Manager](https://www.nexusmods.com/stardewvalley/mods/3231).**
4. **Download Pollen Sprites** from [the Releases page on GitHub](https://github.com/Esca-MMC/PollenSprites/releases), Nexus Mods, or ModDrop.
5. **Unzip Pollen Sprites** into the `Stardew Valley\Mods` folder.

Note for multiplayer: All players must have these mods installed. If only the host has the mods, other players will encounter SMAPI errors and be unable to see/interact with pollen sprites.

## Options

Pollen Sprites have some optional "allergy" effects and customizable seed drop chances. To edit these options:

1. **Run the game** using SMAPI. This will generate the mod's **config.json** file in the `Stardew Valley\Mods\PollenSprites` folder.
2. **Exit the game** and open the **config.json** file with any text editing program.

This mod also supports [spacechase0](https://github.com/spacechase0)'s [Generic Mod Config Menu](https://spacechase0.com/mods/stardew-valley/generic-mod-config-menu/) (GMCM). Users with that mod will be able to change config.json settings from Stardew's main menu.

The available settings are:

Name | Valid settings | Description
-----|----------------|------------
EnableSlowDebuff | true or **false** | If true, Pollen Sprites will apply a slow effect when they touch you. In multiplayer, this option only affects you.
EnableEnergyDrain | true or **false** | If true, Pollen Sprites will slowly drain your energy when they touch you (but never below 10 points). In multiplayer, this option only affects you.
SeedDropChances | N/A | The settings below decide how often Pollen Sprites drop seeds when defeated. Use 0 for a 0% chance, 0.45 for 45%, 1 for 100%, etc. In multiplayer, only the host's settings are used.
MixedSeeds | A number from 0.0 to 1.0 (default **0.45**) | The chance that Pollen Sprites will drop mixed seeds.
FlowerSeeds | A number from 0.0 to 1.0 (default **0.10**) | The chance that Pollen Sprites will drop random flower seeds.
AllSeeds | A number from 0.0 to 1.0 (default **0**) | The chance that Pollen Sprites will drop ANY random seeds, including from modded crops.

## Customization

The number of pollen sprites spawned, their locations, and other settings can be changed by editing the `Stardew Valley\Mods\[FTM] PollenSprites\content.json` file.

Farm Type Manager also includes a visual editor for this file. To use it:

1. Open the `Stardew Valley\Mods\FarmTypeManager\ConfigEditor.html` file with any web browser.
2. Click the "Load" button and select the `Stardew Valley\Mods\[FTM] PollenSprites\content.json` file.
3. Click the "Monsters" tab.
4. Edit any of the available settings.
5. Click the "Save" button and overwrite the `Stardew Valley\Mods\[FTM] PollenSprites\content.json` file.
