/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

using HarmonyLib;

using Leclair.Stardew.BetterCrafting.DynamicRules;
using Leclair.Stardew.BetterCrafting.Managers;
using Leclair.Stardew.BetterCrafting.Menus;
using Leclair.Stardew.BetterCrafting.Models;
using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Enums;
using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.Integrations.GenericModConfigMenu;
using Leclair.Stardew.Common.Inventory;
using Leclair.Stardew.Common.Types;
using Leclair.Stardew.Common.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Nanoray.Pintail;

using Newtonsoft.Json.Linq;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using StardewValley;
using StardewValley.Buildings;
using StardewValley.Inventories;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.TokenizableStrings;

using SMAPIJsonHelper = StardewModdingAPI.Toolkit.Serialization.JsonHelper;

namespace Leclair.Stardew.BetterCrafting;

public class ModEntry : PintailModSubscriber {

	public static readonly string NPCMapLocationPath = "Mods/Bouhm.NPCMapLocations/NPCs";

	public static readonly string HeadsPath = "Mods/leclair.bettercrafting/Heads";

	public static readonly string MagicWorkbenchTexture = @"Mods/leclair.bettercrafting/Texture/MagicWorkbench";
	public static readonly string MagicWorkbenchId = "leclair.bettercrafting_MagicWorkbench";


#nullable disable
	public static ModEntry Instance { get; private set; }
#nullable enable

	internal readonly Dictionary<IManifest, ModAPI> APIInstances = new();

	internal SMAPIJsonHelper? JsonHelper;
	internal Harmony? Harmony;

	private readonly PerScreen<IClickableMenu?> CurrentMenu = new();
	private readonly PerScreen<Menus.BetterCraftingPage?> OldCraftingPage = new();
	private readonly PerScreen<bool> OldCraftingGameMenu = new();

	internal readonly PerScreen<Inventory> TrashedItems = new(() => new());

	private bool? hasBiggerBackpacks;
	private bool? hasLoveOfCooking;

	public bool AtTitle = false;

	private bool ConfigRegistered = false;

#nullable disable
	public ModConfig Config;

	public DataRecipeManager DataRecipes;
	public RecipeManager Recipes;
	public FavoriteManager Favorites;
	public ItemCacheManager ItemCache;
	public TriggerManager Triggers;
	public CraftingStationManager Stations;
	public SpookyActionAtADistance SpookyAction;

	internal ThemeManager<Theme> ThemeManager;

#nullable enable

	private Dictionary<string, HeadSize>? HeadsCache = null;

	private MenuPriority? ActivePriority = null;

	private readonly Hashtable invProviders = new();
	private readonly object providerLock = new();

	private CaseInsensitiveHashSet? ConnectorExamples;
	private Dictionary<string, (string, string)>? FloorMap;
	private Dictionary<string, (string, string)>? FenceMap;

	private GMCMIntegration<ModConfig, ModEntry>? GMCMIntegration;

	internal Integrations.ProducerFrameworkMod.PFMIntegration? intPFM;
	//internal Integrations.RaisedGardenBeds.RGBIntegration? intRGB;
	//internal Integrations.ConvenientChests.CCIntegration? intCC;
	internal Integrations.StackSplitRedux.SSRIntegration? intSSR;
	internal Integrations.CookingSkill.CSIntegration? intCSkill;
	internal Integrations.SpaceCore.SCIntegration? intSCore;
	internal Integrations.CustomCraftingStation.CCSIntegration? intCCStation;

	private bool? _UseGlobalSave;

	private TextureColorWatcher? TextureWatcher;

	public override void Entry(IModHelper helper) {
		base.Entry(helper);
		SpriteHelper.SetHelper(helper);
		RenderHelper.SetHelper(helper);

		Instance = this;
		AdvancedMultipleMutexRequest.Mod = this;
		MultipleEventedInventoryExclusiveRequest.Mod = this;

		// Before Harmony...
		SpookyAction = new SpookyActionAtADistance(this);

		// Harmony
		Harmony = new Harmony(ModManifest.UniqueID);

		SpookyAction.PatchGame(Harmony);
		Patches.CraftingPage_Patches.Patch(this);
		Patches.LetterViewerMenu_Patches.Patch(this);
		Patches.SObject_Patches.Patch(this);
		Patches.Item_Patches.Patch(this);
		Patches.GameLocation_Patches.Patch(this);
		Patches.Torch_Patches.Patch(this);
		Patches.Utility_Patches.Patch(this);
		Patches.Workbench_Patches.Patch(this);

		// Read Config
		Config = Helper.ReadConfig<ModConfig>();

		// Init
		I18n.Init(Helper.Translation);

		RegisterProviders();

		ItemCache = new ItemCacheManager(this);
		Recipes = new RecipeManager(this);
		DataRecipes = new DataRecipeManager(this);
		Favorites = new FavoriteManager(this);
		Triggers = new TriggerManager(this);
		Stations = new CraftingStationManager(this);

		InjectMenuHandler();
	}

	public override object? GetApi(IModInfo mod) {
		if (!APIInstances.TryGetValue(mod.Manifest, out var api)) {
			Log($"Creating specific API instance for {mod.Manifest.Name} ({mod.Manifest.UniqueID})", LogLevel.Debug);
			api = new ModAPI(this, mod.Manifest);
			APIInstances[mod.Manifest] = api;
		}

		return api;
	}

	#region Events

	private void InjectMenuHandler() {
		if (Config is null)
			return;

		if (ActivePriority is not null) {
			if (ActivePriority == Config.MenuPriority)
				return;

			switch (ActivePriority) {
				case MenuPriority.Low:
					Helper.Events.Display.MenuChanged -= LowMenuChanged;
					break;
				case MenuPriority.Normal:
					Helper.Events.Display.MenuChanged -= NormalMenuChanged;
					break;
				case MenuPriority.High:
					Helper.Events.Display.MenuChanged -= HighMenuChanged;
					break;
			}
		}

		switch (Config.MenuPriority) {
			case MenuPriority.Low:
				Helper.Events.Display.MenuChanged += LowMenuChanged;
				ActivePriority = MenuPriority.Low;
				return;
			case MenuPriority.High:
				Helper.Events.Display.MenuChanged += HighMenuChanged;
				ActivePriority = MenuPriority.High;
				return;
		}

		Helper.Events.Display.MenuChanged += NormalMenuChanged;
		ActivePriority = MenuPriority.Normal;
	}

	private static void UpdateTextures(Texture2D? oldTex, Texture2D newTex, IClickableMenu menu) {
		if (menu.allClickableComponents != null)
			foreach (var cmp in menu.allClickableComponents) {
				if (cmp is ClickableTextureComponent tp && tp.texture == oldTex)
					tp.texture = newTex;
			}
	}

	private void OnThemeChanged(object? sender, ThemeChangedEventArgs<Models.Theme> e) {
		var oldTex = Sprites.Buttons._TexCache;
		Sprites.Buttons._TexCache = null;

		if (Game1.activeClickableMenu is GameMenu gm) {
			foreach (var gmp in gm.pages) {
				if (gmp is Menus.BetterCraftingPage bcp) {
					UpdateTextures(oldTex, Sprites.Buttons.Texture!, gmp);
					bcp.LoadTextures();
				}
			}
		}

		if (Game1.activeClickableMenu is BetterCraftingPage page) {
			UpdateTextures(oldTex, Sprites.Buttons.Texture!, page);
			page.LoadTextures();
		}
	}

	[EventPriority(EventPriority.Low)]
	private void LowMenuChanged(object? sender, MenuChangedEventArgs e) {
		HandleMenuChanged();
	}

	private void NormalMenuChanged(object? sender, MenuChangedEventArgs e) {
		HandleMenuChanged();
	}

	[EventPriority(EventPriority.High)]
	private void HighMenuChanged(object? sender, MenuChangedEventArgs e) {
		HandleMenuChanged();
	}

	[Subscriber]
	private void OnClickRecycle(object? sender, ButtonPressedEventArgs e) {
		if (!Config.EffectiveAllowRecoverTrash ||
			!e.Button.IsActionButton() ||
			(Game1.activeClickableMenu is not null && !Game1.activeClickableMenu.readyToClose())
		)
			return;

		var page = Game1.activeClickableMenu;
		if (page is GameMenu gm)
			page = gm.GetCurrentPage();

		int x = Game1.getOldMouseX();
		int y = Game1.getOldMouseY();

		if (page is TrashGrabMenu)
			return;

		else if (page is ItemGrabMenu igm) {
			if (!igm.trashCan.containsPoint(x, y))
				return;

		} else if (page is InventoryPage inv) {
			if (!inv.trashCan.containsPoint(x, y))
				return;

		} else if (page is CraftingPage cp) {
			if (!cp.trashCan.containsPoint(x, y))
				return;

		} else if (page is BetterCraftingPage bcp) {
			if (!bcp.trashCan.containsPoint(x, y))
				return;

		} else
			return;

		Helper.Input.Suppress(e.Button);

		Game1.nextClickableMenu.Insert(0, Game1.activeClickableMenu);
		Game1.activeClickableMenu = new TrashGrabMenu(this, TrashedItems.Value);
		Game1.playSound("trashcan");
	}

	private void HandleMenuChanged() {
		IClickableMenu menu = Game1.activeClickableMenu;
		if (CurrentMenu.Value == menu)
			return;

		Type? type = menu?.GetType();
		string? name = type?.FullName ?? type?.Name;

		// Are we doing GMCM stuff?
		if (OldCraftingPage.Value != null) {
			if (menu != null) {
				// If we're on the specific page for a mod, then
				// everything is fine and we can continue.
				if (name!.Equals("GenericModConfigMenu.Framework.SpecificModConfigMenu"))
					return;

				if (name!.Equals("GenericModConfigMenu.Framework.ModConfigMenu")) {
					CommonHelper.YeetMenu(menu);

					GameMenu? game = null;
					if (OldCraftingGameMenu.Value) {
						game = new GameMenu(false);
						menu = Game1.activeClickableMenu = game;
					}

					var bcm = BetterCraftingPage.Open(
						mod: this,
						location: OldCraftingPage.Value.BenchLocation,
						position: OldCraftingPage.Value.BenchPosition,
						width: game?.width ?? -1,
						height: game?.height ?? -1,
						x: game?.xPositionOnScreen ?? -1,
						y: game?.yPositionOnScreen ?? -1,
						area: OldCraftingPage.Value.BenchArea,
						cooking: OldCraftingPage.Value.cooking,
						standalone_menu: !OldCraftingGameMenu.Value,
						material_containers: OldCraftingPage.Value.MaterialContainers,
						silent_open: true,
						discover_containers: OldCraftingPage.Value.DiscoverContainers,
						discover_buildings: OldCraftingPage.Value.DiscoverBuildings,
						listed_recipes: OldCraftingPage.Value.GetListedRecipes(),
						station: OldCraftingPage.Value.Station?.Id,
						areaOverride: OldCraftingPage.Value.DiscoverAreaOverride
					);

					if (game != null) {
						for (int i = 0; i < game.pages.Count; i++) {
							if (game.pages[i] is CraftingPage cp) {
								CommonHelper.YeetMenu(cp);

								game.pages[i] = bcm;
								game.changeTab(i, false);
								break;
							}
						}

					} else
						Game1.activeClickableMenu = bcm;
				}
			}

			// Clear the old crafting page.
			OldCraftingPage.Value = null;
			OldCraftingGameMenu.Value = false;
		}

		// No menu?
		if (menu == null) {
			// Did we *used* to have a GameMenu
			if (CurrentMenu.Value is GameMenu gm1) {
				if (gm1.GetCurrentPage() is not BetterCraftingPage) {
					foreach (var page1 in gm1.pages) {
						if (page1 is BetterCraftingPage bcp1)
							bcp1.emergencyShutDown();
					}
				}
			}

			CurrentMenu.Value = null;
			return;
		}

		// Replace crafting pages.
		if (Config.SuppressBC?.IsDown() ?? false) {
			CurrentMenu.Value = menu;
			return;
		}

		if (menu is CraftingPage page) {
			if (page.cooking ? Config.ReplaceCooking : Config.ReplaceCrafting) {

				// Make a copy of the existing chests.
				List<object>? chests = page._materialContainers is null ? null : new(page._materialContainers);

				// Find our bench
				var where = page.GetBenchPosition(Game1.player);
				var area = page.GetBenchRegion(Game1.player);

				// Make sure to clean up the existing menu.
				CommonHelper.YeetMenu(page);

				// And now create our own.
				menu = Game1.activeClickableMenu = Menus.BetterCraftingPage.Open(
					mod: this,
					location: Game1.player.currentLocation,
					position: where,
					area: area,
					cooking: page.cooking,
					standalone_menu: true,
					material_containers: chests
				);
			}
		}

		if (intCCStation != null && intCCStation.IsLoaded && name!.Equals("StardewValley.Menus.CustomCraftingMenu")) {
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

					// Make a copy of the existing chests.
					var chests = Helper.Reflection.GetField<List<Chest>>(menu, "_materialContainers", false).GetValue();
					List<object>? containers = chests is null ? null : new(chests);

					// TODO: Find the bench

					// Make sure to clean up the existing menu.
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

					gm.pages[i] = new TemporaryCraftingPage(this, cp);

					// Make sure to clean up the existing menu.
					CommonHelper.YeetMenu(cp);
				}
			}
		}

		CurrentMenu.Value = menu;
	}

	[Subscriber]
	[EventPriority((EventPriority) int.MinValue)]
	private void AfterGameLaunched(object? sender, GameLaunchedEventArgs e) {
		var builder = ReflectionHelper.WhatPatchesMe(this, "  ", false);
		if (builder is not null)
			Log($"Detected Harmony Patches:\n{builder}", LogLevel.Trace);

		ReflectionHelper.UnpatchMe(this, Harmony!, source => {
			if (source == "FlyingTNT.ResourceStorage")
				return true;

			return false;
		});
	}

	[Subscriber]
	private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
		TextureWatcher = new(this, "Mods/leclair.bettercrafting/DynamicTextures/", (name, e) => {
			return () => Helper.ModContent.Load<IRawTextureData>($"assets/{name}");
		});

		ThemeManager = new ThemeManager<Theme>(this, Config.Theme);

		Sprites.Buttons._TexLoader = () => ThemeManager.ActiveThemeId == "default"
			? Helper.GameContent.Load<Texture2D>("Mods/leclair.bettercrafting/DynamicTextures/buttons.png")
			: ThemeManager.Load<Texture2D>("buttons.png");

		ThemeManager.ThemeChanged += OnThemeChanged;
		ThemeManager.Discover();

		// Load Data
		LoadFloorMap();
		LoadFenceMap();
		LoadConnectorExamples();

		// More Init
		AtTitle = true;
		RegisterSettings();

		// Integrations
		intPFM = new(this);
		//intRGB = new(this);
		intSSR = new(this);
		//intCC = new(this);
		intCSkill = new(this);
		intSCore = new(this);
		intCCStation = new(this);

		// Commands
		Helper.ConsoleCommands.Add("bc_update", "Invalidate cached data.", (name, args) => {
			DataRecipes.Invalidate();
			Recipes.Invalidate();
			ItemCache.Invalidate();
			Stations.Invalidate();

			Log($"Invalided caches.", LogLevel.Info);
		});

		Helper.ConsoleCommands.Add("bc_retheme", "Reload all themes.", (name, args) => {
			ThemeManager.Discover();
			Log($"Reloaded themes. You may need to reopen menus.", LogLevel.Info);
		});

		Helper.ConsoleCommands.Add("bc_theme", "List all themes, or switch to a new theme.", (name, args) => {
			if (ThemeManager.PerformThemeCommand(args)) {
				Config.Theme = ThemeManager.SelectedThemeId!;
				SaveConfig();
			}
		});

		Helper.ConsoleCommands.Add("bc_trash", "Open the trash reclaimation menu.", (name, args) => {
			if (!Context.IsWorldReady || Game1.activeClickableMenu is not null)
				return;

			Game1.activeClickableMenu = new TrashGrabMenu(this, TrashedItems.Value);
		});

		Helper.ConsoleCommands.Add("bc_stations", "List all custom crafting stations, or open one if you provide a name.", (name, args) => {
			if (args.Length > 0 && !string.IsNullOrEmpty(args[0])) {
				string key = args[0].Trim();

				if (key.Equals("reload", StringComparison.OrdinalIgnoreCase)) {
					Stations.Invalidate();
					Log($"Invalidated cache.", LogLevel.Info);
					return;
				}

				if (!Stations.IsStation(key)) {
					Log($"No such station: {key}", LogLevel.Warn);
					return;
				}

				Triggers.Map_OpenMenu(Game1.currentLocation, [
					"", "false", "false", key
				], Game1.player, Game1.player.Position.ToPoint());
				return;
			}

			Log($"Available Crafting Stations:", LogLevel.Info);
			foreach (var station in Stations.GetStations())
				Log($" [{station.Id}]: {station.DisplayName} (Recipes: {station.Recipes?.Length ?? 0}, Exclusive: {station.AreRecipesExclusive})", LogLevel.Info);
		});

	}

	[Subscriber]
	private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e) {
		_UseGlobalSave = null;
		AtTitle = false;
		TrashedItems.ResetAllScreens();
		RegisterSettings();

		// Load our settings.
		if (Context.IsMultiplayer && Context.IsMainPlayer)
			UpdateMultiplayerConfig();

		// Touch this to load our texture ahead of time.
		_ = Sprites.Buttons.Texture;
	}


	[Subscriber]
	private void OnReturnToTitle(object? sender, ReturnedToTitleEventArgs e) {
		_UseGlobalSave = null;
		AtTitle = true;
		TrashedItems.ResetAllScreens();
		RegisterSettings();
	}

	[Subscriber]
	private void OnAssetInvalidated(object? sender, AssetsInvalidatedEventArgs e) {
		bool reload_settings = false;

		foreach (var name in e.Names) {
			if (name.IsEquivalentTo(HeadsPath))
				HeadsCache = null;

			/*if (name.IsEquivalentTo(@"Mods/leclair.bettercrafting/DynamicTextures/buttons.png") && ThemeManager.ActiveThemeId == "default")
				Sprites.Buttons._TexCache = null;*/

			bool is_objects = name.IsEquivalentTo(@"Data/Objects");

			if (is_objects || name.IsEquivalentTo(@"Data/Fences")) {
				var old_map = FenceMap;
				FenceMap = null;
				if (old_map is not null) {
					LoadFenceMap();
					if (!old_map.ShallowEquals(FenceMap))
						reload_settings = true;
				}
			}

			if (is_objects || name.IsEquivalentTo(@"Data/FloorsAndPaths")) {
				var old_map = FloorMap;
				FloorMap = null;
				if (old_map is not null) {
					LoadFloorMap();
					if (!old_map.ShallowEquals(FloorMap))
						reload_settings = true;
				}
			}
		}

		if (reload_settings)
			RegisterSettings();
	}

	[Subscriber]
	private void OnObjectsChanged(object? sender, ObjectListChangedEventArgs e) {
		if (!Config.EnableCookoutLongevity)
			return;

		// Disable the destroyOvernight flag on Cookout Kits when they're placed.
		if (e.Added is not null)
			foreach (var pair in e.Added) {
				var obj = pair.Value;
				if (obj is Torch torch && torch.bigCraftable.Value && torch.ParentSheetIndex == 278) {
					torch.destroyOvernight = false;
				}
			}

		// When a Cookout Kit is removed, drop the Cookout Kit item at its location.
		if (e.Removed is not null)
			foreach (var pair in e.Removed) {
				var obj = pair.Value;
				if (obj is Torch torch && torch.bigCraftable.Value && torch.ParentSheetIndex == 278 && torch.Fragility != 2) {
					e.Location.debris.Add(new Debris(ItemRegistry.Create("(O)926", 1), new Vector2(pair.Key.X * 64 + 32, pair.Key.Y * 64 + 32)));
				}
			}
	}


	[Subscriber]
	private void OnAssetRequested(object? sender, AssetRequestedEventArgs e) {
		// Edit the cookout kit's recipe.
		if (Config.EnableCookoutExpensive && e.Name.IsEquivalentTo(@"Data/CraftingRecipes"))
			e.Edit(data => {
				var recipes = data.AsDictionary<string, string>();
				if (recipes.Data.TryGetValue("Cookout Kit", out string? value)) {
					string[] parts = value.Split('/');
					parts[0] = "390 45 388 15 382 8 335 2";
					recipes.Data["Cookout Kit"] = string.Join('/', parts);
				}
			});

		if (e.Name.IsEquivalentTo(HeadsPath))
			e.LoadFrom(() => {
				const string path = "assets/heads.json";
				Dictionary<string, HeadSize>? heads = null;

				try {
					heads = Helper.Data.ReadJsonFile<Dictionary<string, HeadSize>>(path);
					if (heads is null)
						Log($"The {path} file is missing or invalid.", LogLevel.Error);
				} catch (Exception ex) {
					Log($"The {path} file is invalid.", LogLevel.Error, ex);
				}

				heads ??= new();

				// Read any extra data files.
				foreach (var cp in Helper.ContentPacks.GetOwned()) {
					if (!cp.HasFile("heads.json"))
						continue;

					Dictionary<string, HeadSize>? extra = null;
					try {
						extra = cp.ReadJsonFile<Dictionary<string, HeadSize>>("heads.json");
					} catch (Exception ex) {
						Log($"The heads.json file of {cp.Manifest.Name} is invalid.", LogLevel.Error, ex);
					}

					if (extra != null)
						foreach (var entry in extra)
							if (!string.IsNullOrEmpty(entry.Key))
								heads[entry.Key] = entry.Value;
				}

				// Now, read the data file used by NPC Map Locations. This is
				// convenient because a lot of mods support it.
				Dictionary<string, JObject>? content = null;

				try {
					content = Helper.GameContent.Load<Dictionary<string, JObject>>(NPCMapLocationPath);

				} catch (Exception) {
					/* Nothing~ */
				}

				if (content is not null) {
					int count = 0;

					foreach (var entry in content) {
						if (heads.ContainsKey(entry.Key))
							continue;

						int offset;
						try {
							offset = entry.Value.Value<int>("MarkerCropOffset");
						} catch (Exception) {
							continue;
						}

						heads[entry.Key] = new() {
							OffsetY = offset
						};
						count++;
					}

					Log($"Loaded {count} head offsets from NPC Map Location data.");
				}

				return heads;

			}, AssetLoadPriority.High);
	}

	public Dictionary<string, HeadSize> GetHeads() {
		HeadsCache ??= Helper.GameContent.Load<Dictionary<string, HeadSize>>(HeadsPath);
		return HeadsCache;
	}

	#endregion

	#region Configuration

	public bool UseGlobalSave {
		get {
			if (_UseGlobalSave.HasValue)
				return _UseGlobalSave.Value;

			string path = $"savedata/settings/{Constants.SaveFolderName}.json";
			SaveSpecificConfig? data;

			try {
				data = Helper.Data.ReadJsonFile<SaveSpecificConfig>(path);
			} catch (Exception ex) {
				Log($"The {path} file is invalid or corrupt.", LogLevel.Error, ex);
				data = null;
			}

			_UseGlobalSave = data?.UseGlobalSave;

			// We need a sensible default.
			_UseGlobalSave ??=
				!(Favorites.DoesHaveSaveFavorites() ||
				Recipes.DoesHaveSaveCategories());

			return _UseGlobalSave.Value;
		}

		set {
			if (_UseGlobalSave == value || string.IsNullOrEmpty(Constants.SaveFolderName))
				return;

			_UseGlobalSave = value;
			string path = $"savedata/settings/{Constants.SaveFolderName}.json";
			Helper.Data.WriteJsonFile(path, new SaveSpecificConfig() {
				UseGlobalSave = _UseGlobalSave
			});

			Recipes.ReloadCategories();
			Favorites.LoadFavorites();
		}
	}

	public void SaveConfig() {
		Helper.WriteConfig(Config);
		Helper.GameContent.InvalidateCache(@"Data/CraftingRecipes");
		InjectMenuHandler();

		if (Context.IsMultiplayer && Context.IsMainPlayer)
			UpdateMultiplayerConfig();
	}


	private Dictionary<string, PropertyInfo>? ConfigFields;

	private void UpdateMultiplayerConfig() {
		if (Game1.MasterPlayer != Game1.player || Game1.MasterPlayer == null)
			return;

		if (ConfigFields == null) {
			ConfigFields = new();
			foreach (var prop in AccessTools.GetDeclaredProperties(typeof(ModConfig))) {
				if (prop.GetCustomAttribute<MultiplayerSyncSetting>() != null && prop.CanRead)
					ConfigFields[prop.Name] = prop;
			}
		}

		foreach (var pair in ConfigFields) {
			object? value = pair.Value.GetValue(Config);
			string? sval;
			if (value == null)
				sval = null;
			else if (value is bool vbool)
				sval = vbool.ToString();
			else if (value is int vint)
				sval = vint.ToString();
			else if (value is long vlong)
				sval = vlong.ToString();
			else if (value is string vstring)
				sval = vstring;
			else {
				GetJsonHelper();
				sval = JsonHelper?.Serialize(value);
			}

			string key = $"leclair.bettercrafting/{pair.Key}";
			if (sval != null)
				Game1.MasterPlayer.modData[key] = sval;
			else
				Game1.MasterPlayer.modData.Remove(key);
		}
	}


	[MemberNotNullWhen(true, nameof(GMCMIntegration))]
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

		var storages = GetStorages();

		Dictionary<SeasoningMode, Func<string>> seasoning = new();
		seasoning.Add(SeasoningMode.Disabled, I18n.Seasoning_Disabled);
		seasoning.Add(SeasoningMode.Enabled, I18n.Seasoning_Enabled);
		seasoning.Add(SeasoningMode.InventoryOnly, I18n.Seasoning_Inventory);

		GMCMIntegration.Register(true);

		if (!AtTitle)
			GMCMIntegration
				.Add(
					I18n.Setting_ShowAdv,
					I18n.Setting_ShowAdv_Tip,
					c => Game1.options.showAdvancedCraftingInformation,
					(c, v) => Game1.options.changeCheckBoxOption(Options.toggleShowAdvancedCraftingInformation, v)
				)
				.Add(
					I18n.Setting_UsePerSave,
					I18n.Setting_UsePerSave_Tip,
					c => !UseGlobalSave,
					(c, v) => UseGlobalSave = !v
				);

		if (AtTitle || Context.IsMainPlayer)
			GMCMIntegration
				.Add(
					I18n.Setting_EnforceMultiplayer,
					I18n.Setting_EnforceMultiplayer_Tip,
					c => c.EnforceSettings,
					(c, v) => c.EnforceSettings = v
				);

		GMCMIntegration
			.AddLabel(I18n.Setting_General)
			.AddChoice(
				I18n.Setting_Theme,
				I18n.Setting_ThemeDesc,
				c => c.Theme,
				(c, v) => {
					c.Theme = v;
					ThemeManager.SelectTheme(v);
				},
				ThemeManager.GetThemeChoiceMethods()
			)
			.Add(
				I18n.Setting_UseFullHeight,
				I18n.Setting_UseFullHeight_Tip,
				c => c.UseFullHeight,
				(c, v) => c.UseFullHeight = v
			)
			.Add(
				I18n.Setting_Settings,
				I18n.Setting_Settings_Tip,
				c => c.ShowSettingsButton,
				(c, v) => c.ShowSettingsButton = v
			)
			.Add(
				I18n.Setting_ShowEdit,
				I18n.Setting_ShowEdit_Tip,
				c => c.ShowEditButton,
				(c, v) => c.ShowEditButton = v
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
			)
			.Add(
				I18n.Setting_SourceMod,
				I18n.Setting_SourceMod_Tip,
				c => c.ShowSourceModInTooltip,
				(c, v) => c.ShowSourceModInTooltip = v
			)
			.AddChoice(
				name: I18n.Setting_GiftTaste,
				tooltip: I18n.Setting_GiftTaste_Tip,
				get: c => c.ShowTastes,
				set: (c, v) => c.ShowTastes = v,
				choices: new Dictionary<GiftMode, Func<string>> {
					{ GiftMode.Never, I18n.Setting_GiftTaste_Never },
					{ GiftMode.Shift, I18n.Setting_GiftTaste_Shift },
					{ GiftMode.Always, I18n.Setting_GiftTaste_Always }
				}
			)
			.AddChoice(
				name: I18n.Setting_GiftTasteStyle,
				tooltip: I18n.Setting_GiftTasteStyle_Tip,
				get: c => c.TasteStyle,
				set: (c, v) => c.TasteStyle = v,
				choices: new Dictionary<GiftStyle, Func<string>> {
					{ GiftStyle.Heads, I18n.Setting_GiftTasteStyle_Heads },
					{ GiftStyle.Names, I18n.Setting_GiftTasteStyle_Names }
				}
			)
			.Add(
				name: I18n.Setting_GiftTasteAll,
				tooltip: I18n.Setting_GiftTasteAll_Tip,
				get: c => c.ShowAllTastes,
				set: (c, v) => c.ShowAllTastes = v
			)
			.Add(
				name: I18n.Setting_RecoverTrash,
				tooltip: I18n.Setting_RecoverTrash_About,
				get: c => c.AllowRecoverTrash,
				set: (c, v) => c.AllowRecoverTrash = v
			)
			.AddChoice(
				name: I18n.Setting_Priority,
				tooltip: I18n.Setting_Priority_Tip,
				get: c => c.MenuPriority,
				set: (c, v) => c.MenuPriority = v,
				choices: new Dictionary<MenuPriority, Func<string>> {
					{ MenuPriority.Low, I18n.Setting_Priority_Low },
					{ MenuPriority.Normal, I18n.Setting_Priority_Normal },
					{ MenuPriority.High, I18n.Setting_Priority_High },
				}
			)
			.AddChoice(
				name: I18n.Setting_NewRecipes,
				tooltip: I18n.Setting_NewRecipes_Tip,
				get: c => c.NewRecipes,
				set: (c, v) => c.NewRecipes = v,
				choices: new Dictionary<NewRecipeMode, Func<string>> {
					{ NewRecipeMode.Disabled, I18n.Setting_NewRecipes_Disabled },
					{ NewRecipeMode.Uncrafted, I18n.Setting_NewRecipes_Uncrafted },
					{ NewRecipeMode.Unseen, I18n.Setting_NewRecipes_Unseen }
				}
			)
			.Add(
				name: I18n.Setting_NewRecipes_Prismatic,
				tooltip: I18n.Setting_NewRecipes_Prismatic_Tip,
				get: c => c.NewRecipesPrismatic,
				set: (c, v) => c.NewRecipesPrismatic = v
			)

			.AddLabel(I18n.Setting_Bindings, I18n.Setting_Bindings_Tip, "page:bindings")

			.AddLabel(I18n.Setting_Crafting, I18n.Setting_Crafting_Tip)
			.Add(
				I18n.Setting_UniformGrid,
				I18n.Setting_UniformGrid_Tip,
				c => c.UseUniformGrid,
				(c, val) => c.UseUniformGrid = val
			)
			.Add(
				I18n.Setting_Alphabetic,
				I18n.Setting_Alphabetic_Tip,
				c => c.CraftingAlphabetic,
				(c, v) => c.CraftingAlphabetic = v
			)
			.Add(
				I18n.Setting_BigCraftablesLast,
				I18n.Setting_BigCraftablesLast_Tip,
				c => c.SortBigLast,
				(c, val) => c.SortBigLast = val
			)
			.Add(
				I18n.Setting_ShowUnknown,
				I18n.Setting_ShowUnknown_Tip,
				c => c.DisplayUnknownCrafting,
				(c, v) => c.DisplayUnknownCrafting = v
			)

			.AddLabel(I18n.Setting_Cooking, I18n.Setting_Cooking_Tip)
			.Add(
				I18n.Setting_Alphabetic,
				I18n.Setting_Alphabetic_Tip,
				c => c.CookingAlphabetic,
				(c, v) => c.CookingAlphabetic = v
			)
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
			)

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
			)
			.AddChoice(
				I18n.Setting_ShowMatchingItems,
				I18n.Setting_ShowMatchingItems_Tip,
				c => c.ShowMatchingItem,
				(c, v) => c.ShowMatchingItem = v,
				choices: new Dictionary<ShowMatchingItemMode, Func<string>> {
					{ ShowMatchingItemMode.Disabled, I18n.Setting_ShowMatchingItems_Never },
					{ ShowMatchingItemMode.Always, I18n.Setting_ShowMatchingItems_Always },
					{ ShowMatchingItemMode.Fuzzy, I18n.Setting_ShowMatchingItems_Fuzzy },
					{ ShowMatchingItemMode.FuzzyQuality, I18n.Setting_ShowMatchingItems_FuzzyQuality }
				}
			)
			.Add(
				I18n.Setting_Nearby_Nearby,
				I18n.Setting_Nearby_Nearby_Tip,
				c => c.NearbyRadius switch {
					-2 => -2,
					-1 => -1,
					0 => 0,
					_ => (int) (Math.Ceiling(Math.Log2(c.NearbyRadius)) - 1)
				},
				(c, v) => c.NearbyRadius = v switch {
					-2 => -2,
					-1 => -1,
					0 => 0,
					_ => (int) Math.Pow(2, v + 1)
				},
				-1, 4, 1,
				format: val => val switch {
					-2 => I18n.Setting_Nearby_Nearby_Active(),
					-1 => I18n.Setting_Nearby_Nearby_Map(),
					0 => I18n.Setting_Nearby_Nearby_Off(),
					_ => I18n.Setting_Nearby_Nearby_Tiles($"{Math.Pow(2, val + 1)}")
				}
			)

			.AddLabel(
				I18n.Setting_Recycle,
				I18n.Setting_Recycle_About,
				"page:recycle"
			)

			.AddLabel(
				I18n.Setting_Nearby,
				I18n.Setting_Nearby_Tip,
				"page:nearby"
			)

			.AddLabel(
				I18n.Setting_Cookout,
				I18n.Setting_Cookout_About,
				"page:cookout"
			)

			.AddLabel(
				I18n.Setting_Transfer,
				I18n.Setting_Transfer_About,
				"page:transfer"
			);

		Dictionary<RecyclingMode, Func<string>> recycleModes = new() {
			[RecyclingMode.Automatic] = I18n.Setting_RecycleMode_Automatic,
			[RecyclingMode.Enabled] = I18n.Setting_RecycleMode_Enabled,
			[RecyclingMode.Disabled] = I18n.Setting_RecycleMode_Disabled
		};

		Dictionary<TTWhen, Func<string>> whens = new() {
			[TTWhen.Never] = I18n.Setting_Ttwhen_Never,
			[TTWhen.ForController] = I18n.Setting_Ttwhen_ForController,
			[TTWhen.Always] = I18n.Setting_Ttwhen_Always,
		};

		Dictionary<ButtonAction, Func<string>> actions = new() {
			[ButtonAction.None] = I18n.Setting_Action_None,
			[ButtonAction.Craft] = I18n.Setting_Action_Craft,
			[ButtonAction.BulkCraft] = I18n.Setting_Action_BulkCraft,
			[ButtonAction.Favorite] = I18n.Setting_Action_Favorite
		};

		GMCMIntegration
			.StartPage("page:bindings", I18n.Setting_Bindings)
			.AddChoice(
				I18n.Setting_Key_Tooltip,
				I18n.Setting_Key_Tooltip_Tip,
				c => c.ShowKeybindTooltip,
				(c, v) => c.ShowKeybindTooltip = v,
				whens
			)
			.Add(
				I18n.Setting_Suppress,
				I18n.Setting_Suppress_Tip,
				c => c.SuppressBC,
				(c, v) => c.SuppressBC = v
			)
			.AddLabel("")
			.Add(
				I18n.Setting_Key_Modifier,
				I18n.Setting_Key_Modifier_Tip,
				c => c.ModiferKey,
				(c, v) => c.ModiferKey = v
			)
			.Add(
				I18n.Setting_Key_Search,
				I18n.Setting_Key_Search_Tip,
				c => c.SearchKey,
				(c, v) => c.SearchKey = v
			)
			.Add(
				I18n.Setting_Key_Favorite,
				I18n.Setting_Key_Favorite_Tip,
				c => c.FavoriteKey,
				(c, v) => c.FavoriteKey = v
			)
			.Add(
				I18n.Setting_Key_Bulk,
				I18n.Setting_Key_Bulk_Tip,
				c => c.BulkCraftKey,
				(c, v) => c.BulkCraftKey = v
			)
			.AddLabel("");

		// Use Tool
		GMCMIntegration
			.AddChoice(
				I18n.Setting_Key_Behavior_Left,
				I18n.Setting_Key_Behavior_Left_Tip,
				c => c.LeftClick,
				(c, v) => c.LeftClick = v,
				actions
			)
			.AddChoice(
				I18n.Setting_Key_Behavior_Right,
				I18n.Setting_Key_Behavior_Right_Tip,
				c => c.RightClick,
				(c, v) => c.RightClick = v,
				actions
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

		if (storages != null && storages.Count > 0) {
			GMCMIntegration
				.StartPage("page:invalid-storage", I18n.Setting_InvalidStorageTypes)
				.AddParagraph(I18n.Setting_InvalidStorageTypes_Tip);

			foreach (var pair in storages)
				GMCMIntegration.Add(
					() => TokenParser.ParseText(pair.Value),
					null,
					c => c.InvalidStorages.Contains(pair.Key),
					(c, v) => {
						if (v)
							c.InvalidStorages.Add(pair.Key);
						else
							c.InvalidStorages.Remove(pair.Key);
					}
				);
		}

		GMCMIntegration
			.StartPage("page:conn", I18n.Setting_Nearby_Connectors)
			.AddParagraph(I18n.Setting_Nearby_Connectors_Tip);

		if (FloorMap != null) {
			GMCMIntegration.AddLabel(I18n.Setting_Nearby_Floors);

			var floors = FloorMap.Values.ToList();
			floors.Sort((a, b) => a.Item2.CompareTo(b.Item2));

			foreach (var pair in floors) {
				string connector = pair.Item1;
				string displayName = pair.Item2;

				if (!string.IsNullOrEmpty(connector))
					GMCMIntegration.Add(
						displayName,
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

		if (FenceMap != null) {
			GMCMIntegration.AddLabel(I18n.Setting_Nearby_Fences);

			var fences = FenceMap.Values.ToList();
			fences.Sort((a, b) => a.Item2.CompareTo(b.Item2));

			foreach (var pair in fences) {
				string connector = pair.Item1;
				string displayName = pair.Item2;

				if (!string.IsNullOrEmpty(connector))
					GMCMIntegration.Add(
						displayName,
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

		if (ConnectorExamples != null) {
			GMCMIntegration.AddLabel(I18n.Setting_Nearby_Other);

			var sorted = ConnectorExamples.ToList();
			sorted.Sort(StringComparer.InvariantCultureIgnoreCase);

			foreach (string connector in sorted)
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

		GMCMIntegration
			.StartPage("page:recycle", I18n.Setting_Recycle)
			.AddParagraph(I18n.Setting_Recycle_About)
			.Add(
				I18n.Setting_Recycle_ClickToggle,
				I18n.Setting_Recycle_ClickToggle_Tip,
				c => c.RecycleClickToggle,
				(c, v) => c.RecycleClickToggle = v
			)
			.AddChoice(
				I18n.Setting_Recycle_Crafting,
				I18n.Setting_Recycle_Crafting_Tip,
				c => c.RecycleCrafting,
				(c, v) => c.RecycleCrafting = v,
				recycleModes
			)
			.AddChoice(
				I18n.Setting_Recycle_Cooking,
				I18n.Setting_Recycle_Cooking_Tip,
				c => c.RecycleCooking,
				(c, v) => c.RecycleCooking = v,
				recycleModes
			)
			.AddLabel("")
			.Add(
				I18n.Setting_Recycle_HigherQuality,
				I18n.Setting_Recycle_HigherQuality_Tip,
				c => c.RecycleHigherQuality,
				(c, v) => c.RecycleHigherQuality = v
			)
			.Add(
				I18n.Setting_Recycle_Fuzzy,
				I18n.Setting_Recycle_Fuzzy_Tip,
				c => c.RecycleFuzzyItems,
				(c, v) => c.RecycleFuzzyItems = v
			)
			.Add(
				I18n.Setting_Recycle_Unknown,
				I18n.Setting_Recycle_Unknown_Tip,
				c => c.RecycleUnknownRecipes,
				(c, v) => c.RecycleUnknownRecipes = v
			);

		GMCMIntegration
			.StartPage("page:nearby", I18n.Setting_Nearby)
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

		if (storages != null && storages.Count > 0)
			GMCMIntegration
				.AddLabel(
					I18n.Setting_InvalidStorageTypes,
					I18n.Setting_InvalidStorageTypes_Tip,
					"page:invalid-storage"
				);

		GMCMIntegration
			.StartPage("page:cookout", I18n.Setting_Cookout)
			.AddParagraph(I18n.Setting_Cookout_About);

		GMCMIntegration
			.Add(
				I18n.Setting_Cookout_Workbench,
				I18n.Setting_Cookout_Workbench_Tip,
				c => c.EnableCookoutWorkbench,
				(c, v) => c.EnableCookoutWorkbench = v
			)
			.Add(
				I18n.Setting_Cookout_Durable,
				I18n.Setting_Cookout_Durable_Tip,
				c => c.EnableCookoutLongevity,
				(c, v) => c.EnableCookoutLongevity = v
			)
			.Add(
				I18n.Setting_Cookout_Expensive,
				I18n.Setting_Cookout_Expensive_Tip,
				c => c.EnableCookoutExpensive,
				(c, v) => c.EnableCookoutExpensive = v
			);

		GMCMIntegration
			.StartPage("page:transfer", I18n.Setting_Transfer)
			.AddParagraph(I18n.Setting_Transfer_About);

		GMCMIntegration
			.Add(
				I18n.Setting_Transfer_Enable,
				null,
				c => c.UseTransfer,
				(c, v) => c.UseTransfer = v
			)

			.AddLabel("") // Spacer

			.Add(
				I18n.Setting_Key_Modifier,
				I18n.Setting_Key_Modifier_Tip,
				c => c.ModiferKey,
				(c, v) => c.ModiferKey = v
			)

			.AddLabel(""); // Spacer

		GMCMIntegration.AddLabel(() => I18n.Setting_Transfer_UseTool(string.Join(", ", Game1.options.useToolButton.Select(x => x.ToSButton().ToString()))));
		AddBehaviorSettings(c => c.AddToBehaviors.UseTool);

		GMCMIntegration.AddLabel(I18n.Setting_Transfer_UseToolModifier);
		AddBehaviorSettings(c => c.AddToBehaviors.UseToolModified);

		GMCMIntegration.AddLabel(""); // Spacer

		GMCMIntegration.AddLabel(() => I18n.Setting_Transfer_Action(string.Join(", ", Game1.options.actionButton.Select(x => x.ToSButton().ToString()))));
		AddBehaviorSettings(c => c.AddToBehaviors.Action);

		GMCMIntegration.AddLabel(I18n.Setting_Transfer_ActionModifier);
		AddBehaviorSettings(c => c.AddToBehaviors.ActionModified);
	}

	private void AddBehaviorSettings(Func<ModConfig, TransferBehavior> accessor) {
		Dictionary<TransferMode, Func<string>> modes = new() {
			{ TransferMode.None, I18n.Setting_Transfer_Behavior_None },
			{ TransferMode.All, I18n.Setting_Transfer_Behavior_All },
			{ TransferMode.AllButQuantity, I18n.Setting_Transfer_Behavior_AllQuantity },
			{ TransferMode.Half, I18n.Setting_Transfer_Behavior_Half },
			{ TransferMode.Quantity, I18n.Setting_Transfer_Behavior_Quantity }
		};

		GMCMIntegration!
			.AddChoice(
				I18n.Setting_Transfer_Behavior,
				null,
				c => accessor(c).Mode,
				(c, v) => accessor(c).Mode = v,
				modes
			)

			.Add(
				I18n.Setting_Transfer_Quantity,
				I18n.Setting_Transfer_Quantity_Tip,
				c => accessor(c).Quantity,
				(c, v) => accessor(c).Quantity = v,
				min: 1,
				max: 999
			);
	}

	public static string GetInputLabel(InputButton[] buttons) {
		return string.Join(", ", buttons.Reverse().Select(btn => btn.ToString()));
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

	internal void GetJsonHelper() {
		if (JsonHelper is not null)
			return;

		if (Helper.Data.GetType().GetField("JsonHelper", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(Helper.Data) is SMAPIJsonHelper helper) {
			JsonHelper = new();
			var converters = JsonHelper.JsonSettings.Converters;
			converters.Clear();
			foreach (var converter in helper.JsonSettings.Converters)
				if (converter.GetType().Name != "ColorConverter")
					converters.Add(converter);

			//converters.Add(new VariableSetConverter());
			converters.Add(new Common.Serialization.Converters.ColorConverter());
		}
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

	public bool DoesTranslationExist(string key) {
		return Helper.Translation.Get(key).HasValue();
	}

	#region Connectors

	private Dictionary<string, string> GetStorages() {
		Dictionary<string, string> result = [];

		foreach (string key in StorageRuleHandler.VANILLA_CHESTS) {
			var md = ItemRegistry.GetMetadata(key);
			if (md.TypeIdentifier == ItemRegistry.type_bigCraftable) {
				if (Game1.bigCraftableData != null && Game1.bigCraftableData.TryGetValue(md.LocalItemId, out var data))
					result[key] = data.DisplayName;

			} else if (md.TypeIdentifier == ItemRegistry.type_furniture) {
				// TODO: This

			} else if (md.TypeIdentifier == ItemRegistry.type_object) {
				if (Game1.objectData != null && Game1.objectData.TryGetValue(md.LocalItemId, out var data))
					result[key] = data.DisplayName;
			}
		}

		if (Game1.bigCraftableData != null)
			foreach (var pair in Game1.bigCraftableData) {
				if (!result.ContainsKey(pair.Key) &&
					pair.Value.CustomFields is not null &&
					pair.Value.CustomFields.TryGetValue("furyx639.ExpandedStorage/Enabled", out string? isExpanded) &&
					isExpanded == "true"
				) {
					result[pair.Key] = pair.Value.DisplayName;
				}
			}

		if (Game1.buildingData != null)
			foreach (var pair in Game1.buildingData) {
				var chests = pair.Value.Chests;
				if (chests != null)
					result[pair.Key] = pair.Value.Name;
			}

		return result;
	}


	[MemberNotNull(nameof(FloorMap))]
	private void LoadFloorMap() {
		if (FloorMap is not null)
			return;

		var data = DataLoader.FloorsAndPaths(Game1.content);
		FloorMap = new();

		foreach (var pair in data) {
			if (string.IsNullOrEmpty(pair.Value.ItemId) || ItemRegistry.GetData(pair.Value.ItemId) is not ParsedItemData itemData)
				continue;

			string id = pair.Value.Id ?? pair.Key;
			FloorMap[id] = (itemData.InternalName, itemData.DisplayName);
		}
	}

	[MemberNotNull(nameof(FenceMap))]
	private void LoadFenceMap() {
		if (FenceMap is not null)
			return;

		var data = DataLoader.Fences(Game1.content);
		FenceMap = new();

		foreach (var pair in data) {
			if (string.IsNullOrEmpty(pair.Key) || ItemRegistry.GetData(pair.Key) is not ParsedItemData itemData)
				continue;

			FenceMap[pair.Key] = (itemData.InternalName, itemData.DisplayName);
		}
	}

	[MemberNotNull(nameof(ConnectorExamples))]
	private void LoadConnectorExamples() {
		const string path = "assets/connector_examples.json";
		CaseInsensitiveHashSet? examples = null;

		try {
			examples = Helper.Data.ReadJsonFile<CaseInsensitiveHashSet>(path);
			if (examples == null)
				Log($"The {path} file is missing or invalid.", LogLevel.Error);
		} catch (Exception ex) {
			Log($"The {path} file is invalid.", LogLevel.Error, ex);
		}

		examples ??= new();

		// Read any extra data files
		foreach (var cp in Helper.ContentPacks.GetOwned()) {
			if (!cp.HasFile("connector_examples.json"))
				continue;

			List<string>? extra = null;
			try {
				extra = cp.ReadJsonFile<List<string>>("connector_examples.json");
			} catch (Exception ex) {
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

	public bool IsInvalidStorage(object obj) {
		if (obj is SObject sobj)
			return Config.InvalidStorages.Contains(sobj.QualifiedItemId);

		if (obj is Building build)
			return Config.InvalidStorages.Contains(build.buildingType.Value);

		return false;
	}


	public bool IsValidConnector(object obj) {
		if (obj == null)
			return false;

		(string, string) names;

		switch (obj) {
			case Fence fence:
				// Just accept the raw value.
				if (Config.ValidConnectors.Contains(fence.ItemId))
					return true;

				// But also look up a better name.
				return FenceMap != null
					&& FenceMap.TryGetValue(fence.ItemId, out names)
					&& names.Item1 != null
					&& (
						Config.ValidConnectors.Contains(names.Item1) ||
						Config.ValidConnectors.Contains(names.Item2)
					);

			case Item item:
				return Config.ValidConnectors.Contains(item.Name);

			case Flooring floor:
				// Just accept the raw value.
				if (Config.ValidConnectors.Contains(floor.whichFloor.Value))
					return true;

				// But also look up a better name.
				return FloorMap != null
					&& FloorMap.TryGetValue(floor.whichFloor.Value, out names)
					&& names.Item1 != null
					&& (
						Config.ValidConnectors.Contains(names.Item1) ||
						Config.ValidConnectors.Contains(names.Item2)
					);

			default:
				return false;
		}
	}

	#endregion

	#region Providers

	private readonly Dictionary<Type, IProxyFactory<string>?> ProxyFactories = new();

	public bool TryPrimeProviderProxyFactory(string otherMod, Type sourceType, [NotNullWhen(true)] out IProxyFactory<string>? factory) {
		if (ProxyFactories.TryGetValue(sourceType, out factory))
			return factory is not null;

		// This will only run once per type.
		foreach (var type in new Type[] {
			typeof(IEventedInventoryProvider)
		}) {
			if (TryGetProxyFactory(sourceType, otherMod, type, ModManifest.UniqueID, out factory, silent: true)) {
				Log($"Proxying type {sourceType} into advanced IInventoryProvider type {type}.", LogLevel.Debug);
				break;
			}
		}

		ProxyFactories[sourceType] = factory;
		return factory is not null;
	}

	public IInventoryProvider UpgradeProviderProxy(string otherMod, IInventoryProvider provider) {

		if (!TryUnproxy(provider, out object? unboxed, silent: true))
			return provider;

		// If we somehow had a boxed copy of a native IRecipe, go for it.
		if (unboxed is IInventoryProvider ours)
			return ours;

		TryPrimeProviderProxyFactory(otherMod, unboxed.GetType(), out var factory);

		try {
			return factory is null ? provider : (IInventoryProvider) factory.ObtainProxy(GetProxyManager()!, unboxed);
		} catch (Exception) {
			return provider;
		}
	}

	public void RegisterProviders() {
		RegisterInventoryProvider(typeof(Chest), new ChestProvider(any: true));
		RegisterInventoryProvider(typeof(Workbench), new WorkbenchProvider());
		RegisterInventoryProvider(typeof(Building), new BuildingProvider());
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

	public IInventoryProvider? GetInventoryProvider(object obj) {
		Type? type = obj?.GetType();
		if (type is null)
			return null;

		if (!invProviders.ContainsKey(type)) {
			// Try less specific.
			if (obj is Chest)
				type = typeof(Chest);
			else if (obj is Building)
				type = typeof(Building);
			else if (obj is Workbench)
				type = typeof(Workbench);
			else {
				Type? target = null;
				foreach (Type knownType in invProviders.Keys) {
					if (knownType.IsAssignableFrom(type)) {
						if (target == null || knownType.IsAssignableFrom(target))
							target = knownType;
					}
				}

				if (target != null)
					invProviders[type] = invProviders[target];
				else
					return null;
			}
		}

		return invProviders[type] as IInventoryProvider;
	}

	#endregion

}

