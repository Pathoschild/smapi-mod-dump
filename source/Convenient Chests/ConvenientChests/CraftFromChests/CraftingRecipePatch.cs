using System.Collections.Generic;
using System.Reflection;
using Harmony;
using StardewValley;

namespace ConvenientChests.CraftFromChests {
    public class CraftingRecipePatch {
        private static MethodInfo Original => AccessTools.Method(typeof(CraftingRecipe), "consumeIngredients");
        private static MethodInfo Prefix => typeof(CraftingRecipePatch).GetMethod("ConsumeIngredients");
        
        internal static void Register(HarmonyInstance harmony) {
            harmony.Patch(Original, new HarmonyMethod(Prefix), null);
        }

        internal static void Remove(HarmonyInstance harmony) {
            harmony.RemovePatch(Original, HarmonyPatchType.Prefix, harmony.Id);
        }

        public static bool ConsumeIngredients(CraftingRecipe __instance, Dictionary<int, int> ___recipeList) {
            if (CraftFromChestsModule.NearbyItems == null)
                return true;
            
            __instance.ConsumeIngredients(CraftFromChestsModule.NearbyInventories);
            return false;
        }
    }
}