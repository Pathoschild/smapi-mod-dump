/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sikker/SpouseStuff
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpouseStuff.Spouses
{
    class Utility
    {
        public static bool WithinRange(int target, int min, int max)
        {
            return target > min && target < max;
        }
    }
}
