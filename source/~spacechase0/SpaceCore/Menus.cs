/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace SpaceCore
{
    public class Menus
    {
        private static int CurrGameMenuTab = 8;
        internal static Dictionary<int, string> ExtraGameMenuTabs = new();

        public static int ReserveGameMenuTab(string name)
        {
            int tab = Menus.CurrGameMenuTab++;
            Menus.ExtraGameMenuTabs.Add(tab, name);
            return tab;
        }
    }
}
