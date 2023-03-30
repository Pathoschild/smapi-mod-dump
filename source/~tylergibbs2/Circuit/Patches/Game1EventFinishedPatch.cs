/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley;

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(Game1), nameof(Game1.eventFinished))]
    internal class Game1EventFinishedPatch
    {
        public static bool Prefix()
        {
            if (!ModEntry.ShouldPatch())
                return true;

            if (Game1.currentLocation.currentEvent is not null)
                ModEntry.Instance.TaskManager?.OnEventEnd(Game1.currentLocation.currentEvent);

            return true;
        }
    }
}
