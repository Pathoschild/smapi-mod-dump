/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;

namespace GingerIslandMainlandAdjustments.ScheduleManager;

/// <summary>
/// Class that handles patches for debugging schedules.
/// These will only be active if DebugMode is set to true.
/// And thus: no harmony annotations.
/// </summary>
internal static class ScheduleDebugPatches
{
    private static readonly List<NPC> FailedNPCs = new();

    /// <summary>
    /// Applies the patches for this class.
    /// </summary>
    /// <param name="harmony">My harmony instance.</param>
    internal static void ApplyPatches(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.Method(typeof(NPC), "pathfindToNextScheduleLocation"),
            finalizer: new HarmonyMethod(typeof(ScheduleDebugPatches), nameof(ScheduleDebugPatches.FinalizePathfinder)));
    }

    /// <summary>
    /// Nulls out the schedules for problem NPCs.
    /// </summary>
    internal static void FixNPCs()
    {
        foreach (NPC npc in FailedNPCs)
        {
            npc.Schedule = null;
            npc.followSchedule = false;
        }
        FailedNPCs.Clear();
    }

    /// <summary>
    /// Finalizer on NPC pathfindToNextScheduleLocation.
    /// </summary>
    /// <param name="__instance">NPC.</param>
    /// <param name="startingLocation">The starting map.</param>
    /// <param name="startingX">Starting X.</param>
    /// <param name="startingY">Starting Y.</param>
    /// <param name="endingLocation">Ending map.</param>
    /// <param name="endingX">Ending X.</param>
    /// <param name="endingY">Ending Y.</param>
    /// <param name="finalFacingDirection">Facing direction for NPC.</param>
    /// <param name="endBehavior">End animation.</param>
    /// <param name="endMessage">End message for NPC to say.</param>
    /// <param name="__exception">Exception raised, if any.</param>
    /// <param name="__result">The result of the function (an empty schedulePoint).</param>
    /// <returns>null to surpress the exception.</returns>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    private static Exception? FinalizePathfinder(
        NPC __instance,
        string startingLocation,
        int startingX,
        int startingY,
        string endingLocation,
        int endingX,
        int endingY,
        int finalFacingDirection,
        string endBehavior,
        string endMessage,
        Exception __exception,
        ref SchedulePathDescription __result)
    {
        Globals.ModMonitor.Log($"Checking schedule point for {__instance.Name} at map {startingLocation} {startingX} {startingY}");
        if (__exception is not null)
        {
            Globals.ModMonitor.Log($"Encountered error parsing schedule for {__instance.Name}, {startingLocation} {startingX} {startingY} to {endingLocation} {endingX} {endingY}.\n\n{__exception}", LogLevel.Error);
            __result = new SchedulePathDescription(new Stack<Point>(), 2, null, null);
            FailedNPCs.Add(__instance);
        }
        return null;
    }
}