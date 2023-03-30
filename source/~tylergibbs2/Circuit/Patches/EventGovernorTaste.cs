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
    [HarmonyPatch(typeof(Event), "governorTaste")]
    internal class EventGovernorTaste
    {
        public static void Postfix(Event __instance)
        {
            if (!ModEntry.ShouldPatch())
                return;

            ModEntry.Instance.TaskManager?.OnEventGovernorTaste(__instance);
        }
    }
}
