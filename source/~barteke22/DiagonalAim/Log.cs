/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/barteke22/StardewMods
**
*************************************************/

using StardewModdingAPI;

namespace DiagonalAim
{
    class Log
    {
        public static IMonitor Monitor;

        public static void Error(params object[] text)
        {
            string s = "";
            if (text.Length == 1) s += text[0];
            else
            {
                foreach (var item in text)
                {
                    s += ", " + item;
                }
            }
            Monitor.Log(s, LogLevel.Error);
        }
    }
}
