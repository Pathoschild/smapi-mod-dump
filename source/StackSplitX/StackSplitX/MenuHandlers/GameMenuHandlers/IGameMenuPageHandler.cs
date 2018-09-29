using StardewValley.Menus;

namespace StackSplitX.MenuHandlers
{
    public interface IGameMenuPageHandler
    {
        /// <summary>Notifies the page handler that it's corresponding menu has been opened.</summary>
        /// <param name="menu">The native menu owning all the pages.</param>
        /// <param name="page">The specific page this handler is for.</param>
        /// <param name="inventory">The inventory handler.</param>
        void Open(IClickableMenu menu, IClickableMenu page, InventoryHandler inventory);

        /// <summary>Tell the handler to close.</summary>
        void Close();

        /// <summary>Tells the handler that the inventory was shift-clicked.</summary>
        /// <param name="stackAmount">The default stack amount to display in the split menu.</param>
        /// <returns>If the input was handled or consumed. Generally returns not handled if an invalid item was selected.</returns>
        EInputHandled InventoryClicked(out int stackAmount);

        /// <summary>Tells the handler that the interface recieved the hotkey input.</summary>
        /// <param name="stackAmount">The default stack amount to display in the split menu.</param>
        /// <returns>If the input was handled or consumed. Generally returns not handled if an invalid item was selected.</returns>
        EInputHandled OpenSplitMenu(out int stackAmount);

        /// <summary>Tells the handler to cancel the move/run the default behaviour.</summary>
        /// <returns>If the input invoking the cancel should be consumed or not.</returns>
        EInputHandled CancelMove();

        /// <summary>Lets the handler run the logic for doing the split after the amount has been input.</summary>
        /// <param name="amount">The stack size the user requested.</param>
        void OnStackAmountEntered(int amount);
    }
}
