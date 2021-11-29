/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace ItemPipes.Framework
{
    public static class Helper
    {
        private static IModHelper _helper;

        public static void SetHelper(IModHelper helper)
        {
            _helper = helper;
        }

        public static IModHelper GetHelper()
        {
            return _helper;
        }
    }
}
