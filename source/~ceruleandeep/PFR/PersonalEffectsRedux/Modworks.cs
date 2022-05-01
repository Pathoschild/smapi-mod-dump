/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System;
using StardewModdingAPI;

namespace PersonalEffects
{
    public class Log
    {
        private readonly IMonitor Monitor;
        internal Log(IMonitor monitor)
        {
            Monitor = monitor;
        }

        // Tracing info intended for developers, usually troubleshooting details that are useful 
        // when someone sends you their error log.Trace messages won't appear in the console window 
        // by default (unless you have the "SMAPI for developers" version), though they're always 
        // written to the log file.
        public void Trace(string message) { Monitor.Log(message); }

        public void Debug(string message) { Monitor.Log(message, LogLevel.Debug); }

         // An issue the player should be aware of. This should be used rarely. 
        public void Warn(string message) { Monitor.Log(message, LogLevel.Warn); }

        // A message indicating something went wrong.
        public void Error(string message) { Monitor.Log(message, LogLevel.Error); }

    }

    public static class Modworks
    {
        public static Random RNG { get; private set; }

        public static Log Log { get; private set; }

        internal static void Startup(Mod mod)
        {
            Log = new Log(mod.Monitor);
            RNG = new Random(DateTime.Now.Millisecond * DateTime.Now.GetHashCode());
        }
    }
}
