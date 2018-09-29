namespace AllProfessions.Framework
{
    /// <summary>A set of professions to gain for a skill level.</summary>
    internal class ModDataProfessions
    {
        /// <summary>The skill to check.</summary>
        public Skill Skill { get; set; }

        /// <summary>The minimum skill level to gain the professions.</summary>
        public int Level { get; set; }

        /// <summary>The professions to gain.</summary>
        public Profession[] Professions { get; set; }
    }
}