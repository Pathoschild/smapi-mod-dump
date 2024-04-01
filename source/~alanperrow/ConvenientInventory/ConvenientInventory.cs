/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alanperrow/StardewModding
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using ConvenientInventory.Compatibility;
using ConvenientInventory.TypedChests;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ConvenientInventory
{
    internal static class CachedTextures
    {
        public static Texture2D Mill { get; } = ModEntry.Instance.Helper.GameContent.Load<Texture2D>(@"Buildings\Mill");

        public static Texture2D JunimoHut { get; } = ModEntry.Instance.Helper.GameContent.Load<Texture2D>(@"Buildings\Junimo Hut");

        public static Texture2D FarmHouse { get; } = ModEntry.Instance.Helper.GameContent.Load<Texture2D>(@"Maps\farmhouse_tiles");
    }

    public static class ConvenientInventory
    {
        public static Texture2D QuickStackButtonIcon { private get; set; }

        private static IReadOnlyList<ITypedChest> NearbyTypedChests { get; set; }

        private static ClickableTextureComponent QuickStackButton { get; set; }

        private static readonly PerScreen<InventoryPage> playerInventoryPage = new();
        private static InventoryPage PlayerInventoryPage
        {
            get { return playerInventoryPage.Value; }
            set { playerInventoryPage.Value = value; }
        }

        private static bool IsDrawToolTip { get; set; } = false;

        private const int quickStackButtonID = 918021;  // Unique indentifier

        private static readonly List<ItemGrabMenu.TransferredItemSprite> transferredItemSprites = new();

        public static Texture2D FavoriteItemsCursorTexture { private get; set; }

        public static Texture2D FavoriteItemsHighlightTexture { private get; set; }

        public static Texture2D FavoriteItemsBorderTexture { private get; set; }

        private static readonly PerScreen<bool> isFavoriteItemsHotkeyDown = new();
        public static bool IsFavoriteItemsHotkeyDown
        {
            get { return isFavoriteItemsHotkeyDown.Value; }
            set { isFavoriteItemsHotkeyDown.Value = value; }
        }

        private static readonly PerScreen<int> favoriteItemsHotkeyDownCounter = new();
        private static int FavoriteItemsHotkeyDownCounter
        {
            get { return favoriteItemsHotkeyDownCounter.Value; }
            set { favoriteItemsHotkeyDownCounter.Value = value; }
        }

        private static readonly string favoriteItemSlotsModDataKey = $"{ModEntry.Instance.ModManifest.UniqueID}/favoriteItemSlots";

        private static readonly PerScreen<bool[]> favoriteItemSlots = new();
        public static bool[] FavoriteItemSlots
        {
            get
            {
                if (favoriteItemSlots.Value is null)
                {
                    LoadFavoriteItemSlots();
                }

                if (InventoryExpansions.IsPlayerMaxItemsChanged(favoriteItemSlots.Value))
                {
                    favoriteItemSlots.Value = InventoryExpansions.ResizeFavoriteItemSlots(favoriteItemSlots.Value, Math.Max(Game1.player.MaxItems, Game1.player.Items.Count));
                }

                return favoriteItemSlots.Value;
            }
            set { favoriteItemSlots.Value = value; }
        }

        private static readonly PerScreen<bool> favoriteItemsIsItemSelected = new();
        public static bool FavoriteItemsIsItemSelected
        {
            get { return favoriteItemsIsItemSelected.Value; }
            set { favoriteItemsIsItemSelected.Value = value; }
        }

        private static readonly PerScreen<Item> favoriteItemsSelectedItem = new();
        public static Item FavoriteItemsSelectedItem
        {
            get { return favoriteItemsSelectedItem.Value; }
            set { favoriteItemsSelectedItem.Value = value; }
        }

        private static readonly PerScreen<int> favoriteItemsLastSelectedSlot = new();
        public static int FavoriteItemsLastSelectedSlot
        {
            get { return favoriteItemsLastSelectedSlot.Value; }
            set { favoriteItemsLastSelectedSlot.Value = value; }
        }

        public static bool[] LoadFavoriteItemSlots()
        {
            Game1.player.modData.TryGetValue(favoriteItemSlotsModDataKey, out string dataStr);

            FavoriteItemSlots = dataStr?
                .Select(x => x == '1')
                .ToArray()
                ?? new bool[Game1.player.MaxItems];

            dataStr ??= new string('0', FavoriteItemSlots.Length);
            ModEntry.Instance.Monitor.Log($"Favorite item slots loaded for {Game1.player.Name}: '{dataStr}'.", StardewModdingAPI.LogLevel.Trace);
            return FavoriteItemSlots;
        }

        public static string SaveFavoriteItemSlots()
        {
            if (FavoriteItemSlots is null)
            {
                LoadFavoriteItemSlots();
            }

            var saveStr = new string(FavoriteItemSlots.Select(x => x ? '1' : '0').ToArray());

            Game1.player.modData[favoriteItemSlotsModDataKey] = saveStr;

            ModEntry.Instance.Monitor.Log($"Favorite item slots saved to {Game1.player.Name}.modData: '{saveStr}'.", StardewModdingAPI.LogLevel.Trace);
            return saveStr;
        }

        public static void InventoryPageConstructor(InventoryPage inventoryPage, int x, int y, int width, int height)
        {
            PlayerInventoryPage = inventoryPage;

            if (ModEntry.Config.IsEnableQuickStack)
            {
                QuickStackButton = new ClickableTextureComponent("",
                    new Rectangle(inventoryPage.xPositionOnScreen + width, inventoryPage.yPositionOnScreen + height / 3 - 64 + 8 + 80, 64, 64),
                    string.Empty,
                    ModEntry.Instance.Helper.Translation.Get("QuickStackButton.hoverText"),
                    QuickStackButtonIcon,
                    Rectangle.Empty,
                    4f,
                    false)
                {
                    myID = quickStackButtonID,
                    downNeighborID = 105,  // trash can
                    upNeighborID = 106,  // organize button
                    leftNeighborID = 11  // top-right inventory slot
                };

                inventoryPage.organizeButton.downNeighborID = quickStackButtonID;
                inventoryPage.trashCan.upNeighborID = quickStackButtonID;
            }

            if (ModEntry.Config.IsEnableInventoryPageSideWarp)
            {
                if (InventoryPage.ShouldShowJunimoNoteIcon())
                {
                    inventoryPage.inventory.dropItemInvisibleButton.leftNeighborID = inventoryPage.junimoNoteIcon.myID;
                    inventoryPage.junimoNoteIcon.leftNeighborID = inventoryPage.inventory.dropItemInvisibleButton.myID;
                }
                else
                {
                    inventoryPage.inventory.dropItemInvisibleButton.leftNeighborID = inventoryPage.organizeButton.myID;
                }

                inventoryPage.organizeButton.rightNeighborID = inventoryPage.inventory.dropItemInvisibleButton.myID;

                if (ModEntry.Config.IsEnableQuickStack)
                {
                    QuickStackButton.rightNeighborID = inventoryPage.inventory.dropItemInvisibleButton.myID;
                }
            }
        }

        public static bool PreReceiveLeftClickInMenu<T>(T menu, int x, int y) where T : IClickableMenu
        {
            if (ModEntry.Config.IsEnableFavoriteItems)
            {
                InventoryMenu inventory = (menu as InventoryPage)?.inventory    // Player menu - inventory tab
                    ?? (menu as CraftingPage)?.inventory                        // Player menu - crafting tab
                    ?? (menu as ShopMenu)?.inventory                            // Shop menu
                    ?? (menu as MenuWithInventory)?.inventory;                  // Arbitrary menu

                if (IsFavoriteItemsHotkeyDown)
                {
                    ToggleFavoriteItemSlotAtClickPosition(inventory, x, y);

                    // Always ignore the click action in this case; should only allow favorite item slot toggling.
                    return false;
                }
                else
                {
                    TrackSelectedFavoriteItemSlotAtClickPosition(inventory, x, y);
                }
            }

            return true;
        }

        public static bool PreReceiveRightClickInMenu<T>(T menu, int x, int y) where T : IClickableMenu
        {
            if (ModEntry.Config.IsEnableFavoriteItems)
            {
                if (IsFavoriteItemsHotkeyDown)
                {
                    return false;
                }

                InventoryMenu inventory = (menu as InventoryPage)?.inventory    // Player menu - inventory tab
                    ?? (menu as CraftingPage)?.inventory                        // Player menu - crafting tab
                    ?? (menu as ShopMenu)?.inventory                            // Shop menu
                    ?? (menu as MenuWithInventory)?.inventory;                  // Arbitrary menu

                TrackSelectedFavoriteItemSlotAtClickPosition(inventory, x, y, isRightClick: true);
            }

            return true;
        }

        // Toggles the favorited status of a selected item slot. Returns whether an item was toggled.
        private static bool ToggleFavoriteItemSlotAtClickPosition(InventoryMenu inventoryMenu, int clickX, int clickY, bool? favoriteOverride = null)
        {
            if (inventoryMenu is null)
            {
                return false;
            }

            int clickPos = inventoryMenu.getInventoryPositionOfClick(clickX, clickY);

            // Only allow favoriting if selected slot contains an item. Always allow unfavoriting.
            if (clickPos != -1 && inventoryMenu.actualInventory.Count > clickPos && (FavoriteItemSlots[clickPos] || inventoryMenu.actualInventory[clickPos] != null))
            {
                ModEntry.Instance.Monitor
                    .Log($"{(FavoriteItemSlots[clickPos] ? "Un-" : string.Empty)}Favorited item slot {clickPos}: {inventoryMenu.actualInventory[clickPos]?.Name}",
                    StardewModdingAPI.LogLevel.Trace);

                Game1.playSound("smallSelect");

                FavoriteItemSlots[clickPos] = (favoriteOverride is null)
                    ? !FavoriteItemSlots[clickPos]
                    : favoriteOverride.Value;

                return true;
            }

            return false;
        }

        // Tracks and "moves" favorite item slots when selecting/de-selecting items in an inventory menu.
        private static bool TrackSelectedFavoriteItemSlotAtClickPosition(InventoryMenu inventoryMenu, int clickX, int clickY, bool isRightClick = false)
        {
            if (!ModEntry.Config.IsEnableFavoriteItems || IsFavoriteItemsHotkeyDown || inventoryMenu is null)
            {
                return false;
            }

            int clickPos = inventoryMenu.getInventoryPositionOfClick(clickX, clickY);

            if (!FavoriteItemsIsItemSelected || isRightClick)
            {
                if (clickPos != -1 && inventoryMenu.actualInventory.Count > clickPos && inventoryMenu.actualInventory[clickPos] != null)
                {
                    // We've selected a slot with an item in it
                    Item clickedItem = inventoryMenu.actualInventory[clickPos];

                    if (FavoriteItemSlots[clickPos])
                    {
                        // We've selected a favorited item slot with an item in it
                        if (!isRightClick)
                        {
                            if (Game1.oldKBState.IsKeyDown(Keys.LeftShift) && Game1.activeClickableMenu is GameMenu gameMenuIP && gameMenuIP.pages[gameMenuIP.currentTab] is InventoryPage)
                            {
                                // Game logic for shift-clicking in player's inventory page
                                HandleFavoriteItemSlotShiftClickedInInventoryPage(clickPos, clickedItem);
                            }
                            else if (Game1.oldKBState.IsKeyDown(Keys.LeftShift) && Game1.activeClickableMenu is GameMenu gameMenuCP && gameMenuCP.pages[gameMenuCP.currentTab] is CraftingPage
                                && (clickedItem is Hat or Clothing or Boots or Ring or StardewValley.Tools.MeleeWeapon))
                            {
                                // Game logic for shift-clicking in player's crafting page. Idk why it works this way, but this handles it.
                                HandleFavoriteItemSlotShiftClickedInCraftingPage(clickPos, clickedItem);
                            }
                            else
                            {
                                Item cursorSlotItem = GetHeldItemOrCursorSlotItem();

                                if ((cursorSlotItem is null || !cursorSlotItem.canStackWith(clickedItem)) && inventoryMenu.highlightMethod(clickedItem))
                                {
                                    // Left click with either (1.) no item currently selected, or (2.) item selected that cannot stack with the clicked slot, and (3.) clicked slot is not greyed out.
                                    if (!IsCurrentActiveMenuNoHeldItems())
                                    {
                                        StartTrackingFavoriteItemSlot(clickPos, clickedItem);
                                    }
                                    else
                                    {
                                        // Shop menus only allow held items after purchasing something, so we check for that case here.
                                        if (Game1.activeClickableMenu is ShopMenu shopMenu)
                                        {
                                            if (shopMenu.heldItem == null && inventoryMenu.highlightMethod(clickedItem))
                                            {
                                                FavoriteItemSlots[clickPos] = false;
                                            }
                                        }
                                        else
                                        {
                                            FavoriteItemSlots[clickPos] = false;
                                        }
                                    }
                                }
                            }
                        }
                        else if (isRightClick && clickedItem is Tool clickedTool)
                        {
                            Item cursorSlotItem = GetHeldItemOrCursorSlotItem();

                            // Right-click attachments into a favorited tool
                            if (cursorSlotItem != null)
                            {
                                if (clickedTool.canThisBeAttached((StardewValley.Object)cursorSlotItem))
                                {
                                    // We are allowed to attach this item to our tool
                                    var atts = clickedTool.attachments.Where(x => x != null && x.canStackWith(cursorSlotItem));
                                    if (atts.Any())
                                    {
                                        // We found an existing attachment to try to stack with our item
                                        if (atts.Any(x => !IsOverMaxStackSize(x, cursorSlotItem)))
                                        {
                                            // We fully stacked our item and are no longer holding anything
                                            ResetFavoriteItemSlotsTracking();
                                        }
                                    }
                                    else if (clickedTool.attachments.Any(x => x == null))
                                    {
                                        // We did not find any existing attachments to stack with our item, so this item itself will be attached
                                        ResetFavoriteItemSlotsTracking();
                                    }
                                    else if (!clickedTool.attachments.Any(x => cursorSlotItem.CompareTo(x) == 0))
                                    {
                                        // We did not find any existing attachments to stack with our item, and there are no empty attachment slots,
                                        // so this item itself will be swapped with one of the existing attachments
                                        ResetFavoriteItemSlotsTracking();
                                    }
                                }
                            }
                        }
                        else if (isRightClick && clickedItem.Stack == 1 && inventoryMenu.highlightMethod(clickedItem))
                        {
                            // Right click, taking the last item
                            if (!IsCurrentActiveMenuNoHeldItems())
                            {
                                StartTrackingFavoriteItemSlot(clickPos, clickedItem);
                            }
                            else
                            {
                                // Shop menus only allow held items after purchasing something, so we check for that case here.
                                if (Game1.activeClickableMenu is ShopMenu shopMenu)
                                {
                                    if ((shopMenu.heldItem == null || shopMenu.heldItem.canStackWith(clickedItem)) && inventoryMenu.highlightMethod(clickedItem))
                                    {
                                        StartTrackingFavoriteItemSlot(clickPos, clickedItem);
                                    }
                                }
                                else
                                {
                                    FavoriteItemSlots[clickPos] = false;
                                }
                            }
                        }
                    }
                    else if (isRightClick && clickedItem is Tool clickedTool)
                    {
                        // We've right clicked an unfavorited slot with a tool in it
                        Item cursorSlotItem = GetHeldItemOrCursorSlotItem();

                        // Right-click favorited attachments into an unfavorited tool
                        if (cursorSlotItem != null)
                        {
                            if (clickedTool.canThisBeAttached((StardewValley.Object)cursorSlotItem))
                            {
                                // We are allowed to attach this item to our tool
                                var atts = clickedTool.attachments.Where(x => x != null && x.canStackWith(cursorSlotItem));
                                if (atts.Any())
                                {
                                    // We found an existing attachment to try to stack with our item
                                    if (atts.Any(x => !IsOverMaxStackSize(x, cursorSlotItem)))
                                    {
                                        // We fully stacked our item and are no longer holding anything
                                        ResetFavoriteItemSlotsTracking();
                                    }
                                }
                                else if (clickedTool.attachments.Any(x => x == null))
                                {
                                    // We did not find any existing attachments to stack with our item, so this item itself will be attached
                                    ResetFavoriteItemSlotsTracking();
                                }
                                else if (!clickedTool.attachments.Any(x => cursorSlotItem.CompareTo(x) == 0))
                                {
                                    // We did not find any existing attachments to stack with our item, and there are no empty attachment slots,
                                    // so this item itself will be swapped with one of the existing attachments
                                    ResetFavoriteItemSlotsTracking();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (clickPos != -1 && inventoryMenu.actualInventory.Count > clickPos && !isRightClick)
                {
                    // We have a favorited item selected and have clicked a valid inventory slot.
                    Item clickedItem = inventoryMenu.actualInventory[clickPos];

                        if (FavoriteItemSlots[clickPos] && clickedItem != null && inventoryMenu.highlightMethod(clickedItem))
                        {
                            Item cursorSlotItem = GetHeldItemOrCursorSlotItem();

                            // We are placing the selected favorited item into a favorited slot with an item in it.
                            if (cursorSlotItem != null && cursorSlotItem.canStackWith(clickedItem))
                            {
                                if (!IsOverMaxStackSize(cursorSlotItem, clickedItem))
                                {
                                    // Clicked item can stack with ours, so stop tracking.
                                    ResetFavoriteItemSlotsTracking();
                                }
                            }
                            else
                            {
                                // Clicked item cannot stack with ours, so swap which item slot we are tracking.
                                FavoriteItemsLastSelectedSlot = clickPos;
                                FavoriteItemsSelectedItem = clickedItem;
                            }
                        }
                        else
                        {
                            if (clickedItem == null || inventoryMenu.highlightMethod(clickedItem))
                            {
                                // We are placing our selected favorited item into a slot which is either empty or
                                // contains a highlighted item, so stop tracking, and favorite this new slot.
                                ResetFavoriteItemSlotsTracking();

                                if (!FavoriteItemSlots[clickPos])
                                {
                                    FavoriteItemSlots[clickPos] = true;
                                }
                            }
                        }
                    }
                else if (!isRightClick)
                {
                    // We have a favorited item selected and have clicked somewhere outside the inventory slots.
                    Item cursorSlotItem = GetHeldItemOrCursorSlotItem();
                    if (cursorSlotItem != null)
                    {
                        // Check if we have clicked an equipment slot.
                        List<ClickableComponent> equipmentSlots = new();
                        if (Game1.activeClickableMenu is GameMenu gameMenu && gameMenu.pages[gameMenu.currentTab] is InventoryPage inventoryPage)
                        {
                            equipmentSlots = inventoryPage.equipmentIcons;
                        }
                        else if (Game1.activeClickableMenu is ForgeMenu forgeMenu)
                        {
                            equipmentSlots = forgeMenu.equipmentIcons;
                        }

                        foreach (ClickableComponent slot in equipmentSlots)
                        {
                            if (slot.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
                            {
                                OnEquipmentSlotClickedWithFavoriteItem(slot, cursorSlotItem);
                            }

                        }
                    }
                }
            }

            return true;
        }

        // Handles logic for determining whether we are equipping a selected favorited item. If so, resets favorite tracking.
        private static void OnEquipmentSlotClickedWithFavoriteItem(ClickableComponent equipmentSlot, Item cursorSlotItem)
        {
            switch (equipmentSlot.name)
            {
                case "Left Ring":   // Inventory Page
                case "Right Ring":
                case "Ring1":       // Forge Menu
                case "Ring2":
                    if (cursorSlotItem is Ring)
                    {
                        ResetFavoriteItemSlotsTracking();
                    }
                    break;
                case "Boots":
                    if (cursorSlotItem is Boots)
                    {
                        ResetFavoriteItemSlotsTracking();
                    }
                    break;
                case "Hat":
                    if (cursorSlotItem is Hat or StardewValley.Tools.Pan)
                    {
                        ResetFavoriteItemSlotsTracking();
                    }
                    break;
                case "Shirt":
                    if (cursorSlotItem is Clothing maybeShirt && maybeShirt.clothesType.Value == Clothing.ClothesType.SHIRT)
                    {
                        ResetFavoriteItemSlotsTracking();
                    }
                    break;
                case "Pants":
                    if (cursorSlotItem is Clothing maybePants && maybePants.clothesType.Value == Clothing.ClothesType.PANTS
                        || cursorSlotItem is StardewValley.Object && cursorSlotItem.ParentSheetIndex == 71) // trimmed purple shorts
                    {
                        ResetFavoriteItemSlotsTracking();
                    }
                    break;
            }
        }

        // Determines whether two items being stacked will result in a leftover item stack (combined stacks > max stack size).
        private static bool IsOverMaxStackSize(Item item, Item canStackWith)
        {
            return item.Stack + canStackWith.Stack > item.maximumStackSize();
        }

        // Gets the current active menu's "held item", if applicable. If not, gets the player's "cursor slot item" by default.
        private static Item GetHeldItemOrCursorSlotItem()
        {
            Item item = (Game1.activeClickableMenu as ForgeMenu)?.heldItem  // Forge menu cursor slot item
                ?? Game1.player.CursorSlotItem;                             // Arbritrary menu cursor slot item

            return item;
        }

        // Checks for menus which don't allow selecting and holding items, i.e. chests, shipping bins, shops, etc.
        private static bool IsCurrentActiveMenuNoHeldItems()
        {
            bool result = Game1.activeClickableMenu is ItemGrabMenu
                || Game1.activeClickableMenu is ShopMenu;

            return result;
        }

        // Starts tracking a favorite item slot.
        public static void StartTrackingFavoriteItemSlot(int clickPos, Item clickedItem)
        {
            FavoriteItemsLastSelectedSlot = clickPos;
            FavoriteItemsIsItemSelected = true;
            FavoriteItemsSelectedItem = clickedItem;
            FavoriteItemSlots[clickPos] = false;
        }

        // Resets the tracking state of favorite item slots.
        public static void ResetFavoriteItemSlotsTracking()
        {
            FavoriteItemsLastSelectedSlot = -1;
            FavoriteItemsIsItemSelected = false;
            FavoriteItemsSelectedItem = null;
        }

        // Handles shift-click logic for favorited item slots in player's inventory page.
        private static bool HandleFavoriteItemSlotShiftClickedInInventoryPage(int clickPos, Item item)
        {
            bool isItemEquippable = (item is Ring && (Game1.player.leftRing.Value == null || Game1.player.rightRing.Value == null))
                || (item is Hat && Game1.player.hat.Value == null)
                || (item is Boots && Game1.player.boots.Value == null)
                || (item is Clothing && (((item as Clothing).clothesType.Value == Clothing.ClothesType.SHIRT && Game1.player.shirtItem.Value == null)
                                       || (item as Clothing).clothesType.Value == Clothing.ClothesType.PANTS && Game1.player.pantsItem.Value == null));

            if (isItemEquippable)
            {
                FavoriteItemSlots[clickPos] = false;
                return true;
            }

            if (clickPos >= 12)
            {
                for (int k = 0; k < 12; k++)
                {
                    if (Game1.player.Items[k] == null || Game1.player.Items[k].canStackWith(item))
                    {
                        FavoriteItemSlots[clickPos] = false;
                        FavoriteItemSlots[k] = true;
                        return true;
                    }
                }
            }
            else if (clickPos < 12)
            {
                for (int j = 12; j < Game1.player.Items.Count; j++)
                {
                    if (Game1.player.Items[j] == null || Game1.player.Items[j].canStackWith(item))
                    {
                        FavoriteItemSlots[clickPos] = false;
                        FavoriteItemSlots[j] = true;
                        return true;
                    }
                }
            }

            FavoriteItemsLastSelectedSlot = clickPos;
            FavoriteItemsIsItemSelected = true;
            FavoriteItemsSelectedItem = item;
            FavoriteItemSlots[clickPos] = false;
            return false;
        }

        private static bool HandleFavoriteItemSlotShiftClickedInCraftingPage(int clickPos, Item item)
        {
            for (int i = 0; i <= clickPos; i++)
            {
                if (i == clickPos)
                {
                    return true;
                }

                if (Game1.player.Items[i] == null)
                {
                    FavoriteItemSlots[clickPos] = false;
                    FavoriteItemSlots[i] = true;
                    return true;
                }
            }

            FavoriteItemsLastSelectedSlot = clickPos;
            FavoriteItemsIsItemSelected = true;
            FavoriteItemsSelectedItem = item;
            FavoriteItemSlots[clickPos] = false;
            return false;
        }

        public static void PostReceiveLeftClickInMenu<T>(T menu, int x, int y) where T : IClickableMenu
        {
            // Quick stack button clicked (in InventoryPage)
            if (ModEntry.Config.IsEnableQuickStack && menu is InventoryPage inventoryPage && QuickStackButton != null && QuickStackButton.containsPoint(x, y))
            {
                QuickStackLogic.StackToNearbyChests(ModEntry.Config.QuickStackRange, inventoryPage);
            }
        }

        public static void OnQuickStackHotkeyPressed()
        {
            if (Game1.activeClickableMenu is GameMenu gameMenu && gameMenu.pages[gameMenu.currentTab] is InventoryPage inventoryPage)
            {
                QuickStackLogic.StackToNearbyChests(ModEntry.Config.QuickStackRange, inventoryPage);
                return;
            }

            QuickStackLogic.StackToNearbyChests(ModEntry.Config.QuickStackRange);
        }

        // Player shifted toolbar row, so shift all favorited item slots by a row
        public static void ShiftToolbar(bool right)
        {
            RotateArray(FavoriteItemSlots, 12, right);
        }

        private static void RotateArray(bool[] arr, int count, bool right)
        {
            count %= arr.Length;
            if (count == 0) return;

            bool[] toMove = (bool[])arr.Clone();

            if (right)
            {
                // SDV "right" => left shift
                for (int i = 0; i < arr.Length - count; i++)
                {
                    arr[i] = arr[i + count];
                }
                for (int i = 0; i < count; i++)
                {
                    arr[arr.Length - count + i] = toMove[i];
                }

                return;
            }

            // SDV "left" => right shift
            for (int i = 0; i < arr.Length - count; i++)
            {
                arr[arr.Length - 1 - i] = arr[arr.Length - 1 - i - count];
            }
            for (int i = 0; i < count; i++)
            {
                arr[i] = toMove[arr.Length - count + i];
            }

            return;
        }

        public static void PerformHoverActionInInventoryPage(int x, int y)
        {
            if (ModEntry.Config.IsEnableQuickStack)
            {
                QuickStackButton.tryHover(x, y);
                IsDrawToolTip = QuickStackButton.containsPoint(x, y);
            }
        }

        public static void PopulateClickableComponentsListInInventoryPage(InventoryPage inventoryPage)
        {
            if (ModEntry.Config.IsEnableQuickStack)
            {
                inventoryPage.allClickableComponents.Add(QuickStackButton);
            }
        }

        public static bool IsPlayerInventory(InventoryMenu inventoryMenu)
        {
            bool result = inventoryMenu.playerInventory
                || (Game1.activeClickableMenu is GameMenu gameMenu && gameMenu.pages?[gameMenu.currentTab] is CraftingPage)  // CraftingPage.inventory has playerInventory = false
                || (Game1.activeClickableMenu is ItemGrabMenu itemGrabMenu && itemGrabMenu.inventory == inventoryMenu)  // ItemGrabMenu.inventory is the player's InventoryMenu
                || (Game1.activeClickableMenu is ShopMenu)
                || (Game1.activeClickableMenu is ForgeMenu);

            return result;
        }

        // Checks if we are about to remove the final item in a favorited item slot, and if so, unfavorites it.
        public static void PreFarmerReduceActiveItemByOne(Farmer who)
        {
            if (who.CurrentItem?.Stack == 1)
            {
                int index = GetPlayerInventoryIndexOfItem(who.CurrentItem);

                if (index != -1)
                {
                    FavoriteItemSlots[index] = false;
                }
            }
        }

        // Unfavorites any empty favorite item slots.
        public static void UnfavoriteEmptyItemSlots()
        {
            for (int i = 0; i < Game1.player.Items.Count; i++)
            {
                Item item = Game1.player.Items.ElementAtOrDefault(i);
                if (item == null)
                {
                    if (FavoriteItemSlots.ElementAtOrDefault(i) == true)
                    {
                        FavoriteItemSlots[i] = false;
                    }
                }
            }
        }

        // Called when an inventory's Organize button is clicked.
        // Extracts an inventory's favorited items (replacing with null), to be re-inserted after organization is completed.
        public static Item[] ExtractFavoriteItemsFromList(IList<Item> items)
        {
            if (items is null)
            {
                return null;
            }

            Item[] extractedItems = new Item[items.Count];

            for (int i = 0; i < items.Count && i < FavoriteItemSlots.Length; i++)
            {
                if (FavoriteItemSlots[i])
                {
                    extractedItems[i] = items[i];
                    items[i] = null;
                }
            }

            return extractedItems;
        }

        // Called after an inventory's Organize button is clicked.
        // Re-inserts an inventory's favorited items that were extracted before organization began.
        public static void ReinsertExtractedFavoriteItemsIntoList(Item[] extractedItems, IList<Item> items, bool isSorted = true)
        {
            if (extractedItems is null || items is null)
            {
                return;
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (extractedItems[i] != null)
                {
                    if (isSorted)
                    {
                        // Inventory has been organized since we originally extracted favorite items, so we use Insert()
                        items.Insert(i, extractedItems[i]);

                        if (items[items.Count - 1] != null)
                        {
                            // This "Item" should always be null (so this case should never happen), but just in case...
                            Game1.playSound("throwDownITem");
                            Game1.createItemDebris(items[items.Count - 1], Game1.player.getStandingPosition(), Game1.player.FacingDirection)
                                .DroppedByPlayerID.Value = Game1.player.UniqueMultiplayerID;

                            ModEntry.Instance.Monitor
                                .Log($"Found non-null item: '{items[items.Count - 1].Name}' (x {items[items.Count - 1].Stack}) out of bounds of inventory list (index={i}) " +
                                "when re-inserting extracted favorite items. The item was manually dropped; this may have resulted in unexpected behavior.",
                                StardewModdingAPI.LogLevel.Warn);
                        }

                        // Remove the null "Item" we just pushed past the end of the list
                        items.RemoveAt(items.Count - 1);
                    }
                    else
                    {
                        // Inventory is the same as when we originally extracted favorite items, so we can manually place the items back in
                        if (items[i] != null)
                        {
                            // This "Item" should always be null (so this case should never happen), but just in case...
                            Game1.playSound("throwDownITem");
                            Game1.createItemDebris(items[i], Game1.player.getStandingPosition(), Game1.player.FacingDirection)
                                .DroppedByPlayerID.Value = Game1.player.UniqueMultiplayerID;

                            ModEntry.Instance.Monitor
                                .Log($"Found non-null item: '{items[i].Name}' (x {items[i].Stack}) in unexpected position (index={i}) " +
                                "when re-inserting extracted favorite items. The item was manually dropped; this may have resulted in unexpected behavior.",
                                StardewModdingAPI.LogLevel.Warn);
                        }

                        items[i] = extractedItems[i];
                    }
                }
            }
        }

        // Called after drawing everything else in arbitrary inventory menu.
        // Draws favorite items cursor if keybind is being pressed.
        public static void PostMenuDraw<T>(T menu, SpriteBatch spriteBatch) where T : IClickableMenu
        {
            // Draw quick stack button tooltip (in InventoryPage)
            if (ModEntry.Config.IsEnableQuickStack && menu is InventoryPage && IsDrawToolTip)
            {
                DrawQuickStackButtonToolTip(spriteBatch);
            }

            if (ModEntry.Config.IsEnableFavoriteItems)
            {
                // Get inventory if menu has one
                InventoryMenu inventory = (menu as InventoryMenu)   // Inventory item slots container
                    ?? (menu as InventoryPage)?.inventory           // Player menu - inventory tab
                    ?? (menu as CraftingPage)?.inventory            // Player menu - crafting tab
                    ?? (menu as ShopMenu)?.inventory                // Shop menu
                    ?? (menu as MenuWithInventory)?.inventory;      // Arbitrary menu

                // Draw favorite cursor (unless this is an InventoryMenu)
                if (inventory != null && menu is not InventoryMenu)
                {
                    if (IsFavoriteItemsHotkeyDown)
                    {
                        DrawFavoriteItemsCursor(spriteBatch);
                        FavoriteItemsHotkeyDownCounter++;
                    }
                    else
                    {
                        FavoriteItemsHotkeyDownCounter = 0;
                    }
                }
            }
        }

        private static void DrawQuickStackButtonToolTip(SpriteBatch spriteBatch)
        {
            NearbyTypedChests = QuickStackLogic.GetTypedChestsAroundFarmer(Game1.player, ModEntry.Config.QuickStackRange, true).AsReadOnly();

            if (ModEntry.Config.IsQuickStackTooltipDrawNearbyChests)
            {
                int numPos = ModEntry.Config.IsQuickStackIntoBuildingsWithInventories
                    ? NearbyTypedChests.Count + GetExtraNumPosUsedByBuildingChests(NearbyTypedChests)
                    : NearbyTypedChests.Count;

                var text = QuickStackButton.hoverText + new string('\n', 2 * ((numPos + 7) / 8));  // Draw two newlines for each row of chests
                IClickableMenu.drawToolTip(spriteBatch, text, string.Empty, null);

                DrawTypedChestsInToolTip(spriteBatch, NearbyTypedChests);
            }
            else
            {
                IClickableMenu.drawToolTip(spriteBatch, QuickStackButton.hoverText + $" ({NearbyTypedChests.Count})", string.Empty, null);
            }
        }

        public static void DrawFavoriteItemSlotHighlights(SpriteBatch spriteBatch, InventoryMenu inventoryMenu)
        {
            if (inventoryMenu is null)
            {
                return;
            }

            List<Vector2> slotDrawPositions = inventoryMenu.GetSlotDrawPositions();

            for (int i = 0; i < slotDrawPositions.Count && i < FavoriteItemSlots.Length; i++)
            {
                if (!FavoriteItemSlots[i])
                {
                    continue;
                }

                spriteBatch.Draw(FavoriteItemsHighlightTexture,
                    slotDrawPositions[i],
                    new Rectangle(0, 0, FavoriteItemsHighlightTexture.Width, FavoriteItemsHighlightTexture.Height),
                    Color.White,
                    0f, Vector2.Zero, 4f, SpriteEffects.None, 1f
                );
            }
        }

        // Draws favorite item slots in toolbar, and draws current tool red outline and slot text on top of highlight (so we don't cover them up).
        public static void DrawFavoriteItemSlotHighlightsInToolbar(SpriteBatch spriteBatch, int yPositionOnScreen, float transparency, string[] slotText)
        {
            for (int i = 0; i < 12; i++)
            {
                if (!FavoriteItemSlots[i])
                {
                    continue;
                }

                Vector2 toDraw = new(Game1.uiViewport.Width / 2 - 384 + i * 64, yPositionOnScreen - 96 + 8);

                spriteBatch.Draw(FavoriteItemsHighlightTexture,
                    toDraw,
                    new Rectangle(0, 0, FavoriteItemsHighlightTexture.Width, FavoriteItemsHighlightTexture.Height),
                    Color.White,
                    0f, Vector2.Zero, 4f, SpriteEffects.None, 1f
                );

                spriteBatch.DrawString(Game1.tinyFont, slotText[i], toDraw + new Vector2(4f, -8f), Color.DimGray * transparency);

                if (Game1.player.CurrentToolIndex == i)
                {
                    spriteBatch.Draw(Game1.menuTexture,
                        toDraw,
                        Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 56),
                        Color.White * transparency);
                }
            }
        }

        private static void DrawFavoriteItemsCursor(SpriteBatch spriteBatch)
        {
            float scale = (float)(3d + 0.15d * Math.Cos(FavoriteItemsHotkeyDownCounter / 15d));

            spriteBatch.Draw(FavoriteItemsCursorTexture,
               new Vector2(Game1.getOldMouseX() - 32, Game1.getOldMouseY()),
               new Rectangle(0, 0, FavoriteItemsCursorTexture.Width, FavoriteItemsCursorTexture.Height),
               Color.White,
               0f, Vector2.Zero, scale, SpriteEffects.None, 1f
           );
        }

        public static void DrawFavoriteItemsToolTipBorder(Item item, SpriteBatch spriteBatch, int x, int y)
        {
            int index = GetPlayerInventoryIndexOfItem(item);

            if (ModEntry.Config.IsEnableFavoriteItems && index != -1 && FavoriteItemSlots[index])
            {
                spriteBatch.Draw(FavoriteItemsBorderTexture,
                    new Vector2(x, y),
                    new Rectangle(0, 0, FavoriteItemsBorderTexture.Width, FavoriteItemsBorderTexture.Height),
                    Color.White,
                    0f, Vector2.Zero, 4f, SpriteEffects.None, 1f
                );
            }
        }

        public static int GetPlayerInventoryIndexOfItem(Item item)
        {
            if (item is null)
            {
                return -1;
            }

            for (int i = 0; i < Game1.player.Items.Count; i++)
            {
                if (Game1.player.Items[i] == item && Game1.player.Items[i] != null)
                {
                    return i;
                }
            }

            return -1;
        }

        public static void PostClickableTextureComponentDraw(ClickableTextureComponent textureComponent, SpriteBatch spriteBatch)
        {
            // Check if we have just drawn the trash can for this inventory page, which happens before in-game tooltip is drawn.
            if (PlayerInventoryPage?.trashCan != textureComponent)
            {
                return;
            }

            if (ModEntry.Config.IsEnableQuickStack)
            {
                // Draw transferred item sprites
                foreach (ItemGrabMenu.TransferredItemSprite transferredItemSprite in transferredItemSprites)
                {
                    transferredItemSprite.Draw(spriteBatch);
                }

                QuickStackButton?.draw(spriteBatch);
            }
        }

        private static int GetExtraNumPosUsedByBuildingChests(IReadOnlyList<ITypedChest> chests)
        {
            int extraNumPos = 0;

            for (int i = 0; i < chests.Count; i++)
            {
                if (chests[i] is null)
                {
                    continue;
                }

                extraNumPos += chests[i].IsBuildingChestType()
                    ? 1 + ((i % 8 == 7) ? 1 : 0)
                    : 0;
            }

            return extraNumPos;
        }

        private static void DrawTypedChestsInToolTip(SpriteBatch spriteBatch, IReadOnlyList<ITypedChest> typedChests)
        {
            Point toolTipPosition = GetToolTipDrawPosition(QuickStackButton.hoverText);

            for (int i = 0, pos = 0; i < typedChests.Count; i++, pos++)
            {
                pos += typedChests[i]?.DrawInToolTip(spriteBatch, toolTipPosition, pos) ?? 0;
            }
        }

        // Refactored from IClickableMenu drawHoverText()
        // Finds the origin position of where a tooltip will be drawn.
        private static Point GetToolTipDrawPosition(string text)
        {
            int width = (int)Game1.smallFont.MeasureString(text).X + 32;
            int height = Math.Max(20 * 3, (int)Game1.smallFont.MeasureString(text).Y + 32 + 8);

            int x = Game1.getOldMouseX() + 32;
            int y = Game1.getOldMouseY() + 32;

            if (x + width > Utility.getSafeArea().Right)
            {
                x = Utility.getSafeArea().Right - width;
                y += 16;
            }
            if (y + height > Utility.getSafeArea().Bottom)
            {
                x += 16;
                if (x + width > Utility.getSafeArea().Right)
                {
                    x = Utility.getSafeArea().Right - width;
                }
                y = Utility.getSafeArea().Bottom - height;
            }

            return new Point(x, y);
        }

        // Updates transferredItemSprite animation
        public static void Update(GameTime time)
        {
            for (int i = 0; i < transferredItemSprites.Count; i++)
            {
                if (transferredItemSprites[i].Update(time))
                {
                    transferredItemSprites.RemoveAt(i);
                    i--;
                }
            }
        }

        public static void AddTransferredItemSprite(ItemGrabMenu.TransferredItemSprite itemSprite)
        {
            transferredItemSprites.Add(itemSprite);
        }
    }
}
