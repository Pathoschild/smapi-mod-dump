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
* [Coming in 0.6.0 release] Palette randomization (if possible)
  * Randomly shift the color of the in-game graphics towards a different hue
* Enemy changes
  * [Colors coming in 0.6.0 release] New enemy sprites/colors
  * Change where enemies can appear
  * More varied drop randomization (they currently have all the same base drops)
* NPC schedule shuffle and/or randomization
* Cooking recipe randomization
* Tea trees and the new items associated with it need to be randomized
* [Coming in 0.6.0 release] Randomize store stock
  * The desert shop, specifically, is a good candidate for random stuff
* Randomize tool upgrade cost
* Randomize house upgrade cost
* Randomize tailoring recipes
* Add settings for more things, such as stats that can be modified on weapons, possible random names, etc.
  
## Known Issues
* [Fixed in upcoming 0.6.0 release] Special orders:
  * With certain orders, there's no guarantee that any of the item being requested will be possible to get (fish, crops, etc.)
    * There's also no way of actually knowing whether the fish is actually valid for the special order
  * We need to modify the ObjectContextTags to take the randomization in mind so that these orders will actually be possible
* Music restarts when you transition screens, even if it's the same song
* Issues with non-English languages being inconsistent with the letter bundles
  * As such to avoid any issues, do not switch your language after starting a seed, and be consistent which languages are used when playing multiplayer
* This mod does not fully support other languages (but it does for the most part)
  * Randomly generated weapon/crop names are English
  * The mad-lib style crop descriptions are English
  * Everything else has been internationalized
  
## Pending Changes for 0.6.0
* Shop randomization (credit to Vertigon for doing the initial work for this) - note that each of these have their own associated setting
  * Pierre's Seed Shop
    * Random limited-stock of the week that's more expensive than the base price
  * Joja Mart
    * Random limited-stock of the week that's more expensive than the base price (but cheaper than Pierre's would be) 
  * Blacksmith Shop - every day, a chance at one of the following
    * (50%) Discount an ore by 10-25%
    * (35%) A chance of a random bar being added to the shop, with limited stock depending on the bar
    * (12%) Add an artifact to the shop (stock of 1)
    * (3%) Add 5-15 iridium ore to the shop at 5x the base cost
  * Robin's Carpenter Shop (changes daily)
    * Add 20-40 clay to the stock at a random price between 25-75
    * Add a random item that's use to craft tappers, since they can be hard to get (stock limited by the amount to craft 1 tapper)
  * Saloon - changes weekly
    * Beer and Coffee always available
    * 3-5 random cooked foods
    * 3-5 random recipes (not added if the player has learned them)
  * Desert Oasis Shop - random based on the day
    * Weekdays - 1-5 of a random desert foragables
    * Tuesdays - 3-8 of a random crop that corresponds to the seeds sold here
    * Weekends - A random cooked item
    * Every week...
      * Adds a couple random craftable/resource items
      * Replaces all clothing and furniture items with random ones
  * Krobus' Sewer Shop - changes daily
    * Replace the Monster Fireplace with a random Furniture item
    * Replace the Sign of the Vessel with a random BigCraftable item
  * Hat Shop
    * Hat of the week
  * Club/Casino Shop - each week, entire shop is replaced with...
    * 3-5 random furniture
    * A random hat or clothing item
    * A random BigCraftable item
    * 2-3 misc items, such as resources
    * A random totem, always costing 500
* NPC preference randomizer (credit to Vertigon)
  * Includes randomizing secret notes to give random loved item info
* Support the GenericModConfigMenuApi mod to more easily manage settings
  * https://www.nexusmods.com/stardewvalley/mods/5098
* Horse and pet randomizer reworked
  * These are now separate settings, so they can both be active at the same time
  * Added new horse and pet images, modified from the original assets, from Time Fantasy assets, and misc free RPG Maker assets
    * https://www.timefantasy.net/
* Random hue-shifting of certain assets
  * Crops - will impact the crop/seed/plant in the same way, for consistency
  * Fish
  * Boots
  * Monsters
  * Pet/Horses (based on the filename - see the readme in /Assets/CustomImages for details)
* Redid rain randomization to allow for custom rain graphics
  * See the readme in /Assets/CustomImages for details
* Randomize museum rewards
  * Each reward will be randomized to another one in the same category
  * Certain important rewards will remain the same, including the Dwarven Translation Guide, the Ancient Seeds, and the Stardrop
* Tooltip changes (for Community Center convenience)
  * Crops now have their planting season(s) listed on them
  * Foragables now have the seasons they can be foraged in listed on them
* Added more random name strings/descriptions for a bit more variety
* Fixed bugs
  * Fixed rings not being able to be deposited... again
  * Fixed issues with cooking recipes (tooltip & unlocks for ones with changed names not working)
  * Fixed the rain, and animal skins potentially affecting the RNG for other things
    * Additionally, rain should now always be the same for all players on a given day
  * Fixed the name of the "BlueBean" seed/crop image, as it had a casing inconsistency that caused errors in the console if chosen
  * Fixed fish special orders
    * Aquatic Overpopulation - should pick a fish that's actually catchable during this season
    * Biome Balance - for ease of doing logic for this, the following fish work for this:
     * River = Town
     * Ocean = Beach
     * Lake = Mountain
  * Fixed a bug where random weapon defenses were mistakenly initialized to the precision value
     * Redid defense randomization as a result to be more reasonable
  * Fixed several crashes/inconsistencies/issues with non-English locales
* Removed features
  * NPC skin randomizer - most image sizes are not compatible with each other, and could result in glitchy graphics
    * I may look into a better way to do something like this in the future
  * The Galaxy Sword name randomization setting
    * A fix was made so that the current sword name is checked when determing to spawn the bat in a wilderness farm, meaning this setting is no longer necessary
* Technical/FYI changes
  * Code rework to redo how menus are modified (done in a much safer way). Credit to Vertigon for the initial work on a bunch of these changes!
  * Look up Stardew xnb data as appropriate instead of hard-coding the expected values
    * This should allow for things to work properly more consistently across game updates, should this data ever change
  * Shorted weapon names on average, as it could roll a ridiculously long name (they can still be long and ridiculous, though, just not as much!)
  * In summary, a ton of misc code cleanup
