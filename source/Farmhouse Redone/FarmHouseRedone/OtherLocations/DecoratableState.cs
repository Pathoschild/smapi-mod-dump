using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley.Locations;
using Microsoft.Xna.Framework;
using xTile;
using xTile.ObjectModel;

namespace FarmHouseRedone.OtherLocations
{
    public class DecoratableState
    {
        public DecoratableLocation location;

        public string WallsData;
        public string FloorsData;
        public Dictionary<Rectangle, string> floorDictionary;
        public Dictionary<Rectangle, string> wallDictionary;

        public DecoratableState(DecoratableLocation location)
        {
            this.location = location;
            WallsData = null;
            FloorsData = null;
            wallDictionary = new Dictionary<Rectangle, string>();
            floorDictionary = new Dictionary<Rectangle, string>();
        }

        public void clear()
        {
            WallsData = null;
            FloorsData = null;
            wallDictionary = new Dictionary<Rectangle, string>();
            floorDictionary = new Dictionary<Rectangle, string>();
        }

        public void updateFromMapPath(string mapPath)
        {
            clear();
            //Map map = makeMapCopy(house, mapPath);
            Map map = FarmHouseStates.loader.Load<Map>(mapPath, ContentSource.GameContent);
            Logger.Log("Updating walls and floors...");
            PropertyValue floors;
            map.Properties.TryGetValue("Floors", out floors);
            if (floors != null)
                FloorsData = Utility.cleanup(floors.ToString());
            else
                FloorsData = "";
            PropertyValue walls;
            map.Properties.TryGetValue("Walls", out walls);
            if (walls != null)
                WallsData = Utility.cleanup(walls.ToString());
            else
                WallsData = "";
            MapUtilities.FacadeHelper.setWallpaperDefaults(location);
        }

        public List<Rectangle> getFloors()
        {
            List<Rectangle> outFloors = new List<Rectangle>();

            floorDictionary.Clear();
            Logger.Log("Getting floor rectangles...");
            if (FloorsData == null)
                updateFromMapPath(location.mapPath);
            if (FloorsData != "")
            {
                string[] floorArray = FloorsData.Split(' ');
                for (int index = 0; index < floorArray.Length; index += 5)
                {
                    try
                    {
                        Rectangle rectResult = new Rectangle(Convert.ToInt32(floorArray[index]), Convert.ToInt32(floorArray[index + 1]), Convert.ToInt32(floorArray[index + 3]), Convert.ToInt32(floorArray[index + 4]));
                        outFloors.Add(rectResult);
                        floorDictionary[rectResult] = floorArray[index + 2];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Logger.Log("Partial floor rectangle definition detected! (" + FloorsData.Substring(index) + ")  Floor rectangles must be defined as\nX Y Identifier Width Height\nPlease ensure all floor definitions have exactly these 5 values.", LogLevel.Error);
                    }
                    catch (FormatException)
                    {
                        string errorLocation = "";
                        for (int errorIndex = index; errorIndex < floorArray.Length && errorIndex - index < 5; errorIndex += 1)
                        {
                            errorLocation += floorArray[errorIndex] + " ";
                        }
                        Logger.Log("Incorrect floor rectangle format. (" + errorLocation + ")  Floor rectangles must be defined as\nX Y Identifier Width Height\nPlease ensure all floor definitions have exactly these 5 values.", LogLevel.Error);
                    }
                }
            }
            return outFloors;
        }

        public List<Rectangle> getWalls()
        {
            List<Rectangle> outWalls = new List<Rectangle>();

            wallDictionary.Clear();
            Logger.Log("Getting walls...");
            if (WallsData == null)
                updateFromMapPath(location.mapPath);
            if (WallsData != "")
            {
                string[] wallArray = WallsData.Split(' ');
                for (int index = 0; index < wallArray.Length; index += 5)
                {
                    try
                    {
                        Rectangle rectResult = new Rectangle(Convert.ToInt32(wallArray[index]), Convert.ToInt32(wallArray[index + 1]), Convert.ToInt32(wallArray[index + 3]), Convert.ToInt32(wallArray[index + 4]));
                        outWalls.Add(rectResult);
                        wallDictionary[rectResult] = wallArray[index + 2];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Logger.Log("Partial wall rectangle definition detected! (" + WallsData.Substring(index) + ")  Wall rectangles must be defined as\nX Y Identifier Width Height\nPlease ensure all wall definitions have exactly these 5 values.", LogLevel.Error);
                    }
                    catch (FormatException)
                    {
                        string errorLocation = "";
                        for (int errorIndex = index; errorIndex < wallArray.Length && errorIndex - index < 5; errorIndex += 1)
                        {
                            errorLocation += wallArray[errorIndex] + " ";
                        }
                        Logger.Log("Incorrect wall rectangle format. (" + errorLocation + ")  Wall rectangles must be defined as\nX Y Identifier Width Height\nPlease ensure all wall definitions have exactly these 5 values.", LogLevel.Error);
                    }
                }
            }
            string wallString = "Found " + outWalls.Count + " walls: ";
            foreach(Rectangle wall in outWalls)
            {
                wallString += "\n" + wall.ToString();
            }
            Logger.Log(wallString);
            return outWalls;
        }

        public void updateWallpapers()
        {
            
        }
    }
}
