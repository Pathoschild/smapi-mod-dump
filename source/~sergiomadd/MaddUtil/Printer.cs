/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using System.Threading;

namespace MaddUtil
{
    class Printer
    {
        private static IMonitor _monitor;

        public static void SetMonitor(IMonitor monitor)
        {
            _monitor = monitor;
        }

        public static void Info(String message)
        {
            try
            {
                _monitor.Log(message, LogLevel.Info);
            }
            catch (ThreadInterruptedException exception)
            {
            }
        }
        public static void Error(String message)
        {
            _monitor.Log(message, LogLevel.Error);
        }
        public static void Alert(String message)
        {
            _monitor.Log(message, LogLevel.Alert);
        }
        public static void Warn(String message)
        {
            _monitor.Log(message, LogLevel.Warn);
        }

        public static void Debug(String message)
        {
            _monitor.Log(message, LogLevel.Debug);
        }

        public static void Trace(String message)
        {
            _monitor.Log(message, LogLevel.Trace);
        }
    }
}
