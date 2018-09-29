using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace JoysOfEfficiency.OptionsElements
{
    internal class ModifiedInputListener : OptionsElement
    {
        private bool _isListening;
        private bool _conflicting;
        private Keys _button;

        private readonly Action<int, ModifiedInputListener> _onStartListening;
        private readonly Action<int, Keys> _onButtonPressed;
        private Func<int, bool> _isDisabled;
        private Rectangle _buttonRect;

        private static readonly SpriteFont Font = Game1.dialogueFont;

        private readonly ITranslationHelper _translation;
        private readonly IClickableMenu _menu;

        public ModifiedInputListener(IClickableMenu parent ,string label, int which, Keys initial, ITranslationHelper translationHelper, Action<int, Keys> onButtonPressed, Action<int, ModifiedInputListener> onStartListening = null, Func<int, bool> isDisabled = null) : base(label, -1, -1, 9 * Game1.pixelZoom, 9 * Game1.pixelZoom, 0)
        {
            this.label = ModEntry.ModHelper.Translation.Get($"options.{label}");
            _button = initial;
            _onButtonPressed = onButtonPressed;
            _isDisabled = isDisabled ?? (i => false);
            _translation = translationHelper;
            _onStartListening = onStartListening ?? ((i,obj) => { });
            whichOption = which;
            _menu = parent;
        }

        public override void receiveKeyPress(Keys key)
        {
            if(key == _button)
            {
                return;
            }
            if(key == Keys.Escape)
            {
                _conflicting = false;
                _isListening = false;
                _onButtonPressed(whichOption, _button);
                return;
            }
            base.receiveKeyPress(key);
            Config config = ModEntry.Conf;
            if(Game1.options.isKeyInUse(key) || key == ModEntry.Conf.KeyShowMenu)
            {
                _conflicting = true;
                return;
            }
            if(_isListening)
            {
                _button = key;
                _conflicting = false;
                _isListening = false;
                _onButtonPressed(whichOption, key);
            }
        }

        public override void draw(SpriteBatch b, int slotX, int slotY)
        {
            string text = $"{label}: {_button.ToString()}";
            Vector2 size = Game1.dialogueFont.MeasureString(text);
            b.DrawString(Game1.dialogueFont, text, new Vector2(slotX, slotY + 8), Color.Black, 0, new Vector2(), 1f, SpriteEffects.None, 1.0f);

            int x = slotX + (int)size.X + 8;

            _buttonRect = new Rectangle(x, slotY, 90, 45);
            bounds = new Rectangle(0, 0, (int)size.X + _buttonRect.Width, _buttonRect.Height);

            b.Draw(Game1.mouseCursors, _buttonRect, new Rectangle(294, 428, 21, 11), Color.White, 0, Vector2.Zero, SpriteEffects.None, 1.0f);
            //IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), slotX + bounds.Left, slotY + bounds.Top, bounds.Width, bounds.Height, Color.White);
        }

        public override void receiveLeftClick(int x, int y)
        {
            base.receiveLeftClick(x, y);
            x += _menu.xPositionOnScreen;
            y += _buttonRect.Height / 2;
            if(_buttonRect != null && x >= _buttonRect.Left && x <= _buttonRect.Right)
            {
                _onStartListening(whichOption, this);
                _isListening = true;
            }
        }

        public void DrawStrings(SpriteBatch batch, int x, int y)
        {
            x += 16;
            y += 16;
            {
                Vector2 size = Font.MeasureString(_translation.Get("button.awaiting"));
                batch.DrawString(Font, _translation.Get("button.awaiting"), new Vector2(x, y), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                y += (int)size.Y + 8;

                size = Font.MeasureString(_translation.Get("button.esc"));
                batch.DrawString(Font, _translation.Get("button.esc"), new Vector2(x, y), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                y += (int)size.Y + 8;

                if (_conflicting)
                {
                    size = Font.MeasureString(_translation.Get("button.conflict"));
                    batch.DrawString(Font, _translation.Get("button.conflict"), new Vector2(x, y), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                    y += (int)size.Y + 8;
                }
            }
        }

        public Point GetListeningMessageWindowSize()
        {
            int x = 32;
            int y = 16;

            {
                Vector2 size = Font.MeasureString(_translation.Get("button.awaiting"));
                x += (int)size.X;
                y += (int)size.Y;
            }
            {
                Vector2 size = Font.MeasureString(_translation.Get("button.esc"));
                if(size.X + 16 > x)
                {
                    x = (int)size.X + 16;
                }
                y += (int)size.Y + 8;
            }
            if(_conflicting){
                Vector2 size = Font.MeasureString(_translation.Get("button.conflict"));
                if (size.X + 16 > x)
                {
                    x = (int)size.X + 16;
                }
                y += (int)size.Y + 8;
            }

            return new Point(x, y);
        }
    }
}
