/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using System;
using StardewModdingAPI;

namespace DialogueExtension.Tests.MockWrappers
{
  public class MockLogger : IMonitor
  {
    public void Log(string message, LogLevel level = LogLevel.Trace)
    {
      Console.WriteLine($"{level} | {message}");
    }

    public void LogOnce(string message, LogLevel level = LogLevel.Trace)
    {
      Log(message,level);
    }

    public void VerboseLog(string message)
    {
      Log(message);
    }

    public bool IsVerbose { get; }
  }
}
