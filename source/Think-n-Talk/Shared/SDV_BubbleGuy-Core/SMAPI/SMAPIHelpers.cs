/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/Think-n-Talk
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace SDV_Speaker.SMAPIInt
{
    internal static class SMAPIHelpers
    {
        public static IModHelper helper;
 
        public static void Initialize(IModHelper helper)
        {
            SMAPIHelpers.helper = helper;
         }
    }
}
