/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace GingerIslandMainlandAdjustments.ScheduleManager;

/// <summary>
/// Extensions for schedules.
/// </summary>
internal static class ScheduleExtensionMethods
{
    /// <summary>
    /// Checks if the NPC has a specific schedule for today, where a specific schedule is a
    /// [season]_[day] or a marriage_[season]_[day].
    /// </summary>
    /// <param name="npc">NPC in question.</param>
    /// <returns>True if they have a specific schedule, false otherwise.</returns>
    internal static bool HasSpecificSchedule(this NPC npc)
        => npc.getSpouse() is not null
            ? npc.hasMasterScheduleEntry($"marriage_{Game1.currentSeason}_{Game1.dayOfMonth}")
            : npc.hasMasterScheduleEntry($"{Game1.currentSeason}_{Game1.dayOfMonth}");
}
