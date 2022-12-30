/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.Menus;

namespace AFKTimePause
{
    public partial class ModEntry
    {
        [HarmonyPatch(typeof(Game1), nameof(Game1.UpdateGameClock))]
        public class Game1_UpdateGameClock_Patch
        {
            public static bool Prefix()
            {
                if (!Config.ModEnabled || !Game1.IsMasterGame || Game1.eventUp || Game1.isFestival() || elapsedSeconds < Config.SecondsTilAFK)
                    return true;
                if(elapsedSeconds == Config.SecondsTilAFK)
                {
                    elapsedSeconds++;
                    SMonitor.Log("Going AFK");
                }
                return false;
            }
        }
    }
}