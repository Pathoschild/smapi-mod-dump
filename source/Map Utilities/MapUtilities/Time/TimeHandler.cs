using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using xTile;
using xTile.Layers;
using xTile.ObjectModel;

namespace MapUtilities.Time
{
    public static class TimeHandler
    {

        public const int Merge = 0;
        public const int Replace = 1;
        public const int Properties = 2;

        public static void tenMinuteUpdate()
        {
            if (Game1.currentLocation == null || !hasTimeLayer(Game1.currentLocation))
                return;
            Dictionary<int, List<Layer>> timesAndLayers = getLayerTimes(Game1.currentLocation);

            if (timesAndLayers.ContainsKey(Game1.timeOfDay))
            {
                foreach(Layer layer in timesAndLayers[Game1.timeOfDay])
                {
                    Logger.log("Applying " + layer.Id);
                    applyTimeLayer(Game1.currentLocation, layer);
                }
            }
        }

        public static void applyAllLayersToNow(GameLocation location)
        {
            Dictionary<int, List<Layer>> timesAndLayers = getLayerTimes(location);

            int timeOfDay = Game1.timeOfDay;
            if (timeOfDay % 100 == 0)
                timeOfDay -= 40;

            for (int time = 600; time <= timeOfDay; time += 10)
            {
                if (time % 100 == 60)
                    time += 40;
                Logger.log("Testing " + time);
                if (timesAndLayers.ContainsKey(time))
                {
                    List<Layer> timeLayers = timesAndLayers[time];
                    foreach(Layer layer in timeLayers)
                    {
                        Logger.log("Applying " + layer.Id);
                        applyTimeLayer(location, layer);
                    }
                }
            }
        }

        public static Dictionary<int, List<Layer>> getLayerTimes(GameLocation location)
        {
            Dictionary<int, List<Layer>> timesAndLayers = new Dictionary<int, List<Layer>>();
            foreach (Layer layer in getAllTimeLayers(location))
            {
                PropertyValue timeProperty;
                layer.Properties.TryGetValue("Time", out timeProperty);
                if (timeProperty == null)
                {
                    Logger.log("Layer missing Time property!  This shouldn't be able to happen...", StardewModdingAPI.LogLevel.Warn);
                    continue;
                }
                string[] times = timeProperty.ToString().Split(' ');
                foreach (string time in times)
                {
                    try
                    {
                        int timeOfDay = Convert.ToInt32(time);
                        if (timesAndLayers.ContainsKey(timeOfDay))
                        {
                            timesAndLayers[timeOfDay].Add(layer);
                        }
                        else
                        {
                            timesAndLayers[timeOfDay] = new List<Layer>();
                            timesAndLayers[timeOfDay].Add(layer);
                        }
                    }
                    catch (FormatException)
                    {
                        Logger.log("Layer " + layer.Id + " has incorrectly formatted Time property, '" + time + "'!  Each time must be given as an integer value, separated by spaces.\nGiven " + timeProperty.ToString());
                        continue;
                    }
                }
            }
            return timesAndLayers;
        }

        internal static void applyAllLayersTest(GameLocation location)
        {
            foreach(Layer layer in getAllTimeLayers(location))
            {
                applyTimeLayer(location, layer);
            }
        }

        public static void applyTimeLayer(GameLocation location, Layer layer)
        {
            if (location.map == null || layer == null)
                return;
            Map map = location.map;

            int mergeLogic;
            string layerID;
            Layer layerToMergeInto;

            PropertyValue mergeProperty;
            layer.Properties.TryGetValue("Merge", out mergeProperty);
            if (mergeProperty == null)
            {
                mergeLogic = Merge;
            }
            else
            {
                string mergeType = mergeProperty.ToString().Substring(0,1).ToLower();
                switch (mergeType)
                {
                    case "0":
                    case "m":
                        mergeLogic = 0;
                        break;
                    case "1":
                    case "r":
                        mergeLogic = 1;
                        break;
                    case "2":
                    case "p":
                        mergeLogic = 2;
                        break;
                    default:
                        Logger.log("No interperetation found for Merge property '" + mergeProperty.ToString() + "'.  Using default (Merge)...", StardewModdingAPI.LogLevel.Warn);
                        mergeLogic = 0;
                        break;
                }
            }

            PropertyValue layerIDProperty;
            layer.Properties.TryGetValue("Layer", out layerIDProperty);
            if (layerIDProperty == null)
            {
                Logger.log("No layer set to merge into from time-layer " + layer.Id + "!  Aborting...", StardewModdingAPI.LogLevel.Error);
                return;
            }
            else
            {
                layerID = layerIDProperty.ToString();
                if(!hasLayer(location, layerID))
                {
                    Logger.log("Time-layer " + layer.Id + " was set to merge into " + layerID + ", but no layer by that name was found!  Skipping...", StardewModdingAPI.LogLevel.Warn);
                    return;
                }
                layerToMergeInto = getLayer(location, layerID);
            }

            int width = layerToMergeInto.LayerWidth;
            int height = layerToMergeInto.LayerHeight;

            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    if(mergeLogic == Properties)
                    {
                        if(layer.Tiles[x,y] != null && layerToMergeInto.Tiles[x, y] != null)
                        {
                            layerToMergeInto.Tiles[x, y].Properties.Clear();
                            layerToMergeInto.Tiles[x, y].Properties.CopyFrom(layer.Tiles[x, y].Properties);
                        }
                    }
                    if (layer.Tiles[x, y] != null)
                        layerToMergeInto.Tiles[x, y] = layer.Tiles[x, y].Clone(layer);
                    else if (mergeLogic == Replace)
                        layerToMergeInto.Tiles[x, y] = null;
                }
            }

        }

        public static List<Layer> getAllTimeLayers(GameLocation location)
        {
            List<Layer> layers = new List<Layer>();
            if (location.map == null)
                return null;
            Map map = location.map;
            foreach (Layer layer in map.Layers)
            {
                PropertyValue timeProperty;
                layer.Properties.TryGetValue("Time", out timeProperty);
                if (timeProperty != null)
                {
                    Logger.log("Found " + layer.Id + ", Time | " + timeProperty);
                    layers.Add(layer);
                }
            }
            return layers;
        }

        public static bool hasTimeLayer(GameLocation location)
        {
            if (location.map == null)
                return false;
            Map map = location.map;
            foreach(Layer layer in map.Layers)
            {
                PropertyValue timeProperty;
                layer.Properties.TryGetValue("Time", out timeProperty);
                if(timeProperty != null)
                {
                    Logger.log("Found " + layer.Id + ", Time | " + timeProperty);
                    return true;
                }
            }
            return false;
        }

        public static bool hasLayer(GameLocation location, string layerID)
        {
            foreach(Layer layer in location.map.Layers)
            {
                if (layer.Id.Equals(layerID))
                    return true;
            }
            return false;
        }

        public static Layer getLayer(GameLocation location, string layerID)
        {
            foreach(Layer layer in location.map.Layers)
            {
                if (layer.Id.Equals(layerID))
                    return layer;
            }
            return null;
        }
    }
}
