**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/daleao/modular-overhaul**

----

# TWX Changelog

## 4.2.3

### Fixed

* Fixed freeze when collecting trash from Crab Pot with full inventory.
* `TrashDoesNotConsumeBait` now applies to fishing rods as well.

## 4.2.2

### Fixed

* Fixed ring materials not being consumed when crafting rings in the Glowstone progression.

<sup><sup>[🔼 Back to top](#twx-changelog)</sup></sup>

## 4.1.3

### Removed

* Removed the Spawn Crows feature. Apparently scarecrows didn't work in Ginger Island, even though I'm pretty sure I tested it and it was working. But everyone hated that feature anyway so it wasn't worth investigating.

<sup><sup>[🔼 Back to top](#twx-changelog)</sup></sup>

## 4.0.0

### Changed

* Renamed `DairyYieldsQuantityOverQuality` setting to `ImmersiveDairyYield` to avoid confusion over how this setting affects Ostrich and Gold eggs.
    * With this setting enabled, Gold Eggs now max out the quality, instead of setting to gold.
    * Changed the following translation keys:
        * `gmcm.twx.farming.largedairy_yield.title` -> `gmcm.twx.farming.dairy_yield.title`
        * `gmcm.twx.farming.largedairy_yield.desc` -> `gmcm.twx.farming.dairy_yield.desc`

<sup><sup>[🔼 Back to top](#twx-changelog)</sup></sup>

## 3.1.9

### Fixed

* Fixed not being able to combine any rings with Glowstone Progression enabled.

<sup><sup>[🔼 Back to top](#twx-changelog)</sup></sup>

## 3.1.7

### Added

* Added ability to follow the Glowstone Ring progression at the Forge (thereby forgoing the essence cost for cinder shards). This might be completely redundant, but I just thought it was weird that you couldn't just combine a Glow and Magnet rings to make Glowstone.

<sup><sup>[🔼 Back to top](#twx-changelog)</sup></sup>

## 3.1.2

### Fixed

* Fixed Glowstone progression integration with Better Crafting.

<sup><sup>[🔼 Back to top](#twx-changelog)</sup></sup>

## 3.0.0

## Added

* Glowstone ring progression moved to this module.

## Changed

* Re-organized config settings by skill.

<sup><sup>[🔼 Back to top](#twx-changelog)</sup></sup>

## 2.5.5

### Fixed

* Fixed Tea Bush Age quality incorrectly using TreeAgingFactor instead of TeaBushAgingFactor. 

<sup><sup>[🔼 Back to top](#twx-changelog)</sup></sup>

## 2.5.2

### Fixed

* Fixed crops withering even when watered.

<sup><sup>[🔼 Back to top](#twx-changelog)</sup></sup>

## 2.5.0

### Added

* Added CropWitherChance option (default 10%). If left unwatered for more than a day, crops will have an increasingly larger chance to wither and die spontaneously.
* Added TrashDoesNotConsumeBait option.

### Removed

* Removed LegendaryFishAlwaysIridiumQuality option, because it's not needed.
* Removed the MillsPreserveQuality option, because I don't like it.

<sup><sup>[🔼 Back to top](#twx-changelog)</sup></sup>

## 2.4.0

### Fixed

* Mushroom Box age now uses Foraging level instead of Farming.

<sup><sup>[🔼 Back to top](#twx-changelog)</sup></sup>

## 2.3.0

### Fixed

* Custom farm map options in GMCM now only appear if the corresponding mod is installed.

<sup><sup>[🔼 Back to top](#twx-changelog)</sup></sup>

## 2.0.5

### Added

* Added crow spawn settings to GMCM.

### Fixed

* Fruit trees now grow as normal during winter in Ginger Island.

<sup><sup>[🔼 Back to top](#twx-changelog)</sup></sup>

## 2.0.0

### Added

* Added new tweak to allow crows to spawn on custom maps.

<sup><sup>[🔼 Back to top](#twx-changelog)</sup></sup>

## 1.0.2

### Added

* Added option to age Tea Bushes, just like Bee Hives and Mushroom Boxes. Tea Bushes will produce higher quality Tea Leaves as they mature.
* You can now set the amount of exp rewarded for harvesting Berry Bushes, Mushroom Boxes and Tappers.

### Fixed

* Fixed a bug where Mushrom Box quality did not apply when automated.

<sup><sup>[🔼 Back to top](#twx-changelog)</sup></sup>

## 0.9.3

### Fixed

* Fixed Mill output quality compatibility with More Fertilizers.

<sup><sup>[🔼 Back to top](#twx-changelog)</sup></sup>

## 0.9.0 (Initial release)

### Added

* Added option to make legendary fish always iridium-quality.

### Removed

* Removed the Flower Meads feature. Moving forward please use the aptly-named [Flower Meads](https://www.nexusmods.com/stardewvalley/mods/5767) instead.
* Removed the ProfessionalForagingInGingerIsland feature. It has been moved to the [Professions](../Professions) module where it is now a native feature of Ecologists.

[🔼 Back to top](#twx-changelog)