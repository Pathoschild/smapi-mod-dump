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
            // Use ColorTranslator for conversion.
            // ATTENTION: The types System.Drawing.Color and Microsoft.Xna.Framework.Color are incompatible.
            System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(htmlColor);

            return new Color(color.R, color.G, color.B, color.A);
        }

        public static string ColorToHtml(Color color) {
            // Use ColorTranslator for conversion.
            // ATTENTION: The types System.Drawing.Color and Microsoft.Xna.Framework.Color are incompatible.
            return System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B));
        }
    }
}
