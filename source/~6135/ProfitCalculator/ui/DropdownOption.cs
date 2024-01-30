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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Linq;

namespace ProfitCalculator.ui
{
    /// <summary>
    /// Dropdown option for the options menu.
    /// </summary>
    public class DropdownOption : BaseOption
    {
        /// <summary> The width of the dropdown box. </summary>
        public int RequestWidth { get; set; }

        /// <summary> The maximum number of values to display at once. </summary>
        public int MaxValuesAtOnce { get; set; } = 5;

        /// <summary> The texture to draw. </summary>
        public Texture2D Texture { get; set; } = Game1.mouseCursors;

        /// <summary> The texture rectangle to draw for the background. </summary>
        public Rectangle BackgroundTextureRect { get; set; } = OptionsDropDown.dropDownBGSource;

        /// <summary> The texture rectangle to draw for the button. </summary>
        public Rectangle ButtonTextureRect { get; set; } = OptionsDropDown.dropDownButtonSource;

        /// <summary> The value of the option. Defines a get and set behaviour </summary>
        public string Value
        {
            get => this.Choices[this.ActiveChoice];
            set { if (this.Choices.Contains(value)) this.ActiveChoice = Array.IndexOf(this.Choices, value); }
        }

        /// <summary> The width of the dropdown box. </summary>
        public int DropDownBoxWidth => Math.Max(300, Math.Min(300, this.RequestWidth));

        /// <summary> The height of the dropdown box. </summary>
        public int DropDownBoxHeight => 44;

        /// <summary> The name of the option. </summary>
        public new string Label => this.Labels[this.ActiveChoice];

        /// <summary> The current active choice. </summary>
        public int ActiveChoice { get; set; }

        /// <summary> The current active position. </summary>
        public int ActivePosition { get; set; }

        /// <summary> The available choices. </summary>
        public string[] Choices { get; set; }

        /// <summary> The labels for the options. </summary>
        public string[] Labels { get; set; }

        /// <summary> The value setter. Type Action </summary>
        private readonly Action<string> ValueSetter;

        /// <summary> Determines whether the dropdown is dropped. </summary>
        private bool Dropped;

#pragma warning disable S2223, CA2211, S1104

        /// <summary> The current active dropdown. </summary>
        public static DropdownOption ActiveDropdown = null;

#pragma warning restore S2223, CA2211, S1104

        /// <summary> The sound to play when the option is clicked. </summary>
        public override string ClickedSound => "shwip";

        /// <summary>
        /// Creates a new DropdownOption
        /// </summary>
        /// <param name="x"> The x position of the clickable component</param>
        /// <param name="y"> The y position of the clickable component</param>
        /// <param name="name"> The name of the clickable component</param>
        /// <param name="label"> The label of the clickable component</param>
        /// <param name="choices"> The choices of the dropdown</param>
        /// <param name="labels"> The labels of the dropdown</param>
        /// <param name="valueGetter"> The value getter</param>
        /// <param name="valueSetter"> The value setter</param>
        public DropdownOption(
            int x,
            int y,
            Func<string> name,
            Func<string> label,
            Func<string[]> choices,
            Func<string[]> labels,
            Func<string> valueGetter,
            Action<string> valueSetter
        ) : base(x, y, 0, 0, name, label, label)
        {
            this.Choices = choices();
            this.Labels = labels();
            this.ActiveChoice = Array.IndexOf(this.Choices, valueGetter());
            ValueSetter = valueSetter;
            ClickableComponent.bounds.Width = this.DropDownBoxWidth;
            ClickableComponent.bounds.Height = this.DropDownBoxHeight;
        }

        /// <summary>
        /// The Update behaviour of the option
        /// </summary>
        public override void Update()
        {
            bool justClicked = false;

            if (this.Clicked && DropdownOption.ActiveDropdown == null)
            {
                justClicked = true;
                this.Dropped = true;
            }

            if (this.Dropped)
            {
                if (Constants.TargetPlatform != GamePlatform.Android)
                {
                    //print all checked values

                    if ((Mouse.GetState().LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Released ||
                         Game1.input.GetGamePadState().Buttons.A == ButtonState.Pressed && Game1.oldPadState.Buttons.A == ButtonState.Released)
                        && !justClicked)
                    {
                        Game1.playSound("drumkit6");
                        this.Dropped = false;
                    }
                }
                else
                {
                    if ((Game1.input.GetMouseState().LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Released ||
                         Game1.input.GetGamePadState().Buttons.A == ButtonState.Pressed && Game1.oldPadState.Buttons.A == ButtonState.Released)
                        && !justClicked)
                    {
                        Game1.playSound("drumkit6");
                        this.Dropped = false;
                    }
                }
                int tall = Math.Min(this.MaxValuesAtOnce, this.Choices.Length - this.ActivePosition) * this.DropDownBoxHeight;
                int drawY = Math.Min((int)this.Position.Y, Game1.uiViewport.Height - tall);
                var bounds2 = new Rectangle((int)this.Position.X, drawY, this.DropDownBoxWidth, this.DropDownBoxHeight * this.MaxValuesAtOnce);
                if (bounds2.Contains(Game1.getOldMouseX(), Game1.getOldMouseY()))
                {
                    int choice = (Game1.getOldMouseY() - drawY) / this.DropDownBoxHeight;
                    this.ActiveChoice = choice + this.ActivePosition;
                    this.ValueSetter(this.Choices[this.ActiveChoice]);
                }

                DropdownOption.ActiveDropdown = this;
            }
            else
            {
                if (DropdownOption.ActiveDropdown == this)
                    DropdownOption.ActiveDropdown = null;
                this.ActivePosition = Math.Min(this.ActiveChoice, this.Choices.Length - this.MaxValuesAtOnce);
            }
        }

        /// <summary>
        /// Reacts to the scroll wheel action. By showing the next or previous options if the dropdown is dropped and there are more options to show
        /// </summary>
        /// <param name="direction"></param>
        public void ReceiveScrollWheelAction(int direction)
        {
            if (this.Dropped)
                this.ActivePosition = Math.Min(Math.Max(this.ActivePosition - (direction / 120), 0), this.Choices.Length - this.MaxValuesAtOnce);
            else
                DropdownOption.ActiveDropdown = null;
        }

        /// <inheritdoc/>
        public override void Draw(SpriteBatch b)
        {
            IClickableMenu.drawTextureBox(
                b, this.Texture,
                this.BackgroundTextureRect,
                (int)this.Position.X,
                (int)this.Position.Y,
                this.DropDownBoxWidth - 48,
                this.DropDownBoxHeight,
                Color.White,
                4,
                false,
                0.5f);//small clicable initial box

            b.DrawString(
                Game1.smallFont,
                this.Label,
                new Vector2(this.Position.X + 4, this.Position.Y + 8),
                Game1.textColor,
                0,
                Vector2.Zero,
                1,
                SpriteEffects.None,
                0.55f
             ); //Selected text
            b.Draw(
                this.Texture,
                new Vector2(this.Position.X + this.DropDownBoxWidth - 48, this.Position.Y),
                this.ButtonTextureRect,
                Color.White,
                0,
                Vector2.Zero,
                4,
                SpriteEffects.None,
                0f
            ); //Dropdown arrow

            if (this.Dropped)
            {
                int maxValues = this.MaxValuesAtOnce;
                int start = this.ActivePosition;
                int end = Math.Min(this.Choices.Length, start + maxValues);
                int tall = Math.Min(maxValues, this.Choices.Length - this.ActivePosition) * this.DropDownBoxHeight;
                int drawY = Math.Min((int)this.Position.Y, Game1.uiViewport.Height - tall);
                IClickableMenu.drawTextureBox(
                    b,
                    this.Texture,
                    this.BackgroundTextureRect,
                    (int)this.Position.X,
                    drawY,
                    this.DropDownBoxWidth - 48,
                    tall,
                    Color.White, 4,
                    false,
                    0.6f); // Dropdown box with options
                for (int i = start; i < end; ++i)
                {
                    if (i == this.ActiveChoice)
                        b.Draw(
                            Game1.staminaRect,
                            new Rectangle((int)this.Position.X + 4,
                            drawY + (i - this.ActivePosition) * this.DropDownBoxHeight,
                            this.DropDownBoxWidth - 48 - 8, this.DropDownBoxHeight),
                            null,
                            Color.Wheat,
                            0,
                            Vector2.Zero,
                            SpriteEffects.None,
                            0.65f
                        ); // Selected option
                    b.DrawString(
                        Game1.smallFont,
                        this.Labels[i],
                        new Vector2(this.Position.X + 4, drawY + (i - this.ActivePosition) * this.DropDownBoxHeight + 8),
                        Game1.textColor,
                        0,
                        Vector2.Zero,
                        1,
                        SpriteEffects.None,
                        0.7f
                    );
                }
            }
        }

        /// <inheritdoc/>
        public override void BeforeReceiveLeftClick(int x, int y)
        {
        }

        /// <summary>
        /// Behaviour when the left click is received. If the dropdown is not dropped then open it. If it is dropped then close it. If dropdown was open and the click was meant to close it then close it it stops the spread of the click to other options so as to not get any overlap with other options (i.e. clicking on one option and opening another or selecting an option and opening another)
        /// </summary>
        /// <param name="x"> The x position of the mouse</param>
        /// <param name="y"> The y position of the mouse</param>
        /// <param name="stopSpread"> The action to stop the spread of the click</param>
        public override void ReceiveLeftClick(int x, int y, Action stopSpread)
        {
            //if ClickMeantToCloseDropdown is false then open dropdown
            if (ClickableComponent.containsPoint(x, y) && !this.Dropped && !this.Clicked)
            {
                this.ExecuteClick();
            }
            else if (this.Dropped || this.Clicked)
            {
                this.Dropped = false;
                this.Clicked = false;
                stopSpread();
            }
        }
    }
}