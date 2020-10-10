/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System.Globalization;
using Color = System.Drawing.Color;

namespace Igorious.StardewValley.DynamicAPI.Data.Supporting
{
    public sealed class RawColor
    {
        public RawColor() { }

        public RawColor(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }

        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

        public static RawColor FromHex(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex)) return null;
            return new RawColor
            {
                R = int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber),
                G = int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber),
                B = int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber),
            };
        }

        public override string ToString()
        {
            return $"{R} {G} {B}";
        }

        public string ToHex()
        {
            return $"{R:X2}{G:X2}{B:X2}";
        }

        public Microsoft.Xna.Framework.Color ToXnaColor()
        {
            return new Microsoft.Xna.Framework.Color(R, G, B);
        }

        public void ToHSL(out double h, out double s, out double l)
        {
            var c = Color.FromArgb(R, G, B);
            h = c.GetHue();
            s = c.GetSaturation();
            l = c.GetBrightness();
        }

        public static RawColor FromHSL(double h, double s, double l)
        {
            h /= 360;
            double r, g, b;
            if (l == 0 || s == 0)
            {
                r = g = b = l;
            }
            else
            {
                var temp2 = (l < 0.5)
                    ? l * (1.0 + s) 
                    : l + s - (l * s);

                var temp1 = 2.0 * l - temp2;

                r = GetColorComponent(temp1, temp2, h + 1.0 / 3.0);
                g = GetColorComponent(temp1, temp2, h);
                b = GetColorComponent(temp1, temp2, h - 1.0 / 3.0);
            }
            return new RawColor((int)(255 * r), (int)(255 * g), (int)(255 * b));
        }

        private static double GetColorComponent(double temp1, double temp2, double temp3)
        {
            if (temp3 < 0)
            {
                temp3 += 1;
            }
            else if (temp3 > 1)
            {
                temp3 -= 1;
            }

            if (temp3 < 1.0 / 6) return temp1 + (temp2 - temp1) * 6 * temp3;
            if (temp3 < 0.5) return temp2;
            if (temp3 < 2.0 / 3) return temp1 + (temp2 - temp1) * 6 * (2.0 / 3 - temp3);
            return temp1;
        }
    }
}