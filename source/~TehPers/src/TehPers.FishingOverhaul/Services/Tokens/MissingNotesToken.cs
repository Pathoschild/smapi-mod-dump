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
using StardewModdingAPI.Events;
using StardewValley;
using TehPers.Core.Api.Content;
using TehPers.Core.Api.Setup;

namespace TehPers.FishingOverhaul.Services.Tokens
{
    internal abstract class MissingNotesToken : ISetup, IDisposable
    {
        private const string secretNotesAsset = @"Data\SecretNotes";

        private readonly IModHelper helper;
        private readonly IAssetProvider gameAssets;
        private bool updated;
        protected IDictionary<int, string> SecretNotes { get; private set; }

        protected MissingNotesToken(IModHelper helper, IAssetProvider gameAssets)
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.gameAssets = gameAssets ?? throw new ArgumentNullException(nameof(gameAssets));

            this.updated = true;
            this.SecretNotes =
                this.gameAssets.Load<Dictionary<int, string>>(MissingNotesToken.secretNotesAsset);
        }

        void ISetup.Setup()
        {
            this.helper.Events.Content.AssetReady += this.OnAssetLoading;
        }

        void IDisposable.Dispose()
        {
            this.helper.Events.Content.AssetReady -= this.OnAssetLoading;
        }

        private void OnAssetLoading(object? sender, AssetReadyEventArgs e)
        {
            if (!e.NameWithoutLocale.IsEquivalentTo(MissingNotesToken.secretNotesAsset))
            {
                return;
            }

            this.updated = true;
            this.SecretNotes =
                this.gameAssets.Load<Dictionary<int, string>>(MissingNotesToken.secretNotesAsset);
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
