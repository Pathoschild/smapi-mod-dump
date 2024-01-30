/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/6135/StardewValley.ProfitCalculator
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;

namespace ProfitCalculator.ui
{
    /// <summary>
    /// Base class for all writing based options in the options menu.
    /// </summary>
    public class TextOption : BaseOption, IKeyboardSubscriber
    {
        private Texture2D Tex;
        private readonly SpriteFont Font = Game1.smallFont;
        private bool SelectedImpl;

        /// <summary> The Function to retrieve the current value. </summary>
        protected readonly Func<string> ValueGetter;

        /// <summary> The Function to set the current value. </summary>
        protected readonly Action<string> ValueSetter;

        /// <summary>
        /// Whether the option is currently selected.
        /// </summary>
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

        /// <summary>
        /// Creates a new text option.
        /// </summary>
        /// <param name="x"> The x position of the option. </param>
        /// <param name="y"> The y position of the option. </param>
        /// <param name="name"> The name of the option. </param>
        /// <param name="label"> The label of the option. </param>
        /// <param name="valueGetter"> The function to get the value of the option. </param>
        /// <param name="valueSetter"> The function to set the value of the option. </param>
        public TextOption(
            int x,
            int y,
            Func<string> name,
            Func<string> label,
            Func<string> valueGetter,
            Action<string> valueSetter
         ) : base(x, y, 192, 48, name, label, label)
        {
            this.SetTexture(Game1.content.Load<Texture2D>("LooseSprites\\textBox"));
            this.ValueGetter = valueGetter;
            this.ValueSetter = valueSetter;
        }

        /// <summary>
        /// Sets the texture of the option. Updates width and height of the clickable component.
        /// </summary>
        /// <param name="tex"> The texture to set. </param>
        public void SetTexture(Texture2D tex)
        {
            this.Tex = tex;
            ClickableComponent.bounds.Width = tex.Width;
            ClickableComponent.bounds.Height = tex.Height;
        }

        /// <inheritdoc/>
        public override void Draw(SpriteBatch b)
        {
            b.Draw(
                this.Tex,
                this.Position,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                1,
                SpriteEffects.None,
                0.25f
                );

            // Copied from game code - caret and https://github.com/spacechase0/StardewValleyMods/blob/develop/SpaceShared/ui/Element.cs#L91
            string text = this.ValueGetter();
            Vector2 vector2;
            float writeBarOffset = 26f;
            for (vector2 = this.Font.MeasureString(text); vector2.X > (float)this.Tex.Width - writeBarOffset; vector2 = this.Font.MeasureString(text))
                text = text[1..];

            if (DateTime.UtcNow.Millisecond % 1000 >= 500 && this.Selected)
                b.Draw(
                    Game1.staminaRect,
                    new Rectangle(
                        (int)this.Position.X + 16 + (int)vector2.X + 2,
                        (int)this.Position.Y + 8,
                        4,
                        32
                    ),
                    null,
                    Game1.textColor,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0.3f
                );

            b.DrawString(this.Font, text, this.Position + new Vector2(16, 12), Game1.textColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.35f);
        }

        /// <summary>
        /// Recieves text input from the keyboard and adds it to the string. Calls <see cref="ReceiveInput(string)"/>.
        /// </summary>
        /// <param name="inputChar"> The character to add. </param>
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

        /// <summary>
        /// Recieves text input from the keyboard and adds it to the string. Calls <see cref="ReceiveInput(string)"/>.
        /// </summary>
        /// <param name="text"> The text to add. </param>
        public virtual void RecieveTextInput(string text)
        {
            this.ReceiveInput(text);
        }

        /// <summary>
        /// Recieves command input from the keyboard and removes the last character if it is backspace.
        /// </summary>
        /// <param name="command"> The command to recieve. </param>
        public virtual void RecieveCommandInput(char command)
        {
            if (command == '\b' && this.ValueGetter().Length > 0)
            {
                Game1.playSound("tinyWhip");
                this.ValueSetter(this.ValueGetter()[..^1]);
            }
        }

        /// <summary>
        /// Recieves special input from the keyboard.
        /// </summary>
        /// <param name="key"> The key to recieve. </param>
        public virtual void RecieveSpecialInput(Keys key)
        {
        }

        /// <summary>
        /// Recieves text input from the keyboard and adds it to the string. Updates the value on the option.
        /// </summary>
        /// <param name="str"> The string to add. </param>
        protected virtual void ReceiveInput(string str)
        {
            //this.String += str; to value setter and getter
            this.ValueSetter(this.ValueGetter() + str);
        }

        /// <summary>
        /// Called before the left mouse button click action. Deselects the option if the click is not on the option.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void BeforeReceiveLeftClick(int x, int y)
        {
            if (this.Selected && !this.ClickableComponent.containsPoint(x, y))
            {
                this.Selected = false;
            }
        }

        /// <summary>
        /// Called when the left mouse button is clicked. Selects the option.
        /// </summary>
        public override void ExecuteClick()
        {
            base.ExecuteClick();
            this.Selected = true;
        }

        /// <summary>
        /// Called when the option is updated.
        /// </summary>
        public override void Update()
        {
        }
    }
}