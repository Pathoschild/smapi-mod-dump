/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CustomCompanions
**
*************************************************/

using StardewModdingAPI;
using CustomCompanions.Framework.Interfaces;

namespace CustomCompanions.Framework.Managers
{
    internal static class ApiManager
    {
        private static IMonitor monitor = CustomCompanions.monitor;
        private static ISaveAnywhereApi saveAnywhereApi;
        private static IContentPatcherAPI contentPatcherApi;
        private static IJsonAssetsApi jsonAssetsApi;
        private static IWearMoreRingsApi wearMoreRingsApi;

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

        internal static bool HookIntoContentPatcher(IModHelper helper)
        {
            contentPatcherApi = helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");

            if (contentPatcherApi is null)
            {
                monitor.Log("Failed to hook into Pathoschild.ContentPatcher.", LogLevel.Error);
                return false;
            }

            monitor.Log("Successfully hooked into Pathoschild.ContentPatcher.", LogLevel.Debug);
            return true;
        }

        internal static bool HookIntoSaveAnywhere(IModHelper helper)
        {
            saveAnywhereApi = helper.ModRegistry.GetApi<ISaveAnywhereApi>("Omegasis.SaveAnywhere");

            if (saveAnywhereApi is null)
            {
                monitor.Log("Failed to hook into Omegasis.SaveAnywhere.", LogLevel.Error);
                return false;
            }

            monitor.Log("Successfully hooked into Omegasis.SaveAnywhere.", LogLevel.Debug);
            return true;
        }

        public static IContentPatcherAPI GetContentPatcherInterface()
        {
            return contentPatcherApi;
        }

        internal static IJsonAssetsApi GetJsonAssetsApi()
        {
            return jsonAssetsApi;
        }

        internal static IWearMoreRingsApi GetIWMRApi()
        {
            return wearMoreRingsApi;
        }

        internal static ISaveAnywhereApi GetSaveAnywhereApi()
        {
            return saveAnywhereApi;
        }
    }
}
