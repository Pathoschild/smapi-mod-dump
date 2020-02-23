using MegaStorage.Framework.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace MegaStorage.Framework.Interface
{
    public class LargeItemGrabMenu : ItemGrabMenu
    {
        private const int TopHeightChange = -24;
        private const int TopBackgroundChange = 24;

        private const int MoveTop = -24;
        private const int MoveBottom = 116;

        protected const int Rows = 6;
        protected const int ItemsPerRow = 12;
        protected const int Capacity = ItemsPerRow * Rows;

        private Item SourceItem => _sourceItemReflected.GetValue();
        private readonly IReflectedField<Item> _sourceItemReflected;

        private TemporaryAnimatedSprite Poof { set => _poofReflected.SetValue(value); }
        private readonly IReflectedField<TemporaryAnimatedSprite> _poofReflected;

        private behaviorOnItemSelect BehaviorFunction => _behaviorFunctionReflected.GetValue();
        private readonly IReflectedField<behaviorOnItemSelect> _behaviorFunctionReflected;

        private protected readonly CustomChest CustomChest;

        public LargeItemGrabMenu(CustomChest customChest)
            : base(
                inventory: NonNullCustomChest(customChest).items,
                reverseGrab: false,
                showReceivingMenu: true,
                highlightFunction: InventoryMenu.highlightAllItems,
                behaviorOnItemSelectFunction: NonNullCustomChest(customChest).grabItemFromInventory,
                message: null,
                behaviorOnItemGrab: NonNullCustomChest(customChest).grabItemFromChest,
                canBeExitedWithKey: true,
                showOrganizeButton: true,
                source: ItemGrabMenu.source_chest,
                context: customChest)
        {
            CustomChest = customChest;
            _sourceItemReflected = MegaStorageMod.Instance.Helper.Reflection.GetField<Item>(this, "sourceItem");
            _poofReflected = MegaStorageMod.Instance.Helper.Reflection.GetField<TemporaryAnimatedSprite>(this, "poof");
            _behaviorFunctionReflected = MegaStorageMod.Instance.Helper.Reflection.GetField<behaviorOnItemSelect>(this, "behaviorFunction");
            ItemsToGrabMenu = new InventoryMenu(xPositionOnScreen + 32, yPositionOnScreen, false, CustomChest.items, null, Capacity, Rows);
            ItemsToGrabMenu.movePosition(0, MoveTop);
            inventory.movePosition(0, MoveBottom);
            SetupControllerSupport();
        }

        private void SetupControllerSupport()
        {
            if (ItemsToGrabMenu is null || inventory?.inventory is null) return;

            if (Game1.options.SnappyMenus)
            {
                ItemsToGrabMenu.populateClickableComponentList();
                foreach (var cc in ItemsToGrabMenu.inventory.Where(cc => !(cc is null)))
                {
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
                if (inventory.inventory.Count >= 12)
                {
                    inventory.inventory[index].upNeighborID = discreteColorPickerCC is null || ItemsToGrabMenu.inventory.Count > index
                        ? ItemsToGrabMenu.inventory.Count > index ? 53910 + index : 53910
                        : 4343;
                }

                if (!(discreteColorPickerCC is null) && ItemsToGrabMenu.inventory.Count > index)
                {
                    ItemsToGrabMenu.inventory[index].upNeighborID = 4343;
                }
            }

            for (var index = 0; index < 36; ++index)
            {
                if (inventory.inventory.Count <= index) continue;
                inventory.inventory[index].upNeighborID = -7777;
                inventory.inventory[index].upNeighborImmutable = true;
            }

            if (!(trashCan is null) && inventory.inventory.Count >= 12 && !(inventory.inventory[11] is null))
            {
                inventory.inventory[11].rightNeighborID = 5948;
            }

            if (!(trashCan is null))
            {
                trashCan.leftNeighborID = 11;
            }

            if (!(okButton is null))
            {
                okButton.leftNeighborID = 11;
            }

            for (var i = 0; i < 12; i++)
            {
                var item = inventory.inventory[i];
                if (!(item is null))
                {
                    item.upNeighborID = 53910 + 60 + i;
                }
            }

            var rightItems =
                Enumerable.Range(0, 6)
                    .Select(i => ItemsToGrabMenu.inventory.ElementAt(i * 12 + 11))
                    .ToList();

            for (var i = 0; i < rightItems.Count; ++i)
            {
                rightItems[i].rightNeighborID = i < 3
                    ? colorPickerToggleButton?.myID
                      ?? organizeButton.myID
                    : organizeButton.myID;
            }

            if (!(colorPickerToggleButton is null))
            {
                colorPickerToggleButton.leftNeighborID = rightItems[2].myID;
            }

            //fillStacksButton.upNeighborID = colorPickerToggleButton.myID;
            //fillStacksButton.downNeighborID = organizeButton.myID;

            organizeButton.leftNeighborID = rightItems[3].myID;

            populateClickableComponentList();
            snapToDefaultClickableComponent();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            var itemGrabMenu = Game1.activeClickableMenu is ItemGrabMenu
                ? (ItemGrabMenu)Game1.activeClickableMenu
                : null;

            ReceiveLeftClickBase(x, y, !destroyItemOnClick);

            if (!(chestColorPicker is null))
            {
                chestColorPicker.receiveLeftClick(x, y);
                if (SourceItem is Chest chest)
                {
                    chest.playerChoiceColor.Value = chestColorPicker.getColorFromSelection(chestColorPicker.colorSelection);
                }
            }

            if (!(chestColorPicker is null) && !(colorPickerToggleButton is null) && colorPickerToggleButton.containsPoint(x, y))
            {
                Game1.player.showChestColorPicker = !Game1.player.showChestColorPicker;
                chestColorPicker.visible = Game1.player.showChestColorPicker;
                Game1.playSound("drumkit6");
            }

            if (whichSpecialButton != -1 && !(specialButton is null) && specialButton.containsPoint(x, y))
            {
                Game1.playSound("drumkit6");
                if (whichSpecialButton == 1 && !(context is null) && context is JunimoHut hut)
                {
                    hut.noHarvest.Value = !hut.noHarvest.Value;
                    specialButton.sourceRect.X = hut.noHarvest.Value ? 124 : 108;
                }
            }

            if (heldItem is null && showReceivingMenu)
            {
                var itemsBefore = ItemsToGrabMenu.actualInventory.ToList();
                heldItem = ItemsToGrabMenu.leftClick(x, y, heldItem, false);
                var itemsAfter = ItemsToGrabMenu.actualInventory.ToList();
                FixNulls(itemsBefore, itemsAfter);
                if (!(heldItem is null) && !(behaviorOnItemGrab is null))
                {
                    behaviorOnItemGrab(heldItem, Game1.player);
                    if (!(itemGrabMenu is null))
                    {
                        itemGrabMenu.setSourceItem(SourceItem);
                        if (Game1.options.SnappyMenus)
                        {
                            itemGrabMenu.currentlySnappedComponent = currentlySnappedComponent;
                            itemGrabMenu.snapCursorToCurrentSnappedComponent();
                        }
                    }
                }

                if (heldItem is SObject obj)
                {
                    switch (obj.ParentSheetIndex)
                    {
                        case 326:
                            heldItem = null;
                            Game1.player.canUnderstandDwarves = true;
                            Poof = CreatePoof(x, y);
                            Game1.playSound("fireball");
                            break;
                        case 102:
                            heldItem = null;
                            Game1.player.foundArtifact(102, 1);
                            Poof = CreatePoof(x, y);
                            Game1.playSound("fireball");
                            break;
                        default:
                            if (Utility.IsNormalObjectAtParentSheetIndex(heldItem, 434))
                            {
                                heldItem = null;
                                exitThisMenu(false);
                                Game1.player.eatObject(obj, true);
                            }
                            else if (obj.IsRecipe)
                            {
                                var key = heldItem.Name.Substring(0, heldItem.Name.IndexOf("Recipe", StringComparison.InvariantCultureIgnoreCase) - 1);
                                try
                                {
                                    if (obj.Category == -7)
                                    {
                                        Game1.player.cookingRecipes.Add(key, 0);
                                    }
                                    else
                                    {
                                        Game1.player.craftingRecipes.Add(key, 0);
                                    }

                                    Poof = CreatePoof(x, y);
                                    Game1.playSound("newRecipe");
                                }
                                catch (Exception)
                                {
                                    // ignored
                                }

                                heldItem = null;
                            }
                            else if (Game1.player.addItemToInventoryBool(heldItem))
                            {
                                heldItem = null;
                                Game1.playSound("coin");
                            }
                            break;
                    }
                }
            }
            else if ((reverseGrab || !(BehaviorFunction is null)) && isWithinBounds(x, y))
            {
                BehaviorFunction(heldItem, Game1.player);
                if (!(itemGrabMenu is null))
                {
                    itemGrabMenu.setSourceItem(SourceItem);
                    if (Game1.options.SnappyMenus)
                    {
                        itemGrabMenu.currentlySnappedComponent = currentlySnappedComponent;
                        itemGrabMenu.snapCursorToCurrentSnappedComponent();
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
                organizeItemsInList(CustomChest.items);
                Game1.playSound("Ship");
            }
            else if (fillStacksButton != null && fillStacksButton.containsPoint(x, y))
            {
                FillOutStacks();
                Game1.playSound("Ship");
            }
            else if (!(heldItem is null) && !isWithinBounds(x, y) && heldItem.canBeTrashed())
            {
                DropHeldItem();
                //Game1.playSound("throwDownITem");
                //Game1.createItemDebris(heldItem, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
                //inventory.onAddItem?.Invoke(heldItem, Game1.player);
                //heldItem = null;
            }
        }

        private void ReceiveLeftClickBase(int x, int y, bool playSound = true)
        {
            heldItem = inventory.leftClick(x, y, heldItem, playSound);

            if (!isWithinBounds(x, y) && readyToClose())
            {
                trashCan?.containsPoint(x, y);
            }

            if (!(okButton is null) && okButton.containsPoint(x, y) && readyToClose())
            {
                exitThisMenu();
                if (!(Game1.currentLocation.currentEvent is null))
                {
                    ++Game1.currentLocation.currentEvent.CurrentCommand;
                }

                Game1.playSound("bigDeSelect");
            }

            if (trashCan is null || !trashCan.containsPoint(x, y) || heldItem is null || !heldItem.canBeTrashed()) return;
            Utility.trashItem(heldItem);
            heldItem = null;
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (!allowRightClick)
            {
                receiveRightClickOnlyToolAttachments(x, y);
                return;
            }

            var itemGrabMenu = Game1.activeClickableMenu is ItemGrabMenu
                ? (ItemGrabMenu)Game1.activeClickableMenu
                : null;

            heldItem = inventory.rightClick(x, y, heldItem, playSound && playRightClickSound);
            if (heldItem is null && showReceivingMenu)
            {
                var itemsBefore = ItemsToGrabMenu.actualInventory.ToList();
                heldItem = ItemsToGrabMenu.rightClick(x, y, heldItem, false);
                var itemsAfter = ItemsToGrabMenu.actualInventory.ToList();
                FixNulls(itemsBefore, itemsAfter);
                if (!(heldItem is null) && !(behaviorOnItemGrab is null))
                {
                    behaviorOnItemGrab(heldItem, Game1.player);
                    if (!(itemGrabMenu is null))
                    {
                        itemGrabMenu.setSourceItem(SourceItem);
                        if (Game1.options.SnappyMenus)
                        {
                            itemGrabMenu.currentlySnappedComponent = currentlySnappedComponent;
                            itemGrabMenu.snapCursorToCurrentSnappedComponent();
                        }
                    }
                }

                if (heldItem is SObject obj)
                {
                    if (obj.ParentSheetIndex == 326)
                    {
                        heldItem = null;
                        Game1.player.canUnderstandDwarves = true;
                        Poof = CreatePoof(x, y);
                        Game1.playSound("fireball");
                    }
                    else if (Utility.IsNormalObjectAtParentSheetIndex(heldItem, 434))
                    {
                        heldItem = null;
                        exitThisMenu(false);
                        Game1.player.eatObject(obj, true);
                    }
                    else if (obj.IsRecipe)
                    {
                        var key = heldItem.Name.Substring(0,
                            heldItem.Name.IndexOf("Recipe", StringComparison.InvariantCultureIgnoreCase) - 1);
                        try
                        {
                            if (obj.Category == -7)
                            {
                                Game1.player.cookingRecipes.Add(key, 0);
                            }
                            else
                            {
                                Game1.player.craftingRecipes.Add(key, 0);
                            }

                            Poof = CreatePoof(x, y);
                            Game1.playSound("newRecipe");
                        }
                        catch (Exception)
                        {
                            // ignored
                        }

                        heldItem = null;
                    }
                }
                else if (!(heldItem is null) && Game1.player.addItemToInventoryBool(heldItem))
                {
                    heldItem = null;
                    Game1.playSound("coin");
                }
            }
            else if (reverseGrab || !(BehaviorFunction is null))
            {
                BehaviorFunction(heldItem, Game1.player);
                itemGrabMenu?.setSourceItem(SourceItem);
                if (destroyItemOnClick)
                {
                    heldItem = null;
                }
            }
        }

        private void FixNulls(IReadOnlyList<Item> itemsBefore, IReadOnlyList<Item> itemsAfter)
        {
            for (var i = 0; i < itemsBefore.Count; i++)
            {
                var itemBefore = itemsBefore[i];
                var itemAfter = itemsAfter[i];
                if (itemBefore == null || itemAfter != null) continue;
                var index = CustomChest.items.IndexOf(itemBefore);
                if (index > -1)
                {
                    CustomChest.items.RemoveAt(index);
                }
            }
        }

        private static TemporaryAnimatedSprite CreatePoof(int x, int y)
        {
            return new TemporaryAnimatedSprite(
                "TileSheets/animations",
                new Rectangle(0, 320, 64, 64),
                50f,
                8,
                0,
                new Vector2(x - x % 64 + 16, y - y % 64 + 16),
                false,
                false);
        }

        public override void draw(SpriteBatch b)
        {
            Draw(b);
            DrawHover(b);
            drawMouse(b);
        }

        protected void Draw(SpriteBatch b)
        {
            if (b is null) return;

            // opaque background
            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), Color.Black * 0.5f);

            // bottom inventory
            Game1.drawDialogueBox(
                xPositionOnScreen - borderWidth / 2,
                yPositionOnScreen + borderWidth + spaceToClearTopBorder + 64 + MoveBottom,
                width,
                height - (borderWidth + spaceToClearTopBorder + 192),
                false, true);

            okButton?.draw(b);
            inventory.draw(b);

            // bottom inventory icon
            b.Draw(
                Game1.mouseCursors,
                new Vector2(xPositionOnScreen - 64, yPositionOnScreen + height / 2 + MoveBottom + 64 + 16),
                new Rectangle(16, 368, 12, 16),
                Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

            b.Draw(
                Game1.mouseCursors,
                new Vector2(xPositionOnScreen - 64, yPositionOnScreen + height / 2 + MoveBottom + 64 - 16),
                new Rectangle(21, 368, 11, 16),
                Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

            b.Draw(
                Game1.mouseCursors,
                new Vector2(xPositionOnScreen - 40, yPositionOnScreen + height / 2 + MoveBottom + 64 - 44),
                new Rectangle(4, 372, 8, 11),
                Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

            // top inventory
            Game1.drawDialogueBox(
                ItemsToGrabMenu.xPositionOnScreen - borderWidth - spaceToClearSideBorder,
                ItemsToGrabMenu.yPositionOnScreen - borderWidth - spaceToClearTopBorder + TopBackgroundChange,
                ItemsToGrabMenu.width + borderWidth * 2 + spaceToClearSideBorder * 2,
                ItemsToGrabMenu.height + spaceToClearTopBorder + borderWidth * 2 + TopHeightChange,
                false, true);

            ItemsToGrabMenu.draw(b);

            if (!(colorPickerToggleButton is null))
            {
                colorPickerToggleButton.draw(b);
            }
            else
            {
                specialButton?.draw(b);
            }

            chestColorPicker?.draw(b);
            fillStacksButton?.draw(b);
            organizeButton?.draw(b);

            Game1.mouseCursorTransparency = 1f;
        }

        protected void DrawHover(SpriteBatch b)
        {
            if (!(hoverText is null) && hoveredItem is null)
            {
                drawHoverText(b, hoverText, Game1.smallFont);
            }

            if (!(hoveredItem is null))
            {
                drawToolTip(b, hoveredItem.getDescription(), hoveredItem.DisplayName, hoveredItem, !(heldItem is null));
            }
            else
            {
                drawToolTip(b, ItemsToGrabMenu.descriptionText, ItemsToGrabMenu.descriptionTitle, hoveredItem, !(heldItem is null));
            }

            heldItem?.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            // TBD
        }

        private static CustomChest NonNullCustomChest(CustomChest customChest)
        {
            if (customChest is null)
            {
                throw new ArgumentNullException(nameof(customChest));
            }

            return customChest;
        }
    }
}