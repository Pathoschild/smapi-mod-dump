/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace TehPers.CoreMod.Api.ContentLoading {
    public class ContentPackContentSource : IContentSource {
        private readonly IContentPack _pack;
        public string Path => this._pack.DirectoryPath;

        public ContentPackContentSource(IContentPack pack) {
            this._pack = pack;
        }

        public T Load<T>(string path) {
            return this._pack.LoadAsset<T>(path);
        }
    }
}