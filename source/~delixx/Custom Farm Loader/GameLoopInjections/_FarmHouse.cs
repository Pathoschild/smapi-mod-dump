/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-custom-farm-loader
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewValley;
using System.Xml;
using StardewModdingAPI.Events;
using System.Reflection;
using Custom_Farm_Loader.Lib;
using Custom_Farm_Loader.Lib.Enums;
using StardewValley.Locations;

namespace Custom_Farm_Loader.GameLoopInjections
{
    public class _FarmHouse
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
               original: AccessTools.Constructor(typeof(FarmHouse), new[] { typeof(string), typeof(string) }),
               postfix: new HarmonyMethod(typeof(_FarmHouse), nameof(_FarmHouse.FarmHouse_Postfix))
            );
        }

        public static void FarmHouse_Postfix(FarmHouse __instance, string m, string name)
        {
            fixBrokenTVs(__instance);
            if (!CustomFarm.IsCFLMapSelected())
                return;

            loadStartFurniture(__instance);
        }

        private static void fixBrokenTVs(FarmHouse __instance)
        {
            for (int i = 0; i < __instance.furniture.Count; i++) {
                var furniture = __instance.furniture[i];

                if (Furniture.TvIds.Exists(e => e == furniture.ParentSheetIndex.ToString()) && furniture.GetType().Name != "TV")
                    __instance.furniture[i] = new StardewValley.Objects.TV(furniture.ParentSheetIndex, furniture.TileLocation);
            }
        }

        private static void loadStartFurniture(FarmHouse __instance)
        {
            CustomFarm customFarm = CustomFarm.getCurrentCustomFarm();

            foreach (Furniture furniture in customFarm.StartFurniture.Where(el => el.LocationName == "FarmHouse"))
                furniture.tryPlacingFurniture(__instance);

        }

    }
}