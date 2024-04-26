/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thimadera/StardewMods
**
*************************************************/

using StackEverythingRedux.UI;
using StardewValley;
using StardewValley.Menus;

namespace StackEverythingRedux.MenuHandlers.GameMenuHandlers
{
    public class GameMenuHandler : BaseMenuHandler<GameMenu>
    {
        /// <summary>Represents an invalid tab index.</summary>
        protected const int INVALID_TAB = -1;

        /// <summary>Tab index mapped to its handler. Using a dict because not all indices are handled.</summary>
        private readonly Dictionary<int, IGameMenuPageHandler> PageHandlers = new() {
                { GameMenu.inventoryTab, new InventoryPageHandler() },
                { GameMenu.craftingTab, new CraftingPageHandler() }
            };

        /// <summary>The handler for the current tab,</summary>
        private IGameMenuPageHandler CurrentPageHandler = null;

        /// <summary>The last tab that was open.</summary>
        private int PreviousTab = INVALID_TAB;

        /// <summary>The current tab that is open.</summary>
        private int CurrentTab => NativeMenu.currentTab;

        /// <summary>The native list of clickable tabs, used for checking if they were clicked.</summary>
        private List<ClickableComponent> Tabs;

        /// <summary>Null constructor that currently only invokes the base null constructor</summary>
        public GameMenuHandler()
            : base()
        {
        }

        /// <summary>Notifies the handler that its native menu has been opened.</summary>
        /// <param name="menu">The menu that was opened.</param>
        public override bool Open(IClickableMenu menu)
        {
            base.Open(menu);

            Tabs = NativeMenu.tabs;

            if (!ChangeTabs(CurrentTab))
            {
                Log.Trace($"[{nameof(GameMenuHandler)}.{nameof(Open)}] Could not change to tab {CurrentTab}");
            }

            return true;
        }

        /// <summary>Notifies the handler that its native menu was closed.</summary>
        public override void Close()
        {
            base.Close();
            CloseCurrentHandler();
            Tabs = null;
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
            _ = base.CancelMove();
            return CurrentPageHandler != null
                ? CurrentPageHandler.CancelMove()
                : EInputHandled.NotHandled;
        }

        /// <summary>Additional checks before opening the split menu.</summary>
        /// <returns>True if it can be opened.</returns>
        protected override bool CanOpenSplitMenu()
        {
            // Check the current tab is valid
            return PageHandlers.ContainsKey(CurrentTab);
        }

        /// <summary>Alternative of OpenSplitMenu which is invoked when the generic inventory handler is clicked.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected override EInputHandled InventoryClicked()
        {
            if (CurrentPageHandler == null) { return new(); }

            EInputHandled handled = CurrentPageHandler.InventoryClicked(out int stackAmount);
            if (handled != EInputHandled.NotHandled)
            {
                SplitMenu = new StackSplitMenu(OnStackAmountReceived, stackAmount);
            }

            return handled;
        }

        /// <summary>Main event that derived handlers use to setup necessary hooks and other things needed to take over how the stack is split.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected override EInputHandled OpenSplitMenu()
        {
            EInputHandled handled = CurrentPageHandler.OpenSplitMenu(out int stackAmount);
            if (handled != EInputHandled.NotHandled)
            {
                SplitMenu = new StackSplitMenu(OnStackAmountReceived, stackAmount);
            }

            return handled;
        }

        /// <summary>Passes the input to the page handler.</summary>
        /// <param name="s">Stack amount inputted by user.</param>
        protected override void OnStackAmountReceived(string s)
        {
            if (int.TryParse(s, out int amount))
            {
                CurrentPageHandler.OnStackAmountEntered(amount);
            }

            base.OnStackAmountReceived(s);
        }

        /// <summary>Checks if one of the tabs was selected and changes the current tab accordingly.</summary>
        protected override EInputHandled HandleLeftClick()
        {
            // Check which tab was click and switch to the corresponding handler.
            int mX = Game1.getMouseX(true);
            int mY = Game1.getMouseY(true);
            Log.TraceIfD($"[{nameof(GameMenuHandler)}.{nameof(HandleLeftClick)}] Mouse clicked on ({mX}, {mY})");
            int tabIndex = Tabs.FindIndex(tab => tab.containsPoint(mX, mY));
            if (tabIndex > INVALID_TAB)
            {
                Log.TraceIfD($"Changed tab to {tabIndex}");
                _ = ChangeTabs(tabIndex);
            }
            return EInputHandled.NotHandled;
        }

        /// <summary>Switches the current page handler to the one for the new tab.</summary>
        /// <param name="newTab">The index of the new tab.</param>
        /// <returns>True if it successfully changed tabs or is already on that tab.</returns>
        private bool ChangeTabs(int newTab)
        {
            if (PreviousTab == newTab)
            {
                return true;
            }

            CloseCurrentHandler();

            if (!PageHandlers.TryGetValue(newTab, out IGameMenuPageHandler pageHandler))
            {
                // Please note that this is NOT AN ERROR
                Log.TraceIfD($"[{nameof(GameMenuHandler)}.{nameof(ChangeTabs)}] No handler for tab {newTab}");
                return false;
            }

            Log.TraceIfD($"[{nameof(GameMenuHandler)}.{nameof(ChangeTabs)}] Found a handler for tab {newTab} : {pageHandler}");

            List<IClickableMenu> pages = NativeMenu.pages;

            if (pageHandler.Open(NativeMenu, pages[newTab], InvHandler))
            {
                PreviousTab = newTab;
                CurrentPageHandler = pageHandler;

                return true;
            }
            else
            {
                // Please note that this is NOT AN ERROR
                Log.Debug($"[{nameof(GameMenuHandler)}.{nameof(ChangeTabs)}] Custom menu detected, no handler for {pages[newTab]}");
                return false;
            }

        }

        /// <summary>Closes the current handler and sets the previous tab to invalid.</summary>
        private void CloseCurrentHandler()
        {
            CurrentPageHandler?.Close();
            CurrentPageHandler = null;
            PreviousTab = INVALID_TAB;
        }
    }
}
