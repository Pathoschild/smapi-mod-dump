/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace Common.Helpers;

using System;
using System.Linq;
using System.Reflection;

/// <summary>
///     Helper for identifying an Assembly by its name.
/// </summary>
internal static class ReflectionHelper
{
    /// <summary>
    ///     Get the first Assembly that matches the name.
    /// </summary>
    /// <param name="name">The name of the assembly.</param>
    /// <returns>The first assembly that matches the name.</returns>
    public static Assembly GetAssemblyByName(string name)
    {
        return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.FullName?.StartsWith($"{name},") == true);
    }
}