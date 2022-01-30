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
using BitOperations = System.Numerics.BitOperations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using GMCMOptions.Framework.UI;

namespace GMCMOptions.Framework {
    #pragma warning disable format
    /// <summary>
    /// Flags to control how the <c cref="ColorPickerOption">ColorPickerOption</c> widget is displayed.
    /// </summary>
    [Flags]
    public enum ColorPickerStyle : uint {
        Default = 0,
        RGBSliders    = 0b00000001,
        HSVColorWheel = 0b00000010,
        HSLColorWheel = 0b00000100,
        AllStyles     = 0b11111111,
        NoChooser     = 0,
        RadioChooser  = 0b01 << 8,
        ToggleChooser = 0b10 << 8

    }
    #pragma warning restore format

    /// <summary>
    /// A widget for selecting a Color that can be used as a GMCM complex option.
    /// </summary>
    public class ColorPickerOption {
        // constants controlling rendering layout
        const int colorBoxInnerSize = 60;
        const int colorBoxBorder = 12;
        const int colorBoxOuterSize = colorBoxInnerSize + 2 * colorBoxBorder;
        const int colorBoxOffset = 300;
        const int checkerboardSize = 5;
        const int sliderSpacing = 10;

        // textures for the style picker icon buttons

        static readonly Texture2D checkerboard = ColorUtil.CreateCheckerboardTexture(colorBoxInnerSize, checkerboardSize);
        static readonly Texture2D smallColorWheel = ColorUtil.CreateColowWheelTexture(13);
        static readonly Texture2D rgbStripe = CreateRgbStripe();

        static Texture2D CreateRgbStripe() {
            Texture2D rgbStripe = new Texture2D(Game1.graphics.GraphicsDevice, 24, 24);
            Color[] rgbData = new Color[24 * 24];
            for (int row = 0; row < 8; row++) {
                for (int col = 0; col < 24; col++) {
                    rgbData[row * 24 + col] = Color.Red;
                    rgbData[(row + 8) * 24 + col] = Color.Green;
                    rgbData[(row + 16) * 24 + col] = Color.Blue;
                }
            }
            rgbStripe.SetData(rgbData);
            return rgbStripe;
        }

        // saved values from the constructor
        readonly Func<Color> GetValue;
        readonly Action<Color> SetValue;
        readonly bool ShowAlpha;
        readonly bool ShowStylePicker;
        readonly ColorPickerStyle EffectiveStyle;

        // UI widgets

        readonly IconButton RGBStyleButton;
        readonly IconButton HSVStyleButton;
        readonly IconButton HSLStyleButton;
        readonly List<IconButton> StyleButtons;

        readonly ColorSlider sliderR;
        readonly ColorSlider sliderG;
        readonly ColorSlider sliderB;
        readonly ColorSlider sliderA;

        readonly ColorWheel hsvWheel;
        readonly VerticalSlider vSlider;

        readonly ColorWheel hslWheel;
        readonly VerticalSlider lSlider;

        // Our current value
        private Color currentValue;

        /// <summary>
        /// Create a new color picker Option.
        /// </summary>
        /// <param name="getValue">A function that returns the current value in the underlying configuration object</param>
        /// <param name="setValue">A function that should save the given value to the underlying configuration object</param>
        /// <param name="showAlpha">Whether a slider should be shown for setting the Alpha channel or not</param>
        /// <param name="style">Specify which types of color picker to show</param>
        public ColorPickerOption(Func<Color> getValue, Action<Color> setValue, bool showAlpha = true, ColorPickerStyle style = 0) {
            GetValue = getValue;
            SetValue = setValue;
            ShowAlpha = showAlpha;
            currentValue = getValue();

            if (style == ColorPickerStyle.Default) {
                style = ColorPickerStyle.AllStyles | ColorPickerStyle.ToggleChooser;
            }
            EffectiveStyle = style;

            ShowStylePicker = (style.HasFlag(ColorPickerStyle.RadioChooser) || style.HasFlag(ColorPickerStyle.ToggleChooser))
                && BitOperations.PopCount((uint)(style & ColorPickerStyle.AllStyles)) > 1;
            Action<IconButton> styleButtonClick = style.HasFlag(ColorPickerStyle.ToggleChooser) ? IconButton.ToggleSelected : StyleButtonRadio;
            bool defaultSelected = !style.HasFlag(ColorPickerStyle.RadioChooser);
            RGBStyleButton = new IconButton(rgbStripe, null, "RGB", styleButtonClick, defaultSelected && style.HasFlag(ColorPickerStyle.RGBSliders));
            HSVStyleButton = new IconButton(smallColorWheel, null, "HSV", styleButtonClick, defaultSelected && style.HasFlag(ColorPickerStyle.HSVColorWheel));
            HSLStyleButton = new IconButton(smallColorWheel, null, "HSL", styleButtonClick, defaultSelected && style.HasFlag(ColorPickerStyle.HSLColorWheel));
            StyleButtons = new List<IconButton>();
            if (style.HasFlag(ColorPickerStyle.RGBSliders)) StyleButtons.Add(RGBStyleButton);
            if (style.HasFlag(ColorPickerStyle.HSVColorWheel)) StyleButtons.Add(HSVStyleButton);
            if (style.HasFlag(ColorPickerStyle.HSLColorWheel)) StyleButtons.Add(HSLStyleButton);
            if (ShowStylePicker && style.HasFlag(ColorPickerStyle.RadioChooser)) {
                StyleButtons[0].Selected = true;
            }

            sliderR = new ColorSlider((b) => new Color((int)b, 0, 0, 255), currentValue.R, (b) => { currentValue.R = b; RGBChanged(); });
            sliderG = new ColorSlider((b) => new Color(0, (int)b, 0, 255), currentValue.G, (b) => { currentValue.G = b; RGBChanged(); });
            sliderB = new ColorSlider((b) => new Color(0, 0, (int)b, 255), currentValue.B, (b) => { currentValue.B = b; RGBChanged(); });
            sliderA = new ColorSlider((b) => new Color(0, 0, 0, (int)b), currentValue.A, (b) => { currentValue.A = b; });

            hsvWheel = new ColorWheel((h, s) => ColorUtil.FromHSV(h, s, vSlider.Value), HSVWheelChanged, 150);
            vSlider = new VerticalSlider((v) => ColorUtil.FromHSV(hsvWheel.HueRadians, hsvWheel.Saturation, v), VSliderChanged, 150);
            hslWheel = new ColorWheel((h, s) => ColorUtil.FromHSL(h, s, lSlider.Value), HSLWheelChanged, 150);
            lSlider = new VerticalSlider((v) => ColorUtil.FromHSL(hslWheel.HueRadians, hslWheel.Saturation, v), LSliderChanged, 150);
            ResetHSVWheel();
            ResetHSLWheel();
        }

        /// <summary>
        /// Invoke the <c>setValue</c> callback passed to the constructor with this option's current state.
        /// </summary>
        public void SaveChanges() {
            SetValue(currentValue);
        }

        /// <summary>
        /// Reset this option's current state by fetching the current value from the <c>getValue</c> argument passed
        /// to the constructor.
        /// </summary>
        public void Reset() {
            currentValue = GetValue();
            ResetSliders();
            ResetHSVWheel();
            ResetHSLWheel();
        }

        private void StyleButtonRadio(IconButton btn) {
            foreach (IconButton x in StyleButtons) {
                x.Selected = x == btn;
            }
        }

        private void ResetSliders() {
            sliderR.Value = currentValue.R;
            sliderB.Value = currentValue.B;
            sliderG.Value = currentValue.G;
            sliderA.Value = currentValue.A;
        }

        private void ResetHSVWheel() {
            var (hueRadians, saturation, value) = ColorUtil.ToHSVRadians(currentValue);
            hsvWheel.SetValue(hueRadians, saturation);
            vSlider.SetValue(value);
        }
        private void ResetHSLWheel() {
            var (hueRadians, saturation, lightness) = ColorUtil.ToHSLRadians(currentValue);
            hslWheel.SetValue(hueRadians, saturation);
            lSlider.SetValue(lightness);
        }

        private void RGBChanged() {
            ResetHSVWheel();
            ResetHSLWheel();
        }

        private void HSVWheelChanged(double hueRadians, double saturation) {
            Color newValue = ColorUtil.FromHSV(hueRadians, saturation, vSlider.Value);
            newValue.A = currentValue.A;
            currentValue = newValue;
            ResetSliders();
            ResetHSLWheel();
        }
        private void HSLWheelChanged(double hueRadians, double saturation) {
            Color newValue = ColorUtil.FromHSL(hueRadians, saturation, lSlider.Value);
            newValue.A = currentValue.A;
            currentValue = newValue;
            ResetSliders();
            ResetHSVWheel();
        }

        private void VSliderChanged(double value) {
            Color newValue = ColorUtil.FromHSV(hsvWheel.HueRadians, hsvWheel.Saturation, value);
            newValue.A = currentValue.A;
            currentValue = newValue;
            ResetSliders();
            ResetHSLWheel();
        }

        private void LSliderChanged(double lightness) {
            Color newValue = ColorUtil.FromHSL(hslWheel.HueRadians, hslWheel.Saturation, lightness);
            newValue.A = currentValue.A;
            currentValue = newValue;
            ResetSliders();
            ResetHSVWheel();
        }

        /// <summary>
        /// Return the maxiumum height that this option can occupy.
        /// </summary>
        /// <returns>Height in pixels</returns>
        public int Height() {
            int top = sliderSpacing;
            if (ShowStylePicker) {
                int height = 0;
                foreach (IconButton btn in StyleButtons) height = Math.Max(height, btn.Height);
                top += height + sliderSpacing;
            }
            if (ShowAlpha) top += (ColorSlider.height + sliderSpacing);
            if (EffectiveStyle.HasFlag(ColorPickerStyle.RadioChooser)) {
                // showing at most one of these anyway.
                int height = 0;
                if (EffectiveStyle.HasFlag(ColorPickerStyle.RGBSliders)) height = Math.Max(height, 3 * (ColorSlider.height + sliderSpacing));
                if (EffectiveStyle.HasFlag(ColorPickerStyle.HSVColorWheel)) height = Math.Max(height, hsvWheel.Height);
                if (EffectiveStyle.HasFlag(ColorPickerStyle.HSLColorWheel)) height = Math.Max(height, hslWheel.Height);
                top += height;
            } else {
                if (EffectiveStyle.HasFlag(ColorPickerStyle.RGBSliders)) top += 3 * (ColorSlider.height + sliderSpacing);
                if (EffectiveStyle.HasFlag(ColorPickerStyle.HSVColorWheel) && EffectiveStyle.HasFlag(ColorPickerStyle.HSLColorWheel)) top = Math.Max(top, colorBoxOuterSize + sliderSpacing);
                if (EffectiveStyle.HasFlag(ColorPickerStyle.HSVColorWheel) || EffectiveStyle.HasFlag(ColorPickerStyle.HSLColorWheel)) top += Math.Max(hsvWheel.Height, hslWheel.Height);
            }

            return top;
        }

        private int DrawStyleSelector(SpriteBatch b, int posX, int posY) {
            const int buttonSpacing = 10;
            int height = 0;
            int left = posX;

            foreach (IconButton btn in StyleButtons) {
                btn.Draw(b, left, posY);
                left += btn.Width + buttonSpacing;
                height = Math.Max(height, btn.Height);
            }
            return height;
        }

        /// <summary>
        /// Draw this Option at the given position on the screen
        /// </summary>
        public void Draw(SpriteBatch b, Vector2 pos) {
            int left = (int)pos.X;
            int top = (int)pos.Y;
            IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), left + colorBoxOffset, top, colorBoxOuterSize, colorBoxOuterSize, Color.White, 1f, false);
            var colorBox = new Rectangle(left + colorBoxOffset + colorBoxBorder, top + colorBoxBorder, colorBoxInnerSize, colorBoxInnerSize);
            b.Draw(checkerboard, colorBox, Color.White);
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, Utility.ScissorEnabled);
            b.Draw(ColorUtil.Pixel, colorBox, currentValue);
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, Utility.ScissorEnabled);

            top += sliderSpacing;

            if (ShowStylePicker) {
                top += DrawStyleSelector(b, left, top);
                top += sliderSpacing;
            }
            if (RGBStyleButton.Selected) {
                sliderR.Draw(b, left, top);
                top += ColorSlider.height + sliderSpacing;
                sliderG.Draw(b, left, top);
                top += ColorSlider.height + sliderSpacing;
                sliderB.Draw(b, left, top);
                top += ColorSlider.height + sliderSpacing;
            }
            if (ShowAlpha) {
                sliderA.Draw(b, left, top);
                top += ColorSlider.height + sliderSpacing;
            }
            if (HSVStyleButton.Selected && HSLStyleButton.Selected) top = Math.Max(top, (int)pos.Y + colorBoxOuterSize + sliderSpacing);
            if (HSVStyleButton.Selected) {
                hsvWheel.Draw(b, left, top);
                vSlider.Draw(b, left + hsvWheel.Width + 10, top);
                left += hsvWheel.Width + 20 + vSlider.Width;
            }
            if (HSLStyleButton.Selected) {
                hslWheel.Draw(b, left, top);
                lSlider.Draw(b, left + hslWheel.Width + 10, top);
            }

        }
    }
}
