/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using AlternativeTextures.Framework.Interfaces;
using StardewModdingAPI;

namespace AlternativeTextures.Framework.Managers
{
    internal class ApiManager
    {
        private IMonitor _monitor;
        private IMoreGiantCropsApi _moreGiantCropsApi;
        private IDynamicGameAssetsApi _dynamicGameAssetsApi;
        private IContentPatcherApi _contentPatcherApi;
        private IGenericModConfigMenuApi _genericModConfigMenuApi;

        public ApiManager(IMonitor monitor)
        {
            _monitor = monitor;
        }

        internal bool HookIntoMoreGiantCrops(IModHelper helper)
        {
            _moreGiantCropsApi = helper.ModRegistry.GetApi<IMoreGiantCropsApi>("spacechase0.MoreGiantCrops");

            if (_moreGiantCropsApi is null)
            {
                _monitor.Log("Failed to hook into spacechase0.MoreGiantCrops.", LogLevel.Error);
                return false;
            }

            _monitor.Log("Successfully hooked into spacechase0.MoreGiantCrops.", LogLevel.Debug);
            return true;
        }

        internal bool HookIntoDynamicGameAssets(IModHelper helper)
        {
            _dynamicGameAssetsApi = helper.ModRegistry.GetApi<IDynamicGameAssetsApi>("spacechase0.DynamicGameAssets");

            if (_dynamicGameAssetsApi is null)
            {
                _monitor.Log("Failed to hook into spacechase0.DynamicGameAssets.", LogLevel.Error);
                return false;
            }

            _monitor.Log("Successfully hooked into spacechase0.DynamicGameAssets.", LogLevel.Debug);
            return true;
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

        internal bool HookIntoGenericModConfigMenu(IModHelper helper)
        {
            _genericModConfigMenuApi = helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (_genericModConfigMenuApi is null)
            {
                _monitor.Log("Failed to hook into spacechase0.GenericModConfigMenu.", LogLevel.Error);
                return false;
            }

            _monitor.Log("Successfully hooked into spacechase0.GenericModConfigMenu.", LogLevel.Debug);
            return true;
        }

        internal IMoreGiantCropsApi GetMoreGiantCropsApi()
        {
            return _moreGiantCropsApi;
        }

        internal IDynamicGameAssetsApi GetDynamicGameAssetsApi()
        {
            return _dynamicGameAssetsApi;
        }

        public IContentPatcherApi GetContentPatcherApi()
        {
            return _contentPatcherApi;
        }

        public IGenericModConfigMenuApi GetGenericModConfigMenuApi()
        {
            return _genericModConfigMenuApi;
        }
    }
}
