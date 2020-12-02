**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/Chikakoo/stardew-valley-randomizer**

----

# Stardew Valley Randomizer (More Random Edition)

An update for cTooshi's Stardew Valley Randomizer to fix errors, add new features and make the existing features more random.

## Installation

Make sure you have SMAPI installed (https://stardewvalleywiki.com/Modding:Installing_SMAPI_on_Windows), then download the latest release of the Randomizer and unzip into your Stardew Valley Mods folder.

## Changes from Original Randomizer

* Bundle randomization
  * New bundles for each room with random items selected from themed pools and random number of those items required
  * Some bundles are completely random and select from most items in the game.
* Crafting recipe randomization
  * Recipes are now created based on randomly selected items from a pool (not randomly selected premade recipes)
  * Crafting difficulty is balanced based on necessity of the item and difficulty of crafting the item in vanilla
  * Setting to choose to randomize levels you unlock crafting recipes at - must also randomize the crafting recipes themselves to have it do anything
* Crop randomization
  * Crops, including fruits, vegetables, and flowers, have randomized (made-up) names, descriptions, prices (for both seeds and crops), and attributes (trellises, scythe needed, etc.)
  * This also includes custom images for all seeds and saplings to reduce confusion
* Fish randomization
  * Fish have randomized (made-up) names, difficulty, and behavior
  * Locations, time-of-day, weather, and seasons are swapped as well
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
  * Weapon images can be randomized, based on some custom weapon sprites
    * If you go to Assets/CustomImages/Weapons, you can add images to the relevant folder (Slingshots unused currently).
      * They should be 16 x 16 - if they aren't, the graphics will end up looking weird
      * They must end in .png to be potentially picked up by the randomizer
      * Try not to use images with an indexed color palette -  this can cause crashes (on Macs, specifically)
      * If you delete any, make sure there are at least 49 total in the folder, or you risk needing to fall back to the default image
* Boot randomization
  * Stats are randomized
  * Names are randomized
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

## Possible Future Features
* Graphics changes
  * New sprites for crops (item and growing sprites)
  * New sprites for fish
* Palette randomization (if possible)
  * Randomly shift the color of the in-game graphics towards a different hue
* Enemy changes
  * New enemy sprites/colors
  * Change where enemies can appear
  * More varied drop randomization (they currently have all the same base drops)
* NPC schedule shuffle and/or randomization
* Cooking recipe randomization
* Tea trees and the new items associated with it need to be randomized
* Stables need to be randomized still
* Randomize store stock
  * The desert shop, specifically, is a good candidate for random stuff
* Randomize tool upgrade cost
* Randomize house upgrade cost
* Randomize tailoring recipes
* Add settings for more things, such as stats that can be modified on weapons, possible random names, etc.
  
## Known Issues
* Queen of Sauce recipes don't use the randomized crop names for the dishes that you learn how to cook
* This mod does not fully support other languages (but it does for the most part)
  * Randomly generated weapon/crop names are English
  * The mad-lib style crop descriptions are English
  * Everything else has been internationalized

## Pending Changes on the 0.4.0 branch
* Crop changes
  * When randomizing crops, there's the option of using custom crop images. This will link together the crop as it's growing, the seed packet, and the crop itself into a coherent set of images.
      * Credits for the crop sprites not made by us (some images were modified from the originals):
        * Mizu - permission is assumed from this page: https://community.playstarbound.com/threads/mizus-sprites.136549/
        * Bonster - permission is assumed based on the bottom of the mod page for Bonster's Crops: https://www.nexusmods.com/stardewvalley/mods/3438
        * Marrorow
  * Wild seeds will now grow one of the randomized foragables of the season
  * Crop text fixes
    * The Queen of Sauce show now uses the randomized crop/fish names
    * The text in the Secret Woods that tells you to bring a Sweet Gem Berry will now specifically tell you what crop you need to bring, since there's now no way to tell otherwise
    * The Mr Qi quest now mentions which 10 crops to put in the Mayor's fridge for the initial textbox, and not just the quest description
  * Wild seed recipies now require 4 random foragables from the appropriate season
* Fish Changes
  * When randomizing fish, there's the option of using custom fish images
    * The majority of the new fish images on the FishRework branch are mostly directly pulled form the New New Fish mod: https://www.nexusmods.com/stardewvalley/mods/3578
      * Specific credits go to Hisame for the sprites: https://www.nexusmods.com/Users/51209496
  * Legendary fish can now be identified from their tooltip
  * Legendary fish now show up in the spoiler log
* Bundle changes
  * The new bundles now have matching images
  * Made the Rare Foods bundle based off of the crop seeds, rather than the crop
    * This means that Ancient Seeds, the old Starfruit Seeds (from the desert shop), and the Rare Seeds (from the traveling cart) will be requried
  * The fish in the Night market submarine should no longer appear for season-specific bundles that are not winter
  * As a QoL feature, added the option (it's on by default) for a tooltip over the possible things to put into a bundle - this will make it easier to know where/when to get the fish
* Balances changes
  * Frozen tears were marked as easy to get - this has been rebalanced, since it does take time to get there
  * Mead was marked as easy to get - this has been rebalanced since this could potentialaly take a long time to obtain
  * Clay
    * Marked it as harder to get, since it's annoying to get it in bulk
    * Less is now required if chosen for a recipe
  * Rebalanced all animal products to better reflect how long it takes to get them - should result in more reasonable crafting recipes
* Boots now have custom images and descriptions (descriptions are enabled in English only)
* Added the Stable to the list of randomized buildings, as it was overlooked before
* Added Desert Totems to the item pool, and included them in the appropriate bundles
* Fixed an issue where the crop pot recipe is learned twice
* Reworked the config file to be more readable/user friendly
* Reworked logging to log at appropriate levels (Trace/Warn/Error)
* Removed a track called "coin" form the random music list - this seems to just be a sound effect used for picking up an item, and not a real song (so it was really annoying to listen to on loop!)
* The weather string for fish in Korean has been fixed
* Error Item bugfixes
  * You could get "Error Item" as a reward from a bundle - this was due to the "Any Fish" item being rewarded. This has been fixed.
  * If a building required the "Any Fish" item to build it, it showed up as "Error Item". This has been fixed by removing "Any Fish" from that pool.
