/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/OfficialRenny/PrairieKingPrizes
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrairieKingPrizes.Framework
{
    internal static class Extensions
    {
        internal static PrizeTier PickPrizeTier(this Random random, IEnumerable<PrizeTier> prizeTiers)
        {
            var totalSum = prizeTiers.Sum(x => x.Chance);
            var randomNumber = random.NextDouble() * totalSum;

            double sum = 0;
            foreach (var prizeTier in prizeTiers)
            {
                if (randomNumber <= (sum += prizeTier.Chance))
                    return prizeTier;
            }

            return prizeTiers.Last();
        }

        internal static Prize PickPrize(this Random random, PrizeTier prizeTier)
        {
            return prizeTier.Prizes[random.Next(prizeTier.Prizes.Length)];
        } 
    }
}
