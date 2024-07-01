**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/daleao/sdv**

----

# CORE Changelog

## 1.3.1

### Fixed

* Fixed Null-Reference exception when taking damage from a source that is not a monster.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.3.0

### Added

* Added Blind status.
* Added API.
* Added README.md

### Fixed

* Fixed Fear status never actually counting down.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.2.0

### Added

* Added functionality for new [Enchantments](../Enchantments) mod.

### Changed

* OutOfCombat counter now runs continuously (no longer spams Enabled/Disabled) in the console.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.1.2

### Changed

* Removed some drops from Tiger Slime Ball loot table.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.1.1

### Added

* Added `SObject.IsForage` extension.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.1.0

### Removed

* Removed Ring recipe patchers. Apparently that was added in 1.6. Also removed the Glow/Magnet Ring recipe changes.

<sup><sup>[🔼 Back to top](#core-changelog)</sup></sup>

## 1.0.0 - Initial 1.6 release

### Added

* Replaced the vanilla freeze implementation with MARGO's *significantly* better one.
* Some core features have been added to this mod which would have been a part of the old Tweaks module. Most of them (save for one) should be uninanimously non-controversial:
    - **Improved Hoppers:** Added the ability for Hoppers to pull items back out from machines, allowing them to fully automate a single machine at a time for a more balanced version of Automate.
    - **Improved Glowstone Recipes:** Allows crafting recipes to consume other rings, and in turn modifies the recipes for Glow Ring and Magnet Ring to require their smaller counterparts, and for Glowstone Ring to require the former two.
    - **Colored Slime Balls:** Causes Slime Balls to take on the color of Slimes raised in the Slime Hutch, and adds regular color-based Slime drops to Slime Ball loot tables.

    There features are core to the MARGO experience, and so I am placing them here. The one controversial **and entirely optional, disabled by default** feature is the ability to let crops die if unwatered.


[🔼 Back to top](#core-changelog)