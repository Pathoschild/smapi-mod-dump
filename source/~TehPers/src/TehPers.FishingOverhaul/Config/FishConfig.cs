/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.ComponentModel;
using StardewModdingAPI;
using StardewValley;
using System;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Integrations.GenericModConfigMenu;

namespace TehPers.FishingOverhaul.Config
{
    /// <summary>
    /// Configuration for fish.
    /// </summary>
    /// <inheritdoc cref="IModConfig"/>
    public sealed class FishConfig : IModConfig
    {
        /// <summary>
        /// Whether to show the fish being caught in the fishing minigame.
        /// </summary>
        [DefaultValue(false)]
        public bool ShowFishInMinigame { get; set; }

        /// <summary>
        /// Modifier for the dart frequency (difficulty) of all fish.
        /// </summary>
        [DefaultValue(1.0)]
        public float GlobalDartFrequencyFactor { get; set; } = 1f;

        /// <summary>
        /// Affects how fast you catch fish.
        /// </summary>
        [DefaultValue(1f)]
        public float CatchSpeed { get; set; } = 1f;

        /// <summary>
        /// Affects how fast the catch bar drains when the bobber isn't on the fish.
        /// </summary>
        [DefaultValue(1f)]
        public float DrainSpeed { get; set; } = 1f;

        /// <summary>
        /// Required streak for an increase in quality. For example, 3 means that every 3
        /// consecutive perfect catches increases your catch quality by 1. A value of 0 or less
        /// disables any quality increase from your perfect fishing streak. (You still get a
        /// quality increase for perfectly catching a fish though.)
        /// </summary>
        [DefaultValue(3)]
        public int StreakForIncreasedQuality { get; set; } = 3;

        /// <summary>
        /// The max quality fish that can be caught. 0 = normal, 1 = silver, 2 = gold, 3 = iridium,
        /// 4+ = beyond iridium.
        /// </summary>
        [DefaultValue(3)]
        public int MaxFishQuality { get; set; } = 3;

        /// <summary>
        /// Determines the max quality fish a non-perfect catch can get, or null for no
        /// restrictions.
        /// </summary>
        [DefaultValue(null)]
        public int? MaxNormalFishQuality { get; set; }

        /// <summary>
        /// The chance that you'll find a fish instead of trash.
        /// </summary>
        public FishingChances FishChances { get; init; } = new()
        {
            BaseChance = 0.5,
            StreakFactor = 0.005,
            FishingLevelFactor = 0.025,
            DailyLuckFactor = 1,
            LuckLevelFactor = 0.01,
            MinChance = 0.1,
            MaxChance = 0.9,
        };

        void IModConfig.Reset()
        {
            this.ShowFishInMinigame = false;
            this.GlobalDartFrequencyFactor = 1f;
            this.CatchSpeed = 1f;
            this.DrainSpeed = 1f;
            this.StreakForIncreasedQuality = 3;
            this.MaxNormalFishQuality = null;
            this.MaxFishQuality = 3;

            // Fish chances
            this.FishChances.BaseChance = 0.5;
            this.FishChances.StreakFactor = 0.005;
            this.FishChances.FishingLevelFactor = 0.025;
            this.FishChances.DailyLuckFactor = 1;
            this.FishChances.LuckLevelFactor = 0.01;
            this.FishChances.MinChance = 0.1;
            this.FishChances.MaxChance = 0.9;
        }

        void IModConfig.RegisterOptions(
            IGenericModConfigMenuApi configApi,
            IManifest manifest,
            ITranslationHelper translations
        )
        {
            Translation Text(string key) => translations.Get($"text.config.{key}");
            Translation Name(string key) => translations.Get($"text.config.fish.{key}.name");
            Translation Desc(string key) => translations.Get($"text.config.fish.{key}.desc");

            configApi.AddBoolOption(
                manifest,
                () => this.ShowFishInMinigame,
                val => this.ShowFishInMinigame = val,
                () => Name("showFishInMinigame"),
                () => Desc("showFishInMinigame")
            );
            configApi.AddNumberOption(
                manifest,
                () => this.GlobalDartFrequencyFactor,
                val => this.GlobalDartFrequencyFactor = val,
                () => Name("globalDartFrequencyFactor"),
                () => Desc("globalDartFrequencyFactor"),
                0f,
                2f
            );
            configApi.AddNumberOption(
                manifest,
                () => this.CatchSpeed,
                val => this.CatchSpeed = val,
                () => Name("catchSpeed"),
                () => Desc("catchSpeed"),
                0f,
                3f
            );
            configApi.AddNumberOption(
                manifest,
                () => this.DrainSpeed,
                val => this.DrainSpeed = val,
                () => Name("drainSpeed"),
                () => Desc("drainSpeed"),
                0f,
                3f
            );
            configApi.AddNumberOption(
                manifest,
                () => this.StreakForIncreasedQuality,
                val => this.StreakForIncreasedQuality = val,
                () => Name("streakForIncreasedQuality"),
                () => Desc("streakForIncreasedQuality"),
                0,
                20
            );
            configApi.AddTextOption(
                manifest,
                () => this.MaxNormalFishQuality switch
                {
                    null => "disabled",
                    <= 0 => "basic",
                    1 => "silver",
                    2 => "gold",
                    >= 3 => "iridium",
                },
                val => this.MaxNormalFishQuality = val switch
                {
                    "basic" => 0,
                    "silver" => 1,
                    "gold" => 2,
                    "iridium" => 3,
                    "disabled" or _ => null,
                },
                () => Name("maxNormalFishQuality"),
                () => Desc("maxNormalFishQuality"),
                new[] {"disabled", "basic", "silver", "gold", "iridium"},
                val => Text(val)
            );
            configApi.AddTextOption(
                manifest,
                () => this.MaxFishQuality switch
                {
                    <= 0 => "basic",
                    1 => "silver",
                    2 => "gold",
                    >= 3 => "iridium",
                },
                val => this.MaxFishQuality = val switch
                {
                    "basic" => 0,
                    "silver" => 1,
                    "gold" => 2,
                    "iridium" or _ => 3,
                },
                () => Name("maxFishQuality"),
                () => Desc("maxFishQuality"),
                new[] {"basic", "silver", "gold", "iridium"},
                val => Text(val)
            );

            // Fish chances
            configApi.AddSectionTitle(
                manifest,
                () => Name("fishChances"),
                () => Desc("fishChances")
            );
            this.FishChances.RegisterOptions(configApi, manifest, translations);
        }

        /// <summary>
        /// Calculates the quality increase from a <see cref="Farmer"/>'s streak.
        /// </summary>
        /// <param name="streak">The streak.</param>
        /// <returns>The number of quality levels to increase the result by.</returns>
        public int GetQualityIncrease(int streak)
        {
            if (this.StreakForIncreasedQuality <= 0)
            {
                return 0;
            }

            return streak / this.StreakForIncreasedQuality;
        }

        /// <summary>
        /// Clamps the quality of a fish to allowed bounds. Also increases quality level 3 to
        /// quality level 4 (before clamping it).
        /// </summary>
        /// <param name="info">The catch info.</param>
        /// <returns>The clamped fish quality.</returns>
        public CatchInfo.FishCatch ClampQuality(CatchInfo.FishCatch info)
        {
            // Calculate max allowed fish quality
            var maxQuality =
                !info.State.IsPerfect && this.MaxNormalFishQuality is { } maxNormalFishQuality
                    ? Math.Min(maxNormalFishQuality, this.MaxFishQuality)
                    : this.MaxFishQuality;
            maxQuality = Math.Max(maxQuality, 0);

            // Calculate fish quality
            return info with
            {
                FishQuality = info.FishQuality switch
                {
                    // Basic
                    <= 0 => 0,
                    // Iridium (quality of 3 is adjusted to 4)
                    >= 3 when maxQuality >= 3 => 4,
                    >= 3 => maxQuality,
                    // Other
                    var quality when quality >= maxQuality => maxQuality,
                    var quality => quality,
                }
            };
        }
    }
}
