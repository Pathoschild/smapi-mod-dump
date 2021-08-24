/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using HarmonyLib;
using StardewValley;

namespace IncubatePufferfishEggs
{
    internal class HarmonyPatches
    {
        [HarmonyPriority(500)]
        public static bool PerformObjectDropInAction(ref Object __instance, ref Item dropInItem, ref bool probe, ref Farmer who, ref bool __result)
        {
            if (!(dropInItem is Object object1)) return true;
            if (__instance.heldObject.Value != null && !__instance.Name.Equals("Recycling Machine") &&
                !__instance.Name.Equals("Mayonnaise Machine") || object1.bigCraftable.Value)
                return true;
            if (!__instance.name.Equals("Incubator")) return true;
            if ((__instance.heldObject.Value != null || object1.Category != -5 &&
                    !Utility.IsNormalObjectAtParentSheetIndex(object1, 107)
                ) && (__instance.heldObject.Value != null || object1.ParentSheetIndex != ModEntry.Instance.EggId &&
                    object1.ParentSheetIndex != ModEntry.Instance.LEggId))
                return true;
            __instance.heldObject.Value = new Object(object1.ParentSheetIndex, 1);
            if (probe) return true;
            
            who.currentLocation.playSound("coin");
            __instance.MinutesUntilReady = 9000 * (object1.ParentSheetIndex == 107 ? 2 : 1);
            
            if (who.professions.Contains(2))
                __instance.MinutesUntilReady /= 2;
            
            if (object1.ParentSheetIndex == 180 || object1.ParentSheetIndex == 182 || object1.ParentSheetIndex == 305)
                __instance.ParentSheetIndex += 2;
            else
                ++__instance.ParentSheetIndex;

            __result = true;
            return false;
        }
    }
}
