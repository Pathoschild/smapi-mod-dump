**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Terrain Features in mineshaft
You can add different terrain features for the mineshaft.

## Format

All terrain features have the following format.

Depending on the type, they also have unique options.


| name | type | required | description |
|------|------|----------|-------------|
| TerrainFeatureId | `string` | Yes | The ID for the terrain feature.|
| FeatureType | `Enum` | Yes | Can be `Tree`, `FruitTree`, or `GiantCrop`. |
| Health | `int` | Yes | Health (for chopping down).|
| MineSpawns | `MineSpawns`| Yes | Levels to spawn in. |

**NOTE**: Spawning terrain features only works inside the mineshaft and skull cavern. Other options won't work.

### Wild Trees
| name | type | required | description |
|------|------|----------|-------------|
| GrowthStage | `int` | Yes | Tree's growth stage: 0 seed, 1 sprout, 2 sapling, 3 bush, 5 tree. |
| MossChance | `double` | Yes | Chance for this tree to have moss.
| Stump | `bool` | No | If the tree should be a stump.

### Fruit trees
| name | type | required | description |
|------|------|----------|-------------|
| GrowthStage | `int` | Yes | Tree's growth stage: 0 seed, 1 sprout, 2 sapling, 3 bush, 5 tree. |
| FruitAmount | `int` | No | How many fruits it should have. Default 3.


## Example

```jsonc
{
  "Action": "EditData",
  "Target": "Mods/mistyspring.ItemExtensions/Mines/Terrain",
  "Entries": {
    "OakTreeSpawn": {
        //This would be the entry "1" (Oak tree) in Data/WildTrees
        "TerrainFeatureId": "1",
        "FeatureType": "Tree",
        "MossChance": 0.1, //10% chance it'll spawn with moss
        "Stump": false, //not a stump
        "MineSpawns":[
        {
            "Floors": "121/-999", //from skull cavern 1 until infinty
            "Condition": "PLAYER_HAS_MAIL Current SomeCustomFlag", //optional, only if you want it to use extra conditions
            "Type":"Normal" //only if the mine is in normal mode
        }
      ]
    },
    "FruitTreeSpawn": {
        //This would be the entry "628" (Cherry tree) in Data/FruitTrees
        "TerrainFeatureId": "628",
        "FeatureType": "FruitTree",
        "FruitAmount": 2, //only 2 fruits
        "GrowthStage": "5", //fully grown
        "MineSpawns":[
        {
            "Floors": "1/100", //from level 1 to 100
            "Type":"All" //both in qi and normal mines
        }
      ]
    },
    "GiantCropSpawn": {
        //This would be the entry "Cauliflower" in Data/GiantCrops
        "TerrainFeatureId": "Cauliflower",
        "FeatureType": "GiantCrop",
        "MineSpawns":[
        {
            "Floors": "50/110", //from level 50 to 110
            "Type":"Qi" //only during qi missions
        }
      ]
    }
  }
}
```