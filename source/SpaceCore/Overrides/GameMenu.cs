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

namespace SpaceCore.Overrides
{
    public class GameMenuTabNameHook
    {
        public static void Postfix(GameMenuTabNameHook __instance, string name, ref int __result)
        {
            foreach ( var tab in Menus.extraGameMenuTabs )
            {
                if (name == tab.Value)
                    __result = tab.Key;
            }
        }
    }
}
