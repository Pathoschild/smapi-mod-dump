/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mus-candidus/ImagEd
**
*************************************************/

using System;
using System.IO;
using System.Globalization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewValley;


namespace ImagEd.Framework {
    internal static class Utility {
        /// <summary>Returns IContentPack from IModInfo.</summary>
        public static IContentPack GetContentPackFromModInfo(IModInfo modInfo) {
            if (!modInfo.IsContentPack) {
                throw new ArgumentException($"{modInfo.Manifest.UniqueID} is not a content pack");
            }

            // ContentPack is a property of the internal interface IModMetadata
            // which is derived from IModInfo. Access it via reflection.
            return modInfo.GetType().GetProperty("ContentPack").GetValue(modInfo) as IContentPack;
        }

        /// <summary>Inserts a suffix before file extension.</summary>
        public static string AddFileNameSuffix(string fileName, string suffix) {
            string directory = Path.GetDirectoryName(fileName);
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);

            return Path.Combine(directory, $"{fileNameWithoutExt}_{suffix}{extension}");
        }

        /// <summary>Converts a texture to 2D pixel array.</summary>
        public static Color[] TextureToArray(Texture2D texture) {
            Color[] pixels = new Color[texture.Width * texture.Height];
            texture.GetData(pixels);

            return pixels;
        }

        /// <summary>Converts a 2D pixel array to texture.</summary>
        public static Texture2D ArrayToTexture(Color[] pixels, int width, int height) {
            int expectedLength = width * height;
            if (pixels.Length != expectedLength) {
                throw new ArgumentException($"Wrong number of pixels in array, expected {expectedLength}");
            }

            Texture2D texture = new Texture2D(Game1.graphics.GraphicsDevice, width, height);
            texture.SetData(pixels);

            return texture;
        }

        public static Color ColorFromHtml(string htmlColor) {
            // ATTENTION: System.Drawing.ColorTranslator doesn't support hex colors with transparency
            // thus we need our own implementation.
            int htmlColorLength = htmlColor.Length;

            if (!(htmlColorLength == 7 || htmlColorLength == 9) && !htmlColor.StartsWith("#")) {
                throw new ArgumentException($"Invalid color {htmlColor}, format must be '#RRGGBB' or '#RRGGBBAA'");
            }

            byte R = byte.Parse(htmlColor.Substring(1, 2), NumberStyles.HexNumber);
            byte G = byte.Parse(htmlColor.Substring(3, 2), NumberStyles.HexNumber);
            byte B = byte.Parse(htmlColor.Substring(5, 2), NumberStyles.HexNumber);
            byte A = htmlColor.Length == 9
                   ? byte.Parse(htmlColor.Substring(7, 2), NumberStyles.HexNumber)
                   : (byte) 0xFF;

            return new Color(R, G, B, A);
        }

        public static string ColorToHtml(Color color) {
            // ATTENTION: System.Drawing.ColorTranslator doesn't support hex colors with transparency
            // thus we need our own implementation.
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}{color.A:X2}";
        }
    }
}
