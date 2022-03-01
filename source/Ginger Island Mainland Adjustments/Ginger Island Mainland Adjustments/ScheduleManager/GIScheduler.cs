/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments
**
*************************************************/

using GingerIslandMainlandAdjustments.CustomConsoleCommands;
using GingerIslandMainlandAdjustments.ScheduleManager.DataModels;
using GingerIslandMainlandAdjustments.Utils;
using StardewModdingAPI.Utilities;
using StardewValley.Locations;

namespace GingerIslandMainlandAdjustments.ScheduleManager;

/// <summary>
/// Class that handles scheduling if the <see cref="ModConfig.UseThisScheduler"/> option is set.
/// </summary>
internal static class GIScheduler
{
    private static readonly int[] TIMESLOTS = new int[] { 1200, 1400, 1600 };

    /// <summary>
    /// Dictionary of possible island groups. Null is a cache miss.
    /// </summary>
    /// <remarks>Use the getter, which will automatically grab from fake asset.</remarks>
    private static Dictionary<string, HashSet<NPC>>? islandGroups = null;

    /// <summary>
    /// Dictionary of possible explorer groups. Null is a cache miss.
    /// </summary>
    /// <remarks>Use the getter, which will automatically grab from fake asset.</remarks>
    private static Dictionary<string, HashSet<NPC>>? explorerGroups = null;

    private static NPC? bartender;

    private static NPC? musician;

    private static string? currentGroup = null;

    private static HashSet<NPC>? currentVisitingGroup = null;

    /// <summary>
    /// Gets the current group headed off to the island.
    /// </summary>
    /// <remarks>null means no current group.</remarks>
    public static string? CurrentGroup => GIScheduler.currentGroup;

    /// <summary>
    /// Gets the current visting group.
    /// </summary>
    /// <remarks>Used primarily for setting group-based dialogue...</remarks>
    public static HashSet<NPC>? CurrentVisitingGroup => GIScheduler.currentVisitingGroup;

    /// <summary>
    /// Gets the current bartender.
    /// </summary>
    public static NPC? Bartender => bartender;

    /// <summary>
    /// Gets the current musician.
    /// </summary>
    public static NPC? Musician => musician;

    /// <summary>
    /// Gets island groups. Will automatically load if null.
    /// </summary>
    private static Dictionary<string, HashSet<NPC>> IslandGroups
    {
        get
        {
            if (islandGroups is null)
            {
                islandGroups = AssetManager.GetCharacterGroup(SpecialGroupType.Groups);
            }
            return islandGroups;
        }
    }

    /// <summary>
    /// Gets explorer groups. Will automatically load if null.
    /// </summary>
    private static Dictionary<string, HashSet<NPC>> ExplorerGroups
    {
        get
        {
            if (explorerGroups is null)
            {
                explorerGroups = AssetManager.GetCharacterGroup(SpecialGroupType.Explorers);
            }
            return explorerGroups;
        }
    }

    /// <summary>
    /// Clears the cached values for this class.
    /// </summary>
    public static void ClearCache()
    {
        islandGroups = null;
        explorerGroups = null;
    }

    /// <summary>
    /// Deletes references to the current group at the end of the day.
    /// </summary>
    public static void DayEndReset()
    {
        currentGroup = null;
        currentVisitingGroup = null;
    }

    /// <summary>
    /// Generates schedules for everyone.
    /// </summary>
    public static void GenerateAllSchedules()
    {
        Game1.netWorldState.Value.IslandVisitors.Clear();
        if (Game1.getLocationFromName("IslandSouth") is not IslandSouth island || !island.resortRestored.Value
            || Game1.IsRainingHere(island) || !island.resortOpenToday.Value
            || Utility.isFestivalDay(Game1.Date.DayOfMonth, Game1.Date.Season)
            || (Game1.Date.DayOfMonth >= 15 && Game1.Date.DayOfMonth <= 17 && Game1.Date.Season.Equals("winter", StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        Random random = new((int)(Game1.uniqueIDForThisGame * 1.21f) + (int)(Game1.stats.DaysPlayed * 2.5f));
        Dictionary<string, string> animationDescriptions = Globals.ContentHelper.Load<Dictionary<string, string>>("Data/animationDescriptions", ContentSource.GameContent);

        HashSet<NPC> explorers = GenerateExplorerGroup(random);
        if (explorers.Any())
        {
            Globals.ModMonitor.DebugLog($"Found explorer group: {string.Join(", ", explorers.Select((NPC npc) => npc.Name))}.");
            IslandNorthScheduler.Schedule(random, explorers);
        }

        // Resort capacity set to zero, can skip everything else.
        if (Globals.Config.Capacity == 0)
        {
            IslandSouthPatches.ClearCache();
            GIScheduler.ClearCache();
            return;
        }

        List<NPC> visitors = GenerateVistorList(random, Globals.Config.Capacity, explorers);

        GIScheduler.bartender = SetBartender(visitors);
        GIScheduler.musician = SetMusician(random, visitors, animationDescriptions);

        List<GingerIslandTimeSlot> activities = AssignIslandSchedules(random, visitors, animationDescriptions);
        Dictionary<NPC, string> schedules = RenderIslandSchedules(random, visitors, activities);

        foreach (NPC visitor in schedules.Keys)
        {
            Globals.ModMonitor.Log($"Calculated island schedule for {visitor.Name}");
            visitor.islandScheduleName.Value = "island";
            visitor.Schedule = visitor.parseMasterSchedule(schedules[visitor]);
            Game1.netWorldState.Value.IslandVisitors[visitor.Name] = true;
            ConsoleCommands.IslandSchedules[visitor.Name] = schedules[visitor];
        }

        GIScheduler.ClearCache();
    }

    /// <summary>
    /// Yields a group of valid explorers.
    /// </summary>
    /// <param name="random">Seeded random.</param>
    /// <returns>An explorer group (of up to three explorers), or an empty hashset if there's no group today.</returns>
    private static HashSet<NPC> GenerateExplorerGroup(Random random)
    {
        if (random.NextDouble() <= Globals.Config.ExplorerChance)
        {
            List<string> explorerGroups = ExplorerGroups.Keys.ToList();
            if (explorerGroups.Count > 0)
            {
                string explorerGroup = explorerGroups[random.Next(explorerGroups.Count)];
                return ExplorerGroups[explorerGroup].Where((NPC npc) => IslandSouth.CanVisitIslandToday(npc)).Take(3).ToHashSet();
            }
        }
        return new HashSet<NPC>(); // just return an empty hashset.
    }

    /// <summary>
    /// Gets the visitor list for a specific day. Explorers can't be visitors, so remove them.
    /// </summary>
    /// <param name="random">Random to use to select.</param>
    /// <param name="capacity">Maximum number of people to allow on the island.</param>
    /// <returns>Visitor List.</returns>
    /// <remarks>For a deterministic island list, use a Random seeded with the uniqueID + number of days played.</remarks>
    private static List<NPC> GenerateVistorList(Random random, int capacity, HashSet<NPC> explorers)
    {
        currentGroup = null;
        currentVisitingGroup = null;

        List<NPC> visitors = new();
        HashSet<NPC> valid_visitors = new();

        foreach (NPC npc in Utility.getAllCharacters())
        {
            if (IslandSouth.CanVisitIslandToday(npc) && !explorers.Contains(npc))
            {
                valid_visitors.Add(npc);
            }
        }
        if (random.NextDouble() < Globals.Config.GroupChance)
        {
            List<string> groupkeys = new();
            foreach (string key in IslandGroups.Keys)
            {
                // Filter out groups where one member can't make it or are too big
                if (IslandGroups[key].All((NPC npc) => valid_visitors.Contains(npc)) && IslandGroups[key].Count <= capacity)
                {
                    groupkeys.Add(key);
                }
            }
            if (groupkeys.Count > 0)
            {
                currentGroup = Utility.GetRandom(groupkeys, random);
#if DEBUG
                Globals.ModMonitor.Log($"Group {currentGroup} headed to Island.", LogLevel.Debug);
#endif
                HashSet<NPC> possiblegroup = IslandGroups[currentGroup];
                visitors = possiblegroup.ToList(); // limit group size if there's too many people...
                currentVisitingGroup = possiblegroup;
                valid_visitors.ExceptWith(visitors);
            }
        }
        NPC? gus = Game1.getCharacterFromName("Gus");
        if (gus is not null && !visitors.Contains(gus) && valid_visitors.Contains(gus)
            && Globals.Config.GusDayAsShortString().Equals(Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth), StringComparison.OrdinalIgnoreCase)
            && Globals.Config.GusChance > random.NextDouble())
        {
            Globals.ModMonitor.DebugLog($"Forcibly adding Gus");
            visitors.Add(gus);
            valid_visitors.Remove(gus);
        }

        // Prevent children and anyone with the neveralone exclusion from going alone.
        int kidsremoved = valid_visitors.RemoveWhere((NPC npc) => npc.Age == NPC.child);
        int neveralone = valid_visitors.RemoveWhere((NPC npc) => IslandSouthPatches.Exclusions.TryGetValue(npc, out string[]? exclusions) && exclusions.Contains("neveralone"));
        Globals.ModMonitor.DebugLog($"Excluded {kidsremoved} kids and {neveralone} never alone villagers from the valid villagers list");

        if (visitors.Count < capacity)
        {
#if DEBUG
            Globals.ModMonitor.Log($"{capacity} not yet reached, attempting to add more.", LogLevel.Debug);
#endif
            visitors.AddRange(valid_visitors.OrderBy(a => random.Next()).Take(capacity - visitors.Count));
        }

        // If George in visitors, add Evelyn.
        if (visitors.Any((NPC npc) => npc.Name.Equals("George", StringComparison.OrdinalIgnoreCase))
            && visitors.All((NPC npc) => !npc.Name.Equals("Evelyn", StringComparison.OrdinalIgnoreCase))
            && Game1.getCharacterFromName("Evelyn") is NPC evelyn)
        {
            // counting backwards to avoid kicking out a group member.
            for (int i = visitors.Count - 1; i >= 0; i--)
            {
                if (!visitors[i].Name.Equals("Gus", StringComparison.OrdinalIgnoreCase) && !visitors[i].Name.Equals("George", StringComparison.OrdinalIgnoreCase))
                {
                    Globals.ModMonitor.DebugLog($"Replacing one visitor {visitors[i].Name} with Evelyn");
                    visitors[i] = evelyn;
                    break;
                }
            }
        }

        for (int i = 0; i < visitors.Count; i++)
        {
            visitors[i].scheduleDelaySeconds = Math.Min(i * 0.4f, 7f);
        }

        // set schedule Delay for George and Evelyn so they arrive together (in theory)?
        NPC? george = visitors.FirstOrDefault((NPC npc) => npc.Name.Equals("George", StringComparison.OrdinalIgnoreCase));
        NPC? evelyn2 = visitors.FirstOrDefault((NPC npc) => npc.Name.Equals("Evelyn", StringComparison.OrdinalIgnoreCase));
        if (george is not null && evelyn2 is not null)
        {
            george.scheduleDelaySeconds = 7f;
            evelyn2.scheduleDelaySeconds = 6.8f;
        }

        Globals.ModMonitor.DebugLog($"{visitors.Count} vistors: {string.Join(", ", visitors.Select((NPC npc) => npc.Name))}");
        IslandSouthPatches.ClearCache();
        return visitors;
    }

    /// <summary>
    /// Returns either Gus if he's visiting, or a valid bartender from the bartender list.
    /// </summary>
    /// <param name="visitors">List of possible visitors for the day.</param>
    /// <returns>Bartender if it can find one, null otherwise.</returns>
    private static NPC? SetBartender(List<NPC> visitors)
    {
        NPC? bartender = visitors.Find((NPC npc) => npc.Name.Equals("Gus", StringComparison.OrdinalIgnoreCase));
        if (bartender is null)
        { // Gus not visiting, go find another bartender
            HashSet<NPC> bartenders = AssetManager.GetSpecialCharacter(SpecialCharacterType.Bartender);
            bartender = visitors.Find((NPC npc) => bartenders.Contains(npc));
        }
        if (bartender is not null)
        {
            bartender.currentScheduleDelay = 0f;
        }
        return bartender;
    }

    /// <summary>
    /// Returns a possible musician. Prefers Sam.
    /// </summary>
    /// <param name="random">The seeded random.</param>
    /// <param name="visitors">List of visitors.</param>
    /// <param name="animationDescriptions">Animation descriptions dictionary (pass this in to avoid rereading it).</param>
    /// <returns>Musician if it finds one.</returns>
    private static NPC? SetMusician(Random random, List<NPC> visitors, Dictionary<string, string> animationDescriptions)
    {
        NPC? musician = null;
        if (animationDescriptions.ContainsKey("sam_beach_towel"))
        {
            musician = visitors.Find((NPC npc) => npc.Name.Equals("Sam", StringComparison.OrdinalIgnoreCase));
        }
        if (musician is null || random.NextDouble() < 0.25)
        {
            HashSet<NPC> musicians = AssetManager.GetSpecialCharacter(SpecialCharacterType.Musician);
            musician = visitors.Find((NPC npc) => musicians.Contains(npc) && animationDescriptions.ContainsKey($"{npc.Name.ToLowerInvariant()}_beach_towel")) ?? musician;
        }
        if (musician is not null && !musician.Name.Equals("Gus", StringComparison.OrdinalIgnoreCase))
        {
            musician.currentScheduleDelay = 0f;
#if DEBUG
            Globals.ModMonitor.Log($"Found musician {musician.Name}", LogLevel.Debug);
#endif
            return musician;
        }
        return null;
    }

    /// <summary>
    /// Assigns everyone their island schedules for the day.
    /// </summary>
    /// /// <param name="random">Seeded random.</param>
    /// <param name="visitors">List of visitors.</param>
    /// <returns>A list of filled <see cref="GingerIslandTimeSlot"/>s.</returns>
    private static List<GingerIslandTimeSlot> AssignIslandSchedules(Random random, List<NPC> visitors, Dictionary<string, string> animationDescriptions)
    {
        Dictionary<NPC, string> lastactivity = new();
        List<GingerIslandTimeSlot> activities = TIMESLOTS.Select((i) => new GingerIslandTimeSlot(i, Bartender, Musician, random, visitors)).ToList();

        foreach (GingerIslandTimeSlot activity in activities)
        {
            lastactivity = activity.AssignActivities(lastactivity, animationDescriptions);
        }

        return activities;
    }

    /// <summary>
    /// Takes a list of activities and renders them as proper schedules.
    /// </summary>
    /// <param name="random">Sedded random.</param>
    /// <param name="visitors">List of visitors.</param>
    /// <param name="activities">List of activities.</param>
    /// <returns>Dictionary of NPC->raw schedule strings.</returns>
    private static Dictionary<NPC, string> RenderIslandSchedules(Random random, List<NPC> visitors, List<GingerIslandTimeSlot> activities)
    {
        Dictionary<NPC, string> completedSchedules = new();

        foreach (NPC visitor in visitors)
        {
            bool should_dress = IslandSouth.HasIslandAttire(visitor);
            List<SchedulePoint> scheduleList = new();

            if (should_dress)
            {
                scheduleList.Add(new SchedulePoint(
                    random: random,
                    npc: visitor,
                    map: "IslandSouth",
                    time: 1150,
                    point: IslandSouth.GetDressingRoomPoint(visitor),
                    animation: "change_beach",
                    isarrivaltime: true));
            }

            foreach (GingerIslandTimeSlot activity in activities)
            {
                if (activity.Assignments.TryGetValue(visitor, out SchedulePoint? schedulePoint))
                {
                    scheduleList.Add(schedulePoint);
                }
            }

            if (should_dress)
            {
                scheduleList.Add(new SchedulePoint(
                    random: random,
                    npc: visitor,
                    map: "IslandSouth",
                    time: 1730,
                    point: IslandSouth.GetDressingRoomPoint(visitor),
                    animation: "change_normal",
                    isarrivaltime: true));
            }

            scheduleList[0].IsArrivalTime = true; // set the first slot, whatever it is, to be the arrival time.

            // render the schedule points to strings before appending the remainder schedules
            // which are already strings.
            List<string> schedPointString = scheduleList.Select((SchedulePoint pt) => pt.ToString()).ToList();
            if (visitor.Name.Equals("Gus", StringComparison.OrdinalIgnoreCase))
            {
                // Gus needs to tend bar. Hardcoded same as vanilla.
                schedPointString.Add("1800 Saloon 10 18 2/2430 bed");
            }
            else
            {
                // Try to find a GI remainder schedule, if any.
                schedPointString.Add(ScheduleUtilities.FindProperGISchedule(visitor, SDate.Now()) ?? "1800 bed");
            }
            completedSchedules[visitor] = string.Join("/", schedPointString);
            Globals.ModMonitor.DebugLog($"For {visitor.Name}, created island schedule {completedSchedules[visitor]}");
        }
        return completedSchedules;
    }
}