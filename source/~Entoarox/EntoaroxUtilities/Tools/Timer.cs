/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;

namespace Entoarox.Utilities.Tools
{
    public static class Timer
    {
        private static readonly Dictionary<IMonitor, Dictionary<string, DateTime>> Timers = new Dictionary<IMonitor, Dictionary<string, DateTime>>();

        public static void StartTimer(this IMonitor monitor, string id, string message = null)
        {
            if (!Timers.ContainsKey(monitor))
                Timers.Add(monitor, new Dictionary<string, DateTime>());
            Timers[monitor][id] = DateTime.Now;
            if (message != null)
                monitor.Log(message);
        }
        public static TimeSpan StopTimer(this IMonitor monitor, string id, string message = null)
        {
            TimeSpan time = TimeSpan.Zero;
            if (Timers.ContainsKey(monitor) && Timers[monitor].ContainsKey(id))
                time = DateTime.Now.Subtract(Timers[monitor][id]);
            if (message != null)
                monitor.Log(message.Replace("{{TIME}}", time.TotalMilliseconds.ToString()));
            return time;
        }
    }
}
