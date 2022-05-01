/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments
**
*************************************************/

using AtraShared.Schedules.DataModels;
using AtraShared.Utils.Extensions;
using GingerIslandMainlandAdjustments.CustomConsoleCommands;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;

namespace GingerIslandMainlandAdjustments.ScheduleManager.DataModels;

/// <summary>
/// Handles scheduling some islanders in IslandNorth.
/// </summary>
internal static class IslandNorthScheduler
{
    /// <summary>
    /// IslandNorth points for the adventurous.
    /// </summary>
    private static readonly List<Point> CloseAdventurousPoint = new()
    {
        new Point(33, 83),
        new Point(36, 81),
        new Point(39, 83),
    };

    private static readonly List<Point> TentAdventurousPoint = new()
    {
        new Point(44, 51),
        new Point(47, 49),
        new Point(50, 51),
    };

    private static readonly List<Point> VolcanoAdventurousPoint = new()
    {
        new Point(46, 29),
        new Point(48, 26),
        new Point(51, 28),
    };

    /// <summary>
    /// Makes schedules for the.
    /// </summary>
    /// <param name="random">Seeded random.</param>
    /// <param name="explorers">Hashset of explorers.</param>
    internal static void Schedule(Random random, HashSet<NPC> explorers)
    {
        if (explorers.Any())
        {
            bool whichFarpoint = random.NextDouble() < 0.5;
            List<Point> farPoints = whichFarpoint ? TentAdventurousPoint : VolcanoAdventurousPoint;
            string whichDialogue = whichFarpoint ? "Tent" : "Volcano";
            List<NPC> explorerList = explorers.ToList();
            Dictionary<NPC, List<SchedulePoint>> schedules = new();
            int explorerIndex = 0;

            foreach (NPC explorer in explorers)
            {
                schedules[explorer] = new List<SchedulePoint>()
                {
                    new SchedulePoint(
                    random: random,
                    npc: explorer,
                    map: "IslandNorth",
                    time: 1200,
                    point: CloseAdventurousPoint[explorerIndex++],
                    isarrivaltime: true,
                    basekey: "Resort_Adventure",
                    direction: explorerIndex), // this little hackish thing makes them face in different directions.
                };
            }

            explorerIndex = 0;
            Utility.Shuffle(random, explorerList);
            foreach (NPC explorer in explorerList)
            {
                schedules[explorer].Add(new SchedulePoint(
                    random: random,
                    npc: explorer,
                    map: "IslandNorth",
                    time: 1330,
                    point: farPoints[explorerIndex++],
                    basekey: $"Resort_{whichDialogue}",
                    direction: explorerIndex));
            }

            explorerIndex = 0;
            Utility.Shuffle(random, explorerList);
            foreach (NPC explorer in explorerList)
            {
                schedules[explorer].Add(new SchedulePoint(
                    random: random,
                    npc: explorer,
                    map: "IslandNorth",
                    time: 1700,
                    point: CloseAdventurousPoint[explorerIndex++],
                    basekey: "Resort_AdventureReturn",
                    isarrivaltime: true,
                    direction: explorerIndex));

                string renderedSchedule = string.Join("/", schedules[explorer]) + '/'
                    + (ScheduleUtilities.FindProperGISchedule(explorer, SDate.Now())
                    // Child2NPC NPCs don't understand "bed", must send them to the bus stop spouse dropoff.
                    ?? (Globals.IsChildToNPC?.Invoke(explorer) == true ? "1800 BusStop -1 23 3" : "1800 bed"));

                Globals.ModMonitor.DebugOnlyLog($"Calculated island north schedule for {explorer.Name}");
                explorer.islandScheduleName.Value = "island";

                ScheduleUtilities.ParseMasterScheduleAdjustedForChild2NPC(explorer, renderedSchedule);

                Game1.netWorldState.Value.IslandVisitors[explorer.Name] = true;
                ConsoleCommands.IslandSchedules[explorer.Name] = renderedSchedule;
            }
        }
    }
}