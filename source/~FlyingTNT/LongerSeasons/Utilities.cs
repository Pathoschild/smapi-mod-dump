/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlyingTNT/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace LongerSeasons
{
    internal class Utilities
    {
        public static readonly Season[] SeasonsByIndex = new Season[]{Season.Spring, Season.Summer, Season.Fall, Season.Winter};

        public static int GetDaysPerMonth()
        {
            return ModEntry.Config?.DaysPerMonth ?? 28;
        }
    }
}
