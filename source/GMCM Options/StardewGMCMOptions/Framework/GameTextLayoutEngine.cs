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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace GMCMOptions.Framework {
    public class GameTextLayoutEngine : ITextLayoutEngine {
        private string formattedText = "";
        public GameTextLayoutEngine() {
        }

        /// <inheritdoc/>
        public int Layout(string text, int width) {
            formattedText = Game1.parseText(text, Game1.smallFont, width);
            return (int)Game1.smallFont.MeasureString(formattedText).Y;
        }

        /// <inheritdoc/>
        public void DrawLastLayout(SpriteBatch b, int left, int top) {
            b.DrawString(Game1.smallFont, formattedText, new Vector2(left, top), Color.Black);
        }

    }
}

