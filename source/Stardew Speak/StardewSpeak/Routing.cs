/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/evfredericksen/StardewSpeak
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewSpeak
{

    public static class Routing
    {
        private static bool Ready = false;
        public static Dictionary<string, List<LocationConnection>> MapConnections = new Dictionary<string, List<LocationConnection>>();

        public class LocationConnection {
            public string TargetName;
            public int X;
            public int Y;
            public bool IsDoor;
            public bool TargetIsOutdoors;
            public LocationConnection(string targetName, int x, int y, bool isDoor, bool targetIsOutdoors) {
                this.TargetName = targetName;
                this.X = x;
                this.Y = y;
                this.IsDoor = isDoor;
                this.TargetIsOutdoors = targetIsOutdoors;
            }
        }

        public static void Reset()
        {
            Ready = false;
            MapConnections = BuildRouteCache();
            Ready = true;
        }

        public static LocationConnection FindLocationConnection(GameLocation from, GameLocation to) {
            var connections = MapConnections[from.NameOrUniqueName];
            string toName = to.NameOrUniqueName;
            foreach (var cn in connections)
            {
                if (cn.TargetName == toName) return cn;
            }
            throw new InvalidOperationException($"Unable to find warp from {from.NameOrUniqueName} to {to.NameOrUniqueName}");
        }

        private static List<LocationConnection> LocationConnections(GameLocation from)
        {
            var connections = new List<LocationConnection>();
            foreach (Warp warp in from.warps)
            {
                GameLocation targetLoc = Game1.getLocationFromName(warp.TargetName);
                if (targetLoc != null)
                {
                    var lc = new LocationConnection(warp.TargetName, warp.X, warp.Y, false, targetLoc.IsOutdoors);
                    connections.Add(lc);
                }
            }
            foreach (var doorDict in from.doors)
            {
                foreach (var door in doorDict)
                {
                    Point point = door.Key;
                    string locName = door.Value;
                    GameLocation targetLoc = Game1.getLocationFromName(locName);
                    if (targetLoc != null)
                    {
                        var lc = new LocationConnection(locName, point.X, point.Y, true, targetLoc.IsOutdoors);
                        connections.Add(lc);
                    }
                }
            }
            if (from is StardewValley.Locations.BuildableGameLocation)
            {
                StardewValley.Locations.BuildableGameLocation bl = from as StardewValley.Locations.BuildableGameLocation;
                foreach (var b in bl.buildings)
                {
                    if (b.indoors.Value != null)
                    {
                        var point = b.humanDoor.Value;
                        var locName = b.indoors.Value.NameOrUniqueName;
                        var lc = new LocationConnection(locName, point.X + b.tileX.Value, point.Y + b.tileY.Value, true, false);
                        connections.Add(lc);
                    };
                }
            }
            return connections;
        }

        public static Dictionary<string, List<LocationConnection>> BuildRouteCache()
        {
            var routeCache = new Dictionary<string, List<LocationConnection>>();
            var locations = AllGameLocations();
            foreach (var gl in locations)
            {
                string locName = gl.NameOrUniqueName;
                routeCache[locName] = new List<LocationConnection>();
                foreach (var connection in LocationConnections(gl)) 
                {
                    routeCache[locName].Add(connection);
                }
            }
            return routeCache;
        }

        public static List<GameLocation> AllGameLocations(bool includeBuildings = true) 
        {
            var allLocations = new List<GameLocation>();
            foreach (var gl in Game1.locations)
            {
                string name = gl.NameOrUniqueName;
                if (string.IsNullOrWhiteSpace(name)) continue;
                allLocations.Add(gl);
                if (includeBuildings && gl is StardewValley.Locations.BuildableGameLocation)
                {
                    StardewValley.Locations.BuildableGameLocation bl = gl as StardewValley.Locations.BuildableGameLocation;
                    foreach (var b in bl.buildings)
                    {
                        if (b.indoors.Value != null) 
                        {
                            allLocations.Add(b.indoors.Value);
                        };
                    }
                }
            }
            return allLocations;
        }

        public static List<string> GetRoute(string start, string destination)
        {
            return SearchRoute(start, destination);
        }

        private static List<string> SearchRoute(string start, string target)
        {
            Func<dynamic, bool> validateTarget = (dynamic location) =>
            {
                if (location is Building)
                {
                    return location.indoors.Value.NameOrUniqueName == target;
                }
                return location.NameOrUniqueName == target;
            };
            return SearchRoute(start, validateTarget);
        }

        // bfs, just want to find shortest route
        private static List<string> SearchRoute(string start, Func<dynamic, bool> validateTarget) 
        {
            var queue = new Queue<string>();
            string target = null;
            queue.Enqueue(start);
            var seen = new List<string> { start };
            var mapLocationToPrev = new Dictionary<string, string>();
            while (queue.Count > 0)
            {
                var currentLocationName = queue.Dequeue();
                GameLocation currentLocation = Game1.getLocationFromName(currentLocationName);
                if (validateTarget(currentLocation)) 
                {
                    target = currentLocationName;
                    break;
                }
                foreach (var adj in MapConnections[currentLocationName])
                {
                    string adjName = adj.TargetName;
                    if (!seen.Contains(adjName)) 
                    {
                        mapLocationToPrev[adjName] = currentLocationName;
                        queue.Enqueue(adjName);
                        seen.Add(adjName);
                    }
                }
            }
            return target == null ? null : ReconstructRoute(target, mapLocationToPrev);
        }

        private static List<string> ReconstructRoute(string last, Dictionary<string, string> mapLocationToPrev) 
        {
            var route = new List<string> { last };
            var curr = last;
            while (mapLocationToPrev.ContainsKey(curr)) 
            {
                curr = mapLocationToPrev[curr];
                route.Add(curr);
            }
            route.Reverse();
            return route;
        }

    }
}
