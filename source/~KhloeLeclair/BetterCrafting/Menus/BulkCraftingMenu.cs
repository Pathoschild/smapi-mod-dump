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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Leclair.Stardew.BetterCrafting.Models;
using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.Inventory;
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.UI.SimpleLayout;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.BetterCrafting.Menus;

public class BulkCraftingMenu : MenuSubscriber<ModEntry> {

	public readonly IRecipe Recipe;
	public readonly BetterCraftingPage Menu;

	// Caching
	private readonly Dictionary<IIngredient, int> AvailableQuantity = [];

	public int Quantity { get; private set; } = 1;
	private ISimpleNode Layout;
	private Vector2 LayoutSize;

	public int Craftable;
	public int CraftingLimit;

	public TextBox txtQuantity;
	public ClickableComponent btnQuantity;

	public ClickableTextureComponent btnCraft;
	public ClickableTextureComponent btnLess;
	public ClickableTextureComponent btnMore;

	// Paginate Ingredients
	public int currentPage = 1;
	public int totalPages = 1;
	public ClickableTextureComponent? btnPrev;
	public ClickableTextureComponent? btnNext;

	public float maxWidth = -1;

	private readonly SeasoningMode Seasoning;
	private int SeasoningAmount;
	private readonly IIngredient? SeasonIngred;

	public BulkCraftingMenu(ModEntry mod, BetterCraftingPage menu, IRecipe recipe, int initial)
	: base(mod) {
		Menu = menu;
		Recipe = recipe;

		if (!Menu.cooking || Mod.Config.UseSeasoning == SeasoningMode.Disabled)
			Seasoning = SeasoningMode.Disabled;
		else {
			Item? obj = Recipe.CreateItemSafe();
			if (obj is SObject sobj && sobj.Quality == 0) {
				Seasoning = Mod.Config.UseSeasoning;
				SeasonIngred = BetterCraftingPage.SEASONING_RECIPE[0];
			} else
				Seasoning = SeasoningMode.Disabled;
		}

		SetQuantity(initial, false);

		UpdateLayout();

		int width = (int) LayoutSize.X + 32;
		int height = (int) LayoutSize.Y + 32 +
			32 + // Half Button
			48; // Text Input

		Vector2 point = Utility.getTopLeftPositionForCenteringOnScreen(width, height);

		initialize((int) point.X, (int) point.Y, width, height, true);

		txtQuantity = new TextBox(
			textBoxTexture: Game1.content.Load<Texture2D>(@"LooseSprites\textBox"),
			null,
			Game1.smallFont,
			Game1.textColor
		) {
			X = 0,
			Y = 0,
			Width = 200,
			Text = Quantity.ToString(),
			numbersOnly = true
		};

		btnQuantity = new ClickableComponent(
			bounds: new Rectangle(0, 0, txtQuantity.Width, 48),
			name: ""
		) {
			myID = 2,
			upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
		};

		txtQuantity.OnEnterPressed += sender => {
			sender.Selected = false;
			if (!string.IsNullOrEmpty(txtQuantity.Text))
				SetQuantity(StringToValue(txtQuantity.Text));

			txtQuantity.Text = Quantity.ToString();
		};

		txtQuantity.OnTabPressed += sender => {
			sender.Selected = false;
			if (!string.IsNullOrEmpty(txtQuantity.Text))
				SetQuantity(StringToValue(txtQuantity.Text));

			txtQuantity.Text = Quantity.ToString();
			snapToDefaultClickableComponent();
		};

		btnLess = new ClickableTextureComponent(
			new Rectangle(0, 0, 28, 32),
			Menu.Background ?? Game1.mouseCursors,
			Menu.Background is null
				? new Rectangle(177, 345, 7, 8)
				: Sprites.Other.BTN_MINUS,
			4f
		) {
			myID = 1,
			upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
		};

		btnMore = new ClickableTextureComponent(
			new Rectangle(0, 0, 28, 32),
			Menu.Background ?? Game1.mouseCursors,
			Menu.Background is null
				? new Rectangle(184, 345, 7, 8)
				: Sprites.Other.BTN_PLUS,
			4f
		) {
			myID = 2,
			upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
		};

		btnCraft = new ClickableTextureComponent(
			new Rectangle(0, 0, 64, 64),
			Menu.Background ?? Game1.mouseCursors,
			Menu.Background is null
				? new Rectangle(366, 373, 16, 16)
				: Sprites.Other.BTN_HAMMER,
			scale: 4f
		) {
			myID = 4,
			upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
		};

		int ingredients = Recipe.Ingredients?.Length ?? 0;
		if ( ingredients > 20 ) {
			totalPages = (int) Math.Ceiling(ingredients / 20f);

			btnPrev = new ClickableTextureComponent(
				new Rectangle(0, 0, 64, 64),
				Menu.Background ?? Game1.mouseCursors,
				Menu.Background is null
					? new Rectangle(349, 492, 16, 16)
					: Sprites.CustomScroll.PAGE_LEFT,
				scale: 4f
			) {
				myID = 5,
				upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			};

			btnNext = new ClickableTextureComponent(
				new Rectangle(0, 0, 64, 64),
				Menu.Background ?? Game1.mouseCursors,
				Menu.Background is null
					? new Rectangle(365, 492, 16, 16)
					: Sprites.CustomScroll.PAGE_RIGHT,
				scale: 4f
			) {
				myID = 6,
				upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			};
		}

		UpdateComponents();

		if (Game1.options.SnappyMenus)
			snapToDefaultClickableComponent();
	}

	public override void snapToDefaultClickableComponent() {
		currentlySnappedComponent = btnMore;
		snapCursorToCurrentSnappedComponent();
	}

	#region Ingredient Layout

	public void UpdateComponents() {
		width = (int) LayoutSize.X + 32;
		height = (int) LayoutSize.Y + 32 +
			32 + // Half Button
			48; // Text Input

		Vector2 point = Utility.getTopLeftPositionForCenteringOnScreen(width, height);

		xPositionOnScreen = (int) point.X;
		yPositionOnScreen = (int) point.Y;

		if (upperRightCloseButton != null) {
			upperRightCloseButton.bounds.X = xPositionOnScreen + width - 32;
			upperRightCloseButton.bounds.Y = yPositionOnScreen - 16;
		}

		txtQuantity.X = xPositionOnScreen + (width - txtQuantity.Width) / 2;
		txtQuantity.Y = yPositionOnScreen + (int) LayoutSize.Y + 24;

		btnQuantity.bounds.X = txtQuantity.X;
		btnQuantity.bounds.Y = txtQuantity.Y;

		btnLess.bounds.X = txtQuantity.X - btnLess.bounds.Width - 16;
		btnLess.bounds.Y = txtQuantity.Y + 8;

		btnMore.bounds.X = txtQuantity.X + txtQuantity.Width + 16;
		btnMore.bounds.Y = txtQuantity.Y + 8;

		btnCraft.bounds.X = xPositionOnScreen + (width - 64) / 2;
		btnCraft.bounds.Y = yPositionOnScreen + height - 32;

		if (btnPrev is not null) {
			btnPrev.bounds.X = xPositionOnScreen - 16 - 64;
			btnPrev.bounds.Y = yPositionOnScreen + (height - 64) / 2;
		}

		if (btnNext is not null) {
			btnNext.bounds.X = xPositionOnScreen + width + 16;
			btnNext.bounds.Y = yPositionOnScreen + (height - 64) / 2;
		}
	}

	[MemberNotNull(nameof(Layout))]
	public void UpdateLayout() {

		List<ISimpleNode> ingredients = [];

		int crafts = Quantity / Recipe.QuantityPerCraft;

		if (Recipe.Ingredients is not null) {
			int toSkip = (currentPage - 1) * 20;

			foreach (var entry in Recipe.Ingredients) {
				if (toSkip > 0) {
					toSkip--;
					continue;
				}

				if (!AvailableQuantity.TryGetValue(entry, out int amount))
					amount = 0;

				ingredients.Add(BuildIngredientRow(entry, amount, crafts));
				if (ingredients.Count >= 20)
					break;
			}

			if (currentPage > 1)
				while (ingredients.Count < 20)
					ingredients.Add(new TextNode(" "));
		}

		var builder = SimpleHelper
			.Builder(minSize: new Vector2(4 * 80, 0))
			.Text(Menu.cooking ? I18n.Bulk_Cooking() : I18n.Bulk_Crafting(), font: Game1.dialogueFont, align: Alignment.HCenter)
			//.Divider()
			.Group(margin: 8)
				.Space()
				.Sprite(
					Recipe is IDynamicDrawingRecipe ddr
						? new DynamicRecipeSpriteInfo(ddr)
						: new SpriteInfo(Recipe.Texture, Recipe.SourceRectangle),
					quantity: Quantity
				)
				.Space(expand: false)
				.Group()
					.Text(Recipe.DisplayName)
					.Text(Menu.cooking ? I18n.Bulk_Cookable(Craftable) : I18n.Bulk_Craftable(Craftable), color: ((Menu.Theme.CustomTooltip ? Menu.Theme.TooltipTextColor ?? Menu.Theme.TextColor : null) ?? Game1.textColor) * .75f)
				.EndGroup()
				.Space()
			.EndGroup();

		if (ingredients.Count > 0) {
			builder.Divider();

			if (ingredients.Count < 4)
				builder.AddSpacedRange(4, ingredients);
			else {
				List<ISimpleNode> left = [];
				List<ISimpleNode> right = [];

				bool right_side = false;

				foreach (var ing in ingredients) {
					if (right_side)
						right.Add(ing);
					else
						left.Add(ing);

					right_side = !right_side;
				}

				builder
					.Group(margin: 8)
						.Group(align: Alignment.Top)
							.AddSpacedRange(4, left)
						.EndGroup()
						.Divider(false)
						.Group(align: Alignment.Top)
							.AddSpacedRange(4, right)
						.EndGroup()
					.EndGroup();
			}
		}

		if (SeasonIngred != null && SeasoningAmount > 0) {
			builder
				.Divider()
				.Add(BuildIngredientRow(SeasonIngred, SeasoningAmount, crafts));
		}

		builder.Divider();

		Layout = builder.GetLayout();
		LayoutSize = Layout.GetSize(Game1.smallFont, new Vector2(400, 0));

		if (currentPage == 1 && maxWidth == -1) {
			if (totalPages == 1)
				maxWidth = LayoutSize.X;
			else {

				var temp = Layout;
				var tempSize = LayoutSize;

				maxWidth = LayoutSize.X;

				for(int page = 2; page <= totalPages; page++) {
					currentPage = page;
					UpdateLayout();
				}

				Layout = temp;
				LayoutSize = tempSize;
			}
		}

		if (maxWidth > LayoutSize.X)
			LayoutSize.X = maxWidth;
		else
			maxWidth = LayoutSize.X;
	}

	private ISimpleNode BuildIngredientRow(IIngredient ing, int available, int crafts) {
		int quant = ing.Quantity * crafts;

		Color color = available < ing.Quantity ?
			(Menu.Theme.QuantityCriticalTextColor ?? Color.Red) :
			available < quant ?
				(Menu.Theme.QuantityWarningTextColor ?? Color.OrangeRed) :
					(Menu.Theme.CustomTooltip ? Menu.Theme.TooltipTextColor ?? Menu.Theme.TextColor : null) ?? Game1.textColor;

		Color? shadow = available < ing.Quantity ?
			Menu.Theme.QuantityCriticalShadowColor :
			available < quant ?
				Menu.Theme.QuantityWarningShadowColor :
					(Menu.Theme.CustomTooltip ? Menu.Theme.TooltipTextShadowColor ?? Menu.Theme.TextShadowColor : null);

		return SimpleHelper
			.Builder(LayoutDirection.Horizontal, margin: 8)
			.Sprite(
				new SpriteInfo(ing.Texture, ing.SourceRectangle),
				scale: 2,
				quantity: quant,
				align: Alignment.VCenter
			)
			.Text(
				ing.DisplayName,
				color: color,
				shadowColor: shadow,
				align: Alignment.VCenter
			)
			.Space()
			.Text(available.ToString(), align: Alignment.VCenter)
			.Texture(
				Game1.mouseCursors,
				SpriteHelper.MouseIcons.BACKPACK,
				2,
				align: Alignment.VCenter
			)
			.GetLayout();
	}

	#endregion

	#region Changing Stuff

	public bool AssertQuantities() {
		bool changed = CheckQuantities();
		if (changed) {
			UpdateLayout();
			UpdateComponents();
		}
		return changed;
	}

	public bool CheckQuantities() {
		IList<Item?>? items = Menu.GetEstimatedContainerContents();
		IList<IBCInventory>? unsaf = Menu.GetUnsafeInventories();

		int old_craftable = Craftable;
		bool changed = false;

		Craftable = int.MaxValue;
		int crafts = Quantity / Recipe.QuantityPerCraft;

		if (Recipe.Ingredients is not null)
			foreach (var entry in Recipe.Ingredients) {
				int amount = entry.GetAvailableQuantity(Game1.player, items, unsaf, Menu.Quality);
				int quant = entry.Quantity * crafts;

				Craftable = Math.Min(amount / entry.Quantity, Craftable);

				if (!AvailableQuantity.ContainsKey(entry) || AvailableQuantity[entry] != amount) {
					AvailableQuantity[entry] = amount;
					changed = true;
				}
			}

		SeasoningAmount = SeasonIngred?.GetAvailableQuantity(
			Game1.player,
			Seasoning == SeasoningMode.Enabled ? items : null,
			Seasoning == SeasoningMode.Enabled ? unsaf : null,
			Menu.Quality
		) ?? 0;

		if (SeasoningAmount > 0 && Craftable > SeasoningAmount) {
			Craftable *= Recipe.QuantityPerCraft;
			CraftingLimit = SeasoningAmount * Recipe.QuantityPerCraft;
		} else {
			Craftable *= Recipe.QuantityPerCraft;
			CraftingLimit = Craftable;
		}

		while (CraftingLimit > 999)
			CraftingLimit -= Recipe.QuantityPerCraft;

		if (old_craftable != Craftable)
			changed = true;

		return changed;
	}

	public bool ChangeQuantity(int amount) {
		return SetQuantity(Quantity + amount);
	}

	public bool SetQuantity(int amount, bool do_update = true) {
		bool changed = CheckQuantities();

		int old = Quantity;
		Quantity = amount;
		if (Quantity > CraftingLimit)
			Quantity = CraftingLimit;
		if (Quantity < Recipe.QuantityPerCraft)
			Quantity = Recipe.QuantityPerCraft;

		Quantity = (int) Math.Ceiling((double) Quantity / Recipe.QuantityPerCraft) * Recipe.QuantityPerCraft;

		if (txtQuantity != null && !txtQuantity.Selected)
			txtQuantity.Text = Quantity.ToString();

		if (old == Quantity && ! changed)
			return false;

		if (do_update) {
			UpdateLayout();
			UpdateComponents();
		}

		return old != Quantity;
	}

	#endregion

	#region Events and Input

	public override void receiveKeyPress(Keys key) {
		if (txtQuantity.Selected)
			return;

		base.receiveKeyPress(key);
	}

	public override void receiveScrollWheelAction(int direction) {
		base.receiveScrollWheelAction(direction);

		if (totalPages > 1) {
			int target = Math.Clamp(currentPage + (direction < 0 ? 1 : -1), 1, totalPages);
			if (target != currentPage) {
				currentPage = target;
				UpdateLayout();
				UpdateComponents();
				Game1.playSound("smallSelect");
				return;
			}
		}

		int amount = Recipe.QuantityPerCraft;
		if (Game1.oldKBState.IsKeyDown(Keys.LeftShift)) {
			amount *= 5;
			if (Game1.oldKBState.IsKeyDown(Keys.LeftControl))
				amount *= 5;
		}

		if (ChangeQuantity(direction > 0 ? amount : -amount))
			Game1.playSound("smallSelect");
	}

	public override void receiveLeftClick(int x, int y, bool playSound = true) {
		base.receiveLeftClick(x, y, playSound);

		int amount = Recipe.QuantityPerCraft;
		if (Game1.oldKBState.IsKeyDown(Keys.LeftShift)) {
			amount *= 5;
			if (Game1.oldKBState.IsKeyDown(Keys.LeftControl))
				amount *= 5;
		}

		if (btnLess.containsPoint(x, y)) {
			if (ChangeQuantity(-amount))
				Game1.playSound("smallSelect");
		}

		if (btnMore.containsPoint(x, y)) {
			if (ChangeQuantity(amount))
				Game1.playSound("smallSelect");
		}

		if ((btnPrev?.containsPoint(x, y) ?? false) && currentPage > 1) {
			currentPage--;
			UpdateLayout();
			UpdateComponents();
			Game1.playSound("smallSelect");
			return;
		}

		if ((btnNext?.containsPoint(x, y) ?? false) && currentPage < totalPages) {
			currentPage++;
			UpdateLayout();
			UpdateComponents();
			Game1.playSound("smallSelect");
			return;
		}

		if (btnCraft.containsPoint(x, y) && Quantity > 0) {
			bool changed = CheckQuantities();
			if (Quantity <= CraftingLimit) {
				Menu.PerformCraft(Recipe, Quantity / Recipe.QuantityPerCraft);
				exitThisMenu();
				Game1.playSound("coin");
				return;
			}

			if (changed) {
				UpdateLayout();
				UpdateComponents();
			}
		}

		txtQuantity.Update();

	}

	public override void performHoverAction(int x, int y) {
		base.performHoverAction(x, y);

		btnLess.tryHover(x, y);
		btnMore.tryHover(x, y);

		btnPrev?.tryHover(x, currentPage > 1 ? y : -1);
		btnNext?.tryHover(x, currentPage < totalPages ? y : -1);

		btnCraft.tryHover(x, (Quantity == 0 || Quantity > Craftable) ? -1 : y);

		txtQuantity.Hover(x, y);
	}

	#endregion

	private static int StringToValue(string input) {
		try {
			return int.Parse(input);
		} catch {
			return 1;
		}
	}

	#region Drawing

	public override void draw(SpriteBatch b) {

		if (!string.IsNullOrEmpty(txtQuantity.Text))
			SetQuantity(StringToValue(txtQuantity.Text));

		// Dim the Background
		b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);

		Texture2D? texture = Menu.Theme.CustomTooltip ? Menu.Background : null;

		// Background
		RenderHelper.DrawBox(
			b,
			texture: texture ?? Game1.menuTexture,
			sourceRect: texture is null
				? RenderHelper.Sprites.NativeDialogue.ThinBox
				: RenderHelper.Sprites.CustomBCraft.ThinBox,
			x: xPositionOnScreen,
			y: yPositionOnScreen,
			width: width,
			height: height,
			color: Color.White,
			scale: texture is null
				? 1f
				: 4f
		);

		Layout?.Draw(
			b,
			new Vector2(xPositionOnScreen + 16, yPositionOnScreen + 16),
			LayoutSize,
			new Vector2(width, height),
			1f,
			Game1.smallFont,
			(texture is null ? null : Menu.Theme.TooltipTextColor ?? Menu.Theme.TextColor) ?? Game1.textColor,
			(texture is null ? null : Menu.Theme.TooltipTextShadowColor ?? Menu.Theme.TextShadowColor)
		);

		txtQuantity.Draw(b);

		if (Quantity <= Recipe.QuantityPerCraft)
			btnLess.draw(b, Color.Gray * 0.35f, 0.89f);
		else
			btnLess.draw(b);

		if (Quantity >= CraftingLimit)
			btnMore.draw(b, Color.Gray * 0.35f, 0.89f);
		else
			btnMore.draw(b);

		if (Quantity > CraftingLimit || Quantity == 0)
			btnCraft.draw(b, Color.DarkGray, 0.89f);
		else
			btnCraft.draw(b);

		if (btnPrev is not null) {
			if (currentPage > 1)
				btnPrev.draw(b);
			else
				btnPrev.draw(b, Color.Gray, 0.89f);
		}

		if (btnNext is not null) {
			if (currentPage < totalPages)
				btnNext.draw(b);
			else
				btnNext.draw(b, Color.Gray, 0.89f);
		}

		// Base Menu
		base.draw(b);

		// Mouse
		Game1.mouseCursorTransparency = 1f;
		if (!Menu.Theme.CustomMouse || !RenderHelper.DrawMouse(b, Menu.Background, RenderHelper.Sprites.BCraftMouse))
			drawMouse(b);
	}

	#endregion

}
