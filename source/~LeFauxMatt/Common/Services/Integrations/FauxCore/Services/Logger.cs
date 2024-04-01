/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Services.Integrations.FauxCore;

/// <inheritdoc />
internal sealed class Logger(FauxCoreIntegration fauxCoreIntegration, IMonitor monitor) : ILog
{
    private readonly ILog log = fauxCoreIntegration.Api!.CreateLogService(monitor);

    /// <inheritdoc />
    public void Trace(string message, object?[]? args = null) => this.log.Trace(message, args);

    /// <inheritdoc />
    public void TraceOnce(string message, params object?[]? args) => this.log.TraceOnce(message, args);

    /// <inheritdoc />
    public void Debug(string message, object?[]? args = null) => this.log.Debug(message, args);

    /// <inheritdoc />
    public void Info(string message, object?[]? args = null) => this.log.Info(message, args);

    /// <inheritdoc />
    public void Warn(string message, object?[]? args = null) => this.log.Warn(message, args);

    /// <inheritdoc />
    public void WarnOnce(string message, object?[]? args = null) => this.log.WarnOnce(message, args);

    /// <inheritdoc />
    public void Error(string message, object?[]? args = null) => this.log.Error(message, args);

    /// <inheritdoc />
    public void Alert(string message, object?[]? args = null) => this.log.Alert(message, args);
}