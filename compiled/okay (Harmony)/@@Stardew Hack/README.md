# Stardew Hack

## Description
This is a library/core mod, containing bits of code that all my other mods use. It has some functions to make small bytecode changes to Stardew Valley in a way that is somewhat update proof, but also makes sure it reliably breaks in cases where it isn't. My other mods contain descriptions of byte code changes and passes those to StardewHack to apply them. Without any of my other mods installed, StardewHack won't do much. 

While I could just package a copy of this core mod with all my other mods, that allows people to have mixed versions, which cause really weird bugs. Having StardewHack as a separate mod ensures there's only one copy and thus no version issues.

## Used by
This library is used by the following mods:

* [Always Scroll Map](https://www.nexusmods.com/stardewvalley/mods/2733):                   Makes the map scroll past the edge of the map.
* [Bigger Backpack](https://www.nexusmods.com/stardewvalley/mods/1845):                     Adds an additional upgrade to the backpack for 48 slots.
* [Craft Counter](https://www.nexusmods.com/stardewvalley/mods/2739):                       Adds a counter to the description of crafting recipes telling how often it has been crafted.
* [Fix Animal Tool Animations](https://www.nexusmods.com/stardewvalley/mods/3215):          When using the shears or milk pail, the animation no longer plays when no animal is nearby.
* [Grass Growth](https://www.nexusmods.com/stardewvalley/mods/2732):                        Allows long grass to spread everywhere on your farm.
* [Yet Another Harvest With Scythe Mod](https://www.nexusmods.com/stardewvalley/mods/2731): Allows you to harvest all crops and forage using the scythe. They can also still be plucked.
* [Movement Speed](https://www.nexusmods.com/stardewvalley/mods/2736):                      Changes the player's movement speed and charging time of the hoe and watering can.
* [Tilled Soil Decay](https://www.nexusmods.com/stardewvalley/mods/2738):                   Delays decay of watered tilled soil.
* [Tree Spread](https://www.nexusmods.com/stardewvalley/mods/3183):                         Prevents trees from spreading on your farm.
* [Wear More Rings](https://www.nexusmods.com/stardewvalley/mods/3214):                     Adds 4 additional ring slots to your inventory.

## Installation notes
If this mod fails to load with something like the error below, make sure that you have all of the mods listed above updated to at least version 1.0.

    DLL couldn't be loaded: Could not load 'D:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\StardewHack\StardewHack.dll' because it was already loaded

## Known bugs
Please report bugs on [GitHub](https://github.com/bcmpinc/StardewHack/issues).
* StardewHack has been built on Linux and therefore needs rewriting to work on Windows. SMAPI does this automatically, however if during startup Stardew Valley hangs on `Loading StardewHack.dll (rewritten in memory)...`, this is probably due to your virusscanner preventing SMAPI from doing so.

## Changes
#### 1.1:
* User friendly error messages in the log file.
* In game error message for when patches fail to apply cleanly.

#### 2.0:
* Updated for Stardew Valley 1.4
* Improved IL searching capabilities.
* Fix error message upon start for android.
* Also warn when patch fails due to method not being found or being ambiguous.

#### 3.0:
* Changed how StardewHack finds the methods it wants to patch.
* Fix incompatibility with SkillPrestige.CookingSkill causing `Failed to find method` errors.
