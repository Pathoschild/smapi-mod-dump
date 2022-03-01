/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Extensions;

#region using directives

using System;

#endregion using directives

internal static class StringExtensions
{
    /// <summary>Get the index of a given profession by name.</summary>
    public static int ToProfessionIndex(this string professionName)
    {
        if (Enum.TryParse<Profession>(professionName, out var profession)) return (int)profession;
        throw new ArgumentException($"Profession {professionName} does not exist.");
    }

}