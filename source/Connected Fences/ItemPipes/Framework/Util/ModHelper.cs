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

namespace ItemPipes.Framework.Util
{
    public static class ModHelper
    {
        private static IModContentHelper _helper;

        public static void SetHelper(IModContentHelper helper)
        {
            _helper = helper;
        }

        public static IModContentHelper GetHelper()
        {
            return _helper;
        }
    }
}
