using System;
using System.Collections.Generic;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

namespace StardewConfigMenu.Panel {
	static internal class Constants {
		internal const int TabNumber = 11;
	}

	internal class ModOptionsTab: ClickableComponent {

		private ModSettings controller;

		//
		// Constructors
		//
		internal ModOptionsTab(ModSettings controller, Rectangle bounds) : base(bounds, "mods", "Mod Options") {
			this.controller = controller;
			AddListeners();
		}

		internal void AddListeners() {
			RemoveListeners();
			ControlEvents.MouseChanged += receiveLeftClick;
		}

		internal void RemoveListeners() {
			ControlEvents.MouseChanged -= receiveLeftClick;
		}

		public void receiveLeftClick(object sender, EventArgsMouseStateChanged e) {

			if (e.NewState.LeftButton != ButtonState.Pressed)
				return;

			// Prevents repeating sound while holding on button
			if (e.PriorState.LeftButton == ButtonState.Pressed)
				return;

			if (!(Game1.activeClickableMenu is GameMenu))
				return;

			var menu = Game1.activeClickableMenu as GameMenu;

			if (menu.currentTab == GameMenu.mapTab)
				return;

			if (this.containsPoint(e.NewPosition.X, e.NewPosition.Y)) {
				ModOptionsPage.SetActive();
				Game1.playSound("smallSelect");
			}
		}

		public void draw(SpriteBatch b) {

			var gameMenu = (GameMenu) Game1.activeClickableMenu;

			b.Draw(Game1.mouseCursors, new Vector2((float) this.bounds.X, (float) (this.bounds.Y + (gameMenu.currentTab == 8 ? 8 : 0))), new Rectangle(16, 368, 16, 16), Color.White, 0f, Vector2.Zero, (float) Game1.pixelZoom, SpriteEffects.None, 0.0001f);

			// Draw icon
			b.Draw(Game1.mouseCursors, new Vector2((float) this.bounds.X + 8, (float) (this.bounds.Y + (gameMenu.currentTab == 8 ? 8 : 0)) + 14), new Rectangle(32, 672, 16, 16), Color.White, 0, Vector2.Zero, 3f, SpriteEffects.None, 1);
			if (this.containsPoint(Game1.getMouseX(), Game1.getMouseY())) {
				IClickableMenu.drawHoverText(Game1.spriteBatch, base.label, Game1.smallFont);
			}

			string hoverText = ModEntry.helper.Reflection.GetField<string>(Game1.activeClickableMenu, "hoverText").GetValue();

			// Redraw hover text so that it overlaps icon
			if (hoverText == "Exit Game") {
				Utilities.drawHoverTextWithoutShadow(b, "Exit Game", Game1.smallFont);
			}

			//IClickableMenu.drawMouse(b);
			if (!Game1.options.hardwareCursor) {
				b.Draw(Game1.mouseCursors, new Vector2((float) Game1.getMouseX(), (float) Game1.getMouseY()), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, (!Game1.options.SnappyMenus) ? 0 : 44, 16, 16)), Color.White * Game1.mouseCursorTransparency, 0f, Vector2.Zero, (float) Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
			}
		}
	}
}
