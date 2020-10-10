/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using System.Linq;
using StardewValley;

namespace WarpPylons
{
    public partial class WarpPylonsEntry
    {
        private void OpenWarPylonsMenuCommand(string arg1, string[] arg2)
        {
            Game1.activeClickableMenu = new WarpPylonsMenu(Monitor,Utils.GetTestPylons(20).ToList());
        }
    }
}