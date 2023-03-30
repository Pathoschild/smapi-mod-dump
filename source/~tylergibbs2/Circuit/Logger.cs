/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace Circuit
{
    internal static class Logger
    {
        public static void Log(string message, LogLevel logLevel)
        {
            ModEntry.Instance.Monitor.Log(message, logLevel);
        }
    }
}
