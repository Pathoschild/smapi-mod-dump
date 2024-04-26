/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thimadera/StardewMods
**
*************************************************/

using StackEverythingRedux.MenuHandlers.GameMenuHandlers;
using StackEverythingRedux.UI;
using StardewValley.Menus;

namespace StackEverythingRedux.MenuHandlers
{
    /// <summary>This class is for handling the standalone crafting pages (aka cooking).</summary>
    internal class CraftingMenuHandler : BaseMenuHandler<CraftingPage>
    {
        /// <summary>The crafting page handler.</summary>
        private readonly CraftingPageHandler CraftingPageHandler = new();

        /// <summary>Null constructor that currently only invokes the base null constructor</summary>
        public CraftingMenuHandler()
            : base()
        {
        }

        /// <summary>Notifies the handler that its native menu has been opened.</summary>
        /// <param name="menu">The menu that was opened.</param>
        public override bool Open(IClickableMenu menu)
        {
            base.Open(menu);
            _ = CraftingPageHandler.Open(menu, NativeMenu, InvHandler);

            return true;
        }

        /// <summary>Notifies the handler that its native menu was closed.</summary>
        public override void Close()
        {
            base.Close();
            CraftingPageHandler.Close();
        }

        /// <summary>Alternative of OpenSplitMenu which is invoked when the generic inventory handler is clicked.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected override EInputHandled InventoryClicked()
        {
            EInputHandled handled = CraftingPageHandler.InventoryClicked(out int stackAmount);
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
            EInputHandled handled = CraftingPageHandler.OpenSplitMenu(out int stackAmount);
            if (handled != EInputHandled.NotHandled)
            {
                SplitMenu = new StackSplitMenu(OnStackAmountReceived, stackAmount);
            }

            return handled;
        }

        /// <summary>Passes the input to the page handler.</summary>
        /// <param name="s">Stack amount the user input.</param>
        protected override void OnStackAmountReceived(string s)
        {
            if (int.TryParse(s, out int amount))
            {
                CraftingPageHandler.OnStackAmountEntered(amount);
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
