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
    internal class ModConfig
    {
        public int BasicBoxCost { get; set; } = 10;
        public int PremiumBoxCost { get; set; } = 50;
        public bool RequireGameCompletion { get; set; } = false;
        public bool AlternateCoinMethod { get; set; } = false;
    }
}
