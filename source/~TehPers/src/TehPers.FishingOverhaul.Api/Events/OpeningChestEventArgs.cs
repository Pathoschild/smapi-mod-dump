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

namespace TehPers.FishingOverhaul.Api.Events
{
    /// <summary>
    /// Event data for whenever a treasure chest is opened.
    /// </summary>
    public class OpeningChestEventArgs : EventArgs
    {
        /// <summary>
        /// Information about the <see cref="Farmer"/> that is fishing.
        /// </summary>
        public FishingInfo FishingInfo { get; }

        /// <summary>
        /// The items in the chest.
        /// </summary>
        public IList<CaughtItem> CaughtItems { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpeningChestEventArgs"/> class.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <param name="caughtItems">The items in the chest.</param>
        /// <exception cref="ArgumentNullException">An argument was null.</exception>
        public OpeningChestEventArgs(FishingInfo fishingInfo, IList<CaughtItem> caughtItems)
        {
            this.FishingInfo = fishingInfo ?? throw new ArgumentNullException(nameof(fishingInfo));
            this.CaughtItems = caughtItems ?? throw new ArgumentNullException(nameof(caughtItems));
        }
    }
}