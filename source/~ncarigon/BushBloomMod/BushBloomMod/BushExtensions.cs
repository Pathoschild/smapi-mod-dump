/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.TerrainFeatures;

namespace BushBloomMod {
    public static class BushExtensions {
        // exists, not a town bush, medium size, not an island bush
        public static bool IsAbleToBloom(this Bush bush) =>
            bush is not null
            && !bush.townBush.Value
            && bush.size.Value == 1
            && !(bush.Location?.InIslandContext() ?? false);

        public static bool IsBloomingToday(this Bush bush) => Schedule.GetSchedule(bush) is not null;

        public static bool HasBloomedToday(this Bush bush) => Schedule.GetExistingSchedule(bush) is not null;

        public static string GetShakeOffId(this Bush bush) => Schedule.GetExistingSchedule(bush)?.ShakeOffId;
    }
}