using System;
using System.Collections.Generic;
using CustomToolEffect;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using static CustomToolEffect.ModConfig;

namespace StardewModdingAPI.Mods.CustomLocalization.Rewrites
{
    public class ToolRewrites
    {
        public class TilesAffectedRewrite
        {
            public static bool Prefix(Tool __instance, Vector2 tileLocation, int power, Farmer who, ref List<Vector2> __result)
            {
                ModConfig.RangeDefine define = null;
                if (__instance is Hoe)
                {
                    ModEntry.ModConfig.HoeDefine.TryGetValue(power, out define);
                }
                else if (__instance is WateringCan)
                {
                    ModEntry.ModConfig.WateringCanDefine.TryGetValue(power, out define);
                }
                if (define == null)
                {
                    return true;
                }
                Range range = define.Range;
                __result = new List<Vector2> {
                        tileLocation
                    };
                Vector2 directionLength, directionWidth;
                switch (who.FacingDirection)
                {
                    case 0:
                        directionLength = new Vector2(0f, -1f);
                        directionWidth = new Vector2(1f, 0f);
                        break;
                    case 1:
                        directionLength = new Vector2(1f, 0f);
                        directionWidth = new Vector2(0f, 1f);
                        break;
                    case 2:
                        directionLength = new Vector2(0f, 1f);
                        directionWidth = new Vector2(-1f, 0f);
                        break;
                    default:
                        directionLength = new Vector2(-1f, 0f);
                        directionWidth = new Vector2(0f, -1f);
                        break;
                }
                int rangeWidthStart = (int)Math.Ceiling(range.Width / -2.0f);
                int rangeWidthEnd = (int)Math.Ceiling(range.Width / 2.0f);
                for (int offsetWidth = rangeWidthStart; offsetWidth < rangeWidthEnd; offsetWidth++)
                {
                    for (int offsetLength = 0; offsetLength < range.Length; offsetLength++)
                    {
                        __result.Add(directionWidth * offsetWidth + directionLength * offsetLength + tileLocation);
                    }
                }
                return false;
            }
        }
    }
}
