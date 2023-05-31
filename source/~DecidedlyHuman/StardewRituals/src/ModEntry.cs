/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewRituals.Utilities;

namespace StardewRituals
{
    public class ModEntry : Mod
    {
        private IModHelper _helper;
        private Logger _logger;
        private IMonitor _monitor;

        public override void Entry(IModHelper helper)
        {
            this._monitor = this.Monitor;
            this._helper = helper;
            this._logger = new Logger(this._monitor);

            // WHY THE FUCK DOES DGA SAY THERE ARE DUPLICATE ITEMS?
        }
    }
}
