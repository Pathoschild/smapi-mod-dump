**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://gitlab.com/kdau/eastscarpe**

----

# ![[icon]](promo/icon.png) East Scarpe SMAPI component

This is the SMAPI (C#) component of LemurKat's East Scarpe mod. This component is written by kdau.

## Features

### Ambient sounds

For each `Sound` listed in the `AmbientSounds` data field, when the given `Conditions` apply and a player is in the given `Area` of the given `Location`, at each tick there should be the given `Chance` for the sound to play. At most one sound shoiuld play for a given player at any time.

### Crab Pot catches

For each entry in the `CrabPotCatches` data field, when the given `Conditions` apply, a Crab Pot on a tile in the given `Area` and `FishingArea` of the given `Location` should have the given `ExtraTrashChance` and should have ocean catches only if `OceanCatches` is true.

### Critter spawns

For each entry in the `CritterSpawns` data field:

* When the given `Conditions` apply and a player enters the given `Location`, there should be the given `ChanceOnEntry` for spawning to occur.
* When the given `Conditions` apply and a player is in the given `Location`, at each tick there should be the given `ChanceOnTick` for spawning to occur.
* When spawning occurs, a number of clusters between the given `MinClusters` and `MaxClusters` inclusive should spawn. Each cluster should be centered around a tile in the given `Area` and include a number of critters between the given `MinPerCluster` and `MaxPerCluster`.
* Each critter in a cluster should be of the given `Type` and spawn within a few tiles of the center on a clear tile. If a critter's tile has water, the critter should only have the given `ChanceOnWater` of spawning.

This feature only contemplates base game critters, inclusive of any reskinning done via content patches; for custom behaviors, see the Custom Critters mod.

### Fishing areas

For each entry in the `FishingAreas` data field, when the given `Conditions` apply and a player fishes on a tile in the given `Area` of the given `Location`, it should be treated as belonging to the fishing area with the given `Index`. This first matching entry should apply.

### Fruit trees

In locations listed in the `FruitTreeLocations` data field, there should be a one-time spawn of fruit trees based on the sapling indexes given as values of the `FruitTree` property on the `Back` layer. The trees should spawn as fully mature, with in-season trees already bearing one fruit. If a number is given for the `FruitLimit` property on the `Back` layer, the corresponding fruit tree will be limited to bearing that many fruit at any time.

### Sea Monster

For each entry in the `SeaMonsterSpawns` data field, when the given `Conditions` apply and a player is in the given `Location`, at each tick there should be the given `Chance` for the Sea Monster to spawn somewhere within the given `Area`. At most one Sea Monster should be present on a particular map at any time.

### Water color

For each entry in the `WaterColors` data field, when the given `Conditions` apply upon a player entering the given `Location`, the given `Color` should be applied to all water.

### Water effects

For each entry in the `WaterEffects` data field, when the given `Conditions` apply upon a player entering the given `Location`, tiles in the given `Area` should receive or not receive effects associated with water tiles based on the `Apply` value. (Although this is mainly limited to the visual shimmering effect, note that non-water tiles with effects applied may behave like water in some ways, particularly with other water-based mods.)

### Winter grass

For each entry in the `WinterGrasses` data field, grass in the given `Area` of the given `Location` should survive through the winter with a seasonally appropriate appearance.

## Release notes

### Version 2.0.0

* Require Stardew 1.5 or higher
* Redesign all features to be generic and fully configurable
* Remove dependency on TMXL for loading of main East Scarpe map

### Version 1.4.0

* Add Crab Pot fishing area support
* Add Orchard fruit tree spawns

### Version 1.1.3

* Avoid additional errors when map fails to load

### Version 1.1.0

* All constants are now in a `data.json` file for easier editing
* Any grass on the map is now preserved through the winter with appropraite sprites

### Version 1.0.0

* Initial release
