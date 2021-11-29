**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/cantorsdust/StardewMods**

----

﻿[← back to readme](README.md)

# Release notes
## 2.5.1
Released 27 November 2021 for SMAPI 3.12.5 or later. Updated by Pathoschild.

* Fixed the default time speed in the Skull Caverns being faster than in the base game.
* Fixed `FreezeTimeAt` option not applied/unapplied when you change the time of day manually.
* Fixed `FreezeTimeAt` option not applied if you set a time which isn't a multiple of 10 (like `2205`).
* Fixed `FreezeTimeAt` option not resuming time when the next day starts.

## 2.5
Released 26 March 2020 for SMAPI 3.9.5 or later. Updated by Pathoschild.

* Added [multi-key binding](https://stardewvalleywiki.com/Modding:Player_Guide/Key_Bindings#Multi-key_bindings) support.
* Added translation support.
* Added options to configure time speed for the `SkullCavern`, `VolcanoDungeon`, and [`DeepWoods`](https://www.nexusmods.com/stardewvalley/mods/2571) areas.
* You can now change time during events.
* Updated for split-screen mode in Stardew Valley 1.5.
* The mod is now enabled on festival days by default. (This doesn't affect players who already have a `config.json` file.)
* Fixed compatibility with [unofficial 64-bit mode](https://stardewvalleywiki.com/Modding:Migrate_to_64-bit_on_Windows).

**Breaking changes:**
* The `Mine` option in `TickLengthsByLocation` no longer applies to the Skull Cavern (see `SkullCavern` for that).

## 2.4.3
Released 26 November 2019 for SMAPI 3.0 or later. Updated by Pathoschild.

* Updated for Stardew Valley 1.4.

## 2.4.2
Released 27 December 2018 for SMAPI 2.9.3 or later. Updated by Pathoschild.

* Updated for SMAPI 3.0.

## 2.4.1
Released 01 September 2018 for SMAPI 2.8 or later. Updated by Pathoschild.

* Changed the default time speed to match the game, except indoors where it runs at half-speed. (Previously time ran at half-speed everywhere by default.)

## 2.4
Released 09 August 2018 for SMAPI 2.6 or later. Updated by Pathoschild.

* Updated for Stardew Valley 1.3.

## 2.3
Released 25 January 2018 for SMAPI 2.4 or later. Updated by Pathoschild.

* Updated to SMAPI 2.4.
* Added automatic update checks via SMAPI.
* Added controller support.
* Added `Mine` as an option in per-location settings.

## 2.2.1
Released 02 June 2017 for SMAPI 1.14 or later. Updated by Pathoschild.

* Updated to SMAPI 1.14.

## 2.2
Released 24 April 2017 for SMAPI 1.10 or later. Updated by Pathoschild.

* Updated for Stardew Valley 1.2.

## 2.1
Released 05 April 2017 for SMAPI 1.9 or later. Updated by Pathoschild.

* Updated to SMAPI 1.9.
* Internal refactoring.

## 2.0.3
Released 29 January 2017 for SMAPI 1.7 or later.

* Fixed hotkeys being processed when a menu is open.

## 2.0.2
Released 26 December 2016 for SMAPI 1.4 or later.

* Fixed hotkeys being processed before game is loaded.
* Fixed an issue when time is frozen between ticks.

## 2.0.1
Released 25 October 2016 for SMAPI 0.40 or later.

* Added Linux/Mac support.
* Fixed config resetting & parsing.

## 2.0
Released 23 October 2016 for SMAPI 0.40 or later.

* Internal refactoring.

## 1.9.5
Released 21 October 2016 for SMAPI 0.40 or later.

* Fixed time scaling in game areas that have a different clock rate.
* Fixed an issue with festival time scaling.

## 1.9.4
Released 21 October 2016 for SMAPI 0.40 or later.

* Internal refactoring.
* Added hotkey to reload settings from `config.json`.

## 1.9.3
Released 20 October 2016 for SMAPI 0.40 or later.

* Added smoother time flow changes.
* Added validation to prevent invalid time flow changes.

## 1.9.2
Released 19 October 2016 for SMAPI 0.40 or later.

* Added option to disable time changes on festival days.
* Added support for holding `Alt` to change time flow by 0.1-second intervals.

## 1.9.1
Released 18 October 2016 for SMAPI 0.40 or later.

* Added notifications when time is frozen or unfrozen.

## 1.9
_(Lost to the sands of time.)_

## 1.8
Released 23 March 2016 for SMAPI 0.39 or later.

* Updated for SMAPI 0.39.

## 1.7
Released 22 March 2016 for SMAPI 0.38 or later.

* Updated for SMAPI 0.38.
* Migrated ini file to `config.json`.
* Added hotkeys to freeze/unfreeze/speed up/slow down time, and reset settings.
* Added support for holding `Shift` to change time flow by 10-second intervals.

## 1.6
Released 17 March 2016 for SMAPI 0.37.3 or later.

* Added options to freeze time automatically based on location.
* Added smoother time slowing.
* Fixed overflow bug for long tick lengths.

## 1.5
_(Storm API release.)_

## 1.4.2
Released 06 March 2016 for SMAPI 0.37.1 or later.

* Fixed fast time bug if user updated from 1.2 without updating config.

## 1.4.1
Released 05 March 2016 for SMAPI 0.37 or later.

* Fixed config being read from wrong path in some cases.

## 1.4
_(Test release.)_

## 1.3
Released 05 March 2016 for SMAPI 0.37 or later.

* Updated for SMAPI 0.37.
* Fixed config being autocreated with the wrong field name.

## 1.2
Released 03 March 2016 for SMAPI 0.35 or later.

* Renamed config setting for clarity.

## 1.1
Released 03 March 2016 for SMAPI 0.35 or later.

* Added support for speeding up and slowing down time.

## 1.0
Released 03 March 2016 for SMAPI 0.35 or later.

* Initial release.
