**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/drbirbdev/StardewValley**

----

# Binning Skill

A mod which adds a new skill, along with professions, perks, crafting recipes, and other content.

Binning is a skill for digging through trashcans around town.  Level it up to access better treasure, new trashcans, and additional perks.

## New in 1.6

Added more trashcans around the valley.  Find trashcans outside of town, usually near peoples homes.

Added higher tier trashcans with better loot, but they have level requirements to use.

Added new crafting recipes as level up perks.

* TrashCan - Place outside of your farm to get 1 trash per day. Help clean the valley by providing more convenience to villagers.
* Composter - Place unwanted food in here to create fertilizer. Better food makes better fertilizer. Reduce food waste.
* RecyclingBin - Place outside of your farm to get 1 item per day. Villagers sometimes want to recycle some pretty good stuff.
* AdvancedRecyclingMachine - Works like the Recycling Machine, but requires more trash, and provides better items.

Added new cooking recipes as level up perks.

* Grilled Cheese - increases binning level.
* Fish Casserole - increases binning and socializing level.

## Available Configs

* ExperienceFromCheckingTrash
* ExperienceFromCheckingRecycling
* ExperienceFromComposting
* ExperienceFromRecycling
* PerLevelBaseDropChanceBonus
* PerLevelRareDropChanceBonus
* MegaMinLevel
* DoubleMegaMinLevel
* RecyclingCountToGainFriendship
* RecyclingFriendshipGain
* RecyclingPrestigeFriendshipGain
* NoiseReduction
* PrestigeNoiseIncrease
* ReclaimerExtraValuePercent
* ReclaimerPrestigeExtraValuePercent

## Translation

Translation is automated by DeepL.  Feel free to PR suggested changes in your language.

## Available to Modders

### Patchable Content

* Mods/drbirbdev.BinningSkill/SkillTexture - Contains skill UI textures. See [here](assets/skill_texture.png) for sprite layout.
* Mods/drbirbdev.BinningSkill/GarbageHats - Contains hat textures.
* Mods/drbirbdev.BinningSkill/Items - Contains BigCraftable and Recipe textures.
* Mods/drbirbdev.BinningSkill/TrashCanTilesheet - Contains texture for tilesheet including trashcans.
* Mods/drbirbdev.BinningSkill/AnimationCopper - Contains texture for animating trash can searches.  There are also Iron, Gold, etc variants.

### Game State Queries:

* drbirbdev.BinningSkill_Level - Returns the current Binning Skill level.
* drbirbdev.BinningSkill_Random - Returns whether a random check passed, accounting for Binning Skill level and configs.

### Custom Fields on GarbageCans:

* drbirbdev.BinningSkill_AddToMap - Adds corresponding trash can to a map. Takes 4 space-delimited arguments.
  * Location - which map to add the trash can to.
  * X - the x coordinate of the base of the can.
  * Y - the y coordinate of the base of the can.
  * Level - (optional) the tier of the can (Default, Copper, Iron, Gold, Iridium, Radioactive, or Prismatic).  MinLevel, AnimationTexture, and NoiseLevel will be set based on this.
* drbirbdev.BinningSkill_MinLevel - Minimum Binning Skill level required to access this trash can. Uses same syntax as vanilla [RANDOM](https://stardewvalleywiki.com/Modding:Game_state_queries#Randomization)
* drbirbdev.BinningSkill_AnimationTexture - Custom texture to use when animating garbage can search. See [here](./assets/animation_copper_can.png) for sprite layout.
* drbirbdev.BinningSkill_NoiseLevel - Custom noise level to use when searching trash can (range for villagers to catch you).  Defaults to 7 in vanilla.

# Roadmap

* Add trashcans to popular mod locations (SVE, Ridgeside Village, East Scarpe, Downtown Zuzu, etc).
* Gauge interest in turning off certain trashcans in config. (For easier compatibility, aesthetic).
