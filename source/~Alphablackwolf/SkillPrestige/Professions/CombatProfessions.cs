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
        public static IEnumerable<Profession> CombatProfessions => new List<Profession>
        {
            Fighter,
            Scout,
            Brute,
            Defender,
            Acrobat,
            Desperado
        };

        protected static TierOneProfession Fighter { get; set; }
        protected static TierOneProfession Scout { get; set; }
        protected static TierTwoProfession Brute { get; set; }
        protected static TierTwoProfession Defender { get; set; }
        protected static TierTwoProfession Acrobat { get; set; }
        protected static TierTwoProfession Desperado { get; set; }
    }
}
