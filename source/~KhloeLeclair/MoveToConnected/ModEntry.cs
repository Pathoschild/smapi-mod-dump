/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections;

using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.Integrations.GenericModConfigMenu;
using Leclair.Stardew.Common.UI.Overlay;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using StardewValley;
using StardewValley.Menus;

using Leclair.Stardew.MoveToConnected.Providers;

namespace Leclair.Stardew.MoveToConnected {
	public class ModEntry : ModSubscriber {

		public static ModEntry instance;

		private readonly PerScreen<IClickableMenu> CurrentMenu = new();
		private readonly PerScreen<IOverlay> CurrentOverlay = new();

		public ModConfig Config;

		private GMCMIntegration<ModConfig, ModEntry> intGMGC;

		private Hashtable menuProviders = new Hashtable();
		private readonly object providerLock = new object();

		// Textures
		internal Texture2D Buttons;

		public override void Entry(IModHelper helper) {
			base.Entry(helper);

			instance = this;

			// Read Config
			Config = Helper.ReadConfig<ModConfig>();

			// Load Textures
			Buttons = Helper.Content.Load<Texture2D>("assets/buttons.png");

			// Init
			I18n.Init(helper.Translation);
		}

		#region Events

		[Subscriber]
		private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
			// More Init
			RegisterSettings();
		}

		[Subscriber]
		private void OnUpdateTicked(object sender, UpdateTickedEventArgs e) {
			IClickableMenu menu = Game1.activeClickableMenu;
			if (CurrentMenu.Value == menu)
				return;

			CurrentMenu.Value = menu;

			if (CurrentOverlay.Value != null) {
				CurrentOverlay.Value.Dispose();
				CurrentOverlay.Value = null;
			}

			var provider = GetMenuProvider(menu);
			if (provider == null || !provider.IsValid(menu, Game1.player))
				return;

			CurrentOverlay.Value = provider.CreateOverlay(menu, Game1.player);
		}

		#endregion

		#region Configuration

		public void SaveConfig() {
			Helper.WriteConfig(Config);
		}

		public bool HasGMCM() {
			return intGMGC?.IsLoaded ?? false;
		}

		public void OpenGMCM() {
			intGMGC?.OpenMenu();
		}

		private void RegisterSettings() {
			intGMGC = new GMCMIntegration<ModConfig, ModEntry>(this, () => Config, () => Config = new(), () => SaveConfig());

			intGMGC.Register(true);
		}

		#endregion

		#region Providers

		private void RegisterProviders() {

		}

		public void RegisterMenuProvider<T>(IMenuProvider<T> provider) where T : IClickableMenu {
			RegisterMenuProvider(typeof(T), provider);
		}

		public void RegisterMenuProvider<T>(Type type, IMenuProvider<T> provider) where T : IClickableMenu {
			lock (providerLock) {
				menuProviders[type] = provider;
			}
		}

		public IMenuProvider<T> GetMenuProvider<T>(T menu) where T : IClickableMenu {
			Type type = menu == null ? null : menu.GetType();
			if (menu == null || !menuProviders.Contains(type))
				return null;

			return (IMenuProvider<T>)menuProviders[type];
		}

		#endregion
	}
}
