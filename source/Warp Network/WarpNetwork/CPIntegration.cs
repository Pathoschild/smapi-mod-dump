/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using StardewModdingAPI;

namespace WarpNetwork
{
    class CPIntegration
    {
        private static IMonitor Monitor;
        private static IModHelper Helper;
        private static Config Config;
        public static void Init(IMonitor monitor, IModHelper helper, Config config)
        {
            Monitor = monitor;
            Helper = helper;
            Config = config;
        }
        public static void AddTokens(IManifest manifest)
        {
            if (!Helper.ModRegistry.IsLoaded("pathoschild.ContentPatcher"))
                return;
            var api = Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            api.RegisterToken(manifest, "MenuEnabled", () => new[] { Config.MenuEnabled.ToString() });
        }
    }
}
