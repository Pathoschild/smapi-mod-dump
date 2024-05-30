/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Services.Factory;

using StardewMods.Common.Interfaces;

/// <inheritdoc />
internal sealed class ApiFactory : IApiFactory
{
    private readonly ConfigManager configManager;
    private readonly ContainerFactory containerFactory;
    private readonly ContainerHandler containerHandler;

    /// <summary>Initializes a new instance of the <see cref="ApiFactory" /> class.</summary>
    /// <param name="configManager">Dependency used for managing config data.</param>
    /// <param name="containerHandler">Dependency used for handling operations by containers.</param>
    /// <param name="containerFactory">Dependency used for accessing containers.</param>
    public ApiFactory(ConfigManager configManager, ContainerHandler containerHandler, ContainerFactory containerFactory)
    {
        this.configManager = configManager;
        this.containerHandler = containerHandler;
        this.containerFactory = containerFactory;
    }

    /// <inheritdoc />
    public object CreateApi(IModInfo modInfo) =>
        new BetterChestsApi(modInfo, this.configManager, this.containerHandler, this.containerFactory);
}