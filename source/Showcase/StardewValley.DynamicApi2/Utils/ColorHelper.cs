using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;

namespace Igorious.StardewValley.DynamicApi2.Utils
{
    public static class ColorHelper
    {
        private const int RedMask = 0xFF0000;
        private const int RedShift = 16;
        private const int GreenMask = 0x00FF00;
        private const int GreenShift = 8;
        private const int BlueMask = 0x0000FF;
        private const int BlueShift = 0;

        private static readonly Regex HexRegex = new Regex("^#[0-9A-Fa-f]{6}$", RegexOptions.Compiled);
        private static IReadOnlyDictionary<string, Color> NameToColor { get; }

        static ColorHelper()
        {
            var properties = typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public);
            NameToColor = properties.ToDictionary(p => p.Name, p => (Color)p.GetValue(null));
        }

        public static string GetName(Color color)
        {
            return NameToColor.FirstOrDefault(kv => kv.Value == color).Key
                ?? $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        public static Color? TryFindColor(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            else if (HexRegex.IsMatch(name))
            {
                return HexToColor(name);
            }
            else
            {
                if (NameToColor.TryGetValue(name, out var color)) return color;
                Log.Warning($"Can't find color \"{name}\"");
                return null;
            }
        }

        private static Color HexToColor(string hex)
        {
            var i = Convert.ToInt32(hex.Substring(1), 16);
            return new Color(
                r: (i & RedMask) >> RedShift,
                g: (i & GreenMask) >> GreenShift,
                b: (i & BlueMask) >> BlueShift,
                a: 255);
        }
    }
}