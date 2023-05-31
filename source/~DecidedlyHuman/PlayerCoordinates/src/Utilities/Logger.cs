/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using StardewModdingAPI;

namespace PlayerCoordinates.Utilities
{
    public static class Logger
    {
        public static void LogMessage(IMonitor m, LogLevel level,
            string message = "No message specified. Bad developer.")
        {
            m.Log(message, level);
        }

        public static void LogException(IMonitor m, Exception e)
        {
            m.Log($"Exception: {e.Message}.", LogLevel.Error);
            m.Log($"{e.Data}.", LogLevel.Error);
        }
    }
}
