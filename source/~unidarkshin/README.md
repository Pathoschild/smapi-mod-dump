
# Contents
* [**LevelExtender**](#levelextender)
* [Install](#le-install)
* [Use](#le-usage)
* [Configure](#le-config)
* [Compatibility](#le-compatibility)
* [Versions](#le-versions)
 --------------------------------------------------------
* [**Extreme Fishing Overhaul**](#extreme-fishing-overhaul)
* [Install](#install)
* [Use](#use)
* [Configure](#configure)
* [Compatibility](#compatibility)
* [Versions](#versions)
----------------------------------------------------------
## LevelExtender
[**Level Extender**](https://www.nexusmods.com/stardewvalley/mods/1471) is a [Stardew Valley](http://stardewvalley.net/) mod which extends the level cap to 100.
## LE Install
1. [Install the latest version of SMAPI](https://smapi.io).
2. Install this mod from [NexusMods](https://www.nexusmods.com/stardewvalley/mods/1471).
3. Run the game using SMAPI.

## LE Usage
The mod will...
* **Overhaul** the leveling system. Increasing the levels the player can reach, to 100.
* Fishing is overhauled past level 10. Levels after 10 now slow down the degradation of the reeling bar.
* Optional (false by default) monster spawning everywhere. The monster spawning is based on your combat level and has a small chance of spawning at any given game tick. These monsters are generally weaker by default, but scale based on your player's Combat level. 
* Skills have the same XP system UNTIL you reach level 10. The total XP to reach 100 is somewhere around 2,000,000 XP.
* Most skill changes past level 10 will show when you hover your mouse over a particular skill in the game menu.
* SDV has a host of features for each skill; based on skill level. It doesn't matter that the skills are over 10. For example: increased crop yield, based on Farming level.

## LE Config
A `username_(numbers).json` will appear in the mod's folder after you run the game once. It is **STRONGLY** recommended to not change anything in this file. It has a very high chance of causing corruption with the mod files. 

## LE Compatibility
* Works with Stardew Valley 1.3
* Works in single-player and multiplayer.
* Works with [Extreme Fishing Overhaul](https://www.nexusmods.com/stardewvalley/mods/2212), and is recommened to be used with [Extreme Fishing Overhaul](https://www.nexusmods.com/stardewvalley/mods/1471) for a better experience with the game. 

## LE Versions

### 1.3.11
* Initial release.
-------------------------------------------------
## Extreme Fishing Overhaul
[**Extreme Fishing Overhaul**](https://www.nexusmods.com/stardewvalley/mods/2212) is a [Stardew Valley](http://stardewvalley.net/) mod which makes the game's
fishing fun.

## Install
1. [Install the latest version of SMAPI](https://smapi.io).
2. Install this mod from [NexusMods](https://www.nexusmods.com/stardewvalley/mods/2212).
3. Run the game using SMAPI.

## Use
You can edit the `config.json` file to change the mod's settings.

The mod will...
* **Overhaul** the amount of fish in the game, with support of up to 2028 fish. Applying random names to the fish, 
    as well as giving them pictures 

  Note that the mod does support 2028 fish, however if you would like more, you can still generate more, the game just will 
  not have images for the fish.

## Configure
A `config.json` will appear in the mod's folder after you run the game once. You can optionally
open the file in a text editor to configure the mod. If you make a mistake, just delete the file
and it'll be regenerated.

Available fields:

field                     | purpose
------------------------- | -------
`seed`                    | What seed is being used to generate the random fish (In multiplayer all players need the same seed if they want the same fish.)
`maxFishingLevel`         | What the max fishing level is.
`minFishingLevel`         | What the minimum fishing level is.
`maxLevOverride`          | Set to true to Override max level when [Level Extender](https://www.nexusmods.com/stardewvalley/mods/1471) is installed.
`maxFish`                 | The amount of fish to be generated.

## Compatibility
* Works with Stardew Valley 1.3
* Works in single-player and multiplayer.
* Works with [Level Extender](https://www.nexusmods.com/stardewvalley/mods/1471), and is **strongly** recommened to be used with [Level Extender](https://www.nexusmods.com/stardewvalley/mods/1471)

## Versions

### 1.3.11
* Initial release.

