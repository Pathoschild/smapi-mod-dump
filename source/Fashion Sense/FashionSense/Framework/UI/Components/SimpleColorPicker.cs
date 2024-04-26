/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;

namespace FashionSense.Framework.UI.Components
{
    public class SimpleColorPicker
    {
        private Rectangle bounds;

        public SimpleSlider hueSlider;
        public SimpleSlider lightnessSlider;
        public SimpleSlider saturationSlider;
        public SimpleSlider recentSliderBar;

        public const int MIN_LIGHTNESS = 0;
        public const int MAX_LIGHTNESS = 100;


        public SimpleColorPicker(int x, int y)
        {
            hueSlider = new SimpleSlider(new Rectangle(x, y + 5, SliderBar.defaultWidth, 10), 0, 360) { getDrawColor = ((float val) => GetColorForValues(val, 100f)) };
            saturationSlider = new SimpleSlider(new Rectangle(x, y + 25, SliderBar.defaultWidth, 10), 0, 100) { getDrawColor = ((float val) => GetColorForValues(hueSlider.GetValue(), val)) };
            lightnessSlider = new SimpleSlider(new Rectangle(x, y + 45, SliderBar.defaultWidth, 10), MIN_LIGHTNESS, MAX_LIGHTNESS) { getDrawColor = ((float val) => GetColorForValues(hueSlider.GetValue(), saturationSlider.GetValue(), val)) };

            bounds = new Rectangle(x, y, SliderBar.defaultWidth, 60);
        }

        public void KeyPress(Keys key)
        {
            if (hueSlider.input.Selected is true)
            {
                hueSlider.ReceiveKeyPress(key);
            }
            if (saturationSlider.input.Selected is true)
            {
                saturationSlider.ReceiveKeyPress(key);
            }
            if (lightnessSlider.input.Selected is true)
            {
                lightnessSlider.ReceiveKeyPress(key);
            }
        }

        public void Scroll(int delta)
        {
            if (recentSliderBar != null)
            {
                if (recentSliderBar.Equals(hueSlider))
                {
                    hueSlider.ApplyMovementKey(delta > 0 ? 1 : -1);
                }
                if (recentSliderBar.Equals(saturationSlider))
                {
                    saturationSlider.ApplyMovementKey(delta > 0 ? 1 : -1);
                }
                if (recentSliderBar.Equals(lightnessSlider))
                {
                    lightnessSlider.ApplyMovementKey(delta > 0 ? 1 : -1);
                }
            }
            else
            {
                hueSlider.ApplyMovementKey(delta > 0 ? 1 : -1);
            }
        }

        public bool Click(int x, int y)
        {
            hueSlider.input.Update();
            saturationSlider.input.Update();
            lightnessSlider.input.Update();

            recentSliderBar = null;
            if (bounds.Contains(x, y))
            {
                if (hueSlider.ReceiveLeftClick(x, y))
                {
                    recentSliderBar = hueSlider;
                    return true;
                }
                if (saturationSlider.ReceiveLeftClick(x, y))
                {
                    recentSliderBar = saturationSlider;
                    return true;
                }
                if (lightnessSlider.ReceiveLeftClick(x, y))
                {
                    recentSliderBar = lightnessSlider;
                    return true;
                }
            }
            else
            {
                hueSlider.input.Selected = false;
                if (hueSlider.inputCC.containsPoint(x, y) is true)
                {
                    hueSlider.input.Selected = true;
                    recentSliderBar = hueSlider;
                }
                saturationSlider.input.Selected = false;
                if (saturationSlider.inputCC.containsPoint(x, y) is true)
                {
                    saturationSlider.input.Selected = true;
                    recentSliderBar = saturationSlider;
                }
                lightnessSlider.input.Selected = false;
                if (lightnessSlider.inputCC.containsPoint(x, y) is true)
                {
                    lightnessSlider.input.Selected = true;
                    recentSliderBar = lightnessSlider;
                }
            }

            return false;
        }

        public bool ClickHeld(int x, int y)
        {
            if (recentSliderBar != null)
            {
                if (recentSliderBar.Equals(hueSlider) && hueSlider.input.Selected is false)
                {
                    return hueSlider.ReceiveLeftClick(x, y, true);
                }
                if (recentSliderBar.Equals(saturationSlider) && saturationSlider.input.Selected is false)
                {
                    return saturationSlider.ReceiveLeftClick(x, y, true);
                }
                if (recentSliderBar.Equals(lightnessSlider) && lightnessSlider.input.Selected is false)
                {
                    return lightnessSlider.ReceiveLeftClick(x, y, true);
                }
            }
            return Click(x, y);
        }

        public void Draw(SpriteBatch b)
        {
            hueSlider.Draw(b);

            saturationSlider.Draw(b);

            lightnessSlider.Draw(b);
        }

        public bool ContainsPoint(int x, int y)
        {
            return bounds.Contains(x, y);
        }

        public Color GetCurrentColor()
        {
            Utility.HSLtoRGB(hueSlider.GetValue(), saturationSlider.GetValue() / 100f, lightnessSlider.GetValue() / 100f, out var red, out var green, out var blue);
            return new Color(red, green, blue);
        }

        public Color GetColorForValues(float hue_slider, float saturation_slider)
        {
            Utility.HSLtoRGB(hue_slider, saturation_slider / 100f, 0.5, out var red, out var green, out var blue);
            return new Color(red, green, blue);
        }

        public Color GetColorForValues(float hue_slider, float saturation_slider, float lightness_slider)
        {
            Utility.HSLtoRGB(hue_slider, saturation_slider / 100f, Utility.Lerp(0.25f, 1f, (lightness_slider - MIN_LIGHTNESS) / (float)(MAX_LIGHTNESS - MIN_LIGHTNESS)), out var red, out var green, out var blue);
            return new Color(red, green, blue);
        }

        public void SetColor(Color color)
        {
            Utility.RGBtoHSL(color.R, color.G, color.B, out var h, out var s, out var l);
            hueSlider.SetValue((int)Math.Round(h));
            saturationSlider.SetValue((int)Math.Round(s * 100));
            lightnessSlider.SetValue((int)Math.Round(l * 100));
        }
    }
}
