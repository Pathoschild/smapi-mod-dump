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
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.Setup;

namespace TehPers.FishingOverhaul.Services.Tokens
{
    internal abstract class MissingNotesToken : ISetup, IDisposable
    {
        private const string secretNotesAsset = @"Data\SecretNotes";

        private readonly IAssetTracker assetTracker;

        private bool updated;
        protected IDictionary<int, string> SecretNotes { get; private set; }

        protected MissingNotesToken(IAssetTracker assetTracker, IAssetProvider gameAssets)
        {
            this.assetTracker =
                assetTracker ?? throw new ArgumentNullException(nameof(assetTracker));

            this.updated = true;
            this.SecretNotes =
                gameAssets.Load<Dictionary<int, string>>(MissingNotesToken.secretNotesAsset);
        }

        void ISetup.Setup()
        {
            this.assetTracker.AssetLoading += this.OnAssetLoading;
        }

        void IDisposable.Dispose()
        {
            this.assetTracker.AssetLoading -= this.OnAssetLoading;
        }

        private void OnAssetLoading(object? sender, IAssetData e)
        {
            if (!e.AssetNameEquals(MissingNotesToken.secretNotesAsset))
            {
                return;
            }

            this.updated = true;
            this.SecretNotes = e.AsDictionary<int, string>().Data;
        }

        public virtual bool IsReady()
        {
            return Game1.player is not null && this.SecretNotes.Any();
        }

        public virtual bool UpdateContext()
        {
            var updated = this.updated;
            this.updated = false;
            return updated;
        }

        public abstract IEnumerable<string> GetValues(string? input);
    }
}