using furyx639.Common;
using MegaStorage.Framework.Models;
using MegaStorage.Framework.Persistence;
using MegaStorage.Framework.UI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace MegaStorage.Framework.UI
{
    internal enum InventoryType
    {
        Player = 0,
        Chest = 1
    }
    public class CustomItemGrabMenu : ItemGrabMenu
    {
        /*********
        ** Fields
        *********/
        public const int MenuWidth = 840;
        public const int MenuHeight = 736;

        public static readonly Dictionary<string, Rectangle> Categories = new Dictionary<string, Rectangle>()
        {
            {"All", Rectangle.Empty},
            {"Crops", new Rectangle(640, 80, 16, 16)},
            {"Seeds", new Rectangle(656, 64, 16, 16)},
            {"Materials", new Rectangle(672, 64, 16, 16)},
            {"Cooking", new Rectangle(688, 64, 16, 16)},
            {"Fishing", new Rectangle(640, 64, 16, 16)},
            {"Misc", new Rectangle(672, 80, 16, 16)}
        };

        internal CustomChest ActiveChest { get; private set; }
        internal readonly CustomChest ActualChest;
        internal CustomClickableTextureComponent StarButton;

        internal Rectangle GetItemsToGrabMenuBounds => _itemsToGrabMenu.Bounds;
        internal Rectangle GetInventoryBounds => _inventory.Bounds;
        internal Vector2 GetItemsToGrabMenuDimensions => _itemsToGrabMenu.Dimensions;
        internal Vector2 GetInventoryDimensions => _inventory.Dimensions;
        internal Vector2 GetItemsToGrabMenuPosition => _itemsToGrabMenu.Position;
        internal Vector2 GetInventoryPosition => _inventory.Position;

        // Offsets to ItemsToGrabMenu and Inventory
        private static readonly Vector2 Offset = new Vector2(-44, -68);

        // Offsets to Color Picker
        private static readonly Vector2 TopOffset = new Vector2(32, -72);

        // Offsets to Categories
        private static readonly Vector2 LeftOffset = new Vector2(-48, 24);

        // Offsets to Color Toggle, Organize, Stack, OK, and Trash
        private static readonly Vector2 RightOffset = new Vector2(24, -32);

        private CustomInventoryMenu _itemsToGrabMenu;
        private CustomInventoryMenu _inventory;

        private TemporaryAnimatedSprite Poof
        {
            set => _poofReflected.SetValue(value);
        }

        private readonly IReflectedField<TemporaryAnimatedSprite> _poofReflected;
        private behaviorOnItemSelect BehaviorFunction => _behaviorFunction.GetValue();
        private readonly IReflectedField<behaviorOnItemSelect> _behaviorFunction;

        /*********
        ** Public methods
        *********/
        public CustomItemGrabMenu(CustomChest actualChest)
            : base(CommonHelper.NonNull(actualChest).items, actualChest)
        {
            initialize(
                (Game1.viewport.Width - MenuWidth) / 2,
                (Game1.viewport.Height - MenuHeight) / 2,
                MenuWidth,
                MenuHeight);
            if (yPositionOnScreen < IClickableMenu.spaceToClearTopBorder)
                yPositionOnScreen = IClickableMenu.spaceToClearTopBorder;
            if (xPositionOnScreen < 0)
                xPositionOnScreen = 0;

            ActualChest = actualChest;
            ActiveChest = !ActualChest.EnableRemoteStorage || StateManager.MainChest.Equals(ActualChest)
                ? ActualChest
                : StateManager.MainChest;
            allClickableComponents = new List<ClickableComponent>();
            playRightClickSound = true;
            allowRightClick = true;

            _poofReflected = MegaStorageMod.Instance.Helper.Reflection.GetField<TemporaryAnimatedSprite>(this, "poof");
            _behaviorFunction = MegaStorageMod.Instance.Helper.Reflection.GetField<behaviorOnItemSelect>(this, "behaviorFunction");

#pragma warning disable AvoidNetField // Avoid Netcode types when possible
            Game1.player.items.OnElementChanged += Inventory_Changed;
#pragma warning restore AvoidNetField // Avoid Netcode types when possible
            if (!(ActiveChest is null))
            {
                _behaviorFunction.SetValue(ActiveChest.grabItemFromInventory);
                behaviorOnItemGrab = ActiveChest.grabItemFromChest;
                ActiveChest.items.OnElementChanged += Items_Changed;
            }

            SetupItemsMenu();
            SetupInventoryMenu();
        }

        public override void draw(SpriteBatch b)
        {
            if (b is null)
                return;

            // Background
            if (!Game1.options.showMenuBackground)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);
            }

            _itemsToGrabMenu.draw(b);
            _inventory.draw(b);
            chestColorPicker.draw(b);

            // Inventory Icon
            CommonHelper.DrawInventoryIcon(b, _inventory.xPositionOnScreen - 48, _inventory.yPositionOnScreen + 96);

            // Custom Draw
            foreach (var clickableComponent in allClickableComponents
                .OfType<CustomClickableTextureComponent>()
                .Where(c => !(c.DrawAction is null)))
            {
                clickableComponent.DrawAction(b, clickableComponent);
            }

            // Default Draw
            foreach (var clickableComponent in allClickableComponents
                .OfType<ClickableTextureComponent>()
                .Where(c => !(c is CustomClickableTextureComponent customClickableTextureComponent)
                            || customClickableTextureComponent.DrawAction is null))
            {
                clickableComponent.draw(b);
            }

            if (!(hoveredItem is null))
            {
                // Hover Item
                IClickableMenu.drawToolTip(
                    b,
                    hoveredItem.getDescription(),
                    hoveredItem.DisplayName,
                    hoveredItem,
                    !(heldItem is null));
            }
            else if (!(hoverText is null) && hoverAmount > 0)
            {
                // Hover Text w/Amount
                IClickableMenu.drawToolTip(
                    b,
                    hoverText,
                    "",
                    null,
                    true,
                    moneyAmountToShowAtBottom: hoverAmount);
            }
            else if (!(hoverText is null))
            {
                // Hover Text
                IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont);
            }

            // Held Item
            heldItem?.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);

            // Game Cursor
            Game1.mouseCursorTransparency = 1f;
            drawMouse(b);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            heldItem = _inventory.leftClick(x, y, heldItem, playSound);

            chestColorPicker.receiveLeftClick(x, y);
            ActualChest.playerChoiceColor.Value =
                chestColorPicker.getColorFromSelection(chestColorPicker.colorSelection);

            if (ActualChest.EnableRemoteStorage && StateManager.MainChest is null)
            {
                // Cannot use chest
            }
            else if (heldItem is null)
            {
                heldItem = _itemsToGrabMenu.leftClick(x, y, heldItem, false);
                if (!(heldItem is null))
                {
                    behaviorOnItemGrab(heldItem, Game1.player);
                    if (Game1.options.SnappyMenus)
                        snapCursorToCurrentSnappedComponent();
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
                                var key = heldItem.Name.Substring(0,
                                    heldItem.Name.IndexOf("Recipe",
                                        StringComparison.InvariantCultureIgnoreCase) -
                                    1);
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
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                    throw;
                                }

                                heldItem = null;
                            }

                            break;
                    }
                }

                if (!(heldItem is null) && Game1.player.addItemToInventoryBool(heldItem))
                {
                    heldItem = null;
                    Game1.playSound("coin");
                }
            }
            else if (isWithinBounds(x, y))
            {
                BehaviorFunction(heldItem, Game1.player);
            }

            foreach (var clickableComponent in allClickableComponents
                .OfType<CustomClickableTextureComponent>()
                .Where(c => c.containsPoint(x, y) && !(c.LeftClickAction is null)))
            {
                clickableComponent.LeftClickAction(clickableComponent);
            }

            _itemsToGrabMenu.receiveLeftClick(x, y, playSound);
            _inventory.receiveLeftClick(x, y, playSound);
            RefreshItems();
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (!allowRightClick)
            {
                heldItem = _inventory.rightClick(x, y, heldItem, playSound && playRightClickSound, true);
                return;
            }

            heldItem = _inventory.rightClick(x, y, heldItem, playSound && playRightClickSound);
            if (ActualChest.EnableRemoteStorage && StateManager.MainChest is null)
            {
                // Cannot use chest
            }
            else if (heldItem is null)
            {
                heldItem = _itemsToGrabMenu.rightClick(x, y, heldItem, false);
                if (!(heldItem is null))
                {
                    behaviorOnItemGrab(heldItem, Game1.player);
                    if (Game1.options.SnappyMenus)
                        snapCursorToCurrentSnappedComponent();
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
                                var key = heldItem.Name.Substring(0,
                                    heldItem.Name.IndexOf("Recipe",
                                        StringComparison.InvariantCultureIgnoreCase) -
                                    1);
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
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                    throw;
                                }

                                heldItem = null;
                            }

                            break;
                    }
                }

                if (!(heldItem is null) && Game1.player.addItemToInventoryBool(heldItem))
                {
                    heldItem = null;
                    Game1.playSound("coin");
                }
            }
            else if (isWithinBounds(x, y))
            {
                BehaviorFunction(heldItem, Game1.player);
            }

            foreach (var clickableComponent in allClickableComponents
                .OfType<CustomClickableTextureComponent>()
                .Where(c => c.containsPoint(x, y) && !(c.RightClickAction is null)))
            {
                clickableComponent.RightClickAction(clickableComponent);
            }

            _itemsToGrabMenu.receiveRightClick(x, y, playSound && playRightClickSound);
            _inventory.receiveRightClick(x, y, playSound && playRightClickSound);
            RefreshItems();
        }

        public override void receiveScrollWheelAction(int direction)
        {
            var mouseX = Game1.getOldMouseX();
            var mouseY = Game1.getOldMouseY();
            if (chestColorPicker.isWithinBounds(mouseX, mouseY))
            {
                if (direction < 0 && chestColorPicker.colorSelection < chestColorPicker.totalColors - 1)
                {
                    chestColorPicker.colorSelection++;
                }
                else if (direction > 0 && chestColorPicker.colorSelection > 0)
                {
                    chestColorPicker.colorSelection--;
                }

                ((Chest)chestColorPicker.itemToDrawColored).playerChoiceColor.Value =
                    chestColorPicker.getColorFromSelection(chestColorPicker.colorSelection);
                ActualChest.playerChoiceColor.Value =
                    chestColorPicker.getColorFromSelection(chestColorPicker.colorSelection);
            }
            else if (_itemsToGrabMenu.isWithinBounds(mouseX, mouseY))
            {
                _itemsToGrabMenu.receiveScrollWheelAction(direction);
            }
            else
            {
                foreach (var clickableComponent in allClickableComponents
                    .OfType<CustomClickableTextureComponent>()
                    .Where(c => c.containsPoint(mouseX, mouseY) && !(c.ScrollAction is null)))
                {
                    clickableComponent.ScrollAction(direction, clickableComponent);
                }
            }
        }

        public override void performHoverAction(int x, int y)
        {
            hoveredItem = _inventory.hover(x, y, heldItem) ?? _itemsToGrabMenu.hover(x, y, heldItem);
            hoverText = _inventory.hoverText ?? _itemsToGrabMenu.hoverText;
            hoverAmount = 0;
            chestColorPicker.performHoverAction(x, y);

            // Hover Text
            foreach (var clickableComponent in allClickableComponents
                .OfType<CustomClickableTextureComponent>()
                .Where(c => !(c.hoverText is null) && c.containsPoint(x, y)))
            {
                hoverText = clickableComponent.hoverText;
            }

            // Hover Action
            foreach (var clickableComponent in allClickableComponents
                .OfType<CustomClickableTextureComponent>()
                .Where(c => !(c.HoverAction is null)))
            {
                clickableComponent.HoverAction(x, y, clickableComponent);
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            initialize(
                (Game1.viewport.Width - MenuWidth) / 2,
                (Game1.viewport.Height - MenuHeight) / 2,
                MenuWidth,
                MenuHeight);
            if (yPositionOnScreen < IClickableMenu.spaceToClearTopBorder)
                yPositionOnScreen = IClickableMenu.spaceToClearTopBorder;
            if (xPositionOnScreen < 0)
                xPositionOnScreen = 0;

            _itemsToGrabMenu.GameWindowSizeChanged();
            _inventory.GameWindowSizeChanged();

            chestColorPicker.xPositionOnScreen = _itemsToGrabMenu.xPositionOnScreen + (int)TopOffset.X;
            chestColorPicker.yPositionOnScreen = _itemsToGrabMenu.yPositionOnScreen + (int)TopOffset.Y;

            foreach (var clickableComponent in allClickableComponents.OfType<CustomClickableTextureComponent>())
            {
                clickableComponent.GameWindowSizeChanged();
            }
        }

        private CustomChestEventArgs CustomChestEventArgs => new CustomChestEventArgs()
        {
            VisibleItems = _itemsToGrabMenu.VisibleItems,
            AllItems = _itemsToGrabMenu.actualInventory,
            CurrentCategory = _itemsToGrabMenu.SelectedCategory?.name ?? "All",
            HeldItem = heldItem
        };

        internal void RefreshItems()
        {
            if (ActualChest.EnableRemoteStorage && !(ActiveChest is null) && !ActiveChest.Equals(StateManager.MainChest))
            {
                // ReSync to Main Chest
                ActiveChest.items.OnElementChanged -= Items_Changed;
                ActiveChest = StateManager.MainChest;
                ActiveChest.items.OnElementChanged += Items_Changed;

                // Update behavior functions
                behaviorOnItemGrab = ActiveChest.grabItemFromChest;
                _behaviorFunction.SetValue(ActiveChest.grabItemFromInventory);

                // Reassign top inventory
                _itemsToGrabMenu.actualInventory = ActiveChest.items;
            }
            _itemsToGrabMenu.RefreshItems();
            _inventory.RefreshItems();
            MegaStorageApi.InvokeVisibleItemsRefreshed(this, CustomChestEventArgs);
        }

        /*********
        ** Draw
        *********/
        /// <summary>
        /// Draws the Category tab and indents active category
        /// </summary>
        /// <param name="b">The SpriteBatch to draw to</param>
        /// <param name="clickableComponent">The category being drawn</param>
        internal void DrawCategory(SpriteBatch b, CustomClickableTextureComponent clickableComponent)
        {
            if (clickableComponent is ChestCategory chestCategory)
                chestCategory.Draw(b, chestCategory.Equals(_itemsToGrabMenu.SelectedCategory));
        }

        /// <summary>
        /// Draws the Star Button and gray out if inactive
        /// </summary>
        /// <param name="b">The SpriteBatch to draw to</param>
        /// <param name="clickableComponent">The category being drawn</param>
        internal void DrawStarButton(SpriteBatch b, CustomClickableTextureComponent clickableComponent)
        {
            clickableComponent.sourceRect = ActualChest.Equals(ActiveChest)
                ? CommonHelper.StarButtonActive
                : CommonHelper.StarButtonInactive;
            clickableComponent.draw(b, ActualChest.Equals(ActiveChest) ? Color.White : Color.Gray * 0.8f, (float)(0.860000014305115 + clickableComponent.bounds.Y / 20000.0));
        }

        /// <summary>
        /// Draws the Trash Can and the lid
        /// </summary>
        /// <param name="b">The SpriteBatch to draw to</param>
        /// <param name="clickableComponent">The trash can being drawn</param>
        internal void DrawTrashCan(SpriteBatch b, CustomClickableTextureComponent clickableComponent)
        {
            clickableComponent.draw(b);
            b.Draw(
                Game1.mouseCursors,
                new Vector2(clickableComponent.bounds.X + 60, clickableComponent.bounds.Y + 40),
                new Rectangle(564 + Game1.player.trashCanLevel * 18, 129, 18, 10),
                Color.White,
                trashCanLidRotation,
                new Vector2(16f, 10f),
                Game1.pixelZoom,
                SpriteEffects.None,
                0.86f);
        }

        /*********
        ** Left Click
        *********/
        /// <summary>
        /// Toggles the Chest Color Picker on/off
        /// </summary>
        /// <param name="clickableComponent">The toggle button that was clicked</param>
        internal void ClickColorPickerToggleButton(CustomClickableTextureComponent clickableComponent = null)
        {
            Game1.player.showChestColorPicker = !Game1.player.showChestColorPicker;
            chestColorPicker.visible = Game1.player.showChestColorPicker;
            Game1.playSound("drumkit6");
            MegaStorageApi.InvokeColorPickerToggleButtonClicked(this, CustomChestEventArgs);
        }

        /// <summary>
        /// Fills chest inventory from player inventory for items that stack
        /// </summary>
        /// <param name="clickableComponent">The fill button that was clicked</param>
        internal void ClickFillStacksButton(CustomClickableTextureComponent clickableComponent = null)
        {
            MegaStorageApi.InvokeBeforeFillStacksButtonClicked(this, CustomChestEventArgs);
            FillOutStacks();
            Game1.player.Items = _inventory.actualInventory;
            Game1.playSound("Ship");
            MegaStorageApi.InvokeAfterFillStacksButtonClicked(this, CustomChestEventArgs);
        }

        /// <summary>
        /// Sorts chest inventory
        /// </summary>
        /// <param name="clickableComponent">The organize button that was clicked</param>
        internal void ClickOrganizeButton(CustomClickableTextureComponent clickableComponent = null)
        {
            MegaStorageApi.InvokeBeforeOrganizeButtonClicked(this, CustomChestEventArgs);
            organizeItemsInList(_itemsToGrabMenu.actualInventory);
            Game1.playSound("Ship");
            MegaStorageApi.InvokeAfterOrganizeButtonClicked(this, CustomChestEventArgs);
        }

        /// <summary>
        /// Makes this the main chest for remote storage
        /// </summary>
        /// <param name="clickableComponent">The star button that was clicked</param>
        internal void ClickStarButton(CustomClickableTextureComponent clickableComponent = null)
        {
            if (!Context.IsMainPlayer)
                return;

            MegaStorageApi.InvokeBeforeStarButtonClicked(this, CustomChestEventArgs);
            if (ActiveChest is null || !ActualChest.Equals(ActiveChest))
            {
                clickableComponent.sourceRect = CommonHelper.StarButtonActive;

                if (!(ActiveChest is null))
                {
                    // Move items from main chest to this chest
                    ActiveChest.items.OnElementChanged -= Items_Changed;
                    ActualChest.items.CopyFrom(StateManager.MainChest.items);
                    ActiveChest.items.Clear();
                }

                // Assign Main Chest to Current Chest
                StateManager.MainChest = ActualChest;
                ActiveChest = ActualChest;
                ActiveChest.items.OnElementChanged += Items_Changed;

                // Update behavior functions
                behaviorOnItemGrab = ActiveChest.grabItemFromChest;
                _behaviorFunction.SetValue(ActiveChest.grabItemFromInventory);

                // Reassign top inventory
                _itemsToGrabMenu.actualInventory = ActiveChest.items;
                _itemsToGrabMenu.RefreshItems();
            }
            MegaStorageApi.InvokeAfterStarButtonClicked(this, CustomChestEventArgs);
        }

        /// <summary>
        /// Exits the chest menu
        /// </summary>
        /// <param name="clickableComponent">The ok button that was clicked</param>
        internal void ClickOkButton(CustomClickableTextureComponent clickableComponent = null)
        {
            MegaStorageApi.InvokeBeforeOkButtonClicked(this, CustomChestEventArgs);
            exitThisMenu();
            if (!(Game1.currentLocation.currentEvent is null))
                ++Game1.currentLocation.currentEvent.CurrentCommand;
            Game1.playSound("bigDeSelect");
            MegaStorageApi.InvokeAfterOkButtonClicked(this, CustomChestEventArgs);
        }

        /// <summary>
        /// Trashes the currently held item
        /// </summary>
        /// <param name="clickableComponent">The trash can that was clicked</param>
        internal void ClickTrashCan(CustomClickableTextureComponent clickableComponent = null)
        {
            MegaStorageApi.InvokeBeforeTrashCanClicked(this, CustomChestEventArgs);
            if (heldItem is null)
                return;
            Utility.trashItem(heldItem);
            heldItem = null;
            MegaStorageApi.InvokeAfterTrashCanClicked(this, CustomChestEventArgs);
        }

        /// <summary>
        /// Switches the chest menu's currently selected category
        /// </summary>
        /// <param name="categoryName">The name of the category to switch to</param>
        internal void ClickCategoryButton(string categoryName)
        {
            var clickableComponent = allClickableComponents
                .OfType<ChestCategory>()
                .First(c => c.name.Equals(categoryName, StringComparison.InvariantCultureIgnoreCase));
            ClickCategoryButton(clickableComponent);
        }

        /// <summary>
        /// Switches the chest menu's currently selected category
        /// </summary>
        /// <param name="clickableComponent">The category button that was clicked</param>
        internal void ClickCategoryButton(CustomClickableTextureComponent clickableComponent)
        {
            MegaStorageApi.InvokeBeforeCategoryChanged(this, CustomChestEventArgs);
            if (clickableComponent is ChestCategory chestCategory)
                _itemsToGrabMenu.SelectedCategory = chestCategory;
            MegaStorageApi.InvokeAfterCategoryChanged(this, CustomChestEventArgs);
        }

        /*********
        ** Scroll
        *********/
        /// <summary>
        /// Scrolls the chest menu's currently selected category
        /// </summary>
        /// <param name="direction">The direction to scroll in</param>
        /// <param name="clickableComponent">The category that is being hovered over</param>
        internal void ScrollCategory(int direction, CustomClickableTextureComponent clickableComponent = null)
        {
            ChestCategory savedCategory = null;
            ChestCategory beforeCategory = null;
            ChestCategory nextCategory = null;
            foreach (var currentCategory in allClickableComponents.OfType<ChestCategory>())
            {
                if (savedCategory == _itemsToGrabMenu.SelectedCategory)
                {
                    nextCategory = currentCategory;
                    break;
                }
                else
                {
                    beforeCategory = savedCategory;
                }
                savedCategory = currentCategory;
            }
            MegaStorageApi.InvokeBeforeCategoryChanged(this, CustomChestEventArgs);
            if (direction < 0 && !(nextCategory is null))
            {
                _itemsToGrabMenu.SelectedCategory = nextCategory;
            }
            else if (direction > 0 && !(beforeCategory is null))
            {
                _itemsToGrabMenu.SelectedCategory = beforeCategory;
            }
            MegaStorageApi.InvokeAfterCategoryChanged(this, CustomChestEventArgs);
        }

        /*********
        ** Hover
        *********/
        /// <summary>
        /// Zooms in on the hovered component
        /// </summary>
        /// <param name="x">The X-coordinate of the mouse</param>
        /// <param name="y">The Y-coordinate of the mouse</param>
        /// <param name="clickableComponent">The item being hovered over</param>
        internal void HoverZoom(int x, int y, CustomClickableTextureComponent clickableComponent)
        {
            clickableComponent.scale = clickableComponent.containsPoint(x, y)
                ? Math.Min(1.1f, clickableComponent.scale + 0.05f)
                : Math.Max(1f, clickableComponent.scale - 0.05f);
        }

        /// <summary>
        /// Zooms in on the hovered component (scaled up by Game1.pixelZoom)
        /// </summary>
        /// <param name="x">The X-coordinate of the mouse</param>
        /// <param name="y">The Y-coordinate of the mouse</param>
        /// <param name="clickableComponent">The item being hovered over</param>
        internal void HoverPixelZoom(int x, int y, CustomClickableTextureComponent clickableComponent)
        {
            clickableComponent.scale = clickableComponent.containsPoint(x, y)
                ? Math.Min(Game1.pixelZoom * 1.1f, clickableComponent.scale + 0.05f)
                : Math.Max(Game1.pixelZoom * 1f, clickableComponent.scale - 0.05f);
        }

        /// <summary>
        /// Rotates the trash can lid while hovering over the trash can
        /// </summary>
        /// <param name="x">The X-coordinate of the mouse</param>
        /// <param name="y">The Y-coordinate of the mouse</param>
        /// <param name="clickableComponent">The trash can being hovered over</param>
        internal void HoverTrashCan(int x, int y, CustomClickableTextureComponent clickableComponent)
        {
            if (!clickableComponent.containsPoint(x, y))
            {
                trashCanLidRotation = Math.Max(trashCanLidRotation - (float)Math.PI / 48f, 0.0f);
                return;
            }

            if (trashCanLidRotation <= 0f)
                Game1.playSound("trashcanlid");
            trashCanLidRotation = Math.Min(trashCanLidRotation + (float)Math.PI / 48f, 1.570796f);

            if (heldItem is null || Utility.getTrashReclamationPrice(heldItem, Game1.player) <= 0)
                return;
            hoverText = Game1.content.LoadString("Strings\\UI:TrashCanSale");
            hoverAmount = Utility.getTrashReclamationPrice(heldItem, Game1.player);
        }

        /*********
        ** Private methods
        *********/
        /// <summary>
        /// Configures all UI elements related to the top menu
        /// </summary>
        private void SetupItemsMenu()
        {
            _itemsToGrabMenu = new CustomInventoryMenu(
                this,
                Offset,
                InventoryType.Chest);
            ItemsToGrabMenu = _itemsToGrabMenu;

            // Inventory (Clickable Component)
            for (var slot = 0; slot < _itemsToGrabMenu.inventory.Count; ++slot)
            {
                var cc = _itemsToGrabMenu.inventory.ElementAt(slot);
                var col = slot % CustomInventoryMenu.ItemsPerRow;
                var row = slot / CustomInventoryMenu.ItemsPerRow;

                cc.myID += 53910;
                cc.fullyImmutable = true;

                // Top row adjustment
                if (row == 0)
                    cc.upNeighborID = 4343;
                else
                    cc.upNeighborID += 53910;

                // Bottom row adjustment
                if (row == _itemsToGrabMenu.rows)
                    cc.downNeighborID = col;
                else
                    cc.downNeighborID += 53910;

                // Left column adjustment
                if (col == CustomInventoryMenu.ItemsPerRow - 1)
                {
                    cc.rightNeighborID = row switch
                    {
                        0 => 27346, // Color Toggle Button
                        1 => 27346,
                        2 => 12952, // Fill Stacks
                        3 => 12952,
                        4 => 106, // Organize
                        5 => 106,
                        _ => 106
                    };
                }
                else
                {
                    cc.leftNeighborID += 53910;
                }

                // Right column adjustment
                if (col == 0)
                {
                    cc.leftNeighborID = row switch
                    {
                        0 => 239865, // Chest Category 1
                        1 => 239866, // Chest Category 2
                        2 => 239867, // Chest Category 3
                        3 => 239868, // Chest Category 4
                        4 => 239869, // Chest Category 5
                        5 => 239870, // Chest Category 6
                        _ => 239810
                    };
                }
                else
                {
                    cc.rightNeighborID += 53910;
                }
            }

            // Color Picker
            chestColorPicker = new DiscreteColorPicker(
                _itemsToGrabMenu.xPositionOnScreen + (int)TopOffset.X,
                _itemsToGrabMenu.yPositionOnScreen + (int)TopOffset.Y,
                0,
                new Chest(true));
            chestColorPicker.colorSelection =
                chestColorPicker.getSelectionFromColor(ActualChest.playerChoiceColor.Value);
            ((Chest)chestColorPicker.itemToDrawColored).playerChoiceColor.Value =
                chestColorPicker.getColorFromSelection(chestColorPicker.colorSelection);

            // Chest Color Picker (Clickable Component)
            discreteColorPickerCC = new List<ClickableComponent>();
            for (var index = 0; index < chestColorPicker.totalColors; ++index)
            {
                var discreteColorPicker = new ClickableComponent(new Rectangle(chestColorPicker.xPositionOnScreen + IClickableMenu.borderWidth / 2 + index * 9 * 4, chestColorPicker.yPositionOnScreen + IClickableMenu.borderWidth / 2, 36, 28), "")
                {
                    myID = index + 4343,
                    rightNeighborID = index < chestColorPicker.totalColors - 1 ? index + 4343 + 1 : -1,
                    leftNeighborID = index > 0 ? index + 4343 - 1 : -1,
                    downNeighborID = 53910
                };
                discreteColorPickerCC.Add(discreteColorPicker);
                allClickableComponents.Add(discreteColorPicker);
            }

            // Color Picker Toggle
            colorPickerToggleButton = new CustomClickableTextureComponent(
                "colorPickerToggleButton",
                _itemsToGrabMenu,
                RightOffset + _itemsToGrabMenu.Dimensions * new Vector2(1, 1f / 4f),
                Game1.mouseCursors,
                new Rectangle(119, 469, 16, 16),
                Game1.content.LoadString("Strings\\UI:Toggle_ColorPicker"))
            {
                myID = 27346,
                downNeighborID = 12952,
                leftNeighborID = 53933,
                region = 15923,
                LeftClickAction = ClickColorPickerToggleButton
            };
            allClickableComponents.Add(colorPickerToggleButton);

            // Stack
            fillStacksButton = new CustomClickableTextureComponent(
                "fillStacksButton",
                _itemsToGrabMenu,
                RightOffset + _itemsToGrabMenu.Dimensions * new Vector2(1, 2f / 4f),
                Game1.mouseCursors,
                new Rectangle(103, 469, 16, 16),
                Game1.content.LoadString("Strings\\UI:ItemGrab_FillStacks"))
            {
                myID = 12952,
                upNeighborID = 27346,
                downNeighborID = 106,
                leftNeighborID = 53957,
                region = 15923,
                LeftClickAction = ClickFillStacksButton,
                HoverAction = HoverPixelZoom
            };
            allClickableComponents.Add(fillStacksButton);

            // Organize
            organizeButton = new CustomClickableTextureComponent(
                "organizeButton",
                _itemsToGrabMenu,
                RightOffset + _itemsToGrabMenu.Dimensions * new Vector2(1, 3f / 4f),
                Game1.mouseCursors,
                new Rectangle(162, 440, 16, 16),
                Game1.content.LoadString("Strings\\UI:ItemGrab_Organize"))
            {
                myID = 106,
                upNeighborID = 12952,
                downNeighborID = 5948,
                leftNeighborID = 53969,
                region = 15923,
                LeftClickAction = ClickOrganizeButton,
                HoverAction = HoverPixelZoom
            };
            allClickableComponents.Add(organizeButton);

            // Star
            if (ActualChest.EnableRemoteStorage)
            {
                StarButton = new CustomClickableTextureComponent(
                    "starButton",
                    _itemsToGrabMenu,
                    new Vector2(-Game1.tileSize, -Game1.tileSize),
                    Game1.mouseCursors,
                    StateManager.MainChest == ActualChest
                        ? CommonHelper.StarButtonActive
                        : CommonHelper.StarButtonInactive)
                {
                    myID = 239864,
                    downNeighborID = 239865,
                    rightNeighborID = 4343,
                    DrawAction = DrawStarButton,
                    LeftClickAction = ClickStarButton,
                    HoverAction = HoverPixelZoom
                };
                allClickableComponents.Add(StarButton);
            }

            // Categories
            if (!ActualChest.EnableCategories)
                return;

            for (var index = 0; index < Categories.Count; ++index)
            {
                var category = Categories.ElementAt(index);
                if (!ModConfig.Instance.Categories.TryGetValue(category.Key, out var categoryIds) &&
                    !category.Key.Equals("All", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                var categoryCC = category.Key switch
                {
                    "All" => new AllCategory(
                        category.Key,
                        _itemsToGrabMenu,
                        LeftOffset + new Vector2(0, index * 60),
                        category.Value),
                    "Misc" => new MiscCategory(
                        category.Key,
                        _itemsToGrabMenu,
                        LeftOffset + new Vector2(0, index * 60),
                        category.Value,
                        categoryIds),
                    _ => new ChestCategory(
                        category.Key,
                        _itemsToGrabMenu,
                        LeftOffset + new Vector2(0, index * 60),
                        category.Value,
                        categoryIds)
                };

                categoryCC.myID = index + 239865;
                categoryCC.upNeighborID = index > 0 || ActualChest.EnableRemoteStorage ? index + 239864 : 4343;
                categoryCC.downNeighborID = index < Categories.Count - 1 ? index + 239866 : 1;
                categoryCC.rightNeighborID = index switch
                {
                    0 => 53910, // ItemsToGrabMenu.inventory Row 1 Col 1
                    1 => 53922, // ItemsToGrabMenu.inventory Row 2 Col 1
                    2 => 53934, // ItemsToGrabMenu.inventory Row 3 Col 1
                    3 => 53946, // ItemsToGrabMenu.inventory Row 4 Col 1
                    4 => 53946, // ItemsToGrabMenu.inventory Row 4 Col 1
                    5 => 53958, // ItemsToGrabMenu.inventory Row 5 Col 1
                    6 => 53970, // ItemsToGrabMenu.inventory Row 6 Col 1
                    _ => 53970
                };
                categoryCC.DrawAction = DrawCategory;
                categoryCC.LeftClickAction = ClickCategoryButton;
                categoryCC.ScrollAction = ScrollCategory;

                allClickableComponents.Add(categoryCC);
            }
            _itemsToGrabMenu.SelectedCategory = allClickableComponents.OfType<ChestCategory>().First();
        }
        /// <summary>
        /// Configures all the UI elements related to the bottom menu
        /// </summary>
        private void SetupInventoryMenu()
        {
            _inventory = new CustomInventoryMenu(
                this,
                new Vector2(0, _itemsToGrabMenu.height) + Offset,
                InventoryType.Player);
            inventory = _inventory;

            // Inventory (Clickable Component)
            for (var slot = 0; slot < _inventory.inventory.Count; ++slot)
            {
                var cc = _itemsToGrabMenu.inventory.ElementAt(slot);
                var col = slot % CustomInventoryMenu.ItemsPerRow;
                var row = slot / CustomInventoryMenu.ItemsPerRow;

                // Top row adjustment
                if (row == 0)
                    cc.upNeighborID = _itemsToGrabMenu.inventory.Count > slot ? 53910 + slot : 4343;

                // Right column adjustment
                if (col == CustomInventoryMenu.ItemsPerRow - 1)
                    cc.rightNeighborID = row < 2 ? 5948 : 4857;
            }

            // OK Button
            okButton = new CustomClickableTextureComponent(
                "okButton",
                _inventory,
                new Vector2(_inventory.width, 204) + RightOffset,
                Game1.mouseCursors,
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46),
                scale: 1f)
            {
                myID = 4857,
                upNeighborID = 5948,
                leftNeighborID = 11,
                LeftClickAction = ClickOkButton,
                HoverAction = HoverZoom
            };
            allClickableComponents.Add(okButton);

            // Trash Can
            trashCan = new CustomClickableTextureComponent(
                "trashCan",
                _inventory,
                new Vector2(_inventory.width, 68) + RightOffset,
                Game1.mouseCursors,
                new Rectangle(564 + Game1.player.trashCanLevel * 18, 102, 18, 26),
                width: Game1.tileSize,
                height: 104)
            {
                myID = 5948,
                downNeighborID = 4857,
                leftNeighborID = 23,
                upNeighborID = 106,
                DrawAction = DrawTrashCan,
                LeftClickAction = ClickTrashCan,
                HoverAction = HoverTrashCan
            };
            allClickableComponents.Add(trashCan);

            // Add Invisible Drop Item Button?
            dropItemInvisibleButton = new ClickableComponent(
                new Rectangle(xPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 128, yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + 164, Game1.tileSize, Game1.tileSize), "")
            {
                myID = 107
            };
            allClickableComponents.Add(dropItemInvisibleButton);
        }

        private void Items_Changed(Netcode.NetList<Item, Netcode.NetRef<Item>> list, int index, Item oldValue, Item newValue)
        {
            _itemsToGrabMenu.RefreshItems();
            MegaStorageApi.InvokeVisibleItemsRefreshed(this, CustomChestEventArgs);
        }

        private void Inventory_Changed(Netcode.NetList<Item, Netcode.NetRef<Item>> list, int index, Item oldValue, Item newValue)
        {
            _inventory.RefreshItems();
            MegaStorageApi.InvokeVisibleItemsRefreshed(this, CustomChestEventArgs);
        }

        private static TemporaryAnimatedSprite CreatePoof(int x, int y) => new TemporaryAnimatedSprite(
            "TileSheets/animations",
            new Rectangle(0, 320, Game1.tileSize, Game1.tileSize),
            50f,
            8,
            0,
            new Vector2(x - x % Game1.tileSize + 16, y - y % Game1.tileSize + 16),
            false,
            false);
    }
}