using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace StackSplitX.MenuHandlers
{
    public class GameMenuHandler : BaseMenuHandler<GameMenu>
    {
        /// <summary>Represents an invalid tab index.</summary>
        protected const int InvalidTab = -1;

        /// <summary>Tab index mapped to it's handler. Using a dict because not all indices are handled.</summary>
        private Dictionary<int, IGameMenuPageHandler> PageHandlers;

        /// <summary>The handler for the current tab,</summary>
        private IGameMenuPageHandler CurrentPageHandler = null;

        /// <summary>The last tab that was open.</summary>
        private int PreviousTab = InvalidTab;

        /// <summary>The current tab that is open.</summary>
        private int CurrentTab => this.NativeMenu.currentTab;

        /// <summary>The native list of clickable tabs, used for checking if they were clicked.</summary>
        private List<ClickableComponent> Tabs;

        /// <summary>Constructs and instance.</summary>
        /// <param name="helper">Mod helper instance.</param>
        /// <param name="monitor">Monitor instance.</param>
        public GameMenuHandler(IModHelper helper, IMonitor monitor)
            : base(helper, monitor)
        {
            PageHandlers = new Dictionary<int, IGameMenuPageHandler>()
            {
                { GameMenu.inventoryTab, new InventoryPageHandler(helper, monitor) },
                { GameMenu.craftingTab, new CraftingPageHandler(helper, monitor) }
            };
        }

        /// <summary>Notifies the handler that it's native menu has been opened.</summary>
        /// <param name="menu">The menu that was opened.</param>
        public override void Open(IClickableMenu menu)
        {
            base.Open(menu);

            this.Tabs = this.Helper.Reflection.GetField<List<ClickableComponent>>(this.NativeMenu, "tabs").GetValue();

            if (!ChangeTabs(this.CurrentTab))
            {
                this.Monitor.DebugLog($"Could not change to tab {this.CurrentTab}", LogLevel.Warn);
            }
        }

        /// <summary>Notifies the handler that it's native menu was closed.</summary>
        public override void Close()
        {
            base.Close();
            CloseCurrentHandler();
        }

        /// <summary>Initializes the inventory using the most common variable names.</summary>
        protected override void InitInventory()
        {
            // Do nothing; let the PageHandler init the inventory in Open.
        }

        /// <summary>Called when the current handler loses focus when the split menu is open, allowing it to cancel the operation or run the default behaviour.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected override EInputHandled CancelMove()
        {
            base.CancelMove();
            return this.CurrentPageHandler != null
                ? this.CurrentPageHandler.CancelMove()
                : EInputHandled.NotHandled;
        }

        /// <summary>Tells the handler to close the split menu.</summary>
        protected override bool CanOpenSplitMenu()
        {
            // Check the current tab is valid
            return this.PageHandlers.ContainsKey(this.CurrentTab);
        }

        /// <summary>Alternative of OpenSplitMenu which is invoked when the generic inventory handler is clicked.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected override EInputHandled InventoryClicked()
        {
            int stackAmount = 0;
            var handled = this.CurrentPageHandler.InventoryClicked(out stackAmount);
            if (handled != EInputHandled.NotHandled)
                this.SplitMenu = new StackSplitMenu(OnStackAmountReceived, stackAmount);
            return handled;
        }

        /// <summary>Main event that derived handlers use to setup necessary hooks and other things needed to take over how the stack is split.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected override EInputHandled OpenSplitMenu()
        {
            int stackAmount = 0;
            var handled = this.CurrentPageHandler.OpenSplitMenu(out stackAmount);
            if (handled != EInputHandled.NotHandled)
                this.SplitMenu = new StackSplitMenu(OnStackAmountReceived, stackAmount);
            return handled;
        }

        /// <summary>Passes the input to the page handler.</summary>
        /// <param name="s">Stack amount the user input.</param>
        protected override void OnStackAmountReceived(string s)
        {
            int amount = 0;
            if (int.TryParse(s, out amount))
            {
                this.CurrentPageHandler.OnStackAmountEntered(amount);
            }
            base.OnStackAmountReceived(s);
        }

        /// <summary>Checks if one of the tabs was selected and changes the current tab accordingly.</summary>
        protected override EInputHandled HandleLeftClick()
        {
            // Check which tab was click and switch to the corresponding handler.
            int tabIndex = this.Tabs.FindIndex(tab => tab.containsPoint(Game1.getMouseX(), Game1.getMouseY()));
            if (tabIndex > InvalidTab)
            {
                ChangeTabs(tabIndex);
            }
            return EInputHandled.NotHandled;
        }

        /// <summary>Switches the current page handler to the one for the new tab.</summary>
        /// <param name="newTab">The index of the new tab.</param>
        /// <returns>True if it successfully changed tabs or is already on that tab.</returns>
        private bool ChangeTabs(int newTab)
        {
            if (this.PreviousTab == newTab)
                return true;

            CloseCurrentHandler();

            if (this.PageHandlers.ContainsKey(newTab))
            {
                this.PreviousTab = newTab;
                this.CurrentPageHandler = this.PageHandlers[newTab];

                var pages = Helper.Reflection.GetField<List<IClickableMenu>>(this.NativeMenu, "pages").GetValue();
                this.CurrentPageHandler.Open(this.NativeMenu, pages[newTab], this.Inventory);
                return true;
            }
            return false;
        }

        /// <summary>Closes the current handler and sets the previous tab to invalid.</summary>
        private void CloseCurrentHandler()
        {
            this.CurrentPageHandler?.Close();
            this.CurrentPageHandler = null;
            this.PreviousTab = InvalidTab;
        }
    }
}
