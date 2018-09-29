using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.InputHandling;
using SkillPrestige.Logging;
using StardewValley;
using StardewValley.Menus;

namespace SkillPrestige.Menus.Elements.Buttons
{
    internal class IntegerEditor : Button
    {
        protected override string HoverText => string.Empty;
        protected override string Text { get; }
        private const int PixelsWide = 7;
        private const int PixelsHigh = 8;
        private readonly int _linePadding = 4 * Game1.pixelZoom;
        private int Value { get; set; }
        private int Minimum { get; }
        private int Maximum { get; }
        private int Increment { get; }

        private readonly TextureButton _minusButton;
        private readonly TextureButton _plusButton;
        private bool _buttonEventsRegistered;

        public delegate void ClickCallback(int number);

        private readonly ClickCallback _onClick;

        public IntegerEditor(string text, int startingNumber, int minimum, int maximum, Vector2 location, ClickCallback onClickCallback, int increment = 1)
        {
            if(maximum <= minimum) throw new ArgumentException($"{nameof(minimum)} value cannot exceed {nameof(maximum)} value.");
            _onClick = onClickCallback;
            Value = startingNumber.Clamp(minimum, maximum);
            Minimum = minimum;
            Maximum = maximum;
            Text = text;
            Increment = increment;
            var buttonYOffset = Game1.smallFont.MeasureString(text).Y.Ceiling() + _linePadding;
            _minusButton = new TextureButton(new Rectangle(location.X.Floor(), location.Y.Floor() + buttonYOffset, PixelsWide * Game1.pixelZoom, PixelsHigh * Game1.pixelZoom), Game1.mouseCursors, OptionsPlusMinus.minusButtonSource, MinusButtonClicked);
            var plusButtonOffset = MeasureNumberWidth(Maximum) + _minusButton.Bounds.Width;
            _plusButton = new TextureButton(new Rectangle(location.X.Floor() + plusButtonOffset, location.Y.Floor() + buttonYOffset, PixelsWide * Game1.pixelZoom, PixelsHigh * Game1.pixelZoom), Game1.mouseCursors, OptionsPlusMinus.plusButtonSource, PlusButtonClicked);
            var maxWidth = new[] { _plusButton.Bounds.X + _plusButton.Bounds.Width - location.X.Floor(), Game1.smallFont.MeasureString(text).X.Ceiling()}.Max();
            var maxHeight = buttonYOffset + new[] {_minusButton.Bounds.Height, _plusButton.Bounds.Height, NumberSprite.getHeight()}.Max();
            Bounds = new Rectangle(location.X.Floor(), location.Y.Floor(), maxWidth, maxHeight);
        }

        private void MinusButtonClicked()
        {
            Logger.LogVerbose($"{Text} minus button clicked.");
            if (Value <= Minimum) return;
            Game1.playSound("drumkit6");
            Value -= Increment;
            if (Value < Minimum) Value = Minimum;
            SendValue();
        }

        private void PlusButtonClicked()
        {
            Logger.LogVerbose($"{Text} plus button clicked.");
            if (Value >= Maximum) return;
            Game1.playSound("drumkit6");
            Value += Increment;
            if (Value > Maximum) Value = Maximum;
            SendValue();
        }

        private void SendValue()
        {
            Value = Value.Clamp(Minimum, Maximum);
            Logger.LogVerbose($"{Text} value of {Value} sent, button clicked.");
            _onClick.Invoke(Value);
        }

        protected override void OnMouseClick()
        {
            RegisterMouseEvents();
        }

        public void RegisterMouseEvents()
        {
            if (_buttonEventsRegistered) return;
            _buttonEventsRegistered = true;
            Mouse.MouseMoved += _minusButton.CheckForMouseHover;
            Mouse.MouseClicked += _minusButton.CheckForMouseClick;
            Mouse.MouseMoved += _plusButton.CheckForMouseHover;
            Mouse.MouseClicked += _plusButton.CheckForMouseClick;
            Logger.LogVerbose($"{Text} mouse events registered.");
        }

        public void DeregisterMouseEvents()
        {
            if (!_buttonEventsRegistered) return;
            _buttonEventsRegistered = false;
            Mouse.MouseMoved -= _minusButton.CheckForMouseHover;
            Mouse.MouseClicked -= _minusButton.CheckForMouseClick;
            Mouse.MouseMoved -= _plusButton.CheckForMouseHover;
            Mouse.MouseClicked -= _plusButton.CheckForMouseClick;
            Logger.LogVerbose($"{Text} mouse events deregistered.");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var location = new Vector2(Bounds.X, Bounds.Y);
            spriteBatch.DrawString(Game1.smallFont, Text, location, Game1.textColor);
            location.Y += Game1.smallFont.MeasureString(Text).Y + _linePadding ;
            _minusButton.Draw(spriteBatch, Value == Minimum ? Color.Gray : Color.White);
            var numberLocation = location;
            var controlAreaWidth = _plusButton.Bounds.X + _plusButton.Bounds.Width - Bounds.X;
            numberLocation.X += controlAreaWidth / 2 + GetNumberXOffset(Value) / 2;
            numberLocation.Y += _linePadding;
            NumberSprite.draw(Value, spriteBatch, numberLocation, Color.SandyBrown, 1f, .85f, 1f, 0);
            _plusButton.Draw(spriteBatch, Value == Maximum ? Color.Gray : Color.White);
        }

        /// <summary>
        /// // Numbers are printed starting at the given location, then moving to the left for each digit. This does *NOT* mean that the location given is the right edge.
        /// Instead, the location given to the number would be at the X location in this pixel diagram for the number 703, This is why we subtract a single digit width.
        ///             __   _ X__
        ///              /  | |  _|
        ///             /   |_| __|
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static int GetNumberXOffset(int number)
        {
            return MeasureNumberWidth(number) - SingleDigitWidth;
        }

        private static int MeasureNumberWidth(int number)
        {
            return number.ToString().Length * SingleDigitWidth;
        }

        private static int SingleDigitWidth => 8 * Game1.pixelZoom - 4;
    }
}
