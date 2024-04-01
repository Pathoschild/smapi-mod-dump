/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamespfluger/Stardew-EqualMoneySplit
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using EqualMoneySplit.Models;
using EqualMoneySplit.MoneyNetwork;
using EqualMoneySplit.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;

namespace EqualMoneySplit.Events
{
    /// <summary>
    /// Handles events related to game loops
    /// </summary>
    public class GameLoopEventHandlers
    {
        /// <summary>
        /// Updates the local Farmer's stored money value before the game state is updated (~60 times per second)
        /// </summary>
        /// <param name="sender">The sender of the UpdateTicking event</param>
        /// <param name="args">Event arguments for the UpdateTicking event</param>
        public void OnUpdateTicking(object sender, UpdateTickingEventArgs args)
        {
            EqualMoneyMod.FarmerData.Value ??= new();
            EqualMoneyMod.FarmerData.Value.PocketMoney = Game1.player.Money;
        }

        /// <summary>
        /// Calculates the shipping bin money before the day begins to end and
        /// sends the split share of that value after this function but before the day begins to end
        /// </summary>
        /// <param name="sender">The sender of the DayEndingEvent event</param>
        /// <param name="args">Event arguments for the DayEndingEvent event</param>
        public void OnDayEndingHandler(object sender, DayEndingEventArgs args)
        {
            EqualMoneyMod.Logger.Log($"DayEnding | {Game1.player.Name} has {Game1.player.Money} money");

            // Calculate all money that will be earned from the shipping bins
            IEnumerable<Item> miniShippingBinItems = GetMiniShippingBinItems();
            int miniShippingBinsValue = ItemValueUtil.CalculateItemCollectionValue(miniShippingBinItems);
            int mainShippingBinValue = ItemValueUtil.CalculateItemCollectionValue(Game1.player.personalShippingBin);

            EqualMoneyMod.FarmerData.Value.PersonalShippingBinsMoney = miniShippingBinsValue + mainShippingBinValue;
            EqualMoneyMod.FarmerData.Value.ShareToSend = MoneySplitUtil.GetPerPlayerShare(EqualMoneyMod.FarmerData.Value.PersonalShippingBinsMoney);

            // Only send a notification if money has been earned
            if (EqualMoneyMod.FarmerData.Value.ShareToSend != 0)
            {
                MoneyMessenger moneyMessenger = new MoneyMessenger();
                moneyMessenger.SendShippingBinNotification(EqualMoneyMod.FarmerData.Value.ShareToSend);
            }
        }

        /// <summary>
        /// Resets the saved value in the shipping bin and the share from that
        /// </summary>
        /// <param name="sender">The sender of the DayStarted event</param>
        /// <param name="args">Event arguments for the DayStarted event</param>
        public void OnDayStartedHandler(object sender, DayStartedEventArgs args)
        {
            if (!EventSubscriber.Instance.TrySetEventSubscriptions())
                return;

            EqualMoneyMod.Logger.Log($"DayStarted | {Game1.player.Name} has {Game1.player.Money} money");

            EqualMoneyMod.FarmerData.Value.ShareToSend = 0;
            EqualMoneyMod.FarmerData.Value.PersonalShippingBinsMoney = 0;
        }

        /// <summary>
        /// Returns all the items in all the mini-shipping bins from all locations in the game
        /// </summary>
        /// <returns>All items in all the mini-shipping bins</returns>
        private IEnumerable<Item> GetMiniShippingBinItems()
        {
            // A shipping bin may be in any location, so we need to find them in every location in the game
            IEnumerable<Chest> allChests = Game1.locations.SelectMany(l => l.Objects.Values.OfType<Chest>());
            List<Item> itemsInMiniShippingBins = new List<Item>();

            foreach (Chest miniShippingBin in allChests.Where(c => c.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin))
            {
                IEnumerable<Item> currentPlayerItems = miniShippingBin.GetItemsForPlayer(Game1.player.UniqueMultiplayerID);
                itemsInMiniShippingBins.AddRange(currentPlayerItems);
            }

            return itemsInMiniShippingBins;
        }
    }
}
