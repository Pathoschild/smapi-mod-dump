using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bwdyworks.API
{
    public class Locations
    {
        public Point FindPathableAndClearTile(GameLocation l, Point p, int radius = 0)
        {
            Point randomPoint = Point.Zero;
            for (int i = 0; i < 100; i++)
            {
                int sizeX = l.map.GetLayer("Back").TileWidth;
                int sizeY = l.map.GetLayer("Back").TileHeight;
                if (radius > 0)
                    randomPoint = new Point((p.X - radius) + Modworks.RNG.Next(radius * 2), (p.Y - radius) + Modworks.RNG.Next(radius * 2));
                else
                    randomPoint = new Point(Modworks.RNG.Next(sizeX), Modworks.RNG.Next(sizeY));
                if (IsTilePathableAndClear(l, randomPoint.X, randomPoint.Y)) return randomPoint;
            }
            return Point.Zero;
        }

        public bool IsTilePathableAndClear(GameLocation l, Point xy)
        {
            return IsTilePathableAndClear(l, xy.X, xy.Y);
        }

        public bool IsTilePathableAndClear(GameLocation l, int x, int y)
        {
            xTile.Dimensions.Location tileLoc = new xTile.Dimensions.Location(x, y);
            //is solid?
            if (!l.isTilePassable(tileLoc, Game1.viewport)) return false;
            //is clear and placeable?
            if (!l.isTileLocationTotallyClearAndPlaceable(x, y)) return false;
            //is shadow tile? (out of bounds on interior maps, etc)
            xTile.ObjectModel.PropertyValue shadowProp;
            if(l.map.GetLayer("Back").Tiles[tileLoc].TileIndexProperties.TryGetValue("Shadow", out shadowProp)) return false;
            //is it a water tile?
            if(!string.IsNullOrWhiteSpace(l.doesTileHaveProperty(x, y, "Water", "Back"))) return false;
            //is it a wall tile?
            if (!string.IsNullOrWhiteSpace(l.doesTileHaveProperty(x, y, "NoFurniture", "Back"))) return false;
            //is it an NPC barrier tile?
            if (!string.IsNullOrWhiteSpace(l.doesTileHaveProperty(x, y, "NPCBarrier", "Back"))) return false;
            return true;
        }

    }
}
