**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Custom panning drops

The mod allows to add more "results" to panning.

## Contents

* [How to add](#how-to-add)
    * [Format](#format)
    * [Example](#example-1)

---

## How to add

You can add panning items by editing `Mods/mistyspring.ItemExtensions/Panning` . This uses the same fields as [ExtraSpawn](https://github.com/misty-spring/StardewMods/blob/main/ItemExtensions/docs/ExtraSpawns.md), in addition you can state a `MinUpgrade` or `MaxUpgrade`. (For example, MinUpgrade 0 is the regular copper pan.)

### Format

PanningData has these fields:

| name        | type                  | Required | description                                  |
|-------------|-----------------------|----------|----------------------------------------------|
| ItemId      | `string`              | Yes      | Qualified item Id.                           |
| Chance      | `double`              | Yes      | Spawn chance: e.g, 0.5 for 50%.              |
| MinUpgrade  | `int`                 | No       | Minimum upgrade level. Default 0 (Copper)    |
| MaxUpgrade  | `int`                 | No       | Maximum upgrade level. Default -1 (No limit) |
| Condition   | `string`              | No       | Game State Query.                            |
| AvoidRepeat | `bool`                | No       | Advanced, for `ISpawnItemData`.              |
| Filter      | `ItemQuerySearchMode` | No       | Advanced, for `ISpawnItemData`.              |

Asides from this, they **also** have the [same fields as any spawnable item](https://stardewvalleywiki.com/Modding:Item_queries#Item_spawn_fields).

### Example

In this example, we add a new panning drop.

This will add MyCustomItem as a panning drop, with a 5% chance of being dropped.

```jsonc
{
  "Action": "EditData",
  "Target": "Mods/mistyspring.ItemExtensions/Panning",
  "Entries": {
    "CustomIdForMyDrop": { //this is an ID, used in case the patch is buggy (so you know which one to check)
      "ItemId":"(O)MyCustomItem" //can be an item of any type, e.g (BC)CustomBigCraftable, etc
      "Chance": "0.05" //0.5%, the chance goes from 0 to 1
    }
  }
}
```
