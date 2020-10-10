/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/StardewConfigFramework/StardewConfigMenu
**
*************************************************/

using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using StardewConfigMenu.Components;
using Microsoft.Xna.Framework.Input;
using StardewConfigFramework;
using StardewConfigFramework.Options;

namespace StardewConfigMenu {
	public class MenuPage: IClickableMenu {

		private readonly List<IOptionsPackage> Packages;
		private readonly List<ModSheet> Sheets = new List<ModSheet>();
		private readonly ConfigDropdown ModDropdown;
		private readonly ConfigSelection ModSelection;

		internal MenuPage(List<IOptionsPackage> packages, int x, int y, int width, int height) : base(x, y, width, height, false) {
			Packages = packages;

			var modChoices = new List<ISelectionChoice>();

			foreach (IOptionsPackage package in packages) {
				// Skip mods with no tabs
				if (package.Tabs.Count < 1)
					continue;

				// Create mod page and add it, hide it initially
				var sheet = new ModSheet(package, xPositionOnScreen + Game1.pixelZoom * 15, yPositionOnScreen + Game1.pixelZoom * 55, width - (Game1.pixelZoom * 15), height - Game1.pixelZoom * 65) {
					Visible = false
				};
				Sheets.Add(sheet);

				// Add names to mod selector dropdown
				modChoices.Add(new SelectionChoice(package.ModManifest.UniqueID, package.ModManifest.Name, package.ModManifest.Description));
			}

			ModSelection = new ConfigSelection("modDropdown", "", modChoices);
			ModDropdown = new ConfigDropdown(ModSelection, (int) Game1.smallFont.MeasureString("Stardew Configuration Menu Framework").X, (int) (xPositionOnScreen + Game1.pixelZoom * 15), (int) (yPositionOnScreen + Game1.pixelZoom * 30)) {
				Visible = true
			};

			if (Sheets.Count > 0)
				Sheets[ModDropdown.SelectedIndex].Visible = true;

			AddListeners();
		}

		static public void SetActive() {
			var gameMenu = (GameMenu) Game1.activeClickableMenu;
			if (MenuController.PageIndex != null)
				gameMenu.currentTab = (int) MenuController.PageIndex;
		}

		internal void AddListeners() {
			RemoveListeners();

			ControlEvents.MouseChanged += MouseChanged;
			ModSelection.SelectionDidChange += DisableBackgroundSheets;
		}

		internal void RemoveListeners(bool children = false) {
			if (children) {
				ModDropdown.Visible = false;
				Sheets.ForEach(x => { x.RemoveListeners(true); });
			}

			ModSelection.SelectionDidChange -= DisableBackgroundSheets;
			ControlEvents.MouseChanged -= MouseChanged;

		}

		private void DisableBackgroundSheets(IConfigSelection selection) {
			for (int i = 0; i < Sheets.Count; i++) {
				Sheets[i].Visible = (i == selection.SelectedIndex);
			}
		}

		protected virtual void MouseChanged(object sender, EventArgsMouseStateChanged e) {
			if (GameMenu.forcePreventClose) { return; }
			if (!(Game1.activeClickableMenu is GameMenu)) { return; } // must be main menu
			if ((Game1.activeClickableMenu as GameMenu).currentTab != MenuController.PageIndex) { return; } //must be mod tab

			var currentSheet = Sheets.Find(x => x.Visible);

			if (e.NewState.ScrollWheelValue > e.PriorState.ScrollWheelValue) {
				if (currentSheet != null)
					currentSheet.receiveScrollWheelAction(1);
			} else if (e.NewState.ScrollWheelValue < e.PriorState.ScrollWheelValue) {
				if (currentSheet != null)
					currentSheet.receiveScrollWheelAction(-1);
			}

			if (e.PriorState.LeftButton == ButtonState.Released) {
				if (e.NewState.LeftButton == ButtonState.Pressed) {
					// clicked
					if (currentSheet != null)
						currentSheet.receiveLeftClick(e.NewPosition.X, e.NewPosition.Y);
					ModDropdown.ReceiveLeftClick(e.NewPosition.X, e.NewPosition.Y);
				}
			} else if (e.PriorState.LeftButton == ButtonState.Pressed) {
				if (e.NewState.LeftButton == ButtonState.Pressed) {
					if (currentSheet != null)
						currentSheet.leftClickHeld(e.NewPosition.X, e.NewPosition.Y);
					ModDropdown.LeftClickHeld(e.NewPosition.X, e.NewPosition.Y);
				} else if (e.NewState.LeftButton == ButtonState.Released) {
					if (currentSheet != null)
						currentSheet.releaseLeftClick(e.NewPosition.X, e.NewPosition.Y);
					ModDropdown.ReleaseLeftClick(e.NewPosition.X, e.NewPosition.Y);
				}
			}
		}

		public override void draw(SpriteBatch b) {
			if (!(Game1.activeClickableMenu is GameMenu)) { return; } // must be main menu
			if ((Game1.activeClickableMenu as GameMenu).currentTab != MenuController.PageIndex) { return; } //must be mod tab

			Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true, null, false);

			base.drawHorizontalPartition(b, (int) (yPositionOnScreen + Game1.pixelZoom * 40));

			SpriteText.drawString(b, "Mod Options", ModDropdown.X + ModDropdown.Width + Game1.pixelZoom * 5, ModDropdown.Y);

			if (Sheets.Count > 0)
				Sheets[ModDropdown.SelectedIndex].draw(b);

			// draw mod select dropdown last, should cover mod settings
			ModDropdown.Draw(b);
		}
	}
}
