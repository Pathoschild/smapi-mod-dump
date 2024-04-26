/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using ItemBags.Bags;
using ItemBags.Helpers;
using ItemBags.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace ItemBags
{
    public static class InputHandler
    {
        private static IModHelper Helper { get; set; }
        private static IMonitor Monitor { get { return ItemBagsMod.ModInstance.Monitor; } }

        private static ItemBag LastClickedBag { get; set; }
        private static int? LastClickedBagInventoryIndex { get; set; }
        private static DateTime? LastClickedBagTime { get; set; }
        private const int DoubleClickThresholdMS = 300; // Clicking the same Bag in your inventory within this amount of time will register as a double-click

        public const int DefaultChestCapacity = 36;

        internal static ISemanticVersion MegaStorageInstalledVersion { get; private set; } = null;

        private static bool QueuePlaceCursorSlotItem { get; set; }
        private static int? QueueCursorSlotIndex { get; set; }

        /// <summary>Keeps track of which Gamepad navigational buttons are currently depressed, so that the active cursor slot can be repeatedly moved in the held direction every few game frames.</summary>
        internal static Dictionary<NavigationDirection, DateTime> NavigationButtonsPressedTime { get; } = new Dictionary<NavigationDirection, DateTime>();
        internal static bool IsNavigationButtonPressed(NavigationDirection Direction) {
            return NavigationButtonsPressedTime != null && NavigationButtonsPressedTime.TryGetValue(Direction, out DateTime Time);
        }

        /// <summary>Adds several SMAPI Console commands for adding bags to the player's inventory</summary>
        internal static void OnModEntry(IModHelper Helper)
        {
            InputHandler.Helper = Helper;

            string MegaStorageId = "Alek.MegaStorage";
            if (Helper.ModRegistry.IsLoaded(MegaStorageId))
            {
                MegaStorageInstalledVersion = Helper.ModRegistry.Get(MegaStorageId).Manifest.Version;
            }

            Helper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
            Helper.Events.Input.CursorMoved += Input_MouseMoved;
            Helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            Helper.Events.Input.ButtonReleased += Input_ButtonReleased;
        }

        private static void GameLoop_UpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            try
            {
                //  Swaps the current CursorSlotItem with the inventory item at index=QueueCursorSlotIndex
                if (QueuePlaceCursorSlotItem && QueueCursorSlotIndex.HasValue)
                {
                    if (Game1.activeClickableMenu is GameMenu GM && GM.currentTab == GameMenu.inventoryTab)
                    {
                        Item Temp = Game1.player.Items[QueueCursorSlotIndex.Value];
                        Game1.player.Items[QueueCursorSlotIndex.Value] = Game1.player.CursorSlotItem;
                        Game1.player.CursorSlotItem = Temp;
                    }
                }
            }
            finally { QueuePlaceCursorSlotItem = false; QueueCursorSlotIndex = null; }
        }

        private static void Input_MouseMoved(object sender, CursorMovedEventArgs e)
        {
            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemBagMenu IBM)
            {
                try { IBM.OnMouseMoved(e); }
                catch (Exception ex) { Monitor.Log(string.Format("Unhandled error while handling Mouse moved: {0}\n\n{1}", ex.Message, ex.ToString()), LogLevel.Error); }
            }
        }

        private static void Input_ButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemBagMenu IBM)
            {
                if (e.Button == SButton.MouseLeft || e.Button == SButton.MouseMiddle || e.Button == SButton.MouseRight)
                {
                    try { IBM.OnMouseButtonReleased(e); }
                    catch (Exception ex) { Monitor.Log(string.Format("Unhandled error while handling Mouse button released: {0}\n\n{1}", ex.Message, ex.ToString()), LogLevel.Error); }
                }
                else if (e.Button == SButton.LeftShift || e.Button == SButton.RightShift || e.Button == SButton.LeftControl || e.Button == SButton.RightControl)
                {
                    try { IBM.OnModifierKeyReleased(e); }
                    catch (Exception ex) { Monitor.Log(string.Format("Unhandled error while handling Modifier key released: {0}\n\n{1}", ex.Message, ex.ToString()), LogLevel.Error); }
                }
                else if (e.Button.TryGetController(out Buttons GamepadButtons))
                {
                    try
                    {
                        //  Handle navigation buttons
                        foreach (NavigationDirection Direction in Enum.GetValues(typeof(NavigationDirection)).Cast<NavigationDirection>())
                        {
                            if (GamepadControls.IsMatch(GamepadButtons, GamepadControls.NavigateSingleButtons[Direction]))
                                NavigationButtonsPressedTime.Remove(Direction);
                        }

                        IBM.OnGamepadButtonsReleased(GamepadButtons);
                    }
                    catch (Exception ex) { Monitor.Log(string.Format("Unhandled error while handling Gamepad button released: {0}\n\n{1}", ex.Message, ex.ToString()), LogLevel.Error); }
                }
            }
        }

        //  Failsafe in case user has a bag for modded items in their inventory, but they've deleted the modded bag's Type Definition.
        //  This can cause crashes due to null reference exceptions in overridden methods (such as salePrice(bool ignoreProfitMargins = false) )
        private static bool ValidateBag(ItemBag Bag)
        {
            if (Bag is BoundedBag BoundedBag && Bag is not BundleBag && BoundedBag.TypeInfo == null)
            {
                BoundedBag.TryRemoveInvalidItems(Game1.player.Items, Game1.player.MaxItems);
                return false;
            }
            else
                return true;
        }

        private static void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                Point CursorPos = e.Cursor.LegacyScreenPixels().AsAndroidCompatibleCursorPoint();
                bool IsGamepadInput = e.Button.TryGetController(out Buttons GamepadButtons);

                if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemBagMenu IBM)
                {
                    if (e.Button == SButton.MouseLeft || e.Button == SButton.MouseMiddle || e.Button == SButton.MouseRight)
                    {
                        try { IBM.OnMouseButtonPressed(e); }
                        catch (Exception ex) { Monitor.Log(string.Format("Unhandled error while handling Mouse button pressed: {0}\n\n{1}", ex.Message, ex.ToString()), LogLevel.Error); }
                    }
                    else if (e.Button == SButton.LeftShift || e.Button == SButton.RightShift || e.Button == SButton.LeftControl || e.Button == SButton.RightControl)
                    {
                        try { IBM.OnModifierKeyPressed(e); }
                        catch (Exception ex) { Monitor.Log(string.Format("Unhandled error while handling Modifier key pressed: {0}\n\n{1}", ex.Message, ex.ToString()), LogLevel.Error); }
                    }
                    else if (IsGamepadInput)
                    {
                        try
                        {
                            //  Handle navigation buttons
                            foreach (NavigationDirection Direction in Enum.GetValues(typeof(NavigationDirection)).Cast<NavigationDirection>())
                            {
                                if (GamepadControls.IsMatch(GamepadButtons, GamepadControls.NavigateSingleButtons[Direction]))
                                    NavigationButtonsPressedTime[Direction] = DateTime.Now;
                            }

                            IBM.OnGamepadButtonsPressed(GamepadButtons);
                        }
                        catch (Exception ex) { Monitor.Log(string.Format("Unhandled error while handling Gamepad button pressed: {0}\n\n{1}", ex.Message, ex.ToString()), LogLevel.Error); }
                    }
                }
                else if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is GameMenu GM && GM.currentTab == GameMenu.inventoryTab)
                {
                    InventoryPage InvPage = GM.pages.First(x => x is InventoryPage) as InventoryPage;
                    InventoryMenu InvMenu = InvPage.inventory;

                    int ClickedItemIndex = InvMenu.getInventoryPositionOfClick(CursorPos.X, CursorPos.Y);
                    bool IsValidInventorySlot = ClickedItemIndex >= 0 && ClickedItemIndex < InvMenu.actualInventory.Count;
                    if (IsValidInventorySlot)
                    {
                        Item ClickedItem = InvMenu.actualInventory[ClickedItemIndex];

                        //  Double click an ItemBag to open it
                        if (e.Button == SButton.MouseLeft) //SButtonExtensions.IsUseToolButton(e.Button))
                        {
                            //  The first time the user clicks an item in their inventory, Game1.player.CursorSlotItem is set to what they clicked (so it's like drag/drop, they're now holding the item to move it)
                            //  So to detect a double click, we can't just check if they clicked the bag twice in a row, since on the second click the item would no longer be in their inventory.
                            //  Instead, we need to check if they clicked the bag and then we need to check Game1.player.CursorSlotItem on the next click
                            if (ClickedItem is ItemBag ClickedBag && Game1.player.CursorSlotItem == null)
                            {
                                LastClickedBagInventoryIndex = ClickedItemIndex;
                                LastClickedBag = ClickedBag;
                                LastClickedBagTime = DateTime.Now;
                            }
                            else if (ClickedItem == null && Game1.player.CursorSlotItem is ItemBag DraggedBag && LastClickedBag == DraggedBag &&
                                LastClickedBagInventoryIndex.HasValue && LastClickedBagInventoryIndex.Value == ClickedItemIndex &&
                                LastClickedBagTime.HasValue && DateTime.Now.Subtract(LastClickedBagTime.Value).TotalMilliseconds <= DoubleClickThresholdMS)
                            {
                                LastClickedBag = DraggedBag;
                                LastClickedBagTime = DateTime.Now;

                                //  Put the item that's being dragged back into their inventory
                                Game1.player.addItemToInventory(Game1.player.CursorSlotItem, ClickedItemIndex);
                                Game1.player.CursorSlotItem = null;

                                if (ValidateBag(DraggedBag))
                                    DraggedBag.OpenContents(Game1.player.Items, Game1.player.MaxItems);
                            }
                        }
                        //  Right-click an ItemBag to open it
                        else if ((e.Button == SButton.MouseRight || (IsGamepadInput && GamepadControls.IsMatch(GamepadButtons, GamepadControls.Current.OpenBagFromInventory)))
                            && ClickedItem is ItemBag ClickedBag && Game1.player.CursorSlotItem == null)
                        {
                            if (ValidateBag(ClickedBag))
                                ClickedBag.OpenContents(Game1.player.Items, Game1.player.MaxItems);
                        }

                        //  Handle dropping an item into a bag from the Inventory menu
                        if (ClickedItem is ItemBag IB && Game1.player.CursorSlotItem != null && Game1.player.CursorSlotItem is Object Obj)
                        {
                            if (IB.IsValidBagItem(Obj) && (e.Button == SButton.MouseLeft || e.Button == SButton.MouseRight))
                            {
                                int Qty = ItemBag.GetQuantityToTransfer(e, Obj);
                                IB.MoveToBag(Obj, Qty, out int MovedQty, true, Game1.player.Items);

                                if (e.Button == SButton.MouseLeft)
                                //|| (MovedQty > 0 && Obj.Stack == 0)) // Handle moving the last quantity with a right-click
                                {
                                    //  Clicking the bag will have made it become the held CursorSlotItem, so queue up an action that will swap them back on next game tick
                                    QueueCursorSlotIndex = ClickedItemIndex;
                                    QueuePlaceCursorSlotItem = true;
                                }
                            }
                        }
                    }
                }
                else if (Game1.activeClickableMenu == null &&
                    (e.Button == SButton.MouseLeft || e.Button == SButton.MouseRight || (IsGamepadInput && GamepadControls.IsMatch(GamepadButtons, GamepadControls.Current.OpenBagFromToolbar))))
                {
                    //  Check if they clicked a bag on the toolbar, open the bag if so
                    Toolbar toolbar = Game1.onScreenMenus.FirstOrDefault(x => x is Toolbar) as Toolbar;
                    if (toolbar != null)
                    {
                        try
                        {
                            List<ClickableComponent> toolbarButtons = typeof(Toolbar).GetField("buttons", BindingFlags.Public | BindingFlags.Instance).GetValue(toolbar) as List<ClickableComponent>;
                            if (toolbarButtons != null)
                            {
                                //  Find the slot on the toolbar that they clicked, if any
                                for (int i = 0; i < toolbarButtons.Count; i++)
                                {
                                    if (toolbarButtons[i].bounds.Contains(CursorPos) || (IsGamepadInput && toolbar.currentlySnappedComponent == toolbarButtons[i]))
                                    {
                                        int ActualIndex = i;
                                        if (Constants.TargetPlatform == GamePlatform.Android)
                                        {
                                            try
                                            {
                                                int StartIndex = Helper.Reflection.GetField<int>(toolbar, "_drawStartIndex").GetValue(); // This is completely untested
                                                ActualIndex = i + StartIndex;
                                            }
                                            catch (Exception) { }
                                        }

                                        //  Get the corresponding Item from the player's inventory
                                        Item item = Game1.player.Items[ActualIndex];
                                        if (item is ItemBag IB && ValidateBag(IB))
                                        {
                                            IB.OpenContents(Game1.player.Items, Game1.player.MaxItems);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        catch (Exception) { }
                    }
                }
                else if (Game1.activeClickableMenu is ItemGrabMenu IGM)
                {
                    if (IsShippingBinMenu(Game1.activeClickableMenu))
                    {
                        InventoryMenu InvMenu = IGM.inventory;

                        int ClickedItemIndex = InvMenu.getInventoryPositionOfClick(CursorPos.X, CursorPos.Y);
                        bool IsValidInventorySlot = ClickedItemIndex >= 0 && ClickedItemIndex < InvMenu.actualInventory.Count;
                        if (IsValidInventorySlot)
                        {
                            Item ClickedItem = InvMenu.actualInventory[ClickedItemIndex];

                            //  Right-click an ItemBag to open it
                            if ((e.Button == SButton.MouseRight || (IsGamepadInput && GamepadControls.IsMatch(GamepadButtons, GamepadControls.Current.OpenBagFromInventory)))
                                && ClickedItem is ItemBag ClickedBag)
                            {
                                if (ValidateBag(ClickedBag))
                                    ClickedBag.OpenContents(Game1.player.Items, Game1.player.MaxItems);
                            }
                        }
                    }
                    else if (IGM.context is Chest ChestSource &&
                        (e.Button == SButton.MouseRight || e.Button == SButton.MouseMiddle || (IsGamepadInput && GamepadControls.IsMatch(GamepadButtons, GamepadControls.Current.OpenBagFromChest))))
                    {
                        //  Check if they clicked a Bag in the inventory part of the chest interface
                        bool Handled = false;
                        for (int i = 0; i < IGM.inventory.inventory.Count; i++)
                        {
                            ClickableComponent Component = IGM.inventory.inventory[i];
                            if (Component != null && Component.bounds.Contains(CursorPos))
                            {
                                Item ClickedInvItem = i < 0 || i >= IGM.inventory.actualInventory.Count ? null : IGM.inventory.actualInventory[i];
                                if (ClickedInvItem is ItemBag IB && ValidateBag(IB))
                                {
                                    IB.OpenContents(IGM.inventory.actualInventory, Game1.player.MaxItems);
                                }
                                Handled = true;
                                break;
                            }
                        }

                        bool IsMegaStorageCompatibleWithCurrentChest = IGM.ItemsToGrabMenu.capacity == DefaultChestCapacity ||
                            MegaStorageInstalledVersion == null || MegaStorageInstalledVersion.IsNewerThan(new SemanticVersion(1, 4, 4));
                        if (!Handled && IsMegaStorageCompatibleWithCurrentChest)
                        {
                            //  Check if they clicked a Bag in the chest part of the chest interface
                            for (int i = 0; i < IGM.ItemsToGrabMenu.inventory.Count; i++)
                            {
                                ClickableComponent Component = IGM.ItemsToGrabMenu.inventory[i];
                                if (Component != null && Component.bounds.Contains(CursorPos))
                                {
                                    Item ClickedChestItem = i < 0 || i >= IGM.ItemsToGrabMenu.actualInventory.Count ? null : IGM.ItemsToGrabMenu.actualInventory[i];
                                    if (ClickedChestItem is ItemBag IB && ValidateBag(IB))
                                    {
                                        IB.OpenContents(IGM.ItemsToGrabMenu.actualInventory, IGM.ItemsToGrabMenu.capacity);
                                    }
                                    Handled = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ItemBagsMod.ModInstance.Monitor.Log(string.Format("Unhandled error in {0}: {1}", nameof(Input_ButtonPressed), ex.Message), LogLevel.Error);
            }
        }

        private static bool IsShippingBinMenu(IClickableMenu menu)
        {
            if (menu is not ItemGrabMenu IGM)
                return false;

            if (IGM.shippingBin && IGM.context is ShippingBin)
                return true;

            //  The "Chests Anywhere" mod overrides the vanilla shipping bin menu
            bool isChestsAnywhereShippingBin = object.ReferenceEquals(IGM.ItemsToGrabMenu.actualInventory, Game1.getFarm().getShippingBin(Game1.player));
            return isChestsAnywhereShippingBin;
        }
    }
}
