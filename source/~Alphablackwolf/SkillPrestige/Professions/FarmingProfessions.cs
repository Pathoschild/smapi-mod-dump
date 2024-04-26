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
        public static IEnumerable<Profession> FarmingProfessions => new List<Profession>
        {
            Rancher,
            Tiller,
            Coopmaster,
            Shepherd,
            Artisan,
            Agriculturist
        };
        protected static TierOneProfession Rancher { get; set; }
        protected static TierOneProfession Tiller { get; set; }
        protected static TierTwoProfession Coopmaster { get; set; }
        protected static TierTwoProfession Shepherd { get; set; }
        protected static TierTwoProfession Artisan { get; set; }
        protected static TierTwoProfession Agriculturist { get; set; }
    }
}
