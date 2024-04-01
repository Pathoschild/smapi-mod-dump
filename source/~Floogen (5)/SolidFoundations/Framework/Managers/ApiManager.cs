/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using SolidFoundations.Framework.Interfaces;
using StardewModdingAPI;

namespace SolidFoundations.Framework.Managers
{
    internal class ApiManager
    {
        private IMonitor _monitor;
        private ISTFApi _shopTileFrameworkApi;
        private ISaveAnywhereApi _saveAnywhereApi;
        private IContentPatcherApi _contentPatcherApi;
        private IJsonAssetsApi _jsonAssetsApi;

        public ApiManager(IMonitor monitor)
        {
            _monitor = monitor;
        }

        internal bool HookIntoContentPatcher(IModHelper helper)
        {
            _contentPatcherApi = helper.ModRegistry.GetApi<IContentPatcherApi>("Pathoschild.ContentPatcher");

            if (_contentPatcherApi is null)
            {
                _monitor.Log("Failed to hook into Pathoschild.ContentPatcher.", LogLevel.Error);
                return false;
            }

            _monitor.Log("Successfully hooked into Pathoschild.ContentPatcher.", LogLevel.Debug);
            return true;
        }

        public IContentPatcherApi GetContentPatcherApi()
        {
            return _contentPatcherApi;
        }

        internal bool HookIntoShopTileFramework(IModHelper helper)
        {
            _shopTileFrameworkApi = helper.ModRegistry.GetApi<ISTFApi>("Cherry.ShopTileFramework");

            if (_shopTileFrameworkApi is null)
            {
                _monitor.Log("Failed to hook into Cherry.ShopTileFramework.", LogLevel.Error);
                return false;
            }

            _monitor.Log("Successfully hooked into Cherry.ShopTileFramework.", LogLevel.Debug);
            return true;
        }

        public ISTFApi GetShopTileFrameworkApi()
        {
            return _shopTileFrameworkApi;
        }

        public bool HookIntoSaveAnywhere(IModHelper helper)
        {
            _saveAnywhereApi = helper.ModRegistry.GetApi<ISaveAnywhereApi>("Omegasis.SaveAnywhere");

            if (_saveAnywhereApi is null)
            {
                _monitor.Log("Failed to hook into Omegasis.SaveAnywhere.", LogLevel.Error);
                return false;
            }

            _monitor.Log("Successfully hooked into Omegasis.SaveAnywhere.", LogLevel.Debug);
            return true;
        }

        public ISaveAnywhereApi GetSaveAnywhereApi()
        {
            return _saveAnywhereApi;
        }
    }
}
