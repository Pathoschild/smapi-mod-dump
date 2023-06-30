**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/jag3dagster/AutoShaker**

----

**AutoShaker** is an open-source mod for [Stardew Valley](https://stardewvalley.net) that allows players to automatically shake trees and bushes simply by moving nearby to them.

## Documentation
### Overview
This mod checks for:
* Bushes that are currently blooming with berries or tea leaves
* Fruit trees that currently have fruit on them
* Trees that have a seed available to be shaken down

NOTE: This includes trees with hazelnuts, coconuts, and golden coconuts

### Config
* IsShakerActive
    * Whether or not the AutoShaker will shake bushes / trees
    * *Default* - true
* ToggleShaker
    * Which button will toggle the AutoShaker on and off when pressed while holding either Alt key
    * *Default* - LeftAlt + H, RightAlt + H
* ShakeRegularTrees
    * Whether or not the AutoShaker should try and shake regular trees for seeds
    * *Default* - true
* ShakeFruitTrees
    * Whether or not the AutoShaker should try and shake fruit trees for fruit
    * *Default* - true
* FruitsReadyToShake
    * Number of fruits (1-3) that need to be on a fruit tree before it is shaken
    * *Default* - 1
* ShakeTeaBushes
    * Whether or not the AutoShaker should try and shake tea bushes
    * *Default* - true
* ShakeBushes
    * Whether or not the AutoShaker should try and shake bushes
    * *Default* - true
* UsePlayerMagnetism
    * Whether or not to use the players current magnetism distance when looking for things to shake
    * This overrides the *ShakeDistance* config value
    * *Default* - false
* ShakeDistance
    * Distance in tiles that the AutoShaker will look for things to shake
    * This value is ignored if *UsePlayerMagnetism* is set to **true**
    * *Default* - 2


### Install
1. Install the latest version of [SMAPI](https://smapi.io)
    1. [Nexus Mirror](https://www.nexusmods.com/stardewvalley/mods/2400)
    2. [GitHub Mirror](https://github.com/Pathoschild/SMAPI/releases)
2. *OPTIONAL* Install the latest version of [Generic Mod Config Menu](https://spacechase0.com/mods/stardew-valley/generic-mod-config-menu/)
    1. [Nexus Mirror](https://www.nexusmods.com/stardewvalley/mods/5098)
3. Install this mod by unzipping the mod folder into 'Stardew Valley/Mods'
4. Launch the game using SMAPI

### Compatibility
* Compatible with...
    - Stardew Valley 1.5 or later
    - SMAPI 3.15.0 or later
* No known mod conflicts
    - If you find one please feel free to notify me here or on the [Nexus Mod](https://www.nexusmods.com/stardewvalley/mods/7736) site

## Limitations
### Solo + Multiplayer
* This mod is player specific, each player that wants to utilize it must have it installed
* Any bushes that have the potential to have berries on them will be shaken during berry seasons whether or not they have berries on them

## Releases
Releases can be found on [GitHub](https://github.com/jag3dagster/AutoShaker/releases) and on the [Nexus Mod](https://www.nexusmods.com/stardewvalley/mods/7736) site
### 1.5.2
* Fix tea bushes from constantly shaking
### 1.5.1
* Add Chinese localization
     - Translation by: liky123131231 (NexusMods)
### 1.5.0
* Simplify calculations per game tick
* Add translation language support
* Back-end versioning updates
* Thanks to @atravita-mods for this update
### 1.4.0
* Added the ability to shake Tea Bushes for their Tea Leaves
### 1.3.2
* Fixes a NullReferenceException thrown when a second user is joining a split-screen instance
* Updated the way the End-Of-Day messages are built
* Minor backend changes
### 1.3.1
* Fix for not shaking bushes when current language isn't set to English
* Updated default ShakeDistance from 1 to 2
* Minor backend changes
### 1.3.0
* Added the ability to specify the number of fruits (1-3) available on Fruit Tree before attempting to auto-shake it
* Minor backend changes
### 1.2.0
* Swapped config to have separate toggles for regular and fruit trees
* Added a check to ensure a user isn't in a menu when the button(s) for toggling the autoshaker are pressed
* Added some additional "early outs" when checking whether or not a tree or bush should be shaken
### 1.1.0
* Upgrading MinimumApiVerison to SMAPI 3.9.0
* Swap from old single SButton to new KeybindList for ToggleShaker keybind
   - Anyone who has a config.json file will no longer have to press an alt button to toggle the AutoShaker (unless they change their config.json file manually OR delete it and let it get regenerated the next time they launch Stardew Valley via SMAPI)
### 1.0.0
* Initial release
* Allows players to automatically shake trees and bushes by moving nearby to them
* Working as of Stardew Valley 1.5.3
