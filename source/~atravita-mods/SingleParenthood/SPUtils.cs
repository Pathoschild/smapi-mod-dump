/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using StardewValley.Characters;

namespace SingleParenthood;
internal static class SPUtils
{
    internal static bool AllKidsOutOfCrib(this Farmer farmer)
        => farmer.getChildren().AllKidsOutOfCrib();

    internal static bool AllKidsOutOfCrib(this List<Child> kids)
    {
        foreach (var kid in kids)
        {
            if (kid.Age <= Child.crawler)
            {
                return false;
            }
        }
        return true;
    }
}
