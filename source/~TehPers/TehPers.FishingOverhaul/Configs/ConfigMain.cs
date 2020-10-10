/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.ComponentModel;
using TehPers.Core.Json.Serialization;

namespace TehPers.FishingOverhaul.Configs {

    [JsonDescribe]
    public class ConfigMain {
        [Description("Whether or not this mod should make changes to the game.")]
        public bool ModEnabled { get; set; } = true;

        [Description("Whether to make the config files as small as possible. This makes them really hard to edit!")]
        public bool MinifyConfigs { get; set; } = false;

        [Description("Whether this mod affects legendary fish as well. If this is false, then you should never be able to catch legendary fish multiple times.")]
        public bool CustomLegendaries { get; set; } = true;

        [Description("Whether you can catch legendary fish multiple times (including Legend). This setting only matters if " + nameof(ConfigMain.CustomLegendaries) + " is true. False for vanilla")]
        public bool RecatchableLegendaries { get; set; } = true;

        [Description("WIP, doesn't work yet - If legendary fish are recatchable, how many days you must wait before you can catch a particular legendary fish again. Set to 0 for no delay.")]
        public int DaysBeforeLegendaryRecatchable { get; set; } = 28;

        [Description("Whether the fish should be shown while catching it. False for vanilla")]
        public bool ShowFish { get; set; } = true;

        [Description("Whether or not to show current streak, chance for treasure, chance for each fish, etc. while fishing.")]
        public bool ShowFishingData { get; set; } = true;

        [Description("The X coordinate of the top left corner of the fishing HUD.")]
        public int HudTopLeftX { get; set; } = 0;

        [Description("The Y coordinate of the top left corner of the fishing HUD.")]
        public int HudTopLeftY { get; set; } = 0;

        [Description("Settings for streaks.")]
        public ConfigStreak StreakSettings { get; set; } = new ConfigStreak();

        [Description("Global settings for how difficult fishing should be.")]
        public ConfigDifficulty DifficultySettings { get; set; } = new ConfigDifficulty();

        [Description("Global settings for fish.")]
        public ConfigGlobalFish GlobalFishSettings { get; set; } = new ConfigGlobalFish();

        [Description("Global settings for treasure.")]
        public ConfigGlobalTreasure GlobalTreasureSettings { get; set; } = new ConfigGlobalTreasure();

        [Description("Settings for the unaware fish event.")]
        public ConfigUnaware UnawareSettings { get; set; } = new ConfigUnaware();

        [JsonDescribe]
        public class ConfigStreak {
            [Description("Required streak for an increase in quality. For example, 3 means that every 3 consecutive perfect catches increases your catch quality by 1.")]
            public int StreakForIncreasedQuality { get; set; } = 5;

            [Description("Effect that catching treasure during a perfect catch has on the rarity of your treasure. The treasure is still random though.")]
            public float PerfectTreasureQualityMult { get; set; } = 5;
        }

        [JsonDescribe]
        public class ConfigDifficulty {
            [Description("The difficulty multiplier for fishing. For example, 0.85 makes fishing 15% easier. 1.0 for vanilla")]
            public float BaseDifficultyMult { get; set; } = 0.85F;

            [Description("The effect your streak has on the difficulty of the fish. For example. 0.05 means fish are 5% more difficult for each consecutive perfect catch")]
            public float DifficultyStreakEffect { get; set; } = 0.05F;

            [Description("Affects how fast you catch fish. 1.0 for vanilla")]
            public float CatchSpeed { get; set; } = 1F;

            [Description("Affects how fast the catch bar drains when the bobber isn't on the fish. 1.0 for vanilla")]
            public float DrainSpeed { get; set; } = 1F;

            [Description("Affects how fast you catch treasure. 1.0 for vanilla")]
            public float TreasureCatchSpeed { get; set; } = 1.25F;

            [Description("Affects how fast the treasure bar drains when the bobber isn't on the chest. 1.0 for vanilla")]
            public float TreasureDrainSpeed { get; set; } = 1.25F;
            
            [Description("How many times a tackle can be used before it breaks. 20 for vanilla")]
            public int MaxTackleUses { get; set; } = 20;

            [Description("Determines whether a perfect catch is required to get gold quality (or above) fish. True means you can only get normal or silver quality fish on non-perfect catches. False for vanilla")]
            public bool PreventGoldOnNormalCatch { get; set; } = true;
        }

        [JsonDescribe]
        public class ConfigGlobalFish {
            [Description("The base chance that you'll find a fish instead of trash.")]
            public float FishBaseChance { get; set; } = 0.5F;

            [Description("Effect your fishing level has on your chance to find a fish. For example, 0.025 means you have +2.5% chance to find a fish per fishing level.")]
            public float FishLevelEffect { get; set; } = 0.025F;

            [Description("Effect your luck level has on your chance to find a fish. For example, 0.01 means +1% chance to find a fish per luck level.")]
            public float FishLuckLevelEffect { get; set; } = 0.01f;

            [Description("Effect your daily luck has on your chance to find a fish.")]
            public float FishDailyLuckEffect { get; set; } = 1f;

            [Description("Effect your streak has on your chance to find a fish. For example, 0.01 means +1% chance per perfect catch in your streak.")]
            public float FishStreakEffect { get; set; } = 0.005f;

            [Description("Whether all farm types should have fish. The default farm fish can be defined in fish.json. Vanilla is false")]
            public bool AllowFishOnAllFarms { get; set; } = false;

            [Description("The minimum chance of catching a fish.")]
            public float MinFishChance { get; set; } = 0;

            [Description("The maximum chance of catching a fish.")]
            public float MaxFishChance { get; set; } = 1;
        }

        [JsonDescribe]
        public class ConfigGlobalTreasure {
            [Description("Affects the base chance you will find treasure. 0.15 for vanilla")]
            public float TreasureChance { get; set; } = 0.15f;

            [Description("Effect that magnet bait has on your chance to find treasure. 0.15 for vanilla")]
            public float TreasureBaitEffect { get; set; } = 0.15f;

            [Description("Effect that treasure hunter tackle has on your chance to find treasure. 0.05 for vanilla")]
            public float TreasureBobberEffect { get; set; } = 0.05f;

            [Description("Effect that your hidden luck level has on your chance to find treasure. 0.005 for vanilla")]
            public float TreasureLuckLevelEffect { get; set; } = 0.005F;

            [Description("Effect that your daily luck has on your chance to find treasure. 0.5 for vanilla")]
            public float TreasureDailyLuckEffect { get; set; } = 0.5F;

            [Description("Effect that your streak has on your chance to find treasure.")]
            public float TreasureStreakEffect { get; set; } = 0.01f;

            [Description("Chance that you'll obtain a second (or third, or fourth, etc) item from the treasure while fishing. 0.4 for vanilla")]
            public float AdditionalLootChance { get; set; } = 0.5f;

            [Description("Chance you'll obtain additional loot based on streak. For example, 0.01 gives 1% increase per consecutive perfect fish caught.")]
            public float StreakAdditionalLootChance { get; set; } = 0.01f;

            [Description("Whether the treasure randomizer should be allowed to select the same loot option multiple times.")]
            public bool AllowDuplicateLoot { get; set; } = true;

            [Description("Effect that your streak has on the rarity of your treasure. For example, 0.015 means you'll have 1.5% higher chance to obtain treasure in general.")]
            public float StreakLootQuality { get; set; } = 0.01f;

            [Description("Effect that your streak has on the rarity of your treasure. For example, 1.0 means 100% of your daily luck bonus chance is added to your chance to obtain treasure in general.")]
            public float DailyLuckLootQuality { get; set; } = 1f;

            [Description("Maximum chance that you'll find treasure while fishing.")]
            public float MaxTreasureChance { get; set; } = 0.5f;

            [Description("Maximum multiplier for treasure quality.")]
            public float MaxTreasureQualityMult { get; set; } = 7.5f;

            [Description("Maximum amount of treasure you can find in a single chest while fishing.")]
            public int MaxTreasureQuantity { get; set; } = 3;
        }

        [JsonDescribe]
        public class ConfigUnaware {
            [Description("Difficulty multiplier for unaware fish.")]
            public float UnawareMult { get; set; } = 0.5f;

            [Description("Base chance that a fish will be unaware.")]
            public float UnawareChance { get; set; } = 0.01f;

            [Description("Effect that your luck level has on the chance a fish will be unaware.")]
            public float UnawareLuckLevelEffect { get; set; } = 0.01F;

            [Description("Effect that your daily luck has on the chance a fish will be unaware.")]
            public float UnawareDailyLuckEffect { get; set; } = 0.5F;
        }
    }
}
