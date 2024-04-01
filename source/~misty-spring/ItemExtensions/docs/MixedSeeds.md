**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Mixed Seeds

The mod allows to create custom mixed seeds

## Contents

* [Description](#description)
* [How to add](#how-to-add)
* [Via Custom fields](#via-customfields)
  * [Example](#example)
* [Via Mod](#via-mod)
  * [Format](#format)
  * [Example](#example-1)
* [Excluding the main seed](#excluding-the-main-seed)

---

## How to add

You can add custom mixed seeds in two ways: Via custom fields and by editing the mod. The latter has more options (like conditions, weight, etc.)

## Via CustomFields

Just add this to your Object's `CustomFields`:  `mistyspring.ItemExtensions/MixedSeeds` . The seed IDs must be separated by spaces.

### Example

In this example, we add a new seed item to [Data/Objects](https://stardewvalleywiki.com/Modding:Migrate_to_Stardew_Valley_1.6#Define_a_custom_item). **Object's required fields have been ommitted for readability**.

This will add the following seeds to the roster: Strawberry seeds, Starfruit seeds, and Blueberry seeds.

```jsonc
{
  "Action": "EditData",
  "Target": "Data/Objects",
  "Entries": {
    "MyCustomSeeds": {
      //(...seed data)
      "CustomFields": {
        "mistyspring.ItemExtensions/MixedSeeds": "745 486 481"
      }
    }
  }
}
```

As a result, whenever you plant MyCustomSeeds, there's a 25% chance you'll get a strawberry seed (likewise for parsnip and blueberry).

This is because the mod takes all "possible" seeds:
```txt
MyCustomSeeds 745 472 481
```
And chooses one randomly.

## Via Mod

To add mixed seeds via the mod, edit `Mods/mistyspring.ItemExtensions/MixedSeeds` . The key must be the seed Id, and the value is a `MixedSeedData` list with all possible seeds.

### Format

`MixedSeedData` follows the following format:

| name      | type              | Required | description                                        |
|-----------|-------------------|----------|----------------------------------------------------|
| ItemId    | `string`          | Yes      | Id in Data/Objects.                                |
| Condition | `string`          | No       | A Game State Query.                                |
| HasMod    | `string`          | No       | Only add this seed if a mod is installed.          |
| Weight    | `int`             | No       | Default 1.                                         |

### Example

Here is an example of an edit. We add possible seeds to MyCustomSeed (the item **must** exist in Data/Objects).
This will add the following seeds to the roster: Strawberry seeds, Starfruit seeds, and Blueberry seeds.

```jsonc
{
  "Action": "EditData",
  "Target": "Mods/mistyspring.ItemExtensions/MixedSeeds",
  "Entries": {
    "MyCustomSeed": [
      {
        "ItemId": "745",
        "Weight": 3
      },
      {
        "ItemId": "486",
        "Condition": "YEAR 2"
      },
      {
        "ItemId": "481",
        "HasMod": "SomeMod.ForThisSeed"
      }
    ]
  }
}
```

This is what will happen:

- Strawberries will be "added" thrice.
- Starfruit seeds will only be available if it's year 2 or more.
- Blueberry seeds will only be available if you have `SomeMod.ForThisSeed`.

If all these conditions are met, the "possible seeds" would look like this:

```txt
MyCustomSeeds 745 745 745 486 481
```

But let's say it's Y1 and you don't have `SomeMod.ForThisSeed`. Instead, it'd look like this:

```txt
MyCustomSeeds 745 745 745
```

From this list, a random one will be chosen.

(If a condition for x seed doesn't match, the seed will simply be ignored / not added to list).

## Excluding the main seed

The main seed is added by default to the roster. If you don't want it to, add this to its object's custom fields:

```
 "mistyspring.ItemExtensions/AddMainSeed": "0"
```

(Likewise, if you want to add the main seed more than once, just change the number to that. E.g, set it to '2' to add it twice.)