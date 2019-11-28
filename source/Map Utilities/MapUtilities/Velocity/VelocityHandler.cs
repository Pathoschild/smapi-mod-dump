using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Tiles;
using StardewValley;
using xTile.Layers;

namespace MapUtilities.Velocity
{
    public static class VelocityHandler
    {

        public const int Up = 8;
        public const int Down = 9;
        public const int Left = 10;
        public const int Right = 11;

        public const int UpRight = 12;
        public const int UpLeft = 13;
        public const int DownLeft = 14;
        public const int DownRight = 15;

        public static void updateVelocity(Farmer farmer, Tile tile)
        {
            float maxVelocity = 5f;

            List<int> directions = new List<int>();

            if (tile.Properties.ContainsKey("Speed"))
            {
                try
                {
                    maxVelocity = Convert.ToSingle(tile.Properties["Speed"].ToString());
                }
                catch (FormatException)
                {
                    Logger.log("Speed '" + tile.Properties["Speed"].ToString() + "' does not seem to be a numerical value!  Using default...", StardewModdingAPI.LogLevel.Warn);
                }
            }
            switch (tile.TileIndex)
            {
                case Up:
                    directions.Add(Up);
                    break;
                case Down:
                    directions.Add(Down);
                    break;
                case Left:
                    directions.Add(Left);
                    break;
                case Right:
                    directions.Add(Right);
                    break;
                case UpRight:
                    directions.Add(Up);
                    directions.Add(Right);
                    break;
                case UpLeft:
                    directions.Add(Up);
                    directions.Add(Left);
                    break;
                case DownLeft:
                    directions.Add(Down);
                    directions.Add(Left);
                    break;
                case DownRight:
                    directions.Add(Down);
                    directions.Add(Right);
                    break;
                default:
                    break;
            }

            foreach(int direction in directions)
            {
                if(direction == Up)
                {
                    farmer.yVelocity = maxVelocity;
                }
                else if (direction == Down)
                {
                    farmer.yVelocity = -maxVelocity;
                }
                else if (direction == Left)
                {
                    farmer.xVelocity = -maxVelocity;
                }
                else if (direction == Right)
                {
                    farmer.xVelocity = maxVelocity;
                }
            }


            Layer slope = farmer.currentLocation.map.GetLayer("Slope" + Pseudo3D.LevelHandler.getLevelSuffixForCharacter(farmer));
            if (slope != null && slope.Tiles[farmer.getTileX(), farmer.getTileY()] != null)
            {
                Slope.SlopeHandler.modifyVelocity(farmer, slope.Tiles[farmer.getTileX(), farmer.getTileY()], farmer.xVelocity, farmer.yVelocity);
            }
        }
    }
}
