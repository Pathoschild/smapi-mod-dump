using System;
using JoysOfEfficiency.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace JoysOfEfficiency.OptionsElements
{
    internal class ButtonWithLabel : OptionsElement
    {
        private Rectangle _buttonRect = Rectangle.Empty;

        private readonly Action<int> _onButtonPressed;
        private readonly Func<int, bool> _isDisabled;

        public ButtonWithLabel(string label, int which,
            Action<int> onButtonPressed = null, Func<int, bool> isDisabled = null) 
            : base(label, -1, -1, 9 * Game1.pixelZoom, 9 * Game1.pixelZoom, which)
        {
            this.label = InstanceHolder.Translation.Get($"options.{label}");
            _onButtonPressed = onButtonPressed ?? (i => { });
            _isDisabled = isDisabled ?? (i => false);
        }

        public override void receiveLeftClick(int x, int y)
        {
            base.receiveLeftClick(x, y);
            
            if (x >= _buttonRect.Left && x <= _buttonRect.Right)
            {
                _onButtonPressed(whichOption);
            }
        }

        public override void draw(SpriteBatch b, int slotX, int slotY)
        {
            string text = label;
            Vector2 size = Game1.dialogueFont.MeasureString(text);
            b.DrawString(Game1.dialogueFont, text, new Vector2(slotX + 16, slotY + 8), Color.Black, 0, new Vector2(), 1f, SpriteEffects.None, 1.0f);

            int x = slotX + (int)size.X + 24;

            _buttonRect = new Rectangle(x, slotY + 8, 90, 45);
            bounds = new Rectangle(0, 0, (int) size.X + _buttonRect.Width + _buttonRect.Width / 2, _buttonRect.Height + _buttonRect.Height / 2);

            b.Draw(Game1.mouseCursors, _buttonRect, new Rectangle(294, 428, 21, 11), Color.White * (_isDisabled(whichOption) ? 0.66f : 1.0f), 0, Vector2.Zero, SpriteEffects.None, 1.0f);
        }
    }
}
