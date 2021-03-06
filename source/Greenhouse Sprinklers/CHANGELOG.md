**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Bpendragon/GreenhouseSprinklers**

----


<!-- TOC -->

- [v1.4.2](#v142)
- [v1.4.1](#v141)
- [v1.4.0 - Translations Update](#v140---translations-update)
- [v1.3.3](#v133)
- [v1.3.2](#v132)
- [v1.3.1](#v131)
- [v1.3.0 - Harmony Update](#v130---harmony-update)
- [v1.2.1](#v121)
- [v1.2.0 - SDV 1.5 Update](#v120---sdv-15-update)
- [v1.1.0 - Friendship Update](#v110---friendship-update)
- [v1.0.0](#v100)

<!-- /TOC -->
## v1.4.2
* Added Korean Translation from [Wally232](https://github.com/Wally232)
* Added Russian Translation from  [CatMartin](https://github.com/CatMartin)
## v1.4.1
* Fixed issue where Robin would claim the Greenhouse Upgrade was a prefab.
## v1.4.0 - Translations Update
* Added Translations
  * Italian - [Leecanit](https://github.com/LeecanIt)
  * Chinese - [Cccchelsea226](https://github.com/Cccchelsea226)
  * Spanish - [ManHeII](https://github.com/ManHeII)
* Added console command `ghs_waternow`
* Removed `DaysToConstruct` from blueprints. (will still exist in config files for now)
* Added documentation for console commands
* Added update token for github

## v1.3.3
* More stable method of using and converting ModData used.
  * ModData is 
* Fixed issue where upgrades would disapear on mod upgrade.
* Fixed issue where the Wizard would not send letters as needed.

## v1.3.2
* Changed method of getting the Greenhouse
## v1.3.1
* German/Deutsch translation, courtesy Kazel
* Bugfix for #3, "Cannot upgrade other buildings while mod is installed" 
## v1.3.0 - Harmony Update
* Content Patcher token now available
* Moved to using a Translation framework
  * French now available
  * Portuguese now available
* Option added to only allow so many upgrades
* Now Uses Harmony
* Got rid of the "Time To Upgrade" setting in config
  * Wasn't working well with changes required to make Harmony work
* Now works with Instant Build mods!
* Now uses the `modData` dictionary provided to modders by ConcernedApe
* Two new Console Commands:
  *  `ghs_setlevel [value]` Sets the greenhouse to the provided level
  * `ghs_getlevel` prints out the current Greenhouse level (great for debugging)
* `ModData` has been slimmed down.
  * Will be completely removed in future release

## v1.2.1
 * Can Now Turn off watering on the sandy areas of the Beach Farm

## v1.2.0 - SDV 1.5 Update

* Upgrades are now applied to the Greenhouse and not a Silo. 
  * Breaks SDV 1.4 Compatibility
* Visual Upgrades, when viewed from the outside sprinklers are visible in the rafters of the greenhouse.
* Additional config option "ShowVisualUpgrades" set this to false to only use the gameplay effects of this mod.

## v1.1.0 - Friendship Update

* Upgrades are now gated behind friendship with Wizard (as he's translating ideas the Junimos had)
* More difficult to unlock if you went the Joja Route, cannot get first upgrade until you have the required hearts AND have at least 1 Junimo Hut (you did kinda mess up the Community Center)
* Wizard now sends you letters telling you upgrades are available.
* Upgrades now take the correct amount of time to be built
* Upgrades cannot be purchased until Greenhouse unlocked

## v1.0.0
* Initial Release