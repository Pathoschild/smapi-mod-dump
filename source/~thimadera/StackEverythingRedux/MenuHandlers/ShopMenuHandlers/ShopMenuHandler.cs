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
using StardewValley.Menus;

namespace StackEverythingRedux.MenuHandlers.ShopMenuHandlers
{
    public class ShopMenuHandler : BaseMenuHandler<ShopMenu>
    {
        public const float RIGHT_CLICK_POLLING_INTVL = 300f;

        /// <summary>The shop action for the current operation.</summary>
        private IShopAction CurrentShopAction = null;

        /// <summary>Null constructor that currently only invokes the base null constructor</summary>
        public ShopMenuHandler()
            : base()
        {
        }

        /// <summary>Alternative of OpenSplitMenu which is invoked when the generic inventory handler is clicked.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected override EInputHandled InventoryClicked()
        {
            CurrentShopAction = SellAction.Create(NativeMenu, ClickItemLocation);
            return TryOpenSplitMenu(CurrentShopAction);
        }

        /// <summary>Main event that derived handlers use to setup necessary hooks and other things needed to take over how the stack is split.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected override EInputHandled OpenSplitMenu()
        {
            Log.TraceIfD($"[{nameof(ShopMenuHandler)}.{nameof(OpenSplitMenu)}] Entered");
            CurrentShopAction = BuyAction.Create(NativeMenu, ClickItemLocation);
            return TryOpenSplitMenu(CurrentShopAction);
        }

        /// <summary>Callback given to the split menu that is invoked when a value is submitted.</summary>
        /// <param name="s">The user input.</param>
        protected override void OnStackAmountReceived(string s)
        {
            if (int.TryParse(s, out int amount))
            {
                if (amount > 0) // Canceled if 0
                {
                    CurrentShopAction.PerformAction(amount, ClickItemLocation);
                }
            }
            base.OnStackAmountReceived(s);
        }

        /// <summary>How long the right click has to be held for before the receiveRIghtClick gets called rapidly (See Game1.Update)</summary>
        /// <returns>The polling interval.</returns>
        protected override float GetRightClickPollingInterval()
        {
            return RIGHT_CLICK_POLLING_INTVL; // From ShopMenu.receiveRightClick
        }

        /// <summary>Called when the current handler loses focus when the split menu is open, allowing it to cancel the operation or run the default behaviour.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected override EInputHandled CancelMove()
        {
            // TODO: add mod config to set whether or not default behavior should be used when canceling
            // StackAmount will be default for the appropriate sale type, so just fake the input being submitted.
            OnStackAmountReceived(CurrentShopAction?.StackAmount.ToString());
            return EInputHandled.NotHandled;
        }

        /// <summary>Checks if the action can be performed and creates the split menu if it can.</summary>
        /// <param name="action">The action to perform.</param>
        private EInputHandled TryOpenSplitMenu(IShopAction action)
        {
            if (action?.CanPerformAction() == true)
            {
                Log.TraceIfD($"[{nameof(ShopMenuHandler)}.{nameof(TryOpenSplitMenu)}] Creating Split Menu");
                SplitMenu = new StackSplitMenu(OnStackAmountReceived, CurrentShopAction.StackAmount);
                return EInputHandled.Consumed;
            }
            return EInputHandled.NotHandled;
        }
    }
}
