/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CustomCommunityCentre
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommunityKitchen
{
	public class ShopMenuNoInventory : StardewValley.Menus.ShopMenu
	{
		private struct ItemHoverInfo
		{
			public readonly ISalable lastHoverItem;
			public readonly string hoverText;
			public readonly string boldTitleText;
			public readonly int extraItemIndex;
			public readonly int extraItemAmount;

			public ItemHoverInfo(ShopMenu shopMenu)
			{
				lastHoverItem = shopMenu.hoveredItem;
				hoverText = ModEntry.Instance.Helper.Reflection
					.GetField<string>(obj: shopMenu, name: "hoverText")
					.GetValue();
				boldTitleText = ModEntry.Instance.Helper.Reflection
					.GetField<string>(obj: shopMenu, name: "boldTitleText")
					.GetValue();
				extraItemIndex = ModEntry.Instance.Helper.Reflection
					.GetMethod(obj: shopMenu, name: "getHoveredItemExtraItemIndex")
					.Invoke<int>();
				extraItemAmount = ModEntry.Instance.Helper.Reflection
					.GetMethod(obj: shopMenu, name: "getHoveredItemExtraItemAmount")
					.Invoke<int>();
			}
		}

		public Dictionary<Item, int> DeliveryItemsAndCounts = new();
		public Rectangle TextureBoxArea;
		public Rectangle PlatesArea;
		public int PlatesToFit;
		private ItemHoverInfo _itemHoverInfo;

		private static readonly Point TextureBoxMargins = new(20, 12);
		private static readonly Rectangle PlateSourceArea = new(0, 48 + 8, 24, 24);


		public ShopMenuNoInventory(Dictionary<ISalable, int[]> itemPriceAndStock, int currency = 0, string who = null,
			Func<ISalable, Farmer, int, bool> on_purchase = null, Func<ISalable, bool> on_sell = null, string context = null)
			: base(itemPriceAndStock, currency, who, on_purchase, on_sell, context)
		{
			this.SetOrderArea();
		}

		public ShopMenuNoInventory(List<ISalable> itemsForSale, int currency = 0, string who = null,
			Func<ISalable, Farmer, int, bool> on_purchase = null, Func<ISalable, bool> on_sell = null, string context = null)
			: base(itemsForSale, currency, who, on_purchase, on_sell, context)
		{
			this.SetOrderArea();
		}

		public void SetOrderArea()
		{
			// magic number bullshit
			this.TextureBoxArea = new Rectangle(
				this.xPositionOnScreen + this.width - this.inventory.width - 32 - 24,
				this.yPositionOnScreen + this.height - 256 + 40,
				this.inventory.width + 56,
				this.height - 448 + 20);
			this.PlatesToFit = ((this.TextureBoxArea.Width - ((ShopMenuNoInventory.TextureBoxMargins.X + IClickableMenu.spaceToClearSideBorder) * 2))
				/ (ShopMenuNoInventory.PlateSourceArea.Width * Game1.pixelZoom));
			Point platesAreaDimensions = new (
				this.PlatesToFit * ShopMenuNoInventory.PlateSourceArea.Width * Game1.pixelZoom,
				ShopMenuNoInventory.PlateSourceArea.Height * Game1.pixelZoom);
			Vector2 platesAreaOffset = new (
				IClickableMenu.spaceToClearSideBorder + ShopMenuNoInventory.TextureBoxMargins.X + 16,
				16 + ShopMenuNoInventory.TextureBoxMargins.Y);
			this.PlatesArea = new Rectangle(
				this.TextureBoxArea.X + (int)platesAreaOffset.X,
				this.TextureBoxArea.Y + (int)platesAreaOffset.Y + Game1.dialogueFont.LineSpacing + ShopMenuNoInventory.TextureBoxMargins.Y * 2,
				platesAreaDimensions.X,
				platesAreaDimensions.Y);
			this.inventory.yPositionOnScreen = 999999;
		}

		public void AddToOrderDisplay(Item item)
		{
			foreach (Item i in this.DeliveryItemsAndCounts.Keys.ToList())
			{
				if (i.ParentSheetIndex == item.ParentSheetIndex || i.Name == item.Name)
				{
					this.DeliveryItemsAndCounts[i] += item.Stack;
					return;
				}
			}
			this.DeliveryItemsAndCounts[item] = item.Stack;
		}

		public override void draw(SpriteBatch b)
		{
			Game1.mouseCursorTransparency = 0f;
			base.draw(b);

			Vector2 position = Utility.PointToVector2(this.TextureBoxArea.Location)
				+ new Vector2(IClickableMenu.spaceToClearSideBorder, 16)
				+ Utility.PointToVector2(ShopMenuNoInventory.TextureBoxMargins);

			// Draw order title
			{
				string text = ModEntry.i18n.Get("label.phone.delivery.title");
				SpriteFont font = Game1.dialogueFont;
				float textScale = 1f;
				Utility.drawTextWithShadow(
					b: b,
					text: text,
					font: font,
					position: position,
					color: Game1.textColor,
					scale: textScale);
				position.Y += font.MeasureString(text).Y * textScale;
			}

			position = Utility.PointToVector2(this.PlatesArea.Location);
			
			Point boxBiggening = new (16, 8);
			Rectangle areaBox = this.PlatesArea;
			areaBox.X -= boxBiggening.X;
			areaBox.Y -= boxBiggening.Y;
			areaBox.Width += boxBiggening.X * 2;
			areaBox.Height += boxBiggening.Y * 2;
			IClickableMenu.drawTextureBox(
				b: b,
				texture: Game1.mouseCursors,
				sourceRect: new Rectangle(384, 396, 15, 15),
				x: areaBox.X,
				y: areaBox.Y,
				width: areaBox.Width,
				height: areaBox.Height,
				color: Color.White,
				scale: Game1.pixelZoom,
				drawShadow: false);

			Vector2 itemOffsetFromPlate = new Vector2(StardewValley.Object.spriteSheetTileSize)
				- new Vector2(ShopMenuNoInventory.PlateSourceArea.Width, ShopMenuNoInventory.PlateSourceArea.Height);
			List<Item> items = this.DeliveryItemsAndCounts.Keys.ToList();
			for (int i = 0; i < this.DeliveryItemsAndCounts.Count; ++i)
			{
				// Draw plates under items
				b.Draw(
					texture: GusDeliveryService.DeliveryTexture.Value,
					position: position,
					sourceRectangle: ShopMenuNoInventory.PlateSourceArea,
					color: Color.White,
					rotation: 0f,
					origin: Vector2.Zero,
					scale: Game1.pixelZoom,
					effects: SpriteEffects.None,
					layerDepth: 1f);
				// Draw delivery item icons
				b.Draw(
					texture: Game1.objectSpriteSheet,
					position: position - itemOffsetFromPlate,
					sourceRectangle: Game1.getSourceRectForStandardTileSheet(
						tileSheet: Game1.objectSpriteSheet,
						items[i].ParentSheetIndex,
						StardewValley.Object.spriteSheetTileSize,
						StardewValley.Object.spriteSheetTileSize),
					color: Color.White,
					rotation: 0f,
					origin: Vector2.Zero,
					scale: Game1.pixelZoom,
					effects: SpriteEffects.None,
					layerDepth: 1f);
				// Draw delivery item counts
				if (this.DeliveryItemsAndCounts[items[i]] > 1)
				{
					const float tinyScale = 3f;
					const int tinyHeight = 7;
					Vector2 stackCountOffset = new (
						x: (ShopMenuNoInventory.PlateSourceArea.Width * Game1.pixelZoom)
							- Utility.getWidthOfTinyDigitString(this.DeliveryItemsAndCounts[items[i]], tinyScale)
							- (3 * tinyScale),
						y: (ShopMenuNoInventory.PlateSourceArea.Height * Game1.pixelZoom)
							- (tinyHeight * tinyScale)
							- (3 * tinyScale));
					Utility.drawTinyDigits(
						toDraw: this.DeliveryItemsAndCounts[items[i]],
						b: b,
						position: position + stackCountOffset,
						scale: tinyScale,
						layerDepth: 1f,
						c: Color.White);
				}

				position.X += ShopMenuNoInventory.PlateSourceArea.Width * Game1.pixelZoom;
				if (i > 0 && i % this.PlatesToFit == 0)
				{
					position.Y += ShopMenuNoInventory.PlateSourceArea.Height * Game1.pixelZoom;
					position.X = PlatesArea.X + (ShopMenuNoInventory.PlateSourceArea.Width * Game1.pixelZoom / 2);
				}
			}

			// duplicated base game logic
			Game1.mouseCursorTransparency = 1f;
			this.drawMouse(b);

			string hoverText = ModEntry.Instance.Helper.Reflection
				.GetField<string>(obj: this, name: "hoverText")
				.GetValue();
			string boldTitleText = ModEntry.Instance.Helper.Reflection
				.GetField<string>(obj: this, name: "boldTitleText")
				.GetValue();
			int extraItemIndex = ModEntry.Instance.Helper.Reflection
				.GetMethod(obj: this, name: "getHoveredItemExtraItemIndex")
				.Invoke<int>();
			int extraItemAmount = ModEntry.Instance.Helper.Reflection
				.GetMethod(obj: this, name: "getHoveredItemExtraItemAmount")
				.Invoke<int>();
			if (!string.IsNullOrWhiteSpace(hoverText))
			{
				IClickableMenu.drawToolTip(
					b: b,
					hoverText: hoverText,
					hoverTitle: boldTitleText,
					hoveredItem: this.hoveredItem as Item,
					heldItem: false,
					healAmountToDisplay: -1,
					currencySymbol: this.currency,
					extraItemToShowIndex: extraItemIndex,
					extraItemToShowAmount: extraItemAmount,
					craftingIngredients: null,
					moneyAmountToShowAtBottom: (this.hoverPrice > 0) ? this.hoverPrice : (-1));
			}
		}

		public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
		{
			base.gameWindowSizeChanged(oldBounds, newBounds);
			this.SetOrderArea();
		}

		public override void performHoverAction(int x, int y)
		{
			base.performHoverAction(x, y);
		}

		public override void receiveGamePadButton(Buttons b)
		{
			base.receiveGamePadButton(b);
		}

		public override void receiveKeyPress(Keys key)
		{
			base.receiveKeyPress(key);
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			base.receiveLeftClick(x, y, playSound);
		}

		public override void receiveRightClick(int x, int y, bool playSound = true)
		{
			base.receiveRightClick(x, y, playSound);
		}

		public override void update(GameTime time)
		{
			base.update(time);
			if (this.hoveredItem != this._itemHoverInfo.lastHoverItem)
			{
				this._itemHoverInfo = new ItemHoverInfo(shopMenu: this);
			}
		}

		protected override void cleanupBeforeExit()
		{
			base.cleanupBeforeExit();
		}
	}
}
