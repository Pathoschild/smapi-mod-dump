/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Common.Harmony;

#region using directives

using HarmonyLib;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

#endregion using directives

/// <summary>Instantiates and applies <see cref="HarmonyPatch"/> classes in the assembly.</summary>
internal class Harmonizer
{
    /// <inheritdoc cref="Harmony"/>
    private readonly Harmony _Harmony;

    /// <summary>Construct an instance.</summary>
    /// <param name="uniqueID">The unique mod ID.</param>
    internal Harmonizer(string uniqueID)
    {
        _Harmony = new(uniqueID);
    }

    /// <summary>Instantiate and apply one of every <see cref="IHarmonyPatch" /> class in the assembly using reflection.</summary>
    internal void ApplyAll()
    {
        var sw = new Stopwatch();
        sw.Start();

        Log.D("[Harmonizer]: Gathering patches...");
        var patchTypes = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(IHarmonyPatch)))
            .Where(t => t.IsAssignableTo(typeof(IHarmonyPatch)) && !t.IsAbstract)
            .ToArray();

        Log.D($"[Harmonizer]: Found {patchTypes.Length} patch classes. Applying patches...");
        foreach (var p in patchTypes)
        {
            try
            {
                var patch = (IHarmonyPatch)p
                    .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null)!
                    .Invoke(Array.Empty<object>());
                patch.Apply(_Harmony);
                Log.D($"[Harmonizer]: Applied {p.Name} to {patch.Target!.DeclaringType}::{patch.Target.Name}.");
            }
            catch (MissingMethodException ex)
            {
                Log.D($"[Harmonizer]: {ex.Message} The {p.Name} type will be ignored.");
            }
            catch (Exception ex)
            {
                Log.E($"[Harmonizer]: Failed to apply {p.Name}.\nHarmony returned {ex}");
            }
        }

        sw.Stop();
        Log.D($"[Harmonizer]: Patching completed in {sw.ElapsedMilliseconds}ms.");
    }
}