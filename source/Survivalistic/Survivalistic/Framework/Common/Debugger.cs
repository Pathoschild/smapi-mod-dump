/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Survivalistic
**
*************************************************/

using StardewModdingAPI;

namespace Survivalistic.Framework.Common
{
    public class Debugger
    {
        private static IMonitor Monitor = ModEntry.instance.Monitor;

        public static void Log(string message, string type)
        {
            switch (type)
            {
                case "Trace":
                    Monitor.Log(message, LogLevel.Trace);
                    break;

                case "Info":
                    Monitor.Log(message, LogLevel.Info);
                    break;

                case "Error":
                    Monitor.Log(message, LogLevel.Error);
                    break;

                case "Warn":
                    Monitor.Log(message, LogLevel.Warn);
                    break;

                case "Alert":
                    Monitor.Log(message, LogLevel.Alert);
                    break;

                case "Debug":
                    Monitor.Log(message, LogLevel.Debug);
                    break;
            }
        }
    }
}
