using System;
using JoysOfEfficiency.Core;
using JoysOfEfficiency.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace JoysOfEfficiency.OptionsElements
{
    internal class ModifiedClickListener : OptionsElement
    {
        private bool _isListening;
        private Point _point;

        private readonly Action<int, ModifiedClickListener> _onStartListening;
        private readonly Action<int, Point> _onSomewhereClicked;
        private Rectangle _buttonRect;

        private bool _isSuppressed;

        private static readonly SpriteFont Font = Game1.dialogueFont;

        private readonly ITranslationHelper _translation;
        private readonly IClickableMenu _menu;

        public ModifiedClickListener(IClickableMenu parent ,string label, int which, int initialX, int initialY, ITranslationHelper translationHelper, Action<int, Point> onSomewhereClicked, Action<int, ModifiedClickListener> onStartListening = null) : base(label, -1, -1, 9 * Game1.pixelZoom, 9 * Game1.pixelZoom, 0)
        {
            this.label = InstanceHolder.Translation.Get($"options.{label}");
            _point = new Point(initialX, initialY);
            _onSomewhereClicked = onSomewhereClicked;
            _translation = translationHelper;
            _onStartListening = onStartListening ?? ((i,obj) => { });
            whichOption = which;
            _menu = parent;
        }

        public override void draw(SpriteBatch b, int slotX, int slotY)
        {
            string text = $"{label}: [{_point.X},{_point.Y}]";
            Vector2 size = Game1.dialogueFont.MeasureString(text);
            b.DrawString(Game1.dialogueFont, text, new Vector2(slotX+16, slotY + 8), Color.Black, 0, new Vector2(), 1f, SpriteEffects.None, 1.0f);

            int x = slotX + (int)size.X + 24;

            _buttonRect = new Rectangle(x, slotY, 90, 45);
            bounds = new Rectangle(0, 0, (int)size.X + _buttonRect.Width, _buttonRect.Height);

            b.Draw(Game1.mouseCursors, _buttonRect, new Rectangle(294, 428, 21, 11), Color.White, 0, Vector2.Zero, SpriteEffects.None, 1.0f);
        }

        public override void receiveLeftClick(int x, int y)
        {
            if (_isSuppressed)
            {
                return;
            }
            base.receiveLeftClick(x, y);
            if (_isListening)
            {
                _isListening = false;
                _point = new Point(x, y);
                _onSomewhereClicked(whichOption, new Point(x, y));
                return;
            }

            x += _menu.xPositionOnScreen;
            y += _buttonRect.Height / 2;
            if (x >= _buttonRect.Left && x <= _buttonRect.Right)
            {
                _isSuppressed = true;
                _onStartListening(whichOption, this);
                _isListening = true;
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (_isListening && key.HasFlag(Keys.Escape))
            {
                _isListening = false;
                _onSomewhereClicked(-1, Point.Zero);
            }
            base.receiveKeyPress(key);
        }

        public override void leftClickReleased(int x, int y)
        {
            base.leftClickReleased(x, y);
            if (_isSuppressed)
            {
                _isSuppressed = false;
            }
        }

        public void DrawStrings(SpriteBatch batch, int x, int y)
        {
            x += 16;
            y += 16;
            {
                Vector2 size = Font.MeasureString(_translation.Get("location.awaiting"));
                batch.DrawString(Font, _translation.Get("location.awaiting"), new Vector2(x, y), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                y += (int)size.Y + 8;

                size = Font.MeasureString(_translation.Get("button.esc"));
                batch.DrawString(Font, _translation.Get("button.esc"), new Vector2(x, y), Color.Black, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                y += (int)size.Y + 8;

                if (InstanceHolder.Config.ShowMousePositionWhenAssigningLocation)
                {
                    Util.DrawSimpleTextbox(batch, $"[{Game1.getMouseX()},{Game1.getMouseY()}]", Game1.dialogueFont, this);
                }
            }
        }

        public Point GetListeningMessageWindowSize()
        {
            int x = 32;
            int y = 16;

            {
                Vector2 size = Font.MeasureString(_translation.Get("location.awaiting"));
                x += (int)size.X;
                y += (int)size.Y;
            }
            {
                Vector2 size = Font.MeasureString(_translation.Get("button.esc"));
                if(size.X + 16 > x)
                {
                    x = (int)size.X + 16;
                }
                y += (int)size.Y + 16;
            }

            return new Point(x, y);
        }
    }
}
