/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AlejandroAkbal/Stardew-Valley-Quick-Sell-Mod
**
*************************************************/

using StardewModdingAPI;

namespace Quick_Sell
{
    internal class ModLogger
    {
        private readonly IMonitor Monitor;

        public ModLogger(IMonitor monitor)
        {
            this.Monitor = monitor;
        }

        public void Debug(string message)
        {
            this.Monitor.Log(message, LogLevel.Debug);
        }

        public void Warn(string message)
        {
            this.Monitor.Log(message, LogLevel.Warn);
        }
    }
}