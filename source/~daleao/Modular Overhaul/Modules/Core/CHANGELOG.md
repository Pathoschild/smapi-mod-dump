**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/daleao/sdv-mods**

----

# Core Changelog

## 2.5.1 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

### Changed

* Lowered log level of mod conflicts to Debug.

### Fixed

* Fixed patching order of IClickableMenu.drawHoverText.

## 2.5.0 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

### Changed

* Simplified the subfolders within assets. All textures are now consolidated under "sprites".

### Fixed

* Fixed an issue with patchers not obaying priority, which caused, for example, Automated Cheese Press to ignore Artisan bonuses when processing Large Milk, among most likely several other unidentified bugs.

## 2.4.0 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

### Added

* Debug mode now also shows NPC names and current health in the case of monsters.

### Fixed

* Moved Textures out of Core and to each individual module, preventing a possible error when initializing if certain modules are disabled.
* Fixed core menu text not using correct translation keys for module selection option.

## 2.3.0 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

### Added

* Added income and property tax information to the API.
* Added Pathoschild.Stardew.ModTranslationClassBuilder.
* Added all GMCM text options to i18n model, so the mod config menu can now be translated.

### Changed

* Cleaned up several config menus in the transition to Pathoschild.Stardew.ModTranslationClassBuilder.

## 2.2.6 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

### Fixed

* Fixed a bug preventing the config menu from reloading correctly.

## 2.2.5 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

### Fixed

* Made some changes to Integration framework to allow integrations to register when modules are activated via GMCM.

## 2.2.3 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

### Added

* You can now set a key to directly open this mod's GMCM (default LeftShift + F12, because I use F12 as the GMCM key).
* Added some extra highlighting to debug mode for terrain features, large terrain features and objects.

### Changed

* Changed default DebugKey to "OemQuotes, OemTilde" (same as DebugMode mod).

## 2.2.2 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

### Added

* Moved the API interface from the shared library to the mod source.

### Changed

* Moved some more stuff around.

### Removed

* Status conditions now live in CMBT. Because duh.

## 2.2.1 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

### Fixed

* Checksum validation now happens on Saving instead of Saved, allowing it to persist immediately instead of only on the next day.

## 2.2.0 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

### Added

* Added StackableBuff class.
* Added Status Effect framework. Each status condition has a neat correponding animation.
    - **Bleeding:** Causes damage every second. Damage increases exponentially with each additional stack. Stacks up to 5x. Does not affect Ghosts, Skeletons, Golems, Dolls or Mechanical enemies (ex. Dwarven Sentry).
    - **Burning:** Causes damage equal to 1/16th of max health every 3s for 15s, and reduces attack by half. Does not affect fire enemies (i.e., Lava Lurks, Magma Sprites and Magama Sparkers).
    - **Chilled:** Reduces movement speed by half for 5s. If Chilled is inflicted again during this time, then causes Freeze.
    - **Freeze:** Cannot move or attack for 30s. The next hit during the duration deals double damage and ends the effect.
    - **Poisoned:** Causes damage equal to 1/16 of max health every 3s for 15s, stacking up to 3x.
    - **Slowed:** Reduces movement speed by half for the duration.
    - **Stunned:** Cannot move or attack for the duration.

### Changed

* Debug mode is now a toggle.

## 2.0.5 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

### Fixed

* Default value for Combat module has been correctly set to `false`. This would have prevented the initial setup wizard from triggering.
* Updated SpaceCore minimum version requirement.

## 2.0.1 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

### Fixed

* Hotfix for GMCM crash.
* Added some missing GMCM config options and fixed some typos.

## 2.0.0 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

### Added

* Added initial setup "wizard".
* Added Japanese translations by [sakusakusakuya](https://www.nexusmods.com/stardewvalley/users/155983153).


### Changed

* Improved GMCM menu with multi-column options and separators.
* Stun animation moved to Combat module.
* The entry commands for all modules have changed, now using the ticker-like acronyms, which are displayed in the titles of each module's README, in (parentheses).

### Fixed

* Added updated Russian localization by [romario314](https://www.nexusmods.com/stardewvalley/users/68548986).

### Removed

* Removed Hyperlinq library. *"Overoptimization is the root of all evil."*

## 1.3.2 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

### Added

* Added separators from GMCM options.

### Changed

* Added headers to GMCM menus.

## 1.3.1 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

### Fixed

* Improvements to Chinese localization.

## 1.3.0 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

### Added

* Added German translations by [FoxDie1986](https://www.nexusmods.com/stardewvalley/users/1369870).

## 1.2.3 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

## Changed

* Caught some more indexed enumerables which has been replaced with for loops.

## 1.2.2 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

### Fixed

* Fixed integer GMCM fields incorrectly displaying as decimals.

## 1.2.0 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

## Added

* Added Hyperlinq library.

## Changed

* Optimized most iterations, removing excessive use of Linq and Enumerators to reduce allocation, and replacing some instances with Hyperlinq.

## 1.1.0 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

### Changed

* Now using `ReadOnySpan` to split strings.
* Replace leftover reflected SpaceCore code with native.

### Fixed

* Now handles empty arguments in console commands.
* Players now hold their own mod data, rather than concentrating all data on the main player. This fixes some syncronization issues in splitscreen.
* Added parameterless constructors to mod projectiles, which apparently is required by the game for multiplayer syncronization. 

## 1.0.4 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

* Default DebugKey changed to RightShift / RightShoulder.

## 1.0.3 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

### Added

* Added Russian translations by [pavlo2906](https://www.nexusmods.com/stardewvalley/users/46516077).

### Fixed
* Added dependencies for Custom Ore Nodes and Custom Resource Clumps.
* Fixed update keys for this and optional files.

## 1.0.2 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

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

## 1.0.0 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

### Changed

* Rebranded as MARGO.

### Fixed

* Fixed a possible memory leak in the shared Event Manager logic.

### Removed

* Removed Generic Mod Config Menu as a hard requirement.

## 0.9.7 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

### Changed

* Mod integrations now use the Singleton pattern.

### Added

* Added FirstSecondUpdateTickedEvent.

## 0.9.5 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

### Added

* Added Revert command, complementary to the Initialize command from Arsenal. This will undo the changes made by Arsenal to resolve possible issues after disabling the module.

## 0.9.4 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

### Added

* Updated portuguese translations by [Onemix](https://www.nexusmods.com/stardewvalley/users/39429640)

### Changed

* Reverted some Virtual Properties back to PerScreen Mod State where more appropriate.

### Fixed

* Fixed SecondsOutOfCombat not reseting on damaging monsters (incorrect parameter name in Harmony Postfix).

## 0.9.3 <sup><sub><sup>[🔼 Back to top](#core-change-log)</sup></sub></sup>

### Changed

* Renamed the mod folder to be less vague.

### Added

* Added asset invalidation when toggling a module.

## 0.9.0 (Initial release)

* Initial Version

[🔼 Back to top](#core-change-log)