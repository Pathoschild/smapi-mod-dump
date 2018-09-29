using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackSplitX.MenuHandlers
{
    public class JunimoNoteMenuHandler : BaseMenuHandler<JunimoNoteMenu>
    {
        /// <summary>Constructs and instance.</summary>
        /// <param name="helper">Mod helper instance.</param>
        /// <param name="monitor">Monitor instance.</param>
        public JunimoNoteMenuHandler(IModHelper helper, IMonitor monitor)
            : base(helper, monitor)
        {
        }

        /// <summary>Alternative of OpenSplitMenu which is invoked when the generic inventory handler is clicked.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected override EInputHandled InventoryClicked()
        {
            this.Inventory.SelectItem(Game1.getMouseX(), Game1.getMouseY());
            if (this.Inventory.CanSplitSelectedItem())
            {
                int stackAmount = this.Inventory.GetDefaultSplitStackAmount();
                this.SplitMenu = new StackSplitMenu(OnStackAmountReceived, stackAmount);
                return EInputHandled.Consumed;
            }
            return EInputHandled.NotHandled;
        }

        /// <summary>Main event that derived handlers use to setup necessary hooks and other things needed to take over how the stack is split.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected override EInputHandled OpenSplitMenu()
        {
            return EInputHandled.NotHandled;
        }

        /// <summary>Callback given to the split menu that is invoked when a value is submitted.</summary>
        /// <param name="s">The user input.</param>
        protected override void OnStackAmountReceived(string s)
        {
            int amount = 0;
            if (int.TryParse(s, out amount))
            {
                this.Inventory.SplitSelectedItem(amount);
            }
            base.OnStackAmountReceived(s);
        }
    }
}
