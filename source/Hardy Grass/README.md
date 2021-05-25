**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/DiscipleOfEris/HardyGrass**

----

# HardyGrass
A Stardew Valley mod where grass is "cut" instead of destroyed when reaped or eaten.

See [This link](https://www.nexusmods.com/stardewvalley/mods/8772) for the NexusMods mod page, which has a description, screenshots, and a download of the built mod.

## How it works

This mod uses [Harmony](https://github.com/pardeike/Harmony) to:
- Replace `Grass.dayUpdate` to incorporate Quick Grass and configurable growth rates.
- Postfix `Grass.reduceBy` to prevent grass getting destroyed when it reaches 0 tufts.
- Replace `Grass.doCollisionAction` to prevent grass with 0 tufts from slowing the farmer.
- Prefix and Postfix `Grass.performToolAction` so grass is not destroyed by scythes and instead only destroyed by Axe/Pickaxe/Hoe if already fully cut.
- Replace `Grass.draw` to draw the cut grass.
- Replace `FarmAnimal.grassEndPointFunction` so animals ignore grass with 0 tufts.
- Replace `FarmAnimal.Eat` to support animals eating their correct number of tufts even if their first patch doesn't have enough.
- Replace `GameLocation.growWeedGrass` to support Quick Grass and configurable growth/spread rates.
- Prefix `Object.placementAction` to support Quick Grass Starter.
- Postfix `Object.isPlaceable` to support Quick Grass Starter.
- Postfix `Object.isPassable` to support Quick Grass Starter.

Hardy Grass is unlikely to be compatible with any mod that Harmony patches one or more of these methods.
