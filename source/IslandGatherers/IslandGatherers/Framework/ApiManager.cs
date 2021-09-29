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
    public static class ApiManager
    {
        private static DynamicGameAssetsApi dynamicGameAssets;
        private static IExpandedStorageAPI expandedStorageApi;
        private static IGenericModConfigMenuAPI genericModConfigMenuApi;

        public static void HookIntoDynamicGameAssets(IModHelper helper)
        {
            // Attempt to hook into the IMobileApi interface
            dynamicGameAssets = helper.ModRegistry.GetApi<DynamicGameAssetsApi>("spacechase0.DynamicGameAssets");

            if (dynamicGameAssets is null)
            {
                IslandGatherers.monitor.Log("Failed to hook into spacechase0.DynamicGameAssets.", LogLevel.Error);
                return;
            }

            IslandGatherers.monitor.Log("Successfully hooked into spacechase0.DynamicGameAssets.", LogLevel.Debug);
        }

        public static DynamicGameAssetsApi GetDynamicGameAssetsInterface()
        {
            return dynamicGameAssets;
        }


        public static bool HookIntoExpandedStorage(IModHelper helper)
        {
            // Attempt to hook into the IMobileApi interface
            expandedStorageApi = helper.ModRegistry.GetApi<IExpandedStorageAPI>("furyx639.ExpandedStorage");

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

        internal static IExpandedStorageAPI GetExpandedStorageApi()
        {
            return expandedStorageApi;
        }

        public static IGenericModConfigMenuAPI GetGMCMInterface()
        {
            return genericModConfigMenuApi;
        }
        public static string GetParrotPotModDataFlag()
        {
            return IslandGatherers.parrotPotFlag;
        }
    }
}
