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
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.reloadMap)),
               postfix: new HarmonyMethod(typeof(_GameLocation), nameof(_GameLocation.reloadMap_Postfix))
            );
        }
        public static void reloadMap_Postfix(GameLocation __instance)
        {
            ReplaceMapProperties(__instance);
        }

        private static void ReplaceMapProperties(GameLocation __instance)
        {
            if (!CustomFarm.IsCFLMapSelected() || !__instance.Name.Equals("Farm"))
                return;

            CustomFarm customFarm = CustomFarm.getCurrentCustomFarm();


            //Warps
            foreach (var warp in customFarm.Warps)
            {
                var coordinates = new xTile.ObjectModel.PropertyValue(warp.Value.X + " " + warp.Value.Y);
                string propertyName = warp.Key.ToLower().Replace(" ", "") switch
                {
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
            foreach (var location in customFarm.Locations)
            {
                var coordinates = new xTile.ObjectModel.PropertyValue(location.Value.X + " " + location.Value.Y);
                string propertyName = location.Key.ToLower().Replace(" ", "") switch
                {
                    "farmhouse" => "FarmHouseEntry",
                    "greenhouse" => "GreenhouseLocation",
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
    }
}