/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/JoysOfEfficiency
**
*************************************************/

using System;
using JoysOfEfficiency.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace JoysOfEfficiency.OptionsElements
{
    internal class ModifiedSlider : OptionsElement
    {
        private readonly string _label;

        private readonly Action<int, int> _setValue;

        private readonly int _maxValue;
        private readonly int _minValue;

        private int _value;

        private readonly Func<bool> _isDisabled;

        private readonly Func<int, int, string> _format;

        public ModifiedSlider(string label, int which, int initialValue, int minValue, int maxValue, Action<int, int> setValue, Func<bool> disabled = null, Func<int, int, string> format = null, int width = 64)
            : base(label, -1, -1, width * Game1.pixelZoom, 6 * Game1.pixelZoom, 0)
        {
            whichOption = which;
            _label = InstanceHolder.Translation.Get($"options.{label}");
            _value = initialValue - minValue;
            _minValue = minValue;
            _maxValue = maxValue - minValue;
            _setValue = setValue ?? ((i, j) => { });
            _isDisabled = disabled ?? (() => false);
            _format = format ?? ((i, value) => value.ToString());
        }

        public override void leftClickHeld(int x, int y)
        {
            if (greyedOut)
                return;

            base.leftClickHeld(x, y);
            int oldValue = _value;
            _value = x >= bounds.X
                ? (x <= bounds.Right - 10 * Game1.pixelZoom
                    ? (int) ((x - bounds.X) / (bounds.Width - 10d * Game1.pixelZoom) * _maxValue)
                    : _maxValue)
                : 0;
            if (_value != oldValue)
            {
                _setValue?.Invoke(whichOption, _value + _minValue);
            }

        }

        public override void receiveLeftClick(int x, int y)
        {
            if (greyedOut)
                return;
            base.receiveLeftClick(x, y);
            leftClickHeld(x, y);
        }

        public override void leftClickReleased(int x, int y)
        {
            _setValue?.Invoke(whichOption, _value + _minValue);
        }

        public override void draw(SpriteBatch spriteBatch, int slotX, int slotY)
        {
            label = _label.Contains("{0}")
                ? string.Format(_label, _format(whichOption, _value + _minValue))
                : $"{_label}: {_format(whichOption, _value + _minValue)}";

            greyedOut = _isDisabled();

            base.draw(spriteBatch, slotX, slotY - ((int)Game1.dialogueFont.MeasureString(label).Y - bounds.Height) / 2);
            IClickableMenu.drawTextureBox(spriteBatch, Game1.mouseCursors, OptionsSlider.sliderBGSource, slotX + bounds.X, slotY + bounds.Y, bounds.Width, bounds.Height, Color.White * (greyedOut ? 0.33f : 1.0f), Game1.pixelZoom, false);
            spriteBatch.Draw(Game1.mouseCursors, new Vector2(slotX + bounds.X + (bounds.Width - 10 * Game1.pixelZoom) * (_value / (float)_maxValue), slotY + bounds.Y), OptionsSlider.sliderButtonRect, Color.White * (greyedOut ? 0.66f : 1.0f), 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.9f);
        }
    }
}
