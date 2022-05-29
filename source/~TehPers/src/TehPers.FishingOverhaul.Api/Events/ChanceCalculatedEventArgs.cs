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
    /// Event data for when a chance is calculated.
    /// </summary>
    public class ChanceCalculatedEventArgs : EventArgs
    {
        private double chance;

        /// <summary>
        /// Information about the <see cref="Farmer"/> that is fishing.
        /// </summary>
        public FishingInfo FishingInfo { get; }

        /// <summary>
        /// The chance that was calculated.
        /// </summary>
        public double Chance
        {
            get => this.chance;
            set => this.chance = Math.Clamp(value, 0.0, 1.0);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ChanceCalculatedEventArgs"/> class.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <param name="chance">The calculated chance.</param>
        /// <exception cref="ArgumentNullException">An argument was null.</exception>
        public ChanceCalculatedEventArgs(FishingInfo fishingInfo, double chance)
        {
            this.FishingInfo = fishingInfo ?? throw new ArgumentNullException(nameof(fishingInfo));
            this.Chance = chance;
        }
    }
}
