/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewGMCMOptions
**
*************************************************/

// Copyright 2022 Jamie Taylor
﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace GMCMOptions.Framework.UI {
    /// <summary>
    ///   A <c>ColorSlider</c> is a horizontal slider that tracks its value as a <c cref="byte">byte</c>, and renders
    ///   each possible value as a distinct pixel offset on the slider.  (I.e., the selectable range of the slider
    ///   is 255 pixels wide).
    /// </summary>
    public class ColorSlider : ClickAndDragTracking {
        // constants controlling rendering layout
        const int sliderHeight = 5;
        const int handleOverhang = 3;
        const int handleHalfWidth = 2;
        const int handleWidth = 2 * handleHalfWidth + 1;
        public const int height = sliderHeight + handleOverhang * 2;

        // saved values of the constructor args
        readonly Func<byte, Color> GetColor;
        readonly Action<byte> ValueUpdated;

        /// <summary>The current value of this slider</summary>
        public byte Value { get; set; }

        public override int Width => 255 + 2 * handleHalfWidth;
        public override int Height => height;

        /// <summary>
        /// Create a new slider widget.
        /// </summary>
        /// <param name="getColor">A function that returns the color to render for the given value</param>
        /// <param name="initialValue">The initial value of the slider</param>
        /// <param name="valueUpdated">A callback to be invoked when the value changes via user interaction</param>
        public ColorSlider(Func<byte, Color> getColor, byte initialValue, Action<byte> valueUpdated) {
            GetColor = getColor;
            Value = initialValue;
            ValueUpdated = valueUpdated;
        }

        protected override void OnDrag(int mouseX, int mouseY, int drawX, int drawY) {
            int xOffset = mouseX - drawX;
            byte newValue = (byte)Utility.Clamp(xOffset - handleHalfWidth, 0, 255);
            if (newValue != Value) {
                Value = newValue;
                ValueUpdated?.Invoke(Value);
            }
        }

        /// <summary>
        /// Draw this widget at the given position on the screen
        /// </summary>
        public void Draw(SpriteBatch b, int posX, int posY) {
            UpdateMouseState(posX, posY);
            for (int i = 0; i < 256; i++) {
                Color c = GetColor((byte)i);
                Rectangle r = new Rectangle(posX + handleHalfWidth + i, posY + handleOverhang, 1, sliderHeight);
                b.Draw(ColorUtil.Pixel, r, c);
            }
            b.Draw(ColorUtil.Pixel, new Rectangle(posX + Value, posY, handleWidth, height), Color.Black);
        }
    }
}
