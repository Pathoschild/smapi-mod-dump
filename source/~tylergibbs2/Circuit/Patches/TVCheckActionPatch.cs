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
using StardewValley.Objects;

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(TV), nameof(TV.checkForAction))]
    internal class TVCheckActionPatch
    {
        public static bool Prefix(ref bool __result, bool justCheckingForActivity)
        {
            if (!ModEntry.ShouldPatch() || justCheckingForActivity)
                return true;

            if (EventManager.EventIsActive(EventType.PoorService))
            {
                Game1.drawObjectDialogue("The screen is too fuzzy to make anything out.");
                __result = true;
                return false;
            }

            return true;
        }
    }
}
