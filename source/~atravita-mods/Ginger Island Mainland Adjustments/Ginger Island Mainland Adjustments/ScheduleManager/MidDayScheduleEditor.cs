/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.Extensions;

using AtraCore.Framework.Caches;

using AtraShared.Utils.Extensions;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

namespace GingerIslandMainlandAdjustments.ScheduleManager;

/// <summary>
/// Class that handles middle of the day schedule editing.
/// </summary>
internal static class MidDayScheduleEditor
{
    /// <summary>
    /// When Ginger Island schedules end.
    /// </summary>
    private const int GIEndTime = 1800;

    /// <summary>
    /// Which map is the resort on.
    /// </summary>
    private const string GIMap = "IslandSouth";

    /// <summary>
    /// Keep track of the NPCs I've edited already, so I don't edit anyone twice.
    /// </summary>
    private static readonly Dictionary<string, bool> ScheduleAltered = new();

    /// <summary>
    /// Clears the ScheduleAltered dictionary.
    /// </summary>
    public static void Reset()
    {
        ScheduleAltered.Clear();
        Globals.ModMonitor.Log("Reset scheduleAltered", LogLevel.Trace);
    }

    /// <summary>
    /// Attempt a mid-day adjustment of a character's schedule
    /// if they're headed to Ginger Island.
    /// Does one per ten minutes.
    /// </summary>
    /// <param name="e">Time Changed Parameters from SMAPI.</param>
    internal static void AttemptAdjustGISchedule(TimeChangedEventArgs e)
    {
        if (!Context.IsMainPlayer
            || e.NewTime >= 900
            || Globals.Config.UseThisScheduler)
        { // skip after 9AM, or if is not the main player, or if the fancy scheduler is on.
            return;
        }
        foreach (string name in Game1.netWorldState.Value.IslandVisitors.Keys)
        {
            if (name.Equals("Gus", StringComparison.OrdinalIgnoreCase) // Gus runs saloon, skip.
                || !Game1.netWorldState.Value.IslandVisitors[name]
                || (ScheduleAltered.TryGetValue(name, out bool hasbeenaltered) && hasbeenaltered))
            {
                continue;
            }
            Globals.ModMonitor.Log(I18n.MiddayScheduleEditor_NpcFoundForAdjustment(name), LogLevel.Trace);
            ScheduleAltered[name] = true;
            if (NPCCache.GetByVillagerName(name) is NPC npc)
            {
                AdjustSpecificSchedule(npc);
                break; // Do the next person at the next ten minute tick.
            }
        }
    }

    /// <summary>
    /// Midday adjustment of a schedule for a specific NPC.
    /// </summary>
    /// <param name="npc">NPC who's schedule may need adjusting.</param>
    /// <returns>True if successful, false otherwise.</returns>
    internal static bool AdjustSpecificSchedule(NPC npc)
    {
        if (npc.islandScheduleName?.Value is null || npc.islandScheduleName?.Value == string.Empty)
        {
            if (Globals.Config.EnforceGITiming || Globals.Config.DebugMode)
            {
                Globals.ModMonitor.Log(I18n.MiddayScheduleEditor_NpcNotIslander(npc.Name), LogLevel.Warn);
            }
            return false;
        }
        if (npc.IsInvisible)
        {
            Globals.ModMonitor.DebugOnlyLog($"NPC {npc.Name} is invisible, not altering schedule", LogLevel.Trace);
            return false;
        }
        if (npc.Schedule is null)
        {
            if (Globals.Config.EnforceGITiming || Globals.Config.DebugMode)
            {
                Globals.ModMonitor.Log(I18n.MiddayScheduleEditor_NpcHasNoSchedule(npc.Name), LogLevel.Warn);
            }
            return false;
        }

        List<int> keys = npc.Schedule.Keys.ToList();
        keys.Sort();
        if (keys.Count == 0 || keys[^1] != GIEndTime)
        {
            Globals.ModMonitor.DebugOnlyLog($"Recieved {npc.Name} to adjust but last schedule key is not {GIEndTime}");
            return false;
        }
        string? schedule = ScheduleUtilities.FindProperGISchedule(npc, SDate.Now());
        Dictionary<int, SchedulePathDescription>? remainderSchedule = ParseGIRemainderSchedule(schedule, npc);

        if (remainderSchedule is not null)
        {
            npc.Schedule.Update(remainderSchedule);
        }
        return remainderSchedule is null;
    }

    /// <summary>
    /// Gets the SchedulePathDescription schedule for the schedule string,
    /// assumes a 1800 start on Ginger Island.
    /// </summary>
    /// <param name="schedule">Raw schedule string.</param>
    /// <param name="npc">NPC.</param>
    /// <returns>null if the schedule could not be parsed, a schedule otherwise.</returns>
    [ContractAnnotation("schedule:null => null")]
    internal static Dictionary<int, SchedulePathDescription>? ParseGIRemainderSchedule(string? schedule, NPC npc)
    {
        if (schedule is null)
        {
            return null;
        }

        Point lastStop = npc.Schedule[GIEndTime].route.Peek();
        int lasttime = GIEndTime - 10;

        return Globals.UtilitySchedulingFunctions.ParseSchedule(schedule, npc, GIMap, lastStop, lasttime, Globals.Config.EnforceGITiming);
    }
}
