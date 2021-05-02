/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/GreenhouseGatherers
**
*************************************************/

using StardewModdingAPI;
using GreenhouseGatherers.GreenhouseGatherers.API.Interfaces.JsonAssets;
using GreenhouseGatherers.GreenhouseGatherers.API.Interfaces.ExpandedStorage;

namespace GreenhouseGatherers.GreenhouseGatherers.API
{
    public static class ApiManager
    {
        private static IMonitor monitor = ModResources.GetMonitor();

        private static IJsonAssetApi jsonAssetApi;
        private static IExpandedStorageAPI expandedStorageApi;

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

        public static void HookIntoExpandedStorage(IModHelper helper)
        {
            // Attempt to hook into the IMobileApi interface
            expandedStorageApi = helper.ModRegistry.GetApi<IExpandedStorageAPI>("furyx639.ExpandedStorage");

            if (expandedStorageApi is null)
            {
                monitor.Log("Failed to hook into furyx639.ExpandedStorage.", LogLevel.Error);
                return;
            }

            monitor.Log("Successfully hooked into furyx639.ExpandedStorage.", LogLevel.Debug);
        }

        public static IExpandedStorageAPI GetExpandedStorageInterface()
        {
            return expandedStorageApi;
        }
    }
}
