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

using StardewValley;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;

using SObject = StardewValley.Object;

namespace Leclair.Stardew.BetterCrafting.Menus {
	public class BetterCraftingPage : MenuSubscriber<ModEntry> {

		// TODO: Stop hard-coding seasoning.
		public static IIngredient[] SEASONING_RECIPE = new IIngredient[] {
			new BaseIngredient(917, 1)
		};

		// Session State
		public static string LastTab = null;
		public static bool FavoritesOnly = false;

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
		private bool ChestsOnly;
		public bool DiscoverContainers { get; private set; }

		// Editing Mode
		public bool Editing { get; protected set; } = false;
		public TextBox txtCategoryName;
		public ClickableComponent btnCategoryName;

		// Menu Components
		[SkipForClickableAggregation]
		public readonly InventoryMenu inventory;

		public readonly ClickableTextureComponent trashCan;
		public float trashCanLidRotation;

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
		private int tabIndex = 0;
		protected List<TabInfo> Tabs = new();
		protected TabInfo CurrentTab { get => tabIndex >= 0 && Tabs.Count > tabIndex ? Tabs[tabIndex] : Tabs[0]; }

		// Pagination
		private int pageIndex = 0;
		[SkipForClickableAggregation]
		protected List<List<ClickableTextureComponent>> Pages = new();
		protected List<ClickableTextureComponent> CurrentPage { get => pageIndex >= 0 && Pages.Count > pageIndex ? Pages[pageIndex] : Pages[0]; }

		// Components for IClickableComponent
		public List<ClickableComponent> currentPageComponents = new();

		// Held Item
		protected Item HeldItem;

		// Tooltip Nonsense
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

		private int Quality {
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

			btnSettings = (Mod.Config.ShowSettingsButton && Mod.HasGMCM()) ? new ClickableTextureComponent(
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
					leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
					rightNeighborID = ClickableComponent.SNAP_AUTOMATIC
				};

			} else {
				txtCategoryName = null;
				btnCategoryName = null;
			}
		}

		public void SaveCategories() {
			Mod.Recipes.SetCategories(Game1.player, Tabs.Select(val => val.Category), cooking);
			Mod.Recipes.SaveCategories();
		}

		public void UpdateCategorySprite(IRecipe recipe) {
			if (!Editing || CurrentTab == null || recipe == null)
				return;

			CurrentTab.Category.Icon = new CategoryIcon() {
				Type = CategoryIcon.IconType.Item,
				RecipeName = recipe.Name
			};

			CurrentTab.Sprite = SpriteHelper.GetSprite(recipe.CreateItem(), Mod.Helper);
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
						sprite = SpriteHelper.GetSprite(val.CreateItem(), Mod.Helper);
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
					if (count > (misc == cat ? 7 : 6))
						continue;

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
				while (count < (misc != null ? 8 : 7)) {
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
				}
			}

			// Add any remaining, uncategorized items to a Misc. category.
			if (Editing || unused.Count > 0) {
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
			int offsetY = 120;

			foreach (KeyValuePair<Category, List<IRecipe>> entry in categories) {
				Category cat = entry.Key;
				SpriteInfo sprite = null;

				// TODO: Refactor this mess.
				if (cat.Icon != null)
					switch (cat.Icon.Type) {
						case CategoryIcon.IconType.Item:
							string name = cat.Icon.RecipeName;
							if (!string.IsNullOrEmpty(name)) {
								RecipesByName.TryGetValue(name, out IRecipe recipe);

								if (recipe != null)
									sprite = SpriteHelper.GetSprite(recipe.CreateItem(), Mod.Helper);
							}

							if (sprite == null && entry.Value.Count > 0)
								sprite = SpriteHelper.GetSprite(entry.Value[0].CreateItem(), Mod.Helper);

							if (sprite == null)
								sprite = new SpriteInfo(
									SpriteHelper.GetTexture(Common.Enums.GameTexture.MouseCursors),
									new Rectangle(173, 423, 16, 16)
								);

							break;

						case CategoryIcon.IconType.Texture:
							Texture2D texture = cat.Icon.Source.HasValue ? SpriteHelper.GetTexture(cat.Icon.Source.Value) : null;
							if (!string.IsNullOrEmpty(cat.Icon.Path))
								try {
									texture = Mod.Helper.Content.Load<Texture2D>(cat.Icon.Path) ?? texture;
								} catch (Exception ex) {
									Log($"Unable to load texture \"{cat.Icon.Path}\" for category {cat.Name}", StardewModdingAPI.LogLevel.Warn, ex);
								}

							if (texture != null) {
								Rectangle rect = cat.Icon.Rect ?? texture.Bounds;
								sprite = new SpriteInfo(texture, rect, baseScale: cat.Icon.Scale);
							}

							break;
					}

				ClickableComponent tab = new ClickableComponent(
					bounds: new Rectangle(xPositionOnScreen - 48, yPositionOnScreen + offsetY + (64 * idx), 64, 64),
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
					Recipes = entry.Value
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

			// We want to locate all our inventories.
			if (MaterialContainers == null || MaterialContainers.Count == 0) {
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
			Log($"Chests: {MaterialContainers?.Count ?? 0} -- Valid: {CachedInventories.Count}", StardewModdingAPI.LogLevel.Debug);
#endif
		}

		protected virtual IList<Item> GetEstimatedContainerContents() {
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
				sorted = CurrentTab.Recipes.ToList();

			sorted.Sort((a, b) => {
				int result = 0;

				// TODO: Big Craftables Last
				if (Mod.Config.SortBigLast) {
					bool bigA = a.GridHeight > 1 || a.GridWidth > 1;
					bool bigB = b.GridHeight > 1 || b.GridWidth > 1;

					result = bigA.CompareTo(bigB);
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


		public virtual void ChangeTab(int change) {
			SetTab(tabIndex + change);
		}

		public virtual void SetTab(int idx) {
			if (idx < 0)
				idx = 0;
			else if (idx >= Tabs.Count)
				idx = Tabs.Count - 1;

			if (tabIndex == idx)
				return;

			UpdateCategoryName();

			tabIndex = idx;
			LastTab = CurrentTab?.Category?.Id ?? LastTab;
			pageIndex = 0;

			if (Editing && txtCategoryName != null)
				txtCategoryName.Text = CurrentTab?.Category.Name ?? "";

			LayoutRecipes();
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

			if (b.Equals(Buttons.LeftShoulder))
				ChangeTab(-1);
			else if (b.Equals(Buttons.RightShoulder))
				ChangeTab(1);
		}

		public override void receiveKeyPress(Keys key) {
			if (txtCategoryName?.Selected ?? false)
				return;

			base.receiveKeyPress(key);
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
			base.receiveScrollWheelAction(direction);
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

		public override void receiveLeftClick(int x, int y, bool playSound = true) {
			base.receiveLeftClick(x, y, playSound);

			// Inventory
			if (!Editing)
				HeldItem = inventory.leftClick(x, y, HeldItem, playSound);

			// Editing
			txtCategoryName?.Update();

			// Pagination
			if (btnPageUp != null && btnPageUp.containsPoint(x, y) && pageIndex > 0) {
				ChangePage(-1);
				Game1.playSound("smallSelect");
				btnPageUp.scale = btnPageUp.baseScale;
				return;
			}

			if (btnPageDown != null && btnPageDown.containsPoint(x, y) && pageIndex < Pages.Count - 1) {
				ChangePage(+1);
				Game1.playSound("smallSelect");
				btnPageDown.scale = btnPageDown.baseScale;
				return;
			}

			// Tabs
			if (Tabs.Count > 1)
				for (int i = 0; i < Tabs.Count; i++) {
					ClickableComponent cmp = Tabs[i].Component;
					if (cmp.containsPoint(x, y) && tabIndex != i) {
						SetTab(i);
						Game1.playSound("smallSelect");
						return;
					}
				}

			// Clickable Recipes
			bool shifting = Game1.oldKBState.IsKeyDown(Keys.LeftShift);
			bool ctrling  = Game1.oldKBState.IsKeyDown(Keys.LeftControl);

			if (CurrentPage != null)
				foreach (var cmp in CurrentPage) {
					if (cmp.containsPoint(x, y) && ComponentRecipes.TryGetValue(cmp, out IRecipe recipe)) {
						if (Editing) {
							UpdateRecipeInCategory(recipe);

						} else if (!cmp.hoverText.Equals("ghosted")) {
							bool shifted = shifting && recipe.Stackable;
							PerformCraft(recipe, shifted ? (ctrling ? 25 : 5) : 1, moveResultToInventory: shifting);
						}

						cmp.scale = cmp.baseScale;
						return;
					}
				}

			// Toggle Editing
			if (btnToggleEdit != null && btnToggleEdit.containsPoint(x, y)) {
				Game1.playSound("smallSelect");
				ToggleEditMode();
				btnToggleEdit.scale = btnToggleEdit.baseScale;
				return;
			}

			// Settings
			if (btnSettings != null && btnSettings.containsPoint(x, y)) {
				Game1.playSound("smallSelect");
				Mod.OpenGMCM();
				return;
			}

			// Toggle Favorites
			if (!Editing && btnToggleFavorites != null && btnToggleFavorites.containsPoint(x, y)) {
				Game1.playSound("smallSelect");
				FavoritesOnly = !FavoritesOnly;
				LayoutRecipes();
				btnToggleFavorites.sourceRect = SourceFavorites;
				btnToggleFavorites.scale = btnToggleFavorites.baseScale;
				return;
			}

			// Toggle Seasoning
			if (!Editing && btnToggleSeasoning != null && btnToggleSeasoning.containsPoint(x, y)) {
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
				Game1.playSound("throwDownITem");
				Game1.createItemDebris(HeldItem, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
				HeldItem = null;
				return;
			}
		}

		public override void receiveRightClick(int x, int y, bool playSound = true) {
			if (!Editing)
				HeldItem = inventory.rightClick(x, y, HeldItem, playSound);

			// Toggle Seasoning
			if (!Editing && btnToggleSeasoning != null && btnToggleSeasoning.containsPoint(x, y)) {
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
			// TODO: Right-click to open sub-menu?
			if (CurrentPage != null)
				foreach (var cmp in CurrentPage) {
					if (cmp.containsPoint(x, y) && ComponentRecipes.TryGetValue(cmp, out IRecipe recipe)) {
						if (Editing) {
							UpdateCategorySprite(recipe);

						} else if (!cmp.hoverText.Equals("ghosted")) {
							if (Mod.intSSR.IsLoaded && Game1.oldKBState.IsKeyDown(Keys.LeftShift))
								return;

							bool is_favorite = Favorites.Contains(recipe);
							Game1.playSound("coin");
							Mod.Favorites.SetFavoriteRecipe(recipe.Name, cooking, !is_favorite, Game1.player);
							Mod.Favorites.SaveFavorites();

							// Update our favorite tracking.
							if (is_favorite)
								Favorites.Remove(recipe);
							else
								Favorites.Add(recipe);

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
						}

						cmp.scale = cmp.baseScale;
						return;
					}
				}

			// ???
		}

		public override void performHoverAction(int x, int y) {
			base.performHoverAction(x, y);

			hoverTitle = "";
			hoverText = "";
			hoverRecipe = null;
			hoverAmount = -1;

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
					if (tab.Component.containsPoint(x, y)) {
						hover_tabs = true;
						hoverText = tab.Component.label;
						break;
					}
				}

				if (!hover_tabs && Favorites.Count == 0 && tabIndex > 0 && Tabs[0].Category.Id == "favorites") {
					Tabs.RemoveAt(0);
					tabIndex--;

					foreach (TabInfo tab in Tabs)
						tab.Component.bounds.Y -= 64;
				}
			}


			// Navigation Buttons
			if (btnPageUp != null)
				btnPageUp.tryHover(x, pageIndex > 0 ? y : -1);

			if (btnPageDown != null)
				btnPageDown.tryHover(x, pageIndex < Pages.Count - 1 ? y : -1);

			// Toggle Buttons
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

			// If the mode changed, regenerate the fancy tool-tip.
			if (mode != hoverMode) {
				hoverMode = mode;
				// TODO: Tooltip stuff

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
			b.End();
			b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

			if (Editing || Tabs.Count > 1)
				for (int i = 0; i < Tabs.Count; i++) {
					TabInfo tab = Tabs[i];
					int x = tab.Component.bounds.X + (tabIndex == i ? 8 : 0);

					// Rotate the background from the main sprite sheet.
					// We reuse the main sprite sheet to best support UI
					// reskins from other people.
					b.Draw(
						Game1.mouseCursors,
						new Vector2(x, tab.Component.bounds.Y),
						SpriteHelper.Tabs.BACKGROUND,
						Color.White,
						3 * (float) Math.PI / 2f,
						new Vector2(16, 0),
						4f,
						SpriteEffects.None,
						0.0001f
					);

					tab.Sprite?.Draw(b, new Vector2(x + 14, tab.Component.bounds.Y + 8), 3f);
				}


			// Recipes
			List<ClickableTextureComponent> recipes = CurrentPage;
			IList<Item> items = null;
			if (recipes != null)
				items = GetEstimatedContainerContents();

			bool shifting = Game1.oldKBState.IsKeyDown(Keys.LeftShift);
			bool ctrling = Game1.oldKBState.IsKeyDown(Keys.LeftControl);

			if (recipes != null)
				foreach (var cmp in recipes) {
					if (!ComponentRecipes.TryGetValue(cmp, out IRecipe recipe))
						continue;

					bool in_category = Editing && IsRecipeInCategory(recipe);
					bool shifted = shifting && recipe.Stackable;

					if (!Editing && cmp.hoverText.Equals("ghosted")) {
						// Unlearned Recipe
						cmp.DrawBounded(b, Color.Black * 0.35f, 0.89f);

					} else if (Editing ? !in_category : !recipe.HasIngredients(Game1.player, items, UnsafeInventories, Quality)) {
						// Recipe without Ingredients
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
							new Rectangle(338, 400, 8, 8),
							Color.White,
							0f,
							Vector2.Zero,
							2f,
							SpriteEffects.None,
							1f
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

			// Hover Item
			if (hoverItem != null)
				IClickableMenu.drawToolTip(b, hoverText, hoverTitle, hoverItem, HeldItem != null);
			else if (!string.IsNullOrEmpty(hoverText)) {
				if (hoverAmount > 0)
					IClickableMenu.drawToolTip(b, hoverText, hoverTitle, null, true, moneyAmountToShowAtBottom: hoverAmount);
				else
					IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont, HeldItem != null ? 64 : 0, HeldItem != null ? 64 : 0);
			}

			// Held Item
			if (!Editing)
				HeldItem?.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 16, Game1.getOldMouseY() + 16), 1f);

			// TODO: Tooltip

			if (Standalone) {
				Game1.mouseCursorTransparency = 1f;
				drawMouse(b);
			}

			// TODO: Better tooltip

			if (hoverRecipe == null)
				return;

			int offset = HeldItem != null ? 48 : 0;

			string[] buffIconsToDisplay = null;
			Item recipeItem = lastRecipeHover.Value;
			if (cooking && recipeItem != null) {
				string[] temp = Game1.objectInformation[recipeItem.ParentSheetIndex].Split('/');
				if (temp.Length > 7)
					buffIconsToDisplay = temp[7].Split(' ');
			}

			if (Editing && recipeItem != null)
				IClickableMenu.drawHoverText(b, recipeItem.DisplayName, Game1.smallFont);
			else if (!Editing) {
				bool shifted = shifting && hoverRecipe.Stackable;

				int quantity = hoverRecipe.QuantityPerCraft * (shifted ? (ctrling ? 25 : 5) : 1);
				int craftable = int.MaxValue;
				List<ISimpleNode> ingredients = new();

				bool supports_quality = true;

				foreach (var entry in hoverRecipe.Ingredients) {
					int amount = entry.GetAvailableQuantity(Game1.player, items, UnsafeInventories, Quality);
					int quant = entry.Quantity * (shifted ? (ctrling ? 25 : 5) : 1);
					craftable = Math.Min(craftable, amount / entry.Quantity);

					if ( ! entry.SupportsQuality)
						supports_quality = false;

					Color color = amount < entry.Quantity ?
						Color.Red :
						amount < quant ?
							Color.OrangeRed :
								Game1.textColor;

					var ebuilder = SimpleHelper
						.Builder(LayoutDirection.Horizontal, margin: 8)
						.Sprite(
							new SpriteInfo(entry.Texture, entry.SourceRectangle),
							scale: 2,
							quantity: quant,
							align: Alignment.Middle
						)
						.Text(
							entry.DisplayName,
							color: color,
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
				string text = hoverRecipe.DisplayName + (quantity > 1 ? $" x{quantity}" : "");

				if (Game1.options.showAdvancedCraftingInformation)
					builder.Group(margin: 8)
						.Text(text, font: Game1.dialogueFont, shadow: true)
						.Text($"({craftable})", align: Alignment.Middle)
					.EndGroup();
				else
					builder.Text(text, font: Game1.dialogueFont, shadow: true);

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

				builder
					.Divider()
					.Flow(FlowHelper.Builder().Text(hoverRecipe.Description).Build());

				List<ISimpleNode> buffs = new();

				if (recipeItem is SObject sobj && sobj.Edibility != -300) {
					int health = recipeItem.healthRecoveredOnConsumption();
					int stamina = recipeItem.staminaRecoveredOnConsumption();

					if (health != 0) {
						string label = (Convert.ToInt32(health) > 0 ? "+" : "") + health;

						buffs.Add(SimpleHelper.Builder(LayoutDirection.Horizontal, margin: 0)
							.Texture(
								Game1.mouseCursors,
								new Rectangle(health < 0 ? 140 : 0, health < 0 ? 428 : 438, 10, 10),
								scale: 3,
								align: Alignment.Middle
							)
							.Space(false, 4)
							.Text(
								Game1.content.LoadString("Strings\\UI:ItemHover_Health", label),
								align: Alignment.Middle
							)
							.GetLayout()
						);
					}

					if (stamina != 0) {
						string label = (Convert.ToInt32(stamina) > 0 ? "+" : "") + stamina;

						buffs.Add(SimpleHelper.Builder(LayoutDirection.Horizontal, margin: 0)
							.Texture(
								Game1.mouseCursors,
								new Rectangle(stamina < 0 ? 140 : 0, 428, 10, 10),
								scale: 3,
								align: Alignment.Middle
							)
							.Space(false, 4)
							.Text(
								Game1.content.LoadString("Strings\\UI:ItemHover_Energy", label),
								align: Alignment.Middle
							)
							.GetLayout()
						);
					}
				}

				if (buffIconsToDisplay != null) {
					for (int idx = 0; idx < buffIconsToDisplay.Length; idx++) {
						string buff = buffIconsToDisplay[idx];
						if (buff.Equals("0"))
							continue;

						string label = (Convert.ToInt32(buff) > 0 ? "+" : "") + buff;
						if (idx <= 11)
							label = Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + idx, label);

						buffs.Add(SimpleHelper.Builder(LayoutDirection.Horizontal, margin: 0)
							.Texture(
								Game1.mouseCursors,
								new Rectangle(10 + idx * 10, 428, 10, 10),
								scale: 3,
								align: Alignment.Middle
							)
							.Space(false, 4)
							.Text(label, align: Alignment.Middle)
							.GetLayout()
						);
					}
				}

				builder.AddRange(buffs);

				if (Game1.options.showAdvancedCraftingInformation && hoverRecipe != null) {
					int count = hoverRecipe.GetTimesCrafted(Game1.player);
					if (count > 0)
						builder
							.Text(
								I18n.Tooltip_Crafted(count),
								color: Game1.textColor * 0.5f
							);
				}

				builder
					.GetLayout()
					.DrawHover(
						b,
						Game1.smallFont,
						offsetX: HeldItem == null ? 0 : 48,
						offsetY: HeldItem == null ? 0 : 48
					);
			}
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
