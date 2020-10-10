**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/bcmpinc/StardewHack**

----

# Tree Spread

## Description
Prevents trees from dropping their seeds and spreading on your farm. To compensate, trees will have a higher chance of having a seed. 
Furthermore trees on your farm will retain their seed during the night (only if `OnlyPreventTapped` is set to false).

## Config
*Note: run Stardew Valley once with this mod enabled to generate the `config.json` file.*

* `SeedChance`: Chance that a tree will have a seed. Normally this is 0.05 (=5%). Default = 0.15 (=15%).
* `OnlyPreventTapped`: Whether only tapped trees are prevented from dropping seeds. Default = false.

## Dependencies
This mod requires the following mods to be installed:

* [SMAPI](https://www.nexusmods.com/stardewvalley/mods/2400)
* [StardewHack](https://www.nexusmods.com/stardewvalley/mods/3213)

## Known bugs
Please report bugs on [GitHub](https://github.com/bcmpinc/StardewHack/issues).

## Changes
#### 1.0:
* Added dependency on the StardewHack library mod.
* Added option to limit this mod tapped trees only.

#### 1.1:
* Fix: the option for tapped trees wasn't preventing seeds from being dropped at all.

#### 2.0:
* Updated for Stardew Valley 1.4
