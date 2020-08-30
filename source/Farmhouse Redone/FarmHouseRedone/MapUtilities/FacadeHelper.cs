using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using xTile;

namespace FarmHouseRedone.MapUtilities
{
    public static class FacadeHelper
    {
        //Walls

        public static void setMissingWallpaperToDefault(StardewValley.Locations.DecoratableLocation location)
        {
            List<Rectangle> walls = location.getWalls();
            Logger.Log(location.Name + " has " + walls.Count + " walls, and " + location.wallPaper.Count + " wallpapers.");
            while (location.wallPaper.Count < walls.Count)
            {
                location.wallPaper[location.wallPaper.Count] = getWallpaperIndex(location.map, walls[location.wallPaper.Count].X, walls[location.wallPaper.Count].Y);
                Logger.Log("Added wall with wallpaper " + location.wallPaper[location.wallPaper.Count - 1]);
            }
            Logger.Log(location.Name + " now has " + walls.Count + " walls, and " + location.wallPaper.Count + " wallpapers.");
        }

        public static void setWallpaperDefaults(StardewValley.Locations.DecoratableLocation location)
        {
            List<Rectangle> walls = location.getWalls();
            List<Rectangle> floors = location.getFloors();
            Logger.Log("Setting defaults for " + location.name + " (" + location.uniqueName + ")");
            for (int wallIndex = 0; wallIndex < walls.Count; wallIndex++)
            {
                Logger.Log("Setting default wall for the wall " + walls[wallIndex].ToString() + "...");
                int wallPaperIndex = getWallpaperIndex(location.map, walls[wallIndex].X, walls[wallIndex].Y);
                location.wallPaper[wallIndex] = wallPaperIndex;
                //house.setWallpaper(wallPaperIndex, wallIndex, true);
            }
            for(int floorIndex = 0; floorIndex < floors.Count; floorIndex++)
            {
                Logger.Log("Setting default floor for the floor " + floors[floorIndex].ToString() + "...");
                int floorTileIndex = getFloorIndex(location.map, floors[floorIndex].X, floors[floorIndex].Y);
                location.floor[floorIndex] = floorTileIndex;
            }
        }

        public static int getWallpaperIndex(Map map, int x, int y)
        {
            int wallIndex = getWallSpriteIndex(map, x, y);
            if (wallIndex == -1)
            {
                Logger.Log("Could not find any wall tile on any layer at (" + x + ", " + y + ")");
                return 0;
            }
            int wallPaperX = wallIndex % 16;
            int wallPaperY = wallIndex / 48;
            int wallPaperIndex = (wallPaperY * 16) + wallPaperX;
            Logger.Log("Found wallpaper index of " + wallPaperIndex + " for tilesheet index " + wallIndex + ".");
            return wallPaperIndex;
        }

        public static int getWallSpriteIndex(Map map, int x, int y)
        {
            int index = -1;
            if(SheetHelper.isTileOnSheet(map, "Back", x, y, SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 0, 16, 21)))
            {
                index = map.GetLayer("Back").Tiles[x, y].TileIndex;
            }
            else if (SheetHelper.isTileOnSheet(map, "Buildings", x, y, SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 0, 16, 21)))
            {
                index = map.GetLayer("Buildings").Tiles[x, y].TileIndex;
            }
            else if (SheetHelper.isTileOnSheet(map, "Front", x, y, SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 0, 16, 21)))
            {
                index = map.GetLayer("Front").Tiles[x, y].TileIndex;
            }
            return index;
        }


        //Floors

        public static void setMissingFloorsToDefault(StardewValley.Locations.DecoratableLocation location)
        {
            List<Rectangle> floors = location.getFloors();
            while (location.floor.Count < floors.Count)
            {
                location.floor[location.floor.Count] = getFloorIndex(location.map, floors[location.floor.Count].X, floors[location.floor.Count].Y);
            }
        }

        public static int getFloorIndex(Map map, int x, int y)
        {
            int floorIndex = getFloorSpriteIndex(map, x, y);
            if (floorIndex == -1)
            {
                Logger.Log("Could not find any floor tile on any layer at (" + x + ", " + y + ")");
                return 0;
            }
            floorIndex -= 336;
            int floorX = (floorIndex / 2) % 8;
            int floorY = floorIndex / 32;
            int floor = (floorY * 8) + floorX;
            Logger.Log("Found floor index of " + floor + " for tilesheet index " + floorIndex + ".");
            return floor;
        }

        public static int getFloorSpriteIndex(Map map, int x, int y)
        {
            int index = -1;
            if (SheetHelper.isTileOnSheet(map, "Back", x, y, SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 21, 16, 20)))
            {
                index = map.GetLayer("Back").Tiles[x, y].TileIndex;
            }
            return index;
        }
    }
}
