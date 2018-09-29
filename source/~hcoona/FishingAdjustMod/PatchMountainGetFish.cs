using Harmony;
using StardewValley;
using StardewValley.Locations;

namespace FishingAdjustMod
{
    [HarmonyPatch(typeof(Mountain))]
    [HarmonyPatch("getFish")]
    [HarmonyPatch(new[] { typeof(float), typeof(int), typeof(int), typeof(global::StardewValley.Farmer), typeof(double) })]
    internal static class PatchMountainGetFish
    {
        internal static bool Prefix(ref global::StardewValley.Object __result, int waterDepth, global::StardewValley.Farmer who)
        {
            if (Global.Config.OverrideGetSpringFishKing
                && Game1.currentSeason.Equals("spring")
                && Game1.isRaining
                && who.FishingLevel >= 10
                && waterDepth >= 4
                && Game1.random.NextDouble() < Global.Config.SpringFishKingThreshold)
            {
                __result = new global::StardewValley.Object(163, 1, false, -1, 0);
                return false;
            }

            return true;
        }
    }
}
