using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StackSplitX.MenuHandlers
{
    public abstract class GameMenuPageHandler<TPageType> : IGameMenuPageHandler where TPageType : IClickableMenu
    {
        /// <summary>The inventory handler.</summary>
        protected InventoryHandler Inventory = null;

        /// <summary>Does this menu have an inventory section.</summary>
        protected bool HasInventory { get; set; } = true;

        /// <summary>The native menu that owns all the pages.</summary>
        protected IClickableMenu NativeMenu { get; private set; }
        
        /// <summary>The native page this handler is for.</summary>
        protected TPageType MenuPage { get; private set; }

        /// <summary>Mod helper.</summary>
        protected readonly IModHelper Helper;

        /// <summary>Monitor for logging.</summary>
        protected readonly IMonitor Monitor;

        /// <summary>Constructs and instance.</summary>
        /// <param name="helper">Mod helper instance.</param>
        /// <param name="monitor">Monitor instance.</param>
        public GameMenuPageHandler(IModHelper helper, IMonitor monitor)
        {
            this.Helper = helper;
            this.Monitor = monitor;
        }

        /// <summary>Notifies the page handler that it's corresponding menu has been opened.</summary>
        /// <param name="menu">The native menu owning all the pages.</param>
        /// <param name="page">The specific page this handler is for.</param>
        /// <param name="inventory">The inventory handler.</param>
        public virtual void Open(IClickableMenu menu, IClickableMenu page, InventoryHandler inventory)
        {
            this.NativeMenu = menu;
            this.MenuPage = page as TPageType;
            this.Inventory = inventory;

            if (this.HasInventory)
                InitInventory();
        }

        /// <summary>Tell the handler to close.</summary>
        public virtual void Close()
        {
            this.NativeMenu = null;
            this.MenuPage = null;
            this.Inventory = null;
        }

        /// <summary>Initializes the inventory using the most common variable names.</summary>
        public virtual void InitInventory()
        {
            try
            {
                var inventoryMenu = this.MenuPage.GetType().GetField("inventory").GetValue(this.MenuPage) as InventoryMenu;
                var hoveredItemField = Helper.Reflection.GetField<Item>(this.MenuPage, "hoveredItem");

                this.Inventory.Init(inventoryMenu, hoveredItemField);
            }
            catch (Exception e)
            {
                this.Monitor.Log($"Failed to initialize the inventory handler: {e}", LogLevel.Error);
            }
        }

        /// <summary>Tells the handler that the inventory was shift-clicked.</summary>
        /// <param name="stackAmount">The default stack amount to display in the split menu.</param>
        /// <returns>If the input was handled or consumed. Generally returns not handled if an invalid item was selected.</returns>
        public virtual EInputHandled InventoryClicked(out int stackAmount)
        {
            stackAmount = 0;

            // This logic is the same for all the page handlers so we can do it here.
            this.Inventory.SelectItem(Game1.getMouseX(), Game1.getMouseY());
            if (this.Inventory.CanSplitSelectedItem())
            {
                stackAmount = this.Inventory.GetDefaultSplitStackAmount();

                return EInputHandled.Consumed;
            }
            return EInputHandled.NotHandled;
        }

        /// <summary>Tells the handler that the interface recieved the hotkey input.</summary>
        /// <param name="stackAmount">The default stack amount to display in the split menu.</param>
        /// <returns>If the input was handled or consumed. Generally returns not handled if an invalid item was selected.</returns>
        public virtual EInputHandled OpenSplitMenu(out int stackAmount)
        {
            stackAmount = 0;
            return EInputHandled.NotHandled;
        }

        /// <summary>Tells the handler to cancel the move/run the default behaviour.</summary>
        /// <returns>If the input invoking the cancel should be consumed or not.</returns>
        public virtual EInputHandled CancelMove()
        {
            return EInputHandled.NotHandled;
        }

        /// <summary>Lets the handler run the logic for doing the split after the amount has been input.</summary>
        /// <param name="amount">The stack size the user requested.</param>
        public virtual void OnStackAmountEntered(int amount)
        {
            this.Inventory.SplitSelectedItem(amount);
        }
    }
}
