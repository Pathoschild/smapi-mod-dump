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

namespace SkillPrestige.Professions
{
    public partial class Profession
    {
        /*********
        ** Accessors
        *********/
        public static IEnumerable<Profession> ForagingProfessions => new List<Profession>
        {
            Forester,
            Gatherer,
            Lumberjack,
            Tapper,
            Botanist,
            Tracker
        };

        protected static TierOneProfession Forester { get; set; }
        protected static TierOneProfession Gatherer { get; set; }
        protected static TierTwoProfession Lumberjack { get; set; }
        protected static TierTwoProfession Tapper { get; set; }
        protected static TierTwoProfession Botanist { get; set; }
        protected static TierTwoProfession Tracker { get; set; }
    }
}
