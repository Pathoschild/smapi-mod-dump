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
    public class SeparatorOption {
        const int shadowOffset = 2;
        // saved values from the constructor
        readonly Func<double> GetWidth;
        readonly int height;
        readonly int padAbove;
        readonly int padBelow;
        readonly int alignment;
        readonly Func<Color> GetColor;
        readonly Func<Color> GetShadowColor;

        public SeparatorOption(Func<double>? getWidth = null,
                               int height = 3,
                               int padAbove = 0,
                               int padBelow = 0,
                               int alignment = 0,
                               Func<Color>? getColor = null,
                               Func<Color>? getShadowColor = null) {
            this.GetWidth = getWidth ?? (() => 0.85);
            this.height = height;
            this.padAbove = padAbove;
            this.padBelow = padBelow;
            this.alignment = alignment;
            this.GetColor = getColor ?? (() => Game1.textColor);
            this.GetShadowColor = getShadowColor ?? (() => Game1.textShadowColor);
        }

        public int OptionHeight() => padAbove + height + (GetShadowColor() == Color.Transparent ? 0 : shadowOffset) + padBelow;

        /// <summary>
        /// Draw this Option at the given position on the screen
        /// </summary>
        public void Draw(SpriteBatch b, Vector2 pos) {
            int top = (int)(pos.Y + padAbove);
            int gmcmWidth = Math.Min(1200, Game1.uiViewport.Width - 200);
            int gmcmLeft = (Game1.uiViewport.Width - gmcmWidth) / 2;
            int width = (int)(gmcmWidth * GetWidth());
            int left = gmcmLeft;
            if (alignment == 0) {
                left += (int)(gmcmWidth - width) / 2;
            } else if (alignment > 0) {
                left += (int)(gmcmWidth - width);
            }
            b.Draw(ColorUtil.Pixel, new Rectangle(left - shadowOffset, top + shadowOffset, width, height), GetShadowColor());
            b.Draw(ColorUtil.Pixel, new Rectangle(left, top, width, height), GetColor());
        }
    }
}

