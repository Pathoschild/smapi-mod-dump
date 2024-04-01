/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Enums;

using NetEscapades.EnumGenerators;

/// <summary>Specifies the type of patch to be applied.</summary>
[EnumExtensions]
public enum PatchType
{
    /// <summary>Prefix patch type.</summary>
    Prefix,

    /// <summary>Postfix patch type.</summary>
    Postfix,

    /// <summary>Transpiler patch type.</summary>
    Transpiler,

    /// <summary>Finalizer patch type.</summary>
    Finalizer,
}