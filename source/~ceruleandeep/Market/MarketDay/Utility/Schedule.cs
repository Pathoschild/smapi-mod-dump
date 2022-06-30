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
using System.Diagnostics;
using System.Linq;
using MarketDay.Shop;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Network;
using xTile;
using xTile.ObjectModel;
using xTile.Tiles;

namespace MarketDay.Utility
{
    public static class Schedule
    {
        public static Dictionary<int, List<string>> NPCInteractions = new();
        internal static HashSet<NPC> TownieVisitorsToday = new();

        private static Stack<Point> FindPathForNpcSchedules(
            Point startPoint,
            Point endPoint,
            GameLocation location,
            int limit)
        {
            PriorityQueue priorityQueue = new PriorityQueue();
            HashSet<int> intSet = new HashSet<int>();
            int num = 0;
            priorityQueue.Enqueue(new PathNode(startPoint.X, startPoint.Y, 0, null),
                Math.Abs(endPoint.X - startPoint.X) + Math.Abs(endPoint.Y - startPoint.Y));
            PathNode pathNode1 = (PathNode) priorityQueue.Peek();
            int layerWidth = location.map.Layers[0].LayerWidth;
            int layerHeight = location.map.Layers[0].LayerHeight;
            while (!priorityQueue.IsEmpty())
            {
                PathNode pathNode2 = priorityQueue.Dequeue();
                if (pathNode2.x == endPoint.X && pathNode2.y == endPoint.Y)
                    return PathFindController.reconstructPath(pathNode2);
                intSet.Add(pathNode2.id);
                for (int index = 0; index < 4; ++index)
                {
                    int x = pathNode2.x + Directions[index, 0];
                    int y = pathNode2.y + Directions[index, 1];
                    int hash = PathNode.ComputeHash(x, y);
                    if (!intSet.Contains(hash))
                    {
                        PathNode p = new PathNode(x, y, pathNode2);
                        p.g = (byte) (pathNode2.g + 1U);
                        if (p.x == endPoint.X && p.y == endPoint.Y || p.x >= 0 && p.y >= 0 &&
                            (p.x < layerWidth && p.y < layerHeight) &&
                            !isPositionImpassableForNPCSchedule(location, p.x, p.y))
                        {
                            int priority = p.g +
                                           getPreferenceValueForTerrainType(location, p.x, p.y) +
                                           (Math.Abs(endPoint.X - p.x) + Math.Abs(endPoint.Y - p.y) +
                                            (p.x == pathNode2.x && p.x == pathNode1.x ||
                                             p.y == pathNode2.y && p.y == pathNode1.y
                                                ? -2
                                                : 0));
                            if (!priorityQueue.Contains(p, priority))
                                priorityQueue.Enqueue(p, priority);
                        }
                    }
                }

                pathNode1 = pathNode2;
                ++num;
                if (num >= limit)
                    return null;
            }

            return null;
        }

        private static readonly sbyte[,] Directions = {{-1, 0}, {1, 0}, {0, 1}, {0, -1}};

        private static bool isPositionImpassableForNPCSchedule(GameLocation loc, int x, int y)
        {
            Tile tile = loc.Map.GetLayer("Buildings").Tiles[x, y];
            if (tile != null && tile.TileIndex != -1)
            {
                tile.TileIndexProperties.TryGetValue("Action", out var propertyValue);
                if (propertyValue == null)
                    tile.Properties.TryGetValue("Action", out propertyValue);
                if (propertyValue != null)
                {
                    string str = propertyValue.ToString();
                    if (str.StartsWith("LockedDoorWarp") || !str.Contains("Door") && !str.Contains("Passable"))
                        return true;
                }
                else if (loc.doesTileHaveProperty(x, y, "Passable", "Buildings") == null &&
                         loc.doesTileHaveProperty(x, y, "NPCPassable", "Buildings") == null)
                    return true;
            }

            if (loc.doesTileHaveProperty(x, y, "NoPath", "Back") != null)
                return true;
            foreach (Warp warp in loc.warps)
            {
                if (warp.X == x && warp.Y == y)
                    return true;
            }

            return loc.isTerrainFeatureAt(x, y);
        }

        private static int getPreferenceValueForTerrainType(GameLocation l, int x, int y)
        {
            string str = l.doesTileHaveProperty(x, y, "Type", "Back");
            if (str != null)
            {
                string lower = str.ToLower();
                if (lower == "stone")
                    return -7;
                if (lower == "wood")
                    return -4;
                if (lower == "dirt")
                    return -2;
                if (lower == "grass")
                    return -1;
            }

            return 0;
        }

        public static Dictionary<int, SchedulePathDescription> parseMasterSchedule(NPC npc, string rawData)
        {
            var defaultPosition = MarketDay.helper.Reflection.GetField<NetVector2>(npc, "defaultPosition");
            var previousEndPoint = MarketDay.helper.Reflection.GetField<Point>(npc, "previousEndPoint");
            var _lastLoadedScheduleKey = MarketDay.helper.Reflection.GetField<string>(npc, "_lastLoadedScheduleKey");
            var getLocationRoute = MarketDay.helper.Reflection.GetMethod(npc, "getLocationRoute");

            MarketDay.Log($"parseMasterSchedule {npc.Name}: {rawData}", LogLevel.Trace, true);

            string[] scriptParts = rawData.Split('/');
            Dictionary<int, SchedulePathDescription> masterSchedule = new Dictionary<int, SchedulePathDescription>();
            int num = 0;
            if (scriptParts[0].Contains("GOTO"))
            {
                string text = scriptParts[0].Split(' ')[1];
                if (text.ToLower().Equals("season"))
                {
                    text = Game1.currentSeason;
                }

                try
                {
                    scriptParts = npc.getMasterScheduleRawData()[text].Split('/');
                }
                catch (Exception)
                {
                    return npc.parseMasterSchedule(npc.getMasterScheduleEntry("spring"));
                }
            }

            if (scriptParts[0].Contains("NOT"))
            {
                string[] array2 = scriptParts[0].Split(' ');
                if (array2[1].ToLower() == "friendship")
                {
                    int i = 2;
                    bool flag = false;
                    for (; i < array2.Length; i += 2)
                    {
                        string text2 = array2[i];
                        int result = 0;
                        if (int.TryParse(array2[i + 1], out result))
                        {
                            foreach (Farmer allFarmer in Game1.getAllFarmers())
                            {
                                if (allFarmer.getFriendshipHeartLevelForNPC(text2) >= result)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }

                        if (flag)
                        {
                            break;
                        }
                    }

                    if (flag)
                    {
                        return npc.parseMasterSchedule(npc.getMasterScheduleEntry("spring"));
                    }

                    num++;
                }
            }
            else if (scriptParts[0].Contains("MAIL"))
            {
                string item = scriptParts[0].Split(' ')[1];
                num = !Game1.MasterPlayer.mailReceived.Contains(item) &&
                      !NetWorldState.checkAnywhereForWorldStateID(item)
                    ? num + 1
                    : num + 2;
            }

            if (scriptParts[num].Contains("GOTO"))
            {
                string text3 = scriptParts[num].Split(' ')[1];
                if (text3.ToLower().Equals("season"))
                {
                    text3 = Game1.currentSeason;
                }
                else if (text3.ToLower().Equals("no_schedule"))
                {
                    npc.followSchedule = false;
                    return null;
                }

                return npc.parseMasterSchedule(npc.getMasterScheduleEntry(text3));
            }

            Point startPoint = npc.isMarried()
                ? new Point(0, 23)
                : new Point((int) defaultPosition.GetValue().X / 64, (int) defaultPosition.GetValue().Y / 64);
            string startLoc = npc.isMarried() ? "BusStop" : npc.DefaultMap;
            int time = 610;
            string text5 = npc.DefaultMap;
            int num2 = (int) (defaultPosition.GetValue().X / 64f);
            int num3 = (int) (defaultPosition.GetValue().Y / 64f);
            bool flag2 = false;
            for (var j = num; j < scriptParts.Length; j++)
            {
                if (scriptParts.Length <= 1)
                {
                    break;
                }

                // MarketDay.Log($"{j} {scriptParts[j]}", LogLevel.Debug);

                var num4 = 0;
                var scriptWords = scriptParts[j].Split(' ');
                var timeIsArrival = false;
                var stepTimeStr = scriptWords[num4];
                if (stepTimeStr.Length > 0 && scriptWords[num4][0] == 'a')
                {
                    timeIsArrival = true;
                    stepTimeStr = stepTimeStr[1..];
                }

                var stepTime = Convert.ToInt32(stepTimeStr);
                num4++;
                var endLocationName = scriptWords[num4];
                string endBehavior = null;
                string endMessage = null;
                var endingX = 0;
                var endingY = 0;
                var faceDirection = 2;
                if (endLocationName == "bed")
                {
                    if (npc.isMarried())
                    {
                        endLocationName = "BusStop";
                        endingX = -1;
                        endingY = 23;
                        faceDirection = 3;
                    }
                    else
                    {
                        string text7 = null;
                        if (npc.hasMasterScheduleEntry("default"))
                        {
                            text7 = npc.getMasterScheduleEntry("default");
                        }
                        else if (npc.hasMasterScheduleEntry("spring"))
                        {
                            text7 = npc.getMasterScheduleEntry("spring");
                        }

                        if (text7 != null)
                        {
                            try
                            {
                                string[] array4 = text7.Split('/')[^1].Split(' ');
                                endLocationName = array4[1];
                                if (array4.Length > 3)
                                {
                                    if (!int.TryParse(array4[2], out endingX) || !int.TryParse(array4[3], out endingY))
                                    {
                                        text7 = null;
                                    }
                                }
                                else
                                {
                                    text7 = null;
                                }
                            }
                            catch (Exception)
                            {
                                text7 = null;
                            }
                        }

                        if (text7 == null)
                        {
                            endLocationName = text5;
                            endingX = num2;
                            endingY = num3;
                        }
                    }

                    num4++;
                    Dictionary<string, string> dictionary2 =
                        Game1.content.Load<Dictionary<string, string>>("Data\\animationDescriptions");
                    string text8 = npc.Name.ToLower() + "_sleep";
                    if (dictionary2.ContainsKey(text8))
                    {
                        endBehavior = text8;
                    }
                }
                else
                {
                    if (int.TryParse(endLocationName, out var _))
                    {
                        endLocationName = startLoc;
                        num4--;
                    }

                    num4++;
                    endingX = Convert.ToInt32(scriptWords[num4]);
                    num4++;
                    endingY = Convert.ToInt32(scriptWords[num4]);
                    num4++;
                    try
                    {
                        if (scriptWords.Length > num4)
                        {
                            if (int.TryParse(scriptWords[num4], out faceDirection))
                            {
                                num4++;
                            }
                            else
                            {
                                faceDirection = 2;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        faceDirection = 2;
                    }
                }

                // look ahead one schedule step to see how much slack time we might have
                var nextStepTime = 2600;
                var nextTimeIsArrival = false;

                var minutesAvailable = 0;
                if (j < scriptParts.Length - 1)
                {
                    var lookahead = scriptParts[j + 1].Split(' ');
                    var nextStepTimeStr = lookahead[0];

                    if (nextStepTimeStr.Length > 0 && nextStepTimeStr[0] == 'a')
                    {
                        nextStepTimeStr = nextStepTimeStr[1..];
                        nextTimeIsArrival = true;
                    }

                    nextStepTime = Convert.ToInt32(nextStepTimeStr);
                    minutesAvailable =
                        Math.Max(0, StardewValley.Utility.ConvertTimeToMinutes(nextStepTime) -
                                    StardewValley.Utility.ConvertTimeToMinutes(stepTime));
                }

                var arr = timeIsArrival ? "a" : "";
                var narr = nextTimeIsArrival ? "a" : "";
                // MarketDay.Log($"parseMasterSchedule:     [{j}] {npc.Name} {arr}{stepTime} {startLoc} -> {endLocationName} (next step {narr}{nextStepTime})", LogLevel.Debug);

                if (ChangeScheduleForLocationAccessibility(npc, ref endLocationName, ref endingX, ref endingY,
                    ref faceDirection))
                {
                    if (npc.getMasterScheduleRawData().ContainsKey("default"))
                    {
                        return npc.parseMasterSchedule(npc.getMasterScheduleEntry("default"));
                    }

                    return npc.parseMasterSchedule(npc.getMasterScheduleEntry("spring"));
                }

                if (num4 < scriptWords.Length)
                {
                    if (scriptWords[num4].Length > 0 && scriptWords[num4][0] == '"')
                    {
                        endMessage = scriptParts[j].Substring(scriptParts[j].IndexOf('"'));
                    }
                    else
                    {
                        endBehavior = scriptWords[num4];
                        num4++;
                        if (num4 < scriptWords.Length && scriptWords[num4].Length > 0 && scriptWords[num4][0] == '"')
                        {
                            endMessage = scriptParts[j].Substring(scriptParts[j].IndexOf('"')).Replace("\"", "");
                        }
                    }
                }

                if (stepTime == 0)
                {
                    flag2 = true;
                    text5 = endLocationName;
                    num2 = endingX;
                    num3 = endingY;
                    startLoc = endLocationName;
                    startPoint.X = endingX;
                    startPoint.Y = endingY;
                    previousEndPoint.SetValue(new Point(endingX, endingY));
                    continue;
                }

                // here we add to the master schedule
                // which is an opportunity to add another stop for owners or spouses
                // if the location route takes them via the Town

                var shop = RouteViaOwnedShop(npc, startLoc, endLocationName, getLocationRoute, time, nextStepTime,
                    minutesAvailable, startPoint, endBehavior, endMessage, endingX, endingY, faceDirection,
                    masterSchedule, ref stepTime);

                if (shop is null)
                {
                    var schedulePathDescription = PathfindToNextScheduleLocation(npc, stepTime,
                        nextStepTime, true, startLoc, startPoint.X, startPoint.Y, 
                        endLocationName, endingX, endingY,
                        faceDirection, endBehavior, endMessage);
                    if (timeIsArrival)
                    {
                        var oldStepTime = stepTime;
                        var minutes = Minutes(schedulePathDescription);
                        stepTime = Math.Max(
                            StardewValley.Utility.ConvertMinutesToTime(
                                StardewValley.Utility.ConvertTimeToMinutes(stepTime) - minutes), time);
                    }

                    if (stepTime == 0) MarketDay.Log($"stepTime is 0, watch for problems", LogLevel.Warn);
                    if (masterSchedule.ContainsKey(stepTime))
                    {
                        MarketDay.Log($"stepTime {stepTime} already in schedule, watch for problems", LogLevel.Warn);
                        MarketDay.Log($"    {startLoc} -> {endLocationName}", LogLevel.Warn);
                        stepTime += 10;
                    }

                    // MarketDay.Log($"parseMasterSchedule:         {npc.Name} {stepTime} {startLoc} {startPoint} -> {endLocationName} {endingX} {endingY}  lunch: {goingToLunch}", LogLevel.Debug);
                    masterSchedule.Add(stepTime, schedulePathDescription);
                }

                startPoint.X = endingX;
                startPoint.Y = endingY;
                startLoc = endLocationName;
                time = stepTime;
            }

            if (Game1.IsMasterGame && flag2)
            {
                Game1.warpCharacter(npc, text5, new Point(num2, num3));
            }

            if (_lastLoadedScheduleKey.GetValue() != null && Game1.IsMasterGame)
            {
                npc.dayScheduleName.Value = _lastLoadedScheduleKey.GetValue();
            }

            return masterSchedule;
        }

        private static GrangeShop RouteViaOwnedShop(NPC npc, string startLoc, string endLocationName,
            IReflectedMethod getLocationRoute, int time, int nextStepTime, int minutesAvailable, Point startPoint,
            string endBehavior, string endMessage, int endingX, int endingY, int faceDirection,
            IDictionary<int, SchedulePathDescription> masterSchedule,
            ref int stepTime)
        {
            if (startLoc is null || endLocationName is null) return null;

            // sorry, Krobus is not permitted in town
            if (npc.Name == "Krobus") return null;

            var viaLocations = getLocationRoute.Invoke<List<string>>(startLoc, endLocationName);
            if (viaLocations is null || !viaLocations.Contains("Town")) return null;

            // see if they own a shop or their spouse owns a shop
            GrangeShop shop;
            var spouseName = SpouseName(npc);
            if (spouseName is not null) MapUtility.PlayerShopOwners.TryGetValue(spouseName, out shop);
            MapUtility.ShopOwners.TryGetValue(npc.Name, out shop);
            if (shop is null) return null;

            var startToTown = PathfindToNextScheduleLocation(npc, stepTime, nextStepTime, false, startLoc,
                startPoint.X, startPoint.Y, "Town", (int) shop.OwnerTile.X, (int) shop.OwnerTile.Y, 2, null,
                null);
            var arriveInTownAt =
                StardewValley.Utility.ConvertMinutesToTime(StardewValley.Utility.ConvertTimeToMinutes(stepTime) +
                                                           Minutes(startToTown));
            var townToEnd = PathfindToNextScheduleLocation(npc, arriveInTownAt, nextStepTime, true, "Town",
                (int) shop.OwnerTile.X, (int) shop.OwnerTile.Y, endLocationName, endingX, endingY, faceDirection,
                endBehavior, endMessage);

            var leaveTownBy =
                StardewValley.Utility.ConvertMinutesToTime(
                    StardewValley.Utility.ConvertTimeToMinutes(nextStepTime) - Minutes(townToEnd));
            if (leaveTownBy > MarketDay.Config.ClosingTime * 100)
            {
                // they don't need to leave the market before it closes
                // so have them stay until the end and go straight to next scheduled destination
                townToEnd = PathfindToNextScheduleLocation(npc, arriveInTownAt, nextStepTime, false, "Town",
                    (int) shop.OwnerTile.X, (int) shop.OwnerTile.Y, endLocationName, endingX, endingY,
                    faceDirection, endBehavior, endMessage);
                leaveTownBy = MarketDay.Config.ClosingTime * 100;
            }

            if (arriveInTownAt > MarketDay.Config.ClosingTime * 100)
            {
                // MarketDay.Log(
                // 	$"no time for that... would arrive at {arriveInTownAt} which is after closing {MarketDay.Config.ClosingTime * 100}",
                // 	LogLevel.Info);
                shop = null;
            }
            else if (arriveInTownAt >= leaveTownBy)
            {
                // MarketDay.Log(
                // 	$"no time for that... would arrive at {arriveInTownAt} but we have to leave at {leaveTownBy}",
                // 	LogLevel.Info);
                shop = null;
            }
            else
            {
                // MarketDay.Log($"We can do this... arrive at {arriveInTownAt} leave at {leaveTownBy}",
                // 	LogLevel.Info);
                AddWorkingAt(npc, arriveInTownAt, shop, leaveTownBy);
                masterSchedule.Add(stepTime, startToTown);
                masterSchedule.Add(leaveTownBy, townToEnd);
                stepTime = leaveTownBy;
            }

            return shop;
        }

        private static string SpouseName(NPC npc)
        {
            var f = npc.getSpouse();
            var s = f?.getSpouse();
            if (s is null) return null;
            return s != npc ? null : f.Name;
        }

        private static void AddWorkingAt(NPC npc, int stepTime, GrangeShop shop, int leaveTownBy)
        {
            if (!NPCInteractions.ContainsKey(stepTime)) NPCInteractions[stepTime] = new List<string>();
            NPCInteractions[stepTime].Add($"{npc.displayName} working at {shop.ShopName} until {leaveTownBy}");
        }

        /// <summary>
        /// Log an NPC visit to the market
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="stepTime"></param>
        /// <param name="leaveTownBy"></param>
        /// <param name="source"></param>
        private static void AddVisit(NPC npc, int stepTime, int leaveTownBy, string source)
        {
            if (!NPCInteractions.ContainsKey(stepTime)) NPCInteractions[stepTime] = new List<string>();
            NPCInteractions[stepTime]
                .Add($"{npc.displayName} visiting the market (until {leaveTownBy}, source: {source})");
        }

        private static IEnumerable<Point> Disjoints(IEnumerable<Point> route)
        {
            var points = route.ToList();
            var disjoints = new List<Point> { points.First() };
            Point? nextTile = null;
            foreach (var tile in points)
            {
                if (!nextTile.HasValue)
                {
                    nextTile = tile;
                    continue;
                }
                
                if (Math.Abs(nextTile.Value.X - tile.X) + Math.Abs(nextTile.Value.Y - tile.Y) > 3)
                {
                    disjoints.Add(nextTile.Value);
                    disjoints.Add(tile);
                }

                nextTile = tile;
            }

            disjoints.Add(points.Last());
            return disjoints;
        }


        private static double TravelTime(IEnumerable<Point> route)
        {
            var pixels = 0;
            Point? nextTile = null;
            foreach (var tile in route)
            {
                if (!nextTile.HasValue)
                {
                    nextTile = tile;
                    continue;
                }

                if (Math.Abs(nextTile.Value.X - tile.X) + Math.Abs(nextTile.Value.Y - tile.Y) == 1)
                {
                    pixels += 64;
                }

                nextTile = tile;
            }

            var minutes = pixels / 96.0;
            return minutes;
        }

        private static int Minutes(IEnumerable<Point> route)
        {
            var minutes = (int) Math.Round(TravelTime(route) / 10.0) * 10;
            return minutes;
        }

        private static int Minutes(SchedulePathDescription schedulePathDescription)
        {
            return Minutes(schedulePathDescription.route);
        }

        private static bool ChangeScheduleForLocationAccessibility(
            NPC npc,
            ref string locationName,
            ref int tileX,
            ref int tileY,
            ref int facingDirection)
        {
            string str = locationName;
            if (str != "JojaMart" && str != "Railroad")
            {
                if (str == "CommunityCenter") return !Game1.isLocationAccessible(locationName);
            }
            else if (!Game1.isLocationAccessible(locationName))
            {
                if (!npc.hasMasterScheduleEntry(locationName + "_Replacement"))
                    return true;
                string[] strArray = npc.getMasterScheduleEntry(locationName + "_Replacement").Split(' ');
                locationName = strArray[0];
                tileX = Convert.ToInt32(strArray[1]);
                tileY = Convert.ToInt32(strArray[2]);
                facingDirection = Convert.ToInt32(strArray[3]);
            }

            return false;
        }

        private static bool WithinTownieQuota(NPC npc)
        {
            // something's triggered a schedule refresh on a farmhand, just don't crash
            if (!Context.IsMainPlayer) return true;
            
            // random visitors always allowed
            if (npc.Name.StartsWith("RNPC")) return true;

            // once you're in you're in
            if (TownieVisitorsToday.Contains(npc)) return true;
            
            // do we have room in the Townie quotient for this visit?
            if (TownieVisitorsToday.Count >= MarketDay.Progression.NumberOfTownieVisitors) return false;
            
            TownieVisitorsToday.Add(npc);
            return true;
        }    
        
        private static SchedulePathDescription PathfindToNextScheduleLocation(NPC npc, int stepTime, int nextStepTime,
            bool visitShops, string startingLocation, int startingX, int startingY, string endingLocation, int endingX,
            int endingY, int finalFacingDirection, string endBehavior, string endMessage)
        {
            var stack = new Stack<Point>();
            var startPoint = new Point(startingX, startingY);

            // MarketDay.Log($"pfNSL: {npc.Name} {startingLocation} {stepTime} -> {endingLocation} {nextStepTime}  visit: {visitShops}", LogLevel.Debug);

            //     private List<string> getLocationRoute(string startingLocation, string endingLocation)
            var getLocationRoute = MarketDay.helper.Reflection.GetMethod(npc, "getLocationRoute");

            //     private Stack<Point> addToStackForSchedule(Stack<Point> original, Stack<Point> toAdd)
            var addToStackForSchedule = MarketDay.helper.Reflection.GetMethod(npc, "addToStackForSchedule");

            var routeViaLocations = !startingLocation.Equals(endingLocation, StringComparison.Ordinal)
                ? getLocationRoute.Invoke<List<string>>(startingLocation, endingLocation)
                : null;
            if (routeViaLocations != null)
            {
                for (var i = 0; i < routeViaLocations.Count; i++)
                {
                    var locationFromName = Game1.getLocationFromName(routeViaLocations[i]);
                    if (locationFromName.Name.Equals("Trailer") &&
                        Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
                    {
                        locationFromName = Game1.getLocationFromName("Trailer_Big");
                    }

                    var divert = visitShops
                                 && locationFromName.Name == "Town"
                                 && stepTime < MarketDay.Config.ClosingTime * 100
                                 && nextStepTime > MarketDay.Config.OpeningTime * 100
                                 && WithinTownieQuota(npc);

                    if (i < routeViaLocations.Count - 1)
                    {
                        var warpPointTo = locationFromName.getWarpPointTo(routeViaLocations[i + 1]);
                        if (warpPointTo.Equals(Point.Zero) || startPoint.Equals(Point.Zero))
                        {
                            throw new Exception("schedule pathing tried to find a warp point that doesn't exist.");
                        }

                        var maxDuration = StardewValley.Utility.ConvertTimeToMinutes(nextStepTime) -
                                          StardewValley.Utility.ConvertTimeToMinutes(stepTime);

                        var newPoints = divert
                            ? PathFindViaGrangeShops(startPoint, warpPointTo, locationFromName, 30000, maxDuration)
                            : FindPathForNpcSchedules(startPoint, warpPointTo, locationFromName, 30000);

                        if (divert) AddVisit(npc, stepTime, nextStepTime, "pfNSL via Town");

                        stack = addToStackForSchedule.Invoke<Stack<Point>>(stack, newPoints);
                        startPoint = locationFromName.getWarpPointTarget(warpPointTo, npc);
                    }
                    else
                    {
                        stack = addToStackForSchedule.Invoke<Stack<Point>>(stack,
                            PathFindController.findPathForNPCSchedules(startPoint, new Point(endingX, endingY),
                                locationFromName, 30000));
                    }
                }
            }
            else if (startingLocation.Equals(endingLocation, StringComparison.Ordinal))
            {
                var locationFromName2 = Game1.getLocationFromName(startingLocation);
                if (locationFromName2.Name.Equals("Trailer") &&
                    Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
                {
                    locationFromName2 = Game1.getLocationFromName("Trailer_Big");
                }

                var divert = visitShops
                             && locationFromName2.Name == "Town"
                             && stepTime < MarketDay.Config.ClosingTime * 100
                             && nextStepTime > MarketDay.Config.OpeningTime * 100
                             && WithinTownieQuota(npc);
                
                // MarketDay.Log($"pathfindToNextScheduleLocation: {npc.Name} {stepTime} {nextStepTime} - {locationFromName2.Name}", LogLevel.Debug);
                if (divert) {
                    var maxDuration = 
                        StardewValley.Utility.ConvertTimeToMinutes(nextStepTime) -
                        StardewValley.Utility.ConvertTimeToMinutes(stepTime);
                    
                    // MarketDay.Log($"    Diverting {npc.Name} via the market (B)", LogLevel.Trace);
                    stack = PathFindViaGrangeShops(startPoint, new Point(endingX, endingY), locationFromName2, 30000, maxDuration);
                    AddVisit(npc, stepTime, nextStepTime, "pfNSL within Town");
                } else {
                    stack = FindPathForNpcSchedules(startPoint, new Point(endingX, endingY), locationFromName2, 30000);
                }
            }

            return new SchedulePathDescription(stack, finalFacingDirection, endBehavior, endMessage);
        }

        private static List<string> GetLocationRoute(NPC npc, string startingLocation, string endingLocation)
        {
            // 			routesFromLocationToLocation = new List<List<string>>();

            var routesFromLocationToLocation = MarketDay.helper.Reflection
                .GetField<List<List<string>>>(npc, "routesFromLocationToLocation").GetValue();

            foreach (var item in routesFromLocationToLocation)
            {
                if (item.First().Equals(startingLocation, StringComparison.Ordinal) &&
                    item.Last().Equals(endingLocation, StringComparison.Ordinal) &&
                    (npc.Gender == 0 || !item.Contains("BathHouse_MensLocker", StringComparer.Ordinal)) &&
                    (npc.Gender != 0 || !item.Contains("BathHouse_WomensLocker", StringComparer.Ordinal)))
                {
                    return item;
                }
            }

            return null;
        }

        public static Stack<Point> PathFindViaGrangeShops(Point startPoint, Point endPoint, GameLocation location,
            int limit, int maxDuration)
        {
            const int STOP_DURATION_MINS = 5;

            Debug.Assert(location is Town, "don't pathfind via shops if it's not the town");

            MarketDay.Log(
                $"pathFindViaGrangeShops {location.Name} {startPoint} -> {endPoint} max duration {maxDuration}",
                LogLevel.Trace, false);

            Stack<Point> path = new Stack<Point>();
            double duration = 0;

            var placesToVisit = PlacesToVisit(startPoint);
            if (placesToVisit.Count < 2) return path;

            var waypoints = string.Join(", ", placesToVisit);
            MarketDay.Log($"    Waypoints: {waypoints}", LogLevel.Trace, false);

            // work backwards through the waypoints
            placesToVisit.Reverse();

            var thisEndPoint = endPoint;
            var i = 0;
            foreach (var (wptX, wptY) in placesToVisit)
            {
                i++;
                var thisStartPoint = new Point(wptX, wptY);
                var originalPath = FindPathForNpcSchedules(thisStartPoint, thisEndPoint, location, limit);
                if (originalPath is null || originalPath.Count == 0) continue;

                duration += TravelTime(originalPath) + STOP_DURATION_MINS;

                MarketDay.Log(
                    $"    wpt {i} of {placesToVisit.Count}  seg len {originalPath.Count} mins {TravelTime(originalPath):0.0}  {duration:0.0} {maxDuration}  {wptX} {wptY}",
                    LogLevel.Trace);

                if (duration > maxDuration)
                    originalPath = FindPathForNpcSchedules(startPoint, thisEndPoint, location, limit);

                var legPath = originalPath.ToList();
                legPath.Reverse();
                foreach (var pt in legPath) path.Push(pt);
                thisEndPoint = thisStartPoint;

                if (duration > maxDuration) break;
            }

            return path;
        }

        /// <summary>
        /// Compare two points based on their distance to a third point
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        private static Comparison<Point> DistanceToPoint(Point start)
        {
            int Comparator(Point p1, Point p2)
            {
                var (startX, startY) = start;
                var d1 = Math.Pow(startX - p1.X, 2) + Math.Pow(startY - p1.Y, 2);
                var d2 = Math.Pow(startX - p2.X, 2) + Math.Pow(startY - p2.Y, 2);
                return (int) (d1 - d2);
            }

            return Comparator;
        }

        /// <summary>
        /// List of shops for NPCs to stand in front of, in order of distance from start point.
        /// Totally unoptimized solution to a travelling salesperson situation,
        /// but who expects NPCs to act in a globally-efficient way?
        /// </summary>
        /// <param name="startPoint"></param>
        /// <returns></returns>
        private static List<Point> PlacesToVisit(Point startPoint)
        {
            var placesToVisit = new List<Point> {startPoint};

            var available = MapUtility.ShopTiles.Keys.Select(shopLoc => shopLoc.ToPoint()).ToList();

            while (available.Count > 0)
            {
                var current = placesToVisit[^1];

                // sort shops according to distance from current location
                available.Sort(DistanceToPoint(current));

                // we could choose a random shop amongst the closest 2 or 3 in the list
                // but you know what? the visitPoint randomizing probably introduces enough fuzz
                // and we can just pick whatever is closest
                var next = available.First();
                available.Remove(next);


                if (Game1.random.NextDouble() < MarketDay.Config.StallVisitChance)
                {
                    var visitPoint = new Point(next.X + Game1.random.Next(2), next.Y + 4);
                    placesToVisit.Add(visitPoint);
                    placesToVisit.Add(visitPoint + new Point(1, 0));
                }
            }


            return placesToVisit;
        }

        /// <summary>
        /// Generate a complete schedule for NPCs without a schedule this day
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="dayOfMonth"></param>
        /// <returns></returns>
        public static Dictionary<int, SchedulePathDescription> getScheduleWhenNoDefault(NPC npc, int dayOfMonth)
        {
            string there;
            string back;

            // do they have a shop of their own?
            if (MapUtility.NPCShopOwners.TryGetValue(npc.Name, out var shop))
            {
                // MarketDay.Log($"getScheduleWhenNoDefault: {npc.Name} has shop {shop.ShopName}", LogLevel.Debug);
                there = $"a{MarketDay.Config.OpeningTime * 100} Town {shop.OwnerTile.X} {shop.OwnerTile.Y} 2";
                back = $"{MarketDay.Config.ClosingTime * 100} BusStop -1 23 3";
                return parseMasterSchedule(npc, $"{there}/{back}");
            }

            // are they married to a player?
            var spouseName = SpouseName(npc);
            if (spouseName is null) return new Dictionary<int, SchedulePathDescription>();
            if (!MapUtility.PlayerShopOwners.TryGetValue(spouseName, out var spouseShop))
                return new Dictionary<int, SchedulePathDescription>();
            // MarketDay.Log($"getScheduleWhenNoDefault: {npc.Name} married to owner of shop {spouseShop.ShopName}", LogLevel.Trace);
            there = $"a{MarketDay.Config.OpeningTime * 100} Town {spouseShop.OwnerTile.X} {spouseShop.OwnerTile.Y} 2";
            back = $"{MarketDay.Config.ClosingTime * 100} BusStop -1 23 3";
            return parseMasterSchedule(npc, $"{there}/{back}");
        }

        private static string SaloonTile()
        {
            var x = Game1.random.Next(8, 19);
            return $"Saloon {x} 20 0";
        }

        private static string TownTile()
        {
            var choices = new List<string>
            {
                "Town 39 16 3", // mini-garden
                "Town 40 16 0",
                "Town 40 17 1",
                "Town 39 17 3",
                "Town 32 55 2", // tree near harvey's
                "Town 41 57 0", // pierre notices
                "Town 42 57 0",
                "Town 62 16 2", // shrine-seat
                "Town 63 16 2",
                "Town 64 16 2",
                "Town 40 19 1",
                "Town 17 57 1", // bench left of market
                "Town 17 58 1",
                "Saloon 33 18 0", // arcade
                "Saloon 34 18 0",
                "Saloon 35 18 0",
                "Saloon 37 18 0", // joja machine
                "Saloon 38 18 0",
                "Saloon 39 22 2", // tv
            };
            
            // fountain 
            for (var i = 24; i < 30; i++)
            {
                choices.Add($"Town {i} 29 0");
                choices.Add($"Town 23 {i} 1");
                choices.Add($"Town {i} 24 2");
                choices.Add($"Town 29 {i} 3");
            }
            
            // benches south of Saloon 
            for (var i = 42; i < 46; i++)
            {
                choices.Add($"Town {i} 78 2");
                choices.Add($"Town {i} 80 0");
            }

            // snooker
            for (var i = 47; i < 42; i++)
            {
                choices.Add($"Saloon {i} 19 2");
                choices.Add($"Saloon {i} 22 0");
            }

            // riverbank 
            for (var i = 15; i < 24; i++)
            {
                choices.Add($"Town {i} 94 2");
                choices.Add($"Town {i} 95 2");
                choices.Add($"Town {i} 96 2");
            }

            //  grass above sewer
            for (var i = 31; i < 39; i++)
            {
                choices.Add($"Town {i} 90 2");
                choices.Add($"Town {i} 91 0");
            }

            // dirt patches left of market
            for (var i = 9; i < 14; i++)
            {
                choices.Add($"Town {i} 60 2");
                choices.Add($"Town {i} 62 0");
                choices.Add($"Town {i} 68 2");
                choices.Add($"Town {i} 70 0");
            }
            
            return choices[Game1.random.Next(choices.Count)];
        }

        private static Vector2 RandomShopTile()
        {
            var r = Game1.random.Next(MapUtility.ShopTiles.Keys.Count);
            var tile = MapUtility.ShopTiles.Keys.ToList()[r];
            var x = Game1.random.Next(1, 5);
            return tile + new Vector2(x, 4);
        }
        
        public static string ScheduleStringForMarketVisit(NPC npc, string currentSchedule)
        {
            if (currentSchedule is null || currentSchedule.Length == 0) return currentSchedule;

            var newScheduleParts = new List<string>();
            
            var lunchSpot = SaloonTile();

            var (shopX, shopY) = MapUtility.ShopTiles.Keys.First() + new Vector2(2, 4);

            MarketDay.Log($"sSFMV: currentSchedule {currentSchedule}", LogLevel.Trace);
            var scheduleParts = currentSchedule.Split("/");
            if (scheduleParts.Length < 2) return currentSchedule;

            var bedSchedulePart = scheduleParts[^1];
            bedSchedulePart = string.Join(" ", bedSchedulePart.Split(" ")[1..]);

            var hourOffset = Game1.random.Next(5) * 10;
            for (var hour = MarketDay.Config.OpeningTime; hour <= MarketDay.Config.ClosingTime; hour++)
            {
                if (hour == MarketDay.Config.OpeningTime) {
                    var tile = RandomShopTile();
                    newScheduleParts.Add($"{hour * 100 + hourOffset} Town {tile.X} {tile.Y} 0");
                } else if (hour == MarketDay.Config.ClosingTime) {
                    newScheduleParts.Add($"{hour * 100} {bedSchedulePart}");
                } else if (hour == 12) {
                    newScheduleParts.Add($"a{hour * 100 + hourOffset} {lunchSpot}");
                } else if (hour % 2 == 0) {
                    newScheduleParts.Add($"{hour * 100 + hourOffset} {TownTile()}");
                } 
            }

            var schedule = string.Join("/", newScheduleParts);
            MarketDay.Log($"sSFMV: {npc.Name} schedule: {schedule}", LogLevel.Trace);
            return schedule;
        }

        public static bool CanVisitIslandToday(NPC npc)
        {
            if (!npc.isVillager() || !npc.CanSocialize || npc.daysUntilNotInvisible > 0 || npc.IsInvisible ||
                npc.Name is "Pam" or "Emily" && Game1.dayOfMonth == 15 && Game1.currentSeason == "fall")
                return false;
            var str = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
            return npc.currentLocation is not {NameOrUniqueName: "Farm"} &&
                   (str != "Tue" && str != "Fri" && str != "Wed" ||
                    npc.Name != "Vincent" && npc.Name != "Jas" && npc.Name != "Penny") &&
                   (str != "Tue" && str != "Thu" || npc.Name != "Harvey" && npc.Name != "Maru") &&
                   !StardewValley.Utility.IsHospitalVisitDay(npc.Name) &&
                   (npc.Name != "Clint" || str == "Fri") &&
                   (npc.Name != "Robin" || str == "Tue") &&
                   (npc.Name != "Marnie" || str is "Tue" or "Mon") &&
                   npc.Name != "Sandy" && npc.Name != "Dwarf" && npc.Name != "Krobus" && npc.Name != "Wizard" &&
                   npc.Name != "Linus" && npc.Name != "Willy" && npc.Name != "Evelyn" && npc.Name != "George";
        }

        public static bool ExcludedFromIslandEvents(NPC npc)
        {
            var exclusions = MarketDay.helper.GameContent.Load<Dictionary<string, string>>(@"Data/CustomNPCExclusions");
            if (exclusions.TryGetValue(npc.Name, out var exclusion))
            {
                return exclusion.Contains("IslandVisit") || exclusion.Contains("IslandEvent");
            }
            return false;
        }

        internal static void PrintSchedule(NPC npc)
        {
            Dictionary<Point, string> warps = new();
            foreach (var location in Game1.locations)
            {
                foreach (var warp in location.warps)
                {
                    var v = new Point(warp.X, warp.Y);
                    if (! warps.ContainsKey(v)) warps[v] = location.Name;
                    else if (! warps[v].Contains(location.Name)) warps[v] = warps[v] + "/" + location.Name;
                    
                    var w = new Point(warp.TargetX, warp.TargetY);
                    if (! warps.ContainsKey(w)) warps[w] = warp.TargetName;
                    else if (! warps[w].Contains(warp.TargetName)) warps[w] = warps[w] + "/" + warp.TargetName;
                }
            }

            MarketDay.Log($"Schedule for {npc.Name}:", LogLevel.Debug);
            foreach (var kvp in npc.Schedule)
            {
                var mins = TravelTime(kvp.Value.route);
                var steps = Disjoints(kvp.Value.route);

                var stepsStrs = steps.Select(step => warps.ContainsKey(step) ? $"{warps[step]} {step.X} {step.Y}" : $"{step.X} {step.Y}").ToList();

                var stepsStr = string.Join(", ", stepsStrs);

                MarketDay.Log($"    {kvp.Key}: {stepsStr}: {mins:0.} minutes", LogLevel.Debug);
            }
        }

        /// <summary>
        /// Should we ignore this schedule and wait for the parser to resolve a GOTO or other branch
        /// </summary>
        /// <param name="npc">NPC</param>
        /// <param name="rawData">Raw schedule data</param>
        /// <returns>ignore this one</returns>
        public static bool IgnoreThisSchedule(NPC npc, string rawData)
        {
            // MarketDay.Log($"IgnoreThisSchedule: examining {rawData} for {npc}", LogLevel.Debug);
            var scriptParts = rawData.Split('/');
            var num = 0;
            if (scriptParts[0].Contains("GOTO")) return true;
            if (scriptParts[0].Contains("NOT"))
            {
                var array2 = scriptParts[0].Split(' ');
                if (array2[1].ToLower() != "friendship") return false;
                var i = 2;
                for (; i < array2.Length; i += 2)
                {
                    var text2 = array2[i];
                    if (!int.TryParse(array2[i + 1], out var result)) continue;
                    if (Game1.getAllFarmers().Any(allFarmer => allFarmer.getFriendshipHeartLevelForNPC(text2) >= result))
                    {
                        return true;
                    }
                }

                num++;
            }
            else if (scriptParts[0].Contains("MAIL"))
            {
                var item = scriptParts[0].Split(' ')[1];
                num = !Game1.MasterPlayer.mailReceived.Contains(item) && !NetWorldState.checkAnywhereForWorldStateID(item) ? 1 : 2;
            }

            return scriptParts[num].Contains("GOTO");
        }
    }
}