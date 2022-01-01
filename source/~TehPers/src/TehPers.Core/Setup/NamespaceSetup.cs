/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.Linq;
using StardewModdingAPI;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Setup;

namespace TehPers.Core.Setup
{
    internal class NamespaceSetup : ISetup, IDisposable
    {
        private static readonly string[] itemAssets =
        {
            @"Data\ClothingInformation",
            @"Data\Boots",
            @"Data\hats",
            @"Data\weapons",
            @"Data\Furniture",
            @"Data\BigCraftablesInformation",
            @"Data\ObjectInformation",
            @"Data\SecretNotes",
        };

        private readonly IModHelper helper;
        private readonly IAssetTracker assetTracker;
        private readonly INamespaceRegistry namespaceRegistry;

        public NamespaceSetup(IModHelper helper, IAssetTracker assetTracker, INamespaceRegistry namespaceRegistry)
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.assetTracker = assetTracker ?? throw new ArgumentNullException(nameof(assetTracker));
            this.namespaceRegistry = namespaceRegistry
                ?? throw new ArgumentNullException(nameof(namespaceRegistry));
        }

        public void Setup()
        {
            this.helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            this.assetTracker.AssetLoading += this.OnAssetLoading;
        }

        public void Dispose()
        {
            this.helper.Events.GameLoop.SaveLoaded -= this.OnSaveLoaded;
            this.assetTracker.AssetLoading -= this.OnAssetLoading;
        }

        private void OnSaveLoaded(object? sender, EventArgs e)
        {
            this.namespaceRegistry.RequestReload();
        }

        private void OnAssetLoading(object? sender, IAssetData e)
        {
            // Reload namespace registry if any of the vanilla assets which provide item
            // information are reloaded
            if (NamespaceSetup.itemAssets.Any(e.AssetNameEquals))
            {
                this.namespaceRegistry.RequestReload();
            }
        }
    }
}