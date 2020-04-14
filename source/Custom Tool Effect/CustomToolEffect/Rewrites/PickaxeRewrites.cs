using CustomToolEffect;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using System;
using SObject = StardewValley.Object;

namespace StardewModdingAPI.Mods.CustomLocalization.Rewrites
{
    public class PickaxeRewrites
    {
        public class DoFunctionRewrite
        {
            public static void Prefix(Pickaxe __instance, GameLocation location, int x, int y, int power, Farmer who)
            {
                if (!ModEntry.ModConfig.PickaxeDefine.TryGetValue(__instance.UpgradeLevel, out ModConfig.PowerDefine define)) {
                    return;
                }
                if (location.Objects.TryGetValue(new Vector2(x / 0x40, y / 0x40), out SObject obj))
                {
                    if (obj.Name == "Stone")
                    {
                        int num = Math.Max(1, __instance.UpgradeLevel + 1);
                        obj.MinutesUntilReady -= (int)(num * (define.Power - 1));
                    }
                }
            }
        }
    }
}
