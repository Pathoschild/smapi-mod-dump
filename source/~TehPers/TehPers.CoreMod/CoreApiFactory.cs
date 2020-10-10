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
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Drawing;
using TehPers.CoreMod.Items;

namespace TehPers.CoreMod {
    public class CoreApiFactory : ICoreApiFactory {
        private readonly Dictionary<IMod, ICoreApi> _coreApis = new Dictionary<IMod, ICoreApi>();
        private readonly ItemDelegator _itemDelegator;
        private readonly TextureTracker _textureTracker;

        internal CoreApiFactory(IMod mod, ItemDelegator itemDelegator) {
            this._itemDelegator = itemDelegator;

            // Create texture tracker
            this._textureTracker = new TextureTracker(mod);
            mod.Helper.Content.AssetEditors.Add(this._textureTracker);
        }

        public ICoreApi GetApi(IMod mod) => this.GetApi(mod, null);
        public ICoreApi GetApi(IMod mod, Action<ICoreApiInitializer> initialize) {
            ICoreApi coreApi = this._coreApis.GetOrAdd(mod, () => new CoreApi(mod, this._itemDelegator, this._textureTracker));
            initialize?.Invoke(new CoreApiInitializer(coreApi));
            return coreApi;
        }
    }
}