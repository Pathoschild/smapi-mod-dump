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

This document is for modders looking to incorporate this mod into their own
content packs. For users, install the mod as usual from the link above.

## Use
This mod reads extra data defined the [`CustomData` field in `OutputItem`](https://stardewvalleywiki.com/Modding:Machines#Item_processing_rules), which is
a map of arbitrary string keys to string values intended for mod use. Since
`CustomData` is per-output, it's possible to specify different settings for each
recipe, or even each output in the case of multiple possible outputs.

### Adding additional fuel for a specific recipe

| Field Name                         | Description              |
| ---------------------------------- | ------------------------ |
| `selph.ExtraMachineConfig.RequirementId.1` | The additional fuel that should be consumed by this recipe in addition to the ones specified in the machine's `AdditionalConsumedItems` field.<br> You can specify multiple fuels by adding another field with the same name, but with the number at the end incremented (eg. `ExtraMachineConfig.RequirementId.2`).<br> This ID can either be a qualified ID for a specific item, or a category ID for only categories (eg. `-2` will consume any gemstones as fuel).|
| `selph.ExtraMachineConfig.RequirementCount.1` | The count of the additional fuel specified in the field above. Defaults to 1 if not specified. |
| `selph.ExtraMachineConfig.RequirementInvalidMsg` | The message to show to players if all the requirements are not satisfied. Note that if there are multiple output rules with this field, only the last one will be shown.|

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

| Field Name                         | Description              |
| ---------------------------------- | ------------------------ |
| `selph.ExtraMachineConfig.InheritPreserveId` | When set to any value, copies the input item's flavor (e.g. the "Blueberry" part of "Blueberry Wine") into the output item.|

NOTE: Version 1.0.0 also supports this functionality via setting the
`PreserveId` field to `"INHERIT"`. This is not recommended however since
it would lead to weird results if this mod's not installed.

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

| Field Name                         | Description              |
| ---------------------------------- | ------------------------ |
| `selph.ExtraMachineConfig.CopyColor` | When set to any value, copies the input item's color into the output item.<br>The difference between this settings and the base game's `CopyColor` is that the latter only supports copying the color of colored items (eg. flowers), while the former will copy the dye color if the input is not a colored object.|

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

## Appendix: Comparison vs Producer Framework Mod 
[Producer Framework Mod](https://www.nexusmods.com/stardewvalley/mods/4970)
allows defining custom machines and recipes in Stardew Valley, with more
features and configurations than what's possible in the base game. Several of
those features are part of this mod's feature set, and as such it's expected
there will be significant overlap between them.

The main differences:

* PFM was made before the 1.6 overhaul to machine recipes, and is designed to
  make adding extra machines relatively painlessly and agnostic of game
  version. EMC was made to take advantage of the new reworked data structures
  in 1.6.

* PFM uses its own JSON configuration format, while EMC directly reads from
  the CustomData fields in
  [Content Patcher-patched machine
  recipes](https://stardewvalleywiki.com/Modding:Machines). Because of this,
  CP mods that use EMC will work perfectly fine without it,
  whereas PFM is a hard dependency for its users.

* PFM uses its own machine handling code, and is more flexible and extensible
  as a result, while EMC is designed to work around the game's handling and
  data structures as well as keeping things working even when uninstalled, and
  does not have many features as a result, but is much more compatible with any
  other mods that also patch game code.

In general, if you want to stick to pure Content Patcher (for compatibility
with other mods, etc.) for your mod then use this mod; otherwise it's probably
a better idea to stick to PFM.
