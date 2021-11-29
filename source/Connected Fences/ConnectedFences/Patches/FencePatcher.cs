/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewValley;
using StardewModdingAPI;

namespace ConnectedFences.Patches
{
    [HarmonyPatch(typeof(Fence))]
    class FencePatcher
    {
        private static IMonitor Monitor;

        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }
        static void Postfix(ref bool __result)
        {
            __result = true;
        }

        public static void Apply(Harmony harmony)
        {
            try
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(Fence), nameof(Fence.countsForDrawing)),
                    postfix: new HarmonyMethod(typeof(FencePatcher), nameof(FencePatcher.Fence_countsForDrawing_Postfix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add fences postfix: {ex}", LogLevel.Error);
            }
        }

        private static void Fence_countsForDrawing_Postfix(Fence __instance, ref bool __result)
        {
            if (((float)__instance.health > 1f || __instance.repairQueued.Value) && !__instance.isGate)
            {
                __result = true;
            }
            else
            {
                __result = false;
            }

        }
    }
}
