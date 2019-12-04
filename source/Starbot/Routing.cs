using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starbot
{

    public static class Routing
    {
        public static bool Ready = false;
        private static Dictionary<string, HashSet<string>> MapConnections = new Dictionary<string, HashSet<string>>();

        public static void Reset()
        {
            Ready = false;
            if (Game1.IsMultiplayer && !Game1.IsMasterGame)
            {
                //client mode
                MapConnections.Clear();
                Mod.instance.Monitor.Log("Starbot is now in multiplayer client mode.", LogLevel.Info);
                Mod.instance.Monitor.Log("The server will need to have Starbot installed to proceed.", LogLevel.Info);
                Mod.instance.Monitor.Log("Awaiting response from server...", LogLevel.Info);
                Mod.instance.Helper.Multiplayer.SendMessage<int>(0, "authRequest");
            } else
            {
                //host/singleplayer mode
                MapConnections = BuildRouteCache();
                Ready = true;
            }
        }

        public static Dictionary<string, HashSet<string>> BuildRouteCache()
        {
            var returnValue = new Dictionary<string, HashSet<string>>();
            foreach (var gl in Game1.locations)
            {
                string key = gl.NameOrUniqueName;
                if (!string.IsNullOrWhiteSpace(key))// && !gl.isTemp())
                {
                    if (gl.warps != null && gl.warps.Count > 0)
                    {
                        //Mod.instance.Monitor.Log("Learning about " + key, LogLevel.Alert);
                        returnValue[key] = new HashSet<string>();
                        foreach (var w in gl.warps) returnValue[key].Add(w.TargetName);
                        foreach (var d in gl.doors.Values) returnValue[key].Add(d);
                        //foreach (var s in MapConnections[key]) Mod.instance.Monitor.Log("It connects to " + s, LogLevel.Warn);
                    }
                }
                if(gl is StardewValley.Locations.BuildableGameLocation)
                {
                    StardewValley.Locations.BuildableGameLocation bl = gl as StardewValley.Locations.BuildableGameLocation;
                    foreach(var b in bl.buildings)
                    {
                        if(!returnValue.ContainsKey(key)) returnValue[key] = new HashSet<string>();
                        returnValue[key].Add(b.indoors.Value.NameOrUniqueName);
                        //add the way in
                        returnValue[b.indoors.Value.NameOrUniqueName] = new HashSet<string>();
                        //add the way out
                        returnValue[b.indoors.Value.NameOrUniqueName].Add(key);
                    }
                }
            }
            return returnValue;
        }

        public static void Multiplayer_ModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (Game1.IsMasterGame && e.Type == "authRequest")
            {
                Mod.instance.Monitor.Log("Starbot authorization requested by client. Approving...");
                //listen for authorization requests
                Dictionary<string, HashSet<string>> response = null;
                if (MapConnections.Count > 0)
                {
                    //host bot is active, use existing cache
                    response = MapConnections;
                } else
                {
                    response = BuildRouteCache();
                }
                Mod.instance.Helper.Multiplayer.SendMessage<Dictionary<string, HashSet<string>>>(response, "authResponse");
            } else if(!Game1.IsMasterGame && e.Type == "authResponse")
            {
                //listen for authorization responses
                MapConnections = e.ReadAs<Dictionary<string, HashSet<string>>>();
                Mod.instance.Monitor.Log("Starbot authorization request was approved by server.");
                Mod.instance.Monitor.Log("Server offered routing data for " + MapConnections.Count + " locations.");
                Ready = true;
            } else if(e.Type == "taskAssigned")
            {
                string task = e.ReadAs<string>();
                Mod.instance.Monitor.Log("Another player has taken task: " + task);
                Core.ObjectivePool.RemoveAll(x => x.UniquePoolId == task);
            }
        }

        public static List<string> GetRoute(string destination)
        {
            return GetRoute(Game1.player.currentLocation.NameOrUniqueName, destination);
        }

        public static List<string> GetRoute(string start, string destination)
        {
            var result = SearchRoute(start, destination);
            if (result != null) result.Add(destination);
            return result;
        }

        private static List<string> SearchRoute(string step, string target, List<string> route = null, List<string> blacklist = null)
        {
            if (route == null) route = new List<string>();
            if (blacklist == null) blacklist = new List<string>();
            List<string> route2 = new List<string>(route);
            route2.Add(step);
            foreach (string s in MapConnections[step])
            {
                if (route.Contains(s) || blacklist.Contains(s)) continue;
                if (s == target)
                {
                    return route2;
                }
                List<string> result = SearchRoute(s, target, route2, blacklist);
                if (result != null) return result;
            }
            blacklist.Add(step);
            return null;
        }
    }
}
