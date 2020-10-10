/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ZaneYork/CustomToolEffect
**
*************************************************/

using CustomToolEffect;
using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace StardewModdingAPI.Mods.CustomLocalization.Rewrites
{
    public class TreeRewrites
    {
        public class PerformToolActionRewrite
        {
            public static bool Prefix(Tree __instance, Tool t, int explosion, Vector2 tileLocation)
            {
                if (t == null || !(t is Axe) || explosion > 0)
                    return true;
                float num;
                switch (t.UpgradeLevel)
                {
                    case 0:
                        num = 1f;
                        break;
                    case 1:
                        num = 1.25f;
                        break;
                    case 2:
                        num = 1.67f;
                        break;
                    case 3:
                        num = 2.5f;
                        break;
                    case 4:
                        num = 5f;
                        break;
                    default:
                        num = 5f;
                        break;
                }
                if (__instance.growthStage.Value >= 3) {
                    num *= 2;
                }
                if(ModEntry.ModConfig.AxeDefine.TryGetValue(t.UpgradeLevel, out ModConfig.ToolDefine define))
                {
                    __instance.health.Value -= num + define.Power;
                }
                return true;
            }
        }
    }
}
