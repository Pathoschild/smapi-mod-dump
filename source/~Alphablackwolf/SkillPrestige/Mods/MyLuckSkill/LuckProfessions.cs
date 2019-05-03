using System.Collections.Generic;

// ReSharper disable once CheckNamespace - must be in same namespace as the other professions classes.
namespace SkillPrestige.Professions
{
    public partial class Profession
    {
        /// <summary>
        /// Professions created by the Luck Skill Mod.
        /// </summary>
        public static IEnumerable<Profession> LuckProfessions => new List<Profession>
        {
            Lucky,
            Quester,
            SpecialCharm,
            LuckA2,
            NightOwl,
            LuckB2
        };

        protected static TierOneProfession Lucky { get; set; }
        protected static TierOneProfession Quester { get; set; }
        protected static TierTwoProfession SpecialCharm { get; set; }
        /// <summary>
        /// Profession has not been given a function by the Luck Skill mod.
        /// </summary>
        protected static TierTwoProfession LuckA2 { get; set; }

        protected static TierTwoProfession NightOwl { get; set; }
        /// <summary>
        /// Profession has not been given a function by the Luck Skill mod.
        /// </summary>
        protected static TierTwoProfession LuckB2 { get; set; }
    }
}
