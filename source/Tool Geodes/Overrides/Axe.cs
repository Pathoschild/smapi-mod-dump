using StardewValley;
using StardewValley.Tools;
using System;

namespace ToolGeodes.Overrides
{
    public static class AxeStaminaHook
    {
        public static void Prefix(Axe __instance, GameLocation location, int x, int y, int power, Farmer who)
        {
            who.Stamina += Math.Min(who.HasAdornment(ToolType.Axe, Mod.Config.GEODE_LESS_STAMINA), 4) * 0.25f;
        }
    }

    public static class AxeRemoteUseHook
    {
        public static void Prefix(Hoe __instance, GameLocation location, ref int x, ref int y, int power, Farmer who)
        {
            if (who.HasAdornment(ToolType.Axe, Mod.Config.GEODE_REMOTE_USE) > 0)
            {
                x = (int)who.lastClick.X;
                y = (int)who.lastClick.Y;
            }
        }
    }
}
