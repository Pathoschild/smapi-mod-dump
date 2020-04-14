using CustomToolEffect;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;

namespace StardewModdingAPI.Mods.CustomLocalization.Rewrites
{
    public class ResourceClumpRewrites
    {
        public class PerformToolActionRewrite
        {
            public static void Prefix(ResourceClump __instance, Tool t, int damage, Vector2 tileLocation, GameLocation location)
            {
                if (t == null || !(t is Axe || t is Pickaxe))
                    return;;
                switch (__instance.parentSheetIndex.Value)
                {
                    case 0x2f0:
                    case 0x2f2:
                    case 0x2f4:
                    case 0x2f6:
                        if (t is Pickaxe)
                        {
                            break;
                        }
                        return;;

                    case 0x2a0:
                        if ((t is Pickaxe) && (t.UpgradeLevel < 2))
                        {
                            return;;
                        }
                        if (!(t is Pickaxe))
                        {
                            return;;
                        }
                        break;

                    case 0x26e:
                        if ((t is Pickaxe) && (t.UpgradeLevel < 3))
                        {
                            return;;
                        }
                        if (!(t is Pickaxe))
                        {
                            return;;
                        }
                        break;

                    case 600:
                        if ((t is Axe) && (t.UpgradeLevel < 1))
                        {
                            return;;
                        }
                        if (!(t is Axe))
                        {
                            return;;
                        }
                        break;

                    case 0x25a:
                        if ((t is Axe) && (t.UpgradeLevel < 2))
                        {
                            return;;
                        }
                        if (!(t is Axe))
                        {
                            return;;
                        }
                        break;
                }
                float num = Math.Max((float)1f, (float)(((float)(t.UpgradeLevel + 1)) * 0.75f));
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
