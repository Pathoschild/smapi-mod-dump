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
using System.Diagnostics.CodeAnalysis;

using HarmonyLib;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Inventories;
using StardewValley.Menus;

namespace Leclair.Stardew.BetterCrafting.Menus;

public class TrashGrabMenu : ItemGrabMenu {

	private static Action<ItemGrabMenu, SpriteBatch>? DrawMenu;

	private static Action<ItemGrabMenu, SpriteBatch> GetDrawMenu() {
		if (DrawMenu is null) {
			var method = AccessTools.Method(typeof(ItemGrabMenu), nameof(ItemGrabMenu.draw), [typeof(SpriteBatch)]);
			DrawMenu = method.CreateAction<ItemGrabMenu, SpriteBatch>();
		}

		return DrawMenu;
	}

	private readonly ModEntry Mod;
	private readonly IInventory SourceInventory;

	private Texture2D RecyclingBinTexture;

	public TrashGrabMenu(ModEntry mod, IInventory sourceInventory) : base(
		sourceInventory,
		reverseGrab: false,
		showReceivingMenu: true,
		highlightFunction: null,
		behaviorOnItemSelectFunction: null,
		behaviorOnItemGrab: OnGrabItem,
		message: null,
		canBeExitedWithKey: true
	) {
		Mod = mod;
		SourceInventory = sourceInventory;

		initializeUpperRightCloseButton();
		GetDrawMenu();

		LoadTextures();

		ItemsToGrabMenu.highlightMethod = ShouldHighlightItem;
		inventory.highlightMethod = ShouldHighlightItem;
	}

	[MemberNotNull(nameof(RecyclingBinTexture))]
	public void LoadTextures() {
		RecyclingBinTexture = Mod.ThemeManager.Load<Texture2D>("recycle.png");
	}

	public static void OnGrabItem(Item item, Farmer who) {
		who.Money -= Utility.getTrashReclamationPrice(item, who);
	}

	public bool ShouldHighlightItem(Item item) {
		if (SourceInventory.Contains(item)) {
			if (heldItem != null)
				return false;

			int price = Utility.getTrashReclamationPrice(item, Game1.player);
			return Game1.player.Money >= price;
		}

		return true;
	}

	public override void performHoverAction(int x, int y) {
		base.performHoverAction(x, y);

		if (hoveredItem != null && SourceInventory.Contains(hoveredItem))
			hoverAmount = Utility.getTrashReclamationPrice(hoveredItem, Game1.player);
	}


	public override void draw(SpriteBatch b) {
		if (drawBG && !Game1.options.showClearBackgrounds)
			b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);

		/*var node = SimpleHelper.Builder()
			.Text("Trash Reclamation")
			.GetLayout();

		var sz = node.GetSize(Game1.dialogueFont, Vector2.Zero);

		int yPos = ItemsToGrabMenu.yPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + storageSpaceTopBorderOffset;

		node.DrawHover(
			b,
			Game1.dialogueFont,
			overrideX: (int) (xPositionOnScreen + (width - sz.X) / 2),
			overrideY: yPos - 12
		);*/

		// Draw our Source Tab
		b.Draw(Game1.mouseCursors, new Vector2(ItemsToGrabMenu.xPositionOnScreen - 100, base.yPositionOnScreen + 64 + 16), new Rectangle(16, 368, 12, 16), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
		b.Draw(Game1.mouseCursors, new Vector2(ItemsToGrabMenu.xPositionOnScreen - 100, base.yPositionOnScreen + 64 - 16), new Rectangle(21, 368, 11, 16), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

		var sourceRect = new Rectangle(21, 3, 10, 11);

		b.Draw(RecyclingBinTexture, new Vector2(ItemsToGrabMenu.xPositionOnScreen - 80, base.yPositionOnScreen + 64 - 44), sourceRect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

		// Draw the Menu
		bool old_drawBG = drawBG;
		drawBG = false;
		DrawMenu?.Invoke(this, b);
		drawBG = old_drawBG;
		//base.draw(b);

		if (hoveredItem != null && hoverAmount > 0) {
			var node = SimpleHelper.Builder(Common.UI.SimpleLayout.LayoutDirection.Horizontal)
				.Texture(Game1.mouseCursors, Models.CurrencyIngredient.ICON_MONEY, scale: 2)
				.Text($" {hoverAmount}")
				.GetLayout();

			var sz = node.GetSize(Game1.smallFont, Vector2.Zero);
			node.DrawHover(b, Game1.smallFont, offsetY: (int) -sz.Y - borderWidth);
		}

	}

}
