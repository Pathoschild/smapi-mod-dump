using StardewModdingAPI;
using StardewValley;

namespace TehPers.CoreMod.Api.ContentLoading {
    public class GameContentSource : IContentSource {
        public string Path { get; } = System.IO.Path.Combine(Constants.ExecutionPath, "Content");

        public T Load<T>(string path) {
            return Game1.content.Load<T>(path);
        }
    }
}