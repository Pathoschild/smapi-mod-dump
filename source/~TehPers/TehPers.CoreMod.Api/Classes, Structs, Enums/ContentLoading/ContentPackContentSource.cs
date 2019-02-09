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