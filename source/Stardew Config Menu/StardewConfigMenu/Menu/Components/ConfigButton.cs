/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/StardewConfigFramework/StardewConfigMenu
**
*************************************************/

using StardewConfigFramework.Options;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace StardewConfigMenu.Components {

	sealed class ConfigButton: SCMControl {
		private readonly IConfigAction ModData;

		private SCMSprite Button;
		public sealed override int X { get => Button.X; set => Button.X = value; }
		public sealed override int Y { get => Button.Y; set => Button.Y = value; }
		public sealed override int Width => Button.Width;
		public sealed override int Height => Button.Height;

		bool _Visible = true;
		public sealed override string Label { get => ModData.Label; }
		public sealed override bool Enabled { get => ModData.Enabled; }
		internal sealed override bool Visible { get => _Visible; set => _Visible = value; }
		public ButtonType ButtonType => ModData.ButtonType;
		private ButtonType PreviousButtonType = ButtonType.OK;

		internal ConfigButton(IConfigAction option) : this(option, 0, 0) { }

		internal ConfigButton(IConfigAction option, int x, int y) : base(option.Label, option.Enabled) {
			ModData = option;

			Button = GetButtonTile();
			X = x;
			Y = y;
		}

		private SCMSprite GetButtonTile() {
			switch (ButtonType) {
				case ButtonType.DONE:
					return SCMSprite.DoneButton;
				case ButtonType.CLEAR:
					return SCMSprite.ClearButton;
				case ButtonType.OK:
					return SCMSprite.OKButton;
				case ButtonType.SET:
					return SCMSprite.SetButton;
				case ButtonType.GIFT:
					return SCMSprite.GiftButton;
				default:
					return SCMSprite.OKButton;
			}
		}

		public override void ReceiveLeftClick(int x, int y, bool playSound = true) {
			if (!Enabled || !IsAvailableForSelection)
				return;

			if (Button.Bounds.Contains(x, y)) {
				if (playSound)
					Game1.playSound("breathin");
				ModData.Trigger();
			}
		}

		private void CheckForButtonUpdate() {
			if (PreviousButtonType == ButtonType)
				return;

			var newButton = GetButtonTile();
			newButton.X = X;
			newButton.Y = Y;
			Button = newButton;
			PreviousButtonType = ButtonType;
		}

		public override void Draw(SpriteBatch b, int x, int y) {
			if (PreviousButtonType != ButtonType) {
				Button = GetButtonTile();
				PreviousButtonType = ButtonType;
			}

			base.Draw(b, x, y);
		}

		public override void Draw(SpriteBatch b) {
			var labelSize = Game1.dialogueFont.MeasureString(Label);
			var colorAlpha = (Enabled) ? 1f : 0.33f;
			Button.Transparency = colorAlpha;

			Button.Draw(b);

			Utility.drawTextWithShadow(b, Label, Game1.dialogueFont, new Vector2(Button.Bounds.Right + Game1.pixelZoom * 4, Y + ((Height - labelSize.Y) / 2)), Game1.textColor * colorAlpha, 1f, 0.1f, -1, -1, 1f, 3);
		}
	}
}
