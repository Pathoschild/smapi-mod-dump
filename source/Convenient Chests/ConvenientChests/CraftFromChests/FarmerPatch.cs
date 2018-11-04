using System.Reflection;
using Harmony;
using StardewValley;

namespace ConvenientChests.CraftFromChests {
    public static class FarmerPatch {
        private static MethodInfo Original => AccessTools.Method(typeof(Farmer), "hasItemInInventory");
        private static MethodInfo Postfix  => typeof(FarmerPatch).GetMethod("HasItemInInventory");

        internal static void Register(HarmonyInstance harmony) {
            harmony.Patch(Original, null, new HarmonyMethod(Postfix));
        }

        internal static void Remove(HarmonyInstance harmony) {
            harmony.RemovePatch(Original, HarmonyPatchType.Postfix, harmony.Id);
        }

        public static bool HasItemInInventory(bool __result, StardewValley.Farmer __instance, int itemIndex, int quantity, int minPrice = 0) =>
            __result ||
            CraftFromChestsModule.NearbyItems != null && __instance.hasItemInList(CraftFromChestsModule.NearbyItems, itemIndex, quantity, minPrice);
    }
}