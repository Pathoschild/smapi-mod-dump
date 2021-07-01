/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace GenericModConfigMenu.Framework.UI
{
    internal class Textbox : Element, IKeyboardSubscriber
    {
        private readonly Texture2D Tex;
        private readonly SpriteFont Font;

        public virtual string String { get; set; }

        private bool SelectedImpl;
        public bool Selected
        {
            get => this.SelectedImpl;
            set
            {
                if (this.SelectedImpl == value)
                    return;

                this.SelectedImpl = value;
                if (this.SelectedImpl)
                    Game1.keyboardDispatcher.Subscriber = this;
                else
                {
                    if (Game1.keyboardDispatcher.Subscriber == this)
                        Game1.keyboardDispatcher.Subscriber = null;
                }
            }
        }

        public Action<Element> Callback { get; set; }

        public Textbox()
        {
            this.Tex = Game1.content.Load<Texture2D>("LooseSprites\\textBox");
            this.Font = Game1.smallFont;
        }

        public override int Width => 192;
        public override int Height => 48;

        public override void Update(bool hidden = false)
        {
            base.Update(hidden);

            if (this.ClickGestured && this.Callback != null)
            {
                this.Selected = this.Hover;
            }
        }

        public override void Draw(SpriteBatch b)
        {
            b.Draw(this.Tex, this.Position, Color.White);

            // Copied from game code - caret
            string text = this.String;
            Vector2 vector2;
            for (vector2 = this.Font.MeasureString(text); (double)vector2.X > (double)192; vector2 = this.Font.MeasureString(text))
                text = text.Substring(1);
            if (DateTime.UtcNow.Millisecond % 1000 >= 500 && this.Selected)
                b.Draw(Game1.staminaRect, new Rectangle((int)this.Position.X + 16 + (int)vector2.X + 2, (int)this.Position.Y + 8, 4, 32), Game1.textColor);

            b.DrawString(this.Font, text, this.Position + new Vector2(16, 12), Game1.textColor);
        }

        protected virtual void ReceiveInput(string str)
        {
            this.String += str;
            this.Callback?.Invoke(this);
        }

        public void RecieveTextInput(char inputChar)
        {
            this.ReceiveInput(inputChar.ToString());

            // Copied from game code
            switch (inputChar)
            {
                case '"':
                    return;
                case '$':
                    Game1.playSound("money");
                    break;
                case '*':
                    Game1.playSound("hammer");
                    break;
                case '+':
                    Game1.playSound("slimeHit");
                    break;
                case '<':
                    Game1.playSound("crystal");
                    break;
                case '=':
                    Game1.playSound("coin");
                    break;
                default:
                    Game1.playSound("cowboy_monsterhit");
                    break;
            }
        }

        public void RecieveTextInput(string text)
        {
            this.ReceiveInput(text);
        }

        public void RecieveCommandInput(char command)
        {
            if (command == '\b' && this.String.Length > 0)
            {
                Game1.playSound("tinyWhip");
                this.String = this.String.Substring(0, this.String.Length - 1);
                this.Callback?.Invoke(this);
            }
        }

        public void RecieveSpecialInput(Keys key)
        {
        }
    }
}
