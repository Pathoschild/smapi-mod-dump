/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/StardewConfigFramework/StardewConfigMenu
**
*************************************************/

using System.Collections.Generic;
using StardewConfigFramework.Options;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using Microsoft.Xna.Framework;
using StardewConfigMenu.UI;

namespace StardewConfigMenu.Components {

	sealed class ConfigDropdown: SCMControl {
		readonly private IConfigSelection ModData;

		private int hoveredIndex = 0;

		readonly SCMSprite DropdownButton = SCMSprite.DropDownButton;
		SCMTextureBox DropdownBackground = SCMTextureBox.DropdownBackground;

		public sealed override int X {
			get => DropdownBackground.Bounds.X;
			set {
				DropdownBackground.X = value;
				DropdownButton.X = DropdownBackground.Bounds.Right;
			}
		}
		public sealed override int Y {
			get => DropdownButton.Y;
			set {
				DropdownBackground.Y = value;
				DropdownButton.Y = value;
			}
		}
		public sealed override int Height => DropdownButton.Height;
		public sealed override int Width {
			get => DropdownBackground.Bounds.Width + DropdownButton.Width;
			set {
				int width = Math.Max(value - DropdownButton.Width, 15);
				DropdownBackground.Width = width;
				DropdownButton.X = DropdownBackground.Bounds.Right;
			}
		}

		public override string Label => ModData.Label;
		public override bool Enabled => (DropdownOptions.Count > 0) && ModData.Enabled;
		internal override bool Visible {
			get => _Visible;
			set {
				_Visible = value;
			}
		}

		bool _Visible = true;

		public int SelectedIndex { get => ModData.SelectedIndex; set => ModData.SelectedIndex = value; }
		private IList<ISelectionChoice> DropdownOptions => ModData.Choices;

		public ConfigDropdown(IConfigSelection option, int width) : this(option, width, 0, 0) { }

		public ConfigDropdown(IConfigSelection option, int width, int x, int y) : base(option.Label, option.Enabled) {
			ModData = option;
			X = x;
			Y = y;
			Width = width;
			DropdownBackground.Height = DropdownButton.Height;
		}

		private bool containsPoint(int x, int y) {
			return (DropdownBackground.Bounds.Contains(x, y) || DropdownButton.Bounds.Contains(x, y));
		}

		public override void ReceiveLeftClick(int x, int y, bool playSound = true) {
			if (!Enabled || !IsAvailableForSelection)
				return;

			if (containsPoint(x, y)) {
				RegisterAsActiveComponent();
				DropdownBackground.Height = DropdownButton.Height * DropdownOptions.Count;
				hoveredIndex = 0;
				LeftClickHeld(x, y);
				if (playSound)
					Game1.playSound("shwip");
			}
		}

		public override void LeftClickHeld(int x, int y) {
			if (!Enabled || !IsActiveComponent)
				return;

			if (DropdownBackground.Bounds.Contains(x, y)) {
				DropdownBackground.Y = Math.Min(DropdownBackground.Bounds.Y, Game1.viewport.Height - DropdownBackground.Bounds.Height);
				hoveredIndex = (int) Math.Max(Math.Min((y - DropdownBackground.Bounds.Y) / DropdownButton.Height, DropdownOptions.Count - 1), 0f);
			}
		}

		public override void ReleaseLeftClick(int x, int y) {
			if (!IsActiveComponent)
				return;

			UnregisterAsActiveComponent();

			if (DropdownBackground.Bounds.Contains(x, y)) {
				if (Enabled && DropdownOptions.Count > 0) {
					SelectHoveredOption();
				}
			}
			DropdownBackground.Height = DropdownButton.Height;
		}

		private void SelectHoveredOption() {
			if (hoveredIndex == 0)
				return; // 0 is the already selected option

			SelectedIndex = hoveredIndex > SelectedIndex ? hoveredIndex : hoveredIndex - 1;
		}

		public override void Draw(SpriteBatch b) {

			float buttonAlpha = (Enabled) ? 1f : 0.33f;
			DropdownButton.Transparency = buttonAlpha;
			DropdownBackground.Transparency = buttonAlpha;
			var labelSize = Game1.dialogueFont.MeasureString(Label);

			// Draw Label
			Utility.drawTextWithShadow(b, Label, Game1.dialogueFont, new Vector2(DropdownButton.Bounds.Right + Game1.pixelZoom * 2, DropdownButton.Y + ((DropdownButton.Height - labelSize.Y) / 2)), Game1.textColor * buttonAlpha, 1f, 0.1f, -1, -1, 1f, 3);

			if (IsActiveComponent) {

				DropdownButton.Draw(b);
				DropdownBackground.Draw(b);

				if (0 == hoveredIndex && DropdownBackground.Bounds.Contains(Game1.getMouseX(), Game1.getMouseY())) {
					b.Draw(Game1.staminaRect, new Rectangle(DropdownBackground.Bounds.X, DropdownBackground.Bounds.Y + hoveredIndex * DropdownButton.Height, DropdownBackground.Bounds.Width, DropdownButton.Height), new Rectangle?(new Rectangle(0, 0, 1, 1)), Color.Wheat, 0f, Vector2.Zero, SpriteEffects.None, 0.975f);
				}
				b.DrawString(Game1.smallFont, DropdownOptions[SelectedIndex].Label, new Vector2(DropdownBackground.Bounds.X + Game1.pixelZoom, DropdownBackground.Bounds.Y + Game1.pixelZoom * 2), Game1.textColor * buttonAlpha, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.98f);

				int drawnPosition = 1;
				for (int i = 0; i < DropdownOptions.Count; i++) {
					if (i == SelectedIndex)
						continue;

					if (drawnPosition == hoveredIndex && DropdownBackground.Bounds.Contains(Game1.getMouseX(), Game1.getMouseY())) {
						b.Draw(Game1.staminaRect, new Rectangle(DropdownBackground.Bounds.X, DropdownBackground.Bounds.Y + drawnPosition * DropdownButton.Height, DropdownBackground.Bounds.Width, DropdownButton.Height), new Rectangle?(new Rectangle(0, 0, 1, 1)), Color.Wheat, 0f, Vector2.Zero, SpriteEffects.None, 0.975f);
					}
					b.DrawString(Game1.smallFont, DropdownOptions[i].Label, new Vector2(DropdownBackground.Bounds.X + Game1.pixelZoom, DropdownBackground.Bounds.Y + Game1.pixelZoom * 2 + DropdownButton.Height * drawnPosition), Game1.textColor * buttonAlpha, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.98f);
					drawnPosition++;
				}
			} else {

				DropdownButton.Draw(b);
				DropdownBackground.Draw(b);

				b.DrawString(Game1.smallFont, (SelectedIndex >= DropdownOptions.Count) ? string.Empty : DropdownOptions[SelectedIndex].Label, new Vector2(DropdownBackground.Bounds.X + Game1.pixelZoom, DropdownBackground.Bounds.Y + Game1.pixelZoom * 2), Game1.textColor * buttonAlpha, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.88f);

				if (Enabled && DropdownBackground.Bounds.Contains(Game1.getMouseX(), Game1.getMouseY())) {
					if (DropdownOptions[SelectedIndex].HoverText != null) {
						string optionDescription = Utilities.GetWordWrappedString(DropdownOptions[SelectedIndex].HoverText);
						IClickableMenu.drawHoverText(b, optionDescription, Game1.smallFont);
					}
				}
			}
		}
	}
}
