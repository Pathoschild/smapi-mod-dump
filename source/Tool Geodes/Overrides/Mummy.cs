using StardewValley;
using StardewValley.Monsters;

namespace ToolGeodes.Overrides
{
    public static class MummyPiercingHook
    {
        public static void Prefix(Mummy __instance, int damage, int xTrajectory, int yTrajectory, ref bool isBomb, double addedPrecision, Farmer who)
        {
            if (who.HasAdornment(ToolType.Weapon, Mod.Config.GEODE_PIERCE_ARMOR) > 0)
            {
                int revTimer = Mod.instance.Helper.Reflection.GetField<int>(__instance, "reviveTimer").GetValue();
                if (revTimer > 0)
                {
                    isBomb = true;
                }
            }
        }
    }
}
