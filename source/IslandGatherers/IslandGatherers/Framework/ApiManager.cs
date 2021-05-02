/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/IslandGatherers
**
*************************************************/

using StardewModdingAPI;
using IslandGatherers.Framework.Interfaces;

namespace IslandGatherers.Framework
{
    internal static class ApiManager
    {
        private static IJsonAssetsApi jsonAssetsApi;
        private static IExpandedStorageApi expandedStorageApi;
        private static IGenericModConfigMenuAPI genericModConfigMenuApi;

        internal static bool HookIntoJsonAssets(IModHelper helper)
        {
            jsonAssetsApi = helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");

            if (jsonAssetsApi is null)
            {
                IslandGatherers.monitor.Log("Failed to hook into spacechase0.JsonAssets.", LogLevel.Error);
                return false;
            }

            IslandGatherers.monitor.Log("Successfully hooked into spacechase0.JsonAssets.", LogLevel.Debug);
            return true;
        }

        public static bool HookIntoExpandedStorage(IModHelper helper)
        {
            // Attempt to hook into the IMobileApi interface
            expandedStorageApi = helper.ModRegistry.GetApi<IExpandedStorageApi>("furyx639.ExpandedStorage");

            if (expandedStorageApi is null)
            {
                IslandGatherers.monitor.Log("Failed to hook into furyx639.ExpandedStorage.", LogLevel.Error);
                return false;
            }

            IslandGatherers.monitor.Log("Successfully hooked into furyx639.ExpandedStorage.", LogLevel.Debug);
            return true;
        }

        public static bool HookIntoGMCM(IModHelper helper)
        {
            genericModConfigMenuApi = helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

            if (genericModConfigMenuApi is null)
            {
                IslandGatherers.monitor.Log("Failed to hook into spacechase0.GenericModConfigMenu.", LogLevel.Error);
                return false;
            }

            IslandGatherers.monitor.Log("Successfully hooked into spacechase0.GenericModConfigMenu.", LogLevel.Debug);
            return true;
        }

        internal static IJsonAssetsApi GetJsonAssetsApi()
        {
            return jsonAssetsApi;
        }

        internal static IExpandedStorageApi GetExpandedStorageApi()
        {
            return expandedStorageApi;
        }

        public static IGenericModConfigMenuAPI GetGMCMInterface()
        {
            return genericModConfigMenuApi;
        }
    }
}
