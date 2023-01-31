/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Extensions.Reflection;

#region using directives

using DaLion.Shared.Exceptions;
using HarmonyLib;

#endregion using directives

/// <summary>Extensions for the <see cref="string"/> primitive type.</summary>
public static class StringExtensions
{
    /// <summary>Gets a type in the assembly by <paramref name="name"/> and asserts that it was found.</summary>
    /// <param name="name">The name of some type in any executing assembly.</param>
    /// <returns>The corresponding <see cref="Type"/>, if found.</returns>
    /// <exception cref="MissingTypeException">If the requested type is not found.</exception>
    public static Type ToType(this string name)
    {
        return AccessTools.TypeByName(name) ?? ThrowHelperExtensions.ThrowMissingTypeException(name);
    }
}
