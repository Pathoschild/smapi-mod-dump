**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Extra train drops

Extra train drops are calculated when a train passes by the valley.
Every four seconds, it checks every visible train cart and calculates possible drops.

# Contents

* [Format](#format)
* [Examples](#examples)

--------------------

## Format

Train drops have the same fields as [ExtraSpawns](https://github.com/misty-spring/StardewMods/blob/main/ItemExtensions/docs/ExtraSpawns.md), and a few train-specific options:


| name   | type                  | Required | description                                                                                                                           |
|--------|-----------------------|----------|---------------------------------------------------------------------------------------------------------------------------------------|
| Car    | `string`              | Yes      | Type of car: can be `Plain`, `Resource`, `Passenger` or `Engine`.                                                                     |
| Type\* | `string`              | Yes      | What the train carries: can be `Coal`, `Metal`, `Wood`, `Compartments`, `Grass`, `Hay`, `Bricks`, `Rocks`, `Packages`, or `Presents`. |

*=If Car **isn't** `Resource`, set Type to `None`.

Just like ExtraSpawns, they **also** have the [same fields as any spawnable item](https://stardewvalleywiki.com/Modding:Item_queries#Item_spawn_fields).


## Example

Here, we add a new drop for trains.
Every time a train passes by, carts with hay will have a 10% to drop (O)SomeItem.

```jsonc
{
  "Action": "EditData",
  "Target": "Mods/mistyspring.ItemExtensions/Train",
  "Entries": {
    "MyCustomDrop": {
        "Car": "Resource",
        "Type": "Hay",
        "ItemId": "(O)SomeItem",
        "Chance": 0.1
    }
  }
}
```
