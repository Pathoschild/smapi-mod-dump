using StardewValley;
using System;

namespace EqualMoneySplit.Utils
{
    /// <summary>
    /// Utility used to split money between Farmers
    /// </summary>
    public static class MoneySplitUtil
    {
        /// <summary>
        /// Calculates the equal amount of Money each Farmer will receive
        /// </summary>
        /// <param name="totalNewMoney">The amount of money to be divided equally</param>
        /// <returns>The share each Farmer will receive from the total</returns>
        public static int GetPerPlayerShare(int totalNewMoney)
        {
            int numberOfFarmers = Game1.getOnlineFarmers().Count;
            int moneyPerPlayer = (int)Math.Ceiling(Convert.ToDouble(totalNewMoney) / numberOfFarmers);
            EqualMoneyMod.Logger.Log($"Each player will receive: {moneyPerPlayer}");
            return moneyPerPlayer;
        }

        /// <summary>
        /// Corrects the current Farmer's Money after they have earned money for their own wallet
        /// </summary>
        /// <param name="totalNewMoney">The amount of Money the Farmer initially earned</param>
        /// <param name="moneyPerPlayer">The corrected even share of Money the Farmer should have earned</param>
        public static void CorrectLocalPlayer(int totalNewMoney, int moneyPerPlayer)
        {
            // Update the current farmer's money  |  Curr = Curr - (Split Share * (Number of Farmers-1))
            EqualMoneyMod.Logger.Log($"Local farmer {Game1.player.Name} => ({Game1.player.Money } - {totalNewMoney}) + {moneyPerPlayer} = {Game1.player.Money - totalNewMoney + moneyPerPlayer}");
            Game1.player.Money = Game1.player.Money - totalNewMoney + moneyPerPlayer;
        }
    }
}
