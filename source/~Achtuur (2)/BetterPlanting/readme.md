**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Achtuur/StardewMods**

----

# Better Planting

This mod lets you plant faster, by letting you plant multiple seeds at once

# Features

* Lets you plant with different shapes:
  * Three tiles in a row
  * Five tiles in a row
  * A 3x3 square (normal sprinkler)
  * A 5x5 square (iridium sprinkler)
  * A 7x7 square (iridium sprinkler with pressure nozzle)
  * Fill (every possible adjacent tile)
  

# Changelog

## 1.3.1
* Fixes
  * Fixed a bug where cursor tile was counted twice, so you place 1 less seed than you should have if cursor tile cannot be planted

## 1.3.0
Updated to support Stardew Valley 1.6

## 1.2.3
* New/Changed
  * Added Japanese translation by tikamin557

## 1.2.2
* New/Changed
  * Added option to select default fill mode

* Fixes
  * Row plant modes now prioritise seeds closer to player

## 1.2.1
* New/Changed
  * Updated to support AchtuurCore 1.1.5

## 1.2.0
* New/Changed
  * Now works on garden pots as well
  * Added option to change fill mode maximum tiles
  * Mode switching text now decays

## 1.1.0
* New/Changed
  * New option to not plant seeds diagonally.
	* Being able to plant diagonally gives a bit more freedom, but requires more precision from the player
  * Overlay now displays when you do not have enough seeds in red
  * When you don't have enough seeds, it is now attempted to pick seeds in a more logical way (more adjacent seeds, instead of random corners)
  * Now changes plant mode to disabled when changing locations.

* Fixed
  * Fixed seed plant sometimes not displaying properly

## 1.0.4
* New/Changed
  * Changed overlay showup range from 2 to 1 tiles, meaning it only shows up now when you are in range to plant seeds in vanilla.

## 1.0.3
* New/Changed
  * Overlay now takes into account number of seeds in inventory
  * Added text showing how many seeds will be planted

* Fixes
  * Overlay now does not treat dead crops as an already planted crop
  * Fix error when clicking without holding an object
  * Fix bug where more seeds were planted than were available in inventory
  * Fix performance issues when using fill mode on large fields (>=100 tiles)

## 1.0.2
* Fixes
  * Fix error showing in console as a result of left clicking in menus

## 1.0.1
* Fixes
  * Fix a bug where holding seed/fertilizer condition was not working as intended

## 1.0.0

* Initial release

