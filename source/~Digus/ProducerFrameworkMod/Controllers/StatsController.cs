/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using ProducerFrameworkMod.ContentPack;
using StardewValley;

namespace ProducerFrameworkMod.Controllers
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
            var statusValue = ProducerFrameworkModEntry.Helper.Reflection.GetProperty<uint>(Game1.stats, stats.ToString());
            statusValue.SetValue(statusValue.GetValue() + (uint) amount);
        }
    }
}
