**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Achtuur/StardewMods**

----

# Automate Speedup Statue

Do you want to upgrade your machines? Do you feel like your machines are just a tad bit too slow? Do you have enough prismatic shards laying around?
Well then, this is the mod for you! This mod introduces the _Prismatic Statue_, this statue lets you speedup all adjacent machines in the same group.

**Requires [Automate](https://www.nexusmods.com/stardewvalley/mods/1063), [JSON Assets](https://www.nexusmods.com/stardewvalley/mods/1720), and [Mail Framework Mod](https://www.nexusmods.com/stardewvalley/mods/1536)**

## Features

* Adds in the _Prismatic Statue_.
  * Speeds up machines in the same machine group.
  * Recipe purchasable from Robin, after earning 6 friendship hearts with Robin, the Wizard and completing the community center.
  * Multiple statues stack, with diminishing returns and up to a certain maximum.

* Overlay showing the machines currently sped up.
  * Toggle overlay with `L` (can be changed in config).
  * Legend:
    * Rainbow: _Prismatic Statues_.
    * Magenta: Machines that are sped up & processing something.
    * Purple: Machines that are sped up, but not processing anything currently.
    * White: Objects in the same group as the statue, but not affected.

* Speedup and max statues are configurable.
  * Config menu contains a section that lets you see the effects of the speedup. Use this to configure the speed settings you want.

## Changelog

## 1.5.1
* New/Changed
  * Change unlock requirement to be mining level 9 due to issues with mail framework mod.

## 1.5.0
* Updated to Stardew 1.6.3

## 1.4.0
* New/Changed
  * Updated overlay to better fit my other mods

* Fixes
  * Fix id collision bug, so other items don't change texture unintentionally
  * Fix cask duration resetting to default

## 1.3.2
* Fixes
  * Overlay can no longer be toggled in menus

## 1.3.1
* Fixes
  * Fix issues related to changing statue ID:
    * Breaking statue resulting in "???" item
    * Statues not properly speeding up machines
    * Overlay not showing statues properly

### 1.3.0
* New/Changed
  * Updated sprite for Prismatic Statue, thanks to [brightonmorris1](https://forums.nexusmods.com/index.php?/user/52800236-brightonmorris1/)!

### 1.2.2
* New/Changed
  * Add support for Jumino chests
  * Overlay now appears when statue is connected to chest, without needing at least one machine
  * Updated to support AchtuurCore 1.0.7

* Fixes
    * Added error handling for patch
    * Added null check for machine group tiles

### 1.2.1
* Fixes
  * Fixed error when casting to `HashSet<T>`

### 1.2.0
* New/Changed
  * Casks are now also supported
    * Note that the minimum time will always be one night, since quality upgrade is only done once per night
  * Internal code optimisations

### 1.1.0
* New/Changed
  * Added support for [PFM](https://www.nexusmods.com/stardewvalley/mods/4970) machines, requires [PFMAutomate](https://www.nexusmods.com/stardewvalley/mods/5038).

### 1.0.0

* Initial release

