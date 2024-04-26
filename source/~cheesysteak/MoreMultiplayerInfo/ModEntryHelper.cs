/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cheesysteak/stardew-steak
**
*************************************************/

using MoreMultiplayerInfo.EventHandlers;
using StardewModdingAPI;

namespace MoreMultiplayerInfo
{
    public class ModEntryHelper
    {
        public ModEntryHelper(IMonitor monitor, IModHelper modHelper)
        {
            var showIcon = new ShowPlayerIconHandler(monitor, modHelper);

            var playerWatcher = new PlayerStateWatcher(modHelper);

            // var logHandler = new LogInputHandler(monitor, modHelper);
        }



    }
}