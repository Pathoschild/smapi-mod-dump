using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace PondPainter
{
    // I need this to lookup color names without having to go through System.Drawing
    // From John Skeet https://stackoverflow.com/questions/3391462/convert-string-to-color-in-c-sharp
    public static class ColorLookup
    {
        private static readonly Dictionary<string, Color> dictionary =
        typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static)
                    .Where(prop => prop.PropertyType == typeof(Color))
                    .ToDictionary(prop => prop.Name,
                                  prop => (Color)prop.GetValue(null, null));

        public static Color FromName(string name)
        {
            if (name.StartsWith("#"))
            {
                return FromHex(name.TrimStart('#'));
            }
            if (dictionary.ContainsKey(name))
            {
                return dictionary[name];
            }
            else
            {
                PondPainterEntry.Instance.Log($"Can't understand color name {name}. Must be a valid XNA color. See http://www.foszor.com/blog/xna-color-chart/ for a list.", StardewModdingAPI.LogLevel.Warn);
                return Color.White;
            }
        }

        public static Color FromHex(string hex)
        {
            if (hex.Length == 6)
            {
                return new Color(int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber),
                                 int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber),
                                 int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber));
            }
            else if (hex.Length == 3)
            {
                return new Color(17 * int.Parse(hex.Substring(0, 1), NumberStyles.HexNumber),
                                 17 * int.Parse(hex.Substring(1, 1), NumberStyles.HexNumber),
                                 17 * int.Parse(hex.Substring(2, 1), NumberStyles.HexNumber));
            }
            else
            {
                PondPainterEntry.Instance.Log($"Can't understand color name #{hex}. Hex codes must be 3 or 6 characters", StardewModdingAPI.LogLevel.Warn);
            }
            return Color.White;
        }
    }
}
