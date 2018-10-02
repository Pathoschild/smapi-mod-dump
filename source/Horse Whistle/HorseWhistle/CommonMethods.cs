using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using XRectangle = xTile.Dimensions.Rectangle;

namespace HorseWhistle
{
    internal static class CommonMethods
    {
        public static IEnumerable<Vector2> GetVisibleTiles(GameLocation currentLocation, XRectangle viewport)
        {
            const int tileSize = Game1.tileSize;
            var left = viewport.X / tileSize;
            var top = viewport.Y / tileSize;
            var right = (int) Math.Ceiling((viewport.X + viewport.Width) / (decimal) tileSize);
            var bottom = (int) Math.Ceiling((viewport.Y + viewport.Height) / (decimal) tileSize);

            for (var x = left; x < right; x++)
            {
                for (var y = top; y < bottom; y++)
                {
                    if (currentLocation.isTileOnMap(x, y))
                        yield return new Vector2(x, y);
                }
            }
        }
    }
}