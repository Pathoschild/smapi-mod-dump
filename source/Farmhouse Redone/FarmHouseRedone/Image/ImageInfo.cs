/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mjSurber/FarmHouseRedone
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FarmHouseRedone.Image
{
    public class ImageInfo
    {
        public int Width;
        public int Height;
        public Color[] colorInfo;

        public ImageInfo R
        {
            get
            {
                return this.channel(0, false);
            }
        }

        public ImageInfo G
        {
            get
            {
                return this.channel(1, false);
            }
        }

        public ImageInfo B
        {
            get
            {
                return this.channel(2, false);
            }
        }

        public ImageInfo(Texture2D source)
        {
            colorInfo = new Color[source.Width * source.Height];
            Width = source.Width;
            Height = source.Height;
            for (int x = 0; x < source.Width; x++)
            {
                for (int y = 0; y < source.Height; y++)
                {
                    colorInfo[x + (y * source.Width)] = getPixel(source, x, y);
                }
            }

        }

        public ImageInfo(Color[] data, int width, int height)
        {
            Width = width;
            Height = height;
            colorInfo = data;
        }

        public ImageInfo(Color background, int width, int height)
        {
            Width = width;
            Height = height;
            colorInfo = new Color[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    colorInfo[x + (y * width)] = background;
                }
            }
        }

        public Texture2D render()
        {
            Texture2D outTexture = new Texture2D(Game1.graphics.GraphicsDevice, this.Width, this.Height, true, SurfaceFormat.Color);
            outTexture.SetData(this.colorInfo);
            return outTexture;
        }

        public static Color getPixel(Texture2D texture, int x, int y)
        {
            x = (x + texture.Width) % texture.Width;
            y = (y + texture.Height) % texture.Height;

            byte[] buffer = new byte[4];
            try
            {
                texture.GetData<byte>(0, new Rectangle(x, y, 1, 1), buffer, 0, 4);

                Color outColor = new Color((int)(buffer[0]), (int)(buffer[1]), (int)(buffer[2]), (int)(buffer[3]));
                return outColor;
            }
            catch (ArgumentException)
            {
                Logger.Log("Rectangle (" + x + ", " + y + ", 1, 1) was invalid!", StardewModdingAPI.LogLevel.Error);
                return Color.Black;
            }
        }

        public Color getPixel(int x, int y)
        {
            x = (x + this.Width) % this.Width;
            y = (y + this.Height) % this.Height;

            return this.colorInfo[x + (y * this.Width)];
        }

        public static Color getPixel(ImageInfo image, int x, int y)
        {
            x = (x + image.Width) % image.Width;
            y = (y + image.Height) % image.Height;

            return image.colorInfo[x + (y * image.Width)];
        }

        public void setPixel(int x, int y, Color c)
        {
            x = (x + this.Width) % this.Width;
            y = (y + this.Height) % this.Height;

            this.colorInfo[x + (y * this.Width)] = c;
        }

        public ImageInfo mirror(bool horizontal)
        {
            int width = horizontal ? this.Width * 2 : this.Width;
            int height = horizontal ? this.Height : this.Height * 2;

            Color[] data = new Color[width * height];

            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    Color pixel = getPixel(this, x, y);
                    if (horizontal)
                    {
                        data[x + (y * width)] = pixel;
                        data[((width - 1) - x) + (y * width)] = pixel;
                    }
                    else
                    {
                        data[x + (y * width)] = pixel;
                        data[x + (((height - 1) - y) * width)] = pixel;
                    }
                }
            }
            return new ImageInfo(data, width, height);
        }

        public ImageInfo multiply(ImageInfo b, float factor)
        {
            float xScaleFactor = b.Width / this.Width;
            float yScaleFactor = b.Height / this.Height;

            Color[] outData = new Color[this.Width * this.Height];

            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    Color aPixel = getPixel(this, x, y);
                    Color bPixel = getPixel(b, (int)(x * xScaleFactor), (int)(y * yScaleFactor));

                    //float aAverage = (int)((aPixel.R + aPixel.G + aPixel.B) / 3f) / 256f;
                    //float bAverage = (int)((bPixel.R + bPixel.G + bPixel.B) / 3f) / 256f;

                    //int multiplied = (int)(aAverage * bAverage * 256);

                    int rM = (int)((aPixel.R / 256f) * (bPixel.R / 256f) * 256f);
                    int gM = (int)((aPixel.G / 256f) * (bPixel.G / 256f) * 256f);
                    int bM = (int)((aPixel.B / 256f) * (bPixel.B / 256f) * 256f);

                    outData[x + (y * this.Width)] = new Color(rM, gM, bM, 255);
                }
            }
            return new ImageInfo(outData, this.Width, this.Height);
        }

        public ImageInfo divide(ImageInfo b)
        {
            float xScaleFactor = b.Width / this.Width;
            float yScaleFactor = b.Height / this.Height;

            Color[] outData = new Color[this.Width * this.Height];

            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    Color aPixel = getPixel(this, x, y);
                    Color bPixel = getPixel(b, (int)(x * xScaleFactor), (int)(y * yScaleFactor));

                    //float aAverage = (int)((aPixel.R + aPixel.G + aPixel.B) / 3f) / 256f;
                    //float bAverage = (int)((bPixel.R + bPixel.G + bPixel.B) / 3f) / 256f;

                    //int multiplied = (int)(aAverage * bAverage * 256);
                    int rM;
                    int gM;
                    int bM;
                    if (bPixel.R > 0)
                        rM = (int)((aPixel.R / 256f) / (bPixel.R / 256f) * 256f);
                    else
                        rM = 255;
                    if (bPixel.G > 0)
                        gM = (int)((aPixel.G / 256f) / (bPixel.G / 256f) * 256f);
                    else
                        gM = 255;
                    if (bPixel.B > 0)
                        bM = (int)((aPixel.B / 256f) / (bPixel.B / 256f) * 256f);
                    else
                        bM = 255;

                    outData[x + (y * this.Width)] = new Color(rM, gM, bM, 255);
                }
            }
            return new ImageInfo(outData, this.Width, this.Height);
        }

        public ImageInfo multiply(double amount)
        {
            Color[] outData = new Color[this.Width * this.Height];

            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    Color pixel = getPixel(this, x, y);

                    int rM = (int)Math.Min(Math.Max((pixel.R * amount), 0), 255);
                    int gM = (int)Math.Min(Math.Max((pixel.G * amount), 0), 255);
                    int bM = (int)Math.Min(Math.Max((pixel.B * amount), 0), 255);

                    outData[x + (y * this.Width)] = new Color(rM, gM, bM, 255);
                }
            }
            return new ImageInfo(outData, this.Width, this.Height);
        }

        public ImageInfo toGreyScale(bool average = true)
        {
            Color[] outData = new Color[this.Width * this.Height];

            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    Color pixel = getPixel(this, x, y);

                    int grey = 0;
                    if (average)
                        grey = (int)((pixel.R + pixel.G + pixel.B) / 3f);
                    else
                        grey = Math.Max(pixel.R, Math.Max(pixel.G, pixel.G));
                    outData[x + (y * this.Width)] = new Color(grey, grey, grey, 255);
                }
            }
            return new ImageInfo(outData, this.Width, this.Height);
        }

        public ImageInfo divide(double denominator)
        {
            Color[] outData = new Color[this.Width * this.Height];

            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    Color pixel = getPixel(this, x, y);

                    float average = (int)((pixel.R + pixel.G + pixel.B) / 3f) / 256f;

                    int divided = (int)(((float)average / (float)denominator) * 256);
                    outData[x + (y * this.Width)] = new Color(divided, divided, divided, 255);
                }
            }
            return new ImageInfo(outData, this.Width, this.Height);
        }

        public ImageInfo add(double amount)
        {
            Color[] outData = new Color[this.Width * this.Height];

            if (Math.Abs(amount) >= 1.0)
                amount = amount / 256.0;

            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    Color pixel = getPixel(this, x, y);

                    //float average = (int)((pixel.R + pixel.G + pixel.B) / 3f) / 256f;
                    int r = Math.Min(255, pixel.R + (int)(amount * 256));
                    int g = Math.Min(255, pixel.G + (int)(amount * 256));
                    int b = Math.Min(255, pixel.B + (int)(amount * 256));

                    //int added = Math.Min((int)((average + amount) * 256), 255);
                    outData[x + (y * this.Width)] = new Color(r, g, b, 255);
                }
            }
            return new ImageInfo(outData, this.Width, this.Height);
        }

        public ImageInfo maximize(bool asGreyScale = false)
        {
            Color[] outData = new Color[this.Width * this.Height];

            int highest = 0;
            int lowest = 255;

            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    Color pixel = getPixel(this, x, y);
                    if (asGreyScale)
                    {
                        highest = Math.Max((int)((pixel.R + pixel.G + pixel.B) / 3f), highest);
                        lowest = Math.Min((int)((pixel.R + pixel.G + pixel.B) / 3f), lowest);
                    }
                    else
                    {
                        highest = Math.Max(Math.Max(pixel.R, pixel.G), Math.Max(pixel.B, highest));
                        lowest = Math.Min(Math.Min(pixel.R, pixel.G), Math.Min(pixel.B, lowest));
                    }
                }
            }

            int range = highest - lowest;

            if (range <= 0)
                return this;

            double factor = 256.0 / (double)range;
            Logger.Log("Maximization factor: " + factor + " (256 / (" + highest + " - " + lowest + "))");

            return (this - lowest) * factor;
        }

        public ImageInfo add(ImageInfo imageB)
        {
            Color[] outData = new Color[this.Width * this.Height];

            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    Color pixelA = getPixel(this, x, y);
                    Color pixelB = getPixel(imageB, x, y);

                    int r = Math.Min(pixelA.R + pixelB.R, 255);
                    int g = Math.Min(pixelA.G + pixelB.G, 255);
                    int b = Math.Min(pixelA.B + pixelB.B, 255);
                    outData[x + (y * this.Width)] = new Color(r, g, b, 255);
                }
            }
            return new ImageInfo(outData, this.Width, this.Height);
        }

        public ImageInfo channel(int channel, bool toGreyscale)
        {
            Color[] outData = new Color[this.Width * this.Height];

            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    Color pixel = getPixel(this, x, y);

                    int channelValue = channel == 0 ? pixel.R : channel == 1 ? pixel.G : channel == 2 ? pixel.B : pixel.A;
                    if (toGreyscale)
                        outData[x + (y * this.Width)] = new Color(channelValue, channelValue, channelValue, 255);
                    else
                    {
                        outData[x + (y * this.Width)] = new Color(channel == 0 ? channelValue : 0, channel == 1 ? channelValue : 0, channel == 2 ? channelValue : 0, channel > 3 ? channelValue : 255);
                    }
                }
            }
            return new ImageInfo(outData, this.Width, this.Height);
        }

        public ImageInfo proximityMap(int falloff = 4)
        {
            int valueSize = falloff * 2 + 1;
            double[,] valueGrid = new double[valueSize, valueSize];
            for (int x = 0; x < valueSize; x++)
            {
                //string column = "[";
                for (int y = 0; y < valueSize; y++)
                {
                    double value = Math.Sqrt(Math.Pow(x - falloff, 2) + Math.Pow(y - falloff, 2));
                    valueGrid[x, y] = value;
                    //column += Math.Round(value,3) + (y == valueSize - 1 ? "]" : ", ");
                }
                //Logger.Log(column);
            }

            Color[] outData = new Color[this.Width * this.Height];

            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    double smallest = falloff + 1;
                    for (int xV = -falloff; xV < valueSize - falloff; xV++)
                    {
                        int polledX = x + xV;
                        if (polledX < 0)
                            continue;
                        if (polledX >= this.Width)
                            break;
                        for (int yV = -falloff; yV < valueSize - falloff; yV++)
                        {
                            int polledY = y + yV;
                            if (polledY < 0)
                                continue;
                            if (polledY >= this.Height)
                                break;
                            //Logger.Log("Pixel [" + polledX + ", " + polledY + "] is on the map");
                            Color pixel = getPixel(this, polledX, polledY);
                            if (pixel.R > 0 || pixel.G > 0 || pixel.B > 0)
                                smallest = Math.Min(valueGrid[xV + falloff, yV + falloff], smallest);
                            //else
                            //    Logger.Log("Pixel [" + polledX + ", " + polledY + "] is black");
                        }
                    }

                    int proximityValue = 0;
                    //Logger.Log("Smallest value for [" + x + ", " + y + "] is " + smallest);
                    if (smallest < falloff)
                        proximityValue = (int)(((falloff - smallest) / falloff) * 256);
                    outData[x + (y * this.Width)] = new Color(proximityValue, proximityValue, proximityValue, 255);
                }
            }
            return new ImageInfo(outData, this.Width, this.Height);
        }

        public ImageInfo invert()
        {
            Color[] outData = new Color[this.Width * this.Height];

            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    Color pixelA = getPixel(this, x, y);

                    int r = 255 - pixelA.R;
                    int g = 255 - pixelA.G;
                    int b = 255 - pixelA.B;
                    outData[x + (y * this.Width)] = new Color(r, g, b, 255);
                }
            }
            return new ImageInfo(outData, this.Width, this.Height);
        }

        public ImageInfo append(ImageInfo b, Vector2 position)
        {
            return null;
        }

        public ImageInfo crop(Rectangle newCanvas)
        {
            return crop(newCanvas, Color.Transparent);
        }

        public ImageInfo crop(Rectangle newCanvas, Color background)
        {
            Color[] outData = new Color[newCanvas.Width * newCanvas.Height];

            Rectangle sourceBounds = new Rectangle(0, 0, Width, Height);

            for (int x = 0; x < newCanvas.Width; x++)
            {
                for (int y = 0; y < newCanvas.Height; y++)
                {
                    int xSource = x + newCanvas.X;
                    int ySource = y + newCanvas.Y;

                    Color pixel;
                    if (sourceBounds.Contains(xSource, ySource))
                        pixel = getPixel(this, xSource, ySource);
                    else
                        pixel = background;

                    outData[x + (y * newCanvas.Width)] = pixel;
                }
            }
            return new ImageInfo(outData, newCanvas.Width, newCanvas.Height);
        }

        public static explicit operator Texture2D(ImageInfo a) => a.render();
        public static implicit operator ImageInfo(Texture2D a) => new ImageInfo(a);

        public static ImageInfo operator !(ImageInfo a) => a.invert();

        public static ImageInfo operator +(ImageInfo a) => a;
        public static ImageInfo operator +(ImageInfo a, ImageInfo b) => a.add(b);
        public static ImageInfo operator +(ImageInfo a, double b) => a.add(b);

        public static ImageInfo operator -(ImageInfo a) => a.invert();
        public static ImageInfo operator -(ImageInfo a, ImageInfo b) => a.add(-b);
        public static ImageInfo operator -(ImageInfo a, double b) => a.add(-b);

        public static ImageInfo operator *(ImageInfo a, ImageInfo b) => a.multiply(b, 1f);
        public static ImageInfo operator *(ImageInfo a, double b) => a.multiply(b);

        public static ImageInfo operator /(ImageInfo a, ImageInfo b) => a.divide(b);
        public static ImageInfo operator /(ImageInfo a, double b) => a.divide(b);
    }
}
