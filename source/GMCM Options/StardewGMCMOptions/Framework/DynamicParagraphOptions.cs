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
    public class DynamicParagraphOption {
        private readonly Func<string> getText;
        private readonly ITextLayoutEngine textLayoutEngine;
        /// <summary>width available inside the GMCM window</summary>
        private int gmcmWidth = 0;
        /// <summary>Game1.smallFont from the last time the layout was computed</summary>
        private SpriteFont? lastFont;
        /// <summary>the text over which the remaining fields were computed</summary>
        private string lastText = "";

        private int height = 0;

        // vestigial options - not currently set-able, but things will work as expected if they are set
        const int padAbove = 0;
        const int padBelow = 0;

        public DynamicParagraphOption(Func<string> getText, ITextLayoutEngine textLayoutEngine) {
            this.getText = getText;
            this.textLayoutEngine = textLayoutEngine;
            UpdateText();
        }

        /// <summary>Get the current text, and if it or gmcmWidth or Game1.smallFont have changed then recompute the layout.</summary>
        private void UpdateText() {
            string text = getText();
            int gmcmWidth = Math.Min(1200, Game1.uiViewport.Width - 200);
            if (text == lastText && this.gmcmWidth == gmcmWidth && lastFont == Game1.smallFont) {
                return;
            }
            lastText = text;
            this.gmcmWidth = gmcmWidth;
            lastFont = Game1.smallFont;
            height = textLayoutEngine.Layout(text, gmcmWidth);
        }

        public int OptionHeight() {
            UpdateText();
            return padAbove + height + padBelow;
        }

        /// <summary>
        /// Draw this Option at the given position on the screen
        /// </summary>
        public void Draw(SpriteBatch b, Vector2 pos) {
            UpdateText();  // probably just called while getting Height, but do it again just in case
            int top = (int)(pos.Y + padAbove);
            int gmcmLeft = (Game1.uiViewport.Width - gmcmWidth) / 2;
            int left = gmcmLeft;
            textLayoutEngine.DrawLastLayout(b, left, top);
        }
    }
}

