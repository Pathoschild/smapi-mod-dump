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

namespace StackEverythingRedux.MenuHandlers
{
    public class JunimoNoteMenuHandler : BaseMenuHandler<JunimoNoteMenu>
    {
        /// <summary>Null constructor that currently only invokes the base null constructor</summary>
        public JunimoNoteMenuHandler()
            : base()
        {
        }

        /// <summary>Alternative of OpenSplitMenu which is invoked when the generic inventory handler is clicked.</summary>
        /// <returns>If the input was handled or consumed.</returns>
        protected override EInputHandled InventoryClicked()
        {
            InvHandler.SelectItem(Game1.getMouseX(true), Game1.getMouseY(true));
            if (InvHandler.CanSplitSelectedItem())
            {
                int stackAmount = InvHandler.GetDefaultSplitStackAmount();
                SplitMenu = new StackSplitMenu(OnStackAmountReceived, stackAmount);
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
            if (int.TryParse(s, out int amount))
            {
                InvHandler.SplitSelectedItem(amount);
            }
            base.OnStackAmountReceived(s);
        }
    }
}
