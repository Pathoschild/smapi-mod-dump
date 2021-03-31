/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/GiftWrapper
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GiftWrapper
{
	public class GiftWrapMenu : ItemGrabMenu
	{
		public IModHelper Helper => ModEntry.Instance.Helper;
		public ITranslationHelper i18n => ModEntry.Instance.i18n;

		/// <summary> Custom sprite assets for menu elements </summary>
		public Texture2D Texture;
		/// <summary> Clickable container to display items placed by the user for turning into wrapped gifts </summary>
		public ClickableTextureComponent ItemSlot;
		/// <summary> Contextual clickable button to confirm the gift wrap action, revealed when there are items to be wrapped </summary>
		public ClickableTextureComponent WrapButton;
		/// <summary>
		/// Tile position of the placed gift wrap item used to open this menu at the current game location,
		/// passed to ModEntry.Instance.PackItem() to remove from the world objects list
		/// </summary>
		public Vector2 GiftWrapPosition;
		/// <summary> Item instance currently in the ItemSlot container to be wrapped </summary>
		public Item ItemToWrap;
		/// <summary> Whether to have the contextual clickable gift wrap confirm button be visible and interactible </summary>
		public bool ShowWrapButton { get => ItemToWrap != null; }

		/// <summary> Current wrapped gift animation timer </summary>
		private int _animTimer;
		/// <summary> Current wrapped gift animation frame </summary>
		private int _animFrame;
		/// <summary> Time between animation frames </summary>
		private const int AnimFrameTime = 100;
		/// <summary> Number of frames in wrapped gift button animation </summary>
		private const int AnimFrames = 4;
		/// <summary> Value at which animTimer will reset to 0 </summary>
		private const int AnimTimerLimit = AnimFrameTime * AnimFrames;
		/// <summary> Reflects InventoryMenu item shake </summary>
		private readonly IReflectedField<Dictionary<int, double>> _iconShakeTimerField;

		private static readonly Rectangle BackgroundSource = new Rectangle(0, 0, 128, 80);
		private static readonly Rectangle DecorationSource = new Rectangle(0, BackgroundSource.Height, 96, 64);
		private static readonly Rectangle ItemSlotSource = new Rectangle(BackgroundSource.X + BackgroundSource.Width - 18, BackgroundSource.Y + BackgroundSource.Height, 18, 18);
		private static readonly Rectangle WrapButtonSource = new Rectangle(548, 262, 18, 20);
		private Rectangle _backgroundArea;
		private Rectangle _decorationArea;
		private new const int borderWidth = 4;
		private readonly int _borderScaled;
		private readonly int _defaultClickable = -1;
		private readonly int _inventoryExtraWidth = 4 * Game1.pixelZoom;

		public GiftWrapMenu(Vector2 position) : base(inventory: null, context: null)
		{
			Game1.playSound("scissors");
			Game1.freezeControls = true;

			// Custom fields
			GiftWrapPosition = position;
			Texture = Game1.content.Load<Texture2D>(ModEntry.GameContentTexturePath);

			// Base fields
			this.initializeUpperRightCloseButton();
			trashCan = null;
			_iconShakeTimerField = Helper.Reflection.GetField<Dictionary<int, double>>(inventory, "_iconShakeTimer");

			Point centre = Game1.graphics.GraphicsDevice.Viewport.Bounds.Center;
			if (Context.IsSplitScreen)
			{
				// Centre the menu in splitscreen
				centre.X = centre.X / 3 * 2;
			}

			_borderScaled = borderWidth * Game1.pixelZoom;
			int yOffset;
			int ID = 1000;

			// Widen inventory to allow more space in the text area above
			inventory.width += _inventoryExtraWidth;
			inventory.xPositionOnScreen -= _inventoryExtraWidth / 2;

			// Background panel
			yOffset = -32 * Game1.pixelZoom;
			_backgroundArea = new Rectangle(
				inventory.xPositionOnScreen - (_borderScaled / 2),
				centre.Y + yOffset - (BackgroundSource.Height / 2 * Game1.pixelZoom),
				BackgroundSource.Width * Game1.pixelZoom,
				BackgroundSource.Height * Game1.pixelZoom);

			yOffset = -28 * Game1.pixelZoom;
			_decorationArea = new Rectangle(
				_backgroundArea.X + ((BackgroundSource.Width - DecorationSource.Width) / 2 * Game1.pixelZoom),
				centre.Y + yOffset - (DecorationSource.Height / 2 * Game1.pixelZoom),
				DecorationSource.Width * Game1.pixelZoom,
				DecorationSource.Height * Game1.pixelZoom);

			inventory.yPositionOnScreen = _backgroundArea.Y + _backgroundArea.Height + (borderWidth * 2 * Game1.pixelZoom);

			// Item slot clickable
			yOffset = -24 * Game1.pixelZoom;
			ItemSlot = new ClickableTextureComponent(
				bounds: new Rectangle(
					_backgroundArea.X + ((BackgroundSource.Width - ItemSlotSource.Width) / 2 * Game1.pixelZoom),
					_backgroundArea.Y + (_backgroundArea.Height / 2) + yOffset,
					ItemSlotSource.Width * Game1.pixelZoom,
					ItemSlotSource.Height * Game1.pixelZoom),
				texture: Texture,
				sourceRect: ItemSlotSource,
				scale: Game1.pixelZoom, drawShadow: false)
			{
				myID = ++ID
			};

			// Wrap button clickable
			yOffset = 16 * Game1.pixelZoom;
			Texture2D junimoTexture = Game1.content.Load<Texture2D>(@"LooseSprites/JunimoNote");
			WrapButton = new ClickableTextureComponent(
				bounds: new Rectangle(
					ItemSlot.bounds.X,
					_backgroundArea.Y + (_backgroundArea.Height / 2) + yOffset,
					WrapButtonSource.Width * Game1.pixelZoom,
					WrapButtonSource.Height * Game1.pixelZoom),
				texture: junimoTexture,
				sourceRect: WrapButtonSource,
				scale: Game1.pixelZoom, drawShadow: false)
			{
				myID = ++ID
			};

			// Clickable navigation
			_defaultClickable = ItemSlot.myID;
			this.populateClickableComponentList();

			ModEntry.Instance.Helper.Events.GameLoop.UpdateTicked += this.Event_UnfreezeControls;
		}

		/// <summary>
		/// Prevents having the click-down that opens the menu from also interacting with the menu on click-released
		/// </summary>
		private void Event_UnfreezeControls(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
		{
			ModEntry.Instance.Helper.Events.GameLoop.UpdateTicked -= this.Event_UnfreezeControls;
			Game1.freezeControls = false;
			if (Game1.options.SnappyMenus)
			{
				this.snapToDefaultClickableComponent();
			}
		}

		protected override void cleanupBeforeExit()
		{
			if (ItemToWrap != null)
			{
				// Return items in item slot to player when closing
				Game1.createItemDebris(item: ItemToWrap, origin: Game1.player.Position, direction: -1);
			}
			base.cleanupBeforeExit();
		}

		public override void emergencyShutDown()
		{
			this.exitFunction();
			base.emergencyShutDown();
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			if (ItemSlot.containsPoint(x, y) && ItemToWrap != null)
			{
				if (inventory.tryToAddItem(toPlace: ItemToWrap, sound: playSound ? "coin" : null) == null)
				{
					// Take all from wrapping
					ItemToWrap = null;
				}
				else
				{
					// Inventory couldn't take all
					Game1.playSound("cancel");
				}
			}
			else if (WrapButton.containsPoint(x, y) && ShowWrapButton && ItemToWrap != null)
			{
				Item wrappedGift = ModEntry.Instance.GetWrappedGift(modData: null);
				ModEntry.Instance.PackItem(ref wrappedGift, ItemToWrap, GiftWrapPosition, showMessage: true);
				if (wrappedGift != null)
				{
					// Convert wrapping to gift and close menu, giving the player the wrapped gift
					ItemToWrap = null;
					Game1.player.addItemToInventory(wrappedGift);
					Game1.playSound("discoverMineral");
					this.exitThisMenuNoSound();
				}
				else
				{
					// Wrapping couldn't be gifted
					Game1.playSound("cancel");
				}
			}
			else if (inventory.getInventoryPositionOfClick(x, y) is int index && index >= 0 && inventory.actualInventory[index] != null)
			{
				if (ItemToWrap != null && ItemToWrap.canStackWith(inventory.actualInventory[index]))
				{
					// Try add all to wrapping
					int maximumToSend = Math.Min(inventory.actualInventory[index].Stack, ItemToWrap.maximumStackSize() - ItemToWrap.Stack);
					ItemToWrap.Stack += maximumToSend;
					inventory.actualInventory[index].Stack -= maximumToSend;
					if (inventory.actualInventory[index].Stack < 1)
						inventory.actualInventory[index] = null;
					Game1.playSound("coin");
				}
				else
				{
					// Add all to wrapping
					Item tempItem = inventory.actualInventory[index];
					inventory.actualInventory[index] = ItemToWrap;
					ItemToWrap = tempItem;
					Game1.playSound("coin");
				}
			}
			else
			{
				// Close menu
				if (upperRightCloseButton.containsPoint(x, y))
					this.exitThisMenu();
			}
		}
		
		public override void receiveRightClick(int x, int y, bool playSound = true)
		{
			if (ItemSlot.containsPoint(x, y) && ItemToWrap != null)
			{
				if (inventory.tryToAddItem(toPlace: ItemToWrap.getOne()) == null)
				{
					// Take one from wrapping
					if (ItemToWrap.maximumStackSize() <= 1 || --ItemToWrap.Stack < 1)
						ItemToWrap = null;
				}
				else
				{
					// Inventory couldn't take one
					Game1.playSound("cancel");
				}
			}
			else if (inventory.getInventoryPositionOfClick(x, y) is int index && index >= 0 && inventory.actualInventory[index] != null)
			{
				bool movedOne = false;
				if (ItemToWrap != null)
				{
					// Add one to wrapping
					if (ItemToWrap.canStackWith(inventory.actualInventory[index]))
					{
						++ItemToWrap.Stack;
						movedOne = true;
					}
					// Take all of wrapping and add one to wrap
					else if (inventory.tryToAddItem(toPlace: ItemToWrap) == null)
					{
						ItemToWrap = inventory.actualInventory[index].getOne();
						movedOne = true;
					}
				}
				else
				{
					// Add one to wrapping
					ItemToWrap = inventory.actualInventory[index].getOne();
					movedOne = true;
				}

				if (movedOne)
				{
					// Take one from inventory
					if (inventory.actualInventory[index].maximumStackSize() <= 1 || --inventory.actualInventory[index].Stack < 1)
						inventory.actualInventory[index] = null;
					Game1.playSound("coin");
				}
				else
				{
					// None were moved
					Game1.playSound("cancel");
				}
			}
		}

		public override void performHoverAction(int x, int y)
		{
			hoverText = "";
			hoveredItem = null;
			if (ItemSlot.containsPoint(x, y) && ItemToWrap != null) 
			{
				// Hover item slot
				hoveredItem = ItemToWrap;
			}
			else if (inventory.getInventoryPositionOfClick(x, y) is int index && index >= 0 && inventory.actualInventory[index] is Item item && item != null)
			{
				// Hover inventory item
				hoveredItem = item;
			}
		}

		public override void receiveKeyPress(Keys key)
		{
			bool isExitKey = key == Keys.Escape
				|| Game1.options.doesInputListContain(Game1.options.menuButton, key)
				|| Game1.options.doesInputListContain(Game1.options.journalButton, key);
			if (isExitKey)
			{
				this.exitThisMenu();
			}
			else if (Game1.options.SnappyMenus)
			{
				// Gamepad navigation
				int inventoryWidth = this.GetColumnCount();
				int current = currentlySnappedComponent != null ? currentlySnappedComponent.myID : -1;
				int snapTo = -1;
				if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
				{
					// Left
					if (current < inventory.inventory.Count && current % inventoryWidth == 0)
					{
						// Inventory =|
						snapTo = current;
					}
				}
				else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
				{
					// Right
					if (current < inventory.inventory.Count && current % inventoryWidth == inventoryWidth - 1)
					{
						// Inventory =|
						snapTo = current;
					}
				}
				else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
				{
					// Up
					if (current == WrapButton.myID)
					{
						// WrapButton => ItemSlot
						snapTo = ItemSlot.myID;
					}
					else if (current >= 0 && current < inventory.inventory.Count)
					{
						if (current < inventoryWidth)
						{
							// Inventory => WrapButton/ItemSlot
							snapTo = ShowWrapButton ? WrapButton.myID : ItemSlot.myID;
						}
						else
						{
							// Inventory => Inventory
							snapTo = current - inventoryWidth;
						}
					}
				}
				else if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
				{
					// Down
					if (current == ItemSlot.myID)
					{
						// ItemSlot => WrapButton/Inventory
						snapTo = ShowWrapButton ? WrapButton.myID : 0;
					}
					else if (current == WrapButton.myID)
					{
						// WrapButton => Inventory
						snapTo = 0;
					}
				}
				if (snapTo != -1)
				{
					this.setCurrentlySnappedComponentTo(snapTo);
					return;
				}
			}

			base.receiveKeyPress(key);
		}

		public override void receiveGamePadButton(Buttons b)
		{
			// Contextual navigation
			int current = currentlySnappedComponent != null ? currentlySnappedComponent.myID : -1;
			int snapTo = -1;
			if (b == Buttons.LeftShoulder)
			{
				// Left
				if (current == ItemSlot.myID)
					// ItemSlot => Inventory
					snapTo = 0;
				else if (current == WrapButton.myID)
					// WrapButton => ItemSlot
					snapTo = ItemSlot.myID;
				else if (current < inventory.inventory.Count)
					// Inventory => WrapButton/ItemSlot
					snapTo = ShowWrapButton ? WrapButton.myID : ItemSlot.myID;
				else
					// ??? => Default
					snapTo = _defaultClickable;
			}
			if (b == Buttons.RightShoulder)
			{
				// Right
				if (current == ItemSlot.myID)
					// ItemSlot => WrapButton/Inventory
					snapTo = ShowWrapButton ? WrapButton.myID : 0;
				else if (current == WrapButton.myID)
					// WrapButton => Inventory
					snapTo = 0;
				else if (current > inventory.inventory.Count)
					// Inventory => ItemSlot
					snapTo = ItemSlot.myID;
				else
					// ??? => Default
					snapTo = _defaultClickable;
			}
			this.setCurrentlySnappedComponentTo(snapTo);
		}

		public override void snapToDefaultClickableComponent()
		{
			if (_defaultClickable == -1)
				return;
			this.setCurrentlySnappedComponentTo(_defaultClickable);
		}

		public override void setCurrentlySnappedComponentTo(int id)
		{
			if (id == -1 || this.getComponentWithID(id) == null)
				return;

			this.currentlySnappedComponent = this.getComponentWithID(id);
			this.snapCursorToCurrentSnappedComponent();
		}

		public override void update(GameTime time)
		{
			// WrapButton animation loop
			_animTimer += time.ElapsedGameTime.Milliseconds;
			if (_animTimer >= AnimTimerLimit)
				_animTimer = 0;
			_animFrame = (int)((float)_animTimer / AnimTimerLimit * AnimFrames);

			base.update(time);
		}

		public override void draw(SpriteBatch b)
		{
			// Blackout
			b.Draw(
				texture: Game1.fadeToBlackRect,
				destinationRectangle: Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea(),
				sourceRectangle: null,
				Color.Black * 0.3f);

			// Inventory panel
			this.DrawInventory(b);

			// Background panel
			b.Draw(Texture, destinationRectangle: _backgroundArea, sourceRectangle: BackgroundSource,
				Color.White, rotation: 0f, origin: Vector2.Zero, SpriteEffects.None, layerDepth: 1f);
			b.Draw(Texture, destinationRectangle: _decorationArea, sourceRectangle: DecorationSource,
				Color.White, rotation: 0f, origin: Vector2.Zero, SpriteEffects.None, layerDepth: 1f);
			this.DrawPinkBorder(b, area: _backgroundArea, drawBorderOutside: true, drawFillColour: false);

			// Info panel
			Rectangle textPanelArea = new Rectangle(
				_backgroundArea.X + _backgroundArea.Width + _borderScaled,
				_backgroundArea.Y,
				inventory.width - _backgroundArea.Width + (1 * Game1.pixelZoom),
				_backgroundArea.Height);
			this.DrawPinkBorder(b, area: textPanelArea, drawBorderOutside: true, drawFillColour: true);

			Vector2 margin = new Vector2(2, 4) * Game1.pixelZoom;
			string text = i18n.Get("menu.infopanel.body");
			// Give a little extra leeway with non-English locales to fit them into the text area
			text = Game1.parseText(text, Game1.smallFont, width: textPanelArea.Width
				- (LocalizedContentManager.CurrentLanguageCode.ToString() == "en" ? (int)margin.X : 0));
			if (Game1.smallFont.MeasureString(text).Y > textPanelArea.Height)
			{
				// Remove the last line if text overflows
				IEnumerable<string> split = text.Split('.').Where(str => !string.IsNullOrWhiteSpace(str));
				text = split.Aggregate("", (total, str) => str != split.Last() ? total + str + "." : total);
			}
			Utility.drawTextWithShadow(b, text, Game1.smallFont, position: new Vector2(textPanelArea.X + (int)margin.X, textPanelArea.Y + (int)margin.Y), Game1.textColor);

			// Item clickables:
			// Item slot
			ItemSlot.draw(b);
			// Item inside the slot
			ItemToWrap?.drawInMenu(b, location: new Vector2(ItemSlot.bounds.X, ItemSlot.bounds.Y), scaleSize: 1f);
			if (ShowWrapButton)
			{
				// Wrap button
				b.Draw(
					WrapButton.texture,
					destinationRectangle: WrapButton.bounds,
					sourceRectangle: new Rectangle(
						WrapButton.sourceRect.X + (_animFrame * WrapButton.sourceRect.Width),
						WrapButton.sourceRect.Y,
						WrapButton.sourceRect.Width,
						WrapButton.sourceRect.Height),
					Color.White, rotation: 0f, origin: Vector2.Zero, SpriteEffects.None, layerDepth: 1f);
			}

			// Tooltips
			if (hoveredItem != null)
				IClickableMenu.drawToolTip(b, hoveredItem.getDescription(), hoveredItem.DisplayName, hoveredItem, heldItem != null);

			// Cursors
			Game1.mouseCursorTransparency = 1f;
			this.drawMouse(b);
		}

		/// <summary>
		/// Mostly a copy of InventoryMenu.draw(SpriteBatch b, int red, int blue, int green),
		/// though items considered unable to be cooked will be greyed out.
		/// </summary>
		private void DrawInventory(SpriteBatch b)
		{
			// Background card
			Vector4 margin = new Vector4(2, 4, 5, 4) * Game1.pixelZoom;
			Rectangle area = new Rectangle(inventory.xPositionOnScreen - (int)margin.X, inventory.yPositionOnScreen - (int)margin.Y, inventory.width + (int)margin.Z, inventory.height + (int)margin.W);
			this.DrawPinkBorder(b, area, drawBorderOutside: true, drawFillColour: true);

			// Inventory item shakes
			Dictionary<int, double> iconShakeTimer = _iconShakeTimerField.GetValue();
			for (int key = 0; key < inventory.inventory.Count; ++key)
			{
				if (iconShakeTimer.ContainsKey(key) && Game1.currentGameTime.TotalGameTime.TotalSeconds >= iconShakeTimer[key])
					iconShakeTimer.Remove(key);
			}

			// Actual inventory
			for (int i = 0; i < inventory.capacity; ++i)
			{
				Vector2 position = new Vector2(
					inventory.xPositionOnScreen
						+ (_inventoryExtraWidth / 2)
						+ (i % (inventory.capacity / inventory.rows) * 64)
						+ (inventory.horizontalGap * (i % (inventory.capacity / inventory.rows))),
					inventory.yPositionOnScreen
						+ (i / (inventory.capacity / inventory.rows) * (64 + inventory.verticalGap))
						+ (((i / (inventory.capacity / inventory.rows)) - 1) * 4)
						- (i >= inventory.capacity / inventory.rows
							|| !inventory.playerInventory || inventory.verticalGap != 0 ? 0 : 12));

				// Item slot frames
				b.Draw(
					texture: Game1.menuTexture,
					position,
					sourceRectangle: Game1.getSourceRectForStandardTileSheet(tileSheet: Game1.menuTexture, tilePosition: 10),
					Color.PeachPuff, rotation: 0.0f, origin: Vector2.Zero, scale: 1f, SpriteEffects.None, layerDepth: 0.5f);
				b.Draw(
					texture: Game1.menuTexture,
					position,
					sourceRectangle: Game1.getSourceRectForStandardTileSheet(tileSheet: Game1.menuTexture, tilePosition: 10),
					Color.Orchid * 0.75f, rotation: 0.0f, origin: Vector2.Zero, scale: 1f, SpriteEffects.None, layerDepth: 0.5f);

				// Greyed-out item slots
				if ((inventory.playerInventory || inventory.showGrayedOutSlots) && i >= Game1.player.maxItems.Value)
					b.Draw(
						texture: Game1.menuTexture,
						position,
						sourceRectangle: Game1.getSourceRectForStandardTileSheet(tileSheet: Game1.menuTexture, tilePosition: 57),
						Color.White * 0.5f, rotation: 0f, origin: Vector2.Zero, scale: 1f, SpriteEffects.None, layerDepth: 0.5f);

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
				Vector2 textOffset = Game1.tinyFont.MeasureString(text);
				b.DrawString(
					Game1.tinyFont,
					text,
					position + new Vector2(32f - (textOffset.X / 2f), -textOffset.Y),
					i == Game1.player.CurrentToolIndex ? Color.Red : Color.DimGray);
			}
			for (int i = 0; i < inventory.capacity; ++i)
			{
				// Items
				if (inventory.actualInventory.Count <= i || inventory.actualInventory.ElementAt(i) == null)
					continue;

				Vector2 location = new Vector2(
					inventory.xPositionOnScreen
					 + (i % (inventory.capacity / inventory.rows) * 64)
					 + (inventory.horizontalGap * (i % (inventory.capacity / inventory.rows))),
					inventory.yPositionOnScreen
						+ (i / (inventory.capacity / inventory.rows) * (64 + inventory.verticalGap))
						+ (((i / (inventory.capacity / inventory.rows)) - 1) * 4)
						- (i >= inventory.capacity / inventory.rows
						   || !inventory.playerInventory || inventory.verticalGap != 0 ? 0 : 12));

				bool drawShadow = inventory.highlightMethod(inventory.actualInventory[i]);
				if (iconShakeTimer.ContainsKey(i))
					location += 1f * new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2));
				inventory.actualInventory[i].drawInMenu(b,
					location,
					scaleSize: inventory.inventory.Count > i ? inventory.inventory[i].scale : 1f,
					transparency: !inventory.highlightMethod(inventory.actualInventory[i]) ? 0.25f : 1f,
					layerDepth: 0.865f,
					StackDrawType.Draw,
					Color.White,
					drawShadow);
			}
		}

		private void DrawPinkBorder(SpriteBatch b, Rectangle area, bool drawBorderOutside, bool drawFillColour)
		{
			Point point = new Point(100, 80);
			Point cornerSize = new Point(5, 5);

			Color colour = Color.White;
			float rotation = 0f;
			Vector2 origin = Vector2.Zero;
			float scale = Game1.pixelZoom;
			SpriteEffects effects = SpriteEffects.None;
			float layerDepth = 1f;

			Rectangle source;
			Rectangle scaled;

			if (drawBorderOutside)
			{
				area.X -= _borderScaled;
				area.Y -= _borderScaled;
				area.Width += _borderScaled * 2;
				area.Height += _borderScaled * 2;
			}

			if (drawFillColour)
			{
				// Fill colour
				Rectangle fillArea = new Rectangle(area.X + _borderScaled, area.Y + _borderScaled, area.Width - (_borderScaled * 2), area.Height - (_borderScaled * 2));
				source = new Rectangle(380, 437, 1, 8); // Sample the date field background from the HUD clock in cursors
				b.Draw(
					texture: Game1.mouseCursors,
					destinationRectangle: fillArea,
					sourceRectangle: source,
					Color.White, rotation, origin, effects, layerDepth);
				b.Draw(
					 texture: Game1.mouseCursors,
					 destinationRectangle: fillArea,
					 sourceRectangle: source,
					 Color.Plum * 0.65f, rotation, origin, effects, layerDepth);
			}

			// Sides:
			// Top
			source = new Rectangle(point.X + borderWidth + 1, point.Y, 1, borderWidth + 1);
			scaled = new Rectangle(0, 0, source.Width * Game1.pixelZoom, source.Height * Game1.pixelZoom);
			b.Draw(
				Texture,
				destinationRectangle: new Rectangle(area.X + (cornerSize.Y * Game1.pixelZoom), area.Y, area.Width - (cornerSize.X * Game1.pixelZoom * 2), scaled.Height),
				sourceRectangle: source,
				colour, rotation, origin, effects, layerDepth);
			// Bottom
			source = new Rectangle(point.X + borderWidth + 1, point.Y, 1, borderWidth);
			scaled = new Rectangle(0, 0, source.Width * Game1.pixelZoom, source.Height * Game1.pixelZoom);
			b.Draw(
				Texture,
				destinationRectangle: new Rectangle(area.X + (cornerSize.Y * Game1.pixelZoom), area.Y + area.Height - scaled.Height, area.Width - (cornerSize.X * Game1.pixelZoom * 2), scaled.Height),
				sourceRectangle: source,
				colour, rotation, origin, effects, layerDepth);
			// Left
			source = new Rectangle(point.X, point.Y + borderWidth, borderWidth, 1);
			scaled = new Rectangle(0, 0, source.Width * Game1.pixelZoom, source.Height * Game1.pixelZoom);
			b.Draw(
				Texture,
				destinationRectangle: new Rectangle(area.X, area.Y + (cornerSize.Y * Game1.pixelZoom), scaled.Width, area.Height - (cornerSize.Y * Game1.pixelZoom * 2)),
				sourceRectangle: source,
				colour, rotation, origin, effects, layerDepth);
			// Right
			source = new Rectangle(point.X + source.Width + 1, point.Y + borderWidth, borderWidth + 1, 1);
			scaled = new Rectangle(0, 0, source.Width * Game1.pixelZoom, source.Height * Game1.pixelZoom);
			b.Draw(
				Texture,
				destinationRectangle: new Rectangle(area.X + area.Width - scaled.Width, area.Y + (cornerSize.Y * Game1.pixelZoom), scaled.Width, area.Height - (cornerSize.Y * Game1.pixelZoom * 2)),
				sourceRectangle: source,
				colour, rotation, origin, effects, layerDepth);

			// Corners:
			source = new Rectangle(point.X, point.Y, cornerSize.X, cornerSize.Y);
			scaled = new Rectangle(0, 0, source.Width * Game1.pixelZoom, source.Height * Game1.pixelZoom);
			// Top-left
			b.Draw(
				Texture,
				position: new Vector2(area.X, area.Y),
				sourceRectangle: source,
				colour, rotation, origin, scale, effects, layerDepth);
			// Bottom-left
			b.Draw(
				Texture,
				position: new Vector2(area.X, area.Y + area.Height - scaled.Height),
				sourceRectangle: new Rectangle(source.X, source.Y + source.Height, source.Width, source.Height),
				colour, rotation, origin, scale, effects, layerDepth);
			// Top-right
			b.Draw(
				Texture,
				position: new Vector2(area.X + area.Width - scaled.Width, area.Y),
				sourceRectangle: new Rectangle(source.X + source.Width, source.Y, source.Width, source.Height),
				colour, rotation, origin, scale, effects, layerDepth);
			// Bottom-right
			b.Draw(
				Texture,
				position: new Vector2(area.X + area.Width - scaled.Width, area.Y + area.Height - scaled.Height),
				sourceRectangle: new Rectangle(source.X + source.Width, source.Y + source.Height, source.Width, source.Height),
				colour, rotation, origin, scale, effects, layerDepth);
		}
	}
}
