using System;
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

        protected readonly NiceChest NiceChest;

        private Item SourceItem => _sourceItemReflected.GetValue();
        private readonly IReflectedField<Item> _sourceItemReflected;

        private TemporaryAnimatedSprite Poof { set => _poofReflected.SetValue(value); }
        private readonly IReflectedField<TemporaryAnimatedSprite> _poofReflected;

        private behaviorOnItemSelect BehaviorFunction => _behaviorFunctionReflected.GetValue();
        private readonly IReflectedField<behaviorOnItemSelect> _behaviorFunctionReflected;

        public ClickableTextureComponent UpButton;
        public ClickableTextureComponent DownButton;

        public LargeItemGrabMenu(NiceChest niceChest)
            : base(niceChest.items, false, true, InventoryMenu.highlightAllItems, niceChest.grabItemFromInventory, null, niceChest.grabItemFromChest,
                false, true, true, true, true, 1, niceChest, -1, niceChest)
        {
            NiceChest = niceChest;
            _sourceItemReflected = MegaStorageMod.Reflection.GetField<Item>(this, "sourceItem");
            _poofReflected = MegaStorageMod.Reflection.GetField<TemporaryAnimatedSprite>(this, "poof");
            _behaviorFunctionReflected = MegaStorageMod.Reflection.GetField<behaviorOnItemSelect>(this, "behaviorFunction");
            ItemsToGrabMenu = new InventoryMenu(xPositionOnScreen + 32, yPositionOnScreen, false, niceChest.items, null, Capacity, Rows);
            ItemsToGrabMenu.movePosition(0, MoveTop);
            inventory.movePosition(0, MoveBottom);
            CreateArrows();
            SetupControllerSupport();
        }

        private void CreateArrows()
        {
            UpButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + 768 + 32, yPositionOnScreen - 32, 64, 64), Game1.mouseCursors,
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 12, -1, -1), 1f, false)
            {
                myID = 88,
                downNeighborID = 89
            };
            DownButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + 768 + 32, yPositionOnScreen + 256, 64, 64),
                Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 11, -1, -1), 1f, false)
            {
                myID = 89,
                upNeighborID = 88
            };
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

            ItemsToGrabMenu.inventory[0 * 12 + 11].rightNeighborID = 88; // up arrow
            ItemsToGrabMenu.inventory[1 * 12 + 11].rightNeighborID = 88; // up arrow
            ItemsToGrabMenu.inventory[2 * 12 + 11].rightNeighborID = 27346; // color picker
            ItemsToGrabMenu.inventory[3 * 12 + 11].rightNeighborID = 106; // organize
            ItemsToGrabMenu.inventory[4 * 12 + 11].rightNeighborID = 89; // down arrow
            ItemsToGrabMenu.inventory[5 * 12 + 11].rightNeighborID = 89; // down arrow

            colorPickerToggleButton.leftNeighborID = ItemsToGrabMenu.inventory[2 * 12 + 11].myID;
            colorPickerToggleButton.upNeighborID = UpButton.myID;
            organizeButton.leftNeighborID = ItemsToGrabMenu.inventory[3 * 12 + 11].myID;
            organizeButton.downNeighborID = DownButton.myID;

            UpButton.rightNeighborID = colorPickerToggleButton.myID;
            DownButton.rightNeighborID = organizeButton.myID;
            DownButton.leftNeighborID = ItemsToGrabMenu.inventory[4 * 12 + 11].myID;
            DownButton.downNeighborID = ItemsToGrabMenu.inventory[5 * 12 + 11].myID;
            UpButton.leftNeighborID = ItemsToGrabMenu.inventory[0 * 12 + 11].myID;

            populateClickableComponentList();
            snapToDefaultClickableComponent();
        }

        public virtual void Refresh()
        {
            ItemsToGrabMenu.actualInventory = NiceChest.items.ToList();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            ReceiveLeftClickBase(x, y, !destroyItemOnClick);
            if (chestColorPicker != null)
            {
                chestColorPicker.receiveLeftClick(x, y, true);
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
                heldItem = ItemsToGrabMenu.leftClick(x, y, heldItem, false);
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
                    Poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2(x - x % 64 + 16, y - y % 64 + 16), false, false);
                    Game1.playSound("fireball");
                }
                else if (heldItem is Object obj2 && obj2.ParentSheetIndex == 102)
                {
                    heldItem = null;
                    Game1.player.foundArtifact(102, 1);
                    Poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2(x - x % 64 + 16, y - y % 64 + 16), false, false);
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
                        Poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2(x - x % 64 + 16, y - y % 64 + 16), false, false);
                        Game1.playSound("newRecipe");
                    }
                    catch (Exception ex) { }
                    heldItem = null;
                }
                else if (Game1.player.addItemToInventoryBool(heldItem, false))
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
            if (organizeButton != null && organizeButton.containsPoint(x, y))
            {
                organizeItemsInList(NiceChest.items);
                Refresh();
                Game1.playSound("Ship");
            }
            else
            {
                if (heldItem == null || isWithinBounds(x, y) || !heldItem.canBeTrashed())
                    return;
                Game1.playSound("throwDownITem");
                Game1.createItemDebris(heldItem, Game1.player.getStandingPosition(), Game1.player.FacingDirection, null, -1);
                inventory.onAddItem?.Invoke(heldItem, Game1.player);
                heldItem = null;
            }
        }

        private void ReceiveLeftClickBase(int x, int y, bool playSound = true)
        {
            heldItem = inventory.leftClick(x, y, heldItem, playSound);
            if (!isWithinBounds(x, y) && readyToClose())
                trashCan?.containsPoint(x, y);
            if (okButton != null && okButton.containsPoint(x, y) && readyToClose())
            {
                exitThisMenu(true);
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
                heldItem = ItemsToGrabMenu.rightClick(x, y, heldItem, false);
                if (heldItem != null && behaviorOnItemGrab != null)
                {
                    var itemInChest = ItemsToGrabMenu.actualInventory.FirstOrDefault(i => i != null && i.ParentSheetIndex == heldItem.ParentSheetIndex);
                    if (itemInChest == null)
                    {
                        var itemInNiceChest = NiceChest.items.Single(i => i.ParentSheetIndex == heldItem.ParentSheetIndex && i.Stack <= 1);
                        var index = NiceChest.items.IndexOf(itemInNiceChest);
                        NiceChest.items[index] = null;
                    }
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
                    Poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2(x - x % 64 + 16, y - y % 64 + 16), false, false);
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
                        Poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2(x - x % 64 + 16, y - y % 64 + 16), false, false);
                        Game1.playSound("newRecipe");
                    }
                    catch (Exception ex)
                    {
                    }
                    heldItem = null;
                }
                else
                {
                    if (!Game1.player.addItemToInventoryBool(heldItem, false))
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

        public override void draw(SpriteBatch b)
        {
            Draw(b);
            drawMouse(b);
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

            // top inventory icon
            b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen - 72, yPositionOnScreen + 64 + 16), new Rectangle(16, 368, 12, 16), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen - 72, yPositionOnScreen + 64 - 16), new Rectangle(21, 368, 11, 16), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen - 52, yPositionOnScreen + 64 - 44), new Rectangle(sbyte.MaxValue, 412, 10, 11), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

            // top inventory
            Game1.drawDialogueBox(ItemsToGrabMenu.xPositionOnScreen - borderWidth - spaceToClearSideBorder, ItemsToGrabMenu.yPositionOnScreen - borderWidth - spaceToClearTopBorder + TopBackgroundChange,
                ItemsToGrabMenu.width + borderWidth * 2 + spaceToClearSideBorder * 2, ItemsToGrabMenu.height + spaceToClearTopBorder + borderWidth * 2 + TopHeightChange, false, true, null, false, true);
            ItemsToGrabMenu.draw(b);

            if (colorPickerToggleButton != null)
                colorPickerToggleButton.draw(b);
            else
                specialButton?.draw(b);
            chestColorPicker?.draw(b);
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

    }
}