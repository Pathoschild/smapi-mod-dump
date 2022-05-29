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
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Services
{
    internal class EntryManager<TEntry, TAvailability>
        where TEntry : Entry<TAvailability>
        where TAvailability : AvailabilityInfo
    {
        public TEntry Entry { get; }
        public ChanceCalculator ChanceCalculator { get; }

        public EntryManager(ChanceCalculator chanceCalculator, TEntry entry)
        {
            this.ChanceCalculator = chanceCalculator
                ?? throw new ArgumentNullException(nameof(chanceCalculator));
            this.Entry = entry ?? throw new ArgumentNullException(nameof(entry));
        }
    }
}
