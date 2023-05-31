/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using StardewModdingAPI;
using StarterPack.Framework.Interfaces;

namespace StarterPack.Framework.Managers
{
    internal class ApiManager
    {
        private IMonitor _monitor;
        private IArcheryApi _archeryApi;

        public ApiManager(IMonitor monitor)
        {
            _monitor = monitor;
        }

        internal bool HookIntoArchery(IModHelper helper)
        {
            _archeryApi = helper.ModRegistry.GetApi<IArcheryApi>("PeacefulEnd.Archery");

            if (_archeryApi is null)
            {
                _monitor.Log("Failed to hook into PeacefulEnd.Archery.", LogLevel.Error);
                return false;
            }

            _monitor.Log("Successfully hooked into PeacefulEnd.Archery.", LogLevel.Debug);
            return true;
        }

        public IArcheryApi GetArcheryApi()
        {
            return _archeryApi;
        }
    }
}