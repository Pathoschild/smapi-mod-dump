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
