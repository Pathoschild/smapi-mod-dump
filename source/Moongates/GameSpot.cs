using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moongates
{
    public class GameSpot
    {
        public string LocationName;
        public int TileX;
        public int TileY;

        public GameSpot(string location, int x, int y)
        {
            LocationName = location;
            TileX = x;
            TileY = y;
        }
        public GameSpot() { }
        public GameSpot(GameLocation location, int x, int y) : this(location.Name, x, y) { }
        public GameSpot(string location, Point tileLocation) : this(location, tileLocation.X, tileLocation.Y) { }
        public GameSpot(GameLocation location, Point tileLocation) : this(location.Name, tileLocation.X, tileLocation.Y) { }

        public GameLocation GetGameLocation()
        {
            return Game1.getLocationFromName(LocationName);
        }

        public Point GetTileLocation()
        {
            return new Point(TileX, TileY);
        }

        public new string ToString()
        {
            return LocationName + "." + TileX + "." + TileY;
        }
    }
}
