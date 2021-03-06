/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI.Utilities;
using StardewValley;
using TehPers.Core.Api.Enums;
using TehPers.FishingOverhaul.Api;

namespace TehPers.FishingOverhaul.Configs {
    public class DefaultTrashData : ITrashData {
        private const int MIN_TRASH_ID = 167;
        private const int MAX_TRASH_ID = 173;
        public IEnumerable<int> PossibleIds { get; } = Enumerable.Range(DefaultTrashData.MIN_TRASH_ID, DefaultTrashData.MAX_TRASH_ID - DefaultTrashData.MIN_TRASH_ID);

        public bool MeetsCriteria(Farmer who, string locationName, WaterType waterType, SDate date, Weather weather, int time, int fishingLevel, int? mineLevel) {
            return locationName != "Submarine";
        }

        public double GetWeight() {
            return DefaultTrashData.MAX_TRASH_ID - DefaultTrashData.MIN_TRASH_ID;
        }
    }
}