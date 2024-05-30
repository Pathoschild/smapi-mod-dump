/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

#if IS_FAUXCORE
namespace StardewMods.FauxCore.Common.Services.Integrations.FauxCore;

using StardewMods.FauxCore.Common.Enums;
#else
namespace StardewMods.Common.Services.Integrations.FauxCore;

using StardewMods.Common.Enums;
#endif

using System.Reflection;

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