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
using System.Collections.Generic;
using System.ComponentModel;
using StardewModdingAPI;
using StardewValley;
using TehPers.FishingOverhaul.Integrations.GenericModConfigMenu;

namespace TehPers.FishingOverhaul.Config
{
    /// <summary>
    /// Configuration for the chances of catching something while fishing.
    /// </summary>
    /// <inheritdoc cref="IModConfig"/>
    public class FishingChances : IModConfig
    {
        /// <summary>
        /// The base chance. Total chance is calculated as
        /// locationFactor * (baseChance + sum(factor * thing it's a factor of)), bounded in the
        /// range [minChance, maxChance], then bounded once again in the range [0, 1].
        /// </summary>
        [DefaultValue(0d)]
        public double BaseChance { get; set; }

        /// <summary>
        /// The effect that streak has on this chance.
        /// </summary>
        [DefaultValue(0d)]
        public double StreakFactor { get; set; }

        /// <summary>
        /// The effect that fishing level has on this chance.
        /// </summary>
        [DefaultValue(0d)]
        public double FishingLevelFactor { get; set; }

        /// <summary>
        /// The effect that daily luck has on this chance.
        /// </summary>
        [DefaultValue(0d)]
        public double DailyLuckFactor { get; set; }

        /// <summary>
        /// The effect that luck level has on this chance.
        /// </summary>
        [DefaultValue(0d)]
        public double LuckLevelFactor { get; set; }

        /// <summary>
        /// The minimum possible chance.
        /// </summary>
        [DefaultValue(0d)]
        public double MinChance { get; set; }

        /// <summary>
        /// The maximum possible chance.
        /// </summary>
        [DefaultValue(1d)]
        public double MaxChance { get; set; } = 1d;

        internal virtual void Reset()
        {
            this.BaseChance = default;
            this.StreakFactor = default;
            this.FishingLevelFactor = default;
            this.DailyLuckFactor = default;
            this.LuckLevelFactor = default;
            this.MinChance = 0d;
            this.MaxChance = 1d;
            this.LocationFactors.Clear();
        }

        void IModConfig.Reset()
        {
            this.Reset();
        }

        /// <summary>
        /// The effects that specific locations have on this chance. Keys are location names and
        /// values are their factors.
        /// </summary>
        public Dictionary<string, double> LocationFactors { get; set; } = new();

        internal virtual void RegisterOptions(
            IGenericModConfigMenuApi configApi,
            IManifest manifest,
            ITranslationHelper translations
        )
        {
            Translation Name(string key) => translations.Get($"text.config.chances.{key}.name");
            Translation Desc(string key) => translations.Get($"text.config.chances.{key}.desc");

            configApi.AddNumberOption(
                manifest,
                () => (float)this.BaseChance,
                val => this.BaseChance = val,
                () => Name("baseChance"),
                () => Desc("baseChance"),
                0f,
                1f
            );
            configApi.AddNumberOption(
                manifest,
                () => (float)this.StreakFactor,
                val => this.StreakFactor = val,
                () => Name("streakFactor"),
                () => Desc("streakFactor"),
                0f,
                1f
            );
            configApi.AddNumberOption(
                manifest,
                () => (float)this.FishingLevelFactor,
                val => this.FishingLevelFactor = val,
                () => Name("fishingLevelFactor"),
                () => Desc("fishingLevelFactor"),
                0f,
                1f
            );
            configApi.AddNumberOption(
                manifest,
                () => (float)this.DailyLuckFactor,
                val => this.DailyLuckFactor = val,
                () => Name("dailyLuckFactor"),
                () => Desc("dailyLuckFactor"),
                0f,
                1f
            );
            configApi.AddNumberOption(
                manifest,
                () => (float)this.LuckLevelFactor,
                val => this.LuckLevelFactor = val,
                () => Name("luckLevelFactor"),
                () => Desc("luckLevelFactor"),
                0f,
                1f
            );
            configApi.AddNumberOption(
                manifest,
                () => (float)this.MinChance,
                val => this.MinChance = val,
                () => Name("minChance"),
                () => Desc("minChance"),
                0f,
                1f
            );
            configApi.AddNumberOption(
                manifest,
                () => (float)this.MaxChance,
                val => this.MaxChance = val,
                () => Name("maxChance"),
                () => Desc("maxChance"),
                0f,
                1f
            );
            configApi.AddParagraph(manifest, () => Desc("locationFactors"));
        }

        void IModConfig.RegisterOptions(
            IGenericModConfigMenuApi configApi,
            IManifest manifest,
            ITranslationHelper translations
        )
        {
            this.RegisterOptions(configApi, manifest, translations);
        }

        /// <summary>
        /// Gets the unclamped chance for a <see cref="Farmer"/>.
        /// </summary>
        /// <param name="farmer">The farmer to calculate the chance for.</param>
        /// <param name="streak">The farmer's fishing streak.</param>
        /// <returns>The calculated chance for that farmer.</returns>
        public virtual double GetUnclampedChance(Farmer farmer, int streak)
        {
            // Base chance
            var chance = this.BaseChance;

            // Luck
            chance += farmer.LuckLevel * this.LuckLevelFactor;
            chance += farmer.DailyLuck * this.DailyLuckFactor;

            // Stats
            chance += farmer.FishingLevel * this.FishingLevelFactor;

            // Streak
            chance += streak * this.StreakFactor;

            // Location
            if (farmer.currentLocation is {Name: { } locationName}
                && this.LocationFactors.TryGetValue(locationName, out var factor))
            {
                chance += factor;
            }

            return chance;
        }

        /// <summary>
        /// Clamps the chance to an allowed value. This does not take any additional adjustments
        /// by other mods into account. For example, a mod may change the range of valid values
        /// for the chance.
        /// </summary>
        /// <param name="chance">The chance to clamp.</param>
        /// <returns>The clamped chance.</returns>
        public double ClampChance(double chance)
        {
            return Math.Clamp(chance, this.MinChance, this.MaxChance);
        }
    }
}
