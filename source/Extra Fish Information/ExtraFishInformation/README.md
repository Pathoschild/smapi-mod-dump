# Extra Fish Information [1.2.0]

This mod adds extra info to the descriptions for each fish, which will help you find the fish you're hoping to catch!

As the extra info is added to the descriptions, *you will only be able to see information for fish that you have already caught*.

The mod should work fine with [Teh's Fishing Overhaul](https://www.nexusmods.com/stardewvalley/mods/866) - please let me know if you encounter any issues with compatibility! It also works with [Lookup Anything](https://www.nexusmods.com/stardewvalley/mods/541), although there are some visual issues (see Mod compatibility below).

## Features
- For fish that can be caught using the **fishing rod**, the locations, seasons and times when the fish is active are added to the description (note that the 'Temp' and 'FishingGame' locations are ignored).
- For fish that can be caught in **crab pots**, the type of water that the fish is found in is added to the description.

## Requirements
- [SMAPI﻿](https://www.nexusmods.com/stardewvalley/mods/2400)

## Instructions
1. Install the latest version of [SMAPI﻿](https://www.nexusmods.com/stardewvalley/mods/2400)
2. Download and unzip the mod into Stardew Valley/Mods
3. Run Stardew Valley through SMAPI - config.json will be generated when the game is first run with this mod installed

## Changing time format (12 or 24 hour time)
1. This mod is in 24 hour time by default, so "TimeIn24Hours" in config.json will be set to "false" initially
2. To change to 12 hour time, close the game and edit config.json so "TimeIn24Hours" is set to "true"
3. Save the changes made to config.json and they will be in effect the next time Stardew Valley is run through SMAPI! :)
4. This can be changed at any time by closing Stardew Valley and changing the value of "TimeIn24Hours" in config.json, and then re-opening Stardew Valley through SMAPI

## Mod compatibility
- This mod **does** work with [Lookup Anything﻿](https://www.nexusmods.com/stardewvalley/mods/541), but the fish descriptions in the F1 menu using the Lookup Anything﻿ mod will not be properly aligned since the descriptions are changed. Hovering over any fish items for the descriptions will still work as expected.
- This mod currently does not work with the fish added in [More New Fish](https://www.nexusmods.com/stardewvalley/mods/3578)﻿ (sorry!)
- Please let me know if you encounter any other issues with other mods! :)

## Known issues/bugs
- Fish caught in the Night Market will have location "?"
- Please report any other bugs you find on the bugs page. :)

## Version history
- 1.0.0: Initial release
- 1.1.0: Added configuration ability to choose between 24 hour time and 12 hour time
- 1.1.1: Fixed localisation issue, should now provide info correctly for non-English localisations (please note that the extra info will still be in English)
- 1.2.0: Implemented translations, included rough French translation to begin with (please note that location names will still be in English as they are pulled straight from the game code) & implemented better exception handling

## Source code
Source code is available on my [GitHub](https://github.com/ceasg/StardewValleyMods).

## To do list
- Figure out why fish from content packs (e.g. More New Fish) are not being handled
- Parse location names from 'NamesLikeThis' to 'Names Like This'
- Handle Forest location differently - forest has 3 location zones (-1/0/1) which are currently being ignored
