**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/zombifier/My_Stardew_Mods**

----

# Extra Machine Configuration Framework

[Extra Machine Config](https://www.nexusmods.com/stardewvalley/mods/22256)
is a Stardew Valley mod that adds extra functionalities to Content Patcher
machine recipe definitions, and allows modders to define recipes that can do
things beyond what's possible in the base game (e.g per-recipe fuel).

As of 1.5.0, this mod also adds a bunch of miscellaneous features not strictly
related to machines.

This document is for modders looking to incorporate this mod into their own
content packs. For users, install the mod as usual from the link above.

## Table of Contents
- [Extra Machine Configuration Framework](#extra-machine-configuration-framework)
   * [Table of Contents](#table-of-contents)
   * [Item Features](#item-features)
      + [Draw smoke particles around item](#draw-smoke-particles-around-item)
      + [Draw an item's preserve item's sprite instead of its base sprite](#draw-an-items-preserve-items-sprite-instead-of-its-base-sprite)
      + [Define extra loved items for Junimos](#define-extra-loved-items-for-junimos)
      + [Append extra context tags to shop and machine item queries](#append-extra-context-tags-to-shop-and-machine-item-queries)
   * [Machine Features](#machine-features)
      + [Adding additional fuel for a specific recipe](#adding-additional-fuel-for-a-specific-recipe)
      + [Output inherit the flavor of input items](#output-inherit-the-flavor-of-input-items)
      + [Output inherit the dye color of input items](#output-inherit-the-dye-color-of-input-items)
      + [Specify range of input count and scale output count with the input amount consumed](#specify-range-of-input-count-and-scale-output-count-with-the-input-amount-consumed)
      + [Adding extra byproducts for machine recipes](#adding-extra-byproducts-for-machine-recipes)
      + [Generate nearby flower-flavored modded items (or, generate flavored items outside of machines)](#generate-nearby-flower-flavored-modded-items-or-generate-flavored-items-outside-of-machines)
      + [Override display name if the output item is unflavored](#override-display-name-if-the-output-item-is-unflavored)
      + [Generate an input item for recipes that don't have any, and use 'nearby flower' as a possible query](#generate-an-input-item-for-recipes-that-dont-have-any-and-use-nearby-flower-as-a-possible-query)

## Item Features

### Draw smoke particles around item

Items with the context tag `smoked_item` will have its sprite darkened and
have smoke particles drawn around it like smoked fish.

### Draw an item's preserve item's sprite instead of its base sprite

Items with the context tag `draw_preserve_sprite` will have its sprite be
the sprite of its `preservedParentSheetIndex` item instead (if set).

With `smoked_item` and `draw_preserve_sprite` combined, you can implement
custom smoked item similar to smoked fish without having to output a
`(O)SmokedFish` item, such as a smoked egg item that uses the sprite of the egg
used to make it albeit dark and smoking.

More item effects aside from smoke might come in the future.

### Define extra loved items for Junimos

Items with the context tag `junimo_loved_item` can be fed to junimos to improve
their harvest rate just like raisins.

### Append extra context tags to shop and machine item queries

Set the following field in the [item query's `ModData`
field](https://stardewvalleywiki.com/Modding:Item_queries#Item_spawn_fields).

| Field Name                         | Description              |
| ---------------------------------- | ------------------------ |
| `selph.ExtraMachineConfig.ExtraContextTags` | A comma-separated list of extra context tags for the item spawned by this item query.|

Important notes:

* This feature can be used anywhere item queries are used, such as machines or shops.
* If you're using this field, it's highly recommended you also set the
  `ObjectInternalName` field (and optionaly the display name) so the spawned
  items do not stack with other items of the same ID that may not have this
  field, causing the context tags to be lost.

For an example, scroll down to the example for additional fuels for machine recipes.

----

## Machine Features
Unless otherwise specified, this mod reads extra data defined the [`CustomData`
field in
`OutputItem`](https://stardewvalleywiki.com/Modding:Machines#Item_processing_rules),
which is a map of arbitrary string keys to string values intended for mod use.
Since `CustomData` is per-output, it's possible to specify different settings
for each recipe, or even each output in the case of multiple possible outputs.

### Adding additional fuel for a specific recipe

| Field Name                         | Description              |
| ---------------------------------- | ------------------------ |
| `selph.ExtraMachineConfig.RequirementId.1`<br>`selph.ExtraMachineConfig.RequirementTags.1` | The additional fuel that should be consumed by this recipe in addition to the ones specified in the machine's `AdditionalConsumedItems` field.<br> You can specify multiple fuels by adding another field with the same name, but with the number at the end incremented (eg. `ExtraMachineConfig.RequirementId.2`).<br> `RequirementId` allows specifying by qualified ID for a specific item, or a category ID for only categories (eg. `-2` will consume any gemstones as fuel), while `RequirementTags` allow specifying a comma-separated list of tags that must all match.<br>**CURRENT LIMITATION**: Both `RequirementId` and `RequirementTags` currently cannot be used for the same fuel number. If you need to specify both, add the item ID to the tag list (e.g. `"id_(o)itemid"`).|
| `selph.ExtraMachineConfig.RequirementCount.1` | The count of the additional fuel specified in the field above. Defaults to 1 if not specified. |
| `selph.ExtraMachineConfig.RequirementInvalidMsg` | The message to show to players if all the requirements are not satisfied. Note that if there are multiple output rules with this field for the same input item, only the first one will be shown.|

#### Example

The example below adds a new recipe to furnaces that accepts 1 diamond, any 5
ores, and any milk item (in addition to the 1 coal required by the base
machine) and returns 4 iridium-quality diamonds if iridium ore was used, 3
gold-quality diamonds if gold ore was used, 2 silver-quality for iron, and 1
normal for copper.

<details>

<summary>Content Patcher definition</summary>

```
{
  "Changes": [
    {
      "LogName": "Add Diamond Milk Polishing (what) to Furnace Rules",
      "Action": "EditData",
      "Target": "Data/Machines",
      "TargetField": ["(BC)13", "OutputRules"],
      "Entries": {
        "PurifyDiamond": {
          "Id": "PurifyDiamond",
          "Triggers": [
            {
              "Id": "ItemPlacedInMachine",
              "Trigger": "ItemPlacedInMachine",
              "RequiredItemId": "(O)72",
              "RequiredCount": 1,
            }
          ],
          "UseFirstValidOutput": true,
          "OutputItem": [
            {
              "CustomData": {
                "selph.ExtraMachineConfig.RequirementId.1": "(O)386",
                "selph.ExtraMachineConfig.RequirementCount.1": "5",
                "selph.ExtraMachineConfig.RequirementInvalidMsg": "Need 5 ores and milk",
                "selph.ExtraMachineConfig.RequirementId.2": "-6",
              },
              "ItemId": "(O)72",
              "MinStack": 4,
              "Quality": 3,
              "ModData": {
                "selph.ExtraMachineConfig.ExtraContextTags": "milk_polished,milk_polished_iridium"
              },
            },
            {
              "CustomData": {
                "selph.ExtraMachineConfig.RequirementId.1": "(O)384",
                "selph.ExtraMachineConfig.RequirementCount.1": "5",
                "selph.ExtraMachineConfig.RequirementId.2": "-6",
              },
              "ItemId": "(O)72",
              "MinStack": 3,
              "Quality": 2,
              "ModData": {
                "selph.ExtraMachineConfig.ExtraContextTags": "milk_polished,milk_polished_gold"
              },
            },
            {
              "CustomData": {
                "selph.ExtraMachineConfig.RequirementId.1": "(O)380",
                "selph.ExtraMachineConfig.RequirementCount.1": "5",
                "selph.ExtraMachineConfig.RequirementId.2": "-6",
              },
              "ItemId": "(O)72",
              "MinStack": 2,
              "Quality": 1,
              "ModData": {
                "selph.ExtraMachineConfig.ExtraContextTags": "milk_polished,milk_polished_iron"
              },
            },
            {
              "CustomData": {
                "selph.ExtraMachineConfig.RequirementId.1": "(O)378",
                "selph.ExtraMachineConfig.RequirementCount.1": "5",
                "selph.ExtraMachineConfig.RequirementId.2": "-6",
              },
              "ItemId": "(O)72",
              "MinStack": 1,
              "Quality": 0,
              "ModData": {
                "selph.ExtraMachineConfig.ExtraContextTags": "milk_polished,milk_polished_copper"
              },
            },
          ],
          "MinutesUntilReady": 10,
        },
      },
    },
  ]
}
```
</details>

----

### Output inherit the flavor of input items

**NOTE**: This feature is deprecated in the upcoming Stardew Valley 1.6.9, which supports this feature natively.

| Field Name                         | Description              |
| ---------------------------------- | ------------------------ |
| `selph.ExtraMachineConfig.InheritPreserveId` | When set to any value, copies the input item's flavor (e.g. the "Blueberry" part of "Blueberry Wine") into the output item.|

NOTE: Version 1.0.0 also supports this functionality via setting the
`PreserveId` field to `"INHERIT"`. This is deprecated and might no longer work in later versions.

#### Example

The example below modifies the base game's honey to mead recipe to also copy the
honey's flower flavor to the mead, and increment its price accordingly.

<details>

<summary>Content Patcher definition</summary>

```
{
  "Changes": [
    {
      "LogName": "Modify Mead Rules",
      "Action": "EditData",
      "Target": "Data/Machines",
      "TargetField": ["(BC)12", "OutputRules", "Default_Honey", "OutputItem", "(O)459"],
      "Entries": {
        "CustomData": {
          "selph.ExtraMachineConfig.InheritPreserveId": "true",
          // See below for that this does
          "selph.ExtraMachineConfig.UnflavoredDisplayNameOverride": "{{i18n: selph.FlavoredMead.WildMead.name}}"
        },
        "CopyPrice": true,
        "ObjectInternalName": "{0} Mead",
        // See https://stardewvalleywiki.com/Modding:Item_queries#Item_spawn_fields
        "ObjectDisplayName": "[LocalizedText Strings\\Objects:selph.FlavoredMead.name %PRESERVED_DISPLAY_NAME]",
        "PriceModifiers": 
        [
          {
            "Modification": "Add",
            "Amount": 100
          },
          {
            "Modification": "Multiply",
            "Amount": 2
          }
        ],
      },
    },
  ]
}
```

</details>

----

### Output inherit the dye color of input items

**NOTE**: This feature is deprecated in the upcoming Stardew Valley 1.6.9, which supports this feature natively.

| Field Name                         | Description              |
| ---------------------------------- | ------------------------ |
| `selph.ExtraMachineConfig.CopyColor` | When set to any value, copies the input item's color into the output item.<br>The difference between this settings and the base game's `CopyColor` is that the latter only supports copying the color of colored items (eg. flowers), while the former will copy the dye color if the input is not a colored object.<br>Make sure the output item's next sprite is a monochrome color sprite, otherwise the coloring might look weird.|

#### Example

The example below adds a new recipe to preserves jar that turns a gemstone into
5 fairy roses of the gem's color. Without this mod enabled, the roses will all
be of the base color, even with `CopyColor` set.

<details>

<summary>Content Patcher definition</summary>

```
{
  "Changes": [
    {
      "LogName": "Add Gemstone To Rose Rule",
      "Action": "EditData",
      "Target": "Data/Machines",
      "TargetField": ["(BC)15", "OutputRules"],
      "Entries": {
        "RoseMaker": {
          "Id": "RoseMaker",
          "Triggers": [
            {
              "Id": "ItemPlacedInMachine",
              "Trigger": "ItemPlacedInMachine",
              "RequiredTags": ["category_gem"],
              "RequiredCount": 1,
            }
          ],
          "OutputItem": [
            {
              "CustomData": {
                "selph.ExtraMachineConfig.CopyColor": "true",
              },
              "ItemId": "(O)595",
              "MinStack": 5,
              // This does nothing
              "CopyColor": true,
            },
          ],
          "MinutesUntilReady": 10,
        },
      },
    },
  ]
}
```

</details>

----

### Specify range of input count and scale output count with the input amount consumed

| Field Name                         | Description              |
| ---------------------------------- | ------------------------ |
| `selph.ExtraMachineConfig.RequiredCountMax` | When set to an int (as a string), the primary input item's count can be between the min value of the trigger rule's `RequiredCount`, or the max value as specified by this field.<br>The output item's stack count will be set to equal the amount of input item consumed, and `MinStack` and `MaxStack` will be ignored. To modify or the stack count, use `StackModifiers` and `StackModifiersMode`.<br>The required fuels (either via `AdditionalConsumedItems` or this mod's per-recipe fuels) will remain the same regardless of how many input items are consumed. For the fuel added by this mod, if you want the amount consumed to depend on the amount of input items consumed, make multiple output rules conditioned on the input item's stack size.|

Note that this functionality is completely achievable with vanilla machine
rules, using `RequiredCount` and output rules condition. This macro simply
reduces repetition, and is not as customizable as actual machine rules.

#### Example

The example below modifies the vanilla game's wine recipe to be able to process up to 10 fruits at a time, and produce a wine for every fruit used. If less than 10 fruits are used, only that amount of fruit will be processed.

<details>

<summary>Content Patcher definition</summary>

```
{
  "Changes": [
    {
      "LogName": "Modify Wine Rules",
      "Action": "EditData",
      "Target": "Data/Machines",
      "TargetField": ["(BC)12", "OutputRules", "Default_Wine", "OutputItem", "Default"],
      "Fields": {
        "CustomData": {
          "selph.ExtraMachineConfig.RequiredCountMax": "10",
        },
      },
    },
  ]
}
```

</details>

----

### Adding extra byproducts for machine recipes

First, write your extra output item queries to a new asset named
`selph.ExtraMachineConfig/ExtraOutputs`. This asset is a map of unique IDs to
item queries similar to the ones used in the recipe's `OutputItem` field. Most
machine-related features (e.g. `CopyColor`, `CopyQuality`, this mod's
`CustomData` fields, etc.) will be supported in these item queries.

Then, set this field in the actual machine output's `CustomData` dict as usual:

| Field Name                         | Description              |
| ---------------------------------- | ------------------------ |
| `selph.ExtraMachineConfig.ExtraOutputIds` | A comma-separated list of item query IDs written to the asset above to also spawn with this output item.|

#### Example

This example modifies the vanilla fruit to wine keg recipe to also spawn fruit-flavored jelly and mead alongside the wine item.

<details>

<summary>Content Patcher definition</summary>

```
{
  "Changes": [
    {
      "LogName": "Add Jelly&Mead Extra Outputs",
      "Action": "EditData",
      "Target": "selph.ExtraMachineConfig/ExtraOutputs",
      "Entries": {
        "JellyExtra": {
          "Id": "JellyExtra",
          "ItemId": "FLAVORED_ITEM Jelly DROP_IN_ID",
        },
        "MeadExtra": {
          "Id": "MeadExtra",
          "ItemId": "(O)459",
          "PreserveId": "DROP_IN",
          "ObjectInternalName": "{0} Mead",
          "ObjectDisplayName": "%PRESERVED_DISPLAY_NAME Mead",
          "PriceModifiers": 
          [
            {
              "Modification": "Multiply",
              "Amount": 2
            },
            {
              "Modification": "Add",
              "Amount": 100
            }
          ],
        },
      },
    },
    {
      "LogName": "Add Jelly and Mead to Wine Rule",
      "Action": "EditData",
      "Target": "Data/Machines",
      "TargetField": ["(BC)12", "OutputRules", "Default_Wine", "OutputItem", "Default", "CustomData"],
      "Entries": {
        "selph.ExtraMachineConfig.ExtraOutputIds": "JellyExtra,MeadExtra",
      },
    },
  ]
}
```

</details>

---

### Generate nearby flower-flavored modded items (or, generate flavored items outside of machines)

This mod implements a new item query, `selph.ExtraMachineConfig_FLAVORED_ITEM`
that acts as the generic version of the base game's
[`FLAVORED_ITEM`](https://stardewvalleywiki.com/Modding:Item_queries#Available_queries),
but usable for any modded items. The query takes the following arguments:

`selph.ExtraMachineConfig_FLAVORED_ITEM <output item ID> <flavor item ID> [optional override price]`

Replace <output item ID> with your modded artisan item ID, and flavor item ID
with your desired flavor, including
[`NEARBY_FLOWER_ID`](https://stardewvalleywiki.com/Modding:Machines) to take
the ID of a nearby flower if any. Make both of them unqualified (ie. without
the `(O)` part), or you may get harmless errors in the console.

For example, the following creates nearby flower-flavored mead:

`"ItemId": "selph.ExtraMachineConfig_FLAVORED_ITEM 459 NEARBY_FLOWER_ID"`

The flavored output item spawned by this query will:

* Have its flavor set to the flavor item ID.
  * Note that like the vanilla `FLAVORED_ITEM` rule, if the flavor is `-1` (due
    to the `NEARBY_FLOWER_ID` macro) it will be kept as-is and mess up the
    display name if you use `%PRESERVED_DISPLAY_NAME`! Stardew Valley 1.6.9
    will fix this as part of the new built-in flavor inherit feature, but in
    the mean time use the field `UnflavoredDisplayNameOverride` (as detailed
    below) to get around this.
* Inherit the color of the flavor item, if any. If you don't want this, simply
  put an empty sprite next to the item's sprite on the sprite sheet.
* Have its price set to the first matching entry of the below list:
  * The optional third parameter, if specified
  * The flavor item's price, if applicable
  * The item's base price otherwise. It's recommended that the base price be
    lower than the potential price of the flavor ingredient item to avoid the
    unflavored item being more expensive than flavored ones.
* If you want to scale the price further, use the machine rules' `PriceModifiers`.

Everything else (e.g. display name, etc.) will have to be set manually by the rest of the item/machine query.

Note that this item query technically can be used outside of machine rules.

---

### Override display name if the output item is unflavored

| Field Name                         | Description              |
| ---------------------------------- | ------------------------ |
| `selph.ExtraMachineConfig.UnflavoredDisplayNameOverride` | The display name to use for this machine rule's output item if the output happens to be unflavored (due to `InheritPreserveId` copying from an unflavored item, or if `NEARBY_FLOWER_ID` cannot find a nearby flower). |

---

### Generate an input item for recipes that don't have any, and use 'nearby flower' as a possible query

NOTE: This functionality is currently incomplete. It is also *very* specialized
and should not be used unless you know what you're doing. I'll also likely remove this in a future update tbh.

| Field Name                         | Description              |
| ---------------------------------- | ------------------------ |
| `selph.ExtraMachineConfig.OverrideInputItemId` | The item query to use as the actual "input", regardless of what/whether you used an input item. Supports an item query, or if set to `NEARBY_FLOWER_QUALIFIED_ID`, get a nearby flower, or null if no nearby flowers.<br>With this set, you can make use of features that requires a valid input item (`CopyColor`, `CopyPrice`, `PreserveId`, etc.) for non-input trigger rules like `DayUpdate`.|

#### Example

This example modifies the vanilla beehouse recipe to generate flower-flavored
mead instead of honey. It also colors the mead using
`selph.ExtraMachineConfig.CopyColor` (though the sprites will look weird since
the default mead sprite doesn't have a mask).

<details>

<summary>Content Patcher definition</summary>

```
{
  "Changes": [
    {
      "LogName": "Change Beehouse To Mead",
      "Action": "EditData",
      "Target": "Data/Machines",
      "TargetField": ["(BC)10", "OutputRules", "Default", "OutputItem", "Default"],
      "Entries": {
        "ItemId": "(O)459",
        "ObjectInternalName": "{0} Mead",
        "ObjectDisplayName": "%PRESERVED_DISPLAY_NAME Mead",
        "PriceModifiers": [
          {
            "Id": "FlowerBase",
            "Modification": "Multiply",
            "Amount": 4.0,
            // This condition will be false if input item is null, and true otherwise.
            // It is to ensure we only apply the price change if there's an actual nearby flower.
            // Otherwise, if no flowers are found, it will apply the price change on top of the base mead item!
            "Condition": "ITEM_PRICE Input 0"
          },
          {
            "Id": "HoneyBase",
            "Modification": "Add",
            "Amount": 300,
            "Condition": "ITEM_PRICE Input 0"
          },
        ],
        // All fields below this will only apply if there is an actual nearby flower.
        "CopyPrice": true,
        "PreserveId": "DROP_IN",
        "CustomData": {
          "selph.ExtraMachineConfig.CopyColor": "true",
          "selph.ExtraMachineConfig.OverrideInputItemId": "NEARBY_FLOWER_QUALIFIED_ID",
        },
      },
    },
  ]
}
```

</details>
