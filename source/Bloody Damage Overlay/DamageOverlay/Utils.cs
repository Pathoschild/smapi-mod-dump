/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/p/sdvmod-damage-overlay/
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;

namespace DamageOverlay
    {
    internal static class Log
        {
        internal static IMonitor Monitor;
        internal static void Error(string msg) => Monitor.Log(msg, LogLevel.Error);
        internal static void Warn(string msg) => Monitor.Log(msg, LogLevel.Warn);
        internal static void Info(string msg) => Monitor.Log(msg, LogLevel.Info);
        internal static void Debug(string msg) => Monitor.Log(msg, LogLevel.Debug);
        internal static void Trace(string msg) => Monitor.Log(msg, LogLevel.Trace);
        }
    }
