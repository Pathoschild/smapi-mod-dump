using PurrplingCore.Bridges;
using StardewModdingAPI;

namespace QuestFramework.Framework.Bridges
{
    class Bridge
    {
        public Bridge(IModRegistry mod)
        {
            this.Mod = mod;
        }

        private IModRegistry Mod { get; }
        public IJsonAssetsApi JsonAssets { get; private set; }

        private TApi LoadApi<TApi>(string modUid) where TApi : class
        {
            return this.Mod.IsLoaded(modUid) 
                ? this.Mod.GetApi<TApi>(modUid) 
                : default;
        }

        public void Init()
        {
            this.JsonAssets = this.LoadApi<IJsonAssetsApi>("spacechase0.JsonAssets");
        }
    }
}
