/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/SAML
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAML.Utilities
{
    internal static class Logger
    {
        private static IMonitor Monitor => ModEntry.IMonitor;

        public static void Verbose(string message) => Monitor.VerboseLog(message);

        public static void Trace(string message) => Monitor.Log(message, LogLevel.Trace);

        public static void Debug(string message) => Monitor.Log(message, LogLevel.Debug);

        public static void Info(string message) => Monitor.Log(message, LogLevel.Info);

        public static void Warn(string message) => Monitor.Log(message, LogLevel.Warn);

        public static void Error(string message) => Monitor.Log(message, LogLevel.Error);

        public static void Alert(string message) => Monitor.Log(message, LogLevel.Alert);
    }
}
