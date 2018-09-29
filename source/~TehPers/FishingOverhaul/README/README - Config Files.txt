Deleting config.json, fish.json, or treasure.json will make the mod regenerate the missing files with default values. This is useful when updating the mod.
Removing any property from config.json will make the mod regenerate that property with the default value.
Feel free to modify config.json, fish.json, or treasure.json all you'd like. The format is as follows:

[config.json]
 - ModEnabled: Whether the mod should actually do anything but load the configs. You can set this to false rather than delete the mod if you're trying to find mod conflicts.
 
 - OverrideFishing: Whether the mod should override the base fishing in the game. This option does not affect treasure loot.
 - OverrideTreasureLoot: Whether the mod should override your treasure loot. Does not affect fishing or chance of obtaining treasure.
 - OverrideLegendaries: Whether the mod should override the fish you're catching if it's a legendary. Disable for vanilla legendary catching rules. NOTE: You can still catch legendaries following the rules in fish.json, so remove them there if you'd like total vanilla legendary fishing.
 
 - BaseDifficultyMult: The difficulty multiplier for fishing. For example, 0.85 makes fishing 15% easier. 1.0 for vanilla
 - DifficultyStreakEffect: The effect your streak has on the difficulty of the fish. For example. 0.05 means fish are 5% more difficult for each consecutive perfect catch
 - CatchSpeed: Affects how fast you catch fish. 1.0 for vanilla
 - TreasureCatchSpeed: Affects how fast you catch treasure. 1.0 for vanilla
 - RecatchableLegendaries: Whether you can catch legendary fish multiple times. Vanilla is false. NOTE: This *INCLUDES* Legend. It is far more balanced this way.
 - TackleDestroyRate: How fast the tackle is destroyed. For example, 0.5 to make tackle break half as fast, and 2.0 to make it break twice as fast. 1.0 for vanilla

 - GetFishInWaterKey: Which key to press to list what fish you can catch right now. You may leave the quotes empty to remove this keybinding

 - TreasureChance: Affects the base chance you will find treasure. 0.15 for vanilla
 - TreasureBaitEffect: Effect that magnet bait has on your chance to find treasure. 0.15 for vanilla
 - TreasureBobberEffect: Effect that treasure hunter tackle has on your chance to find treasure. 0.05 for vanilla
 - TreasureLuckLevelEffect: Effect that your hidden luck level has on your chance to find treasure. 0.005 for vanilla
 - TreasureDailyLuckEffect: Effect that your daily luck has on your chance to find treasure. 0.5 for vanilla
 - TreasureStreakEffect: Effect that your streak has on your chance to find treasure.

 - UnawareMult: Difficulty multiplier for unaware fish
 - UnawareChance: Base chance that a fish will be unaware
 - UnawareLuckLevelEffect: Effect that your luck level has on the chance a fish will be unaware
 - UnawareDailyLuckEffect: Effect that your daily luck has on the chance a fish will be unaware

 - StreakForIncreasedQuality: Required streak for an increase in quality. For example, 3 means that every 3 consecutive perfect catches increases your catch quality by 1
 - PerfectTreasureQualityMult: Effect that catching treasure during a perfect catch has on the rarity of your treasure. The treasure is still random though

 - AdditionalLootChance: Chance that you'll obtain a second (or third, or fourth, etc) item from the treasure while fishing. 0.4 for vanilla
 - StreakAdditionalLootChance: Chance you'll obtain additional loot based on streak. For example, 0.01 gives 1% increase per consecutive perfect fish caught
 - AllowDuplicateLoot: Whether the treasure randomizer should be allowed to select the same loot option multiple times
 
 - StreakLootQuality: Effect that your streak has on the rarity of your treasure. For example, 0.015 means you'll have 1.5% higher chance to obtain treasure in general.
 - DailyLuckLootQuality: Effect that your streak has on the rarity of your treasure. For example, 1.0 means 100% of your daily luck bonus chance is added to your chance to obtain treasure in general.
 
 - MaxTreasureChance: Maximum chance you'll find treasure while fishing
 - MaxTreasureQualityMult: Maximum multiplier for treasure quality
 - MaxTreasureQuantity: Maximum amount of treasure you can find in a single chest while fishing

[treasure.json]
 - id / chance / minAmount / maxAmount: Self-explanitory. IDs can be looked up online, or you can find them in ObjectInformation.xnb
 - minCastDistance: Minimum distance the bobber must be from the player. If OverrideFishing is false, this will be ignored. Max is 5
 - minLevel / maxLevel: minimum and maximum fishing level required for that loot
 - idRange: The range of possible IDs, starting with "id", that can be obtained. For example, if "id" was 10, and "idRange" was 10, you'd be able to obtain anywhere from IDs 10 to 19
 - meleeWeapon: Whether it's a melee weapon or not. Melee weapons seem to have different IDs, stored in weapons.xnb

[fish.json]
 - <Location name>: These should all be filled out. You shouldn't need to add any locations unless you're using a mod that adds them.
   - <fish id>: Self-explanitory. Fish IDs can be found online or in Fish.xnb
     - Chance: The chance this fish will be chosen
	 - MinCastDistance: Minimum distance the bobber must be from the player. Max is 5
	 - WaterType: The body of water the fish must be in. Lake/Pond - 1, River - 2, Both - 3.
	 - MinTime / MaxTime: The minimum and maximum time of day you can catch the fish. Valid values are between 600 (6:00 am) and 2600 (2:00 am tomorrow).
	 - Season: Which seasons you can catch the fish in. Spring - 1, Summer - 2, Fall - 4, Winter - 8. To specify mutliple seasons, add the values together (for example, Spring + Summer = 3).
	 - MinLevel: Minimum required fishing level
	 - Weather: What kind of weather you can catch the fish in. Sunny - 1, Rainy - 2, Both - 3.
	 - MineLevel: What floor in the mines this fish can be caught in. This is ignored outside of the mines. You may include the same fish mutliple times in the same location.