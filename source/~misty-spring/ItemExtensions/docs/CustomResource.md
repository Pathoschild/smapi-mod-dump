**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Resources

Custom resources can be of two types: Nodes (like ore), or Resource Clumps.

For C# modders: By default, resources are treated as objects. Anything bigger than 1x1 tiles will behave as a Resource Clump.

## Contents

* [Format](#format)
  * [Main fields](#main-fields)
  * [Other fields](#other-fields)
    * [For item drops](#for-item-drops)
    * [For ore customization](#for-ore-customization)
    * [For EXP/stats](#for-expstats)
    * [Spawning in the mines](#spawning-in-the-mines)
* [Debris](#debris)
  * [Pre-defined](#pre-defined-debris-types)
  * [Use item texture](#item-debris)
  * [Custom animation (advanced)](#custom-animation-for-debris-advanced)
* [Adding to stats](#adding-to-stats)
* [Examples](#example)
* [Spawning](#spawning)

---

# Format

Custom resources are very extensive, with many customizable fields:

## Main fields

| name         | type                         | Required | description                                               |
|--------------|------------------------------|----------|-----------------------------------------------------------|
| Type         | `CustomResourceType`         | No       | Used for mod compat. Default Stone, can be Weeds or Wood. |
| Texture      | `string`                     | Yes      | The resource's texture path.                              |
| SpriteIndex  | `int`                        | Yes      | Index in texture to use.                                  |
| Width        | `int`                        | Yes      | Resource size.                                            |
| Height       | `int`                        | Yes      | Resource size.                                            |
| Health       | `int`                        | Yes      | Hits before breaking\*.                                   |
| Tool         | `string`                     | Yes      | Tool to use.\**                                           |
| ItemDropped  | `string`                     | No       | Item dropped.                                             |
| MinDrops     | `int`                        | No       | Min. amount dropped. If no max is set, always drops this. |
| ContextTags  | `List<string>`               | No       | Context tags.                                             |
| CustomFields | `Dictionary<string, string>` | No       | Custom Fields.                                            |


*= Every time the player hits, it reduces UpgradeLevel + 1 (e.g iridium pickaxe removes 5 HP).
For example: copper's health is 3, cinder shards' is 12, and radioactive ores have 25.

**= This can be any tool class (vanilla's classes can be seen in [Data/Tools](https://stardewvalleywiki.com/Modding:Migrate_to_Stardew_Valley_1.6#Custom_tools)), "any", or a Weapon type (e.g. Hammer, Sword, etc.).

## Other fields

### For item drops
| name          | type                         | Required | description                  |
|---------------|------------------------------|----------|------------------------------|
| MaxDrops      | `int`                        | No       | Max. amount dropped.         |
| ExtraItems    | `List<ExtraSpawn>`           | No       | Extra items to drop.         |
| AddHay        | `int`                        | No       | Hay to add.                  |
| SecretNotes   | `bool`                       | No       | If it can drop notes.        |

Fields for ExtraSpawns can be found [here](https://github.com/misty-spring/StardewMods/tree/main/ItemExtensions/docs/ExtraSpawns.md).

### For ore customization
| name          | type        | Required | description                               |
|---------------|-------------|----------|-------------------------------------------|
| Debris        | `string`    | No       | Debris to show on breaking.               |
| BreakingSound | `string`    | No       | Breaking sound.                           |
| Sound         | `string`    | No       | Sound on hit.                             |
| Shake         | `bool`      | No       | Whether to shake on hit.                  |
| MinToolLevel  | `int`       | No       | Minimum level.                            |
| ImmuneToBombs | `bool`      | No       | If this ore shouldn't be harmed by bombs. |
| Light         | `LightData` | No       | Item light.                               |  

Fields for LightData can be found [here](https://github.com/misty-spring/StardewMods/tree/main/ItemExtensions/docs/LightData.md).

### For EXP/stats

| name          | type                         | Required | description                  |
|---------------|------------------------------|----------|------------------------------|
| CountTowards  | `StatCounter`                | No       | Stat to count towards\*\*\*. |
| Exp           | `int`                        | No       | Exp to add on break.         |
| Skill         | `string`                     | No       | Skill to add EXP for.        |


*** = The possible stats are [here](#adding-to-stats).

### Spawning in the mines

You can make mine spawns with `MineSpawns` (which is a `List<MineSpawn>`):

| name                     | type     | Required | description                                                                                                                |
|--------------------------|----------|----------|----------------------------------------------------------------------------------------------------------------------------|
| Floors                   | `string` | Yes      | Floors to spawn in. Must be slash separated. For example, "5, 7/9, 13/25" will spawn it on floors 5, 7 to 9, and 13 to 25. |
| Condition                | `string` | No       | A GSQ for these spawns to apply. (optional) Calculated on mine generation.                                                 |
| Type                     | `string` | No       | Can be: `All` (always spawns in mines),  `Qi` (danger mines only), `Normal` (regular mines only), `Volcano`, `Mountain`, or `Frenzy` (similar to mushroom levels) . Default "All"                           |
| SpawnFrequency           | `double` | No       | How likely it is for this ore to appear in the mines. **Goes from 0 to 1** (e.g, 0.8 = 80%)					                                                                |
| AdditionalChancePerLevel | `double` | No       | Extra chance to spawn per mine level                  					                                                                |

Note: To spawn until infinite floors, use `"<desired floor>/-999"` E.g., "150/-999".

So, for example:
```jsonc
"MineSpawns":[
  {
    "Floors": "121/-999", //from skull cavern 1 until infinty
    "Condition": "PLAYER_HAS_MAIL Current SomeCustomFlag", //optional, only if you want it to use extra conditions
    "Type":"Qi" //only if the mine is in hardmode
  }
]
```

## Debris

The debris can be one of three types: pre-defined, an object, or custom (advanced).

### Pre-defined debris types
- coins
- stone
- wood
- boulder\*
- stump\*

\* = These are the "big" versions of stone and wood. On breaking, they'll show an animation of the stump/boulder cracking apart.

You can apply a tint to pre-defined debris. Just set it after the debris type: e.g, `"Debris": "stone lightpurple""`.

### Item debris

These debris can be any item with qualified Id- it'll be shown in "chunks" like when you eat an item.
For example: `"Debris":"(O)40"`.

### Custom animation for debris (advanced)

You can make debris be a custom animation instead. For this, you must set the debris type as "custom", followed by the parameters:

`custom <tint> <texturepath> <x> <y> <width> <height> <frames> [speed] [alphaFade]`

parameters between `<>` are obligatory. Those in `[]` are optional.

Example:

`"Debris" : "custom white Mods/MyMod/CustomFile 0 0 16 16 5"`

Explanation:
- The tint will be applied over the sprite. it can be a color name (`red`, `lightgreen`), a hex (`#123456`), or "white" to not apply any.
- The framework will search for the file `Mods/MyMod/CustomFile` (you must load it to the game).
- It will start at 0,0, and have size 16x16.
- It will use 5 frames, starting from the position you set

## Adding to stats

You can add to a few stats on breaking a resource.
These are the possible types:


| name            | description                                                 |
|-----------------|-------------------------------------------------------------|
| None            | Default.                                                    |
| Copper          | Copper found.                                               |
| Diamonds        | Diamonds found.                                             |
| GeodesBroken    | Geodes crushed.                                             |
| Gold            | Gold found.                                                 |
| Iridium         | Iridium found.                                              |
| Iron            | Iron found.                                                 |
| MysticStones    | Mystic stones crushed.                                      |
| OtherGems       | Precious gems found (neither diamonds nor prismatic shards) |
| PrismaticShards | Prismatic shards found.                                     |
| Stone           | Stone gathered                                              |
| Stumps          | Wood stumps chopped.                                        |
| Seeds           | Seeds sown.                                                 |
| Weeds           | Weeds cut.                                                  |

## Example

Here is an example of a custom ore.

```jsonc
{
  "Action": "EditData",
  "Target": "Mods/mistyspring.ItemExtensions/Resources",
  "Entries": {
    "MyGemStone": {
      "Texture": "Mods\\MyMod\\Objects",
      "SpriteIndex": 1,
      "Health": 10,
      "Sound": "hammer",
      "BreakingSound": "stoneCrack",
      "Debris": "stone",
      "ItemDropped": "(O)82",
      "Tool": "Pickaxe"
      //(..etc)
    }
  }
}
```

## Spawning

You can spawn either from vanilla [Location Data](https://stardewvalleywiki.com/Modding:Location_data), or via FTM (only accepts 1x1 resources).

If spawning via FTM, set the resource under forage- the mod will figure out tool actions on its own.

If spawning anything bigger than 1x1, the forage Id must start with `ItemExtension.Clump`, like this:

```jsonc
{
  "Action": "EditData",
  "Target": "Data/Locations",
  "Entries": {
    "MyCustomLocation": {
      // (... location data)
      "Forage": [
        {
          "Chance": 0.9,
          "Precedence": 0,
          "Id": "ItemExtension.Clump 1",
          "ItemId": "MyClumpId",
          "MaxItems": 1
        },
        {
          "Chance": 0.9,
          "Precedence": 0,
          "Id": "Node",
          "ItemId": "MyGemStone",
          "MaxItems": 1
        }
      ]
    }
  }
}
```

### Spawn rectangle for clumps

To define a spawn rectangle for clumps, add this to custom fields:

```
"CustomFields":{
  "mistyspring.ItemExtensions/ClumpSpawnRect":"x y width height"
  }
```

For example

```
"CustomFields":{
  "mistyspring.ItemExtensions/ClumpSpawnRect":"1 12 22 8"
  }
```

Here, clumps will only spawn between 1,12 and 23,20.
