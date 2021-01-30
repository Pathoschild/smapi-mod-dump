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

namespace EqualMoneySplit.Events
{
    /// <summary>
    /// Handles events related to when the game saves
    /// </summary>
    public class SaveEventHandlers
    {
        /// <summary>
        /// Corrects the local Farmer's money before the save begins to occur
        /// Also forces the game to handle any messages that have not been handled
        /// </summary>
        /// <param name="sender">The sender of the Saving event</param>
        /// <param name="args">Event arguments for the Saving event</param>
        public void OnSavingHandler(object sender, SavingEventArgs args)
        {
            EqualMoneyMod.Logger.Log($"Saving | {Game1.player.Name} has {Game1.player.Money} money");

            // Correct the local player's money after they sent their mod messages out
            MoneySplitUtil.CorrectLocalPlayer(PersistantFarmerData.PersonalShippingBinsMoney, PersistantFarmerData.ShareToSend);
            
            // Force the listener to check for unhandled messages
            MoneyListener.Instance.CheckForNewMessages();
        }
    }
}
