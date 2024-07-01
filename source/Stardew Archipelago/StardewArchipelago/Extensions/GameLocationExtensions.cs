/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewArchipelago.GameModifications.EntranceRandomizer;
using StardewValley;
using StardewValley.Locations;
using xTile.ObjectModel;

namespace StardewArchipelago.Extensions
{
    public static class GameLocationExtensions
    {
        private static readonly Dictionary<WarpRequest, WarpRequest> ExtraWarps = new()
        {
            { new WarpRequest(Game1.getLocationRequest("Town"), 96, 51, FacingDirection.Down), new WarpRequest(Game1.getLocationRequest("AbandonedJojaMart"), 9, 13, FacingDirection.Up) },
            { new WarpRequest(Game1.getLocationRequest("Town"), 96, 51, FacingDirection.Down), new WarpRequest(Game1.getLocationRequest("MovieTheater"), 13, 15, FacingDirection.Up) },
            // {new WarpRequest(Game1.getLocationRequest("Town"), , , FacingDirection.Down), new WarpRequest(Game1.getLocationRequest("Trailer_Big"), , , FacingDirection.Up)},
            { new WarpRequest(Game1.getLocationRequest("Town"), 35, 97, FacingDirection.Down), new WarpRequest(Game1.getLocationRequest("Sewer"), 16, 11, FacingDirection.Down) },
            { new WarpRequest(Game1.getLocationRequest("Forest"), 94, 100, FacingDirection.Down), new WarpRequest(Game1.getLocationRequest("Sewer"), 3, 48, FacingDirection.Up) },
                        { new WarpRequest(Game1.getLocationRequest("Forest"), 101, 72, FacingDirection.Down), new WarpRequest(Game1.getLocationRequest("MasteryCave"), 7, 11, FacingDirection.Up) },
            // The warp for SVE's Forest to MasteryCave is changed from 101, 71 to 110, 81.  How do we resolve that one?
            { new WarpRequest(Game1.getLocationRequest("Mountain"), 16, 8, FacingDirection.Down), new WarpRequest(Game1.getLocationRequest("LeoTreeHouse"), 3, 8, FacingDirection.Up) },
            { new WarpRequest(Game1.getLocationRequest("BeachNightMarket"), 49, 11, FacingDirection.Down), new WarpRequest(Game1.getLocationRequest("ElliottHouse"), 3, 9, FacingDirection.Down) },
            { new WarpRequest(Game1.getLocationRequest("BeachNightMarket"), 30, 34, FacingDirection.Down), new WarpRequest(Game1.getLocationRequest("FishShop"), 5, 9, FacingDirection.Down) },
            { new WarpRequest(Game1.getLocationRequest("FishShop"), 4, 4, FacingDirection.Down), new WarpRequest(Game1.getLocationRequest("BoatTunnel"), 6, 12, FacingDirection.Up) },
            { new WarpRequest(Game1.getLocationRequest("IslandWest"), 20, 23, FacingDirection.Down), new WarpRequest(Game1.getLocationRequest("QiNutRoom"), 7, 8, FacingDirection.Up) },
            { new WarpRequest(Game1.getLocationRequest("WizardHouse"), 4, 5, FacingDirection.Down), new WarpRequest(Game1.getLocationRequest("WizardHouseBasement"), 4, 4, FacingDirection.Down) },
            { new WarpRequest(Game1.getLocationRequest("IslandWest"), 77, 40, FacingDirection.Down), new WarpRequest(Game1.getLocationRequest("IslandFarmhouse"), 14, 17, FacingDirection.Down) },
            { new WarpRequest(Game1.getLocationRequest("DeepWoods"), 20, 6, FacingDirection.Up), new WarpRequest(Game1.getLocationRequest("DeepWoodsMaxHouse"), 19, 24, FacingDirection.Up) },
            { new WarpRequest(Game1.getLocationRequest("Custom_SpriteSpring2"), 31, 11, FacingDirection.Up), new WarpRequest(Game1.getLocationRequest("Custom_SpriteSpringCave"), 10, 14, FacingDirection.Up) },
            { new WarpRequest(Game1.getLocationRequest("Custom_GrandpasShedRuins"), 15, 16, FacingDirection.Down), new WarpRequest(Game1.getLocationRequest("Custom_GrandpasShedOutside"), 22, 17, FacingDirection.Down) },
            { new WarpRequest(Game1.getLocationRequest("Custom_GrandpasShed"), 15, 16, FacingDirection.Down), new WarpRequest(Game1.getLocationRequest("Custom_GrandpasShedOutside"), 22, 17, FacingDirection.Down) },
            { new WarpRequest(Game1.getLocationRequest("Custom_GrandpasShedRuins"), 25, 14, FacingDirection.Up), new WarpRequest(Game1.getLocationRequest("Custom_GrandpasShedGreenhouse"), 30, 16, FacingDirection.Down) },
            { new WarpRequest(Game1.getLocationRequest("Backwoods"), 21, 1, FacingDirection.Up), new WarpRequest(Game1.getLocationRequest("Custom_EnchantedGrove"), 30, 32, FacingDirection.Up) },
            { new WarpRequest(Game1.getLocationRequest("Custom_EnchantedGrove"), 40, 10, FacingDirection.Down), new WarpRequest(Game1.getLocationRequest("Custom_GalmoranOutpost"), 39, 36, FacingDirection.Down) },
            { new WarpRequest(Game1.getLocationRequest("Custom_EnchantedGrove"), 40, 41, FacingDirection.Up), new WarpRequest(Game1.getLocationRequest("Custom_JunimoForest"), 31, 97, FacingDirection.Down) },
            { new WarpRequest(Game1.getLocationRequest("Custom_EnchantedGrove"), 20, 41, FacingDirection.Up), new WarpRequest(Game1.getLocationRequest("Custom_ApplesRoom"), 2, 9, FacingDirection.Down) },
            { new WarpRequest(Game1.getLocationRequest("Custom_EnchantedGrove"), 17, 25, FacingDirection.Up), new WarpRequest(Game1.getLocationRequest("Custom_WizardBasement"), 8, 18, FacingDirection.Down) },
            { new WarpRequest(Game1.getLocationRequest("Custom_EnchantedGrove"), 20, 10, FacingDirection.Up), new WarpRequest(Game1.getLocationRequest("Custom_SpriteSpring2"), 52, 20, FacingDirection.Down) },
            { new WarpRequest(Game1.getLocationRequest("Custom_EnchantedGrove"), 43, 25, FacingDirection.Up), new WarpRequest(Game1.getLocationRequest("Custom_AdventurerSummit"), 8, 24, FacingDirection.Down) },
            { new WarpRequest(Game1.getLocationRequest("Forest"), 19, 110, FacingDirection.Down), new WarpRequest(Game1.getLocationRequest("Custom_JunimoWoods"), 37, 2, FacingDirection.Down) },
        };

        private static readonly Dictionary<WarpRequest, WarpRequest> ExtraWarpsBothWays = ExtraWarps.Union(ExtraWarps.ToDictionary(x => x.Value, x => x.Key)).ToDictionary(x => x.Key, x => x.Value);

        private static IEnumerable<string> GetValidDestinationNames(string destinationName, EquivalentWarps equivalentAreas)
        {
            var destinationNames = new List<string> { destinationName };
            if (equivalentAreas == null)
            {
                return destinationNames;
            }

            var defaultDesiredWarpName = equivalentAreas.GetDefaultEquivalentEntrance(destinationName);
            if (!defaultDesiredWarpName.Equals(destinationName, StringComparison.InvariantCultureIgnoreCase))
            {
                destinationNames.Add(defaultDesiredWarpName);
            }

            return destinationNames;
        }



        public static List<Point> GetAllWarpPointsTo(this GameLocation origin, string destinationName, EquivalentWarps equivalentAreas = null)
        {
            var warpPoints = GetAllWarpPointsTo(origin, destinationName);
            if (warpPoints.Any())
            {
                return warpPoints;
            }

            var alternateDestinationNames = GetValidDestinationNames(destinationName, equivalentAreas);
            foreach (var alternateDestinationName in alternateDestinationNames)
            {
                warpPoints = GetAllWarpPointsTo(origin, alternateDestinationName);
                if (warpPoints.Any())
                {
                    return warpPoints;
                }
            }

            return warpPoints;
        }

        public static List<Point> GetAllWarpPointsTo(this GameLocation origin, string destinationName)
        {
            var warpPoints = new List<Point>();
            warpPoints.AddRange(GetSpecialTriggerWarps(origin, destinationName).Keys);
            warpPoints.AddRange(GetAllActionWarpsTo(origin, destinationName).Select(x => new Point(x.Key.X, x.Key.Y)));
            warpPoints.AddRange(GetAllTouchWarpsTo(origin, destinationName).Select(warp => new Point(warp.X, warp.Y)));
            warpPoints.AddRange(GetAllTouchActionWarpsTo(origin, destinationName).Select(x => new Point(x.Key.X, x.Key.Y)));
            warpPoints.AddRange(GetDoorWarpPoints(origin, destinationName));
            warpPoints.AddRange(GetBuildingWarps(origin, destinationName).Select(x => new Point(x.Key.X, x.Key.Y)));
            return warpPoints.Distinct().ToList();
        }

        public static Point GetWarpPointTarget(this GameLocation origin, Point warpPointLocation, string destinationName, EquivalentWarps equivalentAreas)
        {
            return GetWarpPointTarget(origin, warpPointLocation, GetValidDestinationNames(destinationName, equivalentAreas));
        }

        public static Point GetWarpPointTarget(this GameLocation origin, Point warpPointLocation, IEnumerable<string> validDestinationNames)
        {
            foreach (var destinationName in validDestinationNames)
            {
                foreach (var (warp, target) in GetAllActionWarpsTo(origin, destinationName))
                {
                    if (warp.X == warpPointLocation.X && warp.Y == warpPointLocation.Y)
                    {
                        return new Point(target.X, target.Y);
                    }
                }

                foreach (var warp in GetAllTouchWarpsTo(origin, destinationName))
                {
                    if (warp.X == warpPointLocation.X && warp.Y == warpPointLocation.Y)
                    {
                        return new Point(warp.TargetX, warp.TargetY);
                    }
                }

                foreach (var (warp, target) in GetAllTouchActionWarpsTo(origin, destinationName))
                {
                    if (warp.X == warpPointLocation.X && warp.Y == warpPointLocation.Y)
                    {
                        return new Point(target.X, target.Y);
                    }
                }

                if (TryGetDoorWarpPointTarget(origin, warpPointLocation, destinationName, out var warpPointTarget))
                {
                    return warpPointTarget;
                }

                foreach (var (warp, warpTarget) in GetSpecialTriggerWarps(origin, destinationName))
                {
                    if (warp.X == warpPointLocation.X && warp.Y == warpPointLocation.Y)
                    {
                        return new Point(warpTarget.X, warpTarget.Y);
                    }
                }

                foreach (var (warp, warpTarget) in GetBuildingWarps(origin, destinationName))
                {
                    if (warp.X == warpPointLocation.X && warp.Y == warpPointLocation.Y)
                    {
                        return new Point(warpTarget.X, warpTarget.Y);
                    }
                }
            }

            throw new Exception(
                $"Could not find Warp Point Target for '{origin.Name}' to '{validDestinationNames.First()}' at [{warpPointLocation.X}, {warpPointLocation.Y}]");
        }

        private static List<Warp> GetAllTouchWarpsTo(GameLocation origin, string destinationName)
        {
            var warps = new List<Warp>();
            foreach (var warp in origin.warps)
            {
                try
                {
                    if (warp.TargetName.Equals(destinationName, StringComparison.OrdinalIgnoreCase))
                    {
                        warps.Add(warp);
                    }
                    else if (warp.TargetName == "VolcanoEntrance" && destinationName == "VolcanoDungeon0")
                    {
                        var realTargetPoint = new Point(warp.TargetX, warp.TargetY).CheckSpecialVolcanoEdgeCaseWarp(destinationName);
                        warps.Add(new Warp(warp.X, warp.Y, warp.TargetName, realTargetPoint.X, realTargetPoint.Y, warp.flipFarmer.Value));
                    }
                }
                catch (Exception ex)
                {
                    var currentMethodName = $"{nameof(GameLocationExtensions)}.{nameof(GetAllTouchWarpsTo)}";
                    var currentMethodCall = $"{currentMethodName}({origin.Name}, {destinationName})";
                    var currentLoop = $"warp: {warp}";
                    var currentState = $"{currentMethodCall} => [{currentLoop}]";
                    var errorMessage = $"Failed in {currentState}:{Environment.NewLine}\t{ex}";
                    throw new Exception(errorMessage, ex);
                }
            }

            warps.AddRange(GetSpecialTouchWarps(origin));
            return warps;
        }

        public static Point CheckSpecialVolcanoEdgeCaseWarp(this Point targetPoint, string destinationName)
        {
            if (destinationName == "VolcanoDungeon0" && targetPoint.X == 1 && targetPoint.Y == 1)
            {
                return new Point(31, 53);
            }

            return targetPoint;
        }

        private static Dictionary<string, Dictionary<Point, Point>> _touchActionWarpCache = new();

        private static Dictionary<Point, Point> GetAllTouchActionWarpsTo(GameLocation origin, string destinationName)
        {
            var key = $"{origin.Name}->{destinationName}";
            if (_touchActionWarpCache.ContainsKey(key))
            {
                return _touchActionWarpCache[key];
            }

            var touchActionWarps = new Dictionary<Point, Point>();
            var map = origin.map;
            var backLayer = map?.GetLayer("Back");
            if (map == null || backLayer == null)
            {
                _touchActionWarpCache.Add(key, touchActionWarps);
                return touchActionWarps;
            }

            for (var y = 0; y < backLayer.LayerHeight; y++)
            {
                for (var x = 0; x < backLayer.LayerWidth; x++)
                {
                    try
                    {
                        var tile = backLayer.Tiles[x, y];
                        if (tile == null || (!tile.TileIndexProperties.TryGetValue("TouchAction", out var propertyValue) && !tile.Properties.TryGetValue("TouchAction", out propertyValue)))
                        {
                            continue;
                        }

                        if (TryGetWarpPointFromProperty(destinationName, propertyValue, out var warpPoint))
                        {
                            touchActionWarps.Add(new Point(x, y), warpPoint);
                        }
                    }
                    catch (Exception ex)
                    {
                        var currentMethodName = $"{nameof(GameLocationExtensions)}.{nameof(GetAllTouchActionWarpsTo)}";
                        var currentMethodCall = $"{currentMethodName}({origin.Name}, {destinationName})";
                        var currentLoop = $"x: {x}, y: {y}";
                        var currentState = $"{currentMethodCall} => [{currentLoop}]";
                        var errorMessage = $"Failed in {currentState}:{Environment.NewLine}\t{ex}";
                        throw new Exception(errorMessage, ex);
                    }
                }
            }

            _touchActionWarpCache.Add(key, touchActionWarps);
            return touchActionWarps;
        }

        private static Dictionary<string, Dictionary<Point, Point>> _actionWarpCache = new();

        private static Dictionary<Point, Point> GetAllActionWarpsTo(GameLocation origin, string destinationName)
        {
            var key = $"{origin.Name}->{destinationName}";
            if (_actionWarpCache.ContainsKey(key))
            {
                return _actionWarpCache[key];
            }

            var actionWarps = new Dictionary<Point, Point>();
            var map = origin.map;
            var buildingsLayer = map?.GetLayer("Buildings");
            if (map == null || buildingsLayer == null)
            {
                _actionWarpCache.Add(key, actionWarps);
                return actionWarps;
            }

            for (var y = 0; y < buildingsLayer.LayerHeight; y++)
            {
                for (var x = 0; x < buildingsLayer.LayerWidth; x++)
                {
                    try
                    {
                        var tile = buildingsLayer.Tiles[x, y];
                        if (tile == null || (!tile.TileIndexProperties.TryGetValue("Action", out var propertyValue) && !tile.Properties.TryGetValue("Action", out propertyValue)))
                        {
                            continue;
                        }

                        if (TryGetWarpPointFromProperty(destinationName, propertyValue, out var warpPoint))
                        {
                            actionWarps.Add(new Point(x, y), warpPoint);
                        }
                    }
                    catch (Exception ex)
                    {
                        var currentMethodName = $"{nameof(GameLocationExtensions)}.{nameof(GetAllActionWarpsTo)}";
                        var currentMethodCall = $"{currentMethodName}({origin.Name}, {destinationName})";
                        var currentLoop = $"x: {x}, y: {y}";
                        var currentState = $"{currentMethodCall} => [{currentLoop}]";
                        var errorMessage = $"Failed in {currentState}:{Environment.NewLine}\t{ex}";
                        throw new Exception(errorMessage, ex);
                    }
                }
            }

            _actionWarpCache.Add(key, actionWarps);
            return actionWarps;
        }

        private static bool TryGetWarpPointFromProperty(string destinationName, PropertyValue propertyValue, out Point warpPoint)
        {
            var propertyString = propertyValue.ToString();
            var touchActionParts = propertyString.Split(' ');
            var touchAction = touchActionParts[0];
            if (touchActionParts.Length < 4 || (!touchAction.Contains("Warp") && !touchAction.Contains("LoadMap")))
            {
                warpPoint = Point.Zero;
                return false;
            }

            var isCoordinatesFirst = int.TryParse(touchActionParts[1], out _);
            var xIndex = isCoordinatesFirst ? 1 : 2;
            var yIndex = isCoordinatesFirst ? 2 : 3;
            var destinationIndex = isCoordinatesFirst ? 3 : 1;
            var locationToWarp = touchActionParts[destinationIndex];
            if (!locationToWarp.Equals(destinationName, StringComparison.OrdinalIgnoreCase))
            {
                warpPoint = Point.Zero;
                return false;
            }

            var locationX = Convert.ToInt32(touchActionParts[xIndex]);
            var locationY = Convert.ToInt32(touchActionParts[yIndex]);
            warpPoint = new Point(locationX, locationY);
            return true;
        }

        private static IEnumerable<Warp> GetSpecialTouchWarps(GameLocation origin)
        {
            if (origin is FarmHouse farmhouse && farmhouse.cellarWarps != null)
            {
                foreach (var cellarWarp in farmhouse.cellarWarps)
                {
                    yield return cellarWarp;
                }
            }
        }

        private static IEnumerable<Point> GetDoorWarpPoints(GameLocation origin, string destinationName)
        {
            foreach (var pair in origin.doors.Pairs)
            {
                //try
                //{
                if (pair.Value.Equals(destinationName, StringComparison.OrdinalIgnoreCase))
                {
                    yield return pair.Key;
                }
                //}
                //catch (Exception ex)
                //{
                //    var currentMethodName = $"{nameof(GameLocationExtensions)}.{nameof(GetDoorWarpPoints)}";
                //    var currentMethodCall = $"{currentMethodName}({origin.Name}, {destinationName})";
                //    var currentLoop = $"pair: {pair}";
                //    var currentState = $"{currentMethodCall} => [{currentLoop}]";
                //    var errorMessage = $"Failed in {currentState}:{Environment.NewLine}\t{ex}";
                //    throw new Exception(errorMessage, ex);
                //}
            }
        }

        private static bool TryGetDoorWarpPointTarget(GameLocation origin, Point warpPointLocation, string targetDestinationName, out Point warpPointTarget)
        {
            foreach (var (warpPoint, destinationName) in origin.doors.Pairs)
            {
                if (!warpPoint.Equals(warpPointLocation) || !destinationName.Equals(targetDestinationName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var str = origin.doesTileHaveProperty(warpPointLocation.X, warpPointLocation.Y, "Action", "Buildings");
                if (str == null || !str.Contains("Warp"))
                {
                    continue;
                }

                var strArray = str.Split(' ');
                if (strArray[0].Equals("WarpCommunityCenter"))
                {
                    warpPointTarget = new Point(32, 23);
                    return true;
                }

                if (strArray[0].Equals("Warp_Sunroom_Door"))
                {
                    warpPointTarget = new Point(5, 13);
                    return true;
                }

                if (strArray.Length > 3 && strArray[3].Equals("BoatTunnel"))
                {
                    warpPointTarget = new Point(17, 43);
                    return true;
                }

                if (strArray.Length > 3 && strArray[3].Equals("Trailer") && Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
                {
                    warpPointTarget = new Point(13, 24);
                    return true;
                }

                if (strArray.Length < 3)
                {
                    continue;
                }

                warpPointTarget = new Point(Convert.ToInt32(strArray[1]), Convert.ToInt32(strArray[2]));
                return true;
            }

            warpPointTarget = Point.Zero;
            return false;
        }

        private static Dictionary<Point, Point> GetSpecialTriggerWarps(GameLocation origin, string destinationName)
        {
            var specialTriggerWarps = new Dictionary<Point, Point>();
            foreach (var (warp1, warp2) in ExtraWarpsBothWays)
            {
                try
                {
                    if (!warp1.LocationRequest.Name.Equals(origin.Name, StringComparison.OrdinalIgnoreCase) ||
                        !warp2.LocationRequest.Name.Equals(destinationName, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    specialTriggerWarps.Add(new Point(warp1.TileX, warp1.TileY), new Point(warp2.TileX, warp2.TileY));
                }
                catch (Exception ex)
                {
                    var currentMethodName = $"{nameof(GameLocationExtensions)}.{nameof(GetSpecialTriggerWarps)}";
                    var currentMethodCall = $"{currentMethodName}({origin.Name}, {destinationName})";
                    var currentLoop = $"warp1: {warp1}, warp2: {warp2}";
                    var currentState = $"{currentMethodCall} => [{currentLoop}]";
                    var errorMessage = $"Failed in {currentState}:{Environment.NewLine}\t{ex}";
                    throw new Exception(errorMessage, ex);
                }
            }

            return specialTriggerWarps;
        }

        private static Dictionary<Point, Point> GetBuildingWarps(GameLocation origin, string destinationName)
        {
            var buildingWarps = new Dictionary<Point, Point>();
            foreach (var building in origin.buildings)
            {
                try
                {
                    var interior = building.GetIndoors();
                    if (interior == null)
                    {
                        continue;
                    }
                    if (!interior.NameOrUniqueName.Equals(destinationName, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    foreach (var warp in interior.warps)
                    {
                        if (warp.TargetName == origin.NameOrUniqueName)
                        {
                            buildingWarps.Add(new Point(warp.TargetX, warp.TargetY), new Point(warp.X, warp.Y));
                        }
                    }
                }
                catch (Exception ex)
                {
                    var currentMethodName = $"{nameof(GameLocationExtensions)}.{nameof(GetBuildingWarps)}";
                    var currentMethodCall = $"{currentMethodName}({origin.Name}, {destinationName})";
                    var currentLoop = $"building: {building}";
                    var currentState = $"{currentMethodCall} => [{currentLoop}]";
                    var errorMessage = $"Failed in {currentState}:{Environment.NewLine}\t{ex}";
                    throw new Exception(errorMessage, ex);
                }
            }

            return buildingWarps;
        }

        public static Point GetClosestWarpPointTo(this GameLocation origin, string destinationName, Point currentLocation)
        {
            var allWarpPoints = origin.GetAllWarpPointsTo(destinationName);
            if (!allWarpPoints.Any())
            {
                return new Point(currentLocation.X, currentLocation.Y - 1);
            }

            return allWarpPoints.OrderBy(x => x.GetTotalDistance(currentLocation)).First();
        }

        public static bool TryGetClosestWarpPointTo(this string originName, ref string destinationName, EquivalentWarps equivalentAreas, out GameLocation originLocation, out Point closestWarpPoint)
        {
            var originParts = originName.Split("|");
            var originTrueLocationName = originParts[0];
            originLocation = Game1.getLocationFromName(originTrueLocationName);
            var destinationParts = destinationName.Split("|");
            destinationName = destinationParts[0];
            var allWarpPoints = originLocation.GetAllWarpPointsTo(destinationName, equivalentAreas);
            if (!allWarpPoints.Any())
            {
                closestWarpPoint = Point.Zero;
                return false;
            }

            if (originParts.Length >= 3)
            {
                var x = originParts[1];
                var y = originParts[2];
                var referencePoint = new Point(int.Parse(x), int.Parse(y));

                closestWarpPoint = allWarpPoints.OrderBy(x => x.GetTotalDistance(referencePoint)).First();
                return true;
            }

            if (destinationParts.Length >= 3)
            {
                var x = destinationParts[1];
                var y = destinationParts[2];
                var referencePoint = new Point(int.Parse(x), int.Parse(y));

                var loc = originLocation;
                var dest = destinationName;
                closestWarpPoint = allWarpPoints.OrderBy(warpPoint => loc.GetWarpPointTarget(warpPoint, dest, equivalentAreas).GetTotalDistance(referencePoint)).First();
                return true;
            }

            closestWarpPoint = allWarpPoints.First();
            return true;
        }
    }
}
