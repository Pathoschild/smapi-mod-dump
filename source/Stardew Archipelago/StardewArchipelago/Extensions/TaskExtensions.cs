/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace StardewArchipelago.Extensions
{
    public static class TaskExtensions
    {
        private static IMonitor _log;

        public static void Initialize(IMonitor log)
        {
            _log = log;
        }

        public static async void FireAndForget(this Task task)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                _log.Log($"Exception occurred in FireAndForget task: {ex}", LogLevel.Error);
            }
        }
    }
}
