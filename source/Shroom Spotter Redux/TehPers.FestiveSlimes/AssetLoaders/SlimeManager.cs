/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.FestiveSlimes.AssetLoaders {
    internal abstract class SlimeManager : IAssetLoader {
        public ICoreApi CoreApi { get; }
        public IMod Mod => this.CoreApi.Owner;
        public string SlimeName { get; }
        public AssetLocation SlimeLocation { get; }
        public ITrackedTexture OverriddenTexture { get; }

        private Texture2D _lastTexture = null;

        protected SlimeManager(ICoreApi coreApi, string slimeName) {
            this.CoreApi = coreApi;
            this.SlimeName = slimeName;
            this.SlimeLocation = new AssetLocation($@"Characters\Monsters\{this.SlimeName}", ContentSource.GameContent);
            this.OverriddenTexture = coreApi.Drawing.GetTrackedTexture(this.SlimeLocation);

            this.OverriddenTexture.Drawing += (sender, info) => this.OverrideDraw(info);
        }

        public virtual bool CanLoad<T>(IAssetInfo asset) {
            return asset.AssetNameEquals($@"Characters\Monsters\{this.SlimeName}") && this.GetCurrentTexture(SDateTime.Today, out _);
        }

        public virtual T Load<T>(IAssetInfo asset) {
            return this.GetCurrentTexture(SDateTime.Today, out Texture2D texture) ? (T) (object) texture : throw new InvalidOperationException($"Unexpected asset {asset.AssetName}");
        }

        public bool InvalidateIfNeeded(SDateTime date) {
            if (this.GetCurrentTexture(date, out Texture2D newTexture)) {
                if (this._lastTexture == newTexture) {
                    return false;
                }

                // Texture should be overridden but the texture it should be isn't currently the texture being used
                this._lastTexture = newTexture;
                this.Mod.Helper.Content.InvalidateCache(this.SlimeLocation.Path);
                return true;
            }

            if (this._lastTexture == null) {
                return false;
            }

            // Texture shouldn't be overridden but currently is
            this._lastTexture = null;
            this.Mod.Helper.Content.InvalidateCache(this.SlimeLocation.Path);
            return true;

        }

        protected abstract bool GetCurrentTexture(SDateTime date, out Texture2D texture);
        protected abstract void OverrideDraw(IDrawingInfo info);
        public abstract void DrawHat(SDateTime date, SpriteBatch batch, Monster slime);
        public abstract IEnumerable<Item> GetExtraDrops(SDateTime date, Monster monster);
    }
}
