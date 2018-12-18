using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;
using xTile.Tiles;

namespace ShaderExample.Framework.Drawers
{
    class Layer
    {

        public static void drawLayer(xTile.Layers.Layer layer, xTile.Display.IDisplayDevice displayDevice, xTile.Dimensions.Rectangle mapViewport, xTile.Dimensions.Location displayOffset, bool wrap, int pixelZoom)
        {
            pixelZoom = (int)Game1.options.zoomLevel;
            int tileWidth = pixelZoom * 16;
            int tileHeight = pixelZoom * 16;
            Location tileInternalOffset = new Location(mapViewport.X*tileWidth, mapViewport.Y*tileHeight);
            int tileXMin = (mapViewport.X >= 0) ? (mapViewport.X / tileWidth) : ((mapViewport.X - tileWidth + 1) / tileWidth);
            int tileYMin = (mapViewport.Y >= 0) ? (mapViewport.Y / tileHeight) : ((mapViewport.Y - tileHeight + 1) / tileHeight);
            if (tileXMin < 0)
            {
                displayOffset.X -= tileXMin * tileWidth;
                tileXMin = 0;
            }
            if (tileYMin < 0)
            {
                displayOffset.Y -= tileYMin * tileHeight;
                tileYMin = 0;
            }
            int tileColumns = 1 + (mapViewport.Size.Width - 1) / tileWidth;
            int tileRows = 1 + (mapViewport.Size.Height - 1) / tileHeight;
            if (tileInternalOffset.X != 0)
            {
                tileColumns++;
            }
            if (tileInternalOffset.Y != 0)
            {
                tileRows++;
            }
            int tileXMax = Math.Min(tileXMin + tileColumns, layer.LayerSize.Width);
            int tileYMax = Math.Min(tileYMin + tileRows, layer.LayerSize.Height);
            Location tileLocation = displayOffset - tileInternalOffset;
            int offset = layer.Id.Equals("Front") ? (16 * pixelZoom) : 0;
            for (int tileY = tileYMin; tileY < tileYMax; tileY++)
            {
                tileLocation.X = displayOffset.X - tileInternalOffset.X;
                for (int tileX = tileXMin; tileX < tileXMax; tileX++)
                {
                    Tile tile = layer.Tiles[tileX, tileY];
                    if (tile != null)
                    {
                        displayDevice.DrawTile(tile, tileLocation, (float)(tileY * (16 * pixelZoom) + 16 * pixelZoom + offset) / 10000f);
                    }
                    tileLocation.X += tileWidth;
                }
                tileLocation.Y += tileHeight;
            }
        }


    }
}
