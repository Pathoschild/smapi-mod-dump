using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

using SObject = StardewValley.Object;

namespace BetterShippingBox.Menus
{
	public class BetterShippingMenu : IClickableMenu
	{
		private string hoverText = "";

		private string boldTitleText = "";

		private readonly List<ClickableComponent> shippedItemButtons = new List<ClickableComponent>();

		private readonly List<TemporaryAnimatedSprite> animations = new List<TemporaryAnimatedSprite>();

		public const int itemsPerPage = 4;

		public const int numberRequiredForExtraItemTrade = 5;

		private InventoryMenu inventory;

		private Item heldItem;

		private Item hoveredItem;

		private Rectangle scrollBarRunner;

		private int currentItemIndex;

		private ClickableTextureComponent upArrow;

		private ClickableTextureComponent downArrow;

		private ClickableTextureComponent scrollBar;

		private ClickableComponent shippingBox;

		private bool scrolling;

		public BetterShippingMenu() : base(Game1.viewport.Width / 2 - (800 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2, 800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2, true)
		{
			bool flag = Game1.viewport.Width < 1500;
			if (flag)
			{
				this.xPositionOnScreen = Game1.tileSize / 2;
			}
			Game1.player.forceCanMove();
			Game1.playSound("dwop");
			this.inventory = new InventoryMenu(this.xPositionOnScreen + Game1.tileSize / 2 + (Game1.tileSize / 3 + Game1.tileSize / 11), this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + Game1.tileSize * 5 + Game1.pixelZoom * 10, false, null, new InventoryMenu.highlightThisItem(this.HighlightItemToShip), -1, 3, 0, 0, true)
			{
				showGrayedOutSlots = true
			};
			this.upArrow = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + Game1.tileSize / 4, this.yPositionOnScreen + Game1.tileSize / 4, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), (float)Game1.pixelZoom, false);
			this.downArrow = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + Game1.tileSize / 4, this.yPositionOnScreen + this.height - Game1.tileSize, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), (float)Game1.pixelZoom, false);
			this.scrollBar = new ClickableTextureComponent(new Rectangle(this.upArrow.bounds.X + Game1.pixelZoom * 3, this.upArrow.bounds.Y + this.upArrow.bounds.Height + Game1.pixelZoom, 6 * Game1.pixelZoom, 10 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), (float)Game1.pixelZoom, false);
			this.scrollBarRunner = new Rectangle(this.scrollBar.bounds.X, this.upArrow.bounds.Y + this.upArrow.bounds.Height + Game1.pixelZoom, this.scrollBar.bounds.Width, this.height - Game1.tileSize - this.upArrow.bounds.Height - Game1.pixelZoom * 7);
			this.shippingBox = new ClickableComponent(new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height - Game1.tileSize * 4 + Game1.tileSize / 2 + Game1.pixelZoom), (Item)null);
			for (int i = 0; i < 4; i++)
			{
				this.shippedItemButtons.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4, this.yPositionOnScreen + Game1.tileSize / 4 + i * ((this.height - Game1.tileSize * 4) / 4), this.width - Game1.tileSize / 2, (this.height - Game1.tileSize * 4) / 4 + Game1.pixelZoom), string.Concat(i)));
			}
		}

		public bool HighlightItemToShip(Item i)
		{
			SObject @object = i as SObject;
			return @object != null && @object.canBeShipped();
		}

		public override void leftClickHeld(int x, int y)
		{
			base.leftClickHeld(x, y);
			bool flag = !this.scrolling;
			if (!flag)
			{
				int y2 = this.scrollBar.bounds.Y;
				this.scrollBar.bounds.Y = Math.Min(this.yPositionOnScreen + this.height - Game1.tileSize - Game1.pixelZoom * 3 - this.scrollBar.bounds.Height, Math.Max(y, this.yPositionOnScreen + this.upArrow.bounds.Height + Game1.pixelZoom * 5));
				this.currentItemIndex = Math.Min(Game1.getFarm().shippingBin.Count - 4, Math.Max(0, (int)((double)Game1.getFarm().shippingBin.Count * (double)((float)(y - this.scrollBarRunner.Y) / (float)this.scrollBarRunner.Height))));
				this.SetScrollBarToCurrentIndex();
				bool flag2 = y2 == this.scrollBar.bounds.Y;
				if (!flag2)
				{
					Game1.playSound("shiny4");
				}
			}
		}

		public override void releaseLeftClick(int x, int y)
		{
			base.releaseLeftClick(x, y);
			this.scrolling = false;
		}

		private void SetScrollBarToCurrentIndex()
		{
			bool flag = Game1.getFarm().shippingBin.Count <= 0;
			if (!flag)
			{
				this.scrollBar.bounds.Y = this.scrollBarRunner.Height / Math.Max(1, Game1.getFarm().shippingBin.Count - 4 + 1) * this.currentItemIndex + this.upArrow.bounds.Bottom + Game1.pixelZoom;
				bool flag2 = this.currentItemIndex != Game1.getFarm().shippingBin.Count - 4;
				if (!flag2)
				{
					this.scrollBar.bounds.Y = this.downArrow.bounds.Y - this.scrollBar.bounds.Height - Game1.pixelZoom;
				}
			}
		}

		public override void receiveScrollWheelAction(int direction)
		{
			base.receiveScrollWheelAction(direction);
			bool flag = direction > 0 && this.currentItemIndex > 0;
			if (flag)
			{
				this.UpArrowPressed();
				Game1.playSound("shiny4");
			}
			else
			{
				bool flag2 = direction >= 0 || this.currentItemIndex >= Math.Max(0, Game1.getFarm().shippingBin.Count - 4);
				if (!flag2)
				{
					this.DownArrowPressed();
					Game1.playSound("shiny4");
				}
			}
		}

		private void DownArrowPressed()
		{
			this.downArrow.scale = this.downArrow.baseScale;
			this.currentItemIndex++;
			this.SetScrollBarToCurrentIndex();
		}

		private void UpArrowPressed()
		{
			this.upArrow.scale = this.upArrow.baseScale;
			this.currentItemIndex--;
			this.SetScrollBarToCurrentIndex();
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			bool flag = Game1.activeClickableMenu == null;
			if (!flag)
			{
				bool flag2 = this.upperRightCloseButton.containsPoint(x, y) && this.readyToClose();
				if (flag2)
				{
					base.exitThisMenu(true);
				}
				Vector2 value = this.inventory.snapToClickableComponent(x, y);
				bool flag3 = this.downArrow.containsPoint(x, y) && this.currentItemIndex < Math.Max(0, Game1.getFarm().shippingBin.Count - 4);
				if (flag3)
				{
					this.DownArrowPressed();
					Game1.playSound("shwip");
				}
				else
				{
					bool flag4 = this.upArrow.containsPoint(x, y) && this.currentItemIndex > 0;
					if (flag4)
					{
						this.UpArrowPressed();
						Game1.playSound("shwip");
					}
					else
					{
						bool flag5 = this.scrollBar.containsPoint(x, y);
						if (flag5)
						{
							this.scrolling = true;
						}
						else
						{
							bool flag6 = !this.downArrow.containsPoint(x, y) && x > this.xPositionOnScreen + this.width && x < this.xPositionOnScreen + this.width + Game1.tileSize * 2 && y > this.yPositionOnScreen && y < this.yPositionOnScreen + this.height;
							if (flag6)
							{
								this.scrolling = true;
								this.leftClickHeld(x, y);
								this.releaseLeftClick(x, y);
							}
						}
					}
				}
				bool flag7 = this.shippingBox.containsPoint(x, y);
				if (flag7)
				{
					bool flag8 = this.heldItem != null;
					if (flag8)
					{
						Game1.getFarm().shippingBin.Add(this.heldItem);
						this.heldItem = null;
						Game1.playSound("Ship");
						return;
					}
				}
				this.currentItemIndex = Math.Max(0, Math.Min(Game1.getFarm().shippingBin.Count - 4, this.currentItemIndex));
				bool flag9 = this.heldItem == null;
				if (flag9)
				{
					Item item = this.inventory.leftClick(x, y, null, false);
					bool flag10 = item != null;
					if (flag10)
					{
						bool flag11 = Game1.oldKBState.IsKeyDown(Keys.LeftShift);
						if (flag11)
						{
							Game1.getFarm().shippingBin.Add(item);
							Game1.playSound("Ship");
							bool flag12 = this.inventory.getItemAt(x, y) == null;
							if (flag12)
							{
								this.animations.Add(new TemporaryAnimatedSprite(5, value + new Vector2(32f, 32f), Color.White, 8, false, 100f, 0, -1, -1f, -1, 0)
								{
									motion = new Vector2(0f, -0.5f)
								});
							}
						}
						else
						{
							this.heldItem = item;
						}
					}
				}
				else
				{
					this.heldItem = this.inventory.leftClick(x, y, this.heldItem, true);
				}
				for (int i = 0; i < this.shippedItemButtons.Count; i++)
				{
					bool flag13 = this.currentItemIndex + i >= Game1.getFarm().shippingBin.Count || !this.shippedItemButtons[i].containsPoint(x, y);
					if (!flag13)
					{
						int index = this.currentItemIndex + i;
						bool flag14 = Game1.getFarm().shippingBin[index] != null && this.TryToRetrieveItem(Game1.getFarm().shippingBin[index]);
						if (flag14)
						{
							Game1.getFarm().shippingBin.RemoveAt(index);
							Game1.playSound("Ship");
						}
						this.currentItemIndex = Math.Max(0, Math.Min(Game1.getFarm().shippingBin.Count - 4, this.currentItemIndex));
						return;
					}
				}
				bool flag15 = !this.readyToClose() || (x >= this.xPositionOnScreen - Game1.tileSize && y >= this.yPositionOnScreen - Game1.tileSize && x <= this.xPositionOnScreen + this.width + Game1.tileSize * 2 && y <= this.yPositionOnScreen + this.height + Game1.tileSize);
				if (!flag15)
				{
					base.exitThisMenu(true);
				}
			}
		}

		public override bool readyToClose()
		{
			bool flag = this.heldItem == null;
			return flag && this.animations.Count == 0;
		}

		public override void emergencyShutDown()
		{
			base.emergencyShutDown();
			bool flag = this.heldItem == null;
			if (!flag)
			{
				Game1.player.addItemToInventoryBool(this.heldItem, false);
				Game1.playSound("coin");
			}
		}

		private bool TryToRetrieveItem(Item item)
		{
			bool flag = this.heldItem == null;
			bool result;
			if (flag)
			{
				bool flag2 = Game1.oldKBState.IsKeyDown(Keys.LeftShift);
				if (flag2)
				{
					result = Game1.player.addItemToInventoryBool(item, false);
				}
				else
				{
					this.heldItem = item;
					result = true;
				}
			}
			else
			{
				bool flag3 = this.heldItem != null && Game1.oldKBState.IsKeyDown(Keys.LeftShift);
				if (flag3)
				{
					result = Game1.player.addItemToInventoryBool(item, false);
				}
				else
				{
					bool flag4 = !this.heldItem.Name.Equals(item.Name) || ((SObject)this.heldItem).quality != ((SObject)item).quality || this.heldItem.getStack() + item.getStack() >= this.heldItem.maximumStackSize();
					if (flag4)
					{
						result = false;
					}
					else
					{
						this.heldItem.addToStack(item.getStack());
						result = true;
					}
				}
			}
			return result;
		}

		public override void receiveRightClick(int x, int y, bool playSound = true)
		{
			bool flag = this.heldItem == null;
			if (flag)
			{
				Item item = this.inventory.rightClick(x, y, null, false);
				bool flag2 = item != null;
				if (flag2)
				{
					this.heldItem = item.getOne();
				}
			}
			else
			{
				this.heldItem = this.inventory.rightClick(x, y, this.heldItem, true);
			}
			for (int i = 0; i < this.shippedItemButtons.Count; i++)
			{
				bool flag3 = this.currentItemIndex + i >= Game1.getFarm().shippingBin.Count || !this.shippedItemButtons[i].containsPoint(x, y);
				if (!flag3)
				{
					int index = this.currentItemIndex + i;
					bool flag4 = Game1.getFarm().shippingBin[index] != null && this.TryToRetrieveItem(Game1.getFarm().shippingBin[index]);
					if (flag4)
					{
						Game1.getFarm().shippingBin.RemoveAt(index);
						Game1.playSound("Ship");
					}
					this.currentItemIndex = Math.Max(0, Math.Min(Game1.getFarm().shippingBin.Count - 4, this.currentItemIndex));
					break;
				}
			}
		}

		public override void performHoverAction(int x, int y)
		{
			base.performHoverAction(x, y);
			this.hoverText = "";
			this.hoveredItem = null;
			this.boldTitleText = "";
			this.upArrow.tryHover(x, y, 0.1f);
			this.downArrow.tryHover(x, y, 0.1f);
			this.scrollBar.tryHover(x, y, 0.1f);
			bool flag = this.scrolling;
			if (!flag)
			{
				for (int i = 0; i < this.shippedItemButtons.Count; i++)
				{
					bool flag2 = this.currentItemIndex + i < Game1.getFarm().shippingBin.Count && this.shippedItemButtons[i].containsPoint(x, y);
					if (flag2)
					{
						Item item = Game1.getFarm().shippingBin[this.currentItemIndex + i];
						this.hoverText = item.getDescription();
						this.boldTitleText = item.Name;
						this.hoveredItem = item;
						this.shippedItemButtons[i].scale = Math.Min(this.shippedItemButtons[i].scale + 0.03f, 1.1f);
					}
					else
					{
						this.shippedItemButtons[i].scale = Math.Max(1f, this.shippedItemButtons[i].scale - 0.03f);
					}
				}
				bool flag3 = this.heldItem != null;
				if (!flag3)
				{
					foreach (ClickableComponent current in this.inventory.inventory)
					{
						bool flag4 = current.containsPoint(x, y);
						if (flag4)
						{
							Item itemFromClickableComponent = this.inventory.getItemFromClickableComponent(current);
							bool flag5 = itemFromClickableComponent != null && this.HighlightItemToShip(itemFromClickableComponent);
							if (flag5)
							{
								this.hoverText = itemFromClickableComponent.Name + " x " + itemFromClickableComponent.getStack();
							}
						}
					}
				}
			}
		}

		public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
		{
			this.xPositionOnScreen = Game1.viewport.Width / 2 - (800 + IClickableMenu.borderWidth * 2) / 2;
			this.yPositionOnScreen = Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2;
			this.width = 800 + IClickableMenu.borderWidth * 2;
			this.height = 600 + IClickableMenu.borderWidth * 2;
			base.initializeUpperRightCloseButton();
			bool flag = Game1.viewport.Width < 1500;
			if (flag)
			{
				this.xPositionOnScreen = Game1.tileSize / 2;
			}
			Game1.player.forceCanMove();
			this.inventory = new InventoryMenu(this.xPositionOnScreen + Game1.tileSize / 2 + (Game1.tileSize / 3 + Game1.tileSize / 11), this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + Game1.tileSize * 5 + Game1.pixelZoom * 10, false, null, new InventoryMenu.highlightThisItem(this.HighlightItemToShip), -1, 3, 0, 0, true)
			{
				showGrayedOutSlots = true
			};
			this.upArrow = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + Game1.tileSize / 4, this.yPositionOnScreen + Game1.tileSize / 4, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), (float)Game1.pixelZoom, false);
			this.downArrow = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + Game1.tileSize / 4, this.yPositionOnScreen + this.height - Game1.tileSize, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), (float)Game1.pixelZoom, false);
			this.scrollBar = new ClickableTextureComponent(new Rectangle(this.upArrow.bounds.X + Game1.pixelZoom * 3, this.upArrow.bounds.Y + this.upArrow.bounds.Height + Game1.pixelZoom, 6 * Game1.pixelZoom, 10 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), (float)Game1.pixelZoom, false);
			this.scrollBarRunner = new Rectangle(this.scrollBar.bounds.X, this.upArrow.bounds.Y + this.upArrow.bounds.Height + Game1.pixelZoom, this.scrollBar.bounds.Width, this.height - Game1.tileSize - this.upArrow.bounds.Height - Game1.pixelZoom * 7);
			this.shippingBox = new ClickableComponent(new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height - Game1.tileSize * 4 + Game1.tileSize / 2 + Game1.pixelZoom), (Item)null);
			this.shippedItemButtons.Clear();
			for (int i = 0; i < 4; i++)
			{
				this.shippedItemButtons.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4, this.yPositionOnScreen + Game1.tileSize / 4 + i * ((this.height - Game1.tileSize * 4) / 4), this.width - Game1.tileSize / 2, (this.height - Game1.tileSize * 4) / 4 + Game1.pixelZoom), string.Concat(i)));
			}
		}

		public override void draw(SpriteBatch b)
		{
			bool flag = !Game1.options.showMenuBackground;
			if (flag)
			{
				b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
			}
			IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), this.xPositionOnScreen + Game1.tileSize / 2, this.yPositionOnScreen + this.height - Game1.tileSize * 4 + Game1.pixelZoom * 10, this.inventory.width + Game1.pixelZoom * 14, this.height - Game1.tileSize * 7 + Game1.pixelZoom * 5, Color.White, (float)Game1.pixelZoom, true);
			IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height - Game1.tileSize * 4 + Game1.tileSize / 2 + Game1.pixelZoom, Color.White, (float)Game1.pixelZoom, true);
			for (int i = 0; i < this.shippedItemButtons.Count; i++)
			{
				bool flag2 = this.currentItemIndex + i >= Game1.getFarm().shippingBin.Count || !Game1.getFarm().shippingBin.Any<Item>();
				if (!flag2)
				{
					IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), this.shippedItemButtons[i].bounds.X, this.shippedItemButtons[i].bounds.Y, this.shippedItemButtons[i].bounds.Width, this.shippedItemButtons[i].bounds.Height, (!this.shippedItemButtons[i].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) || this.scrolling) ? Color.White : Color.Wheat, (float)Game1.pixelZoom, false);
					b.Draw(Game1.mouseCursors, new Vector2((float)(this.shippedItemButtons[i].bounds.X + Game1.tileSize / 2 - Game1.pixelZoom * 3), (float)(this.shippedItemButtons[i].bounds.Y + Game1.pixelZoom * 6 - Game1.pixelZoom)), new Rectangle?(new Rectangle(296, 363, 18, 18)), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1f);
					Game1.getFarm().shippingBin[this.currentItemIndex + i].drawInMenu(b, new Vector2((float)(this.shippedItemButtons[i].bounds.X + Game1.tileSize / 2 - Game1.pixelZoom * 2), (float)(this.shippedItemButtons[i].bounds.Y + Game1.pixelZoom * 6)), 1f);
					SpriteText.drawString(b, Game1.getFarm().shippingBin[this.currentItemIndex + i].Name, this.shippedItemButtons[i].bounds.X + Game1.tileSize * 3 / 2 + Game1.pixelZoom * 2, this.shippedItemButtons[i].bounds.Y + Game1.pixelZoom * 7, 999999, -1, 999999, 1f, 0.88f, false, -1, "", -1);
				}
			}
			bool flag3 = Game1.getFarm().shippingBin.Count == 0;
			if (flag3)
			{
				SpriteText.drawString(b, "Empty", this.xPositionOnScreen + this.width / 2 - SpriteText.getWidthOfString("Empty") / 2, this.yPositionOnScreen + this.height / 2 - Game1.tileSize * 2, 999999, -1, 999999, 1f, 0.88f, false, -1, "", -1);
			}
			this.inventory.draw(b);
			for (int j = this.animations.Count - 1; j >= 0; j--)
			{
				bool flag4 = this.animations[j].update(Game1.currentGameTime);
				if (flag4)
				{
					this.animations.RemoveAt(j);
				}
				else
				{
					this.animations[j].draw(b, true, 0, 0);
				}
			}
			this.upArrow.draw(b);
			this.downArrow.draw(b);
			bool flag5 = Game1.getFarm().shippingBin.Count > 4;
			if (flag5)
			{
				IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), this.scrollBarRunner.X, this.scrollBarRunner.Y, this.scrollBarRunner.Width, this.scrollBarRunner.Height, Color.White, (float)Game1.pixelZoom, true);
				this.scrollBar.draw(b);
			}
			bool flag6 = !this.hoverText.Equals("");
			if (flag6)
			{
				IClickableMenu.drawToolTip(b, this.hoverText, this.boldTitleText, this.hoveredItem, this.heldItem != null, -1, 0, -1, -1, null, -1);
			}
			Item expr_575 = this.heldItem;
			if (expr_575 != null)
			{
				expr_575.drawInMenu(b, new Vector2((float)(Game1.getOldMouseX() + 8), (float)(Game1.getOldMouseY() + 8)), 1f);
			}
			base.draw(b);
			base.drawMouse(b);
		}
	}
}
