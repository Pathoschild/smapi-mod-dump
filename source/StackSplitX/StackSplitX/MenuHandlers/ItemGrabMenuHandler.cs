using StardewModdingAPI;
using System;
using StardewValley.Menus;
using StardewValley;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using SFarmer = StardewValley.Farmer;

namespace StackSplitX.MenuHandlers
{
    public class ItemGrabMenuHandler : BaseMenuHandler<ItemGrabMenu>
    {
        /// <summary>Native player inventory menu.</summary>
        private InventoryMenu PlayerInventoryMenu = null;
        
        /// <summary>Native shop inventory menu.</summary>
        private InventoryMenu ItemsToGrabMenu = null;
        
        /// <summary>If the callbacks have been hooked yet so we don't do it unnecessarily.</summary>
        private bool CallbacksHooked = false;

        /// <summary>Native item select callback.</summary>
        private ItemGrabMenu.behaviorOnItemSelect OriginalItemSelectCallback;

        /// <summary>Native item grab callback.</summary>
        private ItemGrabMenu.behaviorOnItemSelect OriginalItemGrabCallback;

        /// <summary>The item being hovered when the split menu is opened.</summary>
        private Item HoverItem = null;

        /// <summary>The amount we wish to buy/sell.</summary>
        private int StackAmount = 0;

        /// <summary>The total number of items in the hovered stack.</summary>
        private int TotalItems = 0;

        /// <summary>The currently held item.</summary>
        private Item HeldItem => this.NativeMenu.heldItem;


        /// <summary>Constructs and instance.</summary>
        /// <param name="helper">Mod helper instance.</param>
        /// <param name="monitor">Monitor instance.</param>
        public ItemGrabMenuHandler(IModHelper helper, IMonitor monitor)
            : base(helper, monitor)
        {
            // We're handling the inventory in such a way that we don't need the generic handler.
            this.HasInventory = false;
        }

        /// <summary>Allows derived handlers to provide additional checks before opening the split menu.</summary>
        /// <returns>True if it can be opened.</returns>
        protected override bool CanOpenSplitMenu()
        {
            bool canOpen = this.NativeMenu.allowRightClick;
            return (canOpen && base.CanOpenSplitMenu());
        }

        /// <summary>Tells the handler to close the split menu.</summary>
        public override void CloseSplitMenu()
        {
            base.CloseSplitMenu();

            if (this.CallbacksHooked)
                this.Monitor.Log("[CloseSplitMenu] Callbacks shouldn't be hooked", LogLevel.Error);
        }

        /// <summary>Called when the current handler loses focus when the split menu is open, allowing it to cancel the operation or run the default behaviour.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected override EInputHandled CancelMove()
        {
            if (this.HoverItem != null)
            {
                // If being cancelled from a click else-where then the keyboad state won't have shift held (unless they're still holding it),
                // in which case the default right-click behavior will run and only a single item will get moved instead of half the stack.
                // Therefore we must make sure it's still using our callback so we can correct the amount.
                HookCallbacks();

                // Run the regular command
                this.NativeMenu?.receiveRightClick(this.ClickItemLocation.X, this.ClickItemLocation.Y);

                CloseSplitMenu();

                // Consume input so the menu doesn't run left click logic as well
                return EInputHandled.Consumed;
            }

            // Consume input so the menu doesn't run left click logic as well
            return EInputHandled.NotHandled;
        }

        /// <summary>Main event that derived handlers use to setup necessary hooks and other things needed to take over how the stack is split.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected override EInputHandled OpenSplitMenu()
        {
            try
            {
                this.PlayerInventoryMenu = this.NativeMenu.inventory;
                this.ItemsToGrabMenu = this.NativeMenu.ItemsToGrabMenu;

                // Emulate the right click method that would normally happen.
                this.HoverItem = this.NativeMenu.hoveredItem;
            }
            catch (Exception e)
            {
                this.Monitor.Log($"Failed to get properties from native menu: {e}", LogLevel.Error);
                return EInputHandled.NotHandled;
            }

            // Do nothing if we're not hovering over an item
            if (this.HoverItem == null || this.HoverItem.Stack <= 1)
            {
                return EInputHandled.NotHandled;
            }

            this.TotalItems = this.HoverItem.Stack;
            this.StackAmount = (int)Math.Ceiling(this.TotalItems / 2.0); // default at half

            // Create the split menu
            this.SplitMenu = new StackSplitMenu(OnStackAmountReceived, this.StackAmount);

            return EInputHandled.Consumed;
        }

        /// <summary>Callback given to the split menu that is invoked when a value is submitted.</summary>
        /// <param name="s">The user input.</param>
        protected override void OnStackAmountReceived(string s)
        {
            // Store amount
            if (int.TryParse(s, out this.StackAmount))
            {
                if (this.StackAmount > 0)
                {
                    if (!HookCallbacks())
                        throw new Exception("Failed to hook callbacks");

                    this.NativeMenu.receiveRightClick(this.ClickItemLocation.X, this.ClickItemLocation.Y);
                }
                else
                {
                    RevertItems();
                }
            }

            base.OnStackAmountReceived(s);
        }

        /// <summary>Callback override for when an item in the inventory is selected.</summary>
        /// <param name="item">Item that was selected.</param>
        /// <param name="who">The player that selected it.</param>
        private void OnItemSelect(Item item, SFarmer who)
        {
            MoveItems(item, who, this.PlayerInventoryMenu, this.OriginalItemSelectCallback);
        }

        /// <summary>Callback override for when an item in the shop is selected.</summary>
        /// <param name="item">Item that was selected.</param>
        /// <param name="who">The player that selected it.</param>
        private void OnItemGrab(Item item, SFarmer who)
        {
            MoveItems(item, who, this.ItemsToGrabMenu, this.OriginalItemGrabCallback);
        }

        /// <summary>Updates the number of items being held by the player based on what was input to the split menu.</summary>
        /// <param name="item">The selected item.</param>
        /// <param name="who">The player that selected the items.</param>
        /// <param name="inventory">Either the player inventory or the shop inventory.</param>
        /// <param name="callback">The native callback to invoke to continue with the regular behavior after we've modified the stack.</param>
        private void MoveItems(Item item, SFarmer who, InventoryMenu inventory, ItemGrabMenu.behaviorOnItemSelect callback)
        {
            Debug.Assert(this.StackAmount > 0);

            // Get the held item now that it's been set by the native receiveRightClick call
            var heldItem = this.HeldItem;
            if (heldItem != null)
            {
                // update held item stack and item stack
                int numCurrentlyHeld = heldItem.Stack; // How many we're actually holding.
                int numInPile = this.HoverItem.Stack + item.Stack;
                int wantToHold = Math.Min(this.TotalItems, Math.Max(this.StackAmount, 0));

                this.HoverItem.Stack = this.TotalItems - wantToHold;
                heldItem.Stack = wantToHold;

                item.Stack = wantToHold;

                // Remove the empty item from the inventory
                if (this.HoverItem.Stack <= 0)
                {
                    int index = inventory.actualInventory.IndexOf(this.HoverItem);
                    if (index > -1)
                        inventory.actualInventory[index] = null;
                }
            }

            RestoreNativeCallbacks();

            // Update stack to the amount set from OnStackAmountReceived
            callback?.Invoke(item, who);
        }

        /// <summary>Cancels the operation so no items are sold or bought.</summary>
        private void RevertItems()
        {
            if (this.HoverItem != null && this.TotalItems > 0)
            {
                this.Monitor.Log("Reverting items", LogLevel.Trace);
                this.HoverItem.Stack = this.TotalItems;

                RestoreNativeCallbacks();
            }
        }

        /// <summary>Replaces the native shop callbacks with our own so we can intercept the operation to modify the amount.</summary>
        /// <returns>If it was hooked successfully.</returns>
        private bool HookCallbacks()
        {
            if (this.CallbacksHooked)
                return true;

            try
            {
                // Replace the delegates with our own
                var itemSelectCallbackField = this.Helper.Reflection.GetField<ItemGrabMenu.behaviorOnItemSelect>(this.NativeMenu, "behaviorFunction");
                var itemGrabCallbackField = typeof(ItemGrabMenu).GetField("behaviorOnItemGrab");

                this.OriginalItemGrabCallback = itemGrabCallbackField.GetValue(this.NativeMenu) as ItemGrabMenu.behaviorOnItemSelect;
                this.OriginalItemSelectCallback = itemSelectCallbackField.GetValue();

                itemGrabCallbackField.SetValue(this.NativeMenu, new ItemGrabMenu.behaviorOnItemSelect(this.OnItemGrab));
                itemSelectCallbackField.SetValue(this.OnItemSelect);

                this.CallbacksHooked = true;
            }
            catch (Exception e)
            {
                this.Monitor.Log($"Failed to hook ItemGrabMenu callbacks: {e}", LogLevel.Error);
                return false;
            }
            return true;
        }

        /// <summary>Sets the callbacks back to the native ones.</summary>
        private void RestoreNativeCallbacks()
        {
            if (!this.CallbacksHooked)
                return;

            try
            {
                var itemSelectCallbackField = this.Helper.Reflection.GetField<ItemGrabMenu.behaviorOnItemSelect>(this.NativeMenu, "behaviorFunction");
                var itemGrabCallbackField = typeof(ItemGrabMenu).GetField("behaviorOnItemGrab");

                itemSelectCallbackField.SetValue(this.OriginalItemSelectCallback);
                itemGrabCallbackField.SetValue(this.NativeMenu, this.OriginalItemGrabCallback);

                this.CallbacksHooked = false;
            }
            catch (Exception e)
            {
                this.Monitor.Log($"Failed to restore native callbacks: {e}", LogLevel.Error);
            }
        }
    }
}
