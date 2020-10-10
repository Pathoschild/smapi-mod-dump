/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/StardewConfigFramework/StardewConfigMenu
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewConfigFramework.Options;
using StardewValley;
using StardewValley.Menus;

namespace StardewConfigMenu.Components {

	sealed class ConfigCheckbox: SCMControl {
		private readonly IConfigToggle ModData;

		private readonly SCMSprite Checkbox = SCMSprite.CheckboxChecked;

		bool _Visible = true;
		internal override bool Visible { get => _Visible; set => _Visible = value; }
		public override int X { get => Checkbox.X; set => Checkbox.X = value; }
		public override int Y { get => Checkbox.Y; set => Checkbox.Y = value; }
		public override int Width => Checkbox.Width;
		public override int Height => Checkbox.Height;

		public override string Label => ModData.Label;
		public override bool Enabled => ModData.Enabled;
		public bool IsChecked { get => ModData.IsOn; set => ModData.IsOn = value; }

		internal ConfigCheckbox(IConfigToggle option) : this(option, 0, 0) { }

		internal ConfigCheckbox(IConfigToggle option, int x, int y) : base(option.Label, option.Enabled) {
			ModData = option;
			X = x;
			Y = y;
		}

		public override void ReceiveLeftClick(int x, int y, bool playSound = true) {
			if (!Enabled || !IsAvailableForSelection)
				return;

			if (Checkbox.Bounds.Contains(x, y)) {
				IsChecked = !IsChecked;
				if (playSound)
					Game1.playSound("drumkit6");
			}
		}

		public override void Draw(SpriteBatch b) {
			var transparency = (Enabled) ? 1f : 0.33f;

			if (IsChecked && Checkbox.SourceRect != OptionsCheckbox.sourceRectChecked)
				Checkbox.SourceRect = OptionsCheckbox.sourceRectChecked;
			else if (!IsChecked && Checkbox.SourceRect != OptionsCheckbox.sourceRectUnchecked)
				Checkbox.SourceRect = OptionsCheckbox.sourceRectUnchecked;

			Checkbox.Transparency = transparency;
			Checkbox.Draw(b);

			var labelSize = Game1.dialogueFont.MeasureString(Label);
			Utility.drawTextWithShadow(b, Label, Game1.dialogueFont, new Vector2(Checkbox.Bounds.Right + Game1.pixelZoom * 4, Checkbox.Y + ((Checkbox.Height - labelSize.Y) / 2)), Game1.textColor * transparency, 1f, 0.1f, -1, -1, 1f, 3);
		}
	}
}

