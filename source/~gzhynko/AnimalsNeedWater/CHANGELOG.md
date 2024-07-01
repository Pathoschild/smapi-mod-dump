**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/gzhynko/StardewMods**

----

# Version history for AnimalsNeedWater

## 1.5.4
Released June 16, 2024.
- Extend functionality to make mod work on Animal House buildings outside the Farm (thanks to @Smoked-Fish).

## 1.5.3
Released March 25, 2024.
- Changed water pathfinding logic. Water pathfinding should work properly now.
- Lowered default friendship loss when animal was left thirsty (20 -> 10 points).
- Animals now drink from all water sources (wells, rivers, fish ponds, etc.) by default.

## 1.5.2
Released March 24, 2024.
- Fixed not being able to water troughs using upgraded watering cans.

## 1.5.1
Released March 23, 2024.
- Fixed not being able to water crops and pet bowl.

## 1.5.0
Released March 23, 2024.
- Updated for SMAPI 4.0 and Stardew Valley 1.6
- Migrated to .NET 6
- Replaced xnb tilesheets with PNGs.

## 1.4.0
Released December 30, 2021.
- Migrated to net5.0. SMAPI 3.13 or newer is now required.
- Reworked the profiles system, it now loads the trough placement profiles from JSON files.
- Added a convenience API method 'DoesAnimalHaveAccessToWater'. See the API Documentation on GitHub for more info.

## 1.3.4
Released August 13, 2021.
- Updated to support Harmony 2.1. SMAPI 3.12 or newer is now required.

## 1.3.3
Released June 12, 2021.
- Added support for Generic Mod Config Menu by spacechase0.
- Added a spanish translation by bpsys.

## 1.3.2
Released June 1, 2021.
- Fixed animals not drinking water outside.

## 1.3.1
Released March 11, 2021.
- Fixed troughs being filled the day before a festival.

## 1.3.0
Released February 9, 2021.
- Added full support for multiplayer.
- Added a brazilian portuguese translation by tramontina.

## 1.2.6
Released January 27, 2021.
- Fixed an issue with accessing the mod API.

## 1.2.5
Released January 26, 2021.
- Fixed several bugs.
- Added a config option for cleaner troughs. (thanks to Goldenrevolver for the trough tilesheet)
- Modified the API by changing some endpoints and their input and return values. See the API Documentation on GitHub for more info on that.
- Added error handling for harmony patches and moved them to a separate namespace.
- Replaced all animal name identifiers with animal instances.

## 1.2.4
Released January 25, 2021.
- Fixed errors in multiplayer. Data is still not synchronized between different mod instances, though.
- Fixed an issue when entering animal buildings built during current game session (yup, again).

## 1.2.3
Released January 17, 2021.
- Fixed an issue when entering buildings built during the current game session.

## 1.2.0
Released June 2, 2020.
- Animals can now drink water outside.
- Coop texture replacement is now optional.
- Extended API.
- Added German translation by Makytar.

## 1.1.0
Released June 1, 2020.
- Added trough placement profiles for 5 mods that modify Barn's/Coop's interiors. They are listed in the description.
- Optimized code a bit.

## 1.0.1
Released May 31, 2020.
- Fixed error when entering non-animal buildings like Shed or Slime Hutch.
- Extended config to make the amount of friendship points the player gets/loses configurable.

## 1.0.0
Released May 30, 2020.
- Initial release.
