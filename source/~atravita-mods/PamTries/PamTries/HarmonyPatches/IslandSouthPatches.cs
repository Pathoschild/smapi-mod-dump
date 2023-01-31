/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using HarmonyLib;
using StardewValley.Locations;

namespace PamTries.HarmonyPatches;

[HarmonyPatch(typeof(IslandSouth))]
internal static class IslandSouthPatches
{
    [HarmonyPriority(Priority.VeryLow)]
    [HarmonyPatch(nameof(IslandSouth.CanVisitIslandToday))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Convention used by Harmony")]
    private static void Postfix(NPC npc, ref bool __result)
    {
        if (__result && Game1.dayOfMonth is 6 or 16
            && npc.Name.Equals("Pam", StringComparison.OrdinalIgnoreCase)
            && Game1.getAllFarmers().Any(f => f.eventsSeen.Contains(99210002)))
        {
            __result = false;
        }
    }
}
