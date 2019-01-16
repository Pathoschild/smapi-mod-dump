using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;

namespace ToolGeodes.Overrides
{
    public class HoeAccessor
    {
        public static List<Vector2> tilesAffected(Hoe wcan, Vector2 loc, int power, Farmer who)
        {
            return Mod.instance.Helper.Reflection.GetMethod(wcan, "tilesAffected").Invoke<List<Vector2>>(loc, power, who);
        }
    }

    public static class HoeStaminaHook
    {
        public static void Prefix(Hoe __instance, GameLocation location, int x, int y, int power, Farmer who)
        {
            who.Stamina += Math.Min(who.HasAdornment(ToolType.Hoe, Mod.Config.GEODE_LESS_STAMINA), 4) * 0.25f;
        }
    }

    public static class HoeRemoteUseHook
    {
        public static void Prefix(Hoe __instance, GameLocation location, ref int x, ref int y, int power, Farmer who)
        {
            if ( who.HasAdornment(ToolType.Hoe, Mod.Config.GEODE_REMOTE_USE) > 0 )
            {
                x = (int)who.lastClick.X;
                y = (int)who.lastClick.Y;
            }
        }
    }
}
