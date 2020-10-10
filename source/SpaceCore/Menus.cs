/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/SpaceCore_SDV
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceCore
{
    public class Menus
    {
        private static int currGameMenuTab = 8;
        internal static Dictionary<int, string> extraGameMenuTabs = new Dictionary<int, string>();
        public static int ReserveGameMenuTab(string name)
        {
            int tab = currGameMenuTab++;
            extraGameMenuTabs.Add(tab, name);
            return tab;
        }
    }
}
