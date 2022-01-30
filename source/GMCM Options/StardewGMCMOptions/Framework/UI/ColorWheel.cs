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

namespace GMCMOptions.Framework.UI {
    /// <summary>
    ///   A <c>ColorWheel</c> is a widget rendered as a circle that tracks its current value in polor coordinates
    ///   (i.e., as hue and saturation when the wheel represents the HSV or HSL color space).
    /// </summary>
    public class ColorWheel : ClickAndDragTracking {
        // constants controlling rendering layout
        const int margin = 5;
        const int handleOverhang = 1;

        // saved values of the constructor args
        readonly Func<double, double, Color> GetColor;
        readonly Action<double, double> ValueUpdated;
        readonly int height;

        // additional values computed and cached for convenience in the constructor
        private readonly int radius;
        private readonly int originOffset;

        /// <summary>The current value of hue (angle in the wheel, in radians)</summary>
        /// <value>The current hue, in the range -π..π if set via user click/drag, or whatever value was given
        ///   in <c cref="SetValue(double, double)">SetValue</c></value>
        public double HueRadians { get; private set; }
        /// <summary>The current value of saturation (distance from center as a fraction of the radius)</summary>
        /// <value>The current saturation, in the range 0..1 if set via user click/drag, or whatever value was given
        ///   in <c cref="SetValue(double, double)">SetValue</c></value>
        public double Saturation { get; private set; }

        // current value in Cartesian coordinates.  Should match hue/saturation.
        private int valueX;
        private int valueY;

        public override int Width => height;
        public override int Height => height;

        /// <summary>
        ///   Create a new color wheel widget.  The initial value is <c>(0,0)</c> (the center of the wheel);
        ///   call <c cref="SetValue(double, double)">SetValue</c> on the new object to set its value.
        /// </summary>
        /// <param name="getColor">A function which takes the hue (angle on the wheel in radians) and saturation
        ///   (distance from center as a fraction of the radius, range 0..1) and returns the color to render
        ///   at that point in the wheel</param>
        /// <param name="valueUpdated">A callback to be invoked with the hue (in radians, range -π..π)
        ///   and saturation (range 0..1) when the widget's value is changed via user interaction</param>
        /// <param name="height">The height (and width) of the widget (in pixels)</param>
        public ColorWheel(Func<double, double, Color> getColor, Action<double, double> valueUpdated, int height) {
            GetColor = getColor;
            ValueUpdated = valueUpdated;
            this.height = height;
            int side = height - margin * 2;
            radius = side / 2;
            originOffset = margin + radius;
        }

        /// <summary>
        /// Set the value of this widget (i.e., the position in the wheel)
        /// </summary>
        /// <param name="hueRadians">The hue (angle on the wheel) in radians.</param>
        /// <param name="saturation">The saturation (distance from center of the wheel as a fraction of the radius)
        ///   in the range 0..1</param>
        public void SetValue(double hueRadians, double saturation) {
            HueRadians = hueRadians;
            Saturation = saturation;
            valueX = (int)Math.Round(Math.Cos(hueRadians) * saturation * radius);
            valueY = (int)Math.Round(Math.Sin(hueRadians) * saturation * radius);
        }

        protected override void OnDrag(int mouseX, int mouseY, int drawX, int drawY) {
            int x = mouseX - drawX - originOffset;
            int y = mouseY - drawY - originOffset;
            double saturation = Math.Sqrt((x * x) + (y * y)) / radius;
            double hueRadians = Math.Atan2(y, x);
            if (saturation > 1.0) saturation = 1.0;
            int newX = (int)Math.Round(Math.Cos(hueRadians) * saturation * radius);
            int newY = (int)Math.Round(Math.Sin(hueRadians) * saturation * radius);
            if (newX != valueX || newY != valueY) {
                HueRadians = hueRadians;
                Saturation = saturation;
                valueX = newX;
                valueY = newY;
                ValueUpdated?.Invoke(hueRadians, saturation);
            }
        }

        /// <summary>
        /// Draw this widget at the given position on the screen
        /// </summary>
        public void Draw(SpriteBatch b, int posX, int posY) {
            UpdateMouseState(posX, posY);
            for (int x = 1 - radius; x < radius; x++) {
                for (int y = 1 - radius; y < radius; y++) {
                    double saturation = Math.Sqrt((x * x) + (y * y)) / radius;
                    double hueRadians = Math.Atan2(y, x);
                    if (saturation <= 1) {
                        Color c = GetColor(hueRadians, saturation);
                        Rectangle r = new Rectangle(posX + originOffset + x, posY + originOffset + y, 1, 1);
                        b.Draw(ColorUtil.Pixel, r, c);
                    }
                }
            }
            int handleX = posX + originOffset + valueX - handleOverhang;
            int handleY = posY + originOffset + valueY - handleOverhang;
            int handleSide = 2 * handleOverhang + 1;
            b.Draw(ColorUtil.Pixel, new Rectangle(handleX, handleY, handleSide, handleSide), Color.Black);
        }

    }
}
