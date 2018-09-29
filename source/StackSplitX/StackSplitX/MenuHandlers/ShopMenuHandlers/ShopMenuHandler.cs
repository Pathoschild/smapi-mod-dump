using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StackSplitX.MenuHandlers
{
    public class ShopMenuHandler : BaseMenuHandler<ShopMenu>
    {
        /// <summary>The shop action for the current operation.</summary>
        private IShopAction CurrentShopAction = null;

        /// <summary>Constructs and instance.</summary>
        /// <param name="helper">Mod helper instance.</param>
        /// <param name="monitor">Monitor instance.</param>
        public ShopMenuHandler(IModHelper helper, IMonitor monitor)
            : base(helper, monitor)
        {
        }

        /// <summary>Alternative of OpenSplitMenu which is invoked when the generic inventory handler is clicked.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected override EInputHandled InventoryClicked()
        {
            this.CurrentShopAction = SellAction.Create(this.Helper.Reflection, this.Monitor, this.NativeMenu, this.ClickItemLocation);
            return TryOpenSplitMenu(this.CurrentShopAction);
        }

        /// <summary>Main event that derived handlers use to setup necessary hooks and other things needed to take over how the stack is split.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected override EInputHandled OpenSplitMenu()
        {
            this.CurrentShopAction = BuyAction.Create(this.Helper.Reflection, this.Monitor, this.NativeMenu, this.ClickItemLocation);
            return TryOpenSplitMenu(this.CurrentShopAction);
        }

        /// <summary>Callback given to the split menu that is invoked when a value is submitted.</summary>
        /// <param name="s">The user input.</param>
        protected override void OnStackAmountReceived(string s)
        {
            int amount = 0;
            if (int.TryParse(s, out amount))
            {
                if (amount > 0) // Canceled if 0
                    this.CurrentShopAction.PerformAction(amount, this.ClickItemLocation);
            }
            base.OnStackAmountReceived(s);
        }

        /// <summary>How long the right click has to be held for before the receiveRIghtClick gets called rapidly (See Game1.Update)</summary>
        /// <returns>The polling interval.</returns>
        protected override float GetRightClickPollingInterval()
        {
            return 300f; // From ShopMenu.receiveRightClick
        }

        /// <summary>Called when the current handler loses focus when the split menu is open, allowing it to cancel the operation or run the default behaviour.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected override EInputHandled CancelMove()
        {
            // TODO: add mod config to set whether or not default behavior should be used when canceling
            // StackAmount will be default for the appropriate sale type, so just fake the input being submitted.
            OnStackAmountReceived(this.CurrentShopAction?.StackAmount.ToString());
            return EInputHandled.NotHandled;
        }

        /// <summary>Checks if the action can be performed and creates the split menu if it can.</summary>
        /// <param name="action">The action to perform.</param>
        private EInputHandled TryOpenSplitMenu(IShopAction action)
        {
            if (action?.CanPerformAction() == true)
            {
                this.SplitMenu = new StackSplitMenu(OnStackAmountReceived, this.CurrentShopAction.StackAmount);
                return EInputHandled.Consumed;
            }
            return EInputHandled.NotHandled;
        }
    }
}
