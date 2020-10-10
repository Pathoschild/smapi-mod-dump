/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cantorsdust/StardewMods
**
*************************************************/

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