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
using System.Text;
using System.Threading.Tasks;
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
            {new WarpRequest(Game1.getLocationRequest("Town"), 35, 97, FacingDirection.Down), new WarpRequest(Game1.getLocationRequest("Sewer"), 16, 11, FacingDirection.Down)},
            {new WarpRequest(Game1.getLocationRequest("IslandWest"), 20, 23, FacingDirection.Down), new WarpRequest(Game1.getLocationRequest("QiNutRoom"), 7, 8, FacingDirection.Up)},
            {new WarpRequest(Game1.getLocationRequest("WizardHouse"), 4, 5, FacingDirection.Down), new WarpRequest(Game1.getLocationRequest("WizardHouseBasement"), 4, 4, FacingDirection.Down)},
            {new WarpRequest(Game1.getLocationRequest("IslandWest"), 77, 40, FacingDirection.Down), new WarpRequest(Game1.getLocationRequest("IslandFarmhouse"), 14, 17, FacingDirection.Down)},
            {new WarpRequest(Game1.getLocationRequest("DeepWoods"), 20, 6, FacingDirection.Up), new WarpRequest(Game1.getLocationRequest("DeepWoodsMaxHouse"), 19, 24, FacingDirection.Up)}
        };

        public static List<Point> GetAllWarpPointsTo(this GameLocation origin, string destinationName)
        {
            var warpPoints = new List<Point>();
            warpPoints.AddRange(GetAllTouchWarpsTo(origin, destinationName).Select(warp => new Point(warp.X, warp.Y)));
            warpPoints.AddRange(GetAllTouchActionWarpsTo(origin, destinationName).Select(x => new Point(x.Key.X, x.Key.Y)));
            warpPoints.AddRange(GetDoorWarpPoints(origin, destinationName));
            warpPoints.AddRange(GetAllActionWarpsTo(origin, destinationName).Select(x => new Point(x.Key.X, x.Key.Y)));
            warpPoints.AddRange(GetSpecialTriggerWarps(origin, destinationName).Keys);

            return warpPoints.Distinct().ToList();
        }

        public static Point GetWarpPointTarget(this GameLocation origin, Point warpPointLocation, string destinationName)
        {
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
            foreach (var (warp, target) in GetAllActionWarpsTo(origin, destinationName))
            {
                if (warp.X == warpPointLocation.X && warp.Y == warpPointLocation.Y)
                {
                    return new Point(target.X, target.Y);
                }
            }

            foreach (var (warp, warpTarget) in GetSpecialTriggerWarps(origin, destinationName))
            {
                if (warp.X == warpPointLocation.X && warp.Y == warpPointLocation.Y)
                {
                    return new Point(warpTarget.X, warpTarget.Y);
                }
            }

            throw new Exception(
                $"Could not find Warp Point Target for '{origin.Name}' to '{destinationName}' at [{warpPointLocation.X}, {warpPointLocation.Y}]");
        }

        private static List<Warp> GetAllTouchWarpsTo(GameLocation origin, string destinationName)
        {
            var warps = new List<Warp>();
            foreach (var warp in origin.warps)
            {
                if (warp.TargetName.Equals(destinationName, StringComparison.OrdinalIgnoreCase))
                {
                    warps.Add(warp);
                }
                else if (warp.TargetName == "VolcanoEntrance" && destinationName == "VolcanoDungeon0")
                {
                    warps.Add(warp);
                }
            }

            warps.AddRange(GetSpecialTouchWarps(origin));
            return warps;
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
            }

            _actionWarpCache.Add(key, actionWarps);
            return actionWarps;
        }

        private static bool TryGetWarpPointFromProperty(string destinationName, PropertyValue propertyValue, out Point warpPoint)
        {
            var propertyString = propertyValue.ToString();
            var touchActionParts = propertyString.Split(' ');
            var touchAction = touchActionParts[0];
            if (!touchAction.Contains("Warp") || touchActionParts.Length < 4)
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
                if (pair.Value.Equals(destinationName, StringComparison.OrdinalIgnoreCase))
                {
                    yield return pair.Key;
                }
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
            foreach (var (warp1, warp2) in ExtraWarps.Union(ExtraWarps.ToDictionary(x => x.Value, x => x.Key)))
            {
                if (!warp1.LocationRequest.Name.Equals(origin.Name, StringComparison.OrdinalIgnoreCase) ||
                    !warp2.LocationRequest.Name.Equals(destinationName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                specialTriggerWarps.Add(new Point(warp1.TileX, warp1.TileY), new Point(warp2.TileX, warp2.TileY));
            }

            return specialTriggerWarps;
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

        public static bool TryGetClosestWarpPointTo(this string originName, ref string destinationName, out GameLocation originLocation, out Point closestWarpPoint)
        {
            var originParts = originName.Split("|");
            var originTrueLocationName = originParts[0];
            originLocation = Game1.getLocationFromName(originTrueLocationName);
            var destinationParts = destinationName.Split("|");
            destinationName = destinationParts[0];
            var allWarpPoints = originLocation.GetAllWarpPointsTo(destinationName);
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
                closestWarpPoint = allWarpPoints.OrderBy(x => loc.GetWarpPointTarget(x, dest).GetTotalDistance(referencePoint)).First();
                return true;
            }

            closestWarpPoint = allWarpPoints.First();
            return true;
        }
    }
}
