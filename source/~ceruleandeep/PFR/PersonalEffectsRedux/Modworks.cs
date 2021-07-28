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
using System.Collections.Generic;

namespace PersonalEffects
{
    public class Log
    {
        private StardewModdingAPI.IMonitor Monitor;
        internal Log(StardewModdingAPI.IMonitor monitor)
        {
            Monitor = monitor;
        }

        // Tracing info intended for developers, usually troubleshooting details that are useful 
        // when someone sends you their error log.Trace messages won't appear in the console window 
        // by default (unless you have the "SMAPI for developers" version), though they're always 
        // written to the log file.
        public void Trace(string message) { Monitor.Log(message, StardewModdingAPI.LogLevel.Trace); }

        // Troubleshooting info that may be relevant to the player.
        public void Debug(string message) { Monitor.Log(message, StardewModdingAPI.LogLevel.Debug); }

        // Info relevant to the player. This should be used judiciously. 
        public void Info(string message) { Monitor.Log(message, StardewModdingAPI.LogLevel.Info); }

        // An issue the player should be aware of. This should be used rarely. 
        public void Warn(string message) { Monitor.Log(message, StardewModdingAPI.LogLevel.Warn); }

        // A message indicating something went wrong.
        public void Error(string message) { Monitor.Log(message, StardewModdingAPI.LogLevel.Error); }

        // Important information to highlight for the player when player action is needed 
        // (e.g. new version available). This should be used rarely to avoid alert fatigue. 
        public void Alert(string message) { Monitor.Log(message, StardewModdingAPI.LogLevel.Alert); }
    }

    public static class Modworks
    {
        internal static IModHelper Helper;

        internal static Random _RNG;
        public static Random RNG
        {
            get { return _RNG; }
        }
        internal static Log _Log;
        public static Log Log
        {
            get { return _Log; }
        }

        internal static Player _Player;
        public static Player Player
        {
            get { return _Player; }
        }

        internal static NPCs _NPCs;
        public static NPCs NPCs
        {
            get { return _NPCs; }
        }

        internal static Events _Events;
        public static Events Events
        {
            get { return _Events; }
        }
        internal static void Startup(Mod mod)
        {
            _Log = new Log(mod.Monitor);
            _RNG = new Random(DateTime.Now.Millisecond * DateTime.Now.GetHashCode());
            _Player = new Player();
            _NPCs = new NPCs();
            _Events = new Events();

            Helper = mod.Helper;
        }
    }
}
