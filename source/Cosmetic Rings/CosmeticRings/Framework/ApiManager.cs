/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CosmeticRings
**
*************************************************/

using StardewModdingAPI;
using CosmeticRings.Framework.Interfaces;

namespace CosmeticRings.Framework
{
    internal static class ApiManager
    {
        private static IMonitor monitor = CosmeticRings.monitor;
        private static IJsonAssetsApi jsonAssetsApi;
        private static IWearMoreRingsApi wearMoreRingsApi;
        private static IGenericModConfigMenuAPI genericModConfigMenuApi;

        internal static bool HookIntoJsonAssets(IModHelper helper)
        {
            jsonAssetsApi = helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");

            if (jsonAssetsApi is null)
            {
                monitor.Log("Failed to hook into spacechase0.JsonAssets.", LogLevel.Error);
                return false;
            }

            monitor.Log("Successfully hooked into spacechase0.JsonAssets.", LogLevel.Debug);
            return true;
        }

        internal static bool HookIntoIWMR(IModHelper helper)
        {
            wearMoreRingsApi = helper.ModRegistry.GetApi<IWearMoreRingsApi>("bcmpinc.WearMoreRings");

            if (wearMoreRingsApi is null)
            {
                monitor.Log("Failed to hook into bcmpinc.WearMoreRings.", LogLevel.Error);
                return false;
            }

            monitor.Log("Successfully hooked into bcmpinc.WearMoreRings.", LogLevel.Debug);
            return true;
        }

        public static bool HookIntoGMCM(IModHelper helper)
        {
            genericModConfigMenuApi = helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

            if (genericModConfigMenuApi is null)
            {
                monitor.Log("Failed to hook into spacechase0.GenericModConfigMenu.", LogLevel.Error);
                return false;
            }

            monitor.Log("Successfully hooked into spacechase0.GenericModConfigMenu.", LogLevel.Debug);
            return true;
        }

        internal static IJsonAssetsApi GetJsonAssetsApi()
        {
            return jsonAssetsApi;
        }

        internal static IWearMoreRingsApi GetIWMRApi()
        {
            return wearMoreRingsApi;
        }

        public static IGenericModConfigMenuAPI GetGMCMInterface()
        {
            return genericModConfigMenuApi;
        }
    }
}
