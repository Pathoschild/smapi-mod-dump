/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.ExpandedStorage.Framework;

using StardewMods.Common.Interfaces;
using StardewMods.Common.Services;
using StardewMods.Common.Services.Integrations.ExpandedStorage;
using StardewMods.Common.Services.Integrations.FauxCore;
using StardewMods.ExpandedStorage.Framework.Models;
using StardewMods.ExpandedStorage.Framework.Services;

/// <inheritdoc />
public sealed class ExpandedStorageApi : IExpandedStorageApi
{
    private readonly AssetHandler assetHandler;
    private readonly BaseEventManager eventManager;
    private readonly IModInfo modInfo;

    /// <summary>Initializes a new instance of the <see cref="ExpandedStorageApi" /> class.</summary>
    /// <param name="eventSubscriber">Dependency used for subscribing to events.</param>
    /// <param name="log">Dependency used for monitoring and logging.</param>
    /// <param name="modInfo">Mod info from the calling mod.</param>
    /// <param name="assetHandler">Dependency for managing expanded storage chests.</param>
    internal ExpandedStorageApi(IEventSubscriber eventSubscriber, ILog log, IModInfo modInfo, AssetHandler assetHandler)
    {
        // Init
        this.modInfo = modInfo;
        this.assetHandler = assetHandler;
        this.eventManager = new BaseEventManager(log, modInfo.Manifest);

        // Events
        eventSubscriber.Subscribe<ChestCreatedEventArgs>(this.OnChestCreated);
    }

    /// <inheritdoc />
    public bool TryGetData(Item item, [NotNullWhen(true)] out IStorageData? storageData) =>
        this.assetHandler.TryGetData(item, out storageData);

    /// <inheritdoc />
    public void Subscribe<TEventArgs>(Action<TEventArgs> handler) => this.eventManager.Subscribe(handler);

    /// <inheritdoc />
    public void Unsubscribe<TEventArgs>(Action<TEventArgs> handler) => this.eventManager.Unsubscribe(handler);

    private void OnChestCreated(ChestCreatedEventArgs e) =>
        this.eventManager.Publish<IChestCreatedEventArgs, ChestCreatedEventArgs>(e);
}