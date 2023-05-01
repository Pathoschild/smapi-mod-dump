/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared;

#region using directives

using System.Diagnostics;

#endregion using directives

/// <summary>Simplified wrapper for SMAPI's <see cref="IMonitor"/>.</summary>
public static class Log
{
    /// <inheritdoc cref="IMonitor"/>
    private static IMonitor _monitor = null!;

    /// <summary>Initializes the static instance.</summary>
    /// <param name="monitor">Encapsulates monitoring and logging for a given module.</param>
    public static void Init(IMonitor monitor)
    {
        _monitor = monitor;
    }

    /// <summary>Logs a message as debug.</summary>
    /// <param name="message">The message.</param>
    [Conditional("DEBUG")]
    public static void D(string message)
    {
        _monitor.Log(message, LogLevel.Debug);
    }

    /// <summary>Logs a message as trace.</summary>
    /// <param name="message">The message.</param>
    public static void T(string message)
    {
#if DEBUG
        D(message);
#else
        _monitor.Log(message);
#endif
    }

    /// <summary>Logs a message as info.</summary>
    /// <param name="message">The message.</param>
    public static void I(string message)
    {
        _monitor.Log(message, LogLevel.Info);
    }

    /// <summary>Logs a message as alert.</summary>
    /// <param name="message">The message.</param>
    public static void A(string message)
    {
        _monitor.Log(message, LogLevel.Alert);
    }

    /// <summary>Logs a message as warn.</summary>
    /// <param name="message">The message.</param>
    public static void W(string message)
    {
        _monitor.Log(message, LogLevel.Warn);
    }

    /// <summary>Logs a message as error.</summary>
    /// <param name="message">The message.</param>
    public static void E(string message)
    {
        _monitor.Log(message, LogLevel.Error);
    }

    /// <inheritdoc cref="IMonitor.VerboseLog"/>
    public static void V(string message)
    {
        _monitor.VerboseLog(message);
    }

    /// <summary>Logs the caller method as Debug.</summary>
    [Conditional("DEBUG")]
    public static void Caller()
    {
        var caller = new StackTrace().GetFrame(2)?.GetMethod()?.Name ?? string.Empty;
        D($"Called by {caller}.");
    }
}
