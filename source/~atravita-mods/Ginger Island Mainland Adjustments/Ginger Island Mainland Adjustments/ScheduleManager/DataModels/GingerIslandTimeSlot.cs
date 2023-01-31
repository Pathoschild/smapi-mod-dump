/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Schedules.DataModels;
using AtraShared.Utils.Extensions;
using Microsoft.Xna.Framework;

namespace GingerIslandMainlandAdjustments.ScheduleManager.DataModels;

/// <summary>
/// A single timeslot on a Ginger Island schedule.
/// </summary>
internal class GingerIslandTimeSlot
{
    /// <summary>
    /// A list of possible island activities.
    /// </summary>
    private static readonly List<PossibleIslandActivity> PossibleActivities = GenerateIslandActivtyList();

    /// <summary>
    /// Location dancers can be in relation to a musician, if one is found.
    /// </summary>
    private static readonly List<Point> DanceDeltas = new()
    {
        new Point(1, 1),
        new Point(-1, -1),
        new Point(2, 0),
        new Point(0, 2),
    };

    /// <summary>
    /// Where the bartender should stand.
    /// </summary>
    private static readonly Point BartendPoint = new(14, 21);

    /// <summary>
    /// Activity for drinking (The adults only!). Should only happen if a bartender is around.
    /// </summary>
    private static readonly PossibleIslandActivity Drinking = new(new List<Point>() { new Point(12, 23), new Point(15, 23) },
        chanceMap: (NPC npc) => npc.Age == NPC.adult ? 0.5 : 0,
        animation: "beach_drink",
        animation_required: false,
        dialogueKey: "Resort_Bar");

    private static readonly PossibleIslandActivity Music = PossibleActivities[0];
    private static readonly PossibleIslandActivity Dance = PossibleActivities[1];

    private static readonly PossibleIslandActivity IslandNorth = new(new() { new Point(33, 83), new Point(34, 73), new Point(48, 81), new Point(52, 73) },
        map: "IslandNorth",
        basechance: 0.5,
        dialogueKey: "Resort_IslandNorth",
        animation: "square_3_3",
        animation_required: false,
        chanceMap: (NPC npc) => npc.Age == NPC.adult && npc.Optimism == NPC.positive && !npc.Name.Equals("George", StringComparison.OrdinalIgnoreCase) ? 0.5 : 0);

    /// <summary>
    /// Time this timeslot takes place in.
    /// </summary>
    private readonly int timeslot;

    private readonly NPC? bartender;
    private readonly NPC? musician;
    private readonly Random random;
    private readonly List<NPC> visitors;

    private readonly Dictionary<NPC, SchedulePoint> assignments = new();

    /// <summary>
    /// Location points already used, to avoid sticking two NPCs on top of each other...
    /// </summary>
    /// <remarks>Doesn't actually distinguish between different maps! This should be okay as different maps are shaped very differently.</remarks>
    private readonly HashSet<Point> usedPoints = new();

    private readonly Dictionary<NPC, string> animations = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="GingerIslandTimeSlot"/> class.
    /// </summary>
    /// <param name="timeslot">Time this TimeSlot should happen at.</param>
    /// <param name="bartender">Bartender, if I have one.</param>
    /// <param name="musician">Musician, if I have one.</param>
    /// <param name="random">Seeded random.</param>
    /// <param name="visitors">List of NPC visitors.</param>
    internal GingerIslandTimeSlot(int timeslot, NPC? bartender, NPC? musician, Random random, List<NPC> visitors)
    {
        this.timeslot = timeslot;
        this.bartender = bartender;
        this.musician = musician;
        this.random = random;
        this.visitors = visitors.ToList();
        Utility.Shuffle(random, this.visitors);
    }

    /// <summary>
    /// Gets dictionary of animations NPCs may be using for this GITimeSlot.
    /// </summary>
    internal Dictionary<NPC, string> Animations => this.animations;

    /// <summary>
    /// Gets which time this TimeSlot should happen at.
    /// </summary>
    internal int TimeSlot => this.timeslot;

    /// <summary>
    /// Gets a dictionary of current assignment (as a <see cref="SchedulePoint"/>) per NPC.
    /// </summary>
    internal Dictionary<NPC, SchedulePoint> Assignments => this.assignments;

    /// <summary>
    /// Tries to assign all characters to an activity.
    /// </summary>
    /// <param name="lastAssignment">The previous set of animations, to avoid repeating.</param>
    /// <param name="animationDescriptions">The animation dictionary of the game.</param>
    /// <returns>The animations used, so the next time slot has that information.</returns>
    internal Dictionary<NPC, string> AssignActivities(Dictionary<NPC, string> lastAssignment, Dictionary<string, string> animationDescriptions)
    {
        // Get a list of possible dancers (who have _beach_dance as a possible animation).
        HashSet<NPC> dancers = (this.musician is not null)
            ? this.visitors.Where((NPC npc) => animationDescriptions.ContainsKey($"{npc.Name.ToLowerInvariant()}_beach_dance")).ToHashSet()
            : new HashSet<NPC>();

        // assign bartenders and drinkers.
        if (this.bartender is not null)
        {
            {
                string? varkey = GIScheduler.CurrentVisitingGroup?.Contains(this.bartender) == true
                    ? $"Resort_Bartend_{GIScheduler.CurrentGroup}"
                    : null;
                this.AssignSchedulePoint(this.bartender, new SchedulePoint(
                    random: this.random,
                    npc: this.bartender,
                    map: "IslandSouth",
                    time: this.timeslot,
                    point: BartendPoint,
                    basekey: "Resort_Bartend",
                    varKey: varkey));
            }

            foreach (NPC possibledrinker in this.visitors)
            {
                if (!this.assignments.ContainsKey(possibledrinker) && possibledrinker.Age != NPC.child
                    && !dancers.Contains(possibledrinker) && possibledrinker != this.musician)
                {
                    SchedulePoint? schedulePoint = Drinking.TryAssign(
                        random: this.random,
                        character: possibledrinker,
                        time: this.timeslot,
                        usedPoints: this.usedPoints,
                        lastAssignment: lastAssignment,
                        animation_descriptions: animationDescriptions,
                        groupName: GIScheduler.CurrentVisitingGroup?.Contains(possibledrinker) == true ? GIScheduler.CurrentGroup : null);
                    if (schedulePoint is not null)
                    {
                        this.AssignSchedulePoint(possibledrinker, schedulePoint);
                    }
                }
            }
        }

        // assign musician and dancers
        if (this.musician is not null && !this.assignments.ContainsKey(this.musician))
        {
            SchedulePoint? musicianPoint = Music.TryAssign(
                random: this.random,
                character: this.musician,
                time: this.timeslot,
                usedPoints: this.usedPoints,
                lastAssignment: lastAssignment,
                overrideChanceMap: static (NPC npc) => 0.8,
                animation_descriptions: animationDescriptions,
                groupName: GIScheduler.CurrentVisitingGroup?.Contains(this.musician) == true ? GIScheduler.CurrentGroup : null);
            if (musicianPoint is not null)
            {
                Globals.ModMonitor.DebugOnlyLog($"Assigned musician:{this.musician.Name}", LogLevel.Debug);
                this.AssignSchedulePoint(this.musician, musicianPoint);
                Point musician_loc = musicianPoint.Point;
                PossibleIslandActivity closeDancePoint = new(DanceDeltas.Select((Point pt) => new Point(musician_loc.X + pt.X, musician_loc.Y + pt.Y)).ToList(),
                    basechance: 0.7,
                    animation: "beach_dance",
                    animation_required: true);
                foreach (NPC dancer in dancers)
                {
                    SchedulePoint? dancerPoint = closeDancePoint.TryAssign(
                        random: this.random,
                        character: dancer,
                        time: this.timeslot,
                        usedPoints: this.usedPoints,
                        lastAssignment: lastAssignment,
                        animation_descriptions: animationDescriptions,
                        groupName: GIScheduler.CurrentVisitingGroup?.Contains(dancer) == true ? GIScheduler.CurrentGroup : null)
                        ?? Dance.TryAssign(
                            this.random,
                            character: dancer,
                            time: this.timeslot,
                            usedPoints: this.usedPoints,
                            lastAssignment: lastAssignment,
                            animation_descriptions: animationDescriptions,
                            groupName: GIScheduler.CurrentVisitingGroup?.Contains(dancer) == true ? GIScheduler.CurrentGroup : null);
                    if (dancerPoint is not null)
                    {
                        Globals.ModMonitor.DebugOnlyLog($"Assigned dancer {dancer.Name}", LogLevel.Debug);
                        this.AssignSchedulePoint(dancer, dancerPoint);
                        dancer.currentScheduleDelay = 0f;
                        this.musician.currentScheduleDelay = 0f;
                    }
                }
            }
#if DEBUG
            else
            {
                Globals.ModMonitor.Log($"Musician {this.musician.Name} skipped for MusicianPoint", LogLevel.Trace);
            }
#endif
        }

        // consider assigning NPC groups?

        // assign the rest of the NPCs
        foreach (NPC visitor in this.visitors)
        {
            if (this.assignments.ContainsKey(visitor))
            {
                continue;
            }
            foreach (PossibleIslandActivity possibleIslandActivity in PossibleActivities)
            {
                SchedulePoint? schedulePoint = possibleIslandActivity.TryAssign(
                    random: this.random,
                    character: visitor,
                    time: this.timeslot,
                    usedPoints: this.usedPoints,
                    lastAssignment: lastAssignment,
                    animation_descriptions: animationDescriptions,
                    groupName: GIScheduler.CurrentVisitingGroup?.Contains(visitor) == true ? GIScheduler.CurrentGroup : null);
                if (schedulePoint is not null)
                {
                    this.AssignSchedulePoint(visitor, schedulePoint);
                    goto CONTINUELOOP;
                }
            }

            if (this.timeslot == 1400)
            {
                SchedulePoint? schedulePoint = IslandNorth.TryAssign(
                    random: this.random,
                    character: visitor,
                    time: this.timeslot,
                    usedPoints: this.usedPoints,
                    lastAssignment: lastAssignment,
                    animation_descriptions: animationDescriptions,
                    groupName: GIScheduler.CurrentVisitingGroup?.Contains(visitor) == true ? GIScheduler.CurrentGroup : null);
                if (schedulePoint is not null)
                {
                    this.AssignSchedulePoint(visitor, schedulePoint);
                    goto CONTINUELOOP;
                }
            }

            Globals.ModMonitor.DebugOnlyLog($"Now using fall back spot assignment for {visitor.Name} at {this.timeslot}", LogLevel.Warn);

            // now iterate backwards through the list, forcibly assigning people to places....
            for (int i = PossibleActivities.Count - 1; i >= 0; i--)
            {
                SchedulePoint? schedulePoint = PossibleActivities[i].TryAssign(
                    random: this.random,
                    character: visitor,
                    time: this.timeslot,
                    usedPoints: this.usedPoints,
                    lastAssignment: lastAssignment,
                    animation_descriptions: animationDescriptions,
                    overrideChanceMap: (NPC npc) => 1.0,
                    groupName: GIScheduler.CurrentVisitingGroup?.Contains(visitor) == true ? GIScheduler.CurrentGroup : null);
                if (schedulePoint is not null)
                {
                    this.AssignSchedulePoint(visitor, schedulePoint);
                    goto CONTINUELOOP;
                }
            }
            Globals.ModMonitor.DebugOnlyLog($"Warning: No activity found for {visitor.Name} at {this.timeslot}", LogLevel.Warn);
CONTINUELOOP:
            ;
        }
        return this.animations;
    }

    /// <summary>
    /// Adds a schedule point to the usedPoints dictionary, the animations log, and the character's assignment.
    /// </summary>
    /// <param name="npc">NPC in question.</param>
    /// <param name="schedulePoint">SchedulePoint to assign.</param>
    private void AssignSchedulePoint(NPC npc, SchedulePoint schedulePoint)
    {
        this.usedPoints.Add(schedulePoint.Point);
        if (schedulePoint.Animation is not null)
        {
            this.animations[npc] = schedulePoint.Animation;
        }
        this.assignments[npc] = schedulePoint;
    }

    /// <summary>
    /// Generates a list of possible activities.
    /// </summary>
    /// <returns>List of PossibleIslandActivities.</returns>
    [Pure]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "Reviewed")]
    private static List<PossibleIslandActivity> GenerateIslandActivtyList()
    {
        return new List<PossibleIslandActivity>()
        {
            // towel lounging
            new PossibleIslandActivity(
                new List<Point> { new Point(14, 27), new Point(17, 28), new Point(20, 27), new Point(23, 28) },
                basechance: 0.6,
                dialogueKey: "Resort_Towel",
                animation_required: true,
                animation: "beach_towel"),
            // dancing
            new PossibleIslandActivity(
                new List<Point> { new Point(22, 21), new Point(23, 21) },
                chanceMap: static (NPC npc) => npc.Name.Equals("Emily", StringComparison.OrdinalIgnoreCase) ? 0.7 : 0.5,
                dialogueKey: "Resort_Dance",
                animation: "beach_dance",
                animation_required: true),
            // wandering
            new PossibleIslandActivity(
                new List<Point> { new Point(7, 16), new Point(31, 24), new Point(18, 13), new Point(24, 15) },
                basechance: 0.4,
                dialogueKey: "Resort_Wander",
                animation: "square_3_3"),
            // fishing
            new PossibleIslandActivity(
                new List<Point> { new Point(21, 44) },
#if DEBUG
                basechance: 1,
#else
                basechance: 0.4,
#endif
                dialogueKey: "Resort_Fish",
                animation_required: true,
                animation: "beach_fish"),
            // under umbrella
            new PossibleIslandActivity(
                new List<Point> { new Point(26, 26), new Point(28, 29), new Point(10, 27) },
                chanceMap: static (NPC npc) => npc.Name.Equals("Abigail", StringComparison.OrdinalIgnoreCase) ? 0.5 : 0.3,
                dialogueKey: "Resort_Umbrella",
                animation: "beach_umbrella",
                animation_required: false),
            // sitting on chair
            new PossibleIslandActivity(
                new List<Point> { new Point(20, 24), new Point(30, 29) },
                dialogueKey: "Resort_Chair",
                basechance: 0.6,
                chanceMap: static (NPC npc) => (npc.Age == NPC.adult || (npc.Age == NPC.teen && npc.SocialAnxiety == NPC.shy)) ? 0.6 : 0,
                animation: "beach_chair",
                animation_required: false),
#if DEBUG
            // antisocial point
            new PossibleIslandActivity(
                new List<Point> { new Point(3, 28) },
                dialogueKey: "Resort_Antisocial",
                basechance: 0,
                chanceMap: static (NPC npc) => npc.SocialAnxiety == NPC.shy && npc.Optimism == NPC.negative && !npc.Name.Equals("George", StringComparison.OrdinalIgnoreCase) ? 0.6 : 0.0,
                map: "IslandSouthEast",
                animation: "beach_antisocial",
                animation_required: false),
#endif
            // shore points
            new PossibleIslandActivity(
                new List<Point> { new Point(6, 34), new Point(9, 33), new Point(13, 33), new Point(17, 33), new Point(24, 33), new Point(28, 32), new Point(32, 31), new Point(37, 31) },
                dialogueKey: "Resort_Shore",
                basechance: 0.25,
                animation: "beach_shore",
                animation_required: false),
            // pier points
            new PossibleIslandActivity(
                new List<Point> { new Point(22, 43), new Point(22, 40) },
                dialogueKey: "Resort_Pier",
                basechance: 0.25,
                direction: Game1.right,
                animation: "beach_pier",
                animation_required: false,
                chanceMap: static (NPC npc) => npc.Age == NPC.adult ? 0.5 : 0),
        };
    }
}