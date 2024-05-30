**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/lisyce/SDV_Allergies_Mod**

----

# Content Packs

You can create a content pack for BarleyZP's Allergies in order to add custom allergens into the game and assign allergens to modded items.

# Getting Started

If you've never made a content pack before, please check out the [wiki](https://stardewvalleywiki.com/Modding:Content_packs) for an overview. It's recommend to use [Content Patcher](https://www.nexusmods.com/stardewvalley/mods/1915), although you can edit the assets via the C# SMAPI APIs if you prefer.


# Content

> Check out the [Cornucopia Compat Pack](https://www.nexusmods.com/stardewvalley/mods/22329) for an example! This guide assumes you're using Content Patcher.

## Data Format

Allergens are stored in the asset `BarleyZP.BzpAllergies/AllergyData`. This is a dictionary of unique `string` allergy Ids mapped to object entries. They have the following fields:

- `DisplayName`: Translated display name of the allergy.
- `AddedByContentPackId`: For custom allergies, the unique Id of the mod that added it.
- `ObjectIds`: A list of unqualified object Ids that have the allergy. Preserves Items (jelly, aged roe, etc.) are handled by the mod code, so you only need to list the input object. Additionally, any foods that are cooked with these items will retain the allergies of their ingredients. However, you should still assign the cooked foods to any allergens they may have as a backup, just in case the mod code needs something to fall back on.
- `ContextTags`: A list of context tags. Any objects with at least one of these context tags will have the allergy. 

The allergens added by the base mod have Ids `"egg"`, `"gluten"`, `"fish"`, `"shellfish"`, `"treenuts"`, `"dairy"`, and `"mushroom"`, so you only need to edit the `ObjectIds` and `ContextTags` fields for these ones.

Here's an example of a `content.json` that adds some modded objects to the base game `treenuts` allergy and adds a new allergy called `mayo`:

```json
{
  "Format": "2.0.0",
  "Changes": [
    {
      "Action": "EditData",
      "Target": "BarleyZP.BzpAllergies/AllergyData",
      "TargetField": [ "treenuts", "ObjectIds" ],
      "Entries": {
        "#-1": "AlmondId",
        "#-2": "CashewId"
      }
    },
    {
      "Action": "EditData",
      "Target": "BarleyZP.BzpAllergies/AllergyData",
      "Entries": {
        "mayo": {
          "AddedByContentPackId": "{{ModId}}",
          "DisplayName": "Mayo",
          "ObjectIds": [
            "648",
            "213"
          ],
          "ContextTags": [
            "mayo_item"
          ]
        }
      }
    },
  ]
}
```

Using the `ContextTags` field can save typing if there are lots of items that share a context tag that you would like to all have the same allergen. Notice how both Coleslaw (`"648"`) and the Fish Taco (`"213"`) are also marked as having the mayo allergen because they are cooked with mayo.

### Automatically-added allergens

- Any object with the tags `egg_item`, `mayo_item`, or `large_egg_item` contains the egg allergen
- Any object with the tags `milk_item`, `large_milk_item`, `cow_milk_item`, or `goat_milk_item` contains the dairy allergen
- The base mod contains both fish and shellfish allergens, but the game code describes all of these items under the same fish category. Any custom modded objects under the fish category (`-4`) will be automatically marked as having the fish allergy as long as it was NOT registered under the shellfish allergen. Even if your modded shellfish is not edible as-is (like the vanilla crab pot fish), you should still register it under the shellfish allergen so that processed goods from that shellfish are also registered under the shellfish alleregen.

## Registering Alt Milks and Flours

To make an item a possible input for the Plant Milk recipe, assign it the context tag "BarleyZP.BzpAllergies_PlantMilkIngredient". Similarly, to register a modded flour item as flour for use in recipes that require "any flour", give it the tag "BarleyZP.BzpAllergies_FlourIngredient".

## Giving Milled Items Allergens

If you want to give items in the mill (such as new flours) allergens, you'll want to append to the "BarleyZP.BzpAllergies_MadeWith" key in the `ModData` of the output item using [Content Patcher Text Operations](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide/text-operations.md#append). This should have a value of a comma-separated list of allergen Id (not display names or object Ids) with no spaces. For example: "treenuts,almond".

Here's an example of adding another way to make gluten-free flour in the mill with an object with the id "cashew" that contains the "treenuts" and "cashew" allergens:

```json
{
  "Format": "2.0.0",
  "Changes": [
     {
      "LogName": "Add gluten free flour to the mill",
      "Action": "EditData",
      "Target": "Data/Buildings",
      "TargetField": [ "Mill", "ItemConversions" ],
      "Entries": {
        "{{ModId}}_CashewGfFlour": {
          "Id": "{{ModId}}_CashewGfFlour",
          "RequiredTags": [
            "id_o_cashew"
          ],
          "MaxDailyConversions": -1,
          "SourceChest": "Input",
          "DestinationChest": "Output",
          "ProducedItems": [
            {
              "ItemId": "(O)BarleyZP.BzpAllergies_GfFlour",
              "ModData": {
                "BarleyZP.BzpAllergies_MadeWith": "treenuts,cashew"
              }
            }
          ]
        }
      }
    }
  ]
}
```