/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/AnimalProduceExpansion
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using StardewModdingAPI;
// ReSharper disable UnusedMember.Global

namespace AnimalProduceExpansion
{
  internal abstract class Utility
  {
    private static IMonitor _monitor;

    protected IDictionary<string, object> RegisteredApis;

    internal IDictionary<string, object> GetRegisteredApis => RegisteredApis;

    internal static ConfigManager.ConfigManager Config { get; private set; }

    private static void Log(string methodName, string message, LogLevel level) =>
      _monitor.Log($"[{methodName}] {message}", level);

    internal static void LogInfo(string message, [CallerMemberName] string methodName = "") =>
      Log(methodName, message, LogLevel.Info);

    internal static void LogAlert(string message, [CallerMemberName] string methodName = "") =>
      Log(methodName, message, LogLevel.Alert);

    internal static void LogDebug(string message, [CallerMemberName] string methodName = "") =>
      Log(methodName, message, LogLevel.Debug);

    internal static void LogTrace(string message, [CallerMemberName] string methodName = "") =>
      Log(methodName, message, LogLevel.Trace);

    internal static void LogWarn(string message, [CallerMemberName] string methodName = "") =>
      Log(methodName, message, LogLevel.Warn);

    internal static void LogError(string message, Exception ex, [CallerMemberName] string methodName = "")
    {
      LogInfo($"[{methodName}] Exception has occurred: {ex.Message}, see log for more");
      _monitor.Log(ex.StackTrace);
    }

    internal static void SetLogger(IMonitor monitor) =>
      _monitor = monitor;

    internal static void SetConfigManager(ConfigManager.ConfigManager manager) =>
      Config = manager;
  }
}