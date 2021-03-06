**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ferdaber/sdv-mods**

----

# DeluxeGrabberRedux

A Stardew Valley mod - spiritual successor of [Deluxe Grabber](https://www.nexusmods.com/stardewvalley/mods/2462). Download at [Nexus](https://www.nexusmods.com/stardewvalley/mods/7920).

This mod beefs up your Auto Grabber so it can do more things than just collect animal produce:

- Harvest slime balls from the slime hutch.
- Harvest mushrooms from your farm cave.
- Harvest crops in your farm (as well as from indoor pots), with a configurable range for game balance.
- Forage items throughout town.
- Dig up artifact spots throughout town.
- Collect ores from ore pan sites through town.
- Chop down hardwood stumps in the Secret Woods.
- Harvest berries from bushes.
- Harvest fruits from fruit trees.

All of these options are configurable. To use this mod, just place an Auto-Grabber on a specific map, and the Auto-Grabber will do all of the above applicable things in that map.
If you place one in the Secret Woods, it will forage all items that spawn at the beginning of the day, and chop down the stumps for you. If you place one outdoors in your farm, it will
harvest your crops, harvest fruits from fruit trees, etc.

This mod is compatible with the [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) to customize settings. Otherwise you can change your config.json on your own.

Available settings:

- `slimeHutch: boolean` - Enable/disable harvesting slime balls from slime hutches.
- `farmCaveMushrooms: boolean` - Enable/disable harvesting from mushroom boxes inside the farm cave.
- `harvestCrops: boolean` - Enable/disable harvesting crops. Note that disabling this will automatically cause the mod to also disable the following: `harvestCropsIndoorPots`, `harvestCropsRange`, and `flowers`.
- `flowers: boolean` - Enable/disable harvesting flowers. Useful if you want to manually harvest flowers for Bee House purposes.
- `harvestCropsIndoorPots: boolean` - Enable/disable harvesting crops from pots.
- `harvestCropsRange: integer` - Customize the crop-harvesting range of an Auto-Grabber. If set, the Auto-Grabber will only harvest crops within that number of tiles from it. Does not affect other harvesting mechanisms.
- `artifactSpots: boolean` - Enable/disable digging up artifact spots on the ground.
- `orePan: boolean` - Enable/disable collecting ores from ore pan sites in the water.
- `bushes: boolean` - Enable/disable foraging berries from bushes.
- `fruitTrees: boolean` - Enable/disable harvesting fruits from fruit trees.
- `fellSecretWoodsStumps: boolean` - Enable/disable the chopping down of stumps inside the Secret Woods.
- `reportYield: boolean` - Enable/disable reporting of the yield of each auto-grabber inside the SMAPI console.
- `gainExperience: boolean` - Enable/disable skill EXP gain from auto grabbers as if you did the forage/harvest yourself.
