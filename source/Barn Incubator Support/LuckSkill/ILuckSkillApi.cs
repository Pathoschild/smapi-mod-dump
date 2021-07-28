/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace LuckSkill
{
    /// <summary>The mod-provided API for Luck Skill.</summary>
    public interface ILuckSkillApi
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The ID for the Fortunate profession.</summary>
        int FortunateProfessionId { get; }

        /// <summary>The ID for the Popular Helper profession.</summary>
        int PopularHelperProfessionId { get; }

        /// <summary>The ID for the Lucky profession.</summary>
        int LuckyProfessionId { get; }

        /// <summary>The ID for the Un-unlucky profession.</summary>
        int UnUnluckyProfessionId { get; }

        /// <summary>The ID for the Shooting Star profession.</summary>
        int ShootingStarProfessionId { get; }

        /// <summary>The ID for the Spirit Child profession.</summary>
        int SpiritChildProfessionId { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Get the available Luck professions by ID.</summary>
        IDictionary<int, IProfession> GetProfessions();
    }
}
