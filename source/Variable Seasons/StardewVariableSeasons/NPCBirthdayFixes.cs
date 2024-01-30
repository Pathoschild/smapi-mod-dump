/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/calebstein1/StardewVariableSeasons
**
*************************************************/

namespace StardewVariableSeasons
{
    public static class NPCBirthdayFixes
    {
        public static void Prefix(ref string season)
        {
            season = ModEntry.SeasonByDay;
        }
    }
}