/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/OfficialRenny/PrairieKingPrizes
**
*************************************************/

namespace PrairieKingPrizes.Framework
{
    internal class PrizeTier
    {
        public string Name { get; set; } = "";
        public double Chance { get; set; }
        public Prize[] Prizes { get; set; }
    }
}
