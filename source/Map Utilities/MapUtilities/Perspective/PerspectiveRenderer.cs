using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using xTile;
using xTile.Layers;
using xTile.Tiles;
using xTile.Display;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapUtilities.Perspective
{
    public static class PerspectiveRenderer
    {
        public static Texture2D baseLayer;
        public static Dictionary<TileSheet, Bitmap> sheets;

        public static void init()
        {
            sheets = new Dictionary<TileSheet, Bitmap>();
        }

        public static void draw(SpriteBatch b)
        {
            if (baseLayer == null)
                return;
            b.Draw(baseLayer, new Microsoft.Xna.Framework.Rectangle(0, 0, baseLayer.Width, baseLayer.Height), Microsoft.Xna.Framework.Color.White);
        }

        public static void makeMinimap(GameLocation location)
        {
            Logger.log("Making a test minimap of " + location.name);
            prepareMap(location.map);
            Logger.log("Prepared tilesheets.  Making full image...");
            baseLayer = textureFromBitmap(makeImageFromLayer(location.map.GetLayer("Back")));
        }

        public static Bitmap bitmapFromTexture2D(Texture2D tex)
        {
            MemoryStream memoryStream = new MemoryStream();
            tex.SaveAsPng(memoryStream, tex.Width, tex.Height);
            Bitmap outMap = new Bitmap(memoryStream);
            Logger.log("Resulting bitmap: " + outMap.Width + " x " + outMap.Height + "; source: " + tex.Width + " x " + tex.Height);
            return new Bitmap(memoryStream);
        }

        public static Texture2D textureFromBitmap(Bitmap b)
        {
            MemoryStream memoryStream = new MemoryStream();
            b.Save(memoryStream, ImageFormat.Png);
            Texture2D outTexture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, memoryStream);
            Logger.log("Resulting texture: " + outTexture.Width + " x " + outTexture.Height + "; source: " + b.Width + " x " + b.Height);
            return outTexture;
        }

        public static void prepareMap(Map map)
        {
            sheets.Clear();
            for(int sheetID = 0; sheetID < map.TileSheets.Count; sheetID++)
            {
                TileSheet sheet = map.TileSheets[sheetID];
                Texture2D sheetTexture = Loader.load<Texture2D>(sheet.ImageSource) as Texture2D;
                Logger.log("Creating bitmap for tilesheet " + sheet.Id + " (" + sheet.ImageSource + ")...");
                sheets[sheet] = bitmapFromTexture2D(sheetTexture);
            }
        }

        public static Bitmap makeImageFromLayer(Layer layer)
        {
            Bitmap outMap = new Bitmap(layer.LayerWidth * 16, layer.LayerHeight * 16);
            Logger.log("Test minimap dimensions: " + outMap.Width + " x " + outMap.Height);
            Graphics map = Graphics.FromImage(outMap);
            Logger.log("Processing tiles...");
            for(int x = 0; x < layer.LayerWidth; x++)
            {
                for(int y = 0; y < layer.LayerHeight; y++)
                {
                    //Logger.log("Rendering tile [" + x + ", " + y + "]...");
                    renderTile(map, layer.Tiles[x, y], x, y, 16, 16);
                }
            }
            Logger.log("Finished processing tiles.");
            return outMap;
        }

        public static void renderTile(Graphics g, Tile tile, int x, int y, int tileWidth, int tileHeight)
        {
            if (tile == null)
                return;
            System.Drawing.Rectangle tileImageBounds = convertRectangle(tile.TileSheet.GetTileImageBounds(tile.TileIndex));
            //Logger.log("Rendering sheet section " + tileImageBounds.ToString() + " at [" + x * tileWidth + ", " + y * tileWidth + "]");
            Bitmap tileImage = sheets[tile.TileSheet].Clone(tileImageBounds, sheets[tile.TileSheet].PixelFormat);
            g.DrawImage(tileImage, new PointF(x * tileWidth, y * tileHeight));
        }

        public static System.Drawing.Rectangle convertRectangle(xTile.Dimensions.Rectangle inRect)
        {
            return new System.Drawing.Rectangle(inRect.X, inRect.Y, inRect.Width, inRect.Height);
        }
    }
}
