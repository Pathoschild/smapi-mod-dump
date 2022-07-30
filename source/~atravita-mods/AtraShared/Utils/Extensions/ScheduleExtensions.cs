/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace AtraShared.Utils.Extensions;

/// <summary>
/// Extension methods on Stardew's SchedulePathDescription class.
/// </summary>
public static class SchedulePointDescriptionExtensions
{
    /// <summary>
    /// Gets the expected travel time of a SchedulePathDescription.
    /// </summary>
    /// <param name="schedulePathDescription">Schedule Path Description.</param>
    /// <returns>Time in in-game minutes, not rounded.</returns>
    [Pure]
    public static int GetExpectedRouteTime(this SchedulePathDescription schedulePathDescription)
        => schedulePathDescription.route.Count * 32 / 42;
}