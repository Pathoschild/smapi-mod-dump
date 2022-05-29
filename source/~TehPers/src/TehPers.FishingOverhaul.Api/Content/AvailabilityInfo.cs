/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// Information about the availability of a catchable item.
    /// </summary>
    /// <param name="BaseChance">
    /// The base chance this will be caught. This is not a percentage chance, but rather a weight
    /// relative to all available entries.
    /// </param>
    public record AvailabilityInfo
        ([property: JsonRequired] double BaseChance) : AvailabilityConditions
    {
        /// <summary>
        /// The priority tier for this entry. For all available entries, only the entries that
        /// share the highest priority tier can be caught. For example, if all trash entries have a
        /// tier of 0 except for a single entry that has a tier of 1, then that single entry is
        /// guaranteed to be caught, regardless of its calculated weighted chance. If that entry
        /// becomes unavailable, then entries from tier 0 are selected from instead. This can be
        /// useful when creating special items that should always be caught first but which can only
        /// be caught once, for example.
        /// </summary>
        [DefaultValue(0d)]
        public double PriorityTier { get; init; } = 0d;

        /// <summary>
        /// Gets the weighted chance of this being caught, if any. This does not test the
        /// conditions in <see cref="AvailabilityConditions.When"/>.
        /// </summary>
        /// <param name="fishingInfo">Information about the farmer that is fishing.</param>
        /// <returns>The weighted chance of this being caught, or <see langword="null"/> if not available.</returns>
        [Obsolete("Use " + nameof(AvailabilityInfo.GetChance) + " instead.")]
        public double? GetWeightedChance(FishingInfo fishingInfo)
        {
            return this.IsAvailable(fishingInfo) ? this.GetChance(fishingInfo) : null;
        }

        /// <summary>
        /// Gets the weighted chance of this being caught. This does not test the conditions in
        /// <see cref="AvailabilityConditions.When"/>.
        /// </summary>
        /// <param name="fishingInfo">Information about the farmer that is fishing.</param>
        /// <returns>The weighted chance of this being caught.</returns>
        public virtual double GetChance(FishingInfo fishingInfo)
        {
            // Calculate spawn weight
            return this.BaseChance;
        }
    }
}
