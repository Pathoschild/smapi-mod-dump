/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/GreenhouseGatherers
**
*************************************************/

using GreenhouseGatherers.GreenhouseGatherers.API.Interfaces;
using StardewModdingAPI;

namespace GreenhouseGatherers.GreenhouseGatherers.API
{
    public static class ApiManager
    {
        private static IMonitor monitor = ModResources.GetMonitor();

        private static IJsonAssetApi jsonAssetApi;


        public static void HookIntoJsonAssets(IModHelper helper)
        {
            // Attempt to hook into the IMobileApi interface
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


        public static int GetHarvestStatueID()
        {
            if (jsonAssetApi is null)
            {
                return -1;
            }

            return jsonAssetApi.GetBigCraftableId("Harvest Statue");
        }
    }
}
