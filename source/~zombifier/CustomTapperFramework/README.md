**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/zombifier/My_Stardew_Mods**

----

# Machine Terrain Expansion Framework (Formerly Custom Tappers Framework)

[Machine Terrain Expansion Framework](https://www.nexusmods.com/stardewvalley/mods/22975)
is a Stardew Valley mod that adds the following features:

* Define machines that are placeable on terrain features (currently wild trees,
  fruit trees, giant crops).
* Define machines that are placeable on water like crab pots.
* Define aquatic crops that are (only or also) plantable on water, and two new pot-like
  items to plant them with.

This document is for modders looking to incorporate this mod into their own
content packs. For users, install the mod as usual from the link above.

## Table of Contents
* [Terrain-Based Machine Feature](#terrain-based-machine-feature)
    + [Machine API](#machine-api)
    + [Tapper API](#tapper-api)
    + [Example](#example)
* [Aquatic Crops Feature](#aquatic-crops-feature)

## Terrain-Based Machine Feature

There are two APIs available:

* The Machine API using context tags and machine rules, which is more powerful
  and supports water placement, but doesn't support some features of the Tapper
  API (notably, specifying output based on the terrain feature's produce).
* The Tapper API using a custom asset to define automatic produce overtime.
  This API doesn't support water-placeable buildings, but is simpler to use and
  supports defining conditions and outputs based on the terrain feature's
  produce. If you want to modify the output of the base game Tapper and Heavy
  Tapper items this API should be used.

In the future the Tapper API's "output based on terrain" feature is expected to
be folded into the Machine API, but both will continue to work into the future.
For now use only one for your machine, not both.

### Machine API
First, set the appropriate context tags for your big craftables:

* If a tapper-like tree/giant crop machine, add `"tapper_item"` (same as vanilla)
  * If you don't want the vanilla tree tapper output (e.g. Maple Syrup, Oak Resin) to apply, also add `"custom_wild_tree_tapper_item"`
  * All tappers are placeable on trees by default. To disallow this, add `"disallow_wild_tree_placement"`
  * To make this building placeable on fruit trees, add `"custom_fruit_tree_tapper_item"`
  * To make this building placeable on giant crops, add `"custom_giant_crop_tapper_item"`
* If a crab pot-like water building, add `"custom_crab_pot_item"`

Then define your machine behavior in
[`Data/Machines`](https://stardewvalleywiki.com/Modding:Machines) as usual.

### Tapper API
First, add `"tapper_item"` context tag to your big craftables.

Then, write the mod data to the `selph.CustomTapperFramework/Data` asset, unless
you're modifying the base game tappers' data, in which case their data is
populated and you should instead edit/add to them. The asset takes the form of
a map, with the key being the qualified item ID of the tapper and the value
being a model with the following fields:

| Field Name | Type | Description |
| ---------- | ---- | ----------- |
| `AlsoUseBaseGameRules` | `bool` | Whether this tapper can also be used like the base game tapper (ie. place on a wild tree to get their tap produce). Defaults to false, except for base game tappers, where this value will always be true.<br> This will also be true for tapper item that isn't defined in the mod data.<br>Set this or `TreeOutputRules`, not both.|
| `TreeOutputRules` | `List<ExtendedTapItemData>` | A list of output rules to apply when this tapper is placed on a wild tree. If null, will not be placeable on trees (unless `AlsoUseBaseGameRules` is true).|
| `FruitTreeOutputRules` | `List<ExtendedTapItemData>` | A list of output rules to apply when this tapper is placed on a fruit tree. If null, will not be placeable on fruit trees.|
| `GiantCropOutputRules` | `List<ExtendedTapItemData>` | A list of output rules to apply when this tapper is placed on a giant crop. If null, will not be placeable on giant crops.|

`ExtendedTapItemData` is an item query object that defines the items produced
by the tree. The type extends from the entries in a [wild tree's `TapItem`
data](https://stardewvalleywiki.com/Modding:Migrate_to_Stardew_Valley_1.6#Custom_wild_trees),
so visit the wiki page for reference on its base fields.

Additionally, `ExtendedTapItemData` supports the following additional fields:

| Field Name | Type | Description |
| ---------- | ---- | ----------- |
| `SourceId` | `string` | If set, only apply this rule if the tapped tree/fruit tree/giant crop is of this ID. |
| `RecalculateOnCollect` | `bool` | Whether to recalculate the output upon collection. This is really only useful for flower honey, to readjust the honey flavor. |

`ExtendedTapItemData`'s game state and item queries support supplying the input item aside from the target:

* For trees, the input is their seed object
* For fruit trees, the input is their first defined fruit
* For giant crop, the input is their first defined drop

This can be used for defining dynamic output e.g. flavored juice, when combined with the macros below.

The output item query supports the following macros:

| Name | Description |
| ---------- | ----------- |
| `DROP_IN_ID` | The qualified item ID of the "input" item as defined above. |
| `NEARBY_FLOWER_ID` | The qualified item ID of a nearby flower. Only useful for honey rules. |

### Example

See below for an example from the [Additional Tree Equipments](https://www.nexusmods.com/stardewvalley/mods/22991)
mod, which adds a tree bee house that can be placed on any tree, and
produces honey every 4 days except in winter:

<details>

<summary>Content Patcher definition</summary>

```
{
  "Changes": [
    {
      "LogName": "Add Custom Tapper Framework Data",
      "Action": "EditData",
      "Target": "selph.CustomTapperFramework/Data",
      "Entries": {
        "(BC)selph.ExtraTappers.BeeHouse": {
          "AlsoUseBaseGameRules": false,
          "FruitTreeOutputRules": [
            {
              "Id": "Honey",
              "ItemId": "FLAVORED_ITEM Honey NEARBY_FLOWER_ID",
              "DaysUntilReady": 4,
              "Condition": "!LOCATION_SEASON Target Winter",
              // SourceId = null allows all fruit trees. You can set this field if you want to limit it to only certain types of trees.
              "SourceId": null,
              "RecalculateOnCollect": true,
            },
          ],
          "TreeOutputRules": [
            {
              "Id": "Honey",
              "ItemId": "FLAVORED_ITEM Honey NEARBY_FLOWER_ID",
              "DaysUntilReady": 4,
              "Condition": "!LOCATION_SEASON Target Winter",
              "RecalculateOnCollect": true,
            },
          ],
        },
      }
    },
  ]
}
```
</details>

If you want to instead add to the base game tapper's outputs, instead do something like below, which makes tapped fruit trees produce sap:

<details>

<summary>Content Patcher Definition</summary>

```
{
  "Changes": [
    {
      "LogName": "Modify base heavy tapper rules",
      "Action": "EditData",
      "Target": "selph.CustomTapperFramework/Data",
      "TargetField": ["(BC)105", "FruitTreeOutputRules"],
      "Priority": "Late",
      "Entries": {
        "selph.ExtraTappers.Sap": {
          "DaysUntilReady": 1,
          "Chance": 1.0,
          "Id": "selph.ExtraTappers.Sap",
          "ItemId": "(O)92",
          "MinStack": 3,
          "MaxStack": 8,
        },
      },
    },
}

```
</details>

## Aquatic Crops Feature

This mod adds two new items:

* A Water Planter, placeable on water tiles to create a steady planting spot
  for water crops. Craftable with 20 wood.
* A Water Pot, to allow water crops to be plantable on land. Requires a Garden
  Pot to craft.

By default, these items' crafting recipes are disabled unless
aquatic/semiaquatic crops are added by content packs, in which
case they're automatically enabled (after Garden Pots are available for Water Pots).

To define crops that are plantable on water, add the following new keys to the
crop definition's `CustomFields` dict (their values can be anything as long as they're set):

* `selph.CustomTapperFramework.IsAquaticCrop` for crops that are only plantable in the water planter or water pot.
* `selph.CustomTapperFramework.IsSemiAquaticCrop` for crops that are plantable on both land and water.

Additionally, it's recommended that the crop be set to be paddy crops. Paddy
crops will get the growth speed bonus when placed inside water planters,
but *not* water pots. Gotta get that natural water!

Crops in water planters/pots are automatically considered watered every day,
for obvious reasons.

Bushes (specifically [Custom Bushes](https://www.nexusmods.com/stardewvalley/mods/20619)) currently are not supported.
