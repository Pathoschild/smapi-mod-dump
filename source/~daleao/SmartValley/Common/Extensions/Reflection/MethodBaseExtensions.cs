/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

#nullable enable
namespace DaLion.Common.Extensions.Reflection;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;

#endregion using directives

/// <summary>Extensions for the <see cref="MethodBase"/> class.</summary>
public static class MethodBaseExtensions
{
    /// <summary>Get all the patches applied to this method and that satisfy a given predicate.</summary>
    /// <param name="predicate">Filter conditions.</param>
    public static IEnumerable<Patch> GetAppliedPatches(this MethodBase method, Func<Patch, bool>? predicate = null)
    {
        predicate ??= _ => true;
        var patches = Harmony.GetPatchInfo(method);

        foreach (var patch in patches.Prefixes.Where(predicate)) yield return patch;
        foreach (var patch in patches.Postfixes.Where(predicate)) yield return patch;
        foreach (var patch in patches.Transpilers.Where(predicate)) yield return patch;
        foreach (var patch in patches.Finalizers.Where(predicate)) yield return patch;
    }

    /// <summary>Get the patches applied to this method with the specified unique id.</summary>
    /// <param name="uniqueID">A unique id to search for.</param>
    public static IEnumerable<Patch> GetAppliedPatchesById(this MethodBase method, IMonitor monitor, string uniqueID)
        => method.GetAppliedPatches(p => p.owner == uniqueID);
}