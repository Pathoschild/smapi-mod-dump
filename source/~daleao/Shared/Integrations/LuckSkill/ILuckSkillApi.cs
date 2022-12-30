/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

#pragma warning disable CS1591
namespace DaLion.Shared.Integrations.LuckSkill;

#region using directives

using System.Collections.Generic;

#endregion using directives

/// <summary>The API provided by Luck Skill mod.</summary>
public interface ILuckSkillApi
{
    public interface IProfession
    {
        int Id { get; }

        string DefaultName { get; }

        string Name { get; }

        string DefaultDescription { get; }

        string Description { get; }
    }

    IDictionary<int, IProfession> GetProfessions();
}
