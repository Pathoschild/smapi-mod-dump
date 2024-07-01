**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/aceynk/PersistentBuffs**

----

# PersistentBuffs

Persistent buffs is a SMAPI (C#) Stardew Valley mod.
It allows Content Packs to make buffs persist across days and saves.

## Dependencies

Requires SMAPI (https://smapi.io)
Content Packs likely require Content Patcher (https://www.nexusmods.com/stardewvalley/mods/1915)

## Mod Authors: How to use

I'll give an example of how to use a Content Patcher content pack to designate a buff as persistent.
This assumes you already have some knowledge with making Content Patcher mods.

First, find a buff you want to make persistent.
I'll make a custom one, with the entry below:

```json
{
    "Action": "EditData",
    "Target": "Data/Buffs",
    "Entries": {
        "{{ModID}}_PersistentSpeed": {
            "DisplayName": "PersistentSpeed",
            "Duration": 10000000,
            "IconTexture": "TileSheets\\BuffsIcons",
            "IconSpriteIndex": 9,
            "Effects": {
                "Speed": 1
            }
        }
    }
}
```
(see https://stardewvalleywiki.com/Modding:Migrate_to_Stardew_Valley_1.6#Custom_buffs for more information)

Next, let's assign the buff to a food item (or whatever you want to use to add the buff).
I'll assign it to carrot items, with the entry below:

```json
{
    "Action": "EditData",
    "Target": "Data/Objects",
    "TargetField": [
        "Carrot"
    ],
    "Entries": {
        "Buffs": [
            {
                "BuffId": "{{ModID}}_PersistentSpeed"
            }
        ]
    }
}
```
(see https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide/action-editdata.md for more information)

Now, the PersistentSpeed buff will be applied whenever a carrot is eaten.
Next, to make the buff persist throughout days and saves, one more patch is needed.
Add the buff id (here, it's "{{ModID}}_PersistentSpeed" (where {{ModID}} resolves to the id set in the mod manifest)) by targeting PersistentBuffs/PersistentBuffIds.

```json
{
    "Action": "EditData",
    "Target": "aceynk.PersistentBuffs/PersistentBuffIds",
    "Entries": {
        "{{ModID}}_PersistentSpeed": true
    }
}
```

Combining these patches, every carrot now gives a +1 speed buff that lasts through days and saves.


