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
using xTile.Dimensions;

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(Event), nameof(Event.checkAction))]
    internal class EventCheckActionPatch
    {
        public static bool Prefix(Event __instance, Location tileLocation) {
            if (!ModEntry.ShouldPatch())
                return true;

            ModEntry.Instance.TaskManager?.OnEventCheckAction(__instance, tileLocation);
            return true;
        }
    }
}
