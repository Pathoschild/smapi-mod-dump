/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace TMXLoader
{
    public class AssetLoadInjector<TAsset>
    {
        private readonly TAsset asset;
        private readonly Func<IAssetName, bool> predicate;

        public void Invalidate()
        {
            TMXLoaderMod.helper.GameContent.InvalidateCache(a => predicate(a.Name));
        }

        public AssetLoadInjector(string assetName, TAsset asset)
        {
            this.asset = asset;
            predicate = a => a.IsEquivalentTo(assetName, useBaseName: true);
        }

        internal void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (predicate(e.NameWithoutLocale))
                e.LoadFrom(() => this.asset, AssetLoadPriority.Medium);

        }
    }
}
