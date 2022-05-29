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

namespace TehPers.FishingOverhaul.Api.Content
{
    /// <summary>
    /// Information about the availability of a catchable fish.
    /// </summary>
    /// <param name="BaseChance">The base chance of this being caught.</param>
    public record FishAvailabilityInfo(double BaseChance) : AvailabilityInfo(BaseChance)
    {
        /// <summary>
        /// Effect that sending the bobber by less than the max distance has on the chance. This
        /// value should be no more than 1.
        /// </summary>
        [DefaultValue(0.1d)]
        public double DepthMultiplier { get; init; } = 0.1d;

        /// <summary>
        /// The required fishing depth to maximize the chances of catching the fish.
        /// </summary>
        [DefaultValue(null)]
        public int MaxChanceDepth { get; init; } = 4;

        /// <summary>
        /// Obsolete - use <see cref="MaxChanceDepth"/> instead.<br/>
        /// The required fishing depth to maximize the chances of catching the fish.
        /// </summary>
        [Obsolete("Use " + nameof(FishAvailabilityInfo.MaxChanceDepth) + " instead.")]
        [DefaultValue(0)]
        public new int MaxDepth
        {
            get => this.MaxChanceDepth;
            init
            {
                this.MaxChanceDepth = value;
                this.ObsoletionWarnings = this.ObsoletionWarnings.Add(
                    $"{nameof(FishAvailabilityInfo.MaxDepth)} is obsolete. Use {nameof(FishAvailabilityInfo.MaxChanceDepth)} instead."
                );
            }
        }

        /// <inheritdoc/>
        public override double GetChance(FishingInfo fishingInfo)
        {
            var baseChance = base.GetChance(fishingInfo);
            var depthFactor = 1
                - Math.Max(0, this.MaxChanceDepth - fishingInfo.BobberDepth) * this.DepthMultiplier;
            return baseChance * depthFactor + fishingInfo.FishingLevel / 50.0f;
        }
    }
}
