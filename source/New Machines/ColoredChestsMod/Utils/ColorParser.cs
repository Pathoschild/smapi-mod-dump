using System.Reflection;
using System.Text.RegularExpressions;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Microsoft.Xna.Framework;

namespace Igorious.StardewValley.ColoredChestsMod.Utils
{
    public static class ColorParser
    {
        public static Color? Parse(string colorName)
        {
            if (Regex.Match(colorName, @"#?[\dA-Fa-f]{6}").Success)
            {
                return RawColor.FromHex(colorName.Substring(1)).ToXnaColor();
            }
            else
            {
                var propertyInfo = typeof(Color).GetProperty(colorName, BindingFlags.Static | BindingFlags.Public);
                return (Color?)propertyInfo?.GetValue(null);
            }
        }
    }
}