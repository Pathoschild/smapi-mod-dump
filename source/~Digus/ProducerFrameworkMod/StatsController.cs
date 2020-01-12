using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace ProducerFrameworkMod
{
    public class StatsController
    {
        /// <summary>
        /// Increment the given stats for the given amout
        /// </summary>
        /// <param name="stats">The stat to increment</param>
        /// <param name="amount">The amount to increment</param>
        public static void IncrementStardewStats(StardewStats stats, int amount = 1)
        {
            switch (stats)
            {
                case StardewStats.PiecesOfTrashRecycled:
                    Game1.stats.PiecesOfTrashRecycled += (uint) amount;
                    break;
                case StardewStats.GoatCheeseMade:
                    Game1.stats.GoatCheeseMade += (uint)amount;
                    break;
                case StardewStats.CheeseMade:
                    Game1.stats.CheeseMade += (uint)amount;
                    break;
                case StardewStats.PreservesMade:
                    Game1.stats.PreservesMade += (uint)amount;
                    break;
                case StardewStats.BeveragesMade:
                    Game1.stats.BeveragesMade += (uint)amount;
                    break;
            }
        }
    }

    public enum StardewStats
    {
        PiecesOfTrashRecycled,
        GoatCheeseMade,
        CheeseMade,
        PreservesMade,
        BeveragesMade
    }
}
