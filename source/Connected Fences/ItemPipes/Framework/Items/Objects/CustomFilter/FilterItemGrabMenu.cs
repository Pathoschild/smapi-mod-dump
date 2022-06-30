/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using ItemPipes.Framework.Util;
using ItemPipes.Framework.Nodes;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using Microsoft.Xna.Framework.Input;
using Object = StardewValley.Object;


namespace ItemPipes.Framework.Items.CustomFilter
{
    public class FilterItemGrabMenu : MenuWithInventory
	{
		public delegate void behaviorOnItemSelect(Item item, Farmer who);

		public class TransferredItemSprite
		{
			public Item item;

			public Vector2 position;

			public float age;

			public float alpha = 1f;

			public TransferredItemSprite(Item transferred_item, int start_x, int start_y)
			{
				this.item = transferred_item;
				this.position.X = start_x;
				this.position.Y = start_y;
			}

			public bool Update(GameTime time)
			{
				float life_time = 0.15f;
				this.position.Y -= (float)time.ElapsedGameTime.TotalSeconds * 128f;
				this.age += (float)time.ElapsedGameTime.TotalSeconds;
				this.alpha = 1f - this.age / life_time;
				if (this.age >= life_time)
				{
					return true;
				}
				return false;
			}

			public void Draw(SpriteBatch b)
			{
				this.item.drawInMenu(b, this.position, 1f, this.alpha, 0.9f, StackDrawType.Hide, Color.White, drawShadow: false);
			}
		}

		public const int region_organizationButtons = 15923;

		public const int region_itemsToGrabMenuModifier = 53910;

		public const int region_fillStacksButton = 12952;

		public const int region_organizeButton = 106;

		public const int region_colorPickToggle = 27346;

		public const int region_specialButton = 12485;

		public const int region_lastShippedHolder = 12598;

		public const int source_none = 0;

		public const int source_chest = 1;

		public const int source_gift = 2;

		public const int source_fishingChest = 3;

		public const int specialButton_junimotoggle = 1;

		public InventoryMenu ItemsToGrabMenu;

		private TemporaryAnimatedSprite poof;

		public bool reverseGrab;

		public bool showReceivingMenu = true;

		public bool drawBG = true;

		public bool destroyItemOnClick;

		public bool canExitOnKey;

		public bool playRightClickSound;

		public bool allowRightClick;

		public bool shippingBin;

		private string message;

		private behaviorOnItemSelect behaviorFunction;

		public behaviorOnItemSelect behaviorOnItemGrab;

		private Item hoverItem;

		private Item sourceItem;

		public ClickableTextureComponent fillStacksButton;

		public ClickableTextureComponent organizeButton;

		public ClickableTextureComponent colorPickerToggleButton;

		public ClickableTextureComponent specialButton;

		public ClickableTextureComponent lastShippedHolder;

		public List<ClickableComponent> discreteColorPickerCC;

		public int source;

		public int whichSpecialButton;

		public object context;

		private bool snappedtoBottom;

		public DiscreteColorPicker chestColorPicker;

		private bool essential;

		protected List<TransferredItemSprite> _transferredItemSprites = new List<TransferredItemSprite>();

		protected bool _sourceItemInCurrentLocation;

		public ClickableTextureComponent junimoNoteIcon;

		private int junimoNotePulser;

		public FilterItemGrabMenu(IList<Item> inventory, object context = null)
			: base(null, okButton: true, trashCan: true)
		{
			this.context = context;
			this.ItemsToGrabMenu = new InventoryMenu(base.xPositionOnScreen + 32, base.yPositionOnScreen, playerInventory: false, inventory, null, 9, 1, 0, 0, true);
			base.trashCan.myID = 106;
			this.ItemsToGrabMenu.populateClickableComponentList();
			for (int k = 0; k < this.ItemsToGrabMenu.inventory.Count; k++)
			{
				if (this.ItemsToGrabMenu.inventory[k] != null)
				{
					this.ItemsToGrabMenu.inventory[k].myID += 53910;
					this.ItemsToGrabMenu.inventory[k].upNeighborID += 53910;
					this.ItemsToGrabMenu.inventory[k].rightNeighborID += 53910;
					this.ItemsToGrabMenu.inventory[k].downNeighborID = -7777;
					this.ItemsToGrabMenu.inventory[k].leftNeighborID += 53910;
					this.ItemsToGrabMenu.inventory[k].fullyImmutable = true;
					if (k % (this.ItemsToGrabMenu.capacity / this.ItemsToGrabMenu.rows) == 0)
					{
						this.ItemsToGrabMenu.inventory[k].leftNeighborID = base.dropItemInvisibleButton.myID;
					}
					if (k % (this.ItemsToGrabMenu.capacity / this.ItemsToGrabMenu.rows) == this.ItemsToGrabMenu.capacity / this.ItemsToGrabMenu.rows - 1)
					{
						this.ItemsToGrabMenu.inventory[k].rightNeighborID = base.trashCan.myID;
					}
				}
			}
			for (int j = 0; j < this.GetColumnCount(); j++)
			{
				if (base.inventory != null && base.inventory.inventory != null && base.inventory.inventory.Count >= this.GetColumnCount())
				{
					base.inventory.inventory[j].upNeighborID = (this.shippingBin ? 12598 : (-7777));
				}
			}
			if (!this.shippingBin)
			{
				for (int i = 0; i < this.GetColumnCount() * 3; i++)
				{
					if (base.inventory != null && base.inventory.inventory != null && base.inventory.inventory.Count > i)
					{
						base.inventory.inventory[i].upNeighborID = -7777;
						base.inventory.inventory[i].upNeighborImmutable = true;
					}
				}
			}
			if (base.trashCan != null)
			{
				base.trashCan.leftNeighborID = 11;
			}
			if (base.okButton != null)
			{
				base.okButton.leftNeighborID = 11;
			}
			base.populateClickableComponentList();
			if (Game1.options.SnappyMenus)
			{
				this.snapToDefaultClickableComponent();
			}
			base.inventory.showGrayedOutSlots = true;
			this.SetupBorderNeighbors();
		}

		public virtual void DropRemainingItems()
		{
			if (this.ItemsToGrabMenu == null || this.ItemsToGrabMenu.actualInventory == null)
			{
				return;
			}
			foreach (Item item in this.ItemsToGrabMenu.actualInventory)
			{
				if (item != null)
				{
					Game1.createItemDebris(item, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
				}
			}
			this.ItemsToGrabMenu.actualInventory.Clear();
		}

		public override bool readyToClose()
		{
			return base.readyToClose();
		}

		public FilterItemGrabMenu(Filter filter, IList<Item> inventory, int Capacity, bool reverseGrab, bool showReceivingMenu, InventoryMenu.highlightThisItem highlightFunction, behaviorOnItemSelect behaviorOnItemSelectFunction, string message, behaviorOnItemSelect behaviorOnItemGrab = null, bool snapToBottom = false, bool canBeExitedWithKey = false, bool playRightClickSound = true, bool allowRightClick = true, bool showOrganizeButton = false, int source = 0, Item sourceItem = null, int whichSpecialButton = -1, object context = null)
			: base(highlightFunction, okButton: true, trashCan: true, 0, 0, 64)
		{
			this.source = source;
			this.message = message;
			this.reverseGrab = reverseGrab;
			this.showReceivingMenu = showReceivingMenu;
			this.playRightClickSound = playRightClickSound;
			this.allowRightClick = allowRightClick;
			base.inventory.showGrayedOutSlots = true;
			this.sourceItem = sourceItem;
			if (sourceItem != null && Game1.currentLocation.objects.Values.Contains(sourceItem))
			{
				this._sourceItemInCurrentLocation = true;
			}
			else
			{
				this._sourceItemInCurrentLocation = false;
			}
			if (source == 1 && sourceItem != null && sourceItem is Chest && (sourceItem as Chest).SpecialChestType == Chest.SpecialChestTypes.None)
			{
				this.chestColorPicker = new DiscreteColorPicker(base.xPositionOnScreen, base.yPositionOnScreen - 64 - IClickableMenu.borderWidth * 2, 0, new Chest(playerChest: true, sourceItem.ParentSheetIndex));
				this.chestColorPicker.colorSelection = this.chestColorPicker.getSelectionFromColor((sourceItem as Chest).playerChoiceColor);
				(this.chestColorPicker.itemToDrawColored as Chest).playerChoiceColor.Value = this.chestColorPicker.getColorFromSelection(this.chestColorPicker.colorSelection);
				this.colorPickerToggleButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen + base.width, base.yPositionOnScreen + base.height / 3 - 64 + -160, 64, 64), Game1.mouseCursors, new Rectangle(119, 469, 16, 16), 4f)
				{
					hoverText = Game1.content.LoadString("Strings\\UI:Toggle_ColorPicker"),
					myID = 27346,
					downNeighborID = -99998,
					leftNeighborID = 53921,
					region = 15923
				};
				if (InventoryPage.ShouldShowJunimoNoteIcon())
				{
					this.junimoNoteIcon = new ClickableTextureComponent("", new Rectangle(base.xPositionOnScreen + base.width, base.yPositionOnScreen + base.height / 3 - 64 + -216, 64, 64), "", Game1.content.LoadString("Strings\\UI:GameMenu_JunimoNote_Hover"), Game1.mouseCursors, new Rectangle(331, 374, 15, 14), 4f)
					{
						myID = 898,
						leftNeighborID = 11,
						downNeighborID = 106
					};
				}
			}
			this.whichSpecialButton = whichSpecialButton;
			this.context = context;
			/*
			if (whichSpecialButton == 1)
			{
				this.specialButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen + base.width, base.yPositionOnScreen + base.height / 3 - 64 + -160, 64, 64), Game1.mouseCursors, new Rectangle(108, 491, 16, 16), 4f)
				{
					myID = 12485,
					downNeighborID = (showOrganizeButton ? 12952 : 5948),
					region = 15923,
					leftNeighborID = 53921
				};
				if (context != null && context is JunimoHut)
				{
					this.specialButton.sourceRect.X = ((context as JunimoHut).noHarvest ? 124 : 108);
				}
			}
			*/
			if (snapToBottom)
			{
				base.movePosition(0, Game1.uiViewport.Height - (base.yPositionOnScreen + base.height - IClickableMenu.spaceToClearTopBorder));
				this.snappedtoBottom = true;
			}
			if (source == 1 && sourceItem != null && sourceItem is Chest && (sourceItem as Chest).GetActualCapacity() != 36)
			{
				int capacity = (sourceItem as Chest).GetActualCapacity();
				int rows = 3;
				if (capacity < 9)
				{
					rows = 1;
				}
				int containerWidth = 64 * (capacity / rows);
				this.ItemsToGrabMenu = new InventoryMenu(Game1.uiViewport.Width / 2 - containerWidth / 2, base.yPositionOnScreen, playerInventory: false, inventory, null, Capacity, 1, 0, 0, true);
				if ((sourceItem as Chest).SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin)
				{
					base.inventory.moveItemSound = "Ship";
				}
			}
			else
			{
				int containerWidth = 64 * filter.Cols;
				int containerHeight = 64 * filter.Rows;
				//this.ItemsToGrabMenu = new FilterInventoryMenu(Game1.uiViewport.Width / 2 - containerWidth / 2 - 20, Game1.uiViewport.Height / 2 - containerHeight / 2, playerInventory: false, inventory, null, Capacity, 1, 0, 0, true);
				this.ItemsToGrabMenu = new InventoryMenu(Game1.uiViewport.Width / 2 - containerWidth / 2 - 20, Game1.uiViewport.Height / 2 - containerHeight / 2, playerInventory: false, inventory, null, Capacity, 1, 0, 0, true);

			}
			this.ItemsToGrabMenu.populateClickableComponentList();
			for (int j = 0; j < this.ItemsToGrabMenu.inventory.Count; j++)
			{
				if (this.ItemsToGrabMenu.inventory[j] != null)
				{
					this.ItemsToGrabMenu.inventory[j].myID += 53910;
					this.ItemsToGrabMenu.inventory[j].upNeighborID += 53910;
					this.ItemsToGrabMenu.inventory[j].rightNeighborID += 53910;
					this.ItemsToGrabMenu.inventory[j].downNeighborID = -7777;
					this.ItemsToGrabMenu.inventory[j].leftNeighborID += 53910;
					this.ItemsToGrabMenu.inventory[j].fullyImmutable = true;
				}
			}
			this.behaviorFunction = behaviorOnItemSelectFunction;
			this.behaviorOnItemGrab = behaviorOnItemGrab;
			this.canExitOnKey = canBeExitedWithKey;
			if (showOrganizeButton)
			{
				this.fillStacksButton = new ClickableTextureComponent("", new Rectangle(base.xPositionOnScreen + base.width, base.yPositionOnScreen + base.height / 3 - 64 - 64 - 16, 64, 64), "", Game1.content.LoadString("Strings\\UI:ItemGrab_FillStacks"), Game1.mouseCursors, new Rectangle(103, 469, 16, 16), 4f)
				{
					myID = 12952,
					upNeighborID = ((this.colorPickerToggleButton != null) ? 27346 : ((this.specialButton != null) ? 12485 : (-500))),
					downNeighborID = 106,
					leftNeighborID = 53921,
					region = 15923
				};
				this.organizeButton = new ClickableTextureComponent("", new Rectangle(base.xPositionOnScreen + base.width, base.yPositionOnScreen + base.height / 3 - 64, 64, 64), "", Game1.content.LoadString("Strings\\UI:ItemGrab_Organize"), Game1.mouseCursors, new Rectangle(162, 440, 16, 16), 4f)
				{
					myID = 106,
					upNeighborID = 12952,
					downNeighborID = 5948,
					leftNeighborID = 53921,
					region = 15923
				};
			}
			this.RepositionSideButtons();
			if (this.chestColorPicker != null)
			{
				this.discreteColorPickerCC = new List<ClickableComponent>();
				for (int i = 0; i < this.chestColorPicker.totalColors; i++)
				{
					this.discreteColorPickerCC.Add(new ClickableComponent(new Rectangle(this.chestColorPicker.xPositionOnScreen + IClickableMenu.borderWidth / 2 + i * 9 * 4, this.chestColorPicker.yPositionOnScreen + IClickableMenu.borderWidth / 2, 36, 28), "")
					{
						myID = i + 4343,
						rightNeighborID = ((i < this.chestColorPicker.totalColors - 1) ? (i + 4343 + 1) : (-1)),
						leftNeighborID = ((i > 0) ? (i + 4343 - 1) : (-1)),
						downNeighborID = ((this.ItemsToGrabMenu != null && this.ItemsToGrabMenu.inventory.Count > 0) ? 53910 : 0)
					});
				}
			}
			if (this.organizeButton != null)
			{
				foreach (ClickableComponent item in this.ItemsToGrabMenu.GetBorder(InventoryMenu.BorderSide.Right))
				{
					item.rightNeighborID = this.organizeButton.myID;
				}
			}
			if (base.trashCan != null && base.inventory.inventory.Count >= 12 && base.inventory.inventory[11] != null)
			{
				base.inventory.inventory[11].rightNeighborID = 5948;
			}
			if (base.trashCan != null)
			{
				base.trashCan.leftNeighborID = 11;
			}
			if (base.okButton != null)
			{
				base.okButton.leftNeighborID = 11;
			}
			ClickableComponent top_right = this.ItemsToGrabMenu.GetBorder(InventoryMenu.BorderSide.Right).FirstOrDefault();
			if (top_right != null)
			{
				if (this.organizeButton != null)
				{
					this.organizeButton.leftNeighborID = top_right.myID;
				}
				if (this.specialButton != null)
				{
					this.specialButton.leftNeighborID = top_right.myID;
				}
				if (this.fillStacksButton != null)
				{
					this.fillStacksButton.leftNeighborID = top_right.myID;
				}
				if (this.junimoNoteIcon != null)
				{
					this.junimoNoteIcon.leftNeighborID = top_right.myID;
				}
			}
			base.populateClickableComponentList();
			if (Game1.options.SnappyMenus)
			{
				this.snapToDefaultClickableComponent();
			}
			this.SetupBorderNeighbors();
		}

		public virtual void RepositionSideButtons()
		{
			List<ClickableComponent> side_buttons = new List<ClickableComponent>();
			if (this.organizeButton != null)
			{
				side_buttons.Add(this.organizeButton);
			}
			if (this.fillStacksButton != null)
			{
				side_buttons.Add(this.fillStacksButton);
			}
			if (this.colorPickerToggleButton != null)
			{
				side_buttons.Add(this.colorPickerToggleButton);
			}
			if (this.specialButton != null)
			{
				side_buttons.Add(this.specialButton);
			}
			if (this.junimoNoteIcon != null)
			{
				side_buttons.Add(this.junimoNoteIcon);
			}
			int step_size = 80;
			if (side_buttons.Count >= 4)
			{
				step_size = 72;
			}
			for (int i = 0; i < side_buttons.Count; i++)
			{
				ClickableComponent button = side_buttons[i];
				if (i > 0 && side_buttons.Count > 1)
				{
					button.downNeighborID = side_buttons[i - 1].myID;
				}
				if (i < side_buttons.Count - 1 && side_buttons.Count > 1)
				{
					button.upNeighborID = side_buttons[i + 1].myID;
				}
				button.bounds.X = base.xPositionOnScreen + base.width;
				button.bounds.Y = base.yPositionOnScreen + base.height / 3 - 64 - step_size * i;
			}
		}

		public void SetupBorderNeighbors()
		{
			List<ClickableComponent> border = base.inventory.GetBorder(InventoryMenu.BorderSide.Right);
			foreach (ClickableComponent item in border)
			{
				item.rightNeighborID = -99998;
				item.rightNeighborImmutable = true;
			}
			border = this.ItemsToGrabMenu.GetBorder(InventoryMenu.BorderSide.Right);
			bool has_organizational_buttons = false;
			foreach (ClickableComponent allClickableComponent in base.allClickableComponents)
			{
				if (allClickableComponent.region == 15923)
				{
					has_organizational_buttons = true;
					break;
				}
			}
			foreach (ClickableComponent slot in border)
			{
				if (has_organizational_buttons)
				{
					slot.rightNeighborID = -99998;
					slot.rightNeighborImmutable = true;
				}
				else
				{
					slot.rightNeighborID = -1;
				}
			}
			for (int j = 0; j < this.GetColumnCount(); j++)
			{
				if (base.inventory != null && base.inventory.inventory != null && base.inventory.inventory.Count >= 12)
				{
					base.inventory.inventory[j].upNeighborID = (this.shippingBin ? 12598 : ((this.discreteColorPickerCC != null && this.ItemsToGrabMenu != null && this.ItemsToGrabMenu.inventory.Count <= j && Game1.player.showChestColorPicker) ? 4343 : ((this.ItemsToGrabMenu.inventory.Count > j) ? (53910 + j) : 53910)));
				}
				if (this.discreteColorPickerCC != null && this.ItemsToGrabMenu != null && this.ItemsToGrabMenu.inventory.Count > j && Game1.player.showChestColorPicker)
				{
					this.ItemsToGrabMenu.inventory[j].upNeighborID = 4343;
				}
				else
				{
					this.ItemsToGrabMenu.inventory[j].upNeighborID = -1;
				}
			}
			if (this.shippingBin)
			{
				return;
			}
			for (int i = 0; i < 36; i++)
			{
				if (base.inventory != null && base.inventory.inventory != null && base.inventory.inventory.Count > i)
				{
					base.inventory.inventory[i].upNeighborID = -7777;
					base.inventory.inventory[i].upNeighborImmutable = true;
				}
			}
		}

		public virtual int GetColumnCount()
		{
			return this.ItemsToGrabMenu.capacity / this.ItemsToGrabMenu.rows;
		}

		public FilterItemGrabMenu setEssential(bool essential)
		{
			this.essential = essential;
			return this;
		}

		public void initializeShippingBin()
		{
			this.shippingBin = true;
			this.lastShippedHolder = new ClickableTextureComponent("", new Rectangle(base.xPositionOnScreen + base.width / 2 - 48, base.yPositionOnScreen + base.height / 2 - 80 - 64, 96, 96), "", Game1.content.LoadString("Strings\\UI:ShippingBin_LastItem"), Game1.mouseCursors, new Rectangle(293, 360, 24, 24), 4f)
			{
				myID = 12598,
				region = 12598
			};
			for (int i = 0; i < this.GetColumnCount(); i++)
			{
				if (base.inventory != null && base.inventory.inventory != null && base.inventory.inventory.Count >= this.GetColumnCount())
				{
					base.inventory.inventory[i].upNeighborID = -7777;
					if (i == 11)
					{
						base.inventory.inventory[i].rightNeighborID = 5948;
					}
				}
			}
			base.populateClickableComponentList();
			if (Game1.options.SnappyMenus)
			{
				this.snapToDefaultClickableComponent();
			}
		}

		protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
		{
			switch (direction)
			{
			case 2:
			{
				for (int j = 0; j < 12; j++)
				{
					if (base.inventory != null && base.inventory.inventory != null && base.inventory.inventory.Count >= this.GetColumnCount() && this.shippingBin)
					{
						base.inventory.inventory[j].upNeighborID = (this.shippingBin ? 12598 : (Math.Min(j, this.ItemsToGrabMenu.inventory.Count - 1) + 53910));
					}
				}
				if (!this.shippingBin && oldID >= 53910)
				{
					int index = oldID - 53910;
					if (index + this.GetColumnCount() <= this.ItemsToGrabMenu.inventory.Count - 1)
					{
						base.currentlySnappedComponent = base.getComponentWithID(index + this.GetColumnCount() + 53910);
						this.snapCursorToCurrentSnappedComponent();
						break;
					}
				}
				base.currentlySnappedComponent = base.getComponentWithID((oldRegion != 12598) ? ((oldID - 53910) % this.GetColumnCount()) : 0);
				this.snapCursorToCurrentSnappedComponent();
				break;
			}
			case 0:
			{
				if (this.shippingBin && Game1.getFarm().lastItemShipped != null && oldID < 12)
				{
					base.currentlySnappedComponent = base.getComponentWithID(12598);
					base.currentlySnappedComponent.downNeighborID = oldID;
					this.snapCursorToCurrentSnappedComponent();
					break;
				}
				if (oldID < 53910 && oldID >= 12)
				{
					base.currentlySnappedComponent = base.getComponentWithID(oldID - 12);
					break;
				}
				int id = oldID + this.GetColumnCount() * 2;
				for (int i = 0; i < 3; i++)
				{
					if (this.ItemsToGrabMenu.inventory.Count > id)
					{
						break;
					}
					id -= this.GetColumnCount();
				}
				if (this.showReceivingMenu)
				{
					if (id < 0)
					{
						if (this.ItemsToGrabMenu.inventory.Count > 0)
						{
							base.currentlySnappedComponent = base.getComponentWithID(53910 + this.ItemsToGrabMenu.inventory.Count - 1);
						}
						else if (this.discreteColorPickerCC != null)
						{
							base.currentlySnappedComponent = base.getComponentWithID(4343);
						}
					}
					else
					{
						base.currentlySnappedComponent = base.getComponentWithID(id + 53910);
						if (base.currentlySnappedComponent == null)
						{
							base.currentlySnappedComponent = base.getComponentWithID(53910);
						}
					}
				}
				this.snapCursorToCurrentSnappedComponent();
				break;
			}
			}
		}

		public override void snapToDefaultClickableComponent()
		{
			if (this.shippingBin)
			{
				base.currentlySnappedComponent = base.getComponentWithID(0);
			}
			else if (this.source == 1 && this.sourceItem != null && this.sourceItem is Chest && (this.sourceItem as Chest).SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin)
			{
				base.currentlySnappedComponent = base.getComponentWithID(0);
			}
			else
			{
				base.currentlySnappedComponent = base.getComponentWithID((this.ItemsToGrabMenu.inventory.Count > 0 && this.showReceivingMenu) ? 53910 : 0);
			}
			this.snapCursorToCurrentSnappedComponent();
		}

		public void setSourceItem(Item item)
		{
			this.sourceItem = item;
			this.chestColorPicker = null;
			this.colorPickerToggleButton = null;
			if (this.source == 1 && this.sourceItem != null && this.sourceItem is Chest && (this.sourceItem as Chest).SpecialChestType == Chest.SpecialChestTypes.None)
			{
				this.chestColorPicker = new DiscreteColorPicker(base.xPositionOnScreen, base.yPositionOnScreen - 64 - IClickableMenu.borderWidth * 2, 0, new Chest(playerChest: true, this.sourceItem.ParentSheetIndex));
				this.chestColorPicker.colorSelection = this.chestColorPicker.getSelectionFromColor((this.sourceItem as Chest).playerChoiceColor);
				(this.chestColorPicker.itemToDrawColored as Chest).playerChoiceColor.Value = this.chestColorPicker.getColorFromSelection(this.chestColorPicker.colorSelection);
				this.colorPickerToggleButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen + base.width, base.yPositionOnScreen + base.height / 3 - 64 + -160, 64, 64), Game1.mouseCursors, new Rectangle(119, 469, 16, 16), 4f)
				{
					hoverText = Game1.content.LoadString("Strings\\UI:Toggle_ColorPicker")
				};
			}
			this.RepositionSideButtons();
		}

		public override bool IsAutomaticSnapValid(int direction, ClickableComponent a, ClickableComponent b)
		{
			if (direction == 1 && this.ItemsToGrabMenu.inventory.Contains(a) && base.inventory.inventory.Contains(b))
			{
				return false;
			}
			return base.IsAutomaticSnapValid(direction, a, b);
		}

		public void setBackgroundTransparency(bool b)
		{
			this.drawBG = b;
		}

		public void setDestroyItemOnClick(bool b)
		{
			this.destroyItemOnClick = b;
		}

		public override void receiveRightClick(int x, int y, bool playSound = true)
		{
			if (!this.allowRightClick)
			{
				base.receiveRightClickOnlyToolAttachments(x, y);
				return;
			}
			base.receiveRightClick(x, y, playSound && this.playRightClickSound);
			if (base.heldItem == null && this.showReceivingMenu)
			{
				base.heldItem = this.ItemsToGrabMenu.rightClick(x, y, base.heldItem, playSound: false);
				if (base.heldItem != null && this.behaviorOnItemGrab != null)
				{
					this.behaviorOnItemGrab(base.heldItem, Game1.player);
					if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu)
					{
						(Game1.activeClickableMenu as ItemGrabMenu).setSourceItem(this.sourceItem);
					}
					if (Game1.options.SnappyMenus)
					{
						(Game1.activeClickableMenu as ItemGrabMenu).currentlySnappedComponent = base.currentlySnappedComponent;
						(Game1.activeClickableMenu as ItemGrabMenu).snapCursorToCurrentSnappedComponent();
					}
				}
				if (Utility.IsNormalObjectAtParentSheetIndex(base.heldItem, 326))
				{
					base.heldItem = null;
					Game1.player.canUnderstandDwarves = true;
					this.poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2(x - x % 64 + 16, y - y % 64 + 16), flicker: false, flipped: false);
					Game1.playSound("fireball");
				}
				else if (base.heldItem is Object && Utility.IsNormalObjectAtParentSheetIndex(base.heldItem, 434))
				{
					Object held_item = base.heldItem as Object;
					base.heldItem = null;
					base.exitThisMenu(playSound: false);
					Game1.player.eatObject(held_item, overrideFullness: true);
				}
				else if (base.heldItem is Object && (bool)(base.heldItem as Object).isRecipe)
				{
					string recipeName = base.heldItem.Name.Substring(0, base.heldItem.Name.IndexOf("Recipe") - 1);
					try
					{
						if ((base.heldItem as Object).Category == -7)
						{
							Game1.player.cookingRecipes.Add(recipeName, 0);
						}
						else
						{
							Game1.player.craftingRecipes.Add(recipeName, 0);
						}
						this.poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2(x - x % 64 + 16, y - y % 64 + 16), flicker: false, flipped: false);
						Game1.playSound("newRecipe");
					}
					catch (Exception)
					{
					}
					base.heldItem = null;
				}
				else if (Game1.player.addItemToInventoryBool(base.heldItem))
				{
					base.heldItem = null;
					Game1.playSound("coin");
				}
			}
			else if (this.reverseGrab || this.behaviorFunction != null)
			{
				this.behaviorFunction(base.heldItem, Game1.player);
				if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu)
				{
					(Game1.activeClickableMenu as ItemGrabMenu).setSourceItem(this.sourceItem);
				}
				/*
				if (this.destroyItemOnClick)
				{
					base.heldItem = null;
				}
				*/
			}
		}

		public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
		{
			if (this.snappedtoBottom)
			{
				base.movePosition((newBounds.Width - oldBounds.Width) / 2, Game1.uiViewport.Height - (base.yPositionOnScreen + base.height - IClickableMenu.spaceToClearTopBorder));
			}
			else
			{
				base.movePosition((newBounds.Width - oldBounds.Width) / 2, (newBounds.Height - oldBounds.Height) / 2);
			}
			if (this.ItemsToGrabMenu != null)
			{
				this.ItemsToGrabMenu.gameWindowSizeChanged(oldBounds, newBounds);
			}
			this.RepositionSideButtons();
			if (this.source == 1 && this.sourceItem != null && this.sourceItem is Chest && (this.sourceItem as Chest).SpecialChestType == Chest.SpecialChestTypes.None)
			{
				this.chestColorPicker = new DiscreteColorPicker(base.xPositionOnScreen, base.yPositionOnScreen - 64 - IClickableMenu.borderWidth * 2, 0, new Chest(playerChest: true, this.sourceItem.ParentSheetIndex));
				this.chestColorPicker.colorSelection = this.chestColorPicker.getSelectionFromColor((this.sourceItem as Chest).playerChoiceColor);
			}
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			base.receiveLeftClick(x, y, (!this.destroyItemOnClick) ? true : false);
			if (base.heldItem == null && this.showReceivingMenu)
			{

				base.heldItem = this.ItemsToGrabMenu.leftClick(x, y, base.heldItem, playSound: false);
				if (base.heldItem != null && this.behaviorOnItemGrab != null)
				{
					this.behaviorOnItemGrab(base.heldItem, Game1.player);
					if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu)
					{
						(Game1.activeClickableMenu as ItemGrabMenu).setSourceItem(this.sourceItem);
						if (Game1.options.SnappyMenus)
						{
							(Game1.activeClickableMenu as ItemGrabMenu).currentlySnappedComponent = base.currentlySnappedComponent;
							(Game1.activeClickableMenu as ItemGrabMenu).snapCursorToCurrentSnappedComponent();
						}
					}
				}
				else if (base.heldItem != null && !this.isWithinBounds(x, y) && base.heldItem.canBeTrashed())
				{
					this.DropHeldItem();
				}
			}
			else if ((this.reverseGrab || this.behaviorFunction != null) && this.isWithinBounds(x, y))
			{
				this.behaviorFunction(base.heldItem, Game1.player);
				
				if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu)
				{
					(Game1.activeClickableMenu as ItemGrabMenu).setSourceItem(this.sourceItem);
					if (Game1.options.SnappyMenus)
					{
						(Game1.activeClickableMenu as ItemGrabMenu).currentlySnappedComponent = base.currentlySnappedComponent;
						(Game1.activeClickableMenu as ItemGrabMenu).snapCursorToCurrentSnappedComponent();
					}
				}
				
			}
		
		}

		public virtual void DropHeldItem()
		{
			if (base.heldItem != null)
			{
				Game1.playSound("throwDownITem");
				Console.WriteLine("Dropping " + base.heldItem.Name);
				int drop_direction = Game1.player.facingDirection;
				if (this.context is LibraryMuseum)
				{
					drop_direction = 2;
				}
				Game1.createItemDebris(base.heldItem, Game1.player.getStandingPosition(), drop_direction);
				if (base.inventory.onAddItem != null)
				{
					base.inventory.onAddItem(base.heldItem, Game1.player);
				}
				base.heldItem = null;
			}
		}

		public void FillOutStacks()
		{
			for (int i = 0; i < this.ItemsToGrabMenu.actualInventory.Count; i++)
			{
				Item chest_item = this.ItemsToGrabMenu.actualInventory[i];
				if (chest_item == null || chest_item.maximumStackSize() <= 1)
				{
					continue;
				}
				for (int j = 0; j < base.inventory.actualInventory.Count; j++)
				{
					Item inventory_item = base.inventory.actualInventory[j];
					if (inventory_item == null || !chest_item.canStackWith(inventory_item))
					{
						continue;
					}
					TransferredItemSprite item_sprite = new TransferredItemSprite(inventory_item.getOne(), base.inventory.inventory[j].bounds.X, base.inventory.inventory[j].bounds.Y);
					this._transferredItemSprites.Add(item_sprite);
					int stack_count = inventory_item.Stack;
					if (chest_item.getRemainingStackSpace() > 0)
					{
						stack_count = chest_item.addToStack(inventory_item);
						this.ItemsToGrabMenu.ShakeItem(chest_item);
					}
					inventory_item.Stack = stack_count;
					while (inventory_item.Stack > 0)
					{
						Item overflow_stack = null;
						if (!Utility.canItemBeAddedToThisInventoryList(chest_item.getOne(), this.ItemsToGrabMenu.actualInventory, this.ItemsToGrabMenu.capacity))
						{
							break;
						}
						if (overflow_stack == null)
						{
							for (int l = 0; l < this.ItemsToGrabMenu.actualInventory.Count; l++)
							{
								if (this.ItemsToGrabMenu.actualInventory[l] != null && this.ItemsToGrabMenu.actualInventory[l].canStackWith(chest_item) && this.ItemsToGrabMenu.actualInventory[l].getRemainingStackSpace() > 0)
								{
									overflow_stack = this.ItemsToGrabMenu.actualInventory[l];
									break;
								}
							}
						}
						if (overflow_stack == null)
						{
							for (int k = 0; k < this.ItemsToGrabMenu.actualInventory.Count; k++)
							{
								if (this.ItemsToGrabMenu.actualInventory[k] == null)
								{
									Item item = (this.ItemsToGrabMenu.actualInventory[k] = chest_item.getOne());
									overflow_stack = item;
									overflow_stack.Stack = 0;
									break;
								}
							}
						}
						if (overflow_stack == null && this.ItemsToGrabMenu.actualInventory.Count < this.ItemsToGrabMenu.capacity)
						{
							overflow_stack = chest_item.getOne();
							overflow_stack.Stack = 0;
							this.ItemsToGrabMenu.actualInventory.Add(overflow_stack);
						}
						if (overflow_stack == null)
						{
							break;
						}
						stack_count = overflow_stack.addToStack(inventory_item);
						this.ItemsToGrabMenu.ShakeItem(overflow_stack);
						inventory_item.Stack = stack_count;
					}
					if (inventory_item.Stack == 0)
					{
						base.inventory.actualInventory[j] = null;
					}
				}
			}
		}

		public static void organizeItemsInList(IList<Item> items)
		{
			List<Item> copy = new List<Item>(items);
			List<Item> tools = new List<Item>();
			for (int l = 0; l < copy.Count; l++)
			{
				if (copy[l] == null)
				{
					copy.RemoveAt(l);
					l--;
				}
				else if (copy[l] is Tool)
				{
					tools.Add(copy[l]);
					copy.RemoveAt(l);
					l--;
				}
			}
			for (int k = 0; k < copy.Count; k++)
			{
				Item current_item = copy[k];
				if (current_item.getRemainingStackSpace() <= 0)
				{
					continue;
				}
				for (int m = k + 1; m < copy.Count; m++)
				{
					Item other_item = copy[m];
					if (current_item.canStackWith(other_item))
					{
						other_item.Stack = current_item.addToStack(other_item);
						if (other_item.Stack == 0)
						{
							copy.RemoveAt(m);
							m--;
						}
					}
				}
			}
			copy.Sort();
			copy.InsertRange(0, tools);
			for (int j = 0; j < items.Count; j++)
			{
				items[j] = null;
			}
			for (int i = 0; i < copy.Count; i++)
			{
				items[i] = copy[i];
			}
		}

		public bool areAllItemsTaken()
		{
			for (int i = 0; i < this.ItemsToGrabMenu.actualInventory.Count; i++)
			{
				if (this.ItemsToGrabMenu.actualInventory[i] != null)
				{
					return false;
				}
			}
			return true;
		}

		public override void receiveGamePadButton(Buttons b)
		{
			base.receiveGamePadButton(b);
			if (b == Buttons.Back && this.organizeButton != null)
			{
				ItemGrabMenu.organizeItemsInList(Game1.player.items);
				Game1.playSound("Ship");
			}
			if (b == Buttons.RightShoulder)
			{
				ClickableComponent fill_stacks_component = base.getComponentWithID(12952);
				if (fill_stacks_component != null)
				{
					this.setCurrentlySnappedComponentTo(fill_stacks_component.myID);
					this.snapCursorToCurrentSnappedComponent();
				}
				else
				{
					int highest_y = -1;
					ClickableComponent highest_component = null;
					foreach (ClickableComponent component2 in base.allClickableComponents)
					{
						if (component2.region == 15923 && (highest_y == -1 || component2.bounds.Y < highest_y))
						{
							highest_y = component2.bounds.Y;
							highest_component = component2;
						}
					}
					if (highest_component != null)
					{
						this.setCurrentlySnappedComponentTo(highest_component.myID);
						this.snapCursorToCurrentSnappedComponent();
					}
				}
			}
			if (this.shippingBin || b != Buttons.LeftShoulder)
			{
				return;
			}
			ClickableComponent component = base.getComponentWithID(53910);
			if (component != null)
			{
				this.setCurrentlySnappedComponentTo(component.myID);
				this.snapCursorToCurrentSnappedComponent();
				return;
			}
			component = base.getComponentWithID(0);
			if (component != null)
			{
				this.setCurrentlySnappedComponentTo(0);
				this.snapCursorToCurrentSnappedComponent();
			}
		}

		public override void receiveKeyPress(Keys key)
		{
			if (Game1.options.snappyMenus && Game1.options.gamepadControls)
			{
				base.applyMovementKey(key);
			}
			if ((this.canExitOnKey || this.areAllItemsTaken()) && Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
			{
				base.exitThisMenu();
				if (Game1.currentLocation.currentEvent != null && Game1.currentLocation.currentEvent.CurrentCommand > 0)
				{
					Game1.currentLocation.currentEvent.CurrentCommand++;
				}
			}
			else if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && base.heldItem != null)
			{
				Game1.setMousePosition(base.trashCan.bounds.Center);
			}
			if (key == Keys.Delete && base.heldItem != null && base.heldItem.canBeTrashed())
			{
				Utility.trashItem(base.heldItem);
				base.heldItem = null;
			}
		}

		public override void update(GameTime time)
		{
			base.update(time);
			if (this.poof != null && this.poof.update(time))
			{
				this.poof = null;
			}
			if (this.chestColorPicker != null)
			{
				this.chestColorPicker.update(time);
			}
			if (this.sourceItem != null && this.sourceItem is Chest && this._sourceItemInCurrentLocation)
			{
				Vector2 tileLocation = (this.sourceItem as Object).tileLocation;
				if (tileLocation != Vector2.Zero && !Game1.currentLocation.objects.ContainsKey(tileLocation))
				{
					if (Game1.activeClickableMenu != null)
					{
						Game1.activeClickableMenu.emergencyShutDown();
					}
					Game1.exitActiveMenu();
				}
			}
			for (int i = 0; i < this._transferredItemSprites.Count; i++)
			{
				if (this._transferredItemSprites[i].Update(time))
				{
					this._transferredItemSprites.RemoveAt(i);
					i--;
				}
			}
		}

		public override void performHoverAction(int x, int y)
		{
			base.hoveredItem = null;
			base.hoverText = "";
			base.performHoverAction(x, y);
			if (this.colorPickerToggleButton != null)
			{
				this.colorPickerToggleButton.tryHover(x, y, 0.25f);
				if (this.colorPickerToggleButton.containsPoint(x, y))
				{
					base.hoverText = this.colorPickerToggleButton.hoverText;
				}
			}
			if (this.organizeButton != null)
			{
				this.organizeButton.tryHover(x, y, 0.25f);
				if (this.organizeButton.containsPoint(x, y))
				{
					base.hoverText = this.organizeButton.hoverText;
				}
			}
			if (this.fillStacksButton != null)
			{
				this.fillStacksButton.tryHover(x, y, 0.25f);
				if (this.fillStacksButton.containsPoint(x, y))
				{
					base.hoverText = this.fillStacksButton.hoverText;
				}
			}
			if (this.specialButton != null)
			{
				this.specialButton.tryHover(x, y, 0.25f);
			}
			if (this.showReceivingMenu)
			{
				Item item_grab_hovered_item = this.ItemsToGrabMenu.hover(x, y, base.heldItem);
				if (item_grab_hovered_item != null)
				{
					base.hoveredItem = item_grab_hovered_item;
				}
			}
			if (this.junimoNoteIcon != null)
			{
				this.junimoNoteIcon.tryHover(x, y);
				if (this.junimoNoteIcon.containsPoint(x, y))
				{
					base.hoverText = this.junimoNoteIcon.hoverText;
				}
				if (GameMenu.bundleItemHovered)
				{
					this.junimoNoteIcon.scale = this.junimoNoteIcon.baseScale + (float)Math.Sin((float)this.junimoNotePulser / 100f) / 4f;
					this.junimoNotePulser += (int)Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
				}
				else
				{
					this.junimoNotePulser = 0;
					this.junimoNoteIcon.scale = this.junimoNoteIcon.baseScale;
				}
			}
			if (base.hoverText != null)
			{
				return;
			}
			if (this.organizeButton != null)
			{
				base.hoverText = null;
				this.organizeButton.tryHover(x, y);
				if (this.organizeButton.containsPoint(x, y))
				{
					base.hoverText = this.organizeButton.hoverText;
				}
			}
			if (this.shippingBin)
			{
				base.hoverText = null;
				if (this.lastShippedHolder.containsPoint(x, y) && Game1.getFarm().lastItemShipped != null)
				{
					base.hoverText = this.lastShippedHolder.hoverText;
				}
			}
			if (this.chestColorPicker != null)
			{
				this.chestColorPicker.performHoverAction(x, y);
			}
		}

		public override void draw(SpriteBatch b)
		{
			if (this.drawBG)
			{
				b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);
			}
			base.draw(b, drawUpperPortion: false, drawDescriptionArea: true);
			if (this.showReceivingMenu)
			{
				/*
				b.Draw(Game1.mouseCursors, new Vector2(base.xPositionOnScreen - 64, base.yPositionOnScreen + base.height / 2 + 64 + 16), new Rectangle(16, 368, 12, 16), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
				b.Draw(Game1.mouseCursors, new Vector2(base.xPositionOnScreen - 64, base.yPositionOnScreen + base.height / 2 + 64 - 16), new Rectangle(21, 368, 11, 16), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
				b.Draw(Game1.mouseCursors, new Vector2(base.xPositionOnScreen - 40, base.yPositionOnScreen + base.height / 2 + 64 - 44), new Rectangle(4, 372, 8, 11), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
				if ((this.source != 1 || this.sourceItem == null || !(this.sourceItem is Chest) || ((this.sourceItem as Chest).SpecialChestType != Chest.SpecialChestTypes.MiniShippingBin && (this.sourceItem as Chest).SpecialChestType != Chest.SpecialChestTypes.JunimoChest && (this.sourceItem as Chest).SpecialChestType != Chest.SpecialChestTypes.Enricher)) && this.source != 0)
				{
					//b.Draw(Game1.mouseCursors, new Vector2(base.xPositionOnScreen - 72, base.yPositionOnScreen + 64 + 16), new Rectangle(16, 368, 12, 16), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
					//b.Draw(Game1.mouseCursors, new Vector2(base.xPositionOnScreen - 72, base.yPositionOnScreen + 64 - 16), new Rectangle(21, 368, 11, 16), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
					Rectangle sourceRect = new Rectangle(127, 412, 10, 11);
					switch (this.source)
					{
					case 3:
						sourceRect.X += 10;
						break;
					case 2:
						sourceRect.X += 20;
						break;
					}
					b.Draw(Game1.mouseCursors, new Vector2(base.xPositionOnScreen - 52, base.yPositionOnScreen + 64 - 44), sourceRect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
				}
				*/
				Game1.drawDialogueBox(this.ItemsToGrabMenu.xPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder, this.ItemsToGrabMenu.yPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder, this.ItemsToGrabMenu.width + IClickableMenu.borderWidth * 2 + IClickableMenu.spaceToClearSideBorder * 2, this.ItemsToGrabMenu.height + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth * 2, speaker: false, drawOnlyBox: true);
				
				this.ItemsToGrabMenu.draw(b);
			}
			else if (this.message != null)
			{
				Game1.drawDialogueBox(Game1.uiViewport.Width / 2, this.ItemsToGrabMenu.yPositionOnScreen + this.ItemsToGrabMenu.height / 2, speaker: false, drawOnlyBox: false, this.message);
			}
			if (this.poof != null)
			{
				this.poof.draw(b, localPosition: true);
			}
			foreach (TransferredItemSprite transferredItemSprite in this._transferredItemSprites)
			{
				transferredItemSprite.Draw(b);
			}
			if (this.shippingBin && Game1.getFarm().lastItemShipped != null)
			{
				this.lastShippedHolder.draw(b);
				Game1.getFarm().lastItemShipped.drawInMenu(b, new Vector2(this.lastShippedHolder.bounds.X + 16, this.lastShippedHolder.bounds.Y + 16), 1f);
				b.Draw(Game1.mouseCursors, new Vector2(this.lastShippedHolder.bounds.X + -8, this.lastShippedHolder.bounds.Bottom - 100), new Rectangle(325, 448, 5, 14), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
				b.Draw(Game1.mouseCursors, new Vector2(this.lastShippedHolder.bounds.X + 84, this.lastShippedHolder.bounds.Bottom - 100), new Rectangle(325, 448, 5, 14), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
				b.Draw(Game1.mouseCursors, new Vector2(this.lastShippedHolder.bounds.X + -8, this.lastShippedHolder.bounds.Bottom - 44), new Rectangle(325, 452, 5, 13), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
				b.Draw(Game1.mouseCursors, new Vector2(this.lastShippedHolder.bounds.X + 84, this.lastShippedHolder.bounds.Bottom - 44), new Rectangle(325, 452, 5, 13), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
			}
			
			if (this.colorPickerToggleButton != null)
			{
				this.colorPickerToggleButton.draw(b);
			}
			else if (this.specialButton != null)
			{
				this.specialButton.draw(b);
			}
			if (this.chestColorPicker != null)
			{
				this.chestColorPicker.draw(b);
			}
			if (this.organizeButton != null)
			{
				this.organizeButton.draw(b);
			}
			if (this.fillStacksButton != null)
			{
				this.fillStacksButton.draw(b);
			}
			if (this.junimoNoteIcon != null)
			{
				this.junimoNoteIcon.draw(b);
			}
			
			if (base.hoverText != null && (base.hoveredItem == null || base.hoveredItem == null || this.ItemsToGrabMenu == null))
			{
				if (base.hoverAmount > 0)
				{
					IClickableMenu.drawToolTip(b, base.hoverText, "", null, heldItem: true, -1, 0, -1, -1, null, base.hoverAmount);
				}
				else
				{
					IClickableMenu.drawHoverText(b, base.hoverText, Game1.smallFont);
				}
			}
			if (base.hoveredItem != null)
			{
				IClickableMenu.drawToolTip(b, base.hoveredItem.getDescription(), base.hoveredItem.DisplayName, base.hoveredItem, base.heldItem != null);
			}
			else if (base.hoveredItem != null && this.ItemsToGrabMenu != null)
			{
				IClickableMenu.drawToolTip(b, this.ItemsToGrabMenu.descriptionText, this.ItemsToGrabMenu.descriptionTitle, base.hoveredItem, base.heldItem != null);
			}
			if (base.heldItem != null)
			{
				base.heldItem.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);
			}
			Game1.mouseCursorTransparency = 1f;
			base.drawMouse(b);
		}

		public override void emergencyShutDown()
		{
			base.emergencyShutDown();
			Console.WriteLine("ItemGrabMenu.emergencyShutDown");
			if (base.heldItem != null)
			{
				Console.WriteLine("Taking " + base.heldItem.Name);
				base.heldItem = Game1.player.addItemToInventory(base.heldItem);
			}
			if (base.heldItem != null)
			{
				this.DropHeldItem();
			}
			if (this.essential)
			{
				Console.WriteLine("essential");
				foreach (Item item in this.ItemsToGrabMenu.actualInventory)
				{
					if (item != null)
					{
						Console.WriteLine("Taking " + item.Name);
						Item leftOver = Game1.player.addItemToInventory(item);
						if (leftOver != null)
						{
							Console.WriteLine("Dropping " + leftOver.Name);
							Game1.createItemDebris(leftOver, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
						}
					}
				}
			}
			else
			{
				Console.WriteLine("essential");
			}
		}

	}
}
