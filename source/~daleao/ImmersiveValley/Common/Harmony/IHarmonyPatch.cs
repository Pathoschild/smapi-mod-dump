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
using System.Reflection;

#endregion using directives

/// <summary>Interface for a <see cref="Harmony"/> patch class targeting a single method.</summary>
internal interface IHarmonyPatch
{
    /// <summary>The method to be patched.</summary>
    MethodBase? Target { get; }

    /// <summary>The <see cref="HarmonyPrefix"/> patch that should be applied, if any.</summary>
    HarmonyMethod? Prefix { get; }

    /// <summary>The <see cref="HarmonyPostfix"/> patch that should be applied, if any.</summary>
    HarmonyMethod? Postfix { get; }

    /// <summary>The <see cref="HarmonyTranspiler"/> patch that should be applied, if any.</summary>
    HarmonyMethod? Transpiler { get; }

    /// <summary>The <see cref="HarmonyFinalizer"/> patch that should be applied, if any.</summary>
    HarmonyMethod? Finalizer { get; }

    /// <summary>The <see cref="HarmonyReversePatch"/> patch that should be applied, if any.</summary>
    HarmonyMethod? Reverse { get; }

    /// <summary>Apply internally-defined Harmony patches.</summary>
    /// <param name="harmony">The Harmony instance for this mod.</param>
    void Apply(Harmony harmony);
}