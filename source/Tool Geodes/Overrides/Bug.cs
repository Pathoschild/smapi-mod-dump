/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/ToolGeodes
**
*************************************************/

using StardewValley;
using StardewValley.Monsters;

namespace ToolGeodes.Overrides
{
    public static class BugPiercingHook
    {
        public static void Prefix(Bug __instance, int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {
            if (who.HasAdornment(ToolType.Weapon, Mod.Config.GEODE_PIERCE_ARMOR) > 0)
            {
                if (__instance.isArmoredBug.Value)
                    __instance.isArmoredBug.Value = false;
            }
        }
    }
}
