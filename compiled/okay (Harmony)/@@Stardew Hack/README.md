# Stardew Hack

## Description
This is a library/core mod, containing bits of code that all my other mods use. It has some functions to make small bytecode changes to Stardew Valley in a way that is somewhat update proof, but also makes sure it reliably breaks in cases where it isn't. My other mods contain descriptions of byte code changes and passes those to StardewHack to apply them. Without any of my other mods installed, StardewHack won't do much. 

While I could just package a copy of this core mod with all my other mods, that allows people to have mixed versions, which cause really weird bugs. Having StardewHack as a separate mod ensures there's only one copy and thus no version issues.

## Used by
This library is used by the following mods:

* [Always Scroll Map](https://www.nexusmods.com/stardewvalley/mods/2733):                   Makes the map scroll past the edge of the map.
* [Bigger Backpack](https://www.nexusmods.com/stardewvalley/mods/1845):                     Adds an additional upgrade to the backpack for 48 slots.
* [Fix Animal Tool Animations](https://www.nexusmods.com/stardewvalley/mods/3215):          When using the shears or milk pail, the animation no longer plays when no animal is nearby.
* [Flexible Arms](https://www.nexusmods.com/stardewvalley/mods/20902):                      **NEW!** Makes it easier to aim your tools with a mouse.
* [Grass Growth](https://www.nexusmods.com/stardewvalley/mods/2732):                        Allows long grass to spread everywhere on your farm.
* [Yet Another Harvest With Scythe Mod](https://www.nexusmods.com/stardewvalley/mods/2731): Allows you to harvest all crops and forage using the scythe. They can also still be plucked.
* [Movement Speed](https://www.nexusmods.com/stardewvalley/mods/2736):                      Changes the player's movement speed and charging time of the hoe and watering can.
* [Tilled Soil Decay](https://www.nexusmods.com/stardewvalley/mods/2738):                   Delays decay of watered tilled soil.
* [Tree Spread](https://www.nexusmods.com/stardewvalley/mods/3183):                         Prevents trees from spreading on your farm.
* [Wear More Rings](https://www.nexusmods.com/stardewvalley/mods/3214):                     Adds 4 additional ring slots to your inventory.

## FAQ
**Do the mods also work on Android?** With the exception of Bigger Backpack and Wear More Rings, the mods also work on Android. 

**Does mod *x* work on the latest version of Stardew Valley?** Please check the SMAPI [mod compatibility](https://smapi.io/mods) page for up-to-date information. The mods that broke in 1.5 have been updated. If there's no download for 1.5, then the download for 1.4 will also work on 1.5.

## Known bugs
Please report bugs on [GitHub](https://github.com/bcmpinc/StardewHack/issues).

## Changes
#### 7.3:
* French and Polish translations.

#### 7.0:
* Updated for Stardew Valley 1.6
* Localization support.
* Better error checking, reporting & handling.

#### 6.0:
* Update [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) bindings.

#### 5.2:
* Fixed issue with `System.AppDomain.DefineDynamicAssembly` for compatibility with SDV 1.5.5 beta

#### 5.1:
* Updated to use Harmony 2.1 and support SMAPI 3.12
* Fixes the `System.InvalidOperationException: Late bound operations cannot be performed on types or methods for which ContainsGenericParameters is true.` issue.

#### 5.0:
* StardewHack 5.0 is cursed and does not exist.

#### 4.0:
* Added integration for [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098).
* Add 64-bit support

#### 3.0:
* Changed how StardewHack finds the methods it wants to patch.
* Fix incompatibility with SkillPrestige.CookingSkill causing `Failed to find method` errors.
