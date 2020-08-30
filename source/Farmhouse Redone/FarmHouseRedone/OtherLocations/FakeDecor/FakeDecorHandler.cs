using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Locations;
using Microsoft.Xna.Framework;

namespace FarmHouseRedone.OtherLocations.FakeDecor
{
    public static class FakeDecorHandler
    {
        public static Dictionary<string, DecoratableLocation> decoratableHosts;

        public static void init()
        {
            decoratableHosts = new Dictionary<string, DecoratableLocation>();
            //load in all the previously-saved decoratable hosts
            foreach(GameLocation location in Game1.locations)
            {
                if(location.name != null && location.Name.Split('_')[0].Equals("DECORHOST"))
                {
                    if (!(location is DecoratableLocation))
                    {
                        Logger.Log("A decor host was found, but wasn't a DecoratableLocation!  Skipping...", StardewModdingAPI.LogLevel.Warn);
                    }
                    Logger.Log("Found a decor host: " + location.Name + ", hosting " + location.Name.Substring(10));
                    DecoratableLocation host = location as DecoratableLocation;
                    decoratableHosts[location.Name.Substring(10)] = host;
                    //host.wallPaper.OnChange += WallPaper_OnChange;
                    //host.floor.OnChange += Floor_OnChange;
                }
            }
            Logger.Log("Found " + decoratableHosts.Count + " hosts.  Creating any missing hosts...");

            foreach(GameLocation location in Game1.locations)
            {
                //Decoratable locations don't need a host
                if (location is DecoratableLocation)
                    continue;
                //Can't create a host for unnamed locations, or locations without a map.
                if (location.map == null || location.Name == null)
                    continue;
                //Location already has a host
                if (decoratableHosts.ContainsKey(location.Name))
                {
                    Logger.Log("Location " + location.Name + " has a decor host.");
                    continue;
                }

                bool needsHost = false;
                foreach (string propertyKey in location.map.Properties.Keys)
                {
                    if (propertyKey.Equals("Walls") || propertyKey.Equals("Floors"))
                    {
                        needsHost = true;
                        break;
                    }
                }
                if (!needsHost)
                    continue;
                //This is a location which needs a host, so now we generate one.

                DecoratableLocation host = new DecoratableLocation(location.mapPath, "DECORHOST_" + location.Name);
                //host.wallPaper.OnChange += WallPaper_OnChange;
                //host.floor.OnChange += Floor_OnChange;
                Logger.Log("Created host for " + location.Name + ": " + host.Name);
                decoratableHosts[location.Name] = host;
            }

            Logger.Log("Host count is now " + decoratableHosts.Count + " adding any missing hosts to Game1.locations");

            foreach(string key in decoratableHosts.Keys)
            {
                DecoratableLocation host = decoratableHosts[key];
                if (!Game1.locations.Contains(host))
                {
                    Logger.Log("Adding " + host.Name + " to locations");

                    Game1.locations.Add(host);
                }
            }
        }

        //private static void Floor_OnChange(int whichRoom, int which)
        //{
        //    GameLocation currentLoc = Game1.currentLocation;
        //    if (currentLoc is DecoratableLocation)
        //        return;
        //    if (!decoratableHosts.ContainsKey(currentLoc.Name))
        //    {
        //        Logger.Log("No host for " + currentLoc.Name + "!", StardewModdingAPI.LogLevel.Error);
        //        return;
        //    }
        //    DecoratableLocation host = decoratableHosts[currentLoc.Name];


        //}

        //private static void WallPaper_OnChange(int whichRoom, int which)
        //{
        //    throw new NotImplementedException();
        //}

        public static GameLocation getClient(DecoratableLocation location)
        {
            if (!decoratableHosts.ContainsValue(location))
                return null;
            string nameOfClient = "";
            foreach (string name in decoratableHosts.Keys)
            {
                if (decoratableHosts[name] == location)
                {
                    nameOfClient = name;
                    break;
                }
            }
            if (nameOfClient == "")
            {
                Logger.Log("Failed to find client of " + location.Name + "!", StardewModdingAPI.LogLevel.Error);
                return null;
            }
            GameLocation client = Game1.getLocationFromName(nameOfClient);
            if (client == null)
            {
                Logger.Log("No location could be found by the name \"" + nameOfClient + "\"!", StardewModdingAPI.LogLevel.Error);
                return null;
            }
            return client;
        }

        public static DecoratableLocation getHost(GameLocation location)
        {
            if (!decoratableHosts.ContainsKey(location.Name))
            {
                return null;
            }
            return decoratableHosts[location.Name];
        }

        public static void updateAsHost(DecoratableLocation location)
        {
            

            foreach(Rectangle wall in location.getWalls())
            {

            }
        }
    }
}
