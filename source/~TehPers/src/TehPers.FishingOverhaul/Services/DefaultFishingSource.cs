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
using System.Collections.Generic;
using StardewModdingAPI;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.DI;
using TehPers.FishingOverhaul.Api.Content;

namespace TehPers.FishingOverhaul.Services
{
    internal sealed partial class DefaultFishingSource : IFishingContentSource
    {
        private readonly IManifest manifest;
        private readonly IAssetProvider assetProvider;

        private readonly List<FishingContent> defaultContent = new();

        public event EventHandler? ReloadRequested;

        public DefaultFishingSource(
            IManifest manifest,
            [ContentSource(ContentSource.GameContent)] IAssetProvider assetProvider
        )
        {
            this.manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            this.assetProvider =
                assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));
        }

        public IEnumerable<FishingContent> Reload()
        {
            // Reload default content
            this.defaultContent.Clear();
            this.defaultContent.Add(this.GetDefaultFishData());
            this.defaultContent.Add(this.GetDefaultTrashData());
            this.defaultContent.Add(this.GetDefaultTreasureData());

            return this.defaultContent;
        }
    }
}