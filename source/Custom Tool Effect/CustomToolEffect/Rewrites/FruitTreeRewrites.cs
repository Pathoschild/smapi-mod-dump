using CustomToolEffect;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace StardewModdingAPI.Mods.CustomLocalization.Rewrites
{
    public class FruitTreeRewrites
    {
        public class PerformToolActionRewrite
        {
            public static void Prefix(FruitTree __instance, Tool t, int explosion, Vector2 tileLocation)
            {
                if (t == null || !(t is Axe) || explosion > 0)
                    return;
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
                if(ModEntry.ModConfig.AxeDefine.TryGetValue(t.UpgradeLevel, out ModConfig.PowerDefine define))
                {
                    if (define.Power < 0)
                    {
                        define.Power = 1f;
                    }
                    __instance.health.Value -= num * (define.Power - 1);
                }
            }
        }
    }
}
