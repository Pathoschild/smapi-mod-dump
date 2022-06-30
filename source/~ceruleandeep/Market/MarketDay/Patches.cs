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
using MarketDay.Data;
using MarketDay.Shop;
using MarketDay.Utility;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.Characters;
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
            if (owner == $"Farmer:{who.Name}" || MarketDay.Config.PeekIntoChests) return true;

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

            if (owner is null || owner == $"Farmer:{who.Name}") return true;

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
            if (owner == $"Farmer:{Game1.player.Name}" || MarketDay.Config.PeekIntoChests) return true;

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
            if (tileLocation == Vector2.Zero) return;
            
            var drawLayer = Math.Max(0f, (tileLocation.Y * Game1.tileSize - 24) / 10000f) + tileLocation.X * 1E-05f;
            grangeShop.drawGrangeItems(tileLocation, spriteBatch, drawLayer);
            
            drawLayer = Math.Max(0f, (tileLocation.Y + 3) * Game1.tileSize / 10000f) + tileLocation.X * 1E-05f;
            grangeShop.DrawSign(tileLocation, spriteBatch, drawLayer);
        }
    }

    [HarmonyPatch(typeof(PathFindController))]
    [HarmonyPatch("findPathForNPCSchedules")]
    //  alter paths through Town to travel via market shops 
    public class Postfix_findPathForNPCSchedules
    {
        public static void Postfix(Point startPoint, Point endPoint, GameLocation location, int limit, ref Stack<Point> __result)
        {
            if (location is not Town) return;
            if (!MarketDay.IsMarketDay) return;
            if (!MarketDay.Config.NPCVisitors) return;
            if (MapUtility.ShopTiles.Count == 0) return;
            
            // if we're doing rescheduling we'll bend the pathfinding in pathfindToNextScheduleLocation
            if (MarketDay.Config.NPCVisitorRescheduling) return; 

            __result = Schedule.PathFindViaGrangeShops(startPoint, endPoint, location, limit, 3*60);
        }
    }

    [HarmonyPatch(typeof(NPC))]
    [HarmonyPatch("parseMasterSchedule")]
    //  public Dictionary<int, SchedulePathDescription> parseMasterSchedule(string rawData)
    //  rewrite NPC schedules to travel through town via market shops and visit them
    //  or if they have a shop to stand next to, do that if time allows
    public class Postfix_parseMasterSchedule
    {
        public static void Postfix(
            NPC __instance, 
            String rawData,
            ref Dictionary<int, SchedulePathDescription> __result)
        {
            if (!__instance.isVillager() && __instance is not Child) return;
            if (!MarketDay.Config.NPCVisitorRescheduling) return;
            if (!MarketDay.IsMarketDay) return;
            
            if (MapUtility.ShopTiles.Count == 0) return;
            
            __result = Schedule.parseMasterSchedule(__instance, rawData);
        }
    }
    
    [HarmonyPatch(typeof(NPC))]
    [HarmonyPatch("getSchedule")]
    //  public Dictionary<int, SchedulePathDescription> getSchedule(int dayOfMonth)
    //  if NPC has no schedule for today, and they have a shop to stand next to,
    //  schedule them to stand there all day
    public class Postfix_getSchedule
    {
        public static void Postfix(
            NPC __instance, 
            int dayOfMonth,
            ref Dictionary<int, SchedulePathDescription> __result)
        {
            if (!__instance.isVillager() && __instance is not Child) return;
            if (!MarketDay.Config.NPCOwnerRescheduling) return;
            if (!MarketDay.IsMarketDay) return;

            if (__result is not null && __result.Count > 0) return;
            
            __result = Schedule.getScheduleWhenNoDefault(__instance, dayOfMonth);
        }
    }
    
    [HarmonyPatch(typeof(NPC))]
    [HarmonyPatch("getMasterScheduleEntry")]
    //  public string getMasterScheduleEntry(string schedule_key)
    //  if the schedule key is something boring, maybe we should give them
    //  a more interesting one
    //
    //  TODO: check whether this allows NPC owners to visit their shops 
    public class Postfix_getMasterScheduleEntry
    {
        public static void Postfix(
            NPC __instance, 
            string schedule_key,
            ref string __result)
        {
            if (!MarketDay.Config.NPCScheduleReplacement) return;
            if (!__instance.isVillager() && __instance is not Child) return;
            if (!MarketDay.IsMarketDay) return;
            if (Schedule.TownieVisitorsToday is null) return;
            if (Schedule.TownieVisitorsToday.Count > MarketDay.Progression.NumberOfTownieVisitors) return;
            if (Schedule.IgnoreThisSchedule(__instance, __result)) return;
            if (MapUtility.ShopTiles.Count == 0) return;
            if (StardewValley.Utility.GetAllPlayerFriendshipLevel(__instance) <= 0) return;
            
            var alreadyVisitingIsland = Game1.netWorldState.Value.IslandVisitors.ContainsKey(__instance.Name);
            var couldVisitIslandButNot = IslandSouth.CanVisitIslandToday(__instance) && !alreadyVisitingIsland;

            var genericSchedule = "spring,summer,fall,winter,default".Split(",").Contains(schedule_key);
            
            if (!couldVisitIslandButNot) return;
            if (Schedule.ExcludedFromIslandEvents(__instance)) return;
            __result = Schedule.ScheduleStringForMarketVisit(__instance, __result);
            Schedule.TownieVisitorsToday.Add(__instance);
        }
    }
    
    [HarmonyPatch(typeof(NPC))]
    [HarmonyPatch("sayHiTo")]
    //  public void sayHiTo(Character c)
    //
    // things to talk about:
    // recently bought items
    // nearby shops
    public class PostfixSayHiTo
    {
        public static void Postfix(
            NPC __instance,
            Character c)
        {
            if (!MarketDay.IsMarketDay) return;
            if (__instance.currentLocation is null || __instance.currentLocation.Name != "Town") return;

            var qn = Game1.random.Next(1, 4);
            var an = Game1.random.Next(1, 4);
            var manners = __instance.Manners switch
            {
                1 => "polite",
                2 => "rude",
                _ => "neutral"
            };

            var call = MarketDay.Get($"dialog.question.{qn}", new { Name=c.displayName });
            var response = MarketDay.Get($"dialog.answer.{manners}.{an}",new { Name=__instance.displayName });

            var shops = MapUtility.OpenShops().Where(s => s.Owner() is not null).ToList();
            StardewValley.Utility.Shuffle(Game1.random, shops);
            
            foreach (var shop in shops)
            {
                var records = shop.Sales.Where(r => r.npc == __instance).ToList();
                if (records.Count > 0)
                {
                    var record = records[Game1.random.Next(records.Count)];
                    call = MarketDay.Get($"dialog.i-bought.{qn}", new { Name=c.displayName, Item=record.item.DisplayName, Owner=shop.Owner() });
                    response = MarketDay.Get($"dialog.i-bought-response.{manners}.{an}", new { Name=__instance.displayName, Item=record.item.DisplayName, Owner=shop.Owner() });
                    break;
                }

                records = shop.Sales.Where(r => r.npc == c).ToList();
                if (records.Count > 0)
                {
                    var record = records[Game1.random.Next(records.Count)];
                    call = MarketDay.Get($"dialog.did-you-buy.{qn}", new { Name=c.displayName, Item=record.item.DisplayName, Owner=shop.Owner() });
                    response = MarketDay.Get($"dialog.did-you-buy-response.{manners}.{an}", new { Name=__instance.displayName, Item=record.item.DisplayName, Owner=shop.Owner() });
                    break;
                }
            }
            
            __instance.showTextAboveHead(call);
            if (c is not NPC npc) return;
            if (Game1.random.NextDouble() >= 0.66)
                npc.showTextAboveHead(null);
            else
                npc.showTextAboveHead(response, preTimer: 1000 + Game1.random.Next(500));
        }
    }
}