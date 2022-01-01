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
using System.Collections.Generic;
using TehPers.FishingOverhaul.Api.Content;
using TehPers.FishingOverhaul.Api.Weighted;

namespace TehPers.FishingOverhaul.Api.Events
{
    /// <summary>
    /// Event data for after treasure chances are calculated.
    /// </summary>
    public class PreparedTreasureEventArgs : EventArgs
    {
        /// <summary>
        /// Information about the <see cref="Farmer"/> that is fishing.
        /// </summary>
        public FishingInfo FishingInfo { get; }

        /// <summary>
        /// The chances of finding treasure. The weights are not yet normalized.
        /// </summary>
        public IList<IWeightedValue<TreasureEntry>> TreasureChances { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PreparedTreasureEventArgs"/> class.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <param name="treasureChances">The chances of finding treasure.</param>
        /// <exception cref="ArgumentNullException">An argument was null.</exception>
        public PreparedTreasureEventArgs(
            FishingInfo fishingInfo,
            IList<IWeightedValue<TreasureEntry>> treasureChances
        )
        {
            this.FishingInfo = fishingInfo ?? throw new ArgumentNullException(nameof(fishingInfo));
            this.TreasureChances = treasureChances
                ?? throw new ArgumentNullException(nameof(treasureChances));
        }
    }
}