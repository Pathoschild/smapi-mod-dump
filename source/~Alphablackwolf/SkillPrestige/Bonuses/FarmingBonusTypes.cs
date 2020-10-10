/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System.Collections.Generic;

namespace SkillPrestige.Bonuses
{
    public partial class BonusType
    {
        public static IEnumerable<BonusType> FarmingBonusTypes => new List<BonusType>
        {
            FarmingToolProficiency,
            BetterCrops,
            EfficientAnimals,
            RegrowthOpportunity
        };

        protected static BonusType FarmingToolProficiency { get; set; }
        protected static BonusType BetterCrops { get; set; }
        protected static BonusType EfficientAnimals { get; set; }
        protected static BonusType RegrowthOpportunity { get; set; }
    }
}