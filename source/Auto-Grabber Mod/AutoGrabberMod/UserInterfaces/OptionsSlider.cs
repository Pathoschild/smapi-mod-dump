using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace AutoGrabberMod.UserInterfaces
{
	public class OptionsSlider : OptionsElement
	{
		public const int pixelsWide = 48;

		public const int pixelsHigh = 6;

		public const int sliderButtonWidth = 10;

		public readonly int sliderMaxValue = 100;

        public readonly int sliderMinValue = 0;

		public int value;

		private readonly string Label;

		private readonly Action<int> SetValue;

		private readonly Func<bool> IsDisabled;

		private readonly Func<int, string> Format;

		public static Rectangle sliderBGSource = new Rectangle(403, 383, 6, 6);

		public static Rectangle sliderButtonRect = new Rectangle(420, 441, 10, 6);

		public OptionsSlider(string label, int initValue, int minValue, int maxValue, Action<int> setValue, Func<bool> disabled = null, Func<int, string> format = null) : base(label, -1, -1, 192, 24)
		{
			this.Label = label;
			this.value = initValue;
			this.sliderMaxValue = maxValue;
			this.SetValue = setValue;
			this.IsDisabled = disabled ?? (() => false);
			this.Format = format ?? (value => value.ToString());
            this.sliderMinValue = minValue;
		}

		public override void leftClickHeld(int x, int y)
		{
			if (!base.greyedOut)
			{
				base.leftClickHeld(x, y);
				if (x < base.bounds.X)
				{
					this.value = this.sliderMinValue;
				}
				else if (x > base.bounds.Right - 40)
				{
					this.value = this.sliderMaxValue;
				}
				else
				{
					this.value = (int)((float)(x - base.bounds.X) / (float)(base.bounds.Width - 40) * this.sliderMaxValue);
				}
				this.SetValue(this.value);
			}
		}

		public override void receiveLeftClick(int x, int y)
		{
			if (!base.greyedOut)
			{
				base.receiveLeftClick(x, y);
				this.leftClickHeld(x, y);
			}
		}

		public override void receiveKeyPress(Keys key)
		{
			base.receiveKeyPress(key);
			if (Game1.options.snappyMenus && Game1.options.gamepadControls)
			{
				if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
				{
					this.value = Math.Min(this.value + 1, sliderMaxValue);
					this.SetValue(this.value);
				}
				else if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
				{
					this.value = Math.Max(this.value - 1, this.sliderMinValue);
					this.SetValue(this.value);
				}
			}
		}

		public override void draw(SpriteBatch b, int slotX, int slotY)
		{
			this.label = $"{this.Label}: {this.Format(this.value)}";
			this.greyedOut = this.IsDisabled();

            base.draw(b, slotX, slotY);

            IClickableMenu.drawTextureBox(
                b,
                Game1.mouseCursors,
                OptionsSlider.sliderBGSource,
                slotX + base.bounds.X,
                slotY + base.bounds.Y,
                base.bounds.Width,
                base.bounds.Height,
                Color.White,
                4f,
                false
            );

            b.Draw(
                Game1.mouseCursors,
                new Vector2(slotX + this.bounds.X + (this.bounds.Width - 10 * Game1.pixelZoom) * (this.value / (float)this.sliderMaxValue), slotY + this.bounds.Y),
                OptionsSlider.sliderButtonRect,
                Color.White,
                0.0f,
                Vector2.Zero,
                Game1.pixelZoom,
                SpriteEffects.None,
                0.8f
            );
        }
	}
}
