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
namespace StardewMods.FauxCore.Common.Models;

using System.Reflection;
using StardewMods.FauxCore.Common.Enums;
using StardewMods.FauxCore.Common.Services.Integrations.FauxCore;

#else
namespace StardewMods.Common.Models;

using System.Reflection;
using StardewMods.Common.Enums;
using StardewMods.Common.Services.Integrations.FauxCore;
#endif

/// <inheritdoc />
internal sealed class SavedPatch : ISavedPatch
{
    /// <summary>Initializes a new instance of the <see cref="SavedPatch" /> class.</summary>
    /// <param name="original">The original method represented by a MethodBase object.</param>
    /// <param name="patch">The patch method represented by a MethodInfo object.</param>
    /// <param name="patchType">The type of the patch, represented by the PatchType enumeration.</param>
    /// <param name="logId">The log ID for the saved patch. This parameter is optional and defaults to null.</param>
    public SavedPatch(MethodBase original, MethodInfo patch, PatchType patchType, string? logId = default)
    {
        this.Original = original;
        this.Patch = patch;
        this.Type = patchType;
        this.LogId = logId;
    }

    /// <inheritdoc />
    public string? LogId { get; }

    /// <inheritdoc />
    public MethodBase Original { get; }

    /// <inheritdoc />
    public MethodInfo Patch { get; }

    /// <inheritdoc />
    public PatchType Type { get; }
}