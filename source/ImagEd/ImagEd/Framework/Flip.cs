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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace ImagEd.Framework {
    /// <summary>
    /// This class provides methods to flip images.
    /// </summary>
    internal class Flip {
        public enum Mode {
            // No flip.
            None,
            // Flip horizontally.
            FlipHorizontally,
            // Flip vertically.
            FlipVertically,
             //Flip both.
            FlipBoth
        }

        public static Mode ParseEnum(string flip) {
            return Enum.TryParse<Mode>(flip, true, out Mode mode) ? mode : Mode.None;
        }

        /// <summary>Flips an image horizontally.</summary>
        public static Texture2D FlipHorizontally(Texture2D source) {
            Color[] sourcePixels = Utility.TextureToArray(source);
            Color[] flippedPixels = new Color[source.Width * source.Height];

            for (int j = 0; j < source.Height; j++) {
                for (int i = 0; i < source.Width; i++) {
                    // Flip horizontally.
                    int lineBegin = j * source.Width;
                    flippedPixels[lineBegin + i] = sourcePixels[lineBegin + source.Width - i - 1];
                }
            }

            Texture2D flipped = Utility.ArrayToTexture(flippedPixels, source.Width, source.Height);

            return flipped;
        }

        /// <summary>Flips an image vertically.</summary>
        public static Texture2D FlipVertically(Texture2D source) {
            Color[] sourcePixels = Utility.TextureToArray(source);
            Color[] flippedPixels = new Color[source.Width * source.Height];

            for (int j = 0; j < source.Height; j++) {
                // Flip vertically.
                Array.Copy(sourcePixels, j * source.Width, flippedPixels, (source.Height - j - 1) * source.Width, source.Width);
            }

            Texture2D flipped = Utility.ArrayToTexture(flippedPixels, source.Width, source.Height);

            return flipped;
        }
    }
}