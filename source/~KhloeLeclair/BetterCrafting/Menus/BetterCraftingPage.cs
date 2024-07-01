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
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

using Leclair.Stardew.BetterCrafting.DynamicRules;
using Leclair.Stardew.BetterCrafting.Integrations.SpaceCore;
using Leclair.Stardew.BetterCrafting.Models;
using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.Enums;
using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.Inventory;
using Leclair.Stardew.Common.Types;
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.UI.SimpleLayout;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

using StardewValley;
using StardewValley.Buffs;
using StardewValley.Delegates;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;

namespace Leclair.Stardew.BetterCrafting.Menus;


public class BetterCraftingPage : MenuSubscriber<ModEntry>, IBetterCraftingMenu {

#if DEBUG
	private static bool ShowTiming = false;
#endif

	public readonly int MAX_TABS;
	public readonly int VISIBLE_TABS;

	// TODO: Stop hard-coding seasoning.
	public static readonly IIngredient[] SEASONING_RECIPE = [
		new BaseIngredient(917, 1)
	];

	public static readonly Rectangle FAV_STAR = new(338, 400, 8, 8);

	// Session State
	public static string? LastTab { get; private set; } = null;
	public static bool FavoritesOnly { get; private set; } = false;

	// Menu Mode
	public readonly bool cooking; // I forget why this is here but I think it was mod compat so I'm leaving it for now.

	public bool Standalone { get; }
	public bool Cooking => cooking;

	public bool IsReady { get; private set; } = false;

	public bool DrawBG = true;

	GameLocation? IBetterCraftingMenu.Location => BenchLocation;
	Vector2? IBetterCraftingMenu.Position => BenchPosition;
	Rectangle? IBetterCraftingMenu.Area => BenchArea;

	public ISimpleNode? StationLabel { get; set; }

	// Station
	public CraftingStation? Station { get; private set; }
	ICraftingStation? IBetterCraftingMenu.Station => Station;

	// Workbench Tracking
	public readonly GameLocation? BenchLocation;
	public readonly Vector2? BenchPosition;
	public readonly Rectangle? BenchArea;
	public NetMutex? Mutex;

	public IList<LocatedInventory>? MaterialContainers;
	protected IList<LocatedInventory>? CachedInventories;
	private IList<IBCInventory>? UnsafeInventories;
	private HashSet<Item>? InventoryItems;
	private readonly bool ChestsOnly;
	public bool DiscoverContainers { get; private set; }
	public int? DiscoverAreaOverride { get; private set; }
	public bool DiscoverBuildings { get; private set; }

	// Editing Mode
	public bool Editing { get; protected set; } = false;
	public ClickableComponent? btnCategoryIcon;
	public TextBox? txtCategoryName;
	public ClickableComponent? btnCategoryName;
	public ClickableTextureComponent? btnCategoryFilter;
	public ClickableTextureComponent? btnCategoryIncludeInMisc;
	public ClickableTextureComponent? btnCategoryCopy;
	public ClickableTextureComponent? btnCategoryPaste;
	public ClickableTextureComponent? btnCategoryTrash;

	public List<ClickableComponent>? FlowComponents;
	public ClickableTextureComponent? btnFlowUp;
	public ClickableTextureComponent? btnFlowDown;

	private ScrollableFlow? Flow;

	// Menu Components
	[SkipForClickableAggregation]
	public readonly InventoryMenu inventory;

	public readonly ClickableTextureComponent trashCan;
	public float trashCanLidRotation;

	public ClickableTextureComponent? btnTransferTo;
	public ClickableTextureComponent? btnTransferFrom;
	public ClickableTextureComponent btnSearch;
	public ClickableTextureComponent? btnToggleEdit;
	public ClickableTextureComponent? btnSettings;
	public ClickableTextureComponent? btnToggleFavorites;
	public ClickableTextureComponent? btnToggleSeasoning;
	public ClickableTextureComponent? btnToggleQuality;
	public ClickableTextureComponent? btnToggleUniform;

	public ClickableTextureComponent? btnCatUp;
	public ClickableTextureComponent? btnCatDown;

	public ClickableTextureComponent? btnPageUp;
	public ClickableTextureComponent? btnPageDown;

	// Recipe Tracking
	protected List<string>? ListedRecipes;

	protected HashSet<IRecipe>? UnseenRecipes;

	protected List<IRecipe> Recipes = [];
	protected Dictionary<string, IRecipe> RecipesByName = [];
	protected List<IRecipe> Favorites = [];

	[SkipForClickableAggregation]
	protected Dictionary<IRecipe, ClickableTextureComponent> RecipeComponents = [];
	protected Dictionary<IRecipe, int> RecipeQuality = [];
	[SkipForClickableAggregation]
	protected Dictionary<ClickableTextureComponent, IRecipe> ComponentRecipes = [];

	protected readonly Dictionary<Item, IRecipe> RecipesByItem = new(ItemEqualityComparer.Instance);

	//Recipe State
	protected long CraftCachedAt = 0;
	protected Dictionary<IRecipe, bool>? CanCraftCache;

	// Tabs
	public ClickableTextureComponent? btnTabsUp;
	public ClickableTextureComponent? btnTabsDown;
	private int TabScroll = 0;

	private int tabIndex = 0;
	protected List<TabInfo> Tabs = [];
	protected TabInfo CurrentTab => (tabIndex >= 0 && Tabs.Count > tabIndex) ? Tabs[tabIndex] : Tabs[0];

	// Pagination
	private int pageIndex = 0;
	[SkipForClickableAggregation]
	protected List<List<ClickableTextureComponent>> Pages = [];
	protected List<ClickableTextureComponent> CurrentPage { get => pageIndex >= 0 && Pages.Count > pageIndex ? Pages[pageIndex] : Pages[0]; }

	// Search
	private string? Filter = null;
	private bool FilterIngredients = false;
	private bool FilterLikes = false;
	private bool FilterLoves = false;
	private Regex? FilterRegex = null;

	// Components for IClickableComponent
	public List<ClickableComponent> currentPageComponents = [];

	// Held Item
	protected Item? HeldItem;

	// Tooltip Nonsense
	internal ISimpleNode? hoverNode = null;
	internal string hoverTitle = "";
	internal string hoverText = "";
	internal Item? hoverItem = null;
	internal int hoverAmount = -1;
	public Item? HoveredItem = null;

	private readonly Dictionary<NPC, SpriteInfo> Heads = [];

	internal IRecipe? hoverRecipe = null;
	internal readonly Cache<Item?, string?> lastRecipeHover;

	internal IRecipe? CachedTipRecipe;
	internal bool CachedTipShifting;
	internal bool CachedTipCtrling;
	internal ISimpleNode? CachedTip;

	internal IRecipe? activeRecipe = null;

	/// <inheritdoc />
	public IRecipe? ActiveRecipe => activeRecipe ?? hoverRecipe;

	// Better Tooltip
	internal int hoverMode = -1;

	// Item Transfer Stuff
	public bool Working { get; private set; } = false;
	private readonly List<ItemGrabMenu.TransferredItemSprite> tSprites = [];

	// Pending Crafting
	private int craftingRemaining = 0;
	private int craftingSuccessful = 0;
	private bool craftingUsedAdditional = false;
	private Action<int>? craftingOnDone;

	private IList<IBCInventory>? craftingLocked;
	private bool[]? craftingModified;
	private Action? craftingOnDoneLocked;

	private bool craftingMoveResultsToInventory;
	private bool craftingPlaySound;

	// Recycling
	internal readonly Cache<(Item, IRecipe, IRecyclable[], IIngredient[])?, Item?> HeldItemRecyclable;

	public Texture2D RecyclingBinTexture;

	public bool Recycling { get; private set; } = true;

	// Themes
	public string ThemeId { get; }
	public Theme Theme { get; }

	public Texture2D? Background { get; private set; }

	public Texture2D? ButtonTexture { get; private set; }


	// Sprite Sources

	public Rectangle SourceTrashCan => Recycling ? new(0, 0, 18, 26) : new(564 + Game1.player.trashCanLevel * 18, 102, 18, 26);
	public Rectangle SourceTrashCanLid => Recycling ? new(0, 27, 18, 10) : new(564 + Game1.player.trashCanLevel * 18, 129, 18, 10);
	public Rectangle SourceIncludeInMisc => (CurrentTab?.Category?.IncludeInMisc ?? false) ? Sprites.Buttons.INCLUDE_MISC_ON : Sprites.Buttons.INCLUDE_MISC_OFF;
	public Rectangle SourceTransferTo => Sprites.Buttons.TO_INVENTORY;
	public Rectangle SourceTransferFrom => Sprites.Buttons.FROM_INVENTORY;
	public Rectangle SourceFilter => Filter == null ? Sprites.Buttons.SEARCH_OFF : Sprites.Buttons.SEARCH_ON;
	public Rectangle SourceEdit => Sprites.Buttons.WRENCH;
	public Rectangle SourceFavorites => FavoritesOnly ? Sprites.Buttons.FAVORITE_ON : Sprites.Buttons.FAVORITE_OFF;
	public Rectangle SourceCatFilter => (CurrentTab?.Category?.UseRules ?? false) ? Sprites.Buttons.FILTER_ON : Sprites.Buttons.FILTER_OFF;
	public Rectangle SourceSeasoning {
		get {
			return Mod.Config.UseSeasoning switch {
				SeasoningMode.Enabled => Sprites.Buttons.SEASONING_ON,
				SeasoningMode.InventoryOnly => Sprites.Buttons.SEASONING_LOCAL,
				_ => Sprites.Buttons.SEASONING_OFF,
			};
		}
	}
	public Rectangle SourceQuality {
		get {
			return Mod.Config.MaxQuality switch {
				MaxQuality.Iridium => Sprites.Buttons.QUALITY_3,
				MaxQuality.Gold => Sprites.Buttons.QUALITY_2,
				MaxQuality.Silver => Sprites.Buttons.QUALITY_1,
				_ => Sprites.Buttons.QUALITY_0,
			};
			;
		}
	}

	public int Quality {
		get {
			return Mod.Config.MaxQuality switch {
				MaxQuality.None => 0,
				MaxQuality.Silver => 1,
				MaxQuality.Gold => 2,
				_ => int.MaxValue,
			};
		}
	}

	public Rectangle SourceUniform => Mod.Config.UseUniformGrid ? Sprites.Buttons.UNIFORM_ON : Sprites.Buttons.UNIFORM_OFF;

	#region Lifecycle

	public static BetterCraftingPage Open(
		ModEntry mod,
		GameLocation? location = null,
		Vector2? position = null,
		Rectangle? area = null,
		int width = -1,
		int height = -1,
		bool cooking = false,
		bool standalone_menu = false,
		IList<LocatedInventory>? material_containers = null,
		bool discover_containers = true,
		int x = -1,
		int y = -1,
		bool silent_open = false,
		IEnumerable<string>? listed_recipes = null,
		bool discover_buildings = false,
		string? station = null,
		int? areaOverride = null
	) {
		if (width <= 0)
			width = 800 + borderWidth * 2;
		if (height <= 0 && standalone_menu && mod.Config.UseFullHeight)
			height = Math.Max(600 + borderWidth * 2, Game1.uiViewport.Height - borderWidth * 2);
		else if (height <= 0)
			height = 600 + borderWidth * 2;

		int rows = mod.GetBackpackRows(Game1.player);
		if (rows != 3) {
			// First, calculate the remaining height.
			int totalHeight = Game1.uiViewport.Height - height;
			if (y != -1)
				totalHeight -= y;

			int maxRows = 3 + totalHeight / (4 + Game1.tileSize);

			if (rows > maxRows)
				rows = maxRows;

			if (rows != 3)
				height += (4 + Game1.tileSize) * (rows - 3);
		}

		Vector2 pos = x == -1 || y == -1 ? Utility.getTopLeftPositionForCenteringOnScreen(width, height) : Vector2.Zero;
		if (x == -1) x = (int) pos.X;
		if (y == -1) y = (int) pos.Y;

		return new BetterCraftingPage(
			mod,
			x, y,
			width, height,

			location,
			position,
			area,

			cooking: cooking,
			standalone_menu: standalone_menu,
			silent_open: silent_open,
			discover_containers: discover_containers,
			discover_buildings: discover_buildings,

			material_containers: material_containers,
			listed_recipes: listed_recipes,
			rows: rows,
			station: station,
			area_override: areaOverride
		);
	}

	public static BetterCraftingPage Open(
		ModEntry mod,
		GameLocation? location = null,
		Vector2? position = null,
		Rectangle? area = null,
		int width = -1,
		int height = -1,
		bool cooking = false,
		bool standalone_menu = false,
		IList<object>? material_containers = null,
		bool discover_containers = true,
		int x = -1,
		int y = -1,
		bool silent_open = false,
		IEnumerable<string>? listed_recipes = null,
		bool discover_buildings = false,
		string? station = null,
		int? areaOverride = null
	) {
		var located = material_containers == null ? null : InventoryHelper.LocateInventories(
			material_containers,
			mod.GetLocations(),
			mod.GetInventoryProvider,
			location,
			true
		);

		return Open(
			mod: mod,
			location: location,
			position: position,
			area: area,
			width: width,
			height: height,
			x: x,
			y: y,
			cooking: cooking,
			standalone_menu: standalone_menu,
			silent_open: silent_open,
			discover_containers: discover_containers,
			discover_buildings: discover_buildings,
			material_containers: located,
			listed_recipes: listed_recipes,
			station: station,
			areaOverride: areaOverride
		);
	}

	public BetterCraftingPage(
		ModEntry mod,
		int x, int y,
		int width, int height,

		GameLocation? location,
		Vector2? position,
		Rectangle? area,

		bool cooking = false,
		bool standalone_menu = false,
		bool silent_open = false,
		bool discover_containers = true,
		bool discover_buildings = false,
		string? station = null,

		IList<LocatedInventory>? material_containers = null,
		IEnumerable<string>? listed_recipes = null,
		int? rows = null,
		int? area_override = null

	) : base(mod, x, y, width, height) {
		Stopwatch timer = Stopwatch.StartNew();

		BenchLocation = location ?? Game1.player.currentLocation;
		BenchPosition = position;
		BenchArea = area;
		this.cooking = cooking;
		Standalone = standalone_menu;
		DiscoverContainers = discover_containers;
		DiscoverBuildings = discover_buildings;
		DiscoverAreaOverride = area_override;

		MAX_TABS = (height - 120) / 64;
		VISIBLE_TABS = (height - 120) / 64;

		// If we have a station, get it loaded first.
		if (!string.IsNullOrEmpty(station)) {
			if (Mod.Stations.TryGetStation(station, Game1.player, out var stationData)) {
				Station = stationData;
				// Override cooking.
				this.cooking = Station.IsCooking;

			} else {
				// Invalid station. What should we do?
			}
		}

		// Theme Override
		ThemeId = Mod.ThemeManager.ActiveThemeId;
		Theme = Mod.ThemeManager.ActiveTheme;

		if (!string.IsNullOrEmpty(Station?.Theme)) {
			if (!Mod.ThemeManager.TryGetTheme(Station.Theme, out var theme))
				Log($"Unable to find theme '{Station.Theme}' for station '{station}'.", LogLevel.Warn);
			else {
				ThemeId = Station.Theme;
				Theme = theme;
			}
		}

		Log($"Initial settings after {timer.ElapsedMilliseconds}ms.", LogLevel.Trace);

		LoadTextures();

		Log($"Loaded textures after {timer.ElapsedMilliseconds}ms.", LogLevel.Trace);

		// Try to load a station name.
		TryLoadStationName();

		Log($"Loaded station info after {timer.ElapsedMilliseconds}ms.", LogLevel.Trace);

		// Run the event to populate containers.
		// TODO: Track which mod adds each container.
		bool do_disable = false;

		foreach (var api in ModEntry.Instance.APIInstances.Values)
			do_disable = api.EmitMenuPopulate(this, ref material_containers) | do_disable;

		Log($"Emitted populate after {timer.ElapsedMilliseconds}ms.", LogLevel.Trace);

		if (do_disable) {
			DiscoverContainers = false;
			DiscoverBuildings = false;
		}

		MaterialContainers = material_containers;

		if (Station != null) {
			ListedRecipes = new List<string>(Station.Recipes);

		} else if (listed_recipes != null) {
			if (listed_recipes is List<string> basic)
				ListedRecipes = basic;
			else
				ListedRecipes = new List<string>(listed_recipes);
		} else
			ListedRecipes = null;

		// If not given a specific list of recipes, then try loading them.
		ListedRecipes ??= Mod.Stations.GetNonExclusiveRecipes(cooking).Select(recipe => recipe.Name).ToList();

		Log($"Loaded listed recipes after {timer.ElapsedMilliseconds}ms.", LogLevel.Trace);

		ChestsOnly = this.cooking && Mod.intCSkill != null && Mod.intCSkill.IsLoaded;

		lastRecipeHover = new(key => hoverRecipe?.CreateItemSafe(CreateLog), () => hoverRecipe?.Name);

		HeldItemRecyclable = new(item => {
			if (item is null || !item.canBeTrashed() || (InventoryItems != null && InventoryItems.Contains(item)))
				return null;

			if (!RecipesByItem.TryGetValue(item, out IRecipe? recipe) || !recipe.AllowRecycling) {
				var one = item.getOne();
				while (true) {
					foreach (var entry in RecipesByItem) {
						if (entry.Value.AllowRecycling && ItemEqualityComparer.Instance.Equals(entry.Key, one)) {
							recipe = entry.Value;
							break;
						}
					}

					if (!Mod.Config.RecycleHigherQuality || recipe != null || one.Quality <= 0)
						break;

					one.Quality--;
				}
			}

			if (recipe is null || recipe.Ingredients is null)
				return null;

			if (!Mod.Config.EffectiveRecycleUnknownRecipes && !recipe.HasRecipe(Game1.player))
				return null;

			List<IRecyclable>? recyclable = null;
			List<IIngredient>? nonrecyclable = null;

			bool recycle_fuzzy = Mod.Config.EffectiveRecycleFuzzyItems;

			foreach (IIngredient ingredient in recipe.Ingredients) {
				if (ingredient is IRecyclable ing && ing.CanRecycle(Game1.player, item, recycle_fuzzy)) {
					recyclable ??= [];
					recyclable.Add(ing);
				} else {
					nonrecyclable ??= [];
					nonrecyclable.Add(ingredient);
				}
			}

			// If we get nothing from it, don't let them recycle it.
			if (recyclable is null || recyclable.Count == 0)
				return null;

			return (
				item,
				recipe,
				recyclable?.ToArray() ?? [],
				nonrecyclable?.ToArray() ?? []
			);

		}, () => HeldItem);

		Log($"Initialized lazy fields after {timer.ElapsedMilliseconds}ms.", LogLevel.Trace);

		Mod.Recipes.ClearGiftTastes();

		Log($"Cleared gift taste cache after {timer.ElapsedMilliseconds}ms.", LogLevel.Trace);

		// InventoryMenu
		int nRows = rows ?? Mod.GetBackpackRows(Game1.player);

		inventory = new InventoryMenu(
			xPositionOnScreen + spaceToClearSideBorder + borderWidth,
			yPositionOnScreen + height - nRows * 17 * 4 - borderWidth + 4,
			//yPositionOnScreen + spaceToClearTopBorder + borderWidth + 320 - 16,
			false,
			capacity: nRows * 12,
			rows: nRows
		) {
			showGrayedOutSlots = true
		};

		foreach (ClickableComponent cmp in inventory.GetBorder(InventoryMenu.BorderSide.Top))
			cmp.upNeighborID = ClickableComponent.SNAP_AUTOMATIC;

		foreach (ClickableComponent cmp in inventory.GetBorder(InventoryMenu.BorderSide.Bottom))
			cmp.downNeighborID = ClickableComponent.ID_ignore;

		foreach (ClickableComponent cmp in inventory.GetBorder(InventoryMenu.BorderSide.Left))
			cmp.leftNeighborID = ClickableComponent.SNAP_AUTOMATIC;

		foreach (ClickableComponent cmp in inventory.GetBorder(InventoryMenu.BorderSide.Right))
			cmp.rightNeighborID = ClickableComponent.SNAP_AUTOMATIC;


		// Close Button
		if (Standalone)
			initializeUpperRightCloseButton();


		// Buttons
		int btnX = xPositionOnScreen + width + 4;
		int btnY = yPositionOnScreen + 128;

		if (Mod.Config.UseTransfer) {
			btnTransferTo = new ClickableTextureComponent(
				bounds: new Rectangle(btnX, btnY, 64, 64),
				texture: ButtonTexture ?? Sprites.Buttons.Texture,
				sourceRect: SourceTransferTo,
				scale: 4f
			) {
				myID = 200,
				leftNeighborID = 100,
				rightNeighborID = ClickableComponent.ID_ignore
			};

			/*btnTransferFrom = new ClickableTextureComponent(
				bounds: new Rectangle(btnX, btnY, 64, 64),
				texture: ButtonTexture ?? Sprites.Buttons.Texture,
				sourceRect: SourceTransferFrom,
				scale: 4f
			) {
				myID = 201,
				leftNeighborID = 101,
				rightNeighborID = ClickableComponent.ID_ignore
			};*/
		}

		btnSearch = new ClickableTextureComponent(
			bounds: new Rectangle(btnX, btnY, 64, 64),
			texture: ButtonTexture ?? Sprites.Buttons.Texture,
			sourceRect: SourceFilter,
			scale: 4f
		) {
			myID = 100,
			leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			rightNeighborID = ClickableComponent.ID_ignore
		};

		bool use_categories = Mod.Config.UseCategories;
		if (Station != null)
			use_categories = false;

		btnToggleEdit = (use_categories && Mod.Config.ShowEditButton) ? new ClickableTextureComponent(
			bounds: new Rectangle(btnX, btnY, 64, 64),
			texture: ButtonTexture ?? Sprites.Buttons.Texture,
			sourceRect: SourceEdit,
			scale: 4f
		) {
			myID = 101,
			leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			rightNeighborID = ClickableComponent.ID_ignore
		} : null;

		btnSettings = (Mod.Config.ShowSettingsButton && Mod.HasGMCM() && (!Context.IsOnHostComputer || Context.IsMainPlayer)) ? new ClickableTextureComponent(
			bounds: new Rectangle(btnX, btnY, 64, 64),
			texture: ButtonTexture ?? Sprites.Buttons.Texture,
			sourceRect: Sprites.Buttons.SETTINGS,
			scale: 4f
		) {
			myID = 107,
			leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			rightNeighborID = ClickableComponent.ID_ignore
		} : null;

		btnToggleFavorites = Mod.Config.UseCategories ? null : new ClickableTextureComponent(
			bounds: new Rectangle(btnX, btnY, 64, 64),
			texture: ButtonTexture ?? Sprites.Buttons.Texture,
			sourceRect: SourceFavorites,
			scale: 4f
		) {
			myID = 102,
			leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			rightNeighborID = ClickableComponent.ID_ignore
		};

		btnToggleSeasoning = this.cooking ? new ClickableTextureComponent(
			bounds: new Rectangle(btnX, btnY, 64, 64),
			texture: ButtonTexture ?? Sprites.Buttons.Texture,
			sourceRect: SourceSeasoning,
			scale: 4f
		) {
			myID = 103,
			leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			rightNeighborID = ClickableComponent.ID_ignore
		} : null;

		btnToggleQuality = Mod.Config.MaxQuality == MaxQuality.Disabled ? null : new ClickableTextureComponent(
			bounds: new Rectangle(btnX, btnY, 64, 64),
			texture: ButtonTexture ?? Sprites.Buttons.Texture,
			sourceRect: SourceQuality,
			scale: 4f
		) {
			myID = 105,
			leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			rightNeighborID = ClickableComponent.ID_ignore
		};

		btnToggleUniform = this.cooking ? null : new ClickableTextureComponent(
			bounds: new Rectangle(btnX, btnY, 64, 64),
			texture: ButtonTexture ?? Sprites.Buttons.Texture,
			sourceRect: SourceUniform,
			scale: 4f
		) {
			myID = 104,
			leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			rightNeighborID = ClickableComponent.ID_ignore
		};

		// Trash Can

		trashCan = new ClickableTextureComponent(
			bounds: new Rectangle(btnX, btnY, 64, 104),
			texture: Recycling ? RecyclingBinTexture : Game1.mouseCursors,
			sourceRect: SourceTrashCan,
			scale: 4f
		) {
			myID = 106,
			leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			rightNeighborID = ClickableComponent.ID_ignore
		};

		// Layout the buttons

		GUIHelper.LinkComponents(
			GUIHelper.Side.Down,
			id => getComponentWithID(id),
			btnSearch,
			btnToggleEdit,
			btnToggleFavorites,
			btnToggleSeasoning,
			btnToggleQuality,
			btnToggleUniform,
			btnSettings,
			trashCan
		);

		GUIHelper.MoveComponents(
			GUIHelper.Side.Down, 16,
			btnSearch,
			btnToggleEdit,
			btnToggleFavorites,
			btnToggleSeasoning,
			btnToggleQuality,
			btnToggleUniform,
			btnSettings,
			trashCan
		);

		GUIHelper.LinkComponents(
			GUIHelper.Side.Right,
			getComponentWithID,
			btnSearch,
			btnTransferTo
		);

		GUIHelper.MoveComponents(
			GUIHelper.Side.Right, 16,
			btnSearch,
			btnTransferTo
		);

		GUIHelper.LinkComponents(
			GUIHelper.Side.Right,
			getComponentWithID,
			btnToggleEdit,
			btnTransferFrom
		);

		GUIHelper.LinkComponents(
			GUIHelper.Side.Down,
			getComponentWithID,
			btnTransferTo,
			btnTransferFrom
		);

		GUIHelper.MoveComponents(
			GUIHelper.Side.Down, 16,
			btnTransferTo,
			btnTransferFrom
		);

		Log($"Loaded base UI after {timer.ElapsedMilliseconds}ms.", LogLevel.Trace);

		// Initialize our state
		DiscoverInventories();

		Log($"Discovered inventories after {timer.ElapsedMilliseconds}ms.", LogLevel.Trace);

		DiscoverRecipes();

		Log($"Discovered recipes after {timer.ElapsedMilliseconds}ms.", LogLevel.Trace);

		UpdateTabs();

		Log($"Updated tabs after {timer.ElapsedMilliseconds}ms.", LogLevel.Trace);

		LayoutRecipes();

		Log($"Laid out recipes after {timer.ElapsedMilliseconds}ms.", LogLevel.Trace);

		// Final UX
		if (Standalone && !silent_open)
			Game1.playSound("bigSelect");

		if (Game1.options.SnappyMenus)
			snapToDefaultClickableComponent();

		// Install our hooks.
		if (CachedInventories != null)
			Mod.SpookyAction.WatchLocations(CachedInventories.Select(x => x.Location), Game1.player);

		// We are done.
		IsReady = true;

		timer.Stop();
		Log($"Menu initialized in {timer.ElapsedMilliseconds}ms.", LogLevel.Trace);
	}

	public void TryLoadStationName() {
		SpriteInfo? sprite = null;
		string? displayName = null;

		if (!string.IsNullOrEmpty(Station?.DisplayName))
			displayName = Station.DisplayName;

		if (Station?.Icon != null)
			sprite = GetSpriteFromIcon(Station.Icon);

		if ((sprite == null || displayName == null) && BenchLocation != null && BenchPosition != null) {
			// Check for fridge first.
			var fp = BenchLocation.GetFridgePosition();
			if (fp.HasValue && BenchPosition.Value.X == fp.Value.X && BenchPosition.Value.Y == fp.Value.Y) {
				sprite ??= SpriteHelper.GetSprite(ItemRegistry.Create("(BC)216"));
				displayName ??= I18n.Station_Kitchen();

			} else if (BenchLocation.GetObjectAtPosition(BenchPosition.Value, out var sobj)) {
				sprite ??= SpriteHelper.GetSprite(sobj);
				displayName ??= sobj.DisplayName;
			}
		}

		if (string.IsNullOrEmpty(displayName))
			return;

		var builder = SimpleHelper.Builder(LayoutDirection.Horizontal);

		if (sprite != null)
			builder = builder.Sprite(sprite, scale: 4, align: Alignment.VCenter, overrideHeight: 32);
		else
			builder = builder.Text(" ");

		StationLabel = builder
			.Text(displayName, align: Alignment.Bottom)
			.Text(" ")
			.GetLayout();
	}

	[MemberNotNull(nameof(RecyclingBinTexture))]
	public void LoadTextures() {
		RecyclingBinTexture = Mod.ThemeManager.Load<Texture2D>("recycle.png", ThemeId);
		if (Recycling && trashCan is not null)
			trashCan.texture = RecyclingBinTexture;

		if (Theme != Mod.ThemeManager.ActiveTheme)
			ButtonTexture = Mod.ThemeManager.Load<Texture2D>("buttons.png", ThemeId);

		if (Mod.ThemeManager.HasFile("background.png", ThemeId))
			Background = Mod.ThemeManager.Load<Texture2D>("background.png", ThemeId);
		else
			Background = null;
	}

	public IClickableMenu Menu => this;

	protected void ReleaseLocks() {
		if (UnsafeInventories == null)
			return;

		int stuck = 0;

		foreach (var inv in UnsafeInventories) {
			if (inv.Mutex is not null && inv.Mutex.IsLockHeld()) {
				stuck++;
				inv.Mutex.ReleaseLock();
			}
		}

		if (stuck > 0)
			Log($"Released {stuck} mutexes when closing.", LogLevel.Debug);
	}

	public override void emergencyShutDown() {
		base.emergencyShutDown();

		if (craftingSuccessful > 0 || craftingRemaining > 0) {
			craftingRemaining = 0;
			_PerformNextCraft();
		}

		if (CachedInventories != null)
			Mod.SpookyAction.UnwatchLocations(CachedInventories.Select(x => x.Location), Game1.player);
		ReleaseLocks();

		if (HeldItem != null) {
			Utility.CollectOrDrop(HeldItem);
			HeldItem = null;
		}

		foreach (var api in Mod.APIInstances.Values)
			api.EmitMenuClosing(this);
	}

	protected override void cleanupBeforeExit() {
		base.cleanupBeforeExit();

		if (craftingSuccessful > 0 || craftingRemaining > 0) {
			craftingRemaining = 0;
			_PerformNextCraft();
		}

		if (CachedInventories != null)
			Mod.SpookyAction.UnwatchLocations(CachedInventories.Select(x => x.Location), Game1.player);
		ReleaseLocks();

		if (Editing)
			SaveCategories();

		// Uncache our textures and the like. This might lead to some early
		// unloads when playing split screen, but it probably isn't a big enough
		// issue to care.
		foreach (var recipe in Recipes) {
			if (recipe is IRecipeWithCaching rwc)
				rwc.ClearCache();
		}

		foreach (var api in Mod.APIInstances.Values)
			api.EmitMenuClosing(this);
	}

	private void CreateLog(string message, LogLevel level = LogLevel.Debug, Exception? ex = null) {
		Log(message, level, ex);
	}

	#endregion

	#region Editing

	[MemberNotNullWhen(true, nameof(CurrentTab))]
	public bool IsMiscCategory =>
		CurrentTab?.Category?.Id == "miscellaneous";

	public void ToggleFilterMode() {
		if (!Editing || CurrentTab?.Category is null)
			return;

		CurrentTab.Category.UseRules = !CurrentTab.Category.UseRules;

		if (btnCategoryFilter is not null)
			btnCategoryFilter.sourceRect = SourceCatFilter;

		LayoutRecipes();
	}

	public void ToggleIncludeInMisc() {
		if (!Editing || CurrentTab?.Category is null)
			return;

		CurrentTab.Category.IncludeInMisc = !CurrentTab.Category.IncludeInMisc;

		if (btnCategoryIncludeInMisc is not null)
			btnCategoryIncludeInMisc.sourceRect = SourceIncludeInMisc;
	}

	public void ToggleEditMode() {
		if (Editing) {
			// Save any last state.
			UpdateCategoryName();
			SaveCategories();
		}

		if (Station != null) {
			Editing = false;
			return;
		}

		Editing = !Editing;

		// Rebuild the state from the ground up.
		DiscoverRecipes();
		UpdateTabs();
		LayoutRecipes();

		// Visibility
		if (btnTransferTo != null)
			btnTransferTo.visible = !Editing;
		if (btnTransferFrom != null)
			btnTransferFrom.visible = !Editing;
		if (btnToggleFavorites != null)
			btnToggleFavorites.visible = !Editing;
		if (btnToggleSeasoning != null)
			btnToggleSeasoning.visible = !Editing;
		if (btnToggleQuality != null)
			btnToggleQuality.visible = !Editing;
		if (trashCan != null)
			trashCan.visible = !Editing;

		// Buttons~
		if (Editing) {
			int x = BasePageX() + 72;
			int y = CraftingPageY() - 96;

			int txtWidth = width - 2 * (IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder) - 72 - 80 - 80;

			txtCategoryName = new TextBox(
				textBoxTexture: Game1.content.Load<Texture2D>("LooseSprites\\textBox"),
				 null,
				Game1.smallFont,
				Game1.textColor
			) {
				X = x,
				Y = y,
				Width = txtWidth,
				Text = CurrentTab.Category.Name ?? string.Empty
			};

			txtCategoryName.OnEnterPressed += sender => sender.Selected = false;
			txtCategoryName.OnTabPressed += sender => {
				sender.Selected = false;
				snapToDefaultClickableComponent();
			};

			btnCategoryName = new ClickableComponent(
				bounds: new Rectangle(x, y, txtWidth, 48),
				name: ""
			) {
				myID = 536,
				upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				leftNeighborID = 535,
				rightNeighborID = 537
			};

			btnCategoryIcon = new ClickableComponent(
				bounds: new Rectangle(
					BasePageX() + 12 - 16,
					yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 12,
					64, 64
				),
				name: ""
			) {
				myID = 535,
				upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = 536
			};

			btnCategoryFilter = new ClickableTextureComponent(
				bounds: new Rectangle(
					x + txtWidth + 16,
					y - 8,
					64, 64
				),
				texture: ButtonTexture ?? Sprites.Buttons.Texture,
				sourceRect: SourceCatFilter,
				scale: 4f
			) {
				myID = 537,
				upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				leftNeighborID = 536,
				rightNeighborID = 538
			};

			btnCategoryIncludeInMisc = new ClickableTextureComponent(
				bounds: new Rectangle(
					x + txtWidth + 16 + 64 + 16,
					y - 8,
					64, 64
				),
				texture: ButtonTexture ?? Sprites.Buttons.Texture,
				sourceRect: SourceIncludeInMisc,
				scale: 4f
			) {
				myID = 538,
				upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				leftNeighborID = 537,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC
			};

			btnCategoryCopy = new ClickableTextureComponent(
				bounds: new Rectangle(0, 0, 64, 64),
				texture: ButtonTexture ?? Sprites.Buttons.Texture,
				sourceRect: Sprites.Buttons.COPY,
				scale: 4f
			) {
				myID = 539,
				upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC
			};

			btnCategoryPaste = new ClickableTextureComponent(
				bounds: new Rectangle(0, 0, 64, 64),
				texture: ButtonTexture ?? Sprites.Buttons.Texture,
				sourceRect: Sprites.Buttons.PASTE,
				scale: 4f
			) {
				myID = 540,
				upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC
			};

			btnCategoryTrash = new ClickableTextureComponent(
				bounds: new Rectangle(0, 0, 64, 64),
				texture: ButtonTexture ?? Sprites.Buttons.Texture,
				sourceRect: Sprites.Buttons.TRASH,
				scale: 4f
			) {
				myID = 541,
				upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC
			};

			bool custom_scroll = Theme.CustomScroll && Background is not null;

			btnCatUp = new ClickableTextureComponent(
				new Rectangle(0, 0, 32, 32),
				custom_scroll ? Background : Game1.mouseCursors,
				custom_scroll
					? Sprites.CustomScroll.PAGE_UP
					: Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 12),
				custom_scroll
					? 2f
					: 0.5f
			) {
				myID = 997,
				downNeighborID = 998,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			};

			btnCatDown = new ClickableTextureComponent(
				new Rectangle(0, 0, 32, 32),
				custom_scroll ? Background : Game1.mouseCursors,
				custom_scroll
					? Sprites.CustomScroll.PAGE_DOWN
					: Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 11),
				custom_scroll
					? 2f
					: 0.5f
			) {
				myID = 998,
				upNeighborID = 997,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			};

			Flow = new(
				this,
				BasePageX() - 12,
				CraftingPageY(),
				432 - 8,
				height - 248,
				firstID: 10000
			);

			if (Theme.CustomScroll && Background is not null)
				Sprites.CustomScroll.ApplyToScrollableFlow(Flow, Background);

			btnFlowUp = Flow.btnPageUp;
			btnFlowDown = Flow.btnPageDown;
			FlowComponents = Flow.DynamicComponents;

			PositionTabMoveButtons();
			UpdateFlow();

			populateClickableComponentList();

			GUIHelper.LinkComponents(
				GUIHelper.Side.Right,
				getComponentWithID,
				btnSearch,
				btnCategoryCopy
			);

			GUIHelper.LinkComponents(
				GUIHelper.Side.Right,
				getComponentWithID,
				btnToggleEdit,
				btnCategoryPaste
			);

			GUIHelper.MoveComponents(
				GUIHelper.Side.Right, 16,
				btnSearch,
				btnCategoryCopy
			);

			GUIHelper.MoveComponents(
				GUIHelper.Side.Down, 16,
				btnCategoryCopy,
				btnCategoryPaste,
				btnCategoryTrash
			);

		} else {
			txtCategoryName = null;
			btnCategoryName = null;
			btnCategoryIcon = null;
			btnCategoryFilter = null;
			btnCategoryIncludeInMisc = null;
			btnCategoryCopy = null;
			btnCategoryPaste = null;
			btnCategoryTrash = null;
			btnCatUp = null;
			btnCatDown = null;
			FlowComponents = null;
			Flow = null;

			populateClickableComponentList();

			GUIHelper.LinkComponents(
				GUIHelper.Side.Right,
				getComponentWithID,
				btnSearch,
				btnTransferTo
			);

			GUIHelper.LinkComponents(
				GUIHelper.Side.Right,
				getComponentWithID,
				btnToggleEdit,
				btnTransferFrom
			);
		}
	}

	public void UpdateFlow() {
		if (Flow is null)
			return;

		Category? cat = CurrentTab?.Category;
		if (cat is null)
			return;

		cat.CachedRules ??= Mod.Recipes.HydrateDynamicRules(cat.DynamicRules);

		var types = cat.CachedRules;
		var builder = FlowHelper.Builder();

		bool added = false;

		if (types is not null)
			for (int i = 0; i < types.Length; i++) {
				IDynamicRuleHandler handler = types[i].Item1;
				object? state = types[i].Item2;
				DynamicRuleData data = types[i].Item3;

				Texture2D texture;
				Rectangle source;

				if (handler is IDynamicIconRuleHandler dicon) {
					texture = dicon.GetTexture(state);
					source = dicon.GetSource(state);
				} else {
					texture = handler.Texture;
					source = handler.Source;
				}

				float scale = 32f / source.Height;
				if (scale >= 1)
					scale = MathF.Round(scale);

				int index = i;

				var sb = FlowHelper.Builder()
					.Texture(
						Game1.mouseCursors,
						new Rectangle(227 + (data.Inverted ? 0 : 9), 425, 9, 9),
						scale: 2,
						align: Alignment.VCenter
					)
					.Text(" ")
					.Texture(
						texture,
						source,
						scale: scale,
						align: Alignment.VCenter
					)
					.Text(" ")
					.FormatText(handler.DisplayName, align: Alignment.VCenter);

				var extra = handler is IExtraInfoRuleHandler info ? info.GetExtraInfo(state) : null;
				if (extra is not null)
					sb.Text("\n").AddRange(extra);

				var node = new Common.UI.FlowNode.SelectableNode(
					sb.Build(),
					onClick: (_, _, _) => {
						if (OpenRuleEditor(cat, index))
							Game1.playSound("bigSelect");
						return false;
					}
				) {
					SelectedTexture = ButtonTexture ?? Sprites.Buttons.Texture,
					SelectedSource = Sprites.Buttons.SELECT_BG,
					HoverTexture = ButtonTexture ?? Sprites.Buttons.Texture,
					HoverSource = Sprites.Buttons.SELECT_BG,
					HoverColor = Color.White * 0.4f
				};

				builder.Add(node);
				added = true;
			}

		if (added)
			builder.Divider(size: 1, shadowOffset: 1);

		var sb2 = FlowHelper.Builder()
			.Texture(Game1.mouseCursors, new Rectangle(0, 428, 10, 10), 4f, align: Alignment.VCenter)
			.Text(" ")
			.FormatText(I18n.Filter_AddNew(), align: Alignment.VCenter);

		var node2 = new Common.UI.FlowNode.SelectableNode(
			sb2.Build(),
			onClick: (_, _, _) => {
				if (OpenRulePicker(cat))
					Game1.playSound("bigSelect");
				return false;
			}
		) {
			SelectedTexture = ButtonTexture ?? Sprites.Buttons.Texture,
			SelectedSource = Sprites.Buttons.SELECT_BG,
			HoverTexture = ButtonTexture ?? Sprites.Buttons.Texture,
			HoverSource = Sprites.Buttons.SELECT_BG,
			HoverColor = Color.White * 0.4f
		};

		builder.Add(node2);

		builder.Text("\n\n");

		Flow.Set(builder.Build());
	}

	public void ResetCategories() {
		if (Station != null)
			return;

		ConfirmationDialog? dialog = null;

		void OnConfirm(Farmer who) {
			SetChildMenu(null);

			Mod.Recipes.SetCategories(Game1.player, null, cooking);
			Mod.Recipes.SaveCategories();
			UpdateTabs();
			LayoutRecipes();
			Game1.playSound("trashcan");
		}

		void OnCancel(Farmer who) {
			SetChildMenu(null);
		}

		dialog = new ConfirmationDialog(
			I18n.Tooltip_TrashAll_Confirm(),
			OnConfirm,
			OnCancel
		);

		SetChildMenu(dialog);
	}

	public void DeleteCategory() {
		if (Station != null)
			return;

		Category? cat = CurrentTab?.Category;
		if (cat is null || cat.Id == "miscellaneous")
			return;

		ConfirmationDialog? dialog = null;

		void OnConfirm(Farmer who) {
			SetChildMenu(null);

			// Save the categories, excluding this one.
			var categories = Tabs
				.Select(val => val.Category)
				.Where(val => val.Id == "miscellaneous" || (val?.Recipes?.Count ?? 0) > 0 || (val?.DynamicRules?.Count ?? 0) > 0)
				.Where(val => val != cat);

			Mod.Recipes.SetCategories(Game1.player, categories, cooking);
			Mod.Recipes.SaveCategories();
			UpdateTabs();
			LayoutRecipes();
			Game1.playSound("trashcan");
		}

		void OnCancel(Farmer who) {
			SetChildMenu(null);
		}

		dialog = new ConfirmationDialog(
			I18n.Tooltip_TrashCat_Confirm(Mod.Recipes.GetCategoryDisplayName(cat, cooking)),
			OnConfirm,
			OnCancel
		);

		SetChildMenu(dialog);
	}

	public bool PasteCategory(Category newCat) {
		if (Station != null || newCat is null)
			return false;

		var existing = Tabs
			.Select(val => val.Category)
			.Where(val => val.Id == "miscellaneous" || (val?.Recipes?.Count ?? 0) > 0 || (val?.DynamicRules?.Count ?? 0) > 0)
			.ToList();

		foreach (var cat in existing) {
			if (newCat.Id == cat.Id) {
				newCat.Id = Guid.NewGuid().ToString();
				break;
			}
		}

		newCat.Name ??= "New Category";

		int i = 1;
		string name;
		while (true) {
			bool matched = false;
			name = i < 2 ? newCat.Name : $"{newCat.Name} ({i})";

			foreach (var cat in existing) {
				if (cat.Name == name) {
					matched = true;
					break;
				}
			}

			if (matched)
				i++;
			else {
				if (i >= 2) {
					newCat.Name = name;
					newCat.I18nKey = "";
				}
				break;
			}
		}

		// Insert before the current tab.
		int idx = tabIndex + 1;
		if (idx > existing.Count)
			idx = existing.FindIndex(cat => cat.Id == "miscellaneous");
		if (idx == -1)
			existing.Add(newCat);
		else
			existing.Insert(idx, newCat);

		Mod.Recipes.SetCategories(Game1.player, existing, cooking);
		Mod.Recipes.SaveCategories();
		UpdateTabs();

		idx = Tabs?.FindIndex(tab => tab.Category.Id == newCat.Id) ?? -1;
		if (idx != -1)
			SetTab(idx);
		else
			LayoutRecipes();

		return true;
	}



	public void SaveCategories() {
		var categories = Tabs
			.Select(val => val.Category)
			.Where(val => val.Id == "miscellaneous" || (val?.Recipes?.Count ?? 0) > 0 || (val?.DynamicRules?.Count ?? 0) > 0);

		// Categories are not editable for stations yet.
		// TODO: Update this when adding editing for stations.
		if (Station != null)
			return;

		Mod.Recipes.SetCategories(Game1.player, categories, cooking);
		Mod.Recipes.SaveCategories();
	}

	public SpriteInfo? GetSpriteFromIcon(CategoryIcon? icon, List<IRecipe>? recipes = null) {
		if (icon == null)
			return null;

		switch (icon.Type) {
			case CategoryIcon.IconType.Item:
				if (!string.IsNullOrEmpty(icon.ItemId))
					return SpriteHelper.GetSprite(ItemRegistry.Create(icon.ItemId));

				string? name = icon.RecipeName;
				if (!string.IsNullOrEmpty(name)) {
					RecipesByName.TryGetValue(name, out IRecipe? recipe);

					if (recipe is IDynamicDrawingRecipe ddr)
						return new DynamicRecipeSpriteInfo(ddr);

					else if (recipe != null && recipe.CreateItemSafe(CreateLog) is Item itm)
						return SpriteHelper.GetSprite(itm);

					else if (recipe != null && recipe.Texture != null)
						return new SpriteInfo(
							recipe.Texture,
							recipe.SourceRectangle
						);
				}

				if (recipes != null && recipes.Count > 0) {
					if (recipes[0].CreateItemSafe(CreateLog) is Item item)
						return SpriteHelper.GetSprite(item);
					else if (recipes[0].Texture != null)
						return new SpriteInfo(
							recipes[0].Texture,
							recipes[0].SourceRectangle
						);
				}

				return new SpriteInfo(
					Game1.mouseCursors,
					new Rectangle(173, 423, 16, 16)
				);

			case CategoryIcon.IconType.Texture:
				Texture2D? texture = icon.Source.HasValue ?
					SpriteHelper.GetTexture(icon.Source.Value)
					: null;

				if (!string.IsNullOrEmpty(icon.Path))
					try {
						//texture = Mod.Helper.Content.Load<Texture2D>(icon.Path, ContentSource.GameContent) ?? texture;
						texture = Mod.Helper.GameContent.Load<Texture2D>(icon.Path) ?? texture;
					} catch (Exception ex) {
						Log($"Unable to load texture \"{icon.Path}\" for category icon", LogLevel.Warn, ex);
					}

				if (texture != null) {
					Rectangle rect = icon.Rect ?? texture.Bounds;
					return new SpriteInfo(
						texture,
						rect,
						baseScale: icon.Scale,
						baseFrames: icon.Frames
					);
				}

				break;
		}

		return null;
	}

	public void UpdateCategorySprite(CategoryIcon icon) {
		if (icon == null)
			return;

		CurrentTab.Category.Icon = icon;
		CurrentTab.Sprite = GetSpriteFromIcon(icon);
	}

	public void UpdateCategorySprite(IRecipe recipe) {
		if (!Editing || CurrentTab == null || recipe == null)
			return;

		UpdateCategorySprite(new CategoryIcon() {
			Type = CategoryIcon.IconType.Item,
			RecipeName = recipe.Name
		});
	}

	public void UpdateCategoryName() {
		if (!Editing || txtCategoryName == null || CurrentTab == null)
			return;

		string name = txtCategoryName.Text.Trim();
		if (name.Equals(CurrentTab.Component.label))
			return;

		CurrentTab.Category.Name = name;
		CurrentTab.Category.I18nKey = "";
		CurrentTab.Component.label = name;
	}

	public void UpdateRecipeInCategory(IRecipe recipe, bool? present = null) {
		if (!Editing || CurrentTab == null || recipe == null || CurrentTab.Category is null)
			return;

		Category cat = CurrentTab.Category;
		cat.Recipes ??= [];

		string name = recipe.Name;
		bool wanted = present ?? !cat.Recipes.Contains(name);

		if (wanted)
			cat.Recipes.Add(name);
		else
			cat.Recipes.Remove(name);

		// Update the sprite, maybe.
		if (cat.Icon?.Type == CategoryIcon.IconType.Item && string.IsNullOrEmpty(cat.Icon.RecipeName)) {
			SpriteInfo? sprite = null;

			if (cat.Recipes.Count > 0) {
				if (RecipesByName.TryGetValue(cat.Recipes.First(), out IRecipe? val) && val.CreateItemSafe(CreateLog) is Item item)
					sprite = SpriteHelper.GetSprite(item);
			}

			sprite ??= new SpriteInfo(
					Game1.mouseCursors,
					new Rectangle(173, 423, 16, 16)
				);

			CurrentTab.Sprite = sprite;
		}

		PositionTabMoveButtons();
	}

	public bool IsRecipeInCategory(IRecipe recipe) {
		Category? cat = CurrentTab?.Category;
		if (cat is null)
			return false;

		if (cat.UseRules) {
			if (cat.CachedHiddenRecipes is not null && cat.CachedHiddenRecipes.Contains(recipe))
				return false;

			if (cat.CachedRecipes is not null && cat.CachedRecipes.Contains(recipe))
				return true;
		}

		return cat.Recipes is not null && cat.Recipes.Contains(recipe.Name);
	}

	#endregion

	#region Recipes and Inventory

	public virtual IReadOnlyList<string>? GetListedRecipes() {
		return ListedRecipes;
	}

	public virtual void UpdateListedRecipes(IEnumerable<string>? recipes) {
		if (recipes != null) {
			if (recipes is List<string> basic)
				ListedRecipes = basic;
			else
				ListedRecipes = new List<string>(recipes);
		} else
			ListedRecipes = null;

		// Rebuild the state from the ground up.
		DiscoverRecipes();
		UpdateTabs();
		LayoutRecipes();
	}

	public int GetRecipeQuality(IRecipe recipe) {
		if (RecipeQuality.TryGetValue(recipe, out int quality))
			return quality;

		var item = recipe.CreateItemSafe();
		quality = item is null ? 0 : item.Quality;
		RecipeQuality[recipe] = quality;
		return quality;
	}

	protected virtual void DiscoverRecipes() {
		Recipes.Clear();
		RecipesByName.Clear();
		RecipesByItem.Clear();
		Favorites.Clear();
		RecipeQuality.Clear();

		bool update_unseen = UnseenRecipes is null;
		UnseenRecipes ??= [];

		bool want_unknown = cooking
			? !Mod.Config.HideUnknown
			: Mod.Config.DisplayUnknownCrafting;

		foreach (IRecipe recipe in Mod.Recipes.GetRecipes(cooking)) {
			Item? item = recipe.CreateItemSafe(CreateLog);
			if (item is not null)
				RecipesByItem.TryAdd(item, recipe);

			if (!Editing && (Station == null || !Station.DisplayUnknownRecipes) && !recipe.HasRecipe(Game1.player) && !want_unknown)
				continue;

			if (!Editing && ListedRecipes != null && !ListedRecipes.Contains(recipe.Name))
				continue;

			if (RecipesByName.ContainsKey(recipe.Name))
				continue;

			Recipes.Add(recipe);
			RecipesByName.Add(recipe.Name, recipe);

			if (!Mod.Recipes.HasSeenRecipe(Game1.player, recipe))
				UnseenRecipes.Add(recipe);

			if (Mod.Favorites.IsFavoriteRecipe(recipe.Name, cooking, Game1.player))
				Favorites.Add(recipe);
		}

		BuildRecipeComponents();
	}

	protected bool MoveCurrentCategory(int direction) {
		if (CurrentTab is null)
			return false;

		int newIdx = tabIndex + direction;
		if (newIdx < 0 || newIdx > (Tabs.Count - 1))
			return false;

		var tab = CurrentTab;

		Tabs.RemoveAt(tabIndex);
		Tabs.Insert(newIdx, tab);

		SaveCategories();

		tabIndex = newIdx;

		if (!CenterTab(tabIndex))
			UpdateTabs();

		if (Game1.options.SnappyMenus)
			snapCursorToCurrentSnappedComponent();

		return true;
	}

	protected void UpdateTabs() {
		string? oldTab = Tabs.Count > tabIndex ? Tabs[tabIndex].Category.Id : LastTab;

		Tabs.Clear();

		Dictionary<Category, List<IRecipe>> categories = [];
		List<IRecipe> unused = Recipes.ToList();
		Category? misc = null;

		int count = 1;

		bool use_categories = Mod.Config.UseCategories;
		if (Station != null && Station.Categories == null)
			use_categories = false;

		// Only check categories if categories are enabled.
		// Otherwise, everything should go into misc.
		if (use_categories) {
			// We start at 1, to leave room for the Favorites tab.

			// Only show the favorites tab if we have favorites, or
			// if the existing visible tab was favorites.
			if (!Editing && (Favorites.Count > 0 || "favorites".Equals(oldTab))) {
				categories.Add(new Category() {
					Id = "favorites",
					Name = "Favorites",
					I18nKey = "category.favorites",
					Icon = new CategoryIcon() {
						Type = CategoryIcon.IconType.Texture,
						Source = GameTexture.MouseCursors,
						Rect = new Rectangle(338, 400, 8, 8)
					}
				}, Favorites);
			}

			Category[]? rawCats;
			// TODO: Load customized station categories
			if (Station?.Categories != null)
				rawCats = Station.Categories;
			else
				rawCats = Mod.Recipes.GetCategories(Game1.player, cooking);

			foreach (Category cat in rawCats) {
				if (misc == null && (cat.Id == "misc" || cat.Id == "miscellaneous"))
					misc = cat;

				// We continue rather than break in case there is a valid
				// misc tab to use.
				//if (count > (misc == cat ? 7 : 6))
				//	continue;

				cat.CachedRecipes = null;
				cat.CachedRules = null;

				List<IRecipe> recipes = [];

				if (cat.UseRules) {
					cat.CachedRules = Mod.Recipes.HydrateDynamicRules(cat.DynamicRules);

					if (cat.CachedRules is not null) {
						foreach (IRecipe recipe in Recipes) {
							Lazy<Item?> result = new(() => recipe.CreateItemSafe(CreateLog));

							bool matched = false;

							foreach (var handler in cat.CachedRules) {
								if (handler.Item3.Inverted) {
									if (matched && handler.Item1.DoesRecipeMatch(recipe, result, handler.Item2))
										matched = false;

								} else if (!matched && handler.Item1.DoesRecipeMatch(recipe, result, handler.Item2))
									matched = true;
							}

							if (matched) {
								recipes.Add(recipe);
								if (!cat.IncludeInMisc)
									unused.Remove(recipe);
							}
						}
					}

					// Make a copy of the list, so we won't modify it with
					// recipes that aren't rules-based.
					cat.CachedRecipes = new(recipes);
				}

				if (/*!cat.UseRules &&*/ cat.Recipes is not null)
					foreach (string name in cat.Recipes) {
						if (!RecipesByName.TryGetValue(name, out IRecipe? recipe))
							continue;

						if (!recipes.Contains(recipe)) {
							recipes.Add(recipe);
							if (!cat.IncludeInMisc)
								unused.Remove(recipe);
						}
					}

				if (Editing || recipes.Count > 0) {
					count++;
					categories.Add(cat, recipes);
				}
			}
		}

		// If we're editing and don't have enough categories, add a bunch.
		if (Editing && Mod.Config.UseCategories) {
			count++;

			// If we have misc, pop it off so it's last.
			List<IRecipe>? miscRecipes;
			if (misc != null) {
				categories.TryGetValue(misc, out miscRecipes);
				categories.Remove(misc);
			} else
				miscRecipes = null;

			categories.Add(new Category() {
				Id = Guid.NewGuid().ToString(),
				Name = "New Category",
				I18nKey = "",
				Icon = new CategoryIcon() {
					Type = CategoryIcon.IconType.Item
				},
				Recipes = []
			}, []);

			if (misc != null && miscRecipes != null)
				categories.Add(misc, miscRecipes);
		}

		// Add any remaining, uncategorized items to a Misc. category.
		// Also ensure we always have at least one category, in case we
		// somehow entered into a broken state with no recipes.
		if (Editing || unused.Count > 0 || categories.Count == 0) {
			if (misc == null)
				categories.Add(new Category() {
					Id = "miscellaneous",
					Name = "Miscellaneous",
					I18nKey = "category.misc",
					Icon = new CategoryIcon() {
						Type = CategoryIcon.IconType.Texture,
						Source = GameTexture.MouseCursors,
						Rect = cooking
							? new Rectangle(0, 428, 10, 10)
							: new Rectangle(50, 428, 10, 10)
					}
				}, unused);
			else {
				if (categories.ContainsKey(misc))
					categories[misc].AddRange(unused);
				else
					categories.Add(misc, unused);
			}
		}

		// Build the components
		int idx = 0;
		//int offsetY = 120;

		foreach (KeyValuePair<Category, List<IRecipe>> entry in categories) {
			Category cat = entry.Key;
			SpriteInfo? sprite = GetSpriteFromIcon(cat.Icon, entry.Value);

			ClickableComponent tab = new(
				bounds: Rectangle.Empty,
				name: entry.Key.Id,
				label: Mod.Recipes.GetCategoryDisplayName(entry.Key, cooking)
			) {
				myID = 1000 + idx,
				upNeighborID = (idx == 0) ? ClickableComponent.ID_ignore : 1000 + (idx - 1),
				downNeighborID = (idx >= categories.Count - 1) ? ClickableComponent.ID_ignore : 1000 + (idx + 1),
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC
			};

			Tabs.Add(new TabInfo() {
				Category = entry.Key,
				Sprite = sprite,
				Component = tab,
				Recipes = entry.Value,
				FilteredRecipes = Filter == null ?
					entry.Value :
					entry.Value.Where(DoesRecipeMatchFilter).ToList()
			});

			idx++;
		}

		if (tabIndex >= Tabs.Count)
			tabIndex = Tabs.Count - 1;
		else if (tabIndex < 0)
			tabIndex = 0;

		// If the current tab has changed, try to return to the old one.
		string? newTab = CurrentTab.Category.Id;
		if (!string.IsNullOrEmpty(oldTab) && !oldTab.Equals(newTab)) {
			for (int i = 0; i < Tabs.Count; i++) {
				if (oldTab.Equals(Tabs[i].Category.Id)) {
					newTab = oldTab;
					tabIndex = i;
					break;
				}
			}
		}

		// If we did change tabs, reset the current page.
		if (string.IsNullOrEmpty(oldTab) || !oldTab.Equals(newTab))
			pageIndex = 0;

		// Make sure the text component is up to date, if we're editing.
		if (txtCategoryName != null)
			txtCategoryName.Text = CurrentTab.Component.label;

		// Add our buttons or not
		if (Tabs.Count > MAX_TABS) {
			btnTabsUp = new ClickableTextureComponent(
				bounds: Rectangle.Empty,
				texture: Background ?? Game1.mouseCursors,
				sourceRect: Background is null
					? Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 12)
					: Sprites.CustomScroll.PAGE_UP,
				scale: Background is null
					? 0.8f
					: 3.2f
			) {
				myID = 999,
				upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				downNeighborID = 1000,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC
			};

			btnTabsDown = new ClickableTextureComponent(
				bounds: Rectangle.Empty,
				texture: Background ?? Game1.mouseCursors,
				sourceRect: Background is null
					? Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 11)
					: Sprites.CustomScroll.PAGE_DOWN,
				scale: Background is null
					? 0.8f
					: 3.2f
			) {
				myID = 1001 + Tabs.Count,
				upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC
			};

		} else {
			btnTabsDown = null;
			btnTabsUp = null;
		}

		PositionTabs();
	}

	public void PositionTabs() {
		int offsetY = 120;

		if (btnTabsUp != null) {
			btnTabsUp.bounds = new Rectangle(
				xPositionOnScreen - 48,
				yPositionOnScreen + offsetY - 64,
				64, 64
			);
			//offsetY += 64;
		} else
			offsetY = 120;

		for (int i = 0; i < Tabs.Count; i++) {
			var entry = Tabs[i];
			ClickableComponent? cmp = entry?.Component;
			if (cmp == null)
				continue;

			if (btnTabsUp != null && (i < TabScroll || i >= (TabScroll + VISIBLE_TABS))) {
				cmp.visible = false;
				continue;
			}

			cmp.visible = true;
			cmp.bounds = new Rectangle(
				xPositionOnScreen - 48,
				yPositionOnScreen + offsetY,
				64, 64
			);

			offsetY += 64;
		}

		if (btnTabsDown != null) {
			btnTabsDown.bounds = new Rectangle(
				xPositionOnScreen - 48,
				yPositionOnScreen + offsetY,
				64, 64
			);
		}

		PositionTabMoveButtons();
	}

	public void PositionTabMoveButtons() {
		if (!Editing || btnCatUp is null || btnCatDown is null)
			return;

		Rectangle? selected = null;
		bool had_subsequent_moveables = false;

		for (int i = 0; i < Tabs.Count; i++) {
			var entry = Tabs[i];
			var cat = entry?.Category;
			ClickableComponent? cmp = entry?.Component;
			if (cmp is null || cat is null || !cmp.visible)
				continue;

			// Do not allow repositioning the Misc category.
			if (cat.Id == "miscellaneous")
				continue;

			// Do not allow repositioning empty categories, since they won't save
			// and everything will get screwed up.
			if ((cat.Recipes?.Count ?? 0) == 0 && (cat.DynamicRules?.Count ?? 0) == 0)
				continue;

			if (selected.HasValue)
				had_subsequent_moveables = true;
			else if (i == tabIndex)
				selected = cmp.bounds;
		}

		if (!selected.HasValue) {
			btnCatUp.visible = false;
			btnCatDown.visible = false;
			return;
		}

		btnCatUp.visible = tabIndex > 0;
		btnCatDown.visible = had_subsequent_moveables;

		// We have two buttons to center, vertically.
		int total_height = btnCatUp.bounds.Height + btnCatDown.bounds.Height + 4;
		int tab_height = selected.Value.Height;

		int offset_y = (tab_height - total_height) / 2;

		btnCatUp.bounds = new Rectangle(
			selected.Value.X - btnCatUp.bounds.Width - 4,
			selected.Value.Y + offset_y,
			btnCatUp.bounds.Width, btnCatUp.bounds.Height
		);

		btnCatDown.bounds = new Rectangle(
			selected.Value.X - btnCatDown.bounds.Width - 4,
			selected.Value.Y + offset_y + btnCatUp.bounds.Height + 4,
			btnCatDown.bounds.Width, btnCatDown.bounds.Height
		);
	}

	public bool ScrollTabs(int direction) {
		if (Tabs.Count <= MAX_TABS || direction == 0)
			return false;

		int old = TabScroll;
		TabScroll += (direction > 0) ? 1 : -1;
		if (TabScroll < 0)
			TabScroll = 0;
		if (TabScroll > (Tabs.Count - VISIBLE_TABS))
			TabScroll = Tabs.Count - VISIBLE_TABS;

		if (TabScroll == old)
			return false;

		PositionTabs();
		return true;
	}

	public bool CenterTab(int index) {
		if (Tabs.Count <= MAX_TABS)
			return false;

		int old = TabScroll;
		TabScroll = index - (VISIBLE_TABS / 2);
		if (TabScroll < 0)
			TabScroll = 0;
		if (TabScroll > (Tabs.Count - VISIBLE_TABS))
			TabScroll = Tabs.Count - VISIBLE_TABS;

		if (TabScroll == old)
			return false;

		UpdateTabs();
		return true;
	}


	protected virtual void DiscoverInventories() {

		Func<object, IInventoryProvider?> provider = ChestsOnly
			? (obj => obj is Chest ? Mod.GetInventoryProvider(obj) : null)
			: Mod.GetInventoryProvider;

		// So, we may or may not call DiscoverInventories. And we may or
		// may not call it with limited settings.

		// We never call DiscoverInventories if the API call told us not to.
		// We may call it if the user has it disabled.

		// If we have a location, and no material_containers, and were not
		// told to disable discovery by the API, we will at a minimum scan
		// the eight adjacent tiles similarly to how default workbenches
		// work.

		// We also have to use separate calls if we have an area rather
		// than just a tile position.

		// Sanity check the list of material containers. There are mods
		// passing garbage data around, so we use a try/catch to avoid
		// our menu breaking entirely.
		int count;

		try {
			count = MaterialContainers?.Count ?? 0;
		} catch (Exception ex) {
			Log("We received a bad material container list. Ignoring.", LogLevel.Warn, ex);
			count = 0;
		}

		// We want to locate all our inventories.
		if (count == 0) {
			if (DiscoverContainers && BenchLocation != null) {
				if (BenchArea.HasValue) {
					if (Mod.Config.UseDiscovery)
						CachedInventories = InventoryHelper.DiscoverInventories(
							BenchArea.Value,
							BenchLocation,
							Game1.player,
							provider,
							Mod.IsValidConnector,
							distanceLimit: Mod.Config.MaxDistance,
							scanLimit: Mod.Config.MaxCheckedTiles,
							targetLimit: Mod.Config.MaxInventories,
							includeSource: true,
							includeDiagonal: Mod.Config.UseDiagonalConnections,
							includeBuildings: DiscoverBuildings,
							expandSource: Mod.Config.MaxWorkbenchGap
						);
					else
						CachedInventories = InventoryHelper.DiscoverInventories(
							BenchArea.Value,
							BenchLocation,
							Game1.player,
							provider,
							null,
							distanceLimit: 1,
							scanLimit: 25,
							targetLimit: 9,
							includeSource: true,
							includeDiagonal: true,
							includeBuildings: DiscoverBuildings
						);

				} else if (BenchPosition.HasValue) {
					var pos = new AbsolutePosition(BenchLocation, BenchPosition.Value);
					if (Mod.Config.UseDiscovery)
						CachedInventories = InventoryHelper.DiscoverInventories(
							pos,
							Game1.player,
							provider,
							Mod.IsValidConnector,
							distanceLimit: Mod.Config.MaxDistance,
							scanLimit: Mod.Config.MaxCheckedTiles,
							targetLimit: Mod.Config.MaxInventories,
							includeSource: true,
							includeDiagonal: Mod.Config.UseDiagonalConnections,
							includeBuildings: DiscoverBuildings,
							expandSource: Mod.Config.MaxWorkbenchGap
						);
					else
						CachedInventories = InventoryHelper.DiscoverInventories(
							pos,
							Game1.player,
							provider,
							null,
							distanceLimit: 1,
							scanLimit: 25,
							targetLimit: 9,
							includeSource: true,
							includeDiagonal: true,
							includeBuildings: DiscoverBuildings
						);
				}
			}
		} else if (DiscoverContainers && Mod.Config.UseDiscovery) {
			if (BenchLocation != null && BenchArea.HasValue)
				CachedInventories = InventoryHelper.DiscoverInventories(
					BenchArea.Value,
					BenchLocation,
					MaterialContainers,
					Game1.player,
					provider,
					Mod.IsValidConnector,
					distanceLimit: Mod.Config.MaxDistance,
					scanLimit: Mod.Config.MaxCheckedTiles,
					targetLimit: Mod.Config.MaxInventories,
					includeSource: true,
					includeDiagonal: Mod.Config.UseDiagonalConnections,
					includeBuildings: DiscoverBuildings,
					expandSource: Mod.Config.MaxWorkbenchGap
				);
			else
				CachedInventories = InventoryHelper.DiscoverInventories(
					MaterialContainers,
					Game1.player,
					provider,
					Mod.IsValidConnector,
					distanceLimit: Mod.Config.MaxDistance,
					scanLimit: Mod.Config.MaxCheckedTiles,
					targetLimit: Mod.Config.MaxInventories,
					includeSource: true,
					includeDiagonal: Mod.Config.UseDiagonalConnections,
					includeBuildings: DiscoverBuildings,
					extra: (BenchPosition.HasValue && BenchLocation != null)
						? new AbsolutePosition[] { new(BenchLocation, BenchPosition.Value) } : null
				);
		} else
			CachedInventories = MaterialContainers;

		CachedInventories ??= new List<LocatedInventory>();

		// The FarmHouse Kitchen always adds Mini-Fridges.
		if (cooking && BenchLocation?.GetFridge(false) is Chest fridge && TileHelper.GetRealPosition(fridge, BenchLocation) == BenchPosition) {
			// Make sure the main Fridge is in the list.
			CachedInventories.Add(new LocatedInventory(fridge, BenchLocation));
			// And also all Mini-Fridges.
			foreach (var obj in BenchLocation.Objects.Values) {
				if (obj is Chest chest && chest.fridge.Value)
					CachedInventories.Add(new LocatedInventory(chest, BenchLocation));
			}
		}

		// Get nearby inventories.
		int near_count = 0;
		if (DiscoverContainers) {
			List<LocatedInventory>? nearby = null;
			int radius = DiscoverAreaOverride ?? Mod.Config.NearbyRadius;

			if (radius == -2) {
				nearby = InventoryHelper.DiscoverAllLocations(
					Game1.player,
					Mod.GetInventoryProvider,
					Mod.Config.MaxCheckedTiles,
					Mod.Config.MaxInventories
				);

			} else if (radius == -1) {
				nearby = InventoryHelper.DiscoverLocation(
					Game1.player.currentLocation,
					Game1.player,
					Mod.GetInventoryProvider,
					Mod.Config.MaxCheckedTiles,
					Mod.Config.MaxInventories
				);

			} else if (radius > 0)
				nearby = InventoryHelper.DiscoverArea(
					Game1.player.currentLocation,
					Game1.player.Tile,
					radius,
					Game1.player,
					Mod.GetInventoryProvider,
					DiscoverBuildings,
					Mod.Config.MaxInventories
				);

			if (nearby != null) {
				near_count = nearby.Count;
				foreach (var obj in nearby)
					CachedInventories.Add(obj);
			}
		}

		int removed = CachedInventories.Count;
		InventoryHelper.DeduplicateInventories(ref CachedInventories, Mod.GetInventoryProvider);
		removed -= CachedInventories.Count;

		int unloaded = CachedInventories.Count;
		InventoryHelper.RemoveInactiveInventories(ref CachedInventories, provider);
		unloaded -= CachedInventories.Count;

		int invalid = CachedInventories.Count;
		InventoryHelper.RemoveInvalidInventories(ref CachedInventories, Mod.IsInvalidStorage);
		invalid -= CachedInventories.Count;

		UnsafeInventories = InventoryHelper.GetUnsafeInventories(
			CachedInventories,
			provider,
			Game1.player,
			nullLocationValid: true
		);

		InventoryItems = InventoryHelper.GetInventoryItems(
			CachedInventories,
			provider,
			Game1.player
		).ToHashSet();

#if DEBUG
		LogLevel level = LogLevel.Debug;
#else
		LogLevel level = LogLevel.Trace;
#endif

		Log($"Sources: {count} -- Items: {InventoryItems.Count} -- Nearby: {near_count} -- Duplicates: {removed} -- Unloaded: {unloaded} -- Valid: {CachedInventories.Count}", level);
	}

	internal virtual IList<IBCInventory>? GetUnsafeInventories() {
		return UnsafeInventories;
	}

	internal virtual IList<Item?>? GetEstimatedContainerContents() {
		if (CachedInventories == null || CachedInventories.Count == 0)
			return null;

		List<Item?> items = [];
		foreach (LocatedInventory loc in CachedInventories) {
			if (ChestsOnly && loc.Source is not Chest)
				continue;

			var provider = Mod.GetInventoryProvider(loc.Source);
			if (provider == null || !provider.CanExtractItems(loc.Source, loc.Location, Game1.player))
				continue;

			var mutex = provider.GetMutex(loc.Source, loc.Location, Game1.player);
			if (
				(mutex == null && provider.IsMutexRequired(loc.Source, loc.Location, Game1.player)) ||
				(mutex != null && mutex.IsLocked() && !mutex.IsLockHeld())
			)
				continue;

			var oitems = provider.GetItems(loc.Source, loc.Location, Game1.player);
			if (oitems != null)
				foreach (var item in oitems) {
					if (item == null || InventoryItems is null || !InventoryItems.Contains(item))
						items.Add(item);
				}
		}

		return items;
	}

	protected virtual List<Item?> GetActualContainerContents(IList<IBCInventory> locked) {
		List<Item?> items = [];
		foreach (IBCInventory inv in locked) {
			if (!inv.CanExtractItems())
				continue;

			IList<Item?>? oitems = inv.GetItems();
			if (oitems != null)
				foreach (var item in oitems) {
					if (item == null || InventoryItems == null || !InventoryItems.Contains(item))
						items.Add(item);
				}
		}

		return items;
	}

	public void PerformCraft(IRecipe recipe, int times, Action<int>? DoneAction = null, bool playSound = true, bool moveResultToInventory = false) {
		ClearCraftCache();

		//Log($"PerformCraft: {recipe.Name} -- {times}", LogLevel.Trace);

		if (Working) {
			//Log($"- FinishPerformCraft: Successes = 0", LogLevel.Trace);
			DoneAction?.Invoke(0);
			return;
		}

		Working = true;

		_CleanCraftingLocked();

		InventoryHelper.WithInventories(CachedInventories, Mod.GetInventoryProvider, Game1.player, (locked, onDone) => {

			Working = false;

			craftingLocked = locked;
			craftingOnDoneLocked = onDone;

			craftingModified = new bool[locked.Count];

			activeRecipe = recipe;
			craftingRemaining = times;
			craftingSuccessful = 0;
			craftingUsedAdditional = false;
			craftingOnDone = DoneAction;
			craftingPlaySound = playSound;
			craftingMoveResultsToInventory = moveResultToInventory;

			if (locked.Count < (CachedInventories?.Count ?? 0)) {
				LogInventories();
				Game1.addHUDMessage(new HUDMessage(I18n.Error_Locking(), HUDMessage.error_type));
			}

			_PerformNextCraft();
		}, nullLocationValid: true, helper: Mod.Helper);
	}

	private void _CleanCraftingLocked() {
		if (craftingLocked != null) {
			if (craftingModified != null) {
				for (int i = 0; i < craftingLocked.Count; i++) {
					if (craftingModified[i])
						craftingLocked[i].CleanInventory();
				}
				craftingModified = null;
			}

			craftingOnDoneLocked?.Invoke();
			craftingOnDoneLocked = null;
			craftingLocked = null;
		}
	}

	private void _PerformNextCraft() {
		// If there's no next craft, do the cleanup.
		if (Working || activeRecipe is null || craftingLocked is null || craftingRemaining <= 0) {
			_CleanCraftingLocked();

			if (craftingSuccessful > 0 && craftingPlaySound)
				Game1.playSound("coin");
			if (craftingUsedAdditional && craftingPlaySound)
				Game1.playSound("breathin");

			if (craftingSuccessful > 0 && HeldItem != null) {
				bool move_item = (Game1.options.gamepadControls || (craftingMoveResultsToInventory && HeldItem.maximumStackSize() == 1)) && Game1.player.couldInventoryAcceptThisItem(HeldItem);
				if (move_item && Game1.player.addItemToInventoryBool(HeldItem))
					HeldItem = null;
			}

			//Log($"- FinishPerformCraft: Successes = {craftingSuccessful}", LogLevel.Trace);

			// Clear the state and return.
			int success = craftingSuccessful;

			activeRecipe = null;
			craftingRemaining = 0;
			craftingSuccessful = 0;
			craftingUsedAdditional = false;

			// Also call our callback.
			craftingOnDone?.Invoke(success);
			craftingOnDone = null;

			return;
		}

		Working = true;

		int times = Math.Min(craftingRemaining, 20);

		//Log($"- PerformNextCraft: {times} of {craftingRemaining}", LogLevel.Trace);

		craftingRemaining -= times;

		List<Item?> items = GetActualContainerContents(craftingLocked);
		List<Chest>? chests = ChestsOnly ? craftingLocked
			.Where(x => x.Object is Chest)
			.Select(x => (Chest) x.Object)
			.ToList() : null;

		PerformCraftRecursive(
			activeRecipe, 0, times,
			(successes, used_additional) => {
				craftingSuccessful += successes;
				craftingUsedAdditional |= used_additional;

				Working = false;

				//Log($"- PerformCraftRecursive: Success = {successes >= times}", LogLevel.Trace);

				// If we didn't craft successfully the expected
				// number of times, abort early.
				if (successes < times)
					craftingRemaining = 0;

				// Finish immediately if we're done, otherwise
				// wait till the next update() to craft more.
				if (craftingRemaining <= 0)
					_PerformNextCraft();
			},
			craftingLocked, items, chests
		);
	}

	private void PerformCraftRecursive(
		IRecipe recipe, int successes, int times, Action<int, bool> onDone,
		IList<IBCInventory> locked, List<Item?> items, List<Chest>? chests,
		bool used_additional = false
	) {
		Dictionary<IIngredient, List<Item>> matchingItems = [];

		if (!recipe.CanCraft(Game1.player) || !recipe.HasIngredients(Game1.player, items, locked, Quality, matchingItems)) {
			onDone(successes, used_additional);
			return;
		}

		// Remove any InventoryItems automatically.
		if (InventoryItems != null && InventoryItems.Count > 0) {
			foreach (var list in matchingItems.Values)
				list.RemoveAll(InventoryItems.Contains);
		}

		Item? obj;
		try {
			obj = recipe.CreateItem();
		} catch (Exception ex) {
			Log($"Unable to create item instance for recipe '{recipe.Name}': {ex}", LogLevel.Error);
			onDone(successes, used_additional);
			return;
		}

		if (obj is not null)
			obj.Stack = recipe.QuantityPerCraft;

		var _ = new ChainedPerformCraftHandler(Mod, recipe, Game1.player, obj, matchingItems, this, pce => {
			FinishCraftRecursive(
				recipe, successes, times, onDone,
				locked, items, chests, matchingItems,
				used_additional, pce
			);
		});
	}

	private void FinishCraftRecursive(
		IRecipe recipe, int successes, int times, Action<int, bool> onDone,
		IList<IBCInventory> locked, List<Item?> items, List<Chest>? chests,
		Dictionary<IIngredient, List<Item>> matchingItems,
		bool used_additional, ChainedPerformCraftHandler pce
	) {
		if (!pce.Success) {
			if (pce.Exception is not null) {
				var source = pce.ExceptionSource;
				if (source is null) {
					Log($"Encountered exception in recipe '{recipe.Name}' PerformCraft event: {pce.Exception}", LogLevel.Error);
					Game1.addHUDMessage(new HUDMessage(I18n.Error_Crafting(), HUDMessage.error_type));
				} else {
					Log($"Encountered exception in PerformCraft event from mod '{source.Name}' ({source.UniqueID}): {pce.Exception}", LogLevel.Error);
					Game1.addHUDMessage(new HUDMessage(I18n.Error_Crafting_Source(source.Name), HUDMessage.error_type));
				}
			}

			onDone(successes, used_additional);
			return;
		}

		// Double check that we have enough matching items left for all
		// our ingredients.
		if (recipe.Ingredients != null)
			foreach (var entry in recipe.Ingredients) {
				if (entry is IConsumptionPreTrackingIngredient cpt) {
					var remaining = matchingItems.GetValueOrDefault(entry);
					int count = remaining is null ? 0 : remaining.Select(x => x.Stack).Sum();
					if (count < entry.Quantity) {
						onDone(successes, used_additional);
						return;
					}
				}
			}

		Item? obj = pce.Item;
		bool made = false;

		IIngredient[]? ingredients = null;
		bool consume = true;
		bool seasoningInventories = Mod.Config.UseSeasoning != SeasoningMode.InventoryOnly;

		if (cooking) {
			if (Mod.intCSkill != null && Mod.intCSkill.IsLoaded && recipe.CraftingRecipe != null && obj != null && chests != null) {
				consume = Mod.intCSkill.ModifyCookedItem(
					recipe.CraftingRecipe,
					obj,
					chests
				);
			}

			if (Mod.Config.UseSeasoning != SeasoningMode.Disabled && obj is SObject sobj && sobj.Quality == 0) {
				ingredients = SEASONING_RECIPE;

				if (CraftingHelper.HasIngredients(
					ingredients,
					Game1.player,
					seasoningInventories ? items : null,
					seasoningInventories ? locked : null,
					Quality
				))
					sobj.Quality = 2;
				else
					ingredients = null;
			}
		}

		if (obj == null) {
			made = true;

		} else if (HeldItem == null) {
			//HeldItem = obj;
			made = true;

		} else if (HeldItem.Name.Equals(obj.Name)
				&& HeldItem.getOne().canStackWith(obj.getOne())
				&& HeldItem.Stack + obj.Stack <= HeldItem.maximumStackSize()
		) {
			//HeldItem.Stack += recipe.QuantityPerCraft;
			made = true;

		} else if (!recipe.Stackable && Game1.player.couldInventoryAcceptThisItem(HeldItem) /* && Game1.player.addItemToInventoryBool(HeldItem)*/ ) {
			//HeldItem = obj;
			made = true;
		}

		if (!made) {
			onDone(successes, used_additional);
			return;
		}

		successes++;
		List<Item> consumedItems = [];

		// Consume ingredients and rebuild our item list.
		if (consume) {
			recipe.Consume(Game1.player, locked, Quality, Mod.Config.LowQualityFirst, matchingItems, consumedItems, craftingModified);

			// And refresh the working items list since we consumed items, assuming
			// this isn't the last pass. Don't do this if ingredients is not null
			// though as it'd be wasted effort.
			if (times > 1 && ingredients == null)
				items = GetActualContainerContents(locked);
		}

		if (ingredients != null) {
			used_additional = true;

			// Consume ingredients and rebuild our item list.
			CraftingHelper.ConsumeIngredients(ingredients, Game1.player, seasoningInventories ? locked : null, Quality, Mod.Config.LowQualityFirst, null, consumedItems, craftingModified);
			if (times > 1)
				items = GetActualContainerContents(locked);

			// Show a notice if the user used their last seasoning.
			if (!CraftingHelper.HasIngredients(ingredients, Game1.player, seasoningInventories ? items : null, seasoningInventories ? locked : null, Quality))
				Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Seasoning_UsedLast"));
		}

		// Now run the PostCraftEvent.
		PostCraftEvent postce = new(recipe, Game1.player, obj, this, consumedItems);
		if (recipe is IPostCraftEventRecipe pcer)
			try {
				pcer.PostCraft(postce);
			} catch (Exception ex) {
				Log($"Encountered exception in PostCraft event of recipe '{recipe.Name}': {ex}", LogLevel.Error);
			}

		// And the API post craft events.
		foreach (var api in Mod.APIInstances.Values)
			api.EmitPostCraft(postce);

		// Get the final item from the event.
		obj = postce.Item;

		// Now, actually insert it into the inventory.
		if (obj == null) {
			/* do nothing ~ */

		} else if (HeldItem == null) {
			HeldItem = obj;

		} else if (HeldItem.Name.Equals(obj.Name)
				&& HeldItem.getOne().canStackWith(obj.getOne())
				&& HeldItem.Stack + obj.Stack <= HeldItem.maximumStackSize()
		) {
			HeldItem.Stack += obj.Stack;

		} else if (!recipe.Stackable && Game1.player.couldInventoryAcceptThisItem(HeldItem) && Game1.player.addItemToInventoryBool(HeldItem)) {
			HeldItem = obj;
		} else {
			// If all else fails, drop the held item to hold this new item.
			Game1.player.currentLocation.debris.Add(new Debris(HeldItem, Game1.player.Position));
			HeldItem = obj;
		}

		// Finish up the logic.

		if (obj != null)
			Game1.player.checkForQuestComplete(null, -1, -1, obj, null, 2);

		if (!cooking && Game1.player.craftingRecipes.ContainsKey(recipe.Name))
			Game1.player.craftingRecipes[recipe.Name] += recipe.QuantityPerCraft;

		if (cooking) {
			if (obj != null)
				Game1.player.cookedRecipe(obj.ItemId);

			if (obj is SObject sobj && Mod.intCSkill != null)
				Mod.intCSkill.AddCookingExperience(
					Game1.player,
					sobj.Edibility
				);
		}

		if (!cooking)
			Game1.stats.checkForCraftingAchievements();
		else
			Game1.stats.checkForCookingAchievements();

		if (times > 1) {
			PerformCraftRecursive(
				recipe, successes, times - 1, onDone,
				locked, items, chests,
				used_additional
			);

		} else {
			onDone(successes, used_additional);
		}
	}

	#endregion

	#region Add to Inventories

	protected void HandleTransferClick(bool fill, bool isAction) {
		TransferBehavior behavior;
		Behaviors behaviors = fill ? Mod.Config.FillFromBehaviors : Mod.Config.AddToBehaviors;

		if (Mod.Config.ModiferKey?.IsDown() ?? false) {
			if (isAction)
				behavior = behaviors.ActionModified;
			else
				behavior = behaviors.UseToolModified;
		} else {
			if (isAction)
				behavior = behaviors.Action;
			else
				behavior = behaviors.UseTool;
		}

		Action<Item?>? callback;
		if (HeldItem is null)
			callback = null;
		else
			callback = result => {
				HeldItem = result;
			};

		PerformTransfer(behavior, HeldItem, onDoneAction: callback);
	}

	private void LogInventories() {
		if (CachedInventories == null)
			return;

#if DEBUG
		LogLevel level = LogLevel.Debug;
#else
		LogLevel level = LogLevel.Trace;
#endif

		Mod.Log($"Inventory State as follows. Current Location: {Game1.currentLocation.NameOrUniqueName}", level);

		List<string[]> states = [];
		var active_places = Mod.Helper.Multiplayer.GetActiveLocations().ToHashSet();

		var locations = new HashSet<GameLocation>();

		Utility.ForEachLocation(loc => {
			locations.Add(loc);
			return true;
		}, includeInteriors: false, includeGenerated: true);

		foreach (var inv in CachedInventories) {
			var provider = Mod.GetInventoryProvider(inv.Source);
			var mutex = provider?.GetMutex(inv.Source, inv.Location, Game1.player);

			bool active = inv.Location is not null && active_places.Contains(inv.Location);

			states.Add([
				$"{inv.Source.GetHashCode()}",
				$"{inv.Source.GetType().FullName}",
				$"{provider?.GetType()?.FullName}",
				$"{mutex?.GetHashCode()}",
				$"{inv.Location?.NameOrUniqueName}",
				$"{active}",
				$"{inv.Location is not null && locations.Contains(inv.Location)}",
				inv.Location?.Root is not null ? $"{inv.Location.Root.GetHashCode()}" : "---"
			]);
		}

		string[] headers = [
			"ID",
			"Type",
			"Provider",
			"MutexID",
			"Location",
			"IsActive",
			"InLocationsList",
			"NetRoot"
		];

		Mod.LogTable(headers, states, level);

	}

	protected void PerformTransfer(TransferBehavior behavior, Item? item, Action<Item?>? onDoneAction = null) {
		if (Working) {
			onDoneAction?.Invoke(item);
			return;
		}

		Working = true;
		Game1.playSound("Ship");

		if (item == null) {
			// If we have no item, transfer everything.
			InventoryHelper.WithInventories(CachedInventories, Mod.GetInventoryProvider, Game1.player, (locked, onDone) => {
				if (locked.Count < (CachedInventories?.Count ?? 0)) {
					LogInventories();
					Game1.addHUDMessage(new HUDMessage(I18n.Error_Locking(), HUDMessage.error_type));
				}

				void OnTransfer(Item item, int idx) {
					if (idx < 0 || idx >= inventory.inventory.Count)
						return;

					var bounds = inventory.inventory[idx].bounds;
					var sprite = new ItemGrabMenu.TransferredItemSprite(item.getOne(), bounds.X, bounds.Y);
					tSprites.Add(sprite);
				}

				InventoryHelper.AddToInventories(inventory.actualInventory, locked, behavior, OnTransfer);
				onDone();
				Working = false;
				onDoneAction?.Invoke(item);
			}, nullLocationValid: true, helper: Mod.Helper);

		} else {
			InventoryHelper.WithInventories(CachedInventories, Mod.GetInventoryProvider, Game1.player, (locked, onDone) => {
				if (locked.Count < (CachedInventories?.Count ?? 0)) {
					LogInventories();
					Game1.addHUDMessage(new HUDMessage(I18n.Error_Locking(), HUDMessage.error_type));
				}

				List<Item?> items = new() { item };

				InventoryHelper.AddToInventories(items, locked, behavior);

				onDone();
				Working = false;

				if (items.Count > 1) {
					// How did we get extra items?
					for (int i = 1; i < items.Count; i++)
						Utility.CollectOrDrop(items[i]);
				}

				onDoneAction?.Invoke(items.Count > 0 ? items[0] : null);
			}, nullLocationValid: true, helper: Mod.Helper);
		}
	}

	#endregion

	#region Layout and Components

	protected virtual int BasePageX() => xPositionOnScreen
		+ IClickableMenu.spaceToClearSideBorder
		+ IClickableMenu.borderWidth;

	protected virtual int CraftingPageX() => BasePageX()
		+ (Editing && (CurrentTab?.Category?.UseRules ?? false) ? 432 : 0);

	protected virtual int CraftingPageY() => yPositionOnScreen
		+ (Editing ? 88 : 0)
		+ IClickableMenu.spaceToClearTopBorder
		+ IClickableMenu.borderWidth
		- 16;

	protected void BuildRecipeComponents() {
		int idx = 0;

		ComponentRecipes.Clear();
		RecipeComponents.Clear();

		foreach (IRecipe recipe in Recipes) {
			idx++;

			ClickableTextureComponent cmp = RecipeComponents[recipe] = new ClickableTextureComponent(
				name: "",
				bounds: new Rectangle(0, 0, 64, 64),
				label: null,
				hoverText: (Station == null || !Station.DisplayUnknownRecipes) && !recipe.HasRecipe(Game1.player) ? "ghosted" : "",
				texture: recipe.Texture,
				sourceRect: recipe.SourceRectangle,
				scale: 4f
			) {
				myID = 200 + idx,
				upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				//fullyImmutable = true,
				region = 8000
			};

			ComponentRecipes[cmp] = recipe;
		}

	}

	protected virtual List<ClickableTextureComponent> CreateNewPage() {
		List<ClickableTextureComponent> result = [];
		Pages.Add(result);
		return result;
	}

	protected virtual ClickableTextureComponent[,] CreateNewPageLayout() {
		int xSize = Editing && (CurrentTab?.Category?.UseRules ?? false) ? 4 : 10;
		int ySize = 4;
		if (Editing)
			ySize += Math.Max(0, height - (256 + spaceToClearTopBorder + borderWidth + 48 + 88)) / 72;
		else
			ySize = Math.Max(288, (inventory.yPositionOnScreen - yPositionOnScreen) - borderWidth - spaceToClearTopBorder) / 72;

		return new ClickableTextureComponent[xSize, ySize];
	}

	protected virtual bool SpaceOccupied(ClickableTextureComponent[,] layout, int x, int y, int width, int height) {
		if (width == 1 && height == 1 && x < layout.GetLength(0) && y < layout.GetLength(1))
			return layout[x, y] != null;

		if (x + width > layout.GetLength(0) || y + height > layout.GetLength(1))
			return true;

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if (layout[x + i, y + j] != null)
					return true;
			}
		}

		return false;
	}

	protected void UpdatePageSnapping(ClickableTextureComponent[,] layout, int maxX, int startY) {
		// Update our snapping.
		int inventory_columns = inventory.capacity / inventory.rows;

		for (int x = 0; x < maxX; x++) {
			for (int y = startY; y >= 0; y--) {
				// We need a component to work with, of course.
				if (layout[x, y] is not ClickableComponent cmp)
					continue;

				// Left to Right Snapping
				if (x > 0 && layout[x - 1, y] is ClickableComponent toLeft && toLeft != cmp) {
					toLeft.rightNeighborID = cmp.myID;
					cmp.leftNeighborID = toLeft.myID;
				}

				// Up Snapping
				if (y > 0 && layout[x, y - 1] is ClickableComponent toUp && toUp != cmp) {
					toUp.downNeighborID = cmp.myID;
					cmp.upNeighborID = toUp.myID;
				}

				// Bottom snapping
				if (y < startY && layout[x, y + 1] is ClickableComponent toDown && toDown != cmp) {
					toDown.upNeighborID = cmp.myID;
					cmp.downNeighborID = toDown.myID;

				} else if (y >= startY || layout[x, y + 1] is null) {
					int targetSlot = Math.Clamp((int) Math.Floor(inventory_columns * ((float) x / maxX)), 0, inventory_columns - 1);
					if (targetSlot < inventory.inventory.Count) {
						cmp.downNeighborID = inventory.inventory[targetSlot].myID;
					}
				}
			}
		}
	}

	protected void LayoutRecipes() {
		bool uniform = Mod.Config.UseUniformGrid;
		bool favorites_only = !Mod.Config.UseCategories && FavoritesOnly;

		int offsetX = CraftingPageX();
		int offsetY = CraftingPageY();
		int marginX = 8;

		Pages.Clear();

		List<ClickableTextureComponent> page = CreateNewPage();
		ClickableTextureComponent[,] layout = CreateNewPageLayout();

		int xLimit = layout.GetLength(0);
		int yLimit = layout.GetLength(1);

		int x = 0;
		int y = 0;

		List<IRecipe> sorted;
		if (Editing)
			sorted = Recipes.ToList();
		else
			sorted = CurrentTab.FilteredRecipes.ToList();

		sorted.Sort((a, b) => {
			int result = 0;

			if (Mod.Config.SortBigLast) {
				bool bigA = a.GridHeight > 1 || a.GridWidth > 1;
				bool bigB = b.GridHeight > 1 || b.GridWidth > 1;

				result = bigA.CompareTo(bigB);
				if (result != 0)
					return result;
			}

			if (cooking ? Mod.Config.CookingAlphabetic : Mod.Config.CraftingAlphabetic) {
				result = a.DisplayName.CompareTo(b.DisplayName);
				if (result != 0)
					return result;
			}

			// If nothing else, restore original sort.
			int posA = Editing ? Recipes.IndexOf(a) : CurrentTab.Recipes.IndexOf(a);
			int posB = Editing ? Recipes.IndexOf(b) : CurrentTab.Recipes.IndexOf(b);

			return posA.CompareTo(posB);
		});

		foreach (IRecipe recipe in sorted) {
			if (!Editing && favorites_only && !Favorites.Contains(recipe))
				continue;

			if (Editing && !DoesRecipeMatchFilter(recipe))
				continue;

			RecipeComponents.TryGetValue(recipe, out ClickableTextureComponent? cmp);
			if (cmp == null)
				continue;

			// TODO: Optionally skip recipes we don't have the materials for.

			// Ensure that this will fit in the grid.
			int width = uniform ? 1 : Math.Max(1, recipe.GridWidth);
			int height = uniform ? 1 : Math.Max(1, recipe.GridHeight);

			float scale = Math.Min(xLimit / (float) width, yLimit / (float) height);
			if (scale < 1f) {
				width = Math.Max(1, (int) (width * scale));
				height = Math.Max(1, (int) (height * scale));
			}

			while (SpaceOccupied(layout, x, y, width, height)) {
				x++;
				if (x >= xLimit) {
					x = 0;
					y++;
					if (y >= yLimit) {
						UpdatePageSnapping(layout, xLimit, yLimit - 1);
						page = CreateNewPage();
						layout = CreateNewPageLayout();
						y = 0;
					}
				}
			}

			// Update the upNeighborID as appropriate to help
			// make snapping easier
			if (y == 0 && Editing && btnCategoryName != null) {
				if (x < 2 && btnCategoryIcon != null)
					cmp.upNeighborID = btnCategoryIcon.myID;
				else
					cmp.upNeighborID = btnCategoryName.myID;
			} else
				cmp.upNeighborID = ClickableComponent.SNAP_AUTOMATIC;

			// Reset neighbors
			cmp.leftNeighborID = ClickableComponent.SNAP_AUTOMATIC;
			cmp.rightNeighborID = ClickableComponent.SNAP_AUTOMATIC;
			cmp.downNeighborID = ClickableComponent.SNAP_AUTOMATIC;

			cmp.bounds = new Rectangle(
				offsetX + x * (64 + marginX),
				offsetY + y * 72,
				64 * width + (marginX * (width - 1)),
				64 * height + (8 * (height - 1))
			);

			page.Add(cmp);

			if (width == 1 && height == 1)
				layout[x, y] = cmp;
			else
				for (int i = 0; i < width; i++) {
					for (int j = 0; j < height; j++) {
						layout[x + i, y + j] = cmp;
					}
				}
		}

		UpdatePageSnapping(layout, xLimit, y >= yLimit ? yLimit - 1 : y);

		if (Pages.Count > 1 && btnPageUp == null) {
			bool custom_scroll = Theme.CustomScroll && Background is not null;

			btnPageUp = new ClickableTextureComponent(
				new Rectangle(xPositionOnScreen + 768 + 16, offsetY, 64, 64),
				custom_scroll ? Background : Game1.mouseCursors,
				custom_scroll
					? Sprites.CustomScroll.PAGE_UP
					: Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 12),
				custom_scroll
					? 3.2f
					: 0.8f
			) {
				myID = 88,
				downNeighborID = 89,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC
			};

			btnPageDown = new ClickableTextureComponent(
				new Rectangle(xPositionOnScreen + 768 + 16, offsetY + 192 + 32, 64, 64),
				custom_scroll ? Background : Game1.mouseCursors,
				custom_scroll
					? Sprites.CustomScroll.PAGE_DOWN
					: Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 11),
				custom_scroll
					? 3.2f
					: 0.8f
			) {
				myID = 89,
				upNeighborID = 88,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC
			};

		} else if (Pages.Count <= 1 && btnPageUp != null) {
			btnPageUp = null;
			btnPageDown = null;
		}

		if (btnPageUp != null)
			btnPageUp.bounds.Y = offsetY;
		if (btnPageDown != null)
			btnPageDown.bounds.Y = offsetY + (yLimit - 1) * 72 + 8;

		if (pageIndex >= Pages.Count)
			pageIndex = Pages.Count - 1;

		UpdateCurrentButtons();
	}

	protected virtual void UpdateCurrentButtons() {
		currentPageComponents.Clear();
		currentPageComponents.AddRange(CurrentPage);
		currentPageComponents.AddRange(Tabs.Select(val => val.Component));

		if (!Editing) {
			currentPageComponents.AddRange(inventory.inventory);
			currentPageComponents.Add(inventory.dropItemInvisibleButton);
		}

		populateClickableComponentList();
	}

	public virtual void ChangePage(int change) {
		SetPage(pageIndex + change);
	}

	public virtual void SetPage(int idx) {
		pageIndex = idx;
		if (pageIndex < 0)
			pageIndex = 0;
		else if (pageIndex >= Pages.Count)
			pageIndex = Pages.Count - 1;

		UpdateCurrentButtons();

		// Fix the inventory snapping.
		for (int i = 0; i < 12; i++) {
			if (inventory.inventory.Count > i)
				inventory.inventory[i].upNeighborID = ClickableComponent.SNAP_AUTOMATIC;
		}

		// Find closest snapped component.
		snapToNearestClickableComponent();
	}


	public virtual bool ChangeTab(int change) {
		// If we're using filtering, try finding the next tab that has
		// contents. Otherwise, just go.
		if (Filter != null) {
			for (int i = tabIndex + change; i >= 0 && i < Tabs.Count; i += change) {
				if (Tabs[i] is TabInfo tab && (tab.FilteredRecipes?.Count ?? 0) > 0)
					return SetTab(i);
			}

			return false;
		}

		return SetTab(tabIndex + change);
	}

	public virtual bool SetTab(int idx) {
		if (idx < 0)
			idx = 0;
		else if (idx >= Tabs.Count)
			idx = Tabs.Count - 1;

		if (tabIndex == idx)
			return false;

		UpdateCategoryName();

		tabIndex = idx;
		LastTab = CurrentTab?.Category?.Id ?? LastTab;
		pageIndex = 0;

		if (Editing && txtCategoryName != null)
			txtCategoryName.Text = CurrentTab?.Component.label ?? "";

		if (Editing && btnCategoryFilter != null)
			btnCategoryFilter.sourceRect = SourceCatFilter;

		if (Editing && btnCategoryIncludeInMisc != null)
			btnCategoryIncludeInMisc.sourceRect = SourceIncludeInMisc;

		if (Editing && Flow is not null)
			UpdateFlow();

		PositionTabMoveButtons();

		LayoutRecipes();

		// Fix the inventory snapping.
		for (int i = 0; i < 12; i++) {
			if (inventory.inventory.Count > i)
				inventory.inventory[i].upNeighborID = ClickableComponent.SNAP_AUTOMATIC;
		}

		// Find closest snapped component.
		snapToNearestClickableComponent();

		return true;
	}

	#endregion

	#region Filtering

	private string HighlightSearchTerms(string text, bool is_ingredient = false, bool is_like = false, bool is_love = false) {
		if (FilterRegex == null || is_ingredient && !FilterIngredients || is_like && !FilterLikes || is_love && !FilterLoves)
			return text;

		string color = "#FFD70050";
		if (Theme?.SearchHighlightColor != null) {
			Color c = Theme.SearchHighlightColor.Value;
			color = $"#{c.R:X2}{c.G:X2}{c.B:X2}{c.A:X2}";
		}

		return FilterRegex.Replace(text, match => $"@h@r{{{color}}}{match.Value}@r@H");
	}

	private bool DoesRecipeMatchFilter(IRecipe recipe) {
		if (FilterRegex == null)
			return true;

		if (FilterRegex.IsMatch(recipe.Name))
			return true;

		if (FilterRegex.IsMatch(recipe.DisplayName))
			return true;

		if (!string.IsNullOrEmpty(recipe.Description) && FilterRegex.IsMatch(recipe.Description))
			return true;

		if (FilterIngredients && recipe.Ingredients != null) {
			foreach (var ing in recipe.Ingredients) {
				if (FilterRegex.IsMatch(ing.DisplayName))
					return true;
			}
		}

		if (FilterLikes || FilterLoves) {
			(List<NPC>, List<NPC>)? tastes = Mod.Recipes.GetGiftTastes(recipe);
			if (tastes is not null) {
				if (FilterLoves)
					foreach (NPC npc in tastes.Value.Item1) {
						if (FilterRegex.IsMatch(npc.displayName))
							return true;
					}

				if (FilterLikes)
					foreach (NPC npc in tastes.Value.Item2) {
						if (FilterRegex.IsMatch(npc.displayName))
							return true;
					}
			}
		}

		return false;
	}

	public void ToggleSearch() {
		bool filtered = false;

		string? old = Filter;
		if (old != null && FilterIngredients)
			old = $"{I18n.Search_IngredientPrefix()}{old}";

		if (old != null && FilterLikes)
			old = $"{I18n.Search_LikePrefix()}{old}";

		if (old != null && FilterLoves)
			old = $"{I18n.Search_LovePrefix()}{old}";

		var search = new SearchBox(
			Mod,
			this,
			btnSearch.bounds.X - (400 - btnSearch.bounds.Width) + 16,
			btnSearch.bounds.Y - (96 - btnSearch.bounds.Height) / 2,
			400,
			96,
			filter => {
				UpdateFilter(filter);
				filtered = true;
			},
			old
		) {
			exitFunction = () => {
				if (!filtered)
					UpdateFilter(null);
				if (Game1.options.SnappyMenus)
					snapCursorToCurrentSnappedComponent();
			}
		};

		SetChildMenu(search);
		performHoverAction(0, 0);
	}

	public bool UpdateFilter(string? filter) {
		if (!string.IsNullOrEmpty(filter)) {
			filter = filter.Trim();
			string ing_prefix = I18n.Search_IngredientPrefix();
			string like_prefix = I18n.Search_LikePrefix();
			string love_prefix = I18n.Search_LovePrefix();
			bool matched = true;

			FilterIngredients = false;
			FilterLikes = false;
			FilterLoves = false;

			while (matched) {
				matched = false;

				bool has = !FilterIngredients && filter.StartsWith(ing_prefix);
				if (has) {
					FilterIngredients = true;
					matched = true;
					filter = filter[ing_prefix.Length..].TrimStart();
				}

				has = !FilterLikes && filter.StartsWith(like_prefix);
				if (has) {
					FilterLikes = true;
					matched = true;
					filter = filter[like_prefix.Length..].TrimStart();
				}

				has = !FilterLoves && filter.StartsWith(love_prefix);
				if (has) {
					FilterLoves = true;
					matched = true;
					filter = filter[love_prefix.Length..].TrimStart();
				}
			}
		}

		if (string.IsNullOrEmpty(filter)) {
			if (Filter == null)
				return false;

			Filter = null;

		} else
			Filter = filter;

		if (Filter == null) {
			FilterRegex = null;
			FilterIngredients = false;
			FilterLikes = false;
			FilterLoves = false;
		} else
			FilterRegex = new(Regex.Escape(Filter), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

		btnSearch.sourceRect = SourceFilter;

		foreach (TabInfo info in Tabs) {
			if (Filter == null)
				info.FilteredRecipes = info.Recipes;
			else
				info.FilteredRecipes = info.Recipes.Where(DoesRecipeMatchFilter).ToList();
		}

		LayoutRecipes();
		return true;
	}

	#endregion

	#region Events

	protected override bool _ShouldAutoSnapPrioritizeAlignedElements() => cooking || Mod.Config.UseUniformGrid;

	public void snapToNearestClickableComponent() {
		if (!Game1.options.SnappyMenus)
			return;

		int x = Game1.getOldMouseX();
		int y = Game1.getOldMouseY();

		if (allClickableComponents != null)
			foreach (var cmp in allClickableComponents)
				if (cmp.containsPoint(x, y)) {
					currentlySnappedComponent = cmp;
					snapCursorToCurrentSnappedComponent();
					return;
				}

		snapToDefaultClickableComponent();
	}

	public override void snapToDefaultClickableComponent() {
		currentlySnappedComponent = CurrentPage?.Count > 0 ? CurrentPage[0] : null;
		snapCursorToCurrentSnappedComponent();
	}

	protected override void actionOnRegionChange(int oldRegion, int newRegion) {
		base.actionOnRegionChange(oldRegion, newRegion);
		if (newRegion != 9000 || oldRegion == 0)
			return;

		for (int i = 0; i < 10; i++) {
			if (inventory.inventory.Count > i)
				inventory.inventory[i].upNeighborID = currentlySnappedComponent.upNeighborID;
		}
	}

	public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) {
		base.gameWindowSizeChanged(oldBounds, newBounds);

		UpdateTabs();
		LayoutRecipes();

		inventory.xPositionOnScreen = xPositionOnScreen + spaceToClearSideBorder + borderWidth;
		inventory.yPositionOnScreen = yPositionOnScreen + spaceToClearTopBorder + borderWidth + 320 - 16;

		if (upperRightCloseButton is not null) {
			upperRightCloseButton.bounds.X = xPositionOnScreen + width - 36;
			upperRightCloseButton.bounds.Y = yPositionOnScreen - 8;
		}

		// Buttons
		int btnX = xPositionOnScreen + width + 4;
		int btnY = yPositionOnScreen + 128;

		var first = btnSearch ?? btnToggleEdit ?? btnToggleFavorites ?? btnToggleSeasoning ?? btnToggleQuality ?? btnToggleUniform ?? btnSettings ?? trashCan;
		first.bounds.X = btnX;
		first.bounds.Y = btnY;

		GUIHelper.MoveComponents(
			GUIHelper.Side.Down, 16,
			btnSearch,
			btnToggleEdit,
			btnToggleFavorites,
			btnToggleSeasoning,
			btnToggleQuality,
			btnToggleUniform,
			btnSettings,
			trashCan
		);

		GUIHelper.MoveComponents(
			GUIHelper.Side.Right, 16,
			btnSearch,
			btnTransferTo
		);

		GUIHelper.LinkComponents(
			GUIHelper.Side.Right,
			id => getComponentWithID(id),
			btnToggleEdit,
			btnTransferFrom
		);

		GUIHelper.LinkComponents(
			GUIHelper.Side.Down,
			id => getComponentWithID(id),
			btnTransferTo,
			btnTransferFrom
		);

		GUIHelper.MoveComponents(
			GUIHelper.Side.Down, 16,
			btnTransferTo,
			btnTransferFrom
		);
	}

	public override void receiveGamePadButton(Buttons b) {
		if (GetChildMenu() is IClickableMenu menu) {
			if (!Standalone)
				menu.receiveGamePadButton(b);
			return;
		}

		base.receiveGamePadButton(b);

		if (txtCategoryName?.Selected ?? false) {
			switch (b) {
				case Buttons.DPadUp:
				case Buttons.DPadDown:
				case Buttons.DPadLeft:
				case Buttons.DPadRight:
				case Buttons.LeftThumbstickUp:
				case Buttons.LeftThumbstickDown:
				case Buttons.LeftThumbstickLeft:
				case Buttons.LeftThumbstickRight:
					txtCategoryName.Selected = false;
					break;
			}
		}

		if (b.Equals(Buttons.LeftShoulder)) {
			if (ChangeTab(-1)) {
				CenterTab(tabIndex);
				Game1.playSound("smallSelect");
			}
		} else if (b.Equals(Buttons.RightShoulder)) {
			if (ChangeTab(1)) {
				CenterTab(tabIndex);
				Game1.playSound("smallSelect");
			}
		}
	}

	public override void receiveKeyPress(Keys key) {
#if DEBUG
		if (key.Equals(Keys.F9))
			ShowTiming = !ShowTiming;
#endif

		if (GetChildMenu() is IClickableMenu menu) {
			if (!Standalone)
				menu.receiveKeyPress(key);
			return;
		}

		if (txtCategoryName?.Selected ?? false)
			return;

		base.receiveKeyPress(key);

		if (Mod.Config.SearchKey?.JustPressed() ?? false) {
			ToggleSearch();
			return;
		}

		if (!Editing && hoverRecipe != null) {
			if (Mod.Config.FavoriteKey?.JustPressed() ?? false) {
				Mod.Helper.Input.SuppressActiveKeybinds(Mod.Config.FavoriteKey);
				if (ToggleFavorite(hoverRecipe))
					Game1.playSound("coin");
				return;
			}

			if (Mod.Config.BulkCraftKey?.JustPressed() ?? false) {
				Mod.Helper.Input.SuppressActiveKeybinds(Mod.Config.BulkCraftKey);
				if (OpenBulkCraft(hoverRecipe))
					Game1.playSound("bigSelect");
				return;
			}
		}

		if (HeldItem == null || !HeldItem.canBeTrashed())
			return;

		if (!Editing && key.Equals(Keys.Delete) && HeldItem.canBeTrashed()) {
			if (InventoryItems != null && InventoryItems.Contains(HeldItem))
				return;

			if (Recycling) {
				if (Game1.options.SnappyMenus) {
					setCurrentlySnappedComponentTo(trashCan.myID);
					snapCursorToCurrentSnappedComponent();
				}
			} else {
				Utility.trashItem(HeldItem);
				HeldItem = null;
			}
		}

		if (Game1.isAnyGamePadButtonBeingHeld() && Game1.options.doesInputListContain(Game1.options.menuButton, key))
			Game1.setMousePosition(trashCan.bounds.Center);
	}

	public override void receiveScrollWheelAction(int direction) {
		if (GetChildMenu() is IClickableMenu menu) {
			if (!Standalone)
				menu.receiveScrollWheelAction(direction);
			return;
		}

		base.receiveScrollWheelAction(direction);
		int x = Game1.getOldMouseX();
		int y = Game1.getOldMouseY();

		if (x < (xPositionOnScreen + 16 + 8)) {
			if (ScrollTabs(direction > 0 ? -1 : 1))
				Game1.playSound("shwip");

			return;
		}

		if (x < (xPositionOnScreen + 432 + 16 + 8) && Editing && (CurrentTab?.Category?.UseRules ?? false)) {
			if (Flow is not null && Flow.Scroll(direction > 0 ? -1 : 1))
				Game1.playSound("shwip");
			return;
		}

		if (!Editing && y >= inventory.yPositionOnScreen)
			return;

		int change;

		if (direction > 0 && pageIndex > 0)
			change = -1;
		else if (direction < 0 && pageIndex < Pages.Count - 1)
			change = 1;
		else
			return;

		ChangePage(change);
		Game1.playSound("shwip");

		if (!Game1.options.SnappyMenus)
			return;

		setCurrentlySnappedComponentTo((change > 0 ? btnPageDown?.myID : btnPageUp?.myID) ?? ClickableComponent.SNAP_TO_DEFAULT);
		snapCursorToCurrentSnappedComponent();
	}

	public IRecipe? GetRecipeUnderCursor(int x, int y) {
		if (Editing || CurrentPage == null)
			return null;

		foreach (var cmp in CurrentPage) {
			if (cmp.containsPoint(x, y) && ComponentRecipes.TryGetValue(cmp, out IRecipe? recipe)) {
				if (!cmp.hoverText.Equals("ghosted"))
					return recipe;
				break;
			}
		}

		return null;
	}

	public override void update(GameTime time) {
		base.update(time);

		if (!Working && craftingRemaining > 0)
			_PerformNextCraft();

		for (int i = 0; i < tSprites.Count; i++) {
			if (tSprites[i].Update(time)) {
				tSprites.RemoveAt(i);
				i--;
			}
		}
	}

	public override void releaseLeftClick(int x, int y) {
		if (GetChildMenu() is IClickableMenu menu) {
			if (!Standalone)
				menu.releaseLeftClick(x, y);
		}

		base.releaseLeftClick(x, y);
		Flow?.ReleaseLeftClick();
	}

	public override void leftClickHeld(int x, int y) {
		if (GetChildMenu() is IClickableMenu menu) {
			if (!Standalone)
				menu.leftClickHeld(x, y);
		}

		base.leftClickHeld(x, y);
		Flow?.LeftClickHeld(x, y);
	}

	public override void receiveLeftClick(int x, int y, bool playSound = true) {
		if (GetChildMenu() is IClickableMenu menu) {
			if (!Standalone)
				menu.receiveLeftClick(x, y, playSound);
			return;
		}

		base.receiveLeftClick(x, y, playSound);

		if (Editing && Flow is not null && (CurrentTab?.Category?.UseRules ?? false) && Flow.ReceiveLeftClick(x, y, playSound))
			return;

		// Inventory
		if (!Editing)
			HeldItem = inventory.leftClick(x, y, HeldItem, playSound);

		// Editing
		txtCategoryName?.Update();

		if (btnCategoryIcon != null && btnCategoryIcon.containsPoint(x, y)) {
			var picker = new IconPicker(Mod,
				btnCategoryIcon.bounds.Left,
				btnCategoryIcon.bounds.Bottom,

				width - 128,
				height - 256,

				icon => UpdateCategorySprite(icon)
			) {
				exitFunction = () => {
					if (Game1.options.SnappyMenus)
						snapCursorToCurrentSnappedComponent();
				}
			};

			if (playSound)
				Game1.playSound("bigSelect");
			SetChildMenu(picker);
			return;
		}

		if (Editing && btnCategoryCopy != null && btnCategoryCopy.containsPoint(x, y)) {
			bool success = false;

			if (!IsMiscCategory && CurrentTab?.Category is not null) {
				Mod.GetJsonHelper();
				if (Mod.JsonHelper is not null) {
					try {
						string result = Mod.JsonHelper.Serialize(CurrentTab.Category);
						DesktopClipboard.SetText(result);
						success = true;
					} catch (Exception ex) {
						Log($"Unable to copy category to clipboard.", LogLevel.Warn, ex);
						success = false;
					}

				}
			}

			btnCategoryCopy.scale = btnCategoryCopy.baseScale;
			if (playSound)
				Game1.playSound(success ? "bigSelect" : "stoneStep");
			return;
		}

		if (Editing && btnCategoryPaste != null && btnCategoryPaste.containsPoint(x, y)) {

			Category? cat = null;
			try {
				Mod.GetJsonHelper();
				if (Mod.JsonHelper is not null) {
					string result = string.Empty;
					DesktopClipboard.GetText(ref result);
					if (!string.IsNullOrEmpty(result))
						cat = Mod.JsonHelper.Deserialize<Category>(result);
				}

			} catch (Exception ex) {
				Log($"Unable to read category from clipboard: {ex.Message}", LogLevel.Warn);
			}

			if (cat != null && !PasteCategory(cat))
				cat = null;

			if (cat is null)
				Game1.addHUDMessage(new HUDMessage(I18n.Error_Pasting(), HUDMessage.error_type));

			btnCategoryPaste.scale = btnCategoryPaste.baseScale;
			if (playSound)
				Game1.playSound(cat is not null ? "bigSelect" : "stoneStep");
			return;
		}

		if (btnCategoryTrash != null && btnCategoryTrash.containsPoint(x, y)) {
			bool success = false;
			if (Mod.Config.ModiferKey?.IsDown() ?? false) {
				ResetCategories();
				success = true;

			} else if (!IsMiscCategory) {
				DeleteCategory();
				success = true;
			}

			if (playSound)
				Game1.playSound(success ? "bigSelect" : "stoneStep");
			return;
		}

		if (btnCategoryFilter != null && btnCategoryFilter.containsPoint(x, y)) {
			if (IsMiscCategory && !CurrentTab!.Category!.UseRules)
				return;

			ToggleFilterMode();
			btnCategoryFilter.scale = btnCategoryFilter.baseScale;

			if (playSound)
				Game1.playSound("bigSelect");
			return;
		}

		if (btnCategoryIncludeInMisc != null && btnCategoryIncludeInMisc.containsPoint(x, y)) {
			if (IsMiscCategory)
				return;

			ToggleIncludeInMisc();
			btnCategoryIncludeInMisc.scale = btnCategoryIncludeInMisc.baseScale;

			if (playSound)
				Game1.playSound("bigSelect");
			return;
		}

		if (btnCatUp != null && btnCatUp.containsPoint(x, y)) {
			if (MoveCurrentCategory(-1) && playSound)
				Game1.playSound("smallSelect");
			return;
		}

		if (btnCatDown != null && btnCatDown.containsPoint(x, y)) {
			if (MoveCurrentCategory(1) && playSound)
				Game1.playSound("smallSelect");
			return;
		}

		// Pagination
		if (btnPageUp != null && btnPageUp.containsPoint(x, y) && pageIndex > 0) {
			ChangePage(-1);
			if (playSound)
				Game1.playSound("smallSelect");
			btnPageUp.scale = btnPageUp.baseScale;
			return;
		}

		if (btnPageDown != null && btnPageDown.containsPoint(x, y) && pageIndex < Pages.Count - 1) {
			ChangePage(+1);
			if (playSound)
				Game1.playSound("smallSelect");
			btnPageDown.scale = btnPageDown.baseScale;
			return;
		}

		// Tabs
		if (btnTabsUp?.containsPoint(x, y) ?? false) {
			btnTabsUp.scale = btnTabsUp.baseScale;
			if (ScrollTabs(-1) && playSound)
				Game1.playSound("shiny4");
		}

		if (btnTabsDown?.containsPoint(x, y) ?? false) {
			btnTabsDown.scale = btnTabsDown.baseScale;
			if (ScrollTabs(1) && playSound)
				Game1.playSound("shiny4");
		}

		if (Tabs.Count > 1)
			for (int i = 0; i < Tabs.Count; i++) {
				ClickableComponent cmp = Tabs[i].Component;
				if (cmp.visible && cmp.containsPoint(x, y) && tabIndex != i) {
					if (!Editing && Tabs[i].FilteredRecipes.Count == 0)
						return;

					SetTab(i);
					if (playSound)
						Game1.playSound("smallSelect");
					return;
				}
			}

		// Clickable Recipes
		if (CurrentPage != null)
			foreach (var cmp in CurrentPage) {
				if (cmp.containsPoint(x, y) && ComponentRecipes.TryGetValue(cmp, out IRecipe? recipe)) {
					if (Editing) {
						if ((CurrentTab?.Category?.UseRules ?? false) && (CurrentTab?.Category?.CachedRecipes?.Contains(recipe) ?? false)) {
							if (playSound)
								Game1.playSound("stoneStep");
						} else {
							UpdateRecipeInCategory(recipe);
							if (playSound)
								Game1.playSound("smallSelect");
						}

					} else if (!cmp.hoverText.Equals("ghosted")) {
						PerformAction(recipe, Mod.Config.LeftClick, playSound);

					}

					cmp.scale = cmp.baseScale;
					return;
				}
			}

		// Transfer To
		if (!Editing && btnTransferTo != null && btnTransferTo.containsPoint(x, y) && CachedInventories!.Count > 0) {
			HandleTransferClick(false, false);
			btnTransferTo.scale = btnTransferTo.baseScale;
			return;
		}

		// Transfer From
		if (!Editing && btnTransferFrom != null && btnTransferFrom.containsPoint(x, y) && CachedInventories!.Count > 0) {
			HandleTransferClick(true, false);
			btnTransferFrom.scale = btnTransferFrom.baseScale;
			return;
		}

		// Search
		if (btnSearch != null && btnSearch.containsPoint(x, y)) {
			ToggleSearch();
			btnSearch.scale = btnSearch.baseScale;
			return;
		}

		// Toggle Editing
		if (btnToggleEdit != null && btnToggleEdit.containsPoint(x, y)) {
			if (playSound)
				Game1.playSound("smallSelect");
			ToggleEditMode();
			btnToggleEdit.scale = btnToggleEdit.baseScale;
			return;
		}

		// Settings
		if (btnSettings != null && btnSettings.containsPoint(x, y)) {
			if (readyToClose()) {
				if (playSound)
					Game1.playSound("smallSelect");
				Mod.OpenGMCM();
			}
			return;
		}

		// Toggle Favorites
		if (!Editing && btnToggleFavorites != null && btnToggleFavorites.containsPoint(x, y)) {
			if (playSound)
				Game1.playSound("smallSelect");
			FavoritesOnly = !FavoritesOnly;
			LayoutRecipes();
			btnToggleFavorites.sourceRect = SourceFavorites;
			btnToggleFavorites.scale = btnToggleFavorites.baseScale;
			return;
		}

		// Toggle Seasoning
		if (!Editing && btnToggleSeasoning != null && btnToggleSeasoning.containsPoint(x, y)) {
			if (playSound)
				Game1.playSound("smallSelect");

			Mod.Config.UseSeasoning = CommonHelper.Cycle(Mod.Config.UseSeasoning, -1);
			Mod.SaveConfig();

			btnToggleSeasoning.sourceRect = SourceSeasoning;
			btnToggleSeasoning.scale = btnToggleSeasoning.baseScale;
		}

		// Toggle Quality
		if (!Editing && btnToggleQuality != null && btnToggleQuality.containsPoint(x, y)) {
			if (playSound)
				Game1.playSound("smallSelect");

			Mod.Config.MaxQuality = CommonHelper.Cycle(Mod.Config.MaxQuality, 1, [MaxQuality.Disabled]);
			Mod.SaveConfig();
			ClearCraftCache();

			btnToggleQuality.sourceRect = SourceQuality;
			btnToggleQuality.scale = btnToggleQuality.baseScale;
		}

		// Toggle Uniform
		if (btnToggleUniform != null && btnToggleUniform.containsPoint(x, y)) {
			if (playSound)
				Game1.playSound("smallSelect");

			Mod.Config.UseUniformGrid = !Mod.Config.UseUniformGrid;
			Mod.SaveConfig();

			LayoutRecipes();
			btnToggleUniform.sourceRect = SourceUniform;
			btnToggleUniform.scale = btnToggleUniform.baseScale;
		}

		// Trash / Recycle
		if (!Editing && (trashCan?.containsPoint(x, y) ?? false)) {
			if (HeldItem is null && Mod.Config.RecycleClickToggle) {
				var mode = Cooking ? Mod.Config.RecycleCooking : Mod.Config.RecycleCrafting;
				if (mode == RecyclingMode.Enabled)
					mode = RecyclingMode.Disabled;
				else if (mode == RecyclingMode.Disabled)
					mode = RecyclingMode.Enabled;
				else
					return;

				if (Cooking)
					Mod.Config.RecycleCooking = mode;
				else
					Mod.Config.RecycleCrafting = mode;

				Mod.SaveConfig();

				if (playSound)
					Game1.playSound("smallSelect");

			} else
				RecycleOrTrash();
			return;
		}

		// Toss Item
		if (!Editing && !isWithinBounds(x, y) && (HeldItem?.canBeTrashed() ?? false)) {
			if (InventoryItems != null && InventoryItems.Contains(HeldItem))
				return;

			if (playSound)
				Game1.playSound("throwDownITem");
			Game1.createItemDebris(HeldItem, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
			HeldItem = null;
			return;
		}
	}

	protected void RecycleOrTrash(bool once = false) {
		if (HeldItem is null || !HeldItem.canBeTrashed())
			return;

		if (Recycling) {
			var recycling = HeldItemRecyclable.Value;
			if (Recycling && recycling.HasValue) {
				var item = recycling.Value.Item1;
				var recipe = recycling.Value.Item2;
				IRecyclable[] recyclable = recycling.Value.Item3;

				bool handled = false;

				bool recycle_fuzzy = Mod.Config.EffectiveRecycleFuzzyItems;

				int remaining = item.Stack;
				while (remaining >= recipe.QuantityPerCraft) {
					List<Item> recovered = [];

					foreach (IRecyclable ingredient in recyclable) {
						var result = ingredient.Recycle(Game1.player, item, recycle_fuzzy);
						if (result is not null)
							recovered.AddRange(result);
					}

					foreach (var recitem in recovered) {
						if (recitem is null)
							continue;
						Item leftover = Game1.player.addItemToInventory(recitem);
						if (leftover is not null)
							Game1.createItemDebris(leftover, Game1.player.getStandingPosition(), Game1.player.FacingDirection, Game1.player.currentLocation);
					}

					handled = true;
					remaining -= recipe.QuantityPerCraft;
					item.Stack = remaining;

					if (once)
						break;
				}

				if (handled)
					Game1.playSound("dirtyHit");

				if (remaining > 0)
					HeldItem = item;
				else
					HeldItem = null;
			}

		} else if (once && HeldItem.Stack > 1) {
			Item one = HeldItem.getOne();
			one.Stack = 1;
			HeldItem.Stack -= 1;

			Utility.trashItem(one);

		} else {
			Utility.trashItem(HeldItem);
			HeldItem = null;
		}

		// Ensure that the tool-tip is updated after we do stuff.
		hoverMode = int.MinValue;
	}

	public override void receiveRightClick(int x, int y, bool playSound = true) {
		if (GetChildMenu() is IClickableMenu menu) {
			if (!Standalone)
				menu.receiveRightClick(x, y, playSound);
			return;
		}

		if (!Editing)
			HeldItem = inventory.rightClick(x, y, HeldItem, playSound);

		// Transfer To
		if (!Editing && btnTransferTo != null && btnTransferTo.containsPoint(x, y) && CachedInventories!.Count > 0) {
			HandleTransferClick(false, true);
			btnTransferTo.scale = btnTransferTo.baseScale;
			return;
		}

		// Transfer To
		if (!Editing && btnTransferFrom != null && btnTransferFrom.containsPoint(x, y) && CachedInventories!.Count > 0) {
			HandleTransferClick(true, true);
			btnTransferFrom.scale = btnTransferFrom.baseScale;
			return;
		}

		// Toggle Seasoning
		if (!Editing && btnToggleSeasoning != null && btnToggleSeasoning.containsPoint(x, y)) {
			if (playSound)
				Game1.playSound("smallSelect");

			Mod.Config.UseSeasoning = CommonHelper.Cycle(Mod.Config.UseSeasoning, 1);
			Mod.SaveConfig();

			btnToggleSeasoning.sourceRect = SourceSeasoning;
			btnToggleSeasoning.scale = btnToggleSeasoning.baseScale;
		}

		// Toggle Quality
		if (!Editing && btnToggleQuality != null && btnToggleQuality.containsPoint(x, y)) {
			if (playSound)
				Game1.playSound("smallSelect");

			Mod.Config.MaxQuality = CommonHelper.Cycle(Mod.Config.MaxQuality, -1, [MaxQuality.Disabled]);
			Mod.SaveConfig();
			ClearCraftCache();

			btnToggleQuality.sourceRect = SourceQuality;
			btnToggleQuality.scale = btnToggleQuality.baseScale;
		}

		// Right click to favorite components
		if (CurrentPage != null)
			foreach (var cmp in CurrentPage) {
				if (cmp.containsPoint(x, y) && ComponentRecipes.TryGetValue(cmp, out IRecipe? recipe)) {
					if (Editing) {
						UpdateCategorySprite(recipe);
						if (playSound)
							Game1.playSound("smallSelect");

					} else if (!cmp.hoverText.Equals("ghosted")) {
						PerformAction(recipe, Mod.Config.RightClick, playSound);
					}

					cmp.scale = cmp.baseScale;
					return;
				}
			}

		// Trash / Recycle
		if (!Editing && (trashCan?.containsPoint(x, y) ?? false)) {
			RecycleOrTrash(true);
			return;
		}

		// ???
	}

	public bool PerformAction(IRecipe recipe, ButtonAction action, bool playSound = true) {
		switch (action) {
			case ButtonAction.Craft:
				bool shifting = Game1.oldKBState.IsKeyDown(Keys.LeftShift);
				bool ctrling = Game1.oldKBState.IsKeyDown(Keys.LeftControl);

				bool shifted = shifting && recipe.Stackable;
				PerformCraft(recipe, shifted ? (ctrling ? 25 : 5) : 1, moveResultToInventory: shifting);
				return true;

			case ButtonAction.BulkCraft:
				if (OpenBulkCraft(recipe)) {
					if (playSound)
						Game1.playSound("bigSelect");
					return true;
				}
				break;

			case ButtonAction.Favorite:
				if (ToggleFavorite(recipe)) {
					if (playSound)
						Game1.playSound("coin");
				}
				break;
		}

		return false;
	}

	private void RefreshCategoryFilters(Category category) {
		if (category is null)
			return;

		category.CachedRules = Mod.Recipes.HydrateDynamicRules(category.DynamicRules);

		List<IRecipe> recipes = [];
		category.CachedRecipes = recipes;

		if (category.CachedRules is not null) {
			foreach (IRecipe recipe in Recipes) {
				Lazy<Item?> result = new(() => recipe.CreateItemSafe(CreateLog));

				bool matched = false;

				foreach (var handler in category.CachedRules) {
					if (handler.Item3.Inverted) {
						if (matched && handler.Item1.DoesRecipeMatch(recipe, result, handler.Item2))
							matched = false;

					} else if (!matched && handler.Item1.DoesRecipeMatch(recipe, result, handler.Item2))
						matched = true;
				}

				if (matched)
					recipes.Add(recipe);
			}
		}
	}

	public bool OpenRulePicker(Category category) {
		if (!Editing || category is null)
			return false;

		void OnFinished(DynamicRuleData? data, bool openEditor) {
			if (data is null)
				return;

			List<DynamicRuleData> dynamicRuleDatas = [];
			category.DynamicRules ??= dynamicRuleDatas;
			category.DynamicRules.Add(data);

			RefreshCategoryFilters(category);
			UpdateFlow();
			PositionTabMoveButtons();

			if (openEditor)
				OpenRuleEditor(category, category.DynamicRules.Count - 1);
		}

		HashSet<string> existing = [];
		if (category.DynamicRules is not null) {
			foreach (var entry in category.DynamicRules)
				existing.Add(entry.Id);
		}

		int w = width - 128;
		int h = height - 256;

		var pos = Utility.getTopLeftPositionForCenteringOnScreen(w, h);

		var menu = new RulePickerDialog(Mod, this, (int) pos.X, (int) pos.Y, w, h, existing, OnFinished) {
			exitFunction = () => {
				if (Game1.options.SnappyMenus)
					snapCursorToCurrentSnappedComponent();
			}
		};

		SetChildMenu(menu);
		performHoverAction(0, 0);
		return true;
	}

	public void ToggleRuleInverted(Category category, int index) {
		if (!Editing || category is null || category.DynamicRules is null || category.DynamicRules.Count <= index || index < 0)
			return;

		DynamicRuleData data = category.DynamicRules[index];

		data.Inverted = !data.Inverted;

		category.DynamicRules[index] = data;
		RefreshCategoryFilters(category);
		PositionTabMoveButtons();
		UpdateFlow();
	}

	public bool OpenRuleEditor(Category category, int index) {
		if (!Editing || category is null || category.DynamicRules is null || category.DynamicRules.Count <= index || index < 0)
			return false;

		DynamicRuleData data = category.DynamicRules[index];

		if (!Mod.Recipes.TryGetRuleHandler(data.Id, out IDynamicRuleHandler? handler))
			handler = Mod.Recipes.GetInvalidRuleHandler();

		object? obj = handler.ParseState(data);

		void OnFinished(bool save, bool delete, DynamicRuleData data) {
			if (delete)
				category.DynamicRules.RemoveAt(index);
			else if (save)
				category.DynamicRules[index] = data;
			else
				return;

			RefreshCategoryFilters(category);
			PositionTabMoveButtons();
			UpdateFlow();
		}

		var menu = new RuleEditorDialog(Mod, this, handler, obj, data, OnFinished) {
			exitFunction = () => {
				if (Game1.options.SnappyMenus)
					snapCursorToCurrentSnappedComponent();
			}
		};

		SetChildMenu(menu);
		performHoverAction(0, 0);
		return true;
	}

	public bool OpenBulkCraft(IRecipe recipe) {
		if (!recipe.Stackable)
			return false;

		bool shifting = Game1.oldKBState.IsKeyDown(Keys.LeftShift);
		bool ctrling = Game1.oldKBState.IsKeyDown(Keys.LeftControl);

		int quantity = recipe.QuantityPerCraft * (shifting ? (ctrling ? 25 : 5) : 1);

		// TODO: Check that we can craft it.
		var bulk = new BulkCraftingMenu(Mod, this, recipe, quantity) {
			exitFunction = () => {
				if (Game1.options.SnappyMenus)
					snapCursorToCurrentSnappedComponent();
				if (craftingRemaining <= 0)
					activeRecipe = null;
			}
		};

		activeRecipe = recipe;
		SetChildMenu(bulk);
		performHoverAction(0, 0);
		return true;
	}

	public bool ToggleFavorite(IRecipe recipe, bool? favorite = null) {
		bool fav = favorite ?? !Favorites.Contains(recipe);

		if (Favorites.Contains(recipe) == fav)
			return false;

		Mod.Favorites.SetFavoriteRecipe(recipe.Name, cooking, fav, Game1.player);
		Mod.Favorites.SaveFavorites();

		// Update our favorite tracking.
		if (fav)
			Favorites.Add(recipe);
		else
			Favorites.Remove(recipe);

		// Should we modify tabs?
		if (Mod.Config.UseCategories) {
			bool wants_favorites = Favorites.Count > 0;
			bool is_favorites = CurrentTab.Category.Id == "favorites";
			bool has_favorites = is_favorites || Tabs[0].Category.Id == "favorites";
			if (wants_favorites != has_favorites)
				UpdateTabs();
			if (wants_favorites != has_favorites || is_favorites)
				LayoutRecipes();
		}

		return true;
	}

	public override void performHoverAction(int x, int y) {
		base.performHoverAction(x, y);

		hoverTitle = "";
		hoverText = "";
		hoverRecipe = null;
		hoverAmount = -1;

		bool canRecycle = (Cooking ? Mod.Config.RecycleCooking : Mod.Config.RecycleCrafting) switch {
			RecyclingMode.Enabled => true,
			RecyclingMode.Automatic => HeldItemRecyclable.Value.HasValue,
			_ => false
		};
		if (canRecycle != Recycling) {
			Recycling = canRecycle;
			trashCan.texture = Recycling ? RecyclingBinTexture : Game1.mouseCursors;
			trashCan.sourceRect = SourceTrashCan;
		}

		if (GetChildMenu() is IClickableMenu menu) {
			hoverMode = -1;
			hoverNode = null;
			hoverItem = null;

			if (!Standalone)
				menu.performHoverAction(x, y);
			return;
		}

		if (!Editing)
			hoverItem = inventory.hover(x, y, hoverItem);
		else
			hoverItem = null;

		if (hoverItem != null) {
			hoverTitle = inventory.hoverTitle;
			hoverText = inventory.hoverText;
		}

		// Editing
		txtCategoryName?.Hover(x, y);

		// Recipes
		if (CurrentPage != null)
			foreach (var cmp in CurrentPage) {
				if (cmp.containsPoint(x, y)) {
					if (!Editing && cmp.hoverText.Equals("ghosted")) {
						hoverText = "???";
					} else {
						ComponentRecipes.TryGetValue(cmp, out hoverRecipe);

						if (Editing && hoverRecipe is not null &&
							CurrentTab?.Category?.CachedRecipes is not null &&
							CurrentTab.Category.UseRules &&
							CurrentTab.Category.CachedRecipes.Contains(hoverRecipe)
						)
							cmp.scale = Math.Max(cmp.scale - 0.02f, cmp.baseScale);
						else
							cmp.scale = Math.Min(cmp.scale + 0.02f, cmp.baseScale + 0.1f);
					}
				} else
					cmp.scale = Math.Max(cmp.scale - 0.02f, cmp.baseScale);
			}

		// HoveredItem for Lookup Anything
		HoveredItem = hoverItem ?? lastRecipeHover.Value;

		// Tabs
		int mode = -1;

		if (Tabs.Count > 1) {
			bool hover_tabs = false;
			for (int i = 0; i < Tabs.Count; i++) {
				TabInfo tab = Tabs[i];
				if (tab.Component.visible && tab.Component.containsPoint(x, y)) {
					hover_tabs = true;

					if (!Editing && Filter != null && tab.FilteredRecipes.Count == 0) {
						/* Nothing */
					} else
						hoverText = tab.Component.label;

					break;
				}
			}

			if (!hover_tabs && Favorites.Count == 0 && tabIndex > 0 && Tabs[0].Category.Id == "favorites") {
				Tabs.RemoveAt(0);
				tabIndex--;

				if (Tabs.Count <= MAX_TABS && btnTabsDown != null) {
					allClickableComponents?.Remove(btnTabsUp);
					allClickableComponents?.Remove(btnTabsDown);

					btnTabsDown = null;
					btnTabsUp = null;
				}

				PositionTabs();
			}
		}


		// Navigation Buttons
		btnTabsDown?.tryHover(x, (TabScroll + VISIBLE_TABS) >= Tabs.Count ? -1 : y);
		btnTabsUp?.tryHover(x, TabScroll > 0 ? y : -1);

		btnPageUp?.tryHover(x, pageIndex > 0 ? y : -1);
		btnPageDown?.tryHover(x, pageIndex < Pages.Count - 1 ? y : -1);

		btnCatUp?.tryHover(x, btnCatUp.visible ? y : -1);
		btnCatDown?.tryHover(x, btnCatDown.visible ? y : -1);

		// Transfer Buttons
		if (btnTransferTo != null && CachedInventories!.Count > 0) {
			btnTransferTo.tryHover(x, y);
			if (btnTransferTo.containsPoint(x, y)) {
				mode = (Mod.Config.ModiferKey?.IsDown() ?? false) ? 10 : 9;
				if (HeldItem is not null)
					hoverMode = 0;
			}
		}

		if (btnTransferFrom != null && CachedInventories!.Count > 0) {
			btnTransferFrom.tryHover(x, y);
			if (btnTransferFrom.containsPoint(x, y)) {
				mode = (Mod.Config.ModiferKey?.IsDown() ?? false) ? 12 : 11;
				if (HeldItem is not null)
					hoverMode = 0;
			}
		}

		if (btnCatUp != null && btnCatUp.visible && btnCatUp.containsPoint(x, y))
			mode = 20;
		if (btnCatDown != null && btnCatDown.visible && btnCatDown.containsPoint(x, y))
			mode = 21;

		// Toggle Buttons
		if (btnSearch != null) {
			btnSearch.tryHover(x, y);
			if (btnSearch.containsPoint(x, y))
				mode = Filter == null ? 7 : 8;
		}

		if (btnToggleEdit != null) {
			btnToggleEdit.tryHover(x, y);
			if (btnToggleEdit.containsPoint(x, y))
				mode = 0;
		}

		if (btnSettings != null) {
			btnSettings.tryHover(x, y);
			if (btnSettings.containsPoint(x, y) && readyToClose())
				mode = 5;
		}

		if (!Editing && btnToggleFavorites != null) {
			btnToggleFavorites.tryHover(x, y);
			if (btnToggleFavorites.containsPoint(x, y))
				mode = 1;
		}

		if (!Editing && btnToggleSeasoning != null) {
			btnToggleSeasoning.tryHover(x, y);
			if (btnToggleSeasoning.containsPoint(x, y))
				mode = 2;
		}

		if (!Editing && btnToggleQuality != null) {
			btnToggleQuality.tryHover(x, y);
			if (btnToggleQuality.containsPoint(x, y))
				mode = 3;
		}

		if (btnToggleUniform != null) {
			btnToggleUniform.tryHover(x, y);
			if (btnToggleUniform.containsPoint(x, y))
				mode = 4;
		}

		if (btnCategoryIcon != null && btnCategoryIcon.containsPoint(x, y))
			mode = 6;

		bool in_misc = IsMiscCategory;

		if (btnCategoryCopy != null) {
			btnCategoryCopy.tryHover(x, in_misc ? -1 : y);
			if (!in_misc && btnCategoryCopy.containsPoint(x, y))
				mode = 22;
		}

		if (btnCategoryPaste != null) {
			btnCategoryPaste.tryHover(x, y);
			if (btnCategoryPaste.containsPoint(x, y))
				mode = 23;
		}

		bool shifting = Mod.Config.ModiferKey?.IsDown() ?? false;

		if (btnCategoryTrash != null) {
			btnCategoryTrash.tryHover(x, (in_misc && !shifting) ? -1 : y);
			if ((!in_misc || shifting) && btnCategoryTrash.containsPoint(x, y))
				mode = shifting ? 25 : 24;
		}

		if (btnCategoryFilter != null) {
			btnCategoryFilter.tryHover(x, in_misc ? -1 : y);
			if (!in_misc && btnCategoryFilter.containsPoint(x, y))
				mode = (CurrentTab?.Category?.UseRules ?? false) ? 13 : 14;
		}

		if (btnCategoryIncludeInMisc != null) {
			btnCategoryIncludeInMisc.tryHover(x, in_misc ? -1 : y);
			if (!in_misc && btnCategoryIncludeInMisc.containsPoint(x, y))
				mode = (CurrentTab?.Category?.IncludeInMisc ?? false) ? 15 : 16;
		}

		if (!Editing && trashCan != null && Recycling && trashCan.containsPoint(x, y))
			mode = 17;

		// If the mode changed, regenerate the fancy tool-tip.
		if (mode != hoverMode) {
			hoverMode = mode;
			hoverNode = null;

			if (mode == 15 || mode == 16) {
				// Toggle Include In Misc.
				Category? cat = CurrentTab?.Category;
				if (cat is not null) {
					var builder = SimpleHelper.Builder()
						.Text(cat.IncludeInMisc ? I18n.Tooltip_IncludeInMisc() : I18n.Tooltip_IncludeInMisc_Disabled())
						.Divider()
						.FormatText(I18n.Tooltip_IncludeInMisc_About(), wrapText: true);

					hoverNode = builder.GetLayout();
				}
			}

			if (mode == 13 || mode == 14) {
				// Toggle Dynamic Filtering
				Category? cat = CurrentTab?.Category;
				if (cat is not null) {
					var builder = SimpleHelper.Builder()
						.Text(cat.UseRules ? I18n.Tooltip_Filter_Enabled() : I18n.Tooltip_Filter_Disabled())
						.Divider()
						.FormatText(I18n.Tooltip_Filter_About(), wrapText: true);

					hoverNode = builder.GetLayout();
				}
			}

			if (mode == 17 && HeldItem is not null) {
				// Recycling
				var builder = SimpleHelper.Builder();
				var recycling = HeldItemRecyclable.Value;

				if (recycling.HasValue) {
					var item = recycling.Value.Item1;
					var recipe = recycling.Value.Item2;

					int multiplier = item.Stack / recipe.QuantityPerCraft;

					if (multiplier <= 0) {
						string label = recipe.DisplayName;
						if (item.Stack > 1)
							label = $"{label} x{item.Stack}";

						builder
							.Text(I18n.Tooltip_Recycle(label))
							.Divider(false)
							.FormatText(I18n.Tooltip_Recycle_Insufficient(recipe.QuantityPerCraft), color: Color.Red, wrapText: true);

					} else {
						IRecyclable[] recyclable = recycling.Value.Item3;
						IIngredient[] nonrecyclable = recycling.Value.Item4;

						List<ISimpleNode> ingredients = [];
						List<ISimpleNode> wasted = [];

						bool recycle_fuzzy = Mod.Config.EffectiveRecycleFuzzyItems;

						foreach (var entry in recyclable) {
							int amount = entry.GetRecycleQuantity(Game1.player, item, recycle_fuzzy) * multiplier;
							Texture2D texture = entry.GetRecycleTexture(Game1.player, item, recycle_fuzzy);
							Rectangle source = entry.GetRecycleSourceRect(Game1.player, item, recycle_fuzzy);
							string displayName = entry.GetRecycleDisplayName(Game1.player, item, recycle_fuzzy);

							var ebuilder = SimpleHelper
								.Builder(LayoutDirection.Horizontal, margin: 8)
								.Sprite(
									new SpriteInfo(texture, source),
									scale: 2,
									quantity: amount,
									align: Alignment.VCenter
								)
								.Text(
									displayName,
									align: Alignment.VCenter
								);

							ingredients.Add(ebuilder.GetLayout());
						}

						foreach (var entry in nonrecyclable) {
							int amount = entry.Quantity * multiplier;

							var ebuilder = SimpleHelper
								.Builder(LayoutDirection.Horizontal, margin: 8)
								.Sprite(
									new SpriteInfo(entry.Texture, entry.SourceRectangle),
									scale: 2,
									quantity: amount,
									align: Alignment.VCenter
								)
								.Text(
									entry.DisplayName,
									align: Alignment.VCenter
								);

							wasted.Add(ebuilder.GetLayout());
						}

						string label = recipe.DisplayName;
						int amnt = multiplier * recipe.QuantityPerCraft;
						if (amnt > 1)
							label = $"{label} x{amnt}";

						builder
							.Text(I18n.Tooltip_Recycle(label))
							.Divider()
							.Text(I18n.Tooltip_Recycle_Returns())
							.AddRange(ingredients);

						if (wasted.Count > 0)
							builder
								.Divider(false)
								.Text(I18n.Tooltip_Recycle_NotReturned())
								.AddRange(wasted);

						TTWhen when = Mod.Config.ShowKeybindTooltip;
						if (when == TTWhen.Always || (when == TTWhen.ForController && Game1.options.gamepadControls)) {
							builder
								.Divider(false)
								.Group(8)
									.Add(GetLeftClickNode())
									.Text(I18n.Tooltip_Recycle_All(amnt))
								.EndGroup()
								.Group(8)
									.Add(GetRightClickNode())
									.Text(I18n.Tooltip_Recycle_Once(recipe.QuantityPerCraft))
								.EndGroup();
						}
					}

				} else {
					string label = HeldItem.DisplayName;
					if (HeldItem.Stack > 1)
						label = $"{label} x{HeldItem.Stack}";

					builder
						.Text(I18n.Tooltip_Recycle(label))
						.Divider(false)
						.FormatText(I18n.Tooltip_Recycle_Invalid(), color: Color.Red, wrapText: true);
				}

				hoverNode = builder.GetLayout();
			}

			if (mode == 8) {
				string? filter = Filter;

				var builder = SimpleHelper.Builder()
					.Text(I18n.Tooltip_Search());

				if (filter != null) {
					if (FilterIngredients)
						filter = $"{I18n.Search_IngredientPrefix()}{filter}";

					if (FilterLikes)
						filter = $"{I18n.Search_LikePrefix()}{filter}";

					if (FilterLoves)
						filter = $"{I18n.Search_LovePrefix()}{filter}";

					builder
						.Divider()
						.Text(filter, color: Game1.textColor * 0.6f);
				}

				hoverNode = builder.GetLayout();
			}

			if (mode >= 9 && mode <= 12) {
				bool fill = mode == 11 || mode == 12;
				Behaviors behaviors = fill ? Mod.Config.FillFromBehaviors : Mod.Config.AddToBehaviors;

				var builder = SimpleHelper.Builder()
					.Text(fill ?
						(HeldItem is null ? I18n.Tooltip_Transfer_From() : I18n.Tooltip_Transfer_FromItem()) :
						(HeldItem is null ? I18n.Tooltip_Transfer_To() : I18n.Tooltip_Transfer_ToItem())
					)
					.Divider(HeldItem is not null);

				if (HeldItem != null)
					builder
						.Sprite(SpriteHelper.GetSprite(HeldItem), scale: 2, label: HeldItem.DisplayName, quantity: HeldItem.Stack)
						.Divider(false);

				bool modified = Mod.Config.ModiferKey?.IsDown() ?? false;
				TransferBehavior behavior = modified ? behaviors.UseToolModified : behaviors.UseTool;
				if (behavior.Mode != TransferMode.None)
					builder.Group()
						.Add(GetLeftClickNode())
						.Text(GetBehaviorTip(behavior))
						.EndGroup();

				behavior = modified ? behaviors.ActionModified : behaviors.Action;
				if (behavior.Mode != TransferMode.None)
					builder.Group()
						.Add(GetRightClickNode())
						.Text(GetBehaviorTip(behavior))
						.EndGroup();

				hoverNode = builder.GetLayout();
			}

		} else {
			string smode;

			switch (mode) {
				case 0:
					hoverText = I18n.Tooltip_EditMode();
					break;

				case 1:
					// Toggle Favorites
					hoverText = I18n.Tooltip_Favorites();
					break;

				case 2:
					// Toggle Seasoning
					smode = Mod.Config.UseSeasoning switch {
						SeasoningMode.Enabled => I18n.Seasoning_Enabled(),
						SeasoningMode.InventoryOnly => I18n.Seasoning_Inventory(),
						_ => I18n.Seasoning_Disabled(),
					};

					hoverText = $"{I18n.Tooltip_Seasoning()}\n{smode}";
					break;

				case 3:
					// Toggle Quality
					hoverText = Mod.Config.MaxQuality switch {
						MaxQuality.Silver => I18n.Tooltip_Quality_Silver(),
						MaxQuality.Gold => I18n.Tooltip_Quality_Gold(),
						MaxQuality.Iridium => I18n.Tooltip_Quality_Iridium(),
						_ => I18n.Tooltip_Quality_None(),
					};
					break;

				case 4:
					// Toggle Uniform
					hoverText = I18n.Tooltip_Uniform();
					break;

				case 5:
					// Open Settings
					hoverText = I18n.Tooltip_Settings();
					break;

				case 6:
					// Open Icon Picker
					hoverText = I18n.Tooltip_SelectIcon();
					break;

				case 7:
					// Search
					hoverText = I18n.Tooltip_Search();
					break;

				case 20:
					// Category Move Up
					hoverText = I18n.Tooltip_MoveUp();
					break;

				case 21:
					// Category Move Down
					hoverText = I18n.Tooltip_MoveDown();
					break;

				case 22:
					// Copy
					hoverText = I18n.Tooltip_CopyCat();
					break;

				case 23:
					// Paste
					hoverText = I18n.Tooltip_PasteCat();
					break;

				case 24:
					// Delete Category
					hoverText = I18n.Tooltip_TrashCat();
					break;

				case 25:
					// Reset Categories
					hoverText = I18n.Tooltip_TrashAll();
					break;

				default:
					break;
			}
		}

		// Trash Can
		if (!Editing && trashCan != null) {
			if (trashCan.containsPoint(x, y)) {
				bool rotate = !Recycling || HeldItemRecyclable.Value.HasValue;
				if (rotate) {
					if (trashCanLidRotation <= 0)
						Game1.playSound("trashcanlid");

					trashCanLidRotation = Math.Min(trashCanLidRotation + (float) Math.PI / 48f, 1.570796f);
				} else
					trashCanLidRotation = Math.Max(trashCanLidRotation - (float) Math.PI / 48f, 0f);

				if (HeldItem != null && !Recycling) {
					hoverAmount = Utility.getTrashReclamationPrice(HeldItem, Game1.player);
					if (hoverAmount > 0)
						hoverText = Game1.content.LoadString("Strings\\UI:TrashCanSale");
				}

			} else
				trashCanLidRotation = Math.Max(trashCanLidRotation - (float) Math.PI / 48f, 0f);
		}

		if (Editing && Flow is not null && (CurrentTab?.Category?.UseRules ?? false)) {
			if (Flow.PerformMiddleScroll(x, y))
				return;

			Flow.PerformHover(x, y);
		}
	}

	public override bool readyToClose() {
		if (!Standalone && GetChildMenu() != null)
			return false;

		if (!Standalone && (txtCategoryName?.Selected ?? false))
			return false;

		return HeldItem == null;
	}

	#endregion

	#region Caching Can Craft

	public void MaybeClearCraftCache() {

		long now = DateTime.UtcNow.Ticks;

		bool should_uncache = false;

		if (!Editing && now - CraftCachedAt > TimeSpan.TicksPerSecond)
			should_uncache = true;

		else if (Game1.player.Items.LastTickSlotChanged > CraftCachedAt)
			should_uncache = true;

		else if (UnsafeInventories != null)
			foreach (var inv in UnsafeInventories) {
				if (inv.Inventory is not null && inv.Inventory.LastTickSlotChanged > CraftCachedAt) {
					should_uncache = true;
					break;
				}
			}

		if (should_uncache)
			ClearCraftCache();
	}

	public void ClearCraftCache(IRecipe? recipe = null) {
		ClearTooltipCache();

		if (recipe is not null)
			CanCraftCache?.Remove(recipe);
		else
			CanCraftCache = null;
	}

	public bool CanCraft(IRecipe recipe, Lazy<IList<Item?>?>? itemListGetter) {
		if (CanCraftCache == null) {
			CanCraftCache = [];
			CraftCachedAt = DateTime.UtcNow.Ticks;
		}

		if (CanCraftCache.TryGetValue(recipe, out bool value))
			return value;

		if (!recipe.CanCraft(Game1.player))
			value = false;
		else {
			IList<Item?>? items = itemListGetter?.Value ?? GetEstimatedContainerContents();
			value = recipe.HasIngredients(Game1.player, items, UnsafeInventories, Quality);
		}

		CanCraftCache[recipe] = value;
		return value;
	}

	#endregion

	#region Drawing

	public override void draw(SpriteBatch b) {

#if DEBUG
		var watch = ShowTiming ? Stopwatch.StartNew() : null;
#endif

		MaybeClearCraftCache();

		// Background
		if (Standalone && DrawBG)
			b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);

		// Workstation Label
		if (Standalone && StationLabel is not null) {
			var sz = StationLabel.GetSize(Game1.dialogueFont, Vector2.Zero);
			DrawSimpleNodeHover(
				StationLabel,
				b,
				Game1.dialogueFont,
				overrideX: (int) (xPositionOnScreen + 64),
				overrideY: (int) (yPositionOnScreen - sz.Y + 66)
			);
		}

		// Draw custom dialog.
		if (Standalone || Background is not null)
			RenderHelper.DrawDialogueBox(
				b,
				xPositionOnScreen,
				yPositionOnScreen + 64,
				width,
				height - 64,
				texture: Background,
				sources: Background is null ? null : RenderHelper.Sprites.CustomBCraft
			);

		if (!Editing) {
			RenderHelper.DrawHorizontalPartition(
				b,
				xPositionOnScreen,
				inventory.yPositionOnScreen - borderWidth - 16,
				width,
				texture: Background,
				sources: Background is null ? null : RenderHelper.Sprites.CustomBCraft
			);
		}

		// Inventory
		if (!Editing) {
			if (Background is not null)
				inventory.DrawCustomSlots(b, Background, Sprites.Other.SLOT_BORDERS, Sprites.Other.SLOT_DISABLED);

			inventory.drawSlots = Background is null;
			inventory.draw(b);
		}

		// Editing
		if (Editing) {
			RenderHelper.DrawHorizontalPartition(
				b,
				xPositionOnScreen,
				yPositionOnScreen + spaceToClearTopBorder + 64,
				width,
				texture: Background,
				sources: Background is null ? null : RenderHelper.Sprites.CustomBCraft
			);

			CurrentTab.Sprite?.Draw(b, new Vector2(
				BasePageX() + 12 - 16,
				yPositionOnScreen + spaceToClearTopBorder + 12
			), 4f);

			bool is_misc = IsMiscCategory;
			Color faded_in_misc = is_misc ? Color.Black * 0.35f : Color.White;

			txtCategoryName?.Draw(b);
			btnCategoryFilter?.draw(b, faded_in_misc, 1f);
			btnCategoryIncludeInMisc?.draw(b, faded_in_misc, 1f);

			bool modded = Mod.Config.ModiferKey?.IsDown() ?? false;

			btnCategoryCopy?.draw(b, faded_in_misc, 1f);
			btnCategoryPaste?.draw(b);
			btnCategoryTrash?.draw(b, modded ? Color.White : faded_in_misc, 1f);

			btnCatUp?.draw(b);
			btnCatDown?.draw(b);

			if (CurrentTab?.Category?.UseRules ?? false) {
				RenderHelper.DrawVerticalPartition(
					b,
					xPositionOnScreen + 432,
					yPositionOnScreen + spaceToClearTopBorder + 72,
					height - spaceToClearTopBorder - 72,
					texture: Background,
					sources: Background is null ? null : RenderHelper.Sprites.CustomBCraft
				);

				Flow?.Draw(b, Theme.TextColor, Theme.TextShadowColor);
				Flow?.DrawMiddleScroll(b);
			}
		}

		// Buttons
		btnSearch?.draw(b);
		btnToggleEdit?.draw(b);
		if (btnSettings != null && readyToClose())
			btnSettings.draw(b);
		btnToggleUniform?.draw(b);

		if (!Editing) {
			if (CachedInventories!.Count > 0) {
				btnTransferTo?.draw(b);
				btnTransferFrom?.draw(b);
			}

			btnToggleFavorites?.draw(b);
			btnToggleSeasoning?.draw(b);
			btnToggleQuality?.draw(b);
		}

		// Trash
		if (!Editing && trashCan != null) {
			trashCan.draw(b);
			b.Draw(
				Recycling ? RecyclingBinTexture : Game1.mouseCursors,
				new Vector2(trashCan.bounds.X + 60, trashCan.bounds.Y + 40),
				SourceTrashCanLid,
				Color.White,
				trashCanLidRotation,
				new Vector2(16f, 10f),
				4f,
				SpriteEffects.None,
				0.86f
			);
		}


		// Tabs

		if (btnTabsDown != null) {
			if (TabScroll + VISIBLE_TABS >= Tabs.Count)
				btnTabsDown.draw(b, Color.Gray, 0.89f);
			else
				btnTabsDown.draw(b);
		}

		if (btnTabsUp != null) {
			if (TabScroll == 0)
				btnTabsUp.draw(b, Color.Gray, 0.89f);
			else
				btnTabsUp.draw(b);
		}

		b.End();
		b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

		if (Editing || Tabs.Count > 1)
			for (int i = 0; i < Tabs.Count; i++) {
				TabInfo tab = Tabs[i];
				if (tab == null || tab.Component == null || !tab.Component.visible)
					continue;

				int x = tab.Component.bounds.X + (tabIndex == i ? 8 : 0);

				bool filtered = !Editing && Filter != null && tab.FilteredRecipes.Count == 0;

				// We rotate the tab, so we can reuse the main sprite sheet.
				b.Draw(
					Background ?? Game1.mouseCursors,
					new Vector2(x, tab.Component.bounds.Y),
					Background is null
						? SpriteHelper.Tabs.BACKGROUND
						: Sprites.Other.TAB_BLANK,
					filtered ? Color.DarkGray : Color.White,
					3 * (float) Math.PI / 2f,
					new Vector2(16, 0),
					4f,
					SpriteEffects.None,
					0.0001f
				);

				tab.Sprite?.Draw(
					b,
					new Vector2(x + 14, tab.Component.bounds.Y + 8),
					3f,
					alpha: filtered ? .35f : 1f,
					baseColor: filtered ? Color.Black : Color.White
				);
			}


		// Recipes
		List<ClickableTextureComponent> recipes = CurrentPage;
		Lazy<IList<Item?>?>? items = new(GetEstimatedContainerContents);

		bool shifting = Game1.oldKBState.IsKeyDown(Keys.LeftShift) && GetChildMenu() == null;
		bool ctrling = Game1.oldKBState.IsKeyDown(Keys.LeftControl);

		bool drawn = false;

		if (recipes != null) {
			int i = 0;
			foreach (var cmp in recipes) {
				if (!ComponentRecipes.TryGetValue(cmp, out IRecipe? recipe))
					continue;

				bool ghosted = cmp.hoverText.Equals("ghosted");
				bool in_category = Editing && IsRecipeInCategory(recipe);
				bool shifted = !Editing && shifting && recipe.Stackable;
				IDynamicDrawingRecipe? ddr = recipe as IDynamicDrawingRecipe;

				bool draw_dynamic = ddr is not null && ddr.ShouldDoDynamicDrawing;
				float drawDepth = recipe == hoverRecipe ? 0.90f : 0.89f;
				Color qualityColor = Color.White;

				if (!Editing && ghosted) {
					// Unlearned Recipe
					drawn = true;
					if (draw_dynamic)
						ddr!.Draw(b, cmp.bounds, Color.Black * 0.35f, true, false, drawDepth, cmp);
					else
						cmp.DrawBounded(b, Color.Black * 0.35f, drawDepth);

					qualityColor = Color.Black * 0.35f;

				} else if (Editing ? !in_category : !CanCraft(recipe, items)) {
					// Recipe without Ingredients
					drawn = true;
					if (draw_dynamic)
						ddr!.Draw(b, cmp.bounds, Color.DimGray * 0.4f, false, false, drawDepth, cmp);
					else
						cmp.DrawBounded(b, Color.DimGray * 0.4f, drawDepth);

					int count = recipe.QuantityPerCraft * (shifted ? (ctrling ? 25 : 5) : 1);
					if (count > 1)
						NumberSprite.draw(
							count,
							b,
							new Vector2(cmp.bounds.X + cmp.bounds.Width - 2, cmp.bounds.Y + cmp.bounds.Height - 2),
							Color.LightGray * 0.75f,
							(float) (0.5 * (cmp.scale / 4.0)),
							0.97f,
							1f,
							0
						);

				} else {
					// Craftable Recipe
					drawn = true;
					if (draw_dynamic)
						ddr!.Draw(b, cmp.bounds, Color.White, false, true, drawDepth, cmp);
					else
						cmp.DrawBounded(b, layerDepth: drawDepth);

					int count = recipe.QuantityPerCraft * (shifted ? (ctrling ? 25 : 5) : 1);
					if (count > 1)
						NumberSprite.draw(
							count,
							b,
							new Vector2(cmp.bounds.X + cmp.bounds.Width - 2, cmp.bounds.Y + cmp.bounds.Height - 2),
							Color.White,
							(float) (0.5 * (cmp.scale / 4.0)),
							0.97f,
							1f,
							0
						);
				}

				int quality = GetRecipeQuality(recipe);
				if (quality > 0) {
					Rectangle qualityRect = quality < 4
						? new Rectangle(338 + (quality - 1) * 8, 400, 8, 8)
						: new Rectangle(346, 392, 8, 8);

					/*float qualityYOffset = (quality < 4)
						? 0f :
						(((float) Math.Cos(Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1f) * 0.05f);*/

					int qHeight = (int) (qualityRect.Height * 2f);

					b.Draw(
						Game1.mouseCursors,
						new Vector2(
							cmp.bounds.X,
							cmp.bounds.Y + (cmp.bounds.Height - qHeight)
						),
						qualityRect,
						qualityColor,
						0f,
						Vector2.Zero,
						2f,
						SpriteEffects.None,
						1f
					);

				}

				bool is_new = !Editing && !ghosted && Mod.Config.NewRecipes switch {
					NewRecipeMode.Uncrafted => recipe.GetTimesCrafted(Game1.player) == 0,
					NewRecipeMode.Unseen => !Mod.Recipes.HasSeenRecipe(Game1.player, recipe),
					_ => false
				};

				if (is_new)
					b.Draw(
						Background ?? Game1.mouseCursors,
						new Vector2(cmp.bounds.X - 6, cmp.bounds.Y - 6),
						Background is null
							? new Rectangle(141, 438, 20, 9)
							: Sprites.Other.NEW_LABEL,
						Mod.Config.NewRecipesPrismatic ? Utility.GetPrismaticColor(offset: i, speedMultiplier: 1) : Color.White,
						0f,
						Vector2.Zero,
						2f,
						SpriteEffects.None,
						1f
					);

				if (Favorites.Contains(recipe))
					b.Draw(
						Game1.mouseCursors,
						new Vector2(cmp.bounds.X + cmp.bounds.Width - 12, cmp.bounds.Y - 4),
						FAV_STAR,
						Color.White,
						0f,
						Vector2.Zero,
						2f,
						SpriteEffects.None,
						1f
					);

				i++;
			}
		}

		if (!drawn) {
			string none = Mod.Config.UseCategories ?
				I18n.Search_None_Category() :
				I18n.Search_None();

			Vector2 size = Game1.smallFont.MeasureString(none);

			b.DrawString(
				Game1.smallFont,
				none,
				new Vector2(
					xPositionOnScreen + (width - size.X) / 2,
					yPositionOnScreen + 256
				),
				Theme.TextColor ?? Game1.textColor
			);
		}

		b.End();
		b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

		// Base Menu
		base.draw(b);

		if (pageIndex < Pages.Count - 1)
			btnPageDown?.draw(b);
		else
			btnPageDown?.draw(b, Color.Black * 0.35f, 0.89f);
		if (pageIndex > 0)
			btnPageUp?.draw(b);
		else
			btnPageUp?.draw(b, Color.Black * 0.35f, 0.89f);

		if (GetChildMenu() is IClickableMenu menu) {
			if (!Standalone)
				menu.draw(b);
			return;
		}

		// Transfer Sprites
		foreach (var sprite in tSprites)
			sprite.Draw(b);

		// Hover Item
		int offset = HeldItem != null ? 48 : 0;

		if (hoverItem != null)
			IClickableMenu.drawToolTip(b, hoverText, hoverTitle, hoverItem, HeldItem != null);
		else if (hoverNode != null)
			DrawSimpleNodeHover(
				hoverNode,
				b,
				offsetX: offset,
				offsetY: offset
			);
		else if (!string.IsNullOrEmpty(hoverText)) {
			ISimpleNode node = new TextNode(hoverText);
			if (hoverAmount > 0) {
				node = SimpleHelper.Builder()
					.Add(node)
					.Divider(false)
					.Group()
						.Text($"{hoverAmount} ")
						.Texture(Game1.debrisSpriteSheet, new Rectangle(5, 69, 6, 6), scale: 3f)
					.EndGroup()
					.GetLayout();
			}

			DrawSimpleNodeHover(
				node,
				b,
				offsetX: offset,
				offsetY: offset
			);

		}

		// HUD Messages?
		// We need to draw these ourselves to make sure they go over top
		// of our menu, in the event that we have a message to show
		// to the user.
		if (Game1.hudMessages.Count > 0) {
			int heightUsed = 0;
			for (int i = Game1.hudMessages.Count - 1; i >= 0; i--)
				Game1.hudMessages[i].draw(b, i, ref heightUsed);
		}

		// Held Item
		if (!Editing)
			HeldItem?.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 16, Game1.getOldMouseY() + 16), 1f);

		if (Standalone) {
			Game1.mouseCursorTransparency = 1f;
			if (Background is null || !Theme.CustomMouse || !RenderHelper.DrawMouse(b, Background, RenderHelper.Sprites.BCraftMouse, Working ? Common.MouseCursor.Busy : Common.MouseCursor.Auto))
				drawMouse(b, cursor: Working ? 1 : -1);
		}

		var ttip = GetRecipeTooltip(items);
		if (ttip is not null)
			DrawSimpleNodeHover(
				ttip,
				b,
				offsetX: offset,
				offsetY: offset
			);

#if DEBUG
		if (watch != null) {
			watch.Stop();

			SimpleHelper.Builder()
				.Text($"{watch.Elapsed.TotalMilliseconds}ms")
				.GetLayout()
				.DrawHover(b, Game1.smallFont, overrideX: 0, overrideY: 0);
		}
#endif
	}

	public void DrawSimpleNodeHover(ISimpleNode node, SpriteBatch b, SpriteFont? defaultFont = null, int offsetX = 0, int offsetY = 0, int overrideX = -1, int overrideY = -1, float alpha = 1) {
		Texture2D? tex = Theme.CustomTooltip ? Background : null;
		SourceSet? sources = tex is null ? null : RenderHelper.Sprites.CustomBCraft;

		node.DrawHover(
			b,
			defaultFont ?? Game1.smallFont,
			defaultColor: tex is null ? null : Theme.TooltipTextColor ?? Theme.TextColor,
			defaultShadowColor: tex is null ? null : Theme.TooltipTextShadowColor ?? Theme.TextShadowColor,
			offsetX: offsetX,
			offsetY: offsetY,
			overrideX: overrideX,
			overrideY: overrideY,
			alpha: alpha,
			bgTexture: tex,
			bgSource: sources?.ThinBox,
			bgScale: tex is not null ? 4 : 1,
			divTexture: tex,
			divHSource: sources?.ThinHRule,
			divVSource: sources?.ThinVRule
		);
	}


	/// <summary>
	/// Create the static part of an item's tool-tip. This includes the item's
	/// description and icons for any buffs.
	/// </summary>
	/// <param name="item">The item</param>
	/// <returns>A SimpleNode that can be added to the main tool-tip SimpleNode.</returns>
	public ISimpleNode[]? GetRecipeTooltipDescription() {
		if (hoverRecipe is null)
			return null;

		Item? recipeItem = lastRecipeHover.Value;

		var builder = SimpleHelper.Builder();

		// Description
		if (!string.IsNullOrEmpty(hoverRecipe.Description)) {
			if (Filter == null)
				builder.Flow(FlowHelper.Builder()
					.Text(hoverRecipe.Description)
					.Build(),
					wrapText: true
				);
			else
				builder.FormatText(
					HighlightSearchTerms(hoverRecipe.Description), wrapText: true);
		}

		// Extra
		string? extra = hoverRecipe.GetTooltipExtra(Game1.player);
		if (!string.IsNullOrEmpty(extra)) {
			builder.Flow(
				FlowHelper.Builder()
					.FormatText(extra)
					.Build(),
				wrapText: true
			);
		}

		// Buffs
		AddBuffsToTooltip(builder, lastRecipeHover.Value, false, true);

		// If we have nothing, return null
		if (builder.IsEmpty())
			return null;

		return builder.Build();
	}

	public static float[] GetBuffEffects(BuffEffects effects) {
		return [
			effects.FarmingLevel.Value,
			effects.FishingLevel.Value,
			effects.MiningLevel.Value,
			0,
			effects.LuckLevel.Value,
			effects.ForagingLevel.Value,
			0,
			effects.MaxStamina.Value,
			effects.MagneticRadius.Value,
			effects.Speed.Value,
			effects.Defense.Value,
			effects.Attack.Value
		];
	}

	public ISimpleNode[]? GetRecipeTooltipGiftTastes() {
		if (hoverRecipe is null || Mod.Recipes.GetGiftTastes(hoverRecipe) is not (List<NPC>, List<NPC>) tastes)
			return null;

		var builder = SimpleHelper.Builder();

		bool show_likes = tastes.Item2.Count > 0;
		bool show_loves = tastes.Item1.Count > 0;

		if (show_loves) {
			if (Mod.Config.TasteStyle == GiftStyle.Names)
				builder.FormatText(I18n.Tooltip_Loves(HighlightSearchTerms(string.Join(", ", tastes.Item1.Select(x => x.displayName)), is_love: true)), wrapText: true);
			else {
				var b2 = FlowHelper.Builder();
				b2.FormatText(I18n.Tooltip_Loves(""));
				for (int i = 0; i < tastes.Item1.Count; i++) {
					if (i > 0)
						b2.Text(" ");

					var sprite = GetHead(tastes.Item1[i]);
					if (sprite is not null)
						b2.Sprite(sprite, scale: 2f);
				}
				builder.Flow(b2.Build());
			}
		}

		if (show_likes) {
			if (Mod.Config.TasteStyle == GiftStyle.Names)
				builder.FormatText(I18n.Tooltip_Likes(HighlightSearchTerms(string.Join(", ", tastes.Item2.Select(x => x.displayName)), is_like: true)), wrapText: true);
			else {
				var b2 = FlowHelper.Builder();
				b2.FormatText(I18n.Tooltip_Likes(""));
				for (int i = 0; i < tastes.Item2.Count; i++) {
					if (i > 0)
						b2.Text(" ");

					var sprite = GetHead(tastes.Item2[i]);
					if (sprite is not null)
						b2.Sprite(sprite, scale: 2f);
				}
				builder.Flow(b2.Build());
			}
		}

		// If we have nothing, return null
		if (builder.IsEmpty())
			return null;

		return builder.Build();
	}

	public void ClearTooltipCache() {
		CachedTipRecipe = null;
		CachedTip = null;
	}

	public ISimpleNode? GetRecipeTooltip(Lazy<IList<Item?>?> items) {
		// Do we have nothing to do?
		if (hoverRecipe == null) {
			ClearTooltipCache();
			return null;
		}

		// Check some state.
		bool shifting = Game1.oldKBState.IsKeyDown(Keys.LeftShift);
		bool ctrling = Game1.oldKBState.IsKeyDown(Keys.LeftControl);

		// The following things can cause a tool-tip to change:
		// - Editing (cannot be changed while tool-tip is visible)
		// - shifting
		// - ctrling
		// - inventory state (clears this elsewhere)

		// If our state hasn't changed, return the cached tip.
		if (CachedTipRecipe == hoverRecipe && CachedTipCtrling == ctrling && CachedTipShifting == shifting)
			return CachedTip;

#if DEBUG
		var watch = ShowTiming ? Stopwatch.StartNew() : null;
#endif

		// Mark this recipe seen since we're drawing it.
		if (!Editing)
			Mod.Recipes.MarkSeen(Game1.player, hoverRecipe);

		// Alright. We're still here. Let's do this.

		TTWhen when = Mod.Config.ShowKeybindTooltip;
		Item? recipeItem = lastRecipeHover.Value;

		// First, set up the cache to a clear state.
		CachedTipRecipe = hoverRecipe;
		CachedTipShifting = shifting;
		CachedTipCtrling = ctrling;
		CachedTip = null;

		// Are we editing, without shifting?
		if (Editing && !shifting) {
			var ebuild = SimpleHelper.Builder();
			if (Filter != null)
				ebuild.FormatText(HighlightSearchTerms(hoverRecipe.DisplayName));
			else
				ebuild.Text(hoverRecipe.DisplayName);

			bool from_rule = CurrentTab?.Category?.CachedRecipes is not null &&
				CurrentTab.Category.UseRules &&
				CurrentTab.Category.CachedRecipes.Contains(hoverRecipe);

			if (from_rule)
				ebuild.Text(I18n.Filter_FromRule(), shadow: false);

			// TODO: Fix buffs in small mode
			AddBuffsToTooltip(ebuild, recipeItem, true, true);

			if (when == TTWhen.Always || (when == TTWhen.ForController && Game1.options.gamepadControls)) {
				ebuild.Divider(false);
				if (!from_rule)
					ebuild
						.Group(8)
							.Add(GetLeftClickNode())
							.Text(I18n.Tooltip_ToggleRecipe())
						.EndGroup();

				ebuild
					.Group(8)
						.Add(GetRightClickNode())
						.Text(I18n.Tooltip_UseAsIcon())
					.EndGroup();
			}

#if DEBUG
			if (watch != null) {
				watch.Stop();
				ebuild.Text($"Time: {watch.Elapsed.TotalMilliseconds}ms", color: Game1.textColor * 0.5f);
			}
#endif

			// Save this and return it.
			CachedTip = ebuild.GetLayout();
			return CachedTip;
		}

		bool shifted = !Editing && shifting && hoverRecipe.Stackable;
		int quantity = hoverRecipe.QuantityPerCraft * (shifted ? (ctrling ? 25 : 5) : 1);
		bool supports_quality = true;
		int craftable = int.MaxValue;

		List<ISimpleNode> ingredients = [];

		GameStateQueryContext ctx = new(Game1.player.currentLocation, Game1.player, null, null, Game1.random);

		if (hoverRecipe.Ingredients != null)
			foreach (var entry in hoverRecipe.Ingredients) {
				// Skip the ingredient if the quantity is not at least 1.
				// Also skip it if we have a condition and fail.
				if (entry.Quantity < 1 || !entry.PassesConditionQuery(ctx))
					continue;

				int amount;
				List<Item>? matchingItems;
				if (entry is IConsumptionPreTrackingIngredient cpt) {
					var config = Mod.Config.ShowMatchingItem;
					bool wanted = config == ShowMatchingItemMode.Always || config == ShowMatchingItemMode.FuzzyQuality
						|| (config == ShowMatchingItemMode.Fuzzy && cpt.IsFuzzyIngredient);

					matchingItems = wanted ? [] : null;
					amount = cpt.GetAvailableQuantity(Game1.player, items.Value, UnsafeInventories, Quality, matchingItems);

					// TODO: Filter out blacklisted items.

				} else {
					matchingItems = null;
					amount = entry.GetAvailableQuantity(Game1.player, items.Value, UnsafeInventories, Quality);
				}

				int quant = entry.Quantity * (shifted ? (ctrling ? 25 : 5) : 1);
				craftable = Math.Min(craftable, amount / entry.Quantity);

				if (!entry.SupportsQuality)
					supports_quality = false;

				Color color = amount < entry.Quantity ?
					(Theme.QuantityCriticalTextColor ?? Color.Red) :
					amount < quant ?
						(Theme.QuantityWarningTextColor ?? Color.OrangeRed) :
							(Theme.TooltipTextColor ?? Theme.TextColor ?? Game1.textColor);

				Color? shadow = amount < entry.Quantity ?
					Theme.QuantityCriticalShadowColor :
					amount < quant ?
						Theme.QuantityWarningShadowColor :
							null;

				var ebuilder = SimpleHelper
					.Builder(LayoutDirection.Horizontal, margin: 8)
					.Sprite(
						new SpriteInfo(entry.Texture, entry.SourceRectangle),
						scale: 2,
						quantity: quant,
						align: Alignment.VCenter
					);

				if (FilterIngredients)
					ebuilder
						.FormatText(
							HighlightSearchTerms(entry.DisplayName, true),
							color: color,
							shadowColor: shadow,
							align: Alignment.VCenter
						);
				else
					ebuilder
						.Text(
							entry.DisplayName,
							color: color,
							shadowColor: shadow,
							align: Alignment.VCenter
						);

				if (Game1.options.showAdvancedCraftingInformation)
					ebuilder
						.Space()
						.Text($"{amount}", align: Alignment.VCenter)
						.Texture(
							Game1.mouseCursors,
							SpriteHelper.MouseIcons.BACKPACK,
							2,
							align: Alignment.VCenter
						);

				ingredients.Add(ebuilder.GetLayout());

				if (matchingItems is not null && matchingItems.Count > 0) {
					int qq = 0;
					int i = 0;

					if (Mod.Config.LowQualityFirst)
						matchingItems.Sort((a, b) => {
							if (a.Quality < b.Quality) return -1;
							if (b.Quality < a.Quality) return 1;
							return 0;
						});

					bool had_quality = false;

					while (qq < quant && i < matchingItems.Count) {
						var item = matchingItems[i];
						if (item.Quality > 0) {
							had_quality = true;
							break;
						}

						int consumed = Math.Min(quant - qq, item.Stack);
						qq += consumed;
						i++;
					}

					var config = Mod.Config.ShowMatchingItem;
					bool wanted = config == ShowMatchingItemMode.Always ||
						(config != ShowMatchingItemMode.Disabled && entry is IConsumptionPreTrackingIngredient cpt2 && cpt2.IsFuzzyIngredient) ||
						(config == ShowMatchingItemMode.FuzzyQuality && had_quality);

					if (wanted) {
						var fb = FlowHelper.Builder()
							.Text(" ");

						qq = 0;
						i = 0;
						while (qq < quant && i < matchingItems.Count) {
							var item = matchingItems[i];
							int consumed = Math.Min(quant - qq, item.Stack);
							qq += consumed;
							fb.Text(" ");
							fb.Sprite(
								SpriteHelper.GetSprite(item),
								2f,
								align: Alignment.VCenter,
								quantity: consumed
							);
							fb.Text($" {item.DisplayName}",
								shadow: false,
								color: (Theme.TooltipTextColor ?? Theme.TextColor ?? Game1.textColor) * 0.5f,
								align: Alignment.VCenter
							);
							i++;
						}

						ingredients.Add(new FlowNode(fb.Build()));
					} else
						ingredients.Add(EmptyNode.Instance);
				} else
					ingredients.Add(EmptyNode.Instance);
			}

		var builder = SimpleHelper.Builder(minSize: new Vector2(4 * 95, 0));
		string quantText = quantity > 1 ? $" x{quantity}" : "";

		if (Game1.options.showAdvancedCraftingInformation && ingredients.Count > 0) {
			var eb = builder.Group(margin: 8);

			if (Filter == null)
				eb.Text($"{hoverRecipe.DisplayName}{quantText}", font: Game1.dialogueFont, shadow: true);
			else
				eb.FormatText(
					HighlightSearchTerms(hoverRecipe.DisplayName) +
						quantText,
					font: Game1.dialogueFont,
					shadow: true
				);

			eb
				.Text($"({craftable})", align: Alignment.VCenter)
				.EndGroup();

		} else if (Filter != null)
			builder.FormatText(HighlightSearchTerms(hoverRecipe.DisplayName), font: Game1.dialogueFont, shadow: true);
		else
			builder.Text(hoverRecipe.DisplayName, font: Game1.dialogueFont, shadow: true);

		if (recipeItem != null)
			builder.Text(recipeItem.getCategoryName(), color: recipeItem.getCategoryColor());

		if (ingredients.Count > 0) {
			builder
				.Divider()
				.Text(
					Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.567"),
					color: (Theme.TooltipTextColor ?? Theme.TextColor ?? Game1.textColor) * 0.75f
				)
				.Divider(false);

			if (ingredients.Count <= 10)
				builder.AddSpacedRange(4, ingredients);
			else if (ingredients.Count <= 40) {
				List<ISimpleNode> left = [];
				List<ISimpleNode> right = [];

				bool is_left = true;
				for (int i = 0; i < ingredients.Count; i += 2) {
					if (is_left) {
						left.Add(ingredients[i]);
						left.Add(ingredients[i + 1]);
					} else {
						right.Add(ingredients[i]);
						right.Add(ingredients[i + 1]);
					}
					is_left = !is_left;
				}

				builder.Group(align: Alignment.Top)
					.Group(align: Alignment.Top).AddSpacedRange(4, left).EndGroup()
					.Divider(false)
					.Group(align: Alignment.Top).AddSpacedRange(4, right).EndGroup()
					.EndGroup();
			} else {
				List<List<ISimpleNode>> cols = [];
				for (int i = 0; i < ingredients.Count; i += 25)
					cols.Add(new());

				int col = 0;
				for (int i = 0; i < ingredients.Count; i += 2) {
					cols[col].Add(ingredients[i]);
					cols[col].Add(ingredients[i + 1]);
					col = (col + 1) % cols.Count;
				}

				var bg = builder.Group();

				for (int i = 0; i < cols.Count; i++) {
					if (i > 0)
						bg.Divider(false);
					bg.Group(align: Alignment.Top).AddSpacedRange(4, cols[i]).EndGroup();
				}

				bg.EndGroup();

			}

			if (!supports_quality && (Mod.Config.LowQualityFirst || Mod.Config.MaxQuality != MaxQuality.Disabled))
				builder.Flow(
					FlowHelper.Builder()
						.Text(
							I18n.Tooltip_QualityUnsupported(),
							color: (Theme.TooltipTextColor ?? Theme.TextColor ?? Game1.textColor) * 0.75f
						).Build());
		}

		bool divided = false;

		// Description
		ISimpleNode[]? description = GetRecipeTooltipDescription();
		if (description is not null) {
			divided = true;
			builder.Divider(false);

			builder.AddRange(description);
		}

		int slots = recipeItem?.attachmentSlots() ?? 0;
		if (slots > 0)
			builder.Attachments(recipeItem!);

		// Price Catalog
		if (recipeItem != null && Game1.player.stats.Get("Book_PriceCatalogue") != 0 &&
			recipeItem is not Furniture &&
			recipeItem.CanBeLostOnDeath() &&
			recipeItem is not Clothing &&
			recipeItem is not Wallpaper &&
			!(recipeItem is SObject sobj && sobj.bigCraftable.Value)
		) {
			int price = recipeItem.sellToStorePrice() * recipeItem.Stack * quantity;
			if (price > 0) {
				if (!divided) {
					builder.Divider(false);
					divided = true;
				}

				builder
					.Group()
						.Text($"{price} ")
						.Texture(Game1.debrisSpriteSheet, new Rectangle(5, 69, 6, 6), scale: 3f)
					.EndGroup();
			}
		}

		// Times Crafted
		if (Game1.options.showAdvancedCraftingInformation) {
			int count = hoverRecipe.GetTimesCrafted(Game1.player);
			if (count > 0) {
				if (!divided)
					builder.Divider(false);

				builder
					.Text(
						cooking ? I18n.Tooltip_Cooked(count) : I18n.Tooltip_Crafted(count),
						color: (Theme.TooltipTextColor ?? Theme.TextColor ?? Game1.textColor) * 0.5f
					);
			}
		}

		// Gift Tastes
		bool show = Mod.Config.ShowTastes == GiftMode.Always || (Mod.Config.ShowTastes == GiftMode.Shift && shifting);
		if (show) {
			ISimpleNode[]? tastes = GetRecipeTooltipGiftTastes();
			if (tastes is not null) {
				builder.Divider(false);
				builder.AddRange(tastes);
			}
		}

		// Source Mod
		if (Mod.Config.ShowSourceModInTooltip) {
			IModInfo? info = null;

			string name = hoverRecipe.Name;
			if (name.StartsWith("bcbuildings:"))
				name = name[12..];

			int idx = name.IndexOf('_');
			if (idx > 0)
				info = Mod.Helper.ModRegistry.Get(name[..idx]);

			if (info == null && recipeItem != null) {
				idx = recipeItem.ItemId.IndexOf('_');
				if (idx != -1)
					info = Mod.Helper.ModRegistry.Get(recipeItem.ItemId[..idx]);
			}

			if (info != null) {
				builder.Divider(false);
				builder.Text(info.Manifest.Name, color: (Theme.TooltipTextColor ?? Theme.TextColor ?? Game1.textColor) * 0.5f, shadow: false);
			}
		}

		// Keybindings
		// TODO: Refactor keybinding tips somehow?
		if (when == TTWhen.Always || (when == TTWhen.ForController && Game1.options.gamepadControls)) {
			List<ISimpleNode> bindings = [];

			if (Mod.Config.LeftClick != ButtonAction.None)
				bindings.Add(
					SimpleHelper.Builder(LayoutDirection.Horizontal)
						.Add(GetLeftClickNode())
						.Text(GetActionTip(Mod.Config.LeftClick))
					.GetLayout()
				);

			if (Mod.Config.RightClick != ButtonAction.None)
				bindings.Add(
					SimpleHelper.Builder(LayoutDirection.Horizontal)
						.Add(GetRightClickNode())
						.Text(GetActionTip(Mod.Config.RightClick))
					.GetLayout()
				);

			if (Mod.Config.FavoriteKey?.IsBound ?? false) {
				var node = GetNode(Mod.Config.FavoriteKey);
				if (node != null)
					bindings.Add(
						SimpleHelper.Builder(LayoutDirection.Horizontal)
							.Add(node)
							.Text(I18n.Setting_Action_Favorite())
						.GetLayout()
					);
			}

			if (Mod.Config.BulkCraftKey?.IsBound ?? false) {
				var node = GetNode(Mod.Config.BulkCraftKey);
				if (node != null)
					bindings.Add(
					SimpleHelper.Builder(LayoutDirection.Horizontal)
						.Add(node)
						.Text(cooking ? I18n.Setting_Action_BulkCook() : I18n.Setting_Action_BulkCraft())
					.GetLayout()
				);
			}

			if (bindings.Count > 0)
				builder
					.Divider(false)
					.AddRange(bindings);
		}

#if DEBUG
		if (watch != null) {
			watch.Stop();
			builder.Text($"Time: {watch.Elapsed.TotalMilliseconds}ms", color: Game1.textColor * 0.5f);
		}
#endif

		CachedTip = builder.GetLayout();
		return CachedTip;
	}

	private bool AddBuffsToTooltip(ISimpleBuilder builder, Item? item, bool icons_only = false, bool divided = false) {
		if (item is not SObject sobj || sobj.bigCraftable.Value)
			return divided;

		bool grouped = false;

		if (sobj.Edibility != -300) {
			int health = item.healthRecoveredOnConsumption();
			int stamina = item.staminaRecoveredOnConsumption();

			if (health != 0) {
				Rectangle source = new(health < 0 ? 140 : 0, health < 0 ? 428 : 438, 10, 10);

				if (!divided) {
					builder.Divider(false);
					divided = true;
				}

				if (icons_only && !grouped) {
					builder = builder.Group(margin: 8);
					grouped = true;
				}

				if (icons_only)
					builder.Texture(
						Game1.mouseCursors,
						source,
						scale: 2,
						align: Alignment.VCenter
					);
				else {
					string label = (Convert.ToInt32(health) > 0 ? "+" : "") + health;

					builder.Group()
						.Texture(
							Game1.mouseCursors,
							source,
							scale: 3,
							align: Alignment.VCenter
						)
						.Space(false, 4)
						.Text(
							Game1.content.LoadString("Strings\\UI:ItemHover_Health", label),
							align: Alignment.VCenter
						)
					.EndGroup();
				}
			}

			if (stamina != 0) {
				Rectangle source = new(stamina < 0 ? 140 : 0, 428, 10, 10);

				if (!divided) {
					builder.Divider(false);
					divided = true;
				}

				if (icons_only && !grouped) {
					builder = builder.Group(margin: 8);
					grouped = true;
				}

				if (icons_only)
					builder.Texture(
						Game1.mouseCursors,
						source,
						scale: 2,
						align: Alignment.VCenter
					);
				else {
					string label = (Convert.ToInt32(stamina) > 0 ? "+" : "") + stamina;

					builder.Group()
						.Texture(
							Game1.mouseCursors,
							source,
							scale: 3,
							align: Alignment.VCenter
						)
						.Space(false, 4)
						.Text(
							Game1.content.LoadString("Strings\\UI:ItemHover_Energy", label),
							align: Alignment.VCenter
						)
					.EndGroup();
				}
			}
		}

		// Retrieve the item's buffs.
		if (Game1.objectData.TryGetValue(item.ItemId, out var itemData)) {
			BuffEffects effects = new();
			int msDur = int.MinValue;
			foreach (Buff buff in SObject.TryCreateBuffsFromData(itemData, item.Name, item.DisplayName, 1f, item.ModifyItemBuffs)) {
				effects.Add(buff.effects);
				if (buff.millisecondsDuration == -2 || (buff.millisecondsDuration > msDur && msDur != -2))
					msDur = buff.millisecondsDuration;
			}

			if (msDur == int.MinValue)
				msDur = -2;

			float[] values = GetBuffEffects(effects);
			bool inner_divided = icons_only;

			for (int idx = 0; idx < values.Length; idx++) {
				float buff = values[idx];
				if (buff == 0f || (idx == 12 && icons_only) || (idx == 12 && buff == -2))
					continue;

				Rectangle source = new(10 + idx * 10, 428, 10, 10);

				if (!inner_divided) {
					builder.Divider(false);
					inner_divided = true;
				}

				if (icons_only && !grouped) {
					builder = builder.Group(margin: 8);
					grouped = true;
				}

				if (icons_only) {
					builder.Texture(
						Game1.mouseCursors,
						source,
						scale: 2,
						align: Alignment.VCenter
					);

					continue;
				}

				string label = (buff > 0 ? "+" : "") + buff;
				if (idx <= 11)
					label = Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + idx, label);

				builder.Group()
					.Texture(
						Game1.mouseCursors,
						source,
						scale: 3,
						align: Alignment.VCenter
					)
					.Space(false, 4)
					.Text(label, align: Alignment.VCenter)
				.EndGroup();
			}

			if (!icons_only && msDur != -2)
				builder.Group()
					.Texture(
						Game1.mouseCursors,
						new Rectangle(410, 501, 9, 9),
						scale: 3,
						align: Alignment.VCenter
					)
					.Space(false, 4)
					.Text(Utility.getMinutesSecondsStringFromMilliseconds(msDur), align: Alignment.VCenter)
				.EndGroup();

			var buffs = Mod.intSCore?.GetItemBuffs(itemData);
			if (buffs is not null)
				foreach (var buff in buffs) {
					if (buff.Item2 == 0)
						continue;

					var skill = buff.Item1;
					float amount = buff.Item2;

					if (!inner_divided) {
						builder.Divider(false);
						inner_divided = true;
					}

					if (icons_only && !grouped) {
						builder = builder.Group(margin: 8);
						grouped = true;
					}

					if (icons_only) {
						builder.Texture(
							skill.SkillsPageIcon,
							skill.SkillsPageIcon.Bounds,
							scale: 2,
							align: Alignment.VCenter
						);

						continue;
					}

					string positive = amount > 0 ? "+" : "";
					string label = $"{positive}{amount} {skill.SafeGetName()}";

					builder.Group()
						.Texture(
							skill.SkillsPageIcon,
							skill.SkillsPageIcon.Bounds,
							scale: 3,
							align: Alignment.VCenter
						)
						.Space(false, 4)
						.Text(label, align: Alignment.VCenter)
					.EndGroup();

				}

			divided |= inner_divided;
		}

		if (grouped)
			builder.EndGroup();

		return divided;
	}

	private SpriteInfo? GetHead(NPC? npc) {
		if (npc is null)
			return null;

		if (Heads.TryGetValue(npc, out SpriteInfo? sprite))
			return sprite;

		Texture2D texture;
		try {
			texture = Game1.content.Load<Texture2D>(@"Characters\" + npc.getTextureName());
		} catch (Exception) {
			texture = npc.Sprite.Texture;
		}

		Mod.GetHeads().TryGetValue(npc.Name, out HeadSize? info);

		sprite = new SpriteInfo(
			texture,
			new Rectangle(
				info?.OffsetX ?? 0,
				info?.OffsetY ?? 0,
				info?.Width ?? 16,
				info?.Height ?? 15
			)
		);

		Heads[npc] = sprite;
		return sprite;
	}

	public static string? GetBehaviorTip(TransferBehavior behavior) {
		return behavior.Mode switch {
			TransferMode.All => I18n.Tooltip_Transfer_All(),
			TransferMode.AllButQuantity => I18n.Tooltip_Transfer_ButQuantity(behavior.Quantity),
			TransferMode.Half => I18n.Tooltip_Transfer_Half(),
			TransferMode.Quantity => I18n.Tooltip_Transfer_Quantity(behavior.Quantity),
			_ => null,
		};
	}

	public string? GetActionTip(ButtonAction action) {
		return action switch {
			ButtonAction.Craft => I18n.Setting_Action_Craft(),
			ButtonAction.Favorite => I18n.Setting_Action_Favorite(),
			ButtonAction.BulkCraft => cooking ? I18n.Setting_Action_BulkCook() : I18n.Setting_Action_BulkCraft(),
			_ => null,
		};
	}

	public static ISimpleNode GetLeftClickNode() {
		if (Game1.options.gamepadControls)
			return GetNode(SButton.ControllerA);

		return GetNode(Game1.options.useToolButton);
	}

	public static ISimpleNode GetRightClickNode() {
		if (Game1.options.gamepadControls)
			return GetNode(SButton.ControllerX);

		return GetNode(Game1.options.actionButton);
	}

	public static ISimpleNode? GetNode(KeybindList keybind) {
		// We either want gamepad or not.
		bool want_pad = Game1.options.gamepadControls;

		foreach (var kb in keybind.Keybinds) {
			if (kb.Buttons == null || kb.Buttons.Length == 0)
				continue;
			bool is_pad = kb.Buttons[0].TryGetController(out _);
			if (want_pad != is_pad)
				continue;

			return GetNode(kb.Buttons[0]);
		}

		return null;
	}

	public static ISimpleNode GetNode(SButton button) {
		var sprite = SpriteHelper.GetSprite(button);
		if (sprite != null)
			return new SpriteNode(sprite, scale: 2, alignment: Alignment.VCenter);

		return new TextNode($"{button}:");
	}
	public static ISimpleNode GetNode(InputButton[] buttons) {
		SpriteInfo? sprite = null;
		foreach (InputButton btn in buttons) {
			if (btn.mouseLeft) {
				sprite = SpriteHelper.GetSprite(SButton.MouseLeft);
				break;
			} else if (btn.mouseRight) {
				sprite = SpriteHelper.GetSprite(SButton.MouseRight);
				break;
			}
		}

		if (sprite != null)
			return new SpriteNode(sprite, scale: 2, alignment: Alignment.VCenter);

		return new TextNode($"{ModEntry.GetInputLabel(buttons)}:");
	}

	#endregion

	#region CustomCraftingStation Compatibility Hax

	// This only exists to stop CCS from exception-ing. We will never use it.
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CS0169 // Never used
	private List<Dictionary<ClickableTextureComponent, CraftingRecipe>>? pagesOfCraftingRecipes;
#pragma warning restore CS0169 // Never used
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore IDE0044 // Add readonly modifier

	// Continued support to make CCS not break.
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable CA1822 // Mark members as static
	public void layoutRecipes(List<string> recipes) { }
#pragma warning restore CA1822 // Mark members as static
#pragma warning restore IDE0060 // Remove unused parameter

	#endregion

}
