/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.CustomBush.Framework.Services;

using StardewMods.Common.Interfaces;

/// <inheritdoc />
internal sealed class ApiFactory : IApiFactory
{
    private readonly AssetHandler assetHandler;
    private readonly ModPatches modPatches;

    /// <summary>Initializes a new instance of the <see cref="ApiFactory" /> class.</summary>
    /// <param name="assetHandler">Dependency used for handling assets.</param>
    /// <param name="modPatches">Dependency for managing custom bushes.</param>
    public ApiFactory(AssetHandler assetHandler, ModPatches modPatches)
    {
        this.assetHandler = assetHandler;
        this.modPatches = modPatches;
    }

    /// <inheritdoc />
    public object CreateApi(IModInfo modInfo) => new CustomBushApi(this.assetHandler, modInfo, this.modPatches);
}