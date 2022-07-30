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

namespace AtraShared.Schedules.DataModels;

/// <summary>
/// A single schedule point.
/// </summary>
public sealed class SchedulePoint
{
    private readonly NPC npc;
    private readonly string map;
    private readonly int time;
    private readonly Point point;
    private readonly int direction = 2;
    private readonly string? animation;
    private readonly string? dialoguekey;

    private bool isarrivaltime = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="SchedulePoint"/> class.
    /// </summary>
    /// <param name="random">Seeded random.</param>
    /// <param name="npc">NPC.</param>
    /// <param name="map">String mapname.</param>
    /// <param name="time">Timestamp for schedule point. Normally is the departure time, but can be set to arrival time (isarrivaltime).</param>
    /// <param name="point">Tile to arrive at.</param>
    /// <param name="isarrivaltime">Whether or not this is an arrival time schedule.</param>
    /// <param name="direction">Direction to face after arriving.</param>
    /// <param name="animation">Which animation to use after arrival.</param>
    /// <param name="basekey">Base dialogue key.</param>
    /// <param name="varKey">Variant dialogue key.</param>
    /// <remarks>If a dialogue key that isn't in the NPC's dialogue is given, will simply convert  to `null`.</remarks>
    public SchedulePoint(
        Random random,
        NPC npc,
        string map,
        int time,
        Point point,
        bool isarrivaltime = false,
        int direction = Game1.down,
        string? animation = null,
        string? basekey = null,
        string? varKey = null)
    {
        this.npc = npc;
        this.map = map;
        this.time = time;
        this.isarrivaltime = isarrivaltime;
        this.point = point;
        this.direction = direction;
        this.animation = animation;
        if (npc.GetRandomDialogue(varKey, random) is string dialogueKey)
        {
            this.dialoguekey = dialogueKey;
        }
        else
        {
            this.dialoguekey = npc.GetRandomDialogue(basekey, random);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether gets whether or not the time should be an arrival time.
    /// </summary>
    /// <remarks>As the first point has to be an arrival, we allow this to be set outside this class.</remarks>
    public bool IsArrivalTime
    {
        get => this.isarrivaltime;
        set => this.isarrivaltime = value;
    }

    /// <summary>
    /// Gets the point the schedule is set to happen on.
    /// </summary>
    public Point Point => this.point;

    /// <summary>
    /// Gets the animation for this schedule point. Null for no animation.
    /// </summary>
    public string? Animation => this.animation;

    /// <summary>
    /// Method that converts a schedule point into a string Stardew can understand.
    /// </summary>
    /// <returns>Raw schedule string.</returns>
    [Pure]
    public override string ToString()
    {
        List<string> schedulestring = new()
        {
            $"{(this.isarrivaltime ? "a" : string.Empty)}{this.time}",
            this.map,
            this.point.X.ToString(),
            this.point.Y.ToString(),
            this.direction.ToString(),
        };

        if (this.animation is not null)
        {
            schedulestring.Add(this.animation);
        }
        if (this.dialoguekey is not null)
        {
            schedulestring.Add($"\"Characters\\Dialogue\\{this.npc.Name}:{this.dialoguekey}\"");
        }
        return string.Join(" ", schedulestring);
    }
}