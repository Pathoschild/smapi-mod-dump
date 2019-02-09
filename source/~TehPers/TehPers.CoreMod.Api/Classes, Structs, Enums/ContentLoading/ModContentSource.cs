using StardewModdingAPI;

namespace TehPers.CoreMod.Api.ContentLoading {
    public class ModContentSource : IContentSource {
        private readonly IContentHelper _contentHelper;
        public string Path { get; }

        public ModContentSource(IModHelper helper) {
            this._contentHelper = helper.Content;
            this.Path = helper.DirectoryPath;
        }

        public T Load<T>(string path) {
            return this._contentHelper.Load<T>(path);
        }
    }
}