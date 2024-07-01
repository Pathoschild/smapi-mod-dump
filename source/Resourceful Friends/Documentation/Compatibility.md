**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/itsbenter/ResourcefulFriends**

----

# Compatibility

This page includes compatibility patches and pointers to make compatibility for this mod.

* [Petalkin](#petalkin)
  * [Stardew Valley Expanded](#stardew-valley-expanded)
* [Adding Drops to Produce](#adding-drops-to-produce)
* [Adding Forage to Forage Patch](#adding-forage-to-forage-patch)
* [Footnote](#footnote)

## Petalkin

In concept Petalkin appearances would change based on the forages in the area. Mods that add ability to build Barn in area with distinct forages should get a compatibility patch. So far two mods have been identified.

1. [Visit Mount Vapius](https://www.nexusmods.com/stardewvalley/mods/9600)
2. [Ridgeside Village](https://www.nexusmods.com/stardewvalley/mods/7286)

### Stardew Valley Expanded

These are [Stardew Valley Expanded](https://www.nexusmods.com/stardewvalley/mods/3753) forages compatibility patch added by this mod. Petalkin's Forage Patch will produce [Stardew Valley Expanded](https://www.nexusmods.com/stardewvalley/mods/3753) forages along the base game forages.

| Context | Common Forages | Uncommon Forages | Rare Forages |
| ------- | -------------- | ---------------- | ------------ |
| Spring | | Ferngill Primrose<sup>[2](#RailroadBoulderCleared)</sup>, Lucky Four Leaf Clover, Thistle<sup>[2](#RailroadBoulderCleared)</sup> | Rafflesia<sup>[1](#SecretWood)</sup> |
| Summer | Red Baneberry<sup>[1](#SecretWood) | Goldenrod<sup>[2](#RailroadBoulderCleared)</sup>, Lucky Four Leaf Clover, Poison Mushroom<sup>[1](#SecretWood)</sup>, Rafflesia<sup>[1](#SecretWood)</sup>, Thistle<sup>[2](#RailroadBoulderCleared)</sup> | |
| Fall | Mushroom Colony<sup>[1](#SecretWood)</sup> | Poison Mushroom<sup>[1](#SecretWood)</sup>, Thistle<sup>[2](#RailroadBoulderCleared)</sup> | Rafflesia<sup>[1](#SecretWood)</sup> |
| Winter | | Bearberry<sup>[1](#SecretWood)</sup>, Thistle<sup>[2](#RailroadBoulderCleared)</sup> | |

## Adding Drops to Produce

In case of Coalclaw, Molecat, Petalkin, and Timberbark, animals that has intermediary product that need to be interacted upon to gain the resources you can add to what they produce by targeting `ExtraItems` field in the respective entry in `Mods/mistyspring.ItemExtensions/Resources`. For example the following patch would make Molecat Node has 50% chance to drop 1 type of gems that priced between 1G to 200G with a stack size between 1 to 2. It will also double if player has Geologist profession.

```jsonc
{
    "Changes": [
        {
            "LogName": "Gems for Molecat Node",
            "Action": "EditData",
            "Target": "Mods/mistyspring.ItemExtensions/Resources",
            "TargetField": [
                "ItsBenter.ResourcefulFriends.MolecatNode",
                "ExtraItems"
            ],
            "Entries": {
                "ItsBenter.ResourcefulFriends.MolecatNode_Gems": {
                    "Id": "ItsBenter.ResourcefulFriends.MolecatNode_Gems",
                    "Chance" : 0.5,
                    "Quality": 0,
                    "ItemId": "RANDOM_ITEMS (O)",
                    "PerItemCondition": "ITEM_CATEGORY Target -2, ITEM_PRICE Target 1 200", //Items Categorized as Gems with price between 1G to 200G
                    "MinStack": "2",
                    "MaxStack": "1",
                    "MinItems": "1",
                    "MaxItems": "1",
                    "StackModifiers": [
                        {
                            "Id": "ItsBenter.ResourcefulFriends.MolecatNode_GeologistProfession",
                            "Condition": "PLAYER_HAS_PROFESSION Current 19", //Geologist
                            "Modification": "Multiply",
                            "Amount": 2,
                        }
                    ]
                }
            }
        }
    ]
}
```

Here are the list of Id for the intermediary resources.

| Resource | Id |
| -------- | -- |
| Coalclaw Clump | ItsBenter.ResourcefulFriends.CoalclawClump |
| Large Coalclaw Clump | ItsBenter.ResourcefulFriends.LargeCoalclawClump |
| Timberbark Pile | ItsBenter.ResourcefulFriends.TimberbarkPile |
| Large Timberbark Pile | ItsBenter.ResourcefulFriends.LargeTimberbarkPile |
| Spring Forage Patch | ItsBenter.ResourcefulFriends.SpringForagePatch |
| Summer Forage Patch | ItsBenter.ResourcefulFriends.SummerForagePatch |
| Fall Forage Patch | ItsBenter.ResourcefulFriends.FallForagePatch |
| Winter Forage Patch | ItsBenter.ResourcefulFriends.WinterForagePatch |
| Ginger Island Forage Patch | ItsBenter.ResourcefulFriends.GingerIslandForagePatch |
| Molecat Node | ItsBenter.ResourcefulFriends.MolecatNode |

## Adding Forage to Forage Patch

Petalkin Forage Patch detect the drops when destroyed using [context tags](https://stardewcommunitywiki.com/Modding:Context_tags).

| Context Tags | Description |
| ------------ | ----------- |
| itsbenter_petalkin_spring_common | Common forage to spawn during Spring when in the Stardew Valley |
| itsbenter_petalkin_spring_uncommon | Uncommon forage to spawn during Spring when in the Stardew Valley |
| itsbenter_petalkin_spring_rare | Rare forage to spawn during Spring when in the Stardew Valley |
| itsbenter_petalkin_summer_common | Common forage to spawn during Summer when in the Stardew Valley |
| itsbenter_petalkin_summer_uncommon | Uncommon forage to spawn during Summer when in the Stardew Valley |
| itsbenter_petalkin_summer_rare | Rare forage to spawn during Summer when in the Stardew Valley |
| itsbenter_petalkin_fall_common | Common forage to spawn during Fall when in the Stardew Valley |
| itsbenter_petalkin_fall_uncommon | Uncommon forage to spawn during Fall when in the Stardew Valley |
| itsbenter_petalkin_fall_rare | Rare forage to spawn during Fall when in the Stardew Valley |
| itsbenter_petalkin_winter_common | Common forage to spawn during Winter when in the Stardew Valley |
| itsbenter_petalkin_winter_uncommon | Uncommon forage to spawn during Winter when in the Stardew Valley |
| itsbenter_petalkin_winter_rare | Rare forage to spawn during Winter when in the Stardew Valley |
| itsbenter_petalkin_winter_common | Common forage to spawn during Winter when in the Stardew Valley |
| itsbenter_petalkin_winter_uncommon | Uncommon forage to spawn during Winter when in the Stardew Valley |
| itsbenter_petalkin_winter_rare | Rare forage to spawn during Winter when in the Stardew Valley |
| itsbenter_petalkin_gingerisland_common | Common forage to spawn when in the Ginger Island |
| itsbenter_petalkin_gingerisland_uncommon | Uncommon forage to spawn when in the Ginger Island |
| itsbenter_petalkin_gingerisland_rare | Rare forage to spawn when in the Ginger Island |

In case you want to add specific drops refer to [Adding Drops to Produce](Compatibility.md/#adding-drops-to-produce)

## Footnote

<a name="SecretWood">1</a>: Only available after entering Secret Wood<br>
<a name="RailroadBoulderCleared">2</a>: Only available after boulder blocking the path to summit is cleared<br>
