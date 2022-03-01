/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.FuryCore.Enums;

/// <summary>
///     Type type of patch to apply.
/// </summary>
public enum PatchType
{
    /// <summary>Patches before the existing method.</summary>
    Prefix,

    /// <summary>Patches after the existing method.</summary>
    Postfix,

    /// <summary>Transpiles the existing method.</summary>
    Transpiler,
}