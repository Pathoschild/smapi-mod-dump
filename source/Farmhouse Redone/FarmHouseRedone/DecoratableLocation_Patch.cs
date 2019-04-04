using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using Harmony;
using xTile;

namespace FarmHouseRedone
{

    class DecoratableLocation_doSetVisibleWallpaper_Patch
    {
        static void setMapTileIndexIfOnTileSheet(Map map, DecoratableLocation instance, int x, int y, int index, string layer, int tileSheet, int tileSheetToMatch)
        {
            if (map.GetLayer(layer).Tiles[x, y] != null && map.GetLayer(layer).Tiles[x, y].TileSheet.Equals((object)map.TileSheets[tileSheetToMatch]))
                instance.setMapTileIndex(x, y, index, layer, tileSheet);
        }

        static int getNewTileIndex(Map map, int x, int y, string layer, int destinationIndex)
        {
            int currentIndex = map.GetLayer(layer).Tiles[x, y].TileIndex;
            int whichHeight = (currentIndex % 48) / 16;
            return destinationIndex + (whichHeight * 16);
        }

        static void setMapTileIndexForAnyLayer(Map map, DecoratableLocation instance, int x, int y, int index)
        {
            //Logger.Log("Working on tile (" + x + ", " + y + ") on the Back layer...");
            if (isTileOnWallsSheet(map, "Back", x, y))
                instance.setMapTileIndex(x, y, getNewTileIndex(map, x, y, "Back", index), "Back", 0);
            //Logger.Log("Working on tile (" + x + ", " + y + ") on the Buildings layer...");
            if (isTileOnWallsSheet(map, "Buildings", x, y))
                instance.setMapTileIndex(x, y, getNewTileIndex(map, x, y, "Buildings", index), "Buildings", 0);
            //Logger.Log("Working on tile (" + x + ", " + y + ") on the Front layer...");
            if (isTileOnWallsSheet(map, "Front", x, y))
                instance.setMapTileIndex(x, y, getNewTileIndex(map, x, y, "Front", index), "Front", 0);
        }

        static bool isTileOnWallsSheet(Map map, string layer, int x, int y)
        {
            bool result = (map.GetLayer(layer).Tiles[x, y] != null && map.GetLayer(layer).Tiles[x, y].TileSheet.Equals(map.TileSheets[FarmHouseStates.wallAndFloorsSheet]) && map.GetLayer(layer).Tiles[x, y].TileIndex <= 335);
            //Logger.Log("Tile (" + x + ", " + y + ") on layer '" + layer + "' was " + (result ? "" : "not") + " a wall tile.");
            return result;
        }

        internal static bool Prefix(int whichRoom, int which, DecoratableLocation __instance)
        {
            if (!(__instance is FarmHouse) || __instance is Cabin)
                return true;
            //Logger.Log("Setting visible walls...");
            //Logger.Log("Updating map...");
            __instance.updateMap();
            //Logger.Log("Map updated.  Getting walls...");
            List<Microsoft.Xna.Framework.Rectangle> walls = __instance.getWalls();
            int index = which % 16 + which / 16 * 48;
            Logger.Log("Chosen index " + index + ".");

            Map map = __instance.map;

            if (whichRoom == -1)
            {
                Logger.Log("Applying to all wall rectangles...");
                foreach (Microsoft.Xna.Framework.Rectangle rectangle in walls)
                {
                    for(int x = rectangle.X; x < rectangle.Right; x++)
                    {
                        for(int y = rectangle.Y; y < rectangle.Bottom; y++)
                        {
                            //Logger.Log("Working on tile (" + x + ", " + rectangle.Y + ")...");
                            setMapTileIndexForAnyLayer(map, __instance, x, y, index);
                        }
                    }


                    //Logger.Log("Applying wall rectangle (" + rectangle.X + ", " + rectangle.Y + ", " + rectangle.Width + ", " + rectangle.Height + ")...");
                    //for (int x = rectangle.X; x < rectangle.Right; ++x)
                    //{
                    //    //Logger.Log("Setting tile (" + x + ", " + rectangle.Y + ") on the Back layer...");
                    //    setMapTileIndexIfOnTileSheet(map, __instance, x, rectangle.Y, index, "Back", 0, 2);
                    //    //Logger.Log("Setting tile (" + x + ", " + (rectangle.Y + 1) + ") on the Back layer...");
                    //    setMapTileIndexIfOnTileSheet(map, __instance, x, rectangle.Y + 1, index + 16, "Back", 0, 2);
                    //    if (rectangle.Height >= 3)
                    //    {
                    //        //Logger.Log("Rectangle was tall...");
                    //        if (__instance.map.GetLayer("Buildings").Tiles[x, rectangle.Y + 2].TileSheet.Equals((object)__instance.map.TileSheets[2]))
                    //        {
                    //            //Logger.Log("Buildings tile (" + x + ", " + (rectangle.Y + 2) + ") was from the walls and floors sheet.  Setting it to wallpapered state...");
                    //            __instance.setMapTileIndex(x, rectangle.Y + 2, index + 32, "Buildings", 0);
                    //        }
                    //        else
                    //        {
                    //            //Logger.Log("Setting tile (" + x + ", " + (rectangle.Y + 2) + ") to index " + index + 32 + "...");
                    //            __instance.setMapTileIndex(x, rectangle.Y + 2, index + 32, "Back", 0);
                    //        }
                    //    }
                    //}
                }
            }
            else
            {
                Logger.Log("Applying wall rectangle...");
                if (walls.Count <= whichRoom)
                {
                    Logger.Log("Wall rectangle exceeded walls count!", StardewModdingAPI.LogLevel.Warn);
                    return false;
                }

                //List<Rectangle> connectedWalls = new List<Rectangle>();
                //connectedWalls.Add(walls[whichRoom]);
                //if (FarmHouseStates.wallDictionary.ContainsKey(walls[whichRoom]))
                //{
                //    string roomString = FarmHouseStates.wallDictionary[walls[whichRoom]];
                //    //Logger.Log("Looking for all walls for room " + roomString + "...");
                //    foreach (KeyValuePair<Rectangle, string> wallDefinition in FarmHouseStates.wallDictionary)
                //    {
                //        if (wallDefinition.Value.Equals(roomString))
                //            connectedWalls.Add(wallDefinition.Key);
                //    }
                //    //Logger.Log("Found " + connectedWalls.Count + " walls for " + roomString);
                //}
                //foreach (Rectangle rectangle in connectedWalls)
                //{
                //Logger.Log("Applying wall rectangle...");
                Rectangle rectangle = walls[whichRoom];
                for (int x = rectangle.X; x < rectangle.Right; x++)
                {
                    for (int y = rectangle.Y; y < rectangle.Bottom; y++)
                    {
                        //Logger.Log("Working on tile (" + x + ", " + rectangle.Y + ")...");
                        setMapTileIndexForAnyLayer(map, __instance, x, y, index);
                    }
                }


                    //Logger.Log("Applying wall rectangle (" + rectangle.X + ", " + rectangle.Y + ", " + rectangle.Width + ", " + rectangle.Height + ")...");
                    //for (int x = rectangle.X; x < rectangle.Right; ++x)
                    //{
                    //    //Logger.Log("Setting tile (" + x + ", " + rectangle.Y + ") on the Back layer...");
                    //    setMapTileIndexIfOnTileSheet(map, __instance, x, rectangle.Y, index, "Back", 0, 2);
                    //    //Logger.Log("Setting tile (" + x + ", " + (rectangle.Y + 1) + ") on the Back layer...");
                    //    setMapTileIndexIfOnTileSheet(map, __instance, x, rectangle.Y + 1, index + 16, "Back", 0, 2);
                    //    if (rectangle.Height >= 3)
                    //    {
                    //        //Logger.Log("Rectangle was tall...");
                    //        if (__instance.map.GetLayer("Buildings").Tiles[x, rectangle.Y + 2] == null)
                    //            continue;
                    //        if (__instance.map.GetLayer("Buildings").Tiles[x, rectangle.Y + 2].TileSheet.Equals((object)__instance.map.TileSheets[2]))
                    //        {
                    //            //Logger.Log("Buildings tile (" + x + ", " + (rectangle.Y + 2) + ") was from the walls and floors sheet.  Setting it to wallpapered state...");
                    //            __instance.setMapTileIndex(x, rectangle.Y + 2, index + 32, "Buildings", 0);
                    //        }
                    //        else
                    //        {
                    //            //Logger.Log("Setting tile (" + x + ", " + (rectangle.Y + 2) + ") to index " + index + 32 + "...");
                    //            __instance.setMapTileIndex(x, rectangle.Y + 2, index + 32, "Back", 0);
                    //        }
                    //    }
                    //}
            }
            return false;
        }
    }

    class DecoratableLocation_setFloor_Patch
    {

        internal static bool Prefix(int which, int whichRoom, bool persist, DecoratableLocation __instance)
        {
            if (!(__instance is FarmHouse) || __instance is Cabin)
                return true;
            List<Microsoft.Xna.Framework.Rectangle> floors = __instance.getFloors();
            if (!persist)
                return true;

            if (__instance.floor.Count < floors.Count)
            {
                FarmHouseStates.setMissingFloorsToDefault(__instance as FarmHouse);
            }

            //__instance.floor.SetCountAtLeast(floors.Count);
            if (whichRoom == -1)
            {
                for (int index = 0; index < __instance.floor.Count; ++index)
                    __instance.floor[index] = which;
            }
            else
            {
                if (whichRoom > __instance.floor.Count - 1 || whichRoom >= floors.Count)
                    return false;
                if (FarmHouseStates.floorDictionary.ContainsKey(floors[whichRoom]))
                {
                    string roomLabel = FarmHouseStates.floorDictionary[floors[whichRoom]];
                    Logger.Log("Finding all floors for room '" + roomLabel + "'...");
                    foreach(KeyValuePair<Rectangle, string> floorData in FarmHouseStates.floorDictionary)
                    {

                        if (floors.Contains(floorData.Key) && floorData.Value == roomLabel)
                        {
                            Logger.Log(floors.IndexOf(floorData.Key) + " was a part of " + roomLabel);
                            __instance.floor[floors.IndexOf(floorData.Key)] = which;
                        }
                    }
                }
                else
                    __instance.floor[whichRoom] = which;
            }
            return false;
        }
    }

    class DecoratableLocation_setWallpaper_Patch
    {
        internal static bool Prefix(int which, int whichRoom, bool persist, DecoratableLocation __instance)
        {
            if (!(__instance is FarmHouse) || __instance is Cabin)
                return true;
            List<Microsoft.Xna.Framework.Rectangle> walls = __instance.getWalls();
            if (!persist)
                return true;

            Logger.Log("Checking wallpaper indexes before SetCountAtLeast...");
            for (int wallIndex = 0; wallIndex < __instance.wallPaper.Count; wallIndex++)
            {
                Logger.Log("Wall " + wallIndex + " has a wallpaper index of " + __instance.wallPaper[wallIndex] + ".");
            }

            if (__instance.wallPaper.Count < walls.Count)
            {
                FarmHouseStates.setMissingWallpaperToDefault(__instance as FarmHouse);
            }
            //__instance.wallPaper.SetCountAtLeast(walls.Count);

            Logger.Log("Checking wallpaper indexes after SetCountAtLeast...");
            for (int wallIndex = 0; wallIndex < __instance.wallPaper.Count; wallIndex++)
            {
                Logger.Log("Wall " + wallIndex + " has a wallpaper index of " + __instance.wallPaper[wallIndex] + ".");
            }
            if (whichRoom == -1)
            {
                Logger.Log("Whichroom was -1, applying to all walls...");
                for (int index = 0; index < __instance.wallPaper.Count; ++index)
                    __instance.wallPaper[index] = which;
            }
            else
            {
                Logger.Log("Setting wallpaper to " + which + "...");
                if (whichRoom > __instance.wallPaper.Count - 1 || whichRoom >= walls.Count)
                    return false;
                if (FarmHouseStates.wallDictionary.ContainsKey(walls[whichRoom]))
                {
                    string roomLabel = FarmHouseStates.wallDictionary[walls[whichRoom]];
                    Logger.Log("Finding all walls for room '" + roomLabel + "'...");
                    foreach (KeyValuePair<Rectangle, string> wallData in FarmHouseStates.wallDictionary)
                    {

                        if (walls.Contains(wallData.Key) && wallData.Value == roomLabel)
                        {
                            Logger.Log(walls.IndexOf(wallData.Key) + " was a part of " + roomLabel);
                            __instance.wallPaper[walls.IndexOf(wallData.Key)] = which;
                        }
                    }
                }
                else
                {
                    __instance.wallPaper[whichRoom] = which;
                }
            }
            return false;
        }
    }
}
