**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/bcmpinc/StardewHack**

----

# StardewHack
A bunch of Stardew Valley mods that heavily rely on IL code modification. For this purpose it uses [Harmony](https://github.com/pardeike/Harmony/wiki). 

Android is partially supported. Only the mods Wear More Rings and Bigger Backpack are not supported on android, due to android having an entirely different inventory screen.

## Overview
* [Always Scroll Map](/AlwaysScrollMap):                     Makes the map scroll past the edge of the map.
* [Craft Counter](/CraftCounter):                            Adds a counter to the description of crafting recipes telling how often it has been crafted.
* [Fix Animal Tool Animations](/FixAnimalTools):             When using the shears or milk pail, the animation no longer plays when no animal is nearby.
* [Grass Growth](/GrassGrowth):                              Allows long grass to spread everywhere on your farm.
* [Yet Another Harvest With Scythe Mod](/HarvestWithScythe): Allows you to harvest all crops and forage using the scythe. They can also still be plucked.
* [Movement Speed](/MovementSpeed):                          Changes the player's movement speed and charging time of the hoe and watering can.
* [Tilled Soil Decay](/TilledSoilDecay):                     Delays decay of watered tilled soil.
* [Tree Spread](/TreeSpread):                                Prevents trees from spreading on your farm.
* [Wear More Rings](/WearMoreRings):                         Adds 4 additional ring slots to your inventory.

## Compiling

First make sure that you also have bigger backpack checked out. Run: 
```
git submodule init
git submodule update
```

Then open `StardewHack2.sln` in monodevelop (or whatever development tool you are using) and hope it compiles. :)
