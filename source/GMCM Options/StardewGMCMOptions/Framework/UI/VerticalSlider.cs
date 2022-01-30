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
    ///   A <c>VerticalSlider</c> is a vertical slider that tracks its value as a fraction of its height.  Each
    ///   horizontal line of pixels in the slider can be a different color.
    /// </summary>
    public class VerticalSlider : ClickAndDragTracking {
        // constants controlling rendering layout
        const int sliderWidth = 5;
        const int handleOverhang = 3;
        const int handleHalfHeight = 2;
        const int handleHeight = handleHalfHeight * 2 + 1;
        public const int width = sliderWidth + handleOverhang * 2;

        // saved values of the constructor args
        readonly Func<double, Color> GetColor;
        readonly Action<double> ValueUpdated;
        readonly int height;

        // additional values computed and cached for convenience in the constructor
        private readonly int sliderHeight;

        /// <summary>The current value of this slider</summary>
        public double Value { get; private set; }
        // current value as pixel offset
        private int valueY;

        public override int Width => width;
        public override int Height => height;

        /// <summary>
        /// Create a new vertical slider widget.  The initial value is <c>0</c>;
        ///   call <c cref="SetValue(double)">SetValue</c> on the new object to set its value.
        /// </summary>
        /// <param name="getColor">A function that returns the color to render for the given value</param>
        /// <param name="valueUpdated">A callback to be invoked when the value changes via user interaction</param>
        /// <param name="height">The height (and width) of the widget (in pixels)</param>
        public VerticalSlider(Func<double, Color> getColor, Action<double> valueUpdated, int height) {
            GetColor = getColor;
            ValueUpdated = valueUpdated;
            this.height = height;
            sliderHeight = height - handleHalfHeight * 2;
        }

        /// <summary>Set the value of the widget</summary>
        /// <param name="v">The new value of the widget (range 0..1)</param>
        public void SetValue(double v) {
            Value = v;
            valueY = (int)Math.Round(sliderHeight * (1d - v));
        }

        protected override void OnDrag(int mouseX, int mouseY, int drawX, int drawY) {
            int yOffset = mouseY - drawY;
            int newValue = Utility.Clamp(yOffset - handleHalfHeight, 0, sliderHeight);
            if (newValue != valueY) {
                valueY = newValue;
                Value = (double)(sliderHeight - valueY) / sliderHeight;
                ValueUpdated?.Invoke(Value);
            }
        }

        /// <summary>
        /// Draw this widget at the given position on the screen
        /// </summary>
        public void Draw(SpriteBatch b, int posX, int posY) {
            UpdateMouseState(posX, posY);
            for (int i = 0; i < sliderHeight; i++) {
                Color c = GetColor((double)i / sliderHeight);
                Rectangle r = new Rectangle(posX + handleOverhang, posY + Height - handleHalfHeight - i, sliderWidth, 1);
                b.Draw(ColorUtil.Pixel, r, c);
            }
            b.Draw(ColorUtil.Pixel, new Rectangle(posX, posY + valueY, width, handleHeight), Color.Black);
        }
    }
}
