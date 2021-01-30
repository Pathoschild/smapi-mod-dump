**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Chikakoo/stardew-valley-randomizer**

----

# Stardew Valley Randomizer (More Random Edition)

An update for cTooshi's Stardew Valley Randomizer to fix errors, add new features and make the existing features more random.
Nexus Mods page available here: https://www.nexusmods.com/stardewvalley/mods/5311

## Installation

Make sure you have SMAPI installed (https://stardewvalleywiki.com/Modding:Installing_SMAPI_on_Windows), then download the latest release of the Randomizer and unzip into your Stardew Valley Mods folder.

## Info

Note the following:
* This mod's randomization is seeded by the farm name. To generate a new set of random things, simply start a new farm with a different name.
* For details on how to create your own custom images for the mod, see the readme file in <ModFolder>/Assets/CustomImages

## Features

* Bundle randomization
  * New bundles for each room with random items selected from themed pools and random number of those items required
  * IMPORTANT: DO NOT use the new Stardew Valley 1.5 feature for the remixed bundles if using this feature! This will overwrite this mod's randomization and result in the incorrect images being used for the bundle pictures.
  * Some bundles are completely random and select from most items in the game.
  * Optionally (on by default), the community center now has tooltips over the possible things to put in a bundle to make it easier to identify where to get fish and other items.
* Crafting recipe randomization
  * Recipes are now created based on randomly selected items from a pool (not randomly selected premade recipes)
  * Crafting difficulty is balanced based on necessity of the item and difficulty of crafting the item in vanilla
  * Setting to choose to randomize levels you unlock crafting recipes at - must also randomize the crafting recipes themselves to have it do anything
* Crop randomization
  * Crops, including fruits, vegetables, and flowers, have randomized (made-up) names, descriptions, prices (for both seeds and crops), and attributes (trellises, scythe needed, etc.)
  * Seeds, crops and growth stages have randomized images
  * Credits for the crop sprites not made by us (some images were modified from the originals):
    * Mizu - permission is assumed from this page: https://community.playstarbound.com/threads/mizus-sprites.136549/
    * Bonster - permission is assumed based on the bottom of the mod page for Bonster's Crops: https://www.nexusmods.com/stardewvalley/mods/3438
    * Marrorow
* Fish randomization
  * Fish have randomized (made-up) names, difficulty, and behavior
  * Locations, time-of-day, weather, and seasons are swapped as well
  * Fish have randomized images - most of them are from the More New fish mod: https://www.nexusmods.com/stardewvalley/mods/3578
  * Specific credits go to Hisame for the sprites: https://www.nexusmods.com/Users/51209496
* Forageable randomization
  * Forageables for every season and location are now randomly selected from all forageables + fruit (normally from trees)
  * Every forageable appears at least once per year, and some may appear more than once
* Fruit tree randomization
  * Fruit tree saplings are now item saplings that grow a randomly selected item
  * Prices will be randomized and are loosely balanced based on the item they give
* Weapon randomization
  * Weapon stats, types, etc. are randomized
  * Many weapons can now appear in mines containers
  * Setting to rename the Galaxy Sword, since there's a hard-coded check on wilderness farms to spawn a high-level bat if you have an item named "Galaxy Sword" in your inventory
  * Weapon images can be randomized
* Boot randomization
  * Stats are randomized
  * Names are randomized
  * Descriptions are randomized
  * Images are randomized
* Monster randomization
  * Stats are randomized: HP / Resilience / Speed / Experience
  * The threshold before a monster moves toward you is randomized
  * The time a monster moves randomly is randomized
  * Up to a 5% chance to be able to miss an attack on a monster
  * Setting to shuffle unique monster drops among all monsters (Slime, Bat Wing, Solar Essence, Bug Meat, Void Essence, and Squid Ink)
  * Each monster can now drop a new random item
* Blueprint randomization
  * Farm buildings that you get from Robin now choose from a more random pool of resources/items instead of a set list
  * This does not yet include anything you don't get from Robin (Obelisks, the Gold Clock, etc.)
* Music randomization
  * Most in-game songs and ambience are now randomly swapped 1 to 1 with another in-game song or ambience
  * Option to play a completely random song each time an area is moved to
* Quest randomization
  * Quest givers, required items, and rewards are randomly selected.
  * Help Wanted quests are unaffected, but the randomized item names should appear as expected.
* NPC birthday randomization
  * Randomizes the season and day of each NPC's birthday
  * Does not assign birthdays to the same day
  * Does not assign birthdays on the same day of most festivals (excludes night market and the moonlight jellies)
* Spoiler log
  * A spoiler log can be generated to see info about what was randomized
  * You must turn on this option in the settings to generate the log
* Misc
  * Bug fixes to prevent game crashing
  * Different variants of randomized rain can now appear in one playthrough (previously only one type per playthrough)
  * A random item is added to each location's artifact spot pool
  
## Stardew Valley 1.5 Compatibility Notes:
* In terms of the new items added, the following are randomized:
  * New weapons and boots
  * New enemies' stats and their random item drop
  * 3 of the new songs - if anyone knows how to get the song IDs of the rest of them, that would be helpful!
* The following are NOT randomized on the new island (these will come in a future update):
  * The new fish will be there as expected. Existing fish tooltips will NOT reflect whether the specific fish will be found on the island.
  * The new crops (including the fruit trees)
  * The random artifact spot drop
  * Any foragables

## Possible Future Features
* Palette randomization (if possible)
  * Randomly shift the color of the in-game graphics towards a different hue
* Enemy changes
  * New enemy sprites/colors
  * Change where enemies can appear
  * More varied drop randomization (they currently have all the same base drops)
* NPC schedule shuffle and/or randomization
* Cooking recipe randomization
* Tea trees and the new items associated with it need to be randomized
* Randomize store stock
  * The desert shop, specifically, is a good candidate for random stuff
* Randomize tool upgrade cost
* Randomize house upgrade cost
* Randomize tailoring recipes
* Add settings for more things, such as stats that can be modified on weapons, possible random names, etc.
  
## Known Issues
* Music restarts when you transition screens, even if it's the same song
* This mod does not fully support other languages (but it does for the most part)
  * Randomly generated weapon/crop names are English
  * The mad-lib style crop descriptions are English
  * Everything else has been internationalized
  
## Pending Changes for 0.5.0
* NPC Preference Randomizer (credit to Vertigon)
* Reworked the NPC skin randomizer so that most NPCs can be randomized with each other - exceptions (perhaps we can address these in the future as well):
  * Dwarf and Krobus, as they're too small
  * Pam and Willy, as they have fishing animations
  * Henchman and Bouncer, as they don't have that many sprites
* Fixed the rain, animal skins, and NPC skin randomizer settings potentially affecting the RNG for other things
* Added animal skins and NPC skin randomizer settings to the spoiler log
