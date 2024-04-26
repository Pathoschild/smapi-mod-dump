/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using StardewModdingAPI;
using AnythingAnywhere.Framework.Interfaces;
using System;

namespace AnythingAnywhere.Framework.Managers
{
    internal class ApiManager
    {
        private IMonitor _monitor;
        private IGenericModConfigMenuApi _genericModConfigMenuApi;
        private ICustomBushApi _customBushApi;

        public ApiManager(IMonitor monitor)
        {
            _monitor = monitor;
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

        internal bool HookIntoCustomBush(IModHelper helper)
        {
            _customBushApi = helper.ModRegistry.GetApi<ICustomBushApi>("furyx639.CustomBush");

            if (_customBushApi is null)
            {
                _monitor.Log("Failed to hook into furyx639.CustomBush.", LogLevel.Error);
                return false;
            }

            _monitor.Log("Successfully hooked into furyx639.CustomBush.", LogLevel.Debug);
            return true;
        }

        public IGenericModConfigMenuApi GetGenericModConfigMenuApi()
        {
            return _genericModConfigMenuApi;
        }

        public ICustomBushApi GetCustomBushApi()
        {
            return _customBushApi;
        }
    }
}
