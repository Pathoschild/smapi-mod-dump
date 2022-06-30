/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Common.Integrations;

#region using directives

using System.Collections.Generic;

#endregion using directives

public interface ILuckSkillAPI
{
    IDictionary<int, IProfession> GetProfessions();

    public interface IProfession
    {
        int Id { get; }

        string DefaultName { get; }

        string Name { get; }

        string DefaultDescription { get; }

        string Description { get; }
    }
}