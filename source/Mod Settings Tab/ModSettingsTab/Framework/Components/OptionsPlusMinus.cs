using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace ModSettingsTab.Framework.Components
{
    public class OptionsPlusMinus : OptionsElement
    {
        private static readonly Rectangle MinusButtonSource = new Rectangle(27, 236, 10, 11);
        private static readonly Rectangle PlusButtonSource = new Rectangle(36, 236, 10, 11);
        private readonly List<string> _options;
        private int _selectedOption;
        private static bool _snapZoomPlus;
        private static bool _snapZoomMinus;
        private Rectangle _minusButton;
        private Rectangle _plusButton;

        public OptionsPlusMinus(
            string name,
            string modId,
            string label,
            StaticConfig config,
            Point slotSize,
            List<string> plusMinusOptions)
            : base(name, modId, label, config, 32, slotSize.Y / 2-10, 56, 44)
        {
            _options = plusMinusOptions;
            _selectedOption = plusMinusOptions.FindIndex(s => s == config[name].ToString());
            _minusButton = new Rectangle(Bounds.X, Bounds.Y, MinusButtonSource.Width*4, Bounds.Height);
            
            var bWidth = _options
                            .Select(displayOption => (int) Game1.smallFont.MeasureString(displayOption).X + 28)
                            .Concat(new[] {(int) Game1.smallFont.MeasureString(_options[0]).X + 28})
                            .Max();
            _plusButton = new Rectangle(Bounds.X +_minusButton.Width+ bWidth, Bounds.Y, PlusButtonSource.Width*4, Bounds.Height);
            Bounds.Width =  _minusButton.Width + bWidth + _plusButton.Width;
            Offset.Y = 8;
            InfoIconBounds = new Rectangle(bWidth+40,0,0,0);
        }

        public override void ReceiveLeftClick(int x, int y)
        {
            if (GreyedOut || _options.Count <= 0)
                return;
            var selected1 = _selectedOption;
            if (_minusButton.Contains(x, y) &&  _selectedOption > 0)
            {
                --_selectedOption;
                _snapZoomMinus = true;
                Game1.playSound("drumkit6");
            }
            else if (_plusButton.Contains(x, y) && _selectedOption != _options.Count - 1)
            {
                ++_selectedOption;
                _snapZoomPlus = true;
                Game1.playSound("drumkit6");
            }

            if (_selectedOption < 0)
                _selectedOption = 0;
            else if (_selectedOption >= _options.Count)
                _selectedOption = _options.Count - 1;
            var selected2 = _selectedOption;
            if (selected1 == selected2)
                return;
            Config[Name] = _options[_selectedOption];
        }

        public override void ReceiveKeyPress(Keys key)
        {
            if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
                return;
            if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
            {
                ReceiveLeftClick(_plusButton.Center.X, _plusButton.Center.Y);
            }
            else
            {
                if (!Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
                    return;
                ReceiveLeftClick(_minusButton.Center.X, _minusButton.Center.Y);
            }
        }

        public override void Draw(SpriteBatch b, int slotX, int slotY)
        {
            base.Draw(b, slotX, slotY);
            b.Draw(ModData.Texture,
                new Vector2(slotX + _minusButton.X, slotY + _minusButton.Y),
                MinusButtonSource,
                Color.White * (GreyedOut ? 0.33f : 1f) * (_selectedOption == 0 ? 0.5f : 1f), 0.0f, Vector2.Zero, 4f,
                SpriteEffects.None, 0.4f);
            b.DrawString(Game1.smallFont,
                _selectedOption >= _options.Count || _selectedOption == -1 ? "" : _options[_selectedOption],
                new Vector2(slotX + _minusButton.X + _minusButton.Width +4,
                    slotY + Bounds.Y + 10), GreyedOut ? Game1.textColor * 0.33f : Game1.textColor);
            b.Draw(ModData.Texture,
                new Vector2(slotX + _plusButton.X, slotY + _plusButton.Y),
                PlusButtonSource,
                Color.White * (GreyedOut ? 0.33f : 1f) * (_selectedOption == _options.Count - 1 ? 0.5f : 1f),
                0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.4f);
            if (!Game1.options.snappyMenus && Game1.options.gamepadControls)
            {
                if (_snapZoomMinus)
                {
                    Game1.setMousePosition(slotX + _minusButton.Center.X, slotY + _minusButton.Center.Y);
                    _snapZoomMinus = false;
                }
                else if (_snapZoomPlus)
                {
                    Game1.setMousePosition(slotX + _plusButton.Center.X, slotY + _plusButton.Center.Y);
                    _snapZoomPlus = false;
                }
            }
        }
    }
}