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
using System.Text.RegularExpressions;
using System.Linq;

using Leclair.Stardew.BetterCrafting.Models;
using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Crafting;
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
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;

using SObject = StardewValley.Object;

namespace Leclair.Stardew.BetterCrafting.Menus {
	public class BetterCraftingPage : MenuSubscriber<ModEntry> {

		public static readonly int MAX_TABS = 8;
		public static readonly int VISIBLE_TABS = 8;

		// TODO: Stop hard-coding seasoning.
		public static readonly IIngredient[] SEASONING_RECIPE = new IIngredient[] {
			new BaseIngredient(917, 1)
		};

		public static readonly Rectangle FAV_STAR = new(338, 400, 8, 8);

		// Session State
		public static string LastTab { get; private set; } = null;
		public static bool FavoritesOnly { get; private set; } = false;

		// Menu Mode
		public readonly bool cooking;
		public readonly bool Standalone;

		public bool DrawBG = true;

		// Workbench Tracking
		public readonly GameLocation Location;
		public readonly Vector2? Position;
		public readonly Rectangle? Area;
		public readonly SObject Object;
		public NetMutex Mutex;

		public IList<LocatedInventory> MaterialContainers;
		protected IList<LocatedInventory> CachedInventories;
		private IList<IInventory> UnsafeInventories;
		private readonly bool ChestsOnly;
		public bool DiscoverContainers { get; private set; }

		// Editing Mode
		public bool Editing { get; protected set; } = false;
		public ClickableComponent btnCategoryIcon;
		public TextBox txtCategoryName;
		public ClickableComponent btnCategoryName;

		// Menu Components
		[SkipForClickableAggregation]
		public readonly InventoryMenu inventory;

		public readonly ClickableTextureComponent trashCan;
		public float trashCanLidRotation;

		public ClickableTextureComponent btnSearch;
		public ClickableTextureComponent btnToggleEdit;
		public ClickableTextureComponent btnSettings;
		public ClickableTextureComponent btnToggleFavorites;
		public ClickableTextureComponent btnToggleSeasoning;
		public ClickableTextureComponent btnToggleQuality;
		public ClickableTextureComponent btnToggleUniform;

		public ClickableTextureComponent btnPageUp;
		public ClickableTextureComponent btnPageDown;

		// Recipe Tracking
		protected IList<string> ListedRecipes;

		protected List<IRecipe> Recipes = new();
		protected Dictionary<string, IRecipe> RecipesByName = new();
		protected List<IRecipe> Favorites = new();

		[SkipForClickableAggregation]
		protected Dictionary<IRecipe, ClickableTextureComponent> RecipeComponents = new();
		[SkipForClickableAggregation]
		protected Dictionary<ClickableTextureComponent, IRecipe> ComponentRecipes = new();

		// Tabs
		public ClickableTextureComponent btnTabsUp;
		public ClickableTextureComponent btnTabsDown;
		private int TabScroll = 0;

		private int tabIndex = 0;
		protected List<TabInfo> Tabs = new();
		protected TabInfo CurrentTab { get => (tabIndex >= 0 && Tabs.Count > tabIndex) ? Tabs[tabIndex] : Tabs[0]; }

		// Pagination
		private int pageIndex = 0;
		[SkipForClickableAggregation]
		protected List<List<ClickableTextureComponent>> Pages = new();
		protected List<ClickableTextureComponent> CurrentPage { get => pageIndex >= 0 && Pages.Count > pageIndex ? Pages[pageIndex] : Pages[0]; }

		// Search
		private string Filter = null;
		private bool FilterIngredients = false;
		private Regex FilterRegex = null;

		// Components for IClickableComponent
		public List<ClickableComponent> currentPageComponents = new();

		// Held Item
		protected Item HeldItem;

		// Tooltip Nonsense
		internal ISimpleNode hoverNode = null;
		internal string hoverTitle = "";
		internal string hoverText = "";
		internal Item hoverItem = null;
		internal int hoverAmount = -1;
		public Item HoveredItem = null;

		internal IRecipe hoverRecipe = null;
		internal Cache<Item, string> lastRecipeHover;

		// Better Tooltip
		internal int hoverMode = -1;

		// Sprite Sources
		public Rectangle SourceFilter { get => Filter == null ? Sprites.Buttons.SEARCH_OFF : Sprites.Buttons.SEARCH_ON; }
		public Rectangle SourceEdit { get => Sprites.Buttons.WRENCH; }
		public Rectangle SourceFavorites { get => FavoritesOnly ? Sprites.Buttons.FAVORITE_ON : Sprites.Buttons.FAVORITE_OFF; }
		public Rectangle SourceSeasoning {
			get {
				switch (Mod.Config.UseSeasoning) {
					case SeasoningMode.Enabled:
						return Sprites.Buttons.SEASONING_ON;
					case SeasoningMode.InventoryOnly:
						return Sprites.Buttons.SEASONING_LOCAL;
					case SeasoningMode.Disabled:
					default:
						return Sprites.Buttons.SEASONING_OFF;
				}
			}
		}
		public Rectangle SourceQuality {
			get {
				switch (Mod.Config.MaxQuality) {
					case MaxQuality.Iridium:
						return Sprites.Buttons.QUALITY_3;
					case MaxQuality.Gold:
						return Sprites.Buttons.QUALITY_2;
					case MaxQuality.Silver:
						return Sprites.Buttons.QUALITY_1;
					default:
						return Sprites.Buttons.QUALITY_0;
				};
			}
		}

		public int Quality {
			get {
				switch (Mod.Config.MaxQuality) {
					case MaxQuality.None:
						return 0;
					case MaxQuality.Silver:
						return 1;
					case MaxQuality.Gold:
						return 2;
					case MaxQuality.Iridium:
					case MaxQuality.Disabled:
					default:
						return int.MaxValue;
				}
			}
		}


		public Rectangle SourceUniform { get => Mod.Config.UseUniformGrid ? Sprites.Buttons.UNIFORM_ON : Sprites.Buttons.UNIFORM_OFF; }

		#region Lifecycle

		public static BetterCraftingPage Open(
			ModEntry mod,
			GameLocation location = null,
			Vector2? position = null,
			Rectangle? area = null,
			int width = -1,
			int height = -1,
			bool cooking = false,
			bool standalone_menu = false,
			IList<LocatedInventory> material_containers = null,
			bool discover_containers = true,
			int x = -1,
			int y = -1,
			bool silent_open = false,
			IList<string> listed_recipes = null
		) {
			if (width <= 0)
				width = 800 + borderWidth * 2;
			if (height <= 0)
				height = 600 + borderWidth * 2;

			int rows = mod.GetBackpackRows(Game1.player);
			if (rows > 3)
				height += Game1.tileSize * (rows - 3);

			Vector2 pos = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
			if (x == -1) x = (int) pos.X;
			if (y == -1) y = (int) pos.Y;

			return new BetterCraftingPage(
				mod,
				x, y,
				width, height,

				location,
				position,
				area,

				cooking,
				standalone_menu,
				silent_open,
				discover_containers,

				material_containers,
				listed_recipes
			);
		}

		public static BetterCraftingPage Open(
			ModEntry mod,
			GameLocation location = null,
			Vector2? position = null,
			Rectangle? area = null,
			int width = -1,
			int height = -1,
			bool cooking = false,
			bool standalone_menu = false,
			IList<object> material_containers = null,
			bool discover_containers = true,
			int x = -1,
			int y = -1,
			bool silent_open = false,
			IList<string> listed_recipes = null
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
				material_containers: located,
				listed_recipes: listed_recipes
			);
		}

		public BetterCraftingPage(
			ModEntry mod,
			int x, int y,
			int width, int height,

			GameLocation location,
			Vector2? position,
			Rectangle? area,

			bool cooking = false,
			bool standalone_menu = false,
			bool silent_open = false,
			bool discover_containers = true,

			IList<LocatedInventory> material_containers = null,
			IList<string> listed_recipes = null
		) : base(mod, x, y, width, height) {

			Location = location ?? Game1.player.currentLocation;
			Position = position;
			Area = area;
			this.cooking = cooking;
			Standalone = standalone_menu;
			DiscoverContainers = discover_containers;
			MaterialContainers = material_containers;
			ListedRecipes = listed_recipes;

			ChestsOnly = this.cooking && Mod.intCSkill.IsLoaded;

			lastRecipeHover = new(key => hoverRecipe?.CreateItem(), () => hoverRecipe?.Name);

			if (Location != null) {
				// TODO: 
				Object = null;
			} else
				Object = null;

			// InventoryMenu
			int rows = Mod.GetBackpackRows(Game1.player);

			inventory = new InventoryMenu(
				xPositionOnScreen + spaceToClearSideBorder + borderWidth,
				yPositionOnScreen + spaceToClearTopBorder + borderWidth + 320 - 16,
				false,
				capacity: rows * 12,
				rows: rows
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

			btnSearch = new ClickableTextureComponent(
				bounds: new Rectangle(btnX, btnY, 64, 64),
				texture: Sprites.Buttons.Texture,
				sourceRect: SourceFilter,
				scale: 4f
			) {
				myID = 100,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.ID_ignore
			};

			btnToggleEdit = Mod.Config.UseCategories ? new ClickableTextureComponent(
				bounds: new Rectangle(btnX, btnY, 64, 64),
				texture: Sprites.Buttons.Texture,
				sourceRect: SourceEdit,
				scale: 4f
			) {
				myID = 101,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.ID_ignore
			} : null;

			btnSettings = (Mod.Config.ShowSettingsButton && Mod.HasGMCM() && (!Context.IsOnHostComputer || Context.IsMainPlayer)) ? new ClickableTextureComponent(
				bounds: new Rectangle(btnX, btnY, 64, 64),
				texture: Sprites.Buttons.Texture,
				sourceRect: Sprites.Buttons.SETTINGS,
				scale: 4f
			) {
				myID = 107,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.ID_ignore
			} : null;

			btnToggleFavorites = Mod.Config.UseCategories ? null : new ClickableTextureComponent(
				bounds: new Rectangle(btnX, btnY, 64, 64),
				texture: Sprites.Buttons.Texture,
				sourceRect: SourceFavorites,
				scale: 4f
			) {
				myID = 102,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.ID_ignore
			};

			btnToggleSeasoning = this.cooking ? new ClickableTextureComponent(
				bounds: new Rectangle(btnX, btnY, 64, 64),
				texture: Sprites.Buttons.Texture,
				sourceRect: SourceSeasoning,
				scale: 4f
			) {
				myID = 103,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.ID_ignore
			} : null;

			btnToggleQuality = Mod.Config.MaxQuality == MaxQuality.Disabled ? null : new ClickableTextureComponent(
				bounds: new Rectangle(btnX, btnY, 64, 64),
				texture: Sprites.Buttons.Texture,
				sourceRect: SourceQuality,
				scale: 4f
			) {
				myID = 105,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.ID_ignore
			};

			btnToggleUniform = this.cooking ? null : new ClickableTextureComponent(
				bounds: new Rectangle(btnX, btnY, 64, 64),
				texture: Sprites.Buttons.Texture,
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
				texture: Game1.mouseCursors,
				sourceRect: new Rectangle(564 + Game1.player.trashCanLevel * 18, 102, 18, 26),
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

			// Initialize our state
			DiscoverInventories();
			DiscoverRecipes();
			UpdateTabs();
			LayoutRecipes();

			// Final UX
			if (Standalone && ! silent_open)
				Game1.playSound("bigSelect");

			if (Game1.options.SnappyMenus)
				snapToDefaultClickableComponent();
		}

		public override void Dispose() {
			base.Dispose();
		}

		public override void emergencyShutDown() {
			base.emergencyShutDown();

			if (HeldItem != null) {
				Utility.CollectOrDrop(HeldItem);
				HeldItem = null;
			}
		}

		protected override void cleanupBeforeExit() {
			base.cleanupBeforeExit();
			if (Editing)
				SaveCategories();
		}

		#endregion

		#region Editing

		public void ToggleEditMode() {
			if (Editing) {
				// Save any last state.
				UpdateCategoryName();
				SaveCategories();
			}

			Editing = !Editing;

			// Rebuild the state from the ground up.
			DiscoverRecipes();
			UpdateTabs();
			LayoutRecipes();

			// Visibility
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
				int x = CraftingPageX() + 72;
				int y = CraftingPageY() - 96;

				int txtWidth = width - 2 * (IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder) - 72;

				txtCategoryName = new TextBox(
					textBoxTexture: Game1.content.Load<Texture2D>("LooseSprites\\textBox"),
					 null,
					Game1.smallFont,
					Game1.textColor
				) {
					X = x,
					Y = y,
					Width = txtWidth,
					Text = CurrentTab.Category.Name
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
					rightNeighborID = ClickableComponent.SNAP_AUTOMATIC
				};

				btnCategoryIcon = new ClickableComponent(
					bounds: new Rectangle(
						CraftingPageX() + 12 - 16,
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

			} else {
				txtCategoryName = null;
				btnCategoryName = null;
				btnCategoryIcon = null;
			}
		}

		public void SaveCategories() {
			var categories = Tabs
				.Select(val => val.Category)
				.Where(val => val.Id == "miscellaneous" || (val?.Recipes?.Count ?? 0) > 0);

			Mod.Recipes.SetCategories(Game1.player, categories, cooking);
			Mod.Recipes.SaveCategories();
		}

		public SpriteInfo GetSpriteFromIcon(CategoryIcon icon, List<IRecipe> recipes = null) {
			if (icon == null)
				return null;

			switch (icon.Type) {
				case CategoryIcon.IconType.Item:
					string name = icon.RecipeName;
					if (!string.IsNullOrEmpty(name)) {
						RecipesByName.TryGetValue(name, out IRecipe recipe);

						if (recipe != null)
							return SpriteHelper.GetSprite(recipe.CreateItem());
					}

					if (recipes != null && recipes.Count > 0)
						return SpriteHelper.GetSprite(recipes[0].CreateItem());

					return new SpriteInfo(
						Game1.mouseCursors,
						new Rectangle(173, 423, 16, 16)
					);

				case CategoryIcon.IconType.Texture:
					Texture2D texture = icon.Source.HasValue ?
						SpriteHelper.GetTexture(icon.Source.Value)
						: null;

					if (!string.IsNullOrEmpty(icon.Path))
						try {
							texture = Mod.Helper.Content.Load<Texture2D>(icon.Path) ?? texture;
						} catch (Exception ex) {
							Log($"Unable to load texture \"{icon.Path}\" for category icon", LogLevel.Warn, ex);
						}

					if (texture != null) {
						Rectangle rect = icon.Rect ?? texture.Bounds;
						return new SpriteInfo(
							texture,
							rect,
							baseScale: icon.Scale
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
			if (name.Equals(CurrentTab.Category.Name))
				return;

			CurrentTab.Category.Name = name;
			CurrentTab.Category.I18nKey = "";
			CurrentTab.Component.label = name;
		}

		public void UpdateRecipeInCategory(IRecipe recipe, bool? present = null) {
			if (!Editing || CurrentTab == null || recipe == null || CurrentTab.Category.Recipes == null)
				return;

			string name = recipe.Name;
			bool wanted = present ?? !CurrentTab.Category.Recipes.Contains(name);

			if (wanted)
				CurrentTab.Category.Recipes.Add(name);
			else
				CurrentTab.Category.Recipes.Remove(name);

			// Update the sprite, maybe.
			if (CurrentTab.Category.Icon.Type == CategoryIcon.IconType.Item && string.IsNullOrEmpty(CurrentTab.Category.Icon.RecipeName)) {
				SpriteInfo sprite = null;

				if (CurrentTab.Category.Recipes.Count > 0) {
					if (RecipesByName.TryGetValue(CurrentTab.Category.Recipes.First(), out IRecipe val))
						sprite = SpriteHelper.GetSprite(val.CreateItem());
				}

				if (sprite == null)
					sprite = new SpriteInfo(
						SpriteHelper.GetTexture(Common.Enums.GameTexture.MouseCursors),
						new Rectangle(173, 423, 16, 16)
					);

				CurrentTab.Sprite = sprite;
			}
		}

		public bool IsRecipeInCategory(IRecipe recipe) {
			return CurrentTab?.Category.Recipes?.Contains(recipe.Name) ?? false;
		}

		#endregion

		#region Recipes and Inventory

		public virtual IList<string> GetListedRecipes() {
			return ListedRecipes;
		}

		public virtual void UpdateListedRecipes(IList<string> recipes) {
			ListedRecipes = recipes;

			// Rebuild the state from the ground up.
			DiscoverRecipes();
			UpdateTabs();
			LayoutRecipes();
		}

		protected virtual void DiscoverRecipes() {
			Recipes.Clear();
			RecipesByName.Clear();
			Favorites.Clear();

			var knownRecipes = cooking ? Game1.player.cookingRecipes : Game1.player.craftingRecipes;

			foreach (IRecipe recipe in Mod.Recipes.GetRecipes(cooking)) {
				if (!Editing && !knownRecipes.ContainsKey(recipe.Name) && (!cooking || Mod.Config.HideUnknown))
					continue;

				if (!Editing && ListedRecipes != null && !ListedRecipes.Contains(recipe.Name))
					continue;

				Recipes.Add(recipe);
				RecipesByName.Add(recipe.Name, recipe);

				if (Mod.Favorites.IsFavoriteRecipe(recipe.Name, cooking, Game1.player))
					Favorites.Add(recipe);
			}

			BuildRecipeComponents();
		}

		protected void UpdateTabs() {
			string oldTab = Tabs.Count > tabIndex ? Tabs[tabIndex].Category.Id : LastTab;

			Tabs.Clear();

			Dictionary<Category, List<IRecipe>> categories = new();
			List<IRecipe> unused = Recipes.ToList();
			Category misc = null;

			int count = 1;

			// Only check categories if categories are enabled.
			// Otherwise, everything should go into misc.
			if (Mod.Config.UseCategories) {
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
							Source = Common.Enums.GameTexture.MouseCursors,
							Rect = new Rectangle(338, 400, 8, 8)
						}
					}, Favorites);
				}

				foreach (Category cat in Mod.Recipes.GetCategories(Game1.player, cooking)) {
					if (misc == null && (cat.Id == "misc" || cat.Id == "miscellaneous"))
						misc = cat;

					// We continue rather than break in case there is a valid
					// misc tab to use.
					//if (count > (misc == cat ? 7 : 6))
					//	continue;

					List<IRecipe> recipes = new();

					if (cat.Recipes != null)
						foreach (string name in cat.Recipes) {
							if (!RecipesByName.TryGetValue(name, out IRecipe recipe))
								continue;

							recipes.Add(recipe);
							unused.Remove(recipe);
						}

					if (Editing || recipes.Count > 0) {
						count++;
						categories.Add(cat, recipes);
					}
				}
			}

			// If we're editing and don't have enough categories, add a bunch.
			if (Editing && Mod.Config.UseCategories) {
				//while (count < (misc != null ? 8 : 7)) {
					count++;

					categories.Add(new Category() {
						Id = Guid.NewGuid().ToString(),
						Name = "New Category",
						I18nKey = "",
						Icon = new CategoryIcon() {
							Type = CategoryIcon.IconType.Item
						},
						Recipes = new()
					}, new());
				//}
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
							Source = Common.Enums.GameTexture.MouseCursors,
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
				SpriteInfo sprite = GetSpriteFromIcon(cat.Icon, entry.Value);

				ClickableComponent tab = new(
					bounds: Rectangle.Empty, // new Rectangle(xPositionOnScreen - 48, yPositionOnScreen + offsetY + (64 * idx), 64, 64),
					name: entry.Key.Id,
					label: string.IsNullOrEmpty(entry.Key.I18nKey) ? entry.Key.Name : Mod.Helper.Translation.Get(entry.Key.I18nKey)
				) {
					myID = 1000 + idx,
					upNeighborID = (idx == 0) ? ClickableComponent.ID_ignore : 1000 + (idx - 1),
					downNeighborID = (idx >= categories.Count - 1) ? ClickableComponent.ID_ignore : 1000 + (idx + 1),
					rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
					leftNeighborID = ClickableComponent.ID_ignore
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
			string newTab = CurrentTab.Category.Id;
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

			// Add our buttons or not
			if (Tabs.Count > MAX_TABS) {
				btnTabsUp = new ClickableTextureComponent(
					bounds: Rectangle.Empty,
					texture: Game1.mouseCursors,
					sourceRect: Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 12),
					scale: 0.8f
				) {
					myID = 999,
					upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
					downNeighborID = 1000,
					rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
					leftNeighborID = ClickableComponent.SNAP_AUTOMATIC
				};

				btnTabsDown = new ClickableTextureComponent(
					bounds: Rectangle.Empty,
					texture: Game1.mouseCursors,
					sourceRect: Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 11),
					scale: 0.8f
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

			for(int i = 0; i < Tabs.Count; i++) {
				var entry = Tabs?[i];
				ClickableComponent cmp = entry?.Component;
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

			Func<object, IInventoryProvider> provider = ChestsOnly
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
			} catch(Exception ex) {
				Log("We received a bad material container list. Ignoring.", LogLevel.Warn, ex);
				count = 0;
			}

			// We want to locate all our inventories.
			if (count == 0) {
				if (DiscoverContainers && Location != null) {
					if (Area.HasValue) {
						if (Mod.Config.UseDiscovery)
							CachedInventories = InventoryHelper.DiscoverInventories(
								Area.Value,
								Location,
								Game1.player,
								provider,
								Mod.IsValidConnector,
								distanceLimit: Mod.Config.MaxDistance,
								scanLimit: Mod.Config.MaxCheckedTiles,
								targetLimit: Mod.Config.MaxInventories,
								includeSource: true,
								includeDiagonal: Mod.Config.UseDiagonalConnections
							);
						else
							CachedInventories = InventoryHelper.DiscoverInventories(
								Area.Value,
								Location,
								Game1.player,
								provider,
								null,
								distanceLimit: 1,
								scanLimit: 25,
								targetLimit: 9,
								includeSource: true,
								includeDiagonal: true
							);

					} else if (Position.HasValue) {
						var pos = new AbsolutePosition(Location, Position.Value);
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
								includeDiagonal: Mod.Config.UseDiagonalConnections
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
								includeDiagonal: true
							);
					}
				}
			} else if (DiscoverContainers && Mod.Config.UseDiscovery) {
				if (Location != null && Area.HasValue)
					CachedInventories = InventoryHelper.DiscoverInventories(
						Area.Value,
						Location,
						MaterialContainers,
						Game1.player,
						provider,
						Mod.IsValidConnector,
						distanceLimit: Mod.Config.MaxDistance,
						scanLimit: Mod.Config.MaxCheckedTiles,
						targetLimit: Mod.Config.MaxInventories,
						includeSource: true,
						includeDiagonal: Mod.Config.UseDiagonalConnections
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
						extra: (Position.HasValue && Location != null)
							? new AbsolutePosition[] { new(Location, Position.Value) } : null
					);
			} else
				CachedInventories = MaterialContainers;

			if (CachedInventories == null)
				CachedInventories = new List<LocatedInventory>();

			UnsafeInventories = InventoryHelper.GetUnsafeInventories(
				CachedInventories,
				provider,
				Game1.player,
				true
			);

#if DEBUG
			Log($"Chests: {count} -- Valid: {CachedInventories.Count}", StardewModdingAPI.LogLevel.Debug);
#endif
		}

		internal virtual IList<IInventory> GetUnsafeInventories() {
			return UnsafeInventories;
		}

		internal virtual IList<Item> GetEstimatedContainerContents() {
			if (CachedInventories == null || CachedInventories.Count == 0)
				return null;

			List<Item> items = new();
			foreach (LocatedInventory loc in CachedInventories) {
				if (ChestsOnly && loc.Source is not Chest)
					continue;

				var provider = Mod.GetInventoryProvider(loc.Source);
				if (provider == null || !provider.CanExtractItems(loc.Source, loc.Location, Game1.player))
					continue;

				var mutex = provider.GetMutex(loc.Source, loc.Location, Game1.player);
				if (mutex == null || (mutex.IsLocked() && !mutex.IsLockHeld()))
					continue;

				var oitems = provider.GetItems(loc.Source, loc.Location, Game1.player);
				if (oitems != null)
					items.AddRange(oitems);
			}

			return items;
		}

		protected virtual List<Item> GetActualContainerContents(IList<IInventory> locked) {
			List<Item> items = new();
			foreach (WorkingInventory inv in locked) {
				if (!inv.CanExtractItems())
					continue;

				IList<Item> oitems = inv.GetItems();
				if (oitems != null)
					items.AddRange(oitems);
			}

			return items;
		}

		public bool CanPerformCraft(IRecipe recipe) {
			if (recipe == null)
				return false;

			IList<Item> items = GetEstimatedContainerContents();

			if (!recipe.HasIngredients(Game1.player, items, UnsafeInventories, Quality))
				return false;

			if (HeldItem == null)
				return true;

			Item obj = recipe.CreateItem();

			return (HeldItem.Name.Equals(obj.Name)
				&& HeldItem.getOne().canStackWith(obj.getOne())
				&& HeldItem.Stack + recipe.QuantityPerCraft <= HeldItem.maximumStackSize()
			);
		}

		public void PerformCraft(IRecipe recipe, int times, Action<int> DoneAction = null, bool playSound = true, bool moveResultToInventory = false) {
			InventoryHelper.WithInventories(CachedInventories, Mod.GetInventoryProvider, Game1.player, locked => {

				int success = 0;
				bool used_additional = false;
				List<Item> items = GetActualContainerContents(locked);

				List<Chest> chests = ChestsOnly ? locked
					.Where(x => x.Object is Chest)
					.Select(x => x.Object as Chest)
					.ToList() : null;

				for (int i = 0; i < times; i++) {
					bool made = false;

					if (!recipe.HasIngredients(Game1.player, items, locked, Quality))
						break;

					Item obj = recipe.CreateItem();
					IIngredient[] ingredients = null;
					bool consume = true;
					bool seasoningInventories = Mod.Config.UseSeasoning != SeasoningMode.InventoryOnly;

					if (cooking) {
						if (Mod.intCSkill.IsLoaded) {
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

					if (HeldItem == null) {
						HeldItem = obj;
						made = true;

					} else if (HeldItem.Name.Equals(obj.Name)
							&& HeldItem.getOne().canStackWith(obj.getOne())
							&& HeldItem.Stack + recipe.QuantityPerCraft <= HeldItem.maximumStackSize()
					) {
						HeldItem.Stack += recipe.QuantityPerCraft;
						made = true;
					}

					if (!made)
						continue;

					success++;

					// Consume ingredients and rebuild our item list.
					if (consume) {
						// If we are cooking and using the Cooking Skill mod and
						// we have a recipe and we're only operating on chests,
						// then go ahead and use the vanilla CookingRecipe's
						// consumeIngredients method.
						if (cooking && Mod.intCSkill.IsLoaded && ChestsOnly && recipe.CraftingRecipe != null)
							recipe.CraftingRecipe.consumeIngredients(chests);
						else
							recipe.Consume(Game1.player, locked, Quality, Mod.Config.LowQualityFirst);
					}

					// Even if consume is false, Cooking Skill may have
					// modified items so refresh our list.
					items = GetActualContainerContents(locked);

					if (ingredients != null) {
						used_additional = true;

						// Consume ingredients and rebuild our item list.
						CraftingHelper.ConsumeIngredients(ingredients, Game1.player, seasoningInventories ? locked : null, Quality, Mod.Config.LowQualityFirst);
						items = GetActualContainerContents(locked);

						// Show a notice if the user used their last seasoning.
						if (!CraftingHelper.HasIngredients(ingredients, Game1.player, seasoningInventories ? items : null, seasoningInventories ? locked : null, Quality))
							Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Seasoning_UsedLast"));
					}

					Game1.player.checkForQuestComplete(null, -1, -1, obj, null, 2);

					if (!cooking && Game1.player.craftingRecipes.ContainsKey(recipe.Name))
						Game1.player.craftingRecipes[recipe.Name] += recipe.QuantityPerCraft;

					if (cooking) {
						Game1.player.cookedRecipe(HeldItem.ParentSheetIndex);

						if (obj is SObject sobj)
							Mod.intCSkill.AddCookingExperience(
								Game1.player,
								sobj.Edibility
							);
					}

					if (!cooking)
						Game1.stats.checkForCraftingAchievements();
					else
						Game1.stats.checkForCookingAchievements();
				}

				if (success > 0 && playSound)
					Game1.playSound("coin");
				if (used_additional && playSound)
					Game1.playSound("breathin");

				if (success > 0 && HeldItem != null) {
					bool move_item = (Game1.options.gamepadControls || (moveResultToInventory && HeldItem.maximumStackSize() == 1)) && Game1.player.couldInventoryAcceptThisItem(HeldItem);
					if (move_item && Game1.player.addItemToInventoryBool(HeldItem))
						HeldItem = null;
				}

				DoneAction?.Invoke(success);
			});
		}


		#endregion

		#region Layout and Components

		protected virtual int CraftingPageX() => xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth;
		protected virtual int CraftingPageY() => yPositionOnScreen
			+ (Editing ? 96 : 0)
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
					hoverText: cooking && !Game1.player.cookingRecipes.ContainsKey(recipe.Name) ? "ghosted" : "",
					texture: recipe.Texture, // recipe.BigCraftable ? Game1.bigCraftableSpriteSheet : Game1.objectSpriteSheet,
					sourceRect: recipe.SourceRectangle,
					//sourceRect: recipe.BigCraftable ?
					//	Game1.getArbitrarySourceRect(Game1.bigCraftableSpriteSheet, 16, 32, recipe.IndexOfMenuView) :
					//	Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, recipe.IndexOfMenuView, 16, 16),
					scale: 4f
				) {
					myID = 200 + idx,
					upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
					downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
					leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
					rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
					fullyImmutable = true,
					region = 8000
				};

				ComponentRecipes[cmp] = recipe;
			}

		}

		protected virtual List<ClickableTextureComponent> CreateNewPage() {
			List<ClickableTextureComponent> result = new();
			Pages.Add(result);
			return result;
		}

		protected virtual ClickableTextureComponent[,] CreateNewPageLayout() {
			return new ClickableTextureComponent[10, Editing ? 6 : 4];
		}

		protected virtual bool SpaceOccupied(ClickableTextureComponent[,] layout, int x, int y, int width, int height) {
			if (width == 1 && height == 1)
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

			IList<Item> items = GetEstimatedContainerContents();

			foreach (IRecipe recipe in sorted) {
				if (!Editing && favorites_only && !Favorites.Contains(recipe))
					continue;

				if (Editing && !DoesRecipeMatchFilter(recipe))
					continue;

				RecipeComponents.TryGetValue(recipe, out ClickableTextureComponent cmp);
				if (cmp == null)
					continue;

				// TODO: Optionally skip recipes we don't have the materials for.

				// Ensure that this will fit in the grid.
				int width = uniform ? 1 : Math.Max(1, recipe.GridWidth);
				int height = uniform ? 1 : Math.Max(1, recipe.GridHeight);

				float scale = Math.Min(xLimit / width, yLimit / height);
				if (scale < 1) {
					width = Math.Max(1, (int) (width * scale));
					height = Math.Max(1, (int) (height * scale));
				}

				while (SpaceOccupied(layout, x, y, width, height)) {
					x++;
					if (x >= xLimit) {
						x = 0;
						y++;
						if (y >= yLimit) {
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

				// TODO: Start using GridHeight and GridWidth.
				cmp.bounds = new Rectangle(
					offsetX + x * (64 + marginX),
					offsetY + y * 72,
					64 * width,
					64 * height
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

			if (Pages.Count > 1 && btnPageUp == null) {
				btnPageUp = new ClickableTextureComponent(
					new Rectangle(xPositionOnScreen + 768 + 16, offsetY, 64, 64),
					Game1.mouseCursors,
					Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 12),
					0.8f
				) {
					myID = 88,
					downNeighborID = 89,
					rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
					leftNeighborID = ClickableComponent.SNAP_AUTOMATIC
				};

				btnPageDown = new ClickableTextureComponent(
					new Rectangle(xPositionOnScreen + 768 + 16, offsetY + 192 + 32, 64, 64),
					Game1.mouseCursors,
					Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 11),
					0.8f
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

			if (pageIndex > Pages.Count)
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
		}


		public virtual bool ChangeTab(int change) {
			// If we're using filtering, try finding the next tab that has
			// contents. Otherwise, just go.
			if (Filter != null) {
				for(int i = tabIndex + change; i >= 0 && i < Tabs.Count; i += change) {
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
				txtCategoryName.Text = CurrentTab?.Category.Name ?? "";

			LayoutRecipes();
			return true;
		}

		#endregion

		#region Filtering

		private string HighlightSearchTerms(string text, bool is_ingredient = false) {
			if (FilterRegex == null || is_ingredient && !FilterIngredients)
				return text;

			string color = "#50FFD700";
			if (Mod.Theme?.SearchHighlightColor != null) {
				Color c = Mod.Theme.SearchHighlightColor.Value;
				color = $"#{c.A:X2}{c.R:X2}{c.G:X2}{c.B:X2}";
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

			if (FilterRegex.IsMatch(recipe.Description))
				return true;

			if (FilterIngredients) {
				foreach(var ing in recipe.Ingredients) {
					if (FilterRegex.IsMatch(ing.DisplayName))
						return true;
				}
			}

			return false;
		}

		public void ToggleSearch() {
			bool filtered = false;

			string old = Filter;
			if (old != null && FilterIngredients)
				old = $"{I18n.Search_IngredientPrefix()}{old}";

			var search = new SearchBox(
				Mod,
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

		public bool UpdateFilter(string filter) {
			if (!string.IsNullOrEmpty(filter)) {
				filter = filter.Trim();
				string prefix = I18n.Search_IngredientPrefix();
				FilterIngredients = filter.StartsWith(prefix);
				if (FilterIngredients)
					filter = filter[prefix.Length..].TrimStart();
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
			if (GetChildMenu() is IClickableMenu menu) {
				if (!Standalone)
					menu.receiveKeyPress(key);
				return;
			}

			if (txtCategoryName?.Selected ?? false)
				return;

			base.receiveKeyPress(key);

			if ( Mod.Config.SearchKey?.JustPressed() ?? false) {
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

			// TODO: Check for held item.
			if (!Editing && key.Equals(Keys.Delete) && (HeldItem?.canBeTrashed() ?? false)) {
				Utility.trashItem(HeldItem);
				HeldItem = null;
			}

			if (!Game1.isAnyGamePadButtonBeingHeld() || !Game1.options.doesInputListContain(Game1.options.menuButton, key))
				return;

			Game1.setMousePosition(trashCan.bounds.Center);
		}

		public override void receiveScrollWheelAction(int direction) {
			if (GetChildMenu() is IClickableMenu menu) {
				if (!Standalone)
					menu.receiveScrollWheelAction(direction);
				return;
			}

			base.receiveScrollWheelAction(direction);

			if (Game1.getOldMouseX() < (xPositionOnScreen + 16 + 8)) {
				if (ScrollTabs(direction > 0 ? -1 : 1))
					Game1.playSound("shwip");

				return;
			}

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

		public IRecipe GetRecipeUnderCursor(int x, int y) {
			if (Editing || CurrentPage == null)
				return null;

			foreach(var cmp in CurrentPage) {
				if (cmp.containsPoint(x, y) && ComponentRecipes.TryGetValue(cmp, out IRecipe recipe)) {
					if (!cmp.hoverText.Equals("ghosted"))
						return recipe;
					break;
				}
			}

			return null;
		}

		public override void releaseLeftClick(int x, int y) {
			if (GetChildMenu() is IClickableMenu menu) {
				if (!Standalone)
					menu.releaseLeftClick(x, y);
			}

			base.releaseLeftClick(x, y);
		}

		public override void leftClickHeld(int x, int y) {
			if (GetChildMenu() is IClickableMenu menu) {
				if (!Standalone)
					menu.leftClickHeld(x, y);
			}

			base.leftClickHeld(x, y);
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true) {
			if (GetChildMenu() is IClickableMenu menu) {
				if (!Standalone)
					menu.receiveLeftClick(x, y, playSound);
				return;
			}

			base.receiveLeftClick(x, y, playSound);

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
					if (cmp.containsPoint(x, y) && ComponentRecipes.TryGetValue(cmp, out IRecipe recipe)) {
						if (Editing) {
							UpdateRecipeInCategory(recipe);
							if (playSound)
								Game1.playSound("smallSelect");

						} else if (!cmp.hoverText.Equals("ghosted")) {
							PerformAction(recipe, Mod.Config.LeftClick, playSound);

						}

						cmp.scale = cmp.baseScale;
						return;
					}
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
				if (playSound)
					Game1.playSound("smallSelect");
				Mod.OpenGMCM();
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
				switch (Mod.Config.UseSeasoning) {
					case SeasoningMode.Disabled:
						Mod.Config.UseSeasoning = SeasoningMode.InventoryOnly;
						break;
					case SeasoningMode.InventoryOnly:
						Mod.Config.UseSeasoning = SeasoningMode.Enabled;
						break;
					case SeasoningMode.Enabled:
					default:
						Mod.Config.UseSeasoning = SeasoningMode.Disabled;
						break;
				}
				Mod.SaveConfig();
				btnToggleSeasoning.sourceRect = SourceSeasoning;
				btnToggleSeasoning.scale = btnToggleSeasoning.baseScale;
			}

			// Toggle Quality
			if (!Editing && btnToggleQuality != null && btnToggleQuality.containsPoint(x, y)) {
				if (playSound)
					Game1.playSound("smallSelect");
				switch (Mod.Config.MaxQuality) {
					case MaxQuality.None:
						Mod.Config.MaxQuality = MaxQuality.Silver;
						break;
					case MaxQuality.Silver:
						Mod.Config.MaxQuality = MaxQuality.Gold;
						break;
					case MaxQuality.Gold:
						Mod.Config.MaxQuality = MaxQuality.Iridium;
						break;
					case MaxQuality.Iridium:
					default:
						Mod.Config.MaxQuality = MaxQuality.None;
						break;
				}

				Mod.SaveConfig();
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

			// Trash
			if (!Editing && (trashCan?.containsPoint(x, y) ?? false) && (HeldItem?.canBeTrashed() ?? false)) {
				Utility.trashItem(HeldItem);
				HeldItem = null;
				return;
			}

			// Toss Item
			if (!Editing && !isWithinBounds(x, y) && (HeldItem?.canBeTrashed() ?? false)) {
				if (playSound)
					Game1.playSound("throwDownITem");
				Game1.createItemDebris(HeldItem, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
				HeldItem = null;
				return;
			}
		}

		public override void receiveRightClick(int x, int y, bool playSound = true) {
			if (GetChildMenu() is IClickableMenu menu) {
				if (!Standalone)
					menu.receiveRightClick(x, y, playSound);
				return;
			}

			if (!Editing)
				HeldItem = inventory.rightClick(x, y, HeldItem, playSound);

			// Toggle Seasoning
			if (!Editing && btnToggleSeasoning != null && btnToggleSeasoning.containsPoint(x, y)) {
				if (playSound)
					Game1.playSound("smallSelect");
				switch (Mod.Config.UseSeasoning) {
					case SeasoningMode.Disabled:
						Mod.Config.UseSeasoning = SeasoningMode.Enabled;
						break;
					case SeasoningMode.InventoryOnly:
						Mod.Config.UseSeasoning = SeasoningMode.Disabled;
						break;
					case SeasoningMode.Enabled:
					default:
						Mod.Config.UseSeasoning = SeasoningMode.InventoryOnly;
						break;
				}
				Mod.SaveConfig();
				btnToggleSeasoning.sourceRect = SourceSeasoning;
				btnToggleSeasoning.scale = btnToggleSeasoning.baseScale;
			}

			// Toggle Quality
			if (!Editing && btnToggleQuality != null && btnToggleQuality.containsPoint(x, y)) {
				if (playSound)
					Game1.playSound("smallSelect");
				switch (Mod.Config.MaxQuality) {
					case MaxQuality.None:
						Mod.Config.MaxQuality = MaxQuality.Iridium;
						break;
					case MaxQuality.Silver:
						Mod.Config.MaxQuality = MaxQuality.None;
						break;
					case MaxQuality.Gold:
						Mod.Config.MaxQuality = MaxQuality.Silver;
						break;
					case MaxQuality.Iridium:
					default:
						Mod.Config.MaxQuality = MaxQuality.Gold;
						break;
				}

				Mod.SaveConfig();
				btnToggleQuality.sourceRect = SourceQuality;
				btnToggleQuality.scale = btnToggleQuality.baseScale;
			}

			// Right click to favorite components
			if (CurrentPage != null)
				foreach (var cmp in CurrentPage) {
					if (cmp.containsPoint(x, y) && ComponentRecipes.TryGetValue(cmp, out IRecipe recipe)) {
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
				}
			};

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

						if (! Editing && Filter != null && tab.FilteredRecipes.Count == 0) {
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
			if (btnTabsDown != null)
				btnTabsDown.tryHover(x, (TabScroll + VISIBLE_TABS) >= Tabs.Count ? -1 : y);

			if (btnTabsUp != null)
				btnTabsUp.tryHover(x, TabScroll > 0 ? y : -1);

			if (btnPageUp != null)
				btnPageUp.tryHover(x, pageIndex > 0 ? y : -1);

			if (btnPageDown != null)
				btnPageDown.tryHover(x, pageIndex < Pages.Count - 1 ? y : -1);

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
				if (btnSettings.containsPoint(x, y))
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

			// If the mode changed, regenerate the fancy tool-tip.
			if (mode != hoverMode) {
				hoverMode = mode;
				hoverNode = null;
				if (mode == 8) {
					string filter = Filter;
					if (filter != null && FilterIngredients)
						filter = $"{I18n.Search_IngredientPrefix()}{filter}";

					hoverNode = SimpleHelper.Builder()
						.Text(I18n.Tooltip_Search())
						.Divider()
						.Text(filter, color: Game1.textColor * 0.6f)
						.GetLayout();
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

						switch (Mod.Config.UseSeasoning) {
							case SeasoningMode.Enabled:
								smode = I18n.Seasoning_Enabled();
								break;
							case SeasoningMode.InventoryOnly:
								smode = I18n.Seasoning_Inventory();
								break;
							case SeasoningMode.Disabled:
							default:
								smode = I18n.Seasoning_Disabled();
								break;
						}

						hoverText = $"{I18n.Tooltip_Seasoning()}\n{smode}";
						break;
					case 3:
						// Toggle Quality
						switch (Mod.Config.MaxQuality) {
							case MaxQuality.Silver:
								hoverText = I18n.Tooltip_Quality_Silver();
								break;
							case MaxQuality.Gold:
								hoverText = I18n.Tooltip_Quality_Gold();
								break;
							case MaxQuality.Iridium:
								hoverText= I18n.Tooltip_Quality_Iridium();
								break;
							case MaxQuality.None:
							default:
								hoverText = I18n.Tooltip_Quality_None();
								break;
						}

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
						string ing = FilterIngredients ? I18n.Search_IngredientPrefix() : "";

						hoverText = Filter == null
							? I18n.Tooltip_Search()
							: $"{I18n.Tooltip_Search()}\n{ing}{Filter}";
						break;
					default:
						break;
				}
			}

			// Trash Can
			if (!Editing && trashCan != null) {
				if (trashCan.containsPoint(x, y)) {
					if (trashCanLidRotation <= 0)
						Game1.playSound("trashcanlid");

					trashCanLidRotation = Math.Min(trashCanLidRotation + (float) Math.PI / 48f, 1.570796f);

					if (HeldItem != null) {
						hoverAmount = Utility.getTrashReclamationPrice(HeldItem, Game1.player);
						if (hoverAmount > 0)
							hoverText = Game1.content.LoadString("Strings\\UI:TrashCanSale");
					}

				} else
					trashCanLidRotation = Math.Max(trashCanLidRotation - (float) Math.PI / 48f, 0f);
			}
		}

		public override bool readyToClose() {
			if (!Standalone && GetChildMenu() != null)
				return false;

			return HeldItem == null;
		}

		#endregion

		#region Drawing

		public override void draw(SpriteBatch b) {

			// Background
			if (Standalone && DrawBG)
				b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);

			if (Standalone)
				Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);

			if (!Editing)
				drawHorizontalPartition(b, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 248);

			// Inventory
			if (!Editing)
				inventory.draw(b);

			// Editing
			if (Editing) {
				drawHorizontalPartition(b, yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 64);
				CurrentTab.Sprite?.Draw(b, new Vector2(
					CraftingPageX() + 12 - 16,
					yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 12
				), 4f);
				txtCategoryName?.Draw(b);
			}

			// Buttons
			btnSearch?.draw(b);
			btnToggleEdit?.draw(b);
			btnSettings?.draw(b);
			btnToggleUniform?.draw(b);

			if (!Editing) {
				btnToggleFavorites?.draw(b);
				btnToggleSeasoning?.draw(b);
				btnToggleQuality?.draw(b);
			}

			// Trash
			if (!Editing && trashCan != null) {
				trashCan.draw(b);
				b.Draw(
					Game1.mouseCursors,
					new Vector2(trashCan.bounds.X + 60, trashCan.bounds.Y + 40),
					new Rectangle(564 + Game1.player.trashCanLevel * 18, 129, 18, 10),
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

					// Rotate the background from the main sprite sheet.
					// We reuse the main sprite sheet to best support UI
					// reskins from other people.
					b.Draw(
						Game1.mouseCursors,
						new Vector2(x, tab.Component.bounds.Y),
						SpriteHelper.Tabs.BACKGROUND,
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
			IList<Item> items = null;
			if (recipes != null)
				items = GetEstimatedContainerContents();

			bool shifting = Game1.oldKBState.IsKeyDown(Keys.LeftShift) && GetChildMenu() == null;
			bool ctrling = Game1.oldKBState.IsKeyDown(Keys.LeftControl);

			bool drawn = false;

			if (recipes != null)
				foreach (var cmp in recipes) {
					if (!ComponentRecipes.TryGetValue(cmp, out IRecipe recipe))
						continue;

					bool in_category = Editing && IsRecipeInCategory(recipe);
					bool shifted = !Editing && shifting && recipe.Stackable;

					if (!Editing && cmp.hoverText.Equals("ghosted")) {
						// Unlearned Recipe
						drawn = true;
						cmp.DrawBounded(b, Color.Black * 0.35f, 0.89f);

					} else if (Editing ? !in_category : !recipe.HasIngredients(Game1.player, items, UnsafeInventories, Quality)) {
						// Recipe without Ingredients
						drawn = true;
						cmp.DrawBounded(b, Color.DimGray * 0.4f, 0.89f);
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
						cmp.DrawBounded(b);
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

					// TODO: Constant for the star sprite location.
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
					Game1.textColor
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

			// Hover Item
			int offset = HeldItem != null ? 48 : 0;

			if (hoverItem != null)
				IClickableMenu.drawToolTip(b, hoverText, hoverTitle, hoverItem, HeldItem != null);
			else if (hoverNode != null)
				hoverNode.DrawHover(
					b,
					Game1.smallFont,
					offsetX: offset,
					offsetY: offset
				);
			else if (!string.IsNullOrEmpty(hoverText)) {
				if (hoverAmount > 0)
					IClickableMenu.drawToolTip(b, hoverText, hoverTitle, null, true, moneyAmountToShowAtBottom: hoverAmount);
				else
					IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont, HeldItem != null ? 64 : 0, HeldItem != null ? 64 : 0);
			}

			// Held Item
			if (!Editing)
				HeldItem?.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 16, Game1.getOldMouseY() + 16), 1f);

			if (Standalone) {
				Game1.mouseCursorTransparency = 1f;
				drawMouse(b);
			}

			// TODO: Better tooltip
			// TODO: NO SERIOUSLY: BETTER TOOLTIP
			// TODO: CACHE THIS IT IS A MESS

			ISimpleNode recipeTip = GetRecipeTooltip(items);
			if (recipeTip == null)
				return;

			recipeTip.DrawHover(
				b,
				Game1.smallFont,
				offsetX: offset,
				offsetY: offset
			);
		}

		public ISimpleNode GetRecipeTooltip(IList<Item> items) {
			if (hoverRecipe == null)
				return null;

			TTWhen when = Mod.Config.ShowKeybindTooltip;
			bool shifting = Game1.oldKBState.IsKeyDown(Keys.LeftShift);
			bool ctrling = Game1.oldKBState.IsKeyDown(Keys.LeftControl);

			string[] buffIconsToDisplay = null;
			Item recipeItem = lastRecipeHover.Value;
			if (cooking && recipeItem != null) {
				string[] temp = Game1.objectInformation[recipeItem.ParentSheetIndex].Split('/');
				if (temp.Length > 7)
					buffIconsToDisplay = temp[7].Split(' ');
			}

			if (Editing && ! shifting) {
				var ebuild = SimpleHelper.Builder();
				if (Filter != null)
					ebuild.FormatText(HighlightSearchTerms(hoverRecipe.DisplayName));
				else
					ebuild.Text(hoverRecipe.DisplayName);

				AddBuffsToTooltip(ebuild, recipeItem, buffIconsToDisplay, true);

				if (when == TTWhen.Always || (when == TTWhen.ForController && Game1.options.gamepadControls))
					ebuild
						.Divider()
						.Group(8)
							.Add(GetLeftClickNode())
							.Text(I18n.Tooltip_ToggleRecipe())
						.EndGroup()
						.Group(8)
							.Add(GetRightClickNode())
							.Text(I18n.Tooltip_UseAsIcon())
						.EndGroup();

				return ebuild.GetLayout();
			}

			bool shifted = ! Editing && shifting && hoverRecipe.Stackable;
			int quantity = hoverRecipe.QuantityPerCraft * (shifted ? (ctrling ? 25 : 5) : 1);
			bool supports_quality = true;
			int craftable = int.MaxValue;

			List<ISimpleNode> ingredients = new();

			foreach (var entry in hoverRecipe.Ingredients) {
				int amount = entry.GetAvailableQuantity(Game1.player, items, UnsafeInventories, Quality);
				int quant = entry.Quantity * (shifted ? (ctrling ? 25 : 5) : 1);
				craftable = Math.Min(craftable, amount / entry.Quantity);

				if (!entry.SupportsQuality)
					supports_quality = false;

				Color color = amount < entry.Quantity ?
					(Mod.Theme?.QuantityCriticalTextColor ?? Color.Red) :
					amount < quant ?
						(Mod.Theme?.QuantityWarningTextColor ?? Color.OrangeRed) :
							Game1.textColor;

				Color? shadow = amount < entry.Quantity ?
					Mod.Theme?.QuantityCriticalShadowColor :
					amount < quant ?
						Mod.Theme?.QuantityWarningShadowColor :
							null;

				var ebuilder = SimpleHelper
					.Builder(LayoutDirection.Horizontal, margin: 8)
					.Sprite(
						new SpriteInfo(entry.Texture, entry.SourceRectangle),
						scale: 2,
						quantity: quant,
						align: Alignment.Middle
					);

				if (FilterIngredients)
					ebuilder
						.FormatText(
							HighlightSearchTerms(entry.DisplayName, true),
							color: color,
							shadowColor: shadow,
							align: Alignment.Middle
						);
				else
					ebuilder
						.Text(
							entry.DisplayName,
							color: color,
							shadowColor: shadow,
							align: Alignment.Middle
						);

				if (Game1.options.showAdvancedCraftingInformation)
					ebuilder
						.Space()
						.Text($"{amount}", align: Alignment.Middle)
						.Texture(
							Game1.mouseCursors,
							SpriteHelper.MouseIcons.BACKPACK,
							2,
							align: Alignment.Middle
						);

				ingredients.Add(ebuilder.GetLayout());
			}

			var builder = SimpleHelper.Builder(minSize: new Vector2(4 * 80, 0));
			string quantText = quantity > 1 ? $" x{quantity}" : "";

			if (Game1.options.showAdvancedCraftingInformation) {
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
					.Text($"({craftable})", align: Alignment.Middle)
					.EndGroup();

			} else if (Filter != null)
				builder.FormatText(HighlightSearchTerms(hoverRecipe.DisplayName), font: Game1.dialogueFont, shadow: true);
			else
				builder.Text(hoverRecipe.DisplayName, font: Game1.dialogueFont, shadow: true);

			if (recipeItem != null)
				builder.Text(recipeItem.getCategoryName(), color: recipeItem.getCategoryColor());

			builder
				.Divider()
				.Text(
					Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.567"),
					color: Game1.textColor * 0.75f
				)
				.AddSpacedRange(4, ingredients);

			if (!supports_quality && (Mod.Config.LowQualityFirst || Mod.Config.MaxQuality != MaxQuality.Disabled))
				builder.Flow(
					FlowHelper.Builder()
						.Text(
							I18n.Tooltip_QualityUnsupported(),
							color: Game1.textColor * 0.75f
						).Build());

			builder.Divider();

			if (Filter == null)
				builder.Flow(FlowHelper.Builder()
					.Text(hoverRecipe.Description)
					.Build(),
					wrapText: true
				);
			else
				builder.FormatText(
					HighlightSearchTerms(hoverRecipe.Description), wrapText: true);

			AddBuffsToTooltip(builder, recipeItem, buffIconsToDisplay, false);

			if (Game1.options.showAdvancedCraftingInformation && hoverRecipe != null) {
				int count = hoverRecipe.GetTimesCrafted(Game1.player);
				if (count > 0)
					builder
						.Text(
							I18n.Tooltip_Crafted(count),
							color: Game1.textColor * 0.5f
						);
			}

			// Keybindings
			// TODO: Refactor keybinding tips somehow?
			if (when == TTWhen.Always || (when == TTWhen.ForController && Game1.options.gamepadControls)) {
				List<ISimpleNode> bindings = new();

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
							.Text(I18n.Setting_Action_BulkCraft())
						.GetLayout()
					);
				}

				if (bindings.Count > 0)
					builder
						.Divider()
						.AddRange(bindings);
			}

			return builder.GetLayout();
		}

		private static SimpleBuilder AddBuffsToTooltip(SimpleBuilder builder, Item item, string[] buffs, bool icons_only = false) {
			if (icons_only)
				builder = builder.Group(margin: 8);

			if (item is SObject sobj && sobj.Edibility != -300) {
				int health = item.healthRecoveredOnConsumption();
				int stamina = item.staminaRecoveredOnConsumption();

				if (health != 0) {
					Rectangle source = new(health < 0 ? 140 : 0, health < 0 ? 428 : 438, 10, 10);

					if (icons_only)
						builder.Texture(
							Game1.mouseCursors,
							source,
							scale: 2,
							align: Alignment.Middle
						);
					else {
						string label = (Convert.ToInt32(health) > 0 ? "+" : "") + health;

						builder.Group()
							.Texture(
								Game1.mouseCursors,
								source,
								scale: 3,
								align: Alignment.Middle
							)
							.Space(false, 4)
							.Text(
								Game1.content.LoadString("Strings\\UI:ItemHover_Health", label),
								align: Alignment.Middle
							)
						.EndGroup();
					}
				}

				if (stamina != 0) {
					Rectangle source = new(stamina < 0 ? 140 : 0, 428, 10, 10);

					if (icons_only)
						builder.Texture(
							Game1.mouseCursors,
							source,
							scale: 2,
							align: Alignment.Middle
						);
					else {
						string label = (Convert.ToInt32(stamina) > 0 ? "+" : "") + stamina;

						builder.Group()
							.Texture(
								Game1.mouseCursors,
								source,
								scale: 3,
								align: Alignment.Middle
							)
							.Space(false, 4)
							.Text(
								Game1.content.LoadString("Strings\\UI:ItemHover_Energy", label),
								align: Alignment.Middle
							)
						.EndGroup();
					}
				}
			}

			if (buffs != null) {
				for (int idx = 0; idx < buffs.Length; idx++) {
					string buff = buffs[idx];
					if (buff.Equals("0"))
						continue;

					Rectangle source = new(10 + idx * 10, 428, 10, 10);

					if (icons_only) {
						builder.Texture(
							Game1.mouseCursors,
							source,
							scale: 2,
							align: Alignment.Middle
						);

						continue;
					}

					string label = (Convert.ToInt32(buff) > 0 ? "+" : "") + buff;
					if (idx <= 11)
						label = Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + idx, label);

					builder.Group()
						.Texture(
							Game1.mouseCursors,
							source,
							scale: 3,
							align: Alignment.Middle
						)
						.Space(false, 4)
						.Text(label, align: Alignment.Middle)
					.EndGroup();
				}
			}

			if (icons_only)
				builder = builder.EndGroup();

			return builder;
		}

		public static string GetActionTip(ButtonAction action) {
			switch (action) {
				case ButtonAction.Craft:
					return I18n.Setting_Action_Craft();
				case ButtonAction.Favorite:
					return I18n.Setting_Action_Favorite();
				case ButtonAction.BulkCraft:
					return I18n.Setting_Action_BulkCraft();
			}

			return null;
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

		public static ISimpleNode GetNode(KeybindList keybind) {
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
			if ( sprite != null )
				return new SpriteNode(sprite, scale: 2, alignment: Alignment.Middle);

			return new TextNode($"{button}:");
		}
		public static ISimpleNode GetNode(InputButton[] buttons) {
			SpriteInfo sprite = null;
			foreach(InputButton btn in buttons) {
				if (btn.mouseLeft) {
					sprite = SpriteHelper.GetSprite(SButton.MouseLeft);
					break;
				} else if (btn.mouseRight) {
					sprite = SpriteHelper.GetSprite(SButton.MouseRight);
					break;
				}
			}

			if (sprite != null)
				return new SpriteNode(sprite, scale: 2, alignment: Alignment.Middle);

			return new TextNode($"{ModEntry.GetInputLabel(buttons)}:");
		}

		#endregion

		#region CustomCraftingStation Compatibility Hax

#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable 0169    // ... more unused

		// This only exists to stop CCS from exception-ing.
		private List<Dictionary<ClickableTextureComponent, CraftingRecipe>> pagesOfCraftingRecipes;

		// We might want to support this for filtering out recipes
		// that we don't want to show. But we might not?
		public void layoutRecipes(List<string> recipes) {
			UpdateListedRecipes(recipes);
		}

#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore 0169    // ... more unused
#pragma warning restore IDE0044 // Add readonly modifier

		#endregion

	}
}
