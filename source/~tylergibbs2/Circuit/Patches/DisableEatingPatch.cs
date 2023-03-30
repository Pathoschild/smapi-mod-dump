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
    [HarmonyPatch(typeof(Farmer), "eatObject")]
    internal class DisableEatingPatch
    {
        public static bool Prefix(SObject o)
        {
            if (!ModEntry.ShouldPatch() || Utility.IsNormalObjectAtParentSheetIndex(o, 434))
                return true;

            if (EventManager.EventIsActive(EventType.Nauseous))
            {
                Game1.drawObjectDialogue("You feel too sick to eat.");
                return false;
            }

            return true;
        }
    }
}
