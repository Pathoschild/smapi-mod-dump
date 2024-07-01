/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ProfeJavix/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace UIHelper
{
    internal class Console
    {
        internal static void Log(string message, LogLevel level = LogLevel.Warn){
            ModEntry.context.Monitor.Log(message, level);
        }
    }
}
