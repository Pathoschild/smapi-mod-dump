/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CooksAssistant
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LoveOfCooking.Objects
{
    public class CookingMenu : ItemGrabMenu
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static Config Config => ModEntry.Config;
		private static ITranslationHelper i18n => ModEntry.Instance.Helper.Translation;
		private static Texture2D Texture => ModEntry.SpriteSheet;
		private readonly CookingManager _cookingManager;

		// Spritesheet source areas
		// Custom spritesheet
		internal static readonly Rectangle CookbookSource = new Rectangle(0, 80, 240, 128);
		internal static readonly Rectangle CookingSlotOpenSource = new Rectangle(0, 208, 28, 28);
		internal static readonly Rectangle CookingSlotLockedSource = new Rectangle(28, 208, 28, 28);
		private static readonly Rectangle CookButtonSource = new Rectangle(128, 0, 16, 22);
		private static readonly Rectangle SearchTabButtonSource = new Rectangle(58, 0, 16, 16);
		private static readonly Rectangle IngredientsTabButtonSource = new Rectangle(42, 0, 16, 16);
		private static readonly Rectangle FilterContainerSource = new Rectangle(247, 188, 9, 20);
		private static readonly Rectangle FilterIconSource = new Rectangle(56, 208, 12, 12);
		private static readonly Rectangle ToggleViewButtonSource = new Rectangle(96, 224, 16, 16);
		private static readonly Rectangle ToggleFilterButtonSource = new Rectangle(80, 224, 16, 16);
		private static readonly Rectangle ToggleOrderButtonSource = new Rectangle(128, 224, 16, 16);
		private static readonly Rectangle SearchButtonSource = new Rectangle(144, 224, 16, 16);
		private static readonly Rectangle FavouriteIconSource = new Rectangle(247, 178, 9, 9);
		private static readonly Rectangle AutofillButtonSource = new Rectangle(112, 272, 16, 16);
		private static readonly Rectangle InventoryTabButtonSource = new Rectangle(240, 80, 16, 21);
		private static readonly Rectangle InventoryBackpackIconSource = new Rectangle(244, 102, 12, 14);
		private static readonly Rectangle InventoryFridgeIconSource = new Rectangle(244, 118, 12, 14);
		private static readonly Rectangle InventoryMinifridgeIconSource = new Rectangle(244, 134, 12, 14);
		private static readonly Rectangle InventoryChestIconSource = new Rectangle(244, 148, 12, 14);
		// MouseCursors sheet
		private static readonly Rectangle DownButtonSource = new Rectangle(0, 64, 64, 64);
		private static readonly Rectangle UpButtonSource = new Rectangle(64, 64, 64, 64);
		private static readonly Rectangle RightButtonSource = new Rectangle(0, 192, 64, 64);
		private static readonly Rectangle LeftButtonSource = new Rectangle(0, 256, 64, 64);
		private static readonly Rectangle PlusButtonSource = new Rectangle(184, 345, 7, 8);
		private static readonly Rectangle MinusButtonSource = new Rectangle(177, 345, 7, 8);
		private static readonly Rectangle OkButtonSource = new Rectangle(128, 256, 64, 64);
		private static readonly Rectangle NoButtonSource = new Rectangle(192, 256, 64, 64);
		private static readonly Rectangle UpSmallButtonSource = new Rectangle(92, 0, 8, 8);
		private static readonly Rectangle DownSmallButtonSource = new Rectangle(92, 8, 8, 8);
		// Other values
		internal const int Scale = 4;
		internal const int SmallScale = 3;
		private const float KoHeightScale = 0.825f;
		private const float KoWidthScale = 1.25f;
		private readonly bool _resizeKoreanFonts;
		private static readonly Dictionary<string, Rectangle> CookTextSource = new Dictionary<string, Rectangle>();
		private static readonly Point CookTextSourceOrigin = new Point(0, 240);
		private static readonly Dictionary<string, int> CookTextSourceWidths = new Dictionary<string, int>
		{
			{ "en", 32 },
			{ "fr", 45 },
			{ "es", 42 },
			{ "pt", 48 },
			{ "ja", 50 },
			{ "zh", 36 },
			{ "ko", 48 },
			{ "ru", 53 },
			{ "de", 40 },
			{ "it", 48 },
			{ "tr", 27 }
		};
		private const int CookTextSourceHeight = 16;
		private const int CookTextSideSourceWidth = 5;
		private const int FilterContainerSideSourceWidth = 4;
		private int _cookTextMiddleSourceWidth;
		private int _filterContainerMiddleSourceWidth;

		// Clickables
		public readonly List<ClickableComponent> ChildComponents = new List<ClickableComponent>();
		private readonly ClickableTextureComponent _navDownButton;
		private readonly ClickableTextureComponent _navUpButton;
		private readonly ClickableTextureComponent _navRightButton;
		private readonly ClickableTextureComponent _navLeftButton;
		private readonly List<ClickableTextureComponent> _ingredientsClickables = new List<ClickableTextureComponent>();
		private readonly ClickableComponent _cookButton;
		private readonly ClickableTextureComponent _cookQuantityUpButton;
		private readonly ClickableTextureComponent _cookQuantityDownButton;
		private readonly ClickableTextureComponent _cookConfirmButton;
		private readonly ClickableTextureComponent _cookCancelButton;
		private Rectangle _cookIconBounds;
		private readonly ClickableTextureComponent _searchTabButton;
		private readonly ClickableTextureComponent _ingredientsTabButton;
		private readonly ClickableTextureComponent _toggleOrderButton;
		private readonly ClickableTextureComponent _toggleFilterButton;
		private readonly ClickableTextureComponent _toggleViewButton;
		private readonly ClickableComponent _searchBarClickable;
		private readonly ClickableTextureComponent _searchButton;
		private readonly ClickableTextureComponent _recipeIconButton;
		private readonly ClickableTextureComponent _toggleAutofillButton;
		private Rectangle _filterContainerBounds;
		private readonly List<ClickableTextureComponent> _toggleButtonClickables = new List<ClickableTextureComponent>();
		private readonly List<ClickableTextureComponent> _filterButtons = new List<ClickableTextureComponent>();
		private Rectangle _searchResultsArea;
		private Rectangle _quantityScrollableArea;
		private Rectangle _inventoriesScrollableArea;
		private Rectangle _inventoriesPopupArea;
		private Rectangle _inventoryCardArea;
		private readonly ClickableTextureComponent _inventoryTabButton;
		private readonly ClickableTextureComponent _inventoryUpButton;
		private readonly ClickableTextureComponent _inventoryDownButton;
		private readonly List<ClickableTextureComponent> _inventorySelectButtons = new List<ClickableTextureComponent>();
		private readonly List<ClickableComponent> _searchListClickables = new List<ClickableComponent>();
		private readonly List<ClickableComponent> _searchGridClickables = new List<ClickableComponent>();
		private const int ListRows = 5;
		private const int GridRows = 4;
		private const int GridColumns = 4;
		private const int InventorySelectButtonsWide = 2;
		private const int IngredientSlotsWide = 3;
		private bool UsingRecipeGridView
		{
			get => ModEntry.Instance.States.Value.IsUsingRecipeGridView;
			set => ModEntry.Instance.States.Value.IsUsingRecipeGridView = value;
		}
		private bool HideLastRowOfSearchResults => this._showSearchFilters;

		// Layout dimensions (variable with screen size)
		private Rectangle _cookbookLeftRect
			= new Rectangle(-1, -1, CookbookSource.Width * Scale / 2, CookbookSource.Height * Scale);
		private Rectangle _cookbookRightRect
			= new Rectangle(-1, -1, CookbookSource.Width * Scale / 2, CookbookSource.Height * Scale);
		private Point _leftContent;
		private Point _rightContent;
		private int _lineWidth;
		private int _textWidth;

		// Layout definitions
		private const int MarginLeft = 16 * Scale;
		private const int MarginRight = 8 * Scale;
		private const int TextMuffinTopOverDivider = (int)(1.5f * Scale);
		private const int TextDividerGap = 1 * Scale;
		private const int TextSpacingFromIcons = 20 * Scale;
		private const int RecipeListHeight = 16 * Scale;
		private const int RecipeGridHeight = 18 * Scale;

		private static readonly Color SubtextColour = Game1.textColor * 0.75f;
		private static readonly Color BlockedColour = Game1.textColor * 0.325f;
		internal static Color DividerColour;
		private const float DividerOpacity = 0.5f;

		private bool UseHorizontalInventoryButtonArea => _inventorySelectButtons.Any()
			&& Context.IsSplitScreen
			;

		// Animations
		private const int AnimFrameTime = 100;
		private const int AnimFrames = 8;
		private const int AnimTimerLimit = AnimFrameTime * AnimFrames;
		private int _animTimer;
		private int _animFrame;

		// Sounds
		private const string ClickCue = "coin";
		private const string CancelCue = "cancel";
		private const string SuccessCue = "reward";
		private const string HoverInCue = "breathin";
		private const string HoverOutCue = "breathout";
		private const string PageChangeCue = "shwip";
		private const string MenuChangeCue = "bigSelect";
		private const string MenuCloseCue = "bigDeSelect";

		// Text entry
		private readonly TextBox _searchBarTextBox;
		private readonly TextBox _quantityTextBox;
		private Rectangle _quantityTextBoxBounds;
		private Rectangle _searchBarTextBoxBounds;
		private int _searchBarTextBoxMaxWidth;
		private int _searchBarTextBoxMinWidth;
		private const string QuantityTextBoxDefaultText = "1";

		// Menu data
		// state
		public enum State
		{
			Opening,
			Search,
			Recipe,
			Ingredients
		}
		private readonly Stack<State> _stack = new Stack<State>();
		// recipes
		private readonly List<CraftingRecipe> _recipesAvailable;
		private List<CraftingRecipe> _recipesFiltered;
		private readonly List<CraftingRecipe> _recipeSearchResults;
		private CraftingRecipe CurrentRecipe => _recipesFiltered.Count > _recipeIndex ? _recipesFiltered[_recipeIndex] : null;
		private int _recipeIndex;
		private Item _recipeAsItem;
		private List<int> _recipeBuffs;
		private int _recipeBuffDuration;
		private int _recipeDisplayHeight;
		private int _searchResultsPerPage;
		private int _recipeCraftableCount;
		private int _recipeReadyToCraftCount;
		private readonly List<int> _recipeIngredientQuantitiesHeld = new List<int>();
		private bool ReadyToCook => _recipeIndex >= 0
					&& _recipesFiltered.Count > _recipeIndex
					&& _recipeAsItem != null
					&& _recipeReadyToCraftCount > 0
					&& !_showCookingConfirmPopup;
		// inventories
		private int _inventoryId;
		private readonly List<IList<Item>> _allInventories = new List<IList<Item>>();
		public List<Chest> _materialContainers;
		internal const int BackpackInventoryId = 0;
		internal const int MaximumExtraInventories = 24;
		private int _inventoryIdsBeforeMinifridges = 0;
		private int _inventoryIdsBeforeChests = 0;
		private int _numberOfMinifridges = 0;
		private int _numberOfChests = 0;
		private bool _showInventoriesPopup;
		private readonly List<KeyValuePair<Color, bool>> _chestColours = new List<KeyValuePair<Color, bool>>();
		private bool ShouldShowInventoryElements { get => _inventorySelectButtons.Count > 1; }
		// filters
		private bool _showSearchFilters;
		private Filter _lastFilterUsed;	
		public enum Filter
		{
			None,
			Alphabetical,
			Energy,
			Gold,
			Buffs,
			New,
			Ready,
			Favourite
		}
		// miscellanea
		private bool _showCookingConfirmPopup;
		private int _mouseHeldTicks;
		private string _locale;
		internal static int LastBurntCount;
		private readonly IReflectedField<Dictionary<int, double>> _iconShakeTimerField;
		internal static readonly int SpriteId = (int)Game1.player.UniqueMultiplayerID + 5050505;

		// Features
		private const bool IsIngredientsPageEnabled = false;
		private bool IsUsingAutofill
		{
			get
			{
				return bool.Parse(Game1.player.modData[ModEntry.AssetPrefix + "autofill"]);
			}
			set
			{
				Game1.player.modData[ModEntry.AssetPrefix + "autofill"] = value.ToString();
				Log.D($"Autofill set to {value}",
					Config.DebugMode);
			}
		}


		public CookingMenu(CraftingPage craftingPage, string initialRecipe = null) 
			: this(recipes: Utils.TakeRecipesFromCraftingPage(craftingPage), materialContainers: craftingPage._materialContainers, initialRecipe: initialRecipe)
        {}

		public CookingMenu(List<CraftingRecipe> recipes = null, List<Chest> materialContainers = null, string initialRecipe = null)
			: base(inventory: null, context: null)
		{
			this.width = CookbookSource.Width * Scale;
			this.height = 720;

			Game1.displayHUD = true; // Prevents hidden HUD on crash when initialising menu, set to false at the end of this method
			this._locale = LocalizedContentManager.CurrentLanguageCode.ToString();
			if (!CookTextSourceWidths.ContainsKey(this._locale))
			{
				this._locale = "en";
			}
			this._resizeKoreanFonts = Config.ResizeKoreanFonts;
			this.initializeUpperRightCloseButton();
			this.trashCan = null;
			this._cookingManager = new CookingManager(cookingMenu: this)
			{
				MaxIngredients = Utils.GetNearbyCookingStationLevel()
			};

			this._iconShakeTimerField = Helper.Reflection.GetField<Dictionary<int, double>>(inventory, "_iconShakeTimer");

			// Set initial material containers for additional inventories
			this._materialContainers = materialContainers ?? new List<Chest>();

			this._recipesAvailable = recipes != null
				// Recipes may be populated by those of any CraftingMenu that this menu supercedes
				// Should guarantee Limited Campfire Cooking compatibility
				? recipes.Where(recipe => Game1.player.cookingRecipes.ContainsKey(recipe.name)).ToList()
				// Otherwise start off the list of cooking recipes with all those the player has unlocked
				: this._recipesAvailable = Utility.GetAllPlayerUnlockedCookingRecipes()
					.Select(str => new CraftingRecipe(str, true))
					.Where(recipe => recipe.name != "Torch").ToList();

			Utils.UpdateEnglishRecipeDisplayNames(ref _recipesAvailable);

			// Default autofill preferences if none set
			if (!Game1.player.modData.ContainsKey(ModEntry.AssetPrefix + "autofill"))
			{
				IsUsingAutofill = false;
			}
			Log.D($"Autofill on startup: {IsUsingAutofill}",
				Config.DebugMode);

			// Apply default filter to the default recipe list
			bool reverseDefaultFilter = ModEntry.Instance.States.Value.LastFilterReversed;
			this._recipesAvailable = this.FilterRecipes();

			// Initialise filtered search lists
			this._recipesFiltered = _recipesAvailable;
			this._recipeSearchResults = new List<CraftingRecipe>();

			// Clickables and elements
			this._navDownButton = new ClickableTextureComponent(
				"navDown", new Rectangle(-1, -1, DownButtonSource.Width, DownButtonSource.Height),
				null, null, Game1.mouseCursors, DownButtonSource, 1f, true);
			this._navUpButton = new ClickableTextureComponent(
				"navUp", new Rectangle(-1, -1, UpButtonSource.Width, UpButtonSource.Height),
				null, null, Game1.mouseCursors, UpButtonSource, 1f, true);
			this._navRightButton = new ClickableTextureComponent(
				"navRight", new Rectangle(-1, -1, RightButtonSource.Width, RightButtonSource.Height),
				null, null, Game1.mouseCursors, RightButtonSource, 1f, true);
			this._navLeftButton = new ClickableTextureComponent(
				"navLeft", new Rectangle(-1, -1, LeftButtonSource.Width, LeftButtonSource.Height),
				null, null, Game1.mouseCursors, LeftButtonSource, 1f, true);
			this._cookButton = new ClickableComponent(Rectangle.Empty, "cook");
			this._cookQuantityUpButton = new ClickableTextureComponent(
				"quantityUp", new Rectangle(-1, -1, PlusButtonSource.Width * Scale, PlusButtonSource.Height * Scale),
				null, null, Game1.mouseCursors, PlusButtonSource, Scale, true);
			this._cookQuantityDownButton = new ClickableTextureComponent(
				"quantityDown", new Rectangle(-1, -1, MinusButtonSource.Width * Scale, MinusButtonSource.Height * Scale),
				null, null, Game1.mouseCursors, MinusButtonSource, Scale, true);
			this._cookConfirmButton = new ClickableTextureComponent(
				"confirm", new Rectangle(-1, -1, OkButtonSource.Width, OkButtonSource.Height),
				null, null, Game1.mouseCursors, OkButtonSource, 1f, true);
			this._cookCancelButton = new ClickableTextureComponent(
				"cancel", new Rectangle(-1, -1, NoButtonSource.Width, NoButtonSource.Height),
				null, null, Game1.mouseCursors, NoButtonSource, 1f, true);
			this._toggleFilterButton = new ClickableTextureComponent(
				"toggleFilter", new Rectangle(-1, -1, ToggleFilterButtonSource.Width * SmallScale, ToggleFilterButtonSource.Height * SmallScale),
				null, i18n.Get("menu.cooking_search.filter_label"),
				Texture, ToggleFilterButtonSource, SmallScale, true);
			this._toggleOrderButton = new ClickableTextureComponent(
				"toggleOrder", new Rectangle(-1, -1, ToggleOrderButtonSource.Width * SmallScale, ToggleOrderButtonSource.Height * SmallScale),
				null, i18n.Get("menu.cooking_search.order_label"),
				Texture, ToggleOrderButtonSource, SmallScale, true);
			this._toggleViewButton = new ClickableTextureComponent(
				"toggleView", new Rectangle(-1, -1, ToggleViewButtonSource.Width * SmallScale, ToggleViewButtonSource.Height * SmallScale),
				null, i18n.Get("menu.cooking_search.view."
							   + (this.UsingRecipeGridView ? "grid" : "list")),
				Texture, ToggleViewButtonSource, SmallScale, true);
			this._searchButton = new ClickableTextureComponent(
				"search", new Rectangle(-1, -1, SearchButtonSource.Width * SmallScale, SearchButtonSource.Height * SmallScale),
				null, i18n.Get("menu.cooking_recipe.search_label"),
				Texture, SearchButtonSource, SmallScale, true);
			this._toggleAutofillButton = new ClickableTextureComponent(
				"autofill", new Rectangle(-1, -1, AutofillButtonSource.Width * SmallScale, AutofillButtonSource.Height * SmallScale),
				null, i18n.Get("menu.cooking_recipe.autofill_label"),
				Texture, AutofillButtonSource, SmallScale, true);
			this._recipeIconButton = new ClickableTextureComponent(
				"recipeIcon", new Rectangle(-1, -1, 64, 64),
				null, null,
				Game1.objectSpriteSheet, new Rectangle(0, 0, 64, 64), Scale, true);
			this._searchBarClickable = new ClickableComponent(Rectangle.Empty, "searchbox");
			this._searchTabButton = new ClickableTextureComponent(
				"searchTab", new Rectangle(-1, -1, SearchTabButtonSource.Width * Scale, SearchTabButtonSource.Height * Scale),
				null, null, Texture, SearchTabButtonSource, Scale, true);
			this._ingredientsTabButton = new ClickableTextureComponent(
				"ingredientsTab", new Rectangle(-1, -1, IngredientsTabButtonSource.Width * Scale, IngredientsTabButtonSource.Height * Scale),
				null, null, Texture, IngredientsTabButtonSource, Scale, true);
			for (int i = (int)Filter.Alphabetical; i < Enum.GetNames(typeof(Filter)).Length; ++i)
			{
				this._filterButtons.Add(new ClickableTextureComponent(
					$"filter{i}", new Rectangle(-1, -1, FilterIconSource.Width * SmallScale, FilterIconSource.Height * SmallScale),
					null, i18n.Get($"menu.cooking_search.filter.{i}"
						+ (Config.HideFoodBuffsUntilEaten && i == 4 ? "_alt" : "")),
					Texture, new Rectangle(
						FilterIconSource.X + (i - 1) * FilterIconSource.Width, FilterIconSource.Y,
						FilterIconSource.Width, FilterIconSource.Height),
					SmallScale));
			}

			this._toggleButtonClickables.AddRange(new[]
			{
				this._toggleFilterButton,
				this._toggleOrderButton,
				this._toggleViewButton,
				this._toggleAutofillButton
			});

			this._searchBarTextBox = new TextBox(
				Game1.content.Load<Texture2D>("LooseSprites\\textBox"),
				null, Game1.smallFont, Game1.textColor)
			{
				textLimit = 32,
				Selected = false,
				Text = i18n.Get("menu.cooking_recipe.search_label"),
			};
			this._quantityTextBox = new TextBox(
				Game1.content.Load<Texture2D>("LooseSprites\\textBox"),
				null, Game1.smallFont, Game1.textColor)
			{
				numbersOnly = true,
				textLimit = 2,
				Selected = false,
				Text = QuantityTextBoxDefaultText,
			};

			this._quantityTextBox.OnEnterPressed += this.ValidateNumericalTextBox;
			this._searchBarTextBox.OnEnterPressed += sender => { this.CloseTextBox(sender, reapplyFilters: true); };

			// 'Cook!' button localisations
			int xOffset = 0;
			int yOffset = 0;
			CookTextSource.Clear();
			foreach (KeyValuePair<string, int> pair in CookTextSourceWidths)
			{
				if (xOffset + pair.Value > Texture.Width)
				{
					xOffset = 0;
					yOffset += CookTextSourceHeight;
				}
				CookTextSource.Add(pair.Key, new Rectangle(
					CookTextSourceOrigin.X + xOffset, CookTextSourceOrigin.Y + yOffset,
					pair.Value, CookTextSourceHeight));
				xOffset += pair.Value;
			}

			// Search results clickables
			for (int i = 0; i < ListRows; ++i)
			{
				this._searchListClickables.Add(new ClickableComponent(new Rectangle(-1, -1, -1, -1), "searchList" + i));
			}
			for (int i = 0; i < GridRows * GridColumns; ++i)
			{
				this._searchGridClickables.Add(new ClickableComponent(new Rectangle(-1, -1, -1, -1), "searchGrid" + i));
			}

			this._inventoryTabButton = new ClickableTextureComponent(name: "inventoryTab",
				bounds: new Rectangle(-1, -1, InventoryTabButtonSource.Width * Scale, InventoryTabButtonSource.Height * Scale),
				label: null, hoverText: null, texture: Texture, sourceRect: InventoryTabButtonSource, scale: Scale);
			this._inventoryUpButton = new ClickableTextureComponent(name: "inventoryUp",
				bounds: new Rectangle(-1, -1, UpSmallButtonSource.Width * Scale, UpSmallButtonSource.Height * Scale),
				label: null, hoverText: null, texture: Texture, sourceRect: UpSmallButtonSource, scale: Scale);
			this._inventoryDownButton = new ClickableTextureComponent(name: "inventoryDown",
				bounds: new Rectangle(-1, -1, DownSmallButtonSource.Width * Scale, DownSmallButtonSource.Height * Scale),
				label: null, hoverText: null, texture: Texture, sourceRect: DownSmallButtonSource, scale: Scale);

			// Add base player inventories:
			this._inventoryId = BackpackInventoryId;
			this._allInventories.Add(Game1.player.Items);

			// Determine extra inventories:
			this._inventoryIdsBeforeMinifridges = this._allInventories.Count;

			if (this._materialContainers.Any())
			{
				// Secure container mutexes that were lost after replacing the original crafting menu
				List<StardewValley.Network.NetMutex> mutexes = this._materialContainers
					.Select(chest => chest.mutex)
					.ToList();
				MultipleMutexRequest multipleMutexRequest = null;
				multipleMutexRequest = new MultipleMutexRequest(
					mutexes: mutexes,
					success_callback: delegate
					{
						this.exitFunction = delegate
						{
							multipleMutexRequest.ReleaseLocks();
							Log.D("Freeing mutexes.");
						};
					},
					failure_callback: delegate
					{
						Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Kitchen_InUse"));
					});

				// Populate inventory lists
				this._materialContainers = this._materialContainers
					// place fridge first if one exists:
					.OrderByDescending(chest => Utils.IsFridgeOrMinifridge(chest))
					// then minifridges, then chests, if any exist
					.ThenByDescending(chest => !Utils.IsMinifridge(chest))
					.ToList();
				while (this._materialContainers.Count >= MaximumExtraInventories)
				{
					this._materialContainers.Remove(this._materialContainers.Last());
				}
				var items = this._materialContainers
					.Select(chest => chest.items)
					.ToList();
				this._allInventories.AddRange(items);
				this._chestColours.AddRange(this._materialContainers.Select(
					c => new KeyValuePair<Color, bool>(
						key: c.playerChoiceColor.Value,
						value: c.playerChest.Value
							&& (c.ParentSheetIndex == 130 || c.ParentSheetIndex == 232) // Colourable chests
							&& !c.playerChoiceColor.Value.Equals(Color.Black)))); // Coloured chests
				this._numberOfMinifridges = this._materialContainers.Count(chest => Utils.IsMinifridge(chest));
			}

			this._inventoryIdsBeforeChests = this._inventoryIdsBeforeMinifridges + this._numberOfMinifridges;
			this._numberOfChests = this._allInventories.Count - this._inventoryIdsBeforeChests;

			// Populate clickable inventories list
			{
				Rectangle sourceRect = InventoryBackpackIconSource;
				Rectangle destRect = new Rectangle(-1, -1, 16 * Scale, 16 * Scale);
				this._inventorySelectButtons.Add(new ClickableTextureComponent("inventorySelectBackpack",
					destRect, null, null,
					ModEntry.SpriteSheet, sourceRect, Scale, false));
				for (int i = 0; i < this._materialContainers.Count; ++i)
				{
					sourceRect = Utils.IsFridgeOrMinifridge(this._materialContainers[i])
						? Utils.IsMinifridge(this._materialContainers[i])
							? InventoryMinifridgeIconSource
							: InventoryFridgeIconSource
						: InventoryChestIconSource;
					this._inventorySelectButtons.Add(new ClickableTextureComponent(
						$"inventorySelectContainer{i}",
						destRect, null, null,
						ModEntry.SpriteSheet, sourceRect, Scale, false));
				}
			}

			// Setup menu elements layout
			this.RealignElements();
			this.InitialiseControllerFlow();

			// Go to landing page by default
			this.OpenSearchPage();

			if (ModEntry.Instance.States.Value.LastFilterThisSession != Filter.None)
			{
				// Apply previously-used filter
				this._recipesFiltered = this.FilterRecipes(ModEntry.Instance.States.Value.LastFilterThisSession);
			}
			else
			{
				// Apply default filter if no other filter was used this session
				this._recipesFiltered = this.FilterRecipes((Filter)Enum.Parse(typeof(Filter), ModEntry.Config.DefaultSearchFilter));
			}
			if (reverseDefaultFilter)
			{
				// Reverse the filter if required
				this._recipesFiltered = this.ReverseRecipeList(this._recipesFiltered);
			}
			this.UpdateSearchRecipes();

			// Open to a starting recipe if needed
			if (!string.IsNullOrEmpty(initialRecipe))
			{
				this.ChangeCurrentRecipe(initialRecipe);
				this.OpenRecipePage();
			}

			Game1.displayHUD = false;

			if (Game1.options.gamepadControls || Game1.options.SnappyMenus)
			{
				Game1.delayedActions.Add(new DelayedAction(timeUntilAction: 0, behavior: delegate
				{
					// Snap to default
					this.snapToDefaultClickableComponent();
				}));
			}
		}

		private void InitialiseControllerFlow()
		{
			if (Constants.TargetPlatform == GamePlatform.Android)
				return;

			int id = 1000;
			ChildComponents.AddRange(new List<ClickableComponent>
			{
				_navDownButton,
				_navUpButton,
				_navRightButton,
				_navLeftButton,
				_cookButton,
				_cookQuantityUpButton,
				_cookQuantityDownButton,
				_cookConfirmButton,
				_cookCancelButton,
				_searchTabButton,
				_ingredientsTabButton,
				_inventoryTabButton,
				_inventoryUpButton,
				_inventoryDownButton,
				_searchButton,
				_recipeIconButton,
				_searchBarClickable,
			});
			ChildComponents.AddRange(_toggleButtonClickables);
			ChildComponents.AddRange(_ingredientsClickables);
			ChildComponents.AddRange(_inventorySelectButtons);
			ChildComponents.AddRange(_filterButtons);
			ChildComponents.AddRange(_searchListClickables);
			ChildComponents.AddRange(_searchGridClickables);

			foreach (ClickableComponent clickable in ChildComponents)
			{
				clickable.myID = ++id;
			}

			// Component navigation
			_searchBarClickable.rightNeighborID = _toggleButtonClickables.First().myID;
			for (int i = 0; i < _toggleButtonClickables.Count; ++i)
			{
				_toggleButtonClickables[i].leftNeighborID = i > 0
					? _toggleButtonClickables[i - 1].myID
					: _searchBarClickable.myID;

				_toggleButtonClickables[i].rightNeighborID = i < _toggleButtonClickables.Count - 1
					? _toggleButtonClickables[i + 1].myID
					: _ingredientsClickables.First().myID;
			}

			_ingredientsClickables.Last().rightNeighborID = upperRightCloseButton.myID;
			upperRightCloseButton.leftNeighborID = _ingredientsClickables.Last().myID;
			upperRightCloseButton.downNeighborID = _ingredientsClickables.Last().myID;

			_recipeIconButton.downNeighborID = _navLeftButton.downNeighborID = _navRightButton.downNeighborID = 0;
			_navLeftButton.leftNeighborID = _searchTabButton.myID;
			_navLeftButton.rightNeighborID = _recipeIconButton.myID;
			_navRightButton.leftNeighborID = _recipeIconButton.myID;
			_navRightButton.rightNeighborID = _ingredientsClickables.First().myID;

			_navUpButton.upNeighborID = _toggleButtonClickables.Last().myID;
			_navDownButton.downNeighborID = 0;

			_cookButton.upNeighborID = _ingredientsClickables.First().myID;
			_cookButton.downNeighborID = 0;

			_cookConfirmButton.leftNeighborID = _cookQuantityUpButton.myID;
			_cookCancelButton.leftNeighborID = _cookQuantityDownButton.myID;
			_cookQuantityUpButton.rightNeighborID = _cookQuantityDownButton.rightNeighborID = _cookConfirmButton.myID;
			_cookQuantityUpButton.downNeighborID = _cookQuantityDownButton.myID;
			_cookQuantityDownButton.upNeighborID = _cookQuantityUpButton.myID;
			_cookConfirmButton.upNeighborID = _cookQuantityUpButton.upNeighborID = _ingredientsClickables.First().myID;
			_cookCancelButton.upNeighborID = _cookConfirmButton.myID;
			_cookConfirmButton.downNeighborID = _cookCancelButton.myID;
			_cookCancelButton.downNeighborID = _cookQuantityDownButton.downNeighborID = 0;

			// Child component navigation

			// inventory buttons and ingredients slots
			foreach (List<ClickableTextureComponent> clickableGroup in new [] { _ingredientsClickables, _inventorySelectButtons, _filterButtons })
			{
				for (int i = 0; i < clickableGroup.Count; ++i)
				{
					if (i > 0)
					{
						if (UseHorizontalInventoryButtonArea)
							clickableGroup[i].upNeighborID = clickableGroup[i - 1].myID;
						else
							clickableGroup[i].leftNeighborID = clickableGroup[i - 1].myID;
					}
					if (i < clickableGroup.Count - 1)
					{
						if (UseHorizontalInventoryButtonArea)
							clickableGroup[i].downNeighborID = clickableGroup[i + 1].myID;
						else
							clickableGroup[i].rightNeighborID = clickableGroup[i + 1].myID;
					}
				}
			}
			
			// inventory nav buttons
			_inventoryUpButton.rightNeighborID = 0; // first element in inventory
			_inventoryDownButton.rightNeighborID = this.GetColumnCount() * 2; // first element in 3rd row of inventory
			
			for (int i = 0; i < _searchListClickables.Count; ++i)
			{
				if (i > 0)
					_searchListClickables[i].upNeighborID = _searchListClickables[i - 1].myID;
				if (i < _searchListClickables.Count - 1)
					_searchListClickables[i].downNeighborID = _searchListClickables[i + 1].myID;
			}
			_searchListClickables.First().upNeighborID = _toggleFilterButton.myID;
			_searchListClickables.Last().downNeighborID = 0;
			for (int i = 0; i < _searchGridClickables.Count; ++i)
			{
				if (i > 0 && i % GridColumns != 0)
					_searchGridClickables[i].leftNeighborID = _searchGridClickables[i - 1].myID;
				if (i < _searchGridClickables.Count - 1)
					_searchGridClickables[i].rightNeighborID = _searchGridClickables[i + 1].myID;

				_searchGridClickables[i].upNeighborID = i < GridColumns
					? _toggleFilterButton.myID
					: _searchGridClickables[i - GridColumns].myID;
				_searchGridClickables[i].downNeighborID = i > _searchGridClickables.Count - 1 - GridColumns
					? 0
					: _searchGridClickables[i + GridColumns].myID;
			}
			foreach (ClickableTextureComponent clickable in _ingredientsClickables)
			{
				clickable.downNeighborID = 0;
			}
			if (this.ShouldShowInventoryElements)
			{
				_inventorySelectButtons.First().leftNeighborID = UseHorizontalInventoryButtonArea
					? -1
					: this.GetColumnCount() - 1; // last element in the first row of the inventory
				_inventorySelectButtons.First().upNeighborID = UseHorizontalInventoryButtonArea
					? this.GetColumnCount() * 2 // first element in the last row of the inventory
					: _inventorySelectButtons[1].upNeighborID = _ingredientsClickables.Last().myID; // last ingredient slot
			}

			// Add clickables to implicit navigation
			this.populateClickableComponentList();
		}

		private void RealignElements()
		{
			Vector2 centre = Utility.PointToVector2(Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Center);
			int xOffset = 0, yOffset = 0;

			// Menu
			yOffset = 54 * Scale;
			if (Context.IsSplitScreen)
			{
				centre.X /= 2;
			}
			if (UseHorizontalInventoryButtonArea)
			{
				yOffset = yOffset / 3 * 2;
			}
			yPositionOnScreen = (int)(centre.Y - CookbookSource.Center.Y * Scale * Game1.options.uiScale + yOffset);
			xPositionOnScreen = (int)(centre.X - CookbookSource.Center.X * Scale * Game1.options.uiScale + xOffset);

			// Cookbook menu
			_cookbookLeftRect.X = xPositionOnScreen;
			_cookbookRightRect.X = _cookbookLeftRect.X + _cookbookLeftRect.Width;
			_cookbookLeftRect.Y = _cookbookRightRect.Y = yPositionOnScreen;

			_leftContent = new Point(_cookbookLeftRect.X + MarginLeft, _cookbookLeftRect.Y);
			_rightContent = new Point(_cookbookRightRect.X + MarginRight, _cookbookRightRect.Y);

			_lineWidth = _cookbookLeftRect.Width - MarginLeft * 3 / 2; // Actually mostly even for both Left and Right pages
			_textWidth = _lineWidth + TextMuffinTopOverDivider * 2;

			// Extra clickables
			upperRightCloseButton.bounds.Y = yPositionOnScreen + (9 * Scale);
			upperRightCloseButton.bounds.X = xPositionOnScreen + CookbookSource.Width * Scale - (11 * Scale);

			if (Context.IsSplitScreen)
			{
				int pos = upperRightCloseButton.bounds.X + upperRightCloseButton.bounds.Width;
				int bound = Game1.viewport.Width / 2;
				float scale = Game1.options.uiScale;
				float diff = (pos - bound) * scale;
				upperRightCloseButton.bounds.X -= (int)Math.Max(0, diff / 2);
			}

			// Search elements
			_searchTabButton.bounds.Y = _cookbookLeftRect.Y + (18 * Scale);
			_ingredientsTabButton.bounds.Y = _searchTabButton.bounds.Y + _searchTabButton.bounds.Height + (4 * Scale);
			_searchTabButton.bounds.X = _ingredientsTabButton.bounds.X = _cookbookLeftRect.X - (10 * Scale);

			yOffset = 8 * Scale;
			xOffset = 10 * Scale;
			int extraOffset = 2 * Scale;

			// search bar text box
			_searchBarTextBox.X = _leftContent.X;
			_searchBarTextBox.Y = _leftContent.Y + yOffset + (1 * Scale);
			_searchBarTextBox.Selected = false;
			_searchBarTextBox.Update();
			_searchBarTextBoxBounds = new Rectangle(
				_searchBarTextBox.X, _searchBarTextBox.Y, -1, _searchBarTextBox.Height);

			// toggle button group
			_toggleButtonClickables.First().bounds.X = _cookbookRightRect.X
			    - _toggleButtonClickables.Sum(c => c.bounds.Width)
				- (extraOffset * _toggleButtonClickables.Count) - (6 * Scale);
			for (int i = 0; i < _toggleButtonClickables.Count; ++i)
			{
				_toggleButtonClickables[i].bounds.Y = _leftContent.Y + yOffset;
				if (i > 0)
					_toggleButtonClickables[i].bounds.X = _toggleButtonClickables[i - 1].bounds.X
						+ _toggleButtonClickables[i - 1].bounds.Width + extraOffset;
			}
			// contextual icons
			_toggleViewButton.sourceRect.X = ToggleViewButtonSource.X + (this.UsingRecipeGridView
				? ToggleViewButtonSource.Width
				: 0);
			_toggleAutofillButton.sourceRect.X = IsUsingAutofill
				? AutofillButtonSource.X + AutofillButtonSource.Width
				: AutofillButtonSource.X;

			_searchButton.bounds = _toggleButtonClickables.Last().bounds;
			_searchBarTextBoxMaxWidth = _searchButton.bounds.X - _searchBarTextBox.X - (6 * Scale);

			int minWidth = (33 * Scale);
			_searchBarTextBoxMinWidth = Math.Min(_toggleButtonClickables.First().bounds.X - _searchBarTextBoxBounds.X,
				Math.Max(minWidth, (6 * Scale) + (int)Math.Ceiling(Game1.smallFont.MeasureString(_searchBarTextBox.Text).X)));
			_searchBarTextBox.Width = _searchBarTextBoxMinWidth;
			_searchBarClickable.bounds = _searchBarTextBoxBounds;
			_searchBarTextBoxBounds.Width = _searchBarTextBox.Width;

			_navUpButton.bounds.X = _navDownButton.bounds.X = _searchButton.bounds.X + (1 * Scale);
			_navUpButton.bounds.Y = _searchButton.bounds.Y + _searchButton.bounds.Height + (4 * Scale);
			_navDownButton.bounds.Y = _cookbookLeftRect.Bottom - (32 * Scale);

			{
				float buttonScale = _filterButtons.First().baseScale;
				yOffset = (7 * Scale);

				for (int i = 0; i < _filterButtons.Count; ++i)
				{
					_filterButtons[i].bounds.Y = _toggleFilterButton.bounds.Y + _toggleFilterButton.bounds.Height + yOffset;

					// Aligned to right-side:
					//_filterButtons[i].bounds.X = _cookbookRightRect.X - xOffset - ((_filterButtons.Count - i) * _filterButtons[i].bounds.Width);
					// Aligned to left-side:
					_filterButtons[i].bounds.X = (int)(_searchBarTextBoxBounds.X + (7 * buttonScale) + (i * _filterButtons[i].bounds.Width));
				}

				Rectangle bounds = _filterButtons.First().bounds;
				_filterContainerMiddleSourceWidth = _filterButtons.Count * FilterIconSource.Width;
				_filterContainerBounds = new Rectangle(
					//(int)(bounds.X - (FilterContainerSideWidth * buttonScale) - (1 * buttonScale)),
					(int)(bounds.X - (FilterContainerSideSourceWidth * buttonScale) - (1 * buttonScale)),
					(int)(bounds.Y - ((FilterContainerSource.Height * buttonScale) - (FilterIconSource.Height * buttonScale)) / 2),
					(int)(((FilterContainerSideSourceWidth * 2) * buttonScale) + (_filterContainerMiddleSourceWidth * buttonScale)),
					(int)(FilterContainerSource.Height * buttonScale));

				int y = _filterContainerBounds.Y + (_showSearchFilters ? _filterContainerBounds.Height + (3 * Scale) : 0);
				_searchResultsArea = new Rectangle(
					_searchBarTextBox.X,
					y,
					_navUpButton.bounds.X - _searchBarTextBox.X - (8 * Scale),
					_navDownButton.bounds.Y + _navDownButton.bounds.Height - y - (2 * Scale));
			}

			// Recipe search results
			{
				// Set bounds for grid and list clickable buttons
				// centre grid icons in the results area, but keep them more towards the centre of the menu
				int x, y;
				// i tried adding parentheses to each y value and it became misaligned? not quite what it seems.
				yOffset = (_searchResultsArea.Height - (GridRows * RecipeGridHeight)) / 2;
				extraOffset = (_searchResultsArea.Width - (GridColumns * RecipeGridHeight));

				for (int i = 0; i < _searchGridClickables.Count; ++i)
				{
					y = _searchResultsArea.Y + yOffset + (i / GridColumns) * RecipeGridHeight + (RecipeGridHeight - (StardewValley.Object.spriteSheetTileSize * Scale)) / 2;
					x = _searchResultsArea.X + extraOffset + (i % GridColumns) * RecipeGridHeight;
					_searchGridClickables[i].bounds = new Rectangle(
						x,
						y,
						StardewValley.Object.spriteSheetTileSize * Scale,
						StardewValley.Object.spriteSheetTileSize * Scale);
				}

				x = _searchResultsArea.X;
				yOffset = (_searchResultsArea.Height % RecipeListHeight) / 2;
				for (int i = 0; i < ListRows; ++i)
				{
					y = _searchResultsArea.Y + yOffset + i * RecipeListHeight + (RecipeListHeight - (StardewValley.Object.spriteSheetTileSize * Scale)) / 2;
					_searchListClickables[i].bounds = new Rectangle(x, y, _searchResultsArea.Width, -1);
				}
				foreach (ClickableComponent clickable in _searchListClickables)
				{
					clickable.bounds.Height = _searchListClickables[_searchListClickables.Count - 1].bounds.Y
						- _searchListClickables[_searchListClickables.Count - 2].bounds.Y;
				}

				_searchListClickables.Last().visible = !this.HideLastRowOfSearchResults;
			}

			// Ingredient slots buttons
			{
				int w = _ingredientsClickables.First().bounds.Width;
				int h = _ingredientsClickables.First().bounds.Height;
				yOffset = 6 * Scale;
				xOffset = 0;
				extraOffset = 0;
				int extraSpace = (int)(w / 2f * (_ingredientsClickables.Count % IngredientSlotsWide) / 2f);
				for (int i = 0; i < _ingredientsClickables.Count; ++i)
				{
					xOffset += w;
					if (i % IngredientSlotsWide == 0)
					{
						if (i != 0)
							yOffset += h;
						xOffset = 0;
					}

					if (i == _ingredientsClickables.Count - (_ingredientsClickables.Count % IngredientSlotsWide))
						extraOffset = extraSpace;

					_ingredientsClickables[i].bounds.X = _rightContent.X + xOffset + extraOffset + (4 * Scale);
					_ingredientsClickables[i].bounds.Y = _rightContent.Y + yOffset;
				}
			}

			// Recipe nav buttons
			_navLeftButton.bounds.X = _leftContent.X - (6 * Scale);
			_navRightButton.bounds.X = _navLeftButton.bounds.X + _lineWidth - (3 * Scale);
			_navRightButton.bounds.Y = _navLeftButton.bounds.Y = _leftContent.Y + (6 * Scale);

			// Recipe icon
			_recipeIconButton.bounds.Y = _navLeftButton.bounds.Y + (1 * Scale);
			_recipeIconButton.bounds.X = _navLeftButton.bounds.X + _navLeftButton.bounds.Width;

			// Cook! button
			xOffset = _rightContent.X + _cookbookRightRect.Width / 2 - MarginRight;
			yOffset = _rightContent.Y + (86 * Scale);
			_cookTextMiddleSourceWidth = Math.Max(9 * Scale, CookTextSource[_locale].Width);
			_cookButton.bounds = new Rectangle(
				xOffset, yOffset,
				CookTextSideSourceWidth * Scale * 2 + _cookTextMiddleSourceWidth * Scale,
				CookButtonSource.Height * Scale);
			_cookButton.bounds.X -= (CookTextSourceWidths[_locale] / 2 * Scale - CookTextSideSourceWidth * Scale) + MarginLeft;

			// Cooking confirmation popup buttons
			{
				xOffset -= 40 * Scale;
				yOffset -= 9 * Scale;
				_cookIconBounds = new Rectangle(xOffset, yOffset + 6, 90, 90);

				xOffset += (12 * Scale) + _cookIconBounds.Width;
				_cookQuantityUpButton.bounds.X = _cookQuantityDownButton.bounds.X = xOffset;
				_cookQuantityUpButton.bounds.Y = yOffset - 12;

				Vector2 textSize = _quantityTextBox.Font.MeasureString(
					Game1.parseText("999", _quantityTextBox.Font, 96));
				_quantityTextBox.Text = QuantityTextBoxDefaultText;
				_quantityTextBox.limitWidth = false;
				_quantityTextBox.Width = (int)textSize.X + (6 * Scale);

				int extraSpace = (_quantityTextBox.Width - _cookQuantityUpButton.bounds.Width) / 2;
				_quantityTextBox.X = _cookQuantityUpButton.bounds.X - extraSpace;
				_quantityTextBox.Y = _cookQuantityUpButton.bounds.Y + _cookQuantityUpButton.bounds.Height + (2 * Scale);
				_quantityTextBox.Update();
				_quantityTextBoxBounds = new Rectangle(_quantityTextBox.X, _quantityTextBox.Y, _quantityTextBox.Width,
						_quantityTextBox.Height);

				_cookQuantityDownButton.bounds.Y = _quantityTextBox.Y + _quantityTextBox.Height + 5;

				_cookConfirmButton.bounds.X = _cookCancelButton.bounds.X
					= _cookQuantityUpButton.bounds.X + _cookQuantityUpButton.bounds.Width + extraSpace + (4 * Scale);
				_cookConfirmButton.bounds.Y = yOffset - (4 * Scale);
				_cookCancelButton.bounds.Y = _cookConfirmButton.bounds.Y + _cookConfirmButton.bounds.Height + (1 * Scale);

				extraSpace = (4 * Scale);
				_quantityScrollableArea = new Rectangle(
					_cookIconBounds.X - extraSpace,
					_cookIconBounds.Y - extraSpace,
					_cookConfirmButton.bounds.X + _cookConfirmButton.bounds.Width - _cookIconBounds.X + (extraSpace * 2),
					_cookCancelButton.bounds.Y + _cookCancelButton.bounds.Height - _cookConfirmButton.bounds.Y + (extraSpace * 2));
			}

			// Inventory
			const int itemsPerRow = 12;
			const int itemRows = 3;
			inventory.rows = _inventoryId == BackpackInventoryId && Interface.Interfaces.UsingBigBackpack
				? itemRows + 1
				: itemRows;
			inventory.capacity = inventory.rows * itemsPerRow;
			bool isHorizontal = UseHorizontalInventoryButtonArea;
			yOffset = yPositionOnScreen + CookbookSource.Height * Scale + (2 - 5) * Scale;
			extraOffset = (4 * Scale) + (StardewValley.Object.spriteSheetTileSize * Scale / 2);
			inventory.xPositionOnScreen = xPositionOnScreen + (CookbookSource.Width / 2 * Scale) - (inventory.width / 2)
				+ (isHorizontal ? (4 * Scale) : 0);
			inventory.yPositionOnScreen = yOffset + (Interface.Interfaces.UsingBigBackpack && _inventoryId == BackpackInventoryId
				? -(Math.Max(0, extraOffset - (Game1.uiViewport.Height - 720)))
				: 0);
			extraOffset = 2 * Scale;
			inventory.width = StardewValley.Object.spriteSheetTileSize * Scale * itemsPerRow;
			inventory.height = StardewValley.Object.spriteSheetTileSize * Scale * inventory.rows;
			_inventoryCardArea = new Rectangle(
				inventory.xPositionOnScreen - spaceToClearSideBorder - extraOffset,
				inventory.yPositionOnScreen - spaceToClearSideBorder - (extraOffset / 2),
				inventory.width + (spaceToClearSideBorder * 2) + (extraOffset * 2),
				inventory.height + (spaceToClearSideBorder * 2) + (extraOffset / 2));

			// Inventory items
			{
				yOffset = -1 * Scale;
				int rowSize = inventory.capacity / inventory.rows;
				for (int i = 0; i < inventory.capacity; ++i)
				{
					if (i % rowSize == 0 && i != 0)
						yOffset += inventory.inventory[i].bounds.Height + (1 * Scale);
					inventory.inventory[i].bounds.X = inventory.xPositionOnScreen + i % rowSize * inventory.inventory[i].bounds.Width;
					inventory.inventory[i].bounds.Y = inventory.yPositionOnScreen + yOffset;
				}
			}

			// Inventory select buttons
			// inventory buttons flow vertically in a solo-screen game, and horizontally in split-screen
			xOffset = 4 * Scale;
			yOffset = 1 * Scale;
			_inventoryTabButton.bounds.X = _inventoryCardArea.X - _inventoryTabButton.bounds.Width + (1 * Scale);
			_inventoryTabButton.bounds.Y = _inventoryCardArea.Y + ((_inventoryCardArea.Height - (InventoryTabButtonSource.Height * Scale)) / 2);
			_inventoryUpButton.bounds.X = _inventoryDownButton.bounds.X = _inventoryTabButton.bounds.X + xOffset;
			_inventoryUpButton.bounds.Y = _inventoryTabButton.bounds.Y - _inventoryUpButton.bounds.Height - yOffset;
			_inventoryDownButton.bounds.Y = _inventoryTabButton.bounds.Y + _inventoryTabButton.bounds.Height + yOffset;
			if (this.ShouldShowInventoryElements)
			{
				const int areaPadding = 3 * Scale;
				const int longSideSpacing = 4 * Scale;
				const int addedSpacing = 2 * Scale;

				int longSideLength = 2 * ((_inventorySelectButtons.Count + 1) / 2) / 2;
				int wideSideLength = 2;
				int xLength = (isHorizontal ? longSideLength : wideSideLength);
				int yLength = (isHorizontal ? wideSideLength : longSideLength);
				int cardHeight = (int)(_inventorySelectButtons[0].bounds.Height * (yLength + 0.5f)) + areaPadding;

				// Backpack and fridge
				{
					_inventorySelectButtons[0].bounds.X =
						(isHorizontal
							? (int)(centre.X + (32 * Scale)
								- (((_inventorySelectButtons.Count + 1) / 2) * ((_inventorySelectButtons[0].bounds.Width + (1 * Scale)) / 2)))
							: _inventoryTabButton.bounds.X - (2 * Scale)
								- (2 * _inventorySelectButtons[0].bounds.Width) - addedSpacing - (1 * Scale) - (4 * Scale));
					_inventorySelectButtons[1].bounds.X = _inventorySelectButtons[0].bounds.X
						+ (isHorizontal
							? 0
							: _inventorySelectButtons[0].bounds.Width);
					
					int maximumHeight = this.height;
					int itemHeight = _inventorySelectButtons[0].bounds.Height;
					float itemsPerScreen = maximumHeight / itemHeight;
					float itemRatio = (yLength - 1) / itemsPerScreen;
					int verticalPositionY = _inventoryTabButton.bounds.Y + ((_inventoryTabButton.bounds.Height - itemHeight) / 2);
					int heightDifference = maximumHeight - verticalPositionY + (areaPadding * 2);
					float offsetToFillSpaceBelow = (heightDifference + (itemHeight / 2)) * itemRatio / 2;
					verticalPositionY += (this.yPositionOnScreen / 2);
					verticalPositionY += (int)(offsetToFillSpaceBelow);
					_inventorySelectButtons[0].bounds.Y = isHorizontal
							? inventory.yPositionOnScreen + inventory.height + longSideSpacing + addedSpacing
							: verticalPositionY - ((yLength - 1) * itemHeight);
					_inventorySelectButtons[1].bounds.Y = _inventorySelectButtons[0].bounds.Y
						+ (isHorizontal
							? _inventorySelectButtons[0].bounds.Height + longSideSpacing
							: 0);
				}

				// Mini-fridges
				for (int i = _inventoryIdsBeforeMinifridges + 1; i < _inventorySelectButtons.Count; ++i)
				{
					int shortSideIndex = i % 2;
					int shortSidePlacement = 0;
					int longSideIndex = 0;
					int longSidePlacement = i / 2;
					_inventorySelectButtons[i].bounds.X =
						_inventorySelectButtons[(isHorizontal ? longSideIndex : shortSideIndex)].bounds.X
						+ (_inventorySelectButtons[0].bounds.Width * (isHorizontal ? longSidePlacement : shortSidePlacement))
						+ (isHorizontal ? addedSpacing : 0);
					_inventorySelectButtons[i].bounds.Y =
						_inventorySelectButtons[(isHorizontal ? shortSideIndex : longSideIndex)].bounds.Y
						+ (_inventorySelectButtons[0].bounds.Height * (isHorizontal ? shortSidePlacement : longSidePlacement))
						+ (isHorizontal ? 0 : addedSpacing);
				}

				// Area to draw inventory buttons popup
				_inventoriesPopupArea = new Rectangle(
					_inventorySelectButtons[0].bounds.X - addedSpacing,
					_inventorySelectButtons[0].bounds.Y - areaPadding - addedSpacing,
					(_inventorySelectButtons[0].bounds.Width + addedSpacing) * xLength + areaPadding,
					cardHeight);

				// Area to track user scrollwheel actions
				_inventoriesScrollableArea = new Rectangle(
					_inventoryTabButton.bounds.X,
					_inventoryCardArea.Y,
					inventory.xPositionOnScreen + inventory.width - _inventoryTabButton.bounds.X,
					_inventoryCardArea.Height);
			}
		}

		internal void InitialiseIngredientSlotButtons(int buttonsToDisplay, int usableButtons)
		{
			for (int i = 0; i < buttonsToDisplay; ++i)
			{
				Rectangle sourceRectangle = usableButtons <= i ? CookingSlotLockedSource : CookingSlotOpenSource;
				_ingredientsClickables.Add(new ClickableTextureComponent(
					"cookingSlot" + i,
					new Rectangle(-1, -1, CookingSlotOpenSource.Width * Scale, CookingSlotOpenSource.Height * Scale),
					null, null, Texture, sourceRectangle, Scale));
			}
		}

		internal static List<FarmerSprite.AnimationFrame> AnimateForRecipe(CraftingRecipe recipe, int quantity, int burntCount, bool containsFish)
		{
			// TODO: FIX: Why doesn't HUD draw while animating
			Game1.freezeControls = true;
			//Game1.displayHUD = true;
			//Game1.activeClickableMenu = null; // not work!

			string name = recipe.name.ToLower();
			bool isBaked = ModEntry.ItemDefinitions["BakeyFoods"].Any(o => name.StartsWith(o)
				|| ModEntry.ItemDefinitions["CakeyFoods"].Any(o => name.EndsWith(o)));
			string startSound, sound, endSound;
			if (ModEntry.ItemDefinitions["SoupyFoods"].Any(x => name.EndsWith(x)))
			{
				startSound = "dropItemInWater";
				sound = "dropItemInWater";
				endSound = "bubbles";
			}
			else if (ModEntry.ItemDefinitions["DrinkyFoods"].Any(x => name.EndsWith(x)))
			{
				startSound = "Milking";
				sound = "dropItemInWater";
				endSound = "bubbles";
			}
			else if (ModEntry.ItemDefinitions["SaladyFoods"].Any(x => name.EndsWith(x)))
			{
				startSound = "daggerswipe";
				sound = "daggerswipe";
				endSound = "daggerswipe";
			}
			else
			{
				startSound = "slime";
				sound = "slime";
				endSound = "fireball";
			}

			if (containsFish)
			{
				startSound = "fishSlap";
			}
			if (isBaked)
			{
				endSound = "furnace";
			}

			Game1.player.Halt();
			Game1.player.FarmerSprite.StopAnimation();
			Game1.player.completelyStopAnimatingOrDoingAction();
			Game1.player.faceDirection(0);

			Multiplayer multiplayer = Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
			Vector2 spritePosition = Vector2.Zero;
			TemporaryAnimatedSprite sprite = null;
			float spriteScale = Game1.pixelZoom;
			Game1.currentLocation.removeTemporarySpritesWithID(SpriteId);

			int ms = 330;
			List<FarmerSprite.AnimationFrame> frames = startSound == "Milking"
				? new List<FarmerSprite.AnimationFrame>
				{
					// Spout
					new FarmerSprite.AnimationFrame(36, ms)
					{ frameEndBehavior = delegate { Game1.playSound(startSound); } },
					new FarmerSprite.AnimationFrame(66, ms * 5),
					new FarmerSprite.AnimationFrame(44, ms),
				}
				: new List<FarmerSprite.AnimationFrame>
				{
					// Jumble
					new FarmerSprite.AnimationFrame(44, ms)
						{ frameEndBehavior = delegate { Game1.playSound(startSound); } },
					new FarmerSprite.AnimationFrame(66, ms),
					new FarmerSprite.AnimationFrame(44, ms)
						{ frameEndBehavior = delegate { Game1.playSound(sound); } },
					new FarmerSprite.AnimationFrame(66, ms),
					new FarmerSprite.AnimationFrame(44, ms)
						{ frameEndBehavior = delegate { Game1.playSound(endSound); } },
					new FarmerSprite.AnimationFrame(66, ms),
				};

			// Oven-baked foods
			if (isBaked && !ModEntry.ItemDefinitions["PancakeyFoods"].Any(o => name.Contains(o)))
			{
				frames[frames.Count - 1] = new FarmerSprite.AnimationFrame(58, ms * 2);
				frames.Add(new FarmerSprite.AnimationFrame(44, ms * 8)
				{
					frameEndBehavior = delegate
					{
						Game1.playSound("fireball");
						Game1.player.FacingDirection = 0;
					}
				});
				frames.Add(new FarmerSprite.AnimationFrame(58, ms * 2));
				frames.Add(new FarmerSprite.AnimationFrame(0, ms));
			}

			// Dough-tossing foods
			if (ModEntry.ItemDefinitions["PizzayFoods"].Any(o => name.Contains(o)))
			{
				Game1.player.faceDirection(2);

				ms = 100;

				// Before jumble
				List<FarmerSprite.AnimationFrame> newFrames = new List<FarmerSprite.AnimationFrame>
				{
					// Toss dough
					new FarmerSprite.AnimationFrame(54, 0)
					{
						frameEndBehavior = delegate
						{
							multiplayer.broadcastSprites(Game1.currentLocation, sprite);
						}
					},
					new FarmerSprite.AnimationFrame(54, ms)
					{ frameEndBehavior = delegate { Game1.playSound("breathin"); } },
					new FarmerSprite.AnimationFrame(55, ms),
					new FarmerSprite.AnimationFrame(56, ms),
					new FarmerSprite.AnimationFrame(57, ms * 8)
					{ frameEndBehavior = delegate { Game1.playSound("breathout"); } },
					new FarmerSprite.AnimationFrame(56, ms),
					new FarmerSprite.AnimationFrame(55, ms),
					new FarmerSprite.AnimationFrame(54, ms)
						{ frameEndBehavior = delegate { Game1.player.FacingDirection = 0; } },
				};

				// Extra sprite
				spritePosition = new Vector2(Game1.player.Position.X, Game1.player.Position.Y - 40 * spriteScale);
				sprite = new TemporaryAnimatedSprite(
						textureName: AssetManager.GameContentSpriteSheetPath,
						sourceRect: new Rectangle(0, 336, 16, 48),
						animationInterval: ms, animationLength: 16, numberOfLoops: 0,
						position: spritePosition,
						flicker: false, flipped: false)
				{
					scale = spriteScale,
					id = SpriteId
				};

				// Compile frames
				frames = newFrames.Concat(frames).ToList();
			}

			// Pan-flipping foods
			else if (ModEntry.ItemDefinitions["PancakeyFoods"].Any(o => name.Contains(o)))
			{
				ms = 100;

				// After jumble
				List<FarmerSprite.AnimationFrame> newFrames = new List<FarmerSprite.AnimationFrame>
				{
					// Flip pancake
					new FarmerSprite.AnimationFrame(29, 0)
						{ frameEndBehavior = delegate { Game1.player.FacingDirection = 2; } },
					new FarmerSprite.AnimationFrame(29, ms)
					{ frameEndBehavior = delegate { Game1.playSound("swordswipe"); } },
					new FarmerSprite.AnimationFrame(28, ms),
					new FarmerSprite.AnimationFrame(27, ms),
					new FarmerSprite.AnimationFrame(26, ms),
					new FarmerSprite.AnimationFrame(25, ms * 6),
					new FarmerSprite.AnimationFrame(26, ms)
					{ frameEndBehavior = delegate { Game1.playSound("pullItemFromWater"); } },
					new FarmerSprite.AnimationFrame(27, ms),
					new FarmerSprite.AnimationFrame(28, ms),
					new FarmerSprite.AnimationFrame(29, ms * 2),
					new FarmerSprite.AnimationFrame(28, ms),
					new FarmerSprite.AnimationFrame(0, ms),
				};

				// Extra sprite
				spritePosition = new Vector2(Game1.player.Position.X, Game1.player.Position.Y - 40 * spriteScale);
				sprite = new TemporaryAnimatedSprite(
						textureName: AssetManager.GameContentSpriteSheetPath,
						sourceRect: new Rectangle(0, 288, 16, 48),
						animationInterval: ms, animationLength: 16, numberOfLoops: 0,
						position: spritePosition,
						flicker: false, flipped: false)
						{
							scale = spriteScale,
							id = SpriteId
						};

				frames[frames.Count - 1] = new FarmerSprite.AnimationFrame(
					frames[frames.Count - 1].frame, frames [frames.Count - 1].milliseconds)
					{
						frameEndBehavior = delegate
						{
							multiplayer.broadcastSprites(Game1.currentLocation, sprite);
						}
					};

				// Compile frames
				frames = frames.Concat(newFrames).ToList();
			}

			// Burn the whole entire house down
			// TODO: FIX: How do i do this
			burntCount = 0;
			if (burntCount > 0)
			{
				int frameCount = 4, loopCount = 8;
				spritePosition = new Vector2(Game1.player.Position.X, Game1.player.Position.Y - 40 * spriteScale);
				sprite = new TemporaryAnimatedSprite(
					textureName: Game1.animationsName,
					//sourceRect: new Rectangle(0, 1856, 64, 64), // Smoke
					sourceRect: new Rectangle(0, 1916, 64, 64), // Fire
					animationInterval: ms, animationLength: frameCount, numberOfLoops: loopCount,
					position: spritePosition,
					flicker: false, flipped: false);

				frames[frames.Count - 1] = new FarmerSprite.AnimationFrame(
					frames[frames.Count - 1].frame, frames[frames.Count - 1].milliseconds + (ms * frameCount * loopCount))
				{
					frameEndBehavior = delegate
					{
						Game1.player.FacingDirection = 2;
						Game1.player.jitterStrength = 0.2f;
						Game1.player.jump();
						multiplayer.broadcastSprites(Game1.currentLocation, sprite);
					}
				};
			}

			// Avoid animation problems?
			frames.Insert(0, new FarmerSprite.AnimationFrame(44, 0));
			// Face forwards after animation
			frames[frames.Count - 1] = new FarmerSprite.AnimationFrame
				(frames[frames.Count - 1].frame, frames[frames.Count - 1].milliseconds)
				{
					frameEndBehavior = delegate {
						Game1.player.jitterStrength = 0f;
						Game1.player.stopJittering();
						Game1.freezeControls = false;
						Game1.player.FacingDirection = 2;
						//Game1.playSound(SuccessCue);
						Game1.addHUDMessage(new HUDMessage("item", quantity, true, Color.White, recipe.createItem()));
					}
				};
			// Play animation
			Game1.player.FarmerSprite.animateOnce(frames.ToArray());
			//Game1.player.holdUpItemThenMessage(recipe.createItem(), false);

			return frames;
		}

		private void OpenSearchPage()
		{
			if (_stack.Count > 0 && _stack.Peek() == State.Search)
				_stack.Pop();
			_stack.Push(State.Search);

			_searchTabButton.sourceRect.X = SearchTabButtonSource.X + SearchTabButtonSource.Width;
			_ingredientsTabButton.sourceRect.X = IngredientsTabButtonSource.X;
			_showSearchFilters = false;

			if (Game1.options.SnappyMenus)
			{
				this.setCurrentlySnappedComponentTo(_recipeSearchResults.Count > 0
					? this.UsingRecipeGridView
						? _searchGridClickables.First().myID
						: _searchListClickables.First().myID
					: _searchBarClickable.myID);
			}
		}

		private void OpenRecipePage()
		{
			if (_stack.Count > 0 && _stack.Peek() == State.Recipe)
				_stack.Pop();
			_stack.Push(State.Recipe);

			_searchTabButton.sourceRect.X = SearchTabButtonSource.X;
			_ingredientsTabButton.sourceRect.X = IngredientsTabButtonSource.X;
			this.ToggleFilterPopup(playSound: false, forceToggleTo: false);
			this.TryAutoFillIngredients();

			if (Game1.options.SnappyMenus)
			{
				this.setCurrentlySnappedComponentTo(
					IsUsingAutofill
					? ReadyToCook
						? _cookButton.myID
						: 0
					: 0);
			}
		}

		private void CloseRecipePage()
		{
			if (_stack.Count > 0 && _stack.Peek() == State.Recipe)
				_stack.Pop();

			_showCookingConfirmPopup = false;
			_searchTabButton.sourceRect.X = SearchTabButtonSource.X + SearchTabButtonSource.Width;
			_ingredientsTabButton.sourceRect.X = IngredientsTabButtonSource.X;
			this.KeepRecipeIndexInSearchBounds();

			if (Game1.options.SnappyMenus)
			{
				this.setCurrentlySnappedComponentTo(this.UsingRecipeGridView
					? _searchGridClickables[_recipeSearchResults.Count / 2].myID
					: _searchListClickables[_recipeSearchResults.Count / 2].myID);
			}

			if (IsUsingAutofill)
			{
				// Remove all items from ingredients slots
				_cookingManager.ClearCurrentIngredients();
			}
		}

		private void OpenIngredientsPage()
		{
			if (_stack.Count > 0 && _stack.Peek() == State.Ingredients)
				_stack.Pop();
			_stack.Push(State.Ingredients);
			
			_searchTabButton.sourceRect.X = SearchTabButtonSource.X;
			_ingredientsTabButton.sourceRect.X = SearchTabButtonSource.X + SearchTabButtonSource.Width;
			this.ToggleFilterPopup(playSound: false, forceToggleTo: false);
		}

		private void CloseIngredientsPage()
		{
			if (_stack.Count > 0 && _stack.Peek() == State.Ingredients)
				_stack.Pop();

			_ingredientsTabButton.sourceRect.X = IngredientsTabButtonSource.X;

			if (_stack.Count > 0 && _stack.Peek() == State.Search)
			{
				_recipesFiltered = this.FilterRecipes(_lastFilterUsed, _searchBarTextBox.Text);
			}
		}

		private void CloseTextBox(TextBox textBox, bool reapplyFilters)
		{
			textBox.Selected = false;
			Game1.keyboardDispatcher.Subscriber = null;

			if (reapplyFilters && textBox.Text == _searchBarTextBox.Text)
			{
				_recipesFiltered = this.FilterRecipes(_lastFilterUsed, substr: _searchBarTextBox.Text);
				this.UpdateSearchRecipes();
			}
		}

		private List<CraftingRecipe> ReverseRecipeList(List<CraftingRecipe> recipes)
		{
			ModEntry.Instance.States.Value.LastFilterReversed = !ModEntry.Instance.States.Value.LastFilterReversed;
			recipes.Reverse();
			_recipeIndex = _recipeSearchResults.Count / 2;
			return recipes;
		}

		private List<CraftingRecipe> FilterRecipes(Filter which = Filter.Alphabetical,
			string substr = null)
		{
			bool isOrderReversedToStart = false;
			ModEntry.Instance.States.Value.LastFilterReversed = false;
			Func<CraftingRecipe, object> order = null;
			Func<CraftingRecipe, bool> filter = null;
			switch (which)
			{
				case Filter.Energy:
					order = recipe => recipe.createItem().staminaRecoveredOnConsumption();
					isOrderReversedToStart = true;
					break;
				case Filter.Gold:
					order = recipe => recipe.createItem().salePrice();
					isOrderReversedToStart = true;
					break;
				case Filter.Buffs:
					filter = recipe =>
						(!Config.HideFoodBuffsUntilEaten
						|| (ModEntry.Instance.States.Value.FoodsEaten.Contains(recipe.name)))
						&& Game1.objectInformation[recipe.createItem().ParentSheetIndex].Split('/').Length > 6
						&& Game1.objectInformation[recipe.createItem().ParentSheetIndex].Split('/')[7].Split(' ').Any(i => int.Parse(i) != 0);
					break;
				case Filter.New:
					filter = recipe => !Game1.player.recipesCooked.ContainsKey(recipe.createItem().ParentSheetIndex);
					break;
				case Filter.Ready:
					filter = recipe => recipe.recipeList.Count <= _cookingManager.MaxIngredients
						&& 0 < _cookingManager.GetAmountCraftable(recipe: recipe, sourceItems: _allInventories, limitToCurrentIngredients: false);
					break;
				case Filter.Favourite:
					filter = recipe => ModEntry.Instance.States.Value.FavouriteRecipes.Contains(recipe.name);
					break;
				default:
					order = recipe => recipe.DisplayName;
					break;
			}

			List<CraftingRecipe> recipes = (order != null
				? isOrderReversedToStart
					? _recipesAvailable.OrderByDescending(order)
					: _recipesAvailable.OrderBy(order)
				: _recipesAvailable.Where(filter)).ToList();
			if (!string.IsNullOrEmpty(substr) && substr != i18n.Get("menu.cooking_recipe.search_label"))
			{
				substr = substr.ToLower();
				recipes = recipes.Where(recipe => recipe.DisplayName.ToLower().Contains(substr)).ToList();
			}

			if (!recipes.Any())
				recipes.Add(new CraftingRecipe("none", true));

			if (_recipeSearchResults != null)
			{
				this.UpdateSearchRecipes();
				_recipeIndex = _recipeSearchResults.Count / 2;
			}

			_lastFilterUsed = which;

			// Change toggle filter button icon
			if (_toggleFilterButton != null)
				_toggleFilterButton.sourceRect.X = (_lastFilterUsed == Filter.None ? ToggleFilterButtonSource.X : ToggleFilterButtonSource.X - ToggleFilterButtonSource.Width);

			return recipes;
		}

		private void UpdateSearchRecipes()
		{
			_navUpButton.bounds.Y = _searchButton.bounds.Y + _searchButton.bounds.Height + (4 * Scale);

			_recipeSearchResults.Clear();

			bool isGridView = this.UsingRecipeGridView;
			_recipeDisplayHeight = isGridView
				? RecipeGridHeight
				: RecipeListHeight;
			_searchResultsPerPage = isGridView
				? _searchGridClickables.Count
				: _searchListClickables.Count;
			int minRecipe = Math.Max(0, _recipeIndex - _searchResultsPerPage / 2);
			int maxRecipe = Math.Min(_recipesFiltered.Count, minRecipe + _searchResultsPerPage);

			for (int i = minRecipe; i < maxRecipe; ++i)
				_recipeSearchResults.Add(_recipesFiltered[i]);
			while (isGridView && _recipeSearchResults.Count % 4 != 0)
				_recipeSearchResults.Add(null);
		}

		private void ToggleCookingConfirmPopup(bool playSound)
		{
			_showCookingConfirmPopup = !_showCookingConfirmPopup;
			_quantityTextBox.Text = QuantityTextBoxDefaultText.PadLeft(2, ' ');
			if (playSound)
				Game1.playSound(_showCookingConfirmPopup ? "bigSelect" : "bigDeSelect");

			if (Game1.options.SnappyMenus)
			{
				this.setCurrentlySnappedComponentTo(_showCookingConfirmPopup
					? _cookConfirmButton.myID
					: _ingredientsClickables.First().myID);
			}
		}

		private void ToggleFilterPopup(bool playSound, bool? forceToggleTo = null)
		{
			if (forceToggleTo.HasValue && forceToggleTo.Value == _showSearchFilters)
				return;

			_showSearchFilters = forceToggleTo ?? !_showSearchFilters;
			if (playSound)
				Game1.playSound(PageChangeCue);

			this.RealignElements();
			
			if (Game1.options.SnappyMenus)
			{
				this.setCurrentlySnappedComponentTo(_showSearchFilters ? _filterButtons.First().myID : _toggleFilterButton.myID);
			}
		}

		private void ToggleInventoriesPopup(bool playSound, bool? forceToggleTo = null)
		{
			if (!this.ShouldShowInventoryElements)
				return;

			if (forceToggleTo.HasValue && forceToggleTo.Value == this._showInventoriesPopup)
				return;

			if (playSound)
				Game1.playSound(this._showInventoriesPopup ? "bigSelect" : "bigDeSelect");

			this._showInventoriesPopup = forceToggleTo ?? !this._showInventoriesPopup;

			if (Game1.options.SnappyMenus)
			{
				this.setCurrentlySnappedComponentTo(this._showInventoriesPopup ? this._inventorySelectButtons.First().myID : this._inventoryTabButton.myID);
			}
		}

		private void ValidateNumericalTextBox(TextBox sender)
		{
			int.TryParse(sender.Text.Trim(), out int value);
			value = value > 0 ? value : 1;
			sender.Text = Math.Max(1, Math.Min(99,
				Math.Min(value, this._recipeReadyToCraftCount))).ToString();
			sender.Text = sender.Text.PadLeft(sender.Text.Length == 2 ? 3 : 2, ' ');
			sender.Selected = false;
		}

		private void KeepRecipeIndexInSearchBounds()
		{
			this._recipeIndex = Math.Max(this._recipeSearchResults.Count / 2,
				Math.Min(this._recipesFiltered.Count - this._recipeSearchResults.Count / 2 - 1, this._recipeIndex));

			// Avoid showing whitespace after end of list
			if (this.UsingRecipeGridView)
			{
				this._recipeIndex = GridColumns * (this._recipeIndex / GridColumns) + GridColumns;
				if (this._recipesFiltered.Count - 1 - this._recipeIndex < this._recipeSearchResults.Count / 2)
				{
					this._recipeIndex -= GridColumns;
				}
			}
			else
			{
				if (this._recipesFiltered.Count - this._recipeIndex <= (this._recipeSearchResults.Count + 1) / 2)
					--this._recipeIndex;
			}
		}

		private void ChangeCurrentRecipe(bool selectNext)
		{
			if (!this._stack.Any())
				return;
			State state = this._stack.Peek();

			ClickableTextureComponent clickable = selectNext
				? state == State.Search ? this._navDownButton : this._navRightButton
				: state == State.Search ? this._navUpButton : this._navLeftButton;
			this.TryClickNavButton(clickable.bounds.X, clickable.bounds.Y, state == State.Recipe);
		}

		private void ChangeCurrentRecipe(int index)
		{
			if (!this._recipesFiltered.Any())
				return;
			index = Math.Max(0, Math.Min(this._recipesFiltered.Count - 1, index));
			this.ChangeCurrentRecipe(this._recipesFiltered[index].name);
		}

		private void ChangeCurrentRecipe(string name)
		{
			if (!this._recipesFiltered.Any())
				return;
			CraftingRecipe recipe = new CraftingRecipe(name, isCookingRecipe: true);
			this._recipeIndex = this._recipesFiltered.FindIndex(recipe => recipe.name == name);
			this._recipeAsItem = recipe.createItem();
			string[] info = Game1.objectInformation[this._recipeAsItem.ParentSheetIndex].Split('/');
			List<int> buffs = info.Length >= 7
				? info[7].Split(' ').ToList().ConvertAll(int.Parse)
				: null;
			this._recipeBuffs = buffs != null && !buffs.All(b => b == 0)
				? buffs
				: null;
			this._recipeBuffDuration = this._recipeBuffs != null && info.Length >= 8
				? (int.Parse(info[8]) * 7 / 10 / 10) * 10
				: -1;
			if (this._stack.Count > 0 && this._stack.Peek() != State.Search)
			{
				this.TryAutoFillIngredients();
			}
		}

		private void UpdateCraftableCounts(CraftingRecipe recipe)
		{
			this._recipeIngredientQuantitiesHeld.Clear();
			for (int i = 0; i < CurrentRecipe?.getNumberOfIngredients(); ++i)
			{
				int id = CurrentRecipe.recipeList.Keys.ElementAt(i);
				int requiredQuantity = CurrentRecipe.recipeList.Values.ElementAt(i);
				int heldQuantity = 0;
				List<CookingManager.Ingredient> ingredients = CookingManager.GetMatchingIngredients(id: id, sourceItems: this._allInventories, required: requiredQuantity);
				if (ingredients != null && ingredients.Count > 0)
				{
					heldQuantity = ingredients.Sum(ing => this._cookingManager.GetItemForIngredient(ingredient: ing, sourceItems: this._allInventories).Stack);
					requiredQuantity -= heldQuantity;
				}

				this._recipeIngredientQuantitiesHeld.Add(heldQuantity);
			}
			this._recipeCraftableCount = this._cookingManager.GetAmountCraftable(recipe: recipe, sourceItems: this._allInventories, limitToCurrentIngredients: false);
			this._recipeReadyToCraftCount = this._cookingManager.GetAmountCraftable(recipe: recipe, sourceItems: this._allInventories, limitToCurrentIngredients: true);
		}

		private void ChangeInventory(bool selectNext)
		{
			int delta = selectNext ? 1 : -1;
			int index = this._inventoryId;
			if (this._allInventories.Count > 1)
			{
				// Navigate in given direction
				index += delta;
				// Negative-delta navigation cycles around to end
				if (index < BackpackInventoryId)
					index = this._allInventories.Count - 1;
				// Positive-delta navigation cycles around to start
				if (index == this._allInventories.Count)
					index = BackpackInventoryId;
			}

			this.ChangeInventory(index);
		}

		private void ChangeInventory(int index)
		{
			this._inventoryId = index;
			this.inventory.actualInventory = this._allInventories[this._inventoryId];
			this.inventory.showGrayedOutSlots = this._inventoryId == BackpackInventoryId;
			if (Interface.Interfaces.UsingBigBackpack)
				this.RealignElements();
		}

		private void TryAutoFillIngredients()
		{
			if (this.IsUsingAutofill)
			{
				// Remove all items from ingredients slots
				this._cookingManager.ClearCurrentIngredients();

				// Don't fill slots if the player can't cook the recipe
				if (this._stack.Any() && this._recipeIndex >= 0 && this._recipesFiltered.Count >= this._recipeIndex - 1)
				{
					this._cookingManager.AutoFillIngredients(recipe: CurrentRecipe, sourceItems: this._allInventories);
				}
			}

			this.UpdateCraftableCounts(recipe: this.CurrentRecipe);
		}

		private void TryClickNavButton(int x, int y, bool playSound)
		{
			if (!this._stack.Any())
				return;
			int lastRecipe = this._recipeIndex;
			State state = this._stack.Peek();
			int max = this._recipesFiltered.Count - 1;
			if (this.UsingRecipeGridView)
			{
				max = GridColumns * (max / GridColumns) + GridColumns;
			}
			int delta = Game1.isOneOfTheseKeysDown(Game1.oldKBState, new[] {new InputButton(Keys.LeftShift)})
				? this._searchResultsPerPage
				: this.UsingRecipeGridView && state == State.Search ? this._searchResultsArea.Width / this._recipeDisplayHeight : 1;
			switch (state)
			{
				case State.Search:
					if (!this._recipeSearchResults.Any())
						break;

					// Search up/down nav buttons
					if (this._navUpButton.containsPoint(x, y))
					{
						this._recipeIndex = Math.Max(this._recipeSearchResults.Count / 2, this._recipeIndex - delta);
					}
					else if (_navDownButton.containsPoint(x, y))
					{
						this._recipeIndex = Math.Min(max - this._recipeSearchResults.Count / 2, this._recipeIndex + delta);
					}
					else
					{
						return;
					}
					break;

				case State.Recipe:
					// Recipe next/prev nav buttons
					if (this._navLeftButton.containsPoint(x, y))
					{
						this.ChangeCurrentRecipe(this._recipeIndex - delta);
						this._showCookingConfirmPopup = false;
					}
					else if (this._navRightButton.containsPoint(x, y))
					{
						this.ChangeCurrentRecipe(this._recipeIndex + delta);
						this._showCookingConfirmPopup = false;
					}
					else
						return;
					break;

				case State.Ingredients:

					break;

				default:
					return;
			}

			if (Game1.options.SnappyMenus && this.currentlySnappedComponent != null && !this.IsNavButtonActive(this.currentlySnappedComponent.myID))
			{
				if (this.currentlySnappedComponent.myID == this._navLeftButton.myID || this.currentlySnappedComponent.myID == this._navRightButton.myID)
					this.setCurrentlySnappedComponentTo(this._recipeIconButton.myID);
				if (this.currentlySnappedComponent.myID == this._navUpButton.myID || this.currentlySnappedComponent.myID == this._navDownButton.myID)
					this.setCurrentlySnappedComponentTo(this._recipeSearchResults.Count > 0
						? this.UsingRecipeGridView
							? this._searchGridClickables.First().myID
							: this._searchListClickables.First().myID
						: this._toggleFilterButton.myID);
			}

			if (playSound && this._recipeIndex != lastRecipe)
				Game1.playSound(state == State.Search ? ClickCue : "newRecipe");
		}

		private void TryClickQuantityButton(int x, int y)
		{
			int delta = Game1.isOneOfTheseKeysDown(Game1.oldKBState, new[] {new InputButton(Keys.LeftShift)})
				? 10 : 1;
			int.TryParse(_quantityTextBox.Text.Trim(), out int value);
			value = value > 0 ? value : 1;

			if (_cookQuantityUpButton.containsPoint(x, y))
				value += delta;
			else if (_cookQuantityDownButton.containsPoint(x, y))
				value -= delta;
			else
				return;

			_quantityTextBox.Text = value.ToString();
			this.ValidateNumericalTextBox(_quantityTextBox);
			Game1.playSound(int.Parse(_quantityTextBox.Text) == value ? ClickCue : CancelCue);
			_quantityTextBox.Update();
		}

		/// <summary>
		/// Checks for any items under the cursor in either the current inventory or the ingredients dropIn slots, and moves them from one set to another if possible.
		/// </summary>
		/// <returns>Whether items were added or removed from current ingredients.</returns>
		public bool TryClickItem(int x, int y)
		{
			Item item = inventory.getItemAt(x, y);
			int itemIndex = inventory.getInventoryPositionOfClick(x, y);

			// Add an inventory item to an ingredient slot
			bool itemWasMoved = this.ClickedInventoryItem(item: item, itemIndex: itemIndex);

			// Return a dropIn ingredient item to the inventory
			for (int i = 0; i < _ingredientsClickables.Count && !itemWasMoved; ++i)
			{
				if (!_ingredientsClickables[i].containsPoint(x, y))
					continue;
				itemWasMoved = _cookingManager.RemoveFromIngredients(ingredientsIndex: i);
			}

			// Refresh contextual interactibles
			if (itemWasMoved)
			{
				if (_showCookingConfirmPopup)
				{
					this.ToggleCookingConfirmPopup(playSound: false);
				}
				this.UpdateCraftableCounts(recipe: this.CurrentRecipe);
			}

			// Snap to the Cook! button if appropriate
			if (Game1.options.SnappyMenus
				&& itemWasMoved
				&& ReadyToCook
				&& currentlySnappedComponent != null
				&& currentlySnappedComponent.myID != _navLeftButton.myID
				&& currentlySnappedComponent.myID != _navRightButton.myID)
			{
				this.setCurrentlySnappedComponentTo(_cookButton.myID);
			}

			return itemWasMoved;
		}

		private bool ClickedInventoryItem(Item item, int itemIndex)
		{
			bool itemWasMoved = false;
			if (_cookingManager.IsInventoryItemInCurrentIngredients(inventoryIndex: _inventoryId, itemIndex: itemIndex))
			{
				// Try to remove inventory item from its ingredient slot
				itemWasMoved = _cookingManager.RemoveFromIngredients(inventoryId: _inventoryId, itemIndex: itemIndex);
			}
			if (!itemWasMoved && CookingManager.CanBeCooked(item: item) && !_cookingManager.AreAllIngredientSlotsFilled)
			{
				// Try add inventory item to an empty ingredient slot
				itemWasMoved = _cookingManager.AddToIngredients(whichInventory: _inventoryId, whichItem: itemIndex, itemId: item.ParentSheetIndex);
			}
			if (itemWasMoved)
			{
				Game1.playSound(ClickCue);
			}
			else
			{
				if (item != null)
				{
					inventory.ShakeItem(item);
					Game1.playSound(CancelCue);
				}
			}
			return itemWasMoved;
		}

		/// <summary>
		/// Pre-flight checks before calling CookRecipe.
		/// </summary>
		/// <returns>Whether or not any food was crafted.</returns>
		public bool TryCookRecipe(CraftingRecipe recipe, int quantity)
		{
			int craftableCount = Math.Min(quantity, _cookingManager.GetAmountCraftable(recipe, _allInventories, limitToCurrentIngredients: true));
			if (craftableCount < 1)
				return false;

			int burntCount = _cookingManager.CookRecipe(recipe: recipe, sourceItems: _allInventories, quantity: craftableCount);
			if (Config.PlayCookingAnimation)
			{
				if (Game1.activeClickableMenu is CookingMenu cookingMenu)
				{
					Game1.displayHUD = true;
					CookingMenu.AnimateForRecipe(recipe: recipe, quantity: quantity, burntCount: burntCount,
						containsFish: recipe.recipeList.Any(pair => new StardewValley.Object(pair.Key, 0).Category == -4));
					cookingMenu.PopMenuStack(playSound: false, tryToQuit: true);
				}
			}
			else
			{
				Game1.playSound(SuccessCue);
			}

			return true;
		}

		private int TryGetIndexForSearchResult(int x, int y)
		{
			int index = this.UsingRecipeGridView
				? _searchGridClickables.IndexOf(_searchGridClickables.FirstOrDefault(c => c.containsPoint(x, y) && c.visible))
				: _searchListClickables.IndexOf(_searchListClickables.FirstOrDefault(c => c.containsPoint(x, y) && c.visible));
			return index;
		}

		private bool IsNavButtonActive(int id)
		{
			ClickableComponent clickable = getComponentWithID(id);
			if (clickable == null || !clickable.visible)
				return false;

			if (id == _navUpButton.myID)
				return _recipeIndex > _recipeSearchResults.Count / 2;
			if (id == _navLeftButton.myID)
				return _recipeIndex > 0;
			if (id == _navRightButton.myID)
				return _recipeIndex < _recipesFiltered.Count - 1;
			if (id == _navDownButton.myID)
				return (this.UsingRecipeGridView && _recipeIndex < _recipesFiltered.Count - (_searchResultsPerPage / 2))
					|| (!this.UsingRecipeGridView && _recipesFiltered.Count - _recipeIndex > 1 + (_searchResultsPerPage / 2));

			return false;
		}

		internal bool PopMenuStack(bool playSound, bool tryToQuit = false)
		{
			try
			{
				if (!_stack.Any())
					return false;

				if (_showCookingConfirmPopup)
				{
					this.ToggleCookingConfirmPopup(playSound: true);
					if (!tryToQuit)
						return false;
				}
				if (_showSearchFilters)
				{
					this.ToggleFilterPopup(playSound: true);
					if (!tryToQuit)
						return false;
				}
				if (_searchBarTextBox.Selected)
				{
					this.CloseTextBox(_searchBarTextBox, reapplyFilters: true);
					if (!tryToQuit)
						return false;
				}

				_cookingManager.ClearCurrentIngredients();

				State state = _stack.Peek();
				if (state == State.Search)
				{
					_stack.Pop();
				}
				else if (state == State.Recipe)
				{
					this.CloseRecipePage();
				}
				else if (state == State.Ingredients)
				{
					this.CloseIngredientsPage();
				}

				while (tryToQuit && _stack.Count > 0)
					_stack.Pop();

				if (!this.readyToClose() || _stack.Count > 0)
					return false;

				if (playSound)
					Game1.playSound(MenuCloseCue);

				Log.D("Closing cooking menu.",
					Config.DebugMode);

				this.exitThisMenuNoSound();
			}
			catch (Exception e)
			{
				Log.E($"Hit error on pop stack, emergency shutdown.\n{e}");
				this.emergencyShutDown();
				exitFunction();
			}
			return true;
		}

		public override void emergencyShutDown()
		{
			exitFunction();
			base.emergencyShutDown();
		}

		protected override void cleanupBeforeExit()
		{
			Game1.displayHUD = true;
			base.cleanupBeforeExit();
		}

		public override void setUpForGamePadMode()
		{
			base.setUpForGamePadMode();
			this.snapToDefaultClickableComponent();
		}

		public override void setCurrentlySnappedComponentTo(int id)
		{
			if (id == -1)
				return;

			currentlySnappedComponent = this.getComponentWithID(id);
			this.snapCursorToCurrentSnappedComponent();
		}

		public override void snapToDefaultClickableComponent()
		{
			if (!_stack.Any())
				return;
			State state = _stack.Peek();

			switch (state)
			{
				case State.Recipe:
					this.setCurrentlySnappedComponentTo(_recipeIconButton.myID);
					break;

				default:
					this.setCurrentlySnappedComponentTo(_searchBarClickable.myID);
					break;
			}
		}

		public override void automaticSnapBehavior(int direction, int oldRegion, int oldID)
		{
			this.customSnapBehavior(direction, oldRegion, oldID);
			//base.automaticSnapBehavior(direction, oldRegion, oldID);
		}

		protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
		{
			if (!_stack.Any())
				return;
			State state = _stack.Peek();

			if (oldRegion == 9000 && this.currentlySnappedComponent != null)
			{
				switch (direction)
				{
					// Up
					case 0:
						if (state == State.Search)
						{
							if (_recipeSearchResults.Any())
							{
								this.setCurrentlySnappedComponentTo(this.UsingRecipeGridView
									? _searchGridClickables.Last().myID
									: _searchListClickables.Last().myID);
							}
							if (currentlySnappedComponent.myID < inventory.inventory.Count)
							{
								this.setCurrentlySnappedComponentTo(_searchBarClickable.myID);
							}
						}
						break;
					// Right
					case 1:
						break;
					// Down
					case 2:
						break;
					// Left
					case 3:
						break;
				}
			}
		}

		public override bool IsAutomaticSnapValid(int direction, ClickableComponent a, ClickableComponent b)
		{
			return base.IsAutomaticSnapValid(direction, a, b);
		}

		public override void performHoverAction(int x, int y)
		{
			if (!_stack.Any())
				return;

			hoverText = null;
			hoveredItem = null;
			Item obj = inventory.getItemAt(x, y);
			inventory.hover(x, y, heldItem);
			if (CookingManager.CanBeCooked(item: obj))
			{
				hoveredItem = obj;
			}

			for (int i = 0; i < _ingredientsClickables.Count && hoveredItem == null; ++i)
			{
				if (_ingredientsClickables[i].containsPoint(x, y))
				{
					hoveredItem = _cookingManager.GetItemForIngredient(index: i, sourceItems: _allInventories);
				}
			}

			// Menu close button
			upperRightCloseButton.tryHover(x, y, 0.5f);

			// Contextual hovers
			State state = _stack.Peek();
			switch (state)
			{
				case State.Opening:
					break;

				case State.Recipe:
					// Left/right next/prev recipe navigation buttons
					_navRightButton.tryHover(x, y);
					_navLeftButton.tryHover(x, y);
					// Favourite recipe button
					_recipeIconButton.tryHover(x, y, 0.5f);
					if (!_recipeIconButton.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY())
					    && _recipeIconButton.containsPoint(x, y))
						Game1.playSound(HoverInCue);
					else if (_recipeIconButton.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY())
						&& !_recipeIconButton.containsPoint(x, y))
						Game1.playSound(HoverOutCue);
					break;

				case State.Search:
					// Up/down recipe search results navigation buttons
					_navDownButton.tryHover(x, y);
					_navUpButton.tryHover(x, y);

					// Search button
					if (_searchBarTextBox.Selected)
					{
						_searchButton.tryHover(x, y);
						if (_searchButton.containsPoint(x,y ))
							hoverText = _searchButton.hoverText;
					}
					else
					{
						// Search buttons
						foreach (ClickableTextureComponent clickable in _toggleButtonClickables)
						{
							clickable.tryHover(x, y, 0.25f);
							if (clickable.containsPoint(x, y))
								hoverText = clickable.hoverText;
						}

						// Search filter buttons
						if (_showSearchFilters)
						{
							foreach (ClickableTextureComponent clickable in _filterButtons)
							{
								clickable.tryHover(x, y, 0.4f);
								if (clickable.containsPoint(x, y))
									hoverText = clickable.hoverText;
							}
						}
					}

					if (this.UsingRecipeGridView)
					{
						// Hover text over recipe search results when in grid view, which unlike list view, has names hidden
						int index = this.TryGetIndexForSearchResult(x, y);
						if (index >= 0 && index < _recipeSearchResults.Count && _recipeSearchResults[index] != null && _recipeSearchResults[index].name != "Torch")
							hoverText = Game1.player.knowsRecipe(_recipeSearchResults[index].name)
								? _recipeSearchResults[index].DisplayName
								: i18n.Get("menu.cooking_recipe.title_unknown");
					}

					break;

				case State.Ingredients:

					break;
			}

			_searchTabButton.tryHover(x, y, state != State.Search ? 0.5f : 0f); // Button scale gets stuck if we don't call tryHover in State.Search
			if (IsIngredientsPageEnabled)
				_ingredientsTabButton.tryHover(x, y, state != State.Ingredients ? 0.5f : 0f);

			if (_showCookingConfirmPopup)
			{
				_cookQuantityUpButton.tryHover(x, y, 0.5f);
				_quantityTextBox.Hover(x, y);
				_cookQuantityDownButton.tryHover(x, y, 0.5f);

				_cookConfirmButton.tryHover(x, y);
				_cookCancelButton.tryHover(x, y);
			}

			// Inventory select buttons
			if (this.ShouldShowInventoryElements)
			{
				_inventoryTabButton.tryHover(x, y, 0.5f);
				_inventoryUpButton.tryHover(x, y, 0.5f);
				_inventoryDownButton.tryHover(x, y, 0.5f);
				foreach (ClickableTextureComponent clickable in _inventorySelectButtons)
				{
					clickable.tryHover(x, y, 0.5f);
				}
			}
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			if (!_stack.Any() || Game1.activeClickableMenu == null)
				return;
			State state = _stack.Peek();
			if (state == State.Opening)
				return;

			if (upperRightCloseButton.containsPoint(x, y))
			{
				this.PopMenuStack(playSound: false, tryToQuit: true);
				return;
			}

			switch (state)
			{
				case State.Opening:
					break;

				case State.Search:
					// Search result clicks
					int index = this.TryGetIndexForSearchResult(x, y);
					bool clickedSearchResult = index >= 0
						&& index < _recipeSearchResults.Count
						&& _recipeSearchResults[index] != null
						&& _recipeSearchResults[index].name != "Torch";
					if (clickedSearchResult)
					{
						Game1.playSound(PageChangeCue);
						this.ChangeCurrentRecipe(_recipeSearchResults[index].name);
						this.OpenRecipePage();
					}

					// Search text box
					if (_searchBarTextBoxBounds.Contains(x, y))
					{
						_searchBarTextBox.Text = "";
						Game1.keyboardDispatcher.Subscriber = _searchBarTextBox;
						_searchBarTextBox.SelectMe();
						this.ToggleFilterPopup(playSound: false, forceToggleTo: false);
					}
					else if (_searchBarTextBox.Selected)
					{
						if (_searchButton.containsPoint(x, y))
						{
							Game1.playSound(ClickCue);
							_searchBarTextBox.Text = _searchBarTextBox.Text.Trim();
						}
						if (string.IsNullOrEmpty(_searchBarTextBox.Text))
							_searchBarTextBox.Text = i18n.Get("menu.cooking_recipe.search_label");
						this.CloseTextBox(_searchBarTextBox, reapplyFilters: !clickedSearchResult);
					}
					else
					{
						// Search filter buttons
						if (_showSearchFilters)
						{
							ClickableTextureComponent clickable = _filterButtons.FirstOrDefault(c => c.containsPoint(x, y));
							if (clickable != null)
							{
								Filter which = (Filter)int.Parse(clickable.name[clickable.name.Length - 1].ToString());
								if (which == _lastFilterUsed)
								{
									_recipesFiltered = this.ReverseRecipeList(_recipesFiltered);
								}
								else
								{
									_recipesFiltered = this.FilterRecipes(which, _searchBarTextBox.Text);
								}
								Game1.playSound(ClickCue);
								if (ModEntry.Config.RememberLastSearchFilter)
								{
									ModEntry.Instance.States.Value.LastFilterThisSession = which;
								}
							}
						}
						
						// Search filter toggles
						if (_toggleFilterButton.containsPoint(x, y))
						{
							this.ToggleFilterPopup(playSound);
						}
						// Search results order reverse button
						else if (_toggleOrderButton.containsPoint(x, y))
						{
							_recipesFiltered = this.ReverseRecipeList(_recipesFiltered);
							Game1.playSound(PageChangeCue);
						}
						// Search results grid/list view button
						else if (_toggleViewButton.containsPoint(x, y))
						{
							bool isGridView = this.UsingRecipeGridView;
							_toggleViewButton.sourceRect.X = ToggleViewButtonSource.X + (isGridView ? 0 : ToggleViewButtonSource.Width);

							this.KeepRecipeIndexInSearchBounds();

							this.UsingRecipeGridView = !isGridView;
							Game1.playSound(PageChangeCue);
							_toggleViewButton.hoverText = i18n.Get($"menu.cooking_search.view.{(isGridView ? "grid" : "list")}");
						}
						// Autofill button
						else if (_toggleAutofillButton.containsPoint(x, y))
						{
							Game1.playSound(ClickCue);
							IsUsingAutofill = !IsUsingAutofill;
							_toggleAutofillButton.sourceRect.X = IsUsingAutofill // Update toggled button appearance
								? AutofillButtonSource.X + AutofillButtonSource.Width
								: AutofillButtonSource.X;
							//_cookingManager.ClearCurrentIngredients(); // Remove current ingredients from slots
							//this.ChangeCurrentRecipe(_recipeIndex); // Refresh check for ready-to-cook recipe
						}
					}

					break;

				case State.Recipe:

					if (_recipeIconButton.containsPoint(x, y))
					{
						// Favourite recipe button
						if (ModEntry.Instance.States.Value.FavouriteRecipes.Contains(_recipeAsItem.Name))
						{
							ModEntry.Instance.States.Value.FavouriteRecipes.Remove(_recipeAsItem.Name);
							Game1.playSound("throwDownITem"); // not a typo
						}
						else
						{
							ModEntry.Instance.States.Value.FavouriteRecipes.Add(_recipeAsItem.Name);
							Game1.playSound("pickUpItem");
						}
					}
					else if (ReadyToCook && _cookButton.bounds.Contains(x, y))
					{
						// Cook! button
						this.ToggleCookingConfirmPopup(playSound: true);
					}
					else if (_showCookingConfirmPopup)
					{
						// Quantity up/down buttons
						this.TryClickQuantityButton(x, y);

						// Quantity text box
						if (_quantityTextBoxBounds.Contains(x, y))
						{
							_quantityTextBox.Text = "";
							Game1.keyboardDispatcher.Subscriber = _quantityTextBox;
							_quantityTextBox.SelectMe();
						}
						else if (_quantityTextBox.Selected)
						{
							this.ValidateNumericalTextBox(_quantityTextBox);
							this.CloseTextBox(_quantityTextBox, reapplyFilters: true);
						}

						// Cook OK/Cancel buttons
						if (_cookConfirmButton.containsPoint(x, y))
						{
							this.ValidateNumericalTextBox(_quantityTextBox);
							int quantity = int.Parse(_quantityTextBox.Text.Trim());
							if (this.TryCookRecipe(recipe: CurrentRecipe, quantity))
							{
								this.PopMenuStack(playSound: true);
							}
							else
							{
								Game1.playSound(CancelCue);
							}
						}
						else if (_cookCancelButton.containsPoint(x, y))
						{
							this.PopMenuStack(playSound: true);
						}
					}

					break;
			}

			// Search tab
			if (state != State.Search && _searchTabButton.containsPoint(x, y))
			{
				if (state == State.Recipe)
				{
					this.CloseRecipePage();
				}
				else
				{
					_stack.Pop();
				}
				this.OpenSearchPage();
				Game1.playSound(MenuChangeCue);
			}
			// Ingredients tab
			else if (IsIngredientsPageEnabled
				&& Config.AddRecipeRebalancing
				&& state != State.Ingredients
				&& _ingredientsTabButton.containsPoint(x, y))
			{
				_stack.Pop();
				this.OpenIngredientsPage();
				Game1.playSound(MenuChangeCue);
			}

			// Inventory nav buttons
			if (this.ShouldShowInventoryElements)
			{
				if (_inventoryTabButton.containsPoint(x, y))
				{
					this.ToggleInventoriesPopup(playSound: true);
				}
				else if (_inventoryUpButton.containsPoint(x, y))
				{
					this.ChangeInventory(selectNext: false);
				}
				else if (_inventoryDownButton.containsPoint(x, y))
				{
					this.ChangeInventory(selectNext: true);
				}
				else if (_showInventoriesPopup)
				{
					foreach (ClickableTextureComponent clickable in _inventorySelectButtons)
					{
						if (clickable.bounds.Contains(x, y))
						{
							int index = clickable.name == "inventorySelectBackpack"
								// Player backpack
								? BackpackInventoryId
								// Fridges, minifridges and chests
								: int.Parse(clickable.name.Substring(clickable.name.IndexOf(clickable.name.First(c => char.IsDigit(c)))))
									+ this._inventoryIdsBeforeMinifridges;
							this.ChangeInventory(index);
							Game1.playSound(ClickCue);
							break;
						}
					}
				}
			}

			// Up/down/left/right contextual navigation buttons
			this.TryClickNavButton(x, y, playSound: true);
			// Inventory and ingredients dropIn items
			this.TryClickItem(x, y);

			this.UpdateSearchRecipes();

			_mouseHeldTicks = 0;
		}

		public override void receiveRightClick(int x, int y, bool playSound = true)
		{
			if (!_stack.Any())
				return;
			State state = _stack.Peek();
			if (state == State.Opening)
				return;

			base.receiveRightClick(x, y, playSound);

			bool shouldPop = this.TryClickItem(x, y)
				&& !inventory.isWithinBounds(x, y); // Don't pop the menu stack when clicking any inventory slots
			
			if (_showCookingConfirmPopup && shouldPop)
			{
				this.ToggleCookingConfirmPopup(playSound: true);
				_quantityTextBox.Update();
			}
			else
			{
				if (_searchBarTextBox.Selected)
				{
					_searchBarTextBox.Text = i18n.Get("menu.cooking_recipe.search_label");
					this.CloseTextBox(_searchBarTextBox, reapplyFilters: true);
				}
				else if (state == State.Search)
				{
					if (!string.IsNullOrEmpty(_searchBarTextBox.Text)
						 && _searchBarTextBox.Text != i18n.Get("menu.cooking_recipe.search_label"))
					{
						_searchBarTextBox.Text = i18n.Get("menu.cooking_recipe.search_label");
					}
					_recipesFiltered = _recipesAvailable;
				}
			}

			if (shouldPop && state != State.Search)
			{
				this.PopMenuStack(playSound);
				this.KeepRecipeIndexInSearchBounds();
			}

			this.UpdateSearchRecipes();
		}

		public override void leftClickHeld(int x, int y)
		{
			base.leftClickHeld(x, y);

			// Start mouse-held behaviours after a delay
			if (_mouseHeldTicks < 0 || ++_mouseHeldTicks < 30)
				return;
			_mouseHeldTicks = 20;

			// Use mouse-held behaviours on navigation and quantity buttons
			this.TryClickNavButton(x, y, playSound: true);
			if (_showCookingConfirmPopup)
			{
				this.TryClickQuantityButton(x, y);
			}
		}

		public override void releaseLeftClick(int x, int y)
		{
			base.releaseLeftClick(x, y);

			_mouseHeldTicks = -1;
		}

		public override void receiveGamePadButton(Buttons b)
		{
			if (!_stack.Any())
				return;
			State state = _stack.Peek();
			if (state == State.Opening)
				return;

			bool isExitButton = b == Buttons.Start || b == Buttons.B || b == Buttons.Y;
			int cur = currentlySnappedComponent != null ? currentlySnappedComponent.myID : -1;

			if (Config.DebugMode)
				Log.D(currentlySnappedComponent != null
				? $"GP CSC: {currentlySnappedComponent.myID} ({currentlySnappedComponent.name})"
					+ $" [{currentlySnappedComponent.leftNeighborID} {currentlySnappedComponent.upNeighborID}"
					+ $" {currentlySnappedComponent.rightNeighborID} {currentlySnappedComponent.downNeighborID}]"
				: "GP CSC: null");

			// Contextual navigation
			int firstID = state == State.Search
				? _recipeSearchResults.Count > 0
					? this.UsingRecipeGridView
						? _searchGridClickables.First().myID
						: _searchListClickables.First().myID
					: _searchBarClickable.myID
				: _recipeIconButton.myID;
			List<int> set = new List<int> { firstID, 0, _ingredientsClickables.First().myID };
			int index = set.IndexOf(cur);
			if (b == Buttons.LeftShoulder)
			{
				this.setCurrentlySnappedComponentTo(index == -1
					? set.First()
					: index == set.Count - 1
						? set.First()
						: set[index + 1]);
				this.ToggleInventoriesPopup(playSound: false, forceToggleTo: false);
				this.ToggleFilterPopup(playSound: false, forceToggleTo: false);
			}
			else if (b == Buttons.RightShoulder)
			{
				this.setCurrentlySnappedComponentTo(index == -1
					? set.Last()
					: index == 0
						? set.Last()
						: set[index - 1]);
				this.ToggleInventoriesPopup(playSound: false, forceToggleTo: false);
				this.ToggleFilterPopup(playSound: false, forceToggleTo: false);
			}
			else if (b == Buttons.LeftTrigger)
			{
				if (_inventoriesScrollableArea.Contains(Game1.getOldMouseX(), Game1.getOldMouseY()))
					this.ChangeCurrentRecipe(selectNext: false);
				else
					this.ChangeInventory(selectNext: false);
			}
			else if (b == Buttons.RightTrigger)
			{
				if (_inventoriesScrollableArea.Contains(Game1.getOldMouseX(), Game1.getOldMouseY()))
					this.ChangeCurrentRecipe(selectNext: true);
				else
					this.ChangeInventory(selectNext: true);
			}

			// Right thumbstick emulates navigation buttons
			if (b == Buttons.RightThumbstickUp)
				this.TryClickNavButton(_navUpButton.bounds.X, _navUpButton.bounds.Y, playSound: true);
			if (b == Buttons.RightThumbstickDown)
				this.TryClickNavButton(_navDownButton.bounds.X, _navDownButton.bounds.Y, playSound: true);
			if (b == Buttons.RightThumbstickLeft)
				this.TryClickNavButton(_navLeftButton.bounds.X, _navLeftButton.bounds.Y, playSound: true);
			if (b == Buttons.RightThumbstickRight)
				this.TryClickNavButton(_navRightButton.bounds.X, _navRightButton.bounds.Y, playSound: true);

			if (_searchBarTextBox.Selected)
			{
				// Open onscreen keyboard for search bar textbox
				if (b == Buttons.A)
					Game1.showTextEntry(_searchBarTextBox);
				if (isExitButton)
					this.CloseTextBox(_searchBarTextBox, reapplyFilters: true);
			}
			else if (b == Buttons.RightTrigger)
				return;
			else if (b == Buttons.LeftTrigger)
				return;

			//base.receiveGamePadButton(b);
		}

		public override void gamePadButtonHeld(Buttons b)
		{
			base.gamePadButtonHeld(b);
		}

		public override void receiveScrollWheelAction(int direction)
		{
			base.receiveScrollWheelAction(direction);

			if (!_stack.Any())
				return;
			State state = _stack.Peek();
			Point cursor = new Point(Game1.getOldMouseX(), Game1.getOldMouseY());
			if (_showCookingConfirmPopup && _quantityScrollableArea.Contains(cursor))
			{
				this.TryClickQuantityButton(_cookQuantityUpButton.bounds.X,
					direction < 0 ? _cookQuantityDownButton.bounds.Y : _cookQuantityUpButton.bounds.Y);
			}
			else if (_inventoriesScrollableArea.Contains(cursor.X, cursor.Y)
				|| (_showInventoriesPopup && _inventoriesPopupArea.Contains(cursor.X, cursor.Y)))
			{
				// Scroll wheel navigates between backpack, fridge, and minifridge inventories
				this.ChangeInventory(selectNext: direction < 0);
			}
			else
			{
				ClickableTextureComponent clickable = direction < 0
					? state == State.Search ? _navDownButton : _navRightButton
					: state == State.Search ? _navUpButton : _navLeftButton;
				this.TryClickNavButton(clickable.bounds.X, clickable.bounds.Y, state == State.Recipe);
			}

			this.UpdateSearchRecipes();
		}

		public override void receiveKeyPress(Keys key)
		{
			if (!_stack.Any())
				return;
			State state = _stack.Peek();
			if (state == State.Opening)
				return;

			// Contextual navigation
			if (Game1.options.SnappyMenus && currentlySnappedComponent != null)
			{
				int cur = currentlySnappedComponent.myID;
				int next = -1;
				if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
				{
					if (cur < inventory.inventory.Count && cur % this.GetColumnCount() == 0)
						next = _inventorySelectButtons.Any()
							? _inventoryTabButton.myID
							: cur;
					else if (cur == _recipeIconButton.myID)
						next = this.IsNavButtonActive(_navLeftButton.myID) ? _navLeftButton.myID : _searchTabButton.myID;
					else if (cur == _ingredientsClickables.First().myID && state == State.Recipe)
						next = this.IsNavButtonActive(_navRightButton.myID) ? _navRightButton.myID : _recipeIconButton.myID;
					else if (_showInventoriesPopup && !UseHorizontalInventoryButtonArea
						&& (cur == _inventoryTabButton.myID || cur == _inventoryUpButton.myID || cur == _inventoryDownButton.myID))
						next = _inventorySelectButtons.First().myID;
					else if (cur == _navUpButton.myID || cur == _navDownButton.myID)
						next = this.UsingRecipeGridView
							? _searchGridClickables.First().myID
							: _searchListClickables.First().myID;
				}
				if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
				{
					if (cur == _searchTabButton.myID)
						next = this.IsNavButtonActive(_navLeftButton.myID) ? _navLeftButton.myID : _recipeIconButton.myID;
					else if (cur == _recipeIconButton.myID)
						next = this.IsNavButtonActive(_navRightButton.myID) ? _navRightButton.myID : _ingredientsClickables.First().myID;
					else if (cur == _inventoryTabButton.myID)
						next = this.GetColumnCount();
					else if (cur == _inventoryUpButton.myID)
						next = 0;
					else if (cur == _inventoryUpButton.myID)
						next = this.GetColumnCount() * 2;
					else if (((this.UsingRecipeGridView
							&& _searchGridClickables.Any(c => c.myID == cur && int.Parse(string.Join("", c.name.Where(char.IsDigit))) % 4 == 3))
							|| _searchListClickables.Any(c => c.myID == cur)))
						next = this.IsNavButtonActive(_navUpButton.myID)
							? _navUpButton.myID
							: this.IsNavButtonActive(_navDownButton.myID)
								? _navDownButton.myID
								: cur;
				}
				if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
				{
					if (cur < inventory.inventory.Count && cur >= this.GetColumnCount())
						// Inventory row navigation
						next = cur - this.GetColumnCount();
					else if (cur < inventory.inventory.Count && state == State.Recipe)
						// Move out of inventory into crafting page
						next = ReadyToCook
							? _cookButton.myID
							: _showCookingConfirmPopup
								? _cookConfirmButton.myID
								: _ingredientsClickables.First().myID;
					else if (cur == _inventoryTabButton.myID)
						next = _inventoryUpButton.myID;
					else if (cur == _inventoryUpButton.myID)
						next = state == State.Recipe
							? _searchTabButton.myID
							: _searchBarClickable.myID;
					else if (cur == _inventoryDownButton.myID)
						next = _inventoryTabButton.myID;
					else if (cur == _navDownButton.myID)
						// Moving from search results scroll down button
						next = this.IsNavButtonActive(_navUpButton.myID)
							? _navUpButton.myID
							: _navUpButton.upNeighborID;
				}
				if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
				{
					IEnumerable<ClickableComponent> set = _toggleButtonClickables.Concat(new[] { _searchBarClickable });
					if (set.Any(clickable => clickable.myID == cur))
						// Moving into search results from search bar
						// Doesn't include ToggleFilterButton since it inexplicably already navigates to first search result
						next = this.UsingRecipeGridView
							? _searchGridClickables.First().myID
							: _searchListClickables.First().myID;
					else if (cur < inventory.inventory.Count - this.GetColumnCount())
						// Inventory row navigation
						next = cur + this.GetColumnCount();
					else if (cur < inventory.inventory.Count && cur >= inventory.inventory.Count - this.GetColumnCount())
						// Do not scroll further down or wrap around when at bottom of inventory in solo play
						// In split-screen play, select the fridge buttons if available
						next = UseHorizontalInventoryButtonArea && _showInventoriesPopup && _inventorySelectButtons.Any()
							? _inventorySelectButtons.First().myID
							: cur;
					else if (cur < inventory.inventory.Count)
						// Moving into search results from inventory
						next = this.UsingRecipeGridView
							? _searchGridClickables.Last().myID
							: _searchListClickables.Last().myID;
					else if (cur == _ingredientsClickables.Last().myID && state == State.Recipe)
						// Moving from last ingredient slot
						next = ReadyToCook
							? _cookButton.myID
							: _showCookingConfirmPopup
								? _cookConfirmButton.myID
								: 0; // First element in inventory
					else if (cur == _inventoryTabButton.myID)
						next = _inventoryDownButton.myID;
					else if (cur == _inventoryUpButton.myID)
						next = _inventoryTabButton.myID;
					// no behaviour for _inventoryDownButton
					else if (cur == _navUpButton.myID)
						// Moving from search results scroll up arrow
						next = this.IsNavButtonActive(_navDownButton.myID)
							? _navDownButton.myID
							: _navDownButton.downNeighborID;
				}

				if (this.ShouldShowInventoryElements)
				{
					// Inventory select popup button navigation

					InputButton[] inventoryNavUp = UseHorizontalInventoryButtonArea ? Game1.options.moveLeftButton : Game1.options.moveUpButton;
					InputButton[] inventoryNavDown = UseHorizontalInventoryButtonArea ? Game1.options.moveRightButton : Game1.options.moveDownButton;
					InputButton[] inventoryNavLeft = UseHorizontalInventoryButtonArea ? Game1.options.moveUpButton : Game1.options.moveLeftButton;
					InputButton[] inventoryNavRight = UseHorizontalInventoryButtonArea ? Game1.options.moveDownButton : Game1.options.moveRightButton;

					if (Game1.options.doesInputListContain(inventoryNavUp, key))
					{
						if (_inventorySelectButtons.First().myID is int first
							&& _inventorySelectButtons.Last().myID is int last
							&& cur >= first && cur <= last
							&& cur - first is int i
							&& (_inventorySelectButtons.Count - 1) is int count)
							// Moving between inventory select buttons in the inventories card
							next = i < InventorySelectButtonsWide
								// move from first to last row (vertical layout) or column (horizontal layout)
								? Math.Min(last, first + (InventorySelectButtonsWide * (count) / InventorySelectButtonsWide))
								// move from others
								: cur - InventorySelectButtonsWide;
					}
					else if (Game1.options.doesInputListContain(inventoryNavDown, key))
					{
						if (_inventorySelectButtons.First().myID is int first
							&& _inventorySelectButtons.Last().myID is int last
							&& cur >= first && cur <= last
							&& cur - first is int i
							&& (_inventorySelectButtons.Count - 1) is int count)
							// Moving between inventory select buttons in the inventories card
							next = i >= InventorySelectButtonsWide * (count - 1) / InventorySelectButtonsWide
								// move from last to first row (vertical layout) or column (horizontal layout)
								? first + (i % InventorySelectButtonsWide)
								// move from others
								: Math.Min(last, cur + InventorySelectButtonsWide);
					}
					else if (Game1.options.doesInputListContain(inventoryNavLeft, key))
					{
						if (cur == _inventorySelectButtons.First().myID)
							// move from first to last element
							next = _inventorySelectButtons.Last().myID;
					}
					else if (Game1.options.doesInputListContain(inventoryNavRight, key))
					{
						if (cur == _inventorySelectButtons.Last().myID)
							// move from last to first element
							next = _inventorySelectButtons.First().myID;
					}
				}

				if (next != -1)
				{
					if (Config.DebugMode)
						Log.D($"KP CSC: {cur} => {next} ({this.getComponentWithID(next)?.name ?? "null"})");
					this.setCurrentlySnappedComponentTo(next);
					return;
				}
			}

			base.receiveKeyPress(key);

			switch (state)
			{
				case State.Search:
				{
					// Navigate left/right/up/down buttons traverse search results
					if (!Game1.options.SnappyMenus)
					{
						if ((Game1.options.doesInputListContain(Game1.options.moveLeftButton, key) || Game1.options.doesInputListContain(Game1.options.moveUpButton, key)) && IsNavButtonActive(_navUpButton.myID))
							this.TryClickNavButton(_navUpButton.bounds.X, _navUpButton.bounds.Y, false);
						if ((Game1.options.doesInputListContain(Game1.options.moveRightButton, key) || Game1.options.doesInputListContain(Game1.options.moveDownButton, key)) && IsNavButtonActive(_navDownButton.myID))
							this.TryClickNavButton(_navDownButton.bounds.X, _navDownButton.bounds.Y, false);
					}
					break;
				}
				case State.Recipe:
				{
					if (!Game1.options.SnappyMenus)
					{
						// Navigate left/right buttons select recipe
						if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key) && this.IsNavButtonActive(_navLeftButton.myID))
							this.ChangeCurrentRecipe(--_recipeIndex);
						if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key) && this.IsNavButtonActive(_navRightButton.myID))
							this.ChangeCurrentRecipe(++_recipeIndex);
						// Navigate up/down buttons control inventory
						if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key) && this.IsNavButtonActive(_navUpButton.myID))
							this.ChangeInventory(selectNext: false);
						if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key) && this.IsNavButtonActive(_navDownButton.myID))
							this.ChangeInventory(selectNext: true);
					}

					break;
				}
			}

			if (_searchBarTextBox.Selected)
			{
				switch (key)
				{
					case Keys.Enter:
						break;
					case Keys.Escape:
						this.CloseTextBox(_searchBarTextBox, reapplyFilters: true);
						break;
					default:
						_recipesFiltered = this.FilterRecipes(_lastFilterUsed, substr: _searchBarTextBox.Text);
						break;
				}
			}
			else
			{
				if (Config.DebugMode)
				{
					if (key == Keys.L)
					{
						List<string> locales = CookTextSource.Keys.ToList();
						_locale = locales[(locales.IndexOf(_locale) + 1) % locales.Count];
						Log.D($"Changing to locale {_locale} and realigning elements");
						this.RealignElements();
					}
				}

				if (Game1.options.doesInputListContain(Game1.options.menuButton, key)
					|| Game1.options.doesInputListContain(Game1.options.journalButton, key))
				{
					if (Game1.options.SnappyMenus
						&& _showInventoriesPopup
						&& _inventoriesPopupArea.Contains(Game1.getOldMouseX(), Game1.getOldMouseY()))
						this.ToggleInventoriesPopup(playSound: true, forceToggleTo: false);
					else
						this.PopMenuStack(playSound: true);
				}

				if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && canExitOnKey)
				{
					this.PopMenuStack(playSound: true);
					if (Game1.currentLocation.currentEvent != null && Game1.currentLocation.currentEvent.CurrentCommand > 0)
					{
						Game1.currentLocation.currentEvent.CurrentCommand++;
					}
				}
			}

			this.UpdateSearchRecipes();
		}

		public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
		{
			base.gameWindowSizeChanged(oldBounds, newBounds);
			this.RealignElements();
		}

		public override void update(GameTime time)
		{
			_animTimer += time.ElapsedGameTime.Milliseconds;
			if (_animTimer >= AnimTimerLimit)
				_animTimer = 0;
			_animFrame = (int)((float)_animTimer / AnimTimerLimit * AnimFrames);

			// Expand search bar on selected, contract on deselected
			float delta = 256f / time.ElapsedGameTime.Milliseconds;
			if (_searchBarTextBox.Selected && _searchBarTextBox.Width < _searchBarTextBoxMaxWidth)
				_searchBarTextBox.Width = (int)Math.Min(_searchBarTextBoxMaxWidth, _searchBarTextBox.Width + delta);
			else if (!_searchBarTextBox.Selected && _searchBarTextBox.Width > _searchBarTextBoxMinWidth)
				_searchBarTextBox.Width = (int)Math.Max(_searchBarTextBoxMinWidth, _searchBarTextBox.Width - delta);
			_searchBarTextBoxBounds.Width = _searchBarTextBox.Width;
			_searchBarClickable.bounds.Width = _searchBarTextBoxBounds.Width;

			base.update(time);
		}

		public override void draw(SpriteBatch b)
		{
			if (!_stack.Any())
				return;
			State state = _stack.Peek();

			// Blackout
			b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea(), Color.Black * 0.5f);

			// Cookbook
			b.Draw(
				Texture,
				new Vector2(_cookbookLeftRect.X, _cookbookLeftRect.Y),
				CookbookSource,
				Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 1f);

			if (state == State.Recipe)
				this.DrawRecipePage(b);
			else if (state == State.Search)
				this.DrawSearchPage(b);
			else if (state == State.Ingredients)
				this.DrawIngredientsPage(b);
			this.DrawCraftingPage(b);
			this.DrawInventoryMenu(b);
			this.DrawActualInventory(b);
			this.DrawExtraStuff(b);

			if (false && Config.DebugMode)
			{
				if (currentlySnappedComponent != null)
					b.Draw(Game1.fadeToBlackRect, currentlySnappedComponent.bounds, Color.Green * 0.5f);
				b.Draw(Game1.fadeToBlackRect, _searchResultsArea, Color.Red * 0.5f);
				foreach (var c in _searchGridClickables)
					b.Draw(Game1.fadeToBlackRect, c.bounds, Color.Cyan * 0.5f);
			}
		}

		private void DrawIngredientsPage(SpriteBatch b)
		{
			
		}

		private void DrawSearchPage(SpriteBatch b)
		{
			// Search nav buttons
			if (this.IsNavButtonActive(_navUpButton.myID))
				_navUpButton.draw(b);
			if (this.IsNavButtonActive(_navDownButton.myID))
				_navDownButton.draw(b);

			// Recipe entries
			CraftingRecipe recipe;
			string text;

			if (!_recipeSearchResults.Any() || _recipeSearchResults.Any(recipe => recipe?.name == "Torch"))
			{
				text = i18n.Get("menu.cooking_search.none_label");
				this.DrawText(b, text, scale: 1f,
					_leftContent.X - _searchResultsArea.X + TextSpacingFromIcons - (4 * Scale),
					_searchResultsArea.Y + (16 * Scale),
					_searchResultsArea.Width - TextSpacingFromIcons, isLeftSide: true);
			}
			else
			{
				if (this.UsingRecipeGridView)
				{
					for (int i = 0; i < _recipeSearchResults.Count; ++i)
					{
						recipe = _recipeSearchResults[i];
						if (recipe == null || !_searchGridClickables[i].visible)
							continue;

						recipe.drawMenuView(b, _searchGridClickables[i].bounds.X, _searchGridClickables[i].bounds.Y);
					}
				}
				else
				{
					int localWidth = _searchResultsArea.Width - TextSpacingFromIcons;
					for (int i = 0; i < _recipeSearchResults.Count; ++i)
					{
						recipe = _recipeSearchResults[i];
						if (recipe == null || !_searchListClickables[i].visible)
							continue;

						recipe.drawMenuView(b, _searchListClickables[i].bounds.X, _searchListClickables[i].bounds.Y);

						text = Game1.player.knowsRecipe(recipe?.name)
							? recipe.DisplayName
							: i18n.Get("menu.cooking_recipe.title_unknown");

						this.DrawText(b, text, scale: 1f,
							_searchListClickables[i].bounds.X - _leftContent.X + TextSpacingFromIcons,
							_searchListClickables[i].bounds.Y - (int)(Game1.smallFont.MeasureString(Game1.parseText(
								text, Game1.smallFont, _searchResultsArea.Width - TextSpacingFromIcons)).Y / 2 - RecipeListHeight / 2),
							localWidth, isLeftSide: true);
					}
				}
			}

			// Search bar
			_searchBarTextBox.Draw(b);
			if (_searchBarTextBox.Selected)
			{
				_searchButton.draw(b);
				return;
			}

			// Search filter toggles
			foreach (ClickableTextureComponent clickable in _toggleButtonClickables)
				if (_searchBarTextBox.X + _searchBarTextBox.Width < clickable.bounds.X)
					clickable.draw(b);
			if (_lastFilterUsed != Filter.None && _searchBarTextBox.X + _searchBarTextBox.Width < _toggleFilterButton.bounds.X)
			{
				Vector2 origin = new Vector2(FilterIconSource.Width / 2, FilterIconSource.Height / 2);
				b.Draw(
					texture: Texture,
					destinationRectangle: new Rectangle(
						(int)(_toggleFilterButton.bounds.X + ((_toggleFilterButton.sourceRect.Width - FilterIconSource.Width) * _toggleFilterButton.baseScale / 2) + (origin.X * _toggleFilterButton.scale)),
						(int)(_toggleFilterButton.bounds.Y + ((_toggleFilterButton.sourceRect.Height - FilterIconSource.Height) * _toggleFilterButton.baseScale / 2) + (origin.Y * _toggleFilterButton.scale)),
						(int)(FilterIconSource.Width * _toggleFilterButton.scale),
						(int)(FilterIconSource.Height * _toggleFilterButton.scale)),
					sourceRectangle: _filterButtons[(int)_lastFilterUsed - 1].sourceRect,
					color: Color.White,
					rotation: 0f,
					origin: origin,
					effects: SpriteEffects.None,
					layerDepth: 1f);
			}
			
			if (_showSearchFilters)
			{
				float buttonScale = _filterButtons.First().baseScale;
				// Filter clickable icons container
				// left
				b.Draw(
					Texture,
					new Rectangle(
						_filterContainerBounds.X, _filterContainerBounds.Y,
						(int)(FilterContainerSideSourceWidth * buttonScale), _filterContainerBounds.Height),
					new Rectangle(
						FilterContainerSource.X, FilterContainerSource.Y,
						FilterContainerSideSourceWidth, FilterContainerSource.Height),
					Color.White);
				// middle
				b.Draw(
					Texture,
					new Rectangle(
						(int)(_filterContainerBounds.X + (FilterContainerSideSourceWidth * buttonScale)), _filterContainerBounds.Y,
						(int)(_filterContainerMiddleSourceWidth * buttonScale), _filterContainerBounds.Height),
					new Rectangle(
						FilterContainerSource.X + FilterContainerSideSourceWidth, FilterContainerSource.Y,
						1, FilterContainerSource.Height),
					Color.White);
				// right
				b.Draw(
					Texture,
					new Rectangle(
						(int)(_filterContainerBounds.X + (FilterContainerSideSourceWidth * buttonScale) + (_filterContainerMiddleSourceWidth * buttonScale)),
						_filterContainerBounds.Y,
						(int)(FilterContainerSideSourceWidth * buttonScale), _filterContainerBounds.Height),
					new Rectangle(
						FilterContainerSource.X + FilterContainerSideSourceWidth + 1, FilterContainerSource.Y,
						FilterContainerSideSourceWidth, FilterContainerSource.Height),
					Color.White);

				// Filter clickable icons
				foreach (ClickableTextureComponent clickable in _filterButtons)
					clickable.draw(b);
			}
		}

		private void DrawRecipePage(SpriteBatch b)
		{
			bool knowsRecipe = CurrentRecipe != null && Game1.player.knowsRecipe(CurrentRecipe.name);
			float xScale = _locale == "ko" && _resizeKoreanFonts ? KoWidthScale : 1f;
			float yScale = _locale == "ko" && _resizeKoreanFonts ? KoHeightScale : 1f;
			float textHeightCheck;
			int[] textHeightCheckMilestones = new int[]{ 60, 100, 120 };
			Vector2 textPosition = Vector2.Zero;
			int textWidth = (int)(_textWidth * xScale);
			string text;
			const bool isLeftSide = true;

			// Clickables
			if (this.IsNavButtonActive(_navLeftButton.myID))
				_navLeftButton.draw(b);
			if (this.IsNavButtonActive(_navRightButton.myID))
				_navRightButton.draw(b);

			// Recipe icon and title + favourite icon
			_recipeIconButton.sourceRect = Game1.getSourceRectForStandardTileSheet(
				Game1.objectSpriteSheet, _recipeAsItem.ParentSheetIndex, 16, 16);
			_recipeIconButton.draw(b);
			
			if (ModEntry.Instance.States.Value.FavouriteRecipes.Contains(_recipeAsItem.Name))
			{
				b.Draw(
					texture: Texture,
					destinationRectangle: new Rectangle(
						_recipeIconButton.bounds.X + _recipeIconButton.bounds.Width / 3 * 2,
						_recipeIconButton.bounds.Y + _recipeIconButton.bounds.Height / 3 * 2,
						FavouriteIconSource.Width * 3, FavouriteIconSource.Height * 3),
					sourceRectangle: FavouriteIconSource,
					color: Color.White);
			}
			float titleScale = 1f;
			textWidth = (int)(40.5f * Scale * xScale);
			text = knowsRecipe
				? CurrentRecipe.DisplayName
				: i18n.Get("menu.cooking_recipe.title_unknown");
			textPosition.X = _navLeftButton.bounds.Width + (14 * Scale);

			// Attempt to fix for Deutsch lange names
			if (_locale == "de" && Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).X > textWidth)
				text = text.Replace("-", "\n").Trim();

			if (Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).X * 0.8 > textWidth)
				titleScale = 0.735f;
			else if (Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).X > textWidth)
				titleScale = 0.95f;

			textPosition.Y = _navLeftButton.bounds.Y + (1 * Scale);
			textPosition.Y -= (Game1.smallFont.MeasureString(
				Game1.parseText(text, Game1.smallFont, textWidth)).Y / 2 - (6 * Scale)) * yScale;
			textHeightCheck = Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).Y * yScale * titleScale;
			if (textHeightCheck * titleScale > textHeightCheckMilestones[0])
				textPosition.Y += (textHeightCheck - textHeightCheckMilestones[0]) / 2;
			this.DrawText(b, text, 1.5f * titleScale, textPosition.X, textPosition.Y, textWidth, isLeftSide);

			// Recipe description
			textPosition.X = 0;
			textPosition.Y = _navLeftButton.bounds.Y + _navLeftButton.bounds.Height + (6 * Scale);
			if (textHeightCheck > textHeightCheckMilestones[0])
				textPosition.Y += textHeightCheck - 50 * xScale;
			textWidth = (int)(_textWidth * xScale);
			text = knowsRecipe
				? CurrentRecipe.description
				: i18n.Get("menu.cooking_recipe.title_unknown");
			this.DrawText(b, text, 1f, textPosition.X, textPosition.Y, textWidth, isLeftSide);
			textPosition.Y += TextDividerGap * 2;

			// Recipe ingredients
			if (textHeightCheck > textHeightCheckMilestones[0] && Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).Y < 80)
				textPosition.Y -= 6 * Scale;
			textHeightCheck = Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).Y * yScale;
			if (textHeightCheck > textHeightCheckMilestones[2]) 
				textPosition.Y += 6 * Scale;
			if (textHeightCheck > textHeightCheckMilestones[1] && CurrentRecipe?.getNumberOfIngredients() < 6)
				textPosition.Y += 6 * Scale;
			textPosition.Y += TextDividerGap + Game1.smallFont.MeasureString(
				Game1.parseText(yScale < 1 ? "Hoplite!\nHoplite!" : "Hoplite!\nHoplite!\nHoplite!", Game1.smallFont, textWidth)).Y * yScale;
			this.DrawHorizontalDivider(b, 0, textPosition.Y, _lineWidth, isLeftSide);
			textPosition.Y += TextDividerGap;
			text = i18n.Get("menu.cooking_recipe.ingredients_label");
			this.DrawText(b, text, 1f, textPosition.X, textPosition.Y, null, isLeftSide, false, SubtextColour);
			if (Game1.options.showAdvancedCraftingInformation)
			{
				this.DrawText(b, $"({_recipeCraftableCount})", 1f, _cookbookLeftRect.Width - MarginLeft - MarginRight, textPosition.Y,
					null, isLeftSide, true, SubtextColour);
			}
			textPosition.Y += Game1.smallFont.MeasureString(
			Game1.parseText(text, Game1.smallFont, textWidth)).Y * yScale;
			this.DrawHorizontalDivider(b, 0, textPosition.Y, _lineWidth, isLeftSide);
			textPosition.Y += TextDividerGap - ((16 * Scale) / 2) + (1 * Scale);

			if (knowsRecipe)
			{
				for (int i = 0; i < CurrentRecipe.getNumberOfIngredients(); ++i)
				{
					textPosition.Y += ((16 * Scale) / 2) + (CurrentRecipe.getNumberOfIngredients() < 5 ? 4 : 0);

					int id = CurrentRecipe.recipeList.Keys.ElementAt(i);
					string ingredientNameText = CurrentRecipe.getNameFromIndex(id);
					
					// Show category-specific information for general category ingredient rules
					// Icons are furnished with some recognisable stereotypes of items from each category
					if (id < 0)
					{
						switch (id)
						{
							case -81:
								ingredientNameText = i18n.Get("item.forage.label");
								id = 22; // Dandelion
								break;
							case -80:
								ingredientNameText = i18n.Get("item.flower.label");
								id = 591; // Tulip
								break;
							case -79:
								ingredientNameText = i18n.Get("item.fruit.label");
								id = 406; // Wild Plum
								break;
							case -75:
								ingredientNameText = i18n.Get("item.vegetable.label");
								id = 278; // Bok Choy
								break;
							case -14:
								ingredientNameText = i18n.Get("item.meat.label");
								id = 640; // Unused meat (white steak, raw)
								break;
							case -7:
								ingredientNameText = i18n.Get("item.cooking.label");
								id = 662; // Unused cooking (nice bowl of red sauce)
								break;
							case -2:
								ingredientNameText = i18n.Get("item.gem.label");
								id = 60; // Emerald
								break;
							case -12:
								ingredientNameText = i18n.Get("item.mineral.label");
								id = 546; // Geminite
								break;
							case -15:
								ingredientNameText = i18n.Get("item.metal.label");
								id = 380; // Iron Ore (variant A)
								break;
							case -74:
								ingredientNameText = i18n.Get("item.seed.label");
								id = 770; // Mixed Seeds
								break;
						}
					}

					int requiredQuantity = CurrentRecipe.recipeList.Values.ElementAt(i);
					Color drawColour = _recipeIngredientQuantitiesHeld[i] < requiredQuantity
						? BlockedColour
						: i >= _cookingManager.MaxIngredients
							? Color.Firebrick * 0.8f
							: Game1.textColor;

					// Ingredient icon
					b.Draw(
						texture: Game1.objectSpriteSheet,
						position: new Vector2(_leftContent.X, textPosition.Y - 2f),
						sourceRectangle: Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet,
							CurrentRecipe.getSpriteIndexFromRawIndex(id), StardewValley.Object.spriteSheetTileSize, StardewValley.Object.spriteSheetTileSize),
						color: Color.White, rotation: 0f, origin: Vector2.Zero, scale: 2f, effects: SpriteEffects.None, layerDepth: 0.86f);
					// Ingredient quantity
					Utility.drawTinyDigits(
						toDraw: CurrentRecipe.recipeList.Values.ElementAt(i),
						b,
						position: new Vector2(
							_leftContent.X + (8 * Scale) - Game1.tinyFont.MeasureString(string.Concat(CurrentRecipe.recipeList.Values.ElementAt(i))).X,
							textPosition.Y + (4.5f * Scale)),
						scale: 2f,
						layerDepth: 0.87f,
						c: Color.AntiqueWhite);
					// Ingredient name
					this.DrawText(b, ingredientNameText, 1f, (12 * Scale), textPosition.Y, null, isLeftSide, false, drawColour);

					// Ingredient stock
					if (Game1.options.showAdvancedCraftingInformation)
					{
						Point position = new Point((int)(_lineWidth - (16 * Scale * xScale)), (int)(textPosition.Y + (0.5f * Scale)));
						b.Draw(
							texture: Game1.mouseCursors,
							destinationRectangle: new Rectangle(_leftContent.X + position.X, position.Y, 22, 26),
							sourceRectangle: new Rectangle(268, 1436, 11, 13),
							color: Color.White);
						this.DrawText(b, _recipeIngredientQuantitiesHeld[i].ToString(), 1f, position.X + 32, position.Y, 72, isLeftSide, false, drawColour);
					}
				}
			}
			else
			{
				textPosition.Y += (16 * Scale / 2) + (1 * Scale);
				text = i18n.Get("menu.cooking_recipe.title_unknown");
				this.DrawText(b, text, 1f, 10 * Scale, textPosition.Y, textWidth, isLeftSide, false, SubtextColour);
			}
		}

		private void DrawCraftingPage(SpriteBatch b)
		{
			float xScale = _locale == "ko" && _resizeKoreanFonts ? KoWidthScale : 1f;
			float yScale = _locale == "ko" && _resizeKoreanFonts ? KoHeightScale : 1f;

			Dictionary<int, double> iconShakeTimer = _iconShakeTimerField.GetValue();

			// Cooking slots
			foreach (ClickableTextureComponent clickable in _ingredientsClickables)
				clickable.draw(b);

			for (int i = 0; i < _cookingManager.CurrentIngredients.Count; ++i)
			{
				Item item = _cookingManager.GetItemForIngredient(index: i, sourceItems: _allInventories);
				if (item == null)
					continue;

				Vector2 position = new Vector2(
					_ingredientsClickables[i].bounds.X + _ingredientsClickables[i].bounds.Width / 2 - 64 / 2,
					_ingredientsClickables[i].bounds.Y + _ingredientsClickables[i].bounds.Height / 2 - 64 / 2);

				// Item icon
				item.drawInMenu(
					b,
					location: position + (!iconShakeTimer.ContainsKey(i) 
						? Vector2.Zero
						: 1f * new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2))),
					scaleSize: 1f,
					transparency: 1f,
					layerDepth: 0.865f,
					drawStackNumber: StackDrawType.Hide,
					color: Color.White,
					drawShadow: true);

				position = new Vector2(
					_ingredientsClickables[i].bounds.X + _ingredientsClickables[i].bounds.Width - (7 * Scale),
					_ingredientsClickables[i].bounds.Y + _ingredientsClickables[i].bounds.Height - (6.2f * Scale));
				if (item.Stack > 0 && item.Stack < int.MaxValue)
				{
					// Item stack count
					Utility.drawTinyDigits(
						toDraw: item.Stack,
						b,
						position: position,
						scale: SmallScale, 
						layerDepth: 1f,
						c: Color.White);
				}
				if (item is StardewValley.Object o && o.Quality > 0)
				{
					// Item quality star
					//position += new Vector2(0.5f * Scale, -(6 + 1) * Scale);
					position = new Vector2(
						_ingredientsClickables[i].bounds.X + (3 * Scale),
						_ingredientsClickables[i].bounds.Y + _ingredientsClickables[i].bounds.Height - (7 * Scale));
					b.Draw(
						texture: Game1.mouseCursors,
						position: position,
						sourceRectangle: new Rectangle(o.Quality < 2 ? 338 : 346, o.Quality % 4 == 0 ? 392 : 400, 8, 8),
						color: Color.White,
						rotation: 0f,
						origin: Vector2.Zero,
						scale: SmallScale,
						effects: SpriteEffects.None,
						layerDepth: 1f);
				}
			}

			Vector2 textPosition = Vector2.Zero;
			int textWidth = (int)(_textWidth * xScale);
			const int spriteWidth = StardewValley.Object.spriteSheetTileSize;
			string text;

			// Recipe notes
			text = i18n.Get("menu.cooking_recipe.notes_label");
			textPosition.Y = _cookbookRightRect.Y + _cookbookRightRect.Height - (50 * Scale) - Game1.smallFont.MeasureString(
				Game1.parseText(text: text, whichFont: Game1.smallFont, width: textWidth)).Y * yScale;

			int fryingPanLevel = ModEntry.Config.AddCookingToolProgression ? ModEntry.Instance.States.Value.CookingToolLevel : 0;
			if (_showCookingConfirmPopup)
			{
				textPosition.Y += 5 * Scale;
				textPosition.X += 16 * Scale;
				int xOffset = (4 * Scale);
				int yOffset = (2 * Scale);
				int yOffsetExtra = (int)(Math.Cos((Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 300) % 300) * (2 * Scale));
				int ySpacing = ModEntry.Instance.States.Value.CookingToolLevel <= 1 ? 0 : (fryingPanLevel / 2 * Scale);
				int fryingPanOffset = (6 * Scale);
				int fryingPanX = _cookIconBounds.X + xOffset + fryingPanOffset;
				int fryingPanY = _cookIconBounds.Y + yOffset + ySpacing + fryingPanOffset;

				// Frying pan
				b.Draw(
					texture: Game1.shadowTexture,
					position: new Vector2(
						fryingPanX + (0 * Scale),
						fryingPanY + (spriteWidth / 2 * Scale) + (2 * Scale)),
					sourceRectangle: null,
					color: Color.White,
					rotation: 0f,
					origin: Vector2.Zero,
					scale: Scale,
					effects: SpriteEffects.None,
					layerDepth: 1f);
				b.Draw(
					texture: Texture,
					destinationRectangle: new Rectangle(fryingPanX, fryingPanY, spriteWidth * Scale, spriteWidth * Scale),
					sourceRectangle: new Rectangle(176 + fryingPanLevel * spriteWidth, 272, spriteWidth, spriteWidth),
					color: Color.White, rotation: 0f, origin: Vector2.Zero, effects: SpriteEffects.None, layerDepth: 1f);

				// Contextual cooking popup
				//Game1.DrawBox(x: _cookIconBounds.X, y: _cookIconBounds.Y, width: _cookIconBounds.Width, height: _cookIconBounds.Height);
				CurrentRecipe?.drawMenuView(b,
					x: _cookIconBounds.X + xOffset,
					y: _cookIconBounds.Y + yOffset + yOffsetExtra - ySpacing,
					shadow: false);

				_cookQuantityUpButton.draw(b);
				_quantityTextBox.Draw(b);
				_cookQuantityDownButton.draw(b);

				_cookConfirmButton.draw(b);
				_cookCancelButton.draw(b);

				return;
			}

			if (!_stack.Any() || _stack.Peek() != State.Recipe)
				return;

			this.DrawHorizontalDivider(b, 0, textPosition.Y, _lineWidth, false);
			textPosition.Y += TextDividerGap;
			this.DrawText(b, text, 1f, textPosition.X, textPosition.Y, null, false, false, SubtextColour);
			textPosition.Y += Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).Y * yScale;
			this.DrawHorizontalDivider(b, 0, textPosition.Y, _lineWidth, false);
			textPosition.Y += ((6 * Scale) / 2);

			if (_recipeAsItem == null || !_stack.Any() || _stack.Peek() != State.Recipe)
				return;

			if (ReadyToCook)
			{
				textPosition.Y += 3 * Scale;
				textPosition.X = _rightContent.X + _cookbookRightRect.Width / 2 - MarginRight;
				int frypanWidth = false && Config.AddCookingToolProgression ? spriteWidth + (1 * Scale) : 0;

				// Cook! button
				int extraHeight = new [] { "ko", "ja", "zh", "tr" }.Contains(_locale) ? (1 * Scale) : 0;
				Rectangle source = CookButtonSource;
				source.X += _animFrame * CookButtonSource.Width;
				Rectangle dest = new Rectangle(
					(int)textPosition.X - frypanWidth / 2 * Scale,
					(int)textPosition.Y - extraHeight,
					source.Width * Scale,
					source.Height * Scale + extraHeight);
				dest.X -= (CookTextSourceWidths[_locale] / 2 * Scale - CookTextSideSourceWidth * Scale) + MarginLeft - frypanWidth / 2;
				Rectangle clickableArea = new Rectangle(
					dest.X,
					dest.Y - extraHeight,
					CookTextSideSourceWidth * Scale * 2 + (_cookTextMiddleSourceWidth + frypanWidth) * Scale,
					dest.Height + extraHeight);
				if (clickableArea.Contains(Game1.getMouseX(), Game1.getMouseY()))
					source.Y += source.Height;
				// left
				source.Width = CookTextSideSourceWidth;
				dest.Width = source.Width * Scale;
				b.Draw(
					texture: Texture, destinationRectangle: dest, sourceRectangle: source,
					color: Color.White, rotation: 0f, origin: Vector2.Zero, effects: SpriteEffects.None, layerDepth: 1f);
				// middle and text and frypan
				source.X = _animFrame * CookButtonSource.Width + CookButtonSource.X + CookTextSideSourceWidth;
				source.Width = 1;
				dest.Width = (_cookTextMiddleSourceWidth + frypanWidth) * Scale;
				dest.X += CookTextSideSourceWidth * Scale;
				b.Draw(
					texture: Texture, destinationRectangle: dest, sourceRectangle: source,
					color: Color.White, rotation: 0f, origin: Vector2.Zero, effects: SpriteEffects.None, layerDepth: 1f);
				b.Draw(
					texture: Texture,
					destinationRectangle: new Rectangle(
						dest.X + (1 * Scale),
						dest.Y + (int)((2 * Scale) + Math.Cos(_animTimer / (16 * Scale) * 100) * 8),
						CookTextSource[_locale].Width * Scale,
						CookTextSource[_locale].Height * Scale + extraHeight),
					sourceRectangle: CookTextSource[_locale],
					color: Color.White, rotation: 0f, origin: Vector2.Zero, effects: SpriteEffects.None, layerDepth: 1f);
				dest.X += _cookTextMiddleSourceWidth * Scale;
				dest.Width = spriteWidth * Scale;

				// right
				source.X = _animFrame * CookButtonSource.Width + CookButtonSource.X + CookButtonSource.Width - CookTextSideSourceWidth;
				source.Width = CookTextSideSourceWidth;
				dest.Width = source.Width * Scale;
				dest.X += frypanWidth * Scale;
				b.Draw(
					texture: Texture, destinationRectangle: dest, sourceRectangle: source,
					color: Color.White, rotation: 0f, origin: Vector2.Zero, effects: SpriteEffects.None, layerDepth: 1f);

				// DANCING FORKS
				if (Config.DebugMode && false)
				{
					SpriteEffects flipped = _animFrame >= 4 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
					for (int i = 0; i < 2; ++i)
					{
						int sourceOffset = (i == 1 ? 32 : 0) + (_animFrame % 2 == 0 ? 32 : 0);
						int destOffset = i == 1 ? (source.Width * 2 + _cookTextMiddleSourceWidth) * Scale + (24 * Scale) : 0;
						b.Draw(
							texture: Texture,
							position: new Vector2(_rightContent.X + destOffset - (2 * Scale), dest.Y - (8 * Scale)),
							sourceRectangle: new Rectangle(128 + sourceOffset, 48, 32, 32),
							color: Color.White, rotation: 0f, origin: Vector2.Zero, scale: Scale, effects: flipped, layerDepth: 1f);
					}
				}
			}
			else if (Config.HideFoodBuffsUntilEaten
				&& (!ModEntry.Instance.States.Value.FoodsEaten.Contains(_recipeAsItem.Name)))
			{
				text = i18n.Get("menu.cooking_recipe.notes_unknown");
				this.DrawText(b, text, 1f, textPosition.X, textPosition.Y, textWidth, false, false, SubtextColour);
			}
			else
			{
				const float xOffset = (8.25f * Scale);

				// Energy
				textPosition.X = _locale != "zh" ? -(2 * Scale) : (2 * Scale);
				text = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3116",
					_recipeAsItem.staminaRecoveredOnConsumption());
				if (true)
					b.Draw(
						texture: Game1.mouseCursors,
						position: new Vector2(_rightContent.X + textPosition.X, textPosition.Y),
						sourceRectangle: new Rectangle(0, 428, 10, 10),
						color: Color.White, rotation: 0f, origin: Vector2.Zero, scale: SmallScale,
						effects: SpriteEffects.None, layerDepth: 1f);
				else
					Utility.drawWithShadow(b,
						texture: Game1.mouseCursors,
						position: new Vector2(_rightContent.X + textPosition.X, textPosition.Y),
						sourceRect: new Rectangle(0, 428, 10, 10),
						color: Color.White, rotation: 0f, origin: Vector2.Zero, scale: SmallScale);
				textPosition.X += xOffset;
				this.DrawText(b, text, 1f, textPosition.X, textPosition.Y, null, false, false, Game1.textColor);
				textPosition.Y += Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).Y * yScale;
				// Health
				text = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3118",
					_recipeAsItem.healthRecoveredOnConsumption());
				textPosition.X -= xOffset;
				if (true)
					b.Draw(
						texture: Game1.mouseCursors,
						position: new Vector2(_rightContent.X + textPosition.X, textPosition.Y),
						sourceRectangle: new Rectangle(0, 428 + 10, 10, 10),
						color: Color.White, rotation: 0f, origin: Vector2.Zero, scale: SmallScale,
						effects: SpriteEffects.None, layerDepth: 1f);
				else
					Utility.drawWithShadow(b,
						texture: Game1.mouseCursors,
						position: new Vector2(_rightContent.X + textPosition.X, textPosition.Y),
						sourceRect: new Rectangle(0, 428 + 10, 10, 10),
						color: Color.White, rotation: 0f, origin: Vector2.Zero, scale: SmallScale);
				textPosition.X += xOffset;
				this.DrawText(b, text, 1f, textPosition.X, textPosition.Y, null, false, false, Game1.textColor);

				// Buff duration
				text = $"+{(_recipeBuffDuration / 60)}:{(_recipeBuffDuration % 60):00}";

				if (_recipeBuffDuration > 0)
				{
					textPosition.Y += Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).Y * 1.1f * yScale;
					textPosition.X -= xOffset;
					if (true)
						b.Draw(
							texture: Game1.mouseCursors,
							position: new Vector2(_rightContent.X + textPosition.X, textPosition.Y),
							sourceRectangle: new Rectangle(434, 475, 9, 9),
							color: Color.White, rotation: 0f, origin: Vector2.Zero, scale: SmallScale,
							effects: SpriteEffects.None, layerDepth: 1f);
					else
						Utility.drawWithShadow(b,
							texture: Game1.mouseCursors,
							position: new Vector2(_rightContent.X + textPosition.X, textPosition.Y),
							sourceRect: new Rectangle(434, 475, 9, 9),
							color: Color.White, rotation: 0f, origin: Vector2.Zero, scale: SmallScale);
					textPosition.X += xOffset;
					this.DrawText(b, text, 1f, textPosition.X, textPosition.Y, null, false, false, Game1.textColor);
					textPosition.Y -= Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).Y * 1.1f * yScale;
				}

				textPosition.Y -= Game1.smallFont.MeasureString(Game1.parseText(text: text, whichFont: Game1.smallFont, width: textWidth)).Y * yScale;
				textPosition.X += -xOffset + (_lineWidth / 2f) + (4 * Scale);

				// Buffs
				if (_recipeBuffs != null && _recipeBuffs.Count > 0)
				{
					const int maxBuffsToDisplay = 4;
					int count = 0;
					for (int i = 0; i < _recipeBuffs.Count && count < maxBuffsToDisplay; ++i)
					{
						if (_recipeBuffs[i] == 0)
							continue;

						++count;
						if (true)
							b.Draw(
								texture: Game1.mouseCursors,
								position: new Vector2(_rightContent.X + textPosition.X, textPosition.Y),
								sourceRectangle: new Rectangle(10 + 10 * i, 428, 10, 10),
								color: Color.White, rotation: 0f, origin: Vector2.Zero, scale: SmallScale,
								effects: SpriteEffects.None, layerDepth: 1f);
						else
							Utility.drawWithShadow(b,
								texture: Game1.mouseCursors,
								position: new Vector2(_rightContent.X + textPosition.X, textPosition.Y),
								sourceRect: new Rectangle(10 + 10 * i, 428, 10, 10),
								color: Color.White, rotation: 0f, origin: Vector2.Zero, scale: SmallScale);
						textPosition.X += xOffset;
						text = (_recipeBuffs[i] > 0 ? "+" : "")
							   + _recipeBuffs[i]
							   + " " + i18n.Get($"menu.cooking_recipe.buff.{i}");
						this.DrawText(b, text, 1f, textPosition.X, textPosition.Y, null, false, false, Game1.textColor);
						textPosition.Y += Game1.smallFont.MeasureString(Game1.parseText(text: text, whichFont: Game1.smallFont, width: textWidth)).Y * yScale;
						textPosition.X -= xOffset;
					}
				}
			}
		}

		private void drawInventoryIcon(SpriteBatch b, int which, Vector2 position, float scale)
		{
			Rectangle destRect = new Rectangle(
				(int)(position.X + _inventorySelectButtons[which].bounds.Width / 2),
				(int)(position.Y + _inventorySelectButtons[which].bounds.Height / 2),
				(int)(_inventorySelectButtons[which].sourceRect.Width * scale),
				(int)(_inventorySelectButtons[which].sourceRect.Height * scale));
			b.Draw(
				texture: Texture,
				destinationRectangle: destRect,
				sourceRectangle: _inventorySelectButtons[which].sourceRect,
				color: Color.White,
				rotation: 0f,
				origin: new Vector2(
					_inventorySelectButtons[which].sourceRect.Width,
					_inventorySelectButtons[which].sourceRect.Height) / 2,
				effects: SpriteEffects.None,
				layerDepth: 1f);
			if (which >= _inventoryIdsBeforeChests)
			{
				// chest button tint
				KeyValuePair<Color, bool> tintAndEnabled = _chestColours[which - _inventoryIdsBeforeChests];
				if (tintAndEnabled.Value)
				{
					b.Draw(
						texture: Texture,
						destinationRectangle: destRect,
						sourceRectangle: new Rectangle(
							_inventorySelectButtons[which].sourceRect.X,
							_inventorySelectButtons[which].sourceRect.Y + _inventorySelectButtons[which].sourceRect.Height,
							_inventorySelectButtons[which].sourceRect.Width,
							_inventorySelectButtons[which].sourceRect.Height),
						color: tintAndEnabled.Key,
						rotation: 0f,
						origin: new Vector2(
							_inventorySelectButtons[which].sourceRect.Width,
							_inventorySelectButtons[which].sourceRect.Height) / 2,
						effects: SpriteEffects.None,
						layerDepth: 1f);
				}
			}
		}


		private void DrawInventoryMenu(SpriteBatch b)
		{
			if (!_inventorySelectButtons.Any())
				return;

			// Actual inventory card
			Game1.DrawBox(x: _inventoryCardArea.X, y: _inventoryCardArea.Y,
				width: _inventoryCardArea.Width, height: _inventoryCardArea.Height);

			// Inventory select tab
			_inventoryTabButton.draw(b);
			drawInventoryIcon(b,
				which: _inventoryId,
				position: new Vector2(
					_inventoryTabButton.bounds.X + (2 * _inventoryTabButton.baseScale),
					_inventoryTabButton.bounds.Y + (2 * _inventoryTabButton.baseScale)),
				scale: _inventoryTabButton.scale);

			// Inventory nav buttons
			if (this.ShouldShowInventoryElements)
			{
				_inventoryUpButton.draw(b);
				_inventoryDownButton.draw(b);
			}

			// Items
			if (_showInventoriesPopup)
			{
				// Inventory select buttons
				Game1.DrawBox(x: _inventoriesPopupArea.X, y: _inventoriesPopupArea.Y,
					width: _inventoriesPopupArea.Width, height: _inventoriesPopupArea.Height);
				for (int i = 0; i < _inventorySelectButtons.Count; ++i)
				{
					// nav button icon
					drawInventoryIcon(b,
						which: i,
						position: Utility.PointToVector2(_inventorySelectButtons[i].bounds.Location),
						scale: _inventorySelectButtons[i].scale);
				}

				// Inventory nav selected icon
				int w = 9;
				Rectangle sourceRect = new Rectangle(232 + 9 * ((int)(w * ((float)_animFrame / AnimFrames * 6)) / 9), 346, w, w);
				Rectangle currentButton = _inventorySelectButtons[_inventoryId].bounds;
				b.Draw(
					texture: Game1.mouseCursors,
					destinationRectangle: new Rectangle(
						currentButton.X + (Scale * (((currentButton.Width) - (w * Scale)) / Scale / 2)),
						currentButton.Y - (w * Scale) + (4 * Scale),
						w * Scale,
						w * Scale),
					sourceRectangle: sourceRect,
					color: Color.White);
			}
		}

		/// <summary>
		/// Mostly a copy of InventoryMenu.draw(SpriteBatch b, int red, int blue, int green),
		/// though items considered unable to be cooked will be greyed out.
		/// </summary>
		private void DrawActualInventory(SpriteBatch b)
		{
			Dictionary<int, double> iconShakeTimer = _iconShakeTimerField.GetValue();
			for (int key = 0; key < inventory.inventory.Count; ++key)
			{
				if (iconShakeTimer.ContainsKey(key)
					&& Game1.currentGameTime.TotalGameTime.TotalSeconds >= iconShakeTimer[key])
				{
					iconShakeTimer.Remove(key);
				}
			}
			_iconShakeTimerField.SetValue(iconShakeTimer);
			for (int i = 0; i < inventory.capacity; ++i)
			{
				Vector2 position = new Vector2(
					inventory.xPositionOnScreen
					 + i % (inventory.capacity / inventory.rows) * 64
					 + inventory.horizontalGap * (i % (inventory.capacity / inventory.rows)),
					inventory.yPositionOnScreen
						+ i / (inventory.capacity / inventory.rows) * (64 + inventory.verticalGap)
						+ (i / (inventory.capacity / inventory.rows) - 1) * 4
						- (i >= inventory.capacity / inventory.rows
						   || !inventory.playerInventory || inventory.verticalGap != 0 ? 0 : 12));

				b.Draw(
					Game1.menuTexture,
					position,
					Game1.getSourceRectForStandardTileSheet(tileSheet: Game1.menuTexture, tilePosition: 10),
					Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);

				if ((inventory.playerInventory || inventory.showGrayedOutSlots) && i >= Game1.player.maxItems.Value)
					b.Draw(
						texture: Game1.menuTexture,
						position: position,
						sourceRectangle: Game1.getSourceRectForStandardTileSheet(tileSheet: Game1.menuTexture, tilePosition: 57),
						color: Color.White * 0.5f, rotation: 0f, origin: Vector2.Zero, scale: 1f, effects: SpriteEffects.None, layerDepth: 0.5f);

				if (i >= 12 || !inventory.playerInventory)
					continue;

				string text;
				switch (i)
				{
					case 9:
						text = "0";
						break;
					case 10:
						text = "-";
						break;
					case 11:
						text = "=";
						break;
					default:
						text = string.Concat(i + 1);
						break;
				}
				Vector2 textSize = Game1.tinyFont.MeasureString(text);
				b.DrawString(
					spriteFont: Game1.tinyFont,
					text: text,
					position: position + new Vector2((float)(32.0 - textSize.X / 2.0), -textSize.Y),
					color: i == Game1.player.CurrentToolIndex ? Color.Red : Color.DimGray);
			}
			for (int i = 0; i < inventory.capacity; ++i)
			{
				Vector2 location = new Vector2(
					inventory.xPositionOnScreen
					 + i % (inventory.capacity / inventory.rows) * 64
					 + inventory.horizontalGap * (i % (inventory.capacity / inventory.rows)),
					inventory.yPositionOnScreen
						+ i / (inventory.capacity / inventory.rows) * (64 + inventory.verticalGap)
						+ (i / (inventory.capacity / inventory.rows) - 1) * 4
						- (i >= inventory.capacity / inventory.rows
						   || !inventory.playerInventory || inventory.verticalGap != 0 ? 0 : 12));

				if (inventory.actualInventory.Count <= i || inventory.actualInventory.ElementAt(i) == null)
					continue;

				Color colour = !_cookingManager.IsInventoryItemInCurrentIngredients(inventoryIndex: _inventoryId, itemIndex: i)
					? CookingManager.CanBeCooked(item: inventory.actualInventory[i])
						? Color.White
						: Color.Gray * 0.25f
					: Color.White * 0.35f;
				bool drawShadow = inventory.highlightMethod(inventory.actualInventory[i]);
				if (iconShakeTimer.ContainsKey(i))
					location += 1f * new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2));
				inventory.actualInventory[i].drawInMenu(
					b,
					location: location,
					scaleSize: inventory.inventory.Count > i ? inventory.inventory[i].scale : 1f,
					transparency: !inventory.highlightMethod(inventory.actualInventory[i]) ? 0.25f : 1f,
					layerDepth: 0.865f,
					drawStackNumber: StackDrawType.Draw,
					color: colour,
					drawShadow: drawShadow);
			}
		}

		private void DrawExtraStuff(SpriteBatch b)
		{
			/*
			if (poof != null)
			{
				poof.draw(b, true);
			}
			*/

			upperRightCloseButton.draw(b);

			// Hover text
			if (hoverText != null)
			{
				if (hoverAmount > 0)
					drawToolTip(b, hoverText: hoverText, hoverTitle: "", hoveredItem: null, heldItem: true, moneyAmountToShowAtBottom: hoverAmount);
				else
					drawHoverText(b, text: hoverText, font: Game1.smallFont);
			}

			// Hover elements
			if (hoveredItem != null)
				drawToolTip(b, hoverText: hoveredItem.getDescription(), hoverTitle: hoveredItem.DisplayName, hoveredItem: hoveredItem, heldItem: heldItem != null);
			else if (hoveredItem != null && ItemsToGrabMenu != null)
				drawToolTip(b, hoverText: ItemsToGrabMenu.descriptionText, hoverTitle: ItemsToGrabMenu.descriptionTitle, hoveredItem: hoveredItem, heldItem: heldItem != null);
			heldItem?.drawInMenu(b, location: new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), scaleSize: 1f);

			// Search button
			_searchTabButton.draw(b);

			// Ingredients button
			if (IsIngredientsPageEnabled)
				_ingredientsTabButton.draw(b);

			// Cursor
			Game1.mouseCursorTransparency = 1f;
			this.drawMouse(b);
		}

		// we're all float down here
		private void DrawText(SpriteBatch b, string text, float scale, float x, float y, float? w, bool isLeftSide, bool isRightJustified = false, Color? colour = null)
		{
			SpriteFont font = Game1.smallFont;
			Point position = isLeftSide ? _leftContent : _rightContent;
			position.Y -= yPositionOnScreen;
			w ??= font.MeasureString(text).X + (_locale == "ja" || _locale == "zh" ? 20 : 0);
			if (isRightJustified)
				position.X -= (int)font.MeasureString(text).X;
			if (_locale == "ko" && _resizeKoreanFonts)
				scale *= KoHeightScale;
			Utility.drawTextWithShadow(b,
				text: Game1.parseText(text: text, whichFont: font, width: (int)w),
				font: font,
				position: new Vector2(position.X + x, position.Y + y),
				color: colour ?? Game1.textColor,
				scale: scale);
		}

		private void DrawHorizontalDivider(SpriteBatch b, float x, float y, int w, bool isLeftSide)
		{
			Point position = isLeftSide ? _leftContent : _rightContent;
			position.Y -= yPositionOnScreen;
			Utility.drawLineWithScreenCoordinates(
				x1: (int)(position.X + x) + TextMuffinTopOverDivider, y1: (int)(position.Y + y),
				x2: (int)(position.X + x) + w + TextMuffinTopOverDivider, y2: (int)(position.Y + y),
				b, color1: DividerColour * DividerOpacity);
		}
	}
}
