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
using StardewModdingAPI;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.Setup;

namespace TehPers.Core.Content
{
    internal sealed class AssetTracker : IAssetEditor, IAssetTracker, ISetup, IDisposable
    {
        private readonly IModHelper helper;
        private readonly IMonitor monitor;

        public event EventHandler<IAssetData>? AssetLoading;

        public AssetTracker(IModHelper helper, IMonitor monitor)
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
        }

        public void Setup()
        {
            this.helper.Content.AssetEditors.Add(this);
        }

        public void Dispose()
        {
            this.helper.Content.AssetEditors.Remove(this);
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return true;
        }

        public void Edit<T>(IAssetData asset)
        {
            this.monitor.Log($"Reloading {asset.AssetName}");
            this.AssetLoading?.Invoke(this, asset);
        }
    }
}