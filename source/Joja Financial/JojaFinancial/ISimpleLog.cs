/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/JojaFinancial
**
*************************************************/

using NermNermNerm.Stardew.LocalizeFromSource;
using System;
using StardewModdingAPI;

namespace StardewValleyMods.JojaFinancial
{
    /// <summary>
    ///   Objects within a mod should implement this interface to make it easier to report events to the
    ///   log file or whereever.  It only has one function in it, to make it easy to write classes that
    ///   utilize it.  It's also a good idea to make the implementation of this virtual so that it
    ///   can be integrated into tests to validate that errors & such are properly recorded.
    /// </summary>
    public interface ISimpleLog
    {
        /// <summary>
        ///   This is the method that should, directly or indirectly, call functions off of <see cref="IMonitor"/>
        ///   on the mod entry class.
        /// </summary>
        /// <remarks>
        ///   This method isn't intended to be directly called.  It should be called by the extensions in
        ///   <see cref="SimpleLogExtensions"/>.  Its implementations can be marked as virtual to facilitate
        ///   overriding them in testing.
        /// </remarks>
        void WriteToLog(string message, LogLevel level, bool isOnceOnly);
    }

    public static class SimpleLogExtensions
    {
        [ArgumentIsCultureInvariant]
        /// <summary>
        ///   Reports a failure that has been recovered from without throwing an exception, but still indicates a
        ///   problem with the mod severe enough to block its functionality.
        /// </summary>
        public static void LogError(this ISimpleLog _this, FormattableString message) => _this.WriteToLog(message.ToString(), LogLevel.Error, isOnceOnly: false);

        /// <summary>
        ///   Reports a failure that has been recovered from without throwing an exception, but still indicates a
        ///   problem with the mod severe enough to block its functionality.  Use this if the problem might happen so frequently
        ///   that it'd clog up the log.
        /// </summary>
        [ArgumentIsCultureInvariant]
        public static void LogErrorOnce(this ISimpleLog _this, FormattableString message) => _this.WriteToLog(message.ToString(), LogLevel.Error, isOnceOnly: true);

        /// <summary>
        ///   Info that might suggest there's a problem with the mod that the user might be able to work around.
        ///   Mainly it would be when the presence of this issue might explain a more serious fault later.
        /// </summary>
        [ArgumentIsCultureInvariant]
        public static void LogWarning(this ISimpleLog _this, FormattableString message) => _this.WriteToLog(message.ToString(), LogLevel.Warn, isOnceOnly: false);

        /// <summary>
        ///   Info that might suggest there's a problem with the mod that the user might be able to work around.
        ///   Mainly it would be when the presence of this issue might explain a more serious fault later.
        ///   Use this if the problem might happen so frequently that it'd clog up the log.
        /// </summary>
        [ArgumentIsCultureInvariant]
        public static void LogWarningOnce(this ISimpleLog _this, FormattableString message) => _this.WriteToLog(message.ToString(), LogLevel.Warn, isOnceOnly: true);

        /// <summary>
        ///   Output that might be useful to the user who's just stumped by something.  E.g. record the location of hidden objects.
        /// </summary>
        /// <remarks>
        ///   So if the user claims it's just not there at all, they can be told to refer to this part of the log and go see if it isn't there.
        /// </remarks>
        [ArgumentIsCultureInvariant]
        public static void LogInfo(this ISimpleLog _this, FormattableString message) => _this.WriteToLog(message.ToString(), LogLevel.Info, isOnceOnly: false);

        /// <summary>
        ///   Output that might be useful to the user who's just stumped by something.  E.g. record the location of hidden objects.
        ///   Use this if the codepath that calls this might be called often but the information would only be interesting once.
        /// </summary>
        [ArgumentIsCultureInvariant]
        public static void LogInfoOnce(this ISimpleLog _this, FormattableString message) => _this.WriteToLog(message.ToString(), LogLevel.Info, isOnceOnly: true);

        /// <summary>Output to aid in debugging.  Not visible in the regular log.</summary>
        [ArgumentIsCultureInvariant]
        public static void LogTrace(this ISimpleLog _this, FormattableString message) => _this.WriteToLog(message.ToString(), LogLevel.Trace, isOnceOnly: false);

        /// <summary>Output to aid in debugging.  Not visible in the regular log.</summary>
        [ArgumentIsCultureInvariant]
        public static void LogTraceOnce(this ISimpleLog _this, FormattableString message) => _this.WriteToLog(message.ToString(), LogLevel.Trace, isOnceOnly: true);
    }
}
