**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Esca-MMC/EscasModdingPlugins**

----

# Esca's Modding Plugins (EMP)
A mod for the game Stardew Valley that adds new data assets, map/tile properties, and features for other mods to use.

## Contents
* [Installation](#installation)
  * [Multiplayer Note](#multiplayer-note)
* [Features](#features)
  * [Fish Locations](#fish-locations)
    * [Using the data asset](#using-the-data-asset)
    * [Using tile properties](#using-tile-properties)


## Installation
1. **Install the latest version of [SMAPI](https://smapi.io/).**
2. **Download EMP** from [the Releases page on GitHub](https://github.com/Esca-MMC/EscasModdingPlugins/releases), Nexus Mods, or ModDrop.
3. **Unzip EMP** into the `Stardew Valley\Mods` folder.

Mods that use EMP should now work correctly. For information about creating mods, see the sections below.

### Multiplayer Note
* It is recommended that **all players** install this mod for multiplayer sessions.

## Features
EMP adds the following features:

Feature | Summary | Data assets | Map properties | Tile properties
--------|---------|-------------|----------------|----------------
Fish Locations | A location (map) can have multiple "zones" with different fish, including fish from other locations. (See the [Data/Locations](https://stardewvalleywiki.com/Modding:Location_data) asset.) | `Mods/Esca.EMP/FishLocations` | ✘ | `Esca.EMP/FishLocations`

### Fish Locations
This feature allows players to catch different groups of fish at a single in-game location (map). It gives custom maps more control over which fish are used from the [Data/Locations](https://stardewvalleywiki.com/Modding:Location_data) asset. It can also control whether crab pots catch "ocean" or "freshwater" animals from the [Data/Fish](https://stardewvalleywiki.com/Modding:Fish_data) asset.

Mods can use this feature through either data assets or tile properties. The asset will be checked first; if it has no data for a specific tile, the tile property will be used instead.

#### Using the data asset
EMP adds this data asset to the game: `"Mods/Esca.EMP/FishLocations"`

The asset can be edited with Content Patcher's "EditData" action like any other data asset. SMAPI (C#) mods can also edit the asset with "IAssetEditor" methods.

It supports the following fields:

Field | Value | Example | Required? | Description
------|-------|---------|-----------|------------
(entry key) | Any unique string | `"YourName.ExampleMod 1"` | Required | A unique key for this entry. Including your mod's [UniqueID](https://www.stardewvalleywiki.com/Modding:Modder_Guide/APIs/Manifest) is recommended.
Locations | A list of location names | `["Farm", "BusStop"]` | Required | A list of locations (maps) this entry will affect.
TileAreas | A list of tile areas | `[ {"X":0, "Y":0, "Width":999, "Height":999} ]` | Required | A list of tile areas this entry will affect. The earlier example will affect the entire map.
UseZone | Any integer | `-1` | Optional | Only fish with this "zone" ID in Data/Locations can be caught here. Note: Fish with `-1` can be caught in any zone.
UseLocation | A location name | `"Mountain"` | Optional | Fish caught here will come from this location's data. Refer to the keys (location names) in Data/Locations.
UseOceanCrabPots | true or false | `false` | Optional | Crab pots here will catch "ocean" results if this is true, or "freshwater" results if this is false.
Priority | Any integer | `0` | Optional | If a tile is affected by more than one entry, the entry will the highest priority will be used. 0 if not provided.

Below is an example content.json file for a Content Patcher mod. It modifies some areas of the Farm to catch fish from the Forest's river, and crab pots there will catch "freshwater" results.
```js
{
  "Format": "1.23.0",
  "Changes": [
    {
      "Action": "EditData",
      "Target": "Mods/Esca.EMP/FishLocations",
      "Entries": {
        "YourName.ExampleMod 1": {  /* give the entry a unique key */
        "Locations": [ "Farm" ],    /* modify the farm's fish */
          "TileAreas": [
            {"X":0, "Y":0, "Width": 50, "Height": 50}, /* modify fish from tiles 0,0 - 49,49 */
            {"X":80, "Y":90, "Width": 2, "Height": 4}  /* modify fish from tiles 80,90 - 81,93 */
          ],
          "UseLocation": "Forest",  /* use fish from the "Forest" data in Data/Locations */
          "UseZone": 0,             /* use fish set to either zone 0 (river) or -1 (everywhere) in Data/Locations */
          "UseOceanCrabPots": false /* use "freshwater" crab pot results from Data/Fish */
        }
      }
    }
  ]
}
```

#### Using tile properties
Fish locations can also be controlled by this tile property: `Esca.EMP/FishLocations`

The property's value uses this format: `<UseZone> [UseLocation] [UseOceanCrabPots]`

Field | Value | Example | Required? | Description
------|-------|---------|-----------|------------
UseZone | Any integer | `-1` | Required | Only fish with this "zone" ID in Data/Locations can be caught here. Note: Fish with `-1` can be caught in any zone.
UseLocation | A location name | `Mountain` | Optional | Fish caught here will come from this location's data. Refer to the keys (location names) in Data/Locations.
UseOceanCrabPots | true or false | `false` | Optional | Crab pots here will catch "ocean" results if this is true, or "freshwater" results if this is false.

Below is an example tile property where fish will be caught from the Forest's river and pots will catch "freshwater" results: `-1 Forest false`

![Esca.EMP/FishLocations: -1 Forest false](docs/images/FishLocations_TileProperty.png)
