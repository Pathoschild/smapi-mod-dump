/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TSlex/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Force.DeepCloner;
using ItemResearchSpawner.Components.UI.Helpers;
using ItemResearchSpawner.Models;
using ItemResearchSpawner.Models.Enums;
using ItemResearchSpawner.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace ItemResearchSpawner.Components.UI
{
    /**
        MIT License

        Copyright (c) 2018 CJBok

        Permission is hereby granted, free of charge, to any person obtaining a copy
        of this software and associated documentation files (the "Software"), to deal
        in the Software without restriction, including without limitation the rights
        to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        copies of the Software, and to permit persons to whom the Software is
        furnished to do so, subject to the following conditions:

        The above copyright notice and this permission notice shall be included in all
        copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
        AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
        SOFTWARE.
     **/
    internal class SpawnMenu : ItemGrabMenuWrapper
    {
        private readonly SpawnableItem[] _spawnableItems;
        private readonly IMonitor _monitor;
        private readonly IContentHelper _content;
        private readonly IModHelper _helper;

        private const int ItemsPerView = Chest.capacity;
        private const int ItemsPerRow = Chest.capacity / 3;

        private static bool IsAndroid => Constants.TargetPlatform == GamePlatform.Android;

        private ItemResearchArea _researchArea;
        private ItemQualitySelectorTab _qualitySelector;
        private ItemSortTab _itemSortTab;
        private ItemCategorySelectorTab _categorySelector;
        private ItemSearchBarTab _searchBarTab;
        private CashTab _cashTab;
        private ItemMoneyTooltip _moneyTooltip;

        private readonly List<ResearchableItem> _filteredItems = new List<ResearchableItem>();
        private readonly IList<Item> _itemsInView;

        private int _topRowIndex;
        private int _maxTopRowIndex;

        private bool _overDropdown;
        private bool _shiftPressed;
        private bool _dropdownExpanded;

        public Rectangle GrabMenuBounds => new Rectangle(ItemsToGrabMenu.xPositionOnScreen,
            ItemsToGrabMenu.yPositionOnScreen, ItemsToGrabMenu.width, ItemsToGrabMenu.height);

        public SpawnMenu(SpawnableItem[] spawnableItems, IContentHelper content, IModHelper helper,
            IMonitor monitor) : base(
            inventory: new List<Item>(),
            reverseGrab: false,
            showReceivingMenu: true,
            highlightFunction: item => true,
            behaviorOnItemGrab: (item, player) => { },
            behaviorOnItemSelectFunction: (item, player) => { },
            message: null,
            canBeExitedWithKey: true,
            showOrganizeButton: false,
            source: IsAndroid ? source_chest : source_none
        )
        {
            _monitor = monitor;
            _content = content;
            _helper = helper;

            _spawnableItems = spawnableItems;
            _itemsInView = ItemsToGrabMenu.actualInventory;

            ItemsToGrabMenu.highlightMethod = item =>
                !_dropdownExpanded &&
                (ModManager.Instance.ModMode == ModMode.Spawn ||
                 ModManager.Instance.GetItemPrice(item, true) <= Game1.player._money);

            drawBG = false; // disable to draw default ui over new menu
            behaviorOnItemGrab = OnItemGrab;

            InitializeComponents();
            UpdateView(true);

            ModManager.Instance.OnUpdateMenuView += OnUpdateMenuView;

            _categorySelector.OnDropdownToggle += OnDropdownToggle;
            _helper.Events.Input.ButtonsChanged += OnButtonChanged;
        }

        private void OnUpdateMenuView(bool rebuild)
        {
            UpdateView(rebuild);
        }

        protected override void cleanupBeforeExit()
        {
            if (_researchArea.ResearchItem != null)
            {
                DropItem(_researchArea.ReturnItem());
            }

            ModManager.Instance.OnUpdateMenuView -= OnUpdateMenuView;

            _categorySelector.OnDropdownToggle -= OnDropdownToggle;
            _helper.Events.Input.ButtonsChanged -= OnButtonChanged;

            _researchArea.PrepareToBeKilled();

            base.cleanupBeforeExit();
        }

        private void OnButtonChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (e.Pressed.Contains(SButton.LeftShift) || e.Pressed.Contains(SButton.RightShift))
            {
                _shiftPressed = true;
            }
            else if (e.Released.Contains(SButton.LeftShift) || e.Released.Contains(SButton.RightShift))
            {
                _shiftPressed = false;
            }
        }

        private void OnDropdownToggle(bool expanded)
        {
            _dropdownExpanded = expanded;
            inventory.highlightMethod = _ => !expanded;

            if (!expanded && !Game1.lastCursorMotionWasMouse)
            {
                setCurrentlySnappedComponentTo(_categorySelector.MyID);
                snapCursorToCurrentSnappedComponent();
            }
        }

        private void InitializeComponents()
        {
            var rootLeftAnchor = xPositionOnScreen;
            var rootTopAnchor = yPositionOnScreen;
            var rootRightAnchor = rootLeftAnchor + width;
            var rootBottomAnchor = rootTopAnchor + height;

            var sideTopAnchor = rootTopAnchor - Game1.tileSize + UIConstants.BorderWidth - 2 * Game1.pixelZoom;
            var sideRightAnchor = rootRightAnchor;

            var barTopAnchor = rootTopAnchor - Game1.tileSize * 2;

            _researchArea =
                new ItemResearchArea(_content, _monitor, sideRightAnchor, sideTopAnchor + Game1.tileSize + 8);

            _cashTab = new CashTab(_content, _monitor, sideRightAnchor - 34, sideTopAnchor,
                _researchArea.Bounds.Width + 34);

            _moneyTooltip = new ItemMoneyTooltip(_content, _monitor);

            _qualitySelector =
                new ItemQualitySelectorTab(_content, _monitor, rootLeftAnchor - 8, barTopAnchor);

            _itemSortTab = new ItemSortTab(_content, _monitor, _qualitySelector.Bounds.Right + 20, barTopAnchor);

            _categorySelector = new ItemCategorySelectorTab(_content, _monitor, _spawnableItems,
                _itemSortTab.Bounds.Right + 20, _itemSortTab.Bounds.Y);
            _categorySelector?.SelectCategory(ModManager.Instance.Category);

            _searchBarTab = new ItemSearchBarTab(_content, _monitor, _categorySelector.Right + 20, barTopAnchor,
                _researchArea.Bounds.Right - _categorySelector.Right + 20 - 10 * Game1.pixelZoom);
            _searchBarTab.SetText(ModManager.Instance.SearchText);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Game1.fadeToBlackRect,
                new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);

            _researchArea.Draw(spriteBatch);
            _cashTab.Draw(spriteBatch);

            DrawMenu(spriteBatch);

            _qualitySelector.Draw(spriteBatch);
            _itemSortTab.Draw(spriteBatch);
            _categorySelector.Draw(spriteBatch);
            _searchBarTab.Draw(spriteBatch);

            if (ModManager.Instance.ModMode == ModMode.Buy && hoveredItem != null)
            {
                _moneyTooltip.Draw(spriteBatch, hoveredItem);
            }

            DrawHeldItem(spriteBatch);
            drawMouse(spriteBatch);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (_qualitySelector.Bounds.Contains(x, y))
            {
                _qualitySelector.HandleLeftClick();
            }
            else if (_itemSortTab.Bounds.Contains(x, y))
            {
                _itemSortTab.HandleLeftClick();
            }
            else if (_categorySelector.TryClick(x, y))
            {
            }
            else if (_researchArea.Bounds.Contains(x, y) && !_shiftPressed)
            {
                OnResearchAreaLeftClick();
            }
            else if (_researchArea.ButtonBounds.Contains(x, y))
            {
                _researchArea.HandleResearch();
            }
            else if (_searchBarTab.Bounds.Contains(x, y))
            {
                if (!_searchBarTab.Selected || !_searchBarTab.PersistFocus)
                    _searchBarTab.Focus(true);
            }
            else if (trashCan.containsPoint(x, y) && heldItem != null)
            {
                TryTrashItem();
            }
            else if (ModManager.Instance.ModMode == ModMode.Buy && GrabMenuBounds.Contains(x, y) && heldItem != null &&
                     ProgressionManager.Instance.ItemResearched(heldItem))
            {
                ModManager.Instance.SellItem(heldItem);
                heldItem = null;
                UpdateView();
            }
            else
            {
                if (_searchBarTab.Selected)
                {
                    _searchBarTab.Blur();
                }

                if (_shiftPressed && OnShiftLeftClickPressed(x, y))
                {
                }
                else
                {
                    base.receiveLeftClick(x, y, playSound);
                }
            }
        }

        private void OnResearchAreaLeftClick()
        {
            if (_researchArea.ResearchItem != null)
            {
                if (heldItem != null)
                {
                    var temp = _researchArea.ReturnItem();

                    if (heldItem.Name.Equals(temp.Name, StringComparison.InvariantCultureIgnoreCase)
                        && heldItem is Object heldObj && temp is Object resObj &&
                        heldObj.quality.Equals(resObj.quality))
                    {
                        var rest = 0;

                        if (temp.Stack + heldItem.Stack > temp.maximumStackSize())
                        {
                            rest = temp.Stack + heldItem.Stack - temp.maximumStackSize();
                        }

                        var quantityToTransfer = heldItem.Stack - rest;

                        temp.Stack += quantityToTransfer;

                        _researchArea.TrySetItem(temp);

                        if (heldItem.Stack - quantityToTransfer <= 0)
                        {
                            heldItem = null;
                        }
                        else
                        {
                            heldItem.Stack -= quantityToTransfer;
                        }
                    }
                    else
                    {
                        _researchArea.TrySetItem(heldItem);
                        heldItem = temp;
                    }
                }
                else
                {
                    heldItem = _researchArea.ReturnItem();
                }
            }
            else
            {
                _researchArea.TrySetItem(heldItem);
                heldItem = null;
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (_qualitySelector.Bounds.Contains(x, y))
            {
                _qualitySelector.HandleRightClick();
            }
            else if (_itemSortTab.Bounds.Contains(x, y))
            {
                _itemSortTab.HandleRightClick();
            }
            else if (_searchBarTab.Bounds.Contains(x, y))
            {
                _searchBarTab.Clear();
            }
            else if (_categorySelector.IsExpanded)
            {
                _categorySelector.Close();
            }
            else if (_categorySelector.Bounds.Contains(x, y))
            {
                _categorySelector.ResetCategory();
            }
            else if (_researchArea.Bounds.Contains(x, y))
            {
                OnResearchAreaRightClick();
            }
            else
            {
                base.receiveRightClick(x, y, playSound);
            }
        }

        private void TryReturnItemToInventory(Item item)
        {
            if (Game1.player.isInventoryFull())
            {
                if (heldItem != null)
                {
                    DropItem(item);
                }
                else
                {
                    heldItem = item;
                }
            }
            else
            {
                Game1.player.addItemByMenuIfNecessary(item);
            }
        }

        private void OnResearchAreaRightClick()
        {
            if (_researchArea.ResearchItem != null)
            {
                var temp = _researchArea.ReturnItem();

                if (heldItem == null)
                {
                    var newItem = temp.DeepClone();

                    newItem.Stack = 1;
                    temp.Stack--;

                    heldItem = newItem;
                    _researchArea.TrySetItem(temp);
                }
                else if (heldItem.Name.Equals(temp.Name, StringComparison.InvariantCultureIgnoreCase)
                         && heldItem is Object heldObj && temp is Object resObj &&
                         heldObj.quality.Equals(resObj.quality)
                         && heldItem.Stack + 1 <= heldItem.maximumStackSize())
                {
                    heldItem.Stack++;

                    if (temp.Stack - 1 > 0)
                    {
                        temp.Stack--;
                        _researchArea.TrySetItem(temp);
                    }
                }
                else
                {
                    _researchArea.TrySetItem(temp);
                }
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            var inDropdown = _categorySelector.IsExpanded;
            var isEscape = key == Keys.Escape;
            var isExitButton = isEscape || Game1.options.doesInputListContain(Game1.options.menuButton, key) ||
                               Game1.options.doesInputListContain(Game1.options.cancelButton, key);

            if (isEscape && (_searchBarTab.PersistFocus ||
                             _searchBarTab.Selected && !string.IsNullOrEmpty(ModManager.Instance.SearchText)))
            {
                _searchBarTab.Clear();
                _searchBarTab.Blur();
            }
            else if (inDropdown && isExitButton)
            {
                _categorySelector.Close();
            }
            else if (key == Keys.Left || key == Keys.Right)
            {
                var direction = key == Keys.Left ? -1 : 1;
                _categorySelector.NextCategory(direction);
            }
            else if (key == Keys.Up || key == Keys.Down)
            {
                var direction = key == Keys.Up ? -1 : 1;

                if (inDropdown)
                {
                    _categorySelector.HandleScroll(direction);
                }
                else
                {
                    ScrollView(direction);
                }
            }
            else if (key == Keys.Delete && ModManager.Instance.ModMode == ModMode.Spawn)
            {
                if (heldItem != null && ModManager.Instance.ModMode == ModMode.Spawn)
                {
                    if (TryTrashItem(heldItem))
                    {
                        heldItem = null;
                    }
                }
                else if (hoveredItem?.hasbeenInInventory && ProgressionManager.Instance.ItemResearched(hoveredItem))
                {
                    Game1.player.removeItemFromInventory(hoveredItem);
                }
            }
            else
            {
                var isIgnoredExitKey = _searchBarTab.Selected && isExitButton && !isEscape;
                if (!isIgnoredExitKey && !_searchBarTab.IsSearchBoxSelectionChanging)
                {
                    base.receiveKeyPress(key);
                }
            }
        }

        private bool OnShiftLeftClickPressed(int x, int y)
        {
            if (_researchArea.Bounds.Contains(x, y))
            {
                if (_researchArea.ResearchItem != null)
                {
                    TryReturnItemToInventory(_researchArea.ReturnItem());
                }

                return true;
            }

            if (trashCan.containsPoint(x, y))
            {
                foreach (var item in Game1.player.items.Where(item => item != null))
                {
                    if (ProgressionManager.Instance.ItemResearched(item))
                    {
                        switch (ModManager.Instance.ModMode)
                        {
                            case ModMode.Buy:
                                ModManager.Instance.SellItem(item);
                                Game1.player.removeItemFromInventory(item);
                                break;
                            default:
                                Game1.player.removeItemFromInventory(item);
                                break;
                        }
                    }
                }

                return true;
            }

            if (hoveredItem != null && Game1.player.items.Contains(hoveredItem))
            {
                if (ProgressionManager.Instance.ItemResearched(hoveredItem))
                {
                    if (ModManager.Instance.ModMode == ModMode.Buy)
                    {
                        ModManager.Instance.SellItem(hoveredItem);
                        UpdateView();
                    }
                    
                    // hereafter item will be deleted
                }
                else if (_researchArea.ResearchItem != null)
                {
                    TryReturnItemToInventory(_researchArea.ReturnItem());
                    
                    _researchArea.TrySetItem(hoveredItem);
                }
                else
                {
                    _researchArea.TrySetItem(hoveredItem);
                }

                Game1.player.removeItemFromInventory(hoveredItem);

                return true;
            }

            return false;
        }

        private bool TryTrashItem()
        {
            var result = TryTrashItem(heldItem);

            if (result)
            {
                heldItem = null;
            }

            return result;
        }

        private bool TryTrashItem(Item item)
        {
            if (ProgressionManager.Instance.ItemResearched(item))
            {
                Utility.trashItem(item);

                return true;
            }

            return false;
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);

            if (_categorySelector.IsExpanded)
            {
                _categorySelector.ReceiveScrollWheelAction(direction);
            }
            else if (_overDropdown)
            {
                _categorySelector.HandleScroll(-direction);
            }
            else
            {
                ScrollView(-direction);
            }
        }

        public override void performHoverAction(int x, int y)
        {
            _overDropdown = _categorySelector.Bounds.Contains(x, y);

            if (!_searchBarTab.PersistFocus)
            {
                var overSearchBox = _searchBarTab.Bounds.Contains(x, y);

                if (_searchBarTab.Selected != overSearchBox)
                {
                    if (overSearchBox)
                    {
                        _searchBarTab.Focus(false);
                    }
                    else
                    {
                        _searchBarTab.Blur();
                    }
                }
            }

            base.performHoverAction(x, y);
        }

        private void OnItemGrab(Item item, Farmer player)
        {
            if (ModManager.Instance.ModMode == ModMode.Buy &&
                ModManager.Instance.GetItemPrice(item, true) <= Game1.player._money)
            {
                ModManager.Instance.BuyItem(item);
            }

            UpdateView();
        }

        private void DrawHeldItem(SpriteBatch spriteBatch)
        {
            if (hoverText != null && (hoveredItem == null || ItemsToGrabMenu == null))
            {
                if (hoverAmount > 0)
                {
                    drawToolTip(spriteBatch, hoverText, "", null, true, moneyAmountToShowAtBottom: hoverAmount);
                }
                else
                {
                    drawHoverText(spriteBatch, hoverText, Game1.smallFont);
                }
            }

            if (hoveredItem != null)
            {
                drawToolTip(spriteBatch, hoveredItem.getDescription(), hoveredItem.DisplayName, hoveredItem,
                    heldItem != null);
            }
            else if (hoveredItem != null && ItemsToGrabMenu != null)
            {
                drawToolTip(spriteBatch, ItemsToGrabMenu.descriptionText, ItemsToGrabMenu.descriptionTitle, hoveredItem,
                    heldItem != null);
            }

            heldItem?.drawInMenu(spriteBatch, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);
        }

        public override void update(GameTime time)
        {
            _searchBarTab.Update(time);
            base.update(time);
        }

        private static void DropItem(Item item)
        {
            Game1.createItemDebris(item, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
        }

        private void UpdateView(bool rebuild = false, bool resetScroll = true)
        {
            var totalRows = (int) Math.Ceiling(_filteredItems.Count / (ItemsPerRow * 1m));

            _maxTopRowIndex = Math.Max(0, totalRows - 3);

            if (rebuild)
            {
                _filteredItems.Clear();
                _filteredItems.AddRange(GetFilteredItems());

                if (resetScroll || _topRowIndex > _maxTopRowIndex)
                {
                    _topRowIndex = 0;
                }
            }

            ScrollView(0, resetItemView: false);

            _itemsInView.Clear();

            foreach (var prefab in _filteredItems.Skip(_topRowIndex * ItemsPerRow).Take(ItemsPerView))
            {
                var item = prefab.Item.CreateItem();

                var quality = ItemQuality.Normal;

                switch (ModManager.Instance.ModMode)
                {
                    case ModMode.Buy:
                        item.Stack = prefab.GetAvailableQuantity(Game1.player._money, ModManager.Instance.Quality,
                            out var availableQuality);

                        quality = availableQuality;
                        break;
                    default:
                        item.Stack = item.maximumStackSize();
                        break;
                }

                if (ModManager.Instance.ModMode != ModMode.Buy)
                {
                    quality = item is Object
                        ? prefab.GetAvailableQuality(ModManager.Instance.Quality)
                        : ItemQuality.Normal;
                }

                if (item is Object obj)
                {
                    obj.Quality = (int) quality;
                }

                _itemsInView.Add(item);
            }
        }

        private void ScrollView(int direction, bool resetItemView = true)
        {
            if (direction < 0)
            {
                _topRowIndex--;
            }
            else if (direction > 0)
            {
                _topRowIndex++;
            }

            _topRowIndex = (int) MathHelper.Clamp(_topRowIndex, 0, _maxTopRowIndex);

            if (resetItemView)
            {
                UpdateView();
            }
        }

        private IEnumerable<ResearchableItem> GetFilteredItems()
        {
            var items = ProgressionManager.Instance.GetResearchedItems();

            items = ModManager.Instance.SortOption switch
            {
                ItemSortOption.Category => items.OrderBy(p => p.Item.Item.Category),
                ItemSortOption.ID => items.OrderBy(p => p.Item.Item.ParentSheetIndex),
                _ => items.OrderBy(p => p.Item.DisplayName)
            };

            if (!Utils.Helpers.EqualsCaseInsensitive(_categorySelector.SelectedCategory, I18n.Category_All()))
            {
                items = items.Where(item =>
                    Utils.Helpers.EqualsCaseInsensitive(item.Item.Category, _categorySelector.SelectedCategory));
            }

            var search = ModManager.Instance.SearchText.Trim();

            if (search != "")
            {
                items = items.Where(item =>
                    item.Item.Name.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) >= 0
                    || item.Item.DisplayName.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) >= 0
                );
            }

            return items;
        }
    }
}