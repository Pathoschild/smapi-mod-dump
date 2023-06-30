/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewGMCMOptions
**
*************************************************/

// Copyright 2023 Jamie Taylor
using System;
using Microsoft.Xna.Framework.Graphics;

namespace GMCMOptions.Framework {
    public interface ITextLayoutEngine {
        /// <summary>
        ///   Compute the layout for the given text and wrap width, returning the total height used.
        /// </summary>
        /// <param name="text">The text to lay out.</param>
        /// <param name="width">The wrap width.</param>
        /// <returns>The height used by the layout.</returns>
        int Layout(string text, int width);

        /// <summary>
        ///   Draw the layout computed in the most recent call to <c cref="Layout(string, int)">Layout</c>.
        /// </summary>
        /// <param name="b">The SpriteBatch in which to draw.</param>
        /// <param name="left">The left pixel coordinate of the draw position.</param>
        /// <param name="top">The top pixel coordinate of the draw position.</param>
        void DrawLastLayout(SpriteBatch b, int left, int top);
    }
}

