/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mjSurber/FarmHouseRedone
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using xTile;

namespace FarmHouseRedone
{

    class DecoratableLocation_resetForPlayerEntry_Patch
    {
        internal static bool Prefix(DecoratableLocation __instance)
        {
            MapUtilities.FacadeHelper.setWallpaperDefaults(__instance);
            //if (__instance.Name.StartsWith("DECORHOST_"))
            //{
                
            //}
            return true;
        }
    }


    class DecoratableLocation_doSetVisibleWallpaper_Patch
    {
        static int getNewTileIndex(Map map, int x, int y, string layer, int destinationIndex)
        {
            if (map.GetLayer(layer).Tiles[x, y] == null)
                return -1;
            int currentIndex = map.GetLayer(layer).Tiles[x, y].TileIndex;
            int whichHeight = (currentIndex % 48) / 16;
            return destinationIndex + (whichHeight * 16);
        }

        static void setMapTileIndexForAnyLayer(Map map, DecoratableLocation instance, int x, int y, int index)
        {
            MapUtilities.MapMerger.setMapTileIndexIfOnTileSheet(map, x, y, getNewTileIndex(map, x, y, "Back", index), "Back", MapUtilities.SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 0, 16, 21));
            MapUtilities.MapMerger.setMapTileIndexIfOnTileSheet(map, x, y, getNewTileIndex(map, x, y, "Buildings", index), "Buildings", MapUtilities.SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 0, 16, 21));
            MapUtilities.MapMerger.setMapTileIndexIfOnTileSheet(map, x, y, getNewTileIndex(map, x, y, "Front", index), "Front", MapUtilities.SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 0, 16, 21));
        }

        internal static bool Prefix(int whichRoom, int which, DecoratableLocation __instance)
        {
            MapUtilities.MapMerger.DoSetVisibleWallpaper(whichRoom, which, __instance);
            ////if (!(__instance is FarmHouse))
            ////    return true;
            //__instance.updateMap();

            ////Gather a list of all the walls in this map
            //List<Rectangle> walls = __instance.getWalls();
            //int index = which % 16 + which / 16 * 48;

            ////Report the index of the wallpaper being pasted.
            //Logger.Log("Chosen index " + index + ".");

            ////Get the map for this DecoratableLocation
            //Map map = __instance.map;


            //Logger.Log("Applying wall rectangle...");

            ////It's possible that the number of saved wallpapers is greater than the number of walls, so we'll just skip any after we reach the end.
            //if (walls.Count <= whichRoom)
            //{
            //    Logger.Log("Wall rectangle exceeded walls count!  You can ignore this if the farmhouse just upgraded, or you installed a new farmhouse mod.", StardewModdingAPI.LogLevel.Warn);
            //    return false;
            //}

            ////Find the region to paste in
            //Rectangle rectangle = walls[whichRoom];
            //for (int x = rectangle.X; x < rectangle.Right; x++)
            //{
            //    for (int y = rectangle.Y; y < rectangle.Bottom; y++)
            //    {
            //        setMapTileIndexForAnyLayer(map, __instance, x, y, index);
            //    }
            //}
            return false;
        }
    }

    class DecoratableLocation_setFloor_Patch
    {

        internal static bool Prefix(int which, int whichRoom, bool persist, DecoratableLocation __instance)
        {
            //if (!(__instance is FarmHouse))
            //    return true;
            if (!persist)
                return true;
            
            List<Rectangle> floors = __instance.getFloors();

            if (__instance.floor.Count < floors.Count)
            {
                MapUtilities.FacadeHelper.setMissingFloorsToDefault(__instance);
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
                //FarmHouseState state = FarmHouseStates.getState(__instance as FarmHouse);

                OtherLocations.DecoratableState state = OtherLocations.DecoratableStates.getState(__instance);

                if (state.floorDictionary.ContainsKey(floors[whichRoom]))
                {
                    string roomLabel = state.floorDictionary[floors[whichRoom]];
                    Logger.Log("Finding all floors for room '" + roomLabel + "'...");
                    foreach(KeyValuePair<Rectangle, string> floorData in state.floorDictionary)
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

    //This sets the data for the chosen wall to reflect the index of the wallpaper used.
    //This does _not_ set the tiles.
    class DecoratableLocation_setWallpaper_Patch
    {
        internal static bool Prefix(int which, int whichRoom, bool persist, DecoratableLocation __instance)
        {
            
            if (!persist)
                return true;

            List<Rectangle> walls = __instance.getWalls();

            Logger.Log("Checking wallpaper indexes before SetCountAtLeast...");
            for (int wallIndex = 0; wallIndex < __instance.wallPaper.Count; wallIndex++)
            {
                Logger.Log("Wall " + wallIndex + " has a wallpaper index of " + __instance.wallPaper[wallIndex] + ".");
            }

            Logger.Log(__instance.Name + " has " + walls.Count + " walls, and " + __instance.wallPaper.Count + " wallpapers.");
            if (__instance.wallPaper.Count < walls.Count)
            {
                MapUtilities.FacadeHelper.setMissingWallpaperToDefault(__instance);
            }

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
                //FarmHouseState state = FarmHouseStates.getState(__instance as FarmHouse);
                OtherLocations.DecoratableState state = OtherLocations.DecoratableStates.getState(__instance);

                if (state.wallDictionary.ContainsKey(walls[whichRoom]))
                {
                    string roomLabel = state.wallDictionary[walls[whichRoom]];
                    Logger.Log("Finding all walls for room '" + roomLabel + "'...");
                    foreach (KeyValuePair<Rectangle, string> wallData in state.wallDictionary)
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

    class DecoratableLocation_getWalls_Patch
    {
        public static void Postfix(ref List<Rectangle> __result, DecoratableLocation __instance)
        {
            __result.Clear();

            OtherLocations.DecoratableState state = OtherLocations.DecoratableStates.getState(__instance);

            __result = state.getWalls();

            if (__result.Count > 0)
                return;
            else
            {
                __result = new List<Rectangle>()
                {
                    new Rectangle(1, 1, 11, 3)
                };
            }
        }
    }

    class DecoratableLocation_getFloors_Patch
    {
        public static void Postfix(ref List<Rectangle> __result, DecoratableLocation __instance)
        {
            __result.Clear();

            OtherLocations.DecoratableState state = OtherLocations.DecoratableStates.getState(__instance);

            __result = state.getFloors();

            if (__result.Count > 0)
                return;
            else
            {
                __result = new List<Rectangle>()
                {
                    new Rectangle(1, 3, 11, 11)
                };
            }
        }
    }


    class DecoratableLocation_doSetVisibleFloor_Patch
    {

        static void setMapTileIndexesInSquare(Map map, Rectangle floor, DecoratableLocation instance, int x, int y, int index)
        {
            if (floor.Contains(x, y))
                MapUtilities.MapMerger.setMapTileIndexIfOnTileSheet(map, x, y, index, "Back", MapUtilities.SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 21, 16, 10));
            if (floor.Contains(x+1, y))
                MapUtilities.MapMerger.setMapTileIndexIfOnTileSheet(map, x + 1, y, index + 1, "Back", MapUtilities.SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 21, 16, 10));
            if (floor.Contains(x, y+1))
                MapUtilities.MapMerger.setMapTileIndexIfOnTileSheet(map, x, y + 1, index + 16, "Back", MapUtilities.SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 21, 16, 10));
            if (floor.Contains(x+1, y+1))
                MapUtilities.MapMerger.setMapTileIndexIfOnTileSheet(map, x + 1, y + 1, index + 17, "Back", MapUtilities.SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 21, 16, 10));
        }

        internal static bool Prefix(int whichRoom, int which, DecoratableLocation __instance)
        {
            MapUtilities.MapMerger.DoSetVisibleFloor(whichRoom, which, __instance);
            //__instance.updateMap();

            ////Gather a list of all the floors in this map
            //List<Rectangle> floors = __instance.getFloors();
            //int index = 336 + which % 8 * 2 + which / 8 * 32;

            ////Report the index of the floor being pasted.
            //Logger.Log("Chosen index " + index + ".");

            ////Get the map for this DecoratableLocation
            //Map map = __instance.map;


            //Logger.Log("Applying floor rectangle...");

            ////It's possible that the number of saved floors is greater than the number of floors, so we'll just skip any after we reach the end.
            //if (floors.Count <= whichRoom)
            //{
            //    Logger.Log("Floor rectangle exceeded floors count!  You can ignore this if the farmhouse just upgraded, or you installed a new farmhouse mod.", StardewModdingAPI.LogLevel.Warn);
            //    return false;
            //}

            ////Find the region to paste in
            //Rectangle rectangle = floors[whichRoom];
            //int x = rectangle.X;
            //while (x < rectangle.Right)
            //{
            //    int y = rectangle.Y;
            //    while (y < rectangle.Bottom)
            //    {
            //        setMapTileIndexesInSquare(map, rectangle, __instance, x, y, index);
            //        y += 2;
            //    }
            //    x += 2;
            //}
            return false;
        }
    }
}
