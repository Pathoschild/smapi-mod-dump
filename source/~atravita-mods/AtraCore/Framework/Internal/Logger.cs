/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Interfaces;

namespace AtraCore.Framework.Internal;

/// <summary>
/// Wraps AtraBase's logger to use SMAPI's logging service.
/// </summary>
internal class Logger : ILogger
{
    private readonly IMonitor monitor;

    /// <summary>
    /// Initializes a new instance of the <see cref="Logger"/> class.
    /// </summary>
    /// <param name="monitor">Instance of SMAPI's monitor to use.</param>
    internal Logger(IMonitor monitor)
        => this.monitor = monitor;

    /// <inheritdoc />
    public void Error(string message)
        => this.monitor.Log(message, LogLevel.Error);

    /// <inheritdoc />
    public void Info(string message)
        => this.monitor.Log(message, LogLevel.Info);

    /// <inheritdoc />
    public void Verbose(string message)
        => this.monitor.VerboseLog(message);

    /// <inheritdoc />
    public void Warn(string message)
        => this.monitor.Log(message, LogLevel.Warn);
}
