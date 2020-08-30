using QuestFramework.Framework.Bridges.APIs;
using StardewModdingAPI;

namespace QuestFramework.Framework.Bridges
{
    internal class Bridge
    {
        private readonly bool debugMode;

        public Bridge(IModRegistry modRegistry, bool debugMode)
        {
            this.ModRegistry = modRegistry;
            this.debugMode = debugMode;
        }

        private IModRegistry ModRegistry { get; }
        public IJsonAssetsApi JsonAssets { get; private set; }
        public IConditionsChecker EPU { get; private set; }

        private TApi LoadApi<TApi>(string modUid) where TApi : class
        {
            return this.ModRegistry.IsLoaded(modUid) 
                ? this.ModRegistry.GetApi<TApi>(modUid) 
                : default;
        }

        public void Init()
        {
            this.JsonAssets = this.LoadApi<IJsonAssetsApi>(ApiIdentifiers.JSON_ASSETS);
            this.EPU = this.LoadApi<IConditionsChecker>(ApiIdentifiers.EPU);

            this.EPU?.Initialize(this.debugMode, this.ModRegistry.ModID);
        }
    }
}
