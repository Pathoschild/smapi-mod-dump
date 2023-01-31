**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/daleao/sdv-mods**

----

# Modular Overhaul Core Change Log

## 1.3.1

### Fixed

* Improvements to Chinese localization.

## 1.3.0

### Added

* Added German translations by [FoxDie1986](https://www.nexusmods.com/stardewvalley/users/1369870).

## 1.2.3

## Changed

* Caught some more indexed enumerables which has been replaced with for loops.

## 1.2.2

### Fixed

* Fixed integer GMCM fields incorrectly displaying as decimals.

## 1.2.0

## Added

* Added Hyperlinq library.

## Changed

* Optimized most iterations, removing excessive use of Linq and Enumerators to reduce allocation, and replacing some instances with Hyperlinq.

## 1.1.0

### Changed

* Now using `ReadOnySpan` to split strings.
* Replace leftover reflected SpaceCore code with native.

### Fixed

* Now handles empty arguments in console commands.
* Players now hold their own mod data, rather than concentrating all data on the main player. This fixes some syncronization issues in splitscreen.
* Added parameterless constructors to mod projectiles, which apparently is required by the game for multiplayer syncronization. 

## 1.0.4

* Default DebugKey changed to RightShift / RightShoulder.

## 1.0.3

### Added

* Added Russian translations by [pavlo2906](https://www.nexusmods.com/stardewvalley/users/46516077).

### Fixed
* Added dependencies for Custom Ore Nodes and Custom Resource Clumps.
* Fixed update keys for this and optional files.

## 1.0.2

### Added

* Added Chinese translations by [xuzhi1977](https://www.nexusmods.com/users/136644498).
* Added Korean translations by [BrightEast99](https://www.nexusmods.com/users/158443518).
* Added Spanish and French translations.
* Revalidate console command now also removes Dark Swords from chests.
* Added support for Better Chests to prevent accidentally depositing the Dark Sword.

### Changed

* Updated Portuguese translation.

### Fixed

* Fixed some typos in default (English) localization.

## 1.0.0

### Changed

* Rebranded as MARGO.

### Fixed

* Fixed a possible memory leak in the shared Event Manager logic.

### Removed

* Removed Generic Mod Config Menu as a hard requirement.

## 0.9.7

### Changed

* Mod integrations now use the Singleton pattern.

### Added

* Added FirstSecondUpdateTickedEvent.

## 0.9.5

### Added

* Added Revert command, complementary to the Initialize command from Arsenal. This will undo the changes made by Arsenal to resolve possible issues after disabling the module.

## 0.9.4

### Added

* Updated portuguese translations by [Onemix](https://www.nexusmods.com/stardewvalley/users/39429640)

### Changed

* Reverted some Virtual Properties back to PerScreen Mod State where more appropriate.

### Fixed

* Fixed SecondsOutOfCombat not reseting on damaging monsters (incorrect parameter name in Harmony Postfix).

## 0.9.3

### Changed

* Renamed the mod folder to be less vague.

### Added

* Added asset invalidation when toggling a module.

## 0.9.0 (Initial release)

* Initial Version