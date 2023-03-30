**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/StardewMods**

----

## Contents
* [Adding quests](#Adding-quests)

## Adding Quests
Characters can make requests if they're in the same room as you. This has a 30% chance of happening every 30min.
They make use of `Data/Quests` for the ID- so it must be patched there (otherwise it won't be found by this mod).

They are added via `mistyspring.dynamicdialogues/Quests/<NPC>`.

name | description
-----|------------
Dialogue | The dialogue the NPC will say (to ask the question).
ID | The ID of your quest. (See [here for info on quests](https://stardewvalleywiki.com/Modding:Quest_data)).
AcceptQuest | (Optional) The text to accept a quest. Defaults to "Yes".
RejectQuest | (Optional) The text to accept a quest. Defaults to "No".
From | (Optional) The quest will only occur from this hour.
To | (Optional) The quest will only occur until this hour.
Location | (Optional) If used, the quest will only occur when in this location.

**Example**:
Here we will: 
1. Add the quest to Data/Quests 
```
{
      "Action": "EditData",
      "Target": "Data/Quests",
      "Entries": {
        "5200": "ItemDelivery/Rock Candy/Abigail wants you to bring her several amethysts./Bring Abigail 10 amethysts./Abigail 66 10/-1/300/-1/true/Thanks, these look delicious!"
     }
 }
```
2. Edit it into our mod file.
```
{
      "Action": "EditData",
      "Target": "mistyspring.dynamicdialogues/Quests/Abigail",
      "Entries": {
        "rockCandy": {
          "Dialogue": "@, do you have any spare gems? I'm hungry.",
          "ID": 5200,
          "AcceptQuest": "Yeah, let me bring some for you.",
          "RejectQuest": "I don't have any."
        }
     }
 }
```

When playing the game, there will be a 30% of Abigail asking this (if she's in the same room as the player).
