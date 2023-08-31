/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/GreenhouseEntryPatch
**
*************************************************/

using HarmonyLib; // el diavolo nuevo
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace GreenhouseEntryPatch
{
	public class Config
	{
		public bool HideGreenhouseTiles { get; set; } = true;
		public bool HideGreenhouseShadow { get; set; } = false;
		public bool GreenhouseSoftShadow { get; set; } = false;
		public bool HideAllOtherShadows { get; set; } = false;
		public bool HideBarnShadow { get; set; } = false;
		public bool HideCoopShadow { get; set; } = false;
		public bool HideShedShadow { get; set; } = false;
		public bool HideWellShadow { get; set; } = false;
		public bool HideMillShadow { get; set; } = false;
		public bool HideSiloShadow { get; set; } = false;
		public bool HideStableShadow { get; set; } = false;
		public bool HideFishPondShadow { get; set; } = false;
		public bool HideShippingBinShadow { get; set; } = false;
		public bool HideSlimeHutchShadow { get; set; } = false;
		public bool HideCabinShadow { get; set; } = false;
		public bool HideObeliskShadow { get; set; } = false;
		public bool HideGoldClockShadow { get; set; } = false;
		public bool HideJunimoHutShadow { get; set; } = false;
	}

	public static class AssetManager
	{
		public static void OnAssetRequested(object sender, AssetRequestedEventArgs e)
		{
			string name = e.NameWithoutLocale.ToString();
			if (name.StartsWith("Buildings")
				&& !name.EndsWith("_PaintMask")
				&& !name.EndsWith("houses")
				&& (!name.EndsWith("Greenhouse") || Constants.TargetPlatform is GamePlatform.Android))
			{
				e.Edit(apply: AssetManager.Edit);
			}
		}

		private static void Edit(IAssetData asset)
		{
			// Force baked-in shadows for any buildings to be fully transparent if specified in config
			// Works for sprites with broad, single-colour shadows

			string building = Path.GetFileName(path: asset.NameWithoutLocale.ToString());
			PropertyInfo[] properties = ModEntry.Config.GetType().GetProperties();
			PropertyInfo property = properties.FirstOrDefault((PropertyInfo p) => ModEntry.BuildingMatchesProperty(building: building, property: p.Name));

			// Show a notice if a building doesn't have a config entry
			if (property is null)
			{
				ModEntry.Instance.Monitor.Log(
					"We don't have a config option for " + building + "!"
					+ "\nLeave a post on the mod page to have one added.",
					LogLevel.Info);
			}

			// Edit sprite asset
			if (ModEntry.Config.HideAllOtherShadows || ((bool)property?.GetValue(ModEntry.Config)))
			{
				// Read sprite pixels
				Texture2D sprite = asset.AsImage().Data;
				Color[] pixels = new Color[sprite.Width * sprite.Height];
				sprite.GetData(pixels);

				// Many sprites have the bottom-left corner match with the shadow drawn later in-game, so we can use this as a shortcut
				// Otherwise we have to search for a transparent pixel, which may not be the one we want, eg. baked-in light glow, windows, ..
				Color cornerColour = pixels[(sprite.Height - 1) * sprite.Width];
				Color shadowColour = cornerColour.A > 0 && cornerColour.A < 100
					? cornerColour
					: pixels.LastOrDefault(colour => colour.A > 0 && colour.A < 100);
				for (int i = 0; i < pixels.Length; ++i)
				{
					if (pixels[i] == shadowColour)
					{
						pixels[i].A = 0;
					}
				}

				// Replace sprite pixels
				sprite.SetData(pixels);
				asset.ReplaceWith(sprite);
			}
		}
	}

	public class ModEntry : Mod
	{
		internal static ModEntry Instance;
		internal static Config Config;
		internal ITranslationHelper I18n => this.Helper.Translation;
		internal static readonly List<string> HiddenBuildings = new();

		public override void Entry(IModHelper helper)
		{
			ModEntry.Instance = this;
			ModEntry.Config = helper.ReadConfig<Config>();

			this.Helper.Events.Content.AssetRequested += AssetManager.OnAssetRequested;
			this.Helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
		}

		private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			this.ApplyHarmonyPatches();
			this.UpdateEnabledOptions();
			this.RegisterGenericModConfigMenuPage();
		}

		private void ApplyHarmonyPatches()
		{
			Harmony harmony = new(id: this.Helper.ModRegistry.ModID);

			// Draw or hide shadows on select buildings
			harmony.Patch(
				original: AccessTools.Method(typeof(Building), "drawShadow"),
				prefix: new HarmonyMethod(this.GetType(), nameof(ModEntry.Building_DrawShadow_Prefix)));

			if (Constants.TargetPlatform is GamePlatform.Android)
			{
				ModEntry.Instance.Monitor.Log(
					"Looks like you're playing on Android!"
					+ "\nThe greenhouse entry tiles aren't hideable in the same way for Android, and you'll need a Farm map edit to hide them."
					+ "\nHiding building shadows still works as normal!",
					LogLevel.Info);
			}
			else
			{
				// Draw or hide entrance tiles on greenhouse
				harmony.Patch(
					original: AccessTools.Method(typeof(GreenhouseBuilding), "CanDrawEntranceTiles"),
					prefix: new HarmonyMethod(this.GetType(), nameof(ModEntry.Greenhouse_CanDrawEntranceTiles_Prefix)));
				// Draw or hide shadow on greenhouse
				harmony.Patch(
					original: AccessTools.Method(typeof(GreenhouseBuilding), "drawShadow"),
					prefix: new HarmonyMethod(this.GetType(), nameof(ModEntry.Greenhouse_DrawShadow_Prefix)));
			}
		}

		/// <summary>
		/// Update list of buildings affected by current config options to decide which have shadows drawn,
		/// and invalidate assets for all buildings to update appearances re: baked-in shadows.
		/// </summary>
		internal void UpdateEnabledOptions()
		{
			ModEntry.HiddenBuildings.Clear();

			// Identify affected buildings
			var blueprints = this.Helper.GameContent.Load
				<Dictionary<string, string>>
				(Path.Combine("Data", "Blueprints"));
			IEnumerable<string> buildings = blueprints.Keys.Where((string key) => blueprints[key].Split('/')[0] != "animal");
			PropertyInfo[] properties = ModEntry.Config.GetType().GetProperties();
			foreach (PropertyInfo property in properties)
			{
				// Config option must be enabled
				// Config option must match building
				// Duplicate entries are ignored
				ModEntry.HiddenBuildings.AddRange(buildings.Where((string building) =>
					(bool)property.GetValue(ModEntry.Config)
						&& !ModEntry.HiddenBuildings.Contains(building)
						&& ModEntry.BuildingMatchesProperty(building: building, property: property.Name))
					.ToList());
			}

			// Reload building sprite assets to reflect which buildings should have shadows embedded in their sprites
			foreach (string building in buildings)
			{
				this.Helper.GameContent.InvalidateCache(Path.Combine("Buildings", building));
			}
		}

		/// <summary>
		/// Checks for config options for building types
		/// Matches type (Deluxe Coop, Coop => HideCoopShadow) or building (Junimo Hut => HideJunimoHutShadow)
		/// Loose pattern matching may cause problems with custom buildings with roughly similar names to vanilla buildings,
		/// but for now nobody is making any custom buildings
		/// </summary>
		internal static bool BuildingMatchesProperty(string building, string property)
		{
			property = property.Replace("Shadow", "");
			return property.EndsWith(building.Split(' ').Last()) || property.EndsWith(building.Replace(" ", ""));
		}

		private void RegisterGenericModConfigMenuPage()
		{
			IGenericModConfigMenuAPI api = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
			if (api is null)
				return;

			api.Register(
				mod: this.ModManifest,
				reset: () => ModEntry.Config = new Config(),
				save: () =>
				{
					// Apply changes to config
					this.Helper.WriteConfig(ModEntry.Config);

					// Reload list of buildings to affect
					this.UpdateEnabledOptions();
				});

			// Populate config with all (assumed boolean) config values
			List<string> menu = ModEntry.Config.GetType().GetProperties().Select((PropertyInfo p) => p.Name).ToList();

			// Add labels between options manually
			menu.Insert(4, "SpecificBuildingsOptions");
			menu.Insert(3, "OtherBuildingsOptions");
			menu.Insert(0, "GreenhouseOptions");
			foreach (string entry in menu)
			{
				string key = entry.ToLower();
				Translation name, description;
				PropertyInfo property = ModEntry.Config.GetType().GetProperty(entry);
				if (property is not null)
				{
					// Real properties
					name = this.I18n.Get("config." + key + ".name");
					description = this.I18n.Get("config." + key + ".description");
					api.AddBoolOption(
						mod: this.ModManifest,
						name: () => name.HasValue() ? name : property.Name,
						tooltip: () => description.HasValue() ? description : null,
						getValue: () => (bool)property.GetValue(ModEntry.Config),
						setValue: (bool value) => property.SetValue(ModEntry.Config, value));
				}
				else
				{
					// Labels
					name = this.I18n.Get("config." + key + ".label");
					api.AddSectionTitle(
						mod: this.ModManifest,
						text: () => name);
				}
			}
		}

		public static bool Building_DrawShadow_Prefix(Building __instance)
		{
			// Draw shadow (return true) if not hiding every building's shadow and not hiding this building's shadow
			return !ModEntry.Config.HideAllOtherShadows && !ModEntry.HiddenBuildings.Contains(__instance.buildingType.Value);
		}

		public static bool Greenhouse_CanDrawEntranceTiles_Prefix()
		{
			return !ModEntry.Config.HideGreenhouseTiles;
		}

		public static bool Greenhouse_DrawShadow_Prefix(GreenhouseBuilding __instance, SpriteBatch b, int localX = -1, int localY = -1)
		{
			// Hide greenhouse shadow
			if (ModEntry.Config.HideGreenhouseShadow)
			{
				return false;
			}

			// Draw greenhouse shadow with the behaviours of a standard farm building shadow
			if (ModEntry.Config.GreenhouseSoftShadow)
			{
				bool isGhost = localX == -1;
				Vector2 tile = new(x: __instance.tileX.Value, y: __instance.tileY.Value);
				Vector2 size = new(x: __instance.tilesWide.Value, y: __instance.tilesHigh.Value);
				Rectangle source = __instance.getSourceRectForMenu();
				Vector2 top = Game1.GlobalToLocal(globalPosition: tile * Game1.tileSize);
				Vector2 bottom = isGhost
					? new(
						x: top.X,
						y: top.Y + size.Y * Game1.tileSize)
					: new(
						x: localX,
						y: localY + (source.Height * Game1.pixelZoom));
				Color colour = Color.White * (isGhost ? ModEntry.Instance.Helper.Reflection.GetField<NetFloat>(__instance, "alpha").GetValue() : 1);
				const float layerDepth = 1E-05f;

				// Draw shadow underneath greenhouse (visible at the sides of the vanilla sprite and may be visible in custom sprites)
				b.Draw(
					texture: Game1.mouseCursors,
					destinationRectangle: new Rectangle(
						x: (int)top.X,
						y: (int)top.Y,
						width: (int)size.X * Game1.tileSize,
						height: (int)size.Y * Game1.tileSize),
					sourceRectangle: new Rectangle(
						x: Building.leftShadow.X,
						y: Building.leftShadow.Y,
						width: 1,
						height: 1),
					color: colour,
					rotation: 0,
					origin: Vector2.Zero,
					effects: SpriteEffects.None,
					layerDepth: layerDepth);

				// Draw shadow in front of greenhouse
				void draw(Rectangle shadow, float x)
				{
					b.Draw(
						texture: Game1.mouseCursors,
						position: bottom + new Vector2(x: x, y: 0) * Game1.tileSize,
						sourceRectangle: shadow,
						color: colour,
						rotation: 0,
						origin: Vector2.Zero,
						scale: Game1.pixelZoom,
						effects: SpriteEffects.None,
						layerDepth: layerDepth);
				}

				draw(shadow: Building.leftShadow, x: 0);
				for (int x = 1; x < size.X - 1; x++)
				{
					// Avoid drawing over entry tiles if enabled
					const int entryTilesWide = 3;
					float entry = (size.X - entryTilesWide) / 2;
					if (!ModEntry.Config.HideGreenhouseTiles && x > entry && x < size.X - entry)
						continue;

					draw(shadow: Building.middleShadow, x: x);
				}
				draw(shadow: Building.rightShadow, x: size.X - 1);
				
				// Hide default greenhouse shadow
				return false;
			}

			// Draw default greenhouse shadow
			return true;
		}
	}
}
