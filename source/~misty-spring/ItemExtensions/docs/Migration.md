**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

# Migrating from CRC/CON

This page explains how to migrate from Custom Resource Clumps (or Custom Ore Nodes)

# Contents

* [Using the conversion script](#Conversion-script)
* [Format](#format)
* [From CRC](#description)
* [From CON](#from-con)

--------------------

## Conversion script

You can download [the script here](), and it'll  automatically port the files for you.

## Format

The format in Item Extensions is similar to CRC's, but is capitalized instead (e.g. debris to Debris). It follows [1.6's model for new items]().

Some notable changes:
- Debris is now text: you can specify stone, money, wood, or a custom debris (advanced). You can also give it a tint by adding the color right after: e.g, "stone purple" or "stone #6A7B8C"

- Chance is now a `double`: if you want 10%, use 0.1 (this allows for more precision, like 0.001 for a *very* rare chance)

## From CRC

Grab all the clumps inside `custom_resource_clumps.json`, and move them to a ContentPatcher edit: the edit must target `Mods/mistyspring.ItemExtensions/Resources`.

For example:
Let's say this is inside your CRC json:
```json
{
  "clumps": [
    {
      "clumpDesc": "An extremely dense piece of stone with diamonds in it.",
      "debrisType": 14,
      "failSounds": [
        "clubhit",
        "clank"
      ],
      "hitSounds": [
        "hammer"
      ],
      "breakSounds": [
        "boulderBreak"
      ],
      "shake": 100,
      "toolType": "pick",
      "toolMinLevel": 3,
      "tileWidth": 2,
      "tileHeight": 2,
      "spritePath": "assets\\diamond.png",
      "spriteType": "mod",
      "spriteX": 0,
      "spriteY": 0,
      "minLevel": -1,
      "maxLevel": -1,
      "baseSpawnChance": 0.01,
      "additionalChancePerLevel": 0.01,
      "durability": 20,
      "expType": "mining",
      "exp": 25,
      "dropItems": [
        {
          "itemIdOrName": "Diamond",
          "dropChance": 100,
          "minAmount": 3,
          "MaxAmount": 6
        },
        {
          "itemIdOrName": "390",
          "dropChance": 100,
          "minAmount": 5,
          "MaxAmount": 10
        }
      ]
    }
  ]
}
```

In the new format, it'd look like this:

```json
{
  "Changes": [
    {
      "Action": "EditData",
      "Target": "Mods/mistyspring.ItemExtensions/Resources",
      "Entries": {
        "MyClumpId": {
          "Description": "An extremely dense piece of stone with diamonds in it.",
          "Debris": "stone",
          "FailSounds": [
            "clubhit",
            "clank"
          ],
          "Sound": "hammer", //can add multiple as comma-separated
          "BreakingSound": "boulderBreak", //can add multiple as comma-separated
          "Shake": 100,
          "Tool": "pickaxe",
          "MinToolLevel": 3,
          "Width": 2,
          "Height": 2,
          "Texture": "assets\\diamond.png",
          "Index": 0,
          "SpawnOnFloors": "0-120", //all of the vanilla mines, for example. Treasure floors will be ignored
          "SpawnFrequency": 0.01,
          "AdditionalChancePerLevel": 0.01,
          "Health": 20,
          "Skill": "mining",
          "Exp": 25,
          "ExtraItems": [
            {
              "ItemId": "Diamond",
              "Chance": 1.0, //100%
              "MinStack": 3,
              "MaxStack": 6
            },
            {
              "ItemId": "390",
              "Chance": 1.0, //100%
              "MinStack": 5,
              "MaxStack": 10
            }
          ]
        }
      }
    }
  ]
}
```