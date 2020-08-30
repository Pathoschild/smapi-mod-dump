using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;
using xTile.Tiles;
using xTile.ObjectModel;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewModdingAPI;

namespace FarmHouseRedone.MapUtilities
{
    public static class MapMerger
    {
        public const int PASTE_DESTRUCTIVE = 0;
        public const int PASTE_NONDESTRUCTIVE = 1;
        public const int PASTE_PRESERVE_FLOORS = 2;

        public static void pasteMap(GameLocation location, Map map, int pasteX, int pasteY, int pasteMode = PASTE_NONDESTRUCTIVE)
        {
            Dictionary<TileSheet, TileSheet> equivalentSheets = MapUtilities.SheetHelper.getEquivalentSheets(location, map);
            Vector2 mapSize = getMapSize(map);
            Vector2 hostMapSize = getMapSize(location.map);
            for (int x = 0; x < mapSize.X && pasteX + x < hostMapSize.X; x++)
            {
                for (int y = 0; y < mapSize.Y && pasteY + y < hostMapSize.Y; y++)
                {
                    pasteTile(location.map, map, x, y, pasteX + x, pasteY + y, equivalentSheets, pasteMode);
                }
            }
            foreach (KeyValuePair<string, PropertyValue> pair in map.Properties)
            {
                PropertyValue value = pair.Value;
                string propertyName = pair.Key;
                Logger.Log("Map section contained Properties data, applying now...");
                if (propertyName.Equals("Warp"))
                {
                    //propertyName = "Warp";
                    Logger.Log("Adjusting warp property...");
                    string[] warpParts = Utility.cleanup(value.ToString()).Split(' ');
                    string warpShifted = "";
                    for (int index = 0; index < warpParts.Length; index += 5)
                    {
                        try
                        {
                            Logger.Log("Relative warp found: " + warpParts[index + 0] + " " + warpParts[index + 1] + " " + warpParts[index + 2] + " " + warpParts[index + 3] + " " + warpParts[index + 4]);
                            int xi = -1;
                            int yi = -1;
                            int xii = -1;
                            int yii = -1;
                            if (warpParts[0].StartsWith("~"))
                                xi = pasteX + Convert.ToInt32(warpParts[index + 0].TrimStart('~'));
                            else
                                xi = Convert.ToInt32(warpParts[0]);
                            if (warpParts[1].StartsWith("~"))
                                yi = pasteY + Convert.ToInt32(warpParts[index + 1].TrimStart('~'));
                            else
                                yi = Convert.ToInt32(warpParts[1]);
                            if (warpParts[3].StartsWith("~"))
                                xii = pasteX + Convert.ToInt32(warpParts[index + 3].TrimStart('~'));
                            else
                                xii = Convert.ToInt32(warpParts[3]);
                            if (warpParts[4].StartsWith("~"))
                                yii = pasteY + Convert.ToInt32(warpParts[index + 4].TrimStart('~'));
                            else
                                yii = Convert.ToInt32(warpParts[4]);
                            string returnWarp = xi + " " + yi + " " + warpParts[index + 2] + " " + xii + " " + yii + " ";
                            Logger.Log("Relative warp became " + returnWarp);
                            warpShifted += returnWarp;
                        }
                        catch (IndexOutOfRangeException)
                        {
                            Logger.Log("Incomplete warp definition found!  Please ensure all warp definitions are formatted as 'X Y map X Y'", LogLevel.Warn);
                        }
                        catch (FormatException)
                        {
                            Logger.Log("Invalid warp definition found!  Please ensure all warp definitions use numbers fro the X and Y coordinates!", LogLevel.Warn);
                        }
                    }
                    value = warpShifted.Trim(' ');
                    Logger.Log("Warp property is now " + value.ToString());
                }
                if (!location.map.Properties.ContainsKey(propertyName))
                {
                    Logger.Log("Host map did not have a '" + propertyName + "' property, setting it to '" + propertyName + "'...");
                    location.map.Properties.Add(propertyName, value);
                }
                else
                {
                    PropertyValue houseValue = location.map.Properties[propertyName];
                    Logger.Log("Host map already had a '" + propertyName + "' value, appending...");
                    location.map.Properties[propertyName] = (houseValue.ToString() + " " + value.ToString()).Trim(' ');
                    Logger.Log(propertyName + " is now " + location.map.Properties[propertyName].ToString());
                }
            }
        }

        public static void pasteTile(Map map, Map sectionMap, int x, int y, int mapX, int mapY, Dictionary<TileSheet, TileSheet> equivalentSheets, int pasteMode = PASTE_NONDESTRUCTIVE)
        {
            pasteTileInLayer(map, sectionMap, x, y, mapX, mapY, "Back", equivalentSheets, pasteMode == 0);
            pasteTileInLayer(map, sectionMap, x, y, mapX, mapY, "Buildings", equivalentSheets, pasteMode == 0 || pasteMode == 2);
            pasteTileInLayer(map, sectionMap, x, y, mapX, mapY, "Front", equivalentSheets, pasteMode == 0 || pasteMode == 2);
            pasteTileInLayer(map, sectionMap, x, y, mapX, mapY, "AlwaysFront", equivalentSheets, pasteMode == 0 || pasteMode == 2);
        }

        public static void pasteTileInLayer(Map map, Map sectionMap, int x, int y, int mapX, int mapY, string layer, Dictionary<TileSheet, TileSheet> equivalentSheets, bool destructive)
        {
            if (sectionMap.GetLayer(layer) == null)
                return;
            if (sectionMap.GetLayer(layer).Tiles[x, y] != null)
            {
                Tile sectionTile = sectionMap.GetLayer(layer).Tiles[x, y];
                if (sectionTile is AnimatedTile)
                {
                    int framesCount = (sectionTile as AnimatedTile).TileFrames.Length;
                    StaticTile[] frames = new StaticTile[framesCount];
                    for (int i = 0; i < framesCount; i++)
                    {
                        StaticTile frame = (sectionTile as AnimatedTile).TileFrames[i];
                        frames[i] = new StaticTile(map.GetLayer(layer), equivalentSheets[sectionTile.TileSheet], frame.BlendMode, frame.TileIndex);
                    }
                    map.GetLayer(layer).Tiles[mapX, mapY] = new AnimatedTile(map.GetLayer(layer), frames, (sectionTile as AnimatedTile).FrameInterval);
                }
                else
                {
                    map.GetLayer(layer).Tiles[mapX, mapY] = new StaticTile(map.GetLayer(layer), equivalentSheets[sectionTile.TileSheet], sectionTile.BlendMode, sectionTile.TileIndex);
                }
                if (sectionTile.Properties.Keys.Count > 0)
                {
                    foreach (KeyValuePair<string, PropertyValue> pair in sectionTile.Properties)
                    {
                        map.GetLayer(layer).Tiles[mapX, mapY].Properties[pair.Key] = pair.Value;
                    }
                }
            }
            else if (destructive)
            {
                map.GetLayer(layer).Tiles[mapX, mapY] = null;
            }
        }

        public static void setMapTileIndexIfOnTileSheet(Map map, int x, int y, int index, string layer, int tileSheetToMatch, Rectangle region)
        {
            if(SheetHelper.isTileOnSheet(map, layer, x, y, tileSheetToMatch, region))
            {
                map.GetLayer(layer).Tiles[x, y].TileIndex = index;
            }
        }

        public static void setMapTileIndexIfOnTileSheet(Map map, int x, int y, int index, string layer, int tileSheetToMatch)
        {
            if(SheetHelper.isTileOnSheet(map, layer, x, y, tileSheetToMatch))
            {
                map.GetLayer(layer).Tiles[x, y].TileIndex = index;
            }
        }

        private static Vector2 getMapSize(Map map)
        {
            return new Vector2(map.Layers[0].LayerWidth, map.Layers[0].LayerHeight);
        }



        internal static int getWallIndex(Map map, int x, int y, string layer, int destinationIndex)
        {
            if (map.GetLayer(layer).Tiles[x, y] == null)
                return -1;
            int currentIndex = map.GetLayer(layer).Tiles[x, y].TileIndex;
            int whichHeight = (currentIndex % 48) / 16;
            return destinationIndex + (whichHeight * 16);
        }

        internal static void setWallMapTileIndexForAnyLayer(Map map, int x, int y, int index)
        {
            setMapTileIndexIfOnTileSheet(map, x, y, getWallIndex(map, x, y, "Back", index), "Back", SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 0, 16, 21));
            setMapTileIndexIfOnTileSheet(map, x, y, getWallIndex(map, x, y, "Buildings", index), "Buildings", SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 0, 16, 21));
            setMapTileIndexIfOnTileSheet(map, x, y, getWallIndex(map, x, y, "Front", index), "Front", SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 0, 16, 21));
        }

        public static void DoSetVisibleWallpaper(int whichRoom, int which, DecoratableLocation location)
        {
            GameLocation client = OtherLocations.FakeDecor.FakeDecorHandler.getClient(location);
            if (client != null)
            {
                client.updateMap();
                Logger.Log("Found client map to apply wallpaper to: " + client.Name);
            }
            else
            {
                Logger.Log("No client for " + location.Name);
            }

            location.updateMap();

            //Gather a list of all the walls in this map
            List<Rectangle> walls = location.getWalls();
            int index = which % 16 + which / 16 * 48;

            //Report the index of the wallpaper being pasted.
            Logger.Log("Chosen index " + index + ".");

            //Get the map for this DecoratableLocation
            Map map = location.map;


            Logger.Log("Applying wall rectangle...");

            //It's possible that the number of saved wallpapers is greater than the number of walls, so we'll just skip any after we reach the end.
            if (walls.Count <= whichRoom)
            {
                Logger.Log("Wall rectangle exceeded walls count!  You can ignore this if the farmhouse just upgraded, or you installed a new farmhouse mod.", StardewModdingAPI.LogLevel.Warn);
                return;
            }

            //Find the region to paste in
            Rectangle rectangle = walls[whichRoom];
            for (int x = rectangle.X; x < rectangle.Right; x++)
            {
                for (int y = rectangle.Y; y < rectangle.Bottom; y++)
                {
                    setWallMapTileIndexForAnyLayer(map, x, y, index);
                    if (client != null)
                        setWallMapTileIndexForAnyLayer(client.map, x, y, index);
                }
            }
            return;
        }

        internal static int getFloorIndex(Map map, int x, int y, string layer, int destinationIndex)
        {
            if (map.GetLayer(layer).Tiles[x, y] == null)
                return -1;
            int currentIndex = map.GetLayer(layer).Tiles[x, y].TileIndex;

            int horizontalOffset = currentIndex % 2;
            int verticalOffset = ((currentIndex - 336) / 16) % 2;

            return destinationIndex + horizontalOffset + (verticalOffset * 16);

            //TODO: this math is wrong - it's for walls
            //I'm very sleepy right now and can't do math
            //int whichHeight = (currentIndex % 48) / 16;
            //return destinationIndex + (whichHeight * 16);
        }

        internal static void setFloorMapTileIndexForAnyLayer(Map map, int x, int y, int index)
        {
            setMapTileIndexIfOnTileSheet(map, x, y, getFloorIndex(map, x, y, "Back", index), "Back", SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 21, 16, 10));
            setMapTileIndexIfOnTileSheet(map, x, y, getFloorIndex(map, x, y, "Buildings", index), "Buildings", SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 21, 16, 10));
            setMapTileIndexIfOnTileSheet(map, x, y, getFloorIndex(map, x, y, "Front", index), "Front", SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 21, 16, 10));
        }

        internal static void setMapTileIndexesInSquare(Map map, Rectangle floor, int x, int y, int index)
        {
            if (floor.Contains(x, y))
                setMapTileIndexIfOnTileSheet(map, x, y, index, "Back", SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 21, 16, 10));
            if (floor.Contains(x + 1, y))
                setMapTileIndexIfOnTileSheet(map, x + 1, y, index + 1, "Back", SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 21, 16, 10));
            if (floor.Contains(x, y + 1))
                setMapTileIndexIfOnTileSheet(map, x, y + 1, index + 16, "Back", SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 21, 16, 10));
            if (floor.Contains(x + 1, y + 1))
                setMapTileIndexIfOnTileSheet(map, x + 1, y + 1, index + 17, "Back", SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 21, 16, 10));
        }

        internal static void DoSetVisibleFloor(int whichRoom, int which, DecoratableLocation location)
        {
            GameLocation client = OtherLocations.FakeDecor.FakeDecorHandler.getClient(location);
            if (client != null)
                client.updateMap();

            location.updateMap();

            //Gather a list of all the floors in this map
            List<Rectangle> floors = location.getFloors();
            int index = 336 + which % 8 * 2 + which / 8 * 32;

            //Report the index of the floor being pasted.
            Logger.Log("Chosen index " + index + ".");

            //Get the map for this DecoratableLocation
            Map map = location.map;


            Logger.Log("Applying floor rectangle...");

            //It's possible that the number of saved floors is greater than the number of floors, so we'll just skip any after we reach the end.
            if (floors.Count <= whichRoom)
            {
                Logger.Log("Floor rectangle exceeded floors count!  You can ignore this if the farmhouse just upgraded, or you installed a new farmhouse mod.", StardewModdingAPI.LogLevel.Warn);
                return;
            }

            //Find the region to paste in
            Rectangle rectangle = floors[whichRoom];

            for (int x = rectangle.X; x < rectangle.Right; x++)
            {
                for (int y = rectangle.Y; y < rectangle.Bottom; y++)
                {
                    setFloorMapTileIndexForAnyLayer(map, x, y, index);
                    if (client != null)
                        setFloorMapTileIndexForAnyLayer(client.map, x, y, index);
                }
            }

            //int x = rectangle.X;
            //while (x < rectangle.Right)
            //{
            //    int y = rectangle.Y;
            //    while (y < rectangle.Bottom)
            //    {
            //        setMapTileIndexesInSquare(map, rectangle, x, y, index);
            //        if (client != null)
            //            setMapTileIndexesInSquare(client.map, rectangle, x, y, index);
            //        y += 2;
            //    }
            //    x += 2;
            //}
            return;
        }
    }
}
