/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using StardewValley;
using StardewValley.Tools;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Services
{
    internal class FishingTracker
    {
        public Dictionary<Farmer, FisherData> ActiveFisherData { get; } = new();

        public record FisherData(FishingRod Rod, FishingState State);
    }
}