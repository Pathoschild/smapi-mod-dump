/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace MonstersTheFramework
{
    public static class Extensions
    {
        public static bool HasEnchantment( this Tool tool, string type )
        {
            foreach (var ench in tool.enchantments )
            {
                if ( ench.GetName() == type )
                    return true;
            }
            return false;
        }
    }
}
