using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using StardewConfigFramework;
using StardewConfigMenu.Panel;
using System.IO;

namespace StardewConfigMenu {
	public class ModSettings: IModSettingsFramework {
		public static int? pageIndex = null;
		static internal ModEntry Mod;

		internal ModSettings(ModEntry mod) {
			Mod = mod;
			Instance = this;
			MenuEvents.MenuChanged += MenuOpened;
			MenuEvents.MenuClosed += MenuClosed;
		}

		/*
		public override void SaveModOptions(ModOptions options)
		{
				string path = Path.Combine("mods", $"{options.modManifest.UniqueID}.json");
				Mod.Helper.WriteJsonFile<ModOptions>(path, options);
		}

		public override ModOptions LoadModOptions(Mod mod)
		{
				string path = Path.Combine("mods", $"{mod.ModManifest.UniqueID}.json");
				var data = Mod.Helper.ReadJsonFile<ModOptions>(path);
				if (data == null)
						return new ModOptions(mod);
				else return data;
		}*/

		static internal void Log(string str, LogLevel level = LogLevel.Debug) {
			Mod.Monitor.Log(str, level);
		}

		//internal SettingsPage page;
		internal ModOptionsTab tab;
		internal ModOptionsPage page;

		internal List<ModOptions> ModOptionsList = new List<ModOptions>();

		public override void AddModOptions(ModOptions modOptions) {
			// Only one per mod, remove old one
			foreach (ModOptions mod in this.ModOptionsList) {
				if (mod.modManifest.Name == modOptions.modManifest.Name) {
					ModOptionsList.Remove(mod);
				}
			}

			Mod.Monitor.Log($"{modOptions.modManifest.Name} has added its mod options");

			ModOptionsList.Add(modOptions);
		}

		/// <summary>
		/// Removes the delegates that handle the button click and draw method of the tab
		/// </summary>
		private void MenuClosed(object sender, EventArgsClickableMenuClosed e) {
			GraphicsEvents.OnPostRenderGuiEvent -= RenderTab;
			GraphicsEvents.OnPreRenderGuiEvent -= HandleJunimo;

			if (this.tab != null) {
				this.tab.RemoveListeners();
				this.tab = null;
			}

			if (this.page != null) {
				if (e.PriorMenu is GameMenu) {
					List<IClickableMenu> pages = ModEntry.helper.Reflection.GetField<List<IClickableMenu>>((e.PriorMenu as GameMenu), "pages").GetValue();
					pages.Remove(this.page);
				}

				this.page.RemoveListeners(true);
				this.page = null;
				ModSettings.pageIndex = null;
			}
		}

		/// <summary>
		/// Attaches the delegates that handle the button click and draw method of the tab
		/// </summary>
		private void MenuOpened(object sender, EventArgsClickableMenuChanged e) {
			// copied from MenuClosed
			GraphicsEvents.OnPostRenderGuiEvent -= RenderTab;
			GraphicsEvents.OnPreRenderGuiEvent -= HandleJunimo;

			if (this.tab != null) {
				this.tab.RemoveListeners();
				this.tab = null;
			}

			if (this.page != null) {

				if (e.PriorMenu is GameMenu) {
					List<IClickableMenu> oldpages = ModEntry.helper.Reflection.GetField<List<IClickableMenu>>((e.PriorMenu as GameMenu), "pages").GetValue();
					oldpages.Remove(this.page);
				}

				this.page.RemoveListeners(true);
				this.page = null;
				ModSettings.pageIndex = null;
			}

			if (!(e.NewMenu is GameMenu)) {
				this.tab = null;
				this.page = null;
				ModSettings.pageIndex = null;
				return;
			}

			GameMenu menu = (GameMenu) e.NewMenu;
			List<IClickableMenu> pages = ModEntry.helper.Reflection.GetField<List<IClickableMenu>>(menu, "pages").GetValue();

			var options = pages.Find(x => { return x is OptionsPage; });
			int width = options.width;

			//List<ClickableComponent> tabs = ModEntry.helper.Reflection.GetPrivateField<List<ClickableComponent>>(menu, "tabs").GetValue();

			this.page = new ModOptionsPage(this, menu.xPositionOnScreen, menu.yPositionOnScreen, width, menu.height);
			ModSettings.pageIndex = pages.Count;
			pages.Add(page);

			bool infoSuiteInstalled = Mod.Helper.ModRegistry.IsLoaded("Cdaragorn.UiInfoSuite");
			int tabLocation = infoSuiteInstalled ? 9 : 11;
			this.tab = new ModOptionsTab(this, new Rectangle(menu.xPositionOnScreen + Game1.tileSize * tabLocation, menu.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + Game1.tileSize, Game1.tileSize, Game1.tileSize));

			GraphicsEvents.OnPostRenderGuiEvent -= RenderTab;
			GraphicsEvents.OnPostRenderGuiEvent += RenderTab;
			GraphicsEvents.OnPreRenderGuiEvent -= HandleJunimo;
			GraphicsEvents.OnPreRenderGuiEvent += HandleJunimo;

		}

		private ClickableTextureComponent junimoNoteIconStorage;

		private void HandleJunimo(object sender, EventArgs e) {
			if (!(Game1.activeClickableMenu is GameMenu))
				return;

			var gameMenu = Game1.activeClickableMenu as GameMenu;

			// Remove Community Center Icon from Options, Exit Game, and Mod Options pages
			if (gameMenu.currentTab == ModSettings.pageIndex || gameMenu.currentTab == 6 || gameMenu.currentTab == 7) {
				if (gameMenu.junimoNoteIcon != null) {
					this.junimoNoteIconStorage = gameMenu.junimoNoteIcon;
					gameMenu.junimoNoteIcon = null;
				}
			} else if (this.junimoNoteIconStorage != null) {
				gameMenu.junimoNoteIcon = this.junimoNoteIconStorage;
				this.junimoNoteIconStorage = null;
			}
		}


		private void RenderTab(object sender, EventArgs e) {

			if (!(Game1.activeClickableMenu is GameMenu))
				return;

			var gameMenu = (GameMenu) Game1.activeClickableMenu;

			if (gameMenu.currentTab == GameMenu.mapTab) { return; }

			if (tab != null)
				tab.draw(Game1.spriteBatch);

			//var b = Game1.spriteBatch;
			//this.tab.draw(b);
		}
	}
}
