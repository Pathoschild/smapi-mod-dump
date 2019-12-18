using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using StardewValley;
using StardewValley.Menus;
using OptionsPage = ModSettingsTab.Menu.OptionsPage;

namespace ModSettingsTab.Framework.Components
{
    public class OptionsList : OptionsElement
    {
        private static readonly Rectangle DropDownBgSource = new Rectangle(433, 451, 3, 3);
        private static readonly Rectangle DropDownButtonSource = new Rectangle(9, 236, 19, 11);
        private static readonly Rectangle MinusButtonSource = new Rectangle(0, 236, 10, 11);
        private Rectangle _minusButtonBounds;
        private Rectangle _plusButtonBounds;
        private Rectangle _dropDownBounds;
        private readonly List<string> _dropDownOptions;
        private int _selectedOption;
        private int _recentSlotY;
        private bool _clicked;
        private bool _del;
        private IClickableMenu _gm;
        private readonly bool _numbersOnly;

        public OptionsList(
            string name,
            string modId,
            string label,
            StaticConfig config,
            Point slotSize)
            : base(name, modId, label, config, 32, slotSize.Y / 2 - 10, 48, 44)
        {
            _dropDownOptions = Config[Name].Children().Select(t => t.ToString()).ToList();

            _numbersOnly = Config[Name].Children().Any() && Config[Name].Children().First().Type == JTokenType.Integer;

            _minusButtonBounds = new Rectangle(Bounds.X, Bounds.Y, MinusButtonSource.Width * 4, Bounds.Height);

            // dropdown bounds
            var ddWidth = _dropDownOptions
                              .Select(option => (int) Game1.smallFont.MeasureString(option).X + 28)
                              .Concat(new[] {(int) Game1.smallFont.MeasureString("Windowed Borderless Mod").X})
                              .Max() + 9 * 4;

            _dropDownBounds = new Rectangle(Bounds.X + _minusButtonBounds.Width - 4, Bounds.Y,
                ddWidth, Bounds.Height * (_dropDownOptions.Count > 7 ? 7
                             : _dropDownOptions.Count < 1 ? 1 : _dropDownOptions.Count));

            _plusButtonBounds = new Rectangle(Bounds.X - 8 + _minusButtonBounds.Width + _dropDownBounds.Width,
                Bounds.Y, 10 * 4, Bounds.Height);

            Bounds.Width = _minusButtonBounds.Width + _dropDownBounds.Width + _plusButtonBounds.Width;

            Offset.Y = 8;
            InfoIconBounds = new Rectangle(Bounds.Width - 40, 0, 0, 0);
        }

        public override void LeftClickHeld(int x, int y)
        {
            if (GreyedOut)
                return;
            base.LeftClickHeld(x, y);
            if (_minusButtonBounds.Contains(x, y)) return;
            _del = false;
            _clicked = true;
            _dropDownBounds.Y = Math.Min(_dropDownBounds.Y,
                Game1.viewport.Height - _dropDownBounds.Height - _recentSlotY);
            _selectedOption =
                (int) Math.Max(
                    Math.Min((y - _dropDownBounds.Y) / (float) Bounds.Height,
                        _dropDownOptions.Count - 1), 0.0f);
        }

        private void PopupEnter(string s)
        {
            Game1.exitActiveMenu();
            Game1.activeClickableMenu = _gm;
            _gm = null;
            _clicked = false;

            (((GameMenu) Game1.activeClickableMenu).pages[GameMenu.optionsTab] as OptionsPage)
                ?.SetScrollBarToCurrentIndex();
            if (string.IsNullOrEmpty(s)) return;
            _dropDownOptions.Add(s);
            _selectedOption = _dropDownOptions.Count - 1;
            Save();
        }

        public override void ReceiveLeftClick(int x, int y)
        {
            if (GreyedOut)
                return;
            if (_minusButtonBounds.Contains(x, y))
            {
                if (_dropDownOptions.Count < 1) return;
                _del = true;
                return;
            }

            if (_dropDownBounds.Contains(x, y))
            {
                if (_dropDownOptions.Count < 1) return;
                LeftClickHeld(x, y);
                Game1.playSound("shwip");
                return;
            }

            if (_plusButtonBounds.Contains(x, y))
            {
                _gm = Game1.activeClickableMenu;
                Game1.activeClickableMenu = new PopupTextBox(PopupEnter, ModEntry.I18N.Get("OptionsList.NewValue"), _numbersOnly);
            }
        }

        public override void LeftClickReleased(int x, int y)
        {
            if (GreyedOut || _dropDownOptions.Count <= 0)
                return;

            _clicked = false;
            if (!_minusButtonBounds.Contains(x, y) || !_del) return;
            _dropDownOptions.RemoveAt(_selectedOption);
            _selectedOption = _dropDownOptions.Count - 1;
            Save();
            _del = false;
        }

        private void Save()
        {
            _dropDownBounds.Height = _dropDownOptions.Count > 7 ? Bounds.Height * 7
                : _dropDownOptions.Count < 1 ? Bounds.Height : Bounds.Height * _dropDownOptions.Count;
            Config[Name] = new JArray(_dropDownOptions);
        }

        public override bool PerformHoverAction(int x, int y)
        {
            return ShowTooltip && InfoIconBounds.Contains(x, y);
        }


        public override void Draw(SpriteBatch b, int slotX, int slotY)
        {
            _recentSlotY = slotY;
            base.Draw(b, slotX, slotY);
            var num = GreyedOut ? 0.33f : 1f;
            //draw minus
            b.Draw(ModData.Tabs,
                new Vector2(slotX + _minusButtonBounds.X, slotY + _minusButtonBounds.Y),
                MinusButtonSource,
                Color.White * num, 0.0f, Vector2.Zero, 4f,
                SpriteEffects.None, 0.4f);

            // draw dropdown list
            if (_clicked)
            {
                Helper.DrawTextureBox(b, Game1.mouseCursors, DropDownBgSource,
                    slotX + _dropDownBounds.X, slotY + Bounds.Y, _dropDownBounds.Width - 9 * 4,
                    _dropDownBounds.Height, Color.White * num, 4f, false, 0.7f);
                for (var index = 0; index < _dropDownOptions.Count && index < 7; ++index)
                {
                    if (index == _selectedOption)
                        b.Draw(Game1.staminaRect,
                            new Rectangle(slotX + _dropDownBounds.X+4,
                                slotY + Bounds.Y+4 + index * Bounds.Height, _dropDownBounds.Width - 9 * 4-8,
                                Bounds.Height-8), new Rectangle(0, 0, 1, 1), Color.Wheat, 0.0f,
                            Vector2.Zero, SpriteEffects.None, 0.71f);
                    b.DrawString(Game1.smallFont, _dropDownOptions[index],
                        new Vector2(slotX + _dropDownBounds.X + 8,
                            slotY + Bounds.Y + 8 + Bounds.Height * index),
                        Game1.textColor * num, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.72f);
                }
            }
            else if (_del)
            {
                b.Draw(Game1.staminaRect,
                    new Rectangle(slotX + _dropDownBounds.X + 4,
                        slotY + Bounds.Y , _dropDownBounds.Width - 9 * 4-8,
                        Bounds.Height), new Rectangle(0, 0, 1, 1), Color.Brown, 0.0f,
                    Vector2.Zero, SpriteEffects.None, 0.71f);
                b.DrawString(Game1.smallFont, _dropDownOptions[_selectedOption],
                    new Vector2(slotX + _dropDownBounds.X + 8,
                        slotY + Bounds.Y + 8),
                    Game1.textColor * num, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.72f);
            }
            else
            {
                Helper.DrawTextureBox(b, Game1.mouseCursors, DropDownBgSource,
                    slotX + _dropDownBounds.X, slotY + Bounds.Y, _dropDownBounds.Width - 9 * 4, Bounds.Height,
                    Color.White * num, 4f, false, 0.68f);
                b.DrawString(Game1.smallFont,
                    _selectedOption >= _dropDownOptions.Count || _selectedOption < 0
                        ? ""
                        : _dropDownOptions[_selectedOption],
                    new Vector2(slotX + Bounds.X + _minusButtonBounds.X + 8, slotY + Bounds.Y + 8),
                    Game1.textColor * num, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.691f);
            }

            //draw plus
            b.Draw(ModData.Tabs,
                new Vector2(slotX + _plusButtonBounds.X - 9 * 4, slotY + _plusButtonBounds.Y),
                DropDownButtonSource,
                Color.White * num, 0.0f, Vector2.Zero, 4f,
                SpriteEffects.None, 0.4f);
        }
    }
}