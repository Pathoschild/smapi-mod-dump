using System;
using Microsoft.Xna.Framework;

namespace Revitalize.Framework.Illuminate
{
    public static class ColorExtensions
    {
        public static Color GreyScaleAverage(this Color color)
        {
            int value = (color.R + color.G + color.B) / 3;
            return new Color(new Vector3(value));
        }

        public static Color Invert(this Color color)
        {
            int r = Math.Abs(255 - color.R);
            int g = Math.Abs(255 - color.G);
            int b = Math.Abs(255 - color.B);
            return new Color(r, g, b);
        }
    }
}
