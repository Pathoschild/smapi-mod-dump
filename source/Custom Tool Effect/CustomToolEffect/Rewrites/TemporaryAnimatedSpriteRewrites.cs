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
