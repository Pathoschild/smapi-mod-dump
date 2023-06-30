**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Achtuur/StardewTravelSkill**

----

# Stardew Travelling Skill

Travel skill for Stardew Valley, available on [NexusMods](https://www.nexusmods.com/stardewvalley/mods/16820/). The skill is leveled by walking around and requires about 300.000 steps to fully max.
Apart from SMAPI (god bless Pathoschild), this mod relies heavily on [SpaceCore](https://www.nexusmods.com/stardewvalley/mods/521), so a huge thanks to spacechase01 for making this possible.

# Features

- Adds new skill to the game using SpaceCore, meaning it is compatible with Cooking skill and Luck skill mods.
    - Skill levels up based on steps taken by player
    - Every level in the skill gives a configurable amount of movespeed
- Professions:
  - **Movespeed** - +5% movespeed
    - **Champion Runner** - Sprint after walking for a few steps
    - **Marathon Runner** - Regain stamina passively
  - **Warp Enjoyer** - Makes warp totems cheaper to craft
    - **Obelisk Builde** - Makes obelisks 50% cheaper to build (gold cost only)
    - **Totem reuser** - 50% chance to keep totem after use

# For content pack creators

This mod features some custom tokens for content pack creators that can tell you whether the local player has unlocked any of the professions. These tokens are:

* `Achtuur.StardewTravelSkill/hasProfessionMovespeed` - Local player has the _Seasoned Runner_ profession (lvl 5)
* `Achtuur.StardewTravelSkill/hasProfessionSprint` - Local player has the _Champion Runner_ profession (lvl 10)
* `Achtuur.StardewTravelSkill/hasProfessionRestoreStamina` - Local player has the _Marathon Runner_ profession (lvl 10)
* `Achtuur.StardewTravelSkill/hasProfessionCheaperWarpTotems` - Local player has the _Warp Enjoyer_ profession (lvl 5)
* `Achtuur.StardewTravelSkill/hasProfessionCheaperObelisks` - Local player has the _Cheaper Builder_ profession (lvl 10)
* `Achtuur.StardewTravelSkill/hasProfessionTotemReuse` - Local player has the _Totem Preserver_ profession (lvl 10)

# Known Compatibility

- SpaceCore based skill mods
  - [Cooking skill](https://www.nexusmods.com/stardewvalley/mods/522)
  - [Luck Skill](https://www.nexusmods.com/stardewvalley/mods/521)
- [Experience bars](https://www.nexusmods.com/stardewvalley/mods/509)
- [Generic Configuration Menu](https://www.nexusmods.com/stardewvalley/mods/5098) (try out the settings!)


# Changelog

## 1.1.7
* New/Changed
  * Added Simplified Chinese translation, thanks to Andrewxu 

## 1.1.6
* Fixes
 * Movespeed buff now only applied when manually moving, should no longer break cutscenes
 * Fix content patcher dependency requirement

## 1.1.5
* New/Changed content
  * Updated compatibility with AchtuurCore 1.0.7 

## 1.1.4
* New/Changed content
  * Updated compatiblity with AchtuurCore 1.0.4

## 1.1.3
* New/Changed content
    * Korean translation, thanks to poosha7375
    
* Fixes
  * Fix exp gain in splitscreen mode

## 1.1.2
* Fixes
  * Fix config saving properly
## 1.1.1
* New/Changed content
    * Added controller support

## 1.1.0
* New/Changed content
    * Added option for very fast exp gain (every 5 steps)
    * Rename movespeed profession to "Seasoned Runner"
    * Rename restore stamina profession to "Marathon Runner"
    * Rename sprinting profession to "Champion Runner"
    * Remove movespeed per level and movespeed profession bonus console commands, as those are handled by settings
    * Changed color of experience bar to a blue-ish green
* Fixes
    * Fixed stamina restore going over stamina cap   
    * Rework mod to use AssetRequested event, so `[CP] StardewTravelSkill` is now obsolete.
    * Fixed error showing in console when placing furniture
    * Removed config.json from archive
## 1.0.2
* Fixed versioning
## 1.0.1
 * Fixed release archive
## 1.0.0
* Initial release
* Contains
