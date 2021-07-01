**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://gitlab.com/kdau/cropbeasts**

----

# Configuring Cropbeasts

If you have installed Generic Mod Config Menu, you can access this mod's configuration by clicking the cogwheel button at the lower left corner of the Stardew Valley title screen and then choosing "Cropbeasts".

If you have not installed GMCM, or if you need to make direct edits, you can configure this mod by editing its `config.json` file. That file will be create in the mod's main folder (`Mods/Cropbeasts`) the first time you run the game with the mod installed.

## Spawning

### Bypass Magi-Seal of Protection

Whether to spawn cropbeasts on farms that don't otherwise have monsters, disregarding the Wilderness Farm type and the Dark Statue of Night Terrors.

`"SpawnOnAnyFarm"`: `true`, `false` (default)

### Daily Chance

How likely it is for any cropbeasts to spawn on any given day. This chance does not guarantee spawns.

`"DailyChance"`: number between `0.0` (0%) and `1.0` (100%); default is `1.0`

### Allow Simultaneous Cropbeasts

Whether additional cropbeasts can spawn while another cropbeast is still active.

`"AllowSimultaneous"`: `true`, `false` (default)

### Outdoor Spawn Limit

How many times per day cropbeasts can spawn outdoors on a farm.

<details>
<summary>Spoiler for late-game 1.5 content</summary>
The farm on Ginger Island uses this number too, but counts towards a separate limit.
</details>

Set to `-1` for unlimited spawns. Set to `0` to prevent outdoor cropbeasts.

`"OutdoorSpawnLimit"`: integer; default is `5`

### Indoor Spawn Limit

How many times per day cropbeasts can spawn indoors in a greenhouse.

Set to `-1` for unlimited spawns. Set to `0` to prevent indoor cropbeasts.

`"IndoorSpawnLimit"`: integer; default is `2`

### Wicked Statue Range

The radius (in tiles) around a Wicked Statue where crops are protected from becoming cropbeasts.

Set to `-1` for infinite range. Set to `0` to ignore the statues. (Scarecrows have a range of 9 tiles.)

`"WickedStatueRange"`: integer; default is `9`

### Witch Flyovers

Whether the witch should fly over the first outdoor cropbeast crop of each day and cast a spell to start the process.

`"WitchFlyovers"`: `true` (default), `false`

## Visibility

### High Contrast

Whether cropbeasts that look very similar to crops should be tinted more vividly to increase their contrast with the crops around them.

`"HighContrast"`: `true`, `false` (default)

### Tracking Arrows

Whether to display arrows pointing to offscreen cropbeasts, like those provided by the Tracker profession.

`"TrackingArrows"`: `true`, `false` (default)

## Specific Cropbeasts

### Berrybeast Face

What kind of face should appear on Berrybeasts.

`"BerrybeastFace"`: `"Blank"`, `"Eyes"`, `"Mouth"`, `"Both"`, `"Random"` (default)

### Cactusbeast Sandblasts

Whether sandblasts from Cactusbeasts cover the screen with sand.

`"CactusbeastSandblast"`: `true` (default), `false`

### Rootbeasts Hide in Dirt

Whether Rootbeasts hide in the dirt, appearing like a crop, when far from any farmer.

`"RootbeastHiding"`: `true` (default), `false`

### Excluded Cropbeasts

A list of cropbeasts that should *not* be spawned. Add names to the list if certain cropbeasts are not desired on your farm.

`"ExcludedBeasts"`: array of strings; valid values:

* `"Berrybeast"`
* `"Grainbeast"`
* `"Leafbeast"`
* `"Rootbeast"`
* `"Trellisbeast"`
* `"Giant Cropbeast"`
* `"Ancient Beast"`
* `"Cactusbeast"`
* `"Coffeebeast"`
* `"Qi Beast"`
* `"Starbeast"`
