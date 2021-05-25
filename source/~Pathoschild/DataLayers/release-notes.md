**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Pathoschild/StardewMods**

----

# Release notes
## Upcoming release
* Improved translations. Thanks to martin66789 (updated Hungarian)!

## 1.14.3
Released 27 March 2021.

* Fixed compatibility with [unofficial 64-bit mode](https://stardewvalleywiki.com/Modding:Migrate_to_64-bit_on_Windows).

## 1.14.2
Released 06 February 2021.

* Fixed the top-left legend not scaling with the game's UI scale option.

## 1.14.1
Released 23 January 2021.

* Updated for multi-key bindings in SMAPI 3.9.

## 1.14
Released 16 January 2021.

* Reopening the menu now shows your last selected layer.
* Improved translations. Thanks to LeecanIt (updated Italian)!

## 1.13.3
Released 10 January 2021.

* Improved _water for paddy crops_ layer. (It's now updated for the latest game code, and no longer highlights untillable tiles.)

## 1.13.2
Released 04 January 2021.

* Fixed sprinkler layer not showing range for older sprinkler mods in Stardew Valley 1.5.
* Improved translations. Thanks to elCrimar (updated Spanish) and norges (updated German)!

## 1.13.1
Released 21 December 2020.

* Updated for Stardew Valley 1.5, including...
  * split-screen mode and UI scaling;
  * new fertilizer types;
  * new sprinkler upgrades.

## 1.13
Released 04 November 2020.

* Added arrow UI to navigate layers.
* Dropped support for abandoned Cobalt mod.

## 1.12.3
Released 15 October 2020.

* Refactored translation handling.
* Improved translations. Thanks to Enaium (updated Chinese) and zhxxn (updated Korean)!

## 1.12.2
Released 28 August 2020.

* Dead crops are now ignored in the crop fertiliser, harvest, and watered layers.

## 1.12.1
Released 03 July 2020.

* Improved translations. Thanks to Rittsuka (updated Portuguese)!

## 1.12
Released 02 May 2020.

* Added tile grid layer (when grid isn't enabled for all layers).
* Fixed translations not updated after changing language until you restart the game.
* Improved translations. Thanks to Annosz (added Hungarian), BURAKMESE (added Turkish), D0n-A (updated Russian), and misho104 (updated Japanese)!

## 1.11
Released 08 March 2020.

* Added support for multi-key bindings (like `LeftShift + F3`).
* Improved translations. Thanks to Enaium (updated Chinese), kchapelier (updated French), and xCarloC (updated Italian)!

## 1.10
Released 01 February 2020.

* Added shortcut keys to switch to a specific layer while the overlay is open (thanks to Drachenkaetzchen!).
* Added tilled group to tillable layer (thanks to Drachenkätzchen!).
* Improved translations. Thanks to jahangmar (updated German), kchapelier (updated French), shirutan (updated Japanese), and VengelmBjorn (updated Russian)!

## 1.9.1
Released 27 December 2019.

* Improved translations. Thanks to L30Bola (updated Portuguese) and PlussRolf (updated Spanish)!

## 1.9
Released 15 December 2019.

* Added grid feature (disabled by default).
* Fixed the `export` console command not exporting the full map.
* Improved translations. Thanks to LeecanIt (added Italian)!

## 1.8
Released 26 November 2019.

* Updated for Stardew Valley 1.4, including...
  * Deluxe Scarecrow;
  * paddy crops (which have a new layer to show tiles eligible for their growth bonus);
  * new areas and warps;
  * new bee house flower range.
* Added `data-layers export` console command to export the current data layer to a JSON file.
* Updated for compatibility with the latest version of Pelican Fiber.
* Improved translations. Thanks to shiro2579 (updated Portuguese)!

## 1.7
Released 25 July 2019.

* The accessibility layer now shows mine ladders/shafts as warp tiles.
* Fixed accessibility layer showing incorrect warp tiles near some buildings.
* Fixed accessibility layer errors when viewing some areas patched by TMX Loader.
* Dropped Data Maps migration code.
* Improved translations. Thanks to Shirutan (updated Japanese) and Skirmsiher (updated French)!

## 1.6.2
Released 09 June 2019.

* Fixed config parsing errors for some players.
* Improved translations. Thanks to Firevulture (updated German) and YSRyeol (updated Korean)!

## 1.6.1
Released 07 April 2019.

* Improved translations. Thanks to binxhlin (updated Chinese)!

## 1.6
Released 06 April 2019.

* Added _buildable_ and _tillable_ layers.
* Added support for Line Sprinklers mod.
* Improved translations. Thanks to binxhlin (updated Chinese), kelvindules (updated Portuguese), and TheOzonO3 (updated Russian)!

## 1.5.1
Released 05 March 2019.

* Improved translations. Thanks to S2SKY (added Korean) and VincentRoth (added French)!

## 1.5
Released 08 December 2018.

* Added _machine processing_ layer (requires Automate 1.11+).
* Updated for the upcoming SMAPI 3.0.
* Improved translations. Thanks to Nanogamer7 (German)!

## 1.4.2
Released 08 November 2018.

* Fixed error accessing Better Sprinklers in SMAPI 2.8+.

## 1.4.1
Released 17 September 2018.

* Updated for Stardew Valley 1.3.29.
* Improved translations. Thanks to pomepome (added Japanese), Ria (Spanish), and Yllelder (Spanish)!

## 1.4
Released 01 August 2018.

* Renamed to Data Layers due to common confusion about the name Data Maps.
* Updated for Stardew Valley 1.3 (including multiplayer).
* Added _crops: ready to harvest_ layer.
* Added support for Better Junimos and Prismatic Tools.
* Improved layers:
  * _accessibility_ now shows farm building door warps;
  * crop layers now show garden pots.
* Improved performance with more nuanced update rate.
* Improved translations. Thanks to alca259 (added Spanish), fadedDexofan (added Russian), and TaelFayre (added Portuguese)!

## 1.3
Released 14 February 2018.

* Updated to SMAPI 2.4.
* Added _crops: fertilized_ and _crops: watered_ maps. (Thanks to irecinius!)
* Added support for hiding individual maps in `config.json`.
* Improved consistency and sorted by name.
* Fixed error in the Cobalt integration.
* Improved translations. Thanks to Husky110 (added German) and yuwenlan (added Chinese)!

### 1.2
Released 13 January 2018.

* Added: point at a scarecrow/sprinkler/etc in its data map to highlight that one's range.
* Added: two overlapping groups of the same color will now share one border (configurable).
* Fixed error in Junimo hut map when Pelican Fiber isn't installed.

### 1.1
Released 11 January 2018.

* Added bee house coverage map.
* Added support for Cobalt's sprinkler.
* Added support for Simple Sprinkler's custom sprinkler range.
* Updated Better Sprinklers support.
* Fixed deprecated API usage.

### 1.0
Released 26 December 2017.

* Initial version.
* Added Junimo hut coverage, scarecrow coverage, sprinkler coverage, and accessibility maps.
* Added support for Better Sprinklers' custom sprinkler range.
* Added support for Pelican Fiber's custom build menu.
