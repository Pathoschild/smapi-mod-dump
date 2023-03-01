**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/atravita-mods/StardewMods**

----

Documentation
=============

[← back to readme](../../README.md)

### Introduction

Ginger Island Mainland Adjustments adjusts schedules of villagers after they visit the Resort on the Island, adds some dialogue keys for villagers traveling to and from the Island, and optionally overrides the entire vanilla resort scheduler to allow for more visitors at the resort at once.

Certain behaviors of the mod can be altered by using Content Patcher to patch into specific targets this mod monitors. For example, this would add a custom NPC group:

```js
{
    "Format": "1.28.0",
    "Changes": [
        {
            "Action": "EditData",
            "Target": "Mods/atravita_Ginger_Island_Adjustments_groups",
            "Entries": {
                "GoldenTrio": "Harry, Hermione, Ron"
            }
        }
    ]
}
```

The following data is what this mod will look for:

#### (1) Specific GIRemainder schedules located in the NPC's usual schedule file

```js
{
    "Format": "1.28.0",
    "Changes": [
        {
            "Action": "EditData",
            "Target": "Characters/schedules/Leah",
            "Entries": {
                "GIRemainder": "1800 LeahHouse 8 5 0/2200 LeahHouse 13 10 2/2400 LeahHouse 2 4 3 leah_sleep"
            }
        }
    ]
}
```

causes Leah to wander around in her house before she turns in for the night. The code will find the schedule, parse it, and append it to the end of Leah's schedule if she goes to Ginger Island.

For a full list of possible schedule keys, please see the [schedule documentation](./GIRemainderSchedules.md). There are also examples in the [example pack](https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments/blob/master/%5BCP%5D%20Ginger%20Island%20Mainland%20Adjustments/schedules.json).

#### (2) Specific dialogue lines in the NPC's usual dialogue file

```js
    {
        "LogName": "Pam dialogue",
        "Action": "EditData",
        "Target": "Characters/Dialogue/Pam",
        "Entries": {
            "Resort_Approach": "@, I'm gonna be headed to Ginger Island today! Got a nice vacation day from the bus company."
        }
    }
```

will cause Pam to tell you that she's headed to Ginger Island for the day.

A full list of possible dialogue keys can be found [here](./GIDialogueKeys.MD), and there are examples in the [example pack](https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments/tree/master/%5BCP%5D%20Ginger%20Island%20Mainland%20Adjustments/assets/default).

#### (3) Data in specific files this mod looks for.

If this mod's Ginger Island scheduler is enabled, the mod will look at specific data files to find roles and groups. Please see the full documentation for this [here](./GIScheduling.MD)

### FAQ
#### Do I need to list this this mod as a dependency?

Actually not....unless you try to edit any of this mod's data files (see [scheduling](.\GIScheduling.MD)), and even then you could use a [HasMod](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide/tokens.md#HasMod). This is because extra keys in the schedule and dialogue files are just simply ignored by the game, which makes this mod pretty good as an optional dependency.

### See Also:

1. For more examples, please see [the example pack!](https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments/tree/master/%5BCP%5D%20Ginger%20Island%20Mainland%20Adjustments). 

2. See also: [Content Patcher documentation](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide.md).