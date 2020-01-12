using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace ModSettingsTab.Framework.Components
{
    public class OptionsDropDown : OptionsElement
    {
        private static readonly Rectangle DropDownBgSource = new Rectangle(433, 451, 3, 3);
        private static readonly Rectangle DropDownButtonSource = new Rectangle(437, 450, 10, 11);
        private readonly List<string> _dropDownOptions;
        private static OptionsDropDown _selected;
        private int _selectedOption;
        private int _recentSlotY;
        private int _startingSelected;
        private bool _clicked;
        private Rectangle _dropDownBounds;


        public OptionsDropDown(
            string name,
            string modId,
            string label,
            StaticConfig config,
            Point slotSize,
            List<string> dropDownOptions)
            : base(name, modId, label, config, 32, slotSize.Y / 2 - 10,
                (int) Game1.smallFont.MeasureString("Windowed Borderless Mode   ").X + 48, 44)
        {
            _dropDownOptions = dropDownOptions;
            _selectedOption = dropDownOptions.FindIndex(s => s == config[name].ToString());
            _dropDownBounds = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width - 44,
                Bounds.Height * _dropDownOptions.Count);
            Offset.Y = 8;
        }

        public override void LeftClickHeld(int x, int y)
        {
            if (GreyedOut)
                return;
            base.LeftClickHeld(x, y);
            _clicked = true;
            _dropDownBounds.Y = Math.Min(_dropDownBounds.Y,
                Game1.viewport.Height - _dropDownBounds.Height - _recentSlotY);
            _selectedOption =
                (int) Math.Max(
                    Math.Min((y - _dropDownBounds.Y) / (float) Bounds.Height,
                        _dropDownOptions.Count - 1), 0.0f);
        }

        public override void ReceiveLeftClick(int x, int y)
        {
            if (GreyedOut)
                return;
            _startingSelected = _selectedOption;
            LeftClickHeld(x, y);
            Game1.playSound("shwip");
            _selected = this;
        }

        public override void LeftClickReleased(int x, int y)
        {
            if (GreyedOut || _dropDownOptions.Count <= 0)
                return;
            base.LeftClickReleased(x, y);
            _clicked = false;
            if (_dropDownBounds.Contains(x, y))
                Config[Name] = _dropDownOptions[_selectedOption];
            else
                _selectedOption = _startingSelected;
            _selected = null;
        }

        public override void ReceiveKeyPress(Keys key)
        {
            base.ReceiveKeyPress(key);
            if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
                return;
            if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
            {
                ++_selectedOption;
                if (_selectedOption >= _dropDownOptions.Count)
                    _selectedOption = 0;
                Config[Name] = _dropDownOptions[_selectedOption];
            }
            else
            {
                if (!Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
                    return;
                --_selectedOption;
                if (_selectedOption < 0)
                    _selectedOption = _dropDownOptions.Count - 1;
                Config[Name] = _dropDownOptions[_selectedOption];
            }
        }

        public override bool PerformHoverAction(int x, int y)
        {
            return ShowTooltip && InfoIconBounds.Contains(x, y);
        }


        public override void Draw(SpriteBatch b, int slotX, int slotY)
        {
            _recentSlotY = slotY;
            var num = GreyedOut ? 0.33f : 1f;
            base.Draw(b, slotX, slotY);
            if (_clicked)
            {
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, DropDownBgSource,
                    slotX + _dropDownBounds.X, slotY + _dropDownBounds.Y, _dropDownBounds.Width,
                    _dropDownBounds.Height, Color.White * num, 4f, false);
                for (var index = 0; index < _dropDownOptions.Count && index< 7; ++index)
                {
                    if (index == _selectedOption)
                        b.Draw(Game1.staminaRect,
                            new Rectangle(slotX + _dropDownBounds.X+4,
                                slotY + _dropDownBounds.Y+4 + index * Bounds.Height, _dropDownBounds.Width-8,
                                Bounds.Height-8), new Rectangle(0, 0, 1, 1), Color.Wheat, 0.0f,
                            Vector2.Zero, SpriteEffects.None, 0.975f);
                    b.DrawString(Game1.smallFont, _dropDownOptions[index],
                        new Vector2(slotX + _dropDownBounds.X + 4,
                            slotY + _dropDownBounds.Y + 8 + Bounds.Height * index),
                        Game1.textColor * num, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.98f);
                }

                b.Draw(Game1.mouseCursors,
                    new Vector2(slotX + Bounds.X + Bounds.Width - 48,
                        slotY + Bounds.Y), DropDownButtonSource,
                    Color.Wheat * num, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.981f);
            }
            else
            {
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, DropDownBgSource,
                    slotX + Bounds.X, slotY + Bounds.Y, Bounds.Width - 44, Bounds.Height,
                    Color.White * num, 4f, false);
                if (_selected == null || _selected.Equals(this))
                    b.DrawString(Game1.smallFont,
                        _selectedOption >= _dropDownOptions.Count || _selectedOption < 0
                            ? ""
                            : _dropDownOptions[_selectedOption],
                        new Vector2(slotX + Bounds.X + 4, slotY + Bounds.Y + 8),
                        Game1.textColor * num, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.88f);
                b.Draw(Game1.mouseCursors,
                    new Vector2(slotX + Bounds.X + Bounds.Width - 48,
                        slotY + Bounds.Y), DropDownButtonSource,
                    Color.White * num, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
            }
        }
    }
}