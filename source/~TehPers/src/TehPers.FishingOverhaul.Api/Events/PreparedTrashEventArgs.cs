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
    /// Event data for after trash chances are calculated.
    /// </summary>
    public class PreparedTrashEventArgs : EventArgs
    {
        /// <summary>
        /// Information about the <see cref="Farmer"/> that is fishing.
        /// </summary>
        public FishingInfo FishingInfo { get; }

        /// <summary>
        /// The chances of finding trash. The weights are not yet normalized.
        /// </summary>
        public IList<IWeightedValue<TrashEntry>> TrashChances { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PreparedTrashEventArgs"/> class.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <param name="trashChances">The chances of finding trash.</param>
        /// <exception cref="ArgumentNullException">An argument was null.</exception>
        public PreparedTrashEventArgs(
            FishingInfo fishingInfo,
            IList<IWeightedValue<TrashEntry>> trashChances
        )
        {
            this.FishingInfo = fishingInfo ?? throw new ArgumentNullException(nameof(fishingInfo));
            this.TrashChances =
                trashChances ?? throw new ArgumentNullException(nameof(trashChances));
        }
    }
}