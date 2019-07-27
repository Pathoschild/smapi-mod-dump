# Farm Type Manager
This mod allows you to customize the features from Stardew Valley's different farm maps. It currently supports spawning ore and forageable plants on any map in the game, and spawning large objects (stumps, boulders, meteorites, etc) on the farm itself. You can configure the number of objects spawned per day, which areas or terrain types they can spawn on, the chances of spawning each object type, and more.

## Contents
* [Installation](#installation)
* [Setup](#setup)
* [Examples](#examples)
* [Commands](#commands)
* [Settings](#settings)
    * [Basic Settings](#basic-settings)
    * [Forage Spawn Settings](#forage-spawn-settings)
    * [Large Object Spawn Settings](#large-object-spawn-settings)
    * [Ore Spawn Settings](#ore-spawn-settings)
    * [Extra Conditions](#extra-conditions)
    * [Other Settings](#other-settings)
    * [File Conditions](#file-conditions)
    * [Mod Settings](#mod-settings)
* [Content Packs](#content-packs)

## Installation
1. **Install the latest version of [SMAPI](https://smapi.io/).**
2. **Download FarmTypeManager** from [the Releases page on GitHub](https://github.com/Esca-MMC/FarmTypeManager/releases) or from [Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/3231/).
3. **Unzip FarmTypeManager** into your `Stardew Valley\Mods` folder.

If you use other mods that require Farm Type Manager, then you’re all set here!

If you want to customize Farm Type Manager’s settings yourself, follow these instructions:

## Setup
1. **Run the game** using SMAPI.
2. **Load the farm** you want to customize, or create a new character and progress past the game's intro. This will create a configuration file for your character in the `Stardew Valley\Mods\FarmTypeManager\data` folder.
3. **Find the .json file** with your character's name on it. Load it with the **ConfigEditor.html** file in the `FarmTypeManager` folder, or edit manually with any text editor.
4. **Customize the file's settings** (see [Examples](#examples) or [Settings](#settings)) and save your changes.
5. **Exit your farm and reload it**, or progress to the next in-game day. The mod's settings should now be active.
 
Note: This mod will be disabled in multiplayer when you play as a farmhand (i.e. join a game hosted by someone else). Its console commands will still function, though.

## Examples
Below are a few examples of changes you can make to your character's configuration file, spawning various things on your farm or the other in-game maps.

### Spawn forage and respawn stumps on the farm
```
"ForageSpawnEnabled": true,
"LargeObjectSpawnEnabled": true,
```
![Your farm's stumps will respawn every day.](docs/images/ftm_example_1.png)

### Randomly spawn new logs and boulders on the farm
```
"LargeObjectSpawnEnabled": true,
```
```
"Large_Object_Spawn_Settings": {
    "Areas": [
      {
        "ObjectTypes": [
          "Log", "Boulder"
        ],
        "FindExistingObjectLocations": false,
        "PercentExtraSpawnsPerSkillLevel": 0,
        "RelatedSkill": "Foraging",
        "MapName": "Farm",
        "MinimumSpawnsPerDay": 0,
        "MaximumSpawnsPerDay": 2,
        "AutoSpawnTerrainTypes": ["Grass", "Dirt", "Diggable"],
        "IncludeAreas": [],
        "ExcludeAreas": []
```
![Most days, logs and/or boulders will spawn on the farm.](docs/images/ftm_example_2.png)

### Spawn ore in a specific area of Cindersap Forest
```
"OreSpawnEnabled": true,
```
```
"Ore_Spawn_Settings": {
    "Areas": [
      {
        "MiningLevelRequired": null,
        "StartingSpawnChance": null,
        "LevelTenSpawnChance": null,
        "MapName": "Forest",
        "MinimumSpawnsPerDay": 1,
        "MaximumSpawnsPerDay": 5,
        "AutoSpawnTerrainTypes": [],
        "IncludeAreas": [ "65,22/74,27" ],
        "ExcludeAreas": []
```
![Ore will spawn in a specific area of the Forest map.](docs/images/ftm_example_3.png)

### Spawn LOTS of forage on the farm, but not near the house
```
"ForageSpawnEnabled": true,
```
```
"Forage_Spawn_Settings": {
    "Areas": [
      {
	"SpringItemIndex": null,
        "SummerItemIndex": null,
        "FallItemIndex": null,
        "WinterItemIndex": null,
        "MapName": "Farm",
        "MinimumSpawnsPerDay": 9999,
        "MaximumSpawnsPerDay": 9999,
        "AutoSpawnTerrainTypes": [ "All" ],
        "IncludeAreas": [],
        "ExcludeAreas": [ "69,17;57,10" ]
```
![Forage will spawn everywhere on the Farm map, except around the house.](docs/images/ftm_example_4.png)

### Spawn [modded plants](https://www.nexusmods.com/stardewvalley/mods/1598) on the [mod-enabled summit](https://www.nexusmods.com/stardewvalley/mods/2073), but only after rainy days & after year 1
```
"ForageSpawnEnabled": true,
```
```
  "Forage_Spawn_Settings": {
    "Areas": [
      {
        "SpringItemIndex": [ 2018, 2021 ],
        "SummerItemIndex": [ 2018, 2021 ],
        "FallItemIndex": [ 2018, 2021 ],
        "WinterItemIndex": null,
        "MapName": "Summit",
        "MinimumSpawnsPerDay": 4,
        "MaximumSpawnsPerDay": 8,
        "AutoSpawnTerrainTypes": [
          "All"
        ],
        "IncludeAreas": [],
        "ExcludeAreas": [],
        "StrictTileChecking": "High",
        "ExtraConditions": {
          "Years": [ "2+" ],
          "Seasons": [],
          "Days": [],
          "WeatherYesterday": [ "Rain", "Storm" ],
          "WeatherToday": [],
          "WeatherTomorrow": [],
          "LimitedNumberOfSpawns": null
```
![Custom forage spawned on a mod-enabled map with specific time and weather conditions.](docs/images/ftm_example_5.png)

## Commands
This mod adds the command `whereami` to SMAPI's console. Enter it there to display information about the current map, including: 
* The map's name (e.g. "Farm" or "BusStop")
* Your current tile's X/Y coordinates
* The tile's terrain type (e.g. "Dirt" or "Stone")
* Whether the tile is "Diggable" with tools
* The tile's image index number (used to identify "Quarry" tiles or set up custom tile-matching)

This command can be disabled in the **config.json** file if desired, e.g. if it conflicts with another mod. See [Mod Settings](#mod-settings).

## Settings
The settings below change how the mod affects your farm. By default, the mod will not change your farm at all; you'll have to enable features you want in the Basic Settings section, and then (optionally) configure their behavior with the rest of the settings.

Your config files are stored in the `Stardew Valley\Mods\FarmTypeManager\data` folder. When you load a farm in-game and no  file exists yet, a new file will be created for it. The file is named after your save data folder, so it starts with your farmer’s name and then a series of numbers, such as **FarmerName_1234567.json**. Any text editor should be able to open it.

The mod will also generate a **default.json** file in the data folder. It can be edited to change the settings of any newly generated config files. This can be useful if you create new farms frequently or need to customize settings for several farms at once. If the default.json file doesn't exist, it will be created when launching Stardew or loading a farm.

### Basic Settings
This section covers the simple on/off switches you can use to enable default configurations, which work similarly to "vanilla" Stardew farm types.

Name | Valid settings | Description | Notes
-----|----------------|-------------|------
ForageSpawnEnabled | true, **false** | Enables or disables spawning forageable plants. | When set to **true** with other default settings, this will work similarly to the Forest Farm, randomly spawning a few forage items on the farm each day.
LargeObjectSpawnEnabled | true, **false** | Enables or disable spawning large objects (e.g. stumps). | When set to **true** with other default settings, this will find any existing large stumps on your farm and cause those to respawn each day.
OreSpawnEnabled | true, **false** | Enables or disables spawning ore. | When set to **true** with other default settings, this will work similarly to the Hill-top Farm, spawning various kinds of ore on any "Quarry" terrain your farm may have. (If you're not using the Hill-top Farm or a custom farm with similar-looking quarries, nothing will happen. You'll need to change the **AutoSpawnTerrainTypes** or **IncludeAreas** settings in the ore section.)

The sections below cover the more advanced options for each spawn type. When the features in Basic Settings above are enabled, the mod will use the Spawn Settings to determine *which* objects should be spawned, *how many* to spawn each day, and *where* to put them.

### Forage Spawn Settings
Name | Valid settings | Description | Notes
-----|----------------|-------------|------
Areas | *(see Notes)* | A list of forage settings for each in-game area. | The default settings here will provide forage settings for the farm, in a way similar to the game's Forest Farm map. To add forage to multiple maps (or the same map in multiple ways), create copies of the entire bracketed section and separate them with commas, like so: `"Areas": [ {Area 1}, {Area 2}, {Area 3} ]`
SpringItemIndex (Area) | **null**, *(see Notes)* | The index numbers and/or item names for forage items to spawn in spring *in this area*. | This setting is per-area and will *override* the global SpringItemIndex setting below. Leave this set to *null* unless you want different spring forage items in a specific area. Fill this in by copying the format of the global version below.
SummerItemIndex (Area)| **null**, *(see Notes)* | The index numbers and/or item names for forage items to spawn in summer *in this area*. | This setting is per-area and will *override* the global SummerItemIndex setting below. Leave this set to *null* unless you want different summer forage items in a specific area. Fill this in by copying the format of the global version below.
FallItemIndex (Area) | **null**, *(see Notes)* | The index numbers and/or item names for forage items to spawn in fall *in this area*. | This setting is per-area and will *override* the global FallItemIndex setting below. Leave this set to *null* unless you want different fall forage items in a specific area. Fill this in by copying the format of the global version below.
WinterItemIndex (Area) | **null**, *(see Notes)* | The index numbers and/or item names for forage items to spawn in winter *in this area*. | This setting is per-area and will *override* the global WinterItemIndex setting below. Leave this set to *null* unless you want different winter forage items in a specific area. Fill this in by copying the format of the global version below.
UniqueAreaID | Any unique name: "Forage area 1", etc | A unique nickname for this area. | This is used by the save system to record certain info for each area, and in SMAPI's error log. If the same ID is used for multiple areas, the duplicates will be automatically assigned new names; this might cause minor problems with the mod's saved info, e.g. LimitedNumberOfSpawns.
MapName | Any map name: "Farm", "BusStop", etc | The name of the map in which forage will spawn. | Go to a map in-game and use the `whereami` console command to find its technical name (see [Commands](#commands)).
MinimumSpawnsPerDay | Any integer (less than or equal to the maximum) | The minimum number of forage to spawn per day. | Numbers less than or equal to zero are valid choices; negative numbers just increase the chance of spawning no forage for the day. If the number is very high (e.g. 9999), forage will spawn until it runs out of valid space.
MaximumSpawnsPerDay | Any integer (greater than or equal to the minimum) | The maximum number of forage to spawn per day. | Note that this is calculated *before* PercentExtraSpawnsPerForagingLevel, so it's possible to spawn more than this number based on players' skill points.
AutoSpawnTerrainTypes | **"Diggable", "Grass"**, "Dirt", "Stone", "Wood", "All", "Quarry", "Custom" | A list of terrains on which forage can spawn. | The "All" setting will let forage spawn anywhere on the map. For other settings, multiple terrain types can be used by separating them with commas, like so: `"AutoSpawnTerrainTypes": ["Grass", "Diggable"]`
IncludeAreas | `"x,y/x,y"` tile coordinates | A list of coordinates for areas in which forage can spawn. | Use the `whereami` command (see [Commands](#commands)) to get a tile's coordinates. Any space between the two coordinates you use will be open for spawning. Separate multiple include areas with commas, like so: `"IncludeAreas": ["0,0/100,100", "125,125/125,125"]`
ExcludeAreas | `"x,y/x,y"` tile coordinates | A list of coordinates for areas in which forage *cannot* spawn. | See the notes for IncludeAreas to find coordinates. Any space covered by ExcludeAreas will not be used to spawn forage, *overriding* AutoSpawnTerrainTypes and IncludeAreas.
StrictTileChecking | **"High"**, "Medium", "Low", "None" | How strictly the mod will validate possible locations for forage. | Depending on the map's internal settings (especially in custom farms), Stardew might consider some tiles "invalid for object placement". If your other settings seem correct but nothing is spawning in this area, try adjusting this setting. Note that "Low" and "None" might result in missing spawns: if a tile really *can't* have objects, the object won't spawn and the mod will no longer notice. May also cause clipping with terrain, buildings, etc.
DaysUntilSpawnsExpire | **null**, Intenger | The number of days forage will remain on the ground before disappearing. | If set to null, spawned forage will behave normally. If set to a positive number, unharvested spawns will disappear after that number of days. If set to 0, they will never despawn. Spawns with expiration dates will also respawn if deleted overnight, e.g. by the game's weekly forage cleaner.
PercentExtraSpawnsPerForagingLevel | Any integer (default **0**) | The % of extra forage spawned for each level of Foraging skill. | In multiplayer, this is based on the highest skill level among *all* players (even if they're offline). For example, setting this to 10 will spawn +10% forage items per level; if a farmhand has the best Foraging skill of level 8, there will be 80% more forage each day.
SpringItemIndex (Global) | A list of integers and/or item names, e.g. `[16, "Red Mushroom"]` | The index numbers and/or item names for forage items to spawn in spring. | By default, these are the forage items normally spawned on the Forest Farm during spring. To find different item IDs or names, see the "Raw data" section of [this wiki page](https://stardewvalleywiki.com/Modding:Object_data), or use this SMAPI console command: `list_items [item name]`
SummerItemIndex (Global) | A list of integers and/or item names, e.g. `[16, "Red Mushroom"]` | The index numbers for forage items to spawn in summer. | By default, these are the forage items normally spawned on the Forest Farm during summer. To find different item IDs or names, see the "Raw data" section of [this wiki page](https://stardewvalleywiki.com/Modding:Object_data), or use this SMAPI console command: `list_items [item name]`
FallItemIndex (Global) | A list of integers and/or item names, e.g. `[16, "Red Mushroom"]` | The index numbers for forage items to spawn in fall. | By default, these are the forage items normally spawned on the Forest Farm during fall. To find different item IDs or names, see the "Raw data" section of [this wiki page](https://stardewvalleywiki.com/Modding:Object_data), or use this SMAPI console command: `list_items [item name]`
WinterItemIndex (Global) | A list of integers and/or item names, e.g. `[16, "Red Mushroom"]` | The index numbers for forage items to spawn in winter. | By default, this is empty because forage doesn't normally spawn during winter. To add winter forage, use the same format as the other ItemIndex settings above. To find different item IDs or names, see the "Raw data" section of [this wiki page](https://stardewvalleywiki.com/Modding:Object_data), or use this SMAPI console command: `list_items [item name]`
CustomTileIndex | Integer(s) | A list of index numbers from the game's tilesheet images, used by the "Custom" setting for AutoSpawnTerrainTypes. | If the AutoSpawnTerrainTypes setting above includes the "Custom" option, any tiles with image index numbers matching this list will be valid places for forage to spawn. You can find a tile's image index number by standing on it and using the `whereami` command, or by using modding tools that edit Stardew maps and/or tilesheet images.

### Large Object Spawn Settings
Name | Valid settings | Description | Notes
-----|----------------|-------------|------
Areas | *(see Notes)* | A list of large object settings for each in-game area. | The default settings here will automatically respawn any existing large stumps on the farm, in a way vaguely similar to the game's Forest Farm map. To add large object spawning to multiple maps (or the same map in multiple ways), create copies of the entire bracketed section and separate them with commas, like so: `"Areas": [ {Area1}, {Area2}, {Area3} ]`
ObjectTypes | **"Stump"**, "Log", "Boulder", "Meteorite", "Mine Rock 1", "Mine Rock 2", "Mine Rock 3", "Mine Rock 4" | A list of object types to spawn. | Objects spawned in this area will be chosen randomly from this list. Adding the same object type multiple times will increase its chances. Separate multiple objects with commas, like so: `"ObjectTypes: [ "Stump", "Log", "Meteorite" ]`
FindExistingObjectLocations | **true**, false | Finds any existing objects listed in ObjectTypes and adds them to the IncludeAreas list. | This can be used to automatically find existing objects' coordinates and respawn them each day. It will set itself to "false" in your settings file after completion.
PercentExtraSpawnsPerSkillLevel | Any integer (default **0**) | The % of extra objects spawned for each level of the RelatedSkill. | In multiplayer, this is based on the highest skill level among *all* players (even if they're offline). For example, setting this to 10 will spawn +10% items per skill level; if a farmhand has the best skill of level 8, there will be 80% more objects each day.
RelatedSkill | "Farming", "Fishing", **"Foraging"**, "Mining", "Combat" | The skill used by PercentExtraSpawnsPerSkillLevel to spawn extra objects.
UniqueAreaID | Any unique name: "Large object area 1", etc | A unique nickname for this area. | This is used by the save system to record certain info for each area, and in SMAPI's error log. If the same ID is used for multiple areas, the duplicates will be automatically assigned new names; this might cause minor problems with the mod's saved info, e.g. LimitedNumberOfSpawns.
MapName | Any map name: "Farm", "BusStop", etc | The name of the map in which objects will spawn. | Go to a map in-game and use the `whereami` console command to find its technical name (see [Commands](#commands)).
MinimumSpawnsPerDay | Any integer (less than or equal to the maximum) | The minimum number of objects to spawn per day. | Numbers less than or equal to zero are valid choices; negative numbers just increase the chance of spawning no objects for the day. If the number is very high (e.g. 999), objects will spawn until they run out of valid space.
MaximumSpawnsPerDay | Any integer (greater than or equal to the minimum) | The maximum number of objects to spawn per day. | Note that this is calculated *before* PercentExtraSpawnedPerSkillLevel, so it's possible to spawn more than this number based on players' skill points.
AutoSpawnTerrainTypes | "Diggable", "Grass", "Dirt", "Stone", "Wood", "All", "Quarry", "Custom" | A list of terrains on which objects can spawn. | The "All" setting will let objects spawn anywhere on the map. For other settings, multiple terrain types can be used by separating them with commas, like so: `"AutoSpawnTerrainTypes": ["Grass", "Diggable"]`
IncludeAreas | `"x,y/x,y"` tile coordinates | A list of coordinates for areas in which objects can spawn. | Use the `whereami` command (see [Commands](#commands)) to get a tile's coordinates. Any space between the two coordinates you use will be open for spawning. Separate multiple include areas with commas, like so: `"IncludeAreas": ["0,0/100,100", "125,125/125,125"]`
ExcludeAreas | `"x,y/x,y"` tile coordinates | A list of coordinates for areas in which objects *cannot* spawn. | See the notes for IncludeAreas to find coordinates. Any space covered by ExcludeAreas will not be used to spawn forage, *overriding* AutoSpawnTerrainTypes and IncludeAreas.
StrictTileChecking | **"High"**, "Medium", "Low", "None" | How strictly the mod will validate possible locations for large objects. | Depending on the map's internal settings (especially in custom farms), Stardew might consider some tiles "invalid for object placement". If your other settings seem correct but nothing is spawning in this area, try adjusting this setting. Note that "Low" and "None" might result in missing spawns: if a tile really *can't* have objects, the object won't spawn and the mod will no longer notice. May also cause clipping with terrain, buildings, etc.
DaysUntilSpawnsExpire | **null**, Intenger | The number of days objects will remain on the ground before disappearing. | If set to null, spawned objects will behave normally. If set to a positive number, unharvested spawns will disappear after that number of days. If set to 0, they will never despawn. Spawns with expiration dates will also respawn if deleted overnight, e.g. by other mods.
CustomTileIndex | Integer(s) | A list of index numbers from the game's tilesheet images, used by the "Custom" setting for AutoSpawnTerrainTypes. | If the AutoSpawnTerrainTypes setting above includes the "Custom" option, any tiles with image index numbers matching this list will be valid places for objects to spawn. You can find a tile's image index number by standing on it and using the `whereami` command, or by using modding tools that edit Stardew maps and/or tilesheet images.

### Ore Spawn Settings
Name | Valid settings | Description | Notes
-----|----------------|-------------|------
Areas | *(see Notes)* | A list of ore settings for each in-game area. | The default settings here will automatically spawn ore in any areas of the farm that use "Quarry" tiles resembling those on the Hill-top Farm. To add ore spawning to multiple maps (or the same map in multiple ways), create copies of the entire bracketed section and separate them with commas, like so: `"Areas": [ {Area1}, {Area2}, {Area3} ]`
MiningLevelRequired (Area) | **null**, *(see Notes)* | The minimum Mining skill level needed to spawn each ore type *in this area*. | This setting is per-area and will *override* the global MiningLevelRequired setting below. Leave this set to *null* unless you want different level requirements in a specific area. Fill this in by copying the format of the global version below.
StartingSpawnChance (Area) | **null**, *(see Notes)* | Each ore type’s chance of spawning with the minimum required Mining skill *in this area*. | This setting is per-area and will *override* the global StartingSpawnChance setting below. Leave this set to *null* unless you want different chances in a specific area. Fill this in by copying the format of the global version below.
LevelTenSpawnChance (Area) | **null**, *(see Notes)* | Each ore type’s chance of spawning with level 10 Mining skill *in this area*. | This setting is per-area and will *override* the global LevelTenSpawnChance setting below. Leave this set to *null* unless you want different chances in a specific area. Fill this in by copying the format of the global version below.
UniqueAreaID | Any unique name: "Ore area 1", etc | A unique nickname for this area. | This is used by the save system to record certain info for each area, and in SMAPI's error log. If the same ID is used for multiple areas, the duplicates will be automatically assigned new names; this might cause minor problems with the mod's saved info, e.g. LimitedNumberOfSpawns.
MapName | The name of the map in which ore will spawn. | Go to a map in-game and use the `whereami` console command to find its technical name (see [Commands](#commands)).
MinimumSpawnsPerDay | Any integer (less than or equal to the maximum) | The minimum amount of ore to spawn per day. | Numbers less than or equal to zero are valid choices; negative numbers just increase the chance of spawning no ore for the day. If the number is very high (e.g. 999), ore will spawn until it runs out of valid space.
MaximumSpawnsPerDay | Any integer (greater than or equal to the minimum) | The maximum amount of ore to spawn per day. | Note that this is calculated *before* PercentExtraSpawnsPerMiningLevel, so it's possible to spawn more than this number based on players' skill points.
AutoSpawnTerrainTypes | "Diggable", "Grass", "Dirt", "Stone", "Wood", "All", **"Quarry"**, "Custom" | A list of terrains on which ore can spawn. | The "All" setting will let ore spawn anywhere on the map. For other settings, multiple terrain types can be used by separating them with commas, like so: `"AutoSpawnTerrainTypes": ["Grass", "Diggable"]`
IncludeAreas | `"x,y/x,y"` tile coordinates | A list of coordinates for areas in which ore can spawn. | Use the `whereami` command (see [Commands](#commands)) to get a tile's coordinates. Any space between the two coordinates you use will be open for spawning. Separate multiple include areas with commas, like so: `"IncludeAreas": ["0,0/100,100", "125,125/,125,125"]`
ExcludeAreas | `"x,y/x,y"` tile coordinates | A list of coordinates for areas in which ore *cannot* spawn. | See the notes for IncludeAreas to find coordinates. Any space covered by ExcludeAreas will not be used to spawn forage, *overriding* AutoSpawnTerrainTypes and IncludeAreas.
StrictTileChecking | **"High"**, "Medium", "Low", "None" | How strictly the mod will validate possible locations for ore. | Depending on the map's internal settings (especially in custom farms), Stardew might consider some tiles "invalid for object placement". If your other settings seem correct but nothing is spawning in this area, try adjusting this setting. Note that "Low" and "None" might result in missing spawns: if a tile really *can't* have objects, the object won't spawn and the mod will no longer notice. May also cause clipping with terrain, buildings, etc.
DaysUntilSpawnsExpire | **null**, Intenger | The number of days ore will remain on the ground before disappearing. | If set to null, spawned ore will behave normally. If set to a positive number, unharvested spawns will disappear after that number of days. If set to 0, they will never despawn. Spawns with expiration dates will also respawn if deleted overnight, e.g. by other mods.
PercentExtraSpawnsPerMiningLevel | Any integer (default **0**) | The % of extra ore spawned for each level of Mining skill. | In multiplayer, this is based on the highest skill level among *all* players (even if they're offline). For example, setting this to 10 will spawn +10% ore per Mining level; if a farmhand has the best Mining skill of level 8, there will be 80% more ore each day.
MiningLevelRequired (Global) | 0-10 | The minimum Mining skill level needed to spawn each ore type. | An ore type won't start spawning until *any* player (even offline farmhands) has the listed Mining skill. 
StartingSpawnChance (Global) | 0 or more | Each ore type's chance of spawning with the minimum required Mining skill. | These numbers are weighted chances; they don't need to add up to 100. The defaults are roughly based on the native game's spawn chances with slight increases.
LevelTenSpawnChance (Global) | 0 or more | Each ore type's chance of spawning with level 10 Mining skill. | Chances will drift gradually from StartingSpawnChance to LevelTenSpawnChance. For example, in the default settings, frozen geodes' chances of spawning from level 5 to 10 are `4, 4, 3, 3, 2, 2`.
CustomTileIndex | Integer(s) | A list of index numbers from the game's tilesheet images, used by the "Custom" setting for AutoSpawnTerrainTypes. | If the AutoSpawnTerrainTypes setting above includes the "Custom" option, any tiles with image index numbers matching this list will be valid places for ore to spawn. You can find a tile's image index number by standing on it and using the `whereami` command, or by using modding tools that edit Stardew maps and/or tilesheet images.

### Extra Conditions
Name | Valid settings | Description | Notes
-----|----------------|-------------|------
ExtraConditions | *(see Notes)* | A set of optional conditions required to spawn objects in an area. | These can be used to restrict spawning to certain times or weather conditions. Each setting will be ignored if it is set to its default, i.e. `[]` or `null`.
Years | A list of integers, ranges, or "Any"/"All" | A list of years in which things are allowed to spawn. | Years should be inside the brackets with quotation marks, separated by commas if multiple are listed. They can be listed as single years, ranges, or with a **+** to include any following years. See these examples: `["1"]` `["2-4"]` `["1", "3+"]`
Seasons | A list of season names: "Spring", "Summer", "Fall", "Winter", or "Any"/"All" | A list of seasons in which things are allowed to spawn. | Seasons should be inside the brackets with quotation marks, separated by commas if multiple are listed. See these examples: `["Spring"]`, `["Summer", "Winter"]`
Days | A list of integers, ranges, or "Any"/"All" | A list of days on which things are allowed to spawn. | Days should be inside the brackets with quotation marks, separated by commas if multiple are listed. They can be listed as single days, ranges, or with a **+** to include any following days. See these examples: `["1"]` `["2-14"]` `["1", "8+"]`
WeatherYesterday | A list of weather names: "Sun", "Wind", "Rain", "Storm", "Snow", or "Any"/"All" | Things will be allowed to spawn if yesterday's weather matched a name from this list. | Weather names should be inside the brackets with quotation marks, separated by commas if multiple are listed. Note that windy days *do not* count as sunny, and storms *do not* count as rain; remember to include both if needed. See these examples: `["Snow"]`, `["Sun", "Wind"]`, `["Rain", "Storm", "Snow"]`
WeatherToday | A list of weather names: "Sun", "Wind", "Rain", "Storm", "Snow", or "Any"/"All" | Things will be allowed to spawn if today's weather matches a name from this list. | Weather names should be inside the brackets with quotation marks, separated by commas if multiple are listed. Note that windy days *do not* count as sunny, and storms *do not* count as rain; remember to include both if needed. See these examples: `["Snow"]`, `["Sun", "Wind"]`, `["Rain", "Storm", "Snow"]`
WeatherTomorrow | A list of weather names: "Sun", "Wind", "Rain", "Storm", "Snow", or "Any"/"All" | Things will be allowed to spawn if tomorrow's weather forecast matches a name from this list. | Weather names should be inside the brackets with quotation marks, separated by commas if multiple are listed. Note that windy days *do not* count as sunny, and storms *do not* count as rain; remember to include both if needed. See these examples: `["Snow"]`, `["Sun", "Wind"]`, `["Rain", "Storm", "Snow"]`
LimitedNumberOfSpawns | An integer | The number of times this area will spawn things before stopping. | At the end of each day, if things spawned here without being prevented by other "extra conditions", this number will count down by 1 (record in the separate **.save** file). Once it reaches zero, the area will stop spawning things. Note that unlike other ExtraConditions settings, this does not need to be in brackets or quotations. Example: `1`

### Other Settings
Name | Valid settings | Description | Notes
-----|----------------|-------------|------
QuarryTileIndex | Integer(s) | A list of index numbers for "quarry" tiles in the game's tilesheet images. | These have been manually chosen to match the "quarry" tiles in the game's Hill-top Farm and custom maps that use similar mining areas. They're provided here so that people familiar with editing Stardew maps can customize this mod's "Quarry" terrain setting.

### File Conditions
Name | Valid settings | Description | Notes
-----|----------------|-------------|------
File_Conditions | *(see below)* | A list of conditions required to use this config file with a specific farm. | These settings are primarily useful for content packs.
FarmTypes | A list of farm types: "Standard", "Riverland", "Forest", "Hilltop", "Wilderness", or "Any"/"All" | A list of farm types for which this config file will be used. | This should generally be used in content packs that are combined with custom farm maps; for example, if a custom farm replaces your Standard farm type, set this to `["Standard"]`.
FarmerNames | A list of farmer names, e.g. `["Esca"]` | If one of this list's names matches the farmer's name, this config file will be used. | This setting only checks the main farmer's name; it won't be affected by multiplayer farmhands.
SaveFileNames | A list of save file names, e.g. `["Esca_1234567"]` | If one of these names matches the current farm's save file name, this config file will be used. | A niche setting for when other conditions aren't effective at choosing a farm. Note that this technically checks the save *folder* name, not the save file itself.
OtherMods | A list of mods' UniqueIDs and `true` or `false` (see Notes) | If all of the player's loaded mods match this list, this config file will be used. | This can be used to make a content pack or config file only activate with certain mod requirements. `true` means a mod *must* be installed, while `false` means it *can't* be installed. Example: `OtherMods: { "Esca.FarmTypeManager" : true, "FakeModID" : false }`
ResetMainDataFolder | true, **false** | When true, this will move any files in `FarmTypeManager\data` to an archive folder. | This is used by mods migrating from older versions to FTM v1.4's content pack format. It will only happen once, unless the content pack's save data is deleted. The data files are moved to `FarmTypeManager\data\archive\[timestamp]`.

### Mod Settings
These settings are in the **config.json** file, which is in the mod's main folder: `Stardew Valley\Mods\FarmTypeManager`. They change the behavior of the mod itself, rather than one specific farm.

Name | Valid settings | Description
-----|----------------|------------
EnableWhereAmICommand | **true**, false | Enables or disables the `whereami` console command.
EnableContentPacks | **true**, false | Enables or disables any installed content packs for Farm Type Manager.
EnableContentPackFileChanges | **true**, false | Enables or disables special file changes by content packs, such as resetting the `FarmTypeManager\data` folder.

## Content Packs
In version 1.4 and later, Farm Type Manager can use configuration files from content packs. They will be used *in addition* to the farm-specific files in the `FarmTypeManager\data` folder; they will not replace or override each other.

To create a content pack for Farm Type Manager:

1. Create a new folder in the `Stardew Valley\Mods` folder. Its name should follow this format: `[FTM] Your Pack Name`
2. Create a text file in the new folder called **manifest.json**. (See the [Manifest](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Manifest) wiki article for more info.) Copy the format below and fill in the settings appropriately:
```
{
  "Name": "Your Pack Name",
  "Author": "Your Name",
  "Version": "1.0.0",
  "Description": "Your description here. Briefly explain what the content pack does.",
  "UniqueID": "YourName.YourPackName",
  "MinimumApiVersion": "2.10.0",
  "UpdateKeys": [],
  "ContentPackFor": {
    "UniqueID": "Esca.FarmTypeManager",
    "MinimumVersion": "1.4.0"
  }
}
```
3. Create or copy a Farm Type Manager configuration file into the folder and name it **content.json**. The format is exactly the same as a farm configuration file from the `FarmTypeManager\data` folder.
4. If you intend to combine this content pack with a custom farm map or similar mod, consider editing the **FarmTypes** setting under **File_Conditions** at the bottom of the *content.json* file. (See the [File Conditions](#file-conditions) section.)
