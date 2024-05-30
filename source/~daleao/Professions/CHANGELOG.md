**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/daleao/sdv**

----

# PROFESSIONS Changelog

## 0.2.10

### Added

* Added Angler + Ms. Angler mating in legendary ponds.
* Added initial compatibility for Item Extensions.

### Changed

* Removed cache invalidation when opening the Game Menu.
* Some changes to PondQueryMenuDraw.

### Fixed

* Fixed 1.6 level up message not showing in the HUD.
* Fixed issues related to PondQueryMenuDraw for legendary ponds.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.2.9

### Added

* Mastered skills now display a gold icon in the Skills Page menu. Thanks to [silicon](https://next.nexusmods.com/profile/siliconmodding/about-me?gameId=1303) and [KawaiiMuski](https://next.nexusmods.com/profile/KawaiiMuski/about-me) for the icons.
* `set fishdex` command now accepts a flag `-t` for trap fish.

### Changed

* Ecologist now only applies edibility changes to gathered forage items instead of all forage items added to inventory. But those changes are now permanent, instead of being lost when the Ecologist deposits the item.
* Angler sell price bonus now also applies to Smoked Fish.

### Fixed

* Fixed an issue causing incorrect level 20 prestige options to be offered.
* Fixed issue of items not stacking when the player has the Ecologist profession.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.2.8

### Fixed

* Hotfix for BobberBarCtorPatcher.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.2.7

### Added

* Fisher profession now also applies to the bonus bobber bar height granted by Deluxe Bait.

### Fixed

* Fixed an issue where farmhand slingshots would not update to the correct number of attachment slots. Added the console command `professions fix_slingshots` as a workaround for existing saves with this issue.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.2.6

### Changed

* Profession tooltips in the Skills Page now display and properly wrap the entire tooltip text, instead of truncating at 90 characters.

### Fixed

* Fixed a possible Out-Of-Range exception in SkillsPageCtorPatcher
* Added a failsafe that should prevent errors with Luck Skill. But note that Luck Skill is **not** officially supported, and will not be.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.2.5

### Changed

* Updated Chinese translations.

### Fixed

* Fixed issue preventing location interactions introduced in 0.2.3.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.2.4

### Fixed

* Fixed possible Null-Ref exception in MineShaftCheckStoneForItemsPatcher.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.2.3 

### Fixed

* Fixed max-sized fish not being counted correctly.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.2.2

### Added

* Added Chinese, French and Korean translations. Credits added to [README](../Professions).

### Changed

* Additional slingshot ammo slot now draws horizontally in the slingshot's tooltip instead of vertically, matching the style of the Advanced Iridium Rod and saving some vertical space.
* "Memorized" fishing rod tackle now draw correctly in the rod's tooltip.

### Fixed

* Fixed various issues with custom skills.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.2.1

### Fixed

* Fixed possible crash when selecting Prestiged Harvester profession.
* Fixed possible Out-Of-Range exception in FarmerCurrentToolSetterPatcher.
* Fixed possible Null-Ref exception in MonsterFindPlayerPatcher.

### Changed

* Changed Sewer Statue logic to be more compatible with different configurations of Skill Reset / Prestige / Limit Break.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.2.0 - Beta release for 1.6

* No changes.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.1.1

### Added

* Added Mastery Limit Select menu when mastering the Combat skill.

### Fixed

* Fixed player not gaining experience.

<sup><sup>[🔼 Back to top](#professions-changelog)</sup></sup>

## 0.1.0 - Alpha release for 1.6

### Changed

* Prestige levels no longer require having all professions in the skill. It is now a reward for Mastering each individual skill. The Statue of Prestige has been renamed to Statue of Transcendence to avoid confusion. 
* Skill reset feature is unchanged. But note that choosing to Master a skill will prevent it from being reset further, effectively locking you out of any unobtained profession. *This may change in the future, but for now you have been warned.*
* Similarly, the Limit Break is now a reward for Mastering the Combat skill.
* All prestiged profession variants, and a few base variants, have been reworked. Prestige professions are now much more impactful end-game rewards. Please review the section [Professions](README.md#professions) of the README to learn more.
* Treasure Hunts are now triggered on Time Change instead of Player Warped. The chance to start a Treasure Hunt now depends on how far you have traveled or how many rocks you have broken since the previous hunt.
* Misc. code improvements.

### Removed

* Removed custom support for Luck skill and Love of Cooking.
* Removed alost all third-party mod integrations since I don't know which of them were/will be updated. I might re-add Automate integration later, or I might not.


[🔼 Back to top](#professions-changelog)

[View the 1.5 Changelog](resources/CHANGELOG_old.md)