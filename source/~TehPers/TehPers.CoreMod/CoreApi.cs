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
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.ContentLoading;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Json;
using TehPers.CoreMod.ContentLoading;
using TehPers.CoreMod.Drawing;
using TehPers.CoreMod.Items;
using TehPers.CoreMod.Json;

namespace TehPers.CoreMod {
    internal class CoreApi : ICoreApi {
        private readonly Lazy<IDrawingApi> _drawing;
        private readonly Lazy<IItemApi> _items;
        private readonly Lazy<IJsonApi> _json;

        public IMod Owner { get; }
        public string Path => this.ContentSource.Path;
        public IContentSource ContentSource { get; }
        public ICoreTranslationHelper TranslationHelper { get; }
        public IDrawingApi Drawing => this._drawing.Value;
        public IItemApi Items => this._items.Value;
        public IJsonApi Json => this._json.Value;

        public CoreApi(IMod owner, ItemDelegator itemDelegator, TextureTracker textureTracker) {
            this.Owner = owner;
            this.ContentSource = new ModContentSource(owner);
            this.TranslationHelper = new CoreTranslationHelper(this);

            // Create the APIs
            this._drawing = new Lazy<IDrawingApi>(() => new DrawingApi(new ApiHelper(this, "Drawing"), textureTracker));
            this._items = new Lazy<IItemApi>(() => new ItemApi(new ApiHelper(this, "Items"), itemDelegator));
            this._json = new Lazy<IJsonApi>(() => new JsonApi(new ApiHelper(this, "Json")));
        }
    }
}