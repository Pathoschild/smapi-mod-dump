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
    internal class Lootbox
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public PrizeTier[] PrizeTiers { get; set; }
    }
}
