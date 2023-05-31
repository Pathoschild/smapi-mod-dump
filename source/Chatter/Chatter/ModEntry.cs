/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jbossjaslow/Chatter
**
*************************************************/

using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Collections.Generic;

namespace Chatter {
	/// <summary>The mod entry point.</summary>
	internal sealed class ModEntry : Mod {
		/*********
        ** Properties
        *********/
		/// <summary>The mod configuration from the player.</summary>
		private ModConfig Config;
		private ShowWhenNPCNeedsChat _showWhenNPCNeedsChat;

		/*********
        ** Public methods
        *********/
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper) {
			Config = this.Helper.ReadConfig<ModConfig>();

			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
			helper.Events.Input.ButtonPressed += OnButtonPressed;
		}

		/*********
        ** Private methods
        *********/
		private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
			Dictionary<string, int> npcOffsets = this.Helper.ModContent.Load<Dictionary<string, int>>("Customization/NPCOffsets.json");
			Monitor.Log($"Successfully loaded offsets for NPCs", LogLevel.Debug);

			_showWhenNPCNeedsChat = new(Helper, Monitor, Config, npcOffsets);

			// get Generic Mod Config Menu's API (if it's installed)
			var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
			if (configMenu is null)
				return;

			// register mod
			configMenu.Register(
				mod: this.ModManifest,
				reset: () => this.Config = new ModConfig(),
				save: () => {
					this.Helper.WriteConfig(this.Config);
					_showWhenNPCNeedsChat.ToggleMod(Config.enableIndicators);
				}
			);
			Config.SetupGenericConfigMenu(ModManifest, configMenu);

			// watch for changes
			//configMenu.OnFieldChanged(
			//	mod: ModManifest,
			//	onChange: (name, value) => {
			//		if (value is null) return;

			//		switch (name) {
			//			case ModConfigField.enableIndicators:
			//				if (value is bool unwrappedValue) {
			//					_showWhenNPCNeedsChat.ToggleOption(unwrappedValue);
			//				}
			//				break;
			//		}
			//	}
			//);
		}

		/// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e) {
			//foreach (NPC npc in Utility.getAllCharacters())
			//{
			//    if (npc.CanSocialize)
			//        Debug.WriteLine(npc.Name);
			//}

			if (Config.enableIndicators) _showWhenNPCNeedsChat.ToggleMod(true);
		}

		/// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnButtonPressed(object? sender, ButtonPressedEventArgs e) {
			// ignore if player hasn't loaded a save yet
			if (!Context.IsWorldReady) return;

			if (e.Button == Config.enableIndicatorsButton) {
				Config.enableIndicators = !Config.enableIndicators;
				_showWhenNPCNeedsChat.ToggleMod(Config.enableIndicators);
				this.Helper.WriteConfig(this.Config);
			}

			if (e.Button == SButton.Down && Config.useArrowKeysToAdjustDebugOffsets) {
				Config.debugIndicatorYOffset--;
				Monitor.Log($"New y offset: {Config.debugIndicatorYOffset}", LogLevel.Debug);
				this.Helper.WriteConfig(this.Config);
			} else if (e.Button == SButton.Up && Config.useArrowKeysToAdjustDebugOffsets) {
				Config.debugIndicatorYOffset++;
				Monitor.Log($"New y offset: {Config.debugIndicatorYOffset}", LogLevel.Debug);
				this.Helper.WriteConfig(this.Config);
			} else if (e.Button == SButton.Left && Config.useArrowKeysToAdjustDebugOffsets) {
				Config.debugIndicatorXOffset--;
				Monitor.Log($"New x offset: {Config.debugIndicatorXOffset}", LogLevel.Debug);
				this.Helper.WriteConfig(this.Config);
			} else if (e.Button == SButton.Right && Config.useArrowKeysToAdjustDebugOffsets) {
				Config.debugIndicatorXOffset++;
				Monitor.Log($"New x offset: {Config.debugIndicatorXOffset}", LogLevel.Debug);
				this.Helper.WriteConfig(this.Config);
			}
		}
	}
}