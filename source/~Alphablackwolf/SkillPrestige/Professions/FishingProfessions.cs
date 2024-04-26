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
        public static IEnumerable<Profession> FishingProfessions => new List<Profession>
        {
            Fisher,
            Trapper,
            Angler,
            Pirate,
            Mariner,
            Luremaster
        };

        protected static TierOneProfession Fisher { get; set; }
        protected static TierOneProfession Trapper { get; set; }
        protected static TierTwoProfession Angler { get; set; }
        protected static TierTwoProfession Pirate { get; set; }
        protected static TierTwoProfession Mariner { get; set; }
        protected static TierTwoProfession Luremaster { get; set; }
    }
}
