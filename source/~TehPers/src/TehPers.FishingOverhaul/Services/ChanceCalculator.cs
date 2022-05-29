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
using ContentPatcher;
using StardewModdingAPI;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Services
{
    internal class ChanceCalculator : ConditionsCalculator
    {
        private readonly AvailabilityInfo availabilityInfo;

        public ChanceCalculator(
            IMonitor monitor,
            IContentPatcherAPI contentPatcherApi,
            IManifest fishingManifest,
            IManifest owner,
            AvailabilityInfo availabilityInfo
        )
            : base(monitor, contentPatcherApi, fishingManifest, owner, availabilityInfo)
        {
            this.availabilityInfo = availabilityInfo
                ?? throw new ArgumentNullException(nameof(availabilityInfo));
        }

        /// <summary>
        /// Calculates the chance of catching this entry.
        /// </summary>
        /// <param name="fishingInfo">The fishing info to calculate the chance for.</param>
        /// <returns>The chance of catching this entry, or <see langword="null"/> if the entry is not available.</returns>
        public double? GetWeightedChance(FishingInfo fishingInfo)
        {
            if (!this.IsAvailable(fishingInfo))
            {
                return null;
            }

            return this.availabilityInfo.GetChance(fishingInfo);
        }
    }
}
