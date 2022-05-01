/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using Microsoft.Xna.Framework;
using HarmonyLib;
using MarketDay.Shop;
using MarketDay.Utility;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.Locations;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace MarketDay
{
    //     public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
    [HarmonyPatch(typeof(Chest))]
    [HarmonyPatch("checkForAction")]
    public class Prefix_Chest_checkForAction
    {
        public static bool Prefix(Chest __instance, Farmer who, bool justCheckingForActivity, ref bool __result)
        {
            if (justCheckingForActivity) return true;
            var owner = MapUtility.Owner(__instance);
            MarketDay.Log(
                $"Prefix_Chest_checkForAction checking {__instance} {__instance.DisplayName} owner {owner} at {__instance.TileLocation}",
                LogLevel.Debug, true);

            if (owner is null) return true;
            if (owner == "Player" || MarketDay.Config.PeekIntoChests) return true;

            MarketDay.Log(
                $"Prefix_Chest_checkForAction preventing action on object at {__instance.TileLocation} owned by {owner}",
                LogLevel.Debug, true);

            who.currentLocation.playSound("clank");
            __instance.shakeTimer = 500;
            __result = false;
            return false;
        }
    }

    //     public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
    [HarmonyPatch(typeof(Sign))]
    [HarmonyPatch("checkForAction")]
    public class Prefix_Sign_checkForAction
    {
        public static bool Prefix(Sign __instance, Farmer who, bool justCheckingForActivity, ref bool __result)
        {
            if (justCheckingForActivity) return true;
            var owner = MapUtility.Owner(__instance);
            MarketDay.Log(
                $"Prefix_Sign_checkForAction checking {__instance} {__instance.DisplayName} owner {owner} at {__instance.TileLocation}",
                LogLevel.Debug, true);

            if (owner is null or "Player") return true;

            MarketDay.Log(
                $"Prefix_Sign_checkForAction preventing action on object at {__instance.TileLocation} owned by {owner}",
                LogLevel.Debug, true);

            who.currentLocation.playSound("clank");
            __instance.shakeTimer = 500;
            __result = false;
            return false;
        }
    }

    // public virtual bool performUseAction(GameLocation location)
    // does not trip for Signs
    [HarmonyPatch(typeof(Object))]
    [HarmonyPatch("performUseAction")]
    public class Prefix_Object_performUseAction
    {
        public static bool Prefix(Object __instance, GameLocation location, ref bool __result)
        {
            var owner = MapUtility.Owner(__instance);
            MarketDay.Log(
                $"Prefix_Object_performUseAction checking {__instance} {__instance.DisplayName} owner {owner} at {__instance.TileLocation}",
                LogLevel.Debug, true);

            if (owner is null) return true;
            if (owner == "Player" || MarketDay.Config.PeekIntoChests) return true;

            MarketDay.Log(
                $"Prefix_Object_performUseAction preventing use of object at {__instance.TileLocation} owned by {owner}",
                LogLevel.Debug, true);

            location.playSound("clank");
            __instance.shakeTimer = 500;
            __result = false;
            return false;
        }
    }

    //     public virtual bool performToolAction(Tool t, GameLocation location)
    //    this one works leave it alone
    [HarmonyPatch(typeof(Object))]
    [HarmonyPatch("performToolAction")]
    public class Prefix_Object_performToolAction
    {
        public static bool Prefix(Object __instance, GameLocation location, ref bool __result)
        {
            var owner = MapUtility.Owner(__instance);
            MarketDay.Log(
                $"Prefix_Object_performToolAction checking {__instance} {__instance.DisplayName} owner {owner} at {__instance.TileLocation}",
                LogLevel.Debug, true);

            if (MarketDay.Config.RuinTheFurniture) return true;
            if (owner is null) return true;

            MarketDay.Log(
                $"Prefix_Object_performToolAction preventing damage to object at {__instance.TileLocation} owned by {owner}",
                LogLevel.Debug, true);
            location.playSound("clank");
            __instance.shakeTimer = 100;
            __result = false;
            return false;
        }
    }


    [HarmonyPatch(typeof(Chest))]
    [HarmonyPatch("draw")]
    [HarmonyPatch(new Type[] {typeof(SpriteBatch), typeof(int), typeof(int), typeof(float)})]
    public class Postfix_draw
    {
        public static void Postfix(Chest __instance, SpriteBatch spriteBatch, int x, int y)
        {
            if (!__instance.modData.TryGetValue($"{MarketDay.SMod.ModManifest.UniqueID}/{GrangeShop.StockChestKey}",
                out var ShopKey)) return;

            // get shop for ShopKey
            if (!ShopManager.GrangeShops.TryGetValue(ShopKey, out var grangeShop))
            {
                MarketDay.Log(
                    $"Postfix_draw: shop '{ShopKey}' not found in ShopManager.GrangeShops, can't draw",
                    LogLevel.Error);
                return;
            }

            var tileLocation = grangeShop.Origin;
            var drawLayer = Math.Max(0f, (tileLocation.Y * Game1.tileSize - 24) / 10000f) + tileLocation.X * 1E-05f;
            grangeShop.drawGrangeItems(tileLocation, spriteBatch, drawLayer);
            
            drawLayer = Math.Max(0f, (tileLocation.Y + 3) * Game1.tileSize / 10000f) + tileLocation.X * 1E-05f;
            grangeShop.DrawSign(tileLocation, spriteBatch, drawLayer);
        }
    }

    [HarmonyPatch(typeof(PathFindController))]
    [HarmonyPatch("findPathForNPCSchedules")]
    public class Postfix_findPathForNPCSchedules
    {
        public static void Postfix(Point startPoint, Point endPoint, GameLocation location, int limit, ref Stack<Point> __result)
        {
            if (location is not Town) return;
            if (!MarketDay.IsMarketDay()) return;
            if (!MarketDay.Config.NPCVisitors) return;
            if (MapUtility.ShopTiles.Count == 0) return;

            MarketDay.Log($"findPathForNPCSchedules {location.Name} {startPoint} -> {endPoint}", LogLevel.Trace, true);

            var placesToVisit = new List<Point>();
            foreach (var (shopX, shopY) in MapUtility.ShopTiles)
            {
                var visitPoint = new Point((int) shopX + Game1.random.Next(3), (int) shopY + 4);
                if (Game1.random.NextDouble() < MarketDay.Config.StallVisitChance) placesToVisit.Add(visitPoint);
            }

            StardewValley.Utility.Shuffle(Game1.random, placesToVisit);
            placesToVisit.Add(startPoint);
            if (placesToVisit.Count < 2) return;

            var waypoints = string.Join(", ", placesToVisit);
            MarketDay.Log($"    Waypoints: {waypoints}", LogLevel.Trace, true);

            // work backwards through the waypoints
            __result = new Stack<Point>();

            var thisEndPoint = endPoint;
            foreach (var (wptX, wptY) in placesToVisit)
            {
                var thisStartPoint = new Point(wptX, wptY);

                var originalPath = Schedule.findPathForNPCSchedules(thisStartPoint, thisEndPoint, location, limit);
                if (originalPath is null || originalPath.Count == 0) continue;

                var legPath = originalPath.ToList();
                legPath.Reverse();
                foreach (var pt in legPath) __result.Push(pt);
                thisEndPoint = thisStartPoint;
            }
        }
    }
}