/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AlejandroAkbal/Stardew-Valley-OmniSwing-Mod
**
*************************************************/

using StardewModdingAPI;

namespace OmniSwing
{
    public static class ModLogger
    {
        private static readonly IMonitor Monitor = ModEntry.Instance.Monitor;

        public static void Trace(string message)
        {
            Monitor.Log(message, LogLevel.Trace);
        }
    }
}