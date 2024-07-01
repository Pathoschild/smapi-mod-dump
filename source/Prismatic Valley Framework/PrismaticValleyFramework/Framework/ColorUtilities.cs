/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jolly-Alpaca/PrismaticValleyFramework
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace PrismaticValleyFramework.Framework
{
    static class ColorUtilities
    {
        public static Dictionary<string, List<Color>> PrismaticPalettes = new ();

        /// <summary>
        /// StardewValley.Utility.GetPrismaticColor refactored to take a color array instead of referencing a class variable.
        /// </summary>
        /// <param name="PrismaticColors">The list of MonoGame colors to use to create the prismatic effect</param>
        /// <param name="offset">The value to apply when getting a starting color from the color list</param>
        /// <param name="speedMultiplier">The speed to transition between the colors of the color list when simulating the prismatic effect</param>
        /// <returns></returns>
        public static Color GetCustomPrismaticColor(List<Color> PrismaticColors, int offset = 0, float speedMultiplier = 1f)
        {
            float num = 1500f;
            int num2 = ((int)((float)Game1.currentGameTime.TotalGameTime.TotalMilliseconds * speedMultiplier / num) + offset) % PrismaticColors.Count;
            int num3 = (num2 + 1) % PrismaticColors.Count;
            float t = (float)Game1.currentGameTime.TotalGameTime.TotalMilliseconds * speedMultiplier / num % 1f;
            Color result = default(Color);
            result.R = (byte)(Utility.Lerp((float)(int)PrismaticColors[num2].R / 255f, (float)(int)PrismaticColors[num3].R / 255f, t) * 255f);
            result.G = (byte)(Utility.Lerp((float)(int)PrismaticColors[num2].G / 255f, (float)(int)PrismaticColors[num3].G / 255f, t) * 255f);
            result.B = (byte)(Utility.Lerp((float)(int)PrismaticColors[num2].B / 255f, (float)(int)PrismaticColors[num3].B / 255f, t) * 255f);
            result.A = (byte)(Utility.Lerp((float)(int)PrismaticColors[num2].A / 255f, (float)(int)PrismaticColors[num3].A / 255f, t) * 255f);
            return result;
        }

        /// <summary>
        /// Get a MonoGame color from a string representation
        /// </summary>
        /// <param name="colorString">The raw color value to parse. This can be Prismatic, 
        /// a Microsoft.Xna.Framework.Color property name (like SkyBlue), RGB or RGBA hex code 
        /// (like #AABBCC or #AABBCCDD), or 8-bit RGB or RGBA code (like 34 139 34 or 34 139 34 255).</param>
        /// <param name="paletteString">The common delimited list of colors to use to create the prismatic effect</param>
        /// <returns>The matching color. Default: Color.White</returns>
        public static Color getColorFromString(string colorString, string paletteString = "")
        {
            switch (colorString) {
                case "Prismatic": return Utility.GetPrismaticColor();
                case "Custom Palette": return GetCustomColorFromPalette(paletteString);
                // Call the Stardew Valley's existing StringToColor method to handle other input
                default: return Utility.StringToColor(colorString) ?? Color.White;
            }
        } 

        /// <summary>
        /// Get a MonoGame color from a custom prismatic effect
        /// </summary>
        /// <param name="paletteString">The common delimited list of colors to use to create the prismatic effect</param>
        /// <returns>The current color in the prismatic effect. Default: Color.White</returns>
        public static Color GetCustomColorFromPalette(string paletteString = "")
        {
            if (paletteString.Equals("")) return Color.White;
            
            // Pull the parsed Color array from the dictionary if it has already been parsed
            if (PrismaticPalettes.GetValueOrDefault(paletteString) is List<Color> colorPalette)
                return GetCustomPrismaticColor(colorPalette);
           
            // Else, parse paletteString into a Color array and add it to the prismatic palettes dictionary
            var colorStrings = paletteString.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            if (!colorStrings.Any()) return Color.White;
            colorPalette = new List<Color>();
            colorStrings.ForEach(colorString => colorPalette.Add(Utility.StringToColor(colorString) ?? Color.White));
            PrismaticPalettes[paletteString] = colorPalette;
            return GetCustomPrismaticColor(colorPalette);
        }
        
        /// <summary>
        /// Applies a tint to a color by multiplying the tint color on the base color.
        /// </summary>
        /// <param name="baseColor">The base color to be tinted</param>
        /// <param name="tintColor">The tint color to apply to the base color</param>
        /// <returns>The tinted base color</returns>
        public static Color getTintedColor(Color baseColor, Color tintColor)
        {
            Color tintedColor = default(Color);
            // Equivalent to color.R/255 to get the float value, multiply the two floats together, then multiple by 255 to convert back to byte
            tintedColor.R = (byte)(baseColor.R * tintColor.R / 255f);
            tintedColor.G = (byte)(baseColor.G * tintColor.G / 255f);
            tintedColor.B = (byte)(baseColor.B * tintColor.B / 255f);
            tintedColor.A = (byte)(baseColor.A * tintColor.A / 255f);
            return tintedColor;
        }
    }
}