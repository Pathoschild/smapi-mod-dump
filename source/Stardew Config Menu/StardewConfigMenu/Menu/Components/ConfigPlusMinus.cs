using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewConfigFramework.Options;
using StardewValley;
using StardewValley.Menus;

namespace StardewConfigMenu.Components {

	sealed class ConfigPlusMinus: SCMControl {
		private readonly IConfigStepper ModData;

		private readonly SCMSprite Minus = SCMSprite.MinusButton;
		private readonly SCMSprite Plus = SCMSprite.PlusButton;

		public sealed override int X {
			get => Minus.Bounds.X;
			set {
				if (X == value)
					return;

				Minus.X = value;
				Plus.X = Minus.Bounds.Right + (int) MaxLabelSize.X + UnitStringWidth + 4 * Game1.pixelZoom;
			}
		}
		public sealed override int Y {
			get => Minus.Y;
			set {
				if (Y == value)
					return;

				Minus.Y = value;
				Plus.Y = value;
			}
		}
		public sealed override int Width => Plus.Bounds.Right - Minus.X;
		public sealed override int Height => (int) Math.Max(Minus.Height, MaxLabelSize.Y);

		private Vector2 MaxLabelSize = Vector2.Zero;
		private int UnitStringWidth => (int) Game1.dialogueFont.MeasureString(UnitString).X;
		private Vector2 ValueLabelSize => Game1.dialogueFont.MeasureString(Value.ToString());

		public sealed override bool Enabled => ModData.Enabled;
		public sealed override string Label => ModData.Label;
		public decimal Min => ModData.Min;
		public decimal Max => ModData.Max;
		public decimal StepSize => ModData.StepSize;
		public decimal Value { get => ModData.Value; set => ModData.Value = value; }
		public RangeDisplayType DisplayType => ModData.DisplayType;

		private string UnitString {
			get {
				switch (DisplayType) {
					case RangeDisplayType.PERCENT:
						return "%";
					default:
						return "";
				}
			}
		}

		internal ConfigPlusMinus(IConfigStepper option) : this(option, 0, 0) { }

		internal ConfigPlusMinus(IConfigStepper option, int x, int y) : base(option.Label, option.Enabled) {
			ModData = option;

			CalculateMaxLabelSize();

			X = x;
			Y = y;
		}

		private void CalculateMaxLabelSize() {
			var maxRect = Game1.dialogueFont.MeasureString((Max + StepSize % 1).ToString());
			var minRect = Game1.dialogueFont.MeasureString((Min - StepSize % 1).ToString());

			MaxLabelSize = (maxRect.X > minRect.X) ? maxRect : minRect;
		}

		private decimal CheckValidInput(decimal input) {
			if (input > Max)
				return Max;

			if (input < Min)
				return Min;

			return ((input - Min) / StepSize) * StepSize + Min;
		}

		private void StepUp() {
			ModData.StepUp();
		}

		private void StepDown() {
			ModData.StepDown();
		}

		public override void ReceiveLeftClick(int x, int y, bool playSound = true) {
			if (!Enabled || !IsAvailableForSelection)
				return;

			var prevValue = Value;
			if (Minus.Bounds.Contains(x, y)) {
				StepDown();
			} else if (Plus.Bounds.Contains(x, y)) {
				StepUp();
			}

			if (playSound && prevValue != Value)
				Game1.playSound("drumkit6");
		}

		public override void Draw(SpriteBatch b) {
			float buttonAlpha = Enabled ? 1f : 0.33f;
			float transparency = (Enabled && ModData.CanStepUp) ? 1f : 0.33f;
			Minus.Transparency = transparency;
			Plus.Transparency = transparency;
			Minus.Draw(b);
			Plus.Draw(b);

			b.DrawString(Game1.dialogueFont, Value.ToString() + UnitString, new Vector2(Minus.Bounds.Right + (Plus.X - Minus.Bounds.Right - ValueLabelSize.X - UnitStringWidth) / 2, Minus.Y + ((Minus.Height - MaxLabelSize.Y) / 2)), Enabled ? Game1.textColor : (Game1.textColor * 0.33f));

			var labelSize = Game1.dialogueFont.MeasureString(Label);
			Utility.drawTextWithShadow(b, Label, Game1.dialogueFont, new Vector2(Plus.Bounds.Right + Game1.pixelZoom * 4, Minus.Y + ((Minus.Height - labelSize.Y) / 2)), Game1.textColor * buttonAlpha, 1f, 0.1f, -1, -1, 1f, 3);
		}
	}
}
