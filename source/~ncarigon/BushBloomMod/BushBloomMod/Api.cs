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
using System.Linq;

namespace BushBloomMod {
    public class Api {
#pragma warning disable CA1822 // Mark members as static
        public (string, WorldDate, WorldDate)[] GetActiveSchedules(string season, int dayofMonth, int? year = null, GameLocation location = null) =>
            Schedule.GetAllCandidates(year ?? Game1.year, Helpers.GetDayOfYear(season, dayofMonth), true, false, location)
                .Where(c => c.ShakeOffId != null)
                .Select(c => (c.ShakeOffId, c.Entry.FirstDay, c.Entry.LastDay))
                .ToArray();
#pragma warning restore CA1822 // Mark members as static
    }
}
