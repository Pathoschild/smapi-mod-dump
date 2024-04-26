/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Leroymilo/MZG
**
*************************************************/

using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Objects;
using StardewValley.Buildings;
using StardewValley;

namespace ModularZenGarden
{

    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
		private ModConfig? config;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
			config = Helper.ReadConfig<ModConfig>();

			Utils.monitor = Monitor;
			Utils.helper = helper;
			Utils.config = config;
			helper.Events.Content.AssetRequested += on_asset_requested;
			helper.Events.World.FurnitureListChanged += on_furniture_list_changed;
			helper.Events.World.BuildingListChanged += on_building_list_changed;
			helper.Events.Player.Warped += on_player_warped;
			helper.Events.GameLoop.SaveLoaded += on_save_loaded;
			helper.Events.GameLoop.GameLaunched += on_game_launched;

			var harmony = new Harmony(ModManifest.UniqueID);
			patch_furniture_draw(harmony);
			patch_furniture_checkForAction(harmony);

			load_assets(helper);
        }

		private void patch_furniture_draw(Harmony harmony)
		{
			// fetching Furniture.draw MethodInfo
			MethodInfo? original_method = typeof(Furniture)
				.GetMethods()
				.Where(x => x.Name == "draw")
				.Where(x => x.DeclaringType != null
					&& x.DeclaringType.Name == "Furniture")
				.FirstOrDefault();

			harmony.Patch(
				original: original_method,
				prefix: new HarmonyMethod(
					typeof(FurniturePatches),
					nameof(FurniturePatches.draw_prefix)
				)
			);
		}

		private void patch_furniture_checkForAction(Harmony harmony)
		{
			// fetching Furniture.checkForAction MethodInfo
			MethodInfo? original_method = typeof(Furniture)
				.GetMethods()
				.Where(x => x.Name == "checkForAction")
				.Where(x => x.DeclaringType != null
					&& x.DeclaringType.Name == "Furniture")
				.FirstOrDefault();
			
			harmony.Patch(
				original: original_method,
				prefix: new HarmonyMethod(
					typeof(FurniturePatches),
					nameof(FurniturePatches.checkForAction_prefix)
				)
			);
		}

		private void load_assets(IModHelper helper)
		{
			SpriteManager.load_sprites(helper);
			GardenType.load_types(helper);
		}


		/// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void on_game_launched(object? sender, GameLaunchedEventArgs e)
		{
			if (config == null) throw new NullReferenceException("Config was not set.");

			if (Helper.ModRegistry.IsLoaded("leroymilo.USJB"))
			{
				// compatibility with USJB
				Utils.USJB_installed = true;
			}

			if (!config.ignore_SJB_integration)
			{
				config.change_buildings = Utils.USJB_installed;
				Helper.WriteConfig(config);
			}

			// get Generic Mod Config Menu's API (if it's installed)
			var configMenu = Helper.ModRegistry.GetApi
				<GenericModConfigMenu.IGenericModConfigMenuApi>
				("spacechase0.GenericModConfigMenu");
			if (configMenu is null)
				return;
			
			// register mod
			configMenu.Register(
				mod: ModManifest,
				reset: () => config = new ModConfig(),
				save: () => Helper.WriteConfig(config)
			);

			string tooltip = "This will change some Farm Buildings into Modular Zen Garden.";
			tooltip += "\nIf Seasonal Japanese Buildings (SJB) is installed, this is on by default.";

			// add some config options
			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Change Buildings",
				tooltip: () => tooltip,
				getValue: () => config.change_buildings,
				setValue: value => {
					config.change_buildings = value;
					// Utils.invalidate_cache("Data/Buildings");
					reset_building_textures();
				}
			);
			
			tooltip = "This will prevent the previous option from being automatically changed.";

			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => "Ignore SJB Integration",
				tooltip: () => tooltip,
				getValue: () => config.ignore_SJB_integration,
				setValue: value => config.ignore_SJB_integration = value
			);
		}

		/// <inheritdoc cref="IContentEvents.AssetRequested"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void on_asset_requested(object? sender, AssetRequestedEventArgs e)
		{
			if (config == null) return;
			
			if (e.NameWithoutLocale.IsEquivalentTo("Data/Furniture"))
			{
				// Adding all Zen Garden Furniture
				e.Edit(GardenType.patch_furniture_data);
				return;
			}

			if (e.NameWithoutLocale.IsEquivalentTo("Data/Shops"))
			{
				// Adding the Zen Garden catalogue
				e.Edit(GardenType.patch_shop_data);
				return;
			}

			if (e.NameWithoutLocale.IsEquivalentTo("Data/Buildings") && config.change_buildings)
			{
				// Adding Sprites to obelisks
				e.Edit(GardenType.patch_building_data);
				return;
			}
			
			string type_name = e.NameWithoutLocale.ToString() ?? throw new Exception("should not happen");

			if (!type_name.EndsWith("_PaintMask"))
			{
				if (type_name.StartsWith("MZG_B"))
				{
					if (!config.change_buildings) return;
					// Patches a skin texture for buildings
					GardenType type = GardenType.get_type(type_name);

					e.LoadFrom(type.get_base, AssetLoadPriority.Medium);
					e.Edit(asset => type.patch_building(asset, type_name));
					return;
				}

				if (type_name.StartsWith("MZG"))
				{
					// Use cached textures to replace loading of Furniture textures
					GardenType type = GardenType.get_type(type_name);

					e.LoadFrom(type.get_base, AssetLoadPriority.Medium);
					e.Edit(type.patch_default_texture);
					return;
				}
			}
		}

        /// <inheritdoc cref="IWorldEvents.FurnitureListChanged"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
		private void on_furniture_list_changed(object? sender, FurnitureListChangedEventArgs e)
		{
			if (!e.IsCurrentLocation) return;	// ignoring furniture placed outside current location

			foreach (Furniture furniture in e.Added)
			{
				if (!GardenType.is_garden(furniture)) continue;
				GardenCache.add_garden(furniture);
			}
			
			foreach (Furniture furniture in e.Removed)
			{
				if (!GardenType.is_garden(furniture)) continue;
				GardenCache.remove_garden(furniture);
			}

			GardenCache.update_textures();
		}

        /// <inheritdoc cref="IWorldEvents.BuildingListChanged"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
		private void on_building_list_changed(object? sender, BuildingListChangedEventArgs e)
		{
			// This is probably useless since you edit buildings from shops,
			// but I leave it just in case for mods allowing to edit buildings like furniture

			if (!e.IsCurrentLocation) return;	// ignoring buildings placed outside current location
			if (config == null || !config.change_buildings) return;
			
			bool buildings_updated = false;

			foreach (Building building in e.Added)
			{
				if (!GardenType.is_garden(building)) continue;
				Garden garden = GardenCache.add_garden(building);
				buildings_updated = true;
				building.skinId.Value = garden.get_skin_id();
			}
			
			foreach (Building building in e.Removed)
			{
				if (!GardenType.is_garden(building)) continue;
				GardenCache.remove_garden(building);
				buildings_updated = true;
			}

			if (buildings_updated)
			{
				// Invalidate Data/Buildings to add sprites
				Utils.invalidate_cache("Data/Buildings");
			}

			GardenCache.update_textures();
		}

		/// <inheritdoc cref="IPlayerEvents.Warped"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
		private void on_player_warped(object? sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer) return;	// for forward compatibility
			load_gardens_in_location(e.NewLocation);
		}

		/// <inheritdoc cref="IGameLoopEvents.SaveLoaded"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
		private void on_save_loaded(object? sender, SaveLoadedEventArgs e)
		{
			load_gardens_in_location(Game1.currentLocation);
		}

		private void load_gardens_in_location(GameLocation location)
		{
			GardenCache.clear();

			foreach (Furniture furniture in location.furniture) {
				if (!GardenType.is_garden(furniture)) continue;
				GardenCache.add_garden(furniture);
			}

			List<Building> updated_buildings = new();

			if (config != null && config.change_buildings)
			{
				foreach (Building building in location.buildings) {
					if (!GardenType.is_garden(building)) continue;
					Garden garden = GardenCache.add_garden(building);
					updated_buildings.Add(building);
					building.skinId.Set(garden.get_skin_id());
				}

				if (updated_buildings.Count > 0)
				{
					// Invalidate Data/Buildings to add sprites
					Utils.invalidate_cache("Data/Buildings");
				}
			}
			
			GardenCache.update_textures();

			foreach (Building building in updated_buildings)
			{
				building.resetTexture();
			}
		}

		private void reset_building_textures()
		{
			if (config == null) throw new NullReferenceException();
			if (!Context.IsWorldReady) return;

			List<Building> updated_buildings = new();

			GameLocation location = Game1.currentLocation;
			foreach (Building building in location.buildings) {
				if (!GardenType.is_garden(building)) continue;

				Garden? garden;

				if (config.change_buildings)
				{
					garden = GardenCache.add_garden(building);
					building.skinId.Set(garden.get_skin_id());
				}
				else
				{
					garden = GardenCache.get_garden(building);
					if (garden == null) continue;
					GardenCache.remove_garden(building);
					building.skinId.Set(null);
				}

				updated_buildings.Add(building);
			}
			
			Utils.invalidate_cache("Data/Buildings");
			GardenCache.update_textures();
			
			foreach (Building building in updated_buildings)
			{
				building.resetTexture();
			}
		}
    }
	
}