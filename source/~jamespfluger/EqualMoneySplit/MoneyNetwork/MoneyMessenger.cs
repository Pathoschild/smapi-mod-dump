/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamespfluger/Stardew-EqualMoneySplit
**
*************************************************/

using EqualMoneySplit.Models;
using EqualMoneySplit.Networking.Communicators;
using StardewValley;

namespace EqualMoneySplit.MoneyNetwork
{
    public class MoneyMessenger : Messenger
    {
        /// <summary>
        /// Sends all farmers the "EqualMoneySplit.MoneyListener" message to tell them to update their money
        /// </summary>
        /// <param name="newMoney">The amount of money for each farmer to receive</param>
        public void SendWalletNotification(int newMoney)
        {
            MoneyPayload moneyData = new MoneyPayload(newMoney, Game1.player.Name, EventContext.InventoryChanged);
            base.SendMessageToAllFarmers(Constants.MoneySplitListenerAddress, moneyData);

            Game1.chatBox.addInfoMessage($"You sent every player {newMoney}g.");

            EqualMoneyMod.Logger.Log($"Local farmer {Game1.player.Name} is sending {newMoney} to all farmers (at a shop)");
        }

        /// <summary>
        /// Sends all farmers the "EqualMoneySplit.MoneyListener" message to tell them to update their money
        /// </summary>
        /// <param name="newMoney">The amount of money for each farmer to receive</param>
        public void SendShippingBinNotification(int newMoney)
        {
            MoneyPayload moneyData = new MoneyPayload(newMoney, Game1.player.Name, EventContext.EndOfDay);
            base.SendMessageToAllFarmers(Constants.MoneySplitListenerAddress, moneyData);

            Game1.chatBox.addInfoMessage($"You sent every player {newMoney}g today.");

            EqualMoneyMod.Logger.Log($"Local farmer {Game1.player.Name} is sending {newMoney} to all farmers (through the shipping bin)");
        }
    }
}
