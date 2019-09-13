using Microsoft.Xna.Framework;
using System;

namespace FelixDev.StardewMods.FeTK.Framework.Data.Parsers
{
    /// <summary>
    /// Bundles text and color data together.
    /// </summary>
    internal class TextColorInfo
    {
        /// <summary>
        /// Create a new instance of the <see cref="TextColorInfo"/> class.
        /// </summary>
        /// <param name="text">The text with the specified <paramref name="color"/>.</param>
        /// <param name="color">The color of the text.</param>
        /// <exception cref="ArgumentNullException">The specified <paramref name="text"/> is <c>null</c>.</exception>
        public TextColorInfo(string text, Color color)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Color = color;
        }

        /// <summary>The string on which the specified <see cref="Color"/> should be applied to.</summary>
        public string Text { get; }

        /// <summary>The color of the text.</summary>
        public Color Color { get; }
    }
}
