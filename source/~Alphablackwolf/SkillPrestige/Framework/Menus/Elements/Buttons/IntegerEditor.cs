/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Logging;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
// ReSharper disable PossibleLossOfFraction

namespace SkillPrestige.Framework.Menus.Elements.Buttons
{
    internal class IntegerEditor : Button
    {
        /*********
        ** Fields
        *********/
        protected override string HoverText => string.Empty;
        protected override string Text { get; }
        private const int PixelsWide = 7;
        private const int PixelsHigh = 8;
        private const int LinePadding = 4 * Game1.pixelZoom;
        private int Value { get; set; }
        private int Minimum { get; }
        private int Maximum { get; }
        private int Increment { get; }

        private readonly ClickCallback OnClick;
        private readonly TextureButton MinusButton;
        private readonly TextureButton PlusButton;


        /*********
        ** Accessors
        *********/
        public delegate void ClickCallback(int number);


        /*********
        ** Public methods
        *********/
        public IntegerEditor(string text, int startingNumber, int minimum, int maximum, Vector2 location, ClickCallback onClickCallback, int increment = 1)
        {
            if (maximum <= minimum)
                throw new ArgumentException($"{nameof(minimum)} value cannot exceed {nameof(maximum)} value.");
            this.OnClick = onClickCallback;
            this.Value = startingNumber.Clamp(minimum, maximum);
            this.Minimum = minimum;
            this.Maximum = maximum;
            this.Text = text;
            this.Increment = increment;
            int buttonYOffset = Game1.smallFont.MeasureString(text).Y.Ceiling() + LinePadding;
            this.MinusButton = new TextureButton(new Rectangle(location.X.Floor(), location.Y.Floor() + buttonYOffset, PixelsWide * Game1.pixelZoom, PixelsHigh * Game1.pixelZoom), Game1.mouseCursors, OptionsPlusMinus.minusButtonSource, this.MinusButtonClicked);
            int plusButtonOffset = MeasureNumberWidth(this.Maximum) + this.MinusButton.Bounds.Width;
            this.PlusButton = new TextureButton(new Rectangle(location.X.Floor() + plusButtonOffset, location.Y.Floor() + buttonYOffset, PixelsWide * Game1.pixelZoom, PixelsHigh * Game1.pixelZoom), Game1.mouseCursors, OptionsPlusMinus.plusButtonSource, this.PlusButtonClicked);
            int maxWidth = new[] { this.PlusButton.Bounds.X + this.PlusButton.Bounds.Width - location.X.Floor(), Game1.smallFont.MeasureString(text).X.Ceiling() }.Max();
            int maxHeight = buttonYOffset + new[] { this.MinusButton.Bounds.Height, this.PlusButton.Bounds.Height, NumberSprite.getHeight() }.Max();
            this.Bounds = new Rectangle(location.X.Floor(), location.Y.Floor(), maxWidth, maxHeight);
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="e">The event data.</param>
        /// <param name="isClick">Whether the button press is a click.</param>
        public override void OnButtonPressed(ButtonPressedEventArgs e, bool isClick)
        {
            base.OnButtonPressed(e, isClick);

            this.MinusButton.OnButtonPressed(e, isClick);
            this.PlusButton.OnButtonPressed(e, isClick);
        }

        /// <summary>Raised after the player moves the in-game cursor.</summary>
        /// <param name="e">The event data.</param>
        public override void OnCursorMoved(CursorMovedEventArgs e)
        {
            base.OnCursorMoved(e);

            this.MinusButton.OnCursorMoved(e);
            this.PlusButton.OnCursorMoved(e);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 location = new Vector2(this.Bounds.X, this.Bounds.Y);
            spriteBatch.DrawString(Game1.smallFont, this.Text, location, Game1.textColor);
            location.Y += Game1.smallFont.MeasureString(this.Text).Y + LinePadding;
            this.MinusButton.Draw(spriteBatch, this.Value == this.Minimum ? Color.Gray : Color.White);
            Vector2 numberLocation = location;
            int controlAreaWidth = this.PlusButton.Bounds.X + this.PlusButton.Bounds.Width - this.Bounds.X;
            numberLocation.X += controlAreaWidth / 2 + GetNumberXOffset(this.Value) / 2;
            numberLocation.Y += LinePadding;
            NumberSprite.draw(this.Value, spriteBatch, numberLocation, Color.SandyBrown, 1f, .85f, 1f, 0);
            this.PlusButton.Draw(spriteBatch, this.Value == this.Maximum ? Color.Gray : Color.White);
        }


        /*********
        ** Private methods
        *********/
        private void MinusButtonClicked()
        {
            Logger.LogVerbose($"{this.Text} minus button clicked.");
            if (this.Value <= this.Minimum)
                return;
            Game1.playSound("drumkit6");
            this.Value -= this.Increment;
            if (this.Value < this.Minimum)
                this.Value = this.Minimum;
            this.SendValue();
        }

        private void PlusButtonClicked()
        {
            Logger.LogVerbose($"{this.Text} plus button clicked.");
            if (this.Value >= this.Maximum)
                return;
            Game1.playSound("drumkit6");
            this.Value += this.Increment;
            if (this.Value > this.Maximum)
                this.Value = this.Maximum;
            this.SendValue();
        }

        private void SendValue()
        {
            this.Value = this.Value.Clamp(this.Minimum, this.Maximum);
            Logger.LogVerbose($"{this.Text} value of {this.Value} sent, button clicked.");
            this.OnClick.Invoke(this.Value);
        }

        /// <summary>Get the X pixel offset for a digit in the integer editor.</summary>
        private static int GetNumberXOffset(int number)
        {
            // Numbers are printed starting at the given location, then moving to the left for each digit. This does
            // *NOT* mean that the location given is the right edge. Instead, the location given to the number would be
            // at the X location in this pixel diagram for the number 703, This is why we subtract a single digit width.
            //   __   _ X__
            //    /  | |  _|
            //   /   |_| __|
            return MeasureNumberWidth(number) - SingleDigitWidth;
        }

        private static int MeasureNumberWidth(int number)
        {
            return number.ToString().Length * SingleDigitWidth;
        }

        private static int SingleDigitWidth => 8 * Game1.pixelZoom - 4;
    }
}
