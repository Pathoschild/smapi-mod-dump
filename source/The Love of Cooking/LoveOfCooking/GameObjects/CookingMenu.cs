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
using SpaceCore;
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
		private static Texture2D Texture => ModEntry.SpriteSheet;
		private static Config Config => ModEntry.Instance.Config;
		private static ITranslationHelper i18n => ModEntry.Instance.Helper.Translation;

		// Spritesheet source areas
		// Custom spritesheet
		private static readonly Rectangle CookbookSource = new Rectangle(0, 80, 240, 128);
		private static readonly Rectangle CookingSlotOpenSource = new Rectangle(0, 208, 28, 28);
		private static readonly Rectangle CookingSlotLockedSource = new Rectangle(28, 208, 28, 28);
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
		private const int Scale = 4;
		private float KoHeightScale = 0.825f;
		private float KoWidthScale = 1.25f;
		private bool _resizeKoreanFonts;
		private static readonly Dictionary<string, Rectangle> CookTextSource = new Dictionary<string, Rectangle>();
		private static readonly Point CookTextSourceOrigin = new Point(0, 240);
		private readonly Dictionary<string, int> _cookTextSourceWidths = new Dictionary<string, int>
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
		private readonly ClickableTextureComponent NavDownButton;
		private readonly ClickableTextureComponent NavUpButton;
		private readonly ClickableTextureComponent NavRightButton;
		private readonly ClickableTextureComponent NavLeftButton;
		private readonly List<ClickableTextureComponent> CookingSlots = new List<ClickableTextureComponent>();
		private readonly ClickableComponent CookButton;
		private readonly ClickableTextureComponent CookQuantityUpButton;
		private readonly ClickableTextureComponent CookQuantityDownButton;
		private readonly ClickableTextureComponent CookConfirmButton;
		private readonly ClickableTextureComponent CookCancelButton;
		private Rectangle CookIconBounds;
		private readonly ClickableTextureComponent SearchTabButton;
		private readonly ClickableTextureComponent IngredientsTabButton;
		private readonly ClickableTextureComponent ToggleOrderButton;
		private readonly ClickableTextureComponent ToggleFilterButton;
		private readonly ClickableTextureComponent ToggleViewButton;
		private readonly ClickableComponent SearchBarClickable;
		private readonly ClickableTextureComponent SearchButton;
		private readonly ClickableTextureComponent RecipeIconButton;
		private readonly ClickableTextureComponent AutofillButton;
		private Rectangle FilterContainerBounds;
		private readonly List<ClickableTextureComponent> FilterButtons = new List<ClickableTextureComponent>();
		private Rectangle SearchResultsArea;
		private Rectangle QuantityScrollableArea;
		private Rectangle InventoriesScrollableArea;
		private readonly List<ClickableTextureComponent> InventorySelectButtons = new List<ClickableTextureComponent>();
		private readonly List<ClickableComponent> SearchListClickables = new List<ClickableComponent>();
		private readonly List<ClickableComponent> SearchGridClickables = new List<ClickableComponent>();

		// Layout dimensions (variable with screen size)
		private static Rectangle _cookbookLeftRect
			= new Rectangle(-1, -1, CookbookSource.Width * 4 / 2, CookbookSource.Height * Scale);
		private static Rectangle _cookbookRightRect
			= new Rectangle(-1, -1, CookbookSource.Width * 4 / 2, CookbookSource.Height * Scale);
		private static Point _leftContent;
		private static Point _rightContent;
		private static int _lineWidth;
		private static int _textWidth;

		// Layout definitions
		private const int MarginLeft = 16 * Scale;
		private const int MarginRight = 8 * Scale;
		private const int TextMuffinTopOverDivider = 6;
		private const int TextDividerGap = 4;
		private const int TextSpacingFromIcons = 80;
		private const int RecipeListHeight = 16 * Scale;
		private const int RecipeGridHeight = 18 * Scale;

		private static readonly Color SubtextColour = Game1.textColor * 0.75f;
		private static readonly Color BlockedColour = Game1.textColor * 0.325f;
		private static readonly Color DividerColour = Game1.textColor * 0.325f;

		private bool UseHorizontalInventoryButtonArea
		{
			get => InventorySelectButtons.Any() && Context.IsSplitScreen;
		}

		// Text entry
		private readonly TextBox _searchBarTextBox;
		private readonly TextBox _quantityTextBox;
		private Rectangle _quantityTextBoxBounds;
		private Rectangle _searchBarTextBoxBounds;
		private int _searchBarTextBoxMaxWidth;
		private int SearchBarTextBoxMinWidth;
		private const string QuantityTextBoxDefaultText = "1";

		// Menu data
		public enum State
		{
			Opening,
			Search,
			Recipe,
			Ingredients
		}
		private readonly Stack<State> _stack = new Stack<State>();
		private readonly List<CraftingRecipe> _unlockedCookingRecipes;
		private List<CraftingRecipe> _filteredRecipeList;
		private List<CraftingRecipe> _searchRecipes;
		private int _currentRecipe;
		private Item _recipeItem;
		private List<int> _recipeBuffs;
		private int _recipeBuffDuration;
		private int _recipesPerPage;
		private int _recipeHeight;
		private readonly int _cookingSlots;
		private List<Item> _cookingSlotsDropIn;
		private bool _showCookingConfirmPopup;
		private bool _showSearchFilters;
		private Filter _lastFilterUsed;
		private int _mouseHeldTicks;
		private string _locale;
		private List<IList<Item>> _minifridgeList = new List<IList<Item>>();
		private int _currentSelectedInventory;
		private static int _lastBurntCount;


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

		// Animations
		private static readonly int[] AnimTextOffsetPerFrame = { 0, 1, 0, -1, -2, -3, -2, -1 };
		private const int AnimFrameTime = 100;
		private const int AnimFrames = 8;
		private const int AnimTimerLimit = AnimFrameTime * AnimFrames;
		private int _animTimer;
		private int _animFrame;

		// Others
		private readonly IReflectedField<Dictionary<int, double>> _iconShakeTimerField;
		internal static readonly int SpriteId = (int)Game1.player.UniqueMultiplayerID + 5050505;

		// Features
		private bool IsIngredientsPageEnabled = false;
		private bool IsAutofillEnabled = false;
		private bool IsUsingAutofill
		{
			get
			{
				return IsAutofillEnabled && bool.Parse(Game1.player.modData[ModEntry.AssetPrefix + "autofill"]);
			}
			set
			{
				Game1.player.modData[ModEntry.AssetPrefix + "autofill"] = value.ToString();
			}
		}


		public CookingMenu(List<CraftingRecipe> recipes, bool addDummyState = false, string initialRecipe = null) : base(null)
		{
			Game1.displayHUD = true; // Prevents hidden HUD on crash when initialising menu, set to false at the end of this method
			_locale = LocalizedContentManager.CurrentLanguageCode.ToString();
			if (!_cookTextSourceWidths.ContainsKey(_locale))
			{
				_locale = "en";
			}
			_resizeKoreanFonts = Config.ResizeKoreanFonts;
			initializeUpperRightCloseButton();
			trashCan = null;

			_iconShakeTimerField = ModEntry.Instance.Helper.Reflection
				.GetField<Dictionary<int, double>>(inventory, "_iconShakeTimer");

			_unlockedCookingRecipes = recipes != null
				// Recipes may be populated by those of any CraftingMenu that this menu supercedes
				// Should guarantee Limited Campfire Cooking compatibility
				? recipes.Where(recipe => Game1.player.cookingRecipes.ContainsKey(recipe.name)).ToList()
				// Otherwise start off the list of cooking recipes with all those the player has unlocked
				: _unlockedCookingRecipes = Utility.GetAllPlayerUnlockedCookingRecipes()
					.Select(str => new CraftingRecipe(str, true))
					.Where(recipe => recipe.name != "Torch").ToList();

			ModEntry.Instance.UpdateEnglishRecipeDisplayNames(ref _unlockedCookingRecipes);

			// Default autofill preferences if none set
			if (!Game1.player.modData.ContainsKey(ModEntry.AssetPrefix + "autofill"))
			{
				IsUsingAutofill = false;
			}

			// Apply default filter to the default recipe list
			var reverseDefaultFilter = ModEntry.LastFilterReversed;
			_unlockedCookingRecipes = FilterRecipes();

			// Initialise filtered search lists
			_filteredRecipeList = _unlockedCookingRecipes;
			_searchRecipes = new List<CraftingRecipe>();

			// Cooking ingredients item drop-in slots
			_cookingSlots = ModEntry.Instance.GetNearbyCookingStationLevel();
			_cookingSlotsDropIn = new List<Item>(_cookingSlots);
			CookingSlots.Clear();
			for (var i = 0; i < Math.Max(5, _cookingSlots); ++i)
			{
				_cookingSlotsDropIn.Add(null);
				CookingSlots.Add(new ClickableTextureComponent(
					"cookingSlot" + i,
					new Rectangle(-1, -1, CookingSlotOpenSource.Width * Scale, CookingSlotOpenSource.Height * Scale),
					null, null, Texture, _cookingSlots <= i ? CookingSlotLockedSource : CookingSlotOpenSource, Scale));
			}

			// Clickables and elements
			NavDownButton = new ClickableTextureComponent(
				"navDown", new Rectangle(-1, -1, DownButtonSource.Width, DownButtonSource.Height),
				null, null, Game1.mouseCursors, DownButtonSource, 1f, true);
			NavUpButton = new ClickableTextureComponent(
				"navUp", new Rectangle(-1, -1, UpButtonSource.Width, UpButtonSource.Height),
				null, null, Game1.mouseCursors, UpButtonSource, 1f, true);
			NavRightButton = new ClickableTextureComponent(
				"navRight", new Rectangle(-1, -1, RightButtonSource.Width, RightButtonSource.Height),
				null, null, Game1.mouseCursors, RightButtonSource, 1f, true);
			NavLeftButton = new ClickableTextureComponent(
				"navLeft", new Rectangle(-1, -1, LeftButtonSource.Width, LeftButtonSource.Height),
				null, null, Game1.mouseCursors, LeftButtonSource, 1f, true);
			CookButton = new ClickableComponent(Rectangle.Empty, "cook");
			CookQuantityUpButton = new ClickableTextureComponent(
				"quantityUp", new Rectangle(-1, -1, PlusButtonSource.Width * Scale, PlusButtonSource.Height * Scale),
				null, null, Game1.mouseCursors, PlusButtonSource, Scale, true);
			CookQuantityDownButton = new ClickableTextureComponent(
				"quantityDown", new Rectangle(-1, -1, MinusButtonSource.Width * Scale, MinusButtonSource.Height * Scale),
				null, null, Game1.mouseCursors, MinusButtonSource, Scale, true);
			CookConfirmButton = new ClickableTextureComponent(
				"confirm", new Rectangle(-1, -1, OkButtonSource.Width, OkButtonSource.Height),
				null, null, Game1.mouseCursors, OkButtonSource, 1f, true);
			CookCancelButton = new ClickableTextureComponent(
				"cancel", new Rectangle(-1, -1, NoButtonSource.Width, NoButtonSource.Height),
				null, null, Game1.mouseCursors, NoButtonSource, 1f, true);
			ToggleFilterButton = new ClickableTextureComponent(
				"toggleFilter", new Rectangle(-1, -1, ToggleFilterButtonSource.Width * Scale, ToggleFilterButtonSource.Height * Scale),
				null, i18n.Get("menu.cooking_search.filter_label"),
				Texture, ToggleFilterButtonSource, Scale, true);
			ToggleOrderButton = new ClickableTextureComponent(
				"toggleOrder", new Rectangle(-1, -1, ToggleOrderButtonSource.Width * Scale, ToggleOrderButtonSource.Height * Scale),
				null, i18n.Get("menu.cooking_search.order_label"),
				Texture, ToggleOrderButtonSource, Scale, true);
			ToggleViewButton = new ClickableTextureComponent(
				"toggleView", new Rectangle(-1, -1, ToggleViewButtonSource.Width * Scale, ToggleViewButtonSource.Height * Scale),
				null, i18n.Get("menu.cooking_search.view."
				               + (ModEntry.Instance.IsUsingRecipeGridView ? "grid" : "list")),
				Texture, ToggleViewButtonSource, Scale, true);
			SearchButton = new ClickableTextureComponent(
				"search", new Rectangle(-1, -1, SearchButtonSource.Width * Scale, SearchButtonSource.Height * Scale),
				null, i18n.Get("menu.cooking_recipe.search_label"),
				Texture, SearchButtonSource, Scale, true);
			RecipeIconButton = new ClickableTextureComponent(
				"recipeIcon", new Rectangle(-1, -1, 64, 64),
				null, null,
				Game1.objectSpriteSheet, new Rectangle(0, 0, 64, 64), Scale, true);
			AutofillButton = new ClickableTextureComponent(
				"autofill", new Rectangle(-1, -1, 64, 64),
				null, null,
				Texture, AutofillButtonSource, Scale, true);
			SearchBarClickable = new ClickableComponent(Rectangle.Empty, "searchbox");
			SearchTabButton = new ClickableTextureComponent(
				"searchTab", new Rectangle(-1, -1, SearchTabButtonSource.Width * Scale, SearchTabButtonSource.Height * Scale),
				null, null, Texture, SearchTabButtonSource, Scale, true);
			IngredientsTabButton = new ClickableTextureComponent(
				"ingredientsTab", new Rectangle(-1, -1, IngredientsTabButtonSource.Width * Scale, IngredientsTabButtonSource.Height * Scale),
				null, null, Texture, IngredientsTabButtonSource, Scale, true);
			for (var i = (int) Filter.Alphabetical; i < (int) Filter.Count; ++i)
			{
				FilterButtons.Add(new ClickableTextureComponent(
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

			for (var i = 0; i < 5; ++i)
			{
				SearchListClickables.Add(new ClickableComponent(new Rectangle(-1, -1, -1, -1), "searchList" + i));
			}
			for (var i = 0; i < 16; ++i)
			{
				SearchGridClickables.Add(new ClickableComponent(new Rectangle(-1, -1, -1, -1), "searchGrid" + i));
			}

			// 'Cook!' button localisations
			var xOffset = 0;
			var yOffset = 0;
			CookTextSource.Clear();
			foreach (var pair in _cookTextSourceWidths)
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
			_currentSelectedInventory = -2;
			var fridgeForFarmHouse = Game1.currentLocation is FarmHouse fh
				&& ModEntry.Instance.GetFarmhouseKitchenLevel(fh) > 0;
			var fridgeForCommunityCentre = ModEntry.Instance.IsCommunityCentreKitchenEnabledByHost()
				&& Game1.currentLocation is CommunityCenter cc
				&& cc.areasComplete.Count > ModEntry.CommunityCentreAreaNumber && cc.areasComplete[ModEntry.CommunityCentreAreaNumber];
			// Check for community centre fridge
			if (fridgeForCommunityCentre)
			{
				if (!((CommunityCenter)Game1.currentLocation).Objects.ContainsKey(ModEntry.DummyFridgePosition))
				{
					((CommunityCenter)Game1.currentLocation).Objects.Add(
						ModEntry.DummyFridgePosition, new Chest(true, ModEntry.DummyFridgePosition));
				}
			}
			// Check for minifridges
			if (Game1.currentLocation is FarmHouse farmHouse)
			{
				_minifridgeList = farmHouse.Objects.Values.Where(o => o != null && o.bigCraftable.Value && o is Chest && o.ParentSheetIndex == 216)
					.Select(o => ((Chest)o).items).Take(12).Cast<IList<Item>>().ToList();
				for (var i = 0; i < _minifridgeList.Count; ++i)
				{
					InventorySelectButtons.Add(new ClickableTextureComponent($"minifridgeSelect{i}",
						new Rectangle(-1, -1, 16 * Scale, 16 * Scale), null, null,
						ModEntry.SpriteSheet, new Rectangle(243, 114, 11, 14), Scale, false));
				}
			}
			// Populate inventory list
			if (fridgeForFarmHouse || fridgeForCommunityCentre)
			{
				if (fridgeForFarmHouse || fridgeForCommunityCentre)
				{
					InventorySelectButtons.Insert(0, new ClickableTextureComponent("fridgeSelect",
						new Rectangle(-1, -1, 14 * Scale, 14 * Scale), null, null,
						ModEntry.SpriteSheet, new Rectangle(243, 97, 11, 14), Scale, false));
				}
				InventorySelectButtons.Insert(0, new ClickableTextureComponent("inventorySelect",
					new Rectangle(-1, -1, 14 * Scale, 14 * Scale), null, null,
					ModEntry.SpriteSheet, new Rectangle(243, 81, 11, 14), Scale, false));
			}

			// Setup menu elements layout
			RealignElements();
			InitialiseControllerFlow();

			if (addDummyState)
				_stack.Push(State.Opening);
			OpenSearchPage();

			// Apply previously-used filter
			if (ModEntry.LastFilterThisSession != Filter.None)
			{
				_filteredRecipeList = FilterRecipes(ModEntry.LastFilterThisSession);
			}
			if (reverseDefaultFilter)
			{
				_filteredRecipeList = ReverseRecipeList(_filteredRecipeList);
			}
			UpdateSearchRecipes();

			// Open to a starting recipe if needed
			if (!string.IsNullOrEmpty(initialRecipe))
			{
				ChangeCurrentRecipe(initialRecipe);
				OpenRecipePage();
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

			var id = 1000;
			ChildComponents.AddRange(new List<ClickableComponent>
			{
				NavDownButton,
				NavUpButton,
				NavRightButton,
				NavLeftButton,
				CookButton,
				CookQuantityUpButton,
				CookQuantityDownButton,
				CookConfirmButton,
				CookCancelButton,
				SearchTabButton,
				IngredientsTabButton,
				ToggleOrderButton,
				ToggleFilterButton,
				ToggleViewButton,
				SearchButton,
				RecipeIconButton,
				AutofillButton,
				SearchBarClickable,
			});
			ChildComponents.AddRange(CookingSlots);
			ChildComponents.AddRange(InventorySelectButtons);
			ChildComponents.AddRange(FilterButtons);
			ChildComponents.AddRange(SearchListClickables);
			ChildComponents.AddRange(SearchGridClickables);

			foreach (var clickable in ChildComponents)
			{
				clickable.myID = ++id;
			}

			// Component navigation
			SearchTabButton.downNeighborID = IsIngredientsPageEnabled
				? IngredientsTabButton.myID
				: IsAutofillEnabled
					? AutofillButton.myID
					: 0; // inventory

			AutofillButton.rightNeighborID = 0; // inventory

			SearchBarClickable.rightNeighborID = ToggleFilterButton.myID;

			ToggleFilterButton.leftNeighborID = SearchBarClickable.myID;
			ToggleFilterButton.rightNeighborID = ToggleOrderButton.myID;

			ToggleOrderButton.leftNeighborID = ToggleFilterButton.myID;
			ToggleOrderButton.rightNeighborID = ToggleViewButton.myID;

			ToggleViewButton.leftNeighborID = ToggleOrderButton.myID;
			ToggleViewButton.rightNeighborID = CookingSlots[0].myID;

			CookingSlots.Last().rightNeighborID = upperRightCloseButton.myID;
			upperRightCloseButton.leftNeighborID = CookingSlots.Last().myID;
			upperRightCloseButton.downNeighborID = CookingSlots.Last().myID;

			RecipeIconButton.downNeighborID = NavLeftButton.downNeighborID = NavRightButton.downNeighborID = 0;
			NavLeftButton.leftNeighborID = SearchTabButton.myID;
			NavLeftButton.rightNeighborID = RecipeIconButton.myID;
			NavRightButton.leftNeighborID = RecipeIconButton.myID;
			NavRightButton.rightNeighborID = CookingSlots.First().myID;

			NavUpButton.upNeighborID = ToggleFilterButton.myID;
			NavDownButton.downNeighborID = 0; // inventory

			CookButton.upNeighborID = CookingSlots.First().myID;
			CookButton.downNeighborID = 0;

			CookConfirmButton.leftNeighborID = CookQuantityUpButton.myID;
			CookCancelButton.leftNeighborID = CookQuantityDownButton.myID;
			CookQuantityUpButton.rightNeighborID = CookQuantityDownButton.rightNeighborID = CookConfirmButton.myID;
			CookQuantityUpButton.downNeighborID = CookQuantityDownButton.myID;
			CookQuantityDownButton.upNeighborID = CookQuantityUpButton.myID;
			CookConfirmButton.upNeighborID = CookQuantityUpButton.upNeighborID = CookingSlots.First().myID;
			CookCancelButton.upNeighborID = CookConfirmButton.myID;
			CookConfirmButton.downNeighborID = CookCancelButton.myID;
			CookCancelButton.downNeighborID = CookQuantityDownButton.downNeighborID = 0; // inventory

			// Child component navigation
			foreach (var clickableGroup in new [] { CookingSlots, InventorySelectButtons, FilterButtons })
			{
				for (var i = 0; i < clickableGroup.Count; ++i)
				{
					if (i > 0)
						clickableGroup[i].leftNeighborID = clickableGroup[i - 1].myID;
					if (i < clickableGroup.Count - 1)
						clickableGroup[i].rightNeighborID = clickableGroup[i + 1].myID;
				}
			}
			for (var i = 0; i < SearchListClickables.Count; ++i)
			{
				if (i > 0)
					SearchListClickables[i].upNeighborID = SearchListClickables[i - 1].myID;
				if (i < SearchListClickables.Count - 1)
					SearchListClickables[i].downNeighborID = SearchListClickables[i + 1].myID;
			}
			SearchListClickables.First().upNeighborID = ToggleFilterButton.myID;
			SearchListClickables.Last().downNeighborID = 0; // inventory
			for (var i = 0; i < SearchGridClickables.Count; ++i)
			{
				if (i > 0 && i % 4 != 0)
					SearchGridClickables[i].leftNeighborID = SearchGridClickables[i - 1].myID;
				if (i < SearchGridClickables.Count - 1)
					SearchGridClickables[i].rightNeighborID = SearchGridClickables[i + 1].myID;

				SearchGridClickables[i].upNeighborID = i < 4
					? ToggleFilterButton.myID
					: SearchGridClickables[i - 4].myID;
				SearchGridClickables[i].downNeighborID = i > SearchGridClickables.Count - 1 - 4
					? 0 // inventory
					: SearchGridClickables[i + 4].myID;
			}
			foreach (var clickable in CookingSlots)
			{
				clickable.downNeighborID = 0; // inventory
			}
			if (InventorySelectButtons.Count > 1)
			{
				InventorySelectButtons[0].leftNeighborID = UseHorizontalInventoryButtonArea
					? -1
					: GetColumnCount() - 1; // last element in the first row of the inventory
				InventorySelectButtons[0].upNeighborID = UseHorizontalInventoryButtonArea
					? GetColumnCount() * 2 // first element in the last row of the inventory
					: InventorySelectButtons[1].upNeighborID = CookingSlots.Last().myID; // last ingredient slot
			}

			// Probably does nothing
			setUpForGamePadMode();

			// Add clickables to implicit navigation
			populateClickableComponentList();

			if (Game1.options.gamepadControls || Game1.options.SnappyMenus)
			{
				snapToDefaultClickableComponent();
			}
		}

		private void RealignElements()
		{
			var centre = Utility.PointToVector2(Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Center);

			var yOffset = 0;
			var xOffset = 0;

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
				var scale = Game1.options.uiScale;
				var pos = upperRightCloseButton.bounds.X + upperRightCloseButton.bounds.Width;
				var bound = Game1.viewport.Width / 2;
				var diff = (pos - bound) * scale;
				upperRightCloseButton.bounds.X -= (int)Math.Max(0, diff / 2);
			}

			// Search elements
			SearchTabButton.bounds.Y = _cookbookLeftRect.Y + 72;
			IngredientsTabButton.bounds.Y = SearchTabButton.bounds.Y + SearchTabButton.bounds.Height + 16;
			SearchTabButton.bounds.X = IngredientsTabButton.bounds.X = _cookbookLeftRect.X - 40;

			yOffset = 32;
			xOffset = 40;
			var xOffsetExtra = 8;

			_searchBarTextBox.X = _leftContent.X;
			_searchBarTextBox.Y = _leftContent.Y + yOffset + 10;
			_searchBarTextBox.Width = 160;
			_searchBarTextBox.Selected = false;
			_searchBarTextBox.Text = i18n.Get("menu.cooking_recipe.search_label");
			_searchBarTextBox.Update();
			_searchBarTextBoxBounds = new Rectangle(
				_searchBarTextBox.X, _searchBarTextBox.Y, _searchBarTextBox.Width, _searchBarTextBox.Height);
			SearchBarClickable.bounds = _searchBarTextBoxBounds;

			ToggleFilterButton.bounds.X = _cookbookRightRect.X
			                              - ToggleFilterButton.bounds.Width
			                              - ToggleOrderButton.bounds.Width
			                              - ToggleViewButton.bounds.Width
			                              - xOffsetExtra * 3 - 24;
			ToggleOrderButton.bounds.X = ToggleFilterButton.bounds.X + ToggleFilterButton.bounds.Width + xOffsetExtra;
			ToggleViewButton.bounds.X = ToggleOrderButton.bounds.X + ToggleOrderButton.bounds.Width + xOffsetExtra;
			ToggleViewButton.bounds.Y = ToggleOrderButton.bounds.Y = ToggleFilterButton.bounds.Y = _leftContent.Y + yOffset;
			
			ToggleViewButton.sourceRect.X = ToggleViewButtonSource.X + (ModEntry.Instance.IsUsingRecipeGridView
				? ToggleViewButtonSource.Width : 0);

			SearchButton.bounds = ToggleViewButton.bounds;
			_searchBarTextBoxMaxWidth = SearchButton.bounds.X - _searchBarTextBox.X - 24;

			var minWidth = 132;
			SearchBarTextBoxMinWidth = Math.Min(ToggleFilterButton.bounds.X - _searchBarTextBoxBounds.X,
				Math.Max(minWidth, 24 + (int)Math.Ceiling(Game1.smallFont.MeasureString(_searchBarTextBox.Text).X)));

			NavUpButton.bounds.X = NavDownButton.bounds.X = SearchButton.bounds.X;
			NavUpButton.bounds.Y = SearchButton.bounds.Y + SearchButton.bounds.Height + 16;
			NavDownButton.bounds.Y = _cookbookLeftRect.Bottom - 128;
			
			SearchResultsArea = new Rectangle(
				_searchBarTextBox.X,
				NavUpButton.bounds.Y - 8,
				NavUpButton.bounds.X - _searchBarTextBox.X - 16,
				NavDownButton.bounds.Y + NavDownButton.bounds.Height - NavUpButton.bounds.Y + 16);

			// Recipe search results
			{
				int x, y;

				//SearchResultsArea.Y = NavUpButton.bounds.Y - 8;
				//SearchResultsArea.Height = NavDownButton.bounds.Y + NavDownButton.bounds.Height - NavUpButton.bounds.Y + 16;

				var gridColumns = SearchResultsArea.Width / RecipeGridHeight;
				yOffset = (SearchResultsArea.Height % RecipeGridHeight) / 2;
				for (var i = 0; i < SearchGridClickables.Count; ++i)
				{
					y = SearchResultsArea.Y + yOffset + (i / gridColumns) * RecipeGridHeight + (RecipeGridHeight - 16 * Scale) / 2;
					x = SearchResultsArea.X + (i % gridColumns) * RecipeGridHeight;
					SearchGridClickables[i].bounds = new Rectangle(x, y, 16 * Scale, 16 * Scale);
				}

				x = SearchResultsArea.X;
				yOffset = (SearchResultsArea.Height % RecipeListHeight) / 2;
				for (var i = 0; i < SearchListClickables.Count; ++i)
				{
					y = SearchResultsArea.Y + yOffset + i * RecipeListHeight + (RecipeListHeight - 64) / 2;
					SearchListClickables[i].bounds = new Rectangle(x, y, SearchResultsArea.Width, -1);
				}
				foreach (var clickable in SearchListClickables)
				{
					clickable.bounds.Height = SearchListClickables[SearchListClickables.Count - 1].bounds.Y
						- SearchListClickables[SearchListClickables.Count - 2].bounds.Y;
				}
			}

			yOffset = 24;
			for (var i = 0; i < FilterButtons.Count; ++i)
			{
				FilterButtons[i].bounds.X = _cookbookRightRect.X - xOffset - 4 - (FilterButtons.Count - i)
					* FilterButtons[i].bounds.Width;
				FilterButtons[i].bounds.Y = ToggleFilterButton.bounds.Y + ToggleFilterButton.bounds.Height + yOffset;
			}

			_filterContainerMiddleWidth = FilterButtons.Count * FilterIconSource.Width;
			FilterContainerBounds = new Rectangle(
				FilterButtons[0].bounds.X - FilterContainerSideWidth * Scale - 4,
				FilterButtons[0].bounds.Y - (FilterContainerSource.Height - FilterIconSource.Height) * Scale / 2,
				(FilterContainerSideWidth * 2 + _filterContainerMiddleWidth) * Scale,
				FilterContainerSource.Height * Scale);
			
			// Recipe nav buttons
			NavLeftButton.bounds.X = _leftContent.X - 24;
			NavRightButton.bounds.X = NavLeftButton.bounds.X + _lineWidth - 12;
			NavRightButton.bounds.Y = NavLeftButton.bounds.Y = _leftContent.Y
				//+ (Config.CookingTakesTime ? 20 : 23);
				+ 23;

			// Ingredients item slots
			{
				const int slotsPerRow = 3;
				var w = CookingSlots[0].bounds.Width;
				var h = CookingSlots[0].bounds.Height;
				yOffset = 36;
				xOffset = 0;
				xOffsetExtra = 0;
				var extraSpace = (int)(w / 2f * (CookingSlots.Count % slotsPerRow) / 2f);
				for (var i = 0; i < CookingSlots.Count; ++i)
				{
					xOffset += w;
					if (i % slotsPerRow == 0)
					{
						if (i != 0)
							yOffset += h;
						xOffset = 0;
					}

					if (i == CookingSlots.Count - (CookingSlots.Count % slotsPerRow))
						xOffsetExtra = extraSpace;

					CookingSlots[i].bounds.X = _rightContent.X + xOffset + xOffsetExtra + 16;
					CookingSlots[i].bounds.Y = _rightContent.Y + yOffset;
				}
			}

			// Recipe icon
			RecipeIconButton.bounds.Y = NavLeftButton.bounds.Y + 4;
			RecipeIconButton.bounds.X = NavLeftButton.bounds.X + NavLeftButton.bounds.Width;

			// Cook! button
			xOffset = _rightContent.X + _cookbookRightRect.Width / 2 - MarginRight;
			yOffset = _rightContent.Y + 344;
			_cookTextMiddleWidth = Math.Max(32, CookTextSource[_locale].Width);
			CookButton.bounds = new Rectangle(
				xOffset, yOffset,
				CookTextSideWidth * Scale * 2 + _cookTextMiddleWidth * Scale,
				CookButtonSource.Height * Scale);
			CookButton.bounds.X -= (_cookTextSourceWidths[_locale] / 2 * Scale - CookTextSideWidth * Scale) + MarginLeft;

			// Cooking confirmation popup buttons
			{
				xOffset -= 160;
				yOffset -= 36;
				CookIconBounds = new Rectangle(xOffset, yOffset + 6, 90, 90);

				xOffset += 48 + CookIconBounds.Width;
				CookQuantityUpButton.bounds.X = CookQuantityDownButton.bounds.X = xOffset;
				CookQuantityUpButton.bounds.Y = yOffset - 12;

				var textSize = _quantityTextBox.Font.MeasureString(
					Game1.parseText("999", _quantityTextBox.Font, 96));
				_quantityTextBox.Text = QuantityTextBoxDefaultText;
				_quantityTextBox.limitWidth = false;
				_quantityTextBox.Width = (int)textSize.X + 24;

				var extraSpace = (_quantityTextBox.Width - CookQuantityUpButton.bounds.Width) / 2;
				_quantityTextBox.X = CookQuantityUpButton.bounds.X - extraSpace;
				_quantityTextBox.Y = CookQuantityUpButton.bounds.Y + CookQuantityUpButton.bounds.Height + 7;
				_quantityTextBox.Update();
				_quantityTextBoxBounds = new Rectangle(_quantityTextBox.X, _quantityTextBox.Y, _quantityTextBox.Width,
						_quantityTextBox.Height);

				CookQuantityDownButton.bounds.Y = _quantityTextBox.Y + _quantityTextBox.Height + 5;

				CookConfirmButton.bounds.X = CookCancelButton.bounds.X
					= CookQuantityUpButton.bounds.X + CookQuantityUpButton.bounds.Width + extraSpace + 16;
				CookConfirmButton.bounds.Y = yOffset - 16;
				CookCancelButton.bounds.Y = CookConfirmButton.bounds.Y + CookConfirmButton.bounds.Height + 4;

				QuantityScrollableArea = new Rectangle(CookIconBounds.X, CookIconBounds.Y,
					CookConfirmButton.bounds.X + CookConfirmButton.bounds.Width - CookIconBounds.X, CookIconBounds.Height);
			}

			// Inventory
			var isHorizontal = UseHorizontalInventoryButtonArea;
			inventory.xPositionOnScreen = xPositionOnScreen + CookbookSource.Width / 2 * Scale - inventory.width / 2 + (isHorizontal ? 16 : 0);
			inventory.yPositionOnScreen = yPositionOnScreen + CookbookSource.Height * Scale + 8 - 20;

			// Inventory items
			{
				yOffset = -4;
				var rowSize = inventory.capacity / inventory.rows;
				for (var i = 0; i < inventory.capacity; ++i)
				{
					if (i % rowSize == 0 && i != 0)
						yOffset += inventory.inventory[i].bounds.Height + 4;
					inventory.inventory[i].bounds.X = inventory.xPositionOnScreen + i % rowSize * inventory.inventory[i].bounds.Width;
					inventory.inventory[i].bounds.Y = inventory.yPositionOnScreen + yOffset;
				}
			}

			// Inventory nav buttons
			// nav buttons flow vertically in a solo-screen game, and horizontally in split-screen
			if (InventorySelectButtons.Count > 1)
			{
				const int areaPadding = 3 * Scale;
				var itemSpacing = 4 * Scale;
				var addedSpacing = 2 * Scale;

				// Backpack and fridge
				{
					InventorySelectButtons[0].bounds.X =
						(isHorizontal
							? (int)(centre.X + 128 - (((InventorySelectButtons.Count + 1) / 2) * ((InventorySelectButtons[0].bounds.Width + 4) / 2)))
							: upperRightCloseButton.bounds.X);
					InventorySelectButtons[1].bounds.X = InventorySelectButtons[0].bounds.X
						+ (isHorizontal
							? 0
							: InventorySelectButtons[0].bounds.Width);

					InventorySelectButtons[0].bounds.Y = inventory.yPositionOnScreen + 1 * Scale
						+ (isHorizontal
							? inventory.height + itemSpacing + addedSpacing
							: (3 - 2 * ((InventorySelectButtons.Count + 1) / 2) / 2) * InventorySelectButtons[0].bounds.Height / 2);
					InventorySelectButtons[1].bounds.Y = InventorySelectButtons[0].bounds.Y
						+ (isHorizontal
							? InventorySelectButtons[0].bounds.Height + itemSpacing
							: 0);
				}

				// Mini-fridges
				for (var i = 2; i < InventorySelectButtons.Count; ++i)
				{
					var shortSideIndex = i % 2;
					var shortSidePlacement = 0;
					var longSideIndex = 0;
					var longSidePlacement = i / 2;
					InventorySelectButtons[i].bounds.X =
						InventorySelectButtons[(isHorizontal ? longSideIndex : shortSideIndex)].bounds.X
						+ (InventorySelectButtons[0].bounds.Width * (isHorizontal ? longSidePlacement : shortSidePlacement)) + (isHorizontal ? addedSpacing : 0);
					InventorySelectButtons[i].bounds.Y =
						InventorySelectButtons[(isHorizontal ? shortSideIndex : longSideIndex)].bounds.Y
						+ (InventorySelectButtons[0].bounds.Height * (isHorizontal ? shortSidePlacement : longSidePlacement)) + (isHorizontal ? 0 : addedSpacing);
				}

				// Rectangle only exists to track user scrollwheel actions
				var longSideLength = 2 * ((InventorySelectButtons.Count + 1) / 2) / 2;
				var wideSideLength = (InventorySelectButtons.Count > 1 ? 2 : 1);
				var xLength = (isHorizontal ? longSideLength : wideSideLength);
				var yLength = (isHorizontal ? wideSideLength : longSideLength);
				InventoriesScrollableArea = new Rectangle(
					InventorySelectButtons[0].bounds.X - areaPadding,
					InventorySelectButtons[0].bounds.Y - areaPadding,
					(InventorySelectButtons[0].bounds.Width + 4) * xLength + addedSpacing,
					(InventorySelectButtons[0].bounds.Height + itemSpacing) * yLength + (isHorizontal ? addedSpacing : 0));
			}
			
			AutofillButton.bounds.Y = inventory.yPositionOnScreen + (inventory.height - AutofillButton.bounds.Height) / 2 - 8;
			AutofillButton.bounds.X = inventory.xPositionOnScreen - AutofillButton.bounds.Width - 48;
			AutofillButton.sourceRect.X = IsUsingAutofill
				? AutofillButtonSource.X + AutofillButtonSource.Width
				: AutofillButtonSource.X;
		}

		/// <summary>
		/// Checks whether an item can be used in cooking recipes.
		/// Doesn't check for edibility; oil, vinegar, jam, honey, etc are inedible.
		/// </summary>
		public static bool CanBeCooked(Item i)
		{
			return !(i == null || i is Tool || i is Furniture || i is Ring || i is Clothing || i is Boots || i is Hat || i is Wallpaper
				|| i.Category < -90 || i.isLostItem || !i.canBeTrashed()
				|| (i is StardewValley.Object o && (o.bigCraftable.Value || o.specialItem)));
		}

		/// <summary>
		/// Finds all items able to be used in the place of the given ID, and aggregates their stack counts.
		/// </summary>
		/// <param name="id">The required item's identifier for the recipe, given as an index or category.</param>
		/// <param name="items">List of items to seek a match in.</param>
		/// <returns>Accumulated stack size of all matching ingredients.</returns>
		public static int GetIngredientsCount(int id, IList<Item> items)
		{
			var matches = items.Where(item => item != null && (item.ParentSheetIndex == id
				|| item.Category == id || (CanBeCooked(item) && CraftingRecipe.isThereSpecialIngredientRule((StardewValley.Object)item, id))));
			var count = matches.Count() == 0 ? 0 : matches.Aggregate(0, (current, item) => current + item.Stack);
			return count;
		}

		/// <summary>
		/// Find an item in a list that works as an ingredient or substitute in a cooking recipe for some given requirement.
		/// </summary>
		/// <param name="id">The required item's identifier for the recipe, given as an index or category.</param>
		/// <param name="items">List of items to seek a match in.</param>
		/// <param name="item">Returns matching item for the identifier, null if not found.</param>
		public static void GetMatchingIngredient(int id, IList<Item> items, out Item item)
		{
			item = null;
			for (var j = 0; j < items.Count && item == null; ++j)
			{
				if (CanBeCooked(items[j])
					&& (items[j].ParentSheetIndex == id
						|| items[j].Category == id
						|| CraftingRecipe.isThereSpecialIngredientRule((StardewValley.Object)items[j], id)))
				{
					item = items[j];
				}
			}
		}

		/// <summary>
		/// Identify items to consume from the ingredients dropIn.
		/// Abort before consuming if required items are not found.
		/// </summary>
		/// <param name="recipe">Recipe tracking which items are required, and in what quantities.</param>
		/// <param name="items">List to mark items to be consumed from.</param>
		/// <returns>List indexes of items to be crafted and their quantities to be consumed, null if not all required items were found.</returns>
		public static Dictionary<int, int> ChooseItemsForCrafting(CraftingRecipe recipe, IList<Item> items)
		{
			var items1 = items.ToList();
			var currentItems = items1.TakeWhile(_ => true).ToList();
			var craftingItems = new Dictionary<int, int>();
			foreach (var requiredItem in recipe.recipeList)
			{
				var identifier = requiredItem.Key;
				var requiredCount = requiredItem.Value;
				var craftingCount = 0;
				while (craftingCount < requiredCount)
				{
					GetMatchingIngredient(identifier, currentItems, out var item);
					if (item == null)
						return null;
					var consumed = Math.Min(requiredCount, item.Stack);
					craftingCount += consumed;
					var index = items1.FindIndex(i =>
						i != null && i.ParentSheetIndex == item.ParentSheetIndex && i.Stack == item.Stack);
					craftingItems.Add(index, consumed);
					currentItems.Remove(item);
				}
			}
			return craftingItems;
		}

		/// <summary>
		/// Determines the number of times the player can craft a cooking recipe by consuming required items.
		/// Returns 0 if any ingredients are missing entirely.
		/// </summary>
		public static int GetAmountCraftable(CraftingRecipe recipe, IList<Item> inventory)
		{
			var craftableCount = -1;
			foreach (var identifier in recipe.recipeList.Keys)
			{
				var localAvailable = 0;
				GetMatchingIngredient(identifier, inventory, out var item);
				if (item == null)
					return 0;
				localAvailable += item.Stack;
				var localCount = localAvailable / recipe.recipeList[identifier];
				if (localCount < craftableCount || craftableCount == -1)
					craftableCount = localCount;
			}
			return craftableCount;
		}

		/// <summary>
		/// this is a test donation
		/// </summary>
		/// <param name="recipe"></param>
		/// <param name="inventory"></param>
		/// <param name="quantity"></param>
		/// <returns></returns>
		public static List<StardewValley.Object> CraftItemAndConsumeIngredients(CraftingRecipe recipe, ref List<Item> inventory, int quantity)
		{
			var itemsToConsume = ChooseItemsForCrafting(recipe, inventory);
			var qualityStacks = new Dictionary<int, int> { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 4, 0 } };
			var numPerCraft = recipe.numberProducedPerCraft;
			
			for (var i = 0; i < quantity && itemsToConsume != null; ++i)
			{
				// Consume ingredients
				foreach (var pair in itemsToConsume)
				{
					if ((inventory[pair.Key].Stack -= pair.Value) < 1)
						inventory[pair.Key] = null;
				}

				// Add to stack
				qualityStacks[0] += numPerCraft;

				// Apply extra portion bonuses to the amount cooked
				if (ModEntry.CookingSkillApi.HasProfession(ICookingSkillAPI.Profession.ExtraPortion)
					&& Game1.random.NextDouble() * 10 < CookingSkill.ExtraPortionChance)
				{
					qualityStacks[0] += numPerCraft;
				}

				itemsToConsume = ChooseItemsForCrafting(recipe, inventory);
			}

			// Apply oil quality bonuses to the stack choices
			var hasOilPerk = ModEntry.CookingSkillApi.HasProfession(ICookingSkillAPI.Profession.ImprovedOil);

			// Consume Oil to improve the recipe, rebalancing the stack numbers per quality item
			for (var i = qualityStacks[0] - 1; i >= 0 && inventory.Count > 0; i -= numPerCraft)
			{
				var index = inventory.FindIndex(i => i != null && i.Name.EndsWith("Oil"));
				if (index >= 0)
				{
					// Reduce the base quality stack
					qualityStacks[0] -= numPerCraft;

					// Increase higher quality stacks
					switch (inventory[index].ParentSheetIndex)
					{
						case 247: // Oil
							qualityStacks[hasOilPerk ? 2 : 1] += numPerCraft;
							break;
						case 432: // Truffle Oil
							qualityStacks[hasOilPerk ? 4 : 2] += numPerCraft;
							break;
						default: // Oils not yet discovered by science
							qualityStacks[hasOilPerk ? 4 : 2] += numPerCraft;
							break;
					}

					// Remove consumed oil
					if (--inventory[index].Stack < 1)
						inventory[index] = null;
				}
			}

			// TODO: 1.0.12: Qi Seasoning for improved quality
			for (var i = qualityStacks[0] - 1; i >= 0 && inventory.Count > 0; i -= numPerCraft)
			{
				var index = inventory.FindIndex(i => i != null && i.ParentSheetIndex == 917);
				if (index >= 0)
				{
					// Reduce the base quality stack
					qualityStacks[0] -= numPerCraft;

					// Increase gold quality stack
					qualityStacks[2] += numPerCraft;

					// Remove consumed seasoning
					if (--inventory[index].Stack < 1)
						inventory[index] = null;
				}
			}

			// Apply burn chance to destroy cooked food at random
			var burntCount = 0;
			var qualities = qualityStacks.Keys.ToList();
			foreach (var quality in qualities)
			{
				for (var i = qualityStacks[quality] - 1; i >= 0; i -= numPerCraft)
				{
					if (GetBurnChance(recipe) > Game1.random.NextDouble())
					{
						qualityStacks[quality] -= numPerCraft;
						++burntCount;
					}
				}
			}

			// Create item list from quality stacks
			var itemsCooked = new List<StardewValley.Object>();
			foreach (var pair in qualityStacks.Where(pair => pair.Value > 0))
			{
				var item = recipe.createItem() as StardewValley.Object;
				item.Quality = pair.Key;
				item.Stack = pair.Value;

				// Apply sale price bonuses to the cooked items
				if (ModEntry.CookingSkillApi.HasProfession(ICookingSkillAPI.Profession.SaleValue))
				{
					item.Price += item.Price * CookingSkill.SaleValue / 100;
				}

				itemsCooked.Add(item);
			}

			_lastBurntCount = burntCount;
			return itemsCooked;
		}

		public static float GetBurnChance(CraftingRecipe recipe)
		{
			if (!Config.FoodCanBurn || ModEntry.JsonAssets == null)
				return 0f;

			var results = new List<double>();
			for (var i = 0d; i < 5d; ++i)
			{
				results.Add(Math.Log(i, Math.E));
			}

			var cookingLevel = ModEntry.CookingSkillApi.GetLevel();
			var baseRate = 0.22f;
			var chance = Math.Max(0f, (baseRate + 0.0035f * recipe.getNumberOfIngredients())
				- cookingLevel * CookingSkill.BurnChanceModifier * CookingSkill.BurnChanceReduction
				- (ModEntry.Instance.CookingToolLevel / 2f) * CookingSkill.BurnChanceModifier * CookingSkill.BurnChanceReduction);

			return chance;
		}

		private static int CookRecipe(CraftingRecipe recipe, ref List<Item> inventory, int quantity)
		{
			// Craft items to be cooked from recipe
			var itemsCooked = CraftItemAndConsumeIngredients(recipe, ref inventory, quantity);
			var quantityCooked = (itemsCooked.Sum(item => item.Stack) / recipe.numberProducedPerCraft) - _lastBurntCount;
			var item = recipe.createItem();

			// Track experience for items cooked
			if (Config.AddCookingSkillAndRecipes)
			{
				if (!ModEntry.FoodCookedToday.ContainsKey(recipe.name))
					ModEntry.FoodCookedToday[recipe.name] = 0;
				ModEntry.FoodCookedToday[recipe.name] += quantity;

				ModEntry.CookingSkillApi.CalculateExperienceGainedFromCookingItem(item: item, recipe.getNumberOfIngredients(), quantityCooked, applyExperience: true);
				Game1.player.cookedRecipe(item.ParentSheetIndex);

				// Update game stats
				Game1.stats.ItemsCooked += (uint) quantityCooked;
				Game1.player.checkForQuestComplete(null, -1, -1, item, null, 2);
				Game1.stats.checkForCookingAchievements();
			}

			// Add cooked items to inventory if possible
			foreach (var cookedItem in itemsCooked)
			{
				ModEntry.AddOrDropItem(cookedItem);
			}

			// Add burnt items
			if (_lastBurntCount > 0)
			{
				var burntItem = new StardewValley.Object(ModEntry.JsonAssets.GetObjectId(ModEntry.ObjectPrefix + "burntfood"), _lastBurntCount);
				ModEntry.AddOrDropItem(burntItem);
			}

			return _lastBurntCount;
		}

		/// <summary>
		/// Pre-flight checks before calling CookRecipe.
		/// </summary>
		/// <returns>Whether or not any food was crafted.</returns>
		public static bool TryToCookRecipe(CraftingRecipe recipe, ref List<Item> inventory, int quantity)
		{
			var craftableCount = inventory != null ? Math.Min(quantity, GetAmountCraftable(recipe, inventory)) : quantity;
			if (craftableCount < 1)
				return false;

			var burntCount = CookRecipe(recipe, ref inventory, craftableCount);
			if (Config.PlayCookingAnimation)
			{
				if (Game1.activeClickableMenu is CookingMenu cookingMenu)
				{
					Game1.displayHUD = true;
					AnimateForRecipe(recipe, quantity, burntCount, recipe.recipeList.Any(pair => new StardewValley.Object(pair.Key, 0).Category == -4));
					cookingMenu.PopMenuStack(playSound: false, tryToQuit: true);
				}
			}
			else
			{
				Game1.playSound("reward");
			}

			return true;
		}

		internal static List<FarmerSprite.AnimationFrame> AnimateForRecipe(CraftingRecipe recipe, int quantity, int burntCount, bool containsFish)
		{
			// TODO: FIX: Why doesn't HUD draw while animating
			Game1.freezeControls = true;
			//Game1.displayHUD = true;
			//Game1.activeClickableMenu = null; // not work!

			var name = recipe.name.ToLower();
			var isBaked = ModEntry.BakeyFoods.Any(o => name.StartsWith(o) || ModEntry.CakeyFoods.Any(o => name.EndsWith(o)));
			string startSound, sound, endSound;
			if (ModEntry.SoupyFoods.Any(x => name.EndsWith(x)))
			{
				startSound = "dropItemInWater";
				sound = "dropItemInWater";
				endSound = "bubbles";
			}
			else if (ModEntry.DrinkyFoods.Any(x => name.EndsWith(x)))
			{
				startSound = "Milking";
				sound = "dropItemInWater";
				endSound = "bubbles";
			}
			else if (ModEntry.SaladyFoods.Any(x => name.EndsWith(x)))
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

			var saloon = Game1.currentLocation.Name == "Saloon";
			var direction = saloon ? 1 : 0;
			Game1.player.Halt();
			Game1.player.FarmerSprite.StopAnimation();
			Game1.player.completelyStopAnimatingOrDoingAction();
			Game1.player.faceDirection(direction);

			var multiplayer = ModEntry.Instance.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
			var spritePosition = Vector2.Zero;
			TemporaryAnimatedSprite sprite = null;
			Game1.currentLocation.removeTemporarySpritesWithID(SpriteId);

			var ms = 330;
			var frames = startSound == "Milking"
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
			if (isBaked && !ModEntry.PancakeyFoods.Any(o => name.Contains(o)))
			{
				frames[frames.Count - 1] = new FarmerSprite.AnimationFrame(58, ms * 2);
				frames.Add(new FarmerSprite.AnimationFrame(44, ms * 8)
				{
					frameEndBehavior = delegate
					{
						Game1.playSound("fireball");
						Game1.player.FacingDirection = direction;
					}
				});
				frames.Add(new FarmerSprite.AnimationFrame(58, ms * 2));
				frames.Add(new FarmerSprite.AnimationFrame(0, ms));
			}

			// Dough-tossing foods
			if (ModEntry.PizzayFoods.Any(o => name.Contains(o)))
			{
				Game1.player.faceDirection(2);

				ms = 100;

				// Before jumble
				var newFrames = new List<FarmerSprite.AnimationFrame>
				{
					// Toss dough
					new FarmerSprite.AnimationFrame(54, 0)
					{
						frameEndBehavior = delegate
						{
							multiplayer.broadcastSprites(Game1.currentLocation, sprite);
							ModEntry.Instance.Helper.Events.Display.RenderedWorld += ModEntry.Event_RenderTempSpriteOverWorld;
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
						{ frameEndBehavior = delegate { Game1.player.FacingDirection = direction; } },
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
			else if (ModEntry.PancakeyFoods.Any(o => name.Contains(o)))
			{
				ms = 100;

				// After jumble
				var newFrames = new List<FarmerSprite.AnimationFrame>
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
							ModEntry.Instance.Helper.Events.Display.RenderedWorld += ModEntry.Event_RenderTempSpriteOverWorld;
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
						//Game1.playSound("reward");
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

			SearchTabButton.sourceRect.X = SearchTabButtonSource.X + SearchTabButtonSource.Width;
			IngredientsTabButton.sourceRect.X = IngredientsTabButtonSource.X;
			_filteredRecipeList = FilterRecipes();
			_showSearchFilters = false;
			_searchBarTextBox.Text = i18n.Get("menu.cooking_recipe.search_label");

			if (Game1.options.SnappyMenus)
			{
				setCurrentlySnappedComponentTo(_searchRecipes.Count > 0
					? ModEntry.Instance.IsUsingRecipeGridView
						? SearchGridClickables.First().myID
						: SearchListClickables.First().myID
					: SearchBarClickable.myID);
			}
		}

		private void OpenRecipePage()
		{
			if (_stack.Count > 0 && _stack.Peek() == State.Recipe)
				_stack.Pop();
			_stack.Push(State.Recipe);

			SearchTabButton.sourceRect.X = SearchTabButtonSource.X;
			IngredientsTabButton.sourceRect.X = IngredientsTabButtonSource.X;

			if (Game1.options.SnappyMenus)
			{
				setCurrentlySnappedComponentTo(IsUsingAutofill
					? ShouldDrawCookButton()
						? CookButton.myID
						: 0 // First element in inventory
					: 0);
			}
		}

		private void CloseRecipePage()
		{
			if (_stack.Count > 0 && _stack.Peek() == State.Recipe)
				_stack.Pop();

			SearchTabButton.sourceRect.X = SearchTabButtonSource.X + SearchTabButtonSource.Width;
			IngredientsTabButton.sourceRect.X = IngredientsTabButtonSource.X;
			KeepRecipeIndexInSearchBounds();

			if (Game1.options.SnappyMenus)
			{
				setCurrentlySnappedComponentTo(ModEntry.Instance.IsUsingRecipeGridView
					? SearchGridClickables[_searchRecipes.Count / 2].myID
					: SearchListClickables[_searchRecipes.Count / 2].myID);
			}
		}

		private void OpenIngredientsPage()
		{
			if (_stack.Count > 0 && _stack.Peek() == State.Ingredients)
				_stack.Pop();
			_stack.Push(State.Ingredients);
			
			SearchTabButton.sourceRect.X = SearchTabButtonSource.X;
			IngredientsTabButton.sourceRect.X = SearchTabButtonSource.X + SearchTabButtonSource.Width;
		}

		private void CloseIngredientsPage()
		{
			if (_stack.Count > 0 && _stack.Peek() == State.Ingredients)
				_stack.Pop();

			IngredientsTabButton.sourceRect.X = IngredientsTabButtonSource.X;

			if (_stack.Count > 0 && _stack.Peek() == State.Search)
			{
				_filteredRecipeList = FilterRecipes(_lastFilterUsed, _searchBarTextBox.Text);
			}
		}

		private void CloseTextBox(TextBox textBox)
		{
			textBox.Selected = false;
			Game1.keyboardDispatcher.Subscriber = null;

			if (textBox.Text == _searchBarTextBox.Text)
			{
				_filteredRecipeList = FilterRecipes(_lastFilterUsed, substr: _searchBarTextBox.Text);
				UpdateSearchRecipes();
			}
		}

		private List<CraftingRecipe> ReverseRecipeList(List<CraftingRecipe> recipes)
		{
			ModEntry.LastFilterReversed = !ModEntry.LastFilterReversed;
			recipes.Reverse();
			_currentRecipe = _searchRecipes.Count / 2;
			return recipes;
		}

		private List<CraftingRecipe> FilterRecipes(Filter which = Filter.Alphabetical,
			string substr = null)
		{
			ModEntry.LastFilterReversed = false;
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
						|| (ModEntry.Instance.FoodsEaten.Contains(recipe.name)))
						&& Game1.objectInformation[recipe.createItem().ParentSheetIndex].Split('/').Length > 6
						&& Game1.objectInformation[recipe.createItem().ParentSheetIndex].Split('/')[7].Split(' ').Any(i => int.Parse(i) != 0);
					break;
				case Filter.New:
					filter = recipe => !Game1.player.recipesCooked.ContainsKey(recipe.createItem().ParentSheetIndex);
					break;
				case Filter.Ready:
					filter = recipe => recipe.doesFarmerHaveIngredientsInInventory()
						|| (Game1.currentLocation is FarmHouse farmHouse
							&& recipe.doesFarmerHaveIngredientsInInventory(farmHouse.fridge.Value.items))
						|| (Game1.currentLocation is CommunityCenter cc
							&& recipe.doesFarmerHaveIngredientsInInventory(((Chest)cc.Objects[ModEntry.DummyFridgePosition]).items))
						|| (_minifridgeList.Any(mf => recipe.doesFarmerHaveIngredientsInInventory(mf)));
					break;
				case Filter.Favourite:
					filter = recipe => ModEntry.Instance.FavouriteRecipes.Contains(recipe.name);
					break;
				default:
					order = recipe => recipe.DisplayName;
					break;
			}

			var recipes = (order != null
				? _unlockedCookingRecipes.OrderBy(order)
				: _unlockedCookingRecipes.Where(filter)).ToList();
			if (!string.IsNullOrEmpty(substr) && substr != i18n.Get("menu.cooking_recipe.search_label"))
			{
				substr = substr.ToLower();
				recipes = recipes.Where(recipe => recipe.DisplayName.ToLower().Contains(substr)).ToList();
			}

			if (recipes.Count < 1)
				recipes.Add(new CraftingRecipe("none", true));

			if (_searchRecipes != null)
			{
				UpdateSearchRecipes();
				_currentRecipe = _searchRecipes.Count / 2;
			}

			_lastFilterUsed = which;

			return recipes;
		}

		// TODO: POLISH: Find a very suitable position for UpdateSearchRecipes() call, rather than in draw()
		private void UpdateSearchRecipes()
		{
			NavUpButton.bounds.Y = _showSearchFilters
				? SearchButton.bounds.Y + SearchButton.bounds.Height + 16 + FilterContainerBounds.Height
				: SearchButton.bounds.Y + SearchButton.bounds.Height + 16;

			_searchRecipes.Clear();
			SearchResultsArea.Y = NavUpButton.bounds.Y - 8;
			SearchResultsArea.Height = NavDownButton.bounds.Y + NavDownButton.bounds.Height - NavUpButton.bounds.Y + 16;

			var isGridView = ModEntry.Instance.IsUsingRecipeGridView;
			_recipeHeight = isGridView
				? RecipeGridHeight
				: RecipeListHeight;
			_recipesPerPage = isGridView
				? (SearchResultsArea.Width / _recipeHeight) * (SearchResultsArea.Height / _recipeHeight)
				: SearchResultsArea.Height / _recipeHeight;
			var minRecipe = Math.Max(0, _currentRecipe - _recipesPerPage / 2);
			var maxRecipe = Math.Min(_filteredRecipeList.Count, minRecipe + _recipesPerPage);

			for (var i = minRecipe; i < maxRecipe; ++i)
				_searchRecipes.Add(_filteredRecipeList[i]);
			while (isGridView && _searchRecipes.Count % 4 != 0)
				_searchRecipes.Add(null);
		}

		private void ToggleCookingConfirmPopup(bool playSound)
		{
			_showCookingConfirmPopup = !_showCookingConfirmPopup;
			_quantityTextBox.Text = QuantityTextBoxDefaultText.PadLeft(2, ' ');
			if (playSound)
				Game1.playSound(_showCookingConfirmPopup ? "bigSelect" : "bigDeSelect");

			if (Game1.options.SnappyMenus)
			{
				setCurrentlySnappedComponentTo(_showCookingConfirmPopup
					? CookConfirmButton.myID
					: CookingSlots.First().myID);
			}
		}

		private void ToggleFilterPopup(bool playSound)
		{
			_showSearchFilters = !_showSearchFilters;
			if (playSound)
				Game1.playSound("shwip");

			foreach (var clickable in SearchGridClickables)
				clickable.bounds.Y += (_showSearchFilters ? 1 : -1) * clickable.bounds.Height;
			foreach (var clickable in SearchListClickables)
				clickable.bounds.Y += (_showSearchFilters ? 1 : -1) * clickable.bounds.Height;

			if (Game1.options.SnappyMenus)
			{
				setCurrentlySnappedComponentTo(_showSearchFilters ? FilterButtons[0].myID : ToggleFilterButton.myID);
			}
		}

		private void ValidateNumericalTextBox(TextBox sender)
		{
			int.TryParse(sender.Text.Trim(), out var value);
			value = value > 0 ? value : 1;
			sender.Text = Math.Max(1, Math.Min(99,
				Math.Min(value, GetAmountCraftable(_filteredRecipeList[_currentRecipe], _cookingSlotsDropIn)))).ToString();
			sender.Text = sender.Text.PadLeft(sender.Text.Length == 2 ? 3 : 2, ' ');
			sender.Selected = false;
		}

		private void KeepRecipeIndexInSearchBounds()
		{
			_currentRecipe = Math.Max(_searchRecipes.Count / 2,
				Math.Min(_filteredRecipeList.Count - _searchRecipes.Count / 2 - 1, _currentRecipe));

			// Avoid showing whitespace after end of list
			if (ModEntry.Instance.IsUsingRecipeGridView)
			{
				_currentRecipe = 4 * (_currentRecipe / 4) + 4;
				if (_filteredRecipeList.Count - 1 - _currentRecipe < _searchRecipes.Count / 2)
				{
					_currentRecipe -= 4;
				}
			}
			else
			{
				if (_filteredRecipeList.Count - _currentRecipe <= (_searchRecipes.Count + 1) / 2)
					--_currentRecipe;
			}
		}

		private void ChangeCurrentRecipe(int index)
		{
			index = Math.Max(0, Math.Min(_filteredRecipeList.Count - 1, index));
			ChangeCurrentRecipe(_filteredRecipeList[index].name);
		}

		private void ChangeCurrentRecipe(string name)
		{
			var recipe = new CraftingRecipe(name, isCookingRecipe: true);
			_currentRecipe = _filteredRecipeList.FindIndex(recipe => recipe.name == name);
			_recipeItem = recipe.createItem();
			var info = Game1.objectInformation[_recipeItem.ParentSheetIndex]
				.Split('/');
			var buffs = info.Length < 7
				? null
				: info[7].Split(' ').ToList().ConvertAll(int.Parse);
			_recipeBuffs = buffs == null || buffs.All(b => b == 0)
				? null
				: buffs;
			_recipeBuffDuration = _recipeBuffs == null || info.Length < 8
				? -1
				: (int.Parse(info[8]) * 7 / 10 / 10) * 10;

			AutoFillIngredientsFromInventory();
		}

		/// <summary>
		/// Move quantities of stacks of two items, one in the inventory, and one in the ingredients dropIn.
		/// </summary>
		/// <param name="inventoryIndex">Index of item slot in the inventory to draw from.</param>
		/// <param name="ingredientsIndex">Index of item slot in the ingredients dropIn to add to.</param>
		/// <param name="moveEntireStack">If true, the quantity moved will be as large as possible.</param>
		/// <param name="reverse">If true, stack size from the ingredients dropIn is reduced, and added to the inventory.</param>
		/// <param name="sound">Name of sound effect to play when items are moved.</param>
		/// <returns>Quantity moved from one item stack to another. May return a negative number, affected by reverse.</returns>
		private int AddToIngredientsDropIn(int inventoryIndex, int ingredientsIndex, bool moveEntireStack, bool reverse, string sound = null)
		{
			Log.D($"AddToIngredientsDropIn() => Inventory: {_currentSelectedInventory}"
				+ $"\nInventory index: {inventoryIndex}, Ingredients index: {ingredientsIndex}, Reverse: {reverse}",
				Config.DebugMode);

			// Add items to fill in empty slots at our indexes
			if (_cookingSlotsDropIn[ingredientsIndex] == null)
			{
				if (inventoryIndex == -1)
				{
					Log.D("No inventory index or ingredients dropIn index, aborting move",
						Config.DebugMode);
					return 0;
				}

				_cookingSlotsDropIn[ingredientsIndex] = inventory.actualInventory[inventoryIndex].getOne();
				_cookingSlotsDropIn[ingredientsIndex].Stack = 0;
				Log.D($"Adding {_cookingSlotsDropIn[ingredientsIndex]?.Name ?? "null" } to ingredients dropIn",
					Config.DebugMode);
			}
			if (inventoryIndex == -1)
			{
				var dropOut = _cookingSlotsDropIn[ingredientsIndex].getOne();
				dropOut.Stack = 0;
				var item = inventory.actualInventory.FirstOrDefault(i => dropOut.canStackWith(i));
				inventoryIndex = inventory.actualInventory.IndexOf(item);
				Log.D($"Removing {dropOut.Name} from ingredients dropIn",
					Config.DebugMode);
				if (item == null)
				{
					if (_currentSelectedInventory > -2)
					{
						if (inventory.actualInventory.Count > 35)
						{
							Log.D($"Failed to return item {dropOut.Name}: Fridge inventory full",
								Config.DebugMode);
							Game1.playSound("cancel");
							return 0;
						}
						else
						{
							item = inventory.actualInventory.FirstOrDefault(i => dropOut.canStackWith(i));
							inventoryIndex = item == null
								? inventory.actualInventory.ToList().FindIndex(i => i == null)
								: inventory.actualInventory.IndexOf(item);
							if (inventoryIndex < 0)
							{
								Log.D($"Adding item {dropOut.Name} to new fridge slot",
									Config.DebugMode);
								inventoryIndex = inventory.actualInventory.Count;
								inventory.actualInventory.Add(_cookingSlotsDropIn[ingredientsIndex]);
							}
							else
							{
								Log.D($"Returning item {dropOut.Name} to some fridge slot {inventoryIndex}",
									Config.DebugMode);
								inventory.actualInventory[inventoryIndex] = _cookingSlotsDropIn[ingredientsIndex];
							}
						}
					}
					else if (inventoryIndex >= 0)
					{
						Log.D($"Returning {dropOut.Name} to inventory at {inventoryIndex}",
							Config.DebugMode);
						inventory.actualInventory[inventoryIndex] = dropOut;
					}
					else
					{
						for (var i = 0; i < inventory.actualInventory.Count && inventoryIndex < 0; ++i)
						{
							if (inventory.actualInventory[i] == null)
								inventoryIndex = i;
						}
						if (inventoryIndex > 0)
						{
							Log.D($"Returning {dropOut.Name} to inventory at {inventoryIndex} after double-check",
								Config.DebugMode);
							inventory.actualInventory[inventoryIndex] = dropOut;
						}
						else
						{
							Log.D($"Failed to return item {dropOut.Name}: No player inventory slot found",
								Config.DebugMode);
							Game1.playSound("cancel");
							return 0;
						}
					}
				}
			}

			var addTo = !reverse
				? _cookingSlotsDropIn[ingredientsIndex]
				: inventory.actualInventory[inventoryIndex];
			var takeFrom = !reverse
				? inventory.actualInventory[inventoryIndex]
				: _cookingSlotsDropIn[ingredientsIndex];

			// Contextual goal quantity mimics the usual vanilla inventory dropIn interactions
			// (left-click moves entire stack, right-click moves one from stack, shift-right-click moves half the stack)
			var quantity = 0;
			if (addTo != null && takeFrom != null)
			{
				var max = addTo.maximumStackSize();
				quantity = moveEntireStack
					? takeFrom.Stack
					: Game1.isOneOfTheseKeysDown(Game1.oldKBState, new[] { new InputButton(Keys.LeftShift) })
						? (int)Math.Ceiling(takeFrom.Stack / 2.0)
						: 1;
				// Actual quantity is limited by the dest stack limit and source stack quantity
				quantity = Math.Min(quantity, max - addTo.Stack);
			}
			// If quantity is 0, we've probably reached these limits
			if (quantity == 0)
			{
				inventory.ShakeItem(inventory.actualInventory[inventoryIndex]);
				Game1.playSound("cancel");
			}
			// Add/subtract quantities from each stack, and remove items with empty stacks
			else
			{
				if (reverse)
					quantity *= -1;

				if ((_cookingSlotsDropIn[ingredientsIndex].Stack += quantity) < 1)
					_cookingSlotsDropIn[ingredientsIndex] = null;
				if ((inventory.actualInventory[inventoryIndex].Stack -= quantity) < 1)
					if (_currentSelectedInventory == -2)
						inventory.actualInventory[inventoryIndex] = null;
					else
						inventory.actualInventory.RemoveAt(inventoryIndex);

				if (!string.IsNullOrEmpty(sound))
					Game1.playSound(sound);
			}

			return quantity;
		}

		private void AutoFillIngredientsFromInventory()
		{
			if (!IsUsingAutofill)
				return;

			// Remove all items from ingredients slots
			ReturnIngredientsToInventory();

			// Don't fill slots if the player can't cook the recipe
			if (_currentRecipe < 0 || _cookingSlots < _filteredRecipeList[_currentRecipe].recipeList.Count
				|| GetAmountCraftable(_filteredRecipeList[_currentRecipe], inventory.actualInventory) < 1)
				return;

			// todo: 1.0.11: it just ate 499 eggs and 200 flour

			// Fill slots with ingredients
			if (_currentRecipe >= 0 && _filteredRecipeList.Count >= _currentRecipe - 1)
			{
				var itemsToAdd = ChooseItemsForCrafting(_filteredRecipeList[_currentRecipe], inventory.actualInventory);
				if (itemsToAdd == null)
					return;
				foreach (var key in itemsToAdd.Keys.OrderByDescending(key => key))
					ClickedInventoryItem(inventory.actualInventory[key], key, moveEntireStack: true, sound: null);
			}
		}

		private void ReturnIngredientsToInventory()
		{
			if (_cookingSlotsDropIn.All(item => item == null))
				return;

			// Attempt to return items in ingredients dropIn slots
			for (var i = 0; i < _cookingSlotsDropIn.Count; ++i)
			{
				if (_cookingSlotsDropIn[i] != null)
					AddToIngredientsDropIn(inventoryIndex: -1, ingredientsIndex: i, moveEntireStack: true, reverse: true);
			}

			if (_cookingSlotsDropIn.All(item => item == null))
				return;

			// If any items couldn't be returned, toss them on the ground as debris
			foreach (var item in _cookingSlotsDropIn.Where(item => item != null && item.Stack > 0))
			{
				Game1.createItemDebris(item, Game1.player.Position, -1);
			}
			_cookingSlotsDropIn.ForEach(item => item = null);
		}

		private void ChangeInventory(bool isChangingToNext)
		{
			var delta = isChangingToNext ? 1 : -1;
			var index = _currentSelectedInventory;
			var hasFridge = (Game1.currentLocation is FarmHouse farmHouse && ModEntry.Instance.GetFarmhouseKitchenLevel(farmHouse) > 0)
					|| (Game1.currentLocation is CommunityCenter cc && cc.Objects.ContainsKey(ModEntry.DummyFridgePosition));
			if (hasFridge || _minifridgeList.Count > 0)
			{
				index += delta;
				if (index < -2)
					index = _minifridgeList.Count - 1;
				if (index == -1 && !hasFridge)
					index += delta;
				if (index > -1 && _minifridgeList.Count < 1)
					index = hasFridge
						? index == 0
							? -2
							: -1
						: -2;
				if (index == _minifridgeList.Count)
					index = -2;
			}

			ChangeInventory(index);
		}

		private void ChangeInventory(int index)
		{
			inventory.showGrayedOutSlots = false;
			if (index == -2)
			{
				inventory.showGrayedOutSlots = true;
				inventory.actualInventory = Game1.player.Items;
				_currentSelectedInventory = -2;
			}
			else if (index == -1)
			{
				inventory.actualInventory = Game1.currentLocation is CommunityCenter
					? ((Chest)((CommunityCenter)Game1.currentLocation).Objects[ModEntry.DummyFridgePosition]).items
					: Game1.currentLocation is FarmHouse farmHouse ? farmHouse.fridge.Value.items : null;
				_currentSelectedInventory = -1;
			}
			else
			{
				inventory.actualInventory = _minifridgeList[index];
				_currentSelectedInventory = _minifridgeList.IndexOf(inventory.actualInventory);
			}
			Log.D($"New inventory: {_currentSelectedInventory}",
				Config.DebugMode);
		}

		private void TryClickNavButton(int x, int y, bool playSound)
		{
			if (_stack.Count < 1)
				return;
			var lastRecipe = _currentRecipe;
			var state = _stack.Peek();
			var isGridView = ModEntry.Instance.IsUsingRecipeGridView;
			var max = _filteredRecipeList.Count - 1;
			if (isGridView)
			{
				max = 4 * (max / 4) + 4;
			}
			var delta = Game1.isOneOfTheseKeysDown(Game1.oldKBState, new[] {new InputButton(Keys.LeftShift)})
				? _recipesPerPage
				: isGridView && state == State.Search ? SearchResultsArea.Width / _recipeHeight : 1;
			switch (state)
			{
				case State.Search:
					if (_searchRecipes.Count < 1)
						break;

					// Search up/down nav buttons
					if (NavUpButton.containsPoint(x, y))
					{
						_currentRecipe = Math.Max(_searchRecipes.Count / 2, _currentRecipe - delta);
					}
					else if (NavDownButton.containsPoint(x, y))
					{
						_currentRecipe = Math.Min(max - _searchRecipes.Count / 2, _currentRecipe + delta);
					}
					else
					{
						return;
					}
					break;

				case State.Recipe:
					// Recipe next/prev nav buttons
					if (NavLeftButton.containsPoint(x, y))
					{
						ChangeCurrentRecipe(_currentRecipe - delta);
						_showCookingConfirmPopup = false;
					}
					else if (NavRightButton.containsPoint(x, y))
					{
						ChangeCurrentRecipe(_currentRecipe + delta);
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
				if (currentlySnappedComponent.myID == NavLeftButton.myID || currentlySnappedComponent.myID == NavRightButton.myID)
					setCurrentlySnappedComponentTo(RecipeIconButton.myID);
				if (currentlySnappedComponent.myID == NavUpButton.myID || currentlySnappedComponent.myID == NavDownButton.myID)
					setCurrentlySnappedComponentTo(_searchRecipes.Count > 0
						? ModEntry.Instance.IsUsingRecipeGridView
							? SearchGridClickables.First().myID
							: SearchListClickables.First().myID
						: ToggleFilterButton.myID);
			}

			if (playSound && _currentRecipe != lastRecipe)
				Game1.playSound(state == State.Search ? "coin" : "newRecipe");
		}

		private void TryClickQuantityButton(int x, int y)
		{
			var delta = Game1.isOneOfTheseKeysDown(Game1.oldKBState, new[] {new InputButton(Keys.LeftShift)})
				? 10 : 1;
			int.TryParse(_quantityTextBox.Text.Trim(), out var value);
			value = value > 0 ? value : 1;

			if (CookQuantityUpButton.containsPoint(x, y))
				value += delta;
			else if (CookQuantityDownButton.containsPoint(x, y))
				value -= delta;
			else
				return;

			_quantityTextBox.Text = value.ToString();
			ValidateNumericalTextBox(_quantityTextBox);
			Game1.playSound(int.Parse(_quantityTextBox.Text) == value ? "coin" : "cancel");
			_quantityTextBox.Update();
		}

		/// <summary>
		/// Checks for any items under the cursor in either the current inventory or the ingredients dropIn slots, and moves them from one set to another if possible.
		/// </summary>
		/// <param name="moveEntireStack">Whether to move the item under the cursor in as large a quantity as possible, or to transfer a small amount from the stack.</param>
		/// <returns>Quantity of item stack moved.</returns>
		public int TryClickItem(int x, int y, bool moveEntireStack)
		{
			const string sound = "coin";
			var inventoryItem = inventory.getItemAt(x, y);
			var inventoryIndex = inventory.getInventoryPositionOfClick(x, y);

			// Add an inventory item to an ingredient slot
			var quantityMoved = ClickedInventoryItem(inventoryItem, inventoryIndex, moveEntireStack);

			// Return a dropIn ingredient item to the inventory
			for (var ingredientsIndex = 0; ingredientsIndex < _cookingSlotsDropIn.Count && quantityMoved == 0; ++ingredientsIndex)
			{
				if (!CookingSlots[ingredientsIndex].containsPoint(x, y))
					continue;
				if (ingredientsIndex >= _cookingSlots)
					return 0;

				quantityMoved = AddToIngredientsDropIn(inventoryIndex, ingredientsIndex, moveEntireStack, reverse: true, sound);
			}

			if (quantityMoved != 0 && _showCookingConfirmPopup)
				ToggleCookingConfirmPopup(playSound: false);

			if (Game1.options.SnappyMenus && ShouldDrawCookButton() && currentlySnappedComponent != null
				&& currentlySnappedComponent.myID != NavLeftButton.myID && currentlySnappedComponent.myID != NavRightButton.myID)
				setCurrentlySnappedComponentTo(CookButton.myID);

			return quantityMoved;
		}

		private int ClickedInventoryItem(Item inventoryItem, int inventoryIndex, bool moveEntireStack, string sound = "coin")
		{
			var quantityMoved = 0;

			if (inventoryItem != null && !CanBeCooked(inventoryItem))
			{
				inventory.ShakeItem(inventoryItem);
				Game1.playSound("cancel");
				return 0;
			}

			// Add an inventory item to the ingredients dropIn slots in the best available position
			for (var ingredientsIndex = 0; ingredientsIndex < _cookingSlots && inventoryItem != null && quantityMoved == 0; ++ingredientsIndex)
			{
				if (_cookingSlotsDropIn[ingredientsIndex] == null || !_cookingSlotsDropIn[ingredientsIndex].canStackWith(inventoryItem))
					continue;

				quantityMoved = AddToIngredientsDropIn(inventoryIndex, ingredientsIndex, moveEntireStack, reverse: false, sound);
			}

			// Try add inventory item to a new slot if it couldn't be stacked with any elements in dropIn ingredients slots
			if (inventoryItem != null && quantityMoved == 0)
			{
				// Ignore dropIn actions from inventory when ingredients slots are full
				var dropInIsFull = _cookingSlotsDropIn.GetRange(0, _cookingSlots).TrueForAll(i => i != null);
				var ingredientsIndex = _cookingSlotsDropIn.FindIndex(i => i == null);
				if (dropInIsFull || ingredientsIndex < 0)
				{
					inventory.ShakeItem(inventoryItem);
					Game1.playSound("cancel");
					return 0;
				}
				quantityMoved = AddToIngredientsDropIn(inventoryIndex, ingredientsIndex, moveEntireStack, reverse: false, sound);
			}

			return quantityMoved;
		}

		private int TryGetIndexForSearchResult(int x, int y)
		{
			var index = -1;
			if (!SearchResultsArea.Contains(x, y) || _recipeHeight == 0)
				return index;
			var yIndex = (y - SearchResultsArea.Y - (SearchResultsArea.Height % _recipeHeight) / 2) / _recipeHeight;
			var xIndex = (x - SearchResultsArea.X) / _recipeHeight;
			index = ModEntry.Instance.IsUsingRecipeGridView
				? yIndex * (SearchResultsArea.Width / _recipeHeight) + xIndex
				: yIndex;
			return index;
		}

		private bool ShouldDrawCookButton()
		{
			return _currentRecipe >= 0
					&& _filteredRecipeList.Count > _currentRecipe
					&& _recipeItem != null
					&& GetAmountCraftable(_filteredRecipeList[_currentRecipe], _cookingSlotsDropIn) > 0
					&& !_showCookingConfirmPopup;
		}

		private bool IsNavButtonActive(int id)
		{
			var clickable = getComponentWithID(id);
			if (clickable == null || !clickable.visible)
				return false;

			if (id == NavUpButton.myID)
				return _currentRecipe > _searchRecipes.Count / 2;
			if (id == NavLeftButton.myID)
				return _currentRecipe > 0;
			if (id == NavRightButton.myID)
				return _currentRecipe < _filteredRecipeList.Count - 1;
			if (id == NavDownButton.myID)
				return (!ModEntry.Instance.IsUsingRecipeGridView && _currentRecipe < _filteredRecipeList.Count - 3)
					|| (ModEntry.Instance.IsUsingRecipeGridView && _filteredRecipeList.Count - _currentRecipe > 7);

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
					ToggleCookingConfirmPopup(playSound: true);
					if (!tryToQuit)
						return false;
				}
				if (_showSearchFilters)
				{
					ToggleFilterPopup(playSound: true);
					if (!tryToQuit)
						return false;
				}
				if (_searchBarTextBox.Selected)
				{
					CloseTextBox(_searchBarTextBox);
					if (!tryToQuit)
						return false;
				}

				ReturnIngredientsToInventory();

				var state = _stack.Peek();
				if (state == State.Search)
				{
					_stack.Pop();
				}
				else if (state == State.Recipe)
				{
					CloseRecipePage();
				}
				else if (state == State.Ingredients)
				{
					CloseIngredientsPage();
				}

				while (tryToQuit && _stack.Count > 0)
					_stack.Pop();

				if (!readyToClose() || _stack.Count > 0)
					return false;

				if (playSound)
					Game1.playSound("bigDeSelect");

				Log.D("Closing cooking menu.",
					Config.DebugMode);

				exitThisMenuNoSound();
			}
			catch (Exception e)
			{
				Log.E($"Hit error on pop stack, emergency shutdown.\n{e}");
				emergencyShutDown();
				exitFunction();
			}
			return true;
		}

		protected override void cleanupBeforeExit()
		{
			ReturnIngredientsToInventory();

			Game1.displayHUD = true;
			base.cleanupBeforeExit();
		}

		public override void setCurrentlySnappedComponentTo(int id)
		{
			if (id == -1)
				return;

			currentlySnappedComponent = getComponentWithID(id);
			snapCursorToCurrentSnappedComponent();
		}

		public override void snapToDefaultClickableComponent()
		{
			if (_stack.Count == 0)
				return;
			var state = _stack.Peek();

			switch (state)
			{
				case State.Recipe:
					currentlySnappedComponent = RecipeIconButton;
					break;

				default:
					currentlySnappedComponent = SearchBarClickable;
					break;
			}
			snapCursorToCurrentSnappedComponent();
		}

		public override void automaticSnapBehavior(int direction, int oldRegion, int oldID)
		{
			customSnapBehavior(direction, oldRegion, oldID);
			//base.automaticSnapBehavior(direction, oldRegion, oldID);
		}

		protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
		{
			if (_stack.Count < 1)
				return;
			var state = _stack.Peek();

			if (oldRegion == 9000)
			{
				switch (direction)
				{
					// Up
					case 0:
						if (state == State.Search)
						{
							if (_searchRecipes.Count > 0)
							{
								setCurrentlySnappedComponentTo(ModEntry.Instance.IsUsingRecipeGridView
									? SearchGridClickables.Last().myID
									: SearchListClickables.Last().myID);

								/*if (ModEntry.Instance.IsUsingRecipeGridView)
								{
									for (var i = SearchGridClickables.Count - 1; i >= 0; --i)
									{
										if (TryGetIndexForSearchResult(
											SearchGridClickables[i].bounds.X,
											i * SearchGridClickables[i].bounds.Height + SearchGridClickables[i].bounds.Height / 2) >= 0)
										{
											setCurrentlySnappedComponentTo(SearchGridClickables[i].myID - 1);
											break;
										}
									}
								}
								else
								{
									var dimen = SearchListClickables.First().bounds;
									for (var i = SearchListClickables.Count - 1; i >= 0; --i)
									{
										if (TryGetIndexForSearchResult(dimen.X, i * dimen.Height + dimen.Height / 2) >= 0)
										{
											setCurrentlySnappedComponentTo(SearchListClickables[i].myID - 1);
											break;
										}
									}
								}*/
							}
							if (currentlySnappedComponent.myID < inventory.inventory.Count)
							{
								setCurrentlySnappedComponentTo(SearchBarClickable.myID);
							}
						}
						break;
					// Right
					case 1:
						if (InventorySelectButtons.Count > 0 && !Context.IsSplitScreen)
							setCurrentlySnappedComponentTo(InventorySelectButtons[0].myID);
						break;
					// Down
					case 2:
						break;
					// Left
					case 3:
						if (IsAutofillEnabled)
							setCurrentlySnappedComponentTo(AutofillButton.myID);
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
			var obj = inventory.getItemAt(x, y);
			if (CanBeCooked(obj))
			{
				inventory.hover(x, y, heldItem);
				hoveredItem = obj;
			}

			for (var i = 0; i < _cookingSlotsDropIn.Count && hoveredItem == null; ++i)
				if (CookingSlots[i].containsPoint(x, y))
					hoveredItem = _cookingSlotsDropIn[i];
			
			upperRightCloseButton.tryHover(x, y, 0.5f);

			var state = _stack.Peek();
			switch (state)
			{
				case State.Opening:
					break;

				case State.Recipe:
					// Left/right next/prev recipe navigation buttons
					NavRightButton.tryHover(x, y);
					NavLeftButton.tryHover(x, y);
					// Favourite recipe button
					RecipeIconButton.tryHover(x, y, 0.5f);
					if (!RecipeIconButton.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY())
					    && RecipeIconButton.containsPoint(x, y))
						Game1.playSound("breathin");
					else if (RecipeIconButton.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY())
						&& !RecipeIconButton.containsPoint(x, y))
						Game1.playSound("breathout");
					break;

				case State.Search:
					// Up/down recipe search results navigation buttons
					NavDownButton.tryHover(x, y);
					NavUpButton.tryHover(x, y);

					// Search button
					if (_searchBarTextBox.Selected)
					{
						SearchButton.tryHover(x, y);
						if (SearchButton.containsPoint(x,y ))
							hoverText = SearchButton.hoverText;
					}
					else
					{
						// Search buttons
						foreach (var clickable in new[] { ToggleOrderButton, ToggleFilterButton, ToggleViewButton })
						{
							clickable.tryHover(x, y, 0.2f);
							if (clickable.containsPoint(x, y))
								hoverText = clickable.hoverText;
						}

						// Search filter buttons
						if (_showSearchFilters)
						{
							foreach (var clickable in FilterButtons)
							{
								clickable.tryHover(x, y, 0.4f);
								if (clickable.containsPoint(x, y))
									hoverText = clickable.hoverText;
							}
						}
					}

					if (!ModEntry.Instance.IsUsingRecipeGridView)
						break;

					// Hover text over recipe search results when in grid view, which unlike list view, has names hidden
					var index = TryGetIndexForSearchResult(x, y);
					if (index >= 0 && index < _searchRecipes.Count && _searchRecipes[index] != null && _searchRecipes[index].name != "Torch")
						hoverText = Game1.player.knowsRecipe(_searchRecipes[index].name)
							? _searchRecipes[index].DisplayName
							: i18n.Get("menu.cooking_recipe.title_unknown");

					break;

				case State.Ingredients:

					break;
			}

			SearchTabButton.tryHover(x, y, state != State.Search ? 0.5f : 0f); // Button scale gets stuck if we don't call tryHover in State.Search
			if (IsIngredientsPageEnabled)
				IngredientsTabButton.tryHover(x, y, state != State.Ingredients ? 0.5f : 0f);

			if (_showCookingConfirmPopup)
			{
				CookQuantityUpButton.tryHover(x, y, 0.5f);
				_quantityTextBox.Hover(x, y);
				CookQuantityDownButton.tryHover(x, y, 0.5f);

				CookConfirmButton.tryHover(x, y);
				CookCancelButton.tryHover(x, y);
			}

			// Inventory nav buttons
			foreach (var clickable in InventorySelectButtons)
			{
				clickable.tryHover(x, y, 0.25f);
			}

			// Inventory autofill button
			if (IsAutofillEnabled)
			{
				AutofillButton.tryHover(x, y);
				if (AutofillButton.containsPoint(x, y))
					hoverText = AutofillButton.hoverText;
			}
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			if (_stack.Count == 0 || Game1.activeClickableMenu == null)
				return;
			var state = _stack.Peek();
			if (state == State.Opening)
				return;

			if (upperRightCloseButton.containsPoint(x, y))
			{
				PopMenuStack(playSound: false, tryToQuit: true);
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
					}
					else if (_searchBarTextBox.Selected)
					{
						if (SearchButton.containsPoint(x, y))
						{
							Game1.playSound("coin");
							_searchBarTextBox.Text = _searchBarTextBox.Text.Trim();
						}
						if (string.IsNullOrEmpty(_searchBarTextBox.Text))
							_searchBarTextBox.Text = i18n.Get("menu.cooking_recipe.search_label");
						CloseTextBox(_searchBarTextBox);
					}
					else
					{
						// Search filter buttons
						if (_showSearchFilters)
						{
							var clickable = FilterButtons.FirstOrDefault(c => c.containsPoint(x, y));
							if (clickable != null)
							{
								var which = (Filter)int.Parse(clickable.name[clickable.name.Length - 1].ToString());
								if (which == _lastFilterUsed)
								{
									_filteredRecipeList = ReverseRecipeList(_filteredRecipeList);
								}
								else
								{
									_filteredRecipeList = FilterRecipes(which, _searchBarTextBox.Text);
								}
								Game1.playSound("coin");
								ModEntry.LastFilterThisSession = which;
							}
						}
					
						// Search filter toggles
						if (ToggleFilterButton.containsPoint(x, y))
						{
							ToggleFilterPopup(playSound);
						}
						// Search results order reverse button
						else if (ToggleOrderButton.containsPoint(x, y))
						{
							_filteredRecipeList = ReverseRecipeList(_filteredRecipeList);
							Game1.playSound("shwip");
						}
						// Search results grid/list view button
						else if (ToggleViewButton.containsPoint(x, y))
						{
							var isGridView = ModEntry.Instance.IsUsingRecipeGridView;
							ToggleViewButton.sourceRect.X = ToggleViewButtonSource.X
							                                + (isGridView ? 0 : ToggleViewButtonSource.Width);

							KeepRecipeIndexInSearchBounds();

							ModEntry.Instance.IsUsingRecipeGridView = !isGridView;
							Game1.playSound("shwip");
							ToggleViewButton.hoverText =
								i18n.Get($"menu.cooking_search.view.{(isGridView ? "grid" : "list")}");
						}
					}

					var index = TryGetIndexForSearchResult(x, y);
					if (index >= 0 && index < _searchRecipes.Count && _searchRecipes[index] != null && _searchRecipes[index].name != "Torch")
					{
						Game1.playSound("shwip");
						ChangeCurrentRecipe(_searchRecipes[index].name);
						OpenRecipePage();
					}

					break;

				case State.Recipe:

					// Favourite recipe button
					if (RecipeIconButton.containsPoint(x, y))
					{
						if (ModEntry.Instance.FavouriteRecipes.Contains(_recipeItem.Name))
						{
							ModEntry.Instance.FavouriteRecipes.Remove(_recipeItem.Name);
							Game1.playSound("throwDownITem"); // not a typo
						}
						else
						{
							ModEntry.Instance.FavouriteRecipes.Add(_recipeItem.Name);
							Game1.playSound("pickUpItem");
						}
					}

					break;
			}

			// Autofill button
			if (IsAutofillEnabled && AutofillButton.containsPoint(x, y))
			{
				Game1.playSound("coin");
				IsUsingAutofill = !IsUsingAutofill;
				AutofillButton.sourceRect.X = IsUsingAutofill
					? AutofillButtonSource.X + AutofillButtonSource.Width
					: AutofillButtonSource.X;
			}
			// Search tab
			else if (state != State.Search && SearchTabButton.containsPoint(x, y))
			{
				_stack.Pop();
				OpenSearchPage();
				Game1.playSound("bigSelect");
			}
			// Ingredients tab
			else if (IsIngredientsPageEnabled && Config.AddRecipeRebalancing
			         && state != State.Ingredients && IngredientsTabButton.containsPoint(x, y))
			{
				_stack.Pop();
				OpenIngredientsPage();
				Game1.playSound("bigSelect");
			}
			// Cook! button
			else if (ShouldDrawCookButton() && CookButton.bounds.Contains(x, y))
			{
				ToggleCookingConfirmPopup(playSound: true);
			}
			else if (_showCookingConfirmPopup)
			{
				// Quantity up/down buttons
				TryClickQuantityButton(x, y);

				// Quantity text box
				if (_quantityTextBoxBounds.Contains(x, y))
				{
					_quantityTextBox.Text = "";
					Game1.keyboardDispatcher.Subscriber = _quantityTextBox;
					_quantityTextBox.SelectMe();
				}
				else if (_quantityTextBox.Selected)
				{
					ValidateNumericalTextBox(_quantityTextBox);
					CloseTextBox(_quantityTextBox);
				}

				// Cook OK/Cancel buttons
				if (CookConfirmButton.containsPoint(x, y))
				{
					ValidateNumericalTextBox(_quantityTextBox);
					if (TryToCookRecipe(_filteredRecipeList[_currentRecipe],
						ref _cookingSlotsDropIn, int.Parse(_quantityTextBox.Text.Trim())))
					{
						PopMenuStack(playSound: true);
					}
					else
					{
						Game1.playSound("cancel");
					}
				}
				else if (CookCancelButton.containsPoint(x, y))
				{
					PopMenuStack(playSound: true);
				}
			}

			// Inventory nav buttons
			foreach (var clickable in InventorySelectButtons)
			{
				if (clickable.bounds.Contains(x, y))
				{
					var index = clickable.name == "inventorySelect"
						? -2
						: clickable.name == "fridgeSelect"
							? -1
							: int.Parse(clickable.name[clickable.name.Length - 1].ToString());
					inventory.showGrayedOutSlots = false;
					ChangeInventory(index);
					Game1.playSound("coin");
				}
			}

			// Up/down/left/right contextual navigation buttons
			TryClickNavButton(x, y, playSound: true);
			// Inventory and ingredients dropIn items
			TryClickItem(x, y, moveEntireStack: true);

			UpdateSearchRecipes();

			_mouseHeldTicks = 0;
		}

		public override void receiveRightClick(int x, int y, bool playSound = true)
		{
			if (_stack.Count == 0)
				return;
			var state = _stack.Peek();
			if (state == State.Opening)
				return;

			base.receiveRightClick(x, y, playSound);

			var shouldPop = TryClickItem(x, y, moveEntireStack: false) == 0;
			
			if (_showCookingConfirmPopup && shouldPop)
			{
				ToggleCookingConfirmPopup(playSound: true);
				_quantityTextBox.Update();
			}
			else
			{
				if (_searchBarTextBox.Selected)
				{
					_searchBarTextBox.Text = i18n.Get("menu.cooking_recipe.search_label");
					CloseTextBox(_searchBarTextBox);
				}
				else if (state == State.Search)
				{
					if (!string.IsNullOrEmpty(_searchBarTextBox.Text)
						 && _searchBarTextBox.Text != i18n.Get("menu.cooking_recipe.search_label"))
					{
						_searchBarTextBox.Text = i18n.Get("menu.cooking_recipe.search_label");
					}
					_filteredRecipeList = _unlockedCookingRecipes;
				}
			}

			if (shouldPop && state != State.Search)
			{
				PopMenuStack(playSound);
				KeepRecipeIndexInSearchBounds();
			}

			UpdateSearchRecipes();
		}

		public override void leftClickHeld(int x, int y)
		{
			base.leftClickHeld(x, y);

			// Start mouse-held behaviours after a delay
			if (_mouseHeldTicks < 0 || ++_mouseHeldTicks < 30)
				return;
			_mouseHeldTicks = 20;

			// Use mouse-held behaviours on navigation and quantity buttons
			TryClickNavButton(x, y, playSound: true);
			if (_showCookingConfirmPopup)
			{
				TryClickQuantityButton(x, y);
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
			var state = _stack.Peek();
			if (state == State.Opening)
				return;

			bool isExitButton = b == Buttons.Start || b == Buttons.B || b == Buttons.Y;

			var cur = currentlySnappedComponent != null ? currentlySnappedComponent.myID : -1;

			Log.D(currentlySnappedComponent != null
				? $"GP CSC: {currentlySnappedComponent.myID} ({currentlySnappedComponent.name})"
					+ $" [{currentlySnappedComponent.leftNeighborID} {currentlySnappedComponent.upNeighborID}"
					+ $" {currentlySnappedComponent.rightNeighborID} {currentlySnappedComponent.downNeighborID}]"
				: "GP CSC: null",
				Config.DebugMode);
			
			// Contextual navigation
			var firstID = state == State.Search
				? _searchRecipes.Count > 0
					? ModEntry.Instance.IsUsingRecipeGridView
						? SearchGridClickables.First().myID
						: SearchListClickables.First().myID
					: SearchBarClickable.myID
				: RecipeIconButton.myID;
			var set = new List<int> { firstID, 0, CookingSlots.First().myID };
			var index = set.IndexOf(cur);
			if (b == Buttons.LeftShoulder)
			{
				setCurrentlySnappedComponentTo(index == -1
					? set.First()
					: index == set.Count - 1
						? set.First()
						: set[index + 1]);
			}
			if (b == Buttons.RightShoulder)
			{
				setCurrentlySnappedComponentTo(index == -1
					? set.Last()
					: index == 0
						? set.Last()
						: set[index - 1]);
			}
			if (b == Buttons.LeftTrigger)
			{
				ChangeInventory(isChangingToNext: false);
			}
			if (b == Buttons.RightTrigger)
			{
				ChangeInventory(isChangingToNext: true);
			}

			// Right thumbstick emulates navigation buttons
			if (b == Buttons.RightThumbstickUp)
				TryClickNavButton(NavUpButton.bounds.X, NavUpButton.bounds.Y, playSound: true);
			if (b == Buttons.RightThumbstickDown)
				TryClickNavButton(NavDownButton.bounds.X, NavDownButton.bounds.Y, playSound: true);
			if (b == Buttons.RightThumbstickLeft)
				TryClickNavButton(NavLeftButton.bounds.X, NavLeftButton.bounds.Y, playSound: true);
			if (b == Buttons.RightThumbstickRight)
				TryClickNavButton(NavRightButton.bounds.X, NavRightButton.bounds.Y, playSound: true);

			if (_searchBarTextBox.Selected)
			{
				// Open onscreen keyboard for search bar textbox
				if (b == Buttons.A)
					Game1.showTextEntry(_searchBarTextBox);
				if (isExitButton)
					CloseTextBox(_searchBarTextBox);
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
			var state = _stack.Peek();
			var cursor = Game1.getMousePosition();
			if (_showCookingConfirmPopup && QuantityScrollableArea.Contains(cursor))
			{
				TryClickQuantityButton(CookQuantityUpButton.bounds.X,
					direction < 0 ? CookQuantityDownButton.bounds.Y : CookQuantityUpButton.bounds.Y);
			}
			else if (inventory.isWithinBounds(cursor.X, cursor.Y) || InventoriesScrollableArea.Contains(cursor))
			{
				// Scroll wheel navigates between backpack, fridge, and minifridge inventories
				ChangeInventory(isChangingToNext: direction < 0);
			}
			else
			{
				var clickable = direction < 0
					? state == State.Search ? NavDownButton : NavRightButton
					: state == State.Search ? NavUpButton : NavLeftButton;
				TryClickNavButton(clickable.bounds.X, clickable.bounds.Y, state == State.Recipe);
			}

			UpdateSearchRecipes();
		}

		public override void receiveKeyPress(Keys key)
		{
			if (_stack.Count < 1)
				return;
			var state = _stack.Peek();
			if (state == State.Opening)
				return;

			// Contextual navigation
			if (Game1.options.SnappyMenus && currentlySnappedComponent != null)
			{
				var cur = currentlySnappedComponent.myID;
				var next = -1;
				if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
				{
					if (cur < inventory.inventory.Count && cur % GetColumnCount() == 0)
						// Snap to autofill button from leftmost inventory slots
						next = IsAutofillEnabled
							? AutofillButton.myID
							: cur;
					else if (cur == RecipeIconButton.myID)
						next = IsNavButtonActive(NavLeftButton.myID) ? NavLeftButton.myID : SearchTabButton.myID;
					else if (cur == CookingSlots.First().myID && state == State.Recipe)
						next = IsNavButtonActive(NavRightButton.myID) ? NavRightButton.myID : RecipeIconButton.myID;
					else if (cur == NavUpButton.myID || cur == NavDownButton.myID)
						next = ModEntry.Instance.IsUsingRecipeGridView
							? SearchGridClickables.First().myID
							: SearchListClickables.First().myID;
				}
				if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
				{
					if (cur == SearchTabButton.myID)
						next = IsNavButtonActive(NavLeftButton.myID) ? NavLeftButton.myID : RecipeIconButton.myID;
					else if (cur == RecipeIconButton.myID)
						next = IsNavButtonActive(NavRightButton.myID) ? NavRightButton.myID : CookingSlots.First().myID;
					else if (((ModEntry.Instance.IsUsingRecipeGridView
						&& SearchGridClickables.Any(c => c.myID == cur && int.Parse(string.Join("", c.name.Where(char.IsDigit))) % 4 == 3))
						|| SearchListClickables.Any(c => c.myID == cur)))
						next = IsNavButtonActive(NavUpButton.myID)
							? NavUpButton.myID
							: IsNavButtonActive(NavDownButton.myID)
								? NavDownButton.myID
								: cur;
				}
				if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
				{
					if (cur < inventory.inventory.Count && cur >= GetColumnCount())
						// Inventory row navigation
						next = cur - GetColumnCount();
					else if (cur < inventory.inventory.Count && state == State.Recipe)
						// Move out of inventory into crafting page
						next = ShouldDrawCookButton()
							? CookButton.myID
							: _showCookingConfirmPopup
								? CookConfirmButton.myID
								: CookingSlots.First().myID;
					else if (cur == AutofillButton.myID)
						next = state == State.Recipe
							? IsIngredientsPageEnabled ? IngredientsTabButton.myID : SearchTabButton.myID
							: SearchBarClickable.myID;
					else if (cur == NavDownButton.myID)
						next = IsNavButtonActive(NavUpButton.myID)
							? NavUpButton.myID
							: NavUpButton.upNeighborID;
				}
				if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
				{
					var set = new[] { SearchBarClickable, ToggleFilterButton, ToggleOrderButton, ToggleViewButton };
					if (set.Any(clickable => clickable.myID == cur))
						// Moving into search results from search bar
						// Doesn't include ToggleFilterButton since it inexplicably already navigates to first search result
						next = ModEntry.Instance.IsUsingRecipeGridView
							? SearchGridClickables.First().myID
							: SearchListClickables.First().myID;
					else if (cur < inventory.inventory.Count - GetColumnCount())
						// Inventory row navigation
						next = cur + GetColumnCount();
					else if (cur < inventory.inventory.Count && cur >= inventory.inventory.Count - GetColumnCount())
						// Do not scroll further down or wrap around when at bottom of inventory in solo play
						// In split-screen play, select the fridge buttons if available
						next = Context.IsSplitScreen && InventorySelectButtons.Any()
							? InventorySelectButtons.First().myID
							: cur;
					else if (cur < inventory.inventory.Count)
						// Moving into search results from inventory
						next = ModEntry.Instance.IsUsingRecipeGridView
							? SearchGridClickables.Last().myID
							: SearchListClickables.Last().myID;
					else if (cur == CookingSlots.Last().myID && state == State.Recipe)
						next = ShouldDrawCookButton()
							? CookButton.myID
							: _showCookingConfirmPopup
								? CookConfirmButton.myID
								: 0; // First element in inventory
					else if (cur == NavUpButton.myID)
						next = IsNavButtonActive(NavDownButton.myID)
							? NavDownButton.myID
							: NavDownButton.downNeighborID;
				}
				if (next != -1)
				{
					setCurrentlySnappedComponentTo(next);
					return;
				}
			}

			base.receiveKeyPress(key);

			Log.D($"KY CSC: {(currentlySnappedComponent != null ? currentlySnappedComponent.myID : -1)}"
				+ $" | ACC: {(allClickableComponents != null ? allClickableComponents.Count().ToString() : "null")}",
				Config.DebugMode);
			
			switch (state)
			{
				case State.Search:
				{
					// Navigate left/right/up/down buttons traverse search results
					if (!Game1.options.SnappyMenus)
					{
						if ((Game1.options.doesInputListContain(Game1.options.moveLeftButton, key) || Game1.options.doesInputListContain(Game1.options.moveUpButton, key)) && IsNavButtonActive(NavUpButton.myID))
							TryClickNavButton(NavUpButton.bounds.X, NavUpButton.bounds.Y, false);
						if ((Game1.options.doesInputListContain(Game1.options.moveRightButton, key) || Game1.options.doesInputListContain(Game1.options.moveDownButton, key)) && IsNavButtonActive(NavDownButton.myID))
							TryClickNavButton(NavDownButton.bounds.X, NavDownButton.bounds.Y, false);
					}
					break;
				}
				case State.Recipe:
				{
					if (!Game1.options.SnappyMenus)
					{
						// Navigate left/right buttons select recipe
						if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key) && IsNavButtonActive(NavLeftButton.myID))
							ChangeCurrentRecipe(--_currentRecipe);
						if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key) && IsNavButtonActive(NavRightButton.myID))
							ChangeCurrentRecipe(++_currentRecipe);
						// Navigate up/down buttons control inventory
						if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key) && IsNavButtonActive(NavUpButton.myID))
							ChangeInventory(isChangingToNext: false);
						if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key) && IsNavButtonActive(NavDownButton.myID))
							ChangeInventory(isChangingToNext: true);
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
						CloseTextBox(_searchBarTextBox);
						break;
					default:
						_filteredRecipeList = FilterRecipes(_lastFilterUsed, substr: _searchBarTextBox.Text);
						break;
				}
			}
			else
			{
				if (Config.DebugMode)
				{
					if (key == Keys.L)
					{
						var locales = CookTextSource.Keys.ToList();
						_locale = locales[(locales.IndexOf(_locale) + 1) % locales.Count];
						Log.D($"Changing to locale {_locale} and realigning elements");
						RealignElements();
					}
					else if (key == Keys.K)
					{
						Log.D($"Adding ingredients for {_filteredRecipeList[_currentRecipe].name}");
						var i = 0;
						foreach (var pair in _filteredRecipeList[_currentRecipe].recipeList)
						{
							if (i >= Math.Min(_cookingSlotsDropIn.Count, _filteredRecipeList[_currentRecipe].recipeList.Count))
								break;

							var id = pair.Key;
							var quantity = pair.Value;
							var item = new StardewValley.Object(id, quantity);
							_cookingSlotsDropIn[i] = item;
							++i;
						}
					}
				}

				if (Game1.options.doesInputListContain(Game1.options.menuButton, key)
					|| Game1.options.doesInputListContain(Game1.options.journalButton, key))
				{
					PopMenuStack(playSound: true);
				}

				if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && canExitOnKey)
				{
					PopMenuStack(playSound: true);
					if (Game1.currentLocation.currentEvent != null && Game1.currentLocation.currentEvent.CurrentCommand > 0)
						Game1.currentLocation.currentEvent.CurrentCommand++;
				}
			}

			UpdateSearchRecipes();
		}

		public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
		{
			base.gameWindowSizeChanged(oldBounds, newBounds);
			RealignElements();
		}

		public override void update(GameTime time)
		{
			_animTimer += time.ElapsedGameTime.Milliseconds;
			if (_animTimer >= AnimTimerLimit)
				_animTimer = 0;
			_animFrame = (int)((float)_animTimer / AnimTimerLimit * AnimFrames);

			// Expand search bar on selected, contract on deselected
			var delta = 256f / time.ElapsedGameTime.Milliseconds;
			if (_searchBarTextBox.Selected && _searchBarTextBox.Width < _searchBarTextBoxMaxWidth)
				_searchBarTextBox.Width = (int)Math.Min(_searchBarTextBoxMaxWidth, _searchBarTextBox.Width + delta);
			else if (!_searchBarTextBox.Selected && _searchBarTextBox.Width > SearchBarTextBoxMinWidth)
				_searchBarTextBox.Width = (int)Math.Max(SearchBarTextBoxMinWidth, _searchBarTextBox.Width - delta);
			_searchBarTextBoxBounds.Width = _searchBarTextBox.Width;
			SearchBarClickable.bounds.Width = _searchBarTextBoxBounds.Width;

			base.update(time);
		}

		public override void draw(SpriteBatch b)
		{
			if (_stack.Count < 1)
				return;
			var state = _stack.Peek();

			// Blackout
			b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea(), Color.Black * 0.5f);

			// Cookbook
			b.Draw(
				Texture,
				new Vector2(_cookbookLeftRect.X, _cookbookLeftRect.Y),
				CookbookSource,
				Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 1f);

			if (state == State.Recipe)
				DrawRecipePage(b);
			else if (state == State.Search)
				DrawSearchPage(b);
			else if (state == State.Ingredients)
				DrawIngredientsPage(b);
			DrawCraftingPage(b);
			DrawInventoryMenu(b);
			DrawActualInventory(b);
			DrawExtraStuff(b);

			if (false && currentlySnappedComponent != null && Config.DebugMode)
				b.Draw(Game1.fadeToBlackRect, currentlySnappedComponent.bounds, Color.Red * 0.5f);
		}

		private void DrawIngredientsPage(SpriteBatch b)
		{
			
		}

		private void DrawSearchPage(SpriteBatch b)
		{
			// Search nav buttons
			if (IsNavButtonActive(NavUpButton.myID))
				NavUpButton.draw(b);
			if (IsNavButtonActive(NavDownButton.myID))
				NavDownButton.draw(b);

			// Recipe entries
			CraftingRecipe recipe;
			string text;

			SearchResultsArea.Y = NavUpButton.bounds.Y - 8;
			SearchResultsArea.Height = NavDownButton.bounds.Y + NavDownButton.bounds.Height - NavUpButton.bounds.Y + 16;

			if (_searchRecipes.Count == 0 || _searchRecipes.Any(recipe => recipe?.name == "Torch"))
			{
				text = i18n.Get("menu.cooking_search.none_label");
				DrawText(b, text, scale: 1f,
					_leftContent.X - SearchResultsArea.X + TextSpacingFromIcons - 16,
					SearchResultsArea.Y + 64,
					SearchResultsArea.Width - TextSpacingFromIcons, isLeftSide: true);
			}
			else
			{
				if (ModEntry.Instance.IsUsingRecipeGridView)
				{
					for (var i = 0; i < _searchRecipes.Count; ++i)
					{
						recipe = _searchRecipes[i];
						if (recipe == null)
							continue;

						recipe.drawMenuView(b, SearchGridClickables[i].bounds.X, SearchGridClickables[i].bounds.Y);
					}
				}
				else
				{
					var width = SearchResultsArea.Width - TextSpacingFromIcons;
					for (var i = 0; i < _searchRecipes.Count; ++i)
					{
						recipe = _searchRecipes[i];
						if (recipe == null)
							continue;

						recipe.drawMenuView(b, SearchListClickables[i].bounds.X, SearchListClickables[i].bounds.Y);

						text = Game1.player.knowsRecipe(recipe?.name)
							? recipe.DisplayName
							: i18n.Get("menu.cooking_recipe.title_unknown");

						DrawText(b, text, scale: 1f,
							SearchListClickables[i].bounds.X - _leftContent.X + TextSpacingFromIcons,
							SearchListClickables[i].bounds.Y - (int)(Game1.smallFont.MeasureString(Game1.parseText(
								"Strawberry Cake", Game1.smallFont, SearchResultsArea.Width - TextSpacingFromIcons)).Y / 2 - RecipeListHeight / 2),
							width, isLeftSide: true);
					}
				}
			}

			// Search bar
			_searchBarTextBox.Draw(b);
			if (_searchBarTextBox.Selected)
			{
				SearchButton.draw(b);
				return;
			}

			// Search filter toggles
			foreach (var clickable in new[] { ToggleFilterButton, ToggleOrderButton, ToggleViewButton})
				if (_searchBarTextBox.X + _searchBarTextBox.Width < clickable.bounds.X)
					clickable.draw(b);
			
			if (_showSearchFilters)
			{
				// Filter clickable icons container
				// left
				b.Draw(
					Texture,
					new Rectangle(
						FilterContainerBounds.X, FilterContainerBounds.Y,
						FilterContainerSideWidth * Scale, FilterContainerBounds.Height),
					new Rectangle(
						FilterContainerSource.X, FilterContainerSource.Y,
						FilterContainerSideWidth, FilterContainerSource.Height),
					Color.White);
				// middle
				b.Draw(
					Texture,
					new Rectangle(
						FilterContainerBounds.X + FilterContainerSideWidth * Scale, FilterContainerBounds.Y,
						_filterContainerMiddleWidth * Scale, FilterContainerBounds.Height),
					new Rectangle(
						FilterContainerSource.X + FilterContainerSideWidth, FilterContainerSource.Y,
						1, FilterContainerSource.Height),
					Color.White);
				// right
				b.Draw(
					Texture,
					new Rectangle(
						FilterContainerBounds.X + FilterContainerSideWidth * Scale + _filterContainerMiddleWidth * Scale,
						FilterContainerBounds.Y,
						FilterContainerSideWidth * Scale, FilterContainerBounds.Height),
					new Rectangle(
						FilterContainerSource.X + FilterContainerSideWidth + 1, FilterContainerSource.Y,
						FilterContainerSideWidth, FilterContainerSource.Height),
					Color.White);

				// Filter clickable icons
				foreach (var clickable in FilterButtons)
					clickable.draw(b);
			}
		}

		private void DrawRecipePage(SpriteBatch b)
		{
			var cookingTime = "30";

			var xScale = _locale == "ko" && _resizeKoreanFonts ? KoWidthScale : 1f;
			var yScale = _locale == "ko" && _resizeKoreanFonts ? KoHeightScale : 1f;
			var textHeightCheck = 0f;
			var textPosition = Vector2.Zero;
			var textWidth = (int)(_textWidth * xScale);
			string text;

			// Clickables
			if (IsNavButtonActive(NavLeftButton.myID))
				NavLeftButton.draw(b);
			if (IsNavButtonActive(NavRightButton.myID))
				NavRightButton.draw(b);

			// Recipe icon and title + favourite icon
			RecipeIconButton.sourceRect = Game1.getSourceRectForStandardTileSheet(
				Game1.objectSpriteSheet, _recipeItem.ParentSheetIndex, 16, 16);
			RecipeIconButton.draw(b);
			
			if (ModEntry.Instance.FavouriteRecipes.Contains(_recipeItem.Name))
			{
				b.Draw(Texture,
					new Rectangle(
						RecipeIconButton.bounds.X + RecipeIconButton.bounds.Width / 3 * 2,
						RecipeIconButton.bounds.Y + RecipeIconButton.bounds.Height / 3 * 2,
						FavouriteIconSource.Width * 3, FavouriteIconSource.Height * 3),
					FavouriteIconSource, Color.White);
			}
			var titleScale = 1f;
			textWidth = (int)(162 * xScale);
			text = Game1.player.knowsRecipe(_filteredRecipeList[_currentRecipe].name)
				? _filteredRecipeList[_currentRecipe].DisplayName
				: i18n.Get("menu.cooking_recipe.title_unknown");
			textPosition.X = NavLeftButton.bounds.Width + 56;

			// Attempt to fix for Deutsch lange names
			if (_locale == "de" && Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).X > textWidth)
				text = text.Replace("-", "\n").Trim();

			if (Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).X * 0.8 > textWidth)
				titleScale = 0.735f;
			else if (Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).X > textWidth)
				titleScale = 0.95f;

			textPosition.Y = NavLeftButton.bounds.Y + 4;
			textPosition.Y -= (Game1.smallFont.MeasureString(
				Game1.parseText(text, Game1.smallFont, textWidth)).Y / 2 - 24) * yScale;
			textHeightCheck = Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).Y * yScale * titleScale;
			if (textHeightCheck * titleScale > 60)
				textPosition.Y += (textHeightCheck - 60) / 2;
			DrawText(b, text, 1.5f * titleScale, textPosition.X, textPosition.Y, textWidth, true);

			// Recipe description
			textPosition.X = 0;
			textPosition.Y = NavLeftButton.bounds.Y + NavLeftButton.bounds.Height //+ (Config.CookingTakesTime ? 20 : 25);
				+ 25;
			if (textHeightCheck > 60)
				textPosition.Y += textHeightCheck - 50 * xScale;
			textWidth = (int)(_textWidth * xScale);
			text = Game1.player.knowsRecipe(_filteredRecipeList[_currentRecipe].name)
				? _filteredRecipeList[_currentRecipe].description
				: i18n.Get("menu.cooking_recipe.title_unknown");
			DrawText(b, text, 1f, textPosition.X, textPosition.Y, textWidth, true);
			textPosition.Y += TextDividerGap * 2;

			// Recipe ingredients
			if (textHeightCheck > 60 && Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).Y < 80)
				textPosition.Y -= 6 * Scale;
			textHeightCheck = Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).Y * yScale;
			if (textHeightCheck > 120) 
				textPosition.Y += 6 * Scale;
			if (textHeightCheck > 100 && _filteredRecipeList[_currentRecipe].getNumberOfIngredients() < 6)
				textPosition.Y += 6 * Scale;
			textPosition.Y += TextDividerGap + Game1.smallFont.MeasureString(
				Game1.parseText(yScale < 1 ? "Hoplite!\nHoplite!" : "Hoplite!\nHoplite!\nHoplite!", Game1.smallFont, textWidth)).Y * yScale;
			DrawHorizontalDivider(b, 0, textPosition.Y, _lineWidth, true);
			textPosition.Y += TextDividerGap;
			text = i18n.Get("menu.cooking_recipe.ingredients_label");
			DrawText(b, text, 1f, textPosition.X, textPosition.Y, null, true, SubtextColour);
			textPosition.Y += Game1.smallFont.MeasureString(
				Game1.parseText(text, Game1.smallFont, textWidth)).Y * yScale;
			DrawHorizontalDivider(b, 0, textPosition.Y, _lineWidth, true);
			textPosition.Y += TextDividerGap - 64 / 2 + 4;

			if (Game1.player.knowsRecipe(_filteredRecipeList[_currentRecipe].name))
			{
				for (var i = 0; i < _filteredRecipeList[_currentRecipe].getNumberOfIngredients(); ++i)
				{
					textPosition.Y += 64 / 2 + (_filteredRecipeList[_currentRecipe].getNumberOfIngredients() < 5 ? 4 : 0);

					var id = _filteredRecipeList[_currentRecipe].recipeList.Keys.ElementAt(i);
					var requiredCount = _filteredRecipeList[_currentRecipe].recipeList.Values.ElementAt(i);
					var requiredItem = id;
					var bagCount = Game1.player.getItemCount(requiredItem, 8);
					var dropInCount = GetIngredientsCount(id, _cookingSlotsDropIn);
					var fridge = Game1.currentLocation is FarmHouse farmHouse
						&& ModEntry.Instance.GetFarmhouseKitchenLevel(farmHouse) > 0
							? farmHouse.fridge?.Value ?? null
							: null;
					var fridgeCount = fridge != null ? GetIngredientsCount(id, fridge.items) : 0;
					var miniFridgeCount = _minifridgeList.Count > 0 ? _minifridgeList.SelectMany(
							minifridge => minifridge?.Where(item => item != null
								&& (item.ParentSheetIndex == id || item.Category == id
								|| (CanBeCooked(item) && CraftingRecipe.isThereSpecialIngredientRule((StardewValley.Object)item, id)))))
						.Aggregate(0, (current, item) => current + item?.Stack ?? 0) : 0;
					requiredCount -= bagCount + dropInCount + fridgeCount + miniFridgeCount;
					var ingredientNameText = _filteredRecipeList[_currentRecipe].getNameFromIndex(id);
					
					// Show category-specific information for general category ingredient rules
					if (id < 0)
					{
						switch (id)
						{
							case -81:
								ingredientNameText = i18n.Get("item.forage.label");
								id = 22;
								break;
							case -80:
								ingredientNameText = i18n.Get("item.flower.label");
								id = 591;
								break;
							case -79:
								ingredientNameText = i18n.Get("item.fruit.label");
								id = 406;
								break;
							case -75:
								ingredientNameText = i18n.Get("item.vegetable.label");
								id = 278;
								break;
							case -14:
								ingredientNameText = i18n.Get("item.meat.label");
								id = 640;
								break;
						}
					}
					
					var drawColour = requiredCount <= 0 ? Game1.textColor : BlockedColour;

					// Ingredient icon
					b.Draw(
						Game1.objectSpriteSheet,
						new Vector2(_leftContent.X, textPosition.Y - 2f),
						Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet,
							_filteredRecipeList[_currentRecipe].getSpriteIndexFromRawIndex(id), 16, 16),
						Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.86f);
					// Ingredient quantity
					Utility.drawTinyDigits(
						_filteredRecipeList[_currentRecipe].recipeList.Values.ElementAt(i),
						b,
						new Vector2(
							_leftContent.X + 32 - Game1.tinyFont.MeasureString(
								string.Concat(_filteredRecipeList[_currentRecipe].recipeList.Values.ElementAt(i))).X,
							textPosition.Y + 21 - 2f),
						2f,
						0.87f,
						Color.AntiqueWhite);
					// Ingredient name
					DrawText(b, ingredientNameText, 1f, 48, textPosition.Y, null, true, drawColour);

					// Ingredient stock
					if (!Game1.options.showAdvancedCraftingInformation)
						continue;
					var position = new Point((int)(_lineWidth - 64 * xScale), (int)(textPosition.Y + 2));
					b.Draw(
						Game1.mouseCursors,
						new Rectangle(_leftContent.X + position.X, position.Y, 22, 26),
						new Rectangle(268, 1436, 11, 13),
						Color.White);
					DrawText(b, string.Concat(bagCount + dropInCount + fridgeCount + miniFridgeCount), 1f, position.X + 32, position.Y, 72, true, drawColour);
				}
			}
			else
			{
				textPosition.Y += 64 / 2 + 4;
				text = i18n.Get("menu.cooking_recipe.title_unknown");
				DrawText(b, text, 1f, 40, textPosition.Y, textWidth, true, SubtextColour);
			}

			//if (!Config.CookingTakesTime)
				return;

			// Recipe cooking duration and clock icon
			text = i18n.Get("menu.cooking_recipe.time_label");
			textPosition.Y = _cookbookLeftRect.Y + _cookbookLeftRect.Height - 56 - Game1.smallFont.MeasureString(
				Game1.parseText(text, Game1.smallFont, textWidth)).Y * yScale;
			DrawHorizontalDivider(b, 0, textPosition.Y, _lineWidth, true);
			textPosition.Y += TextDividerGap;
			DrawText(b, text, 1f, textPosition.X, textPosition.Y, null, true);
			text = _filteredRecipeList[_currentRecipe].timesCrafted > 0
				? i18n.Get("menu.cooking_recipe.time_value", new { duration = cookingTime })
				: i18n.Get("menu.cooking_recipe.title_unknown");
			textPosition.X = _lineWidth - 16 - Game1.smallFont.MeasureString(
				Game1.parseText(text, Game1.smallFont, textWidth)).X;
			Utility.drawWithShadow(b,
				Game1.mouseCursors,
				new Vector2(_leftContent.X + textPosition.X, textPosition.Y + 6),
				new Rectangle(434, 475, 9, 9),
				Color.White, 0f, Vector2.Zero, 2f, false, 1f,
				-2, 2);
			textPosition.X += 24;
			DrawText(b, text, 1f, textPosition.X, textPosition.Y, null, true);
		}

		private void DrawCraftingPage(SpriteBatch b)
		{
			var xScale = _locale == "ko" && _resizeKoreanFonts ? KoWidthScale : 1f;
			var yScale = _locale == "ko" && _resizeKoreanFonts ? KoHeightScale : 1f;

			// Cooking slots
			foreach (var clickable in CookingSlots)
				clickable.draw(b);

			for (var i = 0; i < _cookingSlotsDropIn.Count; ++i)
				_cookingSlotsDropIn[i]?.drawInMenu(b,
					new Vector2(
						CookingSlots[i].bounds.Location.X + CookingSlots[i].bounds.Width / 2 - 64 / 2,
						CookingSlots[i].bounds.Location.Y + CookingSlots[i].bounds.Height / 2 - 64 / 2),
					1f, 1f, 1f,
					StackDrawType.Draw, Color.White, true);

			var textPosition = Vector2.Zero;
			var textWidth = (int)(_textWidth * xScale);
			string text;

			// Recipe notes
			text = i18n.Get("menu.cooking_recipe.notes_label");
			textPosition.Y = _cookbookRightRect.Y + _cookbookRightRect.Height - 196 - Game1.smallFont.MeasureString(
				Game1.parseText(text, Game1.smallFont, textWidth)).Y * yScale;

			if (_showCookingConfirmPopup)
			{
				textPosition.Y += 16;
				textPosition.X += 64;

				// Contextual cooking popup
				Game1.DrawBox(CookIconBounds.X, CookIconBounds.Y, CookIconBounds.Width, CookIconBounds.Height);
				_filteredRecipeList[_currentRecipe].drawMenuView(b, CookIconBounds.X + 14, CookIconBounds.Y + 14);

				CookQuantityUpButton.draw(b);
				_quantityTextBox.Draw(b);
				CookQuantityDownButton.draw(b);

				CookConfirmButton.draw(b);
				CookCancelButton.draw(b);

				return;
			}

			if (_stack.Count < 1 || _stack.Peek() != State.Recipe)
			{
				return;
			}

			DrawHorizontalDivider(b, 0, textPosition.Y, _lineWidth, false);
			textPosition.Y += TextDividerGap;
			DrawText(b, text, 1f, textPosition.X, textPosition.Y, null, false, SubtextColour);
			textPosition.Y += Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).Y * yScale;
			DrawHorizontalDivider(b, 0, textPosition.Y, _lineWidth, false);
			textPosition.Y += TextDividerGap * 2;

			if (_recipeItem == null || _stack.Count < 1 || _stack.Peek() != State.Recipe)
				return;

			if (ShouldDrawCookButton())
			{
				textPosition.Y += 16;
				textPosition.X = _rightContent.X + _cookbookRightRect.Width / 2 - MarginRight;
				var frypanWidth = Config.AddCookingToolProgression ? 16 + 4 : 0;

				// Cook! button
				var extraHeight = new [] { "ko", "ja", "zh", "tr" }.Contains(_locale) ? 4 : 0;
				var source = CookButtonSource;
				source.X += _animFrame * CookButtonSource.Width;
				var dest = new Rectangle(
					(int)textPosition.X - frypanWidth / 2 * Scale,
					(int)textPosition.Y - extraHeight,
					source.Width * Scale,
					source.Height * Scale + extraHeight);
				dest.X -= (_cookTextSourceWidths[_locale] / 2 * Scale - CookTextSideWidth * Scale) + MarginLeft - frypanWidth / 2;
				var clickableArea = new Rectangle(
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
					Texture, dest, source,
					Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
				// middle and text and frypan
				source.X = _animFrame * CookButtonSource.Width + CookButtonSource.X + CookTextSideWidth;
				source.Width = 1;
				dest.Width = (_cookTextMiddleWidth + frypanWidth) * Scale;
				dest.X += CookTextSideWidth * Scale;
				b.Draw(
					Texture, dest, source,
					Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
				b.Draw(
					Texture,
					new Rectangle(
						dest.X,
						dest.Y + (dest.Height - CookTextSource[_locale].Height * Scale) / 2
									   + AnimTextOffsetPerFrame[_animFrame] * Scale,
						CookTextSource[_locale].Width * Scale,
						CookTextSource[_locale].Height * Scale + extraHeight),
					CookTextSource[_locale],
					Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
				dest.X += _cookTextMiddleWidth * Scale;
				dest.Width = 16 * Scale;
				if (Config.AddCookingToolProgression)
				{
					b.Draw(
						Texture,
						new Rectangle(dest.X + 4 * Scale, dest.Y + (1 + AnimTextOffsetPerFrame[_animFrame]) * Scale, 16 * Scale, 16 * Scale),
						new Rectangle(176 + ModEntry.Instance.CookingToolLevel * 16, 272, 16, 16),
						Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
				}

				// right
				source.X = _animFrame * CookButtonSource.Width + CookButtonSource.X + CookButtonSource.Width - CookTextSideWidth;
				source.Width = CookTextSideWidth;
				dest.Width = source.Width * Scale;
				dest.X += frypanWidth * Scale;
				b.Draw(
					Texture, dest, source,
					Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);

				// DANCING FORKS
				/*var flipped = _animFrame >= 4 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
				for (var i = 0; i < 2; ++i)
				{
					var xSourceOffset = i == 1 ? 32 : 0;
					var ySourceOffset = _animFrame % 2 == 0 ? 32 : 0;
					var xDestOffset = i == 1 ? FilterContainerSideWidth * 2 * Scale + FilterContainerMiddleWidth * Scale + 96 : 0;
					b.Draw(
						Texture,
						new Vector2(_rightContent.X + xDestOffset - 8, dest.Y - 32),
						new Rectangle(128 + xSourceOffset, 16 + ySourceOffset, 32, 32),
						Color.White, 0f, Vector2.Zero, Scale, flipped, 1f);
				}*/
			}
			else if (Config.HideFoodBuffsUntilEaten
				&& (!ModEntry.Instance.FoodsEaten.Contains(_recipeItem.Name)))
			{
				text = i18n.Get("menu.cooking_recipe.notes_unknown");
				DrawText(b, text, 1f, textPosition.X, textPosition.Y, textWidth, false, SubtextColour);
			}
			else
			{
				// Energy
				textPosition.X = _locale != "zh" ? -8f : 8f;
				text = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3116",
					_recipeItem.staminaRecoveredOnConsumption());
				Utility.drawWithShadow(b,
					Game1.mouseCursors,
					new Vector2(_rightContent.X + textPosition.X, textPosition.Y),
					new Rectangle(0, 428, 10, 10),
					Color.White, 0f, Vector2.Zero, 3f);
				textPosition.X += 34f;
				DrawText(b, text, 1f, textPosition.X, textPosition.Y, null, false, Game1.textColor);
				textPosition.Y += Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).Y * yScale;
				// Health
				text = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3118",
					_recipeItem.healthRecoveredOnConsumption());
				textPosition.X -= 34f;
				Utility.drawWithShadow(b,
					Game1.mouseCursors,
					new Vector2(_rightContent.X + textPosition.X, textPosition.Y),
					new Rectangle(0, 428 + 10, 10, 10),
					Color.White, 0f, Vector2.Zero, 3f);
				textPosition.X += 34f;
				DrawText(b, text, 1f, textPosition.X, textPosition.Y, null, false, Game1.textColor);

				// Buff duration
				text = $"+{(_recipeBuffDuration / 60)}:{(_recipeBuffDuration % 60):00}";
				//text = $"+{_recipeBuffDuration:00}";

				if (_recipeBuffDuration > 0)
				{
					textPosition.Y += Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).Y * 1.1f * yScale;
					textPosition.X -= 34f;
					Utility.drawWithShadow(b,
						Game1.mouseCursors,
						new Vector2(_rightContent.X + textPosition.X, textPosition.Y),
						new Rectangle(434, 475, 9, 9),
						Color.White, 0f, Vector2.Zero, 3f);
					textPosition.X += 34f;
					DrawText(b, text, 1f, textPosition.X, textPosition.Y, null, false, Game1.textColor);
					textPosition.Y -= Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).Y * 1.1f * yScale;
				}

				textPosition.Y -= Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).Y * yScale;
				textPosition.X += -34f + _lineWidth / 2f + 16f;

				// Buffs
				if (_recipeBuffs != null && _recipeBuffs.Count > 0)
				{
					var count = 0;
					for (var i = 0; i < _recipeBuffs.Count && count < 4; ++i)
					{
						if (_recipeBuffs[i] == 0)
							continue;

						++count;
						Utility.drawWithShadow(b,
							Game1.mouseCursors,
							new Vector2(_rightContent.X + textPosition.X, textPosition.Y),
							new Rectangle(10 + 10 * i, 428, 10, 10),
							Color.White, 0f, Vector2.Zero, 3f);
						textPosition.X += 34f;
						text = (_recipeBuffs[i] > 0 ? "+" : "")
							   + _recipeBuffs[i]
							   + " " + i18n.Get($"menu.cooking_recipe.buff.{i}");
						DrawText(b, text, 1f, textPosition.X, textPosition.Y, null, false, Game1.textColor);
						textPosition.Y += Game1.smallFont.MeasureString(Game1.parseText(text, Game1.smallFont, textWidth)).Y * yScale;
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
			var iconShakeTimer = _iconShakeTimerField.GetValue();
			for (var key = 0; key < inventory.inventory.Count; ++key)
				if (iconShakeTimer.ContainsKey(key)
					&& Game1.currentGameTime.TotalGameTime.TotalSeconds >= iconShakeTimer[key])
					iconShakeTimer.Remove(key);

			if (InventorySelectButtons.Count > 0)
			{
				// Inventory nav buttons
				Game1.DrawBox(InventoriesScrollableArea.X, InventoriesScrollableArea.Y,
					InventoriesScrollableArea.Width, InventoriesScrollableArea.Height);
				foreach (var clickable in InventorySelectButtons)
					clickable.draw(b);

				// Inventory nav selected icon
				var w = 9;
				var sourceRect = new Rectangle(232 + 9 * ((int)(w * ((float)_animFrame / AnimFrames * 6)) / 9), 346, w, w);
				var currentButton = InventorySelectButtons[_currentSelectedInventory + 2].bounds;
				b.Draw(
					Game1.mouseCursors,
					new Rectangle(
						currentButton.X + Scale * ((currentButton.Width / 2 - w * Scale / 2 - 1 * Scale) / Scale),
						currentButton.Y - w * Scale + 4 * Scale,
						w * Scale,
						w * Scale),
					sourceRect,
					Color.White);
			}

			// Inventory autofill button
			if (IsAutofillEnabled)
				AutofillButton.draw(b);
		}

		/// <summary>
		/// Mostly a copy of InventoryMenu.draw(SpriteBatch b, int red, int blue, int green),
		/// though items considered unable to be cooked will be greyed out.
		/// </summary>
		private void DrawActualInventory(SpriteBatch b)
		{
			var iconShakeTimer = _iconShakeTimerField.GetValue();
			for (var key = 0; key < inventory.inventory.Count; ++key)
			{
				if (iconShakeTimer.ContainsKey(key)
					&& Game1.currentGameTime.TotalGameTime.TotalSeconds >= iconShakeTimer[key])
					iconShakeTimer.Remove(key);
			}
			for (var i = 0; i < inventory.capacity; ++i)
			{
				var position = new Vector2(
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
					Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10),
					Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);

				if ((inventory.playerInventory || inventory.showGrayedOutSlots) && i >= Game1.player.maxItems.Value)
					b.Draw(
						Game1.menuTexture,
						position,
						Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 57),
						Color.White * 0.5f, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);

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
				var vector2 = Game1.tinyFont.MeasureString(text);
				b.DrawString(
					Game1.tinyFont,
					text,
					position + new Vector2((float)(32.0 - vector2.X / 2.0), -vector2.Y),
					i == Game1.player.CurrentToolIndex ? Color.Red : Color.DimGray);
			}
			for (var i = 0; i < inventory.capacity; ++i)
			{
				var location = new Vector2(
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

				var drawShadow = inventory.highlightMethod(inventory.actualInventory[i]);
				if (iconShakeTimer.ContainsKey(i))
					location += 1f * new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2));
				inventory.actualInventory[i].drawInMenu(
					b,
					location,
					inventory.inventory.Count > i ? inventory.inventory[i].scale : 1f,
					!inventory.highlightMethod(inventory.actualInventory[i]) ? 0.25f : 1f,
					0.865f,
					StackDrawType.Draw,
					(CanBeCooked(inventory.actualInventory[i]) ? Color.White : Color.Gray * 0.25f),
					drawShadow);
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
					drawToolTip(b, hoverText, "", null, true, -1, 0,
						-1, -1, null, hoverAmount);
				else
					drawHoverText(b, hoverText, Game1.smallFont);
			}

			// Hover elements
			if (hoveredItem != null)
				drawToolTip(b, hoveredItem.getDescription(), hoveredItem.DisplayName, hoveredItem, heldItem != null);
			else if (hoveredItem != null && ItemsToGrabMenu != null)
				drawToolTip(b, ItemsToGrabMenu.descriptionText, ItemsToGrabMenu.descriptionTitle, hoveredItem, heldItem != null);
			heldItem?.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);

			// Search button
			SearchTabButton.draw(b);

			// Ingredients button
			if (IsIngredientsPageEnabled)
				IngredientsTabButton.draw(b);

			// Cursor
			Game1.mouseCursorTransparency = 1f;
			drawMouse(b);
		}

		private void DrawText(SpriteBatch b, string text, float scale, float x, float y, float? w, bool isLeftSide, Color? colour = null)
		{
			var position = isLeftSide ? _leftContent : _rightContent;
			position.Y -= yPositionOnScreen;
			w ??= Game1.smallFont.MeasureString(text).X + (_locale == "ja" || _locale == "zh" ? 20 : 0);
			if (_locale == "ko" && _resizeKoreanFonts)
				scale *= KoHeightScale;
			Utility.drawTextWithShadow(b, Game1.parseText(text, Game1.smallFont, (int)w), Game1.smallFont,
				new Vector2(position.X + x, position.Y + y), colour ?? Game1.textColor, scale);
		}

		private void DrawHorizontalDivider(SpriteBatch b, float x, float y, int w, bool isLeftSide)
		{
			var position = isLeftSide ? _leftContent : _rightContent;
			position.Y -= yPositionOnScreen;
			Utility.drawLineWithScreenCoordinates(
				position.X + TextMuffinTopOverDivider, (int)(position.Y + y),
				position.X + w + TextMuffinTopOverDivider, (int)(position.Y + y),
				b, DividerColour);
		}
	}
}
