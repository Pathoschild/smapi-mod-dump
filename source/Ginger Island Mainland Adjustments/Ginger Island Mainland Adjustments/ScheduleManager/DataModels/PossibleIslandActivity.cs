/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments
**
*************************************************/

using Microsoft.Xna.Framework;

namespace GingerIslandMainlandAdjustments.ScheduleManager.DataModels;

/// <summary>
/// Struct that holds information about a possible island activity.
/// </summary>
internal readonly struct PossibleIslandActivity
{
    private readonly List<Point> possiblepoints;
    private readonly string map;
    private readonly int direction;
    private readonly string dialogueKey;
    private readonly bool animationRequired;
    private readonly string? animation;
    private readonly Func<NPC, double> chanceMap;

    /// <summary>
    /// Initializes a new instance of the <see cref="PossibleIslandActivity"/> struct.
    /// </summary>
    /// <param name="possiblepoints">A list of possible points for this activty.</param>
    /// <param name="map">Which map the activty is on. (default: IslandSouth).</param>
    /// <param name="direction">Which direction.</param>
    /// <param name="basechance">Base chance to pick this activity. Overridden by ChanceMap.</param>
    /// <param name="dialogueKey">Which dialogue key to use for this location.</param>
    /// <param name="animation_required">Whether or not the animation should be required.</param>
    /// <param name="animation">The animation to play. May be not required.</param>
    /// <param name="chanceMap">Function that maps specific NPCs to custom chances.</param>
    public PossibleIslandActivity(
        [NotNull] List<Point> possiblepoints,
        [NotNull] string map = "IslandSouth",
        [NotNull] int direction = 2,
        [NotNull] double basechance = 1.0,
        [NotNull] string dialogueKey = "Resort",
        [NotNull] bool animation_required = false,
        string? animation = null,
        Func<NPC, double>? chanceMap = null)
    {
        this.map = map;
        this.possiblepoints = possiblepoints;
        this.direction = direction;
        this.dialogueKey = dialogueKey;
        this.animationRequired = animation_required;
        this.animation = animation;
        this.chanceMap = chanceMap ?? ((NPC npc) => basechance);
    }

    /// <summary>
    /// Attempts to assign a SchedulePoint.
    /// </summary>
    /// <param name="random">Seeded random.</param>
    /// <param name="character">NPC to try scheduling.</param>
    /// <param name="time">Integer time for schedule, passed to SchedulePoint.</param>
    /// <param name="usedPoints">Points on the map already assigned.</param>
    /// <param name="lastAssignment">The previous assignment dictionary.</param>
    /// <param name="animation_descriptions">Dictionary of animations.</param>
    /// <param name="overrideChanceMap">Used to set the chances higher on a second pass. Null to leave unused.</param>
    /// <returns>SchedulePoint if one is assigned, null otherwise.</returns>
    public SchedulePoint? TryAssign(
        Random random,
        NPC character,
        int time,
        HashSet<Point> usedPoints,
        Dictionary<NPC, string>? lastAssignment = null,
        Dictionary<string, string>? animation_descriptions = null,
        Func<NPC, double>? overrideChanceMap = null)
    {
        string? schedule_animation = null;
        if (this.animation is not null)
        {
            schedule_animation = this.animation.StartsWith("square_") ? this.animation : $"{character.Name.ToLowerInvariant()}_{this.animation}";
        }
        // avoid repeating assignments.
        if (lastAssignment?.TryGetValue(character, out string? lastanimation) == true && this.IsAnimationUnique() && schedule_animation == lastanimation)
        {
            return null;
        }
        // Run a random chance to not pick this spot.
        double chance = overrideChanceMap is not null ? overrideChanceMap(character) : this.chanceMap(character);
        if (random.NextDouble() > chance)
        {
            return null;
        }

#pragma warning disable CS8604 // Possible null reference argument. IsAnimationUnique() prevents animation from being null
        // Check I have the animation to play if there's one specified
        if (this.IsAnimationUnique() && animation_descriptions?.ContainsKey(schedule_animation) == false)
#pragma warning restore CS8604 // Possible null reference argument.
        {
            if (this.animationRequired)
            {// if the animation is required, not for me
                return null;
            }
            schedule_animation = null; // remove animation if I don't have it.
        }

        Utility.Shuffle(random, this.possiblepoints);
        foreach (Point pt in this.possiblepoints)
        {
            if (usedPoints.Contains(pt))
            {
                continue;
            }
            return new SchedulePoint(
                random: random,
                npc: character,
                map: this.map,
                time: time,
                point: pt,
                isarrivaltime: false,
                direction: this.direction,
                animation: schedule_animation,
                basekey: this.dialogueKey);
        }
        return null;
    }

    /// <summary>
    /// Whether or not an animation is one we should avoid repeating.
    /// </summary>
    /// <returns>true if animation is unique, false otherwise.</returns>
    private bool IsAnimationUnique()
    {
        return !string.IsNullOrEmpty(this.animation)
            && !this.animation.StartsWith("square_");
    }
}