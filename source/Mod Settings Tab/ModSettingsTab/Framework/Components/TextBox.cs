using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ModSettingsTab.Framework.Components
{
    public class TextBox : StardewValley.Menus.TextBox
    {
        public bool FloatOnly;

        public delegate void TextEnter(string s);

        private readonly TextEnter _textEnter;

        static TextBox()
        {
            Helper.Events.Input.ButtonPressed += (sender, e) =>
            {
                if ((Game1.activeClickableMenu is GameMenu || Game1.activeClickableMenu is TitleMenu) &&
                    e.Button == SButton.MouseLeft)
                    GlobalUpdate();
                if (e.Button != Game1.options.menuButton[0].ToSButton() || Game1.keyboardDispatcher.Subscriber == null)
                    return;
                Helper.Input.Suppress(!Game1.options.gamepadControls ? e.Button : Buttons.B.ToSButton());
            };
        }

        public TextBox(TextEnter textEnter)
            : base(null, null, Game1.smallFont, Game1.textColor)
        {
            _textEnter = textEnter;
            OnBackspacePressed += sender =>
            {
                Text = Text.Substring(0, sender.Text.Length - 1);
                textEnter(Text);
            };
            OnEnterPressed += sender => textEnter(Text);
        }

        public new void Update()
        {
            var mp = Game1.getMousePosition();
            if (Game1.gameMode == 0)
            {
                mp.X = (int) (mp.X * 1.1f);
                mp.Y = (int) (mp.Y * 1.1f);
            }
            
            Selected = new Rectangle(X, Y, Width, Height).Contains(mp);
            if (Selected)
                return;
            if (Game1.options.gamepadControls && !Game1.lastCursorMotionWasMouse)
                Game1.showTextEntry(this);
        }

        public override void RecieveTextInput(string text)
        {
            if (!Selected || numbersOnly && !int.TryParse(text, out _) ||
                FloatOnly && float.TryParse(Text + text, NumberStyles.Any,
                    CultureInfo.CreateSpecificCulture("en-US"), out _) ||
                -textLimit != -1 && Text.Length >= textLimit)
                return;
            Text += text;
            _textEnter(Text);
        }

        public static void GlobalUpdate()
        {
            if (Game1.keyboardDispatcher.Subscriber is TextBox)
            {
                ((TextBox) Game1.keyboardDispatcher.Subscriber).Update();
            }
        }

        public override void RecieveTextInput(char inputChar)
        {
            if (!Selected ||
                FloatOnly && !float.TryParse(Text + inputChar, NumberStyles.Any,
                    CultureInfo.CreateSpecificCulture("en-US"), out _) ||
                numbersOnly && !char.IsDigit(inputChar) ||
                textLimit != -1 && Text.Length >= textLimit) return;
            Text += inputChar.ToString();
            _textEnter(Text);
        }

        public override void Draw(SpriteBatch spriteBatch, bool drawShadow = true)
        {
            var flag = DateTime.UtcNow.Millisecond % 1000 >= 500;
            var text = Text;
            var color = Game1.textColor;
            Vector2 vector2;
            for (vector2 = _font.MeasureString(text);
                (double) vector2.X > (double) Width;
                vector2 = _font.MeasureString(text))
                text = text.Substring(1);
            if (flag && Selected)
                Utility.drawTextWithShadow(spriteBatch, "|", _font, new Vector2(X + 16 + (int) vector2.X + 2, Y + 5),
                    color);
            if (drawShadow)
                Utility.drawTextWithShadow(spriteBatch, text, _font, new Vector2(X + 16, Y + 8), color, 1f, 0.01f);
            else
                spriteBatch.DrawString(_font, text, new Vector2(X + 16, Y + 12),
                    color, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
        }
    }
}