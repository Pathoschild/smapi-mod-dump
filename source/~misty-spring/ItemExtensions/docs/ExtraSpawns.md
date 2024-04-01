**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Extra Spawns

# Contents

* [Format](#format)
* [Examples](#examples)

--------------------

## Format

ExtraSpawn has the following fields:


| name        | type                  | Required | description                          |
|-------------|-----------------------|----------|--------------------------------------|
| ItemId      | `string`              | Yes      | Qualified item Id.                   |
| Chance      | `double`              | No       | Spawn chance: e.g, 0.5 for 50%.      |
| Condition   | `string`              | No       | Game State Query.                    |
| AvoidRepeat | `bool`                | No       | Advanced, use with `ISpawnItemData`. |
| Filter      | `ItemQuerySearchMode` | No       | Advanced, use with `ISpawnItemData`. |

Asides from this, they **also** have the [same fields as any spawnable item](https://stardewvalleywiki.com/Modding:Item_queries#Item_spawn_fields).


## Examples


```jsonc
[
  {
    "ItemId": "330",              //clay
    "Chance": 1                   //100% of spawn
  },
  {
    "ItemId": "(O)168",           //trash
    "Chance": 0.3,
    "Condition": "SEASON summer"  //only in summer
  }
]
```