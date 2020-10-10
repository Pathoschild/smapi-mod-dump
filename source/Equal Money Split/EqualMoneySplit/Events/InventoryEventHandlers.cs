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
using EqualMoneySplit.MoneyNetwork;
using EqualMoneySplit.Utils;
using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;

namespace EqualMoneySplit.Events
{
    /// <summary>
    /// Handles events related to inventory changes
    /// </summary>
    public class InventoryEventHandlers
    {
        /// <summary>
        /// Handles the event raised after items are changed or removed from the Farmer's inventory
        /// </summary>
        /// <param name="sender">The sender of the InventoryChanged event</param>
        /// <param name="args">Event arguments for the InventoryChanged event</param>
        public void OnInventoryChanged(object sender, InventoryChangedEventArgs args)
        {
            // Short circuit if the pockey money hasn't increased
            if (Game1.player.Money <= PersistantFarmerData.PocketMoney)
                return;

            // Calculate all money gained from removed items or items that decreased their stack size
            int totalNewMoney = 0;
            totalNewMoney += ItemValueUtil.CalculateItemCollectionValue(args.Removed, false);
            totalNewMoney += ItemValueUtil.CalculateItemCollectionValue(args.QuantityChanged.Where(i => i.NewSize < i.OldSize), true);

            // Short circuit if the items removed don't have a value
            if (totalNewMoney == 0)
                return;

            // Get the share each player will receive
            int moneyPerPlayer = MoneySplitUtil.GetPerPlayerShare(totalNewMoney);

            // Correct the local player's money because they earned everyone's share
            MoneySplitUtil.CorrectLocalPlayer(totalNewMoney, moneyPerPlayer);

            // Tell all the other farmers to update their money too
            MoneyMessenger moneyMessenger = new MoneyMessenger();
            moneyMessenger.SendWalletNotification(moneyPerPlayer);
        }
    }
}
