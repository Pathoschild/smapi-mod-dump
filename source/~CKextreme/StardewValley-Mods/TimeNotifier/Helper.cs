/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CKextreme/StardewValley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeNotifier
{
    public static class Helper
    {
        public static short RoundUp(this short i)
        {
            return (short)(Math.Ceiling(i / 10.0) * 10);
        }

        // test if valid time 0600 to 2600
    }
}
