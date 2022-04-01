/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.Collections.Generic;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace AprilFools
{
    public partial class ModEntry
    {
        private string[] _AsciiChars = { "#", "#", "@", "%", "=", "+", "*", ":", "-", ".", " " };
        public static void ShuffleList<T>(List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Game1.random.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        private Color[] ScaleScreen(int scaleFactor)
        {
            var screenWidth = Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
            var screenHeight = Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
            Color[] screenData = new Color[screenWidth * screenHeight];
            Game1.graphics.GraphicsDevice.GetBackBufferData(screenData);
            if(screenTexture.Width != screenWidth || screenTexture.Height != screenHeight)
            {
                screenTexture = new Texture2D(Game1.graphics.GraphicsDevice, screenWidth, screenHeight);
            }
            screenTexture.SetData(screenData);
            RenderTarget2D renderTarget = new RenderTarget2D(Game1.graphics.GraphicsDevice, screenWidth / scaleFactor, screenHeight / scaleFactor);

            Rectangle destinationRectangle = new Rectangle(0, 0, screenWidth / scaleFactor, screenHeight / scaleFactor);

            Game1.graphics.GraphicsDevice.SetRenderTarget(renderTarget);

            screenBatch.Begin();
            screenBatch.Draw(screenTexture, destinationRectangle, Color.White);
            screenBatch.End();

            Game1.graphics.GraphicsDevice.SetRenderTarget(null);
            Color[] data = new Color[screenWidth / scaleFactor * screenHeight / scaleFactor];
            renderTarget.GetData(data);
            return data;
        }
        private List<string> ConvertToAscii(Color[] data, int width)
        {
            List<string> output = new List<string>();
            string line = "";
            for (int i = 0; i < data.Length; i++)
            {
                Color pixelColor = data[i];
                //Average out the RGB components to find the Gray Color
                int red = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                int green = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                int blue = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                Color grayColor = new Color(red, green, blue);
                int index = (grayColor.R * 10) / 255;
                line += " ";
                line += _AsciiChars[index];
                if (line.Length == width * 2)
                {
                    output.Add(line);
                    line = "";
                }
            }
            return output;
        }
    }
}