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
using GreenhouseGatherers.GreenhouseGatherers.API.Interfaces.DynamicGameAssets;
using GreenhouseGatherers.GreenhouseGatherers.API.Interfaces.ExpandedStorage;

namespace GreenhouseGatherers.GreenhouseGatherers.API
{
    public static class ApiManager
    {
        private static IMonitor monitor = ModResources.GetMonitor();

        private static DynamicGameAssetsApi dynamicGameAssets;
        private static IExpandedStorageAPI expandedStorageApi;

        public static void HookIntoDynamicGameAssets(IModHelper helper)
        {
            // Attempt to hook into the IMobileApi interface
            dynamicGameAssets = helper.ModRegistry.GetApi<DynamicGameAssetsApi>("spacechase0.DynamicGameAssets");

            if (dynamicGameAssets is null)
            {
                monitor.Log("Failed to hook into spacechase0.DynamicGameAssets.", LogLevel.Error);
                return;
            }

            monitor.Log("Successfully hooked into spacechase0.DynamicGameAssets.", LogLevel.Debug);
        }

        public static DynamicGameAssetsApi GetDynamicGameAssetsInterface()
        {
            return dynamicGameAssets;
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

        public static string GetHarvestStatueModDataFlag()
        {
            return ModEntry.harvestStatueFlag;
        }
    }
}
