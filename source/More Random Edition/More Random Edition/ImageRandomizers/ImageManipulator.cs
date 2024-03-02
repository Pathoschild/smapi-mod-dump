/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;

namespace Randomizer
{
    public class ImageManipulator 
    {
        /// <summary>
        /// This color is good for multiplying onto images, since it won't result
        /// in super weird colors
        /// </summary>
        public static readonly Color PaleColor = new(138, 255, 217);

        /// <summary>
        /// Changes the hue of the image by shifting it by 'amountToShift' (values of 0-359 recommended, where 0 doesn't change the color)
        /// </summary>
        public static Texture2D ShiftImageHue(Texture2D image, float amountToShift)
        {
            Color[] imageColors = GetImageColorData(image);
            Color[] alteredImageColors = new Color[image.Width * image.Height];

            for (int i = 0; i < imageColors.Length; i++)
            {
                alteredImageColors[i] = IncreaseHueBy(imageColors[i], amountToShift);
            }

            Texture2D newImage = new(Game1.graphics.GraphicsDevice, image.Width, image.Height);
            newImage.SetData(alteredImageColors);

            return newImage;
        }

        /// <summary>
        /// Shifts any color within the inputted range by more or less than colors not in the range,
        /// if zero it won't shift the color at all
        /// H values mapped to colors for ref:
        ///
        /// Red falls between 0 and 60 degrees.
        /// Yellow falls between 61 and 120 degrees.
        /// Green falls between 121 and 180 degrees.
        /// Cyan falls between 181 and 240 degrees.
        /// Blue falls between 241 and 300 degrees.
        /// Magenta falls between 301 and 360 degrees.
        /// </summary>
        public static Texture2D ShiftImageHue(Texture2D image, float amountToShift, float lowerBound, float upperBound, float multiplier)
        {
            Color[] imageColors = GetImageColorData(image);
            Color[] alteredImageColors = new Color[image.Width * image.Height];

            for (int i = 0; i < imageColors.Length; i++)
            {
                Color currentColor = imageColors[i];
                float currentHue = RgbToHsv(currentColor.R, currentColor.G, currentColor.B)[0];

                // colors outside the range are shifted the full amount (multiplier ignored)
                if (upperBound <= currentHue || currentHue <= lowerBound) alteredImageColors[i] = IncreaseHueBy(imageColors[i], amountToShift); 

                // if the multiplier is 0 color within the range are not changed at all
                else if (multiplier == 0) alteredImageColors[i] = imageColors[i]; 
                // otherwise shift them by a smaller/larger amount defined by the multiplier

                else alteredImageColors[i] = IncreaseHueBy(imageColors[i], amountToShift * multiplier); 
            }

            Texture2D newImage = new(Game1.graphics.GraphicsDevice, image.Width, image.Height);
            newImage.SetData(alteredImageColors);

            return newImage;
        }

        /// <summary>
        /// Combines two images by multiplying their colors together. Similar to using 'TopImage' as a multiply layer over
        /// 'BottomImage' in a graphics program.
        /// 
        /// The two images must have the same width and height.
        /// </summary>
        public static Texture2D MultiplyImages(Texture2D bottomImage, Texture2D topImage)
        {
            Color[] bottomImageColors = GetImageColorData(bottomImage);
            Color[] topImageColors = GetImageColorData(topImage);
            Color[] alteredImageColors = new Color[bottomImage.Width * bottomImage.Height];

            for (int i = 0; i < bottomImageColors.Length; i++)
            {
                alteredImageColors[i] = MultiplyColors(bottomImageColors[i], topImageColors[i]);
            }

            Texture2D newImage = new(Game1.graphics.GraphicsDevice, bottomImage.Width, bottomImage.Height);
            newImage.SetData(alteredImageColors);

            return newImage;
        }

        /// <summary>
        /// Overlays one image on top of another, the top images obscures the bottom unless it has transparency in which
        /// case the colors are blended
        /// </summary>
        public static Texture2D OverlayImages(Texture2D bottomImage, Texture2D topImage)
        {
            Color[] bottomImageColors = GetImageColorData(bottomImage);
            Color[] topImageColors = GetImageColorData(topImage);
            Color[] alteredImageColors = new Color[bottomImage.Width * bottomImage.Height];

            for (int i = 0; i < bottomImageColors.Length; i++)
            {
                if (topImageColors[i].A == 0) alteredImageColors[i] = bottomImageColors[i];
                else if (topImageColors[i].A < 255) alteredImageColors[i] = MultiplyColors(topImageColors[i], bottomImageColors[i]);
                else alteredImageColors[i] = topImageColors[i]; 
            }

            Texture2D newImage = new(Game1.graphics.GraphicsDevice, bottomImage.Width, bottomImage.Height);
            newImage.SetData(alteredImageColors);

            return newImage;
        }

        /// <summary>
        /// Multiplies the colors of an image by a color. Equivalent to placing a multiply layer of a solid color
        /// over the image in a graphics program
        /// </summary>
        public static Texture2D MultiplyImageByColor(Texture2D originalImage, Color colorToBeMultipliedBy)
        {
            Color[] imageColors = GetImageColorData(originalImage);
            Color[] alteredImageColors = new Color[originalImage.Width * originalImage.Height];

            for (int i = 0; i < imageColors.Length; i++)
            {
                alteredImageColors[i] = MultiplyColors(imageColors[i], colorToBeMultipliedBy);
            }

            Texture2D newImage = new(Game1.graphics.GraphicsDevice, originalImage.Width, originalImage.Height);
            newImage.SetData(alteredImageColors);

            return newImage;
        }


        /// <summary>
        /// Gets a array of the colors in an image.
        /// </summary>
        private static Color[] GetImageColorData(Texture2D image)
        {
            Color[] colors = new Color[image.Width * image.Height];
            image.GetData(colors);
            return colors;
        }

        /// <summary>
        /// Increases the hue of a color by 'valueToIncrease'
        /// </summary>
        public static Color IncreaseHueBy(Color originalColor, float valueToIncrease)
        {
            float[] HSV = RgbToHsv(originalColor.R, originalColor.G, originalColor.B);
            float h = HSV[0]; 
            h += valueToIncrease;

            // Only the hue was changed, so just set the saturation and value to what they were originally
            float[] RGB = HsvToRgb(h, HSV[1], HSV[2]); 

            Color alteredColor = new()
            {
                R = (byte)RGB[0],
                G = (byte)RGB[1],
                B = (byte)RGB[2],
                A = originalColor.A
            };
            return alteredColor;
        }

        /// <summary>
        /// Gets a random color
        /// Fixes the saturation and value so that it's not too unrecognizable
        /// </summary>
        /// <param name="hueRange">The hue range to restrict the color to</param>
        /// <param name="saturationRange">The saturation range to restrict the color to</param>
        /// <param name="valueRange">The value range to restrict the color to</param>
        /// <returns>The random color</returns>
        public static Color GetRandomColor(
            Range hueRange = null,
            Range saturationRange = null,
            Range valueRange = null)
        {
            // Value -
            Range hueRangeToUse = hueRange ?? new Range(0, 359);
            int randomH = hueRangeToUse.GetRandomValue();

            // Saturation - the default won't look look too white or bright
            Range saturationRangeToUse = saturationRange ?? new Range(60, 85);
            int randomS = saturationRangeToUse.GetRandomValue();

            // Value - we don't want to it to look too black
            Range valueRangeToUse = valueRange ?? new Range(60, 85);
            int randomV = valueRangeToUse.GetRandomValue();

            Color randomColor = HsvToColor(randomH, randomS, randomV);
            return randomColor;
        }

        /// <summary>
        /// Converts the standard HSV color values to a Color object
        /// See the documentation on HsvToRgb for how these are manipulated
        /// </summary>
        /// <param name="h">Hue - from 0-359</param>
        /// <param name="s">Saturation - from 0-100</param>
        /// <param name="v">Value - from 0-100</param>
        /// <returns>The color it converted from</returns>
        public static Color HsvToColor(int h, int s, int v)
        {
            float modifiedS = s / 100f;
            float modifiedV = (v / 100f) * 255f;
            float[] rgb = HsvToRgb(h, modifiedS, modifiedV);
            return new Color((byte)rgb[0], (byte)rgb[1], (byte)rgb[2]);
        }

        /// <summary>
        /// Multiplies two colors together
        /// </summary>
        /// <param name="color1"></param>
        /// <param name="color2"></param>
        /// <returns></returns>
        public static Color MultiplyColors(Color color1, Color color2)
        {
            Color multipliedColor = new()
            {
                R = (byte)((color1.R * color2.R) / 255),
                G = (byte)((color1.G * color2.G) / 255),
                B = (byte)((color1.B * color2.B) / 255),

                // If there is transparency in either or both images, it uses the most transparent value
                // in order to keep transparent backgrounds transparent and shadows looking natural
                A = Math.Min(color1.A, color2.A)
            };

            return multipliedColor;
        }

        /// <summary>
        /// Averages out the two given colors
        /// </summary>
        /// <param name="firstColor"></param>
        /// <param name="secondColor"></param>
        /// <returns></returns>
        public static Color AverageColors(Color color1, Color color2)
        {
            int newR = GetSingleColorAverage(color1.R, color2.R);
            int newG = GetSingleColorAverage(color1.G, color2.G);
            int newB = GetSingleColorAverage(color1.B, color2.B);
            return new Color(newR, newG, newB);
        }

        /// <summary>
        /// Gets the average of a single color component - r, g, or b
        /// </summary>
        /// <param name="color1"></param>
        /// <param name="color2"></param>
        /// <returns></returns>
        private static int GetSingleColorAverage(int color1, int color2)
        {
            return (int)Math.Sqrt(Math.Pow(color1, 2) + Math.Pow(color2, 2) / 2);
        }

        /// <summary>
        /// Converts rgb values to hsv values
        /// This was modified from this stackoverflow post: https://stackoverflow.com/a/12985385
        /// </summary>
        /// <param name="r">Red - from 0-255</param>
        /// <param name="g">Green - from 0-255</param>
        /// <param name="b">Blue - from 0-255</param>
        /// <returns>
        /// An array of the following:
        /// - [0] = Hue - from 0-360
        /// - [1] = Saturation - from 0-1
        /// - [2] = Value - from 0-255
        /// </returns>
        public static float[] RgbToHsv(float r, float g, float b)
        {
            float[] output = new float[3];
            float h, s, v, min, max, delta;
            min = Math.Min(Math.Min(r, g), b);
            max = Math.Max(Math.Max(r, g), b);
            v = max;               // v
            delta = max - min;
            if (max != 0)
            {
                s = delta / max;       // s

                if (r == max)
                    h = (g - b) / delta;       // between yellow & magenta
                else if (g == max)
                    h = 2 + (b - r) / delta;   // between cyan & yellow
                else
                    h = 4 + (r - g) / delta;   // between magenta & cyan
                h *= 60;               // degrees
                if (h < 0)
                    h += 360;
            }
            else
            {
                // r = g = b = 0       // s = 0, v is undefined
                s = 0;
                h = -1;
            }

            output[0] = h;
            output[1] = s;
            output[2] = v;

            return output;
        }

        /// <summary>
        /// Converts hgv values to rgb
        /// This was modified from this stack overflow post: https://stackoverflow.com/a/12985385
        /// </summary>
        /// <param name="h">Hue - from 0-360</param>
        /// <param name="s">Saturation - from 0-1</param>
        /// <param name="v">Value - from 0-255</param>
        public static float[] HsvToRgb(float h, float s, float v)
        {
            float[] output = new float[3];
            // Keeps h from going over 360
            h = h - ((int)(h / 360) * 360);

            int i;
            float r, g, b, f, p, q, t;
            if (s == 0)
            {
                // achromatic (grey)
                r = g = b = v;

                output[0] = r;
                output[1] = g;
                output[2] = b;

                return output;
            }
            h /= 60;           // sector 0 to 5

            i = (int)h;
            f = h - i;         // factorial part of h
            p = v * (1 - s);
            q = v * (1 - s * f);
            t = v * (1 - s * (1 - f));
            switch (i)
            {
                case 0:
                    r = v;
                    g = t;
                    b = p;
                    break;
                case 1:
                    r = q;
                    g = v;
                    b = p;
                    break;
                case 2:
                    r = p;
                    g = v;
                    b = t;
                    break;
                case 3:
                    r = p;
                    g = q;
                    b = v;
                    break;
                case 4:
                    r = t;
                    g = p;
                    b = v;
                    break;
                default:       // case 5:
                    r = v;
                    g = p;
                    b = q;
                    break;
            }

            output[0] = r;
            output[1] = g;
            output[2] = b;

            return output;
        }
    }
}
