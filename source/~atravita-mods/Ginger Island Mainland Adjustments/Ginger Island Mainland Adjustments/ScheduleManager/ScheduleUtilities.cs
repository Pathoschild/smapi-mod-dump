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
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;

namespace GingerIslandMainlandAdjustments.ScheduleManager;

/// <summary>
/// Class that helps select the right GI remainder schedule.
/// </summary>
internal static class ScheduleUtilities
{
    private const string BASE_SCHEDULE_KEY = "GIRemainder";
    private const string POST_GI_START_TIME = "1800"; // all GI schedules must start at 1800

    private static readonly Dictionary<string, Dictionary<int, SchedulePathDescription>> Schedules = new();

    /// <summary>
    /// Removes schedule cache.
    /// </summary>
    internal static void ClearCache() => Schedules.Clear();

    /// <summary>
    /// Find the correct schedule for an NPC for a given date. Looks into the schedule assets first
    /// then sees if there's a GOTO statement. Resolve that if necessary.
    /// </summary>
    /// <param name="npc">NPC to look for.</param>
    /// <param name="date">Date to search.</param>
    /// <returns>A schedule string if it can, null if it can't find one.</returns>
    internal static string? FindProperGISchedule(NPC npc, SDate date)
    {
        string scheduleKey = BASE_SCHEDULE_KEY;
        if (npc.isMarried())
        {
            Globals.ModMonitor.DebugOnlyLog($"{npc.Name} is married, using married GI schedules");
            scheduleKey += "_married";
        }
        int hearts = Utility.GetAllPlayerFriendshipLevel(npc) / 250;

        // GIRemainder_Season_Day
        string checkKey = $"{scheduleKey}_{date.Season}_{date.Day}";
        string? scheduleEntry;
        if (npc.hasMasterScheduleEntry(checkKey)
            && Globals.UtilitySchedulingFunctions.TryFindGOTOschedule(npc, date, npc.getMasterScheduleEntry(checkKey), out scheduleEntry)
            && scheduleEntry.StartsWith(POST_GI_START_TIME))
        {
            return scheduleEntry;
        }

        // GIRemainder_intDay_heartlevel
        for (int heartLevel = Math.Max((hearts / 2) * 2, 0); heartLevel > 0; heartLevel--)
        {
            checkKey = $"{scheduleKey}_{date.Day}_{heartLevel}";
            if (npc.hasMasterScheduleEntry(checkKey)
                && Globals.UtilitySchedulingFunctions.TryFindGOTOschedule(npc, date, npc.getMasterScheduleEntry(checkKey), out scheduleEntry)
                && scheduleEntry.StartsWith(POST_GI_START_TIME))
            {
                return scheduleEntry;
            }
        }

        // GIRemainder_Day
        checkKey = $"{scheduleKey}_{date.Day}";
        if (npc.hasMasterScheduleEntry(checkKey)
            && Globals.UtilitySchedulingFunctions.TryFindGOTOschedule(npc, date, npc.getMasterScheduleEntry(checkKey), out scheduleEntry)
            && scheduleEntry.StartsWith(POST_GI_START_TIME))
        {
            return scheduleEntry;
        }

        // GIRemainder_rain
        if (Game1.IsRainingHere(npc.currentLocation))
        {
            checkKey = $"{scheduleKey}_rain";
            if (npc.hasMasterScheduleEntry(checkKey)
                && Globals.UtilitySchedulingFunctions.TryFindGOTOschedule(npc, date, npc.getMasterScheduleEntry(checkKey), out scheduleEntry)
                && scheduleEntry.StartsWith(POST_GI_START_TIME))
            {
                return scheduleEntry;
            }
        }

        // GIRemainder_season_DayOfWeekHearts
        for (int heartLevel = Math.Max((hearts / 2) * 2, 0); heartLevel > 0; heartLevel -= 2)
        {
            checkKey = $"{scheduleKey}_{date.Season}_{Game1.shortDayNameFromDayOfSeason(date.Day)}{heartLevel}";
            if (npc.hasMasterScheduleEntry(checkKey)
                && Globals.UtilitySchedulingFunctions.TryFindGOTOschedule(npc, date, npc.getMasterScheduleEntry(checkKey), out scheduleEntry)
                && scheduleEntry.StartsWith(POST_GI_START_TIME))
            {
                return scheduleEntry;
            }
        }

        // GIRemainder_season_DayOfWeek
        checkKey = $"{scheduleKey}_{date.Season}_{Game1.shortDayNameFromDayOfSeason(date.Day)}";
        if (npc.hasMasterScheduleEntry(checkKey)
            && Globals.UtilitySchedulingFunctions.TryFindGOTOschedule(npc, date, npc.getMasterScheduleEntry(checkKey), out scheduleEntry)
            && scheduleEntry.StartsWith(POST_GI_START_TIME))
        {
            return scheduleEntry;
        }

        // GIRemainder_DayOfWeekHearts
        for (int heartLevel = Math.Max((hearts / 2) * 2, 0); heartLevel > 0; heartLevel -= 2)
        {
            checkKey = $"{scheduleKey}_{Game1.shortDayNameFromDayOfSeason(date.Day)}{heartLevel}";
            if (npc.hasMasterScheduleEntry(checkKey)
                && Globals.UtilitySchedulingFunctions.TryFindGOTOschedule(npc, date, npc.getMasterScheduleEntry(checkKey), out scheduleEntry)
                && scheduleEntry.StartsWith(POST_GI_START_TIME))
            {
                return scheduleEntry;
            }
        }

        // GIRemainder_DayOfWeek
        checkKey = $"{scheduleKey}_{Game1.shortDayNameFromDayOfSeason(date.Day)}";
        if (npc.hasMasterScheduleEntry(checkKey)
            && Globals.UtilitySchedulingFunctions.TryFindGOTOschedule(npc, date, npc.getMasterScheduleEntry(checkKey), out scheduleEntry)
            && scheduleEntry.StartsWith(POST_GI_START_TIME))
        {
            return scheduleEntry;
        }

        // GIRemainderHearts
        for (int heartLevel = Math.Max((hearts / 2) * 2, 0); heartLevel > 0; heartLevel -= 2)
        {
            checkKey = $"{scheduleKey}_{heartLevel}";
            if (npc.hasMasterScheduleEntry(checkKey)
                && Globals.UtilitySchedulingFunctions.TryFindGOTOschedule(npc, date, npc.getMasterScheduleEntry(checkKey), out scheduleEntry)
                && scheduleEntry.StartsWith(POST_GI_START_TIME))
            {
                return scheduleEntry;
            }
        }

        // GIREmainder_season
        checkKey = $"{scheduleKey}_{date.Season}";
        if (npc.hasMasterScheduleEntry(checkKey)
            && Globals.UtilitySchedulingFunctions.TryFindGOTOschedule(npc, date, npc.getMasterScheduleEntry(checkKey), out scheduleEntry)
            && scheduleEntry.StartsWith(POST_GI_START_TIME))
        {
            return scheduleEntry;
        }

        // GIREmainder
        if (npc.hasMasterScheduleEntry(scheduleKey)
            && Globals.UtilitySchedulingFunctions.TryFindGOTOschedule(npc, date, npc.getMasterScheduleEntry(scheduleKey), out scheduleEntry)
            && scheduleEntry.StartsWith(POST_GI_START_TIME))
        {
            return scheduleEntry;
        }

        Globals.ModMonitor.Log(I18n.NOGISCHEDULEFOUND(npc: npc.Name));
        return null;
    }

    /// <summary>
    /// Wraps npc.parseMasterSchedule to lie to it about the start location of the NPC, if the NPC lives in the farmhouse.
    /// </summary>
    /// <param name="npc">NPC in question.</param>
    /// <param name="rawData">Raw schedule string.</param>
    /// <returns>True if successful, false otherwise.</returns>
    internal static bool ParseMasterScheduleAdjustedForChild2NPC(NPC npc, string rawData)
    {
        if (Globals.IsChildToNPC?.Invoke(npc) == true)
        {
            // For a Child2NPC, we must handle their scheduling ourselves.
            if (Globals.UtilitySchedulingFunctions.TryFindGOTOschedule(npc, SDate.Now(), rawData, out string scheduleString))
            {
                Dictionary<int, SchedulePathDescription>? schedule = Globals.UtilitySchedulingFunctions.ParseSchedule(scheduleString, npc, "BusStop", new Point(0, 23), 610, Globals.Config.EnforceGITiming);
                if (schedule is not null)
                {
                    npc.Schedule = schedule;
                    if (Context.IsMainPlayer && npc.Schedule is not null
                        && Globals.ReflectionHelper.GetField<string>(npc, "_lastLoadedScheduleKey", false)?.GetValue() is string lastschedulekey)
                    {
                        npc.dayScheduleName.Value = lastschedulekey;
                    }
                    return true;
                }
                else
                {
                    Globals.ModMonitor.Log($"Failed to generate schedule for {npc.Name}: {rawData}");
                    return false;
                }
            }
            else
            {
                Globals.ModMonitor.Log("TryFindGOTOschedule failed for Child2NPC!", LogLevel.Warn);
                return false;
            }
        }
        else if ((npc.DefaultMap.Equals("FarmHouse", StringComparison.OrdinalIgnoreCase) || npc.DefaultMap.Contains("Cabin", StringComparison.OrdinalIgnoreCase))
                  && !npc.isMarried())
        {
            // lie to parse master schedule
            string prevmap = npc.DefaultMap;
            Vector2 prevposition = npc.DefaultPosition;

            if (rawData.EndsWith("bed"))
            {
                rawData = rawData[..^3] + "BusStop -1 23 3";
            }

            npc.DefaultMap = "BusStop";
            npc.DefaultPosition = new Vector2(0, 23) * 64;
            Dictionary<int, SchedulePathDescription>? schedule = null;
            try
            {
                schedule = npc.parseMasterSchedule(rawData);
            }
            catch (Exception ex)
            {
                Globals.ModMonitor.Log($"Ran into issues parsing schedule {rawData} for {npc.Name}.\n\n{ex}", LogLevel.Error);
            }
            npc.DefaultMap = prevmap;
            npc.DefaultPosition = prevposition;

            if (schedule is not null)
            {
                npc.Schedule = schedule;
                Schedules[npc.Name] = npc.Schedule;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            Dictionary<int, SchedulePathDescription>? schedule = null;

            try
            {
                schedule = npc.parseMasterSchedule(rawData);
            }
            catch (Exception ex)
            {
                Globals.ModMonitor.Log($"parseMasterSchedule failed for npc {npc.Name} with rawdata {rawData}: {ex}");
            }
            if (schedule is not null)
            {
                npc.Schedule = schedule;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Goes around at about 620 and fixes up schedules if they've been nulled.
    /// Child2NPC may try to fix up their children around ~610 AM. Sadly, that nulls the schedules.
    /// </summary>
    internal static void FixUpSchedules()
    {
        foreach (NPC npc in Game1.getLocationFromName("FarmHouse").getCharacters())
        {
            if (Globals.IsChildToNPC?.Invoke(npc) == true && npc.Schedule is null
                && ScheduleUtilities.Schedules.TryGetValue(npc.Name, out Dictionary<int, SchedulePathDescription>? schedule))
            {
                Globals.ModMonitor.Log($"Fixing up schedule for {npc.Name}, which appears to have been nulled.", LogLevel.Warn);
                npc.Schedule = schedule;
                ScheduleUtilities.Schedules.Remove(npc.Name);
            }
        }
    }
}