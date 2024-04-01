/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using ExpandedPreconditionsUtility.Framework;
using StardewModdingAPI;

namespace ExpandedPreconditionsUtility
{
    public class ModEntry : Mod
    {
        private IModHelper _helper;
        private IMonitor _monitor;
        public override void Entry(IModHelper helper)
        {
            this._helper = this.Helper;
            this._monitor = this.Monitor;
        }

        public override object GetApi()
        {
            return new ConditionsChecker(this._monitor, this._helper);
        }
    }
}
