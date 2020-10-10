/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tstaples/StackSplitX
**
*************************************************/

using StardewModdingAPI;

namespace StackSplitX
{
    public static class LogExtensions
    {
        public static void DebugLog(this IMonitor monitor, string message, LogLevel level = LogLevel.Trace)
        {
#if DEBUG
            monitor.Log(message, level);
#endif
        }
    }
}
