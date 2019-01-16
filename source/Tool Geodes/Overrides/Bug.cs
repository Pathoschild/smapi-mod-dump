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
