/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Utils.Extensions;

using HarmonyLib;

namespace GingerIslandMainlandAdjustments.Niceties;

/// <summary>
/// Speeds up NPCs if they have a long way to travel to and from the resort.
/// </summary>
/// <remarks>using about six hours as the cutoff for now.</remarks>
[HarmonyPatch(typeof(NPC))]
internal static class NPCTravelSpeedAdjuster
{
    [HarmonyPatch(nameof(NPC.checkSchedule))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    private static void Postfix(NPC __instance)
    {
        if (__instance?.controller is PathFindController controller
            && Game1.IsVisitingIslandToday(__instance.Name)
            && controller.pathToEndPoint.Count * 32 / 42 > 340)
        {
            Globals.ModMonitor.DebugOnlyLog($"Found npc {__instance.Name} with long travel path, speeding them up.");
            __instance.Speed = 4;
            __instance.isCharging = true;
        }
    }
}
