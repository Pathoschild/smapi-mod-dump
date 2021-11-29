**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/spacechase0/StardewValleyMods**

----

﻿[← back to readme](README.md)

# Release notes
## 1.3.2
Released 27 November 2021 for SMAPI 3.12.5 or later. Updated by Pathoschild.

* Fixed error when giving a custom item as a gift when the content pack doesn't specify response translation keys.

## 1.3.1
Released 29 October 2021 for SMAPI 3.12.5 or later. Updated by Pathoschild.

* Improved mod-provided API (thanks to Digus!):
  * Added support for creating a colored item.
  * Fixed error when requesting an unknown/invalid item ID.
* Improved example content pack:
  * Standardized file structure.
  * Added update key.
* Fixed error in some cases when a texture being drawn is null.

## 1.3.0
Released 15 October 2021 for SMAPI 3.12.5 or later. Updated by Pathoschild.

* Added full [translation](https://stardewvalleywiki.com/Modding:Translations) support.
* Added three new furniture types: `Lamp`, `Sconce`, and `Window` (thanks to echangda!).
* Updated for Generic Mod Config Menu 1.5.0.
* Fixed various animation parsing issues.
* Fixed shop ID for Krobus (thanks to echangda!).
* Fixed error when crafting in some cases.

## 1.2.1
Released 19 September 2021 for SMAPI 3.12.5 or later. Updated by Pathoschild.

* Reverted recipe changes in 1.2.0 which broke some content packs.
* Fixed breaking bigcraftables with an axe (thanks to ImJustMatt!).

## 1.2.0
Released 19 September 2021 for SMAPI 3.12.5 or later. Updated by Pathoschild and spacechase0.

* Added animation range syntax (thanks to ImJustMatt!).
* Added support for custom fruit trees with a chance to produce nothing on a given day.
* Fixed edge cases for custom items with " Recipe" in the name.
* Fixed error parsing rectangle fields in some cases.

## 1.1.0
Released 11 September 2021 for SMAPI 3.12.5 or later. Updated by Pathoschild and spacechase0.

* Backported to Stardew Valley 1.5.4.
* Added mod-provided API method to spawn DGA items by their ID.
* Fixed bigcraftable textures being cut off in some places.
* Fixed some shops not identified correctly.
* Fixed game unable to remove vanilla items from the player inventory in some cases.
* Fixed DGA items not placeable in some cases.
* Internal refactoring.

## 1.0.0
Released 06 September 2021 for SMAPI 3.13.0-beta or later. Updated by spacechase0.

* Initial release.
