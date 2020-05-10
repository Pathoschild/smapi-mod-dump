using StardewValley;
using StardewValley.Tools;
using System;
using SObject = StardewValley.Object;

namespace DynamicPrice.Rewrites
{
    public class ObjectRewrites
    {
        public class SellToStorePriceRewrite
        {
            public static bool Prefix(SObject __instance, long specificPlayerID, ref int __result)
            {
                if (__instance is Fence)
                {
                    __result = __instance.Price;
                }
                else if (__instance.Category == -22)
                {
                    __result = (int)(__instance.Price * (1f + (__instance.Quality * 0.25f)) * ((FishingRod.maxTackleUses - __instance.uses.Value + 0f) / FishingRod.maxTackleUses));
                }
                else
                {
                    float startPrice = (int)(__instance.Price * (1f + (__instance.Quality * 0.25f)));
                    startPrice = ModEntry.reflection.GetMethod(__instance, "getPriceAfterMultipliers").Invoke<float>(startPrice, specificPlayerID);
                    if (__instance.ParentSheetIndex == 0x1ed)
                    {
                        startPrice /= 2f;
                    }
                    if (startPrice > 0f)
                    {
                        startPrice = Math.Max(1f, startPrice * Game1.MasterPlayer.difficultyModifier);
                    }
                    __result = (int)startPrice;
                }
                __result = DynamicPriceLogic.calc(__instance, __result);
                return false;
            }
        }
    }
}
