/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using System;

#endregion using directives

/// <summary>Extensions for the <see cref="string"/> primitive type.</summary>
public static class StringExtensions
{
    /// <summary>Get the index of a given profession by name.</summary>
    public static int ToProfessionIndex(this string professionName)
    {
        if (Enum.TryParse<Profession>(professionName, true, out var profession)) return (int) profession;
        throw new ArgumentException($"Profession {professionName} is not recognized.");
    }
}