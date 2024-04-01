/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace GroundhogDay
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry
    {



        private static void Game1__newDayAfterFade_Prefix()
        {
            if (Config.EnableMod && (Game1.dayOfMonth != 0 || Game1.year != 1 || Game1.currentSeason != "spring"))
            {
                SMonitor.Log($"Repeating {Utility.getDateString()}");
                Game1.dayOfMonth--;
            }
        }
    }
}