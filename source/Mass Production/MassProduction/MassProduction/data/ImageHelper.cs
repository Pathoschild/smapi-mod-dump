/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JacquePott/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MassProduction
{
    /// <summary>
    /// Contains methods for image manipulation.
    /// </summary>
    public class ImageHelper
    {
        /// <summary>
        /// Tints a bitmap image using the given RGB values. Taken from https://softwarebydefault.com/2013/04/12/bitmap-color-tint/
        /// </summary>
        /// <param name="sourceBitmap"></param>
        /// <param name="blueTint"></param>
        /// <param name="greenTint"></param>
        /// <param name="redTint"></param>
        /// <returns></returns>
        public static Bitmap ColorTint(Bitmap sourceBitmap, float blueTint, float greenTint, float redTint)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            sourceBitmap.UnlockBits(sourceData);
            
            float blue = 0;
            float green = 0;
            float red = 0;
            
            for (int k = 0; k + 4 < pixelBuffer.Length; k += 4)
            {
                blue = pixelBuffer[k] + (255 - pixelBuffer[k]) * blueTint;
                green = pixelBuffer[k + 1] + (255 - pixelBuffer[k + 1]) * greenTint;
                red = pixelBuffer[k + 2] + (255 - pixelBuffer[k + 2]) * redTint;
                
                if (blue > 255) { blue = 255; }
                if (green > 255) { green = 255; }
                if (red > 255) { red = 255; }
                
                pixelBuffer[k] = (byte)blue;
                pixelBuffer[k + 1] = (byte)green;
                pixelBuffer[k + 2] = (byte)red;
            }
            
            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);
            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            resultBitmap.UnlockBits(resultData);
            
            return resultBitmap;
        }

        /// <summary>
        /// Converts a Texture2D to a Bitmap.
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static Bitmap ConvertImageFormat(Texture2D texture)
        {
            Bitmap bmp;

            using (MemoryStream ms = new MemoryStream())
            {
                texture.SaveAsPng(ms, texture.Width, texture.Height);
                ms.Seek(0, SeekOrigin.Begin);
                bmp = new Bitmap(ms);
                ms.Dispose();
            }

            return bmp;
        }

        /// <summary>
        /// Converts a Bitmap to a Texture2D.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static Texture2D ConvertImageFormat(Bitmap bitmap)
        {
            Texture2D texture;

            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);
                texture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, ms);
                ms.Dispose();
            }

            return texture;
        }

        /// <summary>
        /// Gets the sprite from the big craftable spritesheet.
        /// </summary>
        /// <param name="spritesheet">The spritesheet as a bitmap.</param>
        /// <param name="bigCraftableId">Item ID of the sprite to get.</param>
        /// <param name="offset">Used for getting alternate frames for animation, ready sprites, etc.</param>
        /// <returns></returns>
        public static Bitmap GetBigCraftableSprite(Bitmap spritesheet, int bigCraftableId, int offset)
        {
            int spriteWidth = 16;
            int spriteHeight = 32;
            Tuple<int, int> coords = GetBigCraftableSpritesheetIndex(spritesheet.Width, spritesheet.Height, bigCraftableId, offset);
            Bitmap sprite = new Bitmap(spriteWidth, spriteHeight);

            for (int x2 = 0; x2 < spriteWidth; x2++)
            {
                int referencePixelX = coords.Item1 + x2;

                for (int y2 = 0; y2 < spriteHeight; y2++)
                {
                    int referencePixelY = coords.Item2 + y2;

                    sprite.SetPixel(x2, y2, spritesheet.GetPixel(referencePixelX, referencePixelY));
                }
            }

            return sprite;
        }

        /// <summary>
        /// Gets the (X, Y) coordinates of the start of this big craftable's sprite in the sheet. 
        /// </summary>
        /// <param name="spritesheetWidth">Width of the spritesheet in pixels.</param>
        /// <param name="spritesheetHeight">Height of the spritesheet in pixels.</param>
        /// <param name="bigCraftableId">ID of the big craftable to get the coordinates for.</param>
        /// <param name="offset">Used for getting alternate sprites for the bug craftable.</param>
        /// <returns>(X, Y)</returns>
        public static Tuple<int, int> GetBigCraftableSpritesheetIndex(int spritesheetWidth, int spritesheetHeight, int bigCraftableId, int offset)
        {
            int spriteWidth = 16;
            int spriteHeight = 32;
            int numCols = spritesheetWidth / spriteWidth;
            int numRows = spritesheetHeight / spriteHeight;
            int spriteIndex = bigCraftableId + offset;

            int column = spriteIndex % numCols;
            int row = spriteIndex / numCols;

            int x = spriteWidth * column;
            int y = spriteHeight * row;

            return new Tuple<int, int>(x, y);
        }

        /// <summary>
        /// Gets the (X, Y) coordinates of the start of this object's sprite in the sheet. 
        /// </summary>
        /// <param name="spritesheetWidth">Width of the spritesheet in pixels.</param>
        /// <param name="spritesheetHeight">Height of the spritesheet in pixels.</param>
        /// <param name="objectId">ID of the object to get the coordinates for.</param>
        /// <returns></returns>
        public static Tuple<int, int> GetObjectSpritesheetIndex(int spritesheetWidth, int spritesheetHeight, int objectId)
        {
            int spriteHeight;
            int spriteWidth = spriteHeight = 16;
            int numCols = spritesheetWidth / spriteWidth;
            int numRows = spritesheetHeight / spriteHeight;

            int column = objectId % numCols;
            int row = objectId / numCols;

            int x = spriteWidth * column;
            int y = spriteHeight * row;

            return new Tuple<int, int>(x, y);
        }
    }
}
