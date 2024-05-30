**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Extra fishing treasure

Extra fishing treasure can be customized by patching `Mods/mistyspring.ItemExtensions/Treasure`

# Contents

* [Format](#format)
* [Examples](#examples)

--------------------

## Format

Custom treasures have the same fields as [ExtraSpawns](https://github.com/misty-spring/StardewMods/blob/main/ItemExtensions/docs/ExtraSpawns.md).

Just like ExtraSpawns, they **also** have the [same fields as any spawnable item](https://stardewvalleywiki.com/Modding:Item_queries#Item_spawn_fields).


## Example

Here, we add a new treasure drop.
Every time a treasure box is opened, there'll be a 5% chance for `(O)SomeItem` to be added to the loot.

```jsonc
{
  "Action": "EditData",
  "Target": "Mods/mistyspring.ItemExtensions/Treasure",
  "Entries": {
    "MyCustomLoot": {
        "ItemId": "(O)SomeItem",
        "Chance": 0.05,
        "MinStack": 2,
        "MaxStack": 4
    }
  }
}
```
