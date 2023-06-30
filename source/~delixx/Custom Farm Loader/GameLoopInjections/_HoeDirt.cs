/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-custom-farm-loader
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using HarmonyLib;
using StardewValley.TerrainFeatures;
using Custom_Farm_Loader.Lib;
using Custom_Farm_Loader.Lib.Enums;

namespace Custom_Farm_Loader.GameLoopInjections
{
    public class _HoeDirt
    {
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public static void Initialize(Mod mod)
        {
            Mod = mod;
            Monitor = mod.Monitor;
            Helper = mod.Helper;

            var harmony = new Harmony(mod.ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.seasonUpdate)),
               postfix: new HarmonyMethod(typeof(_HoeDirt), nameof(_HoeDirt.seasonUpdate_Postfix))
            );
        }

        public static void seasonUpdate_Postfix(HoeDirt __instance, bool onLoad)
        {
            if (__instance.crop == null || !CustomFarm.IsCFLMapSelected())
                return;

            clearWildCrops(__instance);
        }

        private static void clearWildCrops(HoeDirt __instance)
        {
            var customFarm = CustomFarm.getCurrentCustomFarm();

            if (!customFarm.DailyUpdates.Any(e =>
                    e.Type == DailyUpdateType.SpawnWildCrops
                    && __instance.currentLocation.Name == e.Area.LocationName
                    && e.Area.isTileIncluded(__instance.currentTileLocation)))
                return;

            //Spring Onions bug out after season change and ginger doesn't decay
            if (__instance.crop.forageCrop.Value)
                __instance.currentLocation.terrainFeatures.Remove(__instance.currentTileLocation);
        }
    }
}
