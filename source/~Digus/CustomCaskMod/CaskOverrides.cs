/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using Harmony;
using StardewValley;
using StardewValley.Objects;

namespace CustomCaskMod
{
    internal class CaskOverrides
    {
        [HarmonyPriority(700)]
        public static bool GetAgingMultiplierForItem(ref Item item, ref float __result)
        {
            __result = 0f;
            if (item != null && Utility.IsNormalObjectAtParentSheetIndex(item, item.ParentSheetIndex))
            {
                if (DataLoader.CaskDataId.ContainsKey(item.ParentSheetIndex))
                {
                    __result = DataLoader.CaskDataId[item.ParentSheetIndex];
                }
                else if (DataLoader.CaskDataId.ContainsKey(item.Category))
                {
                    __result = DataLoader.CaskDataId[item.Category];
                }
                else
                {
                    return true;
                }

            }
            return false;
        }

        [HarmonyPriority(700)]
        public static bool IsValidCaskLocation(ref bool __result)
        {
            if (DataLoader.ModConfig.EnableCasksAnywhere)
            {
                __result = true;
                return false;
            }
            else
            {
                return true;
            }
        }

        [HarmonyPriority(700)]
        public static bool checkForMaturity(ref Cask __instance)
        {
            if (DataLoader.ModConfig.EnableMoreThanOneQualityIncrementPerDay)
            {
                if ((float)__instance.daysToMature.Value <= 0f)
                {
                    __instance.MinutesUntilReady = 1;
                    __instance.heldObject.Value.Quality = 4;
                }
                else if ((float)__instance.daysToMature.Value <= 28f)
                {
                    __instance.heldObject.Value.Quality = 2;
                }
                else if ((float)__instance.daysToMature.Value <= 42f)
                {
                    __instance.heldObject.Value.Quality = 1;
                }
                return false;
            }
            return true;
        }
    }
}
