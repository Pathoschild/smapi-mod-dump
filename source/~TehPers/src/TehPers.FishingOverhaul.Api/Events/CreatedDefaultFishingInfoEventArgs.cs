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
    /// Event data for after the default fishing info is created.
    /// </summary>
    public class CreatedDefaultFishingInfoEventArgs : EventArgs
    {
        /// <summary>
        /// Information about the <see cref="Farmer"/> that is fishing.
        /// </summary>
        public FishingInfo FishingInfo { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatedDefaultFishingInfoEventArgs"/> class.
        /// </summary>
        /// <param name="fishingInfo">Information about the <see cref="Farmer"/> that is fishing.</param>
        /// <exception cref="ArgumentNullException">An argument was null.</exception>
        public CreatedDefaultFishingInfoEventArgs(FishingInfo fishingInfo)
        {
            this.FishingInfo = fishingInfo ?? throw new ArgumentNullException(nameof(fishingInfo));
        }
    }
}