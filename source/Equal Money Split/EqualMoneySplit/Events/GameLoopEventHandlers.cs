using EqualMoneySplit.Models;
using EqualMoneySplit.MoneyNetwork;
using EqualMoneySplit.Utils;
using StardewModdingAPI.Events;
using StardewValley;

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
            PersistantFarmerData.PocketMoney = Game1.player.Money;
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

            // Calculate all money that will be earned from the shipping bin
            PersistantFarmerData.ShippingBinMoney = ItemValueUtil.CalculateItemCollectionValue(Game1.player.personalShippingBin);
            PersistantFarmerData.ShareToSend = MoneySplitUtil.GetPerPlayerShare(PersistantFarmerData.ShippingBinMoney);

            // Only send a notification if money has been earned
            if (PersistantFarmerData.ShareToSend != 0)
            {
                MoneyMessenger moneyMessenger = new MoneyMessenger();
                moneyMessenger.SendShippingBinNotification(PersistantFarmerData.ShareToSend);
            }
        }

        /// <summary>
        /// Resets the saved value in the shipping bin and the share from that
        /// </summary>
        /// <param name="sender">The sender of the DayStarted event</param>
        /// <param name="args">Event arguments for the DayStarted event</param>
        public void OnDayStartedHandler(object sender, DayStartedEventArgs args)
        {
            if (!Game1.player.useSeparateWallets)
            {
                EqualMoneyMod.Logger.Log("EqualMoneySplit cannot be run unless individual wallets are set up! You must either disable the mod or set up individual wallets!", StardewModdingAPI.LogLevel.Warn);
                Game1.chatBox.addErrorMessage("ERROR: EqualMoneySplit cannot be run unless individual wallets are set up!");
                EventSubscriber.Instance.RemoveSubscriptions();

                return;
            }

            EqualMoneyMod.Logger.Log($"DayStarted | {Game1.player.Name} has {Game1.player.Money} money");

            PersistantFarmerData.ShareToSend = 0;
            PersistantFarmerData.ShippingBinMoney = 0;
        }
    }
}
