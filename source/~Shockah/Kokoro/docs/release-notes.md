**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Shockah/Stardew-Valley-Mods**

----

[← back to readme](README.md)

# Release notes

## 3.0.0
Released 19 March 2024.

* Updated to Stardew Valley 1.6 and SMAPI 4.0.0.

## 2.2.0
Released 2 July 2023.

* Added a system to keep track of and display additional save file information, which includes the SMAPI version and the list of mods and their versions. Clicking the button copies the mod list to the clipboard.
* Implemented displaying HUD messages in the main menu.
* Fixed the [Farm Type Manager](https://www.nexusmods.com/stardewvalley/mods/3231) patch, which broke when trying to make it work both for debug and release configurations.

## 2.1.2
Released 19 June 2023.

* Fixed the [Farm Type Manager](https://www.nexusmods.com/stardewvalley/mods/3231) patch to also work when it is built in the debug configuration (which happens to be the case for the latest published version as of writing this changelog line).

## 2.1.1
Released 19 June 2023.

* Updated the [Shrike](https://github.com/Nanoray-pl/Shrike) library to 2.1.0.
* Patching [Farm Type Manager](https://www.nexusmods.com/stardewvalley/mods/3231) to fix an issue with mods changing the day start time or skipping 10 minute intervals.

## 2.1.0
Released 9 June 2023.

* Added `FarmerExt.GetDatingState()`.
* Added `FarmerExt.ConsumeItem()`.
* Improved `ItemExt.IsSameItem()`.
* Improved Harmony errors.
* Fixed an error when [SpaceCore](https://www.nexusmods.com/stardewvalley/mods/1348) was not installed.

## 2.0.1
Released 30 May 2023.

* Fixed a bug where mods referencing the Shrike library failed to load, by force-referencing Shrike assemblies in Kokoro's code.

## 2.0.0
Released 29 May 2023.

* Now includes the [Shrike](https://github.com/Nanoray-pl/Shrike) sequence matching library as a dependency.
* New system for handling multiplayer mod messages, replacing old utility methods.
* New system for tracking machines.
* Improvements to the Skill utilities.
* Improvements to Harmony utilities.
* Fixed reflection utilities.

## 1.0.0
Released 28 February 2023.

* Initial release.