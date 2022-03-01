/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments
**
*************************************************/

namespace GingerIslandMainlandAdjustments.Utils;

/// <summary>
/// Methods to handle the current set of Islanders.
/// </summary>
internal static class Islanders
{
    /// <summary>
    /// Gets a list of Islanders.
    /// </summary>
    /// <returns>Sorted list of Islanders by name.</returns>
    public static List<string> Get()
    {
        List<string> islanders = new();
        foreach (string name in Game1.netWorldState.Value.IslandVisitors.Keys)
        {
            if (Game1.IsVisitingIslandToday(name))
            {
                islanders.Add(name);
            }
        }
        return Utils.ContextSort(islanders);
    }
}
