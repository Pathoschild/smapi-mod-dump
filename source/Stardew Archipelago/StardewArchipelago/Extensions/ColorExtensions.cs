/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace StardewArchipelago.Extensions
{
    public static class ColorExtensions
    {
        private static Dictionary<int, Color> _cache = new();

        public static Color GetAsBrightColor(this string name)
        {
            if (name == null)
            {
                Debugger.Break();
                return "".GetHash().GetAsBrightColor();
            }
            var hash = name.GetHash();
            return hash.GetAsBrightColor();
        }

        public static Color GetAsBrightColor(this int hashedValue)
        {
            if (_cache.ContainsKey(hashedValue))
            {
                return _cache[hashedValue];
            }

            var random = new Random(hashedValue);
            var red = random.Next(0, 256);
            var green = random.Next(0, 256);
            var blue = random.Next(0, 256);
            var total = red + green + blue;
            var whichToChange = random.Next(0, 3);
            while (total < 384)
            {
                red = Math.Min(red + random.Next(0, 32), 255);
                blue = Math.Min(blue + random.Next(0, 32), 255);
                green = Math.Min(green + random.Next(0, 32), 255);
                total = red + green + blue;
            }
            while (total > 624)
            {
                switch (whichToChange)
                {
                    case 0:
                        red = Math.Max(red - random.Next(0, 32), 0);
                        break;
                    case 1:
                        blue = Math.Max(blue - random.Next(0, 32), 0);
                        break;
                    case 2:
                        green = Math.Max(green - random.Next(0, 32), 0);
                        break;
                }
                total = red + green + blue;
            }

            var resultingColor = new Color(red, green, blue);
            _cache.Add(hashedValue, resultingColor);
            return resultingColor;
        }
    }
}
