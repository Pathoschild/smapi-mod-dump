/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/RuiNtD/SVRichPresence
**
*************************************************/

using System;
using DiscordRPC.Logging;
using StardewModdingAPI;
using RPCLogLevel = DiscordRPC.Logging.LogLevel;
using SDVLogLevel = StardewModdingAPI.LogLevel;

namespace SVRichPresence
{
  public class MonitorLogger : ILogger
  {
    public RPCLogLevel Level { get; set; }
    private readonly IMonitor Monitor;

    public MonitorLogger(IMonitor monitor)
    {
      Monitor = monitor;
    }

    public void Trace(string message, params object[] args) =>
      Log(message, args, SDVLogLevel.Trace);

    public void Info(string message, params object[] args) => Log(message, args, SDVLogLevel.Info);

    public void Warning(string message, params object[] args) =>
      Log(message, args, SDVLogLevel.Warn);

    public void Error(string message, params object[] args) =>
      Log(message, args, SDVLogLevel.Error);

    private void Log(string message, object[] args, SDVLogLevel level)
    {
      if (Monitor.IsVerbose)
        Monitor.Log("DISCORD: " + String.Format(message, args), level);
    }
  }
}
