/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BetterShipping
{
    internal enum Neighbor
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    internal enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    internal class BinMenuOverride : MenuWithInventory
    {
        private const int BaseIdPlayerInventory = 36000;
        private const int BaseIdBinInventory = 69500;

        private const int NoId = -7777;
        private const int ExitId = 0;
        private const int ArrowUpId = 1;
        private const int ArrowDownId = 2;
        private const int TrashCanId = 3;

        private const int RowLength = 12;

        private readonly int maxItemsPerPage = Chest.capacity;
        private readonly Farm farm = Game1.RequireLocation<Farm>("Farm");

        private int offset;
        private InventoryMenu itemsMenu;
        private ClickableTextureComponent upArrow;
        private ClickableTextureComponent downArrow;
        private bool updatedBounds;

        private bool isShift => Game1.oldKBState.IsKeyDown(Keys.LeftShift) || Game1.oldKBState.IsKeyDown(Keys.RightShift);
        private bool isCtrl => Game1.oldKBState.IsKeyDown(Keys.LeftControl) || Game1.oldKBState.IsKeyDown(Keys.RightControl);
        private bool canScroll => canScrollUp || canScrollDown;
        private bool canScrollUp => offset > 0;
        private bool canScrollDown => offset < maxOffset;
        public int Offset => offset;
        private int maxOffset => (int)Math.Ceiling(((double)actuallItems.Count - maxItemsPerPage) / ((double)maxItemsPerPage / 3));
        private IList<Item> actuallItems => farm.getShippingBin(Game1.player);
        private List<Item> itemsInView => getItemsInView(offset);

        //Changed accesibility and name for hoverItem for Lookup Anything compat (easiest compat I've ever made... Thanks Pathos)
        public Item HoveredItem;

        public BinMenuOverride(int offset = 0) : base(Utility.highlightShippableObjects, false, true, menuOffsetHack: 64)
        {
            this.offset = offset;
            inventory.showGrayedOutSlots = true;
            HeldItemExitBehavior = ItemExitBehavior.Drop;
            loadViewComponents();
            RenderItems();
            if (Game1.options.SnappyMenus)
                setUpForGamePadMode();
        }

        /// <summary>
        /// Snap the cursor to the default component for GamePad mode
        /// </summary>
        /// <remarks>
        /// This is broken and refuses to fire the first time
        /// </remarks>
        public override void setUpForGamePadMode()
        {
            snapToDefaultClickableComponent();
            snapCursorToCurrentSnappedComponent();
        }

        public override void receiveGamePadButton(Buttons b)
        {
            switch (b)
            {
                case Buttons.Back:
                case Buttons.B:
                case Buttons.Y:
                    exitThisMenu();
                    break;
                case Buttons.LeftShoulder:
                    receiveScrollWheelAction(1);
                    break;
                case Buttons.RightShoulder:
                    receiveScrollWheelAction(-1);
                    break;
            }
        }

        public override void applyMovementKey(int direction)
        {
            if (currentlySnappedComponent is null)
            {
                snapToDefaultClickableComponent();
                return;
            }
            Direction directionEnum = direction switch
            {
                0 => Direction.Up,
                2 => Direction.Down,
                1 => Direction.Right,
                3 => Direction.Left
            };
            snapToNextClickableComponent(currentlySnappedComponent.myID, directionEnum);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            Item? item;
            int stack = getStack();
            if (itemsMenu.isWithinBounds(x, y))
            {
                item = itemsMenu.getItemAt(x, y);

                if (item == null || !Utility.highlightShippableObjects(item))
                    return;

                int slot = getSlotIndexAt(x, y, false);
                if (slot == -1) 
                    return;

                if (heldItem is null)
                {
                    heldItem = TakeItemAt(slot, stack, false);
                    if (playSound)
                        Game1.playSound("dwop");
                    if (isShift)
                        heldItem = Game1.player.addItemToInventory(heldItem);
                    return;
                }
                else if (heldItem.canStackWith(item))
                {
                    var taken = TakeItemAt(slot, stack, false);
                    heldItem.Stack += taken!.Stack;
                    if (playSound)
                        Game1.playSound("dwop");
                    return;
                }
            }
            else if (inventory.isWithinBounds(x, y))
            {
                item = inventory.getItemAt(x, y);

                int slot = getSlotIndexAt(x, y, true);
                if (slot == -1)
                    return;

                if (heldItem is null && item is not null && Utility.highlightShippableObjects(item))
                {
                    var taken = TakeItemAt(slot, stack, true);
                    PushItemToBin(taken!);
                    if (playSound)
                        Game1.playSound("Ship");
                    return;
                }
                else if (heldItem is not null)
                {
                    if (item is null || !Utility.highlightShippableObjects(item))
                        return;
                    heldItem = TryPlaceItemAt(heldItem, slot, true);
                    if (playSound)
                        Game1.playSound("backpackIN");    
                    return;
                }
            }
        }

        public override void receiveScrollWheelAction(int direction)
        {
            //Invert direction because scroll wheel is funny
            if (!ModEntry.IConfig.InvertScrollWheelDirection)
                direction = direction > 0 ? -Math.Abs(direction) : Math.Abs(direction);

            int _lastOffset = Offset;
            if (direction > 0 && canScrollDown)
                offset++;
            else if (direction < 0 && canScrollUp)
                offset--;
            if (Offset != _lastOffset) 
                RenderItems();

            if (canScroll)
                Game1.playSound("smallSelect");
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (trashCan is not null && trashCan.containsPoint(x, y) && (heldItem?.canBeTrashed() ?? false))
            {
                Utility.trashItem(heldItem);
                heldItem = null;
                if (playSound)
                    Game1.playSound("trashcan");
                return;
            }

            if (upArrow.containsPoint(x, y))
                receiveScrollWheelAction(1);
            if (downArrow.containsPoint(x, y))
                receiveScrollWheelAction(-1);
            if (upperRightCloseButton.containsPoint(x, y))
                exitThisMenu(playSound);

            if (itemsMenu.isWithinBounds(x, y))
            {
                if (heldItem is not null)
                {
                    PushItemToBin(heldItem); //This should honestly never be reached
                    if (playSound)
                        Game1.playSound("Ship");
                    heldItem = null;
                    return;
                }

                int slot = getSlotIndexAt(x, y, false);
                if (slot == -1)
                    return;

                heldItem = TakeItemAt(slot, -1, false);
                if (playSound)
                    Game1.playSound("dwoop");
                if (isShift)
                    heldItem = Game1.player.addItemToInventory(heldItem);
                return;
            }
            if (inventory.isWithinBounds(x, y))
            {
                int slot = getSlotIndexAt(x, y, true);
                if (slot == -1)
                    return;

                if (heldItem is not null)
                {
                    heldItem = TryPlaceItemAt(heldItem, slot, true);
                    if (playSound)
                        Game1.playSound("backpackIN");
                    return;
                }

                Item item = TakeItemAt(slot, -1, true)!;
                if (item is null)
                    return;
                if (!Utility.highlightShippableObjects(item))
                {
                    TryPlaceItemAt(item, slot, true);
                    return;
                }
                PushItemToBin(item);
                if (playSound)
                    Game1.playSound("Ship");
                return;
            }
        }

        public override void performHoverAction(int x, int y)
        {
            HoveredItem = null;
            base.performHoverAction(x, y);
            upArrow.tryHover(x, y, .25f);
            downArrow.tryHover(x, y, .25f);
            upperRightCloseButton.tryHover(x, y, .25f);
            Item i = itemsMenu.hover(x, y, heldItem);
            if (i != null && i != HoveredItem && Utility.highlightShippableObjects(i))
            {
                HoveredItem = i;
                return;
            }

            i = inventory.hover(x, y, heldItem);
            if (i != null && i != HoveredItem && Utility.highlightShippableObjects(i))
            {
                HoveredItem = i;
                return;
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);
            if (canScroll && !updatedBounds)
            {
                upperRightCloseButton.bounds = new Rectangle(upArrow.bounds.X + 48, upArrow.bounds.Y - 68, upperRightCloseButton.bounds.Width, upperRightCloseButton.bounds.Height);
                updatedBounds = true;
                return;
            }
            else if (!canScroll && updatedBounds)
            {
                upperRightCloseButton.bounds = new Rectangle(upArrow.bounds.X + 4, upArrow.bounds.Y - 68, upperRightCloseButton.bounds.Width, upperRightCloseButton.bounds.Height);
                updatedBounds = false;
            }
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.6f);
            draw(b, false, false);

            //These three things just to have the little backpack icon...
            //I honestly don't know why I keep insisting on adding this back everytime, but here it is still
            b.Draw(Game1.mouseCursors, new Vector2((xPositionOnScreen - 64), (yPositionOnScreen + height / 2 + 64 + 16)), new Rectangle?(new Rectangle(16, 368, 12, 16)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            b.Draw(Game1.mouseCursors, new Vector2((xPositionOnScreen - 64), (yPositionOnScreen + height / 2 + 64 - 16)), new Rectangle?(new Rectangle(21, 368, 11, 16)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            b.Draw(Game1.mouseCursors, new Vector2((xPositionOnScreen - 40), (yPositionOnScreen + height / 2 + 64 - 44)), new Rectangle?(new Rectangle(4, 372, 8, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

            Game1.drawDialogueBox(itemsMenu.xPositionOnScreen - borderWidth - spaceToClearSideBorder, itemsMenu.yPositionOnScreen - borderWidth - spaceToClearTopBorder, itemsMenu.width + (canScrollUp || canScrollDown ? 11 * Game1.pixelZoom : 0) + (borderWidth * 2) + (spaceToClearSideBorder * 2), itemsMenu.height + spaceToClearTopBorder + (borderWidth * 2), false, true);
            itemsMenu.draw(b);
            if (canScrollUp) 
                upArrow.draw(b);
            if (canScrollDown) 
                downArrow.draw(b);
            upperRightCloseButton.draw(b);
            drawTotalValue(b);

            if (HoveredItem != null)
                drawToolTip(b, HoveredItem.getDescription(), HoveredItem.DisplayName, HoveredItem, moneyAmountToShowAtBottom: getPriceOfItem(HoveredItem));

            heldItem?.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);

            Game1.mouseCursorTransparency = 1f;
            drawMouse(b);
        }

        private void drawTotalValue(SpriteBatch b)
        {
            if (farm.getShippingBin(Game1.player).Count <= 0 || !ModEntry.IConfig.ShowTotalValueBanner)
                return;

            int value = 0;
            string text = ModEntry.IHelper.Translation.Get("Menu.TotalValue");

            for (int i = 0; i < farm.getShippingBin(Game1.player).Count; i++)
                value += getPriceOfItem(farm.getShippingBin(Game1.player)[i]);

            if (value <= 0) return;

            text += $"{value}";

            SpriteText.drawStringWithScrollCenteredAt(b, text, upperRightCloseButton.bounds.X - (itemsMenu.width / 2), upperRightCloseButton.bounds.Y - 10);
        }

        private void broadCastToMultiplayer() => ModEntry.IHelper.Multiplayer.SendMessage("", "reloadItemsInBin", new[] { ModEntry.IHelper.ModRegistry.ModID });

        #region Component Handling
        /// <summary>
        /// Set the currently snapped component to the default (First slot in the bin's inventory)
        /// </summary>
        public override void snapToDefaultClickableComponent() => setCurrentlySnappedComponentTo(BaseIdPlayerInventory);

        /// <summary>
        /// Set the currently snapped component to the component with a given id if possible
        /// </summary>
        /// <param name="id">The id of the component</param>
        public override void setCurrentlySnappedComponentTo(int id)
        {
            if (getComponentWithId(id) is ClickableComponent c)
            {
                currentlySnappedComponent = c;
                snapCursorToCurrentSnappedComponent();
                Game1.playSound("smallSelect");
            }
        }

        /// <summary>
        /// Get the number of rows for an inventory
        /// </summary>
        /// <param name="player">Whether to get the number of rows for the player or the shipping bin</param>
        /// <returns>The number of rows for the requested inventory</returns>
        private int getRowCount(bool player) => (player ? inventory.inventory.Count : itemsMenu.inventory.Count) / RowLength;

        #region Neighboring Ids
        /// <summary>
        /// Get the id for the element above the current element
        /// </summary>
        /// <param name="rowIndex">The row index of the current element</param>
        /// <param name="columnIndex">The column index of the current element</param>
        /// <param name="player">Whether the id is for the player inventory or the shipping bin's inventory</param>
        /// <returns>An id for the neighbor above the current element</returns>
        private int getUpNeighborId(int rowIndex, int columnIndex, bool player)
        {
            if (player)
            {
                if (rowIndex == 0)
                    return BaseIdBinInventory + (RowLength * (getRowCount(false) - 1)) + columnIndex;
                return BaseIdPlayerInventory + (RowLength * rowIndex) - RowLength + columnIndex;
            }
            if (rowIndex == 0)
                return NoId;
            return BaseIdBinInventory + (RowLength * rowIndex) - RowLength + columnIndex;
        }

        /// <summary>
        /// Get the id for the element below the current element
        /// </summary>
        /// <param name="rowIndex">The row index of the current element</param>
        /// <param name="columnIndex">The column index of the current element</param>
        /// <param name="player">Whether the id is for the player inventory or the shipping bin's inventory</param>
        /// <returns>An id for the neighbor below the current element</returns>
        private int getDownNeighborId(int rowIndex, int columnIndex, bool player)
        {
            if (player)
            {
                if (rowIndex == getRowCount(true) - 1)
                    return NoId;
                return BaseIdPlayerInventory + (RowLength * rowIndex) + RowLength + columnIndex;
            }
            if (rowIndex == getRowCount(false) - 1)
                return BaseIdPlayerInventory + columnIndex;
            return BaseIdBinInventory + (RowLength * rowIndex) + RowLength + columnIndex;
        }

        /// <summary>
        /// Get the id for the element to the right of the current element
        /// </summary>
        /// <param name="rowIndex">The row index of the current element</param>
        /// <param name="columnIndex">The column index of the current element</param>
        /// <param name="player">Whether the id is for the player inventory or the shipping bin's inventory</param>
        /// <returns>An id for the neighbor to the right of the current element</returns>
        private int getRightNeighborId(int rowIndex, int columnIndex, bool player)
        {
            if (player)
            {
                if (columnIndex == RowLength - 1)
                {
                    if (rowIndex == 0)
                        return TrashCanId;
                    if (rowIndex == getRowCount(player) - 1)
                        return NoId;
                }
                return BaseIdPlayerInventory + (RowLength * rowIndex) + columnIndex + 1;
            }
            if (columnIndex == RowLength - 1)
            {
                if (rowIndex == 0)
                    return ArrowUpId;
                if (rowIndex == getRowCount(false) - 1)
                    return ArrowDownId;
            }
            return BaseIdBinInventory + (RowLength * rowIndex) + columnIndex + 1;
        }

        /// <summary>
        /// Get the id for the element to the left of the current element
        /// </summary>
        /// <param name="rowIndex">The row index of the current element</param>
        /// <param name="columnIndex">The column index of the current element</param>
        /// <param name="player">Whether the id is for the player inventory or the shipping bin's inventory</param>
        /// <returns>An id for the neighbor to the left of the current element</returns>
        private int getLeftNeighborId(int rowIndex, int columnIndex, bool player)
        {
            if (player)
            {
                if (columnIndex == 0)
                {
                    if (rowIndex == 0)
                        return ArrowDownId;
                    if (rowIndex == 1)
                        return TrashCanId;
                }
                return BaseIdPlayerInventory + (RowLength * rowIndex) + columnIndex - 1;
            }
            if (columnIndex == 0)
            {
                if (rowIndex == 0)
                    return NoId;
                if (rowIndex == 1)
                    return ArrowUpId;
            }
            return BaseIdBinInventory + (RowLength * rowIndex) + columnIndex - 1;
        }
        #endregion

        /// <summary>
        /// Assign the ids for all elements of a row;
        /// </summary>
        /// <param name="rowIndex">The row index of the current row</param>
        /// <param name="player">Whether the ids are for the player inventory or the shipping bin's inventory</param>
        private void assignRowIds(int rowIndex, bool player)
        {
            int baseId = player ? BaseIdPlayerInventory : BaseIdBinInventory;
            for (int i = 0; i < RowLength; i++)
            {
                int index = i + (RowLength * rowIndex);

                int upId = getUpNeighborId(rowIndex, i, player);
                int downId = getDownNeighborId(rowIndex, i, player);
                int rightId = getRightNeighborId(rowIndex, i, player);
                int leftId = getLeftNeighborId(rowIndex, i, player);

                assignId(index, baseId + index, player, Neighbor.None);
                assignId(index, upId, player, Neighbor.Up);
                assignId(index, downId, player, Neighbor.Down);
                assignId(index, rightId, player, Neighbor.Right);
                assignId(index, leftId, player, Neighbor.Left);
            }
        }

        /// <summary>
        /// Assign an id to a slot at a specified index or it's neighbor
        /// </summary>
        /// <param name="index">The index of the current slot</param>
        /// <param name="id">The id to assign</param>
        /// <param name="player">Whether the id is for the players inventory of the shipping bin's inventory</param>
        /// <param name="neighbor">The neighbor side to which to assign the id or <see cref="Neighbor.None"/> for the current slot</param>
        private void assignId(int index, int id, bool player, Neighbor neighbor)
        {
            _ = neighbor switch
            {
                Neighbor.None => player ? inventory.inventory[index].myID = id : itemsMenu.inventory[index].myID = id,
                Neighbor.Left => player ? inventory.inventory[index].leftNeighborID = id : itemsMenu.inventory[index].leftNeighborID = id,
                Neighbor.Right => player ? inventory.inventory[index].rightNeighborID = id : itemsMenu.inventory[index].rightNeighborID = id,
                Neighbor.Up => player ? inventory.inventory[index].upNeighborID = id : itemsMenu.inventory[index].upNeighborID = id,
                Neighbor.Down => player ? inventory.inventory[index].downNeighborID = id : itemsMenu.inventory[index].downNeighborID = id,
                _ => -1
            };
        }

        /// <summary>
        /// Create all the clickable elements and assign the id's required for controller support
        /// </summary>
        private void loadViewComponents()
        {
            itemsMenu = new(xPositionOnScreen + 32, yPositionOnScreen, false, itemsInView);
            upArrow = new(new(itemsMenu.xPositionOnScreen + itemsMenu.width + 12, itemsMenu.yPositionOnScreen - 6, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom), Game1.mouseCursors, new(421, 459, 11, 12), Game1.pixelZoom)
            {
                name = "previous",
                myID = ArrowUpId,
                upNeighborID = ExitId,
                downNeighborID = ArrowDownId,
                rightNeighborID = BaseIdBinInventory + RowLength,
                leftNeighborID = BaseIdBinInventory + RowLength - 1
            };
            downArrow = new(new(upArrow.bounds.X, upArrow.bounds.Y + itemsMenu.height - upArrow.bounds.Height, upArrow.bounds.Width, upArrow.bounds.Height), Game1.mouseCursors, new(421, 472, 11, 12), Game1.pixelZoom)
            {
                name = "next",
                myID = ArrowDownId,
                upNeighborID = ArrowUpId,
                downNeighborID = TrashCanId,
                rightNeighborID = BaseIdPlayerInventory,
                leftNeighborID = BaseIdBinInventory + (RowLength * getRowCount(false)) - 1
            };
            upperRightCloseButton = new(new(upArrow.bounds.X + 4, upArrow.bounds.Y - 68, 12 * Game1.pixelZoom, 12 * Game1.pixelZoom), Game1.mouseCursors, new(337, 494, 12, 12), Game1.pixelZoom)
            {
                name = "close",
                myID = ExitId,
                upNeighborID = NoId,
                downNeighborID = ArrowUpId,
                rightNeighborID = NoId,
                leftNeighborID = ArrowUpId,
            };
            trashCan = new(new(xPositionOnScreen + width + 4, yPositionOnScreen + height - 192 - 32 - borderWidth - 104, 64, 104), Game1.mouseCursors, new(564 + Game1.player.trashCanLevel * 18, 102, 18, 26), 4f)
            {
                name = "trash",
                myID = TrashCanId,
                upNeighborID = ArrowDownId,
                downNeighborID = NoId,
                rightNeighborID = BaseIdPlayerInventory + RowLength,
                leftNeighborID = BaseIdPlayerInventory + RowLength - 1
            };

            for (int i = 0; i < getRowCount(true); i++)
                assignRowIds(i, true);
            for (int i = 0; i < getRowCount(false); i++)
                assignRowIds(i, false);
        }

        /// <summary>
        /// Retrieve a component with a given id
        /// </summary>
        /// <param name="id">The id of the component to get</param>
        /// <returns>The component with the specified id, or null if it could not be found</returns>
        private ClickableComponent? getComponentWithId(int id)
        {
            if (id == NoId)
                return null;

            var component = id switch
            {
                ExitId => upperRightCloseButton,
                ArrowUpId => upArrow,
                ArrowDownId => downArrow,
                TrashCanId => trashCan,
                _ => null
            };

            if (component is not null)
                return component;

            for (int i = 0; i < inventory.inventory.Count; i++)
                if (inventory.inventory[i].myID == id)
                    return inventory.inventory[i];
            for (int i = 0; i < itemsMenu.inventory.Count; i++)
                if (itemsMenu.inventory[i].myID == id)
                    return itemsMenu.inventory[i];
            return null;
        }

        /// <summary>
        /// Snap the mouse to the next element in a given direction
        /// </summary>
        /// <param name="currentId">The id of the current component</param>
        /// <param name="direction">The direction in which to snap the cursor if possible</param>
        private void snapToNextClickableComponent(int currentId, Direction direction)
        {
            var component = getComponentWithId(currentId);
            if (component is null)
            {
                snapToDefaultClickableComponent();
                return;
            }
            int nextId = direction switch
            {
                Direction.Up => component.upNeighborID,
                Direction.Down => component.downNeighborID,
                Direction.Left => component.leftNeighborID,
                Direction.Right => component.rightNeighborID,
                _ => NoId
            };
            if (nextId == ArrowUpId && !canScrollUp || nextId == ArrowDownId && !canScrollDown)
            {
                snapToNextClickableComponent(nextId, direction);
                return;
            }
            setCurrentlySnappedComponentTo(nextId);
        }

        private int getSlotIndexAt(int x, int y, bool player)
        {
            if (player)
            {
                for (int i = 0; i < inventory.inventory.Count; i++)
                    if (inventory.inventory[i].bounds.Contains(x, y) && i < Game1.player.MaxItems)
                        return i;
                return -1;
            }
            for (int i = 0; i < itemsMenu.inventory.Count; i++)
                if (itemsMenu.inventory[i].bounds.Contains(x, y))
                    return i;
            return -1;
        }
        #endregion

        #region Item Handling
        /// <summary>
        /// (Re)load the items in view
        /// </summary>
        public void RenderItems() => itemsMenu.actualInventory = itemsInView;

        /// <summary>
        /// Get the shipping bin's items at an offset (if applicable) and return the items the maximum allowed items (see <see cref="maxItemsPerPage"/>)
        /// </summary>
        /// <param name="offset">The offset in rows</param>
        /// <returns>The items past the current offset in the shipping bin's item list</returns>
        private List<Item> getItemsInView(int offset)
        {
            
            farm.getShippingBin(Game1.player).RemoveEmptySlots();
            return new(actuallItems.Skip(maxItemsPerPage / 3 * offset).Take(maxItemsPerPage));
        }

        /// <summary>
        /// Get the maximum available stack size of an item if it's less than the requested stack amount
        /// </summary>
        /// <param name="stack">The requested stack amount</param>
        /// <param name="i">The item which holds the available stack</param>
        /// <returns>The largest available stack</returns>
        private int getMaxStack(int stack, Item i) => i.Stack < stack || stack == -1 ? i.Stack : stack;

        /// <summary>
        /// Get a stack of items from a slot in either the shipping bin's inventory or the players
        /// </summary>
        /// <remarks>
        /// This method will automatically remove the item stack from the inventory and even remove it from the inventory if it's entire stack is retrieved
        /// </remarks>
        /// <param name="index">The index of the item to obtain</param>
        /// <param name="stack">The requested stack amount</param>
        /// <param name="player">Whether to take from the players inventory or the shipping bin's</param>
        /// <returns>The item from the slot or null if it could not be retrieved</returns>
        private Item? TakeItemAt(int index, int stack, bool player)
        {
            IList<Item> items = player ? inventory.actualInventory : itemsInView;

            if (index >= items.Count || index < 0)
            {
                RenderItems();
                broadCastToMultiplayer();
                return null;
            }
            Item? i = items[index];
            if (i is null)
            {
                RenderItems();
                broadCastToMultiplayer();
                return null;
            }
            int takeStack = getMaxStack(stack, i);
            Item copy = i.getOne();
            copy.Stack = takeStack;
            if ((items[index].Stack -= takeStack) <= 0)
            {
                items[index] = null;
                if (!player)
                    actuallItems[index + (offset * RowLength)] = null;
            }

            RenderItems();
            broadCastToMultiplayer();
            return copy;
        }

        /// <summary>
        /// Try to place an item at a slot in either the shipping bin's inventory or the players
        /// </summary>
        /// <param name="i">The item to place</param>
        /// <param name="index">The index where the item should be placed</param>
        /// <param name="player">Whether to add to the players inventory or the shipping bin's</param>
        /// <returns>
        /// <list type="bullet">
        /// <item><b>null</b>: The item to place was null, or the item was placed successfully</item>
        /// <item><b><see cref="Item"/></b>: The placed item replaced an item or, the placed item still has remaining stack after placement</item>
        /// </list> 
        /// </returns>
        private Item? TryPlaceItemAt(Item? i, int index, bool player)
        {
            if (i is null)
            {
                RenderItems();
                broadCastToMultiplayer();
                return null;
            }
            IList<Item> items = player ? inventory.actualInventory : itemsInView;

            if (index >= items.Count || index < 0)
            {
                RenderItems();
                broadCastToMultiplayer();
                return i;
            }
            Item? currentOccupant = items[index];
            if (currentOccupant is null)
            {
                items[index] = i;
                RenderItems();
                broadCastToMultiplayer();
                return null;
            }
            if (currentOccupant.QualifiedItemId != i.QualifiedItemId || currentOccupant.Quality != i.Quality)
            {
                var old = currentOccupant.getOne();
                old.Stack = currentOccupant.Stack;
                old.Quality = currentOccupant.Quality;
                items[index] = i;
                RenderItems();
                broadCastToMultiplayer();
                return old;
            }
            if (currentOccupant.Stack + i.Stack > currentOccupant.maximumStackSize())
            {
                int remainder = currentOccupant.Stack + i.Stack - currentOccupant.maximumStackSize();
                items[index].Stack += i.Stack;
                i.Stack = remainder;
                RenderItems();
                broadCastToMultiplayer();
                return i;
            }
            items[index].Stack += i.Stack;
            RenderItems();
            broadCastToMultiplayer();
            return null;
        }

        private void PushItemToBin(Item i)
        {
            if (i is null)
            {
                RenderItems();
                broadCastToMultiplayer();
                return;
            }

            foreach (var item in actuallItems)
            {
                if (item?.canStackWith(i) ?? false)
                {
                    int remainder = actuallItems[actuallItems.IndexOf(item)].addToStack(i);
                    i.Stack = remainder;
                    if (i.Stack <= 0)
                    {
                        RenderItems();
                        broadCastToMultiplayer();
                        return;
                    }
                }
            }
            actuallItems.Add(i);
            RenderItems();
            broadCastToMultiplayer();
        }

        /// <summary>
        /// Get the sell price of an item for calculating shipping bin inventory value
        /// </summary>
        /// <param name="i">The item to check the sell price of</param>
        /// <returns>The price of an item when sold, or 0 if the item is null</returns>
        private int getPriceOfItem(Item? i)
        {
            if (i is null)
                return 0;
            return i.sellToStorePrice(Game1.player.UniqueMultiplayerID) * i.Stack;
        }

        /// <summary>
        /// Get the stack to take based on user keyboard input
        /// </summary>
        /// <returns>The stack size to take</returns>
        private int getStack() => isCtrl ? 25 : (isShift ? 5 : 1);
        #endregion
    }
}
