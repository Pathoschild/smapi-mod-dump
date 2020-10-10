/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using StardewModdingAPI;

namespace ExpandedPreconditionsUtility
{
    public class ModEntry : Mod
    {
        private IModHelper _helper;
        private IMonitor _monitor;
        public override void Entry(IModHelper helper)
        {
            _helper = Helper;
            _monitor = Monitor;
        }

        public override object GetApi()
        {
            return new ConditionsChecker(_monitor,_helper);
        }
    }
}
