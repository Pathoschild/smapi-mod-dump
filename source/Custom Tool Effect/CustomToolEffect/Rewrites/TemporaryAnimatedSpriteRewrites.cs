using CustomToolEffect;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;

namespace StardewModdingAPI.Mods.CustomLocalization.Rewrites
{
    public class TemporaryAnimatedSpriteRewrites
    {
        public class ConstructorRewrite
        {
            public static void Postfix(TemporaryAnimatedSprite __instance)
            {
                if(__instance.bombRadius > 0)
                {
                    if (ModEntry.ModConfig.BombDefine.TryGetValue(__instance.initialParentTileIndex, out ModConfig.PowerDefine define))
                    {
                        if (define.Power < 0)
                        {
                            define.Power = 1f;
                        }
                        __instance.bombRadius = (int)(__instance.bombRadius * define.Power);
                    }
                }
            }
        }
    }
}
