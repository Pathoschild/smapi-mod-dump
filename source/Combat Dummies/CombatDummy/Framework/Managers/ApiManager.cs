/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CombatDummy
**
*************************************************/

using CombatDummy.Framework.Interfaces;
using StardewModdingAPI;

namespace CombatDummy.Framework.Managers
{
    internal class ApiManager
    {
        private IMonitor _monitor;
        private IDynamicGameAssetsApi _dynamicGameAssetsApi;

        public ApiManager(IMonitor monitor)
        {
            _monitor = monitor;
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

        public IDynamicGameAssetsApi GetDynamicGameAssetsApi()
        {
            return _dynamicGameAssetsApi;
        }
    }
}