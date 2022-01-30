/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewGMCMOptions
**
*************************************************/

// Copyright 2022 Jamie Taylor
﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace GMCMOptions.Framework {
    public static class ColorUtil {
        /// <summary>
        ///   Return the <c cref="Color">Color</c> represented by the given coordinates in the HSV color space
        /// </summary>
        /// <remarks>see https://en.wikipedia.org/wiki/HSL_and_HSV </remarks>
        /// <param name="hueRadians">The hue specified in radians.  The value should be in either of the ranges -π..π or 0..2π</param>
        /// <param name="saturation">The saturation; in the range 0..1</param>
        /// <param name="value">The value; in the range 0..1</param>
        /// <returns>The <c cref="Color">Color</c> represented by the given arguments</returns>
        public static Color FromHSV(double hueRadians, double saturation, double value) {
            double hue = (360 + hueRadians * 180 / Math.PI) % 360;
            // see https://en.wikipedia.org/wiki/HSL_and_HSV#HSV_to_RGB
            double chroma = saturation * value;
            double hPrime = hue / 60;
            double bigX = chroma * (1 - Math.Abs(hPrime % 2 - 1));
            float m = (float)(value - chroma);
            float C = (float)chroma + m;
            float X = (float)bigX + m;
            return hPrime < 1 ? new Color(C, X, m)
                : hPrime < 2 ? new Color(X, C, m)
                : hPrime < 3 ? new Color(m, C, X)
                : hPrime < 4 ? new Color(m, X, C)
                : hPrime < 5 ? new Color(X, m, C)
                : new Color(C, m, X);
        }

        /// <summary>
        ///   Return the <c cref="Color">Color</c> represented by the given coordinates in the HSL color space
        /// </summary>
        /// <remarks>see https://en.wikipedia.org/wiki/HSL_and_HSV </remarks>
        /// <param name="hueRadians">The hue specified in radians.  The value should be in either of the ranges -π..π or 0..2π</param>
        /// <param name="saturation">The saturation; in the range 0..1</param>
        /// <param name="lightness">The lightness; in the range 0..1</param>
        /// <returns>The <c cref="Color">Color</c> represented by the given arguments</returns>
        public static Color FromHSL(double hueRadians, double saturation, double lightness) {
            double hue = (360 + hueRadians * 180 / Math.PI) % 360;
            // see https://en.wikipedia.org/wiki/HSL_and_HSV#HSL_to_RGB
            double chroma = (1 - Math.Abs(2 * lightness - 1)) * saturation;
            double hPrime = hue / 60;
            double bigX = chroma * (1 - Math.Abs(hPrime % 2 - 1));
            float m = (float)(lightness - chroma / 2);
            float C = (float)chroma + m;
            float X = (float)bigX + m;
            return hPrime < 1 ? new Color(C, X, m)
                : hPrime < 2 ? new Color(X, C, m)
                : hPrime < 3 ? new Color(m, C, X)
                : hPrime < 4 ? new Color(m, X, C)
                : hPrime < 5 ? new Color(X, m, C)
                : new Color(C, m, X);
        }

        /// <summary>
        ///   Return the HSV coordinates for the given <c cref="Color">Color</c>
        /// </summary>
        /// <remarks>see https://en.wikipedia.org/wiki/HSL_and_HSV </remarks>
        /// <param name="c">The <c cref="Color">Color</c></param>
        /// <returns>The hue in radians (in the range 0..2π), saturation (0..1), and value (0..1) of the color</returns>
        public static (double hueRadians, double saturation, double value) ToHSVRadians(Color c) {
            // see https://en.wikipedia.org/wiki/HSL_and_HSV#From_RGB
            double R = c.R / 255d;
            double G = c.G / 255d;
            double B = c.B / 255d;
            double V = Math.Max(R, Math.Max(G, B));
            double C = V - Math.Min(R, Math.Min(G, B));
            double Hdeg = C == 0 ? 0
                : 60 * (V == R ? (G - B) / C
                        : V == G ? (2 + (B - R) / C)
                        : (4 + (R - G) / C));
            double Hrad = Hdeg / 180 * Math.PI;
            double Sv = V == 0 ? 0 : C / V;
            return (Hrad, Sv, V);
        }

        /// <summary>
        ///   Return the HSL coordinates for the given <c cref="Color">Color</c>
        /// </summary>
        /// <remarks>see https://en.wikipedia.org/wiki/HSL_and_HSV </remarks>
        /// <param name="c">The <c cref="Color">Color</c></param>
        /// <returns>The hue in radians (in the range 0..2π), saturation (0..1), and lightness (0..1) of the color</returns>
        public static (double hueRadians, double saturation, double lightness) ToHSLRadians(Color c) {
            // see https://en.wikipedia.org/wiki/HSL_and_HSV#From_RGB
            double R = c.R / 255d;
            double G = c.G / 255d;
            double B = c.B / 255d;
            double V = Math.Max(R, Math.Max(G, B));
            double C = V - Math.Min(R, Math.Min(G, B));
            double L = V - C / 2;
            double Hdeg = C == 0 ? 0
                : 60 * (V == R ? (G - B) / C
                        : V == G ? (2 + (B - R) / C)
                        : (4 + (R - G) / C));
            double Hrad = Hdeg / 180 * Math.PI;
            double Sl = (L == 0 || L == 1) ? 0 : (V - L) / (Math.Min(L, 1 - L));
            return (Hrad, Sl, L);
        }

        /// <summary>
        ///   Create a new square checkerboard image
        /// </summary>
        /// <param name="size">The width and height (in pixels) of the desired image</param>
        /// <param name="checkerboardSize">The width and height of each checker square</param>
        /// <param name="color0">Color for half of the checker squares.  Default <c cref="Color.White">White</c>.</param>
        /// <param name="color1">Color for the other half of the checker squares.  Default <c cref="Color.Gray">Gray</c>.</param>
        /// <returns>A new texture containing the checkerboard image</returns>
        public static Texture2D CreateCheckerboardTexture(int size, int checkerboardSize = 5, Color? color0 = null, Color? color1 = null) {
            Color c0 = color0 ?? Color.White;
            Color c1 = color1 ?? Color.Gray;
            Color[] cbData = new Color[size * size];
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    Color c = (((i / checkerboardSize) % 2) == 0) ^ (((j / checkerboardSize) % 2) == 0)
                        ? c0 : c1;
                    cbData[i * size + j] = c;
                }
            }
            Texture2D checkerboard = new Texture2D(Game1.graphics.GraphicsDevice, size, size);
            checkerboard.SetData(cbData);
            return checkerboard;
        }

        /// <summary>
        ///   Create a new image of a color wheel.  The textures will be a square with
        ///   sides of <c>2 * <paramref name="radius"/> - 1</c>.
        /// </summary>
        /// <param name="radius">The radius (in pixels) of the color wheel</param>
        /// <returns>A new texture containing the color wheel image</returns>
        public static Texture2D CreateColowWheelTexture(int radius) {
            int side = 2 * radius - 1;
            Color[] data = new Color[side * side];
            for (int x = 1 - radius; x < radius; x++) {
                for (int y = 1 - radius; y < radius; y++) {
                    int dataIdx = x + radius - 1 + (y + radius - 1) * side;
                    double saturation = Math.Sqrt((x * x) + (y * y)) / radius;
                    double hueRadians = Math.Atan2(y, x);
                    data[dataIdx] = saturation > 1 ? Color.Transparent : ColorUtil.FromHSV(hueRadians, saturation, 1.0);
                }
            }
            Texture2D colorWheel = new Texture2D(Game1.graphics.GraphicsDevice, side, side);
            colorWheel.SetData(data);
            return colorWheel;
        }

        /// <summary>A 1x1 texture containing a white pixel</summary>
        public static Texture2D Pixel = InitPixel();
        private static Texture2D InitPixel() {
            Color[] data = new Color[1];
            data[0] = Color.White;
            Texture2D pixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            pixel.SetData(data);
            return pixel;
        }
    }
}
