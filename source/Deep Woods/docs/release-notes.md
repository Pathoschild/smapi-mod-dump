**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/maxvollmer/DeepWoodsMod**

----

# <img src="deepwoods_icon.png" alt="DeepWoods Game Icon" height="50"/> DeepWoods

A mod for Stardew Valley that adds a procedurally generated infinite forest with secrets to discover, difficult challenges to overcome, and vast layers of levels to explore.

---

[Back to Main README](README.md)

---

### Release Notes

#### 3.1.0-beta
 - Fixed splitscreen multiplayer
 - Fixed crash bug in infested levels in multiplayer
 - Fixed WoodsObelisk not becoming available if reaching level 20 before having magic ink
 - Fixed an exploit with the Woods Obelisk
 - Added minecart to root level (activates when first entering the level, if minecarts are unlocked)
   - Works with Vanilla and MineCartPatcher
 - Horses lost in DeepWoods now return to their stable
 - Book shelves in the hut now give in-universe clues
 - Infested levels now have orb stones, clearing them activates orb stones in root level
   - (This is purely visual, no functionality yet)
 - Added fruit trees and extra forageables to config.json
 - Removed small signs from deeper levels (broke immersion)
 - Removed "Mod Info" button from all interactive menus, except the big wooden sign.
 - Updated some translations

---
#### 3.0.0-beta
  - Prepared compatibility for SMAPI 4
  - Performance improvement in forest generation
  - Fixed excalibur sword luck condition
  - Fixed weather debris generation (visual bug)
  - Make DeepWoods exit warp target configurable
  - Fixed a possible exploit introduced in 2.0.0-beta
  - Completely reworked first level and added new content.
    - See spoiler section on Nexusmods for details.
  - Note: Local splitscreen coop is still broken. Sorry :(

---
#### 2.0.0-beta
 - Added compatibility for SDV 1.5.5+ (64bit)
 - Added German and Hungarian translations (provided by Deflaktor and martin66789 respectively)
 - Fixed gingerbread house textures for winter and rest of year were switched.
 - Fixed rare crash with PyTK when encountering a unicorn.
 - DeepWoodsModAPI (for modders): Removed external nuget package, instead reference the DeepWoodsMod.dll to use the API.
 - Improved luck check for excalibur sword:
   - Player Luck Level reduced to 8, which is the highest value achievable in vanilla SDV with buffs.
   - Daily Luck needs to be positive.
 - Added more monsters. (Bug, Spider, Putrid Ghost, Dust Sprite.)
 - Added "dangerous" modifier for monsters (new 1.5 feature)
 - Added new and improved settings:
   - LightSourceDensity: Controls how many lights there are in a range from 0 - 100. 0 means no lights at all.
   - BaubleDensity: Controls how many baubles there are in a range from 0 - 100. 0 means no baubles at all.
   - LeafDensity: Controls how many leaves there are in a range from 0 - 100. 0 means no leaves at all.
   - GrassDensity: Controls how much grass there is in a range from 0 - 100. 0 means no grass at all.
   - MonsterDensity: Controls how many monsters there are in a range from 0 - 100. 0 means no monsters at all.
   - DisableBuffedMonsters: Self-explanatory.
   - DisableDangerousMonsters: Self-explanatory.
   - ChanceForNonDangerousMonster: Self-explanatory.
   - EnableGuaranteedClearings and GuaranteedClearingsFrequency: If enabled every level that is a multiple of GuaranteedClearingsFrequency is a clearing. (Default: 50, 100, 150, etc)
 - Local splitscreen coop is still broken. Sorry :(

---
#### 1.9.6-beta
 - Fixed crashes in multiplayer. Thanks to NexusWulf for extensive testing of intermediate versions.

---
#### 1.9.1-beta
 - Fixed crash in unicorns. Thanks to Esca-MMC (https://github.com/Esca-MMC) for providing the fix.

---
#### 1.9.0-beta
 - Added compatibility for SDV 1.5.
 - Small Russian translation update.
 - Removed easter eggs due to too many crash bugs that I don't have the time to fix.

---
#### 1.8.3-beta
 - Fixed Woods Obelisk cost settings not having any effect under certain circumstances.

---
#### 1.8.2-beta
 - Fixed crash bug caused by too early i18n initialization.

---
#### 1.8.1-beta
 - Fixed crash bug caused by other mod loading blueprints before i18n is initialized.

---
#### 1.8.0-beta
 - Added Chinese and Spanish translation. (Thanks, q847633684 and skullSG!)
 - Added proper description for Woods Obelisk.
 - Added acoustic feedback and HUD message when using healing fountain.
 - Fixed bug preventing players in multiplayer from entering DeepWoods levels.
 - Fixed bug preventing Woods Obelisk from being properly built.
 - Improved forest randomization algorithm to prevent groups in multiplayer getting separated as long as they stay not more than 1 level apart.

---
#### 1.7.1-beta
 - Fixed issues with fruit trees in DeepWoods levels.

---
#### 1.7.0-beta
 - Added compatibility for SDV 1.4. Big thanks to Mizzion for the temporary patched version until I was able to release this official update.

---
#### 1.6-beta.5
 - Fixed crash bug introduced in 1.6-beta.4.

---
#### 1.6-beta.4
 - Fixed issues with adding the DeepWoods entrance to ScreetWoods.
 - Added options to config file for changing where and how passage gets added.
 - Added new language files.
 - Introduced a game-breaking crash bug. Do not install!

---
#### 1.6-beta.3
 - Added support for DeepWoodsModAPI 2.0.2:
   - Adds a method for third party mods to warp the local player into the DeepWoods forest at a given level.

---
#### 1.6-beta.2
 - Fixed infinite recursion bug that crashes or freezes the game on load for some people.

---
#### 1.6-beta.0
 - Added compatibility for SMAPI 3.0. (Thanks to Pathoschild for all the help.)
 - Added ko language file (credit to https://github.com/S2SKY)
 - Added gingerbread house texture by technopoptard98

---
#### 1.5-beta.1
 - Updated DeepWoodsAPI to 2.0.1 and added support for replacing texture assets.

---
#### 1.4-beta.1
 - This is identical to 1.3-beta.18.

---
#### 1.3-beta.18
 - Updates by Pathoschild:
   - Added support for i18n translation files.
   - Progress is now safely stored in the save file.
   - Older data will be migrated automatically when you next save.
   - Fixed issue where the mod conflicts with SMAPI's multiplayer API.
   - Updated code for the upcoming SMAPI 3.0.

---
#### 1.3-beta.17
 - Fixed rare error where a dictionary was modified in a foreach loop when saving the game.

---
#### 1.3-beta.16
 - Updated asset by mostlyreal.
 - Fixed minor issue with fruit trees in the first level.
 - Hopefully fixed rare crash issues with Save Anywhere.

---
**Note**  
Versions 1.2.x and 1.3.x mentioned together below were feature identical, but released in parallel to support two game versions that existed in parallel.

---
#### 1.2-beta.18 / 1.3-beta.15
 - Added new assets by mostlyreal and zhuria.
 - Fixed minor issue with lighting and added compatibility with Dynamic Night Time.

---
#### 1.2-beta.17 / 1.3-beta.14
 - Fixed rare crash bug when using Save Anywhere.

---
#### 1.2-beta.16 / 1.3-beta.13
 - Minor fixes and code refactorings, updated API.

---
#### 1.2-beta.15 / 1.3-beta.12
 - Updated API.
 - Fixed version in README.txt.

---
#### 1.2-beta.14 / 1.3-beta.11
 - Added API.
 - Fixed crash bug with Save Anywhere.
 - Fixed minor bugs in animations and luck calculation.
 - Improved forest generation.

---
#### 1.2-beta.13 / 1.3-beta.10
 - Fixed bug where picking up an Easter Egg gives infinite Easter Eggs.

---
#### 1.2-beta.12 / 1.3-beta.9
 - Fixed Woods Obelisk not appearing in the Wizard's magic book menu.

---
#### 1.2-beta.11 / 1.3-beta.8
 - Fixed crystal fruits that couldn't be picked up.
 - Fixed explosions not destroying bushes, flowers, big resources and other custom items.
 - Fixed thorny bushes not appearing in higher levels.
 - Fixed mushroom clearings being empty.

---
#### 1.2-beta.10 / 1.3-beta.7
 - Fixed bug in multiplayer games where clients would get stuck at coords [0,0] when warping from a DeepWoods level into any level except Farm or Woods.

---
#### 1.2-beta.9 / 1.3-beta.6
 - Added compatibility for MTN and Save Anywhere.

---
#### 1.2-beta.8 / 1.3-beta.5
 - Fixed possible multiplayer conflicts with mods that replace the game's multiplayer class.

---
#### 1.2-beta.7 / 1.3-beta.4
 - Changed custom network id from 99 to 220 (PyTK uses 99, causing issues when using both mods).
 - Also made the network id configurable in the config.json.

---
#### 1.2-beta.5 / 1.3-beta.3
 - Fixed capitalization of dll name in manifest.json causing problems on Linux systems.
 - Fixed music not stopping when warping out of Deep Woods.

---
#### 1.2-beta.4 / 1.3-beta.2
 - Fixed issue where players get warped back to previous level when warping between levels.

---
#### 1.2-beta.3 / 1.3-beta.1
 - Earliest public release.
