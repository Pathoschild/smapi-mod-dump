/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;

namespace TheLion.Stardew.Common.Harmony;

public static class MethodInfoExtensions
{
    /// <summary>Construct a <see cref="HarmonyMethod" /> instance from a <see cref="MethodInfo" /> object.</summary>
    /// <returns>
    ///     Returns a new <see cref="HarmonyMethod" /> instance if <paramref name="method" /> is not null, or <c>null</c>
    ///     otherwise.
    /// </returns>
    [CanBeNull]
    public static HarmonyMethod ToHarmonyMethod(this MethodInfo method)
    {
        return method is null ? null : new HarmonyMethod(method);
    }
}