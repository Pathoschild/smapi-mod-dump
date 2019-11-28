# Aggressive Acorns

__Aggressive Acorns__ allow high customisation of tree behavior.
The main features are that spreading trees can replace long grass, and that immature trees cannot be destroyed by melee weapons.

Also fixes some of vanilla's oversights such as stumps shading adjacent saplings, stumps spreading seeds, and a graphical error that affects mushroom stumps that were cut then regrew within the same game session.

[TOC]: #

# Table of Contents
- [Features](#features)
- [Installation](#installation)
- [Configuration](#configuration)
- [Future plans](#future)

## Features
Features are classified as either:
* :icecream: adding configuration to an existing vanilla value (like 'chance' or 'stage')
* :seedling: configurable new feature
* :lock: unconfigurable new feature or change (mainly smaller or fixes)

**All configurable features default to vanilla settings**. Set probability-based features to zero to disable them.

### General
* :seedling: **Immature trees will no longer be destroyed by the scythe** or melee weapons. This is great for when you are growing hay in your timber plantation.
* :lock: **Winter has no effect on trees in the desert or indoors.** This is changed from vanilla, where winter had no effect in the greenhouse or on palm trees. This means that palm trees can be affected by winter outside the desert, that all trees will escape the winter in the desert, and that mushroom trees will never hibernate indoors or in the desert.
* :icecream: In vanilla, the highest growth stage that can be walked over is seeds. While this can be changed, it has graphical errors in-game. I am working on a fix for these.

### Held (Shakeable) Seeds
*  :icecream: As per vanilla, trees can hold a seed each day, but the chance is now configurable.
* :seedling: The seed is kept from one day to the next if unused.

### Spread via Seeds
* :icecream: As in vanilla, each day, every tree on the farm has a certain chance to *try to* place a seed around it. (If the seed can't be placed on the first attempt there is no second chance). The chance is now configurable.
* :lock: Spreading now consumes, *but does not require*, a held seed.
* :lock: **Stumps (and therefore hibernating mushroom trees) no longer spread.**
* :seedling: **Seeds will replace long grass** (the kind that gives hay) (no effect when planting manually).
* :seedling: Spread can be disabled during winter.
* :seedling: **Spread can be disabled for tapped trees.** Useful in keeping your sap plantations tidy.

### Growth
* :icecream: Each day immature trees have a chance to grow one stage. The daily growth chance is configurable.
* :icecream: In vanilla, an immature tree will not grow fully if it is adjacent to a fully grown tree (is shaded). The maximum stage of a shaded tree is now configurable.
* :lock: Stumps no longer shade adjacent trees.
* :seedling: Tree growth during winter can be enabled. See above for the changed rules of where winter applies at all.
* :seedling: Instant growth can be enabled. Immature trees of any stage will mature overnight if they are able to grow (not hibernating, not shaded, not effected by winter).

### Mushroom Trees

* :seedling: The vanilla hibernation of mushroom trees can be disabled. This also disables the regeneration of mushroom stumps on Spring 1st.
* :seedling: **Mushroom stumps can have a chance to regrow each day**. When enabled, it occurs at half the normal growth chance, or always if instant growth is enabled. It will not occur when normal growth would not occur (ie. hibernation).
* While hibernating, growth and spread do not occur. Even if hibernation is disabled, mushroom trees still respect the normal winter growth rules (ie. won't grow/spread in winter unless they are enabled).
* Mushroom stumps will always respect the setting for max shaded growth, whether regrowing from hibernation on Spring 1st or using the daily regrowth feature. This means that if a normal tree grows next to a hibernating stump, it would block the regrowth on Spring 1st.
* In vanilla, there is an error if a mushroom tree is chopped down, then regrows on Spring 1st, without exiting/reopening the save. The (non-serialized) rotation value is not reset after the falling animation, so the top of the tree reappears fallen over. This has been fixed.

## Installation

1. Install [SMAPI](https://smapi.io/) (>3.0.0)
2. Get this mod from Nexus Mods.
3. (Manual installation) Extract the contents of the zip file to `Stardew Valley\Mods`. 
4. Start the game once to create the configuration file. Quit, edit the file (`Stardew Valley\Mods\Aggressive Acorns\config.json`), then play. *All options default to vanilla, so make sure to enable any features you want*.


Compatible with version 1.4 of Stardew Valley. Must be installed for all players in multiplayer.
* Compatible with Stardew Valley Extended as of Aggressive Acorns v1.2.0.

For the latest compatibility information visit https://smapi.io/compat


## Configuration

| **Name** | **Type** | Default | **Description** |
| -------- | -------- | ------- | --------------- |
|`PreventScythe`| boolean (`true`, `false`) | `false` | Whether immature trees are destroyed by melee weapons. |
|`SeedsReplaceGrass`| boolean | `false` | Whether trees are able to replace long grass when they spread by dropping seeds. |
|`MaxShadedGrowthStage`| integer (`0` - `4`)| `4` | The highest stage of growth for trees next to any mature tree. Also effect the regrowth of stumps. |
|`MaxPassibleGrowthStage`| integer (`0` - `4`) | `0` | The highest stage of growth without collision. **Visually broken**|
|`DailyGrowthChance`| float (`0.0` - `1.0`) | `0.20` | Daily chance for a tree to mature by one stage. |
|`DoGrowInWinter`| boolean | `false` | Whether trees grow normally in winter. |
|`DailySpreadChance`| float (`0.0` - `1.0`) | `0.15` | Daily chance to plant a seed nearby. |
|`DoTappedSpread`| boolean | `true` | Whether tapped trees will spread. |
|`DoSpreadInWinter`| boolean | `true` | Whether trees will spread in winter |
|`DoGrowInstantly`| boolean | `false` | Whether immature trees (of any stage) grow to full maturity overnight. |
|`DoSeedsPersist`| boolean | `false` | Whether a tree keeps its seed if not shaken the previous day. |
|`DailySeedChance`| float (`0.0` - `1.0`) | `0.15` | Daily chance for a tree to gain a seed. |
|`DoMushroomTreesHibernate`| boolean | `true` | Whether mushroom trees will hibernate over winter. |
|`DoMushroomTreesRegrow`| boolean | `false` | Whether mushroom tree stumps will regrow. Daily regrowth chance is half the daily growth chance. |

#### Growth stages
| **Index** | Description| **[Stage as per wiki](https://stardewvalleywiki.com/Trees#Maple_Tree)** |
| --- | --- | --- |
| `0` | Seed | 1 |
| `1` | Sprout | 2 |
| `2` | Sapling | 3 |
| `3`, `4` | Bush | 4 |
| &ge;`5` | Mature | 5 |
| *Must use these<br>values in config* | | |

## Future

Future plans and features I'm considering:
 * Fix graphical errors when walking over &gt; stage-0 trees.
 * Mushrooms spread by root-like systems right? Maybe revert the stumps-spread-seeds fix but only for mushroom stumps?
 * Do fruit trees shade normal trees? Should they?
 * Make companion mod for fruit trees. Include random-er fruit tree growth.
 * Look at fix growth for trees (emulating vanilla fruit tree growth) - look at SMAPI data API.
 * Look at adding sprites for trees with seeds.
