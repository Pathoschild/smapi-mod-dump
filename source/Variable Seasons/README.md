**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/calebstein1/StardewVariableSeasons**

----

# Stardew Variable Seasons

## NOTICE: Not yet compatible with Stardew Valley 1.6. Support is in development.

A mod for Stardew Valley that adds some variability to when the seasons actually change, rather than just changing on the first of each season.

Seasons will now change on a random day between 23 of the current season and 7 of the next season, weighted around 1 of the next season.
For example, the actual change from Spring to Summer can now occur on any day between Spring 23 and Summer 7.

About halfway through a season, the weather channel will give hints as to the length of the season, letting the player know whether the current season will be abnormally short, shorter than average, about average, longer than average, or abnormally long.

This mod will also include a mechanic to allow out-of-season crops to survive for some amount of time based on a percentage that decreases each day the crop remains out of season, and will allow for the purchase of crops for the next season from the general store.

What works:

- Randomly selecting a date of next season change occurs during the night prior to day 15 each season.
- The season progresses the night after the randomly selected number, rather than on the 1st of each season.
- The weather channel will give a hint as to the length of the current season from days 16 through 20 each season.
- Festivals in close proximity of the end of a season will occur on the proper day, even if out of season.
- Birthdays in close proximity to the beginning or end of a season will occur on the proper day, even if out of season.
- Out of season crops will survive some amount of time past a season change before dying (still need to test winter transition).
- The general store sells crop seeds for the next season as well as the current season.
- The calendar outside the general store shows the correct season by day when viewed out of season.
- The year will increment on Spring 1 by the calendar, whether or not it's still Winter in-game.

What's in development:

- JojaMart will also sell crop seeds for th next as well as current season. (This likely won't be implemented until after 1.6 releases)
- Add a settings page to enable/disable the mod.
- Add in compatibility with the Longer Seasons mod.
- Maybe see if the season icon in the top-right can be updated based on the calendar season, rather than the in-game season.

Known bugs:

- The season listed on the load save files list will always reflect the actual in-game season, not the calendar season. It's purely cosmetic and doesn't affect how the seasons progress in-game, but I'd like to fix it for clarity's sake if possible.
- Festivals that occur "out of season" will will use their "in season" maps. This is most noticable, for example, if Spirit's Eve occurs after the change to winter, the festival map will display the fall textures just for the festival, then revert back to winter textures the festival ends. This is probably fixable, but it's not really a priority right now. Canon can be that Pelican Town has a secret crew of prolific snow-shovelers.
- ??? (please report any you find)
