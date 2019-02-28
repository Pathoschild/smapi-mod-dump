# Speedy Paths

Current Version: 1.0.0-beta

Dependancies: SMAPI 2.10.2
Note: This is a singleplayer and client-side mod.

Download: Unrelesed

## Notice

I am currently too busy to work on/release many of these mods. I welcome anyone
to take them (especially the unreleased ones!).

## Configuration:
Most of the options in config.json have to do with specific flooring boosts and
boosts designed to occur in the world map. Specific flooring boosts should
hopefully be obvious from the file.

The condition for various world boosts:
* BathHouseBoost: inside the bath house
* DesertRoadBoost: on the road at the bus stop and desert
* DockBoost: on the dock (and that small bridge between beaches)
* WoodBridgeBoost: on the large bridge to the quarry
* DirtPathBoost: on the world's dirt paths (JojaMart & Railroad)
* TownSquareBoost: on the stone paths in the town square

Other options:
* EnableStatusEffect: Whether the status effect icon should show up on screen.
* GeneralBoost: The boost given by default if no other takes presedent. Any boost
with a value of 0 will be ignored and use the general boost instead. If the general
boost is 0, you will walk at vanilla speeds when off-path.
* UnknownFloorBoost: The boost given to any unknown flooring type. This value
should never be used by the mod unless a new version of Stardew (or a mod) adds
a new type of flooring.
* EnableCommand: Enables the sp_floorinfo command that prints three things:
  * the location name
  * the flooring id (if it exists)
  * the tile index underneath the player. These values are useful for adding new
    speed locations to the mod.


#### Other Info:
This mod technically supports negative values as well (it will even change the
effect icon).

#### Thanks to:
Entoarox for the FasterPaths mod and EntoaroxFramework. Both mods assisted me in
figuring out what was required to make this mod.
