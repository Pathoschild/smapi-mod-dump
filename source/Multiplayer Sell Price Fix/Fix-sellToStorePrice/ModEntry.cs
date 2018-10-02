using System;
using System.Reflection;
using System.Runtime.CompilerServices;

using StardewValley;

using StardewModdingAPI;

using Harmony;
using System.Collections.Generic;
using Netcode;

namespace Fix_sellToStorePrice
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            var harmony = HarmonyInstance.Create("ca.drau.stardewvalley");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [HarmonyPatch(typeof(StardewValley.Object))]
        [HarmonyPatch("sellToStorePrice")]
        class Patch
        {
            static bool Prefix()
            {
                return false;
            }

            static void Postfix(StardewValley.Object __instance, ref int __result)
            {
                HashSet<int> professions = new HashSet<int>();

                professions.UnionWith(Game1.player.professions);

                foreach (Farmer farmer in Game1.otherFarmers.Values)
                {
                    professions.UnionWith(farmer.professions);
                }

                // Begin copy-pasta

                if (__instance is Fence)
                {
                    __result = (int)((NetFieldBase<int, NetInt>)__instance.price);
                    return;
                }

                if (__instance.Category == -22)
                {
                    __result = (int)((double)(int)((NetFieldBase<int, NetInt>)__instance.price) * (1.0 + (double)(int)((NetFieldBase<int, NetInt>)__instance.quality) * 0.25) * (double)__instance.scale.Y);
                    return;
                }

                float num = (float)(int)((double)(int)((NetFieldBase<int, NetInt>)__instance.price) * (1.0 + (double)(int)((NetFieldBase<int, NetInt>)__instance.quality) * 0.25));
                bool flag = false;
                if (__instance.name.ToLower().Contains("mayonnaise") || __instance.name.ToLower().Contains("cheese") || (__instance.name.ToLower().Contains("cloth") || __instance.name.ToLower().Contains("wool")))
                    flag = true;
                if (professions.Contains(0) && (flag || __instance.Category == -5 || (__instance.Category == -6 || __instance.Category == -18)))
                    num *= 1.2f;
                if (professions.Contains(1) && (__instance.Category == -75 || __instance.Category == -80 || __instance.Category == -79 && !(bool)((NetFieldBase<bool, NetBool>)__instance.isSpawnedObject)))
                    num *= 1.1f;
                if (professions.Contains(4) && __instance.Category == -26)
                    num *= 1.4f;
                if (professions.Contains(6) && __instance.Category == -4)
                    num *= professions.Contains(8) ? 1.5f : 1.25f;
                if (professions.Contains(12) && ((int)((NetFieldBase<int, NetInt>)__instance.parentSheetIndex) == 388 || (int)((NetFieldBase<int, NetInt>)__instance.parentSheetIndex) == 709))
                    num *= 1.5f;
                if (professions.Contains(15) && __instance.Category == -27)
                    num *= 1.25f;
                if (professions.Contains(20) && (int)((NetFieldBase<int, NetInt>)__instance.parentSheetIndex) >= 334 && (int)((NetFieldBase<int, NetInt>)__instance.parentSheetIndex) <= 337)
                    num *= 1.5f;
                if (professions.Contains(23) && (__instance.Category == -2 || __instance.Category == -12))
                    num *= 1.3f;
                if (Game1.player.eventsSeen.Contains(2120303) && ((int)((NetFieldBase<int, NetInt>)__instance.parentSheetIndex) == 296 || (int)((NetFieldBase<int, NetInt>)__instance.parentSheetIndex) == 410))
                    num *= 3f;
                if (Game1.player.eventsSeen.Contains(3910979) && (int)((NetFieldBase<int, NetInt>)__instance.parentSheetIndex) == 399)
                    num *= 5f;
                if ((int)((NetFieldBase<int, NetInt>)__instance.parentSheetIndex) == 493)
                    num /= 2f;
                if ((double)num > 0.0)
                    num = Math.Max(1f, num * Game1.MasterPlayer.difficultyModifier);
                __result = (int)num;
            }
        }
    }
}
