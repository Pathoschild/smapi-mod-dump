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
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BetterShipping
{
    internal class BinMenuOverride : MenuWithInventory
    {
        private const int BaseIdPlayerInventory = 36000;
        private const int BaseIdBinInventory = 69500;

        private Item _hoverItem;
        private InventoryMenu _itemsMenu;
        private ClickableTextureComponent _upArrow;
        private ClickableTextureComponent _downArrow;
        private Farm _farm;
        private IList<Item> _itemsInView;
        private bool _updatedBounds;
        private bool _finishedInitializing = false;
        public int Offset;

        private int _maxOffset => (int)Math.Ceiling(((double)_actuallItems.Count - _maxItemsPerPage) / ((double)_maxItemsPerPage / 3));
        private bool _canScroll => _canScrollUp || _canScrollDown;
        private bool _canScrollUp => Offset > 0;
        private bool _canScrollDown => Offset < _maxOffset;
        private IList<Item> _actuallItems => _farm.getShippingBin(Game1.player);

        private readonly IModHelper _helper;
        private readonly IMonitor _monitor;
        private readonly int _maxItemsPerPage = Chest.capacity;

        public BinMenuOverride(IModHelper helper, IMonitor monitor, int offset = 0) : base(new InventoryMenu.highlightThisItem(Utility.highlightShippableObjects), true, true, menuOffsetHack: 64)
        {
            Offset = offset < 0 ? 0 : offset;
            _farm = Game1.getFarm();
            _helper = helper;
            _monitor = monitor;
            _itemsInView = new List<Item>(_actuallItems.Take(_maxItemsPerPage));
            if (Offset > 0) _itemsInView = getItemsFromOffset();

            inventory.showGrayedOutSlots = true;
            okButton = null;

            loadViewComponents();
            loadItemsInView();
        }


        public override void snapToDefaultClickableComponent() => currentlySnappedComponent = getComponentWithId(_itemsMenu.actualInventory.Count > 0 ? 69500 : 36000); /* ?? (_itemsMenu.actualInventory.Count > 0 ? _itemsMenu.inventory.FirstOrDefault(x => x.myID == 69500) : inventory.inventory.FirstOrDefault(x => x.myID == 36000)) // I'm sure this made sense to someone someday, but it doesn't to me now*/

        public override void setCurrentlySnappedComponentTo(int id)
        {
            currentlySnappedComponent = getComponentWithId(id);
            if (currentlySnappedComponent == null)
            {
                snapToDefaultClickableComponent();
                _monitor.Log($"Couldn't snap to component with id : {id}, Snapping to default", LogLevel.Warn);
            }
            Game1.playSound("smallSelect");
        }

        public override void setUpForGamePadMode()
        {
            snapToDefaultClickableComponent();
            snapCursorToCurrentSnappedComponent();
        }

        public override void receiveGamePadButton(Buttons b)
        {
            if (!_finishedInitializing) return;

            switch (b)
            {
                case Buttons.Back:
                case Buttons.B:
                case Buttons.Y:
                    exitMenu();
                    return;
                case Buttons.A:
                    if (currentlySnappedComponent != null)
                    {
                        if (currentlySnappedComponent.myID == 12977) receiveScrollWheelAction(-1);
                        else if (currentlySnappedComponent.myID == 12976) receiveScrollWheelAction(1);
                        else if (currentlySnappedComponent.myID == 12975) goto case Buttons.B;
                    }
                    break;
                case Buttons.LeftShoulder:
                    receiveScrollWheelAction(1);
                    break;
                case Buttons.RightShoulder:
                    receiveScrollWheelAction(-1);
                    break;
            }

            base.receiveGamePadButton(b);
        }

        public override void applyMovementKey(int direction)
        {
            if (currentlySnappedComponent == null) snapToDefaultClickableComponent();
            switch (direction)
            {
                case 0: //Up
                    if (currentlySnappedComponent.upNeighborID < 0) goto case 2003;
                    setCurrentlySnappedComponentTo(currentlySnappedComponent.upNeighborID);
                    snapCursorToCurrentSnappedComponent();
                    break;
                case 1: //Right
                    if (currentlySnappedComponent.rightNeighborID < 0) goto case 2003;
                    setCurrentlySnappedComponentTo(currentlySnappedComponent.rightNeighborID);
                    snapCursorToCurrentSnappedComponent();
                    break;
                case 2: //Down
                    if (currentlySnappedComponent.downNeighborID < 0) goto case 2003;
                    setCurrentlySnappedComponentTo(currentlySnappedComponent.downNeighborID);
                    snapCursorToCurrentSnappedComponent();
                    break;
                case 3: //Left
                    if (currentlySnappedComponent.leftNeighborID < 0) goto case 2003;
                    setCurrentlySnappedComponentTo(currentlySnappedComponent.leftNeighborID);
                    snapCursorToCurrentSnappedComponent();
                    break;
                case 2003:
                    snapToNextClickableComponent(currentlySnappedComponent.myID, direction);
                    snapCursorToCurrentSnappedComponent();
                    break;
                default:
                    base.applyMovementKey(direction);
                    break;
            }
        }

        //TODO : Resolve stacking issue's when holding down shift / ctrl \\Fixed (I think (Hell if I now what do I look like? a programmer, nah, just... fixed... ok...))
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (!_finishedInitializing) return;

            if (_itemsMenu.isWithinBounds(x, y))
            {
                var item = _itemsMenu.getItemAt(x, y);
                if (item == null || !Utility.highlightShippableObjects(item)) return;

                if (heldItem == null)
                {
                    heldItem = item.getOne();
                    if (heldItem != null)
                        selectStackFromBin(heldItem, Game1.player, getStack());
                    broadCastToMultiplayer();
                    Game1.playSound("dwop");
                    return;
                }
                else if (heldItem != null && heldItem.canStackWith(item))
                {
                    selectStackFromBin(item, Game1.player, getStack());
                    heldItem.Stack++;
                    broadCastToMultiplayer();
                    Game1.playSound("dwop");
                    return;
                }
                return;
            }
            else if (inventory.isWithinBounds(x, y))
            {
                var item = inventory.getItemAt(x, y);
                if (item == null || !Utility.highlightShippableObjects(item)) return;
                if (item != null && heldItem == null)
                {
                    selectSingleFromInventory(item, Game1.player);
                    addItemToBin(item.getOne(), Game1.player);
                    loadItemsInView();
                    broadCastToMultiplayer();
                    Game1.playSound("Ship");
                    return;
                }
                else if (heldItem != null)
                {
                    if (Game1.player.Items.Where(x => x != null).Count() >= Game1.player.maxItems.Value) return;
                    if (Game1.player.getIndexOfInventoryItem(heldItem) >= 0)
                        Game1.player.addItemToInventory(heldItem.getOne(), Game1.player.getIndexOfInventoryItem(heldItem));
                    else
                        Game1.player.addItemToInventory(heldItem.getOne());
                    heldItem.Stack--;
                    if (heldItem.Stack <= 0)
                        heldItem = null;
                    Game1.playSound("backpackIN");
                    return;
                }
                return;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (!_finishedInitializing) return;

            bool shiftKeyDown = Game1.oldKBState.IsKeyDown(Keys.LeftShift) || Game1.oldKBState.IsKeyDown(Keys.RightShift);

            if (trashCan != null && trashCan.containsPoint(x, y) && heldItem != null && heldItem.canBeTrashed())
            {
                Utility.trashItem(heldItem);
                heldItem = null;
                Game1.playSound("trashcan");
            }

            if (_upArrow.containsPoint(x, y)) receiveScrollWheelAction(1);
            if (_downArrow.containsPoint(x, y)) receiveScrollWheelAction(-1);
            if (upperRightCloseButton.containsPoint(x, y)) exitMenu();

            if (heldItem != null)
            {
                if (_itemsMenu.isWithinBounds(x, y) && Utility.highlightShippableObjects(heldItem))
                {
                    addItemToBin(heldItem, Game1.player);
                    heldItem = null;
                    loadItemsInView();
                    broadCastToMultiplayer();
                    Game1.playSound("Ship");
                    return;
                }
                else if (inventory.isWithinBounds(x, y))
                {
                    if (Game1.player.Items.Where(x => x != null).Count() >= Game1.player.maxItems.Value) return;
                    if (Game1.player.getIndexOfInventoryItem(heldItem) >= 0)
                        Game1.player.addItemToInventory(heldItem, Game1.player.getIndexOfInventoryItem(heldItem));
                    else
                        Game1.player.addItemToInventory(heldItem);
                    heldItem = null;
                    Game1.playSound("backpackIN");
                    return;
                }
            }

            for (int i = 0; i < _itemsMenu.inventory.Count; i++)
            {
                if (_itemsMenu.inventory[i].containsPoint(x, y))
                {
                    Item? item = _itemsMenu.actualInventory.Count <= i ? null : _itemsMenu.actualInventory.ElementAt(i);

                    if (!Utility.highlightShippableObjects(item)) return;

                    if (heldItem == null && item != null)
                    {
                        heldItem = item;
                        selectItemFromBin(heldItem, Game1.player);
                        broadCastToMultiplayer();
                        Game1.playSound("dwop");
                        if (shiftKeyDown && Game1.player.Items.Where(x => x != null).Count() < Game1.player.maxItems.Value)
                        {
                            if (Game1.player.getIndexOfInventoryItem(heldItem) >= 0)
                                Game1.player.addItemToInventory(heldItem, Game1.player.getIndexOfInventoryItem(heldItem));
                            else
                                Game1.player.addItemToInventory(heldItem);
                            heldItem = null;
                        }
                        break;
                    }
                    else if (heldItem != null && item != null)
                    {
                        if (item.canStackWith(heldItem))
                        {
                            var id = _farm.getShippingBin(Game1.player).IndexOf(item);
                            _farm.getShippingBin(Game1.player)[id].addToStack(heldItem);
                            heldItem = null;
                            broadCastToMultiplayer();
                            Game1.playSound("Ship");
                            break;
                        }
                        addItemToBin(heldItem, Game1.player);
                        heldItem = item;
                        selectItemFromBin(heldItem, Game1.player);
                        broadCastToMultiplayer();
                        Game1.playSound("Ship");
                        break;
                    }
                    else if (heldItem != null && item == null)
                    {
                        addItemToBin(heldItem, Game1.player);
                        heldItem = null;
                        loadItemsInView();
                        broadCastToMultiplayer();
                        Game1.playSound("Ship");
                        break;
                    }
                    else break;
                }
            }

            for (int i = 0; i < inventory.inventory.Count; i++)
            {
                if (inventory.inventory[i].containsPoint(x, y))
                {
                    if (i >= Game1.player.maxItems.Value) return;
                    Item? item = inventory.actualInventory.Count <= i ? null : inventory.actualInventory.ElementAt(i);

                    if (item != null && heldItem == null) goto Inventory1;
                    else if (item == null && heldItem != null) goto Inventory2;
                    else if (item != null && heldItem != null)
                    {
                        if (!item.canStackWith(heldItem)) goto Inventory1;
                        else goto Inventory2;
                    }
                    else break;

                    Inventory1:
                    if (!Utility.highlightShippableObjects(item)) return;
                    selectItemFromInventory(item, Game1.player);
                    addItemToBin(item, Game1.player);
                    loadItemsInView();
                    broadCastToMultiplayer();
                    Game1.playSound("Ship");
                    break;

                    Inventory2:
                    if (Game1.player.getIndexOfInventoryItem(heldItem) >= 0)
                        Game1.player.addItemToInventory(heldItem, Game1.player.getIndexOfInventoryItem(heldItem));
                    else
                        Game1.player.addItemToInventory(heldItem);
                    heldItem = null;
                    Game1.playSound("backpackIN");
                    break;
                }
            }
        }

        public override void receiveScrollWheelAction(int direction)
        {
            if (!_finishedInitializing) return;
            if (_canScroll) Game1.playSound("smallSelect");

            //Invert direction because scroll wheel is funny
            if (direction > 0) direction = -Math.Abs(direction);
            else direction = Math.Abs(direction);

            int _lastOffset = Offset;
            if (direction > 0 && _canScrollDown)
                Offset++;
            else if (direction < 0 && _canScrollUp)
                Offset--;
            if (Offset != _lastOffset) loadItemsInView();
        }

        public override void receiveKeyPress(Keys key)
        {
            if (!_finishedInitializing) return;

            bool isDownOrUp = key == Keys.Up || key == Keys.Down;
            bool isDelete = key == Keys.Delete;
            bool isEscape = key == Keys.Escape;
            bool isExit = isEscape || Game1.options.doesInputListContain(Game1.options.menuButton, key);

            if (isDelete && heldItem != null)
            {
                Utility.trashItem(heldItem);
                heldItem = null;
                Game1.playSound("trashcan");
            }
            else if (isExit)
                exitMenu();
            else if (isDownOrUp)
                receiveScrollWheelAction(key == Keys.Up ? 1 : -1);
            else if (Game1.options.gamepadControls)
                applyMovementKeys(key);

        }

        public override void performHoverAction(int x, int y)
        {
            _hoverItem = null;
            base.performHoverAction(x, y);
            _upArrow.tryHover(x, y, 0.25f);
            _downArrow.tryHover(x, y, 0.25f);
            upperRightCloseButton.tryHover(x, y, 0.25f);

            Item i = _itemsMenu.hover(x, y, heldItem);
            if (i != null && i != _hoverItem && Utility.highlightShippableObjects(i))
            {
                _hoverItem = i;
                return;
            }

            i = inventory.hover(x, y, heldItem);
            if (i != null && i != _hoverItem && Utility.highlightShippableObjects(i))
            {
                _hoverItem = i;
                return;
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);
            if (_canScroll && !_updatedBounds)
            {
                upperRightCloseButton.bounds = new Rectangle(_upArrow.bounds.X + 48, _upArrow.bounds.Y - 68, upperRightCloseButton.bounds.Width, upperRightCloseButton.bounds.Height);
                _updatedBounds = true;
                return;
            }
            else if (!_canScroll && _updatedBounds)
            {
                upperRightCloseButton.bounds = new Rectangle(_upArrow.bounds.X + 4, _upArrow.bounds.Y - 68, upperRightCloseButton.bounds.Width, upperRightCloseButton.bounds.Height);
                _updatedBounds = false;
            }
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.6f);
            draw(b, false, false);

            //These three things just to have the little backpack icon...
            b.Draw(Game1.mouseCursors, new Vector2((xPositionOnScreen - 64), (yPositionOnScreen + height / 2 + 64 + 16)), new Rectangle?(new Rectangle(16, 368, 12, 16)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            b.Draw(Game1.mouseCursors, new Vector2((xPositionOnScreen - 64), (yPositionOnScreen + height / 2 + 64 - 16)), new Rectangle?(new Rectangle(21, 368, 11, 16)), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            b.Draw(Game1.mouseCursors, new Vector2((xPositionOnScreen - 40), (yPositionOnScreen + height / 2 + 64 - 44)), new Rectangle?(new Rectangle(4, 372, 8, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

            Game1.drawDialogueBox(_itemsMenu.xPositionOnScreen - borderWidth - spaceToClearSideBorder, _itemsMenu.yPositionOnScreen - borderWidth - spaceToClearTopBorder, _itemsMenu.width + (_canScrollUp || _canScrollDown ? 11 * Game1.pixelZoom : 0) + (borderWidth * 2) + (spaceToClearSideBorder * 2), _itemsMenu.height + spaceToClearTopBorder + (borderWidth * 2), false, true);
            _itemsMenu.draw(b);
            if (_canScrollUp) _upArrow.draw(b);
            if (_canScrollDown) _downArrow.draw(b);
            upperRightCloseButton.draw(b);
            drawTotalValue(b);

            if (_hoverItem != null)
                drawToolTip(b, _hoverItem.getDescription(), _hoverItem.DisplayName, _hoverItem, moneyAmountToShowAtBottom: getPriceOfItem(_hoverItem));

            if (heldItem != null)
                heldItem.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);

            Game1.mouseCursorTransparency = 1f;
            drawMouse(b);
            if (!_finishedInitializing) _finishedInitializing = true;
        }

        private void drawTotalValue(SpriteBatch b)
        {
            if (_farm.getShippingBin(Game1.player).Count <= 0) return;

            int value = 0;
            string text = "Total value : ";

            for (int i = 0; i < _farm.getShippingBin(Game1.player).Count; i++)
                value += getPriceOfItem(_farm.getShippingBin(Game1.player)[i]);

            if (value <= 0) return;

            text += $"{value}";

            SpriteText.drawStringWithScrollCenteredAt(b, text, upperRightCloseButton.bounds.X - (_itemsMenu.width / 2), upperRightCloseButton.bounds.Y - 10);
        }

        private void loadViewComponents()
        {
            initializeUpperRightCloseButton();

            _itemsMenu = new InventoryMenu(xPositionOnScreen + 32, yPositionOnScreen, false, _itemsInView);
            _upArrow = new ClickableTextureComponent(new Rectangle(_itemsMenu.xPositionOnScreen + _itemsMenu.width + 12, _itemsMenu.yPositionOnScreen - 6, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), Game1.pixelZoom);
            _upArrow.name = "previous";
            _downArrow = new ClickableTextureComponent(new Rectangle(_upArrow.bounds.X, _upArrow.bounds.Y + _itemsMenu.height - _upArrow.bounds.Height, _upArrow.bounds.Width, _upArrow.bounds.Height), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), Game1.pixelZoom);
            _downArrow.name = "next";

            upperRightCloseButton.bounds = new Rectangle(_upArrow.bounds.X + 4, _upArrow.bounds.Y - 68, upperRightCloseButton.bounds.Width, upperRightCloseButton.bounds.Height);

            const int rowLength = 12;
            int colLength = inventory.inventory.Count / rowLength;

            for (int i = 0; i < colLength; i++)
            {
                for (int j = 0; j < rowLength; j++)
                {
                    int index = j + (rowLength * i);
                    inventory.inventory[index].myID = BaseIdPlayerInventory + index;

                    if (i != 0) inventory.inventory[index].upNeighborID = BaseIdPlayerInventory + index - rowLength;
                    else inventory.inventory[index].upNeighborID = -1;

                    if (i != (colLength - 1)) inventory.inventory[index].downNeighborID = BaseIdPlayerInventory + index + rowLength;
                    else inventory.inventory[index].downNeighborID = -1;

                    if (j != 0) inventory.inventory[index].leftNeighborID = BaseIdPlayerInventory + index - 1;
                    else inventory.inventory[index].leftNeighborID = -1;

                    if (j != rowLength - 1) inventory.inventory[index].rightNeighborID = BaseIdPlayerInventory + index + 1;
                    else inventory.inventory[index].rightNeighborID = -1;
                }
            }

            colLength = _itemsMenu.inventory.Count / rowLength;

            for (int i = 0; i < colLength; i++)
            {
                for (int j = 0; j < rowLength; j++)
                {
                    int index = j + (rowLength * i);
                    _itemsMenu.inventory[index].myID = BaseIdBinInventory + index;

                    if (i != 0) _itemsMenu.inventory[index].upNeighborID = BaseIdBinInventory + index - rowLength;
                    else _itemsMenu.inventory[index].upNeighborID = -1;

                    if (i != (colLength - 1)) _itemsMenu.inventory[index].downNeighborID = BaseIdBinInventory + index + rowLength;
                    else _itemsMenu.inventory[index].downNeighborID = -1;

                    if (j != 0) _itemsMenu.inventory[index].leftNeighborID = BaseIdBinInventory + index - 1;
                    else _itemsMenu.inventory[index].leftNeighborID = -1;

                    if (j != rowLength - 1) _itemsMenu.inventory[index].rightNeighborID = BaseIdBinInventory + index + 1;
                    else _itemsMenu.inventory[index].rightNeighborID = -1;

                    if (j == rowLength - 1 && i == 0)
                    {
                        _upArrow.leftNeighborID = _itemsMenu.inventory[index].myID;
                        _itemsMenu.inventory[index].rightNeighborID = 12976;
                    }
                    if (j == rowLength - 1 && i == (colLength - 1))
                    {
                        _downArrow.leftNeighborID = _itemsMenu.inventory[index].myID;
                        _itemsMenu.inventory[index].rightNeighborID = 12977;
                    }
                }
            }

            upperRightCloseButton.myID = 12975;
            upperRightCloseButton.downNeighborID = 12976;
            upperRightCloseButton.upNeighborID = upperRightCloseButton.rightNeighborID = upperRightCloseButton.leftNeighborID = -1;

            _upArrow.myID = 12976;
            _upArrow.upNeighborID = 12975;
            _upArrow.downNeighborID = 12977;
            _upArrow.rightNeighborID = BaseIdBinInventory + rowLength;

            _downArrow.myID = 12977;
            _downArrow.upNeighborID = 12976;
            _downArrow.rightNeighborID = BaseIdPlayerInventory;
            _downArrow.downNeighborID = -1;
        }

        public void loadItemsInView()
        {
            _itemsInView = getItemsFromOffset();
            _itemsMenu.actualInventory = _itemsInView;
        }

        private IList<Item> getItemsFromOffset() => new List<Item>(_actuallItems.Skip((_maxItemsPerPage / 3) * Offset).Take(_maxItemsPerPage));

        private void selectItemFromInventory(Item i, Farmer who)
        {
            if (i == null || who == null) return;
            who.removeItemFromInventory(i);
        }

        private void selectSingleFromInventory(Item i, Farmer who)
        {
            if (i == null || who == null) return;

            var index = who.getIndexOfInventoryItem(i);
            who.Items[index].Stack--;
            if (who.Items[index].Stack <= 0)
                who.removeItemFromInventory(i);
        }

        //I'm keeping this, if selectStackFromBin ever throws anything funny I don't want to understand it's getting yeeted and this gets re-implemented
        /*private void selectSingleFromBin(Item i, Farmer who)
        {
            if (i == null || who == null) return;

            var item = _farm.getShippingBin(who).FirstOrDefault(x => x.Stack != x.maximumStackSize() && x.canStackWith(i));
            if (item != null)
            {
                _farm.getShippingBin(who)[_farm.getShippingBin(who).IndexOf(item)].Stack--;
                if (item.Stack <= 0)
                    selectItemFromBin(item, who);
            }
        }*/

        private void selectStackFromBin(Item i, Farmer who, int stackSize)
        {
            if (i == null || who == null) return;

            var item = _farm.getShippingBin(who).FirstOrDefault(x => x.canStackWith(i) && (x as StardewValley.Object).Quality == (x as StardewValley.Object).Quality);
            if (item != null)
            {
                int itemStack = _farm.getShippingBin(who)[_farm.getShippingBin(who).IndexOf(item)].Stack;
                _farm.getShippingBin(who)[_farm.getShippingBin(who).IndexOf(item)].Stack -= stackSize < itemStack ? stackSize : itemStack;
                if (stackSize > 1)
                    heldItem.Stack += stackSize < itemStack ? stackSize - 1 : itemStack - 1;
                if (item.Stack <= 0)
                    selectItemFromBin(item, who);
            }
        }

        private void selectItemFromBin(Item i, Farmer who)
        {
            if (i == null || who == null) return;

            _farm.getShippingBin(who).Remove(i);
            loadItemsInView();
        }

        private void addItemToBin(Item i, Farmer who)
        {
            if (i == null || who == null) return;

            for (int id = 0; id < _farm.getShippingBin(who).Count; id++)
            {
                if (_farm.getShippingBin(who)[id].canStackWith(i) && _farm.getShippingBin(who)[id].Stack < _farm.getShippingBin(who)[id].maximumStackSize())
                {
                    var remaining = _farm.getShippingBin(who)[id].addToStack(i);
                    if (remaining > 0)
                    {
                        i.Stack = remaining;
                        _farm.getShippingBin(who).Add(i);
                    }
                    return;
                }
            }

            _farm.getShippingBin(who).Add(i);
        }

        private void snapToNextClickableComponent(int id, int direction)
        {
            var component = getComponentWithId(id);
            bool isPlayerInv = $"{id}".StartsWith("360");
            bool isItemInv = $"{id}".StartsWith("695");
            int currentRowIndex = (isPlayerInv ? (id - BaseIdPlayerInventory) : (id - BaseIdBinInventory)) % 12;

            switch (direction)
            {
                case 0: //Up
                    if (isPlayerInv)
                        setCurrentlySnappedComponentTo(BaseIdBinInventory + currentRowIndex + 24);
                    else if (isItemInv)
                        setCurrentlySnappedComponentTo(upperRightCloseButton.myID);
                    break;
                case 1: //Right
                    if (component.downNeighborID > 0)
                        setCurrentlySnappedComponentTo(id + 1);
                    else if (isItemInv)
                        setCurrentlySnappedComponentTo(BaseIdPlayerInventory);
                    break;
                case 2: //Down
                    if (isItemInv)
                        setCurrentlySnappedComponentTo(BaseIdPlayerInventory + currentRowIndex);
                    break;
                case 3: //Left
                    if (component.upNeighborID > 0)
                        setCurrentlySnappedComponentTo(id - 1);
                    else if (isPlayerInv)
                        setCurrentlySnappedComponentTo(BaseIdBinInventory + 35);
                    break;
            }
        }

        private void applyMovementKeys(Keys key)
        {
            if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
                applyMovementKey(0);
            else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
                applyMovementKey(1);
            else if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
                applyMovementKey(2);
            else if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
                applyMovementKey(3);
        }

        private void getClickableComponentList()
        {
            allClickableComponents = new List<ClickableComponent>();
            for (int i = 0; i < inventory.inventory.Count; i++)
                allClickableComponents.Add(inventory.inventory[i]);
            for (int i = 0; i < _itemsMenu.inventory.Count; i++)
                allClickableComponents.Add(_itemsMenu.inventory[i]);
            allClickableComponents.Add(_upArrow);
            allClickableComponents.Add(_downArrow);
            allClickableComponents.Add(upperRightCloseButton);
        }

        private ClickableComponent getComponentWithId(int id)
        {
            getClickableComponentList();
            for (int i = 0; i < allClickableComponents.Count; i++)
                if (allClickableComponents[i].myID == id || allClickableComponents[i].myAlternateID == id)
                    return allClickableComponents[i];
            return null;
        }

        private int getPriceOfItem(Item i)
        {
            if (i == null) return 0;

            int val;

            if (i is StardewValley.Object) val = (i as StardewValley.Object).sellToStorePrice();
            else val = i.salePrice();

            val *= i.Stack;
            return val;
        }

        private int getStack()
        {
            bool shiftKeyDown = Game1.oldKBState.IsKeyDown(Keys.LeftShift) || Game1.oldKBState.IsKeyDown(Keys.RightShift);
            bool ctrlKeyDown = Game1.oldKBState.IsKeyDown(Keys.LeftControl) || Game1.oldKBState.IsKeyDown(Keys.RightControl);

            if (!shiftKeyDown && !ctrlKeyDown) return 1;
            else if (shiftKeyDown && !ctrlKeyDown) return 5;
            else return 25;
        }

        private void broadCastToMultiplayer() => _helper.Multiplayer.SendMessage("", "reloadItemsInBin", new[] { _helper.ModRegistry.ModID });

        private void exitMenu()
        {
            Game1.playSound("bigDeSelect");
            if (heldItem != null)
                Game1.createItemDebris(heldItem, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
            exitThisMenu();
        }
    }
}
