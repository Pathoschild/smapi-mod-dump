/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using StardewValley;
using System.Collections.Generic;

namespace MultiplayerExpShare.Integrations;

public interface IModularOverhaul
{
    /// <summary>Interface for all of the <see cref="Farmer"/>'s professions.</summary>
    public interface IProfession
    {
        /// <summary>Gets a string that uniquely identifies this profession.</summary>
        string StringId { get; }

        /// <summary>Gets the localized and gendered name for this profession.</summary>
        string DisplayName { get; }

        /// <summary>Gets the index used in-game to track professions acquired by the player.</summary>
        int Id { get; }

        /// <summary>Gets the level at which this profession is offered.</summary>
        /// <remarks>Either 5 or 10.</remarks>
        int Level { get; }

        /// <summary>Gets the <see cref="ISkill"/> which offers this profession.</summary>
        ISkill Skill { get; }

        /// <summary>Gets get the professions which branch off from this profession, if any.</summary>
        IEnumerable<int> BranchingProfessions { get; }

        /// <summary>Get the localized description text for this profession.</summary>
        /// <param name="prestiged">Whether to get the prestiged or normal variant.</param>
        /// <returns>A human-readability <see cref="string"/> description of the profession.</returns>
        string GetDescription(bool prestiged = false);
    }

    /// <summary>Interface for all of the <see cref="Farmer"/>'s skills.</summary>
    public interface ISkill
    {
        /// <summary>Gets the skill's unique string id.</summary>
        string StringId { get; }

        /// <summary>Gets the localized in-game name of this skill.</summary>
        string DisplayName { get; }

        /// <summary>Gets the current experience total gained by the local player for this skill.</summary>
        int CurrentExp { get; }

        /// <summary>Gets the current level for this skill.</summary>
        int CurrentLevel { get; }

        /// <summary>Gets the amount of experience required for the next level-up.</summary>
        int ExperienceToNextLevel { get; }

        /// <summary>Gets the base experience multiplier set by the player for this skill.</summary>
        float BaseExperienceMultiplier { get; }

        /// <summary>Gets the new levels gained during the current game day, which have not yet been accomplished by an overnight menu.</summary>
        IEnumerable<int> NewLevels { get; }

        /// <summary>Gets the <see cref="IProfession"/>s associated with this skill.</summary>
        IList<IProfession> Professions { get; }

        /// <summary>Gets integer ids used in-game to track professions acquired by the player.</summary>
        IEnumerable<int> ProfessionIds { get; }

        /// <summary>Gets subset of <see cref="ProfessionIds"/> containing only the level five profession ids.</summary>
        /// <remarks>Should always contain exactly 2 elements.</remarks>
        IEnumerable<int> TierOneProfessionIds { get; }

        /// <summary>Gets subset of <see cref="ProfessionIds"/> containing only the level ten profession ids.</summary>
        /// <remarks>
        ///     Should always contains exactly 4 elements. The elements are assumed to be ordered correctly with respect to
        ///     <see cref="TierOneProfessionIds"/>, such that elements 0 and 1 in this array correspond to branches of element 0
        ///     in the latter, and elements 2 and 3 correspond to branches of element 1.
        /// </remarks>
        IEnumerable<int> TierTwoProfessionIds { get; }
    }
}