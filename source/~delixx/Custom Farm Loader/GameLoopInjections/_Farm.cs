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
using StardewValley.Buildings;
using System.Xml;
using StardewModdingAPI.Events;
using System.Reflection;
using Custom_Farm_Loader.Lib;
using Custom_Farm_Loader.Lib.Enums;
using StardewValley.Locations;
using Microsoft.Xna.Framework;

namespace Custom_Farm_Loader.GameLoopInjections
{
    public class _Farm
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
               original: AccessTools.Method(typeof(Farm), nameof(Farm.performTenMinuteUpdate)),
               postfix: new HarmonyMethod(typeof(_Farm), nameof(_Farm.performTenMinuteUpdate_Postfix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(Farm), nameof(Farm.catchOceanCrabPotFishFromThisSpot)),
               prefix: new HarmonyMethod(typeof(_Farm), nameof(_Farm.catchOceanCrabPotFishFromThisSpot_Prefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Farm), nameof(Farm.GetSpouseOutdoorAreaCorner)),
                prefix: new HarmonyMethod(typeof(_Farm), nameof(_Farm.GetSpouseOutdoorAreaCorner_Prefix))
             );
        }

        public static bool GetSpouseOutdoorAreaCorner_Prefix(Farm __instance, ref Vector2 __result)
        {
            if (!CustomFarm.IsCFLMapSelected())
                return true;

            CustomFarm customFarm = CustomFarm.getCurrentCustomFarm();

            foreach (var location in customFarm.Locations)
                if (location.Key.ToLower().Replace(" ", "") == "spousearea") {
                    __result = Utility.PointToVector2(location.Value);
                    return false;
                }


            return true; ;
        }

        public static void performTenMinuteUpdate_Postfix(Farm __instance, int timeOfDay)
        {
            if (!CustomFarm.IsCFLMapSelected())
                return;

            CustomFarm customFarm = CustomFarm.getCurrentCustomFarm();

            updateFishSplash(__instance, customFarm, timeOfDay);
        }

        private static void updateFishSplash(Farm __instance, CustomFarm customFarm, int timeOfDay)
        {
            if (!Game1.IsMasterGame)
                return;

            if (!customFarm.Properties.FishSplashing)
                return;

            Random r = new Random(timeOfDay + (int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed);
            if (!__instance.fishSplashPoint.Value.Equals(Point.Zero) || r.NextDouble() < 0.5)
                return;

            for (int tries = 0; tries < 2; tries++) {
                Point p = new Point(r.Next(0, __instance.map.GetLayer("Back").LayerWidth), r.Next(0, __instance.map.GetLayer("Back").LayerHeight));
                if (!__instance.isOpenWater(p.X, p.Y) || __instance.doesTileHaveProperty(p.X, p.Y, "NoFishing", "Back") != null) {
                    continue;
                }
                int toLand = StardewValley.Tools.FishingRod.distanceToLand(p.X, p.Y, __instance);
                if (toLand > 1 && toLand < 5) {
                    if (Game1.player.currentLocation.Equals(__instance)) {
                        __instance.playSound("waterSlosh");
                    }
                    __instance.fishSplashPoint.Value = p;
                    break;

                }
            }
        }

        public static bool catchOceanCrabPotFishFromThisSpot_Prefix(Farm __instance, ref bool __result, int x, int y)
        {
            if (!CustomFarm.IsCFLMapSelected())
                return true;

            CustomFarm customFarm = CustomFarm.getCurrentCustomFarm();

            var validFishingRules = customFarm.FishingRules.FindAll(el => el.Area.isTileIncluded(new Vector2(x, y))
                                                                       && el.Filter.isValid()
                                                                       && el.ChangedCatchOceanCrabPotFish);

            if (validFishingRules.Count == 0)
                return true;

            FishingRule chosenFishingRule = UtilityMisc.PickSomeInRandomOrder(validFishingRules, 1).First();

            __result = chosenFishingRule.CatchOceanCrabPotFish;
            return false;
        }
    }
}