/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace MoreBuildings.Overrides
{
    // Not sure why I need this in the first place, but it prevents an error from showing up during save loading
    // If the error shows up, update transition functions won't apply (and probably other things)
    public class ShedUpdateLayoutWorkaround
    {
        public static bool Prefix(Shed __instance)
        {
            // Why does this happen? Who knows
            if (__instance.map == null)
                return false;

            return true;
        }
    }
}
