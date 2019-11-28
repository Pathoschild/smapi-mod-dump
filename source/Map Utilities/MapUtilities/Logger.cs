using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace MapUtilities
{
    public static class Logger
    {
        public static IMonitor monitor;

        public static void log(string log, LogLevel level = LogLevel.Info)
        {
            if (monitor == null)
                return;
            monitor.Log(log, level);
        }
    }
}
