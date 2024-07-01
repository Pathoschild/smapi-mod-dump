**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/poohnhi/PoohCore**

----

## Bigger NPCs

My framework support creating NPCs that is higher than 32px and wider than 16px, using the game's customizable `Size` system.

All you have to do is specify your NPC's Size in disposition and add one line in `CustomFields`. For example:

```json
﻿"Size": { "X": 32, "Y": 38 }, // Setting the size for your NPC. Your spritesheet for them should match this data. See example pack.
"CustomFields": {
  "poohnhi.PoohCore/WideCharacter": "true", // If your NPC is wider than 16px, put this in their CustomFields
  "poohnhi.PoohCore/HighCharacter": "true" // If your NPC is higher than 32px, put this in their CustomFields
}
```

### Editing Vanilla/Existed NPCs

For existed NPCs (such as NPCs from vanilla or other mods), you will need some extra steps. First, you will need to load their new bigger sprite in to replace the original sprite. Second, you need to edit their NPC data to make sure that the game know you want their size to be bigger. This code block is a simple example for editing an existed NPC (with comments explaining why you need it):
```json
{
  "$schema": "https://smapi.io/schemas/content-patcher.json",
  "Format": "2.1.0",
  "Changes": [
    {
      "LogName": "Load new sprites to the game to replace the original",
      "Action": "Load",
      "Target": "Characters/Robin, Characters/Harvey",
      "FromFile": "Assets/{{TargetPathOnly}}/{{TargetWithoutPath}}.png"
      // This step load a bigger sprite into the game for Robin and Harvey. It uses Content Patcher's tokens to shorten the process for multiple characters, but you can make it simpler by just calling the file path in case that you only want to edit one character. In this code here, it will load the file path at Assets/Characters/Robin.png and Assets/Characters/Harvey.png inside your mod folder. 
    },
    {
      "LogName": "Edit NPC size data",
      "Action": "EditData",
      "Target": "Data/Characters",
      "Fields": {
        "Robin": {
          "Size": {"X": 32, "Y": 34}
        },
        "Harvey": {
          "Size": {"X": 32, "Y": 34}
        }
      }
      // This step will edit their Size data to match the new data you specified. Notice that I'm using "Fields" here - this is needed because we only want to replace the Size and not the other original data. If you use "Entries", ALL of the original data will be overrwritten and we don't want it to happen.
    },
    {
      "LogName": "Add custom fields for Robin",
      "Action": "EditData",
      "Target": "Data/Characters",
      "TargetField": [
        "Robin",
        "CustomFields"
      ],
      "Entries": {
        "poohnhi.PoohCore/WideCharacter": "true",
        "poohnhi.PoohCore/HighCharacter": "true"
      }
    },
    // This step will add a new entry to their CustomFields. Notice that I'm using "TargetField" and "Entries" here. The reason for this is CustomFields can contains other mod's data and we should not touch it for better compatibility. If I use "Fields" like the Size patch above, it will replace their CustomFields and make it have only my own data. By using "TargetField" and "Entries", I won't replacing any existing data but instead add a new entry "poohnhi.PoohCore/WideCharacter": "true" to that NPC CustomFields. Do the same for other NPCs like Harvey if you want to change multiple NPCs data.
    {
      "LogName": "Add custom fields for Harvey",
      "Action": "EditData",
      "Target": "Data/Characters",
      "TargetField": [
        "Harvey",
        "CustomFields"
      ],
      "Entries": {
        "poohnhi.PoohCore/WideCharacter": "true"
      }
    }
  ]
}
```

You can test it by using `patch export Data/Characters` and check what happened with the NPC data to see if your patches are applied correctly. If the changes don't show up for their size and custom fields, make sure that your EditData patches are applied after the original (by using dependencies in your mod's manifest file or set up a Priority. You can check Content Patcher's documentation for better explaination.). 

Notes:
- You might need to modify some more vanilla fields such as Shadow, EmoteOffset, MugShotSourceRect,... to polish them.
- See `CP.PoohCoreExample.zip` at [released file](https://github.com/poohnhi/PoohCore/releases) to download an example mod for NPCs that have bigger size. They have 16x48, 32x32 and 32x48 sprite as example, which you can meet from the bus stop.
- From testing, 48px height seems to cause some visual error so it's recommended to use just enough. Which means if your NPC just need to be a little bit higher than usual (about 34px), you should set them as 34px instead of 48px.
- Wider and higher sprites might clip into buildings or objects if they get too close on the schedule path and destination so make sure to test everything.
- The test NPCs from `PoohCoreExample`` also have a schedule to test pathing (which might need to sleep in game for at least one day to make it work if it's added to previous saves), appear in egg festival, and have a test event which you can play by using debug command:
`debug ebi poohnhi.PoohCoreTest.Event`
- Credit: NotJeremy's snail sprite is from [Axell](https://next.nexusmods.com/profile/7thAxis/about-me?gameId=1303)'s mod Diamond In the Rough - which is an alpha mod that you can find in [East Scarp Discord](https://discord.gg/JgZtzUSXY3) channel #useful-files.

## Daily Dialogue Progress
See example [here](https://github.com/poohnhi/PoohCore/tree/main/.%5BCP%5D%20Daily%20Dialogue%20Progression%20Example)

## Get Random Gift Taste Data (included modded objects)
See example [here](https://github.com/poohnhi/PoohCore/tree/main/.%5BCP%5D%20More%20Gift%20Taste%20Reveal%20Dialogue)
