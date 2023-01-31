**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/misty-spring/SpousesIsland**

----

### How to add spouses

You must know [how the game's schedules work.](https://stardewvalleywiki.com/Modding:Schedule_data)


Edit the spouse's schedule via ContentPatcher, and add the following token: `mistyspring.spousesisland/CanVisitIsland` as condition.

Starting from 10pm, the mod will path spouses to the bed. Just make sure their __last schedule point__ is `IslandWest 77 41 0`, followed by `IslandFarmhouse <x> <y>`- the mod will handle things!

The schedule can include any map in the island (except volcano).


For example:

```
{
  "Action": "EditData",
  "Target": "Characters/schedules/<your-spouse-name>",
  "Entries": {
     "marriage_Mon": "700 IslandFarmHouse 16 9 0/900 IslandFarmHouse 20 15 0/1200 IslandWest 39 41 0/1400 IslandWest 39 45 3/1500 IslandWest 85 39 2/1700 IslandSouth 12 27 2/a21500 IslandWest 77 41 0"
     "marriage_Tue": "GOTO marriage_Mon", 
     "marriage_Wed": "GOTO marriage_Mon",
     "marriage_Thu": "GOTO marriage_Mon",
     "marriage_Fri": "GOTO marriage_Mon",
     "marriage_Sat": "GOTO marriage_Mon",
     "marriage_Sun": "GOTO marriage_Mon"
     },
  "When": {
     "mistyspring.spousesisland/CanVisitIsland": "true"
    }
  },
```

### Randomizing visit

If you want to make your spouse go somewhere random, paste [these tokens](https://raw.githubusercontent.com/misty-spring/SpousesIsland/main/docs/randomized_tokens.md) into your `content.json`

Then, just use them in the schedule. Like this: `(...)/TIME {{RandomMap}} {{Random_Tile}}/(...)`

For example:
```
"marriage_Mon": "630 IslandFarmHouse 16 9 0/900 IslandFarmHouse 20 15 0/1200 {{Random_Map}} {{Random_Tile}}/1600 {{Random_Map}} {{Random_Tile}}/2000 {{Random_Map}} {{Random_Tile}}/a21500 IslandWest 77 41 0/2200 IslandFarmHouse 19 6 0"
```

Here, the spouse will go to the island farmhouse, then three *randomized* maps (at 12pm, 4pm and 8pm) and return home.

### For more help

If you still need help, you can download the [mod spouse template](https://www.nexusmods.com/stardewvalley/mods/11037?tab=files) and look how it works.
