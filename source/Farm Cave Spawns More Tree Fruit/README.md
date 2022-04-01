**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/FarmCaveSpawn**

----

Farm Cave Spawns More Tree Fruit
=======

Modded farm caves don't really play well with how the fruit cave spawns its fruits. In vanilla, each tile has an equal chance to be picked for a spawn, and for modded caves that have large amounts of void in order to implement separate "rooms", it's quite likely that the tile picked is a void tile and nothing will be spawned. This is an attempt to help adjust for that by spawning in more fruit - either the four common vanilla forage fruit, or any item that is produced by a fruit tree. Inspired by but very different from Better FarmCave Forage, which uses FTM to spawn in a selection of various fruits.

Any fruit tree's products can be spawned. Mod trees, as long as they're registered with `Data/fruitTrees`, will be used as well. (Tested with RSV and PPJA More Trees). Fruit not associated with a tree will not be produced.

#### Installation
Unzip into your mods folder. To uninstall, simply delete from mods folder.

#### Multiplayer
Host should install mod. Mod does not do anything for farmhands.

#### SVE compat

Will also spawn in the MinecartCave and the DeepCave, if `UseModCaves` is enabled (after 1.0.8). It goes room by room, so if you don't see spawns in MinecartCave and Deepcave try setting the spawn chance lower and the max daily spawns higher.

#### Known compatability issues:

I check for `Game1.MasterPlayer.caveChoice` to see what farm cave you've picked. If you're using a mod that causes that flag to never be set (such as [Farm Cave Framework](https://www.nexusmods.com/stardewvalley/mods/10506)), this mod will never think the cave is a fruit bat cave. To get around it, you can set either `Ignore Farm Cave Type` to `true` or `Allow Early Farm Cave` to `true`.

Farm Cave Framework is handled seperately - this mod will check to see if that mod's field for cave type contains the word "bat" or "fruit" and assume that it is a fruit cave. It should be compatible now.

#### Configuration:

1. `MaxDailySpawns`: the maximum number of fruit that will spawn.
2. `SpawnChance`: the percent chance any empty square will spawn a fruit, capped by MaxDailySpawns. (Range: 0-100, where 100 means every unoccupied square will have a fruit)
3. `TreeFruitChance`: the percent chance any particular spawn will be fruit tree fruit and not common forage. (Range: 0-100, where 100 means every spawn will be a tree fruit)
4. `IgnoreFarmCaveType`: if true, will spawn fruit in any farm cave (as long as you've seen the cutscene). If false, requires that the fruit cave be selected.
5. `EarlyFarmCave`: if true, will spawn even before the cutscene is seen.
5. `UseModCaves`: if true, will spawn fruit in caves added by mods, if they're registered with this mod.
6. `UseMineCave`: if true, will spawn fruit in the mine cave entrances as well (if `MaxDailySpawns` is not hit.)
6. `UseVanillaFruitOnly`: use only the six tree fruit that will appear in the fruit bat cave in vanilla. (Yes, this will exlude mangos and bananas.)
7. `SeasonalOnly`: limits to just the fruits that are in-season. After all, those bats have to be getting the fruit from somewhere, right?
8. `AllowAnyTreeProduct`: if true, allows any product found for any fruit tree to be spawned. If false, will only spawn items that are categorized as fruit in the game.
9. `EdiblesOnly`: If true, will only spawn items with a positive value for edibility. Do note that this field is not always perfect; some items that do not seem like they should be edible have positive values for edibility.
10. `NoBananasBeforeShrine`: there's an objective in vanilla that requires getting a hold of a banana, which is typically quite hard to do. To avoid trivializing that objective, this setting will prevent bananas from spawning until that objective is complete.
11. `PriceCap`: Caps the value of the fruit that can be spawned.

The console command `av.fcs.list_fruits` will tell you which tree fruits are available for spawning.

#### Denylist

Don't want this mod to spawn *your* fruit? If you use Content Patcher to add an entry to `Mods/atravita_FarmCaveSpawn_denylist` that looks like

```
{
    "Action": "EditData",
    "Target": "Mods/atravita_FarmCaveSpawn_denylist",
    "Entries": {
        "your.uniqueID": "Comma,Seperated,List,Of,Fruit,Names"
    }
}
```

this mod won't spawn your fruit.

#### AdditionalLocations

Want this mod to spawn fruit in a mod-added location? Just use Content patcher to add an entry to `Mods/atravita_FarmCaveSpawn_additionalLocations` that looks like

```
{
    "Action": "EditData",
    "Target": "Mods/atravita_FarmCaveSpawn_additionalLocations",
    "Entries": {
        "your.uniqueID": "Comma,Seperated,List,Of,Location,Names"
    }
}
```

to add your location as a location fruit will spawn. (To limit the spawn location, add `:[(x1;y1);(x2;y2)]` after the location name - ie `Custom_NewLocation:[(3;4);(57;54)]`). Note - semicolons, not commas. 

**Changelog**: https://github.com/atravita-mods/FarmCaveSpawn/blob/master/FarmCaveSpawn/docs/CHANGELOG.MD
