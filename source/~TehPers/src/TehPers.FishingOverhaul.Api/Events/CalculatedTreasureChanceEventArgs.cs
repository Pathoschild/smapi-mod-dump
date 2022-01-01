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
    /// Event data for after the chance for a treasure chest is calculated.
    /// </summary>
    public class CalculatedTreasureChanceEventArgs : EventArgs
    {
        private double chanceForTreasure;

        /// <summary>
        /// Information about the <see cref="Farmer"/> that is fishing.
        /// </summary>
        public FishingInfo FishingInfo { get; }

        /// <summary>
        /// The chance of finding a treasure chest.
        /// </summary>
        public double ChanceForTreasure
        {
            get => this.chanceForTreasure;
            set => this.chanceForTreasure = Math.Clamp(value, 0.0, 1.0);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatedTreasureChanceEventArgs"/> class.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <param name="chanceForTreasure">The chance of finding a treasure chest.</param>
        /// <exception cref="ArgumentNullException">An argument was null.</exception>
        public CalculatedTreasureChanceEventArgs(FishingInfo fishingInfo, double chanceForTreasure)
        {
            this.FishingInfo = fishingInfo ?? throw new ArgumentNullException(nameof(fishingInfo));
            this.ChanceForTreasure = chanceForTreasure;
        }
    }
}