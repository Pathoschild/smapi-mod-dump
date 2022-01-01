/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using StardewValley;
using System;

namespace TehPers.FishingOverhaul.Api.Events
{
    /// <summary>
    /// Event data for after the chance for a fish is calculated.
    /// </summary>
    public class CalculatedFishChanceEventArgs : EventArgs
    {
        private double chanceForFish;

        /// <summary>
        /// Information about the <see cref="Farmer"/> that is fishing.
        /// </summary>
        public FishingInfo FishingInfo { get; }

        /// <summary>
        /// The chance of hitting a fish (instead of trash).
        /// </summary>
        public double ChanceForFish
        {
            get => this.chanceForFish;
            set => this.chanceForFish = Math.Clamp(value, 0.0, 1.0);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatedFishChanceEventArgs"/> class.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <param name="chanceForFish">The chance of hitting a fish (instead of trash).</param>
        /// <exception cref="ArgumentNullException">An argument was null.</exception>
        public CalculatedFishChanceEventArgs(FishingInfo fishingInfo, double chanceForFish)
        {
            this.FishingInfo = fishingInfo ?? throw new ArgumentNullException(nameof(fishingInfo));
            this.ChanceForFish = chanceForFish;
        }
    }
}