/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/HandyHeadphones
**
*************************************************/

using HandyHeadphones.API.Interfaces;
using StardewModdingAPI;

namespace HandyHeadphones.API.Interfaces
{
    public static class ApiManager
    {
        private static IMonitor monitor = ModEntry.monitor;
        private static IJsonAssetApi jsonAssetApi;

        public static void HookIntoJsonAssets(IModHelper helper)
        {
            jsonAssetApi = helper.ModRegistry.GetApi<IJsonAssetApi>("spacechase0.JsonAssets");

            if (jsonAssetApi is null)
            {
                monitor.Log("Failed to hook into spacechase0.JsonAssets.", LogLevel.Error);
                return;
            }

            monitor.Log("Successfully hooked into spacechase0.JsonAssets.", LogLevel.Debug);
        }

        public static IJsonAssetApi GetJsonAssetInterface()
        {
            return jsonAssetApi;
        }

        public static int GetHeadphonesID()
        {
            return jsonAssetApi.GetHatId("Headphones");
        }
    }
}
