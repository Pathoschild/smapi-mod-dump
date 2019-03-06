using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;

namespace StackSplitX.MenuHandlers
{
    /// <summary>This class is for handling the standalone crafting pages (aka cooking).</summary>
    class CraftingMenuHandler : BaseMenuHandler<CraftingPage>
    {
        /// <summary>The crafting page handler.</summary>
        private CraftingPageHandler CraftingPageHandler;

        /// <summary>Constructs and instance.</summary>
        /// <param name="helper">Mod helper instance.</param>
        /// <param name="monitor">Monitor instance.</param>
        public CraftingMenuHandler(IModHelper helper, IMonitor monitor)
            : base(helper, monitor)
        {
            this.CraftingPageHandler = new CraftingPageHandler(helper, monitor);
        }

        /// <summary>Notifies the handler that it's native menu has been opened.</summary>
        /// <param name="menu">The menu that was opened.</param>
        public override void Open(IClickableMenu menu)
        {
            base.Open(menu);
            this.CraftingPageHandler.Open(menu, this.NativeMenu, this.Inventory);
        }

        /// <summary>Notifies the handler that it's native menu was closed.</summary>
        public override void Close()
        {
            base.Close();
            this.CraftingPageHandler.Close();
        }

        /// <summary>Alternative of OpenSplitMenu which is invoked when the generic inventory handler is clicked.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected override EInputHandled InventoryClicked()
        {
            int stackAmount = 0;
            var handled = this.CraftingPageHandler.InventoryClicked(out stackAmount);
            if (handled != EInputHandled.NotHandled)
                this.SplitMenu = new StackSplitMenu(OnStackAmountReceived, stackAmount, this.Helper.Input);
            return handled;
        }

        /// <summary>Main event that derived handlers use to setup necessary hooks and other things needed to take over how the stack is split.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected override EInputHandled OpenSplitMenu()
        {
            int stackAmount = 0;
            var handled = this.CraftingPageHandler.OpenSplitMenu(out stackAmount);
            if (handled != EInputHandled.NotHandled)
                this.SplitMenu = new StackSplitMenu(OnStackAmountReceived, stackAmount, this.Helper.Input);
            return handled;
        }

        /// <summary>Passes the input to the page handler.</summary>
        /// <param name="s">Stack amount the user input.</param>
        protected override void OnStackAmountReceived(string s)
        {
            int amount = 0;
            if (int.TryParse(s, out amount))
            {
                this.CraftingPageHandler.OnStackAmountEntered(amount);
            }
            base.OnStackAmountReceived(s);
        }

        /// <summary>Initializes the inventory using the most common variable names.</summary>
        protected override void InitInventory()
        {
            // Do nothing; CraftingPageHandler.Open will init the inventory.
        }
    }
}
