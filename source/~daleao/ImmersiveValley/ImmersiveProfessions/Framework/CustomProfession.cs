/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework;

/// <summary>Represents a custom profession tied to a mod-provided <see cref="ISkill"/>.</summary>
/// <param name="StringId">The string that uniquely identifies this profession.</param>
/// <param name="DisplayName">The localized in-game name of this profession.</param>
/// <param name="Description">The localized in-game description of this profession.</param>
/// <param name="Id">The integer id used in-game to track professions acquired by the player.</param>
/// <param name="Level">The level at which this profession is offered.</param>
/// <param name="Skill">The <see cref="ISkill"/> to which this profession belongs.</param>
/// <remarks>This applies to both SpaceCore <see cref="CustomSkill"/>s and the special-case <see cref="LuckSkill"/>.</remarks>
public record CustomProfession(string StringId, string DisplayName, string Description, int Id, int Level,
    ISkill Skill) : IProfession
{
    /// <inheritdoc />
    public string GetDisplayName(bool isMale = false) => DisplayName;

    /// <inheritdoc />
    public string GetDescription(bool prestiged = false) => Description;
}