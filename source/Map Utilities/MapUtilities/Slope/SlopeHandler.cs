using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using xTile.Tiles;

namespace MapUtilities.Slope
{
    public static class SlopeHandler
    {

        public static void modifyVelocity(Farmer player, Tile tile, float xVelocity = 0f, float yVelocity = 0f)
        {

            int slope = (int)Math.Floor(tile.TileIndex / 2f) + 1;
            bool upRight = (tile.TileIndex % 2) == 1;
            bool playerAscend = (upRight && player.movementDirections.Contains(1)) || (!upRight && player.movementDirections.Contains(3)) || (upRight && Math.Abs(xVelocity) > 0.01f && xVelocity > 0f) || (!upRight && Math.Abs(xVelocity) > 0.01f && xVelocity < 0f);

            //Move directions: 1 is Right, 3 is Left.
            if (Math.Abs(xVelocity) < 0.01f && (player.movementDirections.Contains(1) || player.movementDirections.Contains(3)))
            {
                double yOffset = player.getMovementSpeed() / slope;

                if (!player.currentLocation.isCollidingPosition(slopePosition(player, yOffset, playerAscend), Game1.viewport, true, -1, false, (Character)player))
                {
                    player.position.Y += playerAscend ? (float)yOffset * -1 : (float)yOffset;
                }
                else if(!player.currentLocation.isCollidingPosition(slopePositionHalf(player, yOffset, playerAscend), Game1.viewport, true, -1, false, (Character)player))
                {
                    player.position.Y += (float)yOffset * (playerAscend ? -0.5f : 0.5f);
                }
            }
            else if (Math.Abs(xVelocity) > 0.01f)
            {
                //Logger.log("Modifying player velocity!");
                player.yVelocity = Math.Abs(xVelocity) / slope * (playerAscend ? 1 : -1);
            }
        }

        public static Microsoft.Xna.Framework.Rectangle slopePosition(Farmer player, double yOffset, bool playerAscend)
        {
            Microsoft.Xna.Framework.Rectangle boundingBox = player.GetBoundingBox();

            if (player.movementDirections.Contains(1))
            {
                boundingBox.X += (int)Math.Ceiling((double)player.getMovementSpeed());
                boundingBox.Y += (int)Math.Ceiling(yOffset) * (playerAscend ? -1 : 1);
            }
            else if (player.movementDirections.Contains(3))
            {
                boundingBox.X -= (int)Math.Ceiling((double)player.getMovementSpeed());
                boundingBox.Y += (int)Math.Ceiling(yOffset) * (playerAscend ? -1 : 1);
            }
            return boundingBox;
        }

        public static Microsoft.Xna.Framework.Rectangle slopePositionHalf(Farmer player, double yOffset, bool playerAscend)
        {
            Microsoft.Xna.Framework.Rectangle boundingBox = player.GetBoundingBox();

            if (player.movementDirections.Contains(1))
            {
                boundingBox.X += (int)Math.Ceiling((double)player.getMovementSpeed());
                boundingBox.Y += (int)(Math.Ceiling(yOffset) * (playerAscend ? -0.5 : 0.5));
            }
            else if (player.movementDirections.Contains(3))
            {
                boundingBox.X -= (int)Math.Ceiling((double)player.getMovementSpeed());
                boundingBox.Y += (int)Math.Ceiling((yOffset) * (playerAscend ? -0.5 : 0.5));
            }
            return boundingBox;
        }
    }
}
