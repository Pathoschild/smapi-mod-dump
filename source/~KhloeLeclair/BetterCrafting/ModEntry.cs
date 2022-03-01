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
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using HarmonyLib;

using Microsoft.Xna.Framework.Graphics;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.Integrations.GenericModConfigMenu;
using Leclair.Stardew.Common.Inventory;
using Leclair.Stardew.Common.Types;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

using Leclair.Stardew.BetterCrafting.Managers;

namespace Leclair.Stardew.BetterCrafting {

	public class ModEntry : ModSubscriber {

		public static ModEntry instance;
		public static IBetterCrafting API;

		internal Harmony Harmony;

		private readonly PerScreen<IClickableMenu> CurrentMenu = new();
		private readonly PerScreen<Menus.BetterCraftingPage> OldCraftingPage = new();
		private readonly PerScreen<bool> OldCraftingGameMenu = new();

		private bool? hasBiggerBackpacks;
		private bool? hasLoveOfCooking;

		public bool AtTitle = false;

		public ModConfig Config;
		private bool ConfigRegistered = false;

		public RecipeManager Recipes;
		public FavoriteManager Favorites;

		private Hashtable invProviders = new Hashtable();
		private readonly object providerLock = new();

		private CaseInsensitiveHashSet ConnectorExamples;
		private Dictionary<int, string> FloorMap;

		private GMCMIntegration<ModConfig, ModEntry> GMCMIntegration;

		internal Integrations.RaisedGardenBeds.RGBIntegration intRGB;
		internal Integrations.StackSplitRedux.SSRIntegration intSSR;
		internal Integrations.CookingSkill.CSIntegration intCSkill;
		internal Integrations.SpaceCore.SCIntegration intSCore;
		internal Integrations.CustomCraftingStation.CCSIntegration intCCStation;

		public Texture2D ButtonTexture;

		public override void Entry(IModHelper helper) {
			base.Entry(helper);

			instance = this;
			API = new ModAPI(this);

			// Harmony
			Harmony = new Harmony(ModManifest.UniqueID);

			// Read Config
			Config = Helper.ReadConfig<ModConfig>();

			// Init
			I18n.Init(Helper.Translation);

			RegisterProviders();

			Recipes = new RecipeManager(this);
			Favorites = new FavoriteManager(this);

			Sprites.Load(Helper.Content);
		}

		public override object GetApi() {
			return API;
		}


		#region Events

		[Subscriber]
		private void OnMenuChanged(object sender, MenuChangedEventArgs e) {
			IClickableMenu menu = Game1.activeClickableMenu;
			if (CurrentMenu.Value == menu)
				return;

			// Are we doing GMCM stuff?
			if (OldCraftingPage.Value != null) {
				if (menu != null) {
					string name = menu.GetType().FullName;

					// If we're on the specific page for a mod, then
					// everything is fine and we can continue.
					if (name.Equals("GenericModConfigMenu.Framework.SpecificModConfigMenu"))
						return;

					if (name.Equals("GenericModConfigMenu.Framework.ModConfigMenu")) {
						CommonHelper.YeetMenu(menu);

						menu = Game1.activeClickableMenu = Menus.BetterCraftingPage.Open(
							mod: this,
							location: OldCraftingPage.Value.Location,
							position: OldCraftingPage.Value.Position,
							area: OldCraftingPage.Value.Area,
							cooking: OldCraftingPage.Value.cooking,
							standalone_menu: !OldCraftingGameMenu.Value,
							material_containers: OldCraftingPage.Value.MaterialContainers,
							silent_open: true,
							discover_containers: OldCraftingPage.Value.DiscoverContainers,
							listed_recipes: OldCraftingPage.Value.GetListedRecipes()
						);

						if (OldCraftingGameMenu.Value) {
							var bcm = menu;
							var game = new GameMenu(false);
							menu = Game1.activeClickableMenu = game;

							for(int i = 0; i < game.pages.Count; i++) {
								if (game.pages[i] is CraftingPage cp) {
									CommonHelper.YeetMenu(cp);

									game.pages[i] = bcm;
									game.changeTab(i, false);
									break;
								}
							}
						}
					}
				}

				// Clear the old crafting page.
				OldCraftingPage.Value = null;
				OldCraftingGameMenu.Value = false;
			}

			// No menu?
			if (menu == null) {
				CurrentMenu.Value = null;
				return;
			}

			// Replace crafting pages.
			if (Config.SuppressBC?.IsDown() ?? false) { 
				CurrentMenu.Value = menu;
				return;
			}

			if (menu is CraftingPage page) {
				bool cooking = CraftingPageHelper.IsCooking(page);
				if (cooking ? Config.ReplaceCooking : Config.ReplaceCrafting) {

					// Make a copy of the existing chests, in case yeeting
					// the menu creates an issue.
					List<object> chests = new(page._materialContainers);

					// Find our bench
					var where = page.GetBenchPosition(Game1.player);
					var area = page.GetBenchRegion(Game1.player);

					// Get recipes
					var recipes = cooking ?
						intCCStation.GetCookingRecipes() :
						intCCStation.GetCraftingRecipes();

					// Make sure to clean up the existing menu.
					CommonHelper.YeetMenu(page);

					// And now create our own.
					menu = Game1.activeClickableMenu = Menus.BetterCraftingPage.Open(
						mod: this,
						location: Game1.player.currentLocation,
						position: where,
						area: area,
						cooking: cooking,
						standalone_menu: true,
						material_containers: chests,
						listed_recipes: recipes
					);
				}
			}

			if (intCCStation.IsLoaded && menu.GetType().FullName.Equals("StardewValley.Menus.CustomCraftingMenu")) {
				// CustomCraftingStation Menu?

				// See which recipes it's using. If it's not mixed, then
				// replace it with our menu.

				var recipes = Helper.Reflection.GetField<List<Dictionary<ClickableTextureComponent, CraftingRecipe>>>(menu, "pagesOfCraftingRecipes", false).GetValue();
				if (recipes != null) {
					List<string> names = new();
					int crafting = 0;
					int cooking = 0;

					foreach (var rpage in recipes) {
						foreach (var recipe in rpage.Values) {
							if (recipe.isCookingRecipe)
								cooking++;
							else
								crafting++;

							names.Add(recipe.name);
						}
					}

					if (crafting == 0 || cooking == 0 && names.Count > 0) {
						bool is_cooking = cooking > 0;

						var chests = Helper.Reflection.GetField<List<Chest>>(menu, "_materialContainers", false).GetValue();
						List<object> containers = chests == null ? null : new(chests);

						// TODO: Find the bench

						CommonHelper.YeetMenu(menu);

						menu = Game1.activeClickableMenu = Menus.BetterCraftingPage.Open(
							mod: this,
							location: Game1.player.currentLocation,
							cooking: is_cooking,
							standalone_menu: true,
							material_containers: containers,
							listed_recipes: names
						);
					}
				}
			}

			// Replace crafting pages in the menu.
			if (menu is GameMenu gm && Config.ReplaceCrafting) {
				for (int i = 0; i < gm.pages.Count; i++) {
					if (gm.pages[i] is CraftingPage cp) {
						CommonHelper.YeetMenu(cp);

						gm.pages[i] = Menus.BetterCraftingPage.Open(
							mod: this,
							location: Game1.player.currentLocation,
							position: null,
							width: gm.width,
							height: gm.height,
							cooking: false,
							standalone_menu: false,
							material_containers: (IList<LocatedInventory>) null,
							listed_recipes: intCCStation.GetCraftingRecipes(),
							x: gm.xPositionOnScreen,
							y: gm.yPositionOnScreen
						);
					}
				}
			}

			CurrentMenu.Value = menu;
		}

		[Subscriber]
		private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
			// Load Data
			LoadFloorMap();
			LoadConnectorExamples();

			// More Init
			AtTitle = true;
			RegisterSettings();

			// Integrations
			intRGB = new(this);
			intSSR = new(this);
			intCSkill = new(this);
			intSCore = new(this);
			intCCStation = new(this);

			// Load Data
			Recipes.LoadRecipes();
			Recipes.LoadDefaults();
		}

		[Subscriber]
		private void OnSaveLoaded(object sender, SaveLoadedEventArgs e) {
			AtTitle = false;
			RegisterSettings();
		}

		[Subscriber]
		private void OnReturnToTitle(object sender, ReturnedToTitleEventArgs e) {
			AtTitle = true;
			RegisterSettings();
		}

		#endregion

		#region Configuration

		public void SaveConfig() {
			Helper.WriteConfig(Config);
		}

		public bool HasGMCM() {
			return GMCMIntegration?.IsLoaded ?? false;
		}

		public void OpenGMCM() {
			if (HasGMCM()) {
				if (Game1.activeClickableMenu is GameMenu gm && gm.GetCurrentPage() is Menus.BetterCraftingPage p) {
					OldCraftingPage.Value = p;
					OldCraftingGameMenu.Value = true;
				} 

				if (Game1.activeClickableMenu is Menus.BetterCraftingPage page)
					OldCraftingPage.Value = page;

				GMCMIntegration.OpenMenu();
			}
		}


		private void RegisterSettings() {
			GMCMIntegration = new(this, () => Config, () => Config = new ModConfig(), () => SaveConfig());
			if (!GMCMIntegration.IsLoaded)
				return;

			if (ConfigRegistered)
				GMCMIntegration.Unregister();

			ConfigRegistered = true;

			Dictionary<Models.SeasoningMode, Func<string>> seasoning = new();
			seasoning.Add(Models.SeasoningMode.Disabled, I18n.Seasoning_Disabled);
			seasoning.Add(Models.SeasoningMode.Enabled, I18n.Seasoning_Enabled);
			seasoning.Add(Models.SeasoningMode.InventoryOnly, I18n.Seasoning_Inventory);

			GMCMIntegration.Register(true);

			if (!AtTitle)
				GMCMIntegration.Add(
					I18n.Setting_ShowAdv,
					I18n.Setting_ShowAdv_Tip,
					c => Game1.options.showAdvancedCraftingInformation,
					(c, v) => Game1.options.changeCheckBoxOption(Options.toggleShowAdvancedCraftingInformation, v)
				);

			GMCMIntegration
				.AddLabel(I18n.Setting_General)
				.Add(
					I18n.Setting_Settings,
					I18n.Setting_Settings_Tip,
					c => c.ShowSettingsButton,
					(c, v) => c.ShowSettingsButton = v
				)
				.Add(
					I18n.Setting_Suppress,
					I18n.Setting_Suppress_Tip,
					c => c.SuppressBC,
					(c, v) => c.SuppressBC = v
				)
				.Add(
					I18n.Setting_ReplaceCrafting,
					I18n.Setting_ReplaceCrafting_Tip,
					c => c.ReplaceCrafting,
					(c, val) => c.ReplaceCrafting = val
				)
				.Add(
					I18n.Setting_ReplaceCooking,
					I18n.Setting_ReplaceCooking_Tip,
					c => c.ReplaceCooking,
					(c, val) => c.ReplaceCooking = val
				)
				.Add(
					I18n.Setting_EnableCategories,
					I18n.Setting_EnableCategories_Tip,
					c => c.UseCategories,
					(c, val) => c.UseCategories = val
				);

			GMCMIntegration
				.AddLabel(I18n.Setting_Crafting, I18n.Setting_Crafting_Tip)
				.Add(
					I18n.Setting_UniformGrid,
					I18n.Setting_UniformGrid_Tip,
					c => c.UseUniformGrid,
					(c, val) => c.UseUniformGrid = val
				)
				.Add(
					I18n.Setting_BigCraftablesLast,
					I18n.Setting_BigCraftablesLast_Tip,
					c => c.SortBigLast,
					(c, val) => c.SortBigLast = val
				);

			GMCMIntegration
				.AddLabel(I18n.Setting_Cooking, I18n.Setting_Cooking_Tip)
				.AddChoice(
					I18n.Setting_Seasoning,
					I18n.Setting_Seasoning_Tip,
					c => c.UseSeasoning,
					(c, val) => c.UseSeasoning = val,
					choices: seasoning
				)
				.Add(
					I18n.Setting_HideUnknown,
					I18n.Setting_HideUnknown_Tip,
					c => c.HideUnknown,
					(c, val) => c.HideUnknown = val
				);

			GMCMIntegration
				.AddLabel(I18n.Setting_Quality)
				.AddParagraph(I18n.Setting_Quality_Tip)
				.Add(
					I18n.Setting_EnableQuality,
					I18n.Setting_EnableQuality_Tip,
					c => c.MaxQuality != MaxQuality.Disabled,
					(c, v) => {
						if (v && c.MaxQuality == MaxQuality.Disabled)
							c.MaxQuality = MaxQuality.Iridium;
						else if (!v && c.MaxQuality != MaxQuality.Disabled)
							c.MaxQuality = MaxQuality.Disabled;
					}
				)
				.Add(
					I18n.Setting_SortQuality,
					I18n.Setting_SortQuality_Tip,
					c => c.LowQualityFirst,
					(c, v) => c.LowQualityFirst = v
				);

			GMCMIntegration
				.AddLabel(I18n.Setting_Nearby)
				.AddParagraph(I18n.Setting_Nearby_Tip)
				.Add(
					I18n.Setting_Nearby_Enable,
					null,
					c => c.UseDiscovery,
					(c, v) => c.UseDiscovery = v
				)
				.Add(
					I18n.Setting_Nearby_Diagonal,
					I18n.Setting_Nearby_Diagonal_Tip,
					c => c.UseDiagonalConnections,
					(c, v) => c.UseDiagonalConnections = v
				)
				.AddLabel(
					I18n.Setting_Nearby_Performance,
					I18n.Setting_Nearby_Performance_Tip,
					"page:perf"
				)
				.AddLabel(
					I18n.Setting_Nearby_Connectors,
					I18n.Setting_Nearby_Connectors_Tip,
					"page:conn"
				);

			GMCMIntegration
				.StartPage("page:perf", I18n.Setting_Nearby_Performance)
				.AddParagraph(I18n.Setting_Nearby_Performance_Tip)
				.Add(
					I18n.Setting_Nearby_MaxChests,
					I18n.Setting_Nearby_MaxChests_Tip,
					c => c.MaxInventories,
					(c, v) => c.MaxInventories = v,
					min: 4,
					max: 100
				)
				.Add(
					I18n.Setting_Nearby_MaxDistance,
					I18n.Setting_Nearby_MaxDistance_Tip,
					c => c.MaxDistance,
					(c, v) => c.MaxDistance = v,
					min: 1,
					max: 100
				)
				.Add(
					I18n.Setting_Nearby_MaxCheck,
					I18n.Setting_Nearby_MaxCheck_Tip,
					c => c.MaxCheckedTiles,
					(c, v) => c.MaxCheckedTiles = v,
					min: 0,
					max: 1000
				);

			GMCMIntegration
				.StartPage("page:conn", I18n.Setting_Nearby_Connectors)
				.AddParagraph(I18n.Setting_Nearby_Connectors_Tip);

			if (FloorMap != null) {
				GMCMIntegration.AddLabel(I18n.Setting_Nearby_Floors);

				var floors = FloorMap.Values.ToList();
				floors.Sort(StringComparer.InvariantCultureIgnoreCase);

				foreach (string connector in floors)
					if (!string.IsNullOrEmpty(connector))
						GMCMIntegration.Add(
							connector,
							null,
							c => c.ValidConnectors.Contains(connector),
							(c, v) => {
								if (v)
									c.ValidConnectors.Add(connector);
								else
									c.ValidConnectors.Remove(connector);
							}
						);
			}

			if (ConnectorExamples != null) {
				GMCMIntegration.AddLabel(I18n.Setting_Nearby_Other);

				var sorted = ConnectorExamples.ToList();
				sorted.Sort(StringComparer.InvariantCultureIgnoreCase);

				foreach (string connector in sorted)
					if ( ! string.IsNullOrEmpty(connector) )
						GMCMIntegration.Add(
							connector,
							null,
							c => c.ValidConnectors.Contains(connector),
							(c, v) => {
								if (v)
									c.ValidConnectors.Add(connector);
								else
									c.ValidConnectors.Remove(connector);
							}
						);
			}
		}

		#endregion

		public bool HasLoveOfCooking() {
			if (!hasLoveOfCooking.HasValue)
				hasLoveOfCooking = Helper.ModRegistry.IsLoaded("blueberry.LoveOfCooking");

			return hasLoveOfCooking.Value;
		}

		public bool HasBiggerBackpacks() {
			if (!hasBiggerBackpacks.HasValue)
				hasBiggerBackpacks = Helper.ModRegistry.IsLoaded("spacechase0.BiggerBackpack");

			return hasBiggerBackpacks.Value;
		}

		public IEnumerable<GameLocation> GetLocations() {
			return Helper.Multiplayer.GetActiveLocations();
		}

		public int GetBackpackRows(Farmer who) {
			int rows = who.MaxItems / 12;
			if (rows < 3) rows = 3;
			if (rows < 4 && HasBiggerBackpacks()) rows = 4;
			return rows;
		}

		public bool CanEnterNutDoor() {
			int num = Math.Max(Game1.netWorldState.Value.GoldenWalnutsFound.Value - 1, 0);
			return num >= 100;
		}

		public bool DoesTranslationExist(string key) {
			// SMAPI's translation API is very opaque.
			// But SMAPI's reflection helper is here to help with SMAPI.
			// Thank you, SMAPI.
			object Translator = Helper.Reflection.GetField<object>(Helper.Translation, "Translator", false).GetValue();
			IDictionary<string, Translation> ForLocale = Translator == null ? null : Helper.Reflection.GetField<IDictionary<string, Translation>>(Translator, "ForLocale", false).GetValue();
			return ForLocale != null && ForLocale.ContainsKey(key);
		}

		#region Connectors

		private void LoadFloorMap() {
			const string path = "assets/floors.json";
			Dictionary<int, string> floors = null;

			try {
				floors = Helper.Data.ReadJsonFile<Dictionary<int, string>>(path);
				if (floors == null)
					Log($"The {path} file is missing or invalid.", LogLevel.Error);
			} catch(Exception ex) {
				Log($"The {path} file is invalid.", LogLevel.Error, ex);
			}

			if (floors == null)
				floors = new();

			// Read any extra data files
			foreach(var cp in Helper.ContentPacks.GetOwned()) {
				if (!cp.HasFile("floors.json"))
					continue;

				Dictionary<int, string> extra = null;
				try {
					extra = cp.ReadJsonFile<Dictionary<int, string>>("floors.json");
				} catch(Exception ex) {
					Log($"The floors.json file of {cp.Manifest.Name} is invalid.", LogLevel.Error, ex);
				}

				if (extra != null)
					foreach(var entry in extra)
						if ( string.IsNullOrEmpty(entry.Value) )
							floors.Remove(entry.Key);
						else
							floors[entry.Key] = entry.Value;
			}

			FloorMap = floors;
		}

		private void LoadConnectorExamples() {
			const string path = "assets/connector_examples.json";
			CaseInsensitiveHashSet examples = null;

			try {
				examples = Helper.Data.ReadJsonFile<CaseInsensitiveHashSet>(path);
				if (examples == null)
					Log($"The {path} file is missing or invalid.", LogLevel.Error);
			} catch (Exception ex) {
				Log($"The {path} file is invalid.", LogLevel.Error, ex);
			}

			if (examples == null)
				examples = new();

			// Read any extra data files
			foreach(var cp in Helper.ContentPacks.GetOwned()) {
				if (!cp.HasFile("connector_examples.json"))
					continue;

				List<string> extra = null;
				try {
					extra = cp.ReadJsonFile<List<string>>("connector_examples.json");
				} catch(Exception ex) {
					Log($"The connector_examples.json file of {cp.Manifest.Name} is invalid.", LogLevel.Error, ex);
				}

				if (extra != null)
					foreach (string entry in extra) {
						if (string.IsNullOrEmpty(entry))
							continue;
						else if (entry.StartsWith("--"))
							examples.Remove(entry[2..]);
						else if (entry.StartsWith(" --"))
							examples.Add(entry[1..]);
						else
							examples.Add(entry);
					}
			}

			ConnectorExamples = examples;
		}

		public bool IsValidConnector(object obj) {
			// TODO: Check for MoveToConnected?

			if (obj == null) return false;

			switch(obj) {
				case Item item:
					return Config.ValidConnectors.Contains(item.Name);

				case Flooring floor:
					return FloorMap != null
						&& FloorMap.TryGetValue(floor.whichFloor.Value, out string name)
						&& name != null
						&& Config.ValidConnectors.Contains(name);

				default:
					return false;
			}

		}

		#endregion

		#region Providers

		public void RegisterProviders() {
			RegisterInventoryProvider(typeof(Chest), new ChestProvider(any: true));
			RegisterInventoryProvider(typeof(Workbench), new WorkbenchProvider());
		}

		public void RegisterInventoryProvider(Type type, IInventoryProvider provider) {
			lock (providerLock) {
				invProviders[type] = provider;
			}
		}

		public void UnregisterInventoryProvider(Type type) {
			lock (providerLock) {
				if (invProviders.ContainsKey(type))
					invProviders.Remove(type);
			}
		}

		public IInventoryProvider GetInventoryProvider(object obj) {
			// TODO: Check for MoveToConnected?

			Type type = obj?.GetType();
			if (type == null || !invProviders.ContainsKey(type))
				return null;

			return (IInventoryProvider)invProviders[type];
		}

		#endregion

	}

}
