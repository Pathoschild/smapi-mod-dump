/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Shared.Extensions.Reflection;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

#endregion using directives

/// <summary>Extensions for the <see cref="MethodBase"/> class.</summary>
public static class MethodBaseExtensions
{
    /// <summary>The full name of the method, including the declaring type.</summary>
    /// <param name="method">The <see cref="MethodBase"/>.</param>
    /// <returns>A <see cref="string"/> representation of the <paramref name="method"/>'s qualified full name.</returns>
    public static string GetFullName(this MethodBase method)
    {
        return $"{method.DeclaringType}::{method.Name}";
    }

    /// <summary>
    ///     Gets all the <see cref="Patch"/> instances applied to the <paramref name="method"/> and that optionally
    ///     satisfy a given <paramref name="predicate"/>.
    /// </summary>
    /// <param name="method">The <see cref="MethodBase"/>.</param>
    /// <param name="predicate">Filter conditions.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Patch"/> instances currently applied to the <paramref name="method"/> and that satisfy the given <paramref name="predicate"/>, if any.</returns>
    public static IEnumerable<Patch> GetAppliedPatches(this MethodBase method, Func<Patch, bool>? predicate = null)
    {
        predicate ??= _ => true;
        var patches = Harmony.GetPatchInfo(method);
        if (patches is null)
        {
            yield break;
        }

        foreach (var patch in patches.Prefixes.Where(predicate))
        {
            yield return patch;
        }

        foreach (var patch in patches.Postfixes.Where(predicate))
        {
            yield return patch;
        }

        foreach (var patch in patches.Transpilers.Where(predicate))
        {
            yield return patch;
        }

        foreach (var patch in patches.Finalizers.Where(predicate))
        {
            yield return patch;
        }
    }

    /// <summary>
    ///     Gets all the <see cref="Patch"/> instances applied to the <paramref name="method"/> that include a <see cref="HarmonyPrefix"/>
    ///     and that optionally satisfy a given <paramref name="predicate"/>.
    /// </summary>
    /// <param name="method">The <see cref="MethodBase"/>.</param>
    /// <param name="predicate">Filter conditions.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Patch"/> instances currently applied to the <paramref name="method"/> which contain at least one <see cref="HarmonyPrefix"/>, and that satisfy the given <paramref name="predicate"/>, if any.</returns>
    public static IEnumerable<Patch> GetAppliedPrefixes(this MethodBase method, Func<Patch, bool>? predicate = null)
    {
        predicate ??= _ => true;
        var patches = Harmony.GetPatchInfo(method);
        if (patches is null)
        {
            yield break;
        }

        foreach (var patch in patches.Prefixes.Where(predicate))
        {
            yield return patch;
        }
    }

    /// <summary>
    ///     Gets all the <see cref="Patch"/> instances applied to the <paramref name="method"/> that include a <see cref="HarmonyPostfix"/>
    ///     and that optionally satisfy a given <paramref name="predicate"/>.
    /// </summary>
    /// <param name="method">The <see cref="MethodBase"/>.</param>
    /// <param name="predicate">Filter conditions.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Patch"/> instances currently applied to the <paramref name="method"/> which contain at least one <see cref="HarmonyPostfix"/>, and that satisfy the given <paramref name="predicate"/>, if any.</returns>
    public static IEnumerable<Patch> GetAppliedPostfixes(this MethodBase method, Func<Patch, bool>? predicate = null)
    {
        predicate ??= _ => true;
        var patches = Harmony.GetPatchInfo(method);
        if (patches is null)
        {
            yield break;
        }

        foreach (var patch in patches.Postfixes.Where(predicate))
        {
            yield return patch;
        }
    }

    /// <summary>
    ///     Gets all the <see cref="Patch"/> instances applied to the <paramref name="method"/> that include a <see cref="HarmonyTranspiler"/>
    ///     and that optionally satisfy a given <paramref name="predicate"/>.
    /// </summary>
    /// <param name="method">The <see cref="MethodBase"/>.</param>
    /// <param name="predicate">Filter conditions.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Patch"/> instances currently applied to the <paramref name="method"/> which contain at least one <see cref="HarmonyTranspiler"/>, and that satisfy the given <paramref name="predicate"/>, if any.</returns>
    public static IEnumerable<Patch> GetAppliedTranspilers(this MethodBase method, Func<Patch, bool>? predicate = null)
    {
        predicate ??= _ => true;
        var patches = Harmony.GetPatchInfo(method);
        if (patches is null)
        {
            yield break;
        }

        foreach (var patch in patches.Transpilers.Where(predicate))
        {
            yield return patch;
        }
    }

    /// <summary>
    ///     Gets all the <see cref="Patch"/> instances applied to the <paramref name="method"/> that include a <see cref="HarmonyFinalizer"/>
    ///     and that optionally satisfy a given <paramref name="predicate"/>.
    /// </summary>
    /// <param name="method">The <see cref="MethodBase"/>.</param>
    /// <param name="predicate">Filter conditions.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Patch"/> instances currently applied to the <paramref name="method"/> which contain at least one <see cref="HarmonyFinalizer"/>, and that satisfy the given <paramref name="predicate"/>, if any.</returns>
    public static IEnumerable<Patch> GetAppliedFinalizers(this MethodBase method, Func<Patch, bool>? predicate = null)
    {
        predicate ??= _ => true;
        var patches = Harmony.GetPatchInfo(method);
        if (patches is null)
        {
            yield break;
        }

        foreach (var patch in patches.Finalizers.Where(predicate))
        {
            yield return patch;
        }
    }

    /// <summary>
    ///     Gets the <see cref="Patch"/> instances applied to this method with the specified <paramref name="uniqueId"/>
    ///     .
    /// </summary>
    /// <param name="method">The <see cref="MethodBase"/>.</param>
    /// <param name="uniqueId">A unique ID to search for.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Patch"/> instances currently applied to the <paramref name="method"/> by the mod with the specified <paramref name="uniqueId"/>.</returns>
    public static IEnumerable<Patch> GetAppliedPatchesById(this MethodBase method, string uniqueId)
    {
        return method.GetAppliedPatches(p => p.owner == uniqueId);
    }
}
