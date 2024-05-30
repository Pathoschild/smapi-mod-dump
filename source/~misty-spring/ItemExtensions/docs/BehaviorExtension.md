**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Other extensibility
These changes are made to the file `Mods/mistyspring.ItemExtensions/Data`, and use the qualified item Id.

## Contents

* [Options](#options)
* [Force quality](#force-quality)
* [Max stack change](#max-stack-change)
* [Held over head](#held-over-head)
* [Item light](#light)
* [OnBehavior](#on-behavior)
  * [Accepted behavior types](#accepted-types)
  * [Format](#format)
* [Examples](#examples)

--------------------

## Options

Item extensibility is added in the mod's /Data file, and has the following options:

| name         | type              | required | description                                |
|--------------|-------------------|----------|--------------------------------------------|
| MaximumStack | `int`             | No       | Maximum item stack.                        |
| HideItem     | `bool`            | No       | Disable showing item above head (on held.) |
| Light        | `LightData`       | No       | Item's LightData.                          |
| OnEquip      | `OnBehavior`\*    | No       | Action to do on equipping item.            |
| OnUnequip    | `OnBehavior`      | No       | Action to do on unequipping item.          |
| OnUse        | `OnBehavior`      | No       | Action on using item.                      |
| OnDrop       | `OnBehavior`      | No       | Action on dropping item.                   |
| OnPurchase   | `OnBehavior`      | No       | Action on purchasing.                      |
| OnAttached   | `OnBehavior`      | No       | Action on attaching to a tool.             |
| OnDetached   | `OnBehavior`      | No       | Action on detaching from tool.             |
| Eating       | `FarmerAnimation` | No       | Custom eating animation.                   |
| AfterEating  | `FarmerAnimation` | No       | Animation to play after eating.            |


\* = For OnBehavior fields, [see here](#on-behavior).

## Force quality

You can force item quality- anytime you get the item, it'll have the quality.
Just add this to your Object's `CustomFields`:

```
"mistyspring.ItemExtensions/ForceQuality":"quality"
```

The quality can be "none", "silver", "gold" or "iridium"

## Max stack change

By default, objects can stack up to 999 (while others, like tools, only stack up to 1). This mod lets you override that
You can do it in two ways: Via custom fields, or mod data.

### Via custom fields

Just add this to your Item's `CustomFields`:

```
"mistyspring.ItemExtensions/MaximumStack":"123" //replace 123 for desired quanity
```

### Via mod

This is useful if you're making multiple changes to the item.

Edit `Mods/mistyspring.ItemExtensions/Data`, and add the field to the mod:
```jsonc
{
  "Action": "EditData",
  "Target": "Mods/mistyspring.ItemExtensions/Data",
  "Entries": {
    "(O)CustomObject": {
      "MaximumStack": 123
      //(..etc of changes)
    }
  }
}
```

## Held over head

By default, objects are held over head. This mod lets you disable it. You can do it in two ways: Via custom fields, or mod data.

### Via custom fields

Just add this to your Item's `CustomFields`:

```
"mistyspring.ItemExtensions/ShowAboveHead":"false"
```

### Via mod

This is useful if you're making multiple changes to the item.

Edit `Mods/mistyspring.ItemExtensions/Data`, and add the field to the mod:
```jsonc
{
  "Action": "EditData",
  "Target": "Mods/mistyspring.ItemExtensions/Data",
  "Entries": {
    "(O)CustomObject": {
      "HideItem": true
      //(..etc of changes)
    }
  }
}
```

## Light
You can add light to non-torch items, by editing its "Light" field in `Mods/mistyspring.ItemExtensions/Data`.
For light parameters, [see here](https://github.com/misty-spring/StardewMods/tree/main/ItemExtensions/docs/LightData.md).

## On behavior

You can make many things occur when the item fits criteria (e.g was equipped, unequipped, sold, etc).

### Accepted behavior types

You can use any of these behaviors: `OnEquip`, `OnUnequip`, `OnUse`, `OnDrop`, `OnPurchase`.

### Format

**All fields are optional.**
Behaviors follow the following format:

| name               | type                      | description                                          |
|--------------------|---------------------------|------------------------------------------------------|
| Message            | `string`                  | Message to show.                                     |
| Confirm            | `string`                  | Requires `Message`. Option to confirm this behavior. |
| Reject             | `string`                  | Requires `Message`. Option to reject this behavior.  |
| ReduceBy           | `int`                     | Reduce the item by this count.                       |
| PlaySound          | `string`                  | Plays a sound.                                       |
| ChangeMoney        | `string`                  | Changes money\*.                                     |
| Health             | `string`                  | Changes health\*.                                    |
| Stamina            | `string`                  | Changes stamina\*.                                   |
| AddItems           | `Dictionary<string, int>` | Adds the following items (uses Qualified Id).        |
| RemoveItems        | `Dictionary<string, int>` | Removes the following items (uses Qualified Id).     |
| PlayMusic          | `string`                  | Changes map track to this one.                       |
| AddQuest           | `string`                  | Adds a quest.                                        |
| AddSpecialOrder    | `string`                  | Adds a special order.                                |
| RemoveQuest        | `string`                  | Removes a quest                                      |
| RemoveSpecialOrder | `string`                  | Removes a special order.                             |
| Condition          | `string`                  | Condition for this behavior to trigger.              |
| TriggerAction      | `string`                  | Trigger action to call.                              |
| ShowNote           | `NoteData`                | (ADVANCED) Shows a note.\**                          |

\*= These use a special format: set/add, followed by amount. e.g, `add 50` to add 50 to current value
\**= This is advanced: The fields for NoteData are [found here]().

## Examples

This is an example from "Start in Ginger Island". Here, if you buy a copper axe while in the island, the normal one will be removed from your inventory.

```jsonc
{
  "Action": "EditData",
  "Target": "Mods/mistyspring.ItemExtensions/Data",
  "Entries": {
    "(T)CopperAxe": {
      "OnPurchase": {
        "RemoveItems": {
          "(T)Axe": 1
        },
        "Condition": "PLAYER_LOCATION_CONTEXT Current Island"
      }
    }
  }
}
```