/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/GreenhouseEntryPatch
**
*************************************************/

using Harmony; // el diavolo
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
	public interface IGenericModConfigMenuAPI
	{
		void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);
		void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);
		void RegisterLabel(IManifest mod, string labelName, string labelDesc);
	}

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

	public class AssetManager : IAssetEditor
	{
		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetName.StartsWith("Buildings")
				&& !asset.AssetName.EndsWith("_PaintMask")
				&& !asset.AssetName.EndsWith("Greenhouse")
				&& !asset.AssetName.EndsWith("houses");
		}

		public void Edit<T>(IAssetData asset)
		{
			// Force baked-in shadows for any buildings to be fully transparent if specified in config
			// Works for sprites with broad, single-colour shadows
			
			string building = Path.GetFileName(asset.AssetName);
			PropertyInfo[] properties = ModEntry.Config.GetType().GetProperties();
			PropertyInfo property = properties.FirstOrDefault(p => ModEntry.BuildingMatchesProperty(building: building, property: p.Name));
			if (property == null)
			{
				ModEntry.Instance.Monitor.Log(
					"We don't have a config option for " + building + "!"
					+ "\nLeave a post on the mod page to have one added.",
					LogLevel.Info);
			}
			if (ModEntry.Config.HideAllOtherShadows || (property != null && (bool)property.GetValue(ModEntry.Config)))
			{
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
				sprite.SetData(pixels);
				asset.ReplaceWith(sprite);
			}
		}
	}

	public class ModEntry : Mod
	{
		internal static ModEntry Instance;
		internal static Config Config;
		internal ITranslationHelper i18n => Helper.Translation;
		internal static readonly List<string> HiddenBuildings = new List<string>();

		public override void Entry(IModHelper helper)
		{
			Instance = this;
			Config = helper.ReadConfig<Config>();
			Helper.Content.AssetEditors.Add(new AssetManager());
			Helper.Events.GameLoop.GameLaunched += this.GameLoopOnGameLaunched;
		}

		private void GameLoopOnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			this.ApplyHarmonyPatches();
			this.UpdateEnabledOptions();
			this.RegisterGenericModConfigMenuPage();
		}

		private void ApplyHarmonyPatches()
		{
			HarmonyInstance harmony = HarmonyInstance.Create(Helper.ModRegistry.ModID);

			// Draw or hide shadows on select buildings
			harmony.Patch(
				original: AccessTools.Method(typeof(Building), "drawShadow"),
				prefix: new HarmonyMethod(this.GetType(), nameof(Building_DrawShadow_Prefix)));
			// Draw or hide entrance tiles on greenhouse
			harmony.Patch(
				original: AccessTools.Method(typeof(GreenhouseBuilding), "CanDrawEntranceTiles"),
				prefix: new HarmonyMethod(this.GetType(), nameof(Greenhouse_CanDrawEntranceTiles_Prefix)));
			// Draw or hide shadow on greenhouse
			harmony.Patch(
				original: AccessTools.Method(typeof(GreenhouseBuilding), "drawShadow"),
				prefix: new HarmonyMethod(this.GetType(), nameof(Greenhouse_DrawShadow_Prefix)));
			// Draw generic shadow on greenhouse
			harmony.Patch(
				original: AccessTools.Method(typeof(GreenhouseBuilding), "drawShadow"),
				prefix: new HarmonyMethod(this.GetType(), nameof(Greenhouse_DrawGenericShadow_Prefix)));
		}

		/// <summary>
		/// Update list of buildings affected by current config options to decide which have shadows drawn,
		/// and invalidate assets for all buildings to update appearances re: baked-in shadows.
		/// </summary>
		internal void UpdateEnabledOptions()
		{
			HiddenBuildings.Clear();

			// Identify affected buildings
			var blueprints = Game1.content.Load
				<Dictionary<string, string>>
				(Path.Combine("Data", "Blueprints"));
			IEnumerable<string> buildings = blueprints.Keys.Where(key => blueprints[key].Split('/')[0] != "animal");
			PropertyInfo[] properties = Config.GetType().GetProperties();
			foreach (PropertyInfo property in properties)
			{
				// Config option must be enabled
				// Config option must match building
				// Duplicate entries are ignored
				HiddenBuildings.AddRange(buildings.Where(
					building => (bool)property.GetValue(Config)
								&& !HiddenBuildings.Contains(building)
								&& BuildingMatchesProperty(building: building, property: property.Name)
				).ToList());
			}

			// Reload building sprite assets to reflect which buildings should have shadows embedded in their sprites
			foreach (string building in buildings)
			{
				Helper.Content.InvalidateCache(Path.Combine("Buildings", building));
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
			IGenericModConfigMenuAPI api = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
			if (api == null)
				return;

			api.RegisterModConfig(ModManifest,
				revertToDefault: () => Config = new Config(),
				saveToFile: () =>
				{
					// Apply changes to config
					Helper.WriteConfig(Config);

					// Reload list of buildings to affect
					this.UpdateEnabledOptions();
				});

			// Populate config with all (assumed boolean) config values
			List<string> menu = Config.GetType().GetProperties().Select(p => p.Name).ToList();

			// Add labels between options manually
			menu.Insert(4, "SpecificBuildingsOptions");
			menu.Insert(3, "OtherBuildingsOptions");
			menu.Insert(0, "GreenhouseOptions");
			foreach (string entry in menu)
			{
				string key = entry.ToLower();
				Translation name, description;
				PropertyInfo property = Config.GetType().GetProperty(entry);
				if (property != null)
				{
					// Real properties
					name = i18n.Get("config." + key + ".name");
					description = i18n.Get("config." + key + ".description");
					api.RegisterSimpleOption(ModManifest,
						optionName: name.HasValue() ? name : property.Name,
						optionDesc: description.HasValue() ? description : null,
						optionGet: () => (bool)property.GetValue(Config),
						optionSet: (bool value) => property.SetValue(Config, value));
				}
				else
				{
					// Labels
					name = i18n.Get("config." + key + ".label");
					api.RegisterLabel(ModManifest,
						labelName: name,
						labelDesc: null);
				}
			}
		}

		public static bool Building_DrawShadow_Prefix(Building __instance)
		{
			// Draw shadow (return true) if not hiding every building's shadow and not hiding this building's shadow
			return !Config.HideAllOtherShadows && !HiddenBuildings.Contains(__instance.buildingType.Value);
		}

		public static bool Greenhouse_CanDrawEntranceTiles_Prefix()
		{
			return !Config.HideGreenhouseTiles;
		}

		public static bool Greenhouse_DrawShadow_Prefix()
		{
			return !Config.HideGreenhouseShadow;
		}

		public static bool Greenhouse_DrawGenericShadow_Prefix(GreenhouseBuilding __instance, SpriteBatch b, int localX = -1, int localY = -1)
		{
			if (!Config.HideGreenhouseShadow && Config.GreenhouseSoftShadow)
			{
				const int entryTilesWide = 3;
				float alpha = Instance.Helper.Reflection.GetField<NetFloat>(__instance, "alpha").GetValue();
				Vector2 basePosition = (localX == -1)
					? Game1.GlobalToLocal(new Vector2(__instance.tileX.Value * Game1.tileSize, (__instance.tileY.Value + __instance.tilesHigh.Value) * Game1.tileSize))
					: new Vector2(localX, localY + (__instance.getSourceRectForMenu().Height * 4));
				Vector2 topPosition = Game1.GlobalToLocal(new Vector2(__instance.tileX.Value * Game1.tileSize, __instance.tileY.Value * Game1.tileSize));
				Color colour = Color.White * ((localX == -1) ? alpha : 1f);

				// Draw shadow underneath greenhouse (visible at the sides of the vanilla sprite and may be visible in custom sprites)
				// '1E-05f' layerDepth value is an artefact from decompiled game code
				b.Draw(
					texture: Game1.mouseCursors,
					destinationRectangle: new Rectangle(
						(int)topPosition.X, (int)topPosition.Y,
						__instance.tilesWide.Value * Game1.tileSize, __instance.tilesHigh.Value * Game1.tileSize),
					sourceRectangle: new Rectangle(Building.leftShadow.X, Building.leftShadow.Y, 1, 1),
					color: colour,
					rotation: 0f, origin: Vector2.Zero, SpriteEffects.None, layerDepth: 1E-05f);
				// Shadow start
				b.Draw(
					texture: Game1.mouseCursors,
					position: basePosition,
					sourceRectangle: Building.leftShadow,
					color: colour,
					rotation: 0f, origin: Vector2.Zero, scale: Game1.pixelZoom, SpriteEffects.None, layerDepth: 1E-05f);
				for (int x = 1; x < __instance.tilesWide.Value - 1; x++)
				{
					// Avoid drawing over entry tiles if enabled
					if (!Config.HideGreenhouseTiles
						&& x > (__instance.tilesWide.Value - entryTilesWide) / 2
						&& x < __instance.tilesWide.Value - ((__instance.tilesWide.Value - entryTilesWide) / 2))
						continue;
					// Shadow middle
					b.Draw(
						texture: Game1.mouseCursors,
						position: basePosition + new Vector2(x * 64, 0f),
						sourceRectangle: Building.middleShadow,
						color: colour,
					rotation: 0f, origin: Vector2.Zero, scale: Game1.pixelZoom, SpriteEffects.None, layerDepth: 1E-05f);
				}
				// Shadow end
				b.Draw(
					texture: Game1.mouseCursors,
					position: basePosition + new Vector2((__instance.tilesWide.Value - 1) * 64, 0f),
					sourceRectangle: Building.rightShadow,
					color: colour,
					rotation: 0f, origin: Vector2.Zero, scale: Game1.pixelZoom, SpriteEffects.None, layerDepth: 1E-05f);
				
				return false;
			}

			return true;
		}
	}
}
