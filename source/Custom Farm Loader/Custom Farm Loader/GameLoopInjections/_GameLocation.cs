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
using Microsoft.Xna.Framework;
using System.Reflection;
using Custom_Farm_Loader.Lib;
using StardewValley.Buildings;

namespace Custom_Farm_Loader.GameLoopInjections
{
    public class _GameLocation
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
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.loadMap)),
               postfix: new HarmonyMethod(typeof(_GameLocation), nameof(_GameLocation.loadMap_Postfix))
            );

            harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(GameLocation), nameof(GameLocation.Map)),
                prefix: new HarmonyMethod(typeof(_GameLocation), nameof(_GameLocation.getMap_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.getFish)),
               prefix: new HarmonyMethod(typeof(_GameLocation), nameof(_GameLocation.getFish_Prefix))
            );
        }

        public static bool getMap_Prefix(GameLocation __instance, ref xTile.Map __result)
        {
            ReplaceMapProperties(__instance);

            return true;
        }

        public static void loadMap_Postfix(GameLocation __instance, string mapPath)
        {
            ReplaceMapProperties(__instance);
        }

        private static void ReplaceMapProperties(GameLocation __instance)
        {
            if (!CustomFarm.IsCFLMapSelected() || !__instance.Name.Equals("Farm"))
                return;

            CustomFarm customFarm = CustomFarm.getCurrentCustomFarm();


            //Warps
            foreach (var warp in customFarm.Warps) {
                var coordinates = new xTile.ObjectModel.PropertyValue(warp.Value.X + " " + warp.Value.Y);
                string propertyName = warp.Key.ToLower().Replace(" ", "") switch {
                    "busstop" => "BusStopEntry",
                    "forest" => "ForestEntry",
                    "backwoods" => "BackwoodsEntry",
                    "farmcave" => "FarmCaveEntry",
                    "warptotem" => "WarpTotemEntry",
                    _ => ""
                };

                if (__instance.map.Properties.ContainsKey(propertyName))
                    __instance.map.Properties.Remove(propertyName);

                __instance.map.Properties.Add(propertyName, coordinates);
            }


            //Locations
            foreach (var location in customFarm.Locations) {
                var coordinates = new xTile.ObjectModel.PropertyValue(location.Value.X + " " + location.Value.Y);
                string propertyName = location.Key.ToLower().Replace(" ", "") switch {
                    "farmhouse" => "FarmHouseEntry",
                    "greenhouse" => "GreenhouseLocation",
                    //The SpouseAreaLocation isn't updated in reloadMap, so we prefix GetSpouseOutdoorAreaCorner in _Farm
                    "spousearea" => "SpouseAreaLocation",
                    "farmcave" => "FarmCaveEntry",
                    "mailbox" => "MailboxLocation",
                    "shippingbin" => "ShippingBinLocation",
                    "grandpashrine" => "GrandpaShrineLocation",
                    _ => ""
                };

                if (__instance.map.Properties.ContainsKey(propertyName))
                    __instance.map.Properties.Remove(propertyName);

                __instance.map.Properties.Add(propertyName, coordinates);
            }
        }

        public static bool getFish_Prefix(GameLocation __instance, ref StardewValley.Object __result, float millisecondsAfterNibble, int bait, int waterDepth, Farmer who, double baitPotency, Vector2 bobberTile, string locationName = null)
        {

            if (!CustomFarm.IsCFLMapSelected())
                return true;

            if(__instance.IsFarm)
            foreach (Building b in (__instance as Farm).buildings)
                if (b is FishPond && b.isTileFishable(bobberTile)) {
                    __result = (b as FishPond).CatchFish();
                    return false;
                }

            CustomFarm customFarm = CustomFarm.getCurrentCustomFarm();

            bool isUsingMagicBait = __instance.IsUsingMagicBait(who);
            var validFishingRules = customFarm.FishingRules.FindAll(el => el.Area.LocationName == __instance.Name
                                                                       && el.Area.isTileIncluded(bobberTile)
                                                                       && el.Filter.isValid(excludeSeason: isUsingMagicBait, excludeTime: isUsingMagicBait, excludeWeather: isUsingMagicBait, who: who)
                                                                       && el.Fish.Count > 0);

            if (validFishingRules.Count == 0)
                return true;

            FishingRule chosenFishingRule = UtilityMisc.PickSomeInRandomOrder(validFishingRules, 1).First();

            __result = chosenFishingRule.getFish(isUsingMagicBait, millisecondsAfterNibble, bait, waterDepth, who, baitPotency, bobberTile);
            return false;
        }
    }
}