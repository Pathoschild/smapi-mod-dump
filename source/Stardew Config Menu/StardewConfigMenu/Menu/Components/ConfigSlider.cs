/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/StardewConfigFramework/StardewConfigMenu
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewConfigFramework.Options;
using StardewConfigMenu.UI;
using StardewValley;
using StardewValley.Menus;

namespace StardewConfigMenu.Components {

	sealed class ConfigSlider: SCMControl {
		readonly IConfigRange ModData;

		private SCMTextureBox SliderBackground = SCMTextureBox.SliderBackground;
		private SCMSprite SliderBar = SCMSprite.SliderBar;

		private Vector2 MaxLabelSize = Vector2.Zero;
		private Point Origin = new Point();
		public override int X {
			get => Origin.X;
			set {
				if (Origin.X == value)
					return;

				Origin.X = value;
				SliderBackground.X = value;
				if (ShowValue)
					SliderBackground.X += (int) MaxLabelSize.X + (4 * Game1.pixelZoom);

				UpdateSliderLocation(Value, Min, Max, StepSize);
			}
		}
		public override int Y {
			get => Origin.Y;
			set {
				if (Origin.Y == value)
					return;

				Origin.Y = value;
				SliderBackground.Y = value;
				SliderBar.Y = value;
			}
		}
		public override int Height => Math.Max(SliderBackground.Height, (int) MaxLabelSize.Y);
		public override int Width => SliderBackground.Bounds.Right - Origin.X;

		public override bool Enabled => ModData.Enabled;
		public override string Label => ModData.Label;
		public decimal Min => ModData.Min;
		public decimal Max => ModData.Max;
		public decimal StepSize => ModData.StepSize;
		public bool ShowValue => ModData.ShowValue;
		public decimal Value { get => ModData.Value; set => ModData.Value = value; }

		internal ConfigSlider(IConfigRange option) : this(option, 0, 0) { }

		internal ConfigSlider(IConfigRange option, int x, int y) : base(option.Label, option.Enabled) {
			ModData = option;
			CalculateMaxLabelSize();
			SliderBackground.Width = OptionsSlider.pixelsWide * Game1.pixelZoom;
			SliderBackground.Height = OptionsSlider.pixelsHigh * Game1.pixelZoom;
			//SliderBar.bounds.Width = OptionsSlider.sliderButtonWidth * Game1.pixelZoom;
			//SliderBar.bounds.Height = OptionsSlider.pixelsHigh * Game1.pixelZoom;
			X = x;
			Y = y;
		}

		private void CalculateMaxLabelSize() {
			var maxRect = Game1.dialogueFont.MeasureString((Max + StepSize % 1).ToString());
			var minRect = Game1.dialogueFont.MeasureString((Min - StepSize % 1).ToString());

			MaxLabelSize = (maxRect.X > minRect.X) ? maxRect : minRect;
		}

		private void UpdateSliderLocation(decimal value, decimal min, decimal max, decimal stepSize) {
			var sectionNum = ((value - min) / stepSize);
			var totalSections = ((max - min) / stepSize);
			var sectionWidth = (SliderBackground.Width - SliderBar.Width) / totalSections;
			SliderBar.X = SliderBackground.X + (int) (sectionWidth * sectionNum);
		}

		private decimal GetValueFromMouseLocation(int x, decimal min, decimal max, decimal stepSize) {
			var halfBarWidth = SliderBar.Width / 2;

			if (x < SliderBackground.X + halfBarWidth) {
				return Min;
			}
			if (x > SliderBackground.Bounds.Right - halfBarWidth) {
				return Max;
			}

			var totalSections = ((max - min) / stepSize);
			var sectionWidth = (SliderBackground.Width - SliderBar.Width) / totalSections;
			var sectionNum = (x - (SliderBackground.X + halfBarWidth)) / sectionWidth;
			return Min + sectionNum * StepSize;
		}

		private decimal CheckValidInput(decimal input) {
			if (input > Max)
				return Max;

			if (input < Min)
				return Min;

			return ((input - Min) / StepSize) * StepSize + Min;
		}

		public override void ReceiveLeftClick(int x, int y, bool playSound = true) {
			if (!Enabled || !IsAvailableForSelection)
				return;

			if (SliderBackground.Bounds.Contains(x, y)) {
				RegisterAsActiveComponent();
				LeftClickHeld(x, y);
			}
		}

		public override void LeftClickHeld(int x, int y) {
			if (IsActiveComponent) {
				Value = GetValueFromMouseLocation(x, Min, Max, StepSize);
				UpdateSliderLocation(Value, Min, Max, StepSize);
			}
		}

		public override void ReleaseLeftClick(int x, int y) {
			UnregisterAsActiveComponent();
		}

		public override void Draw(SpriteBatch b) {
			var labelSize = Game1.dialogueFont.MeasureString(Label);
			var valueLabelSize = Game1.dialogueFont.MeasureString($"{Value}");
			float buttonAlpha = Enabled ? 1f : 0.33f;

			if (ShowValue)
				b.DrawString(Game1.dialogueFont, $"{Value}", new Vector2(SliderBackground.X - ((MaxLabelSize.X - valueLabelSize.X) / 2 + valueLabelSize.X + 4 * Game1.pixelZoom), SliderBackground.Y + ((SliderBackground.Height - labelSize.Y) / 2)), Game1.textColor * buttonAlpha);

			SliderBackground.Transparency = buttonAlpha;
			SliderBackground.Draw(b);
			SliderBar.Transparency = buttonAlpha;
			SliderBar.Draw(b);

			Utility.drawTextWithShadow(b, Label, Game1.dialogueFont, new Vector2(SliderBackground.Bounds.Right + Game1.pixelZoom * 4, SliderBackground.Y + ((SliderBackground.Height - labelSize.Y) / 2)), Game1.textColor * buttonAlpha, 1f, 0.1f, -1, -1, 1f, 3);
		}
	}
}
