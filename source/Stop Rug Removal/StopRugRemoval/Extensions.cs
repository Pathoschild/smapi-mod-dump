/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StopRugRemoval
**
*************************************************/

using System.Reflection;
using NotNullAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;

namespace StopRugRemoval;

/// <summary>
/// Small extensions to get the full name of a method.
/// </summary>
internal static class MethodExtensions
{
    /// <summary>
    /// Gets the fully qualified name of a method.
    /// </summary>
    /// <param name="method">MethodBase to analyze.</param>
    /// <returns>Fully qualified name.</returns>
    [Pure]
    public static string GetFullName([NotNull] this MethodBase method) => $"{method.DeclaringType}::{method.Name}";

    /// <summary>
    /// Gets the fully qualified name of a method.
    /// </summary>
    /// <param name="method">MethodInfo to analyze.</param>
    /// <returns>Fully qualified name.</returns>
    [Pure]
    public static string GetFullName([NotNull] this MethodInfo method) => $"{method.DeclaringType}::{method.Name}";
}
