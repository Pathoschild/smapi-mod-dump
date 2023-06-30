**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Achtuur/StardewTravelSkill**

----

# Multiplayer Exp sharing

Ever played a multiplayer game where you'd have to fight over who gets to harvest the crops, chop the trees and mine the diamond node? Well that is no more! With this mod, every exp point you gain is shared with nearby players so that everyone levels about equally.

This mod lets you redistribute the exp you get with nearby players, players on the same map, or globally. With default settings, the person doing the action gaining exp still gets the majority, however this can be configured to share exp equally. Every skill can also be toggled on and off to work with exp share.

# Important notes

* Every player in the server should have this mod, if they don't the exp sharing will not work properly and you will lose exp overall.
* Currently this mod only works with vanilla skills, excluding the unimplemented luck skill
* All settings in the `config.json` (or when you use the [GenericModConfigMenu](https://www.nexusmods.com/stardewvalley/mods/5098) mod) only apply to the exp you share. For example, if you disable the farming skill that means you will not share your experience gained from farming, but you can still receive farming exp from other players. For the best experience, I would recommend everyone use the same settings.

# Compatibility

This mod relies on the base game's exp and levelling system. Any mod that changes how that work will most likely break this mod, or cause unintended behaviour.

# Changelog

## Planned
* Change config to use the host's options

## 1.1.1
* New/Changed
  * Updated to support AchtuureCore 1.0.1, bringing with it better behaving exp share particles
  * Updated foraging color to be brown (previously yellowish-green, which was kind of invisible) 

## 1.1.0
* New/Changed
	* Added support for [MARGO](https://www.nexusmods.com/stardewvalley/mods/14470).
		* Note that there is some EXP waste in a specific situation. If you can level beyond 10, but your friends cannot and are currently level 10, then the shared exp is wasted. Make sure to prestige as soon as possible!
	* Now shows particle when you share exp!
		* C# devs can use interface to add colors for their own custom skill's trails
  * Added overlay (toggle with 'K') that shows exp sharing range!
	
* Fixes
	* Fix mod not loading without SpaceCore
	* Internal optimisations

## 1.0.5
* New/Changed
  * Updated compatibility with AchtuurCore 1.0.7

## 1.0.4
* Fixes
  * Fixed bug where Spacecore based skill mods loaded after MultiplayerExpShare were not recognised

## 1.0.3
* New/Changed content
  * Updated compatiblity with AchtuurCore 1.0.4

## 1.0.2
* New/Changed
  * Added option to share all exp in a skill when it's maxed (level 10)

## 1.0.1
* New/Changed
  * Added support for Spacecore based skill mods

* Fixes
  * Config now properly saves
## 1.0.0
* Initial release


