/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Diagnostics;
using System.Runtime.CompilerServices;
using AtraBase.Toolkit;

namespace AtraShared.Utils.Extensions;

/// <summary>
/// Extension methods for SMAPI's logging service.
/// </summary>
public static class LogExtensions
{
    /// <summary>
    /// Logs to level (DEBUG by default) if compiled with the DEBUG flag
    /// Logs to verbose otherwise.
    /// </summary>
    /// <param name="monitor">SMAPI's logger.</param>
    /// <param name="message">Message to log.</param>
    /// <param name="level">Level to log at.</param>
    [DebuggerHidden]
    [MethodImpl(TKConstants.Hot)]
    public static void DebugLog(this IMonitor monitor, string message, LogLevel level = LogLevel.Debug) =>
#if DEBUG
        monitor.Log(message, level);
#else
        monitor.VerboseLog(message);
#endif

    /// <summary>
    /// Logs to level (DEBUG by default) if compiled with the DEBUG flag only.
    /// </summary>
    /// <param name="monitor">SMAPI's logger.</param>
    /// <param name="message">Message to log.</param>
    /// <param name="level">Level to log at.</param>
    [DebuggerHidden]
    [Conditional("DEBUG")]
    [MethodImpl(TKConstants.Hot)]
    public static void DebugOnlyLog(this IMonitor monitor, string message, LogLevel level = LogLevel.Debug)
        => monitor.Log(message, level);

    /// <summary>
    /// Logs to level (DEBUG by default) if compiled with the DEBUG flag only.
    /// </summary>
    /// <param name="monitor">SMAPI's logger.</param>
    /// <param name="message">Message to log.</param>
    /// <param name="pred">Whether to log or not.</param>
    /// <param name="level">Level to log at.</param>
    /// <remarks>This exists because the entire function call is remvoed when compiled not debug
    /// including the predicate code.</remarks>
    [DebuggerHidden]
    [Conditional("DEBUG")]
    [MethodImpl(TKConstants.Hot)]
    public static void DebugOnlyLog(this IMonitor monitor, string message, bool pred, LogLevel level = LogLevel.Debug)
    {
        if (pred)
        {
            monitor.Log(message, level);
        }
    }

    /// <summary>
    /// Logs to level (TRACE by default) only if shouldLog is true.
    /// </summary>
    /// <param name="monitor">SMAPI's logger.</param>
    /// <param name="message">Message to log.</param>
    /// <param name="shouldLog">Whether the logging statement should be enabled or not.</param>
    /// <param name="level">Level to log at.</param>
    /// <remarks>This is meant to prevent the creation of a bunch of strings if they're just going to be ignored anyways.
    /// Must weigh the delegate against string creation, use sparingly.</remarks>
    [DebuggerHidden]
    [MethodImpl(TKConstants.Hot)]
    public static void LogOnlyIf(this IMonitor monitor, Func<string> message, bool shouldLog, LogLevel level = LogLevel.Trace)
    {
        if (shouldLog)
        {
            monitor.Log(message(), level);
        }
    }

    /// <summary>
    /// Logs only if verbose is enabled.
    /// </summary>
    /// <param name="monitor">SMAPI's logger.</param>
    /// <param name="message">Message to log.</param>
    /// <param name="level">Level to log at.</param>
    /// <remarks>This is meant to prevent the creation of a bunch of strings if they're just going to be ignored anyways.
    /// Must weigh the delegate against string creation, use sparingly.</remarks>
    [DebuggerHidden]
    [MethodImpl(TKConstants.Hot)]
    public static void LogIfVerbose(this IMonitor monitor, Func<string> message, LogLevel level = LogLevel.Trace)
    {
        if (monitor.IsVerbose)
        {
            monitor.Log(message(), level);
        }
    }
}