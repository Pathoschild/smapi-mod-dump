**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://gitlab.com/daleao/sdv-mods**

----

# Arsenal Change Log

## 1.0.1

### Changed

* Minimal edits to Blade of Dawn description.

### Fixed

* Galaxy and Infinity weapons, Blade of Ruin and Blade of Dawn are now marked as Special and cannot be trashed.

## 1.0.0

### Added

* Added flavor text for each virtue at the Yoba altar.

### Changed

* Removed Gil requirement for advancing the BOTRK quest-line. Now, simply exhausting the Yoba altar's dialogue options will advance the quest.
* Improved the textures for Blade of Ruin and Blade of Dawn.

### Fixed

* Garnet Enchantment can now be applied to weapons correctly.
* Garnet Enchantment's effect will now apply correctly.
* Fixed an issue where holding anything besides a weapon or slingshot would cause the player to take 0 damage.
* Fixed a possible Null-Reference Exception when receiving a Galaxy weapon.
* Infinity weapons now have 4 forge slots as advertised under the rebalance.
* Fixed a bug with Ruby Enchantment applying only a single point of damage.
* Weapon and Slingshot Forges now unapply correctly.
* Hoe is not longer classified as Rare (color-coded Blue).

## 0.9.9

### Added

* Ruined Blade quest-line is now fully implemented.
* Added option to draw bulls-eye instead of mouse cursor while firing a slingshot.

### Changed

* Few translations changed.
* Reduced the aura on the Blade of Ruin texture.

### Fixed

* Small fix for Slick Moves to prevent the player from sliding during combo cooldown.
* Scythes can no longer perform the stabby special move.
* The item hold up animation now plays correctly when obtaining the Blade of Ruin.
* The Blade of Ruin is now a Stabbing Sword, as it should.
* No longer triggers stabbing special move when obtaining the Blade of Dawn.
* Fixed possible bug with Bloodthirsty enchantment.
* Fixed some incorrect translation keys.

## 0.9.8

### Added

* The natural damage and knockback modifiers of advanced slingshots now display in the tooltip.

### Changed

* Improvements to Slick Moves. Can now drift in the parallel direction (i.e., when swinging directly in front or behind you). Before, drifting only worked for perpendicular directions (i.e., when swinging to the sides). Also increased the drift velocity slightly. Finally, the drift now comes to a halt at the end of the animation, removing the unimmersive impression of buttery floor.
* Clint's follow-up quest now begins correctly.

## 0.9.7

### Changed

* SVE's Treasure Cave now rewards an Obsidian Edge instead of Galaxy Slingshot.
* Clint's forging quest is no longer random, and now takes less time the higher your friendship with him.
* Changed the knockback of some weapons:
    * Wooden Blade: +0.25
    * Rapier: -0.05
    * Steel Falchion: -0.05
    * Katana: -0.2
* The Dark Sword's curse has been changed. No longer affects stamina consumption. Now, as the curse strengthens the sword will gain a progressively higher chance to auto-equip itself, preventing you from using other weapons.
* Community Center is no longer a requirement for obtaining the Dark Sword.

### Fixed

* Fixed an issue which caused critical hits to deal absurdly low damage.
* Fixed an error that could be thrown when playing the hold-up-item animation.
* Clint's forging quest was still bugged. Now tested.
* Fixed a Null-Reference Exception when attempting to grab the Dwarvish Blueprint from a Scavenger Hunt chest.
* Fixed a Null-Reference Exception in pirate treasure menu when a Neptune's Glaive is produced.
* Fixed missing patch targets for allowing crits to ignore monster defense.
* Added config checks to logic that was missing it.

## 0.9.6

### Fixed

* The event which counts the days left until Clint finishes translating the blueprints should now trigger correctly.
* Fixed a compatibility issue with any modded recipes containing Dragon Tooth.
* Fixed a null-reference exception when farmer takes damage from bombs and maybe other sources too.

## 0.9.5

### Added

* Added Initialize command to apply all necessary configurations to weapons and farmers on existing saves.

### Fixed

* Club second combo hit now actually does damage.

### Removed

* Removed automatic initialization script from SaveLoaded event. This was inefficient and unreliable. Replaced with manual console command.

## 0.9.4

### Added

* If SVE is installed and Infinity +1 feature is enabled, Tempered Galaxy weapons will now be automatically removed from Alesia and Isaac's shops, so users no longer have to manually remove them from SVE's shop.json file. 

### Changed

* Slingshot special move cooldown is now a property of Farmer and not Slingshot, as it should be.
* Default values for difficulty sliders are now set to 1.
* The base damage of some starter weapons has been slightly increased.

### Fixed

* Fixed Slick Moves applying to Scythe.
* Fixed Brute prestige perk not applying with Arsenal module enabled.
* Fixed Galaxy Slingshot being instantiated as Melee Weapon.
* Fixed special item hold up message for first Galaxy weapon not being displayed.
* Fixed Mine chests changes not being applied as intended with the Weapon Rebalance setting.
* Players on existing save files that have already obtained the Galaxy Sword should now be able to obtain the remaining Galaxy weapons.
* Fixed a bug in the monster stat randomization logic, which was generating monsters with current HP higher than max HP.

## 0.9.3

### Fixed

* Fixed null-reference exception when opening Marlon's shop (forgot to pass `__result` by `ref`).
* Control settings now apply only to weapons, as they should.

## 0.9.2

### Changed

* Weapon tooltips now revert to vanilla when `RebalancedStats` option is disabled.

### Fixed

* Added even more robust null-checking for custom JA items to avoid issues.
* Fixed SlickMoves config setting in GMCM which was incorrectly mapped to FaceMouseCursor.

### Fixed

* The category of Dwarven Blueprint has been changed from Artifact to Junk. This avoids the error caused by the game attempting to spawn a blueprint when the player digs an artifact spot.
* Fixed player's facing direction changing during active menu.

## 0.9.0 (Initial release)

* Initial Version