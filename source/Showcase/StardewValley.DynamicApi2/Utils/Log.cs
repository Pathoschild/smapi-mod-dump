/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/Stardew_Valley_Showcase_Mod
**
*************************************************/

using StardewModdingAPI;

namespace Igorious.StardewValley.DynamicApi2.Utils
{
    public sealed class Log
    {
        private static IMonitor Monitor { get; } = Smapi.GetMonitor("DynamicAPI2");

        public static void Debug(string message) => Monitor.Log(message);

        public static void Trace(string message) => Monitor.Log(message, LogLevel.Trace);

        public static void Error(string message) => Monitor.Log(message, LogLevel.Error);

        public static void Warning(string message) => Monitor.Log(message, LogLevel.Warn);
    }
}