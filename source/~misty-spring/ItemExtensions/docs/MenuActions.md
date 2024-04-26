**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Menu actions

Menu actions happen by making two items interact. This is made via the menu: you hold an item, and right click another.

If your held item has "custom behavior" for the clicked one, it'll be called.

------------------------------

## Contents
* [How to use](#how-to-use)
* [Parameters](#parameters)
* [Example](#example)


## Parameters
| name              | type           | required | description                                                          |
|-------------------|----------------|----------|----------------------------------------------------------------------|
| TargetId          | `string`       | true     | The item to affect. Uses qualified item ID.                          |
| RemoveAmount      | `int`          | false    | How many to remove (per item replaced). Default 0                    |
| ReplaceBy         | `string`       | false    | The item to replace with. Uses qualified item ID.                    |
| RetainAmount      | `bool`         | false    | Whether to keep the item stack when replacing. Default true          |
| RetainQuality     | `bool`         | false    | Whether to keep the item quality when replacing. Default true        |
| AddContextTags    | `List<string>` | false    | Context tags to add.                                                 |
| RemoveContextTags | `List<string>` | false    | Context tags to remove.                                              |
| QualityChange     | `string`       | false    | How the quality should be changed. (E.g +1)                          |
| PriceChange       | `string`       | false    | How the price should be changed. (E.g 5x for 5 times original price) |
| TextureIndex      | `int`          | false    | Changes the index in spritesheet.                                    |
| PlaySound         | `string`       | false    | Plays a sound.                                                       |
| TriggerActionID   | `string`       | false    | Trigger action to call.                                              |
| Conditions        | `string`       | false    | Game state query to check.                                           |

## How to use

Edit the item's `Mods/mistyspring.dynamicdialogues/MenuActions` file.
The key is to identify your mod's patch (in case of failure), and the value is the item action.

## Example

Here, we add behavior for the lava katana.
When you click a large brown egg with it, the egg will turn into an omelet.

This will happen for all eggs in the stack. It will also play a fire sound.

```
{
    "Action": "EditData",
    "Target": "Mods/mistyspring.ItemExtensions/MenuActions/(W)9",
    "Entries": {
        "nameOfMyPatch":
        {
            "TargetId": "(O)182", //egg
            "ReplaceBy": "(O)195", //omelet
            "RetainAmount": true,
            "RetainQuality": false,
            "PlaySound": "fireball"
        }
    }
}
```