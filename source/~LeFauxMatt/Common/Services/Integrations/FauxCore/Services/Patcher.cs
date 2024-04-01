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

/// <inheritdoc />
internal sealed class Patcher(FauxCoreIntegration fauxCore, ILog log) : IPatchManager
{
    private readonly IPatchManager patchManager = fauxCore.Api!.CreatePatchService(log);

    /// <inheritdoc />
    public void Add(string id, params ISavedPatch[] patches) => this.patchManager.Add(id, patches);

    /// <inheritdoc />
    public void Patch(string id) => this.patchManager.Patch(id);

    /// <inheritdoc />
    public void Unpatch(string id) => this.patchManager.Unpatch(id);
}