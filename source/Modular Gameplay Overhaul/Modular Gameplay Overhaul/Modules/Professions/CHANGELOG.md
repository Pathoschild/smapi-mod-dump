**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/daleao/sdv-mods**

----

# Professions Module Change Log

## 1.0.0

### Changed

* Reduced the spawn chance of extra mining nodes by Prospector.

### Fixed

* Fixed the range for triggering Demolitionist's Get Excited buff.

## 0.9.9

### Fixed

* Fixed the display of Brute and Spelunker buffs.
* Brute buff now activates in a more clever way.
* Desperado Overcharge meter no longer renders in legacy firing mode.

## 0.9.7

### Changed

* Custom skills are now loaded on 2nd SecondUpdateTicked instead of SaveLoaded.

### Fixed

* Fixed a typo causing a Null-Reference Exception when resetting a custom skill.
* Treasure hunts will no longer trigger during cutscenes.
* Fixed an incorrect patch target which should allow taunted monsters to reset their aggro when the taunter dies.

## 0.9.6

### Added

* Added OnProfessionAdded and OnProfessionRemoved events. So far they only serve to invalidate the cache of profession icons, but will likely be useful in the future.

### Changed

* Lowered the log level of Ultimate setter from Warn to Info.

### Fixed

* The level 20 prestige choices should now accurately reflect the prestige branch chosen at level 15.
* The profession choice menu should now correctly display prestige profession icons.
* The skills page profession icons should now also correctly display the prestige choices made on the night previous.
* Replaced two more incorrect FieldGetters with the correct PropertyGetterGetter in Automate integration.
* Fixed a typo in "multiplyer" FieldGetter.

## 0.9.4

### Fixed

* Fixed the order of displayed Prestige ribbons in Skills page.

## 0.9.2

### Fixed

* Fixed Scavenger and Prospector tracking arrows not being displayed.
* Improved tracking arrow position for tracked bushes on-screen.
* Replaced an incorrect FieldGetter with the correct PropertyGetterGetter in Automate integration.

## 0.9.1

### Fixed

* Fixed an issue with Luck Skill integration.

## 0.9.0 (Initial release)

### Added

* When a Gemologist reaches a new quality threshold, all currently processing Crystalariums owned by that player will likewise receive a quality upgrade to reflect that.
* Scavenger profession now has a chance, proportional to the Treasure Hunt Streak, to spawn additional forage when entering a new map.
* Prospector profession, likewise, now has a chance, proportional to the Treasure Hunt Streak, to spawn additional ore nodes when entering a new mine level.
* Added golden versions of profession icons for Prestiged professions.
* Added config setting to disable Bee House being affected by Producer profession.
* Added API for custom Skill mods to register prestiged professions.

### Removed

* Removed the SeaweedIsTrash config setting.
* Removed configs from the [IProfessions interface](../../../Shared/Integrations/ModularOverhaul/IModularOverhaul.cs).

### Changed

* **Rascal** - ~~Slingshot damage +25%. 60% chance to recover spent ammo.~~ Gain one additional ammo slot. 35% chance to recover spent ammo.
    * The damage perk is gone. I always felt like Slingshot damage was overpowered anyway. In its place comes a second ammo slot; Rascals can now equip 2 different types of ammo (or the same ammo twice). Use the Mod Key (default LeftShift) to toggle between equipped ammos.
    * The ammo recovery perk has been nerfed to account for new enchantments provided by the [Slingshots](../Arsenal) module. In exchange, the Prestige perk now doubles this value to a whopping 70%---a higher value than the original.
    * Ability to equip Slime ammo moved from Slimed Piper to Rascal. It will still cause a slow debuff, but will not heal ally Slimes unless the player *also* has the Piper profession.
* **Slimed Piper** - Summoned Slimes are now friendly (will not attack players, but will still cause damage if touched).
* **Desperado (Prestiged)** - ~~Overcharged shots become spreadshots.~~ Overcharged shots can pierce enemies.
    * The Spreadshot perk has been moved to a new enchantments provided by the [Slingshots](../Arsenal) module. The ability to pierce enemies with overcharged shots is now the new prestige perk.
* The [API](../../../Shared/Integrations/ModularOverhaul/IModularOverhaulApi.cs) has been slightly changed. Some mods may need to update the corresponding interfaces.

### Fixed

* Fixed an issue with Automated Junimo Chests being unable to decide their owner.
* Fixed a bug where Conservationist Trash Collected would not reset if no trash was collected during the season.
* Fixed a bug where Tapper perk would incorrectly apply to tapped Mushroom Trees in Winter, causing multiple progressively shortened harvests.
* Fixed a bug where the Piper Concerto super ability caused all Slimes in the Hutch to disappear.
* Crab Pots now correctly trap Seaweed instead of Green Algae in the Beach farm map.
* Fixed a bug where Special Abiltiy charge would still accumulate with Enable Specials config set to false.
* Fixed a possible NullReferenceException when shooting at Slimes.
* Fixed TrackerPointerScale and TrackerPointerBobbingRate config settings.
* Added a setter to the CustomArtisanMachines config, preventing it from being reset on game load.
* The print_fishdex console command now takes into account the value of AnglerMultiplierCap config.
* Fixed some translation errors.
