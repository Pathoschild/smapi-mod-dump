/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;

namespace BattleRoyale.Patches
{
    class PostEatFoodDebuff1 : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(Farmer), "doneEating");

        internal static Dictionary<long, DateTime> timesSinceLastEat = new();
        internal static int debuffMilliseconds = 3500;
        internal static float speedMultiplier = 0.6f;
        private static readonly int minimumHealthBonusForDebuff = 50;

        public static void Prefix(Farmer __instance, Item ___mostRecentlyGrabbedItem, Item ___itemToEat)
        {
            if (___mostRecentlyGrabbedItem != null && ___itemToEat is StardewValley.Object consumed)
            {
                int staminaToHeal = (int)Math.Ceiling((double)consumed.Edibility * 2.5) + (int)consumed.quality * consumed.Edibility;
                int healthBonus = ((consumed.Edibility >= 0) ? ((int)((float)staminaToHeal * 0.45f)) : 0);

                if (healthBonus >= minimumHealthBonusForDebuff)
                    timesSinceLastEat[__instance.UniqueMultiplayerID] = DateTime.Now;
            }
        }
    }

    class PostEatFoodDebuff2 : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(Farmer), "getMovementSpeed");

        public static void Postfix(Farmer __instance, ref float __result)
        {
            if (PostEatFoodDebuff1.timesSinceLastEat.ContainsKey(__instance.UniqueMultiplayerID))
            {
                if ((DateTime.Now - PostEatFoodDebuff1.timesSinceLastEat[__instance.UniqueMultiplayerID]).TotalMilliseconds < PostEatFoodDebuff1.debuffMilliseconds)
                    __result *= PostEatFoodDebuff1.speedMultiplier;
            }
        }
    }
}
