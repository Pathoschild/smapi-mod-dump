using Harmony;
using StardewValley;
using StardewValley.Locations;

namespace FishingAdjustMod
{
    [HarmonyPatch(typeof(Forest))]
    [HarmonyPatch("getFish")]
    [HarmonyPatch(new[] { typeof(float), typeof(int), typeof(int), typeof(global::StardewValley.Farmer), typeof(double) })]
    internal static class PatchForestGetFish
    {
        internal static bool Prefix(ref global::StardewValley.Object __result, int waterDepth, global::StardewValley.Farmer who)
        {
            if (Global.Config.OverrideGetWinterFishKing
                && Game1.currentSeason.Equals("winter")
                && who.getTileX() == 58
                && who.getTileY() == 87
                && who.FishingLevel >= 6
                && waterDepth >= 3
                && Game1.random.NextDouble() < Global.Config.WinterFishKingThreshold)
            {
                __result = new global::StardewValley.Object(775, 1, false, -1, 0);
                return false;
            }

            return true;
        }
    }
}
