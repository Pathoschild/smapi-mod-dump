**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/gzhynko/stardew-mods**

----

# Version history for FishExclusions

## 1.3.1
Released April 5, 2024.
- Fixed potential problems with weather exclusions.

## 1.3.0
Released April 5, 2024.
- Fixed issues related to JsonAssets and removed dependency on it.
- Added a console command to reload the mod config: `fex_reload`.
- Renamed "FishToExclude" in conditional exclusions to "Exclusions".

## 1.2.0
Released March 23, 2024.
- Updated for SMAPI 4.0 and Stardew Valley 1.6
- Migrated to .NET 6

## 1.1.5
Released August 13, 2021.
- Updated to support Harmony 2.1. SMAPI 3.12 or newer is now required.

## 1.1.4
Released June 13, 2021.
- Added support for exclusions by the JsonAssets object name (for fish added through JsonAssets).
- Removed legacy config format support.

## 1.1.3
Released June 12, 2021.
- Added support for Generic Mod Config Menu by spacechase0.

## 1.1.2
Released January 24, 2021.
- Fixed an issue where the catchable fish at 100th mine level was not affected.

## 1.1.1
Released January 23, 2021.
- Added a console command to allow toggling all exclusions during runtime.

## 1.1.0
Released January 23, 2021.
- Added conditional exclusions.
- Changed config file format. The mod should automatically convert legacy configs to the new format.

## 1.0.0
Released January 23, 2021.
- Initial release.
