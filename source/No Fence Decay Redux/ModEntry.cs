/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EnderTedi/No-Fence-Decay-Redux
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Fences;

namespace NoFenceDecayRedux
{
    internal sealed class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.PatchAll(typeof(ModEntry).Assembly);
        }
    }

    [HarmonyPatch(typeof(Fence), nameof(Fence.minutesElapsed))]
    public static class NoFenceDecay
    {
        private static bool Prefix(Fence __instance, ref bool __result)
        {
            return false;
        }
        private static void Postfix(Fence __instance, ref bool __result)
        {
            if (__instance.health.Value < __instance.maxHealth.Value && Game1.IsMasterGame)
            {
                FenceData data = __instance.GetData();
                __instance.ResetHealth(data.RepairHealthAdjustmentMaximum);
                __result = false;
            }
        }
    }
}
