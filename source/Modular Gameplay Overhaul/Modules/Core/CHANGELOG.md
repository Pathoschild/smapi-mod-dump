**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/daleao/modular-overhaul**

----

# Core Changelog

## 3.1.4

### Changed

* Now uses `IntegrationRegistry` from updated Shared lib.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 3.1.3

### Changed

* Rolled back dependency updates due to conflicts with AtraCore.

### Fixed

* Added missing translations. Improved some Chinese translations, thanks to xuzhi1977.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 3.1.2

### Added

* The config menu will now detect if gamepad mode is enabled and choose a controller-friendly layout. The detection only happens when the menu is constructed, and will not change dynamically if the player switches between gamepad mode and mouse/keyboard mode.
* Added Korean GMCM translations by [Jun9273](https://github.com/Jun9273).
* Added Chinese GMCM translations by [Jumping-notes](https://github.com/Jumping-notes).

### Changed

* Several lookups refactored to Shared lib.

### Fixed

* Added some missing translations and corrected some translation keys.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 3.0.2

### Added

* Added debuff framework to the API.

### Fixed

* Fixed a few more translation issues.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 3.0.1

### Fixed

* Fixed some missing translation issues.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 3.0.0

### Changed

* Changed several translation keys for better formatting with Pathoschild's Translation Generator. Possible mistakes may have been created.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.5.1

### Changed

* Lowered log level of mod conflicts to Debug.

### Fixed

* Fixed patching order of IClickableMenu.drawHoverText.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.5.0

### Changed

* Simplified the subfolders within assets. All textures are now consolidated under "sprites".

### Fixed

* Fixed an issue with patchers not obaying priority, which caused, for example, Automated Cheese Press to ignore Artisan bonuses when processing Large Milk, among most likely several other unidentified bugs.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.4.0

### Added

* Debug mode now also shows NPC names and current health in the case of monsters.

### Fixed

* Moved Textures out of Core and to each individual module, preventing a possible error when initializing if certain modules are disabled.
* Fixed core menu text not using correct translation keys for module selection option.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.3.0

### Added

* Added income and property tax information to the API.
* Added Pathoschild.Stardew.ModTranslationClassBuilder.
* Added all GMCM text options to i18n model, so the mod config menu can now be translated.

### Changed

* Cleaned up several config menus in the transition to Pathoschild.Stardew.ModTranslationClassBuilder.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.2.6

### Fixed

* Fixed a bug preventing the config menu from reloading correctly.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.2.5

### Fixed

* Made some changes to Integration framework to allow integrations to register when modules are activated via GMCM.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.2.3

### Added

* You can now set a key to directly open this mod's GMCM (default LeftShift + F12, because I use F12 as the GMCM key).
* Added some extra highlighting to debug mode for terrain features, large terrain features and objects.

### Changed

* Changed default DebugKey to "OemQuotes, OemTilde" (same as DebugMode mod).

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.2.2

### Added

* Moved the API interface from the shared library to the mod source.

### Changed

* Moved some more stuff around.

### Removed

* Status conditions now live in CMBT. Because duh.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.2.1

### Fixed

* Checksum validation now happens on Saving instead of Saved, allowing it to persist immediately instead of only on the next day.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.2.0

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

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.0.5

### Fixed

* Default value for Combat module has been correctly set to `false`. This would have prevented the initial setup wizard from triggering.
* Updated SpaceCore minimum version requirement.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.0.1

### Fixed

* Hotfix for GMCM crash.
* Added some missing GMCM config options and fixed some typos.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 2.0.0

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

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.3.2

### Added

* Added separators from GMCM options.

### Changed

* Added headers to GMCM menus.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.3.1

### Fixed

* Improvements to Chinese localization.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.3.0

### Added

* Added German translations by [FoxDie1986](https://www.nexusmods.com/stardewvalley/users/1369870).

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.2.3

## Changed

* Caught some more indexed enumerables which has been replaced with for loops.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.2.2

### Fixed

* Fixed integer GMCM fields incorrectly displaying as decimals.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.2.0

## Added

* Added Hyperlinq library.

## Changed

* Optimized most iterations, removing excessive use of Linq and Enumerators to reduce allocation, and replacing some instances with Hyperlinq.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.1.0

### Changed

* Now using `ReadOnySpan` to split strings.
* Replace leftover reflected SpaceCore code with native.

### Fixed

* Now handles empty arguments in console commands.
* Players now hold their own mod data, rather than concentrating all data on the main player. This fixes some syncronization issues in splitscreen.
* Added parameterless constructors to mod projectiles, which apparently is required by the game for multiplayer syncronization. 

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.0.4

### Changed

* Default DebugKey changed to RightShift / RightShoulder.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.0.3

### Added

* Added Russian translations by [pavlo2906](https://www.nexusmods.com/stardewvalley/users/46516077).

### Fixed
* Added dependencies for Custom Ore Nodes and Custom Resource Clumps.
* Fixed update keys for this and optional files.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

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

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.0.0

### Changed

* Rebranded as MARGO.

### Fixed

* Fixed a possible memory leak in the shared Event Manager logic.

### Removed

* Removed Generic Mod Config Menu as a hard requirement.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 0.9.7

### Changed

* Mod integrations now use the Singleton pattern.

### Added

* Added FirstSecondUpdateTickedEvent.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 0.9.5

### Added

* Added Revert command, complementary to the Initialize command from Arsenal. This will undo the changes made by Arsenal to resolve possible issues after disabling the module.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 0.9.4

### Added

* Updated portuguese translations by [Onemix](https://www.nexusmods.com/stardewvalley/users/39429640)

### Changed

* Reverted some Virtual Properties back to PerScreen Mod State where more appropriate.

### Fixed

* Fixed SecondsOutOfCombat not reseting on damaging monsters (incorrect parameter name in Harmony Postfix).

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 0.9.3

### Changed

* Renamed the mod folder to be less vague.

### Added

* Added asset invalidation when toggling a module.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 0.9.0 (Initial release)

* Initial Version

[🔼 Back to top](#core-changelog)