/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Services.Integrations.FauxCore;

using System.Reflection;
using StardewMods.Common.Enums;

/// <summary>Represents a patch for modifying a method using Harmony.</summary>
public interface ISavedPatch
{
    /// <summary>Gets the unique identifier of the patch.</summary>
    string? LogId { get; }

    /// <summary>Gets the original method.</summary>
    public MethodBase Original { get; }

    /// <summary>Gets the harmony method.</summary>
    public MethodInfo Patch { get; }

    /// <summary>Gets the patch type.</summary>
    public PatchType Type { get; }
}