/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sarahvloos/StardewMods
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AlwaysShowBarValues
{
    internal static class Logger
    {
        /// <summary>
        /// The mod's monitor
        /// </summary>
        private static IMonitor? Monitor {  get; set; }
        /// <summary>
        /// Whether this mod is currently being debugged by its author
        /// </summary>
        private static bool DebugMode { get; set; } = false;

        /// <summary>
        /// Adds the mod's monitor to this class
        /// </summary>
        /// <param name="monitor">This mod's monitor</param>
        internal static void Initialize(IMonitor monitor, bool debugMode) { 
            Monitor = monitor;
            DebugMode = debugMode;
        }

        /// <summary>
        /// Sends a message that will only appear in the author's own console or in the console of someone very curious who is reading this right now.
        /// </summary>
        /// <param name="message">The message to be sent</param>
        internal static void DevLog(string message)
        {
            if (!DebugMode) return;
            if (message == null || message == string.Empty) 
                Error("[DEVLOG] No message received");
            else Warn(message);
        }

        /// <summary>
        /// Sends a message that will only appear in the log of an user who enabled verbose logging or in the console of an user who enabled developer mode.
        /// </summary>
        /// <param name="message">The message to be sent</param>
        internal static void Verbose(string message)
        {
            if (message == null || message == string.Empty) return;
            Monitor?.VerboseLog(message);
        }

        /// <summary>
        /// Sends a message that will appear in a developer's console and an user's log.
        /// </summary>
        /// <param name="message">The message to be sent</param>
        /// <param name="logOnce">Whether the message should only be sent once per game launch</param>
        internal static void Trace(string message, bool logOnce = false)
        {
            SendLog(LogLevel.Trace, message, logOnce);
        }

        /// <summary>
        /// Sends a message that will appear in the end user's console, in dark gray
        /// </summary>
        /// <param name="message">The message to be sent</param>
        /// <param name="logOnce">Whether the message should only be sent once per game launch</param>
        internal static void Debug(string message, bool logOnce = false)
        {
            SendLog(LogLevel.Debug, message, logOnce);
        }

        /// <summary>
        /// Sends a message that will appear in the end user's console, in white. Information relevant to the player.
        /// </summary>
        /// <param name="message">The message to be sent</param>
        /// <param name="logOnce">Whether the message should only be sent once per game launch</param>
        internal static void Info(string message, bool logOnce = false)
        {
            SendLog(LogLevel.Info, message, logOnce);
        }

        /// <summary>
        /// Sends a message that will appear in the end user's console, in orange. A potential problem the user should be aware of.
        /// </summary>
        /// <param name="message">The message to be sent</param>
        /// <param name="logOnce">Whether the message should only be sent once per game launch</param>
        internal static void Warn(string message, bool logOnce = false)
        {
            SendLog(LogLevel.Warn, message, logOnce);
        }

        /// <summary>
        /// Sends a message that will appear in the end user's console, in red. A critical error.
        /// </summary>
        /// <param name="message">The message to be sent</param>
        /// <param name="logOnce">Whether the message should only be sent once per game launch</param>
        internal static void Error(string message, bool logOnce = false)
        {
            SendLog(LogLevel.Error, message, logOnce);
        }

        private static void SendLog(LogLevel logLevel, string message, bool LogOnce) {
            if (message == null || message == string.Empty) return;
            if (LogOnce) Monitor?.LogOnce(message, logLevel);
            else Monitor?.Log(message, logLevel);
        }


    }
}
