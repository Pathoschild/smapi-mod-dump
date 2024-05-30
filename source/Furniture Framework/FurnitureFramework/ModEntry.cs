/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Leroymilo/FurnitureFramework
**
*************************************************/

using GenericModConfigMenu;
using GMCMOptions;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;


namespace FurnitureFramework
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
		static IMonitor? monitor;
		static IModHelper? helper;
		static ModConfig? config;

		public static bool print_debug = false;

		static public IModHelper get_helper()
		{
			if (helper == null) throw new NullReferenceException("Helper was not set.");
			return helper;
		}

		static public ModConfig get_config()
		{
			if (config == null) throw new NullReferenceException("Config was not set.");
			return config;
		}

		static public void log(string message, LogLevel log_level = LogLevel.Debug)
		{
			if (monitor == null) throw new NullReferenceException("Monitor was not set.");
			monitor.Log(message, log_level);
		}

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
			// Harmony.DEBUG = true;

			monitor = Monitor;
			ModEntry.helper = Helper;
			config = helper.ReadConfig<ModConfig>();
			HarmonyPatcher.harmony = new(ModManifest.UniqueID);
            helper.Events.Input.ButtonPressed += on_button_pressed;
			helper.Events.GameLoop.GameLaunched += on_game_launched;
			helper.Events.Content.AssetRequested += on_asset_requested;
			helper.Events.World.FurnitureListChanged += on_furniture_list_changed;
			
			HarmonyPatcher.patch();
        }

		#region On Game Launched

        /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
		private void on_game_launched(object? sender, GameLaunchedEventArgs e)
		{
			parse_furniture_packs();
			register_config();
			register_commands();

			if (
				Helper.ModRegistry.IsLoaded("PeacefulEnd.AlternativeTextures")
				&& !get_config().disable_AT_warning
			)
			{
				log("Furniture made with the Furniture Framework mod are not compatible with Alternative Textures.", LogLevel.Warn);
				log("You can disable this message in the config of the Furniture Framework.", LogLevel.Warn);
			}
		}

		private void register_config()
		{
			if (config == null) throw new NullReferenceException("Config was not set.");

			// get Generic Mod Config Menu's API (if it's installed)
			var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
			if (configMenu is null)
				return;

			// register mod
			configMenu.Register(
				mod: ModManifest,
				reset: () => config = new ModConfig(),
				save: () => Helper.WriteConfig(config)
			);

			configMenu.AddKeybind(
				mod: ModManifest,
				name: () => "Slot Place Keybind",
				tooltip: () => "The key to press to place an item in a slot.",
				getValue: () => config.slot_place_key,
				setValue: value => config.slot_place_key = value
			);

			configMenu.AddKeybind(
				mod: ModManifest,
				name: () => "Slot Take Keybind",
				tooltip: () => "The key to press to take an item from a slot.",
				getValue: () => config.slot_take_key,
				setValue: value => config.slot_take_key = value
			);
			
			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Disable AT Warning",
				tooltip: () => "Check this to disable the warning about Alternative Textures.",
				getValue: () => config.disable_AT_warning,
				setValue: value => config.disable_AT_warning = value
			);

			configMenu.AddPageLink(
				mod: ModManifest,
				pageId: $"{ModManifest.UniqueID}.slots",
				text: () => "Slots Debug Options",
				tooltip: () => "Options to draw slots areas for debugging purposes."
			);

			configMenu.AddPage(
				mod: ModManifest,
				pageId: $"{ModManifest.UniqueID}.slots",
				pageTitle: () => "Slots Debug Options"
			);

			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Enable slots debug",
				tooltip: () => "Check this to draw a colored rectangle over the areas of Furniture slots.",
				getValue: () => config.enable_slot_debug,
				setValue: value => config.enable_slot_debug = value
			);

			configMenu.AddNumberOption(
				mod: ModManifest,
				getValue: () => config.slot_debug_alpha,
				setValue: value => config.slot_debug_alpha = Math.Clamp(value, 0f, 1f),
				name: () => "Slot Debug Opacity",
				tooltip: () => "The opacity of rectangles drawn over the areas of Furniture slots.",
				min: 0f, max: 1f, interval: 0.01f
			);

			// get GMCM Options' API (if it's installed)
			var configMenuExt = Helper.ModRegistry.GetApi<IGMCMOptionsAPI>("jltaylor-us.GMCMOptions");
			if (configMenuExt is null)
				return;

			configMenuExt.AddColorOption(
				mod: ModManifest,
				getValue: () => config.slot_debug_default_color,
				setValue: value => config.slot_debug_default_color = value,
				name: () => "Default Slot Debug Color",
				tooltip: () => "The default color of the rectangles drawn over the areas of Furniture slots. It will only update on Pack reload or restart.",
				showAlpha: false,
				colorPickerStyle: (uint)IGMCMOptionsAPI.ColorPickerStyle.HSLColorWheel
			);
		}

		#region Commands

		private void register_commands()
		{
			Helper.ConsoleCommands.Add(
				"reload_furniture_pack",
				"Reloads a Furniture Pack.\n\nUsage: reload_furniture_pack <ModID>\n- ModID: the UniqueID of the Furniture Pack to reload.",
				FurniturePack.reload_pack
			);
		}

		#endregion

		#region Furniture Pack Parsing

		private void parse_furniture_packs()
		{
			foreach (IContentPack pack in Helper.ContentPacks.GetOwned())
			{
				FurniturePack.load_pack(pack);
			}

			log("Finished loading Furniture Types.");
			Helper.GameContent.InvalidateCache("Data/Furniture");
			Helper.GameContent.InvalidateCache("Data/Shops");
		}

		#endregion

		#endregion

        /// <inheritdoc cref="IInputEvents.ButtonPressed"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void on_button_pressed(object? sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
			
			// if (e.Button == SButton.K)
			// {
			// 	print_debug = !print_debug;
			// 	log($"=== Debug Print {(print_debug ? "On": "Off")} ===", LogLevel.Info);
			// }

			#region Slot Interactions

			if (!Game1.player.CanMove) return;

			bool clicked_slot = false;
			
			Point pos = new(Game1.viewport.X + Game1.getOldMouseX(), Game1.viewport.Y + Game1.getOldMouseY());

			if (e.Button == get_config().slot_place_key)
			{
				foreach (Furniture item in Game1.currentLocation.furniture)
				{
					FurniturePack.try_get_type(item, out FurnitureType? type);
					if (type == null || !type.is_table) continue;

					if (type.place_in_slot(item, pos, Game1.player))
					{
						Helper.Input.Suppress(get_config().slot_place_key);
						clicked_slot = true;
						break;
					}
				}
			}

			if (e.Button == get_config().slot_take_key && !clicked_slot)
			{
				foreach (Furniture item in Game1.currentLocation.furniture)
				{
					FurniturePack.try_get_type(item, out FurnitureType? type);
					if (type == null) continue;
					if (!type.is_table) continue;

					if (type.remove_from_slot(item, pos, Game1.player))
					{
						Helper.Input.Suppress(get_config().slot_take_key);
						break;
					}
				}
			}

			#endregion
        }


        /// <inheritdoc cref="IContentEvents.AssetRequested"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
		private void on_asset_requested(object? sender, AssetRequestedEventArgs e)
		{
			if (e.NameWithoutLocale.StartsWith("Data/Furniture"))
				e.Edit(FurniturePack.edit_data_furniture);

			if (e.NameWithoutLocale.StartsWith("Data/Shops"))
				e.Edit(FurniturePack.edit_data_shop);

			if (FurniturePack.try_get_type(e.Name.Name, out FurnitureType? type))
			{
				e.LoadFrom(type.get_icon_texture, AssetLoadPriority.Medium);
			}
		}

        /// <inheritdoc cref="IWorldEvents.FurnitureListChanged"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
		private void on_furniture_list_changed(object? sender, FurnitureListChangedEventArgs e)
		{
			foreach (Furniture furniture in e.Added)
			{
				if (FurniturePack.try_get_type(furniture, out FurnitureType? type))
				{
					type.on_placed(furniture);
				}
			}
			
			foreach (Furniture furniture in e.Removed)
			{
				if (FurniturePack.try_get_type(furniture, out FurnitureType? type))
				{
					type.on_removed(furniture);
				}
			}
		}
    }
}