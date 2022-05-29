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
using StardewModdingAPI;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Services
{
    internal class EntryManagerFactory<TEntry, TAvailability>
        where TEntry : Entry<TAvailability>
        where TAvailability : AvailabilityInfo
    {
        private readonly CalculatorFactory calculatorFactory;

        public EntryManagerFactory(CalculatorFactory calculatorFactory)
        {
            this.calculatorFactory = calculatorFactory
                ?? throw new ArgumentNullException(nameof(calculatorFactory));
        }

        public EntryManager<TEntry, TAvailability> Create(IManifest owner, TEntry entry)
        {
            return new(this.calculatorFactory.Chances(owner, entry.AvailabilityInfo), entry);
        }
    }
}
