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

namespace MultiplayerExpShare.Integrations;

/// <summary>Implementation of the mod API.</summary>
public interface IModularOverhaulApi
{
    /// <summary>Sets a flag to allow the specified SpaceCore skill to level past 10 and offer prestige professions.</summary>
    /// <param name="id">The SpaceCore skill id.</param>
    /// <remarks>
    ///     All this does is increase the level cap for the skill with the specified <paramref name="id"/>.
    ///     The custom Skill mod author is responsible for making sure their professions return the correct
    ///     description and icon when prestiged. To check if a <see cref="Farmer"/> instance has a given prestiged
    ///     profession, simply add 100 to the profession's base ID.
    /// </remarks>
    void RegisterCustomSkillForPrestige(string id);
}