/**
 * 2xBR Filter
 *
 * Javascript implementation of the 2xBR filter.
 *
 * This is a rewrite of the previous 0.2.5 version, it outputs the same quality,
 * however this version is about a magnitude **slower** than its predecessor. 
 *
 * Use this version if you want to learn how the algorithms works, as the code is 
 * much more readable.
 *
 * @version 0.3.0
 * @author Ascari <carlos.ascari.x@gmail.com>
 */

/**
 * Originally written in JavaScript
 * @url https://github.com/carlosascari/2xBR-Filter/blob/master/xbr.js
 * 
 * Converted to C# / XNA
 * @author NinthWorld
 * @date June 11, 2019
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace HDSprites
{
    public class Upscaler
    {
        public const bool WRAP = false;
        public const int SCALE = 2;
        public const int Y_WEIGHT = 48;
        public const int U_WEIGHT = 7;
        public const int V_WEIGHT = 6;

        public static Color[] Matrix = new Color[]
        {
                         new Color(), new Color(), new Color(),
            new Color(), new Color(), new Color(), new Color(), new Color(),
            new Color(), new Color(), new Color(), new Color(), new Color(),
            new Color(), new Color(), new Color(), new Color(), new Color(),
                         new Color(), new Color(), new Color()
        };

        public static double D(Color colorA, Color colorB)
        {
            int r = Math.Abs((int)colorA.R - (int)colorB.R);
            int g = Math.Abs((int)colorA.G - (int)colorB.G);
            int b = Math.Abs((int)colorA.B - (int)colorB.B);
            double y = r * 0.299000 + g * 0.587000 + b * 0.144000;
            double u = r * -.168736 + g * -.331264 + b * 0.500000;
            double v = r * 0.500000 + g * -.418688 + b * -.081312;
            return (y * Y_WEIGHT) + (u * U_WEIGHT) + (v * V_WEIGHT);
        }
        
        public static Color Blend(Color colorA, Color colorB, double alpha)
        {
            double reverseAlpha = 1.0 - alpha;
            int r = (int)Math.Floor((alpha * colorB.R) + (reverseAlpha * colorA.R));
            int g = (int)Math.Floor((alpha * colorB.G) + (reverseAlpha * colorA.G));
            int b = (int)Math.Floor((alpha * colorB.B) + (reverseAlpha * colorA.B));
            int a = (int)Math.Floor((alpha * colorB.A) + (reverseAlpha * colorA.A));
            return new Color(r, g, b, a);
        }

        public static Texture2D Upscale(Texture2D input)
        {
            int srcW = input.Width;
            int srcH = input.Height;
            Color[] srcColors = new Color[srcW * srcH];
            input.GetData(srcColors);

            int scaledW = srcW * SCALE;
            int scaledH = srcH * SCALE;
            Color[] scaledColors = new Color[scaledW * scaledH];
            
            int Mod(int x, int m)
            {
                return (x % m + m) % m;
            }

            Color GetColor(int x, int y)
            {
                if (WRAP)
                {
                    x = Mod(x, srcW);
                    y = Mod(y, srcH);
                }
                else if (x < 0 || x >= srcW || y < 0 || y >= srcH)
                {
                    return new Color(0, 0, 0, 0);
                }
                return srcColors[srcW * y + x];
            }

            for (int x = 0; x < srcW; ++x)
            {
                for (int y = 0; y < srcH; ++y)
                {
                    /* Matrix: 10 is (0,0) i.e. current pixel.
                        -2 | -1|  0| +1| +2 	(x)
                    ______________________________
                    -2 |	    [ 0][ 1][ 2]
                    -1 |	[ 3][ 4][ 5][ 6][ 7]
                     0 |	[ 8][ 9][10][11][12]
                    +1 |	[13][14][15][16][17]
                    +2 |	    [18][19][20]
                    (y)|
                    */

                    Matrix[ 0] = GetColor(x - 1, y - 2);
                    Matrix[ 1] = GetColor(    x, y - 2);
                    Matrix[ 2] = GetColor(x + 1, y - 2);
                    Matrix[ 3] = GetColor(x - 2, y - 1);
                    Matrix[ 4] = GetColor(x - 1, y - 1);
                    Matrix[ 5] = GetColor(    x, y - 1);
                    Matrix[ 6] = GetColor(x + 1, y - 1);
                    Matrix[ 7] = GetColor(x + 2, y - 1);
                    Matrix[ 8] = GetColor(x - 2,     y);
                    Matrix[ 9] = GetColor(x - 1,     y);
                    Matrix[10] = GetColor(    x,     y);
                    Matrix[11] = GetColor(x + 1,     y);
                    Matrix[12] = GetColor(x + 2,     y);
                    Matrix[13] = GetColor(x - 2, y + 1);
                    Matrix[14] = GetColor(x - 1, y + 1);
                    Matrix[15] = GetColor(    x, y + 1);
                    Matrix[16] = GetColor(x + 1, y + 1);
                    Matrix[17] = GetColor(x + 2, y + 1);
                    Matrix[18] = GetColor(x - 1, y + 2);
                    Matrix[19] = GetColor(    x, y + 2);
                    Matrix[20] = GetColor(x + 1, y + 2);

                    double d_10_9 = D(Matrix[10], Matrix[9]);
                    double d_10_5 = D(Matrix[10], Matrix[5]);
                    double d_10_11 = D(Matrix[10], Matrix[11]);
                    double d_10_15 = D(Matrix[10], Matrix[15]);
                    double d_10_14 = D(Matrix[10], Matrix[14]);
                    double d_10_6 = D(Matrix[10], Matrix[6]);
                    double d_4_8 = D(Matrix[4], Matrix[8]);
                    double d_4_1 = D(Matrix[4], Matrix[1]);
                    double d_9_5 = D(Matrix[9], Matrix[5]);
                    double d_9_15 = D(Matrix[9], Matrix[15]);
                    double d_9_3 = D(Matrix[9], Matrix[3]);
                    double d_5_11 = D(Matrix[5], Matrix[11]);
                    double d_5_0 = D(Matrix[5], Matrix[0]);
                    double d_10_4 = D(Matrix[10], Matrix[4]);
                    double d_10_16 = D(Matrix[10], Matrix[16]);
                    double d_6_12 = D(Matrix[6], Matrix[12]);
                    double d_6_1 = D(Matrix[6], Matrix[1]);
                    double d_11_15 = D(Matrix[11], Matrix[15]);
                    double d_11_7 = D(Matrix[11], Matrix[7]);
                    double d_5_2 = D(Matrix[5], Matrix[2]);
                    double d_14_8 = D(Matrix[14], Matrix[8]);
                    double d_14_19 = D(Matrix[14], Matrix[19]);
                    double d_15_18 = D(Matrix[15], Matrix[18]);
                    double d_9_13 = D(Matrix[9], Matrix[13]);
                    double d_16_12 = D(Matrix[16], Matrix[12]);
                    double d_16_19 = D(Matrix[16], Matrix[19]);
                    double d_15_20 = D(Matrix[15], Matrix[20]);
                    double d_15_17 = D(Matrix[15], Matrix[17]);
                    
                    // Top Left
                    double a1 = (d_10_14 + d_10_6 + d_4_8 + d_4_1 + (4 * d_9_5));
                    double b1 = (d_9_15 + d_9_3 + d_5_11 + d_5_0 + (4 * d_10_4));
                    if (a1 < b1)
                    {
                        Color newColor = (d_10_9 <= d_10_5 ? Matrix[9] : Matrix[5]);
                        Color blended = Blend(newColor, Matrix[10], 0.5);
                        scaledColors[((y * SCALE) * scaledW) + (x * SCALE)] = blended;
                    }
                    else
                    {
                        scaledColors[((y * SCALE) * scaledW) + (x * SCALE)] = Matrix[10];
                    }

                    // Top Right
                    double a2 = (d_10_16 + d_10_4 + d_6_12 + d_6_1 + (4 * d_5_11));
                    double b2 = (d_11_15 + d_11_7 + d_9_5 + d_5_2 + (4 * d_10_6));
                    if (a2 < b2)
                    {
                        Color newColor = (d_10_5 <= d_10_11 ? Matrix[5] : Matrix[11]);
                        Color blended = Blend(newColor, Matrix[10], 0.5);
                        scaledColors[((y * SCALE) * scaledW) + (x * SCALE + 1)] = blended;
                    }
                    else
                    {
                        scaledColors[((y * SCALE) * scaledW) + (x * SCALE + 1)] = Matrix[10];
                    }

                    // Bottom Left
                    double a3 = (d_10_4 + d_10_16 + d_14_8 + d_14_19 + (4 * d_9_15));
                    double b3 = (d_9_5 + d_9_13 + d_11_15 + d_15_18 + (4 * d_10_14));
                    if (a3 < b3)
                    {
                        Color newColor = (d_10_9 <= d_10_15 ? Matrix[9] : Matrix[15]);
                        Color blended = Blend(newColor, Matrix[10], 0.5);
                        scaledColors[((y * SCALE + 1) * scaledW) + (x * SCALE)] = blended;
                    }
                    else
                    {
                        scaledColors[((y * SCALE + 1) * scaledW) + (x * SCALE)] = Matrix[10];
                    }
                                    
                    // Bottom Right
                    double a4 = (d_10_6 + d_10_14 + d_16_12 + d_16_19 + (4 * d_11_15));
                    double b4 = (d_9_15 + d_15_20 + d_15_17 + d_5_11 + (4 * d_10_16));
                    if (a4 < b4)
                    {
                        Color newColor = (d_10_11 <= d_10_15 ? Matrix[11] : Matrix[15]);
                        Color blended = Blend(newColor, Matrix[10], 0.5);
                        scaledColors[((y * SCALE + 1) * scaledW) + (x * SCALE + 1)] = blended;
                    }
                    else
                    {
                        scaledColors[((y * SCALE + 1) * scaledW) + (x * SCALE + 1)] = Matrix[10];
                    }
                }
            }

            Texture2D output = new Texture2D(input.GraphicsDevice, scaledW, scaledH);
            output.SetData(scaledColors);

            return output;
        }
    }
}
