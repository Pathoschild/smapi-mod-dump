using System;
using System.Collections.Generic;
using System.Linq;
using MegaStorage.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace MegaStorage.UI
{
    public class LargeItemGrabMenu : ItemGrabMenu
    {
        private const int TopHeightChange = -24;
        private const int TopBackgroundChange = 24;

        private const int MoveTop = -40;
        private const int MoveBottom = 108;

        protected const int Rows = 6;
        protected const int ItemsPerRow = 12;
        protected const int Capacity = ItemsPerRow * Rows;

        private Item SourceItem => _sourceItemReflected.GetValue();
        private readonly IReflectedField<Item> _sourceItemReflected;

        private TemporaryAnimatedSprite Poof { set => _poofReflected.SetValue(value); }
        private readonly IReflectedField<TemporaryAnimatedSprite> _poofReflected;

        private behaviorOnItemSelect BehaviorFunction => _behaviorFunctionReflected.GetValue();
        private readonly IReflectedField<behaviorOnItemSelect> _behaviorFunctionReflected;

        public ClickableTextureComponent UpArrow;
        public ClickableTextureComponent DownArrow;
        public List<ClickableComponent> CategoryComponents;

        protected readonly CustomChest CustomChest;

        private ChestCategory[] _chestCategories;
        private ChestCategory _hoverCategory;
        protected ChestCategory SelectedCategory;

        public LargeItemGrabMenu(CustomChest customChest)
            : base(customChest.items, false, true, InventoryMenu.highlightAllItems, customChest.grabItemFromInventory, null, customChest.grabItemFromChest,
                false, true, true, true, true, 1, customChest, -1, customChest)
        {
            CustomChest = customChest;
            _sourceItemReflected = MegaStorageMod.Instance.Helper.Reflection.GetField<Item>(this, "sourceItem");
            _poofReflected = MegaStorageMod.Instance.Helper.Reflection.GetField<TemporaryAnimatedSprite>(this, "poof");
            _behaviorFunctionReflected = MegaStorageMod.Instance.Helper.Reflection.GetField<behaviorOnItemSelect>(this, "behaviorFunction");
            ItemsToGrabMenu = new InventoryMenu(xPositionOnScreen + 32, yPositionOnScreen, false, customChest.items, null, Capacity, Rows);
            ItemsToGrabMenu.movePosition(0, MoveTop);
            inventory.movePosition(0, MoveBottom);
            CreateArrows();
            SetupCategories();
            SetupControllerSupport();
            Refresh();
        }

        private void CreateArrows()
        {
            UpArrow = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + 768 + 32, yPositionOnScreen - 32, 64, 64), Game1.mouseCursors,
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 12), 1f)
            {
                myID = 88,
                downNeighborID = 89
            };
            DownArrow = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + 768 + 32, yPositionOnScreen + 256, 64, 64),
                Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 11), 1f)
            {
                myID = 89,
                upNeighborID = 88
            };
        }

        private void SetupCategories()
        {
            _chestCategories = new[]
            {
                new AllCategory(0, "All", xPositionOnScreen, yPositionOnScreen),
                new ChestCategory(1, "Crops", new Vector2(640, 80), new []{ -81, -80, -79, -75 }, xPositionOnScreen, yPositionOnScreen),
                new ChestCategory(2, "Seeds", new Vector2(656, 64), new []{ -74, -19 }, xPositionOnScreen, yPositionOnScreen),
                new ChestCategory(3, "Materials", new Vector2(672, 64), new []{ -15, -16, -2, -12, -8, -28 }, xPositionOnScreen, yPositionOnScreen),
                new ChestCategory(4, "Cooking", new Vector2(688, 64), new []{ -25, -7, -18, -14, -6, -5, -27, -26 }, xPositionOnScreen, yPositionOnScreen),
                new ChestCategory(5, "Fishing", new Vector2(640, 64), new []{ -4, -21, -22 }, xPositionOnScreen, yPositionOnScreen),
                new MiscCategory(6, "Misc", new Vector2(672, 80), new []{ -24, -20 }, xPositionOnScreen, yPositionOnScreen)
            };
            SelectedCategory = _chestCategories.First();
        }

        private void SetupControllerSupport()
        {
            if (Game1.options.SnappyMenus)
            {
                ItemsToGrabMenu.populateClickableComponentList();
                foreach (var cc in ItemsToGrabMenu.inventory)
                {
                    if (cc == null) continue;
                    cc.myID += 53910;
                    cc.upNeighborID += 53910;
                    cc.rightNeighborID += 53910;
                    cc.downNeighborID = -7777;
                    cc.leftNeighborID += 53910;
                    cc.fullyImmutable = true;
                }
            }

            for (var index = 0; index < 12; ++index)
            {
                if (inventory?.inventory != null && inventory.inventory.Count >= 12)
                    inventory.inventory[index].upNeighborID = discreteColorPickerCC == null || ItemsToGrabMenu == null || ItemsToGrabMenu.inventory.Count > index
                        ? ItemsToGrabMenu.inventory.Count > index ? 53910 + index : 53910
                        : 4343;
                if (discreteColorPickerCC != null && ItemsToGrabMenu != null && ItemsToGrabMenu.inventory.Count > index)
                    ItemsToGrabMenu.inventory[index].upNeighborID = 4343;
            }

            for (var index = 0; index < 36; ++index)
            {
                if (inventory?.inventory == null || inventory.inventory.Count <= index) continue;
                inventory.inventory[index].upNeighborID = -7777;
                inventory.inventory[index].upNeighborImmutable = true;
            }

            if (trashCan != null && inventory.inventory.Count >= 12 && inventory.inventory[11] != null)
                inventory.inventory[11].rightNeighborID = 5948;
            if (trashCan != null)
                trashCan.leftNeighborID = 11;
            if (okButton != null)
                okButton.leftNeighborID = 11;

            for (var i = 0; i < 12; i++)
            {
                var item = inventory.inventory[i];
                item.upNeighborID = 53910 + 60 + i;
            }

            var right0 = ItemsToGrabMenu.inventory[0 * 12 + 11];
            var right1 = ItemsToGrabMenu.inventory[1 * 12 + 11];
            var right2 = ItemsToGrabMenu.inventory[2 * 12 + 11];
            var right3 = ItemsToGrabMenu.inventory[3 * 12 + 11];
            var right4 = ItemsToGrabMenu.inventory[4 * 12 + 11];
            var right5 = ItemsToGrabMenu.inventory[5 * 12 + 11];
            //var right6 = ItemsToGrabMenu.inventory[6 * 12 + 11];

            right0.rightNeighborID = UpArrow.myID;
            right1.rightNeighborID = UpArrow.myID;
            right2.rightNeighborID = colorPickerToggleButton.myID;
            //right3.rightNeighborID = fillStacksButton.myID;
            right3.rightNeighborID = organizeButton.myID;
            right4.rightNeighborID = DownArrow.myID;
            right5.rightNeighborID = DownArrow.myID;

            colorPickerToggleButton.leftNeighborID = right2.myID;
            colorPickerToggleButton.upNeighborID = UpArrow.myID;

            //fillStacksButton.upNeighborID = colorPickerToggleButton.myID;
            //fillStacksButton.downNeighborID = organizeButton.myID;

            organizeButton.leftNeighborID = right3.myID;
            organizeButton.downNeighborID = DownArrow.myID;

            UpArrow.rightNeighborID = colorPickerToggleButton.myID;
            UpArrow.leftNeighborID = right0.myID;

            DownArrow.rightNeighborID = organizeButton.myID;
            DownArrow.leftNeighborID = right4.myID;
            DownArrow.downNeighborID = right5.myID;

            CategoryComponents = new List<ClickableComponent>();
            for (var index = 0; index < _chestCategories.Length; index++)
            {
                var cat = _chestCategories[index];
                var catComponent = (ClickableComponent)cat;
                catComponent.myID = 239865 + index;
                CategoryComponents.Add(catComponent);
            }

            var left0 = ItemsToGrabMenu.inventory[0 * 12];
            var left1 = ItemsToGrabMenu.inventory[1 * 12];
            var left2 = ItemsToGrabMenu.inventory[2 * 12];
            var left3 = ItemsToGrabMenu.inventory[3 * 12];
            var left4 = ItemsToGrabMenu.inventory[4 * 12];
            var left5 = ItemsToGrabMenu.inventory[5 * 12];

            left0.leftNeighborID = CategoryComponents[0].myID;
            left1.leftNeighborID = CategoryComponents[1].myID;
            left2.leftNeighborID = CategoryComponents[2].myID;
            left3.leftNeighborID = CategoryComponents[4].myID;
            left4.leftNeighborID = CategoryComponents[5].myID;
            left5.leftNeighborID = CategoryComponents[6].myID;

            CategoryComponents[0].rightNeighborID = left0.myID;
            CategoryComponents[1].rightNeighborID = left1.myID;
            CategoryComponents[2].rightNeighborID = left2.myID;
            CategoryComponents[3].rightNeighborID = left2.myID;
            CategoryComponents[4].rightNeighborID = left3.myID;
            CategoryComponents[5].rightNeighborID = left4.myID;
            CategoryComponents[6].rightNeighborID = left5.myID;

            CategoryComponents[0].downNeighborID = CategoryComponents[1].myID;
            CategoryComponents[1].downNeighborID = CategoryComponents[2].myID;
            CategoryComponents[2].downNeighborID = CategoryComponents[3].myID;
            CategoryComponents[3].downNeighborID = CategoryComponents[4].myID;
            CategoryComponents[4].downNeighborID = CategoryComponents[5].myID;
            CategoryComponents[5].downNeighborID = CategoryComponents[6].myID;

            CategoryComponents[6].upNeighborID = CategoryComponents[5].myID;
            CategoryComponents[5].upNeighborID = CategoryComponents[4].myID;
            CategoryComponents[4].upNeighborID = CategoryComponents[3].myID;
            CategoryComponents[3].upNeighborID = CategoryComponents[2].myID;
            CategoryComponents[2].upNeighborID = CategoryComponents[1].myID;
            CategoryComponents[1].upNeighborID = CategoryComponents[0].myID;

            populateClickableComponentList();
            snapToDefaultClickableComponent();
        }

        public virtual void Refresh()
        {
            MegaStorageMod.Instance.Monitor.VerboseLog("Category: " + SelectedCategory.name);
            ItemsToGrabMenu.actualInventory = SelectedCategory.Filter(CustomChest.items);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (_hoverCategory != null)
            {
                ChangeCategory(_hoverCategory);
            }
            ReceiveLeftClickBase(x, y, !destroyItemOnClick);
            if (chestColorPicker != null)
            {
                chestColorPicker.receiveLeftClick(x, y);
                if (SourceItem is Chest chest)
                    chest.playerChoiceColor.Value = chestColorPicker.getColorFromSelection(chestColorPicker.colorSelection);
            }
            if (colorPickerToggleButton != null && colorPickerToggleButton.containsPoint(x, y))
            {
                Game1.player.showChestColorPicker = !Game1.player.showChestColorPicker;
                chestColorPicker.visible = Game1.player.showChestColorPicker;
                Game1.playSound("drumkit6");
            }
            if (whichSpecialButton != -1 && specialButton != null && specialButton.containsPoint(x, y))
            {
                Game1.playSound("drumkit6");
                if (whichSpecialButton == 1 && context != null && context is JunimoHut hut)
                {
                    hut.noHarvest.Value = !hut.noHarvest.Value;
                    specialButton.sourceRect.X = hut.noHarvest.Value ? 124 : 108;
                }
            }
            if (heldItem == null && showReceivingMenu)
            {
                var itemsBefore = ItemsToGrabMenu.actualInventory.ToList();
                heldItem = ItemsToGrabMenu.leftClick(x, y, heldItem, false);
                var itemsAfter = ItemsToGrabMenu.actualInventory.ToList();
                FixNulls(itemsBefore, itemsAfter);
                if (heldItem != null && behaviorOnItemGrab != null)
                {
                    behaviorOnItemGrab(heldItem, Game1.player);
                    if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu)
                    {
                        ((ItemGrabMenu)Game1.activeClickableMenu).setSourceItem(SourceItem);
                        if (Game1.options.SnappyMenus)
                        {
                            ((ItemGrabMenu)Game1.activeClickableMenu).currentlySnappedComponent = currentlySnappedComponent;
                            ((ItemGrabMenu)Game1.activeClickableMenu).snapCursorToCurrentSnappedComponent();
                        }
                    }
                }
                if (heldItem is Object obj1 && obj1.ParentSheetIndex == 326)
                {
                    heldItem = null;
                    Game1.player.canUnderstandDwarves = true;
                    Poof = CreatePoof(x, y);
                    Game1.playSound("fireball");
                }
                else if (heldItem is Object obj2 && obj2.ParentSheetIndex == 102)
                {
                    heldItem = null;
                    Game1.player.foundArtifact(102, 1);
                    Poof = CreatePoof(x, y);
                    Game1.playSound("fireball");
                }
                else if (heldItem is Object obj3 && obj3.IsRecipe)
                {
                    var key = heldItem.Name.Substring(0, heldItem.Name.IndexOf("Recipe") - 1);
                    try
                    {
                        if (obj3.Category == -7)
                            Game1.player.cookingRecipes.Add(key, 0);
                        else
                            Game1.player.craftingRecipes.Add(key, 0);
                        Poof = CreatePoof(x, y);
                        Game1.playSound("newRecipe");
                    }
                    catch (Exception) { }
                    heldItem = null;
                }
                else if (Game1.player.addItemToInventoryBool(heldItem))
                {
                    heldItem = null;
                    Game1.playSound("coin");
                }
            }
            else if ((reverseGrab || BehaviorFunction != null) && isWithinBounds(x, y))
            {
                BehaviorFunction(heldItem, Game1.player);
                if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu)
                {
                    ((ItemGrabMenu)Game1.activeClickableMenu).setSourceItem(SourceItem);
                    if (Game1.options.SnappyMenus)
                    {
                        ((ItemGrabMenu)Game1.activeClickableMenu).currentlySnappedComponent = currentlySnappedComponent;
                        ((ItemGrabMenu)Game1.activeClickableMenu).snapCursorToCurrentSnappedComponent();
                    }
                }
                if (destroyItemOnClick)
                {
                    heldItem = null;
                    return;
                }
            }
            //Test Fill Stash Button
            if (fillStacksButton != null && fillStacksButton.containsPoint(x, y))
            {
                FillOutStacks();
                Game1.playSound("Ship");
            }
            if (organizeButton != null && organizeButton.containsPoint(x, y))
            {
                organizeItemsInList(CustomChest.items);
                Refresh();
                Game1.playSound("Ship");
            }
            else
            {
                if (heldItem == null || isWithinBounds(x, y) || !heldItem.canBeTrashed())
                    return;
                Game1.playSound("throwDownITem");
                Game1.createItemDebris(heldItem, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
                inventory.onAddItem?.Invoke(heldItem, Game1.player);
                heldItem = null;
            }
        }

        protected virtual void ChangeCategory(ChestCategory cat)
        {
            SelectedCategory = cat;
            Refresh();
        }

        private void ReceiveLeftClickBase(int x, int y, bool playSound = true)
        {
            heldItem = inventory.leftClick(x, y, heldItem, playSound);
            if (!isWithinBounds(x, y) && readyToClose())
                trashCan?.containsPoint(x, y);
            if (okButton != null && okButton.containsPoint(x, y) && readyToClose())
            {
                exitThisMenu();
                if (Game1.currentLocation.currentEvent != null)
                    ++Game1.currentLocation.currentEvent.CurrentCommand;
                Game1.playSound("bigDeSelect");
            }
            if (trashCan == null || !trashCan.containsPoint(x, y) || (heldItem == null || !heldItem.canBeTrashed()))
                return;
            if (heldItem is Object obj && Game1.player.specialItems.Contains(obj.ParentSheetIndex))
                Game1.player.specialItems.Remove(obj.ParentSheetIndex);
            heldItem = null;
            Game1.playSound("trashcan");
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (!allowRightClick)
                return;
            heldItem = inventory.rightClick(x, y, heldItem, playSound && playRightClickSound);
            if (heldItem == null && showReceivingMenu)
            {
                var itemsBefore = ItemsToGrabMenu.actualInventory.ToList();
                heldItem = ItemsToGrabMenu.rightClick(x, y, heldItem, false);
                if (heldItem != null && behaviorOnItemGrab != null)
                {
                    var itemsAfter = ItemsToGrabMenu.actualInventory.ToList();
                    FixNulls(itemsBefore, itemsAfter);
                    behaviorOnItemGrab(heldItem, Game1.player);
                    if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu)
                        ((ItemGrabMenu)Game1.activeClickableMenu).setSourceItem(SourceItem);
                    if (Game1.options.SnappyMenus)
                    {
                        ((ItemGrabMenu)Game1.activeClickableMenu).currentlySnappedComponent = currentlySnappedComponent;
                        (Game1.activeClickableMenu as ItemGrabMenu)?.snapCursorToCurrentSnappedComponent();
                    }
                }
                if (heldItem is Object obj && obj.ParentSheetIndex == 326)
                {
                    heldItem = null;
                    Game1.player.canUnderstandDwarves = true;
                    Poof = CreatePoof(x, y);
                    Game1.playSound("fireball");
                }
                else if (heldItem is Object obj2 && obj2.IsRecipe)
                {
                    var key = obj2.Name.Substring(0, obj2.Name.IndexOf("Recipe") - 1);
                    try
                    {
                        if (obj2.Category == -7)
                            Game1.player.cookingRecipes.Add(key, 0);
                        else
                            Game1.player.craftingRecipes.Add(key, 0);
                        Poof = CreatePoof(x, y);
                        Game1.playSound("newRecipe");
                    }
                    catch (Exception) { }
                    heldItem = null;
                }
                else
                {
                    if (!Game1.player.addItemToInventoryBool(heldItem))
                        return;
                    heldItem = null;
                    Game1.playSound("coin");
                }
            }
            else
            {
                if (!reverseGrab && BehaviorFunction == null)
                    return;
                BehaviorFunction(heldItem, Game1.player);
                if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu)
                    ((ItemGrabMenu)Game1.activeClickableMenu).setSourceItem(SourceItem);
                if (!destroyItemOnClick)
                    return;
                heldItem = null;
            }
        }

        private void FixNulls(List<Item> itemsBefore, List<Item> itemsAfter)
        {
            for (var i = 0; i < itemsBefore.Count; i++)
            {
                var itemBefore = itemsBefore[i];
                var itemAfter = itemsAfter[i];
                if (itemBefore != null && itemAfter == null)
                {
                    var index = CustomChest.items.IndexOf(itemBefore);
                    if (index > -1)
                    {
                        CustomChest.items.RemoveAt(index);
                    }
                }
            }
        }

        private TemporaryAnimatedSprite CreatePoof(int x, int y)
        {
            return new TemporaryAnimatedSprite("TileSheets/animations",
                new Rectangle(0, 320, 64, 64), 50f, 8, 0,
                new Vector2(x - x % 64 + 16, y - y % 64 + 16), false, false);
        }

        public override void draw(SpriteBatch b)
        {
            Draw(b);
            drawMouse(b);
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            _hoverCategory = _chestCategories.FirstOrDefault(c => c.containsPoint(x, y));
        }

        protected void Draw(SpriteBatch b)
        {
            // opaque background
            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), Color.Black * 0.5f);

            // bottom inventory
            Game1.drawDialogueBox(xPositionOnScreen - borderWidth / 2, yPositionOnScreen + borderWidth + spaceToClearTopBorder + 64 + MoveBottom, width, height - (borderWidth + spaceToClearTopBorder + 192), false, true);
            okButton?.draw(b);
            inventory.draw(b);

            // bottom inventory icon
            b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen - 64, yPositionOnScreen + height / 2 + MoveBottom + 64 + 16), new Rectangle(16, 368, 12, 16), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen - 64, yPositionOnScreen + height / 2 + MoveBottom + 64 - 16), new Rectangle(21, 368, 11, 16), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen - 40, yPositionOnScreen + height / 2 + MoveBottom + 64 - 44), new Rectangle(4, 372, 8, 11), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

            // top inventory
            Game1.drawDialogueBox(ItemsToGrabMenu.xPositionOnScreen - borderWidth - spaceToClearSideBorder, ItemsToGrabMenu.yPositionOnScreen - borderWidth - spaceToClearTopBorder + TopBackgroundChange,
                ItemsToGrabMenu.width + borderWidth * 2 + spaceToClearSideBorder * 2, ItemsToGrabMenu.height + spaceToClearTopBorder + borderWidth * 2 + TopHeightChange, false, true, null, false, true);
            ItemsToGrabMenu.draw(b);

            foreach (var chestCategory in _chestCategories)
            {
                var xOffset = chestCategory == SelectedCategory ? 8 : 0;
                chestCategory.Draw(b, xPositionOnScreen + xOffset, yPositionOnScreen);
            }
            _hoverCategory?.DrawTooltip(b);

            if (colorPickerToggleButton != null)
                colorPickerToggleButton.draw(b);
            else
                specialButton?.draw(b);
            chestColorPicker?.draw(b);
            fillStacksButton?.draw(b);
            organizeButton?.draw(b);
            if (hoverText != null && (hoveredItem == null || ItemsToGrabMenu == null))
                drawHoverText(b, hoverText, Game1.smallFont);
            if (hoveredItem != null)
                drawToolTip(b, hoveredItem.getDescription(), hoveredItem.DisplayName, hoveredItem, heldItem != null);
            else if (hoveredItem != null && ItemsToGrabMenu != null)
                drawToolTip(b, ItemsToGrabMenu.descriptionText, ItemsToGrabMenu.descriptionTitle, hoveredItem, heldItem != null);
            heldItem?.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);
            Game1.mouseCursorTransparency = 1f;
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
        }

    }
}