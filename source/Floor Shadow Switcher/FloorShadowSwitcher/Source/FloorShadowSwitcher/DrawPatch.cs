using System.Reflection;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;
using Harmony;
using Netcode;

namespace FloorShadowSwitcher
{
    [HarmonyPatch]
    internal class DrawPatch
    {
        private static bool originalIsPathway;
        private static NetBool isPathway;

        private static MethodBase TargetMethod()
        {
            return typeof(Flooring).GetMethod(nameof(Flooring.draw));
        }

        private static void Prefix(Flooring __instance)
        {
            isPathway = ModEntry.Instance.Helper.Reflection.GetField<NetBool>(__instance, "isPathway").GetValue();
            originalIsPathway = isPathway.Value;

            // true config value means they want shadows, which is false for isPathway
            isPathway.Value = !ModEntry.Instance.Enabled[__instance.whichFloor.Value];
        }

        private static void Postfix(Flooring __instance)
        {
            isPathway.Value = originalIsPathway;
        }

    }
}
