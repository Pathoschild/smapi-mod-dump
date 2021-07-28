/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LoveOfCooking.GameObjects
{
	public class CookingMenu : ItemGrabMenu
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static Config Config => ModEntry.Instance.Config;
		private static ITranslationHelper i18n => ModEntry.Instance.Helper.Translation;
		private static Texture2D Texture => ModEntry.SpriteSheet;
		private readonly CookingManager _cookingManager;

		// Spritesheet source areas
		// Custom spritesheet
		private static readonly Rectangle CookbookSource = new Rectangle(0, 80, 240, 128);
		internal static readonly Rectangle CookingSlotOpenSource = new Rectangle(0, 208, 28, 28);
		internal static readonly Rectangle CookingSlotLockedSource = new Rectangle(28, 208, 28, 28);
		private static readonly Rectangle CookButtonSource = new Rectangle(128, 0, 16, 22);
		private static readonly Rectangle SearchTabButtonSource = new Rectangle(48, 0, 16, 16);
		private static readonly Rectangle IngredientsTabButtonSource = new Rectangle(32, 0, 16, 16);
		private static readonly Rectangle FilterContainerSource = new Rectangle(58, 208, 9, 20);
		private static readonly Rectangle FilterIconSource = new Rectangle(69, 208, 12, 12);
		private static readonly Rectangle ToggleViewButtonSource = new Rectangle(80, 224, 16, 16);
		private static readonly Rectangle ToggleFilterButtonSource = new Rectangle(112, 224, 16, 16);
		private static readonly Rectangle ToggleOrderButtonSource = new Rectangle(128, 224, 16, 16);
		private static readonly Rectangle SearchButtonSource = new Rectangle(144, 224, 16, 16);
		private static readonly Rectangle FavouriteIconSource = new Rectangle(58, 230, 9, 9);
		private static readonly Rectangle AutofillButtonSource = new Rectangle(112, 272, 16, 16);
		// MouseCursors sheet
		private static readonly Rectangle DownButtonSource = new Rectangle(0, 64, 64, 64);
		private static readonly Rectangle UpButtonSource = new Rectangle(64, 64, 64, 64);
		private static readonly Rectangle RightButtonSource = new Rectangle(0, 192, 64, 64);
		private static readonly Rectangle LeftButtonSource = new Rectangle(0, 256, 64, 64);
		private static readonly Rectangle PlusButtonSource = new Rectangle(184, 345, 7, 8);
		private static readonly Rectangle MinusButtonSource = new Rectangle(177, 345, 7, 8);
		private static readonly Rectangle OkButtonSource = new Rectangle(128, 256, 64, 64);
		private static readonly Rectangle NoButtonSource = new Rectangle(192, 256, 64, 64);
		// Other values
		internal const int Scale = 4;
		private const float KoHeightScale = 0.825f;
		private const float KoWidthScale = 1.25f;
		private readonly bool _resizeKoreanFonts;
		private static readonly Dictionary<string, Rectangle> CookTextSource = new Dictionary<string, Rectangle>();
		private static readonly Point CookTextSourceOrigin = new Point(0, 240);
		private static readonly Dictionary<string, int> CookTextSourceWidths = new Dictionary<string, int>
		{
			{"en", 32},
			{"fr", 45},
			{"es", 42},
			{"pt", 48},
			{"ja", 50},
			{"zh", 36},
			{"ko", 48},
			{"ru", 53},
			{"de", 40},
			{"it", 48},
			{"tr", 27 }
		};
		private const int CookTextSourceHeight = 16;
		private const int CookTextSideWidth = 5;
		private const int FilterContainerSideWidth = 4;
		private int _cookTextMiddleWidth;
		private int _filterContainerMiddleWidth;

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
		private readonly ClickableTextureComponent _autofillButton;
		private Rectangle _filterContainerBounds;
		private readonly List<ClickableTextureComponent> _filterButtons = new List<ClickableTextureComponent>();
		private Rectangle _searchResultsArea;
		private Rectangle _quantityScrollableArea;
		private Rectangle _inventoriesScrollableArea;
		private readonly List<ClickableTextureComponent> _inventorySelectButtons = new List<ClickableTextureComponent>();
		private readonly List<ClickableComponent> _searchListClickables = new List<ClickableComponent>();
		private readonly List<ClickableComponent> _searchGridClickables = new List<ClickableComponent>();

		// Layout dimensions (variable with screen size)
		private Rectangle _cookbookLeftRect
			= new Rectangle(-1, -1, CookbookSource.Width * 4 / 2, CookbookSource.Height * Scale);
		private Rectangle _cookbookRightRect
			= new Rectangle(-1, -1, CookbookSource.Width * 4 / 2, CookbookSource.Height * Scale);
		private Point _leftContent;
		private Point _rightContent;
		private int _lineWidth;
		private int _textWidth;

		// Layout definitions
		private const int MarginLeft = 16 * Scale;
		private const int MarginRight = 8 * Scale;
		private const int TextMuffinTopOverDivider = 6;
		private const int TextDividerGap = 4;
		private const int TextSpacingFromIcons = 80;
		private const int RecipeListHeight = 16 * Scale;
		private const int RecipeGridHeight = 18 * Scale;
		private const int InventoryRegionStartID = 0;

		private static readonly Color SubtextColour = Game1.textColor * 0.75f;
		private static readonly Color BlockedColour = Game1.textColor * 0.325f;
		private static readonly Color DividerColour = Game1.textColor * 0.325f;

		private bool UseHorizontalInventoryButtonArea => _inventorySelectButtons.Any() && Context.IsSplitScreen;

		// Animations
		private static readonly int[] CookButtonAnimTextOffsetPerFrame = { 0, 1, 0, -1, -2, -3, -2, -1 };
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
		private List<CraftingRecipe> _recipeSearchResults;
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
		internal const int BackpackInventoryId = 0;
		internal const int FridgeInventoryId = 1;
		internal const int MaximumMiniFridges = 12;
		internal static readonly int InventoryIdsBeforeMinifridges = new int[]
			{ BackpackInventoryId, FridgeInventoryId }.Length;
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
			Favourite,
			Count
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
		private const bool IsAutofillEnabled = true;
		private bool IsUsingAutofill
		{
			get
			{
				return IsAutofillEnabled && bool.Parse(Game1.player.modData[ModEntry.AssetPrefix + "autofill"]);
			}
			set
			{
				Game1.player.modData[ModEntry.AssetPrefix + "autofill"] = value.ToString();
				Log.D($"Autofill set to {value}",
					Config.DebugMode);
			}
		}


		public CookingMenu(List<CraftingRecipe> recipes, bool addDummyState = false, string initialRecipe = null) : base(inventory: null, context: null)
		{
			Game1.displayHUD = true; // Prevents hidden HUD on crash when initialising menu, set to false at the end of this method
			_locale = LocalizedContentManager.CurrentLanguageCode.ToString();
			if (!CookTextSourceWidths.ContainsKey(_locale))
			{
				_locale = "en";
			}
			_resizeKoreanFonts = Config.ResizeKoreanFonts;
			this.initializeUpperRightCloseButton();
			trashCan = null;
			_cookingManager = new CookingManager(cookingMenu: this)
			{
				MaxIngredients = ModEntry.Instance.GetNearbyCookingStationLevel()
			};

			_iconShakeTimerField = Helper.Reflection.GetField<Dictionary<int, double>>(inventory, "_iconShakeTimer");

			_recipesAvailable = recipes != null
				// Recipes may be populated by those of any CraftingMenu that this menu supercedes
				// Should guarantee Limited Campfire Cooking compatibility
				? recipes.Where(recipe => Game1.player.cookingRecipes.ContainsKey(recipe.name)).ToList()
				// Otherwise start off the list of cooking recipes with all those the player has unlocked
				: _recipesAvailable = Utility.GetAllPlayerUnlockedCookingRecipes()
					.Select(str => new CraftingRecipe(str, true))
					.Where(recipe => recipe.name != "Torch").ToList();

			ModEntry.Instance.UpdateEnglishRecipeDisplayNames(ref _recipesAvailable);

			// Default autofill preferences if none set
			if (!Game1.player.modData.ContainsKey(ModEntry.AssetPrefix + "autofill"))
			{
				IsUsingAutofill = false;
			}
			Log.D($"Autofill on startup: {IsUsingAutofill}",
				Config.DebugMode);

			// Apply default filter to the default recipe list
			bool reverseDefaultFilter = ModEntry.Instance.States.Value.LastFilterReversed;
			_recipesAvailable = this.FilterRecipes();

			// Initialise filtered search lists
			_recipesFiltered = _recipesAvailable;
			_recipeSearchResults = new List<CraftingRecipe>();

			// Clickables and elements
			_navDownButton = new ClickableTextureComponent(
				"navDown", new Rectangle(-1, -1, DownButtonSource.Width, DownButtonSource.Height),
				null, null, Game1.mouseCursors, DownButtonSource, 1f, true);
			_navUpButton = new ClickableTextureComponent(
				"navUp", new Rectangle(-1, -1, UpButtonSource.Width, UpButtonSource.Height),
				null, null, Game1.mouseCursors, UpButtonSource, 1f, true);
			_navRightButton = new ClickableTextureComponent(
				"navRight", new Rectangle(-1, -1, RightButtonSource.Width, RightButtonSource.Height),
				null, null, Game1.mouseCursors, RightButtonSource, 1f, true);
			_navLeftButton = new ClickableTextureComponent(
				"navLeft", new Rectangle(-1, -1, LeftButtonSource.Width, LeftButtonSource.Height),
				null, null, Game1.mouseCursors, LeftButtonSource, 1f, true);
			_cookButton = new ClickableComponent(Rectangle.Empty, "cook");
			_cookQuantityUpButton = new ClickableTextureComponent(
				"quantityUp", new Rectangle(-1, -1, PlusButtonSource.Width * Scale, PlusButtonSource.Height * Scale),
				null, null, Game1.mouseCursors, PlusButtonSource, Scale, true);
			_cookQuantityDownButton = new ClickableTextureComponent(
				"quantityDown", new Rectangle(-1, -1, MinusButtonSource.Width * Scale, MinusButtonSource.Height * Scale),
				null, null, Game1.mouseCursors, MinusButtonSource, Scale, true);
			_cookConfirmButton = new ClickableTextureComponent(
				"confirm", new Rectangle(-1, -1, OkButtonSource.Width, OkButtonSource.Height),
				null, null, Game1.mouseCursors, OkButtonSource, 1f, true);
			_cookCancelButton = new ClickableTextureComponent(
				"cancel", new Rectangle(-1, -1, NoButtonSource.Width, NoButtonSource.Height),
				null, null, Game1.mouseCursors, NoButtonSource, 1f, true);
			_toggleFilterButton = new ClickableTextureComponent(
				"toggleFilter", new Rectangle(-1, -1, ToggleFilterButtonSource.Width * Scale, ToggleFilterButtonSource.Height * Scale),
				null, i18n.Get("menu.cooking_search.filter_label"),
				Texture, ToggleFilterButtonSource, Scale, true);
			_toggleOrderButton = new ClickableTextureComponent(
				"toggleOrder", new Rectangle(-1, -1, ToggleOrderButtonSource.Width * Scale, ToggleOrderButtonSource.Height * Scale),
				null, i18n.Get("menu.cooking_search.order_label"),
				Texture, ToggleOrderButtonSource, Scale, true);
			_toggleViewButton = new ClickableTextureComponent(
				"toggleView", new Rectangle(-1, -1, ToggleViewButtonSource.Width * Scale, ToggleViewButtonSource.Height * Scale),
				null, i18n.Get("menu.cooking_search.view."
				               + (ModEntry.Instance.States.Value.IsUsingRecipeGridView ? "grid" : "list")),
				Texture, ToggleViewButtonSource, Scale, true);
			_searchButton = new ClickableTextureComponent(
				"search", new Rectangle(-1, -1, SearchButtonSource.Width * Scale, SearchButtonSource.Height * Scale),
				null, i18n.Get("menu.cooking_recipe.search_label"),
				Texture, SearchButtonSource, Scale, true);
			_recipeIconButton = new ClickableTextureComponent(
				"recipeIcon", new Rectangle(-1, -1, 64, 64),
				null, null,
				Game1.objectSpriteSheet, new Rectangle(0, 0, 64, 64), Scale, true);
			_autofillButton = new ClickableTextureComponent(
				"autofill", new Rectangle(-1, -1, 64, 64),
				null, null,
				Texture, AutofillButtonSource, Scale, true);
			_searchBarClickable = new ClickableComponent(Rectangle.Empty, "searchbox");
			_searchTabButton = new ClickableTextureComponent(
				"searchTab", new Rectangle(-1, -1, SearchTabButtonSource.Width * Scale, SearchTabButtonSource.Height * Scale),
				null, null, Texture, SearchTabButtonSource, Scale, true);
			_ingredientsTabButton = new ClickableTextureComponent(
				"ingredientsTab", new Rectangle(-1, -1, IngredientsTabButtonSource.Width * Scale, IngredientsTabButtonSource.Height * Scale),
				null, null, Texture, IngredientsTabButtonSource, Scale, true);
			for (int i = (int) Filter.Alphabetical; i < (int) Filter.Count; ++i)
			{
				_filterButtons.Add(new ClickableTextureComponent(
					$"filter{i}", new Rectangle(-1, -1, FilterIconSource.Width * Scale, FilterIconSource.Height * Scale),
					null, i18n.Get($"menu.cooking_search.filter.{i}"
						+ (Config.HideFoodBuffsUntilEaten && i == 4 ? "_alt" : "")),
					Texture, new Rectangle(
						FilterIconSource.X + (i - 1) * FilterIconSource.Width, FilterIconSource.Y,
						FilterIconSource.Width, FilterIconSource.Height),
					Scale));
			}

			_searchBarTextBox = new TextBox(
				Game1.content.Load<Texture2D>("LooseSprites\\textBox"),
				null, Game1.smallFont, Game1.textColor)
			{
				textLimit = 32,
				Selected = false,
				Text = i18n.Get("menu.cooking_recipe.search_label"),
			};
			_quantityTextBox = new TextBox(
				Game1.content.Load<Texture2D>("LooseSprites\\textBox"),
				null, Game1.smallFont, Game1.textColor)
			{
				numbersOnly = true,
				textLimit = 2,
				Selected = false,
				Text = QuantityTextBoxDefaultText,
			};

			_quantityTextBox.OnEnterPressed += ValidateNumericalTextBox;
			_searchBarTextBox.OnEnterPressed += sender => { CloseTextBox(sender); };

			for (int i = 0; i < 5; ++i)
			{
				_searchListClickables.Add(new ClickableComponent(new Rectangle(-1, -1, -1, -1), "searchList" + i));
			}
			for (int i = 0; i < 16; ++i)
			{
				_searchGridClickables.Add(new ClickableComponent(new Rectangle(-1, -1, -1, -1), "searchGrid" + i));
			}

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

			// Determine extra inventories:
			_inventoryId = BackpackInventoryId;
			// Add player inventory
			_allInventories.Add(Game1.player.Items);
			if (Game1.currentLocation is CommunityCenter cc)
			{
				// Check to recognise community centre fridge
				if (!cc.Objects.ContainsKey(Bundles.FridgeChestPosition))
				{
					cc.Objects.Add(Bundles.FridgeChestPosition, new Chest(true, Bundles.FridgeChestPosition));
				}
				if (Bundles.IsCommunityCentreKitchenComplete())
				{
					// Add fridge inventory
					_allInventories.Add(((Chest)(cc.Objects[Bundles.FridgeChestPosition])).items);
				}
			}
			if (Game1.currentLocation is FarmHouse farmHouse && ModEntry.Instance.GetFarmhouseKitchenLevel(farmHouse) > 0)
			{
				// Recognise farmhouse fridge
				_allInventories.Add(farmHouse.fridge.Value.items);
			}
			if (Game1.currentLocation is IslandFarmHouse islandFarmHouse)
			{
				// Recognise island farmhouse fridge
				_allInventories.Add(islandFarmHouse.fridge.Value.items);
			}
			if (_allInventories.Count > 0)
			{
				// Check for minifridges
				_allInventories.AddRange(Game1.currentLocation.Objects.Values.Where(o => o != null && o.bigCraftable.Value && o is Chest && o.ParentSheetIndex == 216)
					.Select(o => ((Chest)o).items).Take(MaximumMiniFridges).Cast<IList<Item>>().ToList());
				for (int i = 0; i < _allInventories.Count - InventoryIdsBeforeMinifridges; ++i)
				{
					_inventorySelectButtons.Add(new ClickableTextureComponent($"minifridgeSelect{i}",
						new Rectangle(-1, -1, 16 * Scale, 16 * Scale), null, null,
						ModEntry.SpriteSheet, new Rectangle(243, 114, 11, 14), Scale, false));
				}
			}
			// Populate list of inventories
			if (_allInventories.Count > 1)
			{
				_inventorySelectButtons.Insert(0, new ClickableTextureComponent("fridgeSelect",
					new Rectangle(-1, -1, 14 * Scale, 14 * Scale), null, null,
					ModEntry.SpriteSheet, new Rectangle(243, 97, 11, 14), Scale, false));
				_inventorySelectButtons.Insert(0, new ClickableTextureComponent("inventorySelect",
					new Rectangle(-1, -1, 14 * Scale, 14 * Scale), null, null,
					ModEntry.SpriteSheet, new Rectangle(243, 81, 11, 14), Scale, false));
			}

			// Setup menu elements layout
			this.RealignElements();
			this.InitialiseControllerFlow();

			if (addDummyState)
				_stack.Push(State.Opening);
			this.OpenSearchPage();

			// Apply previously-used filter
			if (ModEntry.Instance.States.Value.LastFilterThisSession != Filter.None)
			{
				_recipesFiltered = this.FilterRecipes(ModEntry.Instance.States.Value.LastFilterThisSession);
			}
			if (reverseDefaultFilter)
			{
				_recipesFiltered = this.ReverseRecipeList(_recipesFiltered);
			}
			this.UpdateSearchRecipes();

			// Open to a starting recipe if needed
			if (!string.IsNullOrEmpty(initialRecipe))
			{
				this.ChangeCurrentRecipe(initialRecipe);
				this.OpenRecipePage();
			}

			Game1.displayHUD = false;
		}

		public override void emergencyShutDown()
		{
			exitFunction();
			base.emergencyShutDown();
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
				_toggleOrderButton,
				_toggleFilterButton,
				_toggleViewButton,
				_searchButton,
				_recipeIconButton,
				_autofillButton,
				_searchBarClickable,
			});
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
			_searchTabButton.downNeighborID = IsIngredientsPageEnabled
				? _ingredientsTabButton.myID
				: IsAutofillEnabled
					? _autofillButton.myID
					: InventoryRegionStartID;

			_autofillButton.rightNeighborID = InventoryRegionStartID;

			_searchBarClickable.rightNeighborID = _toggleFilterButton.myID;

			_toggleFilterButton.leftNeighborID = _searchBarClickable.myID;
			_toggleFilterButton.rightNeighborID = _toggleOrderButton.myID;

			_toggleOrderButton.leftNeighborID = _toggleFilterButton.myID;
			_toggleOrderButton.rightNeighborID = _toggleViewButton.myID;

			_toggleViewButton.leftNeighborID = _toggleOrderButton.myID;
			_toggleViewButton.rightNeighborID = _ingredientsClickables[0].myID;

			_ingredientsClickables.Last().rightNeighborID = upperRightCloseButton.myID;
			upperRightCloseButton.leftNeighborID = _ingredientsClickables.Last().myID;
			upperRightCloseButton.downNeighborID = _ingredientsClickables.Last().myID;

			_recipeIconButton.downNeighborID = _navLeftButton.downNeighborID = _navRightButton.downNeighborID = InventoryRegionStartID;
			_navLeftButton.leftNeighborID = _searchTabButton.myID;
			_navLeftButton.rightNeighborID = _recipeIconButton.myID;
			_navRightButton.leftNeighborID = _recipeIconButton.myID;
			_navRightButton.rightNeighborID = _ingredientsClickables.First().myID;

			_navUpButton.upNeighborID = _toggleFilterButton.myID;
			_navDownButton.downNeighborID = InventoryRegionStartID;

			_cookButton.upNeighborID = _ingredientsClickables.First().myID;
			_cookButton.downNeighborID = InventoryRegionStartID;

			_cookConfirmButton.leftNeighborID = _cookQuantityUpButton.myID;
			_cookCancelButton.leftNeighborID = _cookQuantityDownButton.myID;
			_cookQuantityUpButton.rightNeighborID = _cookQuantityDownButton.rightNeighborID = _cookConfirmButton.myID;
			_cookQuantityUpButton.downNeighborID = _cookQuantityDownButton.myID;
			_cookQuantityDownButton.upNeighborID = _cookQuantityUpButton.myID;
			_cookConfirmButton.upNeighborID = _cookQuantityUpButton.upNeighborID = _ingredientsClickables.First().myID;
			_cookCancelButton.upNeighborID = _cookConfirmButton.myID;
			_cookConfirmButton.downNeighborID = _cookCancelButton.myID;
			_cookCancelButton.downNeighborID = _cookQuantityDownButton.downNeighborID = InventoryRegionStartID;

			// Child component navigation
			foreach (List<ClickableTextureComponent> clickableGroup in new [] { _ingredientsClickables, _inventorySelectButtons, _filterButtons })
			{
				for (int i = 0; i < clickableGroup.Count; ++i)
				{
					if (i > 0)
						clickableGroup[i].leftNeighborID = clickableGroup[i - 1].myID;
					if (i < clickableGroup.Count - 1)
						clickableGroup[i].rightNeighborID = clickableGroup[i + 1].myID;
				}
			}
			for (int i = 0; i < _searchListClickables.Count; ++i)
			{
				if (i > 0)
					_searchListClickables[i].upNeighborID = _searchListClickables[i - 1].myID;
				if (i < _searchListClickables.Count - 1)
					_searchListClickables[i].downNeighborID = _searchListClickables[i + 1].myID;
			}
			_searchListClickables.First().upNeighborID = _toggleFilterButton.myID;
			_searchListClickables.Last().downNeighborID = InventoryRegionStartID;
			for (int i = 0; i < _searchGridClickables.Count; ++i)
			{
				if (i > 0 && i % 4 != 0)
					_searchGridClickables[i].leftNeighborID = _searchGridClickables[i - 1].myID;
				if (i < _searchGridClickables.Count - 1)
					_searchGridClickables[i].rightNeighborID = _searchGridClickables[i + 1].myID;

				_searchGridClickables[i].upNeighborID = i < 4
					? _toggleFilterButton.myID
					: _searchGridClickables[i - 4].myID;
				_searchGridClickables[i].downNeighborID = i > _searchGridClickables.Count - 1 - 4
					? InventoryRegionStartID
					: _searchGridClickables[i + 4].myID;
			}
			foreach (ClickableTextureComponent clickable in _ingredientsClickables)
			{
				clickable.downNeighborID = InventoryRegionStartID;
			}
			if (_inventorySelectButtons.Count > 1)
			{
				_inventorySelectButtons[0].leftNeighborID = UseHorizontalInventoryButtonArea
					? -1
					: this.GetColumnCount() - 1; // last element in the first row of the inventory
				_inventorySelectButtons[0].upNeighborID = UseHorizontalInventoryButtonArea
					? this.GetColumnCount() * 2 // first element in the last row of the inventory
					: _inventorySelectButtons[1].upNeighborID = _ingredientsClickables.Last().myID; // last ingredient slot
			}

			// Probably does nothing
			this.setUpForGamePadMode();

			// Add clickables to implicit navigation
			this.populateClickableComponentList();

			Helper.Events.GameLoop.UpdateTicked += this.Event_SnapOnOpen;
		}

		private void Event_SnapOnOpen(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
		{
			Helper.Events.GameLoop.UpdateTicked -= this.Event_SnapOnOpen;

			if (Game1.options.gamepadControls || Game1.options.SnappyMenus)
			{
				this.snapToDefaultClickableComponent();
			}
		}

		private void RealignElements()
		{
			Vector2 centre = Utility.PointToVector2(Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Center);

			int yOffset = 0;
			int xOffset = 0;

			// Menu
			{
				yOffset = 216;
				if (Context.IsSplitScreen)
				{
					centre.X /= 2;
				}
				if (UseHorizontalInventoryButtonArea)
				{
					yOffset /= 2;
				}
				yPositionOnScreen = (int)(centre.Y - CookbookSource.Center.Y * Scale * Game1.options.uiScale + yOffset);
				xPositionOnScreen = (int)(centre.X - CookbookSource.Center.X * Scale * Game1.options.uiScale + xOffset);
			}

			// Cookbook menu
			_cookbookLeftRect.X = xPositionOnScreen;
			_cookbookRightRect.X = _cookbookLeftRect.X + _cookbookLeftRect.Width;
			_cookbookLeftRect.Y = _cookbookRightRect.Y = yPositionOnScreen;

			_leftContent = new Point(_cookbookLeftRect.X + MarginLeft, _cookbookLeftRect.Y);
			_rightContent = new Point(_cookbookRightRect.X + MarginRight, _cookbookRightRect.Y);

			_lineWidth = _cookbookLeftRect.Width - MarginLeft * 3 / 2; // Actually mostly even for both Left and Right pages
			_textWidth = _lineWidth + TextMuffinTopOverDivider * 2;

			// Extra clickables
			upperRightCloseButton.bounds.Y = yPositionOnScreen + 32;
			upperRightCloseButton.bounds.X = xPositionOnScreen + CookbookSource.Width * Scale - 12;

			if (Context.IsSplitScreen)
			{
				int pos = upperRightCloseButton.bounds.X + upperRightCloseButton.bounds.Width;
				int bound = Game1.viewport.Width / 2;
				float scale = Game1.options.uiScale;
				float diff = (pos - bound) * scale;
				upperRightCloseButton.bounds.X -= (int)Math.Max(0, diff / 2);
			}

			// Search elements
			_searchTabButton.bounds.Y = _cookbookLeftRect.Y + 72;
			_ingredientsTabButton.bounds.Y = _searchTabButton.bounds.Y + _searchTabButton.bounds.Height + 16;
			_searchTabButton.bounds.X = _ingredientsTabButton.bounds.X = _cookbookLeftRect.X - 40;

			yOffset = 32;
			xOffset = 40;
			int xOffsetExtra = 8;

			_searchBarTextBox.X = _leftContent.X;
			_searchBarTextBox.Y = _leftContent.Y + yOffset + 10;
			_searchBarTextBox.Width = 160;
			_searchBarTextBox.Selected = false;
			_searchBarTextBox.Text = i18n.Get("menu.cooking_recipe.search_label");
			_searchBarTextBox.Update();
			_searchBarTextBoxBounds = new Rectangle(
				_searchBarTextBox.X, _searchBarTextBox.Y, _searchBarTextBox.Width, _searchBarTextBox.Height);
			_searchBarClickable.bounds = _searchBarTextBoxBounds;

			_toggleFilterButton.bounds.X = _cookbookRightRect.X
			                              - _toggleFilterButton.bounds.Width
			                              - _toggleOrderButton.bounds.Width
			                              - _toggleViewButton.bounds.Width
			                              - xOffsetExtra * 3 - 24;
			_toggleOrderButton.bounds.X = _toggleFilterButton.bounds.X + _toggleFilterButton.bounds.Width + xOffsetExtra;
			_toggleViewButton.bounds.X = _toggleOrderButton.bounds.X + _toggleOrderButton.bounds.Width + xOffsetExtra;
			_toggleViewButton.bounds.Y = _toggleOrderButton.bounds.Y = _toggleFilterButton.bounds.Y = _leftContent.Y + yOffset;
			
			_toggleViewButton.sourceRect.X = ToggleViewButtonSource.X + (ModEntry.Instance.States.Value.IsUsingRecipeGridView
				? ToggleViewButtonSource.Width : 0);

			_searchButton.bounds = _toggleViewButton.bounds;
			_searchBarTextBoxMaxWidth = _searchButton.bounds.X - _searchBarTextBox.X - 24;

			int minWidth = 132;
			_searchBarTextBoxMinWidth = Math.Min(_toggleFilterButton.bounds.X - _searchBarTextBoxBounds.X,
				Math.Max(minWidth, 24 + (int)Math.Ceiling(Game1.smallFont.MeasureString(_searchBarTextBox.Text).X)));

			_navUpButton.bounds.X = _navDownButton.bounds.X = _searchButton.bounds.X;
			_navUpButton.bounds.Y = _searchButton.bounds.Y + _searchButton.bounds.Height + 16;
			_navDownButton.bounds.Y = _cookbookLeftRect.Bottom - 128;
			
			_searchResultsArea = new Rectangle(
				_searchBarTextBox.X,
				_navUpButton.bounds.Y - 8,
				_navUpButton.bounds.X - _searchBarTextBox.X - 16,
				_navDownButton.bounds.Y + _navDownButton.bounds.Height - _navUpButton.bounds.Y + 16);

			// Recipe search results
			{
				int x, y;

				//SearchResultsArea.Y = NavUpButton.bounds.Y - 8;
				//SearchResultsArea.Height = NavDownButton.bounds.Y + NavDownButton.bounds.Height - NavUpButton.bounds.Y + 16;

				int gridColumns = _searchResultsArea.Width / RecipeGridHeight;
				yOffset = (_searchResultsArea.Height % RecipeGridHeight) / 2;
				for (int i = 0; i < _searchGridClickables.Count; ++i)
				{
					y = _searchResultsArea.Y + yOffset + (i / gridColumns) * RecipeGridHeight + (RecipeGridHeight - 16 * Scale) / 2;
					x = _searchResultsArea.X + (i % gridColumns) * RecipeGridHeight;
					_searchGridClickables[i].bounds = new Rectangle(x, y, 16 * Scale, 16 * Scale);
				}

				x = _searchResultsArea.X;
				yOffset = (_searchResultsArea.Height % RecipeListHeight) / 2;
				for (int i = 0; i < _searchListClickables.Count; ++i)
				{
					y = _searchResultsArea.Y + yOffset + i * RecipeListHeight + (RecipeListHeight - 64) / 2;
					_searchListClickables[i].bounds = new Rectangle(x, y, _searchResultsArea.Width, -1);
				}
				foreach (ClickableComponent clickable in _searchListClickables)
				{
					clickable.bounds.Height = _searchListClickables[_searchListClickables.Count - 1].bounds.Y
						- _searchListClickables[_searchListClickables.Count - 2].bounds.Y;
				}
			}

			yOffset = 24;
			for (int i = 0; i < _filterButtons.Count; ++i)
			{
				_filterButtons[i].bounds.X = _cookbookRightRect.X - xOffset - 4 - (_filterButtons.Count - i)
					* _filterButtons[i].bounds.Width;
				_filterButtons[i].bounds.Y = _toggleFilterButton.bounds.Y + _toggleFilterButton.bounds.Height + yOffset;
			}

			_filterContainerMiddleWidth = _filterButtons.Count * FilterIconSource.Width;
			_filterContainerBounds = new Rectangle(
				_filterButtons[0].bounds.X - FilterContainerSideWidth * Scale - 4,
				_filterButtons[0].bounds.Y - (FilterContainerSource.Height - FilterIconSource.Height) * Scale / 2,
				(FilterContainerSideWidth * 2 + _filterContainerMiddleWidth) * Scale,
				FilterContainerSource.Height * Scale);

			// Ingredient slots buttons
			{
				const int slotsPerRow = 3;
				int w = _ingredientsClickables[0].bounds.Width;
				int h = _ingredientsClickables[0].bounds.Height;
				yOffset = 36;
				xOffset = 0;
				xOffsetExtra = 0;
				int extraSpace = (int)(w / 2f * (_ingredientsClickables.Count % slotsPerRow) / 2f);
				for (int i = 0; i < _ingredientsClickables.Count; ++i)
				{
					xOffset += w;
					if (i % slotsPerRow == 0)
					{
						if (i != 0)
							yOffset += h;
						xOffset = 0;
					}

					if (i == _ingredientsClickables.Count - (_ingredientsClickables.Count % slotsPerRow))
						xOffsetExtra = extraSpace;

					_ingredientsClickables[i].bounds.X = _rightContent.X + xOffset + xOffsetExtra + 16;
					_ingredientsClickables[i].bounds.Y = _rightContent.Y + yOffset;
				}
			}

			// Recipe nav buttons
			_navLeftButton.bounds.X = _leftContent.X - 24;
			_navRightButton.bounds.X = _navLeftButton.bounds.X + _lineWidth - 12;
			_navRightButton.bounds.Y = _navLeftButton.bounds.Y = _leftContent.Y + 23;

			// Recipe icon
			_recipeIconButton.bounds.Y = _navLeftButton.bounds.Y + 4;
			_recipeIconButton.bounds.X = _navLeftButton.bounds.X + _navLeftButton.bounds.Width;

			// Cook! button
			xOffset = _rightContent.X + _cookbookRightRect.Width / 2 - MarginRight;
			yOffset = _rightContent.Y + 344;
			_cookTextMiddleWidth = Math.Max(32, CookTextSource[_locale].Width);
			_cookButton.bounds = new Rectangle(
				xOffset, yOffset,
				CookTextSideWidth * Scale * 2 + _cookTextMiddleWidth * Scale,
				CookButtonSource.Height * Scale);
			_cookButton.bounds.X -= (CookTextSourceWidths[_locale] / 2 * Scale - CookTextSideWidth * Scale) + MarginLeft;

			// Cooking confirmation popup buttons
			{
				xOffset -= 160;
				yOffset -= 36;
				_cookIconBounds = new Rectangle(xOffset, yOffset + 6, 90, 90);

				xOffset += 48 + _cookIconBounds.Width;
				_cookQuantityUpButton.bounds.X = _cookQuantityDownButton.bounds.X = xOffset;
				_cookQuantityUpButton.bounds.Y = yOffset - 12;

				Vector2 textSize = _quantityTextBox.Font.MeasureString(
					Game1.parseText("999", _quantityTextBox.Font, 96));
				_quantityTextBox.Text = QuantityTextBoxDefaultText;
				_quantityTextBox.limitWidth = false;
				_quantityTextBox.Width = (int)textSize.X + 24;

				int extraSpace = (_quantityTextBox.Width - _cookQuantityUpButton.bounds.Width) / 2;
				_quantityTextBox.X = _cookQuantityUpButton.bounds.X - extraSpace;
				_quantityTextBox.Y = _cookQuantityUpButton.bounds.Y + _cookQuantityUpButton.bounds.Height + 7;
				_quantityTextBox.Update();
				_quantityTextBoxBounds = new Rectangle(_quantityTextBox.X, _quantityTextBox.Y, _quantityTextBox.Width,
						_quantityTextBox.Height);

				_cookQuantityDownButton.bounds.Y = _quantityTextBox.Y + _quantityTextBox.Height + 5;

				_cookConfirmButton.bounds.X = _cookCancelButton.bounds.X
					= _cookQuantityUpButton.bounds.X + _cookQuantityUpButton.bounds.Width + extraSpace + 16;
				_cookConfirmButton.bounds.Y = yOffset - 16;
				_cookCancelButton.bounds.Y = _cookConfirmButton.bounds.Y + _cookConfirmButton.bounds.Height + 4;

				_quantityScrollableArea = new Rectangle(_cookIconBounds.X, _cookIconBounds.Y,
					_cookConfirmButton.bounds.X + _cookConfirmButton.bounds.Width - _cookIconBounds.X, _cookIconBounds.Height);
			}

			// Inventory
			bool isHorizontal = UseHorizontalInventoryButtonArea;
			inventory.xPositionOnScreen = xPositionOnScreen + CookbookSource.Width / 2 * Scale - inventory.width / 2 + (isHorizontal ? 16 : 0);
			inventory.yPositionOnScreen = yPositionOnScreen + CookbookSource.Height * Scale + 8 - 20;

			// Inventory items
			{
				yOffset = -4;
				int rowSize = inventory.capacity / inventory.rows;
				for (int i = 0; i < inventory.capacity; ++i)
				{
					if (i % rowSize == 0 && i != 0)
						yOffset += inventory.inventory[i].bounds.Height + 4;
					inventory.inventory[i].bounds.X = inventory.xPositionOnScreen + i % rowSize * inventory.inventory[i].bounds.Width;
					inventory.inventory[i].bounds.Y = inventory.yPositionOnScreen + yOffset;
				}
			}

			// Inventory nav buttons
			// nav buttons flow vertically in a solo-screen game, and horizontally in split-screen
			if (_inventorySelectButtons.Count > 1)
			{
				const int areaPadding = 3 * Scale;
				int itemSpacing = 4 * Scale;
				int addedSpacing = 2 * Scale;

				// Backpack and fridge
				{
					_inventorySelectButtons[0].bounds.X =
						(isHorizontal
							? (int)(centre.X + 128 - (((_inventorySelectButtons.Count + 1) / 2) * ((_inventorySelectButtons[0].bounds.Width + 4) / 2)))
							: upperRightCloseButton.bounds.X);
					_inventorySelectButtons[1].bounds.X = _inventorySelectButtons[0].bounds.X
						+ (isHorizontal
							? 0
							: _inventorySelectButtons[0].bounds.Width);

					_inventorySelectButtons[0].bounds.Y = inventory.yPositionOnScreen + 1 * Scale
						+ (isHorizontal
							? inventory.height + itemSpacing + addedSpacing
							: (3 - 2 * ((_inventorySelectButtons.Count + 1) / 2) / 2) * _inventorySelectButtons[0].bounds.Height / 2);
					_inventorySelectButtons[1].bounds.Y = _inventorySelectButtons[0].bounds.Y
						+ (isHorizontal
							? _inventorySelectButtons[0].bounds.Height + itemSpacing
							: 0);
				}

				// Mini-fridges
				for (int i = InventoryIdsBeforeMinifridges; i < _inventorySelectButtons.Count; ++i)
				{
					int shortSideIndex = i % 2;
					int shortSidePlacement = 0;
					int longSideIndex = 0;
					int longSidePlacement = i / 2;
					_inventorySelectButtons[i].bounds.X =
						_inventorySelectButtons[(isHorizontal ? longSideIndex : shortSideIndex)].bounds.X
						+ (_inventorySelectButtons[0].bounds.Width * (isHorizontal ? longSidePlacement : shortSidePlacement)) + (isHorizontal ? addedSpacing : 0);
					_inventorySelectButtons[i].bounds.Y =
						_inventorySelectButtons[(isHorizontal ? shortSideIndex : longSideIndex)].bounds.Y
						+ (_inventorySelectButtons[0].bounds.Height * (isHorizontal ? shortSidePlacement : longSidePlacement)) + (isHorizontal ? 0 : addedSpacing);
				}

				// Rectangle only exists to track user scrollwheel actions
				int longSideLength = 2 * ((_inventorySelectButtons.Count + 1) / 2) / 2;
				int wideSideLength = (_inventorySelectButtons.Count > 1 ? 2 : 1);
				int xLength = (isHorizontal ? longSideLength : wideSideLength);
				int yLength = (isHorizontal ? wideSideLength : longSideLength);
				_inventoriesScrollableArea = new Rectangle(
					_inventorySelectButtons[0].bounds.X - areaPadding,
					_inventorySelectButtons[0].bounds.Y - areaPadding,
					(_inventorySelectButtons[0].bounds.Width + 4) * xLength + addedSpacing,
					(_inventorySelectButtons[0].bounds.Height + itemSpacing) * yLength + (isHorizontal ? addedSpacing : 0));
			}
			
			_autofillButton.bounds.Y = inventory.yPositionOnScreen + (inventory.height - _autofillButton.bounds.Height) / 2 - 8;
			_autofillButton.bounds.X = inventory.xPositionOnScreen - _autofillButton.bounds.Width - 48;
			_autofillButton.sourceRect.X = IsUsingAutofill
				? AutofillButtonSource.X + AutofillButtonSource.Width
				: AutofillButtonSource.X;
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
			bool isBaked = ModEntry.ItemDefinitions["BakeyFoods"].Any(o => name.StartsWith(o) || ModEntry.ItemDefinitions["CakeyFoods"].Any(o => name.EndsWith(o)));
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
				spritePosition = new Vector2(Game1.player.Position.X, Game1.player.Position.Y - 40 * Game1.pixelZoom);
				sprite = new TemporaryAnimatedSprite(
						textureName: ModEntry.GameContentSpriteSheetPath,
						sourceRect: new Rectangle(0, 336, 16, 48),
						animationInterval: ms, animationLength: 16, numberOfLoops: 0,
						position: spritePosition,
						flicker: false, flipped: false)
				{
					scale = Game1.pixelZoom,
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
				spritePosition = new Vector2(Game1.player.Position.X, Game1.player.Position.Y - 40 * Game1.pixelZoom);
				sprite = new TemporaryAnimatedSprite(
						textureName: ModEntry.GameContentSpriteSheetPath,
						sourceRect: new Rectangle(0, 288, 16, 48),
						animationInterval: ms, animationLength: 16, numberOfLoops: 0,
						position: spritePosition,
						flicker: false, flipped: false)
						{
							scale = Game1.pixelZoom,
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
				spritePosition = new Vector2(Game1.player.Position.X, Game1.player.Position.Y - 40 * Game1.pixelZoom);
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
			_recipesFiltered = this.FilterRecipes();
			_showSearchFilters = false;
			_searchBarTextBox.Text = i18n.Get("menu.cooking_recipe.search_label");

			if (Game1.options.SnappyMenus)
			{
				this.setCurrentlySnappedComponentTo(_recipeSearchResults.Count > 0
					? ModEntry.Instance.States.Value.IsUsingRecipeGridView
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

			if (Game1.options.SnappyMenus)
			{
				this.setCurrentlySnappedComponentTo(
					IsUsingAutofill
					? ReadyToCook
						? _cookButton.myID
						: InventoryRegionStartID
					: InventoryRegionStartID);
			}
		}

		private void CloseRecipePage()
		{
			if (_stack.Count > 0 && _stack.Peek() == State.Recipe)
				_stack.Pop();

			_searchTabButton.sourceRect.X = SearchTabButtonSource.X + SearchTabButtonSource.Width;
			_ingredientsTabButton.sourceRect.X = IngredientsTabButtonSource.X;
			this.KeepRecipeIndexInSearchBounds();

			if (Game1.options.SnappyMenus)
			{
				this.setCurrentlySnappedComponentTo(ModEntry.Instance.States.Value.IsUsingRecipeGridView
					? _searchGridClickables[_recipeSearchResults.Count / 2].myID
					: _searchListClickables[_recipeSearchResults.Count / 2].myID);
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

		private void CloseTextBox(TextBox textBox)
		{
			textBox.Selected = false;
			Game1.keyboardDispatcher.Subscriber = null;

			if (textBox.Text == _searchBarTextBox.Text)
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
			ModEntry.Instance.States.Value.LastFilterReversed = false;
			Func<CraftingRecipe, object> order = null;
			Func<CraftingRecipe, bool> filter = null;
			switch (which)
			{
				case Filter.Energy:
					order = recipe => recipe.createItem().staminaRecoveredOnConsumption();
					break;
				case Filter.Gold:
					order = recipe => recipe.createItem().salePrice();
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
				? _recipesAvailable.OrderBy(order)
				: _recipesAvailable.Where(filter)).ToList();
			if (!string.IsNullOrEmpty(substr) && substr != i18n.Get("menu.cooking_recipe.search_label"))
			{
				substr = substr.ToLower();
				recipes = recipes.Where(recipe => recipe.DisplayName.ToLower().Contains(substr)).ToList();
			}

			if (recipes.Count < 1)
				recipes.Add(new CraftingRecipe("none", true));

			if (_recipeSearchResults != null)
			{
				this.UpdateSearchRecipes();
				_recipeIndex = _recipeSearchResults.Count / 2;
			}

			_lastFilterUsed = which;

			return recipes;
		}

		private void UpdateSearchRecipes()
		{
			_navUpButton.bounds.Y = _showSearchFilters
				? _searchButton.bounds.Y + _searchButton.bounds.Height + 16 + _filterContainerBounds.Height
				: _searchButton.bounds.Y + _searchButton.bounds.Height + 16;

			_recipeSearchResults.Clear();
			_searchResultsArea.Y = _navUpButton.bounds.Y - 8;
			_searchResultsArea.Height = _navDownButton.bounds.Y + _navDownButton.bounds.Height - _navUpButton.bounds.Y + 16;

			bool isGridView = ModEntry.Instance.States.Value.IsUsingRecipeGridView;
			_recipeDisplayHeight = isGridView
				? RecipeGridHeight
				: RecipeListHeight;
			_searchResultsPerPage = isGridView
				? (_searchResultsArea.Width / _recipeDisplayHeight) * (_searchResultsArea.Height / _recipeDisplayHeight)
				: _searchResultsArea.Height / _recipeDisplayHeight;
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

			foreach (ClickableComponent clickable in _searchGridClickables)
				clickable.bounds.Y += (_showSearchFilters ? 1 : -1) * clickable.bounds.Height;
			foreach (ClickableComponent clickable in _searchListClickables)
				clickable.bounds.Y += (_showSearchFilters ? 1 : -1) * clickable.bounds.Height;

			if (Game1.options.SnappyMenus)
			{
				this.setCurrentlySnappedComponentTo(_showSearchFilters ? _filterButtons[0].myID : _toggleFilterButton.myID);
			}
		}

		private void ValidateNumericalTextBox(TextBox sender)
		{
			int.TryParse(sender.Text.Trim(), out int value);
			value = value > 0 ? value : 1;
			sender.Text = Math.Max(1, Math.Min(99,
				Math.Min(value, _recipeReadyToCraftCount))).ToString();
			sender.Text = sender.Text.PadLeft(sender.Text.Length == 2 ? 3 : 2, ' ');
			sender.Selected = false;
		}

		private void KeepRecipeIndexInSearchBounds()
		{
			_recipeIndex = Math.Max(_recipeSearchResults.Count / 2,
				Math.Min(_recipesFiltered.Count - _recipeSearchResults.Count / 2 - 1, _recipeIndex));

			// Avoid showing whitespace after end of list
			if (ModEntry.Instance.States.Value.IsUsingRecipeGridView)
			{
				_recipeIndex = 4 * (_recipeIndex / 4) + 4;
				if (_recipesFiltered.Count - 1 - _recipeIndex < _recipeSearchResults.Count / 2)
				{
					_recipeIndex -= 4;
				}
			}
			else
			{
				if (_recipesFiltered.Count - _recipeIndex <= (_recipeSearchResults.Count + 1) / 2)
					--_recipeIndex;
			}
		}

		private void ChangeCurrentRecipe(int index)
		{
			if (_recipesFiltered.Count == 0)
				return;
			index = Math.Max(0, Math.Min(_recipesFiltered.Count - 1, index));
			this.ChangeCurrentRecipe(_recipesFiltered[index].name);
		}

		private void ChangeCurrentRecipe(string name)
		{
			if (_recipesFiltered.Count == 0)
				return;
			CraftingRecipe recipe = new CraftingRecipe(name, isCookingRecipe: true);
			_recipeIndex = _recipesFiltered.FindIndex(recipe => recipe.name == name);
			_recipeAsItem = recipe.createItem();
			string[] info = Game1.objectInformation[_recipeAsItem.ParentSheetIndex].Split('/');
			List<int> buffs = info.Length >= 7
				? info[7].Split(' ').ToList().ConvertAll(int.Parse)
				: null;
			_recipeBuffs = buffs != null && !buffs.All(b => b == 0)
				? buffs
				: null;
			_recipeBuffDuration = _recipeBuffs != null && info.Length >= 8
				? (int.Parse(info[8]) * 7 / 10 / 10) * 10
				: -1;
			this.TryAutoFillIngredients();
			this.UpdateCraftableCounts(recipe: recipe);
		}

		private void UpdateCraftableCounts(CraftingRecipe recipe)
		{
			_recipeIngredientQuantitiesHeld.Clear();
			for (int i = 0; i < CurrentRecipe?.getNumberOfIngredients(); ++i)
			{
				int id = CurrentRecipe.recipeList.Keys.ElementAt(i);
				int requiredQuantity = CurrentRecipe.recipeList.Values.ElementAt(i);
				int heldQuantity = 0;
				List<CookingManager.Ingredient> ingredients = CookingManager.GetMatchingIngredients(id: id, sourceItems: _allInventories, required: requiredQuantity);
				if (ingredients != null && ingredients.Count > 0)
				{
					heldQuantity = ingredients.Sum(ing => _cookingManager.GetItemForIngredient(ingredient: ing, sourceItems: _allInventories).Stack);
					requiredQuantity -= heldQuantity;
				}

				_recipeIngredientQuantitiesHeld.Add(heldQuantity);
			}
			_recipeCraftableCount = _cookingManager.GetAmountCraftable(recipe: recipe, sourceItems: _allInventories, limitToCurrentIngredients: false);
			_recipeReadyToCraftCount = _cookingManager.GetAmountCraftable(recipe: recipe, sourceItems: _allInventories, limitToCurrentIngredients: true);
		}

		private void ChangeInventory(bool isChangingToNext)
		{
			int delta = isChangingToNext ? 1 : -1;
			int index = _inventoryId;
			if (_allInventories.Count > 1)
			{
				// Navigate in given direction
				index += delta;
				// Negative-delta navigation cycles around to end
				if (index < BackpackInventoryId)
					index = _allInventories.Count - 1;
				// Positive-delta navigation without further elements cycles around to start
				if (index > FridgeInventoryId && _allInventories.Count <= InventoryIdsBeforeMinifridges)
					index = BackpackInventoryId;
				// Positive-delta navigation cycles around to start
				if (index == _allInventories.Count)
					index = BackpackInventoryId;
			}

			this.ChangeInventory(index);
		}

		private void ChangeInventory(int index)
		{
			_inventoryId = index;
			inventory.actualInventory = _allInventories[_inventoryId];
			inventory.showGrayedOutSlots = _inventoryId == BackpackInventoryId;
			Log.D($"New inventory: {_inventoryId}",
				Config.DebugMode);
		}

		private void TryAutoFillIngredients()
		{
			if (!IsUsingAutofill)
				return;

			// Remove all items from ingredients slots
			_cookingManager.ClearCurrentIngredients();

			if (_recipeIndex < 0 || _recipesFiltered.Count < _recipeIndex - 1) // Don't fill slots if the player can't cook the recipe
				return;

			_cookingManager.AutoFillIngredients(recipe: CurrentRecipe, sourceItems: _allInventories);
		}

		private void TryClickNavButton(int x, int y, bool playSound)
		{
			if (_stack.Count < 1)
				return;
			int lastRecipe = _recipeIndex;
			State state = _stack.Peek();
			bool isGridView = ModEntry.Instance.States.Value.IsUsingRecipeGridView;
			int max = _recipesFiltered.Count - 1;
			if (isGridView)
			{
				max = 4 * (max / 4) + 4;
			}
			int delta = Game1.isOneOfTheseKeysDown(Game1.oldKBState, new[] {new InputButton(Keys.LeftShift)})
				? _searchResultsPerPage
				: isGridView && state == State.Search ? _searchResultsArea.Width / _recipeDisplayHeight : 1;
			switch (state)
			{
				case State.Search:
					if (_recipeSearchResults.Count < 1)
						break;

					// Search up/down nav buttons
					if (_navUpButton.containsPoint(x, y))
					{
						_recipeIndex = Math.Max(_recipeSearchResults.Count / 2, _recipeIndex - delta);
					}
					else if (_navDownButton.containsPoint(x, y))
					{
						_recipeIndex = Math.Min(max - _recipeSearchResults.Count / 2, _recipeIndex + delta);
					}
					else
					{
						return;
					}
					break;

				case State.Recipe:
					// Recipe next/prev nav buttons
					if (_navLeftButton.containsPoint(x, y))
					{
						this.ChangeCurrentRecipe(_recipeIndex - delta);
						_showCookingConfirmPopup = false;
					}
					else if (_navRightButton.containsPoint(x, y))
					{
						this.ChangeCurrentRecipe(_recipeIndex + delta);
						_showCookingConfirmPopup = false;
					}
					else
						return;
					break;

				case State.Ingredients:

					break;

				default:
					return;
			}

			if (Game1.options.SnappyMenus && currentlySnappedComponent != null && !IsNavButtonActive(currentlySnappedComponent.myID))
			{
				if (currentlySnappedComponent.myID == _navLeftButton.myID || currentlySnappedComponent.myID == _navRightButton.myID)
					this.setCurrentlySnappedComponentTo(_recipeIconButton.myID);
				if (currentlySnappedComponent.myID == _navUpButton.myID || currentlySnappedComponent.myID == _navDownButton.myID)
					this.setCurrentlySnappedComponentTo(_recipeSearchResults.Count > 0
						? ModEntry.Instance.States.Value.IsUsingRecipeGridView
							? _searchGridClickables.First().myID
							: _searchListClickables.First().myID
						: _toggleFilterButton.myID);
			}

			if (playSound && _recipeIndex != lastRecipe)
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

			if (itemWasMoved)
			{
				if (_showCookingConfirmPopup)
				{
					this.ToggleCookingConfirmPopup(playSound: false);
				}
				this.UpdateCraftableCounts(recipe: CurrentRecipe);
			}

			if (Game1.options.SnappyMenus && ReadyToCook && currentlySnappedComponent != null
				&& currentlySnappedComponent.myID != _navLeftButton.myID && currentlySnappedComponent.myID != _navRightButton.myID)
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
				itemWasMoved = _cookingManager.AddToIngredients(inventoryId: _inventoryId, itemIndex: itemIndex);
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
			int index = -1;
			if (!_searchResultsArea.Contains(x, y) || _recipeDisplayHeight == 0)
				return index;
			int yIndex = (y - _searchResultsArea.Y - (_searchResultsArea.Height % _recipeDisplayHeight) / 2) / _recipeDisplayHeight;
			int xIndex = (x - _searchResultsArea.X) / _recipeDisplayHeight;
			index = ModEntry.Instance.States.Value.IsUsingRecipeGridView
				? yIndex * (_searchResultsArea.Width / _recipeDisplayHeight) + xIndex
				: yIndex;
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
				return (ModEntry.Instance.States.Value.IsUsingRecipeGridView && _recipeIndex < _recipesFiltered.Count - (_searchResultsPerPage / 2))
					|| (!ModEntry.Instance.States.Value.IsUsingRecipeGridView && _recipesFiltered.Count - _recipeIndex > 1 + (_searchResultsPerPage / 2));

			return false;
		}

		internal bool PopMenuStack(bool playSound, bool tryToQuit = false)
		{
			try
			{
				if (_stack.Count < 1)
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
					this.CloseTextBox(_searchBarTextBox);
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

		protected override void cleanupBeforeExit()
		{
			Game1.displayHUD = true;
			base.cleanupBeforeExit();
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
			if (_stack.Count == 0)
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
			if (_stack.Count < 1)
				return;
			State state = _stack.Peek();

			if (oldRegion == 9000)
			{
				switch (direction)
				{
					// Up
					case 0:
						if (state == State.Search)
						{
							if (_recipeSearchResults.Count > 0)
							{
								this.setCurrentlySnappedComponentTo(ModEntry.Instance.States.Value.IsUsingRecipeGridView
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
						if (_inventorySelectButtons.Count > 0 && !Context.IsSplitScreen)
							this.setCurrentlySnappedComponentTo(_inventorySelectButtons[0].myID);
						break;
					// Down
					case 2:
						break;
					// Left
					case 3:
						if (IsAutofillEnabled)
							this.setCurrentlySnappedComponentTo(_autofillButton.myID);
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
			if (_stack.Count < 1)
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

			upperRightCloseButton.tryHover(x, y, 0.5f);

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
						foreach (ClickableTextureComponent clickable in new[] { _toggleOrderButton, _toggleFilterButton, _toggleViewButton })
						{
							clickable.tryHover(x, y, 0.2f);
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

					if (!ModEntry.Instance.States.Value.IsUsingRecipeGridView)
						break;

					// Hover text over recipe search results when in grid view, which unlike list view, has names hidden
					int index = this.TryGetIndexForSearchResult(x, y);
					if (index >= 0 && index < _recipeSearchResults.Count && _recipeSearchResults[index] != null && _recipeSearchResults[index].name != "Torch")
						hoverText = Game1.player.knowsRecipe(_recipeSearchResults[index].name)
							? _recipeSearchResults[index].DisplayName
							: i18n.Get("menu.cooking_recipe.title_unknown");

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

			// Inventory nav buttons
			foreach (ClickableTextureComponent clickable in _inventorySelectButtons)
			{
				clickable.tryHover(x, y, 0.25f);
			}

			// Inventory autofill button
			if (IsAutofillEnabled)
			{
				_autofillButton.tryHover(x, y);
				if (_autofillButton.containsPoint(x, y))
					hoverText = _autofillButton.hoverText;
			}
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			if (_stack.Count == 0 || Game1.activeClickableMenu == null)
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
					// Search text box
					if (_searchBarTextBoxBounds.Contains(x, y))
					{
						_searchBarTextBox.Text = "";
						Game1.keyboardDispatcher.Subscriber = _searchBarTextBox;
						_searchBarTextBox.SelectMe();
						_showSearchFilters = false;
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
						this.CloseTextBox(_searchBarTextBox);
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
								ModEntry.Instance.States.Value.LastFilterThisSession = which;
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
							bool isGridView = ModEntry.Instance.States.Value.IsUsingRecipeGridView;
							_toggleViewButton.sourceRect.X = ToggleViewButtonSource.X
							                                + (isGridView ? 0 : ToggleViewButtonSource.Width);

							this.KeepRecipeIndexInSearchBounds();

							ModEntry.Instance.States.Value.IsUsingRecipeGridView = !isGridView;
							Game1.playSound(PageChangeCue);
							_toggleViewButton.hoverText =
								i18n.Get($"menu.cooking_search.view.{(isGridView ? "grid" : "list")}");
						}
					}

					int index = this.TryGetIndexForSearchResult(x, y);
					if (index >= 0 && index < _recipeSearchResults.Count && _recipeSearchResults[index] != null && _recipeSearchResults[index].name != "Torch")
					{
						Game1.playSound(PageChangeCue);
						this.ChangeCurrentRecipe(_recipeSearchResults[index].name);
						this.OpenRecipePage();
					}

					break;

				case State.Recipe:

					// Favourite recipe button
					if (_recipeIconButton.containsPoint(x, y))
					{
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

					break;
			}

			// Autofill button
			if (IsAutofillEnabled && _autofillButton.containsPoint(x, y))
			{
				Game1.playSound(ClickCue);
				IsUsingAutofill = !IsUsingAutofill;
				_autofillButton.sourceRect.X = IsUsingAutofill // Update toggled button appearance
					? AutofillButtonSource.X + AutofillButtonSource.Width
					: AutofillButtonSource.X;
				_cookingManager.ClearCurrentIngredients(); // Remove current ingredients from slots
				this.TryAutoFillIngredients(); // Actually auto-add ingredients to cooking slots
				this.ChangeCurrentRecipe(_recipeIndex); // Refresh check for ready-to-cook recipe
			}
			// Search tab
			else if (state != State.Search && _searchTabButton.containsPoint(x, y))
			{
				_stack.Pop();
				this.OpenSearchPage();
				Game1.playSound(MenuChangeCue);
			}
			// Ingredients tab
			else if (IsIngredientsPageEnabled && Config.AddRecipeRebalancing
			         && state != State.Ingredients && _ingredientsTabButton.containsPoint(x, y))
			{
				_stack.Pop();
				this.OpenIngredientsPage();
				Game1.playSound(MenuChangeCue);
			}
			// Cook! button
			else if (ReadyToCook && _cookButton.bounds.Contains(x, y))
			{
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
					this.CloseTextBox(_quantityTextBox);
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

			// Inventory nav buttons
			foreach (ClickableTextureComponent clickable in _inventorySelectButtons)
			{
				if (clickable.bounds.Contains(x, y))
				{
					int index = clickable.name == "inventorySelect"
						// Player backpack
						? BackpackInventoryId
						: clickable.name == "fridgeSelect"
							// Fridge
							? FridgeInventoryId
							// Minifridges
							: int.Parse(clickable.name[clickable.name.Length - 1].ToString()) + InventoryIdsBeforeMinifridges;
					this.ChangeInventory(index);
					Game1.playSound(ClickCue);
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
			if (_stack.Count == 0)
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
					this.CloseTextBox(_searchBarTextBox);
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
			if (_stack.Count == 0)
				return;
			State state = _stack.Peek();
			if (state == State.Opening)
				return;

			bool isExitButton = b == Buttons.Start || b == Buttons.B || b == Buttons.Y;

			int cur = currentlySnappedComponent != null ? currentlySnappedComponent.myID : -1;

			Log.D(currentlySnappedComponent != null
				? $"GP CSC: {currentlySnappedComponent.myID} ({currentlySnappedComponent.name})"
					+ $" [{currentlySnappedComponent.leftNeighborID} {currentlySnappedComponent.upNeighborID}"
					+ $" {currentlySnappedComponent.rightNeighborID} {currentlySnappedComponent.downNeighborID}]"
				: "GP CSC: null",
				Config.DebugMode);

			// Contextual navigation
			int firstID = state == State.Search
				? _recipeSearchResults.Count > 0
					? ModEntry.Instance.States.Value.IsUsingRecipeGridView
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
			}
			if (b == Buttons.RightShoulder)
			{
				this.setCurrentlySnappedComponentTo(index == -1
					? set.Last()
					: index == 0
						? set.Last()
						: set[index - 1]);
			}
			if (b == Buttons.LeftTrigger)
			{
				this.ChangeInventory(isChangingToNext: false);
			}
			if (b == Buttons.RightTrigger)
			{
				this.ChangeInventory(isChangingToNext: true);
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
					this.CloseTextBox(_searchBarTextBox);
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

			if (_stack.Count < 1)
				return;
			State state = _stack.Peek();
			Point cursor = Game1.getMousePosition();
			if (_showCookingConfirmPopup && _quantityScrollableArea.Contains(cursor))
			{
				this.TryClickQuantityButton(_cookQuantityUpButton.bounds.X,
					direction < 0 ? _cookQuantityDownButton.bounds.Y : _cookQuantityUpButton.bounds.Y);
			}
			else if (inventory.isWithinBounds(cursor.X, cursor.Y) || _inventoriesScrollableArea.Contains(cursor))
			{
				// Scroll wheel navigates between backpack, fridge, and minifridge inventories
				this.ChangeInventory(isChangingToNext: direction < 0);
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
			if (_stack.Count < 1)
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
					if (cur < inventory.inventory.Count && cur % GetColumnCount() == 0)
						// Snap to autofill button from leftmost inventory slots
						next = IsAutofillEnabled
							? _autofillButton.myID
							: cur;
					else if (cur == _recipeIconButton.myID)
						next = this.IsNavButtonActive(_navLeftButton.myID) ? _navLeftButton.myID : _searchTabButton.myID;
					else if (cur == _ingredientsClickables.First().myID && state == State.Recipe)
						next = this.IsNavButtonActive(_navRightButton.myID) ? _navRightButton.myID : _recipeIconButton.myID;
					else if (cur == _navUpButton.myID || cur == _navDownButton.myID)
						next = ModEntry.Instance.States.Value.IsUsingRecipeGridView
							? _searchGridClickables.First().myID
							: _searchListClickables.First().myID;
				}
				if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
				{
					if (cur == _searchTabButton.myID)
						next = this.IsNavButtonActive(_navLeftButton.myID) ? _navLeftButton.myID : _recipeIconButton.myID;
					else if (cur == _recipeIconButton.myID)
						next = this.IsNavButtonActive(_navRightButton.myID) ? _navRightButton.myID : _ingredientsClickables.First().myID;
					else if (((ModEntry.Instance.States.Value.IsUsingRecipeGridView
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
					else if (cur == _autofillButton.myID)
						next = state == State.Recipe
							? IsIngredientsPageEnabled ? _ingredientsTabButton.myID : _searchTabButton.myID
							: _searchBarClickable.myID;
					else if (cur == _navDownButton.myID)
						next = this.IsNavButtonActive(_navUpButton.myID)
							? _navUpButton.myID
							: _navUpButton.upNeighborID;
				}
				if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
				{
					ClickableComponent[] set = new[] { _searchBarClickable, _toggleFilterButton, _toggleOrderButton, _toggleViewButton };
					if (set.Any(clickable => clickable.myID == cur))
						// Moving into search results from search bar
						// Doesn't include ToggleFilterButton since it inexplicably already navigates to first search result
						next = ModEntry.Instance.States.Value.IsUsingRecipeGridView
							? _searchGridClickables.First().myID
							: _searchListClickables.First().myID;
					else if (cur < inventory.inventory.Count - this.GetColumnCount())
						// Inventory row navigation
						next = cur + this.GetColumnCount();
					else if (cur < inventory.inventory.Count && cur >= inventory.inventory.Count - this.GetColumnCount())
						// Do not scroll further down or wrap around when at bottom of inventory in solo play
						// In split-screen play, select the fridge buttons if available
						next = Context.IsSplitScreen && _inventorySelectButtons.Any()
							? _inventorySelectButtons.First().myID
							: cur;
					else if (cur < inventory.inventory.Count)
						// Moving into search results from inventory
						next = ModEntry.Instance.States.Value.IsUsingRecipeGridView
							? _searchGridClickables.Last().myID
							: _searchListClickables.Last().myID;
					else if (cur == _ingredientsClickables.Last().myID && state == State.Recipe)
						next = ReadyToCook
							? _cookButton.myID
							: _showCookingConfirmPopup
								? _cookConfirmButton.myID
								: 0; // First element in inventory
					else if (cur == _navUpButton.myID)
						next = this.IsNavButtonActive(_navDownButton.myID)
							? _navDownButton.myID
							: _navDownButton.downNeighborID;
				}
				if (next != -1)
				{
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
						if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key) && IsNavButtonActive(_navLeftButton.myID))
							this.ChangeCurrentRecipe(--_recipeIndex);
						if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key) && IsNavButtonActive(_navRightButton.myID))
							this.ChangeCurrentRecipe(++_recipeIndex);
						// Navigate up/down buttons control inventory
						if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key) && IsNavButtonActive(_navUpButton.myID))
							this.ChangeInventory(isChangingToNext: false);
						if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key) && IsNavButtonActive(_navDownButton.myID))
							this.ChangeInventory(isChangingToNext: true);
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
						this.CloseTextBox(_searchBarTextBox);
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
			if (_stack.Count < 1)
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

			if (false && currentlySnappedComponent != null && Config.DebugMode)
				b.Draw(Game1.fadeToBlackRect, currentlySnappedComponent.bounds, Color.Red * 0.5f);
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

			_searchResultsArea.Y = _navUpButton.bounds.Y - 8;
			_searchResultsArea.Height = _navDownButton.bounds.Y + _navDownButton.bounds.Height - _navUpButton.bounds.Y + 16;

			if (_recipeSearchResults.Count == 0 || _recipeSearchResults.Any(recipe => recipe?.name == "Torch"))
			{
				text = i18n.Get("menu.cooking_search.none_label");
				this.DrawText(b, text, scale: 1f,
					_leftContent.X - _searchResultsArea.X + TextSpacingFromIcons - 16,
					_searchResultsArea.Y + 64,
					_searchResultsArea.Width - TextSpacingFromIcons, isLeftSide: true);
			}
			else
			{
				if (ModEntry.Instance.States.Value.IsUsingRecipeGridView)
				{
					for (int i = 0; i < _recipeSearchResults.Count; ++i)
					{
						recipe = _recipeSearchResults[i];
						if (recipe == null)
							continue;

						recipe.drawMenuView(b, _searchGridClickables[i].bounds.X, _searchGridClickables[i].bounds.Y);
					}
				}
				else
				{
					int width = _searchResultsArea.Width - TextSpacingFromIcons;
					for (int i = 0; i < _recipeSearchResults.Count; ++i)
					{
						recipe = _recipeSearchResults[i];
						if (recipe == null)
							continue;

						recipe.drawMenuView(b, _searchListClickables[i].bounds.X, _searchListClickables[i].bounds.Y);

						text = Game1.player.knowsRecipe(recipe?.name)
							? recipe.DisplayName
							: i18n.Get("menu.cooking_recipe.title_unknown");

						this.DrawText(b, text, scale: 1f,
							_searchListClickables[i].bounds.X - _leftContent.X + TextSpacingFromIcons,
							_searchListClickables[i].bounds.Y - (int)(Game1.smallFont.MeasureString(Game1.parseText(
								"Strawberry Cake", Game1.smallFont, _searchResultsArea.Width - TextSpacingFromIcons)).Y / 2 - RecipeListHeight / 2),
							width, isLeftSide: true);
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
			foreach (ClickableTextureComponent clickable in new[] { _toggleFilterButton, _toggleOrderButton, _toggleViewButton})
				if (_searchBarTextBox.X + _searchBarTextBox.Width < clickable.bounds.X)
					clickable.draw(b);
			
			if (_showSearchFilters)
			{
				// Filter clickable icons container
				// left
				b.Draw(
					Texture,
					new Rectangle(
						_filterContainerBounds.X, _filterContainerBounds.Y,
						FilterContainerSideWidth * Scale, _filterContainerBounds.Height),
					new Rectangle(
						FilterContainerSource.X, FilterContainerSource.Y,
						FilterContainerSideWidth, FilterContainerSource.Height),
					Color.White);
				// middle
				b.Draw(
					Texture,
					new Rectangle(
						_filterContainerBounds.X + FilterContainerSideWidth * Scale, _filterContainerBounds.Y,
						_filterContainerMiddleWidth * Scale, _filterContainerBounds.Height),
					new Rectangle(
						FilterContainerSource.X + FilterContainerSideWidth, FilterContainerSource.Y,
						1, FilterContainerSource.Height),
					Color.White);
				// right
				b.Draw(
					Texture,
					new Rectangle(
						_filterContainerBounds.X + FilterContainerSideWidth * Scale + _filterContainerMiddleWidth * Scale,
						_filterContainerBounds.Y,
						FilterContainerSideWidth * Scale, _filterContainerBounds.Height),
					new Rectangle(
						FilterContainerSource.X + FilterContainerSideWidth + 1, FilterContainerSource.Y,
						FilterContainerSideWidth, FilterContainerSource.Height),
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
			float textHeightCheck = 0f;
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
				b.Draw(Texture,
					new Rectangle(
						_recipeIconButton.bounds.X + _recipeIconButton.bounds.Width / 3 * 2,
						_recipeIconButton.bounds.Y + _recipeIconButton.bounds.Height / 3 * 2,
						FavouriteIconSource.Width * 3, FavouriteIconSource.Height * 3),
					FavouriteIconSource, Color.White);
			}
			float titleScale = 1f;
			textWidth = (int)(162 * xScale);
			text = knowsRecipe
				? CurrentRecipe.DisplayName
				: i18n.Get("menu.cooking_recipe.title_unknown");
			textPosition.X = _navLeftButton.bounds.Width + 56;

			// Attempt to fix for Deutsch lange names
			if (_locale == "de" && Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).X > textWidth)
				text = text.Replace("-", "\n").Trim();

			if (Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).X * 0.8 > textWidth)
				titleScale = 0.735f;
			else if (Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).X > textWidth)
				titleScale = 0.95f;

			textPosition.Y = _navLeftButton.bounds.Y + 4;
			textPosition.Y -= (Game1.smallFont.MeasureString(
				Game1.parseText(text, Game1.smallFont, textWidth)).Y / 2 - 24) * yScale;
			textHeightCheck = Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).Y * yScale * titleScale;
			if (textHeightCheck * titleScale > 60)
				textPosition.Y += (textHeightCheck - 60) / 2;
			this.DrawText(b, text, 1.5f * titleScale, textPosition.X, textPosition.Y, textWidth, isLeftSide);

			// Recipe description
			textPosition.X = 0;
			textPosition.Y = _navLeftButton.bounds.Y + _navLeftButton.bounds.Height + 25;
			if (textHeightCheck > 60)
				textPosition.Y += textHeightCheck - 50 * xScale;
			textWidth = (int)(_textWidth * xScale);
			text = knowsRecipe
				? CurrentRecipe.description
				: i18n.Get("menu.cooking_recipe.title_unknown");
			this.DrawText(b, text, 1f, textPosition.X, textPosition.Y, textWidth, isLeftSide);
			textPosition.Y += TextDividerGap * 2;

			// Recipe ingredients
			if (textHeightCheck > 60 && Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).Y < 80)
				textPosition.Y -= 6 * Scale;
			textHeightCheck = Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).Y * yScale;
			if (textHeightCheck > 120) 
				textPosition.Y += 6 * Scale;
			if (textHeightCheck > 100 && CurrentRecipe?.getNumberOfIngredients() < 6)
				textPosition.Y += 6 * Scale;
			textPosition.Y += TextDividerGap + Game1.smallFont.MeasureString(
				Game1.parseText(yScale < 1 ? "Hoplite!\nHoplite!" : "Hoplite!\nHoplite!\nHoplite!", Game1.smallFont, textWidth)).Y * yScale;
			this.DrawHorizontalDivider(b, 0, textPosition.Y, _lineWidth, isLeftSide);
			textPosition.Y += TextDividerGap;
			text = i18n.Get("menu.cooking_recipe.ingredients_label");
			this.DrawText(b, text, 1f, textPosition.X, textPosition.Y, null, isLeftSide, SubtextColour);
			textPosition.Y += Game1.smallFont.MeasureString(
				Game1.parseText(text, Game1.smallFont, textWidth)).Y * yScale;
			this.DrawHorizontalDivider(b, 0, textPosition.Y, _lineWidth, isLeftSide);
			textPosition.Y += TextDividerGap - 64 / 2 + 4;

			if (knowsRecipe)
			{
				for (int i = 0; i < CurrentRecipe.getNumberOfIngredients(); ++i)
				{
					textPosition.Y += 64 / 2 + (CurrentRecipe.getNumberOfIngredients() < 5 ? 4 : 0);

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
					Color drawColour = _recipeIngredientQuantitiesHeld[i] >= requiredQuantity ? Game1.textColor : BlockedColour;

					// Ingredient icon
					b.Draw(
						texture: Game1.objectSpriteSheet,
						position: new Vector2(_leftContent.X, textPosition.Y - 2f),
						sourceRectangle: Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet,
							CurrentRecipe.getSpriteIndexFromRawIndex(id), 16, 16),
						color: Color.White, rotation: 0f, origin: Vector2.Zero, scale: 2f, effects: SpriteEffects.None, layerDepth: 0.86f);
					// Ingredient quantity
					Utility.drawTinyDigits(
						toDraw: CurrentRecipe.recipeList.Values.ElementAt(i),
						b,
						position: new Vector2(
							_leftContent.X + 32 - Game1.tinyFont.MeasureString(string.Concat(CurrentRecipe.recipeList.Values.ElementAt(i))).X,
							textPosition.Y + 21 - 2f),
						scale: 2f,
						layerDepth: 0.87f,
						c: Color.AntiqueWhite);
					// Ingredient name
					this.DrawText(b, ingredientNameText, 1f, 48, textPosition.Y, null, isLeftSide, drawColour);

					// Ingredient stock
					if (!Game1.options.showAdvancedCraftingInformation)
						continue;
					Point position = new Point((int)(_lineWidth - 64 * xScale), (int)(textPosition.Y + 2));
					b.Draw(
						Game1.mouseCursors,
						new Rectangle(_leftContent.X + position.X, position.Y, 22, 26),
						new Rectangle(268, 1436, 11, 13),
						Color.White);
					this.DrawText(b, _recipeIngredientQuantitiesHeld[i].ToString(), 1f, position.X + 32, position.Y, 72, isLeftSide, drawColour);
				}
			}
			else
			{
				textPosition.Y += 64 / 2 + 4;
				text = i18n.Get("menu.cooking_recipe.title_unknown");
				this.DrawText(b, text, 1f, 40, textPosition.Y, textWidth, isLeftSide, SubtextColour);
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
				Vector2 location = new Vector2(
					_ingredientsClickables[i].bounds.Location.X + _ingredientsClickables[i].bounds.Width / 2 - 64 / 2,
					_ingredientsClickables[i].bounds.Location.Y + _ingredientsClickables[i].bounds.Height / 2 - 64 / 2);
				if (iconShakeTimer.ContainsKey(i))
					location += 1f * new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2));
				Item item = _cookingManager.GetItemForIngredient(index: i, sourceItems: _allInventories);
				item?.drawInMenu(
					b,
					location: location,
					scaleSize: 1f,
					transparency: 1f,
					layerDepth: 0.865f,
					drawStackNumber: StackDrawType.Draw,
					color: Color.White,
					drawShadow: true);
			}

			Vector2 textPosition = Vector2.Zero;
			int textWidth = (int)(_textWidth * xScale);
			string text;

			// Recipe notes
			text = i18n.Get("menu.cooking_recipe.notes_label");
			textPosition.Y = _cookbookRightRect.Y + _cookbookRightRect.Height - 196 - Game1.smallFont.MeasureString(
				Game1.parseText(text: text, whichFont: Game1.smallFont, width: textWidth)).Y * yScale;

			if (_showCookingConfirmPopup)
			{
				textPosition.Y += 16;
				textPosition.X += 64;

				// Contextual cooking popup
				Game1.DrawBox(x: _cookIconBounds.X, y: _cookIconBounds.Y, width: _cookIconBounds.Width, height: _cookIconBounds.Height);
				CurrentRecipe?.drawMenuView(b, x: _cookIconBounds.X + 14, y: _cookIconBounds.Y + 14);

				_cookQuantityUpButton.draw(b);
				_quantityTextBox.Draw(b);
				_cookQuantityDownButton.draw(b);

				_cookConfirmButton.draw(b);
				_cookCancelButton.draw(b);

				return;
			}

			if (_stack.Count < 1 || _stack.Peek() != State.Recipe)
			{
				return;
			}

			this.DrawHorizontalDivider(b, 0, textPosition.Y, _lineWidth, false);
			textPosition.Y += TextDividerGap;
			this.DrawText(b, text, 1f, textPosition.X, textPosition.Y, null, false, SubtextColour);
			textPosition.Y += Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).Y * yScale;
			this.DrawHorizontalDivider(b, 0, textPosition.Y, _lineWidth, false);
			textPosition.Y += TextDividerGap * 2;

			if (_recipeAsItem == null || _stack.Count < 1 || _stack.Peek() != State.Recipe)
				return;

			if (ReadyToCook)
			{
				textPosition.Y += 16;
				textPosition.X = _rightContent.X + _cookbookRightRect.Width / 2 - MarginRight;
				int frypanWidth = Config.AddCookingToolProgression ? 16 + 4 : 0;

				// Cook! button
				int extraHeight = new [] { "ko", "ja", "zh", "tr" }.Contains(_locale) ? 4 : 0;
				Rectangle source = CookButtonSource;
				source.X += _animFrame * CookButtonSource.Width;
				Rectangle dest = new Rectangle(
					(int)textPosition.X - frypanWidth / 2 * Scale,
					(int)textPosition.Y - extraHeight,
					source.Width * Scale,
					source.Height * Scale + extraHeight);
				dest.X -= (CookTextSourceWidths[_locale] / 2 * Scale - CookTextSideWidth * Scale) + MarginLeft - frypanWidth / 2;
				Rectangle clickableArea = new Rectangle(
					dest.X,
					dest.Y - extraHeight,
					CookTextSideWidth * Scale * 2 + (_cookTextMiddleWidth + frypanWidth) * Scale,
					dest.Height + extraHeight);
				if (clickableArea.Contains(Game1.getMouseX(), Game1.getMouseY()))
					source.Y += source.Height;
				// left
				source.Width = CookTextSideWidth;
				dest.Width = source.Width * Scale;
				b.Draw(
					texture: Texture, destinationRectangle: dest, sourceRectangle: source,
					color: Color.White, rotation: 0f, origin: Vector2.Zero, effects: SpriteEffects.None, layerDepth: 1f);
				// middle and text and frypan
				source.X = _animFrame * CookButtonSource.Width + CookButtonSource.X + CookTextSideWidth;
				source.Width = 1;
				dest.Width = (_cookTextMiddleWidth + frypanWidth) * Scale;
				dest.X += CookTextSideWidth * Scale;
				b.Draw(
					texture: Texture, destinationRectangle: dest, sourceRectangle: source,
					color: Color.White, rotation: 0f, origin: Vector2.Zero, effects: SpriteEffects.None, layerDepth: 1f);
				b.Draw(
					texture: Texture,
					destinationRectangle: new Rectangle(
						dest.X,
						dest.Y + (dest.Height - CookTextSource[_locale].Height * Scale) / 2
									   + CookButtonAnimTextOffsetPerFrame[_animFrame] * Scale,
						CookTextSource[_locale].Width * Scale,
						CookTextSource[_locale].Height * Scale + extraHeight),
					sourceRectangle: CookTextSource[_locale],
					color: Color.White, rotation: 0f, origin: Vector2.Zero, effects: SpriteEffects.None, layerDepth: 1f);
				dest.X += _cookTextMiddleWidth * Scale;
				dest.Width = 16 * Scale;
				if (Config.AddCookingToolProgression)
				{
					b.Draw(
						texture: Texture,
						destinationRectangle: new Rectangle(dest.X + 4 * Scale, dest.Y + (1 + CookButtonAnimTextOffsetPerFrame[_animFrame]) * Scale, 16 * Scale, 16 * Scale),
						sourceRectangle: new Rectangle(176 + ModEntry.Instance.States.Value.CookingToolLevel * 16, 272, 16, 16),
						color: Color.White, rotation: 0f, origin: Vector2.Zero, effects: SpriteEffects.None, layerDepth: 1f);
				}

				// right
				source.X = _animFrame * CookButtonSource.Width + CookButtonSource.X + CookButtonSource.Width - CookTextSideWidth;
				source.Width = CookTextSideWidth;
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
						int destOffset = i == 1 ? (source.Width * 2 + _cookTextMiddleWidth) * Scale + 96 : 0;
						b.Draw(
							texture: Texture,
							position: new Vector2(_rightContent.X + destOffset - 8, dest.Y - 32),
							sourceRectangle: new Rectangle(128 + sourceOffset, 48, 32, 32),
							color: Color.White, rotation: 0f, origin: Vector2.Zero, scale: Scale, effects: flipped, layerDepth: 1f);
					}
				}
			}
			else if (Config.HideFoodBuffsUntilEaten
				&& (!ModEntry.Instance.States.Value.FoodsEaten.Contains(_recipeAsItem.Name)))
			{
				text = i18n.Get("menu.cooking_recipe.notes_unknown");
				this.DrawText(b, text, 1f, textPosition.X, textPosition.Y, textWidth, false, SubtextColour);
			}
			else
			{
				// Energy
				textPosition.X = _locale != "zh" ? -8f : 8f;
				text = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3116",
					_recipeAsItem.staminaRecoveredOnConsumption());
				Utility.drawWithShadow(b,
					texture: Game1.mouseCursors,
					position: new Vector2(_rightContent.X + textPosition.X, textPosition.Y),
					sourceRect: new Rectangle(0, 428, 10, 10),
					color: Color.White, rotation: 0f, origin: Vector2.Zero, scale: 3f);
				textPosition.X += 34f;
				this.DrawText(b, text, 1f, textPosition.X, textPosition.Y, null, false, Game1.textColor);
				textPosition.Y += Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).Y * yScale;
				// Health
				text = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3118",
					_recipeAsItem.healthRecoveredOnConsumption());
				textPosition.X -= 34f;
				Utility.drawWithShadow(b,
					texture: Game1.mouseCursors,
					position: new Vector2(_rightContent.X + textPosition.X, textPosition.Y),
					sourceRect: new Rectangle(0, 428 + 10, 10, 10),
					color: Color.White, rotation: 0f, origin: Vector2.Zero, scale: 3f);
				textPosition.X += 34f;
				this.DrawText(b, text, 1f, textPosition.X, textPosition.Y, null, false, Game1.textColor);

				// Buff duration
				text = $"+{(_recipeBuffDuration / 60)}:{(_recipeBuffDuration % 60):00}";

				if (_recipeBuffDuration > 0)
				{
					textPosition.Y += Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).Y * 1.1f * yScale;
					textPosition.X -= 34f;
					Utility.drawWithShadow(b,
						texture: Game1.mouseCursors,
						position: new Vector2(_rightContent.X + textPosition.X, textPosition.Y),
						sourceRect: new Rectangle(434, 475, 9, 9),
						color: Color.White, rotation: 0f, origin: Vector2.Zero, scale: 3f);
					textPosition.X += 34f;
					this.DrawText(b, text, 1f, textPosition.X, textPosition.Y, null, false, Game1.textColor);
					textPosition.Y -= Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).Y * 1.1f * yScale;
				}

				textPosition.Y -= Game1.smallFont.MeasureString(Game1.parseText(text: text, whichFont: Game1.smallFont, width: textWidth)).Y * yScale;
				textPosition.X += -34f + _lineWidth / 2f + 16f;

				// Buffs
				if (_recipeBuffs != null && _recipeBuffs.Count > 0)
				{
					int count = 0;
					for (int i = 0; i < _recipeBuffs.Count && count < 4; ++i)
					{
						if (_recipeBuffs[i] == 0)
							continue;

						++count;
						Utility.drawWithShadow(b,
							texture: Game1.mouseCursors,
							position: new Vector2(_rightContent.X + textPosition.X, textPosition.Y),
							sourceRect: new Rectangle(10 + 10 * i, 428, 10, 10),
							color: Color.White, rotation: 0f, origin: Vector2.Zero, scale: 3f);
						textPosition.X += 34f;
						text = (_recipeBuffs[i] > 0 ? "+" : "")
							   + _recipeBuffs[i]
							   + " " + i18n.Get($"menu.cooking_recipe.buff.{i}");
						this.DrawText(b, text, 1f, textPosition.X, textPosition.Y, null, false, Game1.textColor);
						textPosition.Y += Game1.smallFont.MeasureString(Game1.parseText(text: text, whichFont: Game1.smallFont, width: textWidth)).Y * yScale;
						textPosition.X -= 34f;
					}
				}
			}
		}

		private void DrawInventoryMenu(SpriteBatch b)
		{
			// Card
			Game1.drawDialogueBox(
				inventory.xPositionOnScreen - borderWidth / 2 - 32,
				inventory.yPositionOnScreen - borderWidth - spaceToClearTopBorder + 28,
				width,
				height - (borderWidth + spaceToClearTopBorder + 192) - 12,
				false, true);

			// Items
			if (_inventorySelectButtons.Count > 0)
			{
				// Inventory nav buttons
				Game1.DrawBox(x: _inventoriesScrollableArea.X, y: _inventoriesScrollableArea.Y,
					width: _inventoriesScrollableArea.Width, height: _inventoriesScrollableArea.Height);
				foreach (ClickableTextureComponent clickable in _inventorySelectButtons)
					clickable.draw(b);

				// Inventory nav selected icon
				int w = 9;
				Rectangle sourceRect = new Rectangle(232 + 9 * ((int)(w * ((float)_animFrame / AnimFrames * 6)) / 9), 346, w, w);
				Rectangle currentButton = _inventorySelectButtons[_inventoryId].bounds;
				b.Draw(
					texture: Game1.mouseCursors,
					destinationRectangle: new Rectangle(
						currentButton.X + Scale * ((currentButton.Width / 2 - w * Scale / 2 - 1 * Scale) / Scale),
						currentButton.Y - w * Scale + 4 * Scale,
						w * Scale,
						w * Scale),
					sourceRectangle: sourceRect,
					color: Color.White);
			}

			// Inventory autofill button
			if (IsAutofillEnabled)
				_autofillButton.draw(b);
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
		private void DrawText(SpriteBatch b, string text, float scale, float x, float y, float? w, bool isLeftSide, Color? colour = null)
		{
			Point position = isLeftSide ? _leftContent : _rightContent;
			position.Y -= yPositionOnScreen;
			w ??= Game1.smallFont.MeasureString(text).X + (_locale == "ja" || _locale == "zh" ? 20 : 0);
			if (_locale == "ko" && _resizeKoreanFonts)
				scale *= KoHeightScale;
			Utility.drawTextWithShadow(b,
				text: Game1.parseText(text: text, whichFont: Game1.smallFont, width: (int)w),
				font: Game1.smallFont,
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
				b, color1: DividerColour);
		}
	}
}
