using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace ModSettingsTab.Framework.Components
{
    public class OptionsSlider : OptionsElement
    {
        private static readonly Rectangle SliderBgSource = new Rectangle(403, 383, 6, 6);
        private static readonly Rectangle SliderButtonRect = new Rectangle(420, 441, 10, 6);
        private readonly int _sliderButtonWidth = SliderButtonRect.Width * 4;
        private const float Scale = 4f;
        public int SliderMaxValue;
        public int SliderMinValue;
        private int _sliderStep;

        public int SliderStep
        {
            get => _sliderStep;
            set => _sliderStep = value <= 0 ? 1 : value;
        }

        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                Label = $"{_originalLabel}: {_value}";
            }
        }

        private int _value;

        private readonly string _originalLabel;

        public OptionsSlider(
            string name,
            string modId,
            string label,
            StaticConfig config,
            Point slotSize)
            : base(name, modId, label, config, 32, slotSize.Y / 2 + 12, slotSize.X / 4, 24)
        {
            _originalLabel = label;
            Value = int.Parse(Config[Name].ToString());
            InfoIconBounds = new Rectangle(80,-12,0,0);
            Offset.X = 8;
            Offset.Y = -4;
        }

        public override void LeftClickHeld(int x, int y)
        {
            if (GreyedOut)
                return;
            Value = x >= Bounds.X
                ? x <= Bounds.Right - _sliderButtonWidth ? CountValue(x) : SliderMaxValue
                : SliderMinValue;
            Config[Name] = Value;
        }

        private int CountValue(int x)
        {
            var num1 = (int) ((x - Bounds.X) / (double) (Bounds.Width - _sliderButtonWidth) *
                              (SliderMaxValue - SliderMinValue) + SliderMinValue);
            if (SliderStep == 1)
                return num1;
            var num2 = num1 % (float) SliderStep;
            var num3 = (double) num2 >= (double) SliderStep / 2.0
                ? (int) (num1 + (SliderStep - (double) num2))
                : (int) (num1 - (double) num2);
            return num3 < SliderMaxValue ? num3 : SliderMaxValue;
        }

        public override void ReceiveLeftClick(int x, int y)
        {
            if (GreyedOut)
                return;
            LeftClickHeld(x, y);
        }

        public override void ReceiveKeyPress(Keys key)
        {
            if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
                return;
            if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
            {
                Value = Math.Min(Value + SliderStep, SliderMaxValue);
                Config[Name] = Value;
            }
            else
            {
                if (!Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
                    return;
                Value = Math.Max(Value - SliderStep, SliderMinValue);
                Config[Name] = Value;
            }
        }

        public override void Draw(SpriteBatch b, int slotX, int slotY)
        {
            Helper.DrawTextureBox(b, Game1.mouseCursors, SliderBgSource, slotX + Bounds.X,
                slotY + Bounds.Y, Bounds.Width, Bounds.Height, Color.White, 4f, false,0.688f);
            b.Draw(Game1.mouseCursors,
                new Vector2(
                    slotX + Bounds.X +
                    (float) ((Bounds.Width - (double) _sliderButtonWidth) *
                             ((Value - (double) SliderMinValue) /
                              (SliderMaxValue - SliderMinValue))),
                    slotY + Bounds.Y), SliderButtonRect, Color.White, 0.0f,
                Vector2.Zero, 4f, SpriteEffects.None, 0.689f);
            base.Draw(b, slotX, slotY);
        }
    }
}