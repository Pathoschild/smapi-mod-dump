/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Text;
using AtraShared.Utils.Extensions;
using GingerIslandMainlandAdjustments.Utils;
using StardewModdingAPI.Utilities;

namespace GingerIslandMainlandAdjustments.CustomConsoleCommands;

/// <summary>
/// Class that handles this mod's console commands.
/// </summary>
internal static class ConsoleCommands
{
    /// <summary>
    /// Holds the raw island schedule strings.
    /// </summary>
    internal static readonly Dictionary<string, string> IslandSchedules = new();

    /// <summary>
    /// All console commands in this will start with the following.
    /// </summary>
    private const string PrePendCommand = "av.gima";

    private static readonly string Antisocial = PathUtilities.NormalizeAssetName("Data/AntiSocialNPCs");

    /// <summary>
    /// Register the console commands for this mod.
    /// </summary>
    /// <param name="commandHelper">SMAPI's console command helper.</param>
    internal static void Register(ICommandHelper commandHelper)
    {
        commandHelper.Add(
            name: PrePendCommand,
            documentation: I18n.BaseCommand_Documentation(),
            callback: static (string command, string[] args) => Globals.ModMonitor.Log(
                I18n.BaseCommand()
                + $"\n\t{PrePendCommand}.get_schedule"
                + $"\n\t{PrePendCommand}.get_islanders"
                + $"\n\t{PrePendCommand}.get_locations_list",
                LogLevel.Info));
        commandHelper.Add(
            name: PrePendCommand + ".get_schedule",
            documentation: I18n.GetSchedule_Documentation(),
            callback: ConsoleSchedule);
        commandHelper.Add(
            name: PrePendCommand + ".get_islanders",
            documentation: I18n.GetIslanders_Documentation(),
            callback: ConsoleGetIslanders);
        commandHelper.Add(
            name: PrePendCommand + ".get_locations_list",
            documentation: I18n.GetLocations_Documentation(),
            callback: ConsoleGetLocations);
#if !DEBUG
        if (Globals.Config.DebugMode)
#endif
        {
            commandHelper.Add(
                name: PrePendCommand + ".queue_npc",
                documentation: "Queues an NPC for visit to Ginger Island on next valid day",
                callback: QueueNPC);
        }
    }

    /// <summary>
    /// Clear the island schedules at the end of the day.
    /// </summary>
    internal static void ClearCache() => IslandSchedules.Clear();

    /// <summary>Queues NPCs up for a visit to Ginger Island.</summary>
    /// <remarks>Note that this will override exclusions! It'll also only be available if the DEBUG config is set.</remarks>
    private static void QueueNPC(string command, string[] args)
    {
        if (!Context.IsWorldReady || !Context.IsMainPlayer || Globals.SaveDataModel is null)
        {
            Globals.ModMonitor.Log("NPCs can only be queued by the main player, and from within a save.", LogLevel.Info);
            return;
        }
        List<string> npcsFound = new();
        List<string> npcsNotFound = new();
        foreach (string npcname in args)
        {
            if (Utility.fuzzyCharacterSearch(npcname, must_be_villager: true) is not null)
            {
                Globals.SaveDataModel.NPCsForTomorrow.Add(npcname);
                npcsFound.Add(npcname);
            }
            else
            {
                npcsNotFound.Add(npcname);
            }
        }
        if (npcsFound.Count > 0)
        {
            Globals.ModMonitor.Log($"Queued for Ginger Island: {string.Join(", ", npcsFound)}", LogLevel.Info);
        }
        if (npcsNotFound.Count > 0)
        {
            Globals.ModMonitor.Log($"Not found for queuing: {string.Join(", ", npcsNotFound)}", LogLevel.Warn);
        }
    }

    /// <summary>
    /// Displays the schedule of a specific NPC.
    /// </summary>
    /// <param name="npc">NPC in question.</param>
    /// <param name="level">Log level to display.</param>
    private static void DisplaySchedule(NPC npc, LogLevel level)
    {
        Globals.ModMonitor.Log(I18n.DisplaySchedule_CheckNPC(npc.Name), level);
        if (npc.IsInvisible)
        {
            Globals.ModMonitor.Log('\t' + I18n.DisplaySchedule_IsInvisible(npc.Name), level);
            return;
        }
        if (!npc.followSchedule)
        {
            Globals.ModMonitor.Log('\t' + I18n.DisplaySchedule_NoSchedule(npc.Name), level);
            if (npc.Schedule is null || npc.Schedule.Count == 0)
            { // For some reason, sometimes even when followSchedule is not set, the NPC goes through their schedule anyways?
                return;
            }
        }
        if (Game1.netWorldState.Value.IslandVisitors.TryGetValue(npc.Name, out bool atIsland) && atIsland)
        {
            Globals.ModMonitor.Log('\t' + I18n.DisplaySchedule_ToIsland(npc.Name), level);
            if (IslandSchedules.TryGetValue(npc.Name, out string? schedulestring))
            {
                Globals.ModMonitor.Log($"\t{npc.islandScheduleName.Value}\n\t\t{schedulestring}", level);
            }
        }
        else
        {
            if (npc.dayScheduleName?.Value is null)
            {
                Globals.ModMonitor.Log($"\t{npc.Name} lacks a npc.dayScheduleName.Value yet claims to be following a schedule today.", LogLevel.Error);
            }
            else if (npc.hasMasterScheduleEntry(npc.dayScheduleName.Value))
            {
                if (Globals.UtilitySchedulingFunctions.TryFindGOTOschedule(npc, SDate.Now(), npc.getMasterScheduleEntry(npc.dayScheduleName.Value), out string schedulestring))
                {
                    Globals.ModMonitor.Log($"\t{npc.dayScheduleName.Value}\n\t\t{schedulestring}", level);
                }
                else
                {
                    Globals.ModMonitor.Log($"Schedule lookup for {npc.Name} failed!", LogLevel.Error);
                }
            }
            else
            {
                Globals.ModMonitor.Log($"\t{npc.Name} claims to be using {npc.dayScheduleName.Value} but that was not found!", LogLevel.Error);
            }
        }
        if (npc.Schedule is null)
        {
            Globals.ModMonitor.Log($"Something very odd has happened to the schedule of {npc.Name} - it appears to have been nulled since generation", LogLevel.Error);
            if (!npc.CanSocialize)
            {
                Dictionary<string, string>? antisocial;
                try
                {
                    antisocial = Game1.content.Load<Dictionary<string, string>>(Antisocial);
                }
                catch (Exception)
                {
                    antisocial = new();
                }
                Globals.ModMonitor.Log($"\t{npc.Name} appears to be antisocial: they {(antisocial.ContainsKey(npc.Name) ? "are" : "aren't")} registered with AntisocialNPCs.", LogLevel.Info);
            }
            return;
        }
        List<int> keys = new(npc.Schedule.Keys);
        keys.Sort();
        StringBuilder sb = new();
        foreach (int key in keys)
        {
            SchedulePathDescription schedulePathDescription = npc.Schedule[key];
            if (schedulePathDescription is null)
            {
                Globals.ModMonitor.Log("Found a null schedule description?", LogLevel.Error);
                continue;
            }
            int expectedRouteTime = schedulePathDescription.GetExpectedRouteTime();
            sb.Append('\t').Append(key).Append(": ");
            sb.AppendJoin(", ", schedulePathDescription.route).AppendLine();
            sb.Append("\t\t").Append(I18n.DisplaySchedule_ExpectedTime()).Append(expectedRouteTime).AppendLine(I18n.DisplaySchedule_ExpectedTime_Minutes());
            sb.Append("\t\t").Append(I18n.DisplaySchedule_ExpectedArrival()).AppendLine(Utility.ModifyTime(key, expectedRouteTime).ToString());
            sb.Append("\t\t").Append(I18n.DisplaySchedule_Direction()).AppendLine(schedulePathDescription.facingDirection.ToString());
            sb.Append("\t\t").Append(I18n.DisplaySchedule_Animation()).AppendLine(schedulePathDescription.endOfRouteBehavior);
            sb.Append("\t\t").Append(I18n.DisplaySchedule_Message()).AppendLine(schedulePathDescription.endOfRouteMessage);
        }
        sb.AppendLine();
        Globals.ModMonitor.Log(sb.ToString(), level);
    }

    /// <summary>
    /// Console command to get the islanders.
    /// </summary>
    /// <param name="command">Name of command.</param>
    /// <param name="args">Arguments, if any.</param>
    private static void ConsoleGetIslanders(string command, string[] args)
        => Globals.ModMonitor.Log($"{I18n.Islanders()}: {string.Join(", ", Islanders.Get())}", LogLevel.Debug);

    /// <summary>
    /// Yields the schedule for one or more characters.
    /// </summary>
    /// <param name="command">Name of command.</param>
    /// <param name="args">List of islanders.</param>
    private static void ConsoleSchedule(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Globals.ModMonitor.Log("This command can only be run while in a save!", LogLevel.Error);
            return;
        }
        if (!Context.IsMainPlayer)
        {
            Globals.ModMonitor.Log("This command may return inaccurate results for farmhands!", LogLevel.Warn);
        }
        foreach (string name in args)
        {
            if (Utility.fuzzyCharacterSearch(name, must_be_villager: true) is NPC npc)
            {
                DisplaySchedule(npc, LogLevel.Debug);
            }
            else
            {
                Globals.ModMonitor.Log(I18n.NpcNotFound(name), LogLevel.Debug);
            }
        }
    }

    private static void ConsoleGetLocations(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            return;
        }
        if (Globals.ReflectionHelper.GetField<List<List<string>>>(typeof(NPC), "routesFromLocationToLocation").GetValue() is not List<List<string>> locations)
        {
            Globals.ModMonitor.Log(I18n.GetLocations_NoneFound(), LogLevel.Info);
            return;
        }
        HashSet<string> locations_to_find = new(args, StringComparer.OrdinalIgnoreCase);
        if (args.Length > 0)
        {
            Globals.ModMonitor.Log($"Looking for {string.Join(", ", args)} in routesFromLocationToLocation", LogLevel.Info);
        }
        Func<List<string>, bool> filter = args.Length == 0 ? (_) => true : (List<string> loclist) => locations_to_find.Intersect(loclist, StringComparer.OrdinalIgnoreCase).Any();
        foreach (List<string>? loclist in locations)
        {
            if (loclist is not null && filter(loclist))
            {
                Globals.ModMonitor.Log(string.Join(", ", loclist), LogLevel.Info);
            }
        }
    }
}
